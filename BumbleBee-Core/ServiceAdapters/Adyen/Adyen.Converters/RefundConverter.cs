using Isango.Entities.AdyenPayment;
using ServiceAdapters.Adyen.Adyen.Converters.Contracts;
using ServiceAdapters.Adyen.Adyen.Entities;
using Util;

namespace ServiceAdapters.Adyen.Adyen.Converters
{
    public class RefundConverter : ConverterBase, IRefundConverter
    {
        public override object Convert(string response, object inputObject)
        {
            var RefundResponse = SerializeDeSerializeHelper.DeSerialize<RefundResponse>(response);

            var adyenPaymentResponse = new AdyenPaymentResponse
            {
                RequestType = ((AdyenCriteria)inputObject).MethodType.ToString(),
                RequestJson = SerializeDeSerializeHelper.Serialize(inputObject),
                ResponseJson = response
            };

            if (!string.IsNullOrEmpty(RefundResponse.ErrorCode) && RefundResponse.ErrorCode != "0")
            {
                adyenPaymentResponse.Code = RefundResponse.ErrorCode;
                adyenPaymentResponse.ErrorMessage = RefundResponse.Message;
            }
            else
            {
                adyenPaymentResponse.PspReference = RefundResponse.PspReference;
                adyenPaymentResponse.Status = RefundResponse.Response;
            }

            adyenPaymentResponse.RefusalReason = RefundResponse.RefusalReason;
            adyenPaymentResponse.RefusalReasonCode = RefundResponse.RefusalReasonCode;

            return adyenPaymentResponse;
        }
    }
}