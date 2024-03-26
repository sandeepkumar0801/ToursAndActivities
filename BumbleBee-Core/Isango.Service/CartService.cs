using Isango.Entities;
using Isango.Entities.Bokun;
using Isango.Entities.HotelBeds;
using Isango.Entities.PrioHub;
using Isango.Entities.Rezdy;
using Isango.Entities.TourCMS;
using Isango.Entities.Ventrata;
using Isango.Service.Contract;
using Logger.Contract;
using ServiceAdapters.Bokun;
using ServiceAdapters.Bokun.Bokun.Entities.GetPickupPlaces;
using ServiceAdapters.FareHarbor;
using ServiceAdapters.GlobalTix;
using ServiceAdapters.HotelBeds;
using ServiceAdapters.Rezdy;
using ServiceAdapters.Ventrata;
using ServiceAdapters.Ventrata.Ventrata.Entities.Response;
using Util;
using RezdyAnswerOption = Isango.Entities.Rezdy.AnswerOption;
namespace Isango.Service
{
    public class CartService : ICartService
    {
        private readonly ILogger _log;
        private readonly ITicketAdapter _ticketAdapter;
        private readonly IBokunAdapter _bokunAdapter;
        private readonly IMasterService _masterService;
        private readonly IRezdyAdapter _rezdyAdapter;
        private readonly IGlobalTixAdapter _globalTixAdapter;
        private readonly IFareHarborAdapter _fareHarborAdapter;
        private readonly IVentrataAdapter _ventrataAdapter;

        public CartService(ITicketAdapter ticketAdapter, ILogger log, IBokunAdapter bokunAdapter, IMasterService masterService, IRezdyAdapter rezdyAdapter, IGlobalTixAdapter globalTixAdapter, IFareHarborAdapter fareHarborAdapter, IVentrataAdapter ventrataAdapter)
        {
            _ticketAdapter = ticketAdapter;
            _log = log;
            _bokunAdapter = bokunAdapter;
            _masterService = masterService;
            _rezdyAdapter = rezdyAdapter;
            _globalTixAdapter = globalTixAdapter;
            _fareHarborAdapter = fareHarborAdapter;
            _ventrataAdapter = ventrataAdapter;
        }

