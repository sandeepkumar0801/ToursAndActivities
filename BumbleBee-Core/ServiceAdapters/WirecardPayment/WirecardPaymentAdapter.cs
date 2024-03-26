using Isango.Entities;
using Isango.Entities.Booking;
using Isango.Entities.WirecardPayment;
using ServiceAdapters.WirecardPayment.WirecardPayment.Commands.Contracts;
using ServiceAdapters.WirecardPayment.WirecardPayment.Converters.Contracts;
using ServiceAdapters.WirecardPayment.WirecardPayment.Entities;
using System;
using System.Configuration;
using System.Linq;
using Util;

namespace ServiceAdapters.WirecardPayment
{
    public class WirecardPaymentAdapter : IWirecardPaymentAdapter, IAdapter
    {
        #region "Private Members"

        private readonly IBookBackCommandHandler _bookBackCommandHandler;
        private readonly IBookBackConverter _bookBackConverter;
        private readonly ICapturePreauthorizeCommandHandler _capturePreauthorizeCommandHandler;
        private readonly ICapturePreauthorizeConverter _capturePreauthorizeConverter;
        private readonly IProcessPaymentCommandHandler _processPaymentCommandHandler;
        private readonly IProcessPaymentConverter _processPaymentConverter;
        private readonly IProcessPayment3DCommandHandler _processPayment3DCommandHandler;
        private readonly IProcessPayment3DConverter _processPayment3DConverter;
        private readonly IRollBackCommandHandler _rollBackCommandHandler;
        private readonly IRollBackConverter _rollBackConverter;
        private readonly IEmiEnrollmentCheckCommandHandler _emiEnrollmentCheckCommandHandler;
        private readonly IEmiEnrollmentCheckConverter _emiEnrollmentCheckConverter;
        private readonly ICapturePreauthorize3DCommandHandler _capturePreauthorize3DCommandHandler;
        private readonly ICapturePreauthorize3DConverter _capturePreauthorize3DConverter;
        private readonly IEnrollmentCheckCommandHandler _enrollmentCheckCommandHandler;
        private readonly IEnrollmentCheckConverter _enrollmentCheckConverter;

        #endregion "Private Members"

        #region "Constructor"

        public WirecardPaymentAdapter(IBookBackCommandHandler bookBackCommandHandler, IBookBackConverter bookBackConverter, ICapturePreauthorizeCommandHandler capturePreauthorizeCommandHandler, ICapturePreauthorizeConverter capturePreauthorizeConverter, IProcessPaymentCommandHandler processPaymentCommandHandler, IProcessPaymentConverter processPaymentConverter, IProcessPayment3DCommandHandler processPayment3DCommandHandler, IProcessPayment3DConverter processPayment3DConverter, IRollBackCommandHandler rollBackCommandHandler, IRollBackConverter rollBackConverter, IEmiEnrollmentCheckCommandHandler emiEnrollmentCheckCommandHandler, ICapturePreauthorize3DCommandHandler capturePreauthorize3DCommandHandler, ICapturePreauthorize3DConverter capturePreauthorize3DConverter, IEmiEnrollmentCheckConverter emiEnrollmentCheckConverter, IEnrollmentCheckCommandHandler enrollmentCheckCommandHandler, IEnrollmentCheckConverter enrollmentCheckConverter)
        {
            _bookBackCommandHandler = bookBackCommandHandler;
            _bookBackConverter = bookBackConverter;
            _capturePreauthorizeCommandHandler = capturePreauthorizeCommandHandler;
            _capturePreauthorizeConverter = capturePreauthorizeConverter;
            _processPaymentCommandHandler = processPaymentCommandHandler;
            _processPaymentConverter = processPaymentConverter;
            _processPayment3DCommandHandler = processPayment3DCommandHandler;
            _processPayment3DConverter = processPayment3DConverter;
            _rollBackCommandHandler = rollBackCommandHandler;
            _rollBackConverter = rollBackConverter;
            _emiEnrollmentCheckCommandHandler = emiEnrollmentCheckCommandHandler;
            _capturePreauthorize3DCommandHandler = capturePreauthorize3DCommandHandler;
            _capturePreauthorize3DConverter = capturePreauthorize3DConverter;
            _emiEnrollmentCheckConverter = emiEnrollmentCheckConverter;
            _enrollmentCheckCommandHandler = enrollmentCheckCommandHandler;
            _enrollmentCheckConverter = enrollmentCheckConverter;
        }

