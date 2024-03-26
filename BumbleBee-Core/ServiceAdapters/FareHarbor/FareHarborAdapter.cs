using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Enums;
using Isango.Entities.FareHarbor;
using ServiceAdapters.FareHarbor.Constants;
using ServiceAdapters.FareHarbor.FareHarbor.Commands.Contracts;
using ServiceAdapters.FareHarbor.FareHarbor.Converters.Contracts;
using ServiceAdapters.FareHarbor.FareHarbor.Entities;
using ServiceAdapters.FareHarbor.FareHarbor.Entities.RequestResponseModels;
using Util;
using Booking = Isango.Entities.Booking;
using Product = Isango.Entities.ConsoleApplication.AgeGroup.FareHarbor.Product;
using SelectedProduct = Isango.Entities.SelectedProduct;

namespace ServiceAdapters.FareHarbor
{
    public class FareHarborAdapter : IFareHarborAdapter, IAdapter
    {
        #region "Private Members"

        private readonly ICreateBookingCommandHandler _createBookingCommandHandler;
        private readonly IDeleteBookingCommandHandler _deleteBookingCommandHandler;
        private readonly IGetAvailabilitiesByDateCommandHandler _getAvailabilitiesByDateCommandHandler;
        private readonly IGetAvailabilityCommandHandler _getAvailabilityCommandHandler;
        private readonly IGetAvailabilitiesCommandHandler _getAvailabilitiesCommandHandler;
        private readonly IGetBookingCommandHandler _getBookingCommandHandler;
        private readonly IGetCompaniesCommandHandler _getCompaniesCommandHandler;
        private readonly IGetItemsCommandHandler _getItemsCommandHandler;
        private readonly IGetLodgingsAvailabilityCommandHandler _getLodgingsAvailabilityCommandHandler;
        private readonly IGetLodgingsCommandHandler _getLodgingsCommandHandler;
        private readonly IRebookingCommandHandler _rebookingCommandHandler;
        private readonly IUpdateBookingNoteCommandHandler _updateBookingNoteCommandHandler;
        private readonly IValidateBookingCommandHandler _validateBookingCommandHandler;
        private readonly IGetCustomerPrototypesCommandHandler _customerPrototypesCommandHandler;

        private readonly IAvailabilityConverter _availabilityConverter;
        private readonly ICompaniesConverter _companiesConverter;
        private readonly IItemsConverter _itemsConverter;
        private readonly IBookingConverter _bookingConverter;
        private readonly int _maxParallelThreadCount;
        private readonly bool _isRollbackLiveAPIBookingsOtherThanPROD;

        #endregion "Private Members"

        #region "Constructor"

