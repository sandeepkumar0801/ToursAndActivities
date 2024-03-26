using Isango.Entities;
using Isango.Entities.Bokun;
using Isango.Entities.Enums;
using Isango.Service.Contract;
using System;
using System.Collections.Generic;
using TableStorageOperations.Contracts;
using TableStorageOperations.Models.AdditionalPropertiesModels.Availabilities;
using System.Linq;
using Util;
using WebAPI.Mapper;
using Constant = WebAPI.Constants.Constant;
using PaymentExtraDetailRQ = WebAPI.Models.RequestModels.PaymentExtraDetail;
using PaymentExtraDetailRS = WebAPI.Models.ResponseModels.PaymentExtraDetail;
using Question = WebAPI.Models.ResponseModels.Question;
using Isango.Entities.Ventrata;
using Logger.Contract;
using Isango.Entities.TourCMS;
using Isango.Entities.PrioHub;
using ILogger = Logger.Contract.ILogger;

namespace WebAPI.Helper
{
    /// <summary>
    /// Helper method for checkout controller
    /// </summary>
    public class CheckoutHelper
    {
        private readonly ICartService _cartService;
        private readonly ITableStorageOperation _TableStorageOperations;
        private readonly CheckoutMapper _checkoutMapper;
        private readonly ILogger _log;
        /// <summary>
        ///
        /// </summary>
        public CheckoutHelper(ICartService cartService,
            ITableStorageOperation TableStorageOperations,
            CheckoutMapper checkoutMapper, ILogger log = null)
        {
            _cartService = cartService;
            _TableStorageOperations = TableStorageOperations;
            _checkoutMapper = checkoutMapper;
            _log = log;
        }

