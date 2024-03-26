using Logger.Contract;
using ServiceAdapters.HB.Constants;
using ServiceAdapters.HB.HB.Commands.Contract;
using ServiceAdapters.HB.HB.Entities;
using ServiceAdapters.HB.HB.Entities.Cancellation;
using System;
using System.Threading.Tasks;
using Util;
using Booking = ServiceAdapters.HB.HB.Entities.Booking;

namespace ServiceAdapters.HB.HB.Commands
{
    public class HBBookingCancelSimulationCmdHandler : CommandHandlerBase, IHbBookingCancelSimulationCmdHandler
    {
        public HBBookingCancelSimulationCmdHandler(ILogger iLog) : base(iLog)
        {
        }

        protected override object CreateInputRequest<T>(T hotelbedsApitudeCriteria)
        {
            var inputContext = hotelbedsApitudeCriteria as InputContext;
            var getBookingDetailRq = new Booking.BookingRq
            {
                Language = inputContext.Language,
                CustomerRefrerence = inputContext.ClientReference
            };
            return getBookingDetailRq;
        }

        //protected override object GetResponseObject(string responseText)
        //{
        //    throw new NotImplementedException();
        //}
        protected override object GetResponseObject(string responseText)
        {
            var result = default(CancellationRS);
            try
            {
                result = SerializeDeSerializeHelper.DeSerialize<CancellationRS>(responseText);
            }
            catch (Exception ex)
            {
                //ignored
                //#TODO Add logging here
                result = null;
            }
            return result;
        }

        protected override async Task<object> GetResultsAsync(object input)
        {
            var url = string.Format($"{_hotelBedsApitudeServiceURL}");
            var getBookingDetailRq = (Booking.BookingRq)input;

            if (!string.IsNullOrWhiteSpace(getBookingDetailRq.Language))
                url = $"{url}{Constant.BookingConfirm}{Constant.Slash}{getBookingDetailRq.Language}";

            if (!string.IsNullOrWhiteSpace(getBookingDetailRq.CustomerRefrerence))
                url = $"{url}{Constant.Slash}{getBookingDetailRq.CustomerRefrerence}";

            var paramflag = Constant.SimulationCancellationFlagValue;
            url = url + "?cancellationFlag=" + paramflag;
            var response = await GetResponseFromAPIEndPoint(input, url, "DELETE");
            return response;
        }
    }
}