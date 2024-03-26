using Isango.Entities.AdyenPayment;
using ServiceAdapters.Adyen.Adyen.Converters.Contracts;
using ServiceAdapters.Adyen.Adyen.Entities;
using Util;

namespace ServiceAdapters.Adyen.Adyen.Converters
{
    public class CancelConverter : ConverterBase, ICancelConverter
    {
        public override object Convert(string response, object inputObject)
        {
            var cancelResponse = SerializeDeSerializeHelper.DeSerialize<CancelResponse>(response);
            var adyenPaymentResponse = new AdyenPaymentResponse
            {
                TransactionID = cancelResponse.PspReference,
                AuthorizationCode = cancelResponse.PspReference,
                RequestType = ((AdyenCriteria)inputObject).MethodType.ToString(),
                RequestJson = SerializeDeSerializeHelper.Serialize(inputObject),
                ResponseJson = response
            };

            if (!string.IsNullOrEmpty(cancelResponse.ErrorCode) && cancelResponse.ErrorCode != "0")
            {
                adyenPaymentResponse.Code = cancelResponse.ErrorCode;
                adyenPaymentResponse.ErrorMessage = cancelResponse.Message;
            }
            else
            {
                adyenPaymentResponse.PspReference = cancelResponse.PspReference;
                adyenPaymentResponse.Status = cancelResponse.Response;
            }

            adyenPaymentResponse.RefusalReason = cancelResponse.RefusalReason;
            adyenPaymentResponse.RefusalReasonCode = cancelResponse.RefusalReasonCode;
            return adyenPaymentResponse;
        }
    }
}