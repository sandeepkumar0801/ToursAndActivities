using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Rezdy;
using ServiceAdapters.Rezdy.Rezdy.Commands.Contracts;
using ServiceAdapters.Rezdy.Rezdy.Converters.Contracts;
using ServiceAdapters.Rezdy.Rezdy.Entities;
using ServiceAdapters.Rezdy.Rezdy.Entities.Availability;
using ServiceAdapters.Rezdy.Rezdy.Entities.CancelBooking;
using ServiceAdapters.Rezdy.Rezdy.Entities.PickUpLocation;
using ServiceAdapters.Rezdy.Rezdy.Entities.ProductDetails;
using Util;

namespace ServiceAdapters.Rezdy
{
    public class RezdyAdapter : IRezdyAdapter, IAdapter
    {
        #region Properties

        private readonly IGetProductCommandHandler _getProductCommandHandler;
        private readonly IGetAvailabilityCommandHandler _getAvailabilityCommandHandler;
        private readonly ICreateBookingCommandHandler _createBookingCommandHandler;
        private readonly ICancelBookingCommandHandler _cancelBookingCommandHandler;
        private readonly IGetAllProductsCommandHandler _getAllProductsCommandHandler;
        private readonly IGetPickUpLocationCommandHandler _getPickUpLocationCommandHandler;

        private readonly IGetProductConverter _getProductConverter;
        private readonly IGetAvailabilityConverter _getAvailabilityConverter;
        private readonly ICreateBookingConverter _createBookingConverter;
        private readonly ICancelBookingConverter _cancelBookingConverter;
        private readonly IGetAllProductsConverter _getAllProductsConverter;
        private readonly IGetPickUpLocationConverter _getPickUpLocationConverter;

        #endregion Properties

        #region Constructor

        public RezdyAdapter(
           IGetProductCommandHandler getProductCommandHandler,
           IGetAvailabilityCommandHandler getAvailabilityCommandHandler,
           ICreateBookingCommandHandler createBookingCommandHandler,
           ICancelBookingCommandHandler cancelBookingCommandHandler,
           IGetAllProductsCommandHandler getAllProductsCommandHandler,
           IGetPickUpLocationCommandHandler getPickUpLocationCommandHandler,

           IGetProductConverter getProductConverter,
           IGetAvailabilityConverter getAvailabilityConverter,
           ICreateBookingConverter createBookingConverter,
           ICancelBookingConverter cancelBookingConverter,
           IGetAllProductsConverter getAllProductsConverter,
           IGetPickUpLocationConverter getPickUpLocationConverter
           )
        {
            _getProductCommandHandler = getProductCommandHandler;
            _getAvailabilityCommandHandler = getAvailabilityCommandHandler;
            _createBookingCommandHandler = createBookingCommandHandler;
            _cancelBookingCommandHandler = cancelBookingCommandHandler;
            _getAllProductsCommandHandler = getAllProductsCommandHandler;
            _getPickUpLocationCommandHandler = getPickUpLocationCommandHandler;

            _getProductConverter = getProductConverter;
            _getAvailabilityConverter = getAvailabilityConverter;
            _createBookingConverter = createBookingConverter;
            _cancelBookingConverter = cancelBookingConverter;
            _getAllProductsConverter = getAllProductsConverter;
            _getPickUpLocationConverter = getPickUpLocationConverter;
        }

        #endregion Constructor

        public async Task<List<RezdyProduct>> GetAllRezdyProducts(int supplierId, string supplierAlias, string token)
        {
            var totalLimit = 20000;
            var fixedRecordsAtOneTime = 100;
            var rezdyProducts = new List<RezdyProduct>();
            //Loop of 100 records at a time
            //Paging Concept here
            for (int index = 0; index < totalLimit; index = index + fixedRecordsAtOneTime)
            {
                var getProiductRq = new GetProductReqeust
                {
                    SupplierId = supplierId,
                    SupplierAlias = supplierAlias,
                    Limit = fixedRecordsAtOneTime,
                    Offset = index
                };
                try
                {
                    var response = await _getAllProductsCommandHandler.ExecuteAsync(getProiductRq, MethodType.GetAllProducts, token);
                    var rezdyProduct = _getAllProductsConverter.Convert(response.ToString());
                    if (rezdyProduct == null || ((List<RezdyProduct>)rezdyProduct).Count == 0)
                    {
                        break;
                    }
                    rezdyProducts.AddRange((List<RezdyProduct>)rezdyProduct);
                }
                catch (Exception ex)
                {
                    continue;
                }
            }
            return rezdyProducts as List<RezdyProduct>;
        }

        public RezdyProduct GetProductDetails(string productCode, string token)
        {
            var getProductRq = new GetProductReqeust
            {
                ProductCode = productCode
            };
            var response = _getProductCommandHandler.Execute(getProductRq, MethodType.GetAllProducts, token);
            if (response == null) return new RezdyProduct();
            var rezdyProducts = _getProductConverter.Convert(response.ToString());
            return rezdyProducts as RezdyProduct;
        }

