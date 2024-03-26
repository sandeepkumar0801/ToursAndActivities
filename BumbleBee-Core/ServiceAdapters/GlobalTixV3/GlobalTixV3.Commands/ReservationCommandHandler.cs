using Isango.Entities.Activities;
using Isango.Entities.Enums;
using ServiceAdapters.GlobalTixV3.GlobalTixV3.Commands.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Isango.Entities;
using ServiceAdapters.GlobalTixV3.Constants;
using Util;
using System.Text;
using Logger.Contract;
using ServiceAdapters.GlobalTixV3.GlobalTix.Entities;
using ServiceAdapters.GlobalTixV3.GlobalTixV3.Entities;
using Isango.Entities.Canocalization;
using System.Globalization;

namespace ServiceAdapters.GlobalTixV3.GlobalTixV3.Commands
{
    public class ReservationCommandHandler : CommandHandlerBase, IReservationCommandHandler
    {
        private string _supportEmail = "support@isango.com";

        public ReservationCommandHandler(ILogger iLog) : base(iLog)
        {
            _supportEmail = ConfigurationManagerHelper.GetValuefromAppSettings("mailfrom");
        }

        protected override object CreateInputRequest(InputContext inputContext)
        {
            var bookCtx = inputContext as BookInputContextV3;
            if (bookCtx == null)
            {
                return null;
            }

            var bookRQ = new ReservationRQ() { TicketTypes = new List<TickettypeReservation>() };
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

                        var bookTicketType = new TickettypeReservation()
						{
							Id = int.Parse(ticketId),
							Index = index++,
							Quantity = psgrTypePaxCount.Value,
							VisitDate = actOpt.TravelInfo.StartDate.ToString("yyyy-MM-dd"),
                            QuestionList = GetContractQuestions(actOpt.ContractQuestions,((CanocalizationSelectedProduct)selProduct)?.ContractQuestions, actOpt?.HotelPickUpLocation)
                        };

                        int? evtId = GetActivityEventId(actOpt.SellPrice?.DatePriceAndAvailabilty, actOpt.TravelInfo.StartDate);
                        if (evtId != null)
                        {
                            bookTicketType.Event_id = evtId;
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
            bookRQ.OtherInfo = new Otherinfo();
            bookRQ.OtherInfo.PartnerReference = bookCtx.BookingReferenceNumber;
            return bookRQ;
        }

        protected override object GetResults(object input, string authString,bool isNonThailandProduct)
        {
            var bookRQ = input as ReservationRQ;
            if (bookRQ == null)
            {
                return null;
            }

            AsyncClient client = GetServiceClient(Constant.URL_Reservation);
            return client.ConsumePostServiceHttpResponse(GetHttpRequestHeaders(isNonThailandProduct), Constant.HttpHeader_ContentType_JSON, SerializeDeSerializeHelper.Serialize(bookRQ), Encoding.UTF8);
        }

        protected override async Task<object> GetResultsAsync(object input, string authString,bool isNonThailandProduct)
        {
            if (input != null)
            {
                var client = new AsyncClient
                {
                    ServiceURL = $"{ConfigurationManagerHelper.GetValuefromAppSettings(Constant.CfgParam_GlobalTixBaseUrl)}{Constant.URL_Reservation}"
                };
                return await client.PostJsonAsync<ReservationRQ>(authString, (ReservationRQ)input);
            }
            return null;
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
                    if (!mapperPassengerTypeToTicketIds.ContainsKey(psgrType))
                    {
                        mapperPassengerTypeToTicketIds.Add(psgrType, psgrTypeToTktIds[i + 1]);
                    }
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
        private List<Questionlist> GetContractQuestions(List<ContractQuestion> apiContractQuestions, List<ContractQuestion> userSelectedContractQuestions, string userSelectedPickupLocation)
        {
            var bookedQuestions = new List<Questionlist>();
            if (apiContractQuestions != null && apiContractQuestions.Count() > 0)
            {
                foreach (var item in apiContractQuestions)
                {
                    if (String.IsNullOrEmpty(item.Answer))
                    {
                        var userSelectedAnswer = userSelectedContractQuestions?.Where(x => x.Name?.Trim()?.ToLowerInvariant() == item?.Description?.Trim()?.ToLowerInvariant())?.SingleOrDefault();
                        if (userSelectedAnswer != null)
                        {
                            var bookedQuestion = new Questionlist
                            {
                                Id = Convert.ToInt32(item?.Code),
                                Answer = userSelectedAnswer?.Answer?.Trim()
                            };

                            //check string is date
                            DateTime temp;
                            if (DateTime.TryParse(item.Answer, out temp))
                            {
                                // format 1984-10-31
                                bookedQuestion.Answer = temp.ToString("yyyy-MM-dd");
                            }

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
                                var bookedQuestion = new Questionlist
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
            else
            {
                if (userSelectedContractQuestions != null && userSelectedContractQuestions.Count() > 0)
                {
                    foreach (var item in userSelectedContractQuestions)
                    {
                        var bookedQuestion = new Questionlist
                        {
                            Id = Convert.ToInt32(item?.Code),
                            Answer = item?.Answer?.Trim()
                        };
                        bookedQuestions.Add(bookedQuestion);
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
