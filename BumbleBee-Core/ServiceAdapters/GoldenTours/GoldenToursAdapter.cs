using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.GoldenTours;
using Logger.Contract;
using ServiceAdapters.GoldenTours.GoldenTours.Commands.Contracts;
using ServiceAdapters.GoldenTours.GoldenTours.Converters.Contracts;
using ServiceAdapters.GoldenTours.GoldenTours.Entities;
using ServiceAdapters.GoldenTours.GoldenTours.Entities.Availability;
using ServiceAdapters.GoldenTours.GoldenTours.Entities.GetBookingDates;
using ServiceAdapters.GoldenTours.GoldenTours.Entities.PickupPoints;
using ServiceAdapters.GoldenTours.GoldenTours.Entities.ProductDetails;
using Util;
using AgeGroup = Isango.Entities.GoldenTours.AgeGroup;
using Unit = ServiceAdapters.GoldenTours.GoldenTours.Entities.ProductDetails.Unit;

// ReSharper disable PossibleNullReferenceException

namespace ServiceAdapters.GoldenTours
{
    public class GoldenToursAdapter : IGoldenToursAdapter
    {
        private readonly IProductDetailsCommandHandler _productDetailsCommandHandler;
        private readonly IAvailabilityCommandHandler _availabilityCommandHandler;
        private readonly IGetProductDatesCommandHandler _getProductDatesCommandHandler;
        private readonly IGetBookingDatesCommandHandler _getBookingDatesCommandHandler;
        private readonly IPickupPointsCommandHandler _pickupPointsCommandHandler;
        private readonly IBookingCommandHandler _bookingCommandHandler;

        private readonly IProductDetailsConverter _productDetailsConverter;
        private readonly IAvailabilityConverter _availabilityConverter;
        private readonly IGetProductDatesConverter _getProductDatesConverter;
        private readonly IGetBookingDatesConverter _getBookingDatesConverter;
        private readonly IPickupPointsConverter _pickupPointsConverter;
        private readonly IBookingConverter _bookingConverter;
        private readonly ILogger _logger;

        public GoldenToursAdapter(IProductDetailsCommandHandler productDetailsCommandHandler, IProductDetailsConverter productDetailsConverter, IAvailabilityCommandHandler availabilityCommandHandler, IAvailabilityConverter availabilityConverter,
            IGetProductDatesCommandHandler getProductDatesCommandHandler, IGetProductDatesConverter getProductDatesConverter, IGetBookingDatesCommandHandler getBookingDatesCommandHandler, IGetBookingDatesConverter getBookingDatesConverter,
            IPickupPointsCommandHandler pickupPointsCommandHandler, IPickupPointsConverter pickupPointsConverter,
            IBookingCommandHandler bookingCommandHandler, IBookingConverter bookingConverter, ILogger logger)
        {
            _productDetailsCommandHandler = productDetailsCommandHandler;
            _availabilityCommandHandler = availabilityCommandHandler;
            _getProductDatesCommandHandler = getProductDatesCommandHandler;
            _getBookingDatesCommandHandler = getBookingDatesCommandHandler;
            _pickupPointsCommandHandler = pickupPointsCommandHandler;
            _bookingCommandHandler = bookingCommandHandler;

            _productDetailsConverter = productDetailsConverter;
            _availabilityConverter = availabilityConverter;
            _getProductDatesConverter = getProductDatesConverter;
            _getBookingDatesConverter = getBookingDatesConverter;
            _pickupPointsConverter = pickupPointsConverter;
            _bookingConverter = bookingConverter;
            _logger = logger;
        }

        #region Public Methods

