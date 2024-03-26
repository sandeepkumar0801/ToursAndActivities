using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Bokun;
using Isango.Entities.ConsoleApplication.DataDumping;
using Isango.Entities.ConsoleApplication.ServiceAvailability;
using Isango.Entities.Enums;
using Isango.Entities.GoogleMaps;
using Isango.Service.Contract;
using Logger.Contract;
using ServiceAdapters.GlobalTix.GlobalTix.Commands.Contracts;
using ServiceAdapters.GlobalTix.GlobalTix.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Util;
using Activity = Isango.Entities.Activities.Activity;
using ExtraDetail = Isango.Entities.GoogleMaps.ExtraDetail;
using PassengerType = Isango.Entities.Enums.PassengerType;

namespace Isango.Service.ConsoleApplication
{
    public class GoogleMapsDataDumpingService : IGoogleMapsDataDumpingService
    {
        private readonly IStorageOperationService _storageOperationService;
        private readonly ICartService _cartService;
        private readonly ILogger _logger;
        private IActivityTicketTypeCommandHandler _activityTicketTypeCommandHandler;

        public GoogleMapsDataDumpingService(IStorageOperationService storageOperationService, ICartService cartService, ILogger logger, IActivityTicketTypeCommandHandler activityTicketTypeCommandHandler)
        {
            _storageOperationService = storageOperationService;
            _cartService = cartService;
            _logger = logger;
            _activityTicketTypeCommandHandler = activityTicketTypeCommandHandler;
        }

        public int DumpPriceAndAvailabilities(List<TempHBServiceDetail> serviceDetails, List<ProductOption> productOptions, APIType apiType)
        {
            serviceDetails = FilterServiceDetailsData(serviceDetails);
            if (serviceDetails?.Count <= 0) return 0;

            var storageServiceDetails = MapStorageServiceDetails(serviceDetails, productOptions, apiType);
            var watch = Stopwatch.StartNew();
            var partitionKey = DateTime.UtcNow.ToString("dd_MMM_yyyy");
            var oldServiceDetails = _storageOperationService.GetServiceDetails(partitionKey).Where(w => w.ApiType == apiType.ToString()).ToList();
            watch.Stop();
            _logger.WriteTimer("DumpPriceAndAvailabilities:GetServiceDetails", "DataDumping", apiType.ToString(), watch.Elapsed.ToString());

            watch.Reset();
            var updatedStorageServiceDetails = ChangeServiceDetailsStatus(oldServiceDetails, storageServiceDetails);
            watch.Stop();
            _logger.WriteTimer("DumpPriceAndAvailabilities:ChangeServiceDetailsStatus", "DataDumping", apiType.ToString(), watch.Elapsed.ToString());

            if (updatedStorageServiceDetails?.Count <= 0) return 0;
            watch.Reset();
            _storageOperationService.InsertServiceDetails(updatedStorageServiceDetails.Distinct().ToList());
            _logger.WriteTimer("DumpPriceAndAvailabilities:InsertServiceDetails", "DataDumping", apiType.ToString(), watch.Elapsed.ToString());
            return updatedStorageServiceDetails?.Count ?? 0;
        }

        public ExtraDetail DumpExtraDetail(List<Activity> activities, APIType apiType)
        {
            Entities.GoogleMaps.ExtraDetail extraDetail = null;
            switch (apiType)
            {
                case APIType.Bokun:
                    extraDetail = GetBokunExtraDetail(activities, "ExtraDetails_Bokun");
                    break;

                case APIType.Goldentours:
                    extraDetail = GetGoldenToursExtraDetail(activities, "ExtraDetails_GoldenTours");
                    break;

                case APIType.Graylineiceland:
                    extraDetail = GetGrayLineIceLandExtraDetail(activities, "ExtraDetails_GrayLineIceLand");
                    break;

                case APIType.Hotelbeds:
                    extraDetail = GetHotelBedsExtraDetail(activities, "ExtraDetails_HotelBeds");
                    break;

                case APIType.GlobalTix:
                    extraDetail = GetGlobalTixExtraDetail(activities, "ExtraDetails_GlobalTix");
                    break;
            }

            if (extraDetail != null)
                _storageOperationService.InsertExtraDetail(extraDetail);

            return extraDetail;
        }

        public void DumpCancellationPolicies(List<GoogleCancellationPolicy> cancellationPolicies)
        {
            // Insert the cancellation policy data in table storage
            _storageOperationService.InsertCancellationPolicy(cancellationPolicies);
        }

