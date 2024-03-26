using Isango.Entities.MoulinRouge;
using Logger.Contract;
using ServiceAdapters.MoulinRouge.Constants;
using ServiceAdapters.MoulinRouge.MoulinRouge.Commands.Contracts;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TempOrderGetSendingFees = ServiceAdapters.MoulinRouge.MoulinRouge.Entities.TempOrderGetSendingFees;

namespace ServiceAdapters.MoulinRouge.MoulinRouge.Commands
{
    public class TempOrderGetSendingFeesCommandHandler : CommandHandlerBase, ITempOrderGetSendingFeesCommandHandler
    {
        public TempOrderGetSendingFeesCommandHandler(ILogger log) : base(log)
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
            var requestObject = new TempOrderGetSendingFees.Request();
            var rq = requestObject.Body.AcpTempOrderGetSendingFeesRequest;
            if (input != null) rq.IdTemporaryOrder = input.MoulinRougeContext.TemporaryOrderId;
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
            var requestObject = inputContext as TempOrderGetSendingFees.Request;
            if (requestObject == null) return null;
            var content = new StringContent(SerializeXml(requestObject), Encoding.UTF8, Constant.TextOrXml);
            var result = HttpClient.PostAsync(HttpClient.BaseAddress, content).Result;
            var responseText = result.Content.ReadAsStringAsync();
            return responseText;
        }

        protected override async Task<object> GetResultAsync<T>(T inputContext)
        {
            var requestObject = inputContext as TempOrderGetSendingFees.Request;
            if (requestObject == null) return null;
            var content = new StringContent(SerializeXml(requestObject), Encoding.UTF8, Constant.TextOrXml);
            var result = HttpClient.PostAsync(HttpClient.BaseAddress, content).Result;
            var responseText = await result.Content.ReadAsStringAsync();
            return responseText;
        }
    }
}