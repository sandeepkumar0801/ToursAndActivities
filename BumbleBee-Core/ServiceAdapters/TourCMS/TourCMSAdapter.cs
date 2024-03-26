using Isango.Entities.TourCMSCriteria;
using ServiceAdapters.TourCMS;
using ServiceAdapters.TourCMS.TourCMS.Commands.Contracts;
using Util;
using ServiceAdapters.TourCMS.TourCMS.Entities;
using ServiceAdapters.TourCMS.TourCMS.Entities.ChannelListResponse;
using Isango.Entities;
using ServiceAdapters.TourCMS.TourCMS.Entities.DatesnDealsResponse;
using Isango.Entities.Activities;
using ServiceAdapters.TourCMS.TourCMS.Converters.Contracts;
using ServiceAdapters.TourCMS.TourCMS.Entities.CheckAvailabilityResponse;
using Isango.Entities.Booking;
using ServiceAdapters.TourCMS.TourCMS.Entities.CancelBookingResponse;
using ServiceAdapters.TourCMS.TourCMS.Entities.CancelBookingRequest;
using ServiceAdapters.TourCMS.TourCMS.Entities.NewBooking;
using ServiceAdapters.TourCMS.TourCMS.Entities.DeleteBookingResponse;
using ServiceAdapters.TourCMS.TourCMS.Entities.DeleteBookingRequest;
using ServiceAdapters.TourCMS.TourCMS.Entities.Redemption;
using Isango.Entities.TourCMS;

namespace ServiceAdapters.Tiqets
{
    public class TourCMSAdapter : ITourCMSAdapter, IAdapter
    {
        private readonly InputContext _inputContext = new InputContext();
        //Command Handlers
        private readonly IChannelCommandHandler _getChannelCommandHandler;
        private readonly IChannelShowCommandHandler _getChannelShowCommandHandler;

        private readonly ITourCommandHandler _getTourCommandHandler;
        private readonly ITourShowCommandHandler _getTourShowCommandHandler;

        private readonly IDatesnDealsCommandHandler _datesnDealsCommandHandler;
        private readonly IDatesnDealsConverter _datenDealsConverter;

        private readonly ICheckAvailabilityCommandHandler _availabilitytourCMSCommandHandler;
        private readonly IAvailabilityConverter _availabilitytourCMSConverter;

        private readonly INewBookingCommandHandler _newBookingCommandHandler;
        private readonly ICommitBookingCommandHandler _commitBookingCommandHandler;

        private readonly INewBookingConverter _newBookingConverter;
        private readonly ICommitBookingConverter _commitBookingConverter;

        private readonly ICancelBookingCommandHandler _cancelBookingCommandHandler;
        private readonly ICancelBookingConverter _cancelBookingConverter;

        private readonly IDeleteBookingCommandHandler _deleteBookingCommandHandler;
        private readonly IDeleteBookingConverter _deleteBookingConverter;

        private readonly ITourCMSRedemptionCommandHandler _tourCMSRedemptionCommandHandler;


        public TourCMSAdapter(
            IChannelCommandHandler getChannelCommandHandler,
            IChannelShowCommandHandler getChannelShowCommandHandler,
            ITourCommandHandler getTourCommandHandler,
            ITourShowCommandHandler getTourShowCommandHandler,
            IDatesnDealsCommandHandler datesnDealsCommandHandler,
            IDatesnDealsConverter datenDealsConverter,
            ICheckAvailabilityCommandHandler availabilitytourCMSCommandHandler,
            IAvailabilityConverter availabilitytourCMSConverter,
            INewBookingCommandHandler newBookingCommandHandler,
            ICommitBookingCommandHandler commitBookingCommandHandler,
            INewBookingConverter newBookingConverter,
            ICommitBookingConverter commitBookingConverter,
            ICancelBookingCommandHandler cancelBookingCommandHandler,
            ICancelBookingConverter cancelBookingConverter,
            IDeleteBookingCommandHandler deleteBookingCommandHandler,
            IDeleteBookingConverter deleteBookingConverter,
            ITourCMSRedemptionCommandHandler tourCMSRedemptionCommandHandler
         )
        {
            _getChannelCommandHandler = getChannelCommandHandler;
            _getChannelShowCommandHandler = getChannelShowCommandHandler;
            _getTourCommandHandler = getTourCommandHandler;
            _getTourShowCommandHandler = getTourShowCommandHandler;
            _datesnDealsCommandHandler = datesnDealsCommandHandler;
            _datenDealsConverter = datenDealsConverter;
            _availabilitytourCMSCommandHandler = availabilitytourCMSCommandHandler;
            _availabilitytourCMSConverter = availabilitytourCMSConverter;

            _newBookingCommandHandler = newBookingCommandHandler;
            _commitBookingCommandHandler = commitBookingCommandHandler;

            _newBookingConverter = newBookingConverter;
            _commitBookingConverter = commitBookingConverter;

            _cancelBookingCommandHandler = cancelBookingCommandHandler;
            _cancelBookingConverter = cancelBookingConverter;

            _deleteBookingCommandHandler = deleteBookingCommandHandler;
            _deleteBookingConverter = deleteBookingConverter;

            _tourCMSRedemptionCommandHandler = tourCMSRedemptionCommandHandler;
        }