        public async Task<List<ProductOption>> GetAvailability(RezdyCriteria criteria, string token)
        {
            var productOptions = new List<ProductOption>();
            foreach (var productCode in criteria.ProductCodes)
            {
                var rezdyProduct = GetProductDetails(productCode, token);
                if (rezdyProduct == null)
                    continue;

                criteria.ProductName = rezdyProduct.Name;
                criteria.PassengerMappings = new List<RezdyPassengerMapping>();

                foreach (var priceOption in rezdyProduct.PriceOptions)
                {
                    var passengerInfo = criteria.RezdyPaxMappings?.FirstOrDefault(x => x.AgeGroupCode.ToLowerInvariant().Equals(priceOption.Label.ToLowerInvariant())
            && x.SupplierCode.ToLowerInvariant().Equals(priceOption.ProductCode.ToLowerInvariant()));
                    if (passengerInfo != null)
                    {
                        criteria.PassengerMappings.Add(
                            new RezdyPassengerMapping()
                            {
                                AgeGroupCode = passengerInfo.AgeGroupCode,
                                PassengerLabel = priceOption.Label,
                                PassengerTypeId = Convert.ToInt32(passengerInfo.PassengerType)
                            }
                        );
                    }
                }
                var totalLimit = Convert.ToInt32(ConfigurationManagerHelper.GetValuefromAppSettings("RezdyTotalRecordsLimit"));
                var fixedRecordsAtOneTime = Convert.ToInt32(ConfigurationManagerHelper.GetValuefromAppSettings("RezdyFixedRecordGetOneTime"));
                if (totalLimit == 0 && fixedRecordsAtOneTime == 0)
                {
                    totalLimit = 1000;
                    fixedRecordsAtOneTime = 100;
                }
                //Loop of 100 records at a time
                //Paging Concept here
                for (int index = 0; index < totalLimit; index = index + fixedRecordsAtOneTime)
                {
                    var availabilityRequest = new AvailabilityRequest
                    {
                        ProductCode = productCode,
                        StartDate = criteria.CheckinDate,
                        EndDate = criteria.CheckoutDate,
                        Limit = fixedRecordsAtOneTime,
                        Offset = index
                    };
                    var response = await _getAvailabilityCommandHandler.ExecuteAsync(availabilityRequest, MethodType.GetAvailability, token);

                    if (response != null)
                    {
                        var result = (List<ProductOption>)_getAvailabilityConverter.Convert(response.ToString(), criteria, rezdyProduct);
                        if (result == null || result.Count == 0)
                        {
                            break;
                        }

                        result.ForEach(x => ((ActivityOption)x).PickUpId = Convert.ToInt32(rezdyProduct.PickupId));
                        result.ForEach(x => ((ActivityOption)x).QuantityRequired = Convert.ToBoolean(rezdyProduct.QuantityRequired));
                        result.ForEach(x => ((ActivityOption)x).QuantityRequiredMin = Convert.ToInt32(rezdyProduct.QuantityRequiredMin));
                        result.ForEach(x => ((ActivityOption)x).QuantityRequiredMax = Convert.ToInt32(rezdyProduct.QuantityRequiredMax));
                        productOptions.AddRange(result);
                        if (result.Count <100)// don't continue call if records are less than 100 
                        {
                            break;
                        }

                    }

                }
            }

            return productOptions;
        }

        public List<SelectedProduct> CreateBooking(List<SelectedProduct> selectedProducts, string token, out string apiRequest, out string apiResponse)
        {
            apiRequest = string.Empty;
            apiResponse = string.Empty;
            //Create booking one by one, if multiple items
            var rezdySelectedProducts = selectedProducts?.Where(x => x.APIType == Isango.Entities.Enums.APIType.Rezdy);
            var bookedProduct = new List<SelectedProduct>();
            foreach (var rezdyItem in rezdySelectedProducts)
            {
                var bookedItemProduct = new List<SelectedProduct>
                {
                    rezdyItem
                };
                var response = _createBookingCommandHandler.Execute(bookedItemProduct, MethodType.CreateBooking, token, out apiRequest, out apiResponse);

                //Mocking api booking
                /*
                var isMock = false;
                apiRequest = File.ReadAllText(@"D:\Sandeep\Isango\API BB\Api Booking Response\14 Rezdy\14 03  BookingRequest.json");
                apiResponse = File.ReadAllText(@"D:\Sandeep\Isango\API BB\Api Booking Response\14 Rezdy\14 04 BookingResponse.json");
                var response = apiResponse;
                isMock = true;
                //*/

                if (response == null) return null;
                var getBookedProduct = _createBookingConverter.Convert(response.ToString(), bookedItemProduct);
                bookedProduct.AddRange(getBookedProduct as List<SelectedProduct>);
            }
            if (bookedProduct == null) return null;
            return bookedProduct as List<SelectedProduct>;
        }

        public CancelBookingResponse CancelBooking(string orderNumber, string token, out string apiRequest, out string apiResponse)
        {
            apiRequest = string.Empty;
            apiResponse = string.Empty;
            if (String.IsNullOrEmpty(orderNumber))
            {
                return null;
            }
            var cancelBookingRq = new CancelBookingRequest
            {
                OrderNumber = orderNumber
            };
            var response = _cancelBookingCommandHandler.Execute(cancelBookingRq, MethodType.CancelBooking, token, out apiRequest, out apiResponse);
            if (response == null) return null;
            var cancelBookingResponse = _cancelBookingConverter.Convert(response.ToString());
            return cancelBookingResponse as CancelBookingResponse;
        }

        public List<RezdyPickUpLocation> GetPickUpLocationDetails(int pickUpId, string token)
        {
            var pickUpLocationRequest = new PickUpLocationRequest
            {
                PickUpId = pickUpId
            };

            var response = _getPickUpLocationCommandHandler.Execute(pickUpLocationRequest, MethodType.PickUpDetails, token);
            if (response == null) return null;
            var pickUpLocations = _getPickUpLocationConverter.Convert(response.ToString());
            return pickUpLocations as List<RezdyPickUpLocation>;
        }
    }
}