        #region Private Methods

        /// <summary>
        /// This method filter out the delta from the old and new service details with StatusType
        /// </summary>
        /// <param name="oldServiceDetails"></param>
        /// <param name="newServiceDetails"></param>
        /// <returns></returns>
        public List<StorageServiceDetail> ChangeServiceDetailsStatus(List<StorageServiceDetail> oldServiceDetails, List<StorageServiceDetail> newServiceDetails)
        {
            if (oldServiceDetails == null || oldServiceDetails.Count == 0) return newServiceDetails;
            var oldServiceDetailsIds = oldServiceDetails.Select(x => x.RowKey).ToList();
            var newServiceDetailsIds = newServiceDetails.Select(x => x.RowKey).ToList();
            var deltaNewServiceDetailsIds = newServiceDetailsIds.Except(oldServiceDetailsIds).ToList();

            var serviceDetails = new List<StorageServiceDetail>();
            deltaNewServiceDetailsIds.ForEach(deltaNewServiceDetailsId =>
            {
                var serviceDetail = newServiceDetails.FirstOrDefault(x => x.RowKey == deltaNewServiceDetailsId);
                if (serviceDetail == null) return;
                serviceDetail.DumpingStatus =
                    DumpingStatus.Inserted.ToString();
                if (serviceDetail != null)
                    serviceDetails.Add(serviceDetail);
            });
            var maxParallelThreadCount = MaxParallelThreadCountHelper.GetMaxParallelThreadCount("MaxParallelThreadCount");
            Parallel.ForEach(oldServiceDetails, new ParallelOptions { MaxDegreeOfParallelism = maxParallelThreadCount }, oldServiceDetail =>
            {
                var newServiceDetail = newServiceDetails.FirstOrDefault(x => x.RowKey == oldServiceDetail.RowKey);
                if (newServiceDetail == null)
                {
                    oldServiceDetail.DumpingStatus = DumpingStatus.Deleted.ToString();
                    if (oldServiceDetail != null)
                        serviceDetails.Add(oldServiceDetail);
                    return;
                }

                if (oldServiceDetail.AvailableOn == newServiceDetail.AvailableOn &&
                    oldServiceDetail.Capacity == newServiceDetail.Capacity &&
                    oldServiceDetail.Status == newServiceDetail.Status) return;

                newServiceDetail.DumpingStatus = DumpingStatus.Updated.ToString();
                if (newServiceDetail != null)
                    serviceDetails.Add(newServiceDetail);
            });

            return serviceDetails;
        }

        /// <summary>
        /// Filtering only 30 days data with Status = "AVAILABLE" and UnitType = PerPerson
        /// </summary>
        /// <param name="serviceDetails"></param>
        /// <returns></returns>
        private static List<TempHBServiceDetail> FilterServiceDetailsData(List<TempHBServiceDetail> serviceDetails)
        {
            var startDate = DateTime.UtcNow.Date;
            var endDate = startDate.AddDays(30);
            var filteredServiceDetails = serviceDetails.Where(e => e.AvailableOn >= startDate && e.AvailableOn <= endDate && e.Status == AvailabilityStatus.AVAILABLE.ToString() && e.UnitType == UnitType.PerPerson.ToString() && e.PassengerTypeId != (int)PassengerType.Undefined).ToList();
            return filteredServiceDetails;
        }

        private List<StorageServiceDetail> MapStorageServiceDetails(List<TempHBServiceDetail> serviceDetails, List<ProductOption> productOptions, APIType apiType)
        {
            var storageServiceDetails = new List<StorageServiceDetail>();
            foreach (var serviceDetail in serviceDetails)
            {
                var updatedSellPrice = UpdateSellPrice(productOptions, serviceDetail);

                var startTime = serviceDetail.StartTime;
                var storageServiceDetail = new StorageServiceDetail
                {
                    TicketTypeId = GenerateTicketTypeId(serviceDetail),
                    AvailableOn = new DateTime(serviceDetail.AvailableOn.Ticks, DateTimeKind.Utc),
                    Currency = serviceDetail.Currency,
                    MinPax = serviceDetail.MinAdult,
                    ActivityId = serviceDetail.ActivityId,
                    CommissionPercent = serviceDetail.CommissionPercent,
                    SellPrice = updatedSellPrice,

                    Status = serviceDetail.Status,
                    ServiceOptionId = serviceDetail.ServiceOptionID,
                    CreatedOn = DateTime.UtcNow,
                    StartTime = startTime.ToString(),
                    Variant = serviceDetail.Variant,
                    PassengerTypeId = serviceDetail.PassengerTypeId,
                    UnitType = serviceDetail.UnitType,
                    Capacity = serviceDetail.Capacity,
                    ApiType = apiType.ToString(),
                    //Setting default StatusType as 'Unchanged'
                    DumpingStatus = DumpingStatus.Unchanged.ToString()
                };

                var availableOnWithStartTime = storageServiceDetail.AvailableOn.Add(startTime);
                long unixStartSec = new DateTimeOffset(availableOnWithStartTime).ToUnixTimeSeconds();
                storageServiceDetail.UnixStartSec = unixStartSec;
                storageServiceDetail.RowKey = $"{storageServiceDetail.TicketTypeId}-{storageServiceDetail.UnixStartSec}";
                storageServiceDetails.Add(storageServiceDetail);
            }
            return storageServiceDetails;
        }

