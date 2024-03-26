using Isango.Entities;
using Isango.Entities.ApexxPayment;
using Isango.Entities.Booking;
using Isango.Entities.Payment;
using ServiceAdapters.Apexx.Apexx.Commands.Contracts;
using ServiceAdapters.Apexx.Apexx.Converters.Contracts;
using ServiceAdapters.Apexx.Apexx.Entities;
using ServiceAdapters.Apexx.Constants;
using System;
using System.Linq;
using Util;

namespace ServiceAdapters.Apexx
{
    public class ApexxAdapter : IApexxAdapter, IAdapter
    {
        private readonly IEnrollmentCheckCommandHandler _enrollmentCheckCommandHandler;
        private readonly IThreeDSVerifyCommandHandler _threeDsVerifyCommandHandler;
        private readonly ICaptureCommandHandler _captureCommandHandler;
        private readonly IRefundCommandHandler _refundCommandHandler;
        private readonly ICancelCommandHandler _cancelCommandHandler;
        private readonly ICancelCaptureCommandHandler _cancelCaptureCommandHandler;
        private readonly ICreateCardTransactionCommandHandler _createCardTransactionCommandHandler;

        private readonly IEnrollmentCheckConverter _enrollmentCheckConverter;
        private readonly IThreeDsVerifyConverter _threeDsVerifyConverter;
        private readonly ICaptureConverter _captureConverter;
        private readonly IRefundConverter _refundConverter;
        private readonly ICancelConverter _cancelConverter;
        private readonly ICancelCaptureConverter _cancelCaptureConverter;
        private readonly ICreateCardTransactionConverter _createCardTransactionConverter;

        private static bool _isRiskified;
        private static bool _isRiskifiedWith3D;
        private static bool _isRiskifiedTestingPhase;

        public ApexxAdapter(
            IEnrollmentCheckCommandHandler enrollmentCheckCommandHandler,
            IThreeDSVerifyCommandHandler threeDsVerifyCommandHandler,
            ICaptureCommandHandler captureCommandHandler,
            IRefundCommandHandler refundCommandHandler,
            ICancelCommandHandler cancelCommandHandler,
            ICreateCardTransactionCommandHandler createCardTransactionCommandHandler,

            IEnrollmentCheckConverter enrollmentCheckConverter,
            IThreeDsVerifyConverter threeDsVerifyConverter,
            ICaptureConverter captureConverter,
            IRefundConverter refundConverter,
            ICancelConverter cancelConverter,
            ICancelCaptureCommandHandler cancelCaptureCommandHandler,
            ICancelCaptureConverter cancelCaptureConverter,
            ICreateCardTransactionConverter createCardTransactionConverter
            )
        {
            _enrollmentCheckCommandHandler = enrollmentCheckCommandHandler;
            _threeDsVerifyCommandHandler = threeDsVerifyCommandHandler;
            _captureCommandHandler = captureCommandHandler;
            _refundCommandHandler = refundCommandHandler;
            _cancelCommandHandler = cancelCommandHandler;
            _createCardTransactionCommandHandler = createCardTransactionCommandHandler;

            _enrollmentCheckConverter = enrollmentCheckConverter;
            _threeDsVerifyConverter = threeDsVerifyConverter;
            _captureConverter = captureConverter;
            _refundConverter = refundConverter;
            _cancelConverter = cancelConverter;
            _cancelCaptureCommandHandler = cancelCaptureCommandHandler;
            _cancelCaptureConverter = cancelCaptureConverter;
            _createCardTransactionConverter = createCardTransactionConverter;
            _isRiskified = Convert.ToBoolean(ConfigurationManagerHelper.GetValuefromAppSettings("IsRiskifiedEnabled"));
            _isRiskifiedWith3D = Convert.ToBoolean(ConfigurationManagerHelper.GetValuefromAppSettings("IsRiskifiedEnabledWith3D"));
            var riskifiedTestingPhase = ConfigurationManagerHelper.GetValuefromAppSettings("IsRiskifiedTestingPhase");
            _isRiskifiedTestingPhase = !string.IsNullOrEmpty(riskifiedTestingPhase) && Convert.ToBoolean(riskifiedTestingPhase);
        }

