using Isango.Entities.ApexxPayment;
using ServiceAdapters.Apexx.Apexx.Converters.Contracts;
using ServiceAdapters.Apexx.Apexx.Entities;
using Util;

namespace ServiceAdapters.Apexx.Apexx.Converters
{
    public class CancelConverter : ConverterBase, ICancelConverter
    {
        public override object Convert(string response, object inputObject)
        {
            var cancelResponse = SerializeDeSerializeHelper.DeSerialize<CancelResponse>(response);
            var apexxPaymentResponse = new ApexxPaymentResponse
            {
                TransactionID = cancelResponse.TransactionId,
                AuthorizationCode = cancelResponse.AuthorizationCode,
                RequestType = ((ApexxCriteria)inputObject).MethodType.ToString(),
                RequestJson = SerializeDeSerializeHelper.Serialize(inputObject),
                ResponseJson = response
            };

            if (!string.IsNullOrEmpty(cancelResponse.Code))
            {
                apexxPaymentResponse.Code = cancelResponse.Code;
                apexxPaymentResponse.ErrorMessage = cancelResponse.Message;
            }
            else
            {
                apexxPaymentResponse.Status = cancelResponse.Status;
                apexxPaymentResponse.Amount = cancelResponse.Amount;
            }

            return apexxPaymentResponse;
        }
    }
}