using Isango.Entities.MoulinRouge;
using Logger.Contract;
using ServiceAdapters.MoulinRouge.Constants;
using ServiceAdapters.MoulinRouge.MoulinRouge.Commands.Contracts;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CreateAccount = ServiceAdapters.MoulinRouge.MoulinRouge.Entities.CreateAccount;

namespace ServiceAdapters.MoulinRouge.MoulinRouge.Commands
{
    public class CreateAccountCommandHandler : CommandHandlerBase, ICreateAccountCommandHandler
    {
        public CreateAccountCommandHandler(ILogger log) : base(log)
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
            var requestObject = new CreateAccount.Request();
            var rq = requestObject.Body.ACP_CreateAccountRequest;
            if (input == null) return requestObject;
            rq.identity.FirstName = input.MoulinRougeContext.FirstName;
            rq.identity.Name = input.MoulinRougeContext.FullName;

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
            var requestObject = inputContext as CreateAccount.Request;
            if (requestObject == null) return null;
            var content = new StringContent(SerializeXml(requestObject), Encoding.UTF8, Constant.TextOrXml);
            var result = HttpClient.PostAsync(HttpClient.BaseAddress, content).Result;
            var responseText = result.Content.ReadAsStringAsync();
            return responseText;
        }

        protected override async Task<object> GetResultAsync<T>(T inputContext)
        {
            var requestObject = inputContext as CreateAccount.Request;
            if (requestObject == null) return null;
            var content = new StringContent(SerializeXml(requestObject), Encoding.UTF8, "text/xml");
            var result = HttpClient.PostAsync(HttpClient.BaseAddress, content).Result;
            var responseText = await result.Content.ReadAsStringAsync();
            return responseText;
        }
    }
}