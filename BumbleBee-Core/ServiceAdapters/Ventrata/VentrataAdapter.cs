using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Ventrata;
using Logger.Contract;
using ServiceAdapters.Ventrata.Ventrata.Commands.Contracts;
using ServiceAdapters.Ventrata.Ventrata.Converters.Contracts;
using ServiceAdapters.Ventrata.Ventrata.Entities;
using ServiceAdapters.Ventrata.Ventrata.Entities.Response;
using Util;
using ConstantsForVentrata = ServiceAdapters.Ventrata.Constants.Constants;
using PassengerTypeForIsango = Isango.Entities.Enums.PassengerType;

namespace ServiceAdapters.Ventrata
{
    public class VentrataAdapter : IVentrataAdapter, IAdapter
    {
        private readonly List<PassengerTypeForIsango> _validPassengerTypes = new List<PassengerTypeForIsango> { PassengerTypeForIsango.Adult, PassengerTypeForIsango.Youth, PassengerTypeForIsango.Child, PassengerTypeForIsango.Infant, PassengerTypeForIsango.Family, PassengerTypeForIsango.Senior, PassengerTypeForIsango.Student, PassengerTypeForIsango.Military };

        private static readonly string VentrataApiBearerToken;
        private static readonly string _className;
        private readonly IAvailabilityCommandHandler _availablityCmdHandler;
        private readonly IBookingReservationCommandHandler _bookingReservationCmdHandler;
        private readonly IPackageReservationCommandHandler _packageReservationCmdHandler;
        private readonly IGetAllProductsCommandHandler _getAllProductsCmdHandler;
        private readonly IBookingConfirmationCommandHandler _bookingConfirmationCmdHandler;
        private readonly IBookingConfirmationConverter _bookingConfirmationConverter;
        private readonly IBookingAndReservationCancellationCommandHandler _bookingAndReservationCancellationCommandHandler;
        private readonly IAvailabilityConverter _availabilityConverter;

        private readonly ICustomQuestionsCommandHandler _customQuestionsCmdHandler;
        private readonly ICustomQuestionsConverter _customQuestionsConverter;


        private readonly ILogger _logger;
        private static readonly int _maxParallelThreadCount;

        static VentrataAdapter()
        {
            //TODO - To Get from Database
            VentrataApiBearerToken = ConfigurationManagerHelper.GetValuefromAppSettings(ConstantsForVentrata.VentrataAPIBearerToken);
            _maxParallelThreadCount = MaxParallelThreadCountHelper.GetMaxParallelThreadCount(ConstantsForVentrata.MaxParallelThreadCount);
            _className = Constants.Constants.VentrataAdapterClassName;
        }

        public VentrataAdapter(IAvailabilityCommandHandler availablityCmdHandler, IAvailabilityConverter availabilityConverter,
            IBookingReservationCommandHandler bookingReservationCmdHandler, IBookingConfirmationCommandHandler bookingConfirmationCmdHandler,
            IBookingConfirmationConverter bookingConfirmationConverter, IBookingAndReservationCancellationCommandHandler bookingAndReservationCancellationCommandHandler,
            IGetAllProductsCommandHandler getAllProductsCommandHandler, ILogger logger,
            IPackageReservationCommandHandler packageReservationCmdHandler,
            ICustomQuestionsCommandHandler customQuestionsCommandHandler,
            ICustomQuestionsConverter customQuestionsConverter)
        {
            _availablityCmdHandler = availablityCmdHandler;
            _availabilityConverter = availabilityConverter;
            _getAllProductsCmdHandler = getAllProductsCommandHandler;
            _bookingReservationCmdHandler = bookingReservationCmdHandler;
            _bookingConfirmationCmdHandler = bookingConfirmationCmdHandler;
            _bookingConfirmationConverter = bookingConfirmationConverter;
            _bookingAndReservationCancellationCommandHandler = bookingAndReservationCancellationCommandHandler;
            _logger = logger;
            _packageReservationCmdHandler = packageReservationCmdHandler;
            _customQuestionsCmdHandler = customQuestionsCommandHandler;
            _customQuestionsConverter = customQuestionsConverter;
        }

        #region Get All Products

        public object GetAllProducts(string supplierBearerToken
            , string token, string baseURL)
        {
            var _returnValue = default(object);
            var _inputContext = new InputContext();
            var request = string.Empty;
            var response = string.Empty;

            if (!string.IsNullOrEmpty(supplierBearerToken))
            {
                _inputContext.SupplierBearerToken = supplierBearerToken;
                _inputContext.MethodType = MethodType.GetAllProducts;
                _inputContext.VentrataBaseURL = baseURL;
                _returnValue = _getAllProductsCmdHandler.Execute(_inputContext, token, out request, out response);
            }
            return _returnValue as List<ProductRes>;
        }

