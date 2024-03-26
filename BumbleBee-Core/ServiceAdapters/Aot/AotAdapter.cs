using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Aot;
using Isango.Entities.Enums;
using ServiceAdapters.Aot.Aot.Commands.Contracts;
using ServiceAdapters.Aot.Aot.Converters.Contracts;
using ServiceAdapters.Aot.Aot.Entities;
using ServiceAdapters.Aot.Aot.Entities.RequestResponseModels;
using System.Text;
using System.Xml.Serialization;
using Util;
using Constant = ServiceAdapters.Aot.Constants.Constant;
using OptionGeneralInfoResponse = Isango.Entities.ConsoleApplication.AgeGroup.AOT.OptionGeneralInfoResponse;

namespace ServiceAdapters.Aot
{
    public class AotAdapter : IAotAdapter, IAdapter
    {
        private readonly ICancelEntireBookingCommandHandler _cancelEntireBookingCommandHandler;
        private readonly ICancelSingleServiceBookingCommandHandler _cancelSingleServiceBookingCommandHandler;
        private readonly ICreateBookingCommandHandler _createBookingCommandHandler;
        private readonly IGetBookingCommandHandler _getBookingCommandHandler;
        private readonly IGetBookingListCommandHandler _getBookingListCommandHandler;
        private readonly IGetBulkPricingAvailabilityDetailsCommandHandler _getBulkPricingAvailabilityDetailsCommandHandler;
        private readonly IGetDetailedPricingAvailabilityCommandHandler _getDetailedPricingAvailabilityCommandHandler;
        private readonly IGetLocationCommandHandler _getLocationCommandHandler;
        private readonly IGetProductDetailsCommandHandler _getProductDetailsCommandHandler;
        private readonly IGetSupplierCommandHandler _getSupplierCommandHandler;
        private readonly IUpdateBookingCommandHandler _updateBookingCommandHandler;

        private readonly ICancelEntireBookingConverter _cancelEntireBookingConverter;
        private readonly ICancelSingleServiceBookingConverter _cancelSingleServiceBookingConverter;
        private readonly ICreateBookingConverter _createBookingConverter;
        private readonly IGetBookingConverter _getBookingConverter;
        private readonly IGetBookingListConverter _getBookingListConverter;
        private readonly IGetBulkPricingAvailabilityDetailsConverter _getBulkPricingAvailabilityDetailsConverter;
        private readonly IGetDetailedPricingAvailabilityConverter _getDetailedPricingAvailabilityConverter;
        private readonly IGetLocationConverter _getLocationConverter;
        private readonly IGetProductDetailsConverter _getProductDetailsConverter;
        private readonly IGetSupplierConverter _getSupplierConverter;
        private readonly IUpdateBookingConverter _updateBookingConverter;

        public string AgentId { get; set; }
        public string Password { get; set; }
        private CountryType country;

        private CountryType Country
        {
            get => country;
            set
            {
                country = value;
                SetAgentIdPassword(value);
            }
        }