        private decimal UpdateSellPrice(List<ProductOption> productOptions, TempHBServiceDetail serviceDetail)
        {
            var updatedPrice = serviceDetail.SellPrice;
            try
            {
                var options = productOptions.Where(e =>
                    e.Id == serviceDetail.ServiceOptionID
                    && e.Variant == serviceDetail.Variant
                    && e.StartTime == serviceDetail.StartTime);
                foreach (var option in options)
                {
                    if (option == null) continue;

                    var perPersonPricingUnitsByDate = option.BasePrice.DatePriceAndAvailabilty
                        .FirstOrDefault(x => x.Key == serviceDetail.AvailableOn).Value?.PricingUnits
                        ?.Where(e => e.UnitType == UnitType.PerPerson).ToList();

                    var matchedPricingUnit = perPersonPricingUnitsByDate?.FirstOrDefault(e =>
                        ((PerPersonPricingUnit)e).PassengerType == (PassengerType)serviceDetail.PassengerTypeId);

                    if (matchedPricingUnit == null) continue;
                    updatedPrice = matchedPricingUnit.Price;
                }
            }
            catch (Exception ex)
            {
            }
            return updatedPrice;
        }

        /// <summary>
        /// Generate ticket type id based on unique combination
        /// </summary>
        /// <param name="serviceDetail"></param>
        /// <returns></returns>
        private string GenerateTicketTypeId(TempHBServiceDetail serviceDetail)
        {
            var variant = string.IsNullOrEmpty(serviceDetail.Variant) ? string.Empty : serviceDetail.Variant.Trim();
            var uniqueCombination = $"{serviceDetail.ActivityId}{serviceDetail.ServiceOptionID}{serviceDetail.PassengerTypeId}{variant}{serviceDetail.SellPrice}";
            var ticketTypeId = uniqueCombination.GetHashCode().ToString("X16");
            return ticketTypeId;
        }

        #endregion Private Methods

        #region Extra Details Calls

        public Entities.GoogleMaps.ExtraDetail GetBokunExtraDetail(List<Activity> activities, string token)
        {
            var paymentExtraDetails = new List<PaymentExtraDetail>();

            try
            {
                foreach (var activity in activities)
                {
                    if (activity.ProductOptions == null) continue;
                    foreach (var productOption in activity.ProductOptions)
                    {
                        var option = (ActivityOption)productOption;
                        var bokunSelectedProduct = new BokunSelectedProduct
                        {
                            Id = Convert.ToInt32(productOption.SupplierOptionCode),
                            DateStart = option.TravelInfo.StartDate,
                            StartTimeId = option.StartTimeId,
                            PricingCategoryIds = option.PricingCategoryId,
                            RateId = Convert.ToInt32(option.RateKey)
                        };
                        var paymentExtraDetail = GetBokunPaymentExtraDetail(activity.ID, bokunSelectedProduct, token, option.ServiceOptionId);
                        if (paymentExtraDetail == null) continue;
                        paymentExtraDetails.Add(paymentExtraDetail);
                    }
                }
            }
            catch (Exception ex)
            {
                //ignored
                _logger.Error("GetBokunExtraDetail | GoogleMapsDataDumpingServices", ex);
            }

            var extraDetail = new Entities.GoogleMaps.ExtraDetail
            {
                TokenId = token,
                PaymentExtraDetails = paymentExtraDetails
            };
            return extraDetail;
        }