        #endregion "Constructor"

        /// <summary>
        /// This method is called for refunding the amount.
        /// </summary>
        /// <param name="payment"></param>
        /// <param name="token"></param>
        public Tuple<WirecardPaymentResponse, string, string> BookBack(Isango.Entities.Payment.Payment payment, string token)
        {
            var paymentCardCriteria = new PaymentCardCriteria
            {
                JobId = payment.JobId,
                BusinessCaseSignature_Wirecard = SetBusinessCaseSign(payment.Is3D),
                TransactionId = payment.TransactionId,
                Guwid = payment.Guwid,
                ChargeAmount = payment.ChargeAmount,
                CurrencyCode = payment.CurrencyCode,
                MethodType = MethodType.BookBack
            };

            var responseXmlData = _bookBackCommandHandler.Execute(paymentCardCriteria, payment.Is3D, token);
            var responseData = _bookBackConverter.Convert(responseXmlData.Item2, null);
            var response = Tuple.Create(responseData, responseXmlData.Item1.Replace("'", "\\\""), responseXmlData.Item2.Replace("'", "\\\""));
            return response;
        }

        /// <summary>
        /// This method is called in case of purchase or capture preauthorize.
        /// </summary>
        /// <param name="payment"></param>
        /// <param name="token"></param>
        public Tuple<WirecardPaymentResponse, string, string> Charge(Isango.Entities.Payment.Payment payment, string token)
        {
            return !string.IsNullOrEmpty(payment.PaymentGatewayReferenceId) ? CapturePreauthorize(payment, token) : ProcessPayment(payment, "Purchase", token);
        }

        /// <summary>
        /// This method is called in case of purchase or capture preauthorize for 3D card only.
        /// </summary>
        /// <param name="payment"></param>
        /// <param name="booking"></param>
        /// <param name="token"></param>
        public Tuple<WirecardPaymentResponse, string, string> Charge3D(Isango.Entities.Payment.Payment payment, Booking booking, string token)
        {
            return !string.IsNullOrEmpty(payment.PaymentGatewayReferenceId) ? CapturePreauthorize(payment, token) : ProcessPayment3D(payment, booking, "Purchase", token);
        }

        /// <summary>
        /// Enrollment check process only for mastro cards(3D).
        /// If card is enrolled then we create a http request with wirecard PARes
        /// </summary>
        /// <param name="booking"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Tuple<WirecardPaymentResponse, string, string> EnrollmentCheck(Booking booking, string token)
        {
            var payment = booking.Payment.FirstOrDefault();
            var creditCard = (CreditCard)payment?.PaymentType;
            var paymentCardCriteria = GetPaymentCardForEnrollmentCheck(booking, null, creditCard, MethodType.EnrollmentCheck);
            var responseXmlData = _enrollmentCheckCommandHandler.Execute(paymentCardCriteria, true, token);
            var responseData = _enrollmentCheckConverter.Convert(responseXmlData.Item2, paymentCardCriteria);
            var response = Tuple.Create(responseData, responseXmlData.Item1.Replace("'", "\\\""), responseXmlData.Item2.Replace("'", "\\\""));
            return response;
        }

        public Tuple<WirecardPaymentResponse, string, string> EmiEnrollmentCheck(Booking booking, Installment installment, string token)
        {
            var creditCard = installment.Card;
            var paymentCardCriteria = GetPaymentCardForEnrollmentCheck(booking, installment, creditCard, MethodType.EmiEnrollmentCheck);
            var responseXmlData = _emiEnrollmentCheckCommandHandler.Execute(paymentCardCriteria, true, token);
            var responseData = _emiEnrollmentCheckConverter.Convert(responseXmlData.Item2, paymentCardCriteria);
            var response = Tuple.Create(responseData, responseXmlData.Item1.Replace("'", "\\\""), responseXmlData.Item2.Replace("'", "\\\""));
            return response;
        }