        /// <summary>
        ///  This operation is used to get extra details required for products on confirm booking
        /// </summary>
        /// <param name="extraDetailsRequest"></param>
        /// <param name="baseAvailabilitiesEntity"></param>
        /// <param name="token"></param>
        /// <param name="extraDetailsResponse"></param>
        /// <returns></returns>
        public List<PaymentExtraDetailRS> GetPaymentExtraDetails(PaymentExtraDetailRQ extraDetailsRequest, BaseAvailabilitiesEntity baseAvailabilitiesEntity, string token, ref List<PaymentExtraDetailRS> extraDetailsResponse)
        {
            try
            {
                var apiType = (APIType)Enum.Parse(typeof(APIType), baseAvailabilitiesEntity.ApiType.ToString());
                switch (apiType)
                {
                    case APIType.Undefined:
                        {
                            try
                            {
                                var IsangoAvailabilities = _TableStorageOperations.RetrieveData<DefaultAvailabilities>(extraDetailsRequest.ReferenceId, token);
                                var pickupLocations = SerializeDeSerializeHelper.DeSerialize<List<ActivityPickupLocations>>(IsangoAvailabilities.PickupLocations);
                                var pickupvalues = new Dictionary<int, string>();
                                foreach (var location in pickupLocations)
                                {
                                    if (location.Serviceoptionid == 0 || (baseAvailabilitiesEntity.ServiceOptionId == location.Serviceoptionid))
                                    {
                                        pickupvalues.Add(location.ID ?? 0, location.Pickuplocation);
                                    }
                                }
                                var extraDetails = _checkoutMapper.MapPaymentExtraDetailsResponse(baseAvailabilitiesEntity, null, pickupvalues);
                                if (extraDetails != null)
                                {
                                    extraDetailsResponse.Add(extraDetails);
                                }
                            }
                            catch (Exception ex)
                            {
                                //ignore - booking should be smooth
                            }
                            break;
                        }
                    case APIType.Hotelbeds:
                        {
                            var ticketAvailabilities = _TableStorageOperations.RetrieveData<TicketsAvailabilities>(extraDetailsRequest.ReferenceId, token);
                            var hotelBedsSelectedProduct = _checkoutMapper.GetHotelBedsSelectedProduct(ticketAvailabilities, extraDetailsRequest);
                            var extraDetails = _checkoutMapper.MapPaymentExtraDetailsResponse(baseAvailabilitiesEntity, _checkoutMapper.MapHotelBedQuestions(hotelBedsSelectedProduct?.ContractQuestions), null);

                            if (extraDetails?.DropoffLocations?.Any() == true
                                || extraDetails?.PickupLocations?.Any() == true
                                || extraDetails?.Questions?.Any() == true
                            )
                            {
                                extraDetailsResponse.Add(extraDetails);
                            }
                            break;
                        }
                    case APIType.Bokun:
                        {
                            var bokunAvailabilities = _TableStorageOperations.RetrieveData<BokunAvailabilities>(extraDetailsRequest.ReferenceId, token);
                            var isSkipPickupDropOff = false;

                            try
                            {
                                //Chargeable pickup skip
                                isSkipPickupDropOff = bokunAvailabilities.ActivityId == 5266;
                                if (isSkipPickupDropOff)
                                {
                                    break;
                                }
                            }
                            catch (Exception)
                            {

                            }
                            var priceCategoryIds = new List<int>();
                            var savedTravelInfo = SerializeDeSerializeHelper.DeSerialize<TravelInfo>(bokunAvailabilities.TravelInfo);
                            var charArr = new char[] { '[', ']' };
                            foreach (var item in bokunAvailabilities.PricingCategoryId.Trim(charArr).Split(','))
                            {
                                priceCategoryIds.Add(Convert.ToInt32(item));
                            }
                            var tempInt = 0;
                            int.TryParse(bokunAvailabilities?.RateId, out tempInt);
                            var bokunSelectedProduct = new BokunSelectedProduct
                            {
                                Id = Convert.ToInt32(bokunAvailabilities.SupplierOptionCode),
                                DateStart = savedTravelInfo.StartDate,
                                StartTimeId = bokunAvailabilities.StartTimeId,
                                PricingCategoryIds = priceCategoryIds,
                                RateId = tempInt
                            };

                            var detailsForBokun = _cartService.GetExtraDetailsForBokun(bokunSelectedProduct, token);
                            var responseModelsQuestions = GetResponseModelQuestions(detailsForBokun);
                            var extraDetails = _checkoutMapper.MapPaymentExtraDetailsResponse(baseAvailabilitiesEntity, responseModelsQuestions, detailsForBokun.PickupDetails, detailsForBokun.DropoffDetails);

                            if (extraDetails?.DropoffLocations?.Any() == true
                                || extraDetails?.PickupLocations?.Any() == true
                                || extraDetails?.Questions?.Any() == true
                            )
                            {
                                extraDetailsResponse.Add(extraDetails);
                            }
                            break;
                        }
                    case APIType.Graylineiceland:
                        {
                            var gliAvailabilities = _TableStorageOperations.RetrieveData<GraylineIcelandAvailabilities>(extraDetailsRequest.ReferenceId, token);
                            var pickupLocations = !string.IsNullOrWhiteSpace(gliAvailabilities.PickupLocations) ? SerializeDeSerializeHelper.DeSerialize<Dictionary<int, String>>(gliAvailabilities.PickupLocations) : null;
                            var extraDetails = _checkoutMapper.MapPaymentExtraDetailsResponse(baseAvailabilitiesEntity, null, pickupLocations);
                            if (extraDetails?.DropoffLocations?.Any() == true
                                || extraDetails?.PickupLocations?.Any() == true
                                || extraDetails?.Questions?.Any() == true
                            )
                            {
                                extraDetailsResponse.Add(extraDetails);
                            }
                            break;
                        }
                    case APIType.Ventrata:
                        {
                            var ventrataAvailabilities = _TableStorageOperations.RetrieveData<VentrataAvailabilities>(extraDetailsRequest.ReferenceId, token);
                            var pickupLocationsList = !string.IsNullOrWhiteSpace(ventrataAvailabilities.PickupPointsDetailsForVentrata) ? SerializeDeSerializeHelper.DeSerialize<List<PickupPointsDetailsForVentrata>>(ventrataAvailabilities.PickupPointsDetailsForVentrata) : null;
                            var pickUpLocations = new Dictionary<int, string>();

                            if (pickupLocationsList != null && pickupLocationsList.Count > 0)
                            {
                                foreach (var pickupPointDetails in pickupLocationsList)
                                {
                                    pickUpLocations.Add(pickupPointDetails.RandomIntegerId, pickupPointDetails.PickupPointAddress);
                                }
                            }
                            var responseModelsQuestions = new List<WebAPI.Models.ResponseModels.Question>();
                            //Custom Contract Questions call
                            var isAgeQuestions = false;
                            if (!string.IsNullOrEmpty(ventrataAvailabilities?.VentrataProductId))
                            {

                                var customQuestionsForVentrata = _cartService.GetCustomQuestionsForVentrata(
                                    ventrataAvailabilities?.VentrataSupplierId, ventrataAvailabilities?.VentrataBaseURL
                                    , ventrataAvailabilities?.VentrataProductId, token, ventrataAvailabilities?.SupplierOptionCode);
                                if (customQuestionsForVentrata != null && customQuestionsForVentrata.Count > 0)
                                {

                                    customQuestionsForVentrata?.RemoveAll(x=>!x.Description.ToLower().Contains("age"));

                                    responseModelsQuestions = GetResponseModelQuestionsVentrata(customQuestionsForVentrata);
                                    isAgeQuestions = true;
                                }
                            }
                            var extraDetails = _checkoutMapper.MapPaymentExtraDetailsResponse(baseAvailabilitiesEntity, responseModelsQuestions, pickUpLocations);
                            if (isAgeQuestions)
                            {
                                extraDetails.IsPaxLevel = true;
                            }

                            if (extraDetails?.DropoffLocations?.Any() == true
                                || extraDetails?.PickupLocations?.Any() == true
                                || extraDetails?.Questions?.Any() == true
                            )
                            {
                                extraDetailsResponse.Add(extraDetails);
                            }
                            break;
                        }
                    case APIType.Goldentours:
                        {
                            var goldenToursAvailabilities = _TableStorageOperations.RetrieveData<GoldenToursAvailabilities>(extraDetailsRequest.ReferenceId, token);

                            var transferOptions = !string.IsNullOrWhiteSpace(goldenToursAvailabilities.TransferOptions) ? SerializeDeSerializeHelper.DeSerialize<List<ContractQuestion>>(goldenToursAvailabilities.TransferOptions) : null;
                            var questions = MapQuestions(transferOptions);

                            var pickupPlaces = !string.IsNullOrWhiteSpace(goldenToursAvailabilities.PickupLocations) ? SerializeDeSerializeHelper.DeSerialize<Dictionary<int, String>>(goldenToursAvailabilities.PickupLocations) : null;

                            var extraDetail = _checkoutMapper.MapPaymentExtraDetailsResponse(baseAvailabilitiesEntity, questions, pickupPlaces);

                            if (extraDetail?.DropoffLocations?.Any() == true
                                || extraDetail?.PickupLocations?.Any() == true
                                || extraDetail?.Questions?.Any() == true
                            )
                            {
                                extraDetailsResponse.Add(extraDetail);
                            }
                            break;
                        }
                    case APIType.Rezdy:
                        {
                            var rezdyAvailabilities = _TableStorageOperations.RetrieveData<RezdyAvailabilities>(extraDetailsRequest.ReferenceId, token);
                            var productCode = !string.IsNullOrWhiteSpace(rezdyAvailabilities.SupplierOptionCode) ? rezdyAvailabilities.SupplierOptionCode : String.Empty;
                            //var passengerCount = extraDetailsRequest.TravelInfo.NoOfPassengers.Sum(x => x.Value);
                            var savedTravelInfo = SerializeDeSerializeHelper.DeSerialize<TravelInfo>(rezdyAvailabilities.TravelInfo);
                            var passengerCount = savedTravelInfo.NoOfPassengers.Sum(x => x.Value);
                            var extraDataForRezdy = _cartService.GetExtraDetailsForRezdy(productCode, passengerCount, token);
                            _TableStorageOperations.InsertRezdyPickUpLocations(extraDataForRezdy.RezdyPickUpLocations, extraDataForRezdy.PickUpId);
                            var responseModelsQuestions = GetResponseModelQuestionsRezdy(extraDataForRezdy.ExtraDetailsBookingQuestions);
                            extraDetailsResponse.Add(_checkoutMapper.MapPaymentExtraDetailsResponse(baseAvailabilitiesEntity, responseModelsQuestions, extraDataForRezdy.PickupDetails));
                            break;
                        }
                    case APIType.GlobalTix:
                        {
                            var globalTixAvailabilities = _TableStorageOperations.RetrieveData<GlobalTixAvailabilities>(extraDetailsRequest.ReferenceId, token);
                            if (!string.IsNullOrEmpty(globalTixAvailabilities?.TicketTypeIds))
                            {
                                var listStrElements = globalTixAvailabilities?.TicketTypeIds.Split(',').ToList();
                                var detailsForGlobalTix = _cartService.GetExtraDetailsForGlobalTix(listStrElements, token);
                                var contractQuestions = detailsForGlobalTix?.Item1;
                                var pickupPlaces = detailsForGlobalTix?.Item2;
                                if (contractQuestions != null && contractQuestions?.Count > 0)
                                {
                                    globalTixAvailabilities.ContractQuestions = SerializeDeSerializeHelper.Serialize(contractQuestions);
                                    _TableStorageOperations.UpdateGlobalTixData(globalTixAvailabilities);
                                    contractQuestions = contractQuestions?.Where(x => x.Answer == string.Empty)?.ToList();
                                }
                                var questions = _checkoutMapper.MapHotelBedQuestions(contractQuestions);
                                var extraDetails = _checkoutMapper.MapPaymentExtraDetailsResponse(baseAvailabilitiesEntity, questions, pickupPlaces);
                                if (extraDetails?.DropoffLocations?.Any() == true
                                    || extraDetails?.PickupLocations?.Any() == true
                                    || extraDetails?.Questions?.Any() == true
                                )
                                {
                                    extraDetailsResponse.Add(extraDetails);
                                }
                            }
                            break;
                        }
                    case APIType.TourCMS:
                        {
                            var tourCMSAvailabilities = _TableStorageOperations.RetrieveData<TourCMSAvailabilities>(extraDetailsRequest.ReferenceId, token);
                            var savedTravelInfo = SerializeDeSerializeHelper.DeSerialize<TravelInfo>(tourCMSAvailabilities.TravelInfo);

                            var pickupLocationsList = !string.IsNullOrWhiteSpace(tourCMSAvailabilities.PickupPointsDetails) ? SerializeDeSerializeHelper.DeSerialize<List<PickupPointsForTourCMS>>(tourCMSAvailabilities.PickupPointsDetails) : null;
                            var questionList = !string.IsNullOrWhiteSpace(tourCMSAvailabilities.ContractQuestions) ? SerializeDeSerializeHelper.DeSerialize<List<QuestionsForTourCMS>>(tourCMSAvailabilities.ContractQuestions) : null;

                            var tourCMSSelectedProduct = new TourCMSSelectedProduct
                            {
                                Id = Convert.ToInt32(tourCMSAvailabilities.SupplierOptionCode),
                                DateStart = savedTravelInfo.StartDate,
                                //StartTimeId = tourCMSAvailabilities.StartTimeId,
                                //RateId = tempInt
                            };

                            var detailsForTourCMS = _cartService.GetExtraDetailsForTourCMS(tourCMSSelectedProduct, pickupLocationsList, questionList, token);
                            var responseModelsQuestions = GetResponseModelQuestionsTourCMS(detailsForTourCMS, questionList);
                            responseModelsQuestions = responseModelsQuestions?.Where(x => !string.IsNullOrWhiteSpace(x.Label))?.ToList();
                            var extraDetails = _checkoutMapper.MapPaymentExtraDetailsResponse(baseAvailabilitiesEntity, responseModelsQuestions, detailsForTourCMS.PickupDetails);

                            if (extraDetails?.PickupLocations?.Any() == true
                                || extraDetails?.Questions?.Any() == true
                            )
                            {
                                extraDetailsResponse.Add(extraDetails);
                            }
                            break;
                        }

                    case APIType.PrioHub:
                        {
                            var prioHubAvailabilities = _TableStorageOperations.RetrieveData<PrioHubAvailabilities>(extraDetailsRequest.ReferenceId, token);
                            var pickupLocationsList = !string.IsNullOrWhiteSpace(prioHubAvailabilities.PickupPointsDetails) ? SerializeDeSerializeHelper.DeSerialize<List<PickUpPointForPrioHub>>(prioHubAvailabilities.PickupPointsDetails) : null;
                            var detailsForPrioHub = _cartService.GetExtraDetailsForPrioHub(pickupLocationsList, token);

                            var extraDetails = _checkoutMapper.MapPaymentExtraDetailsResponse(baseAvailabilitiesEntity, null, detailsForPrioHub.PickupDetails);
                            if (extraDetails?.PickupLocations?.Any() == true)
                            {
                                extraDetailsResponse.Add(extraDetails);
                            }
                            break;
                        }
                    case APIType.Fareharbor:
                        {
                            var fhbAvailabilities = _TableStorageOperations.RetrieveData<FareharborAvailabilities>(extraDetailsRequest.ReferenceId, token);
                            var detailsForFHB = _cartService.GetExtraDetailsForFareHarbor(fhbAvailabilities.AvailToken, fhbAvailabilities.SupplierName, fhbAvailabilities.UserKey, token);

                            var extraDetails = _checkoutMapper.MapPaymentExtraDetailsResponse(baseAvailabilitiesEntity, null, detailsForFHB);
                            if (extraDetails?.PickupLocations?.Any() == true)
                            {
                                extraDetailsResponse.Add(extraDetails);
                            }
                            break;
                        }

                    case APIType.Tiqets:
                        {
                            var tiqetAvailabilities = _TableStorageOperations.RetrieveData<TiqetsAvailabilities>(extraDetailsRequest.ReferenceId, token);
                            var extraOptions = !string.IsNullOrWhiteSpace(tiqetAvailabilities.RequiresVisitorsDetails) ? SerializeDeSerializeHelper.DeSerialize<List<string>>(tiqetAvailabilities.RequiresVisitorsDetails) : null;
                            extraOptions.RemoveAll(t => t != "passport_ids" && t != "nationality");
                            var questions = MapQuestionsTiqets(extraOptions);
                            var extraDetail = _checkoutMapper.MapPaymentExtraDetailsResponse(baseAvailabilitiesEntity, questions, null);
                            extraDetail.IsPaxLevel = true;
                            if (extraDetail?.Questions?.Any() == true)
                            {
                                extraDetailsResponse.Add(extraDetail);
                            }
                            break;
                        }
                    // ReSharper disable once RedundantEmptySwitchSection
                    default:
                        {
                            break;// throw new Exception("Unexpected Case");
                        }
                }
                return extraDetailsResponse;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CheckoutHelper",
                    MethodName = "GetPaymentExtraDetails",
                    Token = token,
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        #region Private Methods

        private List<Question> MapQuestions(List<ContractQuestion> transferOptions)
        {
            if (transferOptions == null) return null;

            var questions = new List<Question>();
            foreach (var transferOption in transferOptions)
            {
                if (!string.IsNullOrWhiteSpace(transferOption?.Name))
                {
                    var question = new Question
                    {
                        Id = GetIdForTransferOption(transferOption.Name),
                        Label = transferOption.Name,
                        Required = transferOption.IsRequired
                    };
                    questions.Add(question);
                }
            }
            return questions;
        }
        private List<Question> MapQuestionsTiqets(List<string> extraOptions)
        {
            if (extraOptions == null) return null;

            var questions = new List<Question>();
            foreach (var extraOption in extraOptions)
            {
                if (!string.IsNullOrWhiteSpace(extraOption))
                {
                    var question = new Question
                    {
                        Id = extraOption,
                        Label = extraOption,
                        Required = true,
                    };
                    questions.Add(question);
                }
            }
            return questions;
        }

        private string GetIdForTransferOption(string transferOptionName)
        {
            var transferOptionId = string.Empty;
            switch (transferOptionName.Trim())
            {
                case Constant.PostCode:
                    transferOptionId = Constant.PostCodeId;
                    break;

                case Constant.Destination:
                    transferOptionId = Constant.DestinationId;
                    break;

                case Constant.AirlineName:
                    transferOptionId = Constant.AirlineNameId;
                    break;

                case Constant.FlightNumber:
                    transferOptionId = Constant.FlightNumberId;
                    break;

                case Constant.GreetingName:
                    transferOptionId = Constant.GreetingNameId;
                    break;

                case Constant.HotelAddress:
                    transferOptionId = Constant.HotelAddressId;
                    break;

                case Constant.HotelName:
                    transferOptionId = Constant.HotelNameId;
                    break;

                case Constant.Mobile:
                    transferOptionId = Constant.MobileId;
                    break;

                case Constant.Origin:
                    transferOptionId = Constant.OriginId;
                    break;

                case Constant.TransferTime:
                    transferOptionId = Constant.TransferTimeId;
                    break;

                // ReSharper disable once RedundantEmptySwitchSection
                default:
                    break;
            }

            return transferOptionId;
        }

        /// <summary>
        /// Return converted questions for response model fro bokun
        /// </summary>
        /// <param name="detailsForBokun"></param>
        /// <returns></returns>
        public List<Question> GetResponseModelQuestions(ExtraDetailsForBokun detailsForBokun)
        {
            List<Question> responseModelQuestions = null;

            if (detailsForBokun?.Questions?.Count > 0)
            {
                responseModelQuestions = new List<Question>();
                detailsForBokun?.Questions?.ForEach(x =>
                {
                    if (!string.IsNullOrWhiteSpace(x.Label))
                    {
                        var responseModelQuestion = new Question
                        {
                            AnswerOptions = new List<Models.ResponseModels.AnswerOption>(),
                            DataType = x?.DataType,
                            DefaultValue = x?.DefaultValue,
                            Id = x?.QuestionId,
                            Label = x?.Label,
                            QuestionType = x?.QuestionType,
                            Required = x?.Required ?? false,
                            SelectFromOptions = x?.SelectFromOptions ?? false
                        };
                        x?.AnswerOptions?.ForEach(a =>
                        {
                            var answerOption = new Models.ResponseModels.AnswerOption
                            {
                                Label = a.Label,
                                Value = a.Value
                            };
                            if (!string.IsNullOrWhiteSpace(answerOption.Label))
                            {
                                responseModelQuestion.AnswerOptions.Add(answerOption);
                            }
                        });
                        responseModelQuestions.Add(responseModelQuestion);
                    }
                });
            }
            return responseModelQuestions;
        }

        /// <summary>
        /// Return converted questions for response model fro bokun
        /// </summary>
        /// <param name="detailsForTourCMS"></param>
        ///  <param name="questionsForTourCMSList"></param>
        /// <returns></returns>
        public List<Question> GetResponseModelQuestionsTourCMS(
            ExtraDetailsForTourCMS detailsForTourCMS,
            List<QuestionsForTourCMS> questionsForTourCMSList)
        {
            List<Question> responseModelQuestions = null;

            if (detailsForTourCMS?.Questions?.Count > 0)
            {
                responseModelQuestions = new List<Question>();
                detailsForTourCMS?.Questions?.ForEach(x =>
                {
                    if (!string.IsNullOrWhiteSpace(x.Code))
                    {
                        var responseModelQuestion = new Question
                        {
                            AnswerOptions = new List<Models.ResponseModels.AnswerOption>(),
                            //DataType = x?.DataType,
                            //DefaultValue = x?.DefaultValue,
                            Id = x?.Code,
                            Label = x?.Description,
                            //QuestionType = x?.QuestionType,
                            Required = x.IsRequired
                            //SelectFromOptions = x?.SelectFromOptions ?? false
                        };

                        x.AnswerOptions?.ForEach(a =>
                        {
                            var answerOption = new Models.ResponseModels.AnswerOption
                            {
                                Label = a.Label,
                                Value = a.Value
                            };
                            if (!string.IsNullOrWhiteSpace(answerOption.Label))
                            {
                                responseModelQuestion.AnswerOptions.Add(answerOption);
                            }
                        });
                        responseModelQuestions.Add(responseModelQuestion);
                    }
                });
            }
            return responseModelQuestions;
        }
        public List<WebAPI.Models.ResponseModels.Question> GetResponseModelQuestionsRezdy(List<Isango.Entities.Rezdy.BookingQuestions> rezdyQuetions)
        {
            List<WebAPI.Models.ResponseModels.Question> responseModelQuestions = null;

            if (rezdyQuetions?.Count > 0)
            {
                responseModelQuestions = new List<WebAPI.Models.ResponseModels.Question>();
                rezdyQuetions?.ForEach(x =>
                {
                    var responseModelQuestion = new WebAPI.Models.ResponseModels.Question
                    {
                        AnswerOptions = new List<Models.ResponseModels.AnswerOption>(),
                        Label = x?.Question,
                        Required = x.Required
                    };
                    x.AnswerOptions?.ForEach(a =>
                    {
                        var answerOption = new Models.ResponseModels.AnswerOption
                        {
                            Label = a.Label,
                            Value = a.Value
                        };
                        responseModelQuestion.AnswerOptions.Add(answerOption);
                    });
                    responseModelQuestions.Add(responseModelQuestion);
                });
            }
            return responseModelQuestions;
        }

        public List<WebAPI.Models.ResponseModels.Question> GetResponseModelQuestionsVentrata(List<VentrataExtraQuestion> ventrataQuestions)
        {
            List<WebAPI.Models.ResponseModels.Question> responseModelQuestions = null;

            if (ventrataQuestions?.Count > 0)
            {
                responseModelQuestions = new List<WebAPI.Models.ResponseModels.Question>();
                ventrataQuestions?.ForEach(x =>
                {
                    var responseModelQuestion = new WebAPI.Models.ResponseModels.Question
                    {
                        AnswerOptions = new List<Models.ResponseModels.AnswerOption>(),
                        Label = x?.Description,
                        Required = x.Required,
                        Id = x.Id
                    };
                    responseModelQuestions.Add(responseModelQuestion);
                });
            }
            return responseModelQuestions;
        }
        #endregion Private Methods
    }
}