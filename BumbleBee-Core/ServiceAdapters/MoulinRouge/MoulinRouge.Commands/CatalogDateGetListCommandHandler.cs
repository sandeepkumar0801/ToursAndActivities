using Isango.Entities.MoulinRouge;
using Logger.Contract;
using ServiceAdapters.MoulinRouge.Constants;
using ServiceAdapters.MoulinRouge.MoulinRouge.Commands.Contracts;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CatalogDateGetList = ServiceAdapters.MoulinRouge.MoulinRouge.Entities.CatalogDateGetList;

namespace ServiceAdapters.MoulinRouge.MoulinRouge.Commands
{
    public class CatalogDateGetListCommandHandler : CommandHandlerBase, ICatalogDateGetListCommandHandler
    {
        private readonly ILogger _log;
        public CatalogDateGetListCommandHandler(ILogger log) : base(log)
        {
            _log = log;
        }

        /// <summary>
        /// Retun request required to hit API
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputContext"></param>
        /// <returns></returns>
        protected override object CreateInputRequest<T>(T inputContext)
        {
            var input = inputContext as MoulinRougeCriteria;
            var requestObject = new CatalogDateGetList.Request();
            if (input == null) return requestObject;
            requestObject.Body.ACP_CatalogDateGetListRequest.dateFrom = input.CheckinDate;
            requestObject.Body.ACP_CatalogDateGetListRequest.dateTo = input.CheckoutDate;

            return requestObject;
        }

        /// <summary>
        /// Returns API result
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputContext"></param>
        /// <returns></returns>
        protected override object GetResult<T>(T inputContext)
        {
            var requestObject = inputContext as CatalogDateGetList.Request;
            if (requestObject == null) return null;
            var content = new StringContent(SerializeXml(requestObject), Encoding.UTF8, Constant.TextOrXml);
            var result = HttpClient.PostAsync(HttpClient.BaseAddress, content).Result;
            var responseText = result.Content.ReadAsStringAsync();
            return responseText;
        }

        protected override async Task<object> GetResultAsync<T>(T inputContext)
        {
            var requestObject = inputContext as CatalogDateGetList.Request;
            if (requestObject == null) return null;
            var content = new StringContent(SerializeXml(requestObject), Encoding.UTF8, Constant.TextOrXml);
            try
            {
                var result = HttpClient.PostAsync(HttpClient.BaseAddress, content).Result;
                var responseText = await result.Content.ReadAsStringAsync();
                return responseText;
            }
            catch(Exception ex)
            {
                var isangoErrorEntity = new Isango.Entities.IsangoErrorEntity
                {
                    ClassName = "CatalogDateGetListCommandHandler",
                    MethodName = "GetResultAsync",
                    Params = Util.SerializeDeSerializeHelper.Serialize(inputContext)
                };
                _log.Error(isangoErrorEntity, ex);
                //timeout probably - check logs;
                return null;
            }
            
        }
    }
}