        /// <summary>
        /// This method is called for pre authorization.
        /// </summary>
        /// <param name="payment"></param>
        /// <param name="token"></param>
        public Tuple<WirecardPaymentResponse, string, string> PreAuthorize(Isango.Entities.Payment.Payment payment, string token)
        {
            return ProcessPayment(payment, "PreAuthorize", token);
        }

        /// <summary>
        ///  This method is called for pre authorization for 3D cards only.
        /// </summary>
        /// <param name="payment"></param>
        /// <param name="booking"></param>
        /// <param name="token"></param>
        public Tuple<WirecardPaymentResponse, string, string> PreAuthorize3D(Isango.Entities.Payment.Payment payment, Booking booking, string token)
        {
            return ProcessPayment3D(payment, booking, "PreAuthorize", token);
        }

        public Tuple<WirecardPaymentResponse, string, string> PreAuthPurchase3D(Isango.Entities.Payment.Payment preAuthPayment, Isango.Entities.Payment.Payment purchasePayment, Booking booking, string token)
        {
            return ProcessAndCapturePayment(preAuthPayment, purchasePayment, booking, token);
        }

        /// <summary>
        /// This method is called for reversal.
        /// </summary>
        /// <param name="payment"></param>
        /// <param name="token"></param>
        public Tuple<WirecardPaymentResponse, string, string> Rollback(Isango.Entities.Payment.Payment payment, string token)
        {
            var paymentCardCriteria = new PaymentCardCriteria
            {
                JobId = payment.JobId,
                BusinessCaseSignature_Wirecard = SetBusinessCaseSign(payment.Is3D),
                TransactionId = payment.TransactionId,
                PaymentGatewayReferenceId = payment.PaymentGatewayReferenceId,
                MethodType = MethodType.Rollback
            };

            var responseXmlData = _rollBackCommandHandler.Execute(paymentCardCriteria, payment.Is3D, token);
            var responseData = _rollBackConverter.Convert(responseXmlData.Item2, null);
            var response = Tuple.Create(responseData, responseXmlData.Item1.Replace("'", "\\\""),
                responseXmlData.Item2.Replace("'", "\\\""));
            return response;
        }

        /// <summary>
        /// This method is called for Capture Pre authorization.
        /// </summary>
        /// <param name="payment"></param>
        /// <param name="token"></param>
        public Tuple<WirecardPaymentResponse, string, string> CapturePreauthorize(Isango.Entities.Payment.Payment payment, string token)
        {
            var paymentCardCriteria = new PaymentCardCriteria
            {
                JobId = payment.JobId,
                BusinessCaseSignature_Wirecard = SetBusinessCaseSign(false),
                TransactionId = payment.TransactionId,
                Guwid = payment.Guwid,
                PaymentGatewayReferenceId = payment.PaymentGatewayReferenceId,
                ChargeAmount = payment.ChargeAmount,
                MethodType = MethodType.CapturePreauthorize
            };
            var responseXmlData = _capturePreauthorizeCommandHandler.Execute(paymentCardCriteria, payment.Is3D, token);
            var responseData = _capturePreauthorizeConverter.Convert(responseXmlData.Item2, paymentCardCriteria);
            var response = Tuple.Create(responseData, responseXmlData.Item1.Replace("'", "\\\""),
                responseXmlData.Item2.Replace("'", "\\\""));
            return response;
        }

        #region Private Methods

        /// <summary>
        /// This method is called for Purchase.
        /// </summary>
        /// <param name="payment"></param>
        /// <param name="operationFlag"></param>
        /// <param name="token"></param>
        private Tuple<WirecardPaymentResponse, string, string> ProcessPayment(Isango.Entities.Payment.Payment payment, string operationFlag, string token)
        {
            var paymentCardCriteria = GetPaymentCardCriteria(payment, null, operationFlag, MethodType.ProcessPayment);
            var responseXmlData = _processPaymentCommandHandler.Execute(paymentCardCriteria, false, token);
            var responseData = _processPaymentConverter.Convert(responseXmlData.Item2, paymentCardCriteria);
            var response = Tuple.Create(responseData, responseData.RequestXml.Replace("'", "\\\""),
                responseXmlData.Item2.Replace("'", "\\\""));
            return response;
        }

