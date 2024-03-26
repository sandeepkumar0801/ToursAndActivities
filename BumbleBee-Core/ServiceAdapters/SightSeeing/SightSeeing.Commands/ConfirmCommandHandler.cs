using Isango.Entities.CitySightseeing;
using Logger.Contract;
using ServiceAdapters.SightSeeing.Constants;
using ServiceAdapters.SightSeeing.SightSeeing.Commands.Contracts;
using ServiceAdapters.SightSeeing.SightSeeing.Entities;
using ServiceAdapters.SightSeeing.SightSeeing.Entities.RequestResponseModels;
using System.Text;
using Util;

namespace ServiceAdapters.SightSeeing.SightSeeing.Commands
{
    public class ConfirmCommandHandler : CommandHandlerBase, IConfirmCommandHandler
    {
        public ConfirmCommandHandler(ILogger iLog) : base(iLog)
        {
        }

        protected override string SightSeeingApiRequest<T>(T inputContext)
        {
            var confirmRequest = inputContext as ConfirmRequest;
            if (confirmRequest == null) return null;
            confirmRequest.ApiKey = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.Token);

            var confirmRequestJson = SerializeDeSerializeHelper.Serialize(confirmRequest);
            var content = new StringContent(confirmRequestJson, Encoding.UTF8, Constant.ApplicationOrJson);
            var result = HttpClient.PostAsync(UriConstants.Confirm, content);

            result.Wait();
            return result.Result.Content.ReadAsStringAsync().Result;
        }

        protected override async Task<string> SightSeeingApiRequestAsync<T>(T inputContext)
        {
            var confirmRequest = inputContext as ConfirmRequest;
            if (confirmRequest == null) return null;
            confirmRequest.ApiKey = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.Token);
            var confirmRequestJson = SerializeDeSerializeHelper.Serialize(confirmRequest);
            var content = new StringContent(confirmRequestJson, Encoding.UTF8, Constant.ApplicationOrJson);
            var result = await HttpClient.PostAsync(UriConstants.Confirm, content);
            return result.Content.ReadAsStringAsync().Result;
        }

        protected override object CreateInputRequest(InputContext inputContext)
        {
            var confirmRequest = new ConfirmRequest { PnrList = new List<string>() };
            var maxParallelThreadCount = MaxParallelThreadCountHelper.GetMaxParallelThreadCount("MaxParallelThreadCount");
            Parallel.ForEach(inputContext.SelectedProducts, new ParallelOptions { MaxDegreeOfParallelism = maxParallelThreadCount }, product =>
            {
                confirmRequest.PnrList.Add(((CitySightseeingSelectedProduct)product).Pnr);
            });

            return confirmRequest;
        }
    }
}