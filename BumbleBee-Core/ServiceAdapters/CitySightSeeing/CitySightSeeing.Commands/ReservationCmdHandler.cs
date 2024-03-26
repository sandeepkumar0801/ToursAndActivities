using Logger.Contract;
using ServiceAdapters.CitySightSeeing.CitySightSeeing.Commands.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util;
using ServiceAdapters.CitySightSeeing.Constants;
using ServiceAdapters.CitySightSeeing.CitySightSeeing.Entities;
using System.Net.Http;

namespace ServiceAdapters.CitySightSeeing.CitySightSeeing.Commands
{
    public class ReservationCmdHandler : CommandHandlerBase, IreservationCommandHandler
    {
        public ReservationCmdHandler(ILogger log) : base(log)
        {

        }

        protected override object CssApiRequest<T>(T inputContext, string token)
        {
            var reservationRequest = inputContext as ReservationRequest;
            AddIdempotentHeader(token);
            var createBooking = SerializeDeSerializeHelper.SerializeWithContractResolver(reservationRequest);
            var content = new StringContent(createBooking, Encoding.UTF8, "application/json"); // Set content type to application/json
            var result = HttpClient.PostAsync(FormUrlCreateBooking(reservationRequest), content);
            result.Wait();
            return result.Result;
        }
        private string FormUrlCreateBooking(ReservationRequest reservationRequest)
        {
            return $"{Constant.reserveBooking}";
        }
    }
}
