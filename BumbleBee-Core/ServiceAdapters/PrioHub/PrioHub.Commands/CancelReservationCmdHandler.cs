using Logger.Contract;
using ServiceAdapters.PrioHub.Constants;
using ServiceAdapters.PrioHub.PrioHub.Commands.Contract;
using ServiceAdapters.PrioHub.PrioHub.Entities.CancelReservationResponse;
using Util;

namespace ServiceAdapters.PrioHub.PrioHub.Commands
{
    public class CancelReservationCmdHandler : CommandHandlerBase, ICancelReservationCmdHandler
    {
        public CancelReservationCmdHandler(ILogger iLog) : base(iLog)
        {
        }
        protected override object CreateInputRequest<T>(T inputContext)
        {
            var newPrioCriteria = inputContext as Tuple<string,int>;
            return newPrioCriteria;
        }

        protected override object GetResultsAsync(object input)
        {
            var newPrioHubCriteria = input as Tuple<string, int>;
            var scope = _PrioHubApiScopeReservation;
            //1. Using basic Auth Get AccessToken 
            var accessToken = AddRequestHeadersAndAddressToApi(scope, newPrioHubCriteria.Item2);
            var url = newPrioHubCriteria?.Item1;
            var client = new AsyncClient
            {
                ServiceURL = url
            };
            var queryParams = new Dictionary<string, string>
            {
                {Constant.Authorization,$"{Constant.Bearer}{accessToken}"},
                {Constant.Accept, Constant.App_Json},
                {Constant.Content_type, Constant.App_Json}
            };
            var httpResponse = client.PostJsonWithHeadersAsync("", queryParams, "DELETE");
            if (httpResponse != null)
            {
                var returnData = httpResponse.GetAwaiter().GetResult();
                var response = (HttpResponseMessage)returnData;
                var responseText = response;//.Content.ReadAsStringAsync().Result;
                return responseText;
            }
            else
            {
                return null;
            }
        }
      
        protected override object GetResponseObject(string responseText)
        {
            // DeserializeObject to the Response class
            return SerializeDeSerializeHelper.DeSerialize<CancelReservationResponse>(responseText.ToString());
        }
        }
}