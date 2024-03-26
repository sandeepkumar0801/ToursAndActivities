//using System.Web.Http;
//using Isango.Entities;
//using Isango.Entities.Enums;
//using Isango.Entities.GoogleMaps.BookingServer;
//using WebAPI.Filters;
//using WebAPI.Helper;
//using WebAPI.Mapper;
//using WebAPI.Models.GoogleMapsBookingServer;

//namespace WebAPI.Controllers
//{
//    //[BasicAuthenticationFilter]
//    public class GoogleMapsController : ApiBaseController
//    {
//        /* No longer used
//        private readonly GoogleMapsHelper _googleMapsHelper;
//        private readonly GoogleMapsMapper _googleMapsMapper;
//        private readonly BookingHelper _bookingHelper;

//        public GoogleMapsController(GoogleMapsHelper googleMapsHelper, GoogleMapsMapper googleMapsMapper, BookingHelper bookingHelper)
//        {
//            _googleMapsHelper = googleMapsHelper;
//            _googleMapsMapper = googleMapsMapper;
//            _bookingHelper = bookingHelper;
//        }

//        [Route("v3/HealthCheck")]
//        [HttpGet]
//        public IHttpActionResult HealthCheck()
//        {
//            return Ok();
//        }

//        [CustomActionWebApiFilter]
//        [Route("v3/CheckOrderFulfillability")]
//        [HttpPost]
//        public IHttpActionResult CheckOrderFulfillability(CheckOrderFulfillabilityRequest requestData)
//        {
//            var fulfillabilityResponse = _googleMapsHelper.CheckOrderFulfillability(requestData);
//            return Ok(fulfillabilityResponse);
//        }

//        [CustomActionWebApiFilter]
//        [Route("v3/CreateOrder")]
//        [HttpPost]
//        public IHttpActionResult CreateOrder(CreateOrderRequest createOrderRequest)
//        {
//            var tokenId = string.Empty;
//            var bookingResult = new BookingResult
//            {
//                BookingStatus = BookingStatus.Failed
//            };

//            // Call the CheckOrderFulfillability method
//            var checkOrderFulfillabilityRequest = new CheckOrderFulfillabilityRequest
//            {
//                LineItems = createOrderRequest.Order.Items
//            };

//            var checkOrderFulfillabilityResponse = _googleMapsHelper.CheckOrderFulfillability(checkOrderFulfillabilityRequest);

//            // Return if OrderFulfillabilityResult is not equals CAN_FULFILL
//            if (checkOrderFulfillabilityResponse.OrderFulfillability.OrderFulfillabilityResult ==
//                OrderFulfillabilityResult.CAN_FULFILL)
//            {
//                // CreateBooking API call
//                var bookingRequest =
//                    _googleMapsMapper.PrepareCreateBookingRequest(createOrderRequest, checkOrderFulfillabilityResponse);
//                var result = _bookingHelper.CreateBooking(bookingRequest);
//                bookingResult = result.Item1;
//                bookingResult.BookingRefNo = result.Item2.ReferenceNumber;
//                tokenId = bookingRequest.TokenId;
//            }

//            // Prepare CreateOrder response on the basis of the Booking Status
//            var createOrderResponse = _googleMapsMapper.PrepareCreateOrderResponse(checkOrderFulfillabilityResponse, bookingResult, createOrderRequest.Order);

//            // Store the Order Response data in the storage
//            if (createOrderResponse.Order != null)
//                _googleMapsHelper.InsertOrderResponse(createOrderResponse.Order, tokenId);

//            return GetResponseWithHttpActionResult(createOrderResponse);
//        }

//        [CustomActionWebApiFilter]
//        [Route("v3/ListOrders")]
//        [HttpPost]
//        public IHttpActionResult ListOrders(ListOrdersRequest request)
//        {
//            var userId = request.UserId;
//            var orderIds = request.OrderIds.OrderId;
//            var orders = _googleMapsHelper.GetOrders(userId, orderIds);
//            return GetResponseWithHttpActionResult(orders);
//        }
//        */
//    }
//}