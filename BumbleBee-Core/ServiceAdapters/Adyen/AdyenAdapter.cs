using Isango.Entities;
using Isango.Entities.AdyenPayment;
using Isango.Entities.Booking;
using ServiceAdapters.Adyen.Adyen.Commands.Contracts;
using ServiceAdapters.Adyen.Adyen.Converters.Contracts;
using ServiceAdapters.Adyen.Adyen.Entities;
using Util;

namespace ServiceAdapters.Adyen
{
    public class AdyenAdapter : IAdyenAdapter, IAdapter
    {
        private readonly IEnrollmentCheckCommandHandler _enrollmentCheckCommandHandler;
        private readonly IEnrollmentCheckConverter _enrollmentCheckConverter;
        private readonly IPaymentMethodsCommandHandler _paymentMethodsCommandHandler;
        private readonly IPaymentMethodsConverter _paymentMethodsConverter;
        private readonly IThreeDSVerifyCommandHandler _threeDsVerifyCommandHandler;
        private readonly IThreeDsVerifyConverter _threeDsVerifyConverter;
        private readonly ICaptureCommandHandler _captureCommandHandler;
        private readonly ICaptureConverter _captureConverter;
        private readonly ICancelCommandHandler _cancelCommandHandler;
        private readonly ICancelConverter _cancelConverter;
        private readonly IRefundCommandHandler _refundCommandHandler;
        private readonly IRefundConverter _refundConverter;
        private readonly IGeneratePaymentConverter _generatePaymentConverter;
        private readonly IGeneratePaymentCommandHandler _generatePaymentCommandHandler;
        public AdyenAdapter(IEnrollmentCheckCommandHandler enrollmentCheckCommandHandler,
             IEnrollmentCheckConverter enrollmentCheckConverter,
             IPaymentMethodsCommandHandler paymentMethodsCommandHandler,
             IPaymentMethodsConverter paymentMethodsConverter,
             IThreeDSVerifyCommandHandler threeDsVerifyCommandHandler,
             IThreeDsVerifyConverter threeDsVerifyConverter,
             ICaptureCommandHandler captureCommandHandler,
             ICaptureConverter captureConverter,
             ICancelCommandHandler cancelCommandHandler,
             ICancelConverter cancelConverter,
             IRefundCommandHandler refundCommandHandler,
             IRefundConverter refundConverter,
             IGeneratePaymentCommandHandler generatePaymentCommandHandler,
             IGeneratePaymentConverter generatePaymentConverter
             )
        {
            _enrollmentCheckCommandHandler = enrollmentCheckCommandHandler;
            _enrollmentCheckConverter = enrollmentCheckConverter;
            _paymentMethodsCommandHandler = paymentMethodsCommandHandler;
            _paymentMethodsConverter = paymentMethodsConverter;
            _threeDsVerifyCommandHandler = threeDsVerifyCommandHandler;
            _threeDsVerifyConverter = threeDsVerifyConverter;
            _captureCommandHandler = captureCommandHandler;
            _captureConverter = captureConverter;
            _cancelCommandHandler = cancelCommandHandler;
            _cancelConverter = cancelConverter;
            _refundCommandHandler = refundCommandHandler;
            _refundConverter = refundConverter;
            _generatePaymentCommandHandler = generatePaymentCommandHandler;
            _generatePaymentConverter = generatePaymentConverter;
        }

        public AdyenPaymentResponse EnrollmentCheck(Booking booking, string token, int merchantType = 1)
        {
            var adyenCriteria = CreateEnrollmentCheckCriteria(booking);
            adyenCriteria.MethodType = MethodType.EnrollmentCheck;

            var response = _enrollmentCheckCommandHandler.Execute(adyenCriteria, token, merchantType);

            if (response != null)
            {
                var result = _enrollmentCheckConverter.Convert(response, adyenCriteria);
                return (AdyenPaymentResponse)result;
            }

            return null;
        }

