using Isango.Entities.AdyenPayment;
using ServiceAdapters.Adyen.Adyen.Converters.Contracts;
using ServiceAdapters.Adyen.Adyen.Entities;
using Util;

namespace ServiceAdapters.Adyen.Adyen.Converters
{
    public class ThreeDsVerifyConverter : ConverterBase, IThreeDsVerifyConverter
    {
        public override object Convert(string response, object inputObject)
        {
            var threeDsVerifyResponse = SerializeDeSerializeHelper.DeSerialize<ThreeDVerifyResponse>(response);

            var adyenPaymentResponse = new AdyenPaymentResponse
            {
                RequestJson = SerializeDeSerializeHelper.Serialize(inputObject),
                ResponseJson = response,
                FallbackFingerPrint = SerializeDeSerializeHelper.Serialize(inputObject),
                RefusalReason = threeDsVerifyResponse?.RefusalReason,
                RefusalReasonCode = threeDsVerifyResponse?.RefusalReasonCode
            };

            if (!string.IsNullOrEmpty(threeDsVerifyResponse.ErrorCode) && threeDsVerifyResponse.ErrorCode != "0")
            {
                adyenPaymentResponse.Code = threeDsVerifyResponse.ErrorCode;
                adyenPaymentResponse.ErrorMessage = threeDsVerifyResponse.Message;
            }
            else if (!string.IsNullOrEmpty(threeDsVerifyResponse?.RefusalReason))
            { }
            else if (threeDsVerifyResponse.Action != null && threeDsVerifyResponse.resultCode == "ChallengeShopper")
            {
                var Response = SerializeDeSerializeHelper.DeSerialize<EnrollmentCheckResponse>(response);
                adyenPaymentResponse.AcsRequest = GenerateAcsRequest(Response);
                adyenPaymentResponse.TransactionID = Response.Action.Data?.MD ?? Response.Action.PaymentData.Substring(10, 25).Replace("/", "");
            }
            else if (threeDsVerifyResponse.Action != null && threeDsVerifyResponse.resultCode == "RedirectShopper")
            {
                var Response = SerializeDeSerializeHelper.DeSerialize<EnrollmentCheckResponse>(response);
                adyenPaymentResponse.AcsRequest = GenerateAcsRequest(Response);
                adyenPaymentResponse.TransactionID = Response.Action.Data?.MD ?? Response.Action.PaymentData.Substring(10, 25).Replace("/", "");
            }
            else
            {
                adyenPaymentResponse.TransactionID = threeDsVerifyResponse.pspReference;
                adyenPaymentResponse.PspReference = threeDsVerifyResponse.pspReference;
                adyenPaymentResponse.Status = threeDsVerifyResponse.resultCode;
                adyenPaymentResponse.MerchantReference = threeDsVerifyResponse.MerchantReference;
            }

            return adyenPaymentResponse;
        }
    }
}