        public ApexxPaymentResponse EnrollmentCheck(Booking booking, string token)
        {
            var apexxCriteria = CreateEnrollmentCheckCriteria(booking);
            apexxCriteria.MethodType = MethodType.EnrollmentCheck;

            var response = _enrollmentCheckCommandHandler.Execute(apexxCriteria, token);

            if (response != null)
            {
                var result = _enrollmentCheckConverter.Convert(response, apexxCriteria);
                var apexxPaymentResponse = (ApexxPaymentResponse)result;

                if (!string.IsNullOrWhiteSpace(apexxPaymentResponse?.ErrorMessage) || apexxPaymentResponse == null)
                {
                    booking?.UpdateErrors(CommonErrorCodes.PaymentGatewayError
                                , System.Net.HttpStatusCode.BadRequest
                                , response?.ToString());
                }

                return apexxPaymentResponse;
            }
            else
            {
                booking?.UpdateErrors(CommonErrorCodes.PaymentGatewayError
                            , System.Net.HttpStatusCode.BadRequest
                            , "EnrollmentCheck response was null.");
            }

            return null;
        }

        public ApexxPaymentResponse ThreeDSVerify(string transactionId, string pares, string token)
        {
            var apexxCriteria = new ApexxCriteria()
            {
                TransactionId = transactionId,
                Pares = pares,
                MethodType = MethodType.ThreeDSVerify
            };

            var response = _threeDsVerifyCommandHandler.Execute(apexxCriteria, token);

            if (response != null)
            {
                var result = _threeDsVerifyConverter.Convert(response, apexxCriteria);
                return (ApexxPaymentResponse)result;
            }

            return null;
        }

        public ApexxPaymentResponse CaptureCardTransaction(decimal amount, string captureReference, string transactionId, string token)
        {
            var apexxCriteria = new ApexxCriteria()
            {
                Amount = (amount * 100).ToString("0"),
                CaptureRefernce = captureReference,
                TransactionId = transactionId,
                MethodType = MethodType.CaptureCard
            };

            var response = _captureCommandHandler.Execute(apexxCriteria, token);

            if (response != null)
            {
                var result = _captureConverter.Convert(response, apexxCriteria);
                return (ApexxPaymentResponse)result;
            }

            return null;
        }

        public ApexxPaymentResponse RefundCardTransaction(decimal amount, string reason, string transactionId, string token, string captureGuid)
        {
            var apexxCriteria = new ApexxCriteria()
            {
                Amount = (amount * 100).ToString("0"),
                Reason = reason,
                TransactionId = transactionId,
                MethodType = MethodType.RefundCard,
                CaptureGuid = captureGuid
            };

            var response = _refundCommandHandler.Execute(apexxCriteria, token);

            if (response != null)
            {
                var result = _refundConverter.Convert(response, apexxCriteria);
                return (ApexxPaymentResponse)result;
            }

            return null;
        }

        public ApexxPaymentResponse CancelCardTransaction(string transactionId, string token)
        {
            var apexxCriteria = new ApexxCriteria()
            {
                TransactionId = transactionId,
                MethodType = MethodType.CancelCard
            };

            var response = _cancelCommandHandler.Execute(apexxCriteria, token);

            if (response != null)
            {
                var result = _cancelConverter.Convert(response, apexxCriteria);
                return (ApexxPaymentResponse)result;
            }

            return null;
        }

        private ApexxCriteria CreateEnrollmentCheckCriteria(Booking booking)
        {
            var payment = booking.Payment.FirstOrDefault();
            var creditCard = (CreditCard)payment?.PaymentType;

            bool isThreeDs = Convert.ToBoolean(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.ThreeDSRequired));

