using Logger.Contract;
using ServiceAdapters.PrioHub.PrioHub.Commands.Contract;
using ServiceAdapters.PrioHub.PrioHub.Entities.GetVoucherRes;
using Util;
namespace ServiceAdapters.PrioHub.PrioHub.Commands
{
    public class GetVoucherCmdHandler : CommandHandlerBase, IGetVoucherCommandHandler
    {
        public GetVoucherCmdHandler(ILogger iLog) : base(iLog)
        {
        }

        protected override object CreateInputRequest<T>(T criteria)
        {
            var newPrioCriteria = criteria as Tuple<string,int>;
            return newPrioCriteria;
        }
        protected override object GetResponseObject(string responseText)
        {
            var result = default(GetVoucherRes);
            try
            {
                result = SerializeDeSerializeHelper.DeSerialize<GetVoucherRes>(responseText);
            }
            catch (Exception ex)
            {
                //ignored
                //#TODO Add logging here
                result = null;
            }
            return result;
        }
        protected override object GetResultsAsync(object input)
        {
            var newPrioCriteria = input as Tuple<string,int>;
            var scope = _PrioHubApiScopeProducts;
            //1. Using basic Auth Get AccessToken 
            var accessToken = AddRequestHeadersAndAddressToApi(scope, newPrioCriteria.Item2);
            //https://sandbox-distributor-api.prioticket.com/v3.5/distributor/
            var url = newPrioCriteria.Item1;

            IDictionary<string, string> queryParams = input as IDictionary<string, string>;
            var client = new AsyncClient
            {
                ServiceURL = url
            };
            //2. Using Bearer"
            return client.ConsumeGetServicePrioHub(GetHttpRequestHeaders(accessToken), queryParams);
        }

    }
}