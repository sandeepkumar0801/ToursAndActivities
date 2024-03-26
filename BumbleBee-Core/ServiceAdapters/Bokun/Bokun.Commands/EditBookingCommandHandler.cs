using Logger.Contract;
using ServiceAdapters.Bokun.Bokun.Commands.Contracts;
using ServiceAdapters.Bokun.Bokun.Entities;
using ServiceAdapters.Bokun.Bokun.Entities.EditBooking;
using ServiceAdapters.Bokun.Constants;
using System.Text;
using Util;

namespace ServiceAdapters.Bokun.Bokun.Commands
{
    public class EditBookingCommandHandler : CommandHandlerBase, IEditBookingCommandHandler
    {
        public EditBookingCommandHandler(ILogger log) : base(log)
        {
        }

        /// <summary>
        /// Call API to Edit Booking
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputContext"></param>
        /// <returns></returns>
        protected override object BokunApiRequest<T>(T inputContext)
        {
            var input = inputContext as EditBookingRq;
            var methodPath = GenerateMethodPath();
            var httpClient = AddRequestHeadersAndAddressToApi(Constant.Post, methodPath);

            var orderCreate = SerializeDeSerializeHelper.Serialize(input);
            var content = new StringContent(orderCreate, Encoding.UTF32, Constant.ApplicationOrJasonMediaType);

            var result = httpClient.PostAsync(methodPath, content);
            result.Wait();
            return ValidateApiResponse(result.Result);
        }

        /// <summary>
        /// Call API to Edit Booking asynchronously
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputContext"></param>
        /// <returns></returns>
        protected override async Task<object> BokunApiRequestAsync<T>(T inputContext)
        {
            var methodPath = GenerateMethodPath();
            var httpClient = AddRequestHeadersAndAddressToApi(Constant.Post, methodPath);

            var orderCreate = SerializeDeSerializeHelper.Serialize(inputContext as EditBookingRq);
            var content = new StringContent(orderCreate, Encoding.UTF32, Constant.ApplicationOrJasonMediaType);

            var result = await httpClient.PostAsync(methodPath, content);
            return ValidateApiResponse(result);
        }

        protected override object CreateInputRequest<T>(T inputContext)
        {
            return inputContext;
        }

        private readonly Func<string> GenerateMethodPath = () => UriConstants.EditBooking;
    }
}