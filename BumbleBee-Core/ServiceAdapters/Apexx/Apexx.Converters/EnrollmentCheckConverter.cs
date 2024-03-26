using Isango.Entities.ApexxPayment;
using ServiceAdapters.Apexx.Apexx.Converters.Contracts;
using ServiceAdapters.Apexx.Apexx.Entities;
using ServiceAdapters.Apexx.Constants;
using Util;

namespace ServiceAdapters.Apexx.Apexx.Converters
{
    public class EnrollmentCheckConverter : ConverterBase, IEnrollmentCheckConverter
    {
        public override object Convert(string response, object inputObject)
        {
            var apexxCriteria = (ApexxCriteria)inputObject;

            var twoDResponse = new CreateCardTranasactionResponse();
            var threeDResponse = new EnrollmentCheckResponse();

            var apexxPaymentResponse = new ApexxPaymentResponse();

            var is3D = false;

            if (response.IndexOf(Constant.Status, StringComparison.Ordinal) != -1)
            {
                twoDResponse = SerializeDeSerializeHelper.DeSerialize<CreateCardTranasactionResponse>(response);
            }
            else
            {
                threeDResponse = SerializeDeSerializeHelper.DeSerialize<EnrollmentCheckResponse>(response);
                is3D = true;
            }

            if (is3D)
            {
                if (!string.IsNullOrEmpty(threeDResponse.Code)
                    || (!string.IsNullOrEmpty(twoDResponse.ReasonCode) && twoDResponse.ReasonCode != "0")
                    )
                {
                    apexxPaymentResponse.Code = threeDResponse.Code ?? threeDResponse.ReasonCode;
                    apexxPaymentResponse.ErrorMessage = threeDResponse.Message;
                }
                else
                {
                    apexxPaymentResponse.TransactionID = threeDResponse.TransactionId;
                    apexxPaymentResponse.AcsRequest = GenerateAcsRequest(threeDResponse, apexxCriteria.BaseUrl);
                    apexxPaymentResponse.Is2D = false;
                }
            }
            else
            {
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
            }

            apexxPaymentResponse.RequestJson = SerializeDeSerializeHelper.Serialize(inputObject);
            apexxPaymentResponse.ResponseJson = response;
            apexxPaymentResponse.RequestType = ((ApexxCriteria)inputObject).MethodType.ToString();

            return apexxPaymentResponse;
        }
    }
}