        #endregion Get All Products

        #region Booking Reservation

        public object CreateReservation(VentrataSelectedProduct selectedProduct,
            out string request, out string response, string token,
            List<VentrataPackages> packages)
        {
            request = string.Empty;
            response = string.Empty;
            var _returnValue = default(object);
            var _inputContext = new InputContext();

            if (selectedProduct != null)
            {
                var productOption = selectedProduct.ProductOptions.SingleOrDefault(x => x.IsSelected);
                _inputContext.SupplierBearerToken = selectedProduct.ActivityCode;
                _inputContext.ProductId = ((ActivityOption)productOption)?.VentrataProductId;
                _inputContext.OptionCode = productOption.SupplierOptionCode;
                _inputContext.MethodType = MethodType.BookingReservation;
                _inputContext.AvailabilityId = ((ActivityOption)productOption).RateKey;
                _inputContext.VentrataBaseURL = selectedProduct.VentrataBaseURL;
                _inputContext.VentrataIsPerPaxQRCode = selectedProduct.VentrataIsPerPaxQRCode;
                _inputContext.customers = productOption?.Customers;
                AddUnitIdsInInputContext(productOption, _inputContext, selectedProduct.VentrataPaxMappings);

                //Set pick up point if its requested
                if (!string.IsNullOrEmpty(((ActivityOption)productOption).pickupPointId))
                {
                    _inputContext.pickUpRequested = true;
                    _inputContext.pickUpId = ((ActivityOption)productOption).pickupPointId;
                }
                if (packages != null && packages.Count() > 0)
                {
                    _inputContext.packages = packages;
                    _returnValue = _packageReservationCmdHandler.Execute(_inputContext, token, out request, out response);
                }
                else
                {

                    _returnValue = _bookingReservationCmdHandler.Execute(_inputContext, token, out request, out response);
                }
                //if (_returnValue != null)
                //{
                //    _returnValue = _reservationConverter.Convert((EntityBase)_returnValue);
                //}
            }
            return _returnValue as BookingReservationRes;
        }



        private void AddUnitIdsInInputContext(ProductOption option, InputContext inputContext, List<VentrataPaxMapping> ventrataPaxMappings)
        {
            var travelInfo = option?.TravelInfo;

            inputContext.UnitIdsForBooking = new List<string>();
            var validPassengers = travelInfo.NoOfPassengers.Where(e => _validPassengerTypes.Contains(e.Key));
            foreach (var passengerType in validPassengers)
            {
                for (int passNo = 0; passNo < passengerType.Value; passNo++)
                {
                    var pax = ventrataPaxMappings.FirstOrDefault(x => x.PassengerType == passengerType.Key);
                    inputContext.UnitIdsForBooking.Add(pax.AgeGroupCode);
                }
            }
        }

        private static string GetVentrataPassengerType(PassengerTypeForIsango passengerType)
        {
            switch (passengerType)
            {
                case PassengerTypeForIsango.Adult:
                    return ConstantsForVentrata.Adult;

                case PassengerTypeForIsango.Child:
                    return ConstantsForVentrata.Child;

                case PassengerTypeForIsango.Infant:
                    return ConstantsForVentrata.Infant;

                case PassengerTypeForIsango.Senior:
                    return ConstantsForVentrata.Senior;

                case PassengerTypeForIsango.Student:
                    return ConstantsForVentrata.Student;

                case PassengerTypeForIsango.Family:
                    return ConstantsForVentrata.Family;

                case PassengerTypeForIsango.Youth:
                    return ConstantsForVentrata.Youth;

                case PassengerTypeForIsango.Military:
                    return ConstantsForVentrata.Military;

                default:
                    return ConstantsForVentrata.Adult;
            }
        }

        #endregion Booking Reservation

        #region Availability

