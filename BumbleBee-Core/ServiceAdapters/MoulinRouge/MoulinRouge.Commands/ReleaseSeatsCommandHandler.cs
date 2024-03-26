using Isango.Entities.MoulinRouge;
using Logger.Contract;
using ServiceAdapters.MoulinRouge.Constants;
using ServiceAdapters.MoulinRouge.MoulinRouge.Commands.Contracts;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ReleaseSeats = ServiceAdapters.MoulinRouge.MoulinRouge.Entities.ReleaseSeats;

namespace ServiceAdapters.MoulinRouge.MoulinRouge.Commands
{
    public class ReleaseSeatsCommandHandler : CommandHandlerBase, IReleaseSeatsCommandHandler
    {
        public ReleaseSeatsCommandHandler(ILogger log) : base(log)
        {
        }

        /// <summary>
        /// Return request required to hit API
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputContext"></param>
        /// <returns></returns>
        protected override object CreateInputRequest<T>(T inputContext)
        {
            var input = inputContext as MoulinRougeCriteria;
            var requestObject = new ReleaseSeats.Request();
            var rq = requestObject.Body.AcpReleaseSeatsRequest;
            if (input == null) return requestObject;
            rq.IdTemporaryOrderRow = input.MoulinRougeContext.TemporaryOrderRowId;
            rq.ListSeats = input.MoulinRougeContext.Ids;

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
            var requestObject = inputContext as ReleaseSeats.Request;
            if (requestObject == null) return null;
            var content = new StringContent(SerializeXml(requestObject), Encoding.UTF8, Constant.TextOrXml);
            var result = HttpClient.PostAsync(HttpClient.BaseAddress, content).Result;
            var responseText = result.Content.ReadAsStringAsync();
            return responseText;
        }

        protected override async Task<object> GetResultAsync<T>(T inputContext)
        {
            var requestObject = inputContext as ReleaseSeats.Request;
            if (requestObject == null) return null;
            var content = new StringContent(SerializeXml(requestObject), Encoding.UTF8, Constant.TextOrXml);
            var result = HttpClient.PostAsync(HttpClient.BaseAddress, content).Result;
            var responseText = await result.Content.ReadAsStringAsync();
            return responseText;
        }
    }
}