        private AdyenCriteria CreateEnrollmentCheckCriteria(Booking booking)
        {
            var payment = booking?.Payment?.FirstOrDefault();
            var creditCard = (CreditCard)payment?.PaymentType;
            Int32.TryParse(booking?.BrowserInfo?.ScreenHeight, out int screenHeight);
            Int32.TryParse(booking?.BrowserInfo?.ScreenWidth, out int screenWidth);
            Int32.TryParse(booking?.BrowserInfo?.ColorDepth, out int colorDepth);
            Int32.TryParse(booking?.BrowserInfo?.TimeZoneOffset, out int timeZoneOffset);
            var adyenCriteria = new AdyenCriteria
            {
                Currency = booking?.Currency?.IsoCode?.ToUpperInvariant(),
                Amount = booking?.Currency?.IsoCode?.ToLower() == "jpy" ? Math.Ceiling(booking.Amount).ToString("0") : (booking.Amount * 100).ToString("0"), //this logic is confirmed with ADYEN Team
                CaptureNow = false,
                CustomerIp = booking?.IpAddress,
                CustomerEmail = booking?.VoucherEmailAddress,
                MerchantReference = booking?.ReferenceNumber,//Always value should be unique.

                CardNumber = creditCard?.CardNumber,
                ExpiryMonth = creditCard?.ExpiryMonth,
                ExpiryYear = creditCard?.ExpiryYear?.Length == 4 ? creditCard?.ExpiryYear.Substring(2, 2) : creditCard?.ExpiryYear,
                SecurityCode = creditCard?.SecurityCode,
                BaseUrl = booking?.Affiliate?.AffiliateBaseURL,
                Browserinfo = new Browserinfo()
                {
                    Language = booking?.BrowserInfo?.Language,
                    UserAgent = booking?.BrowserInfo?.UserAgent ?? booking?.BrowserInfo?.Language,
                    ScreenHeight = screenHeight,
                    ScreenWidth = screenWidth,
                    ColorDepth = colorDepth,
                    AcceptHeader = string.IsNullOrEmpty(booking?.BrowserInfo?.AcceptHeader) ? "*/*" : booking?.BrowserInfo?.AcceptHeader,
                    JavaEnabled = booking.BrowserInfo.JavaEnabled,
                    TimeZoneOffset = timeZoneOffset
                },
                UserCountry = booking?.User?.CountryCode,
                UserCity = booking?.User?.City,
                UserPostalCode = booking?.User?.ZipCode,
                UserStreet = booking?.User?.Address1,
                CardType = creditCard?.CardType,
                UserStateOrProvince = booking?.User?.State
            };

            return adyenCriteria;
        }

        private AdyenCriteria CreatePaymentMethodsCriteria(string countryCode,
             string shopperLocale, string amount, string currency)
        {
            var adyenCriteria = new AdyenCriteria
            {
                CountryCode = countryCode,
                ShopperLocale = shopperLocale,
                Amount = amount,
                Currency = currency
            };
            return adyenCriteria;
        }

        public PaymentMethodsResponse PaymentMethods(string countryCode
            , string shopperLocale, string amount, string currency, string token)
        {
            
            var adyenCriteria = CreatePaymentMethodsCriteria(countryCode
             , shopperLocale, amount, currency);
            adyenCriteria.MethodType = MethodType.PaymentMethods;

            var response = _paymentMethodsCommandHandler.Execute(adyenCriteria, null);

            if (!string.IsNullOrEmpty(response))
            {
                var result = _paymentMethodsConverter.Convert(response, adyenCriteria);
                return (PaymentMethodsResponse)result;
            }
            return null;
        }

        public string PaymentMethodsV2(string countryCode
            , string shopperLocale, string amount, string currency, string token)
        {
            //For production this value blank
            var testSofortCountry = ConfigurationManagerHelper.GetValuefromAppSettings("AdyenSofortCountryTesting");
            var testSofortLocale = ConfigurationManagerHelper.GetValuefromAppSettings("AdyenSofortLocaleTesting");
            var testSofortCurrency = ConfigurationManagerHelper.GetValuefromAppSettings("AdyenSofortCurrencyTesting");

            var testIDealCountry = ConfigurationManagerHelper.GetValuefromAppSettings("AdyenIDealCountryTesting");
            var testIDealLocale = ConfigurationManagerHelper.GetValuefromAppSettings("AdyenIDealLocaleTesting");
            var testIDealCurrency = ConfigurationManagerHelper.GetValuefromAppSettings("AdyenIDealCurrencyTesting");

            if (!string.IsNullOrEmpty(testSofortCountry))
            {
                countryCode = testSofortCountry;//for testing only environments
                shopperLocale = testSofortLocale;//for testing only environments
                currency = testSofortCurrency;//for testing only environments
            }
            if (!string.IsNullOrEmpty(testIDealCountry))
            {
                countryCode = testIDealCountry;//for testing only environments
                shopperLocale = testIDealLocale;//for testing only environments
                currency = testIDealCurrency;//for testing only environments
            }
            var adyenCriteria = CreatePaymentMethodsCriteria(countryCode
             , shopperLocale, amount, currency);
            adyenCriteria.MethodType = MethodType.PaymentMethods;

            var response = _paymentMethodsCommandHandler.Execute(adyenCriteria, null);

            if (!string.IsNullOrEmpty(response))
            {
                return response;
                //var result = _paymentMethodsConverter.Convert(response, adyenCriteria);
                //removed converter it create problem in binding and also useless
            }
            return null;
        }

