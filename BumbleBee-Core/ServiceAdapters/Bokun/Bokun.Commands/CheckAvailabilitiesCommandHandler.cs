using Logger.Contract;
using ServiceAdapters.Bokun.Bokun.Commands.Contracts;
using ServiceAdapters.Bokun.Bokun.Entities;
using ServiceAdapters.Bokun.Bokun.Entities.CheckAvailabilities;
using ServiceAdapters.Bokun.Constants;
using System;
using System.Threading.Tasks;

namespace ServiceAdapters.Bokun.Bokun.Commands
{
    public class CheckAvailabilitiesCommandHandler : CommandHandlerBase, ICheckAvailabilitiesCommandHandler
    {
        public CheckAvailabilitiesCommandHandler(ILogger log) : base(log)
        {
        }

        /// <summary>
        /// Call API to check activity availabilites data from API
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputContext"></param>
        /// <returns></returns>
        protected override object BokunApiRequest<T>(T inputContext)
        {
            var input = inputContext as CheckAvailabilitiesRq;
            var methodPath = GenerateMethodPath(input);
            var httpClient = AddRequestHeadersAndAddressToApi(Constant.Get, methodPath);
            var result = httpClient.GetAsync(methodPath)?.GetAwaiter().GetResult();
            //try
            //{
            //    result.Wait();
            //}
            //catch(Exception ex)
            //{
            //    var isangoErrorEntity = new Isango.Entities.IsangoErrorEntity
            //    {
            //        ClassName = "CheckAvailabilitiesCommandHandler",
            //        MethodName = "BokunApiRequest",
            //        Params = Util.SerializeDeSerializeHelper.Serialize(inputContext)
            //    };
            //    _log.Error(isangoErrorEntity, ex);
            //    //timeout probably - check logs;
            //    return null;
            //}
            return ValidateApiResponse(result);
        }

        /// <summary>
        /// Call API to check activity availabilites data from API asynchronously
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputContext"></param>
        /// <returns></returns>
        protected override async Task<object> BokunApiRequestAsync<T>(T inputContext)
        {
            var input = inputContext as CheckAvailabilitiesRq;
            var methodPath = GenerateMethodPath(input);
            var httpClient = AddRequestHeadersAndAddressToApi(Constant.Get, methodPath);
            var result = await httpClient.GetAsync(methodPath);
            return ValidateApiResponse(result);
        }

        protected override object CreateInputRequest<T>(T inputContext)
        {
            return inputContext;
        }

        private string GenerateMethodPath(CheckAvailabilitiesRq input)
        {
            return string.Format(UriConstants.CheckAvailabilities, input.ActivityId, input.StartDate, input.EndDate, input.CurrencyIsoCode);
        }
    }
}