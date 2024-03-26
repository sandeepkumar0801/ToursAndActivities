using Logger.Contract;
using ServiceAdapters.PrioTicket.Constants;
using ServiceAdapters.PrioTicket.PrioTicket.Commands.Contracts;
using ServiceAdapters.PrioTicket.PrioTicket.Entities;
using ServiceAdapters.PrioTicket.PrioTicket.Entities.Request;
using System;
using System.Threading.Tasks;
using Util;
using PrioResponse = ServiceAdapters.PrioTicket.PrioTicket.Entities.Response;

namespace ServiceAdapters.PrioTicket.PrioTicket.Commands
{
    public class CreateBookingCmdHandler : CommandHandlerBase, ICreateBookingCmdHandler
    {
        private readonly AsyncClient _asyncClient;
        private readonly CreateBookingRq _createBookingRq;

        public CreateBookingCmdHandler(ILogger iLog) : base(iLog)
        {
            _asyncClient = new AsyncClient();
            _createBookingRq = new CreateBookingRq();
        }

        // Command Handler are use to Create Input Request , Call Api and GetResult and convert it into the DTO in Entities
        /// <summary>
        /// Create Input Request
        /// </summary>
        /// <param name="inputContext"></param>
        /// <returns></returns>
        protected override object CreateInputRequest(InputContext inputContext)
        {
            _createBookingRq.Data = new PrioCreateBookingData
            {
                BookingType = new BookingType()
            };

            #region Notes

            if (inputContext?.Notes.Count > 0 && !string.IsNullOrWhiteSpace(inputContext.Notes[0]))
            {
                _createBookingRq.Data.Notes = new string[1];
                _createBookingRq.Data.Notes[0] = inputContext.Notes[0];
            }

            #endregion Notes

            _createBookingRq.Data.Contact = new Contact()
            {
                Address = new Address()
            };

            _createBookingRq.RequestType = Constant.Booking;
            _createBookingRq.Data.DistributorId = Convert.ToString(inputContext?.UserName);
            _createBookingRq.Data.BookingType.TicketId = Convert.ToString(inputContext?.ActivityId);
            if ((inputContext?.PrioTicketClass == (int)TicketClass.TicketClassTwo) || (inputContext?.PrioTicketClass == (int)TicketClass.TicketClassThree))
            {
                _createBookingRq.Data.BookingType.ReservationReference = inputContext.ReservationReference;
            }
            else
            {
                var count = inputContext?.TicketType.Count ?? 0;
                _createBookingRq.Data.BookingType.BookingDetails = new BookingDetails[count];

                for (int i = 0; i <= count - 1; i++)
                {
                    _createBookingRq.Data.BookingType.BookingDetails[i] = new BookingDetails
                    {
                        TicketType = inputContext?.TicketType[i],
                        Count = inputContext?.Count[i] ?? 0
                    };
                }
            }

            _createBookingRq.Data.BookingName = inputContext?.BookingName;
            _createBookingRq.Data.BookingEmail = inputContext?.BookingEmail;

            _createBookingRq.Data.Contact.Address.Street = inputContext?.Street;
            _createBookingRq.Data.Contact.Address.PostalCode = inputContext?.PostalCode;

            _createBookingRq.Data.Contact.Address.City = inputContext?.City;
            _createBookingRq.Data.Contact.Phonenumber = inputContext?.PhoneNumber;

            _createBookingRq.Data.ProductLanguage = Constant.En;
            _createBookingRq.Data.DistributorReference = inputContext?.DistributorReference + "_" + Convert.ToString(inputContext?.ActivityId);

            var jsonRequest = SerializeDeSerializeHelper.Serialize(_createBookingRq);
            return jsonRequest;
        }

        /// <summary>
        /// Get Json Results
        /// </summary>
        /// <param name="jsonObject"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        protected override object GetJsonResults(object inputJson, string token)
        {
            return _asyncClient.PostPrioJsonAsync(inputJson, token);
        }

        /// <summary>
        /// Get Results
        /// </summary>
        /// <param name="jsonResult"></param>
        /// <returns></returns>
        protected override object GetResults(object jsonResult)
        {
            return SerializeDeSerializeHelper.DeSerialize<PrioResponse.CreateBookingRs>(jsonResult.ToString());
        }

        protected override Task<object> GetJsonResultsV2(object inputJson, string token)
        {
            return _asyncClient.PostPrioJsonAsyncV2(inputJson, token);
        }
    }
}