        /// <summary>
        /// This method is called for Purchase for 3D cards only.
        /// </summary>
        /// <param name="payment"></param>
        /// <param name="booking"></param>
        /// <param name="operationFlag"></param>
        /// <param name="token"></param>
        public Tuple<WirecardPaymentResponse, string, string> ProcessPayment3D(Isango.Entities.Payment.Payment payment, Booking booking, string operationFlag, string token)
        {
            var paymentCardCriteria = GetPaymentCardCriteria(payment, booking, operationFlag, MethodType.ProcessPayment3D);
            var responseXmlData = _processPayment3DCommandHandler.Execute(paymentCardCriteria, true, token);
            var responseData = _processPayment3DConverter.Convert(responseXmlData.Item2, paymentCardCriteria);
            var response = Tuple.Create(responseData, responseData.RequestXml.Replace("'", "\\\""),
                responseXmlData.Item2.Replace("'", "\\\""));
            return response;
        }

        private Tuple<WirecardPaymentResponse, string, string> ProcessAndCapturePayment(Isango.Entities.Payment.Payment preAuthPayment, Isango.Entities.Payment.Payment purchasePayment, Booking booking, string token)
        {
            var response = ProcessPayment3D(preAuthPayment, booking, "PreAuthorize", token);
            response.Item1.RequestType = "PreAuthFull";
            return response.Item1.Status == "Success" ? CapturePreauthorize3D(preAuthPayment, purchasePayment, token) : response;
        }

        /// <summary>
        /// This method is called for Capture Pre authorization for 3D cards only.
        /// </summary>
        /// <param name="preAuthPayment"></param>
        /// <param name="purchasePayment"></param>
        /// <param name="token"></param>
        public Tuple<WirecardPaymentResponse, string, string> CapturePreauthorize3D(Isango.Entities.Payment.Payment preAuthPayment, Isango.Entities.Payment.Payment purchasePayment, string token)
        {
            var paymentCardCriteria = new PaymentCardCriteria
            {
                JobId = preAuthPayment.JobId,
                BusinessCaseSignature_Wirecard = SetBusinessCaseSign(preAuthPayment.Is3D),
                TransactionId = purchasePayment.TransactionId,
                PaymentGatewayReferenceId = preAuthPayment.PaymentGatewayReferenceId,
                ChargeAmount = purchasePayment.ChargeAmount,
                AuthorizationCode = preAuthPayment.AuthorizationCode,
                CurrencyCode = preAuthPayment.CurrencyCode,
                MethodType = MethodType.CapturePreauthorize3D
            };
            var responseXmlData = _capturePreauthorize3DCommandHandler.Execute(paymentCardCriteria, true, token);
            var responseData = _capturePreauthorize3DConverter.Convert(responseXmlData.Item2, paymentCardCriteria);
            var response = Tuple.Create(responseData, responseData.RequestXml.Replace("'", "\\\""), responseXmlData.Item2.Replace("'", "\\\""));
            return response;
        }

        private string SetBusinessCaseSign(bool is3D)
        {
            var isTestMode = Convert.ToBoolean(ConfigurationManager.AppSettings["IsWireCardInTestMode"]);
            if (!isTestMode)
            {
                return is3D ? ConfigurationManagerHelper.GetValuefromAppSettings("LiveBusinessCaseSignature_3DWirecard") : ConfigurationManagerHelper.GetValuefromAppSettings("LiveBusinessCaseSignature_Wirecard");
            }
            return is3D ? ConfigurationManagerHelper.GetValuefromAppSettings("TestBusinessCaseSignature_3DWirecard") : ConfigurationManagerHelper.GetValuefromAppSettings("TestBusinessCaseSignature_Wirecard");
        }

        private void SplitName(string fullName, PaymentCardCriteria paymentCardCriteria)
        {
            var strArrCardHolderName = fullName.Split(new[] { ' ' }, 2);
            if (strArrCardHolderName.Length >= 2)
            {
                paymentCardCriteria.FirstName = strArrCardHolderName[0];
                paymentCardCriteria.LastName = strArrCardHolderName[1];
            }
            else if (strArrCardHolderName.Length == 1)
                paymentCardCriteria.FirstName = strArrCardHolderName[0];
        }