        /// <summary>
        /// Get the product detail
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="tokenId"></param>
        public List<Activity> GetProductDetails(GoldenToursCriteria criteria, string tokenId)
        {
            #region Create copy of suppler codes to re-add to criteria, send only required code to GT API.

            var activity = new Activity();
            var actvities = new List<Activity>();

            int count = criteria?.SupplierOptionCodes?.Count ?? 0;
            var copySupplierOptionCodes = new string[count];
            criteria?.SupplierOptionCodes?.CopyTo(copySupplierOptionCodes);
            var optionCodes = copySupplierOptionCodes.ToList();

            #endregion Create copy of suppler codes to re-add to criteria, send only required code to GT API.

            var productOptions = new List<ProductOption>();

            // looping on the available supplier option codes
            foreach (var optionCode in optionCodes)
            {
                try
                {
                    //Continue the loop if the option code is null or empty
                    if (string.IsNullOrWhiteSpace(optionCode)) continue;

                    //assign the option code in the criteria that will be used in the supplier API call
                    criteria.SupplierOptionCode = optionCode;

                    //Remove additional/ other option codes
                    criteria.SupplierOptionCodes.RemoveAll(x => x != optionCode);

                    // GetBookingDates call, continue if response is null
                    var availability = GetBookingDates(criteria, tokenId);
                    if (availability == null) continue;

                    // ProductDetails call, continue if response is null
                    var responseString = _productDetailsCommandHandler.Execute(criteria, MethodType.Productdetails, tokenId);
                    if (responseString == null) continue;

                    // Pass both the responses and the criteria in the converter to prepare product options
                    var result = _productDetailsConverter.Convert(responseString, criteria, availability) as List<ProductOption>;
                    if (result == null) continue;
                    productOptions.AddRange(result);
                }
                catch (Exception ex)
                {
                    var isangoErrorEntity = new IsangoErrorEntity
                    {
                        ClassName = "GoldenTours.GoldenToursAdapter",
                        MethodName = "GetProductDetails.CreateOption"
                    };
                    _logger.Error(isangoErrorEntity, ex);
                    continue;
                }
            }
            //Re-add original/ all options' code to criteria
            criteria.SupplierOptionCodes = optionCodes;
            activity.ProductOptions = productOptions;
            actvities.Add(activity);
            return actvities;
        }

        /// <summary>
        /// Get the product detail asynchronously
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="tokenId"></param>
        public async Task<List<ProductOption>> GetProductDetailsAsync(GoldenToursCriteria criteria, string tokenId)
        {
            #region Create copy of suppler codes to re-add to criteria, send only required code to GT API.

            int count = criteria?.SupplierOptionCodes?.Count ?? 0;
            var copySupplierOptionCodes = new string[count];
            criteria?.SupplierOptionCodes.CopyTo(copySupplierOptionCodes);
            var optionCodes = copySupplierOptionCodes.ToList();

            #endregion Create copy of suppler codes to re-add to criteria, send only required code to GT API.

            var productOptions = new List<ProductOption>();

            // looping on the available supplier option codes
            foreach (var optionCode in optionCodes)
            {
                try
                {
                    //Continue the loop if the option code is null or empty
                    if (string.IsNullOrWhiteSpace(optionCode)) continue;

                    //assign the option code in the criteria that will be used in the supplier API call
                    criteria.SupplierOptionCode = optionCode;

                    //Remove additional/ other option codes
                    criteria.SupplierOptionCodes.RemoveAll(x => x != optionCode);

                    // GetBookingDates call, continue if response is null
                    var availability = await GetBookingDatesAsync(criteria, tokenId);
                    if (availability == null) continue;

                    // ProductDetails call, continue if response is null
                    var responseString = await _productDetailsCommandHandler.ExecuteAsync(criteria, MethodType.Productdetails, tokenId);
                    if (responseString == null) continue;

                    // Pass both the responses and the criteria in the converter to prepare product options
                    var result = _productDetailsConverter.Convert(responseString, criteria, availability) as List<ProductOption>;
                    if (result == null) continue;
                    productOptions.AddRange(result);
                }
                catch (Exception)
                {
                    //ignored //#TODO Add logging here
                }
            }
            //Re-add original/ all options' code to criteria
            criteria.SupplierOptionCodes = optionCodes;

            return await Task.FromResult(productOptions);
        }

        /// <summary>
        /// Get the product dates
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="tokenId"></param>
        public List<DateTime> GetProductDates(GoldenToursCriteria criteria, string tokenId)
        {
            var returnValue = default(List<DateTime>);
            try
            {
                // Return null if response is null
                var responseString = _getProductDatesCommandHandler.Execute(criteria, MethodType.Getproductdates, tokenId);
                if (responseString == null) return null;

                // Pass response and criteria in the converter to prepare available dates
                var result = _getProductDatesConverter.Convert(responseString, criteria);
                returnValue = result as List<DateTime>;
            }
            catch (Exception ex)
            {
                Task.Run(() =>
                     _logger.Error(new Isango.Entities.IsangoErrorEntity
                     {
                         ClassName = "GoldenToursAdapter",
                         MethodName = "GetProductDates",
                         Params = SerializeDeSerializeHelper.Serialize(criteria),
                     }, ex)
                 );
            }
            return returnValue;
        }

        /// <summary>
        /// Get the product dates asynchronously
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="tokenId"></param>
        public async Task<List<DateTime>> GetProductDatesAsync(GoldenToursCriteria criteria, string tokenId)
        {
            // Return null if response is null
            var responseString = await _getProductDatesCommandHandler.ExecuteAsync(criteria, MethodType.Getproductdates, tokenId);
            if (responseString == null) return null;

            // Pass response and criteria in the converter to prepare available dates
            var result = _getProductDatesConverter.Convert(responseString, criteria);
            return await Task.FromResult(result as List<DateTime>);
        }

