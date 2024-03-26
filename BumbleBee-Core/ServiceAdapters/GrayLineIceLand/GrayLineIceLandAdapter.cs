using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Booking;
using Isango.Entities.Enums;
using Isango.Entities.GrayLineIceLand;
using ServiceAdapters.GrayLineIceLand.Constants;
using ServiceAdapters.GrayLineIceLand.GrayLineIceLand.Commands.Contracts;
using ServiceAdapters.GrayLineIceLand.GrayLineIceLand.Converters.Contracts;
using ServiceAdapters.GrayLineIceLand.GrayLineIceLand.Entities;
using ServiceAdapters.GrayLineIceLand.GrayLineIceLand.Entities.RequestResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Util;
using AgrGroup = Isango.Entities.ConsoleApplication.AgeGroup.GrayLineIceLand;

namespace ServiceAdapters.GrayLineIceLand
{
    public class GrayLineIceLandAdapter : IGrayLineIceLandAdapter, IAdapter
    {
        #region "Private Members"

        private readonly ICancelBookingCommandHandler _cancelBookingCommandHandler;
        private readonly ICreateBookingCmdHandler _createBookingCmdHandler;
        private readonly IGetAvailabilityCommandHandler _getAvailabilityCommandHandler;
        private readonly IGetAgeGroupsCommandHandler _getAgeGroupsCommandHandler;
        private readonly IGetPickupLocationsCommandHandler _getPickupLocationsCommandHandler;

        private readonly IAvailabilityConverter _availabilityConverter;
        private readonly IBookingConverter _bookingConverter;
        private readonly int _agentProfileId;

        #endregion "Private Members"

        #region "Constructor"

        public GrayLineIceLandAdapter(ICancelBookingCommandHandler cancelBookingCommandHandler,
            ICreateBookingCmdHandler createBookingCmdHandler, IGetAvailabilityCommandHandler getAvailabilityCommandHandler, IGetAgeGroupsCommandHandler getAgeGroupsCommandHandler, IGetPickupLocationsCommandHandler getPickupLocationsCommandHandler,
              IAvailabilityConverter availabilityConverter, IBookingConverter bookingConverter)
        {
            _cancelBookingCommandHandler = cancelBookingCommandHandler;
            _createBookingCmdHandler = createBookingCmdHandler;
            _getAvailabilityCommandHandler = getAvailabilityCommandHandler;
            _getAgeGroupsCommandHandler = getAgeGroupsCommandHandler;
            _getPickupLocationsCommandHandler = getPickupLocationsCommandHandler;

            _availabilityConverter = availabilityConverter;
            _bookingConverter = bookingConverter;
            try
            {
                _agentProfileId = Convert.ToInt32(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.GrayLineAgentProfileID));
            }
            catch (Exception ex)
            {
                _agentProfileId = 0;
            }
        }

        #endregion "Constructor"

