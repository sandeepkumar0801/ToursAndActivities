using Isango.Entities.Bokun;
using Logger.Contract;
using ServiceAdapters.Bokun.Bokun.Commands.Contracts;
using ServiceAdapters.Bokun.Bokun.Entities;
using ServiceAdapters.Bokun.Bokun.Entities.CheckoutOptions;
using ServiceAdapters.Bokun.Constants;
using System.Text;
using Util;

namespace ServiceAdapters.Bokun.Bokun.Commands
{
    public class CheckoutOptionsCommandHandler : CommandHandlerBase, ICheckoutOptionsCommandHandler
    {
        public CheckoutOptionsCommandHandler(ILogger log) : base(log)
        {
        }

        /// <summary>
        /// Return request required to hit API
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputContext"></param>
        /// <returns></returns>
        protected override object CreateInputRequest<T>(T inputContext)
        {
            CheckoutOptionsRq request = null;
            var input = inputContext as BokunSelectedProduct;
            if (input != null)
            {
                var activityBookings = new List<Activitybooking>();
                var passengers = input.PricingCategoryIds?.Select(pricingCategoryId => new PassengerDto()
                {
                    PricingCategoryId = pricingCategoryId
                }).ToList();

                var pickupIdPartintDesc = input?.HotelPickUpLocation?.Split('-')?.FirstOrDefault();
                int? pickupId = Convert.ToInt32(pickupIdPartintDesc);
                pickupId = pickupId > 0 ? pickupId : null;
                var isPickup = pickupId > 0;

                var dropoffIdPartintDesc = input?.HotelDropoffLocation?.Split('-')?.FirstOrDefault();
                int? dropoffId = Convert.ToInt32(dropoffIdPartintDesc);
                dropoffId = dropoffId > 0 ? dropoffId : null;
                var isDropoff = dropoffId > 0;

                var activityBooking = new Activitybooking
                {
                    ActivityId = input.Id,
                    Date = input.DateStart.ToString(Constant.DateInyyyyMMdd),
                    StartTimeId = input.StartTimeId,
                    Passengers = passengers,
                    Pickup = isPickup,
                    PickupPlaceId = pickupId,
                    PickupDescription = input?.HotelPickUpLocation,
                    RateId = Convert.ToInt32(input?.RateId),

                    Dropoff = isDropoff,
                    DropOffPlaceId = (isDropoff == true ? dropoffId:null),
                    DropoffDescription = (isDropoff==true?(input?.HotelDropoffLocation):null),
                };
                activityBookings.Add(activityBooking);

                request = new CheckoutOptionsRq
                {
                    ActivityBookings = activityBookings,
                    CurrencyISOCode = input.SupplierCurrency,
                    SendCustomerNotification = false
                };
            }
            return request;
        }

        /// <summary>
        /// Call API to get the Checkout Options from API
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputContext"></param>
        /// <returns></returns>
        protected override object BokunApiRequest<T>(T inputContext)
        {
            var input = inputContext as CheckoutOptionsRq;
            var methodPath = GenerateMethodPath();
            var httpClient = AddRequestHeadersAndAddressToApi(Constant.Post, methodPath);

            var orderCreate = SerializeDeSerializeHelper.Serialize(input);
            var content = new StringContent(orderCreate, Encoding.UTF32, Constant.ApplicationOrJasonMediaType);

            var result = httpClient.PostAsync(methodPath, content);
            result.Wait();
            return ValidateApiResponse(result.Result);
        }

        /// <summary>
        /// Call API to get the Checkout Options from API asynchronously
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputContext"></param>
        /// <returns></returns>
        protected override async Task<object> BokunApiRequestAsync<T>(T inputContext)
        {
            var input = inputContext as CheckoutOptionsRq;
            var methodPath = GenerateMethodPath();
            var httpClient = AddRequestHeadersAndAddressToApi(Constant.Post, methodPath);

            var orderCreate = SerializeDeSerializeHelper.Serialize(input);
            var content = new StringContent(orderCreate, Encoding.UTF32, Constant.ApplicationOrJasonMediaType);

            var result = await httpClient.PostAsync(methodPath, content);
            return ValidateApiResponse(result);
        }

        private readonly Func<string> GenerateMethodPath = () => UriConstants.CheckoutOptions;
    }
}