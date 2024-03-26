using Isango.Entities.MoulinRouge;
using Logger.Contract;
using ServiceAdapters.MoulinRouge.Constants;
using ServiceAdapters.MoulinRouge.MoulinRouge.Commands.Contracts;
using ServiceAdapters.MoulinRouge.MoulinRouge.Entities;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using OrderConfirm = ServiceAdapters.MoulinRouge.MoulinRouge.Entities.OrderConfirm;

namespace ServiceAdapters.MoulinRouge.MoulinRouge.Commands
{
    public class OrderConfirmCommandHandler : CommandHandlerBase, IOrderConfirmCommandHandler
    {
        public OrderConfirmCommandHandler(ILogger log) : base(log)
        {
        }

        /// <summary>
        /// Return Converted object by converting Response from API
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputContext"></param>
        /// <returns></returns>
        protected override object CreateInputRequest<T>(T inputContext)
        {
            var input = inputContext as MoulinRougeCriteria;
            var requestObject = new OrderConfirm.Request();
            var rq = requestObject.Body.ACP_OrderConfirmRequest;
            var apiConfig = MoulinRougeAPIConfig.GetInstance();
            if (input != null)
            {
                rq.id_TemporaryOrder = input.MoulinRougeContext.TemporaryOrderId;
                rq.listPaymentMode.ACPO_OrderPaymentMode.Amount = input.MoulinRougeContext.Amount;
                rq.listPaymentMode.ACPO_OrderPaymentMode.ID_PaymentMode = apiConfig.PaymentModeId;
                rq.listPaymentMode.ACPO_OrderPaymentMode.KeyPayReference =
                    input.MoulinRougeContext.IsangoBookingReferenceNumber;
                rq.id_transcodeOrder = Constant.Isango + DateTime.Now.ToString(Constant.DateFormatinSeconds);
                rq.id_IdentityConsumer = input.MoulinRougeContext.IdentityConsumerId;
            }

            rq.ID_DeliveryAddress = apiConfig.AddressId;
            rq.id_Identitymain = apiConfig.ClientId;
            return requestObject;
        }

        /// <summary>
        /// Returns API Result
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputContext"></param>
        /// <returns></returns>
        protected override object GetResult<T>(T inputContext)
        {
            var requestObject = inputContext as OrderConfirm.Request;
            if (requestObject == null) return null;
            var content = new StringContent(SerializeXml(requestObject), Encoding.UTF8, Constant.TextOrXml);
            var result = HttpClient.PostAsync(HttpClient.BaseAddress, content).Result;
            var responseText = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return responseText;
        }

        /// <summary>
        /// Returns API Result asynchronously
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputContext"></param>
        /// <returns></returns>
        protected override async Task<object> GetResultAsync<T>(T inputContext)
        {
            var requestObject = inputContext as OrderConfirm.Request;
            if (requestObject == null) return null;
            var content = new StringContent(SerializeXml(requestObject), Encoding.UTF8, Constant.TextOrXml);
            var result = HttpClient.PostAsync(HttpClient.BaseAddress, content).Result;
            var responseText = await result.Content.ReadAsStringAsync();
            return responseText;
        }
    }
}