        public HotelBedsSelectedProduct GetExtraDetailsForHotelBeds(HotelBedsSelectedProduct selectedProduct, string token)
        {
            try
            {
                var tmpHbProduct = _ticketAdapter.GetTicketPrice(selectedProduct, "ISANGOUK1013><ISANGOUK1013", token);
                return tmpHbProduct;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CartService",
                    MethodName = "GetExtraDetailsForHotelBeds",
                    Token = token,
                    Params = $"CartService|GetExtraDetailsForHotelBeds|{SerializeDeSerializeHelper.Serialize(selectedProduct)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public ExtraDetailsForBokun GetExtraDetailsForBokun(BokunSelectedProduct selectedProduct, string token)
        {
            try
            {
                var dictPickup = new Dictionary<int, string>();
                var dictDropOff = new Dictionary<int, string>();

                GetPickupPlacesRS pickupPlaces = null;
                try
                {
                    pickupPlaces = _bokunAdapter.GetPickupPlaces(selectedProduct.Id, token);
                    if (pickupPlaces?.PickupPlaces?.Count > 0)
                    {
                        foreach (var place in pickupPlaces.PickupPlaces)
                        {
                            if (!dictPickup.ContainsKey(place.Id) && !string.IsNullOrWhiteSpace(place.Title))
                            {
                                dictPickup.Add(place.Id, place.Title);
                            }
                        }
                    }
                    if (pickupPlaces?.DropoffPlaces?.Count > 0)
                    {
                        foreach (var place in pickupPlaces.DropoffPlaces)
                        {
                            if (!dictDropOff.ContainsKey(place.Id) && !string.IsNullOrWhiteSpace(place.Title))
                            {
                                dictDropOff.Add(place.Id, place.Title);
                            }
                        }
                    }
                }
                catch
                {
                    //ignore
                }
                var pickupPlace = dictPickup?.FirstOrDefault(x => x.Key > 0);
                var dropoffPlace = dictDropOff?.FirstOrDefault(x => x.Key > 0);

                try
                {
                    if (pickupPlace != null)
                    {
                        selectedProduct.HotelPickUpLocation = $"{ pickupPlace?.Key}-{pickupPlace.Value}";
                    }
                    if (dropoffPlace != null)
                    {
                        selectedProduct.HotelDropoffLocation = $"{ dropoffPlace?.Key}-{dropoffPlace.Value}";
                    }
                    var questions = _bokunAdapter.CheckoutOptions(selectedProduct, token);
                    if (questions?.Count > 0)
                    {
                        selectedProduct.Questions = new List<Entities.Bokun.Question>();
                        var DuplicateQuestionIds = new string[] { "firstName", "lastName", "email" };
                        //string[] DuplicateQuestionIds = { "test", "test2", "test3" };
                        foreach (var question in questions)
                        { 
                            if (question != null)
                            {
                                if (!DuplicateQuestionIds.Contains(question.QuestionId) && question.Required)
                                {
                                    if ((selectedProduct?
                                        .Questions?
                                        .Any(x => x.QuestionId == question.QuestionId
                                               || x.QuestionType == question.QuestionType
                                            )) == false
                                    )
                                    {
                                        if (selectedProduct?.Questions?.Contains(question) == false
                                            && !string.IsNullOrWhiteSpace(question?.Label) 
                                        )
                                        {
                                            selectedProduct.Questions.Add(question);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch
                {
                    //ignore
                }
                var result = new ExtraDetailsForBokun
                {
                    DropoffDetails = dictDropOff,
                    PickupDetails = dictPickup,
                    Questions = selectedProduct?.Questions
                };
                return result;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CartService",
                    MethodName = "GetExtraDetailsForBokun",
                    Token = token,
                    Params = $"CartService|GetExtraDetailsForBokun|{SerializeDeSerializeHelper.Serialize(selectedProduct)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public List<PickupLocation> GetExtraDetailsForGrayLineIceLand(int activityId, string token)
        {
            try
            {
                return _masterService.GetPickupLocationsByActivityAsync(activityId).Result;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CartService",
                    MethodName = "GetExtraDetailsForGrayLineIceLand",
                    Token = token,
                    Params = $"CartService|GetExtraDetailsForGrayLineIceLand|{activityId}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public ExtraDetailsForRezdy GetExtraDetailsForRezdy(string ProductCode, int passengerCount, string token)
        {
            var pickUpDetails = new Dictionary<int, string>();
            var pickUpLocations = new List<RezdyPickUpLocation>();
            try
            {
                var isPickLocationInBookingField = false;
                var productDetails = _rezdyAdapter.GetProductDetails(ProductCode, token);
                var rezdyLabel = _masterService.GetLabelDetailsAsync().GetAwaiter().GetResult();
                var fieldsPerBooking = productDetails.BookingFields.ToList();
                fieldsPerBooking = fieldsPerBooking?.Where(x => x.RequiredPerBooking == true || x.RequiredPerParticipant == true)?.ToList();
                var bookingQuestions = new List<BookingQuestions>();
                var isRequired = false;
                foreach (var bookingField in fieldsPerBooking)
                {
                    if (!(rezdyLabel.Any(x => x.Label == bookingField.Label)))
                    {
                        if (bookingField.Label.IndexOf("pick", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            isPickLocationInBookingField = true;
                        }

                        if (bookingField.RequiredPerParticipant == true || bookingField.RequiredPerBooking == true || bookingField.VisiblePerBooking == true || bookingField.RequiredPerParticipant == true)
                        {
                            isRequired = true;
                        }
                        var questionField = new BookingQuestions
                        {
                            Question = bookingField.Label.ToString(),
                            Required = isRequired
                        };

                        if (bookingField.FieldType == "List" && !string.IsNullOrEmpty(bookingField.ListOptions))
                        {
                            var answers = bookingField.ListOptions?.Split(',').ToList();
                            questionField.AnswerOptions = new List<RezdyAnswerOption>();
                            answers?.ForEach(x =>
                            {
                                questionField.AnswerOptions.Add(
                                    new Entities.Rezdy.AnswerOption
                                    {
                                        Value = x,
                                        Label = x
                                    });
                            });
                            bookingQuestions.Add(questionField);
                        }
                        else if (bookingField.FieldType != "List")
                        {
                            bookingQuestions.Add(questionField);
                        }

                    }
                }

                if (!isPickLocationInBookingField)
                {
                    if (!string.IsNullOrEmpty(productDetails.PickupId))
                    {
                        pickUpLocations = _rezdyAdapter.GetPickUpLocationDetails(Convert.ToInt32(productDetails.PickupId), token);
                        if (pickUpLocations != null)
                        {
                            pickUpLocations.ForEach(x =>
                            {
                                pickUpDetails.Add(x.Id, x.LocationName);
                            });
                        }
                    }
                }

                // Commented It: This Code create the duplicate questions
                //for (var i = 1; i < passengerCount; i++)
                //{
                //    bookingQuestions.AddRange(bookingQuestions);
                //}

                var extraDetailsForRezdy = new ExtraDetailsForRezdy
                {
                    ExtraDetailsBookingQuestions = bookingQuestions,
                    PickupDetails = pickUpDetails,
                    RezdyPickUpLocations = pickUpLocations,
                    PickUpId = Convert.ToInt32(productDetails.PickupId)
                };

                return extraDetailsForRezdy;
            }
            catch (Exception ex)
            {
                _log.Error($"CartService|GetExtraDetailsForRezdy|{ProductCode},{passengerCount},{token}", ex);
                throw;
            }
        }


        public List<VentrataExtraQuestion> GetCustomQuestionsForVentrata(string supplierBearerToken, string baseURL,
            string ventrataProductId, string token,string supplierOptionCode)
        {
           var lstData = new List<VentrataExtraQuestion>();
           try
            {
                var customQuestions = _ventrataAdapter.GetCustomQuestions(supplierBearerToken, 
                    token, baseURL, ventrataProductId);
                
                var finalcustomQuestions= (CustomQuestions)customQuestions;
                if (finalcustomQuestions != null)
                {
                     finalcustomQuestions?.Options?.RemoveAll(z => z.Id != supplierOptionCode);
                     if (finalcustomQuestions != null)
                     {
                        if (finalcustomQuestions?.Options != null && finalcustomQuestions?.Options.Count>0)
                        {
                            foreach (var item in finalcustomQuestions?.Options?.FirstOrDefault()?.Units)
                            {
                                foreach (var itemInner in item?.Questions)
                                {
                                    var getQuest = new VentrataExtraQuestion
                                    {
                                        Id = itemInner.Id,
                                        Description = itemInner.Description,
                                        InputType = itemInner.InputType,
                                        Label = itemInner.Label,
                                        Required = itemInner.Required,
                                        SelectOptions = itemInner.SelectOptions
                                    };
                                    if (!lstData.Contains(getQuest))
                                    {
                                        lstData.Add(getQuest);
                                    }
                                }
                            }
                        }
                     }
                    lstData = lstData?.GroupBy(e => e.Id)?.Select(e => e.FirstOrDefault())?.ToList();
                }
                return lstData;
            }
            catch (Exception ex)
            {
                _log.Error($"CartService|GetCustomQuestionsForVentrata|{supplierBearerToken},{ventrataProductId},{token}", ex);
                throw;
            }
        }

        /// <summary>
        /// Get ExtraDetails For GlobalTix
        /// </summary>
        /// <param name="ticketTypeIds"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Tuple<List<ContractQuestion>, Dictionary<int, string>> GetExtraDetailsForGlobalTix(List<string> ticketTypeIds, string token)
        {
            try
            {
                var ticketTypeDetails = new Dictionary<int, ServiceAdapters.GlobalTix.GlobalTix.Entities.RequestResponseModels.TicketTypeDetail>();
                return _globalTixAdapter.GetTicketTypeDetail(ticketTypeIds, token);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CartService",
                    MethodName = "GetExtraDetailsForGlobalTix",
                    Token = token,
                    Params = $"CartService|GetExtraDetailsForGlobalTix|{SerializeDeSerializeHelper.Serialize(ticketTypeIds)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }

        }

        public ExtraDetailsForTourCMS GetExtraDetailsForTourCMS(
           TourCMSSelectedProduct selectedProduct,
           List<PickupPointsForTourCMS> pickupPointsForTourCMSList,
           List<QuestionsForTourCMS> questionsForTourCMSList,
           string token)
        {
            try
            {
                var dictPickup = new Dictionary<int, string>();
                try
                {
                    if (pickupPointsForTourCMSList?.Count > 0)
                    {
                        foreach (var place in pickupPointsForTourCMSList)
                        {
                            if (!dictPickup.ContainsKey(Convert.ToInt32(place.PickupId)) && !string.IsNullOrWhiteSpace(place.PickUpName))
                            {
                                dictPickup.Add(Convert.ToInt32(place.PickupId), place.PickUpName + " ," + place.Description);
                            }
                        }
                    }

                }
                catch
                {
                    //ignore
                }
                var pickupPlace = dictPickup?.FirstOrDefault(x => x.Key > 0);
                try
                {
                    if (pickupPlace != null)
                    {
                        selectedProduct.HotelPickUpLocation = $"{ pickupPlace?.Key}-{pickupPlace.Value}";
                    }

                    var questions = questionsForTourCMSList;
                    if (questions?.Count > 0)
                    {
                        selectedProduct.Questions = new List<QuestionsForTourCMS>();
                        foreach (var question in questions)
                        {
                            if (question != null)
                            {
                                if (selectedProduct?.Questions?.Contains(question) == false
                                            && !string.IsNullOrWhiteSpace(question?.Code)
                                        )
                                {
                                    selectedProduct.Questions.Add(question);
                                }
                            }
                        }
                    }
                }
                catch
                {
                    //ignore
                }
                var result = new ExtraDetailsForTourCMS
                {

                    PickupDetails = dictPickup,
                    Questions = selectedProduct?.Questions
                };
                return result;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CartService",
                    MethodName = "GetExtraDetailsForTourCMS",
                    Token = token,
                    Params = $"CartService|GetExtraDetailsForTourCMS|{SerializeDeSerializeHelper.Serialize(selectedProduct)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public ExtraDetailsForPrioHub GetExtraDetailsForPrioHub(
          List<PickUpPointForPrioHub> pickUpPointForPrioHubList,
          string token)
        {
            try
            {
                var dictPickup = new Dictionary<int, string>();
                try
                {
                    if (pickUpPointForPrioHubList?.Count > 0)
                    {
                        foreach (var place in pickUpPointForPrioHubList)
                        {
                            if (!dictPickup.ContainsKey(Convert.ToInt32(place.PickupPointId)) && !string.IsNullOrWhiteSpace(place.PickupPointName))
                            {
                                dictPickup.Add(Convert.ToInt32(place.PickupPointId), place.PickupPointName + " ," + place.PickupPointDescription);
                            }
                        }
                    }

                }
                catch
                {
                    //ignore
                }
                var result = new ExtraDetailsForPrioHub
                {
                  PickupDetails = dictPickup
                };
                return result;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CartService",
                    MethodName = "GetExtraDetailsForTourCMS",
                    Token = token,
                    Params = $"CartService|GetExtraDetailsForTourCMS|{SerializeDeSerializeHelper.Serialize(pickUpPointForPrioHubList)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public Dictionary<int, string> GetExtraDetailsForFareHarbor(string availabilityPK, string supplierName, string userKey, string token)
        {
            try
            {
                var pickupLocs = new Dictionary<int, string>();
                var Lodgings = _fareHarborAdapter.GetLodgingsAvailability(supplierName, availabilityPK, userKey, token);
                if(Lodgings != null && Lodgings.lodgings?.Count > 0)
                {
                    foreach(var item in Lodgings.lodgings)
                    {
                        pickupLocs.Add(item.pk, item.name);
                    }
                    return pickupLocs;
                }
                return null;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CartService",
                    MethodName = "GetExtraDetailsForFareHarbor",
                    Token = token,
                    Params = $"CartService|GetExtraDetailsForFareHarbor|{availabilityPK}_{supplierName}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }

        }
    }
}