        /// <summary>
        /// Get Channel Data
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public ChannelListResponse GetChannelData(string token, int channelId = 0)
        {
            var result = new ChannelListResponse();
            var criteria = new TourCMSCriteria
            {
                ChannelId = channelId
            };
            //Get Channel Data
            string request = "";
            string response = "";
            var responseString = _getChannelCommandHandler.Execute(criteria, MethodType.ChannelList, token, out request, out response);

            if (responseString == null)
                return null;

            if (responseString != null)
            {
                result = SerializeDeSerializeHelper.DeSerializeXml
                     <ChannelListResponse>(responseString as string);
            }
            return result;

        }
        /// <summary>
        /// Get Channel Data
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public ChannelShowResponse GetChannelShowData(string token, int channelId = 0)
        {
            var result = new ChannelShowResponse();
            var criteria = new TourCMSCriteria
            {
                ChannelId = channelId
            };
            //Get Channel Data
            string request = "";
            string response = "";
            var responseString = _getChannelShowCommandHandler.Execute(criteria, MethodType.ChannelShow, token, out request, out response);

            if (responseString == null)
                return null;

            if (responseString != null)
            {
                result = SerializeDeSerializeHelper.DeSerializeXml
                     <ChannelShowResponse>(responseString as string);
            }
            return result;

        }

        /// <summary>
        /// Get Channel Data
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public TourListResponse GetTourData(string token, int channelId = 0)
        {
            var result = new TourListResponse();
            var criteria = new TourCMSCriteria
            {
                ChannelId = channelId
            };
            //Get Channel Data

            string request = "";
            string response = "";
            var responseString = _getTourCommandHandler.Execute(criteria, MethodType.TourList, token, out request, out response);

            if (responseString == null)
                return null;

            if (responseString != null)
            {
                result = SerializeDeSerializeHelper.DeSerializeXml
                     <TourListResponse>(responseString as string);
            }
            return result;

        }
        /// <summary>
        /// Get Channel Data
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public TourShowResponse GetTourShowData(string token, int channelId = 0, int tourId = 0)
        {
            var result = new TourShowResponse();
            var criteria = new TourCMSCriteria
            {
                ChannelId = channelId,
                TourId = tourId
            };
            //Get Channel Data

            string request = "";
            string response = "";
            var responseString = _getTourShowCommandHandler.Execute(criteria, MethodType.TourShow, token, out request, out response);

            if (responseString == null)
                return null;

            if (responseString != null)
            {
                result = SerializeDeSerializeHelper.DeSerializeXml
                     <TourShowResponse>(responseString as string);
            }
            return result;

        }

        /// <summary>
        /// Get Price and Availability for dumping
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        public DatesnDealsResponse GetCalendarDatafromAPI(TourCMSCriteria criteria, string tokenId)
        {
            var result = new DatesnDealsResponse();
            string request = "";
            string response = "";
            var responseString = _datesnDealsCommandHandler.Execute(criteria, MethodType.DatesnDeals, tokenId, out request, out response);
            if (responseString == null)
                return null;

            if (responseString != null)
            {
                result = SerializeDeSerializeHelper.DeSerializeXml
                     <DatesnDealsResponse>(responseString as string);
            }
            return result;
        }

