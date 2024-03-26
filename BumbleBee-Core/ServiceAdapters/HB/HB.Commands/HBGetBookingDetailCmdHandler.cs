using Logger.Contract;
using ServiceAdapters.HB.Constants;
using ServiceAdapters.HB.HB.Commands.Contract;
using ServiceAdapters.HB.HB.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Util;
using Booking = ServiceAdapters.HB.HB.Entities.Booking;

namespace ServiceAdapters.HB.HB.Commands
{
    public class HBGetBookingDetailCmdHandler : CommandHandlerBase, IHbGetBookingDetailCmdHandler
    {
        public HBGetBookingDetailCmdHandler(ILogger iLog) : base(iLog)
        {
        }

        protected override object CreateInputRequest<T>(T hotelbedsApitudeCriteria)
        {
            var inputContext = hotelbedsApitudeCriteria as InputContext;
            var getBookingDetailRq = new Booking.BookingRq
            {
                Language = inputContext.Language,
                BookingReference = inputContext.BookingReference,
                CustomerRefrerence = inputContext.ClientReference,
                HolderName = inputContext.Holder.Name,
                HolderSurname = inputContext.Holder.Surname,
                StartDate = inputContext.FromDate,
                EndDate = inputContext.ToDate
            };
            return getBookingDetailRq;
        }

        protected override object GetResponseObject(string responseText)
        {
            throw new NotImplementedException();
        }

        protected override async Task<object> GetResultsAsync(object input)
        {
            if (input != null)
            {
                var client = new AsyncClient
                {
                    ServiceURL =
                        Convert.ToString(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.EndpointBooking))
                };
                var getBookingDetailRq = (Booking.BookingRq)input;

                if (!string.IsNullOrWhiteSpace(getBookingDetailRq.Language))
                    client.ServiceURL = client.ServiceURL + Constant.Slash + getBookingDetailRq.Language;

                if (!string.IsNullOrWhiteSpace(getBookingDetailRq.BookingReference))
                {
                    client.ServiceURL = client.ServiceURL + Constant.Slash + getBookingDetailRq.BookingReference;
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(getBookingDetailRq.CustomerRefrerence))
                        client.ServiceURL = $"{client.ServiceURL}{Constant.Slash}{getBookingDetailRq.CustomerRefrerence}";
                    if (!string.IsNullOrWhiteSpace(getBookingDetailRq.HolderName))
                        client.ServiceURL = $"{client.ServiceURL}{Constant.Slash}{getBookingDetailRq.HolderName}";
                    if (!string.IsNullOrWhiteSpace(getBookingDetailRq.HolderSurname))
                        client.ServiceURL = $"{client.ServiceURL}{Constant.Slash}{getBookingDetailRq.HolderSurname}";
                    if (!string.IsNullOrWhiteSpace(getBookingDetailRq.StartDate))
                        client.ServiceURL = $"{client.ServiceURL}{Constant.Slash}{getBookingDetailRq.StartDate}";
                    if (!string.IsNullOrWhiteSpace(getBookingDetailRq.EndDate))
                        client.ServiceURL = $"{client.ServiceURL}{Constant.Slash}{getBookingDetailRq.EndDate}";
                }
                var headers = new Dictionary<string, string>
                {
                    {"Accept", "application/json"},
                    {"X-Originating-Ip", ExternalIpAddress()},
                    {"Content-Type", "application/json"}
                };
                return await client.GetJsonAsync(headers);
            }
            return null;
        }
    }
}