        private PaymentCardCriteria GetPaymentCardCriteria(Isango.Entities.Payment.Payment payment, Booking booking, string operationFlag, MethodType methodType)
        {
            var creditCard = (CreditCard)payment.PaymentType;
            var paymentCardCriteria = new PaymentCardCriteria
            {
                JobId = payment.JobId,
                BusinessCaseSignature_Wirecard = SetBusinessCaseSign(payment.Is3D),
                TransactionId = payment.TransactionId,
                PaymentGatewayReferenceId = payment.PaymentGatewayReferenceId,
                SecurityCode = creditCard.SecurityCode,
                ChargeAmount = payment.ChargeAmount,
                CurrencyCode = payment.CurrencyCode,
                CardHoldersCountryName = creditCard.CardHoldersCountryName,
                CardHoldersAddress1 = creditCard.CardHoldersAddress1,
                CardHoldersCity = creditCard.CardHoldersCity,
                CardHoldersZipCode = creditCard.CardHoldersZipCode,
                CardHoldersState = creditCard.CardHoldersState,
                CardHoldersEmail = creditCard.CardHoldersEmail,
                ExpiryYear = creditCard.ExpiryYear,
                ExpiryMonth = creditCard.ExpiryMonth,
                CardHoldersName = creditCard.CardHoldersName,
                IpAddress = payment.IpAddress,
                CardNumber = creditCard.CardNumber,
                MethodType = methodType
            };
            SplitName(creditCard.CardHoldersName, paymentCardCriteria);
            if (booking != null)
            {
                paymentCardCriteria.Guwid = booking.Guwid;
                paymentCardCriteria.PaRes = booking.PaRes;
            }
            if (operationFlag.Equals("Purchase"))
            {
                paymentCardCriteria.TagText = "FNC_CC_PURCHASE";
                paymentCardCriteria.TagValue = "Purchase 1";
                paymentCardCriteria.LogText = "Purchase";
            }
            else if (operationFlag.Equals("PreAuthorize"))
            {
                paymentCardCriteria.TagText = "FNC_CC_PREAUTHORIZATION";
                paymentCardCriteria.TagValue = "Preauthorization 1";
                paymentCardCriteria.LogText = "Preauthorize";
            }
            return paymentCardCriteria;
        }

        private PaymentCardCriteria GetPaymentCardForEnrollmentCheck(Booking booking, Installment installment, CreditCard creditCard, MethodType methodType)
        {
            var paymentCardCriteria = new PaymentCardCriteria
            {
                BusinessCaseSignature_Wirecard = SetBusinessCaseSign(true),
                TagValue = string.Empty,
                CardHoldersCountryName = creditCard.CardHoldersCountryName,
                CardNumber = creditCard.CardNumber,
                SecurityCode = creditCard.SecurityCode,
                ExpiryYear = creditCard.ExpiryYear,
                ExpiryMonth = creditCard.ExpiryMonth,

                CardHoldersName = creditCard.CardHoldersName,
                IpAddress = booking.IpAddress,
                AcceptHeader = booking.BookingUserAgent.AcceptHeader,
                UserAgent = booking.BookingUserAgent.UserAgent,
                DeviceCategory = booking.BookingUserAgent.DeviceCategory,
                CardHoldersAddress1 = creditCard.CardHoldersAddress1,
                CardHoldersCity = creditCard.CardHoldersCity,
                CardHoldersZipCode = creditCard.CardHoldersZipCode,
                CardHoldersState = creditCard.CardHoldersState,
                CardHoldersEmail = creditCard.CardHoldersEmail,
                CurrencyCode = booking.Currency.IsoCode.ToUpperInvariant(),
                BaseUrl = booking.Affiliate.AffiliateBaseURL,
                MethodType = methodType
            };
            SplitName(creditCard.CardHoldersName, paymentCardCriteria);
            if (installment != null)
            {
                paymentCardCriteria.UserId = installment.UserId;
                paymentCardCriteria.BookingRefNo = installment.BookingRefNo;
                paymentCardCriteria.InstallmentAmount = (installment.Amount * 100).ToString("0");
                //paymentCardCriteria.CurrencyCode = installment.CurrencyCode;
            }
            else
            {
                paymentCardCriteria.InstallmentAmount = (booking.Amount * 100).ToString("0");
            }
            return paymentCardCriteria;
        }

        #endregion Private Methods
    }
}