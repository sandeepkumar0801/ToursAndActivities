using Logger.Contract;
using ServiceAdapters.GrayLineIceLand.Constants;
using ServiceAdapters.GrayLineIceLand.GrayLineIceLand.Commands.Contracts;
using ServiceAdapters.GrayLineIceLand.GrayLineIceLand.Entities.RequestResponseModels;
using System;
using System.Threading.Tasks;
using Util;

namespace ServiceAdapters.GrayLineIceLand.GrayLineIceLand.Commands
{
    public class GetPickupLocationsCommandHandler : CommandHandlerBase, IGetPickupLocationsCommandHandler
    {
        public GetPickupLocationsCommandHandler(ILogger iLog) : base(iLog)
        {
        }

        protected override async Task<object> GetResultsAsync(object input, string authString)
        {
            if (input != null)
            {
                var request = input as ToursAvailabilityRQ;

                var client = new AsyncClient
                {
                    ServiceURL = $"{ConfigurationManagerHelper.GetValuefromAppSettings(Constant.GrayLineBaseUrl)}{Constant.AvailabilityUrl}"
                };

                return await client.PostJsonAsync<ToursAvailabilityRQ, ToursAvailabilityRS>(authString, request);
            }
            return null;
        }

        protected override object CreateInputRequest(InputContext inputContext)
        {
            var toursAvailabilityRq = new ToursAvailabilityRQ
            {
                MakeBooking = false,
                IncludeBookingDetails = true,
                SuppressPrices = false,
                CurrencyCode = Constant.ISK,
                Language = 2,
                Tours = new Tour[]
                {
                    new Tour()
                    {
                        TourNumber = inputContext.TourNumber,
                        Dimension = Constant.Dimension,
                        FromDeparture = DateTime.Now.ToString(Constant.DateFormat),
                        UntilDeparture = DateTime.Now.AddMonths(3).ToString(Constant.DateFormat),
                        Passengers = new Passenger[]
                            {
                                //Adults
                                new Passenger
                                {
                                    AgeGroup = Constant.AdultAgeGroup,
                                    NumberOfPax = Constant.AdultNumberOfPax
                                },
                                //Children
                                new Passenger
                                {
                                    AgeGroup = Constant.ChildAgeGroup,
                                    NumberOfPax = Constant.ChildNumberOfPax
                                }
                             }
                        }
                }
            };

            return toursAvailabilityRq;
        }

        protected override object GetResults(object input, string authString)
        {
            return null;
        }

        protected override object GetResults(object input, string authString, out string requestXml, out string responseXml)
        {
            requestXml = "";
            responseXml = "";
            return null;
        }

        protected override async Task<string> GetStringResultsAsync(object input)
        {
            return null;
        }
    }
}