            if (_isRiskified && booking.isRiskifiedEnabled && !_isRiskifiedWith3D && !_isRiskifiedTestingPhase)
            {
                isThreeDs = false;
            }
            var billingAddress = new BillingAddress
            {
                //Country = booking?.User?.Country
                Country = Constant.ApexxCountry
            };
            var apexxCriteria = new ApexxCriteria
            {
                //Account = Constant.Account,  //Instead of Account we will use Organisation and Currency
                Organisation = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.Organisation),
                Currency = booking?.Currency?.IsoCode?.ToUpperInvariant(),
                Amount = (booking.Amount * 100).ToString("0"), //this logic is confirmed with APEXX Team
                CaptureNow = false,
                CustomerIp = booking.IpAddress,
                //DynamicDescriptor = booking?.Affiliate?.Name,
                DynamicDescriptor = Constant.DynamicDescriptor,
                MerchantReference = booking.ReferenceNumber + "_" + UniqueTransactionIdGenerator.GenerateTransactionId(),  //Always value should be unique.
                UserAgent = Constant.DefaultUserAgent, // Hard coded value after getting confirming from Prashant.
                CardNumber = creditCard?.CardNumber,
                ExpiryMonth = creditCard?.ExpiryMonth,
                ExpiryYear = creditCard?.ExpiryYear.Length == 4 ? creditCard?.ExpiryYear.Substring(2, 2) : creditCard?.ExpiryYear,
                SecurityCode = creditCard?.SecurityCode,
                BaseUrl = booking.Affiliate.AffiliateBaseURL,
                ThreeDsRequired = isThreeDs.ToString(),
                BillingAddress = billingAddress
            };

            if (booking.SelectedProducts?.Count > 0)
            {
                //check booking contain Product other than confirm product
                apexxCriteria.IsOnRequestProduct = booking.SelectedProducts.Any(x => x.ProductOptions.Any(y => y.AvailabilityStatus != Isango.Entities.Enums.AvailabilityStatus.AVAILABLE));
            }

            return apexxCriteria;
        }

        public ApexxPaymentResponse CancelCaptureCardTransaction(string transactionId, string captureReference, string token)
        {
            var apexxCriteria = new ApexxCriteria()
            {
                TransactionId = transactionId,
                MethodType = MethodType.CancelCaptureCard,
                CaptureRefernce = captureReference
            };

            var response = _cancelCaptureCommandHandler.Execute(apexxCriteria, token);

            if (response != null)
            {
                var result = _cancelCaptureConverter.Convert(response, apexxCriteria);
                return (ApexxPaymentResponse)result;
            }

            return null;
        }

        public ApexxPaymentResponse CreateCardTransaction(Booking booking, string token)
        {
            var apexxCriteria = CreateCardTransactionCriteria(booking);
            apexxCriteria.MethodType = MethodType.CreateCardTransaction;

            var response = _createCardTransactionCommandHandler.Execute(apexxCriteria, token);

            if (response != null)
            {
                var result = _createCardTransactionConverter.Convert(response, apexxCriteria);
                return (ApexxPaymentResponse)result;
            }

            return null;
        }

        private ApexxCriteria CreateCardTransactionCriteria(Booking booking)
        {
            var payment = booking.Payment?.Where(x => x.PaymentStatus == PaymentStatus.PreAuthorized)?.FirstOrDefault();
            var apexxCriteria = new ApexxCriteria
            {
                Organisation = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.Organisation),
                Currency = booking?.Currency?.IsoCode?.ToUpperInvariant(),
                Amount = (payment.ChargeAmount * 100).ToString("0"),
                CaptureNow = false,
                MerchantReference = booking.ReferenceNumber + "_" + UniqueTransactionIdGenerator.GenerateTransactionId(),
                ApexxToken = payment.Token
            };
            return apexxCriteria;
        }
    }
}