        public AotAdapter(ICancelEntireBookingCommandHandler cancelEntireBookingCommandHandler, ICancelSingleServiceBookingCommandHandler cancelSingleServiceBookingCommandHandler,
            ICreateBookingCommandHandler createBookingCommandHandler, IGetBookingCommandHandler getBookingCommandHandler, IGetBookingListCommandHandler getBookingListCommandHandler, IGetBulkPricingAvailabilityDetailsCommandHandler getBulkPricingAvailabilityDetailsCommandHandler,
            IGetDetailedPricingAvailabilityCommandHandler getDetailedPricingAvailabilityCommandHandler, IGetLocationCommandHandler getLocationCommandHandler,
            IGetProductDetailsCommandHandler getProductDetailsCommandHandler, IGetSupplierCommandHandler getSupplierCommandHandler, IUpdateBookingCommandHandler updateBookingCommandHandler,
            ICancelEntireBookingConverter cancelEntireBookingConverter, ICancelSingleServiceBookingConverter cancelSingleServiceBookingConverter,
            ICreateBookingConverter createBookingConverter, IGetBookingConverter getBookingConverter, IGetBookingListConverter getBookingListConverter, IGetBulkPricingAvailabilityDetailsConverter getBulkPricingAvailabilityDetailsConverter,
            IGetDetailedPricingAvailabilityConverter getDetailedPricingAvailabilityConverter, IGetLocationConverter getLocationConverter,
            IGetProductDetailsConverter getProductDetailsConverter, IGetSupplierConverter getSupplierConverter, IUpdateBookingConverter updateBookingConverter)
        {
            _cancelEntireBookingCommandHandler = cancelEntireBookingCommandHandler;
            _cancelSingleServiceBookingCommandHandler = cancelSingleServiceBookingCommandHandler;
            _createBookingCommandHandler = createBookingCommandHandler;
            _getBookingCommandHandler = getBookingCommandHandler;
            _getBookingListCommandHandler = getBookingListCommandHandler;
            _getBulkPricingAvailabilityDetailsCommandHandler = getBulkPricingAvailabilityDetailsCommandHandler;
            _getDetailedPricingAvailabilityCommandHandler = getDetailedPricingAvailabilityCommandHandler;
            _getLocationCommandHandler = getLocationCommandHandler;
            _getProductDetailsCommandHandler = getProductDetailsCommandHandler;
            _getSupplierCommandHandler = getSupplierCommandHandler;
            _updateBookingCommandHandler = updateBookingCommandHandler;
            _cancelEntireBookingConverter = cancelEntireBookingConverter;
            _cancelSingleServiceBookingConverter = cancelSingleServiceBookingConverter;
            _createBookingConverter = createBookingConverter;
            _getBookingConverter = getBookingConverter;
            _getBookingListConverter = getBookingListConverter;
            _getBulkPricingAvailabilityDetailsConverter = getBulkPricingAvailabilityDetailsConverter;
            _getDetailedPricingAvailabilityConverter = getDetailedPricingAvailabilityConverter;
            _getLocationConverter = getLocationConverter;
            _getProductDetailsConverter = getProductDetailsConverter;
            _getSupplierConverter = getSupplierConverter;
            _updateBookingConverter = updateBookingConverter;
            Country = CountryType.Australia;
        }

        /// <summary>
        /// Set Agent Id and Password based on the country
        /// </summary>
        /// <param name="countryType">Country Type</param>
        public void SetAgentIdPassword(CountryType countryType)
        {
            switch (countryType)
            {
                case CountryType.Australia:
                    AgentId = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.AustraliaProductAgentId);
                    Password = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.AustraliaProductPassword);
                    break;

