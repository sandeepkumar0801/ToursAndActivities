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
    public class IssuingCommandHandler : CommandHandlerBase, IIssuingCommandHandler
    {
        public IssuingCommandHandler(ILogger iLog) : base(iLog)
        {
        }

        protected override string SightSeeingApiRequest<T>(T inputContext)
        {
            var issueRequest = inputContext as IssueRequest;
            if (issueRequest == null) return null;
            issueRequest.ApiKey = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.Token);
            var issueRequestJson = SerializeDeSerializeHelper.Serialize(issueRequest);
            var content = new StringContent(issueRequestJson, Encoding.UTF8, Constant.ApplicationOrJson);
            var result = HttpClient.PostAsync(UriConstants.Issue, content);
            result.Wait();
            return result.Result.Content.ReadAsStringAsync().Result;
        }

        protected override async Task<string> SightSeeingApiRequestAsync<T>(T inputContext)
        {
            var issueRequest = inputContext as IssueRequest;
            if (issueRequest == null) return null;
            issueRequest.ApiKey = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.Token);
            var issueRequestJson = SerializeDeSerializeHelper.Serialize(issueRequest);
            var content = new StringContent(issueRequestJson, Encoding.UTF8, Constant.ApplicationOrJson);
            var result = await HttpClient.PostAsync(UriConstants.Issue, content);
            return result.Content.ReadAsStringAsync().Result;
        }

        protected override object CreateInputRequest(InputContext inputContext)
        {
            var issueRequest = new IssueRequest { TicketList = new List<TicketList>() };
            var maxParallelThreadCount = MaxParallelThreadCountHelper.GetMaxParallelThreadCount("MaxParallelThreadCount");
            Parallel.ForEach(inputContext.SelectedProducts, new ParallelOptions { MaxDegreeOfParallelism = maxParallelThreadCount }, product =>
            {
                var ticket = new TicketList
                {
                    Qty = ((CitySightseeingSelectedProduct)product).Quantity,
                    TicketId = product.Id + "_" + inputContext.BookingReferenceNumber,
                    TicketType = product.ActivityCode,
                    DepartureDate = product.ProductOptions.FirstOrDefault()?.TravelInfo.StartDate.ToString(Constant.DateFormat)
                };
                issueRequest.TicketList.Add(ticket);
            });

            return issueRequest;
        }
    }
}