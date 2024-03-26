using Logger.Contract;
using ServiceAdapters.CitySightSeeing.CitySightSeeing.Commands.Contracts;
using ServiceAdapters.CitySightSeeing.CitySightSeeing.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util;
using ServiceAdapters.CitySightSeeing.Constants;
using System.Net.Http;

namespace ServiceAdapters.CitySightSeeing.CitySightSeeing.Commands
{
    public class CreateBookingCommandHandler : CommandHandlerBase, ICreateBookingCommandHandler
    {
        public CreateBookingCommandHandler(ILogger log) : base(log)
        {

        }

        protected override object CssApiRequest<T>(T inputContext, string token = null)
        {
            var selectedProduct = inputContext as CreateBookingRequest;
            AddIdempotentHeader(token);
            var createbooking = SerializeDeSerializeHelper.Serialize(selectedProduct);
            var content = new StringContent(createbooking, Encoding.UTF8, "application/json"); // Set content type to application/json
            var result = HttpClient.PostAsync(FormUrlCreateBooking(selectedProduct), content);
            result.Wait();
            return result.Result;
        }
        private string FormUrlCreateBooking(CreateBookingRequest cancellationRequest)
        {
            return $"{Constant.CreateBooking}";
        }

    }
}