        private PaymentExtraDetail GetBokunPaymentExtraDetail(int activityId, BokunSelectedProduct bokunSelectedProduct, string token, int ServiceOptionID = 0)
        {
            var bokunExtraDetails = _cartService.GetExtraDetailsForBokun(bokunSelectedProduct, token);
            if (bokunExtraDetails == null) return null;
            var questions = new List<Entities.GoogleMaps.Question>();
            if (bokunExtraDetails.Questions != null)
            {
                foreach (var bokunQuestion in bokunExtraDetails.Questions)
                {
                    var question = new Entities.GoogleMaps.Question
                    {
                        Id = bokunQuestion.QuestionId,
                        Required = bokunQuestion.Required,
                        Label = bokunQuestion.Label,
                        QuestionType = bokunQuestion.QuestionType,
                        DataType = bokunQuestion.DataType,
                        DefaultValue = bokunQuestion.DefaultValue,
                        SelectFromOptions = bokunQuestion.SelectFromOptions,
                        AnswerOptions = new List<Entities.GoogleMaps.AnswerOption>()
                    };
                    //Add answer options as well
                    if (bokunQuestion.AnswerOptions != null && bokunQuestion.AnswerOptions.Count > 0)
                    {
                        foreach (var ans in bokunQuestion.AnswerOptions)
                        {
                            var Ques_ans = new Entities.GoogleMaps.AnswerOption();
                            Ques_ans.Label = ans.Label;
                            Ques_ans.Value = ans.Value;

                            question.AnswerOptions.Add(Ques_ans);
                        }
                    }
                    questions.Add(question);
                }
            }
            var paymentExtraDetail = new PaymentExtraDetail
            {
                ActivityId = activityId,
                OptionId = ServiceOptionID,
                PickupLocations = bokunExtraDetails.PickupDetails,
                DropoffLocations = bokunExtraDetails.DropoffDetails,
                Questions = questions
            };
            return paymentExtraDetail;
        }

        public Entities.GoogleMaps.ExtraDetail GetGoldenToursExtraDetail(List<Activity> activities, string token)
        {
            var paymentExtraDetails = new List<PaymentExtraDetail>();
            foreach (var activity in activities)
            {
                if (activity.ProductOptions == null) continue;
                foreach (var productOption in activity.ProductOptions)
                {
                    var option = (ActivityOption)productOption;
                    var questions = new List<Entities.GoogleMaps.Question>();
                    if (option.ContractQuestions != null)
                    {
                        foreach (var contractQuestion in option.ContractQuestions)
                        {
                            var question = new Entities.GoogleMaps.Question
                            {
                                Id = contractQuestion.Code,
                                Required = contractQuestion.IsRequired,
                                Label = contractQuestion.Name
                            };
                            questions.Add(question);
                        }
                    }

                    var paymentExtraDetail = new PaymentExtraDetail
                    {
                        ActivityId = activity.ID,
                        OptionId = option.Id,
                        Variant = option.Variant,
                        StartTime = option.StartTime,
                        PickupLocations = option.PickupLocations,
                        Questions = questions
                    };
                    paymentExtraDetails.Add(paymentExtraDetail);
                }
            }
            var extraDetail = new Entities.GoogleMaps.ExtraDetail
            {
                TokenId = token,
                PaymentExtraDetails = paymentExtraDetails
            };
            return extraDetail;
        }

        public Entities.GoogleMaps.ExtraDetail GetGrayLineIceLandExtraDetail(List<Activity> activities, string token)
        {
            var paymentExtraDetails = new List<PaymentExtraDetail>();
            foreach (var activity in activities)
            {
                if (activity.ProductOptions == null) continue;
                foreach (var productOption in activity.ProductOptions)
                {
                    var option = (ActivityOption)productOption;
                    var paymentExtraDetail = new PaymentExtraDetail
                    {
                        ActivityId = activity.ID,
                        OptionId = option.Id,
                        PickupLocations = option.PickupLocations
                    };
                    paymentExtraDetails.Add(paymentExtraDetail);
                }
            }
            var extraDetail = new Entities.GoogleMaps.ExtraDetail
            {
                TokenId = token,
                PaymentExtraDetails = paymentExtraDetails
            };
            return extraDetail;
        }

