using Logger.Contract;
using ServiceAdapters.CitySightSeeing.CitySightSeeing.Commands.Contracts;
using ServiceAdapters.CitySightSeeing.CitySightSeeing.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Util;
using ServiceAdapters.CitySightSeeing.Constants;

namespace ServiceAdapters.CitySightSeeing.CitySightSeeing.Commands
{
    public class CancellationCommandHandler : CommandHandlerBase, ICancellationCommandHandler
    {
        public CancellationCommandHandler(ILogger log) : base(log)
        {

        }

        protected override object CssApiRequest<T>(T inputContext, string token)
        {
            try
            {
                var cancellerProduct = inputContext as CancellationRequest;
                AddIdempotentHeader(token);
                var cancelbooking = SerializeDeSerializeHelper.SerializeWithContractResolver(cancellerProduct);
                var content = new StringContent(cancelbooking, Encoding.UTF8, "application/json"); // Set content type to application/json
                var result = HttpClient.PostAsync(FormUrlCreateBooking(cancellerProduct), content);
                result.Wait();
                var statusCode = (int)result.Result.StatusCode;
                return statusCode;
            }
            catch (Exception ex)
            {
                // Handle exceptions that might occur during the request
                return ex.Message; // You should log the exception for debugging
            }
        }

        private string FormUrlCreateBooking(CancellationRequest cancelledrequest)
        {
            return $"{Constant.cancelBooking}";
        }
    }
}