        public FareHarborAdapter(ICreateBookingCommandHandler createBookingCommandHandler, IDeleteBookingCommandHandler deleteBookingCommandHandler, IGetAvailabilitiesByDateCommandHandler getAvailabilitiesByDateCommandHandler, IGetAvailabilityCommandHandler getAvailabilityCommandHandler, IGetAvailabilitiesCommandHandler getAvailabilitiesCommandHandler, IGetBookingCommandHandler getBookingCommandHandler, IGetCompaniesCommandHandler getCompaniesCommandHandler, IGetItemsCommandHandler getItemsCommandHandler, IGetLodgingsAvailabilityCommandHandler getLodgingsAvailabilityCommandHandler, IGetLodgingsCommandHandler getLodgingsCommandHandler, IRebookingCommandHandler rebookingCommandHandler, IUpdateBookingNoteCommandHandler updateBookingNoteCommandHandler, IValidateBookingCommandHandler validateBookingCommandHandler,
            IGetCustomerPrototypesCommandHandler customerPrototypesCommandHandler, IAvailabilityConverter availabilityConverter, ICompaniesConverter companiesConverter, IItemsConverter itemsConverter, IBookingConverter bookingConverter)
        {
            _createBookingCommandHandler = createBookingCommandHandler;
            _deleteBookingCommandHandler = deleteBookingCommandHandler;
            _getAvailabilitiesByDateCommandHandler = getAvailabilitiesByDateCommandHandler;
            _getAvailabilityCommandHandler = getAvailabilityCommandHandler;
            _getAvailabilitiesCommandHandler = getAvailabilitiesCommandHandler;
            _getBookingCommandHandler = getBookingCommandHandler;
            _getCompaniesCommandHandler = getCompaniesCommandHandler;
            _getItemsCommandHandler = getItemsCommandHandler;
            _getLodgingsAvailabilityCommandHandler = getLodgingsAvailabilityCommandHandler;
            _getLodgingsCommandHandler = getLodgingsCommandHandler;
            _rebookingCommandHandler = rebookingCommandHandler;
            _updateBookingNoteCommandHandler = updateBookingNoteCommandHandler;
            _validateBookingCommandHandler = validateBookingCommandHandler;
            _customerPrototypesCommandHandler = customerPrototypesCommandHandler;

            _availabilityConverter = availabilityConverter;
            _companiesConverter = companiesConverter;
            _itemsConverter = itemsConverter;
            _bookingConverter = bookingConverter;
            _maxParallelThreadCount = MaxParallelThreadCountHelper.GetMaxParallelThreadCount(Constant.MaxParallelThreadCount);
            try
            {
                _isRollbackLiveAPIBookingsOtherThanPROD = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.RollbackLiveAPIBookingsOtherThanPROD) == "1";
            }
            catch (Exception)
            {
                _isRollbackLiveAPIBookingsOtherThanPROD = false;
            }
        }

        #endregion "Constructor"

        /// <summary>
        /// Get List of Companies
        /// </summary>
        /// <returns> List of Companies</returns>
        public List<Supplier> GetCompanies(FareHarborUserKey userKey, string token)
        {
            var res = _getCompaniesCommandHandler.Execute(userKey, MethodType.Companies, token);

            if (res != null)
            {
                res = _companiesConverter.Convert(res, null);
                return res as List<Supplier>;
            }
            return null;
        }

        /// <summary>
        /// Get List of Companies Asynchronously
        /// </summary>
        /// <returns>List of Companies</returns>
        public async Task<List<Supplier>> GetCompaniesAsync()
        {
            var res = await _getCompaniesCommandHandler.ExecuteAsync(string.Empty, MethodType.Companies, Guid.NewGuid().ToString());

            if (res != null)
            {
                res = _companiesConverter.Convert(res, null);
                return res as List<Supplier>;
            }
            return null;
        }

        /// <summary>
        /// Get List of Items associated with a Particular Company
        /// </summary>
        /// <returns>List of Activities</returns>
        public List<Activity> GetItems(Supplier supplier, string token)
        {
            var res = _getItemsCommandHandler.Execute(supplier, MethodType.Items, token);

            if (res != null)
            {
                res = _itemsConverter.Convert(res, null);
                return res as List<Activity>;
            }
            return null;
        }

        /// <summary>
        /// Get List of Items associated with a Particular Company Asynchronously
        /// </summary>
        /// <returns>List of Items</returns>
        public async Task<List<Activity>> GetItemsAsync(Supplier supplier, string token)
        {
            var res = await _getItemsCommandHandler.ExecuteAsync(supplier, MethodType.Items, token);

            if (res != null)
            {
                res = _itemsConverter.Convert(res, null);
                return res as List<Activity>;
            }
            return null;
        }

        /// <summary>
        /// Get List of Availability Slot for that particular Date Range.
        /// </summary>
        /// <returns>List of Availabilities</returns>
        public List<Activity> GetAvailabilities(FareHarborCriteria criteria, string token)
        {
            var res = _getAvailabilitiesCommandHandler.Execute(criteria, MethodType.Availabilities, token);

            if (res != null)
            {
                return GetAvailabilityDetail(res.ToString(), criteria, false, token);
            }
            return null;
        }