        private Entities.GoogleMaps.ExtraDetail GetHotelBedsExtraDetail(List<Activity> activities, string token)
        {
            var paymentExtraDetails = new List<PaymentExtraDetail>();
            foreach (var activity in activities)
            {
                if (activity.ProductOptions == null) continue;
                foreach (var productOption in activity.ProductOptions)
                {
                    var option = (ActivityOption)productOption;
                    var questions = new List<Entities.GoogleMaps.Question>();
                    if (option.ContractQuestions != null)
                    {
                        foreach (var contractQuestion in option.ContractQuestions)
                        {
                            var question = new Entities.GoogleMaps.Question
                            {
                                Id = contractQuestion.Code,
                                Required = contractQuestion.IsRequired,
                                Label = contractQuestion.Name,
                                DefaultValue = contractQuestion.Description
                            };
                            questions.Add(question);
                        }
                    }

                    var paymentExtraDetail = new PaymentExtraDetail
                    {
                        ActivityId = activity.ID,
                        OptionId = option.ServiceOptionId,
                        Variant = option.Variant,
                        StartTime = option.StartTime,
                        PickupLocations = option.PickupLocations ?? null,
                        Questions = questions
                    };
                    paymentExtraDetails.Add(paymentExtraDetail);
                }
            }
            var extraDetail = new Entities.GoogleMaps.ExtraDetail
            {
                TokenId = token,
                PaymentExtraDetails = paymentExtraDetails
            };
            return extraDetail;
        }

        private Entities.GoogleMaps.ExtraDetail GetGlobalTixExtraDetail(List<Activity> activities, string token)
        {
            var paymentExtraDetails = new List<PaymentExtraDetail>();
            foreach (var activity in activities)
            {
                try
                {
                    foreach (ActivityOption option in activity.ProductOptions)
                    {
                        var questions = new List<Entities.GoogleMaps.Question>();
                        var pickupLocation = new Dictionary<int, string>();
                        try
                        {
                            foreach (var ProductID in option.ProductIDs)
                            {
                                ActivityTicketTypeInputContext inCtx = new ActivityTicketTypeInputContext
                                {
                                    MethodType = MethodType.TicketTypeDetail,
                                    TicketType = ProductID.ToString()
                                };

                                var ticketTypeRSRaw = _activityTicketTypeCommandHandler.Execute(inCtx, token);
                                if (ticketTypeRSRaw == null)
                                {
                                    continue;
                                }
                                var ticketTypeRS = SerializeDeSerializeHelper.DeSerialize<ServiceAdapters.GlobalTix.GlobalTix.Entities.RequestResponseModels.TicketTypeRS>(ticketTypeRSRaw.ToString());
                                if (ticketTypeRS == null || ticketTypeRS.IsSuccess == false)
                                {
                                    continue;
                                }

                                foreach (var ques in ticketTypeRS.Data.Questions)
                                {
                                    var question = new Entities.GoogleMaps.Question
                                    {
                                        Id = ques.Id.ToString(),
                                        Required = (ques.IsOptional == false),
                                        Label = ques.Type.Name.ToString(),
                                        DefaultValue = ques.QuestionText.ToString(),
                                        AnswerOptions = new List<Entities.GoogleMaps.AnswerOption>()
                                    };

                                    if (ques.Options != null && ques.Options.ToList().Count > 0)
                                    {
                                        foreach (var ans in ques.Options)
                                        {
                                            var QuesAns = new Entities.GoogleMaps.AnswerOption();
                                            QuesAns.Label = ans.ToString();
                                            QuesAns.Value = ans.ToString();

                                            question.SelectFromOptions = true;
                                            question.AnswerOptions.Add(QuesAns);
                                        }
                                    }

                                    try
                                    {
                                        if (ques.Type.Name.ToLower().Equals("option"))
                                        {
                                            pickupLocation.Add(ques.Id, string.Join("|", ques.Options));
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        //ignored - already added
                                    }

                                    questions.Add(question);
                                }
                            }
                            var paymentExtraDetail = new PaymentExtraDetail
                            {
                                ActivityId = activity.ID,
                                OptionId = option.ServiceOptionId,
                                Variant = option.Variant,
                                StartTime = option.StartTime,
                                PickupLocations = pickupLocation ?? null,
                                Questions = questions
                            };
                            paymentExtraDetails.Add(paymentExtraDetail);
                        }
                        catch (Exception ex)
                        {
                            _logger.Error("GetGlobalTixExtraDetail | GoogleMapsDataDumpingServices", ex);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error("GetGlobalTixExtraDetail | GoogleMapsDataDumpingServices", ex);
                }
            }
            var extraDetail = new Entities.GoogleMaps.ExtraDetail
            {
                TokenId = token,
                PaymentExtraDetails = paymentExtraDetails
            };
            return extraDetail;
        }

        #endregion Extra Details Calls
    }
}