using Isango.Entities.CitySightseeing;
using Logger.Contract;
using ServiceAdapters.SightSeeing.Constants;
using ServiceAdapters.SightSeeing.SightSeeing.Commands.Contracts;
using ServiceAdapters.SightSeeing.SightSeeing.Entities;
using ServiceAdapters.SightSeeing.SightSeeing.Entities.RequestResponseModels;
using Util;

namespace ServiceAdapters.SightSeeing.SightSeeing.Commands
{
    public class CancelTicketCommandHandler : CommandHandlerBase, ICancelTicketCommandHandler
    {
        public CancelTicketCommandHandler(ILogger iLog) : base(iLog)
        {
        }

        protected override string SightSeeingApiRequest<T>(T inputContext)
        {
            var cancelRequest = inputContext as CancelRequest;
            if (cancelRequest == null) return null;
            cancelRequest.Token = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.Token);
            //create query parameters string
            var inputUrl = FormUrl(cancelRequest);
            var result = HttpClient.GetAsync(inputUrl);
            result.Wait();
            return result.Result.Content.ReadAsStringAsync().Result;
        }

        protected override async Task<string> SightSeeingApiRequestAsync<T>(T inputContext)
        {
            var cancelRequest = inputContext as CancelRequest;
            if (cancelRequest == null) return null;
            cancelRequest.Token = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.Token);
            //create query parameters string
            var inputUrl = FormUrl(cancelRequest);
            var result = await HttpClient.GetAsync(inputUrl);

            return result.Content.ReadAsStringAsync().Result;
        }

        /// <summary>
        /// Create input parameter - PNR list
        /// </summary>
        /// <param name="inputContext"></param>
        /// <returns></returns>
        protected override object CreateInputRequest(InputContext inputContext)
        {
            var cancelRequest = new CancelRequest
            {
                BookingInfo = new Entities.RequestResponseModels.BookingInfo { PnrList = new List<string>() }
            };
            var maxParallelThreadCount = MaxParallelThreadCountHelper.GetMaxParallelThreadCount("MaxParallelThreadCount");
            //create pnr list for cancellation request
            Parallel.ForEach(inputContext.SelectedProducts, new ParallelOptions { MaxDegreeOfParallelism = maxParallelThreadCount }, product =>
            {
                cancelRequest.BookingInfo.PnrList.Add(((CitySightseeingSelectedProduct)product).Pnr);
            });
            return cancelRequest;
        }

        /// <summary>
        /// Create GET request URL for cancellation
        /// </summary>
        /// <param name="cancelRequest"></param>
        /// <returns></returns>
        private string FormUrl(CancelRequest cancelRequest)
        {
            return $"{UriConstants.CancelAsciiJson}{UriConstants.Token}{cancelRequest.Token}{UriConstants.JsonBookingInfo}{SerializeDeSerializeHelper.Serialize(cancelRequest.BookingInfo)}";
        }
    }
}