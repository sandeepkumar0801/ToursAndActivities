using Isango.Entities.ApexxPayment;
using ServiceAdapters.Apexx.Apexx.Converters.Contracts;
using ServiceAdapters.Apexx.Apexx.Entities;
using Util;

namespace ServiceAdapters.Apexx.Apexx.Converters
{
    public class ThreeDsVerifyConverter : ConverterBase, IThreeDsVerifyConverter
    {
        public override object Convert(string response, object inputObject)
        {
            var threeDsVerifyResponse = SerializeDeSerializeHelper.DeSerialize<ThreeDVerifyResponse>(response);

            var apexxPaymentResponse = new ApexxPaymentResponse
            {
                RequestJson = SerializeDeSerializeHelper.Serialize(inputObject),
                ResponseJson = response,
                RequestType = ((ApexxCriteria)inputObject).MethodType.ToString()
            };

            if (!string.IsNullOrEmpty(threeDsVerifyResponse.ReasonCode) && threeDsVerifyResponse.ReasonCode != "0")
            {
                apexxPaymentResponse.Code = threeDsVerifyResponse.ReasonCode;
                apexxPaymentResponse.ErrorMessage = threeDsVerifyResponse.ReasonMessage;
            }
            else
            {
                apexxPaymentResponse.TransactionID = threeDsVerifyResponse.TransactionId;
                apexxPaymentResponse.Pares = threeDsVerifyResponse.Pares;
                apexxPaymentResponse.Status = threeDsVerifyResponse.Status;
                apexxPaymentResponse.MerchantReference = threeDsVerifyResponse.MerchantReference;
                apexxPaymentResponse.Token = threeDsVerifyResponse.Card?.Token;
            }

            return apexxPaymentResponse;
        }
    }
}