        public AdyenPaymentResponse ThreeDSVerify(string transactionId, string pares, string paymentData, string token, string fallbackFingerPrint = "", int merchantType = 1)
        {
            var adyenCriteria = new AdyenCriteria();
            if (pares.Contains("fingerprint"))
            {
                adyenCriteria.fingerprint = pares.Replace("{\"threeds2.fingerprint\":\"", "").Replace("\"}", "");
                adyenCriteria.PaymentData = paymentData;
                adyenCriteria.MethodType = MethodType.ThreeDSVerify;
            }
            else if (pares.Contains("challengeResult"))
            {
                adyenCriteria.Challenge = pares.Replace("{\"threeds2.challengeResult\":\"", "").Replace("\"}", "");
                adyenCriteria.PaymentData = paymentData;
                adyenCriteria.MethodType = MethodType.ThreeDSVerify;
                adyenCriteria.fingerprint = fallbackFingerPrint;
            }
            else if (pares.Contains("facilitatorAccessToken"))
            {
                adyenCriteria.FacilitatorAccessToken = pares;
                adyenCriteria.PaymentData = paymentData;
                adyenCriteria.MethodType = MethodType.ThreeDSVerify;
            }
            else
            {
                adyenCriteria.TransactionId = transactionId;
                adyenCriteria.MD = transactionId;
                adyenCriteria.Pares = pares;
                adyenCriteria.PaymentData = paymentData;
                adyenCriteria.fingerprint = fallbackFingerPrint;
                adyenCriteria.MethodType = MethodType.ThreeDSVerify;
            }

            var response = _threeDsVerifyCommandHandler.Execute(adyenCriteria, token, merchantType);

            if (response != null)
            {
                var result = _threeDsVerifyConverter.Convert(response, adyenCriteria);
                return (AdyenPaymentResponse)result;
            }

            return null;
        }

        public AdyenPaymentResponse CaptureCardTransaction(decimal amount, string currencyCode, string pspReference, string isangoReference, string token, int merchantType = 1)
        {
            var adyenCriteria = new AdyenCriteria()
            {
                Amount = currencyCode.ToLower() == "jpy" ? Math.Ceiling(amount).ToString("0") : (amount * 100).ToString("0"),
                PspReference = pspReference,
                MerchantReference = isangoReference,
                Currency = currencyCode,
                MethodType = MethodType.CaptureCard
            };

            var response = _captureCommandHandler.Execute(adyenCriteria, token, merchantType);

            if (response != null)
            {
                var result = _captureConverter.Convert(response, adyenCriteria);
                return (AdyenPaymentResponse)result;
            }

            return null;
        }

        public AdyenPaymentResponse CancelCardTransaction(string pspReference, string bookingRefNo, string token, int adyenMerchantType = 1)
        {
            var adyenCriteria = new AdyenCriteria()
            {
                PspReference = pspReference,
                MerchantReference = bookingRefNo,
                MethodType = MethodType.CancelCard
            };

            var response = _cancelCommandHandler.Execute(adyenCriteria, token, adyenMerchantType);

            if (response != null)
            {
                var result = _cancelConverter.Convert(response, adyenCriteria);
                return (AdyenPaymentResponse)result;
            }

            return null;
        }

        public AdyenPaymentResponse RefundCardTransaction(decimal amount, string currencyCode, string reason, string transactionId, string isangoReference, string token, int adyenMerchantType = 1)
        {
            var adyenCriteria = new AdyenCriteria()
            {
                Amount = currencyCode.ToLower() == "jpy" ? Math.Ceiling(amount).ToString("0") : (amount * 100).ToString("0"),
                Currency = currencyCode,
                PspReference = transactionId,
                MerchantReference = isangoReference,
                MethodType = MethodType.RefundCard
            };

            var response = _refundCommandHandler.Execute(adyenCriteria, token, adyenMerchantType);

            if (response != null)
            {
                var result = _refundConverter.Convert(response, adyenCriteria);
                return (AdyenPaymentResponse)result;
            }

            return null;
        }

        public GeneratePaymentIsangoResponse GeneratePaymentLinks(string countryCode
         , string shopperLocale, string amount, string currency, string token)
        {
            var adyenCriteria = CreatePaymentMethodsCriteria(countryCode
             , shopperLocale, amount, currency);
            adyenCriteria.MethodType = MethodType.PaymentLinks;

            var response = _generatePaymentCommandHandler.Execute(adyenCriteria, null);

            if (response != null)
            {
                var result = _generatePaymentConverter.Convert(response, adyenCriteria);
                return (GeneratePaymentIsangoResponse)result;
            }

            return null;
        }

    }
}