using Isango.Entities.Aot;
using Isango.Entities.Enums;
using Logger.Contract;
using ServiceAdapters.Aot.Aot.Commands.Contracts;
using ServiceAdapters.Aot.Aot.Entities.RequestResponseModels;
using System.Text;
using Constant = ServiceAdapters.Aot.Constants.Constant;

namespace ServiceAdapters.Aot.Aot.Commands
{
    public class GetDetailedPricingAvailabilityCommandHandler : CommandHandlerBase, IGetDetailedPricingAvailabilityCommandHandler
    {
        public GetDetailedPricingAvailabilityCommandHandler(ILogger iLog) : base(iLog)
        {
        }

        protected override object AotApiRequest<T>(T inputContext)
        {
            var optionStayPricingRequest = inputContext as OptionStayPricingRequest;
            if (optionStayPricingRequest == null) return null;

            optionStayPricingRequest.AgentId = AgentId;
            optionStayPricingRequest.Password = Password;

            var content = new StringContent(SerializeXml(optionStayPricingRequest), Encoding.UTF8, Constant.ApplicationMediaType);
            var result = HttpClient.PostAsync(string.Empty, content);
            result.Wait();
            return result.Result.Content.ReadAsStringAsync().Result;
        }

        protected override async Task<object> AotApiRequestAsync<T>(T inputContext)
        {
            var optionStayPricingRequest = inputContext as OptionStayPricingRequest;
            if (optionStayPricingRequest == null) return null;
            optionStayPricingRequest.AgentId = AgentId;
            optionStayPricingRequest.Password = Password;

            var content = new StringContent(SerializeXml(optionStayPricingRequest), Encoding.UTF8, Constant.ApplicationMediaType);
            var result = await HttpClient.PostAsync(string.Empty, content);
            return result.Content.ReadAsStringAsync().Result;
        }

        protected override object CreateInputRequest<T>(T inputContext)
        {
            var input = inputContext as AotCriteria;
            var optionStayPricingRequest = new OptionStayPricingRequest();
            if (input == null) return optionStayPricingRequest;

            optionStayPricingRequest.Opts = new Opts { Opt = input.OptCode };
            optionStayPricingRequest.DateFrom = input.CheckinDate.ToString(Constant.DateFormatmmddyyhipen);
            var roomDetail = new RoomConfig()
            {
                Adults = input.NoOfPassengers.FirstOrDefault(x => x.Key == PassengerType.Adult).Value.ToString(),
                Children = input.NoOfPassengers.FirstOrDefault(x => x.Key == PassengerType.Child).Value.ToString()
            };
            optionStayPricingRequest.MachineCancelPolicies = 1;
            optionStayPricingRequest.RoomConfigs = new RoomConfigs();
            optionStayPricingRequest.ReturnCancelPolicy = input.CancellationPolicy.Equals(true) ? "1" : "";
            optionStayPricingRequest.RoomConfigs.RoomConfig = new List<RoomConfig>
            {
                roomDetail
            };
            return optionStayPricingRequest;
        }
    }
}