                case CountryType.NewZealand:
                    AgentId = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.NewZealandProductAgentId);
                    Password = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.NewZealandProductPassword);
                    break;

                case CountryType.Fiji:
                    AgentId = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.FijiProductAgentId);
                    Password = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.FijiProductPassword);
                    break;
            }
        }

        /// <summary>
        /// Get Locations for Location Type and Code
        /// </summary>
        /// <returns>Return Locations </returns>
        public GetLocationsResponse GetLocations(GetLocationsRequest getLocationRequest, string token)
        {
            _getLocationCommandHandler.AgentId = AgentId;
            _getLocationCommandHandler.Password = Password;
            var res = _getLocationCommandHandler.Execute(getLocationRequest, MethodType.Getlocations, token);

            if (res == null) return null;
            res = _getLocationConverter.Convert(res);
            return res as GetLocationsResponse;
        }

        /// <summary>
        /// Get Locations for Location Type and Code Asynchronously
        /// </summary>
        /// <returns>Return Locations </returns>
        public async Task<GetLocationsResponse> GetLocationsAsync(GetLocationsRequest getLocationRequest, string token)
        {
            _getLocationCommandHandler.AgentId = AgentId;
            _getLocationCommandHandler.Password = Password;
            var res = await _getLocationCommandHandler.ExecuteAsync(getLocationRequest, MethodType.Getlocations, token);

            if (res == null) return null;
            res = _getLocationConverter.Convert(res);
            return res as GetLocationsResponse;
        }

        /// <summary>
        /// Get Supplier information
        /// </summary>
        /// <returns>Return supplier info </returns>
        public SupplierInfoResponse GetSupplierInformation(SupplierInfoRequest supplierInfoRequest, string token)
        {
            _getSupplierCommandHandler.AgentId = AgentId;
            _getSupplierCommandHandler.Password = Password;
            var res = _getSupplierCommandHandler.Execute(supplierInfoRequest, MethodType.Getsupplier, token);

            if (res == null) return null;
            res = _getSupplierConverter.Convert(res);
            return res as SupplierInfoResponse;
        }

        /// <summary>
        /// Get Supplier information  Asynchronously
        /// </summary>
        /// <returns>Return supplier info </returns>
        public async Task<SupplierInfoResponse> GetSupplierInformationAsync(SupplierInfoRequest supplierInfoRequest, string token)
        {
            _getSupplierCommandHandler.AgentId = AgentId;
            _getSupplierCommandHandler.Password = Password;
            var res = await _getSupplierCommandHandler.ExecuteAsync(supplierInfoRequest, MethodType.Getsupplier, token);

            if (res == null) return null;
            res = _getSupplierConverter.Convert(res);
            return res as SupplierInfoResponse;
        }

        /// <summary>
        /// Get Product Details (API Request : OptionGeneralInfoRequest)
        /// </summary>
        /// <returns>Return product Details</returns>
        public OptionGeneralInfoResponse GetProductDetails(OptionGeneralInfoRequest optionGeneralInfoRequest, string token)
        {
            _getProductDetailsCommandHandler.AgentId = AgentId;
            _getProductDetailsCommandHandler.Password = Password;
            var res = _getProductDetailsCommandHandler.Execute(optionGeneralInfoRequest, MethodType.Getproductdetails, token);

            if (res == null || res.ToString().Contains(Constant.ErrorCode)) return null;
            res = _getProductDetailsConverter.Convert(res);
            return res as OptionGeneralInfoResponse;
        }

        /// <summary>
        /// Get Product Details (API Request : OptionGeneralInfoRequest) Asynchronously
        /// </summary>
        /// <returns>Return product Details</returns>
        public async Task<OptionGeneralInfoResponse> GetProductDetailsAsync(OptionGeneralInfoRequest optionGeneralInfoRequest, string token)
        {
            _getProductDetailsCommandHandler.AgentId = AgentId;
            _getProductDetailsCommandHandler.Password = Password;
            var res = await _getProductDetailsCommandHandler.ExecuteAsync(optionGeneralInfoRequest, MethodType.Getproductdetails, token);

            if (res == null) return null;
            res = _getProductDetailsConverter.Convert(res);
            return res as OptionGeneralInfoResponse;
        }

        /// <summary>
        /// Get Pricing and Availability Details (API Request : OptionStayPricingRequest)
        /// </summary>
        /// <returns>Return Detailed Pricing and Availability Details</returns>
        public object GetDetailedPricingAvailability(AotCriteria criteria, string token, bool isBulkPricingResponseRequired = true)
        {
            _getDetailedPricingAvailabilityCommandHandler.AgentId = AgentId;
            _getDetailedPricingAvailabilityCommandHandler.Password = Password;

            var allDates = GetDatesBetween(criteria.CheckinDate, criteria.CheckoutDate);
            var availabilities = new Dictionary<DateTime, OptionStayPricingResponse>();

            var maxParallelThreadCount = MaxParallelThreadCountHelper.GetMaxParallelThreadCount("MaxParallelThreadCount");

            Parallel.ForEach(allDates, new ParallelOptions { MaxDegreeOfParallelism = maxParallelThreadCount }, date =>
             {
                 try
                 {
                     var dateCriteria = new AotCriteria
                     {
                         ActivityId = criteria.ActivityId,
                         Ages = criteria.Ages,
                         CancellationPolicy = criteria.CancellationPolicy,
                         CheckinDate = date,
                         CheckoutDate = criteria.CheckoutDate,
                         NoOfPassengers = criteria.NoOfPassengers,
                         OptCode = criteria.OptCode,
                         PassengerInfo = criteria.PassengerInfo
                     };
                     var res = _getDetailedPricingAvailabilityCommandHandler.Execute(dateCriteria, MethodType.Getdetailedpricingavailability, token);
                     if (res == null || res.ToString().Contains(Constant.ErrorCode)) return;
                     var dateAvailability = DeSerializeXml<OptionStayPricingResponse>(res as string);
                     availabilities.Add(date, dateAvailability);
                 }
                 catch (Exception)
                 {
                     //ignore
                 }
             });

            if (availabilities?.Any(x => x.Value?.OptStayResults?.Count > 0) == true)
            {
                var availableOptCodesQuery = from avail in availabilities
                                             from optStayResult in avail.Value.OptStayResults
                                             select optStayResult.Opt;

                criteria.OptCode = availableOptCodesQuery.Distinct().ToList();

                if (!isBulkPricingResponseRequired)
                    return availabilities;

                object bulkPricingResponse = GetBulkPricingAvailabilityDetails(criteria, token);
                return bulkPricingResponse != null
                    ? _getDetailedPricingAvailabilityConverter.Convert(availabilities,
                        criteria, bulkPricingResponse)
                    : null;
            }

            return null;
        }

        /// <summary>
        /// Get Pricing and Availability Details Asynchronously (API Request : OptionStayPricingRequest)
        /// </summary>
        /// <returns>Return Detailed Pricing and Availability Details</returns>
        public async Task<object> GetDetailedPricingAvailabilityAsync(AotCriteria criteria, string token)
        {
            _getDetailedPricingAvailabilityCommandHandler.AgentId = AgentId;
            _getDetailedPricingAvailabilityCommandHandler.Password = Password;
            var res = await _getDetailedPricingAvailabilityCommandHandler.ExecuteAsync(criteria, MethodType.Getdetailedpricingavailability, token);

            if (res == null) return null;
            var availability = DeSerializeXml<OptionStayPricingResponse>(res as string);
            if (availability.OptStayResults.Count > 0)
            {
                var availableOptCodes = availability.OptStayResults.Select(x => x.Opt).ToList();
                criteria.OptCode = new List<string>();
                criteria.OptCode = availableOptCodes;
                var bulkPricingResponse = await GetBulkPricingAvailabilityDetailsAsync(criteria, token);
                res = _getDetailedPricingAvailabilityConverter.Convert(res, criteria, bulkPricingResponse);
            }
            else
            {
                return null;
            }
            return res;
        }

        /// <summary>
        /// Get Pricing and Availability for Date range (API Request : OptionAvailRequest)
        /// </summary>
        /// <returns>Return Pricing and Availability Details for Date range</returns>
        public object GetBulkPricingAvailabilityDetails(AotCriteria criteria, string token)
        {
            _getBulkPricingAvailabilityDetailsCommandHandler.AgentId = AgentId;
            _getBulkPricingAvailabilityDetailsCommandHandler.Password = Password;
            var res = _getBulkPricingAvailabilityDetailsCommandHandler.Execute(criteria, MethodType.Getbulkpricingavailabilitydetails, token);

            if (res == null) return null;
            res = _getBulkPricingAvailabilityDetailsConverter.Convert(res, criteria);
            return res;
        }

        /// <summary>
        /// Get Pricing and Availability for Date range Asychronously (API Request : OptionAvailRequest)
        /// </summary>
        /// <returns>Return Pricing and Availability Details for Date range</returns>
        public async Task<object> GetBulkPricingAvailabilityDetailsAsync(AotCriteria criteria, string token)
        {
            _getBulkPricingAvailabilityDetailsCommandHandler.AgentId = AgentId;
            _getBulkPricingAvailabilityDetailsCommandHandler.Password = Password;
            var res = await _getBulkPricingAvailabilityDetailsCommandHandler.ExecuteAsync(criteria, MethodType.Getbulkpricingavailabilitydetails, token);

            if (res == null) return null;
            res = _getBulkPricingAvailabilityDetailsConverter.Convert(res, criteria);
            return res;
        }

        /// <summary>
        /// Create Booking for one or more services (API Request : AddBookingRequest)
        /// </summary>
        /// <returns>Return Booking Ref No with Service Line Id for Each Service</returns>
        public object CreateBooking(List<SelectedProduct> addBookingRequest, string token, out string request, out string response)
        {
            request = string.Empty;
            response = string.Empty;
            _createBookingCommandHandler.AgentId = AgentId;
            _createBookingCommandHandler.Password = Password;
            var res = _createBookingCommandHandler.Execute(addBookingRequest, MethodType.Createbooking, token, out request, out response);

            if (res == null || res.ToString().Contains("error_code")) return null;
            var result = _createBookingConverter.Convert(res, addBookingRequest);
            return result;
        }

        /// <summary>
        /// Create Booking for one or more services (API Request : AddBookingRequest)
        /// </summary>
        /// <returns>Return Booking Ref No with Service Line Id for Each Service</returns>
        public object CreateBooking(List<SelectedProduct> addBookingRequest, string token, string referenceNumber, out string request, out string response)
        {
            request = string.Empty;
            response = string.Empty;
            _createBookingCommandHandler.AgentId = AgentId;
            _createBookingCommandHandler.Password = Password;
            var res = _createBookingCommandHandler.Execute(addBookingRequest, MethodType.Createbooking, token, referenceNumber, out request, out response);

            //Mocking api booking
            /*
            var isMock = false;
            request = File.ReadAllText(@"D:\Sandeep\Isango\API BB\Api Booking Response\8 AOT 2 ApiREQ Booking.json");
            response = File.ReadAllText(@"D:\Sandeep\Isango\API BB\Api Booking Response\8 AOT 2 ApiRES Booking.xml");
            var bookingConfirmRS = (response);
            var res = (object)bookingConfirmRS;
            isMock = true;
            //*/

            if (String.IsNullOrEmpty(Convert.ToString(res)) || res == null || res.ToString().Contains("error_code")) return null;
            var result = _createBookingConverter.Convert(res, addBookingRequest);
            return result;
        }

        /// <summary>
        /// Create Booking for one or more services Asynchronously (API Request : AddBookingRequest)
        /// </summary>
        /// <returns>Return Booking Ref No with Service Line Id for Each Service</returns>
        public async Task<object> CreateBookingAsync(AddBookingRequest addBookingRequest, string token)
        {
            _createBookingCommandHandler.AgentId = AgentId;
            _createBookingCommandHandler.Password = Password;
            var res = await _createBookingCommandHandler.ExecuteAsync(addBookingRequest, MethodType.Createbooking, token);

            if (res == null) return null;
            res = _createBookingConverter.Convert(res, addBookingRequest);
            return res;
        }

        /// <summary>
        /// Cancel entire booking is used to cancel entire booking (API Request : CancelServicesRequest)
        /// </summary>
        /// <returns>Return cancel service response</returns>
        public CancelServicesResponse CancelEntireBooking(string referenceNumber, string token)
        {
            _cancelEntireBookingCommandHandler.AgentId = AgentId;
            _cancelEntireBookingCommandHandler.Password = Password;
            var res = _cancelEntireBookingCommandHandler.Execute(referenceNumber, MethodType.Cancelentirebooking, token);

            if (res == null) return null;
            res = _cancelEntireBookingConverter.Convert(res, referenceNumber);
            return res as CancelServicesResponse;
        }

        public async Task<CancelServicesResponse> CancelEntireBookingAsync(CancelServicesRequest cancelServicesRequest, string token)
        {
            _cancelEntireBookingCommandHandler.AgentId = AgentId;
            _cancelEntireBookingCommandHandler.Password = Password;
            var res = await _cancelEntireBookingCommandHandler.ExecuteAsync(cancelServicesRequest, MethodType.Cancelentirebooking, token);

            if (res == null) return null;
            res = _cancelEntireBookingConverter.Convert(res, cancelServicesRequest);
            return res as CancelServicesResponse;
        }

        public CancelServiceResponse CancelSingleServiceBooking(string referenceNumber, string serviceLineNumber, string token, out string request, out string response)
        {
            _cancelSingleServiceBookingCommandHandler.AgentId = AgentId;
            _cancelSingleServiceBookingCommandHandler.Password = Password;
            var cancelServiceRequest = new CancelServiceRequest
            {
                AgentID = AgentId,
                Password = Password,
                Ref = referenceNumber,
                ServiceLineId = serviceLineNumber
            };
            var res = _cancelSingleServiceBookingCommandHandler.Execute(cancelServiceRequest, MethodType.Cancelsingleservicebooking, token, out request, out response);

            if (res == null || res.ToString().Contains("error_code")) return null;
            res = _cancelSingleServiceBookingConverter.Convert(res, cancelServiceRequest);
            return res as CancelServiceResponse;
        }

        public async Task<CancelServiceResponse> CancelSingleServiceBookingAsync(CancelServiceRequest cancelServicesRequest, string token)
        {
            _cancelSingleServiceBookingCommandHandler.AgentId = AgentId;
            _cancelSingleServiceBookingCommandHandler.Password = Password;
            var res = await _cancelSingleServiceBookingCommandHandler.ExecuteAsync(cancelServicesRequest, MethodType.Cancelsingleservicebooking, token);

            if (res == null) return null;
            res = _cancelSingleServiceBookingConverter.Convert(res, cancelServicesRequest);
            return res as CancelServiceResponse;
        }

        public ListBookingsResponse GetBookingList(ListBookingsRequest listBookingsRequest, string token)
        {
            _getBookingListCommandHandler.AgentId = AgentId;
            _getBookingListCommandHandler.Password = Password;
            var res = _getBookingListCommandHandler.Execute(listBookingsRequest, MethodType.Getbookinglist, token);

            if (res == null) return null;
            res = _getBookingListConverter.Convert(res, listBookingsRequest);
            return res as ListBookingsResponse;
        }

        public async Task<ListBookingsResponse> GetBookingListAsync(ListBookingsRequest listBookingsRequest, string token)
        {
            var res = await _getBookingListCommandHandler.ExecuteAsync(listBookingsRequest, MethodType.Getbookinglist, token);

            if (res == null) return null;
            res = _getBookingListConverter.Convert(res, listBookingsRequest);
            return res as ListBookingsResponse;
        }

        public GetBookingResponse GetBooking(GetBookingRequest getBookingRequest, string token)
        {
            var res = _getBookingCommandHandler.Execute(getBookingRequest, MethodType.Getbooking, token);

            if (res == null) return null;
            res = _getBookingConverter.Convert(res, getBookingRequest);
            return res as GetBookingResponse;
        }

        public async Task<GetBookingResponse> GetBookingAsync(GetBookingRequest getBookingRequest, string token)
        {
            var res = await _getBookingCommandHandler.ExecuteAsync(getBookingRequest, MethodType.Getbooking, token);

            if (res == null) return null;
            res = _getBookingConverter.Convert(res, getBookingRequest);
            return res as GetBookingResponse;
        }

        public AddServiceResponse UpdateBooking(AddServiceRequest addServiceRequest, string token)
        {
            var res = _updateBookingCommandHandler.Execute(addServiceRequest, MethodType.Updatebooking, token);

            if (res == null) return null;
            res = _updateBookingConverter.Convert(res, addServiceRequest);
            return res as AddServiceResponse;
        }

        public async Task<AddServiceResponse> UpdateBookingAsync(AddServiceRequest addServiceRequest, string token)
        {
            var res = await _updateBookingCommandHandler.ExecuteAsync(addServiceRequest, MethodType.Updatebooking, token);

            if (res == null) return null;
            res = _updateBookingConverter.Convert(res, addServiceRequest);
            return res as AddServiceResponse;
        }

        public T DeSerializeXml<T>(string responseXmlString)
        {
            if (string.IsNullOrEmpty(responseXmlString))
                throw new ArgumentNullException();

            var utf8Encoding = new UTF8Encoding();
            var byteArray = utf8Encoding.GetBytes(responseXmlString);

            var memoryStream = new System.IO.MemoryStream(byteArray);
            var deSerializer = new XmlSerializer(typeof(T));

            var xmlTextWriter = new System.Xml.XmlTextWriter(memoryStream, Encoding.UTF8);

            var deSerializedObject = (T)deSerializer.Deserialize(xmlTextWriter.BaseStream);
            return deSerializedObject;
        }

        public object GetPricingAvailabilityForDumping(AotCriteria criteria, string token)
        {
            var res = GetBulkPricingAvailabilityDetails(criteria, token) as OptionAvailResponse;
            if (!(res?.OptAvail?.Count > 0)) return null;
            var result = ConvertAvailibilityResult(res, criteria, token);
            return result;
        }

        #region Private Methods

        private List<Activity> ConvertAvailibilityResult(OptionAvailResponse apiResponse, AotCriteria criteria, string token)
        {
            var optAvail = apiResponse.OptAvail.FirstOrDefault();
            var activity = new Activity();
            var options = new List<ProductOption>();
            var travelInfo = new TravelInfo
            {
                NoOfPassengers = new Dictionary<PassengerType, int>(),
                Ages = new Dictionary<PassengerType, int>()
            };

            if (optAvail != null)
            {
                activity.ID = criteria.ActivityId;
                activity.FactsheetId = 0;
                activity.Code = optAvail.Opt;
                activity.CurrencyIsoCode = optAvail.OptRates?.Currency;
                foreach (var optRate in optAvail.OptRates.Rates.OptRate)
                {
                    travelInfo.StartDate = DateTime.Parse(optRate.DateFrom);
                }

                activity.ApiType = APIType.Aot;
                activity.RegionName = string.Empty;
                activity.Inclusions = string.Empty;
                activity.Exclusions = string.Empty;
            }

            var serviceOptionsFoIsango = apiResponse.OptAvail.Select(x => x.Opt).Distinct().ToList();
            if (serviceOptionsFoIsango.Count > 0)
            {
                var optAvailabilities = apiResponse.OptAvail.Where(e => serviceOptionsFoIsango.Contains(e.Opt));
                foreach (var optAvailability in optAvailabilities)
                {
                    var activityOption = CreateOption(optAvailability, criteria, token);
                    if (activityOption != null)
                    {
                        travelInfo.NumberOfNights = 0;
                        travelInfo.NoOfPassengers = criteria.NoOfPassengers;
                        activityOption.TravelInfo = travelInfo;
                        options.Add(activityOption);
                    }
                }
            }

            activity.ProductOptions = options;
            var activities = new List<Activity>
            {
                activity
            };
            return activities;
        }

        private ActivityOption CreateOption(OptAvail request, AotCriteria criteria, string token)
        {
            if (request.OptRates.Rates.OptRate?.Count <= 0)
                return null;

            var option = new ActivityOption();

            decimal minAdultPrice = 0;
            decimal minChildPrice = 0;
            decimal minInfantPrice = 0;

            foreach (var optRate in request.OptRates.Rates.OptRate)
            {
                if (optRate.PersonRates != null)
                {
                    minAdultPrice = Convert.ToDecimal(optRate.PersonRates.AdultRates.AdultRate) / 100;
                    minChildPrice = Convert.ToDecimal(optRate.PersonRates.ChildRate) / 100;
                    option.OptionType = Constant.PaxBased;
                }
                else if (optRate.RoomRates != null)
                {
                    minAdultPrice = Convert.ToDecimal(optRate.RoomRates.SingleRate) / 100;
                    option.OptionType = Constant.PaxBased;
                }
                else if (optRate.OptionRates != null)
                {
                    minAdultPrice = Convert.ToDecimal(optRate.OptionRates.OptionRate.Min()) / 100;
                    option.OptionType = Constant.GroupBased;
                }
                else if (optRate.ExtrasRates != null)
                {
                    minAdultPrice = Convert.ToDecimal(optRate.ExtrasRates.ExtrasRate.FirstOrDefault()?.AdultRate) / 100;
                    minChildPrice = Convert.ToDecimal(optRate.ExtrasRates.ExtrasRate.FirstOrDefault()?.ChildRate) / 100;
                }
            }

            var adultCount = criteria.NoOfPassengers?.Where(x => x.Key == PassengerType.Adult)
                                 .Select(s => s.Value).FirstOrDefault() ?? 0;
            var childCount = criteria.NoOfPassengers?.Where(x => x.Key == PassengerType.Child)
                                 .Select(s => s.Value).FirstOrDefault() ?? 0;
            var infantCount = criteria.NoOfPassengers?.Where(x => x.Key == PassengerType.Infant)
                                 .Select(s => s.Value).FirstOrDefault() ?? 0;

            var minCostPrice = minAdultPrice * adultCount + minChildPrice * childCount + minInfantPrice * infantCount;
            var currency = request.OptRates.Currency;

            var date = DateTime.Parse(request.OptRates.Rates.OptRate.FirstOrDefault()?.DateFrom);

            option.SupplierOptionCode = request.Opt;
            var capacity = ConfigurationManagerHelper.GetValuefromAppSettings("DefaultCapacity");
            option.Capacity = Convert.ToInt32(capacity);

            var priceAndAvailabiltyCost = new Dictionary<DateTime, PriceAndAvailability>();
            var availabilityValues = request.Avail.Split(' ');
            var dateKey = date.ToString(Constant.DateFormatmmddyy).ToDateTimeExact();

            foreach (var value in availabilityValues)
            {
                var availabilityStatus = AvailabilityStatus.NOTAVAILABLE;
                if (request.Status == Constant.Rq && Convert.ToInt32(value) < 1)
                {
                    availabilityStatus = AvailabilityStatus.ONREQUEST;
                }
                else if (request.Status == Constant.No && Convert.ToInt32(value) < 1)
                {
                    availabilityStatus = AvailabilityStatus.NOTAVAILABLE;
                }
                else
                {
                    availabilityStatus = AvailabilityStatus.AVAILABLE;
                }

                var priceAndAvailability = new DefaultPriceAndAvailability
                {
                    AvailabilityStatus = availabilityStatus
                };

                if (option.OptionType != Constant.PaxBased)
                {
                    var pricingUnit = new PerUnitPricingUnit
                    {
                        Price = minCostPrice
                    };
                    priceAndAvailability.PricingUnits = new List<PricingUnit>
                    {
                        pricingUnit
                    };
                }
                else
                {
                    var adultPricingUnit = new AdultPricingUnit
                    {
                        Price = minAdultPrice
                    };
                    var childPricingUnit = new ChildPricingUnit
                    {
                        Price = minChildPrice
                    };
                    var infantPricingUnit = new InfantPricingUnit
                    {
                        Price = minInfantPrice
                    };

                    priceAndAvailability.PricingUnits = new List<PricingUnit>
                    {
                        adultPricingUnit,
                        childPricingUnit,
                        infantPricingUnit
                    };
                }

                priceAndAvailability.IsSelected = criteria.CheckinDate.Date == date.Date;
                priceAndAvailability.TotalPrice = minCostPrice;

                if (!priceAndAvailabiltyCost.Keys.Contains(dateKey))
                    priceAndAvailabiltyCost.Add(dateKey, priceAndAvailability);

                dateKey = dateKey.AddDays(1);
            }

            option.CostPrice = new Price
            {
                Amount = minCostPrice,
                Currency = new Currency { IsoCode = currency, IsPostFix = true, Name = "", Symbol = "" },
                DatePriceAndAvailabilty = priceAndAvailabiltyCost
            };

            option.Code = request.Opt;
            option.Description = null;
            option.Id = 0;
            switch (request.Status)
            {
                case Constant.Ok:
                    option.AvailabilityStatus = AvailabilityStatus.AVAILABLE;
                    break;

                case Constant.Rq:
                    option.AvailabilityStatus = AvailabilityStatus.ONREQUEST;
                    break;

                case Constant.No:
                    option.AvailabilityStatus = AvailabilityStatus.NOTAVAILABLE;
                    break;
            }

            var cancellationCost = new List<CancellationPrice>();
            var cancellationPrice = new CancellationPrice()
            {
                CancellationFromdate = date,
                CancellationToDate = date.AddHours(Convert.ToInt32(request.OptRates.CancelHours))
            };
            cancellationCost.Add(cancellationPrice);
            option.CancellationPrices = cancellationCost;

            //Cancelation text related
            if (!string.IsNullOrEmpty(request.OptRates.CancelHours))
            {
                var cancellationHours = Convert.ToInt32(request.OptRates.CancelHours);
                option.Cancellable = true;
                option.CancellationText = cancellationHours <= 72 ? string.Format(Constant.CancellationPolicyBeforeTourStart, cancellationHours) : string.Format(Constant.CancellationPolicyBeforeTourStart, Constant.SeventyTwoHours);
            }

            return option;
        }

        private List<DateTime> GetDatesBetween(DateTime startDate, DateTime endDate)
        {
            List<DateTime> allDates = new List<DateTime>();
            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                allDates.Add(date);
            return allDates;
        }

        #endregion Private Methods
    }
}