        /// <summary>
        /// Get the booking dates of a product
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="tokenId"></param>
        public GetBookingDatesResponse GetBookingDates(GoldenToursCriteria criteria, string tokenId)
        {
            // GetBookingDates call, return null if response is null
            var responseString = _getBookingDatesCommandHandler.Execute(criteria, MethodType.Getbookingdates, tokenId);
            if (responseString == null) return null;

            // Pass the response and the criteria in the converter to get the GetBookingDatesResponse
            var result = _getBookingDatesConverter.Convert(responseString, criteria);
            return result as GetBookingDatesResponse;
        }

        /// <summary>
        /// Get the booking dates of a product asynchronously
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="tokenId"></param>
        public async Task<GetBookingDatesResponse> GetBookingDatesAsync(GoldenToursCriteria criteria, string tokenId)
        {
            // GetBookingDates call, return null if response is null
            var responseString = await _getBookingDatesCommandHandler.ExecuteAsync(criteria, MethodType.Getbookingdates, tokenId);
            if (responseString == null) return null;

            // Pass the response and the criteria in the converter to get the GetBookingDatesResponse
            var result = _getBookingDatesConverter.Convert(responseString, criteria);
            return await Task.FromResult(result as GetBookingDatesResponse);
        }

        /// <summary>
        /// Get the booking dates of a product
        /// </summary>
        /// <param name="selectedProducts"></param>
        /// <param name="tokenId"></param>
        /// <param name="apiRequest"></param>
        /// <param name="apiResponse"></param>
        /// <returns></returns>
        public List<SelectedProduct> CreateBooking(List<SelectedProduct> selectedProducts, string tokenId, out string apiRequest, out string apiResponse)
        {
            apiRequest = string.Empty;
            apiResponse = string.Empty;

            // Booking call, return null if response is null
            var responseString = _bookingCommandHandler.Execute(selectedProducts, MethodType.Booking, tokenId, out apiRequest, out apiResponse);
            if (responseString == null) return null;

            // Pass the response and the criteria in the converter to get the selected products
            var result = _bookingConverter.Convert(responseString, selectedProducts);
            return result as List<SelectedProduct>;
        }

        /// <summary>
        /// Get the booking dates of a product asynchronously
        /// </summary>
        /// <param name="selectedProducts"></param>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        public async Task<List<SelectedProduct>> CreateBookingAsync(List<SelectedProduct> selectedProducts, string tokenId)
        {
            // Booking call, return null if response is null
            var responseString = await _bookingCommandHandler.ExecuteAsync(selectedProducts, MethodType.Booking, tokenId);
            if (responseString == null) return null;

            // Pass the response and the criteria in the converter to get the selected products
            var result = _bookingConverter.Convert(responseString, selectedProducts);
            return await Task.FromResult(result as List<SelectedProduct>);
        }

        #endregion Public Methods

        #region Data dumping related calls

        /// <summary>
        /// Get Price and Availability for dumping
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        public List<ProductOption> GetPriceAvailabilityForDumping(GoldenToursCriteria criteria, string tokenId)
        {
            // ProductDetails call, return null if response is null
            var responseString = _productDetailsCommandHandler.Execute(criteria, MethodType.Productdetails, tokenId);
            if (responseString == null) return null;

            // ProductDates call
            var productDates = GetProductDates(criteria, tokenId);

            // BookingDates call
            var bookingDates = GetBookingDates(criteria, tokenId);

            // Pass both ProductDetails and ProductDates response along with the criteria in converter to get the product options
            var result = _productDetailsConverter.Convert(responseString, criteria, productDates, bookingDates);
            return result as List<ProductOption>;
        }