        public async Task<List<Activity>> GetAvailabilityAndPriceAsync(GrayLineIcelandCriteria criteria, string token)
        {
            var activities = new List<Activity>();
            var activity = new Activity();
            if (AgentAuthenticateDetails.Instance.IsAuthenticated)
            {
                var inputContext = new InputContext
                {
                    MethodType = MethodType.Availability,
                    Adults = criteria.NoOfPassengers?.Where(x => x.Key == PassengerType.Adult).Select(s => s.Value).FirstOrDefault() ?? 0,
                    DateFrom = criteria.CheckinDate,
                    DateTo = criteria.CheckoutDate,
                    Children = criteria.NoOfPassengers?.Where(x => x.Key == PassengerType.Child).Select(s => s.Value).FirstOrDefault() ?? 0,
                    ChildAges = criteria.Ages.Where(x => x.Key == PassengerType.Child).Select(x => Convert.ToInt32(x.Value)).ToList(),
                    TourNumber = criteria.ActivityCode,
                    Youths = criteria.NoOfPassengers?.Where(x => x.Key == PassengerType.Youth).Select(s => s.Value).FirstOrDefault() ?? 0
                };
                if (Int32.TryParse(criteria.Language, out var languageCode))
                {
                    inputContext.Language = languageCode;
                }

                inputContext.CurrencyCode = Constant.ISK;
                inputContext.PaxAgeGroupIds = criteria.PaxAgeGroupIds;
                inputContext.AgentProfileId = Convert.ToInt32(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.GrayLineAgentProfileID));
                inputContext.AuthToken = AgentAuthenticateDetails.Instance.AuthRs.Access_token;
                var returnValue = await _getAvailabilityCommandHandler.ExecuteAsync(inputContext, token);
                var response = (HttpResponseMessage)returnValue;

                returnValue = response.IsSuccessStatusCode ? SerializeDeSerializeHelper.DeSerialize<ToursAvailabilityRS>(response.Content.ReadAsStringAsync().Result) : null;

                if (returnValue != null)
                {
                    var toursAvailabilityResponse = (ToursAvailabilityRS)returnValue;
                    returnValue = _availabilityConverter.Convert(toursAvailabilityResponse);
                }
                activity = (Activity)returnValue;
                activities.Add(activity);
                return activities;
            }
            return null;
        }

        public List<Activity> GetAvailabilityAndPrice(GrayLineIcelandCriteria criteria, string token)
        {
            if (!AgentAuthenticateDetails.Instance.IsAuthenticated) return null;

            var activities = new List<Activity>();
            var activity = new Activity();

            #region Prepare InputContext

            var inputContext = new InputContext
            {
                MethodType = MethodType.Availability,
                Adults = criteria.NoOfPassengers.FirstOrDefault(x => x.Key == PassengerType.Adult && x.Value != 0).Value,
                DateFrom = criteria.CheckinDate,
                DateTo = criteria.CheckoutDate,
                Children = criteria.NoOfPassengers.FirstOrDefault(x => x.Key == PassengerType.Child && x.Value != 0).Value,
                ChildAges = criteria.Ages.Where(x => x.Key == PassengerType.Child).Select(x => Convert.ToInt32(x.Value)).ToList(),
                TourNumber = criteria.ActivityCode,
                Youths = criteria.NoOfPassengers.FirstOrDefault(x => x.Key == PassengerType.Youth && x.Value != 0).Value
            };
            if (Int32.TryParse(criteria.Language, out var languageCode))
            {
                inputContext.Language = languageCode;
            }
            inputContext.CurrencyCode = Constant.ISK;
            inputContext.PaxAgeGroupIds = criteria.PaxAgeGroupIds;
            inputContext.AgentProfileId = _agentProfileId;
            inputContext.AuthToken = AgentAuthenticateDetails.Instance.AuthRs.Access_token;

            #endregion Prepare InputContext

            #region Adapter Call

            var returnValue = _getAvailabilityCommandHandler.Execute(inputContext, token);
            var response = returnValue;
            returnValue = SerializeDeSerializeHelper.DeSerialize<ToursAvailabilityRS>(response.ToString());
            if (returnValue != null)
            {
                var toursAvailabilityResponse = returnValue as ToursAvailabilityRS;
                if (returnValue != null)
                {
                    returnValue = _availabilityConverter.Convert(toursAvailabilityResponse, criteria);
                }
            }
            activity = returnValue as Activity;
            if (activity != null)
                activities.Add(activity);

            #endregion Adapter Call

            return activities;
        }

        public async Task<Booking> CreateBookingAsync(List<GrayLineIceLandSelectedProduct> selectedProducts, string token)
        {
            if (AgentAuthenticateDetails.Instance.IsAuthenticated)
            {
                var inputContext = new InputContext
                {
                    MethodType = MethodType.CreateBooking,
                    SelectedProducts = selectedProducts,
                    AgentProfileId = Convert.ToInt32(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.GrayLineAgentProfileID)),
                    AuthToken = AgentAuthenticateDetails.Instance.AuthRs.Access_token,
                    CurrencyCode = selectedProducts.FirstOrDefault()?.SupplierCurrency
                };
                if (selectedProducts.Count <= 0) return null;
                var returnValue = await _createBookingCmdHandler.ExecuteAsync(inputContext, token);
                var response = (HttpResponseMessage)returnValue;
                returnValue = response.IsSuccessStatusCode ? SerializeDeSerializeHelper.DeSerialize<BookingRS>(response.Content.ReadAsStringAsync().Result) : null;

                if (returnValue != null)
                    returnValue = _bookingConverter.Convert(inputContext);
                return (Booking)returnValue;
            }

            return null;
        }

        public Booking CreateBooking(List<GrayLineIceLandSelectedProduct> selectedProducts, string bookingReference, string token, out string requestJson, out string responseJson)
        {
            requestJson = string.Empty;
            responseJson = string.Empty;

            if (AgentAuthenticateDetails.Instance.IsAuthenticated)
            {
                var inputContext = new InputContext
                {
                    MethodType = MethodType.CreateBooking,
                    SelectedProducts = selectedProducts,
                    AgentProfileId = Convert.ToInt32(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.GrayLineAgentProfileID)),
                    AuthToken = AgentAuthenticateDetails.Instance.AuthRs.Access_token,
                    CurrencyCode = Constant.ISK,
                    BookingReference = bookingReference
                };
                if (selectedProducts.Count <= 0) return null;
                var response = _createBookingCmdHandler.Execute(inputContext, token, out requestJson, out responseJson);
                var bookingRS = SerializeDeSerializeHelper.DeSerialize<BookingRS>(response.ToString());
                bookingRS.SelectedProduct = selectedProducts;

                if (bookingRS == null || bookingRS.BookingNumber == 0 || !bookingRS.OrderDetails.Any())
                    return null;

                return (Booking)_bookingConverter.Convert(bookingRS);
            }
            return null;
        }

        public async Task<Boolean> DeleteBookingAsync(Booking booking, string token)
        {
            if (AgentAuthenticateDetails.Instance.IsAuthenticated)
            {
                var inputContext = new InputContext
                {
                    MethodType = MethodType.CancelBooking,
                    AgentProfileId = Convert.ToInt32(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.GrayLineAgentProfileID)),
                    AuthToken = AgentAuthenticateDetails.Instance.AuthRs.Access_token,
                    BookingNumber = booking.ReferenceNumber
                };
                var returnValue = await _cancelBookingCommandHandler.ExecuteCancelAsync(inputContext, token);
                return ((CancelBookingRS)returnValue).ErrorCode == 0;
            }

            return false;
        }

        public Boolean DeleteBooking(string supplierReferenceNumber, string token)
        {
            bool result = false;
            var requestText = string.Empty;
            var responseText = string.Empty;
            try
            {
                if (AgentAuthenticateDetails.Instance.IsAuthenticated)
                {
                    var inputContext = new InputContext
                    {
                        MethodType = MethodType.CancelBooking,
                        AgentProfileId = Convert.ToInt32(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.GrayLineAgentProfileID)),
                        AuthToken = AgentAuthenticateDetails.Instance.AuthRs.Access_token,
                        BookingNumber = supplierReferenceNumber
                    };
                    var returnValue = _cancelBookingCommandHandler.ExecuteCancel(inputContext, token, out requestText, out responseText);
                    result = ((CancelBookingRS)returnValue).ErrorCode == 0;
                }
            }
            catch (Exception)
            {
                result = false;
            }

            return result;
        }

        public Boolean DeleteBooking(string supplierReferenceNumber, string token, out string requestXml, out string responseXml)
        {
            requestXml = "";
            responseXml = "";
            if (AgentAuthenticateDetails.Instance.IsAuthenticated)
            {
                var inputContext = new InputContext
                {
                    MethodType = MethodType.CancelBooking,
                    AgentProfileId = Convert.ToInt32(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.GrayLineAgentProfileID)),
                    AuthToken = AgentAuthenticateDetails.Instance.AuthRs.Access_token,
                    BookingNumber = supplierReferenceNumber
                };
                var returnValue = _cancelBookingCommandHandler.ExecuteCancel(inputContext, token, out requestXml, out responseXml);
                return ((CancelBookingRS)returnValue).ErrorCode == 0;
            }

            return false;
        }

        public async Task<Dictionary<int, List<AgrGroup.AgeGroup>>> GetAgeGroupsByToursAsync(List<IsangoHBProductMapping> products, string token)
        {
            var tourAgeGroups = new Dictionary<int, List<AgrGroup.AgeGroup>>();
            try
            {
                if (products?.Count > 0)
                {
                    foreach (var product in products)
                    {
                        var inputContext = new InputContext
                        {
                            MethodType = MethodType.AgeGroup,
                            AgentProfileId = Convert.ToInt32(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.GrayLineAgentProfileID)),
                            AuthToken = AgentAuthenticateDetails.Instance.AuthRs.Access_token,
                            TourNumber = product.HotelBedsActivityCode
                        };
                        var response = await _getAgeGroupsCommandHandler.ExecuteStringAsync(inputContext, token);
                        var jsonResult = SerializeDeSerializeHelper.DeSerialize<List<AgrGroup.AgeGroup>>(response);
                        tourAgeGroups.Add(product.IsangoHotelBedsActivityId, jsonResult);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return tourAgeGroups;
        }

        public async Task<Dictionary<int, List<AgrGroup.Pickuplocation>>> GetAllPickupLocationsAsync(List<IsangoHBProductMapping> grayLineProducts, string token)
        {
            var pickupLocationsDict = new Dictionary<int, List<AgrGroup.Pickuplocation>>();

            if (grayLineProducts?.Count > 0)
            {
                foreach (var product in grayLineProducts)
                {
                    var inputContext = new InputContext
                    {
                        MethodType = MethodType.TourAndAvailabilityPickupLocations,
                        AgentProfileId = Convert.ToInt32(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.GrayLineAgentProfileID)),
                        AuthToken = AgentAuthenticateDetails.Instance.AuthRs.Access_token,
                        TourNumber = product.HotelBedsActivityCode
                    };

                    var returnValue = await _getPickupLocationsCommandHandler.ExecuteAsync(inputContext, token);
                    var response = (HttpResponseMessage)returnValue;
                    var jsonResult = response.IsSuccessStatusCode ? SerializeDeSerializeHelper.DeSerializeWithIsoDateTime<ToursAvailabilityRS>(response.Content.ReadAsStringAsync().Result) : null;

                    if (jsonResult?.TourDepartures?.FirstOrDefault()?.PickUpLocations?.Count() > 0)
                    {
                        pickupLocationsDict.Add(product.IsangoHotelBedsActivityId, jsonResult.TourDepartures.FirstOrDefault()?.PickUpLocations.ToList());
                    }
                }
            }

            return pickupLocationsDict;
        }
    }
}