        /// <summary>
        /// Get List of Availability Slot for that particular Date Range Asynchronously.
        /// </summary>
        /// <returns>List of Availabilities</returns>
        public async Task<List<Activity>> GetAvailabilitiesAsync(FareHarborCriteria criteria, string token)
        {
            var res = await _getAvailabilitiesCommandHandler.ExecuteAsync(criteria, MethodType.Availabilities, token);

            if (res != null)
            {
                return GetAvailabilityDetail(res.ToString(), criteria, true, token);
            }
            return null;
        }

        /// <summary>
        /// Get List of Availability Slot for that particular Date.
        /// </summary>
        /// <returns>List of Availabilities</returns>
        public List<Activity> GetAvailabilitiesByDate(FareHarborCriteria criteria, string token)
        {
            var res = _getAvailabilitiesByDateCommandHandler.Execute(criteria, MethodType.AvailabilitiesByDate, token);

            if (res != null)
            {
                return GetAvailabilityDetail(res.ToString(), criteria, false, token);
            }

            return null;
        }

        /// <summary>
        /// Get List of Availability Slot for that particular Date Asynchronously.
        /// </summary>
        /// <returns>List of Availabilities</returns>
        public async Task<List<Activity>> GetAvailabilitiesByDateAsync(FareHarborCriteria criteria, string token)
        {
            var res = await _getAvailabilitiesByDateCommandHandler.ExecuteAsync(criteria, MethodType.AvailabilitiesByDate, token);

            if (res != null)
            {
                return GetAvailabilityDetail(res.ToString(), criteria, false, token);
            }
            return null;
        }

        /// <summary>
        /// Book the activity
        /// </summary>
        /// <returns>Booking Details</returns>
        public Booking.Booking CreateBooking(Booking.Booking booking, string token)
        {
            var bookingSelectedProducts = booking.SelectedProducts;
            if (bookingSelectedProducts.Count > 0)
            {
                var selectedProductList = new List<SelectedProduct>();
                Parallel.ForEach(bookingSelectedProducts, new ParallelOptions { MaxDegreeOfParallelism = _maxParallelThreadCount }, selectedProduct =>
                 {
                     var fareHarborSelectedProduct = (FareHarborSelectedProduct)selectedProduct;
                     var res = _createBookingCommandHandler.Execute(fareHarborSelectedProduct, MethodType.Create, token);
                     if (res != null)
                     {
                         var bookingResponse = SerializeDeSerializeHelper.DeSerialize<BookingResponse>(res.ToString());
                         if (bookingResponse.Booking != null)
                         {
                             fareHarborSelectedProduct.UuId = bookingResponse.Booking.Uuid;
                         }
                     }
                     selectedProductList.Add(fareHarborSelectedProduct);
                 });

                booking.SelectedProducts = selectedProductList;
            }
            return booking;
        }