        public List<Activity> GetOptionsForVentrataActivity(VentrataAvailabilityCriteria criteria, string token)
        {
            var activities = new List<Activity>();
            var listOfOptionsFromAPI = new Dictionary<string, List<AvailabilityRes>>();
            try
            {
                if (criteria?.SupplierOptionCodesAndProductIdVsApiOptionIds?.Count > 0)
                {
                    var count = 0;
                    foreach (var codeAndProductIdVsApiOptionIds in criteria.SupplierOptionCodesAndProductIdVsApiOptionIds)
                    {
                        var isangoOptionCode = codeAndProductIdVsApiOptionIds.Key.Split('*')[0];
                        var supplierOptionCode = codeAndProductIdVsApiOptionIds.Key.Split('*')[1];
                        var supplierProductIdForThisOptionCode = codeAndProductIdVsApiOptionIds.Key.Split('*')[2];
                        if (!string.IsNullOrEmpty(supplierOptionCode) && !string.IsNullOrEmpty(supplierProductIdForThisOptionCode))
                        {
                            var tempCriteria = new VentrataAvailabilityCriteria()
                            {
                                ActivityId = criteria.ActivityId,
                                SupplierBearerToken = criteria.SupplierBearerToken,
                                SupplierOptionCodesAndProductIdVsApiOptionIds = criteria.SupplierOptionCodesAndProductIdVsApiOptionIds,
                                IsSupplementOffer = criteria.IsSupplementOffer,
                                ActivityMargin = criteria.ActivityMargin,
                                Ages = criteria.Ages,
                                CheckinDate = criteria.CheckinDate,
                                CheckoutDate = criteria.CheckoutDate,
                                IsBundle = criteria.IsBundle,
                                Language = criteria.Language,
                                NoOfPassengers = criteria.NoOfPassengers,
                                PassengerAgeGroupIds = criteria.PassengerAgeGroupIds,
                                PassengerInfo = criteria.PassengerInfo,
                                ProductId = criteria.ProductId,
                                Token = criteria.Token,
                                VentrataBaseURL = criteria.VentrataBaseURL,
                                VentrataPaxMappings = criteria.VentrataPaxMappings.Where(x => x.ServiceOptionId.ToString() == isangoOptionCode)?.ToList()
                            };
                            var optionsFromApi = GetOptionsFromAPI(tempCriteria, supplierOptionCode, supplierProductIdForThisOptionCode, token);
                            if (optionsFromApi?.Count > 0)
                            {
                                optionsFromApi.ForEach(thisApiOption => codeAndProductIdVsApiOptionIds.Value.Add(thisApiOption.Id));
                                listOfOptionsFromAPI.Add(codeAndProductIdVsApiOptionIds.Key, optionsFromApi);
                            }
                        }
                        count++;
                    }

                    //Code block for Converter
                    if (listOfOptionsFromAPI?.Count > 0)
                    {
                        var activityMadeFromApiOptions = _availabilityConverter.Convert(listOfOptionsFromAPI, criteria);
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

        private List<AvailabilityRes> GetOptionsFromAPI(VentrataAvailabilityCriteria criteria, string optionCode, string supplierProductidForThisOptionCode, string loggingToken)
        {
            try
            {
                var apiOptionCode = optionCode;
                var ventrataAvailablity = GetVentrataAvailablity(criteria, VentrataApiBearerToken, loggingToken, apiOptionCode, supplierProductidForThisOptionCode);
                if (ventrataAvailablity == null && ventrataAvailablity?.Count == 0) return new List<AvailabilityRes>(null);
                return ventrataAvailablity;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = _className,
                    MethodName = nameof(GetOptionsFromAPI),
                    Token = loggingToken
                };
                _logger.Error(isangoErrorEntity, ex);
            }
            return new List<AvailabilityRes>(null);
        }

        public List<AvailabilityRes> GetVentrataAvailablity(VentrataAvailabilityCriteria criteria, string bearerToken, string apiLoggingToken, string optionCode, string supplierProductIdForThisOptionCode)
        {
            var _responseValue = default(object);
            var _inputContext = new InputContext();
            if (criteria != null)
            {
                _inputContext.SupplierBearerToken = criteria.SupplierBearerToken;
                _inputContext.VentrataBaseURL = criteria.VentrataBaseURL;
                _inputContext.ProductId = supplierProductIdForThisOptionCode;
                _inputContext.OptionCode = optionCode;
                _inputContext.CheckInDate = criteria.CheckinDate.ToString(ConstantsForVentrata.DateFormat);
                _inputContext.CheckOutDate = criteria.CheckoutDate.ToString(ConstantsForVentrata.DateFormat);
                _inputContext.MethodType = MethodType.Availability;
                _inputContext.PassengerDetails = new List<PassengerDetails>();
                //TODO Ventrata and Isango Passenger type enums mapping should be used instead
                foreach (var thisPassengerInCriteria in criteria.NoOfPassengers)
                {
                    //var paxTypeIsango = thisPassengerInCriteria.Key.ToString().ToLowerInvariant();
                    var ventrataUnit = criteria.VentrataPaxMappings.Where(x => x.PassengerType == thisPassengerInCriteria.Key)?.FirstOrDefault();
                    var ventrataPassengerDetails = new PassengerDetails
                    {
                        //Get Ventrata enum type on the basis of Isango Entities enum type
                        //TODO
                        PassengerType = ventrataUnit?.AgeGroupCode,
                        Quantity = thisPassengerInCriteria.Value
                    };
                    _inputContext.PassengerDetails.Add(ventrataPassengerDetails);
                }

                _responseValue = _availablityCmdHandler.Execute(_inputContext, apiLoggingToken);
            }
            return _responseValue as List<AvailabilityRes>;
        }

        #endregion Availability

        #region Booking Confirmation

        public object CreateBooking(VentrataSelectedProduct ventrataSelectedProduct, string token, out string request, out string response)
        {
            request = string.Empty;
            response = string.Empty;
            var _returnValue = default(object);
            var _inputContext = new InputContext();
            if (ventrataSelectedProduct != null)
            {
                var productOption = ventrataSelectedProduct.ProductOptions.SingleOrDefault(x => x.IsSelected);
                _inputContext.Uuid = ventrataSelectedProduct.Uuid;
                _inputContext.ResellerReference = ventrataSelectedProduct.ResellerReference;
                _inputContext.SupplierBearerToken = ventrataSelectedProduct.ActivityCode;
                _inputContext.VentrataBaseURL = ventrataSelectedProduct.VentrataBaseURL;
                _inputContext.VentrataIsPerPaxQRCode = ventrataSelectedProduct.VentrataIsPerPaxQRCode;
                var leadPassenger = productOption?.Customers.FirstOrDefault(x => x.IsLeadCustomer);
                _inputContext.ContactDetails = new Ventrata.Entities.Contact()
                {
                    FullName = $"{leadPassenger?.FirstName} {leadPassenger?.LastName}",
                    EmailAddress = leadPassenger?.Email,
                    PhoneNo = ventrataSelectedProduct.Supplier.PhoneNumber
                };

                _inputContext.MethodType = MethodType.CreateBooking;
                _returnValue = _bookingConfirmationCmdHandler.Execute(_inputContext, token, out request, out response);

                //Mocking api booking
                /*
                var isMock = false;
                request = File.ReadAllText(@"D:\Sandeep\Isango\API BB\Api Cancellation Policy in Booking Response\15 Ventrata\16 Ventrata 2 ApiREQ Booking.json");
                response = File.ReadAllText(@"D:\Sandeep\Isango\API BB\Api Cancellation Policy in Booking Response\15 Ventrata\16 Ventrata 2 ApiRES Booking.json");
                var bookingConfirmRS = SerializeDeSerializeHelper.DeSerialize<BookingConfirmationRes>(response);
                _returnValue = bookingConfirmRS;
                isMock = true;
                //*/

                if (_returnValue == null)
                {
                    return null;
                }

                _returnValue = _bookingConfirmationConverter.Convert((BookingConfirmationRes)_returnValue);
            }
            return (VentrataApiBookingDetails)_returnValue;
        }

        #endregion Booking Confirmation

        #region Booking and Reservation cancellation

        public string CancelReservationAndBooking(VentrataSelectedProduct ventrataSelectedProduct, string token, out string request, out string response)
        {
            request = string.Empty;
            response = string.Empty;
            var status = string.Empty;
            var _returnValue = default(object);
            var _inputContext = new InputContext();
            if (ventrataSelectedProduct != null)
            {
                _inputContext.Uuid = ventrataSelectedProduct.Uuid;
                _inputContext.MethodType = MethodType.CancelReservationAndBooking;
                _inputContext.ReasonForCancellation = ventrataSelectedProduct.ReasonForCancellation;
                _inputContext.SupplierBearerToken = ventrataSelectedProduct.ActivityCode;
                _inputContext.VentrataBaseURL = ventrataSelectedProduct.VentrataBaseURL;
                _inputContext.VentrataIsPerPaxQRCode = ventrataSelectedProduct.VentrataIsPerPaxQRCode;
                _returnValue = _bookingAndReservationCancellationCommandHandler.Execute(_inputContext, token, out request, out response);
                if (_returnValue == null)
                {
                    return null;
                }

                status = ((BookingConfirmationRes)_returnValue)?.Status;
            }

            return status;
        }

        #endregion Booking and Reservation cancellation

        public object GetCustomQuestions(string supplierBearerToken
           , string token, string baseURL, string id)
        {
            var _returnValue = default(object);
            var _inputContext = new InputContext();
            var request = string.Empty;
            var response = string.Empty;

            if (!string.IsNullOrEmpty(id))
            {
                _inputContext.SupplierBearerToken = supplierBearerToken;
                _inputContext.MethodType = MethodType.CustomQuestions;
                _inputContext.VentrataBaseURL = baseURL;
                _inputContext.Uuid = id;
                _returnValue = _customQuestionsCmdHandler.Execute(_inputContext, token, out request, out response);
            }
            return _returnValue as CustomQuestions;
        }

    }
}