using Isango.Entities.MoulinRouge;
using Logger.Contract;
using ServiceAdapters.MoulinRouge.Constants;
using ServiceAdapters.MoulinRouge.MoulinRouge.Commands.Contracts;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TempOrderSetSendingFees = ServiceAdapters.MoulinRouge.MoulinRouge.Entities.TempOrderSetSendingFees;

namespace ServiceAdapters.MoulinRouge.MoulinRouge.Commands
{
    public class TempOrderSetSendingFeesCommandHandler : CommandHandlerBase, ITempOrderSetSendingFeesCommandHandler
    {
        public TempOrderSetSendingFeesCommandHandler(ILogger log) : base(log)
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
            var requestObject = new TempOrderSetSendingFees.Request();
            var rq = requestObject.Body.ACP_TempOrderSetSendingFeesRequest;
            if (input == null) return requestObject;
            rq.id_TemporaryOrder = input.MoulinRougeContext.TemporaryOrderId;
            rq.SendingFee.Comment = new object();
            rq.SendingFee.FeeType = input.MoulinRougeContext.FeeType;
            rq.SendingFee.Label = new object();
            rq.SendingFee.Nombre = input.MoulinRougeContext.Nombre;
            rq.SendingFee.TypeCalcul = input.MoulinRougeContext.TypeCalcul;
            rq.SendingFee.UnitAmount = input.MoulinRougeContext.UnitAmount;
            rq.SendingFee.GlobalAmount = input.MoulinRougeContext.GlobalAmount;
            rq.SendingFee.ID_SendingFee = input.MoulinRougeContext.SendingFeeId;
            rq.SendingFee.Status = input.MoulinRougeContext.Status;

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
            var requestObject = inputContext as TempOrderSetSendingFees.Request;
            if (requestObject == null) return null;
            var content = new StringContent(SerializeXml(requestObject), Encoding.UTF8, Constant.TextOrXml);
            var result = HttpClient.PostAsync(HttpClient.BaseAddress, content).Result;
            var responseText = result.Content.ReadAsStringAsync();
            return responseText;
        }

        protected override async Task<object> GetResultAsync<T>(T inputContext)
        {
            var requestObject = inputContext as TempOrderSetSendingFees.Request;
            if (requestObject == null) return null;
            var content = new StringContent(SerializeXml(requestObject), Encoding.UTF8, Constant.TextOrXml);
            var result = HttpClient.PostAsync(HttpClient.BaseAddress, content).Result;
            var responseText = await result.Content.ReadAsStringAsync();
            return responseText;
        }
    }
}