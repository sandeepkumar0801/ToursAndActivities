using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Enums;
using Logger.Contract;
using ServiceAdapters.GlobalTix.Constants;
using ServiceAdapters.GlobalTix.GlobalTix.Commands.Contracts;
using ServiceAdapters.GlobalTix.GlobalTix.Entities;
using ServiceAdapters.GlobalTix.GlobalTix.Entities.RequestResponseModels;
using System.Text;
using Util;
namespace ServiceAdapters.GlobalTix.GlobalTix.Commands
{
    public class CreateBookingCommandHandler : CommandHandlerBase, ICreateBookingCommandHandler
    {
        private string _supportEmail = "support@isango.com";

        public CreateBookingCommandHandler(ILogger iLog) : base(iLog)
        {
            _supportEmail = ConfigurationManagerHelper.GetValuefromAppSettings("mailfrom");
        }

        protected override object CreateInputRequest(InputContext inputContext)
        {
            BookInputContext bookCtx = inputContext as BookInputContext;
            if (bookCtx == null)
            {
                return null;
            }

            BookingRQ bookRQ = new BookingRQ() { TicketTypes = new List<BookTicketType>() };
            int index = 0;

            foreach (SelectedProduct selProduct in bookCtx.SelectedProducts)
            {
                ActivityOption actOpt = selProduct.ProductOptions.FirstOrDefault(prodOpt => prodOpt.IsSelected) as ActivityOption;
                if (actOpt == null)
                {
                    // TODO: Set failed status and continue
                    continue;
                }

                IDictionary<PassengerType, string> mapperPassengerTypeToTicketIds = CreatePassengerTypeToTicketIdMapper(actOpt.RateKey);
                foreach (KeyValuePair<PassengerType, int> psgrTypePaxCount in actOpt.TravelInfo.NoOfPassengers)
                {
                    if (mapperPassengerTypeToTicketIds.ContainsKey(psgrTypePaxCount.Key))
                    {
						string ticketId = mapperPassengerTypeToTicketIds[psgrTypePaxCount.Key];

						BookTicketType bookTicketType = new BookTicketType()
						{
							Id = int.Parse(ticketId),
							Index = index++,
							Quantity = psgrTypePaxCount.Value,
							VisitDate = actOpt.TravelInfo.StartDate.ToString("yyyy-MM-dd"),
                            Questions = GetContractQuestions(actOpt.ContractQuestions,((GlobalTixSelectedProduct)selProduct)?.ContractQuestions, actOpt?.HotelPickUpLocation)
                        };

						int? evtId = GetActivityEventId(actOpt.SellPrice?.DatePriceAndAvailabilty, actOpt.TravelInfo.StartDate);
                        if (evtId != null)
                        {
                            bookTicketType.EventId = evtId;
                        }
                        bookRQ.TicketTypes.Add(bookTicketType);
                    }
                }

                // TODO: Check if the customer information can be retrieved from somewhere else instead of ActivityOption???
                Customer leadCustomer = actOpt.Customers.FirstOrDefault(x => x.IsLeadCustomer);
                if (leadCustomer != null)
                {
                    bookRQ.CustomerName = $"{leadCustomer.FirstName} {leadCustomer.LastName}";
                    bookRQ.Email = GetBookingEmail(leadCustomer.Email);
                }
            }

            // As this booking is a transaction between Isango and GlobalTix, payment method is set to CREDIT
            bookRQ.PaymentMethod = "CREDIT";

            //Add support email to alternative email
            bookRQ.AlternateEmail = _supportEmail;

            return bookRQ;
        }

        protected override object GetResults(object input, string authString)
        {
            BookingRQ bookRQ = input as BookingRQ;
            if (bookRQ == null)
            {
                return null;
            }

            AsyncClient client = GetServiceClient(Constant.URL_CreateBooking);
            return client.ConsumePostService(GetHttpRequestHeaders(), Constant.HttpHeader_ContentType_JSON, SerializeDeSerializeHelper.Serialize(bookRQ), Encoding.UTF8);
        }

        protected override async Task<object> GetResultsAsync(object input, string authString)
        {
            if (input != null)
            {
                var client = new AsyncClient
                {
                    ServiceURL = $"{ConfigurationManagerHelper.GetValuefromAppSettings(Constant.CfgParam_GlobalTixBaseUrl)}{Constant.URL_CreateBooking}"
                };
                return await client.PostJsonAsync<BookingRQ>(authString, (BookingRQ)input);
            }
            return null;
        }