        /// <summary>
        /// Book the Activity
        /// </summary>
        /// <param name="selectedProducts"></param>
        /// <param name="bookingReferenceNumber"></param>
        /// <param name="token"></param>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        public List<FareHarborSelectedProduct> CreateBooking(List<FareHarborSelectedProduct> selectedProducts, string bookingReferenceNumber, string token, out string request, out string response)
        {
            request = "";
            response = "";
            foreach (var selectedProduct in selectedProducts)
            {
                selectedProduct.BookingReferenceNumber = bookingReferenceNumber;
                var res = _createBookingCommandHandler.Execute(selectedProduct, MethodType.Create, token, out request, out response);
                if (res != null)
                {
                    var bookingResponse = SerializeDeSerializeHelper.DeSerialize<BookingResponse>(res.ToString());
                    if (bookingResponse.Booking != null)
                    {
                        selectedProduct.UuId = bookingResponse.Booking.Uuid;
                        selectedProduct.SupplierCancellationPolicy = bookingResponse.Booking.EffectiveCancellationPolicy == null ? "" :
                            SerializeDeSerializeHelper.Serialize(bookingResponse.Booking.EffectiveCancellationPolicy);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            return selectedProducts;
        }

        /// <summary>
        /// Book Activity at supplier end.
        /// </summary>
        /// <param name="selectedProduct"></param>
        /// <param name="bookingReferenceNumber"></param>
        /// <param name="token"></param>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        public FareHarborSelectedProduct CreateBooking(FareHarborSelectedProduct selectedProduct, string bookingReferenceNumber, string token, out string request, out string response)
        {
            request = "";
            response = "";

            selectedProduct.BookingReferenceNumber = bookingReferenceNumber;

            var res = _createBookingCommandHandler.Execute(selectedProduct, MethodType.Create, token, out request, out response);

            var isMock = false;
            /*
            request = File.ReadAllText(@"D:\Sandeep\Isango\API BB\Api Booking Response\6 FareHarbour - BookingRequest.json");
            response = File.ReadAllText(@"D:\Sandeep\Isango\API BB\Api Booking Response\6 FareHarbour - BookingResponse.json");
            var res = response;
            isMock = true;
            //*/

            if (res != null)
            {
                var bookingResponse = SerializeDeSerializeHelper.DeSerialize<BookingResponse>(res.ToString());
                if (bookingResponse.Booking != null)
                {
                    selectedProduct.UuId = bookingResponse.Booking.Uuid;

                    if (_isRollbackLiveAPIBookingsOtherThanPROD
                        && !string.IsNullOrWhiteSpace(bookingResponse.Booking?.Uuid)
                        && !isMock
                    )
                    {
                        try
                        {
                            var bookedOption = (ActivityOption)selectedProduct.ProductOptions.FirstOrDefault();
                            var canceRq = string.Empty;
                            var canceRs = string.Empty;
                            DeleteBooking(companyShortName: bookedOption.SupplierName,
                                bookingReferenceNumber: bookingResponse.Booking.Uuid,
                                 userKey: bookedOption.UserKey,
                                 request: out canceRq,
                                response: out canceRs,
                                 token: token);
                        }
                        catch (Exception)
                        {
                            //throw;
                        }
                    }
                    var ao = ((ActivityOption)selectedProduct.ProductOptions.FirstOrDefault());
                    ao.Cancellable = bookingResponse.Booking.IsEligibleForCancellation;
                    var cp = new
                    {
                        bookingResponse.Booking.EffectiveCancellationPolicy?.Cutoff,
                        bookingResponse.Booking.EffectiveCancellationPolicy?.Type,
                        bookingResponse.Booking.IsEligibleForCancellation,
                    };
                    var SerializeCancellationPolicy = SerializeDeSerializeHelper.Serialize(cp);
                    selectedProduct.SupplierCancellationPolicy = ao.ApiCancellationPolicy = SerializeCancellationPolicy;
                    try
                    {
                        if (bookingResponse?.Booking?.Pickup != null)
                        {
                            selectedProduct.HotelPickUpLocation = ao.HotelPickUpLocation = bookingResponse?.Booking?.Pickup?.DisplayText ?? bookingResponse?.Booking?.Pickup?.Name;
                        }
                    }
                    catch(Exception e)
                    {
                        //ignore
                    }
                }
                else
                {
                    return null;
                }
            }

            return selectedProduct;
        }

        /// <summary>
        /// Book the activity asynchronously
        /// </summary>
        /// <returns>Booking Details</returns>
        public async Task<Booking.Booking> CreateBookingAsync(Booking.Booking booking, string token)
        {
            var bookingSelectedProducts = booking.SelectedProducts;
            if (bookingSelectedProducts.Count > 0)
            {
                var selectedProductList = new List<SelectedProduct>();
                foreach (var selectedProduct in bookingSelectedProducts)
                {
                    var fareHarborSelectedProduct = (FareHarborSelectedProduct)selectedProduct;
                    var res = await _createBookingCommandHandler.ExecuteAsync(fareHarborSelectedProduct, MethodType.Create, token);

                    if (res != null)
                    {
                        var bookingResponse = SerializeDeSerializeHelper.DeSerialize<BookingResponse>(res.ToString());
                        if (bookingResponse.Booking != null)
                        {
                            fareHarborSelectedProduct.UuId = bookingResponse.Booking.Uuid;
                        }
                    }
                    selectedProductList.Add(fareHarborSelectedProduct);
                }
                booking.SelectedProducts = selectedProductList;
            }

            return booking;
        }

        /// <summary>
        ///  Validate the Booking details
        /// </summary>
        /// <param name="booking"></param>
        /// <param name="token"></param>
        /// <returns>Price and Details with isBookable (Whether it is bookable)</returns>
        public List<object> ValidateBooking(Booking.Booking booking, string token)
        {
            var objectList = new List<object>();

            if (booking.SelectedProducts.Count > 0)
            {
                var bookingProduct = booking.SelectedProducts;
                Parallel.ForEach(bookingProduct, new ParallelOptions { MaxDegreeOfParallelism = _maxParallelThreadCount }, selectedProduct =>
                {
                    var res = _validateBookingCommandHandler.Execute(selectedProduct, MethodType.Validate, token);

                    if (res != null)
                    {
                        objectList.Add(res);
                    }
                });
            }

            return objectList;
        }

        /// <summary>
        /// Validate the Booking details asynchronously
        /// </summary>
        /// <returns>Price Details with isBookable (Whether it is bookable)</returns>
        public async Task<List<object>> ValidateBookingAsync(Booking.Booking booking, string token)
        {
            var objectList = new List<Object>();
            foreach (var selectedProduct in booking.SelectedProducts)
            {
                var res = await _validateBookingCommandHandler.ExecuteAsync(selectedProduct, MethodType.Validate, token);

                if (res != null)
                {
                    objectList.Add(res);
                }
            }
            return objectList;
        }

        /// <summary>
        /// Get Booking Details
        /// </summary>
        /// <returns>Booking Details</returns>
        public Booking.Booking GetBooking(string companyShortName, string bookingReferenceNumber, string token)
        {
            var inputObjects = new[] { companyShortName, bookingReferenceNumber };
            var res = _getBookingCommandHandler.Execute(inputObjects, MethodType.Get, token);

            if (res != null)
            {
                res = _bookingConverter.Convert(res, null);
                return res as Booking.Booking;
            }
            return null;
        }

        /// <summary>
        /// Get Booking Details asynchronously
        /// </summary>
        /// <returns>Booking Details</returns>
        public async Task<Booking.Booking> GetBookingAsync(string companyShortName, string bookingReferenceNumber, string token)
        {
            var inputObjects = new[] { companyShortName, bookingReferenceNumber };
            var res = await _getBookingCommandHandler.ExecuteAsync(inputObjects, MethodType.Get, token);

            if (res != null)
            {
                res = _bookingConverter.Convert(res, null);
                return res as Booking.Booking;
            }
            return null;
        }

        /// <summary>
        /// Delete Booking
        /// </summary>
        /// <returns>Booking Details with status cancelled</returns>
        public bool DeleteBooking(List<FareHarborSelectedProduct> selectedProducts, string token)
        {
            var isCancel = true;
            Parallel.ForEach(selectedProducts, new ParallelOptions { MaxDegreeOfParallelism = _maxParallelThreadCount }, item =>
            {
                var activityOption = (ActivityOption)item.ProductOptions.FirstOrDefault();
                var booking = DeleteBooking(item.Code, item.UuId, activityOption?.UserKey, out string request, out string response, token);
                if (booking?.Status != BookingStatus.Cancelled)
                {
                    isCancel = false;
                }
            });

            return isCancel;
        }

        /// <summary>
        /// Delete Booking asynchronously
        /// </summary>
        /// <returns>Booking Details with status cancelled</returns>
        public async Task<Booking.Booking> DeleteBookingAsync(string companyShortName, string bookingReferenceNumber, string token)
        {
            var inputObjects = new[] { companyShortName, bookingReferenceNumber };
            var res = await _deleteBookingCommandHandler.ExecuteAsync(inputObjects, MethodType.Create, token);

            if (res != null)
            {
                res = _bookingConverter.Convert(res, MethodType.Book);
                return res as Booking.Booking;
            }
            return null;
        }

        /// <summary>
        /// Get Lodgings information associated with that company.
        /// </summary>
        /// <returns> List of Lodgings</returns>
        public object GetLodgings(FareHarborRequest fareHarborRequest, string token)
        {
            var res = _getLodgingsCommandHandler.Execute(fareHarborRequest, MethodType.Lodgings, token);

            return res;
        }

        /// <summary>
        /// Get Lodgings information associated with that company Asynchronously.
        /// </summary>
        /// <returns> List of Lodgings</returns>
        public async Task<object> GetLodgingsAsync(FareHarborRequest fareHarborRequest, string token)
        {
            var res = await _getLodgingsCommandHandler.ExecuteAsync(fareHarborRequest, MethodType.Lodgings, token);

            return res;
        }

        /// <summary>
        /// Get Lodgings Availability associated with the availability
        /// </summary>
        /// <returns>List of Available Lodgings</returns>
        public object GetLodgingsAvailability(FareHarborRequest fareHarborRequest, string token)
        {
            var res = _getLodgingsAvailabilityCommandHandler.Execute(fareHarborRequest, MethodType.LodgingsAvailability, token);

            return res;
        }

        /// <summary>
        /// Get Lodgings Availability associated with the availability
        /// </summary>
        /// <param name="shortName"></param>
        /// <param name="pK"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public AvailabilityLodgingsResponse GetLodgingsAvailability(string shortName, string pK, string userKey, string token)
        {
            var fareHarborRequest = new FareHarborRequest
            {
                ShortName = shortName,
                Availability = pK,
                Uuid = userKey
            };
            var res = _getLodgingsAvailabilityCommandHandler.Execute(fareHarborRequest, MethodType.LodgingsAvailability, token);
            if(res != null)
            {
                var lodgingsResponse = SerializeDeSerializeHelper.DeSerialize<AvailabilityLodgingsResponse>(res.ToString());
                return lodgingsResponse;
            }
            return null;
        }

        /// <summary>
        /// Get Lodgings Availability associated with the availability asynchronously
        /// </summary>
        /// <returns>List of Available Lodgings</returns>
        public async Task<object> GetLodgingsAvailabilityAsync(FareHarborRequest fareHarborRequest, string token)
        {
            var res = await _getLodgingsAvailabilityCommandHandler.ExecuteAsync(fareHarborRequest, MethodType.LodgingsAvailability, token);

            return res;
        }

        /// <summary>
        /// Update Booking Note associated with the booking
        /// </summary>
        /// <returns>Booking Details with updated note</returns>
        public Booking.Booking UpdateBookingNote(Booking.Booking booking, string token)
        {
            var bookingProducts = booking.SelectedProducts;
            if (bookingProducts.Count > 0)
            {
                var selectedProductList = new List<SelectedProduct>();
                Parallel.ForEach(bookingProducts, new ParallelOptions { MaxDegreeOfParallelism = _maxParallelThreadCount }, selectedProduct =>
                {
                    var fareHarborSelectedProduct = (FareHarborSelectedProduct)selectedProduct;
                    var res = _updateBookingNoteCommandHandler.Execute(fareHarborSelectedProduct, MethodType.UpdateNote, token);
                    if (res != null)
                    {
                        var bookingResponse = SerializeDeSerializeHelper.DeSerialize<BookingResponse>(res.ToString());
                        if (bookingResponse.Booking != null)
                        {
                            fareHarborSelectedProduct.ActivityPleaseNote = bookingResponse.Booking.Uuid;
                        }
                    }
                    selectedProductList.Add(fareHarborSelectedProduct);
                });
                booking.SelectedProducts = selectedProductList;
            }

            return booking;
        }

        /// <summary>
        /// Update Booking Note associated with the booking asynchronously
        /// </summary>
        /// <returns>Booking Details with updated note</returns>
        public async Task<Booking.Booking> UpdateBookingNoteAsync(Booking.Booking booking, string token)
        {
            var bookingSelectedProducts = booking.SelectedProducts;
            if (bookingSelectedProducts.Count > 0)
            {
                var selectedProductList = new List<SelectedProduct>();
                foreach (var selectedProduct in bookingSelectedProducts)
                {
                    var fareHarborSelectedProduct = (FareHarborSelectedProduct)selectedProduct;
                    var res = await _updateBookingNoteCommandHandler.ExecuteAsync(fareHarborSelectedProduct, MethodType.Rebook, token);

                    if (res != null)
                    {
                        var bookingResponse = SerializeDeSerializeHelper.DeSerialize<BookingResponse>(res.ToString());
                        if (bookingResponse.Booking != null)
                        {
                            fareHarborSelectedProduct.UuId = bookingResponse.Booking.Uuid;
                        }
                    }
                    selectedProductList.Add(fareHarborSelectedProduct);
                }
                booking.SelectedProducts = selectedProductList;
            }

            return booking;
        }

        /// <summary>
        /// Rebook the activity
        /// </summary>
        /// <returns>Booking Details with new UUID</returns>
        public Booking.Booking Rebooking(Booking.Booking booking, string token)
        {
            var bookingSelectedProducts = booking.SelectedProducts;
            if (bookingSelectedProducts.Count > 0)
            {
                var selectedProductList = new List<SelectedProduct>();
                foreach (var selectedProduct in bookingSelectedProducts)
                {
                    var fareHarborSelectedProduct = (FareHarborSelectedProduct)selectedProduct;
                    var res = _rebookingCommandHandler.Execute(fareHarborSelectedProduct, MethodType.Rebook, token);
                    if (res != null)
                    {
                        var bookingResponse = SerializeDeSerializeHelper.DeSerialize<BookingResponse>(res.ToString());
                        if (bookingResponse.Booking != null)
                        {
                            fareHarborSelectedProduct.UuId = bookingResponse.Booking.Uuid;
                        }
                    }
                    else
                    {
                        return null;
                    }
                    selectedProductList.Add(fareHarborSelectedProduct);
                }
                booking.SelectedProducts = selectedProductList;
            }

            return booking;
        }

        /// <summary>
        /// Rebook the activity asynchronously
        /// </summary>
        /// <returns>Booking Details with new UUID</returns>
        public async Task<Booking.Booking> RebookingAsync(Booking.Booking booking, string token)
        {
            var bookingSelectedProducts = booking.SelectedProducts;
            if (bookingSelectedProducts.Count > 0)
            {
                var selectedProductList = new List<SelectedProduct>();
                foreach (var selectedProduct in bookingSelectedProducts)
                {
                    var fareHarborSelectedProduct = (FareHarborSelectedProduct)selectedProduct;
                    var res = await _rebookingCommandHandler.ExecuteAsync(fareHarborSelectedProduct, MethodType.Rebook, token);

                    if (res != null)
                    {
                        var bookingResponse = SerializeDeSerializeHelper.DeSerialize<BookingResponse>(res.ToString());
                        if (bookingResponse.Booking != null)
                        {
                            fareHarborSelectedProduct.UuId = bookingResponse.Booking.Uuid;
                        }
                    }
                    else
                    {
                        return null;
                    }

                    selectedProductList.Add(fareHarborSelectedProduct);
                }
                booking.SelectedProducts = selectedProductList;
            }

            return booking;
        }

        /// <summary>
        /// Get all customer prototypes by Product Ids
        /// </summary>
        /// <param name="products"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public ItemDTO GetCustomerPrototypesByProductId(Product product, string token)
        {
            ItemDTO items = null;
            if (product != null && product.ServiceId != 0)
            {
                //Get ALL Availabilities containing prototypes and types
                var result = _customerPrototypesCommandHandler.Execute(product, MethodType.CustomerPrototype, token);
                if (result != null)
                {
                    items = SerializeDeSerializeHelper.DeSerialize<ItemDTO>(result.ToString());
                }
            }

            return items;
        }

        /// <summary>
        /// Delete Booking
        /// </summary>
        /// <param name="companyShortName"></param>
        /// <param name="bookingReferenceNumber"></param>
        /// <param name="userKey"></param>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="token"></param>
        /// <returns>Booking Details with status canceled</returns>
        public Booking.Booking DeleteBooking(string companyShortName, string bookingReferenceNumber, string userKey, out string request, out string response, string token)
        {
            request = "";
            response = "";
            var inputObjects = new[] { companyShortName, bookingReferenceNumber, userKey };
            var res = _deleteBookingCommandHandler.Execute(inputObjects, MethodType.Delete, token, out request, out response);

            if (res != null)
            {
                res = _bookingConverter.Convert(res, MethodType.Create);
                return res as Booking.Booking;
            }
            return null;
        }

        #region Private Methods

        /// <summary>
        /// Get availability details
        /// </summary>
        /// <param name="responseObject"></param>
        /// <param name="criteria"></param>
        /// <param name="isAsync"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private List<Activity> GetAvailabilityDetail(string responseObject, FareHarborCriteria criteria, bool isAsync, string token)
        {
            var activities = new List<Activity>();
            var activity = new Activity();
            var availabilitiesResponse = SerializeDeSerializeHelper.DeSerialize<AvailabilitiesResponse>(responseObject);
            if (availabilitiesResponse.Availabilities != null)
            {
                var productOptions = new List<ProductOption>();
                var availabilityList = availabilitiesResponse.Availabilities;
                availabilityList.ForEach(availability =>
                {
                    var activityOption = new ActivityOption
                    {
                        Id = availability.Pk,
                        UserKey = criteria.UserKey,
                        SupplierName = criteria.CompanyName
                    };
                    if (isAsync)
                    {
                        var availabilityResponse = GetAvailabilityAsync(activityOption, token);
                        availabilityResponse.Wait();
                        var availabilityProductOptions = availabilityResponse.Result;
                        Parallel.ForEach(availabilityProductOptions, new ParallelOptions { MaxDegreeOfParallelism = _maxParallelThreadCount }, item => { productOptions.Add(item); });
                    }
                    else
                    {
                        var collection = GetAvailability(activityOption, criteria, token, availability);
                        if (collection != null)
                        {
                            productOptions.AddRange(collection);
                        }
                    }
                });
                activity.ProductOptions = productOptions;
                activities.Add(activity);
                return activities;
            }
            return null;
        }

        /// <summary>
        /// Get Availability Details for Availability Id asynchronously
        /// </summary>
        /// <returns>Availability Details</returns>
        private async Task<List<ActivityOption>> GetAvailabilityAsync(ActivityOption activityOption, string token)
        {
            var res = await _getAvailabilityCommandHandler.ExecuteAsync(activityOption, MethodType.Availability, token);

            if (res != null)
            {
                var result = SerializeDeSerializeHelper.DeSerialize<AvailabilityResponse>(res as string);
                res = _availabilityConverter.Convert(result, null);
                return res as List<ActivityOption>;
            }
            return null;
        }

        /// <summary>
        /// Get Availability Details for Availability Id
        /// </summary>
        /// <returns>Availability Details</returns>
        private List<ProductOption> GetAvailability(ActivityOption activityOption, FareHarborCriteria criteria, string token, Availability response)
        {
            //var res = _getAvailabilityCommandHandler.Execute(activityOption, MethodType.Availability, token);
            //var result = SerializeDeSerializeHelper.DeSerialize<AvailabilityResponse>(res as string);
            try
            {
                var res = _availabilityConverter.Convert(response, criteria);
                return res as List<ProductOption>;
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        #endregion Private Methods
    }
}