using Isango.Entities.MoulinRouge;
using Logger.Contract;
using ServiceAdapters.MoulinRouge.Constants;
using ServiceAdapters.MoulinRouge.MoulinRouge.Commands.Contracts;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CatalogDateGetDetailMulti = ServiceAdapters.MoulinRouge.MoulinRouge.Entities.CatalogDateGetDetailMulti;

namespace ServiceAdapters.MoulinRouge.MoulinRouge.Commands
{
    public class CatalogDateGetDetailMultiCommandHandler : CommandHandlerBase, ICatalogDateGetDetailMultiCommandHandler
    {
        private readonly ILogger _log;
        public CatalogDateGetDetailMultiCommandHandler(ILogger log) : base(log)
        {
            _log = log;
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
            var requestObject = new CatalogDateGetDetailMulti.Request();
            if (input == null) return requestObject;
            requestObject.Body.ACP_CatalogDateGetDetailMultiRequest.listID_CatalogDate =
                input.MoulinRougeContext.Ids;
            return requestObject;
        }

        /// <summary>
        /// Reutns API Result
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputContext"></param>
        /// <returns></returns>
        protected override object GetResult<T>(T inputContext)
        {
            var requestObject = inputContext as CatalogDateGetDetailMulti.Request;
            if (requestObject == null) return null;
            var content = new StringContent(SerializeXml(requestObject), Encoding.UTF8, Constant.TextOrXml);
            var result = HttpClient.PostAsync(HttpClient.BaseAddress, content).Result;
            var responseText = result.Content.ReadAsStringAsync();
            return responseText;
        }

        protected override async Task<object> GetResultAsync<T>(T inputContext)
        {
            var requestObject = inputContext as CatalogDateGetDetailMulti.Request;
            if (requestObject == null) return null;
            var content = new StringContent(SerializeXml(requestObject), Encoding.UTF8, Constant.TextOrXml);
            try
            {
                var result = HttpClient.PostAsync(HttpClient.BaseAddress, content).Result;
                var responseText = await result.Content.ReadAsStringAsync();
                return responseText;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new Isango.Entities.IsangoErrorEntity
                {
                    ClassName = "CatalogDateGetDetailMultiCommandHandler",
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