        public Passenger GeneratePassanger(Dictionary<PassengerType, int> ageGroupIds, TravelInfo travelInfo, PassengerType type)
        {
            var passenger = new Passenger()
            {
                AgeGroup = ageGroupIds.FirstOrDefault(x => x.Key.Equals(type)).Value,
                NumberOfPax = travelInfo.NoOfPassengers.FirstOrDefault(x => x.Key.Equals(type)).Value,
                Quantity = travelInfo.NoOfPassengers.FirstOrDefault(x => x.Key.Equals(type)).Value
            };

			return passenger;
        }

        private IDictionary<PassengerType, string> CreatePassengerTypeToTicketIdMapper(string suppOptCode)
        {
            IDictionary<PassengerType, string> mapperPassengerTypeToTicketIds = new Dictionary<PassengerType, string>();
            string[] psgrTypeToTktIds = suppOptCode.Split(':');
            // TODO: Need to check that these are even number of tokens. What to do if it is not?
            for (int i = 0; i < psgrTypeToTktIds.Length; i += 2)
            {
                if (Enum.TryParse(psgrTypeToTktIds[i], out PassengerType psgrType) && Enum.IsDefined(typeof(PassengerType), psgrType))
                {
                    mapperPassengerTypeToTicketIds.Add(psgrType, psgrTypeToTktIds[i + 1]);
                }
            }

            return mapperPassengerTypeToTicketIds;
        }

        private int? GetActivityEventId(Dictionary<DateTime, PriceAndAvailability> dtPrcAvails, DateTime activityDate)
        {
            // If there are no datewise price & availabilities, there will be no corresponding event id 
            if (dtPrcAvails == null || dtPrcAvails.Count < 1 || dtPrcAvails.ContainsKey(activityDate) == false)
            {
                return null;
            }

            PriceAndAvailability dtPrcAvail = dtPrcAvails[activityDate];
            return dtPrcAvail.TourDepartureId;
        }

		private string GetBookingEmail(string leadCustomerEmail)
		{
			string notifyIsango = ConfigurationManagerHelper.GetValuefromAppSettings($"{APIType.GlobalTix}{Constant.AppSettings_Suffix_Notify_Isango}");
			string notifyIsangoEmail = ConfigurationManagerHelper.GetValuefromAppSettings($"{APIType.GlobalTix}{Constant.AppSettings_Suffix_Notify_Isango_Email}");
			return (Constant.Flag_True_Value.Equals(notifyIsango) && String.IsNullOrEmpty(notifyIsangoEmail) == false) ? notifyIsangoEmail : leadCustomerEmail;
		}
        /// <summary>
        /// Get Contract Questions
        /// </summary>
        /// <param name="contractQuestions"></param>
        /// <returns></returns>
        private List<BookQuestion> GetContractQuestions(List<ContractQuestion> apiContractQuestions,List<ContractQuestion> userSelectedContractQuestions,string userSelectedPickupLocation)
        {
            var bookedQuestions = new List<BookQuestion>();
            if (apiContractQuestions != null && apiContractQuestions.Count()>0)
            {
                foreach (var item in apiContractQuestions)
                {
                    if (String.IsNullOrEmpty(item.Answer))
                    {
                        var userSelectedAnswer = userSelectedContractQuestions?.Where(x => x.Name?.Trim()?.ToLowerInvariant() == item?.Description?.Trim()?.ToLowerInvariant())?.SingleOrDefault();
                        if (userSelectedAnswer != null)
                        {
                            var bookedQuestion = new BookQuestion
                            {
                                Id = Convert.ToInt32(item?.Code),
                                Answer = userSelectedAnswer?.Answer?.Trim()
                            };
                            bookedQuestions.Add(bookedQuestion);
                        }
                    }
                    else
                    {
                        var splitPickpLocation = userSelectedPickupLocation?.Trim()?.ToLowerInvariant().Split('-');
                        if (splitPickpLocation?.Count() > 1)
                        {
                            if (item.Answer.Trim().ToLowerInvariant().Contains(splitPickpLocation[1]?.Trim()?.ToLowerInvariant()))
                            {
                                var bookedQuestion = new BookQuestion
                                {
                                    Id = Convert.ToInt32(item?.Code),
                                    Answer = splitPickpLocation[1]?.Trim()
                                };
                                bookedQuestions.Add(bookedQuestion);
                            }
                        }
                    }
                }
            }
            return bookedQuestions;
        }

        protected override object CreateInputRequest(InputContext inputContext, bool isNonThailandProduct)
        {
            throw new NotImplementedException();
        }
    }
}
