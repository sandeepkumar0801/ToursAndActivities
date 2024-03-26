using Isango.Entities.ApexxPayment;
using ServiceAdapters.Apexx.Apexx.Converters.Contracts;
using ServiceAdapters.Apexx.Apexx.Entities;
using Util;

namespace ServiceAdapters.Apexx.Apexx.Converters
{
    public class CreateCardTransactionConverter : ConverterBase, ICreateCardTransactionConverter
    {
        public override object Convert(string response, object inputObject)
        {
            var apexxCriteria = (ApexxCriteria)inputObject;
            var twoDResponse = new CreateCardTranasactionResponse();
            var apexxPaymentResponse = new ApexxPaymentResponse();
            twoDResponse = SerializeDeSerializeHelper.DeSerialize<CreateCardTranasactionResponse>(response);
            if (!string.IsNullOrEmpty(twoDResponse.ReasonCode) && twoDResponse.ReasonCode != "0")
                {
                    apexxPaymentResponse.Code = twoDResponse.ReasonCode;
                    apexxPaymentResponse.ErrorMessage = twoDResponse.ReasonMessage;
                }
                else
                {
                    apexxPaymentResponse.TransactionID = twoDResponse.TransactionId;
                    apexxPaymentResponse.Pares = twoDResponse.Pares;
                    apexxPaymentResponse.Status = twoDResponse.Status;
                    apexxPaymentResponse.Is2D = true;
                }
            apexxPaymentResponse.RequestJson = SerializeDeSerializeHelper.Serialize(inputObject);
            apexxPaymentResponse.ResponseJson = response;
            apexxPaymentResponse.RequestType = ((ApexxCriteria)inputObject).MethodType.ToString();
            return apexxPaymentResponse;
        }
    }
}