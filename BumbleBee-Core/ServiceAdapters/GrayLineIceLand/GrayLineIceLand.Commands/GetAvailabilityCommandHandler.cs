using Isango.Entities.Enums;
using Logger.Contract;
using ServiceAdapters.GrayLineIceLand.Constants;
using ServiceAdapters.GrayLineIceLand.GrayLineIceLand.Commands.Contracts;
using ServiceAdapters.GrayLineIceLand.GrayLineIceLand.Entities.RequestResponseModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using Util;

namespace ServiceAdapters.GrayLineIceLand.GrayLineIceLand.Commands
{
    public class GetAvailabilityCommandHandler : CommandHandlerBase, IGetAvailabilityCommandHandler
    {
        public GetAvailabilityCommandHandler(ILogger iLog) : base(iLog)
        {
        }

        protected override object CreateInputRequest(InputContext inputContext)
        {
            var toursAvailabilityRq = new ToursAvailabilityRQ
            {
                MakeBooking = false,
                IncludeBookingDetails = true,
                SuppressPrices = false,
                CurrencyCode = inputContext.CurrencyCode,
                Language = inputContext.Language,
                AgentProfileId = inputContext.AgentProfileId,
                ReducePickupDetails = true
            };
            var passengers = new Passenger[inputContext.PaxAgeGroupIds.Count];
            var i = 0;
            foreach (var item in inputContext.PaxAgeGroupIds)
            {
                passengers[i] = new Passenger()
                {
                    NumberOfPax = (item.Key == PassengerType.Adult) ? inputContext.Adults : item.Key == PassengerType.Child ? inputContext.Children : inputContext.Youths,
                    AgeGroup = item.Value
                };
                i++;
            }
            toursAvailabilityRq.Tours = new Tour[] {
                new Tour(){
                    TourNumber = inputContext.TourNumber,
                    Dimension = 1,
                    FromDeparture = inputContext.DateFrom.ToString(Constant.DateFormat),
                    UntilDeparture = inputContext.DateTo.ToString(Constant.DateFormat),
                    Passengers = passengers
                }
            };
            return toursAvailabilityRq;
        }

        protected override object GetResults(object input, string authString)
        {
            if (input == null) return null;
            var client = new AsyncClient
            {
                ServiceURL = $"{ConfigurationManagerHelper.GetValuefromAppSettings(Constant.GrayLineBaseUrl)}{Entities.Constants.AvailibilityURL}"
            };
            var headers = new Dictionary<string, string>
            {
                {Constant.Authorization, $"{Constant.Bearer}{authString}"},
                {Constant.Accept, Constant.App_Json},
                {Constant.Content_type, Constant.App_Json}
            };
            return client.PostJsonWithHeader((ToursAvailabilityRQ)input, headers);
        }

        protected override object GetResults(object input, string authString, out string requestXml, out string responseXml)
        {
            requestXml = "";
            responseXml = "";
            return null;
        }

        protected override Task<object> GetResultsAsync(object input, string authString)
        {
            if (input != null)
            {
                var client = new AsyncClient
                {
                    ServiceURL =
                        $"{ConfigurationManagerHelper.GetValuefromAppSettings(Constant.GrayLineBaseUrl)}{Entities.Constants.AvailibilityURL}"
                };
                return client.PostJsonAsync(authString, (ToursAvailabilityRQ)input);
            }
            return null;
        }

        protected override async Task<string> GetStringResultsAsync(object input)
        {
            return null;
        }
    }
}