        /// <summary>
        /// Get the product detail response
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="tokenId"></param>
        public AgeGroupWrapper GetProductDetailsResponse(GoldenToursCriteria criteria, string tokenId)
        {
            // ProductDetails call, return null if response is null
            var responseString = _productDetailsCommandHandler.Execute(criteria, MethodType.Productdetails, tokenId);
            if (responseString == null)
                return null;
            var result = SerializeDeSerializeHelper.DeSerializeXml
                    <ProductDetailsResponse>(responseString as string);

            var product = result?.Product;
            var pricePeriods = product.Priceperiods;
            var priceUnits = product?.Priceperiods?.Period?.SelectMany(x => x.Priceunits.Unit)?.ToList();
            var productDetails = new List<ProductDetail>();
            // Preparing ProductDetail model from the supplier response
            foreach (var desc in product.Descriptions.Description)
            {
                var productDetail = new ProductDetail
                {
                    ProductId = criteria.SupplierOptionCode,
                    Ref = product?.Ref,
                    City = product?.City,
                    ProductType = product?.Producttype,
                    PricingUnitType = product?.Priceunittype,
                    Duration = product?.Duration,
                    BookBefore = product?.Book_before,
                    BookBeforeType = product?.Book_before_type,
                    Content = desc?.Content,
                    Title = desc?.Title
                };

                productDetails.Add(productDetail);
            }
            

            // Passing ProductDetails and AgeGroup data in the AgeGroupWrapper
            var ageGroupWrapper = new AgeGroupWrapper
            {
                ProductDetails = productDetails,
                AgeGroups = MapAgeGroups(criteria.SupplierOptionCode, priceUnits),                
                PricePeriods = (pricePeriods != null && pricePeriods.Period != null && pricePeriods.Period.Count > 0) ? MapPeriods(criteria.SupplierOptionCode, pricePeriods) : new List<Periods>()
            };
            return ageGroupWrapper;
        }

        #endregion Data dumping related calls

        #region Not used in the current flow

        /// <summary>
        /// Get the booking dates of a product
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="tokenId"></param>
        public PickupPointsResponse GetPickupPoints(string productId, string tokenId)
        {
            var responseString = _pickupPointsCommandHandler.Execute(productId, MethodType.Getpickuppoints, tokenId);
            if (responseString == null) return null;
            var result = _pickupPointsConverter.Convert(responseString, productId);
            return result as PickupPointsResponse;
        }

        /// <summary>
        /// Get the booking dates of a product asynchronously
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="tokenId"></param>
        public async Task<PickupPointsResponse> GetPickupPointsAsync(string productId, string tokenId)
        {
            var responseString = await _pickupPointsCommandHandler.ExecuteAsync(productId, MethodType.Getpickuppoints, tokenId);
            if (responseString == null) return null;
            var result = _pickupPointsConverter.Convert(responseString, productId);
            return await Task.FromResult(result as PickupPointsResponse);
        }

        /// <summary>
        /// Get the product detail
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="tokenId"></param>
        public AvailabilityResponse GetAvailability(GoldenToursCriteria criteria, string tokenId)
        {
            var responseString = _availabilityCommandHandler.Execute(criteria, MethodType.Availability, tokenId);
            if (responseString == null) return null;
            var result = _availabilityConverter.Convert(responseString, criteria);
            return result as AvailabilityResponse;
        }

        /// <summary>
        /// Get the product detail asynchronously
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="tokenId"></param>
        public async Task<AvailabilityResponse> GetAvailabilityAsync(GoldenToursCriteria criteria, string tokenId)
        {
            var responseString = await _availabilityCommandHandler.ExecuteAsync(criteria, MethodType.Availability, tokenId);
            if (responseString == null) return null;
            var result = _availabilityConverter.Convert(responseString, criteria);
            return await Task.FromResult(result as AvailabilityResponse);
        }

        #endregion Not used in the current flow

        #region Private Methods

        /// <summary>
        /// Map age group data
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="priceUnits"></param>
        /// <returns></returns>
        private List<AgeGroup> MapAgeGroups(string productId, List<Unit> priceUnits)
        {
            var ageGroups = new List<AgeGroup>();
            if (priceUnits == null) return ageGroups;

            priceUnits = priceUnits.DistinctBy(x => x.Id).ToList();
            foreach (var priceUnit in priceUnits)
            {
                var ageGroup = new AgeGroup
                {
                    ProductId = productId,
                    UnitID = priceUnit?.Id,
                    UnitTitle = priceUnit?.Title
                };
                ageGroups.Add(ageGroup);
            }

            return ageGroups;
        }

        private List<Periods> MapPeriods(string productId, Priceperiods pricePeriods)
        {
            var periods = new List<Periods>();
            if (pricePeriods == null) return periods;
                       
            foreach (var pricePeriod in pricePeriods.Period)
            {
                var period = new Periods
                {
                    ProductId = productId,
                    StartDate = pricePeriod.Start_date,
                    EndDate = pricePeriod.End_date,
                    MinCapacity = pricePeriod.Minimum_pax,
                    MaxCapacity = !string.IsNullOrEmpty(pricePeriod.Maximum_pax) ? (Convert.ToInt32(pricePeriod.Maximum_pax) == 0 ? "99" : pricePeriod.Maximum_pax) : "99"
                };
                periods.Add(period);
            }

            return periods;
        }

        #endregion Private Methods
    }
}