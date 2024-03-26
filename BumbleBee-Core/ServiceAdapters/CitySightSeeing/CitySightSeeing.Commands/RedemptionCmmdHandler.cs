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
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ServiceAdapters.CitySightSeeing.CitySightSeeing.Commands
{
    class RedemptionCmmdHandler : CommandHandlerBase, IRedemptionCmmdHandler
    {
        public RedemptionCmmdHandler(ILogger log) : base(log)
        {

        }

        protected override object CssApiRequest<T>(T inputContext, string token = null)
        {
            try
            {


                var selectedProduct = inputContext as RedemptionRequest;
                AddIdempotentHeader(token);
                var redemptiondata = JsonConvert.SerializeObject(selectedProduct);
                var content = new StringContent(redemptiondata, Encoding.UTF8, "application/json"); // Set content type to application/json
                var result = HttpClient.PostAsync(FormUrlCreateBooking(selectedProduct), content);
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
        private string FormUrlCreateBooking(RedemptionRequest redemptionRequest)
        {
            return $"{Constant.redemptionBooking}";
        }
    }
}
