using Isango.Entities.AdyenPayment;
using Isango.Entities.Booking;

namespace ServiceAdapters.Adyen
{
    public interface IAdyenAdapter
    {
        AdyenPaymentResponse EnrollmentCheck(Booking booking, string token, int merchantType = 1);

        AdyenPaymentResponse CaptureCardTransaction(decimal amount, string currencyCode, string pspReference, string isangoReference, string token, int adyenMerchantType = 1);
        PaymentMethodsResponse PaymentMethods(string countryCode
            , string shopperLocale,string amount,string currency,string token);

        string PaymentMethodsV2(string countryCode
           , string shopperLocale, string amount, string currency, string token);

        AdyenPaymentResponse ThreeDSVerify(string transactionId, string pares, string paymentData, string token, string fallbackFingerPrint = "", int merchantType = 1);

        AdyenPaymentResponse CancelCardTransaction(string pspReference, string bookingRefNo, string token, int adyenMerchantType = 1);

        AdyenPaymentResponse RefundCardTransaction(decimal amount, string currencyCode, string reason, string transactionId, string isangoReference, string token, int adyenMerchantType = 1);

        GeneratePaymentIsangoResponse GeneratePaymentLinks(string countryCode
        ,string shopperLocale, string amount, string currency, string token);
    }
}