        public List<Activity> GetOptionsForTourCMSActivity(TourCMSCriteria criteria, string token)
        {
            var activities = new List<Activity>();
            var listOfOptionsFromAPI = new Dictionary<string, DatesnDealsResponse>();
            try
            {
                if (criteria?.SupplierOptionCodesAndProductIdVsApiOptionIds?.Count > 0)
                {
                    var count = 0;
                    foreach (var codeAndProductIdVsApiOptionIds in criteria.SupplierOptionCodesAndProductIdVsApiOptionIds)
                    {
                        var accountId = 0;
                        var supplierOptionCode = codeAndProductIdVsApiOptionIds.Key.Split('_')[1];
                        var supplierProductIdForThisOptionCode = codeAndProductIdVsApiOptionIds.Key.Split('_')[0];
                        int channelId = 0;
                        try
                        {
                            var GetchannelId = Int32.TryParse(codeAndProductIdVsApiOptionIds.Key.Split('_')[2], out channelId);
                            var GetaccountId = Int32.TryParse(codeAndProductIdVsApiOptionIds.Key.Split('_')[3], out accountId);
                        }
                        catch (Exception ex)
                        {
                            return null;

                        }
                        if (!string.IsNullOrEmpty(supplierOptionCode) &&
                            !string.IsNullOrEmpty(supplierProductIdForThisOptionCode)
                            && channelId > 0 && accountId > 0)
                        {
                            criteria.ChannelId = channelId;
                            criteria.AccountId = accountId;
                            criteria.TourId = Convert.ToInt32(supplierOptionCode);
                            var optionsFromApi = GetCalendarDatafromAPI(criteria, token);

                            if (optionsFromApi?.DatesAndPrices != null)
                            {
                                var FilterData = optionsFromApi?.DatesAndPrices?.DateList.Where(x => x.Price1 != "0.00").ToList();

                                var res = (from element in FilterData
                                           group element by element?.StartDate
                                           into groups
                                           select groups.OrderBy(p => decimal.Parse(p.Price1))
                                           ?.FirstOrDefault())?.ToList();



                                //modified api data based on  filter
                                optionsFromApi.DatesAndPrices.DateList = res;
                                optionsFromApi.TotalDateCount = Convert.ToString(res.Count);
                            }

                            if (optionsFromApi?.DatesAndPrices?.DateList?.Count > 0)
                            {
                                optionsFromApi.DatesAndPrices.DateList.ForEach(thisApiOption => codeAndProductIdVsApiOptionIds.Value.Add(thisApiOption.StartDate));
                                listOfOptionsFromAPI.Add(codeAndProductIdVsApiOptionIds.Key, optionsFromApi);
                            }
                        }
                        count++;
                    }

                    //Code block for Converter
                    if (listOfOptionsFromAPI?.Count > 0)
                    {
                        var activityMadeFromApiOptions = _datenDealsConverter.Convert(listOfOptionsFromAPI, criteria);
                        if (activityMadeFromApiOptions != null)
                            activities.AddRange(activityMadeFromApiOptions as List<Activity>);
                    }

                    return activities;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

        /// <summary>
        /// Get Price and Availability for dumping
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        public CheckAvailabilityResponse GetAvailablityfromAPI(TourCMSCriteria criteria, string tokenId)
        {
            var result = new CheckAvailabilityResponse();
            string request = "";
            string response = "";
            var responseString = _availabilitytourCMSCommandHandler.Execute(criteria, MethodType.Availability, tokenId, out request, out response);

            if (responseString == null)
                return null;

            if (responseString != null)
            {
                result = SerializeDeSerializeHelper.DeSerializeXml
                     <CheckAvailabilityResponse>(responseString as string);
            }
            return result;
        }

        /// <summary>
        /// Get product availability and price based on check-in date, Will be adding check-out date as Check-in + 6 to get multi-day options
        ///
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        public Isango.Entities.Activities.Activity ActivityDetails(TourCMSCriteria criteria, string tokenId)
        {
            if (criteria == null) return null;

            var resultActivity = default(Isango.Entities.Activities.Activity);

            var returnValue = _availabilitytourCMSCommandHandler.Execute(criteria, MethodType.Availability, tokenId, out string apiRequestAPI, out string apiResponseAPI);

            if (returnValue != null)
            {
                var responseObject = SerializeDeSerializeHelper.DeSerializeXml
                     <CheckAvailabilityResponse>(returnValue as string);
                if (responseObject != null)
                {
                    var convertedActivity = _availabilitytourCMSConverter.Convert(returnValue, MethodType.Availability, criteria);
                    resultActivity = convertedActivity as Isango.Entities.Activities.Activity;
                }
            }
            return resultActivity;
        }
        /// <summary>
        /// This call is used to create reservation
        /// </summary>
        /// <param name="TourCMSSelectedProducts"></param>
        /// <param name="token"></param>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        public List<SelectedProduct> CreateReservation(
            Booking booking, string token,
            out string request, out string response)
        {
            var _returnValue = new List<SelectedProduct>();
            request = string.Empty;
            response = string.Empty;
            var selectedProducts = booking.SelectedProducts.Where(x => x.APIType == Isango.Entities.Enums.APIType.TourCMS);
            foreach (var selectedProduct in selectedProducts)
            {
                var productOption = selectedProduct.ProductOptions.FirstOrDefault(x => x.IsSelected);
                if (productOption == null || string.IsNullOrWhiteSpace(productOption?.SupplierOptionCode))
                {
                    throw new Exception($"productOption / SupplierOptionCode(TourCMSProductId) not found for ActivityId {selectedProduct.Id}");
                }

                _inputContext.SelectedProducts = selectedProduct;
                _inputContext.LanguageCode = booking?.Language?.Code;
                _inputContext.VoucherEmailAddress = booking?.VoucherEmailAddress;
                _inputContext.VoucherPhoneNumber = booking?.VoucherPhoneNumber;
                _inputContext.TotalCustomers = productOption.TravelInfo.NoOfPassengers.Sum(x => x.Value);
                _inputContext.BookingReference = booking?.ReferenceNumber;
                _newBookingCommandHandler.Execute(_inputContext, token, MethodType.NewBooking, out request, out response);
                if (response == null) return null;
                var _responseValueIsango = _newBookingConverter.Convert(response, selectedProduct);
                var selectedProductIsango = _responseValueIsango as SelectedProduct;
                _returnValue.Add(selectedProductIsango);
            }
            return _returnValue;
        }

        public SelectedProduct CreateReservationSingle(
            SelectedProduct selectedProduct, string languageCode,
            string voucherEmailAddress, string voucherPhoneNumber,
            string referenceNumber, string token,
            out string request, out string response
           )
        {
            var _returnValue = new SelectedProduct();
            request = string.Empty;
            response = string.Empty;
            //var selectedProducts = booking.SelectedProducts.Where(x => x.APIType == Isango.Entities.Enums.APIType.TourCMS);

            var productOption = selectedProduct.ProductOptions.FirstOrDefault(x => x.IsSelected);
            if (productOption == null || string.IsNullOrWhiteSpace(productOption?.SupplierOptionCode))
            {
                throw new Exception($"productOption / SupplierOptionCode(TourCMSProductId) not found for ActivityId {selectedProduct.Id}");
            }

            _inputContext.SelectedProducts = selectedProduct;
            _inputContext.LanguageCode = languageCode; //booking?.Language?.Code;
            _inputContext.VoucherEmailAddress = voucherEmailAddress; //booking?.VoucherEmailAddress;
            _inputContext.VoucherPhoneNumber = voucherPhoneNumber; //booking?.VoucherPhoneNumber;
            _inputContext.TotalCustomers = productOption.TravelInfo.NoOfPassengers.Sum(x => x.Value);
            _inputContext.BookingReference = referenceNumber;
            _newBookingCommandHandler.Execute(_inputContext, token, MethodType.NewBooking, out request, out response);
            if (response == null) return null;
            var _responseValueIsango = _newBookingConverter.Convert(response, selectedProduct);
            var selectedProductIsango = _responseValueIsango as SelectedProduct;
            return selectedProductIsango;
        }
        public NewBookingResponse CreateReservationOnly(
            SelectedProduct selectedProduct, string languageCode,
            string voucherEmailAddress, string voucherPhoneNumber,
            string referenceNumber, string token,
            out string request, out string response)
        {
            var _returnValue = default(object);
            request = string.Empty;
            response = string.Empty;


            var productOption = selectedProduct.ProductOptions.FirstOrDefault(x => x.IsSelected);
            if (productOption == null || string.IsNullOrWhiteSpace(productOption?.SupplierOptionCode))
            {
                throw new Exception($"productOption / SupplierOptionCode(TourCMSProductId) not found for ActivityId {selectedProduct.Id}");
            }

            _inputContext.SelectedProducts = selectedProduct;
            _inputContext.LanguageCode = languageCode;//booking?.Language?.Code;
            _inputContext.VoucherEmailAddress = voucherEmailAddress;// booking?.VoucherEmailAddress;
            _inputContext.VoucherPhoneNumber = voucherPhoneNumber;//booking?.VoucherPhoneNumber;
            _inputContext.TotalCustomers = productOption.TravelInfo.NoOfPassengers.Sum(x => x.Value);
            _inputContext.BookingReference = referenceNumber;//booking?.ReferenceNumber;
            _returnValue = _newBookingCommandHandler.Execute(_inputContext, token, MethodType.NewBooking, out request, out response);
            if (_returnValue == null) return null;
            var reservationRs = SerializeDeSerializeHelper.DeSerializeXml<NewBookingResponse>(_returnValue.ToString());
            return reservationRs;
        }


        /// <summary>
        /// This call is used to create reservation
        /// </summary>
        /// <param name="TourCMSSelectedProducts"></param>
        /// <param name="token"></param>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        public List<SelectedProduct> CommitBooking(
            List<SelectedProduct> selectedProducts, string token,
            out string request, out string response)
        {
            var _returnValue = new List<SelectedProduct>();
            request = string.Empty;
            response = string.Empty;

            foreach (var selectedProduct in selectedProducts)
            {
                var productOption = selectedProduct.ProductOptions.FirstOrDefault(x => x.IsSelected);
                if (productOption == null || string.IsNullOrWhiteSpace(productOption?.SupplierOptionCode))
                {
                    throw new Exception($"productOption / SupplierOptionCode(TourCMSProductId) not found for ActivityId {selectedProduct.Id}");
                }
                _inputContext.SelectedProducts = selectedProduct;
                _inputContext.TotalCustomers = productOption.TravelInfo.NoOfPassengers.Sum(x => x.Value);
                _commitBookingCommandHandler.Execute(_inputContext, token, MethodType.CommitBooking, out request, out response);
                if (response == null) return null;
                var _responseValueIsango = _commitBookingConverter.Convert(response, selectedProduct);
                var selectedProductIsango = _responseValueIsango as SelectedProduct;
                _returnValue.Add(selectedProductIsango);
            }
            return _returnValue;
        }

        public SelectedProduct CommitBookingSingle(
            SelectedProduct selectedProduct, string token,
            out string request, out string response)
        {

            request = string.Empty;
            response = string.Empty;

            var productOption = selectedProduct.ProductOptions.FirstOrDefault(x => x.IsSelected);
            if (productOption == null || string.IsNullOrWhiteSpace(productOption?.SupplierOptionCode))
            {
                throw new Exception($"productOption / SupplierOptionCode(TourCMSProductId) not found for ActivityId {selectedProduct.Id}");
            }
            _inputContext.SelectedProducts = selectedProduct;
            _inputContext.TotalCustomers = productOption.TravelInfo.NoOfPassengers.Sum(x => x.Value);
            _commitBookingCommandHandler.Execute(_inputContext, token, MethodType.CommitBooking, out request, out response);
            if (response == null) return null;
            var _responseValueIsango = _commitBookingConverter.Convert(response, selectedProduct);
            var selectedProductIsango = _responseValueIsango as SelectedProduct;
            return selectedProductIsango;

        }
        public CancelBookingResponse CancelBooking(int bookingId, string prefixServiceCode, string token,
            out string apiRequest, out string apiResponse)
        {
            apiRequest = string.Empty;
            apiResponse = string.Empty;
            if (bookingId == 0)
            {
                return null;
            }
            var cancelBookingRq = new CancelBookingPassRequest
            {
                BookingId = bookingId,
                PrefixServiceCode = prefixServiceCode
            };
            var response = _cancelBookingCommandHandler.Execute(cancelBookingRq, token, MethodType.CancelBooking, out apiRequest, out apiResponse);
            if (response == null) return null;
            var cancelBookingResponse = _cancelBookingConverter.Convert(response.ToString(), cancelBookingRq);
            return cancelBookingResponse as CancelBookingResponse;
        }

        public DeleteBookingResponse DeleteBooking(int bookingId, string prefixServiceCode, string token,
           out string apiRequest, out string apiResponse)
        {
            apiRequest = string.Empty;
            apiResponse = string.Empty;
            if (bookingId == 0)
            {
                return null;
            }
            var delBookingRq = new DeleteBookingPassRequest
            {
                BookingId = bookingId,
                PrefixServiceCode = prefixServiceCode
            };
            var response = _deleteBookingCommandHandler.Execute(delBookingRq, token, MethodType.DeleteBooking, out apiRequest, out apiResponse);
            if (response == null) return null;
            var cancelBookingResponse = _deleteBookingConverter.Convert(response.ToString(), delBookingRq);
            return cancelBookingResponse as DeleteBookingResponse;
        }

        public RedemptionResponse RedemptionBookingData(TourCMSRedemptionCriteria criteria, string tokenId,
            out string apiRequest, out string apiResponse)
        {
            var result = new RedemptionResponse();
            apiRequest = string.Empty;
            apiResponse = string.Empty;

            try
            {
                var response = _tourCMSRedemptionCommandHandler.Execute(criteria, MethodType.RedemptionBooking, tokenId, out apiRequest, out apiResponse);
                if (response == null) return null;

                if (response != null)
                {
                    result = SerializeDeSerializeHelper.DeSerializeXml<RedemptionResponse>(response.ToString() as string);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result as RedemptionResponse;
        }


    }
}