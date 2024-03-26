using Isango.Entities.ApexxPayment;
using ServiceAdapters.Apexx.Apexx.Converters.Contracts;
using ServiceAdapters.Apexx.Apexx.Entities;
using Util;

namespace ServiceAdapters.Apexx.Apexx.Converters
{
    public class RefundConverter : ConverterBase, IRefundConverter
    {
        public override object Convert(string response, object inputObject)
        {
            var refundResponse = SerializeDeSerializeHelper.DeSerialize<RefundResponse>(response);

            var apexxPaymentResponse = new ApexxPaymentResponse
            {
                TransactionID = refundResponse.TransactionId,
                AuthorizationCode = refundResponse.AuthorizationCode,
                RequestJson = SerializeDeSerializeHelper.Serialize(inputObject),
                ResponseJson = response,
                RequestType = ((ApexxCriteria)inputObject).MethodType.ToString()
            };

            // 2 cases handle here(1.Case "_id":"","code":"107") 2.) "reason_code": "5", 
            if ((!string.IsNullOrEmpty(refundResponse.Code) && refundResponse.Code != "0")
                || (!string.IsNullOrEmpty(refundResponse.ReasonCode) && refundResponse.ReasonCode != "0"))
                
            {
                apexxPaymentResponse.Code = refundResponse.Code;
                apexxPaymentResponse.ErrorMessage = refundResponse.Message;
            }
            else
            {
                apexxPaymentResponse.Status = refundResponse.Status;
                apexxPaymentResponse.Amount = refundResponse.Amount;
                apexxPaymentResponse.ReasonCode = refundResponse.ReasonCode;
                apexxPaymentResponse.ReasonMessage = refundResponse.ReasonMessage;
            }

            return apexxPaymentResponse;
        }
    }
}