using Logger.Contract;
using ServiceAdapters.Bokun.Bokun.Commands.Contracts;
using ServiceAdapters.Bokun.Bokun.Entities;
using ServiceAdapters.Bokun.Bokun.Entities.CancelBooking;
using ServiceAdapters.Bokun.Constants;
using System.Text;
using Util;

namespace ServiceAdapters.Bokun.Bokun.Commands
{
    public class CancelBookingCommandHandler : CommandHandlerBase, ICancelBookingCommandHandler
    {
        private bool _isNotificationOn = false;
        private bool _isSendNotificationToCustomer = false;

        // ReSharper disable once NotAccessedField.Local
        private string _notificationEmailAddressIsango;

        public CancelBookingCommandHandler(ILogger log) : base(log)
        {
            try
            {
                var apiConfig = BokunAPIConfig.GetInstance();
                _isNotificationOn = apiConfig.IsNotificationOn;
                if (_isNotificationOn)
                {
                    _isSendNotificationToCustomer = apiConfig.IsSendNotificationToCustomer;
                    _notificationEmailAddressIsango = apiConfig.NotificationEmailAddressIsango;

                    if (_isSendNotificationToCustomer)
                    {
                        _notificationEmailAddressIsango = string.Empty;
                    }
                }
            }
            catch (Exception)
            {
                _isNotificationOn = false;
            }
        }

        /// <summary>
        /// Return request required to hit API
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputContext"></param>
        /// <returns></returns>
        protected override object CreateInputRequest<T>(T inputContext)
        {
            var input = Convert.ToString(inputContext);
            if (!string.IsNullOrWhiteSpace(input))
            {
                var requestObject = new CancelBookingRq()
                {
                    BookingConfirmationCode = input,
                    Refund = true,
                    Notify = _isNotificationOn
                };
                return requestObject; 
            }
            return null;
        }

        /// <summary>
        /// Call API to Cancel Booking from API
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputContext"></param>
        /// <returns></returns>
        protected override object BokunApiRequest<T>(T inputContext)
        {
            var input = inputContext as CancelBookingRq;
            var methodPath = GenerateMethodPath(input);
            var httpClient = AddRequestHeadersAndAddressToApi(Constant.Post, methodPath);

            var orderCreate = SerializeDeSerializeHelper.Serialize(input);
            var content = new StringContent(orderCreate, Encoding.UTF32, Constant.ApplicationOrJasonMediaType);

            var result = httpClient.PostAsync(methodPath, content);
            result.Wait();
            return ValidateApiResponse(result.Result);
        }

        /// <summary>
        /// Call API to Cancel Booking from API asynchronously
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputContext"></param>
        /// <returns></returns>
        protected override async Task<object> BokunApiRequestAsync<T>(T inputContext)
        {
            var input = inputContext as CancelBookingRq;
            var methodPath = GenerateMethodPath(input);
            var httpClient = AddRequestHeadersAndAddressToApi(Constant.Post, methodPath);

            var orderCreate = SerializeDeSerializeHelper.Serialize(input);
            var content = new StringContent(orderCreate, Encoding.UTF32, Constant.ApplicationOrJasonMediaType);

            var result = await httpClient.PostAsync(methodPath, content);
            return ValidateApiResponse(result);
        }

        private string GenerateMethodPath(CancelBookingRq input)
        {
            return string.Format(UriConstants.CanceBooking, input.BookingConfirmationCode);
        }
    }
}