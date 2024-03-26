using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Enums;
using Isango.Entities.Master;
using Isango.Entities.PricingRules;
using Isango.Entities.Region;
using Isango.Entities.Review;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Util;
using Constant = Isango.Persistence.Constants.Constants;

namespace Isango.Persistence.Data
{
    public class ActivityData
    {
        /// <summary>
        /// This method returns full text activity id mapping from reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public RegionActivityMapping GetFullTextSearchActivitiyIdMappingData(IDataReader reader)
        {
            var mapping = new RegionActivityMapping
            {
                IsHBService = DbPropertyHelper.BoolPropertyFromRow(reader, "IsHBAPIService"),
                ServiceId = DbPropertyHelper.Int32PropertyFromRow(reader, "serviceid"),
                RegionId = DbPropertyHelper.Int32PropertyFromRow(reader, "regionid")
            };
            return mapping;
        }

        /// <summary>
        /// This method returns region ids from attraction id from reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public string GetRegionIdsFromAttractionIdData(IDataReader reader)
        {
            var regionId = DbPropertyHelper.StringPropertyFromRow(reader, "regionid");
            return regionId;
        }

        /// <summary>
        /// This method returns region category mapping from reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public RegionCategoryMapping LoadRegionCategoryMappingData(IDataReader reader)
        {
            var mapping =
                new RegionCategoryMapping
                {
                    RegionId = DbPropertyHelper.Int32PropertyFromRow(reader, "regionid"),
                    CountryId = DbPropertyHelper.Int32PropertyFromRow(reader, "CountryID"),
                    Order = DbPropertyHelper.Int32PropertyFromRow(reader, "sequence"),
                    IsVisibleOnSearch = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "IsVisibleonSearch"),
                    IsTopCategory = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "IsTopAttraction"),
                    NumberOfProducts = DbPropertyHelper.Int32PropertyFromRow(reader, "ProductCount"),
                    CategoryName = DbPropertyHelper.StringPropertyFromRow(reader, "attractionname"),
                    CategoryType = DbPropertyHelper.StringPropertyFromRow(reader, "Type_"),
                    CategoryId = DbPropertyHelper.Int32PropertyFromRow(reader, "attractionid"),
                    Languagecode = DbPropertyHelper.StringPropertyFromRow(reader, "languagecode")
                };

            if (!string.IsNullOrWhiteSpace(DbPropertyHelper.StringPropertyFromRow(reader, "latitude")) && !string.IsNullOrWhiteSpace(DbPropertyHelper.StringPropertyFromRow(reader, "longitude")))
            {
                mapping.CoOrdinates = $"{DbPropertyHelper.StringPropertyFromRow(reader, "latitude")},{DbPropertyHelper.StringPropertyFromRow(reader, "longitude")}";
            }

            mapping.ImageId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "ImageID");
            mapping.ImageName = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "FileName_");
            mapping.ImageAltText = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "imgalttext");

            // we have to discuss about the image handling
            mapping.ReviewCount = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "ReviewCount");
            mapping.AverageReviewRating = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "avgreviewrating");

            return mapping;
        }

        /// <summary>
        /// This method returns service id from reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public int GetServiceIdData(IDataReader reader)
        {
            return DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "ServiceID");
        }

        /// <summary>
        /// This method returns license key from reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public string GetLicenseKeyData(IDataReader reader)
        {
            return DbPropertyHelper.StringPropertyFromRow(reader, "LicenseKey");
        }

        /// <summary>
        /// This method returns auto suggest data from reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public AutoSuggest GetAutoSuggestData(IDataReader reader)
        {
            var autoSuggest = new AutoSuggest
            {
                Label = DbPropertyHelper.StringPropertyFromRow(reader, Constant.CachedKeyword),
                Display = DbPropertyHelper.StringPropertyFromRow(reader, Constant.DisplayName),
                Category =
                    (DbPropertyHelper.StringPropertyFromRow(reader, Constant.Category) == Constant.D || DbPropertyHelper.StringPropertyFromRow(reader, Constant.Category) == Constant.C)
                        ? Constant.Destinations : (DbPropertyHelper.StringPropertyFromRow(reader, Constant.Category) == Constant.A) ? Constant.TopAttractions : Constant.ToursAndActivity,
                Url = DbPropertyHelper.StringPropertyFromRow(reader, Constant.SeoUrl),
                Type = DbPropertyHelper.StringPropertyFromRow(reader, Constant.Category),
                ParentId = DbPropertyHelper.StringPropertyFromRow(reader, Constant.CacheParentId),
                ReferenceId = DbPropertyHelper.StringDefaultPropertyFromRow(reader, Constant.RefId),
                IsTop = DbPropertyHelper.BoolPropertyFromRow(reader, Constant.IsTop),
                Languagecode = DbPropertyHelper.StringPropertyFromRow(reader, Constant.CacheLanguageCode)
            };

            return autoSuggest;
        }

        /// <summary>
        /// This method returns activity id from reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public int GetActivityIdData(IDataReader reader)
        {
            return DbPropertyHelper.Int32PropertyFromRow(reader, "newserviceid");
        }

        /// <summary>
        /// this method return activity from reader.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="isHbActivity"></param>
        /// <returns></returns>
        public Activity LoadActivityPropertiesData(IDataReader reader, bool? isHbActivity)
        {
            var activity = new Activity
            {
                Id = DbPropertyHelper.StringPropertyFromRow(reader, "ServiceID"),
                ID = DbPropertyHelper.Int32PropertyFromRow(reader, "ServiceID"),
                Name = DbPropertyHelper.StringPropertyFromRow(reader, "ServiceName"),
                ShortIntroduction = DbPropertyHelper.StringPropertyFromRow(reader, "ShortDescription"),
                MetaKeywords = DbPropertyHelper.StringPropertyFromRow(reader, "MetaKeywords"),
                MetaDescription = DbPropertyHelper.StringPropertyFromRow(reader, "MetaDescription"),
                Introduction = DbPropertyHelper.StringPropertyFromRow(reader, "Introduction"),
                DurationString = DbPropertyHelper.StringPropertyFromRow(reader, "Duration"),
                Schedule = DbPropertyHelper.StringPropertyFromRow(reader, "Schedule"),
                ScheduleOperates = DbPropertyHelper.StringPropertyFromRow(reader, "ScheduleOperates"),
                ScheduleUnavailableDates =
                    DbPropertyHelper.StringPropertyFromRow(reader, "ScheduleUnavailableDates"),
                ScheduleLocation = DbPropertyHelper.StringPropertyFromRow(reader, "ScheduleLocation"),
                HotelPickUpLocation = DbPropertyHelper.StringPropertyFromRow(reader, "ScheduleHotelPickup"),
                ScheduleReturnDetails = DbPropertyHelper.StringPropertyFromRow(reader, "ScheduleReturnDetails"),
                Inclusions = DbPropertyHelper.StringPropertyFromRow(reader, "Inclusions"),
                Exclusions = DbPropertyHelper.StringPropertyFromRow(reader, "Exclusions"),
                DoDont = DbPropertyHelper.StringPropertyFromRow(reader, "DoDont"),
                PleaseNote = DbPropertyHelper.StringPropertyFromRow(reader, "PleaseNote"),
                ChildPolicy = DbPropertyHelper.StringPropertyFromRow(reader, "ChildPolicy"),
                CancellationPolicy = FilterCancellationPolicy(DbPropertyHelper.StringPropertyFromRow(reader, "CancellationPolicy")),
                AlertNote = DbPropertyHelper.StringPropertyFromRow(reader, "AlertNote"),
                AdditionalInfo = DbPropertyHelper.StringPropertyFromRow(reader, "AdditionalInfo"),
                CoOrdinates = DbPropertyHelper.StringPropertyFromRow(reader, "CoOrdinates"),
                BookingWindow = DbPropertyHelper.Int32PropertyFromRow(reader, "BookingWindowDays"),
                AdditionalMarkUp = DbPropertyHelper.FloatPropertyFromRow(reader, "destinationmarkup"),
                Title = DbPropertyHelper.StringPropertyFromRow(reader, "PageTitle"),
                IsServiceLevelPickUp = DbPropertyHelper.BoolPropertyFromRow(reader, "IsPickUpServicelevel"),
                CurrencyIsoCode = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "Sell_CurrencyIsoCode"),
                LineOfBusinessId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "LINEOFBUSINESSID"),
                AttractionsCovered = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "ATTRACTIONSCOVERED"),
                ToDoOnArrival = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "TODOONARRIVAL"),
                WhyDoThis = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "WHYDOTHIS"),
                LiveOnDate = DbPropertyHelper.DateTimeNullablePropertyFromRow(reader, "LiveOnDate"),
                DurationDay = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "DurationDay"),
                DurationTime = TimeSpan.Parse(reader["DurationTime"].ToString()),
                DurationAdditionText = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "DurationAdditionText")
            };

            activity.OnSale = (activity.AdditionalMarkUp <= 0);

            var durationInDays = new List<double>();
            if (!string.IsNullOrEmpty(DbPropertyHelper.StringDefaultPropertyFromRow(reader, "DurationInDays")))
            {
                var durations = DbPropertyHelper.StringPropertyFromRow(reader, "DurationInDays").Split(',');
                foreach (var duration in durations)
                {
                    double.TryParse(duration, out var resultedDuration);
                    durationInDays.Add(Convert.ToDouble(resultedDuration));
                }
            }
            else
            {
                durationInDays.Add(0);
            }
            activity.Duration = durationInDays;

            var timeIn24Hour = new List<int>();

            if (!string.IsNullOrEmpty(DbPropertyHelper.StringDefaultPropertyFromRow(reader, "StartTime")))
            {
                var timeList = DbPropertyHelper.StringPropertyFromRow(reader, "StartTime").Split(',');
                foreach (var time in timeList)
                {
                    if (DateTime.TryParse(time, out var resultedTime))
                        timeIn24Hour.Add(resultedTime.Hour * 100 + resultedTime.Minute);
                    else
                        timeIn24Hour.Add(0);
                }
            }
            else
            {
                timeIn24Hour.Add(0);
            }
            activity.Time = timeIn24Hour;

            activity.Priority = DbPropertyHelper.Int32PropertyFromRow(reader, "Priority");

            activity.ShortName = DbPropertyHelper.StringPropertyFromRow(reader, "SERVICESHORTNAME");

            activity.ActualServiceUrl = DbPropertyHelper.StringPropertyFromRow(reader, "actual_url");

            activity.IsPaxDetailRequired = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "IsPaxDetailRequired");

            activity.IsReceipt = DbPropertyHelper.BoolPropertyFromRow(reader, "IsReceipt");
            activity.TotalReviews = DbPropertyHelper.Int32PropertyFromRow(reader, "customer_reviewcount");
            activity.OverAllRating = DbPropertyHelper.DoublePropertyFromRow(reader, "customer_reviewavgrating");

            if (isHbActivity.HasValue && isHbActivity == true)
            {
                activity.Code = DbPropertyHelper.StringPropertyFromRow(reader, "ServiceCode");
                activity.FactsheetId = DbPropertyHelper.Int32PropertyFromRow(reader, "FactsheetId");
            }

            var tempBoolean = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "ismarginpercent");

            if (tempBoolean)
            {
                if (activity.ProductOptions != null)
                {
                    //ToDo: Margin related changes
                    //activity.ProductOptions.FirstOrDefault().Margin = new Margin { IsPercentage = true };
                    //if (!string.IsNullOrEmpty(DbPropertyHelper.StringPropertyFromRow(reader, "marginamount")))
                    //{
                    //    activity.ProductOptions.FirstOrDefault().Margin.Value = DbPropertyHelper.DecimalPropertyFromRow(reader, "marginamount");
                    //}
                }

                #region Restaurant

                activity.FactsheetId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "Factsheetid");

                #endregion Restaurant
            }
            var apiType = APIType.Undefined;

            if (!string.IsNullOrEmpty(DbPropertyHelper.StringDefaultPropertyFromRow(reader, "APITypeID")))
            {
                apiType = (APIType)Enum.Parse(typeof(APIType), DbPropertyHelper.StringDefaultPropertyFromRow(reader, "APITypeID"));
            }

            activity.ApiType = apiType;
            activity.MeetingPointCoordinate = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "MeetingPointCoordinate");

            return activity;
        }

        /// <summary>
        /// this method return the list itineraries from activity.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public void LoadActivityItinerariesData(IDataReader reader, List<Activity> hbActivities)
        {
            var listActivityItineraries = new List<ActivityItinerary>();
            var listedProduct = new Activity();
            while (reader.Read())
            {
                var activityId = DbPropertyHelper.Int32PropertyFromRow(reader, "serviceid");
                listedProduct = hbActivities.Find(a => a.ID == activityId);

                var activityItinerary = new ActivityItinerary
                {
                    Title = DbPropertyHelper.StringPropertyFromRow(reader, "Title"),
                    Description = DbPropertyHelper.StringPropertyFromRow(reader, "Description"),
                    Order = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "ZOrder")
                };
                listActivityItineraries.Add(activityItinerary);
            }
            if (listActivityItineraries != null && listActivityItineraries.Count > 0)
            {
                listedProduct.Itineraries = listActivityItineraries;
            }
        }

        public List<OptionDetail> LoadPaxPrices(IDataReader reader, List<OptionDetail> optionDetails)
        {
            while (reader.Read())
            {
                try
                {
                    var paxPrice = new PaxPrice
                    {
                        ServiceOptionId = DbPropertyHelper.Int32PropertyFromRow(reader, "serviceoptionid"),
                        PassengerTypeId = DbPropertyHelper.Int32PropertyFromRow(reader, "passengertypeid"),
                        PriceId = DbPropertyHelper.Int32PropertyFromRow(reader, "priceid"),
                        CostPrice = DbPropertyHelper.DecimalDefaultNullablePropertyFromRow(reader, "Costprice"),
                        GateBasePrice = DbPropertyHelper.DecimalDefaultNullablePropertyFromRow(reader, "GateBasePrice"),
                        MinPaxCapacity = DbPropertyHelper.Int32PropertyFromRow(reader, "MinPaxCapacity"),
                        MaxPaxCapacity = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "MaxPaxCapacity"),
                        ShareablePax = DbPropertyHelper.BoolPropertyFromRow(reader, "shareablePax"),
                        TravelDate = DbPropertyHelper.DateTimePropertyFromRow(reader, "TDate"),
                        PassengerType = (PassengerType)DbPropertyHelper.Int32PropertyFromRow(reader, "passengertypeid")
                    };

                    optionDetails.Where(x => x.ServiceOptionId == paxPrice.ServiceOptionId).ToList().ForEach(x => x.PaxPrices.Add(paxPrice));
                }
                catch
                {
                    //#TODO Add logging here
                }
            }

            return optionDetails;
        }

        /// <summary>
        /// this method return list of activity reasons to book an activity.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public void LoadActivityReasonsToBookData(IDataReader reader, List<Activity> hbActivities)
        {
            var listActivityReasonToBook = new List<string>();
            var listedProduct = new Activity();
            while (reader.Read())
            {
                var activityId = DbPropertyHelper.Int32PropertyFromRow(reader, "serviceid");
                listedProduct = hbActivities.Find(a => a.ID == activityId);
                listActivityReasonToBook.Add(DbPropertyHelper.StringPropertyFromRow(reader, "TextDetails"));
            }
            if (listActivityReasonToBook != null && listActivityReasonToBook.Count > 0)
            {
                listedProduct.ReasonToBook = listActivityReasonToBook;
            }
        }

        /// <summary>
        /// this method return list of badges
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public void LoadBadgesData(IDataReader reader, List<Activity> hbActivities)
        {
            var badges = new List<Badge>();
            var listedProduct = new Activity();
            while (reader.Read())
            {
                var activityId = DbPropertyHelper.Int32PropertyFromRow(reader, "serviceid");
                listedProduct = hbActivities.Find(a => a.ID == activityId);
                var badge = new Badge
                {
                    Id = DbPropertyHelper.Int32PropertyFromRow(reader, "id"),
                    Name = DbPropertyHelper.StringPropertyFromRow(reader, "TagName")
                };
                badges.Add(badge);
            }
            if (badges != null && badges.Count > 0)
            {
                listedProduct.Badges = badges;
            }
        }

        /// <summary>
        /// This method returns region metadata from reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public RegionMetaData MapRegionMetaData(IDataReader reader)
        {
            var regionMetaData = new RegionMetaData
            {
                Name = DbPropertyHelper.StringPropertyFromRow(reader, "regionName"),
                Description = DbPropertyHelper.StringPropertyFromRow(reader, "regionAboutUs"),
                MetaDescription = DbPropertyHelper.StringPropertyFromRow(reader, "MetaDescription"),
                MetaKeywords = DbPropertyHelper.StringPropertyFromRow(reader, "MetaKeywords"),
                Title = DbPropertyHelper.StringPropertyFromRow(reader, "Title"),
                Tips = DbPropertyHelper.StringPropertyFromRow(reader, "regiontips"),
                GettingAround = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "gettingaround"),
                BestTime = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "visitbesttime"),
                MoneySavingTips = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "moneySavingTips"),
                DidYouKnow = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "didYouKnow"),
                StaySafe = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "staysafe"),
                Heading = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "Heading"),
                ImageName = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "imageName"),
                ImageId = (!string.IsNullOrEmpty(DbPropertyHelper.StringPropertyFromRow(reader, "imageID"))
                    ? DbPropertyHelper.Int32PropertyFromRow(reader, "imageID")
                    : 1),
                HeroImageId = (!string.IsNullOrEmpty(DbPropertyHelper.StringPropertyFromRow(reader, "heroImageID"))
                    ? DbPropertyHelper.Int32PropertyFromRow(reader, "heroImageID")
                    : 1)
            };

            if (reader.NextResult())
            {
                while (reader.Read())
                {
                    var userReview = new Review
                    {
                        Title = DbPropertyHelper.StringPropertyFromRow(reader, "title"),
                        Text = DbPropertyHelper.StringPropertyFromRow(reader, "reviewcomments"),
                        Rating = DbPropertyHelper.StringPropertyFromRow(reader, "overallrating"),
                        UserName = DbPropertyHelper.StringPropertyFromRow(reader, "firstname"),
                        Country = DbPropertyHelper.StringPropertyFromRow(reader, "country"),
                        ServiceName = DbPropertyHelper.StringPropertyFromRow(reader, "servicename"),
                        ServiceId = DbPropertyHelper.StringPropertyFromRow(reader, "serviceid")
                    };
                    if (regionMetaData.Reviews == null)
                        regionMetaData.Reviews = new List<Review>();
                    regionMetaData.Reviews.Add(userReview);
                }
            }

            if (reader.NextResult())
            {
                while (reader.Read())
                {
                    var region = new Region
                    {
                        Id = DbPropertyHelper.Int32PropertyFromRow(reader, "RegionID"),
                        Name = DbPropertyHelper.StringPropertyFromRow(reader, "RegionName"),
                        Url = DbPropertyHelper.StringPropertyFromRow(reader, "url")
                    };
                    if (regionMetaData.Regions == null)
                        regionMetaData.Regions = new List<Region>();
                    regionMetaData.Regions.Add(region);
                }
            }

            if (reader.NextResult())
            {
                regionMetaData.QuickLinks = new List<string>();
                while (reader.Read())
                {
                    regionMetaData.QuickLinks.Add(DbPropertyHelper.StringPropertyFromRow(reader, "anchortext"));
                }
            }

            if (reader.NextResult())
            {
                while (reader.Read())
                {
                    regionMetaData.IsCountry = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "iscountry");
                }
            }

            return regionMetaData;
        }

        /// <summary>
        /// This method returns live hb activities from reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public List<Activity> LoadLiveHbActivityData(IDataReader reader)
        {
            var hbActivities = new List<Activity>();

            while (reader.Read())
            {
                #region Service Details

                var type = ActivityType.Undefined;
                if (!string.IsNullOrEmpty(DbPropertyHelper.StringDefaultPropertyFromRow(reader, "ServiceTypeId")))
                {
                    type = (ActivityType)Enum.Parse(typeof(ActivityType), DbPropertyHelper.StringPropertyFromRow(reader, "ServiceTypeId"));
                }

                var activity = type.Equals(ActivityType.Theatre) ? new Show() : new Activity();
                activity.ID = DbPropertyHelper.Int32PropertyFromRow(reader, "ServiceID");
                activity.Id = DbPropertyHelper.StringPropertyFromRow(reader, "ServiceID");
                activity.Name = DbPropertyHelper.StringPropertyFromRow(reader, "ServiceName");
                activity.ActivityType = type;
                activity.ShortIntroduction = DbPropertyHelper.StringPropertyFromRow(reader, "ShortDescription");
                activity.MetaKeywords = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "MetaKeywords");
                activity.MetaDescription = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "MetaDescription");
                activity.Introduction = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "Introduction");
                activity.DurationString = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "Duration");
                activity.Schedule = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "Schedule");
                activity.ScheduleOperates = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "ScheduleOperates");
                activity.ScheduleUnavailableDates = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "ScheduleUnavailableDates");
                activity.ScheduleLocation = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "ScheduleLocation");
                activity.HotelPickUpLocation = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "ScheduleHotelPickup");
                activity.ScheduleReturnDetails = DbPropertyHelper.StringPropertyFromRow(reader, "ScheduleReturnDetails");
                activity.Inclusions = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "Inclusions");
                activity.Exclusions = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "Exclusions");
                activity.DoDont = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "DoDont");
                activity.PleaseNote = DbPropertyHelper.StringPropertyFromRow(reader, "PleaseNote");
                activity.ChildPolicy = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "ChildPolicy");

                activity.MinNoOfPax = DbPropertyHelper.Int32PropertyFromRow(reader, "MinCapacity");
                activity.MaxNoOfPax = DbPropertyHelper.Int32PropertyFromRow(reader, "MaxCapacity");

                var cancellationPolicies = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "CancellationPolicy");
                activity.CancellationPolicy = FilterCancellationPolicy(cancellationPolicies);

                activity.AlertNote = DbPropertyHelper.StringPropertyFromRow(reader, "AlertNote");
                activity.AdditionalInfo = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "AdditionalInfo");
                activity.CoOrdinates = DbPropertyHelper.StringPropertyFromRow(reader, "CoOrdinates");
                activity.BookingWindow = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "BookingWindowDays");
                activity.PriceTypeId = (PriceTypeId)Enum.Parse(typeof(PriceTypeId),
                    DbPropertyHelper.StringDefaultPropertyFromRow(reader, "PriceTypeID"));
                activity.AdditionalMarkUp = DbPropertyHelper.FloatPropertyFromRow(reader, "destinationmarkup");
                activity.OnSale = (activity.AdditionalMarkUp <= 0);
                activity.Title = DbPropertyHelper.StringPropertyFromRow(reader, "PageTitle");

                activity.IsServiceLevelPickUp = DbPropertyHelper.BoolPropertyFromRow(reader, "IsPickUpServicelevel");

                var durationInDaysList = new List<double>();
                var durationInDays = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "DurationInDays");
                if (!string.IsNullOrEmpty(durationInDays))
                {
                    var durationDays = durationInDays.Split(',');
                    foreach (var duration in durationDays)
                    {
                        durationInDaysList.Add(Convert.ToDouble(duration));
                    }
                }

                activity.Duration = durationInDaysList;

                var timeIn24Hour = new List<int>();
                var startTime = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "StartTime");
                if (!string.IsNullOrEmpty(startTime))
                {
                    var splitedTime = startTime.Split(',');
                    foreach (var time in splitedTime)
                    {
                        if (DateTime.TryParse(time, out var resultedTime))
                            timeIn24Hour.Add(resultedTime.Hour * 100 + resultedTime.Minute);
                        else
                            timeIn24Hour.Add(0);
                    }
                }
                activity.Time = timeIn24Hour;

                activity.Priority = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "Priority");

                activity.ShortName = DbPropertyHelper.StringPropertyFromRow(reader, "SERVICESHORTNAME");
                ((ActivityLite)activity).ActualServiceUrl = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "actual_url");
                var isPaxDetailRequired = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "IsPaxDetailRequired");
                activity.IsPaxDetailRequired = (!string.IsNullOrEmpty(isPaxDetailRequired) &&
                                                Convert.ToInt32(isPaxDetailRequired) > 0);

                activity.IsReceipt = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "IsReceipt");
                activity.TotalReviews = DbPropertyHelper.Int32PropertyFromRow(reader, "customer_reviewcount");
                activity.OverAllRating = DbPropertyHelper.DoublePropertyFromRow(reader, "customer_reviewavgrating");

                activity.Code = DbPropertyHelper.StringPropertyFromRowWithTrim(reader, "ServiceCode");
                activity.FactsheetId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "FactsheetId");
                //ToDo: Margin related changes
                //var isMarginPercent = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "ismarginpercent");
                //var isMarginAmount = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "marginamount");
                //activity.ProductOptions.FirstOrDefault().Margin = new Margin
                //{
                //    IsPercentage = isMarginPercent,
                //    Value = (decimal.TryParse(isMarginAmount, out var tempDecimal)) ? tempDecimal : 0
                //};

                var apiType = APIType.Undefined;
                if (!string.IsNullOrEmpty(DbPropertyHelper.StringDefaultPropertyFromRow(reader, "APITypeID")))
                {
                    apiType = (APIType)Enum.Parse(typeof(APIType), DbPropertyHelper.StringPropertyFromRow(reader, "APITypeID"));
                }
                activity.ApiType = apiType;
                activity.MeetingPointCoordinate = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "MeetingPointCoordinate");
                activity.IsLivePrice = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "IsLivePrice");
                activity.CurrencyIsoCode = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "Sell_CurrencyIsoCode");

                activity.LanguageCode = DbPropertyHelper.StringPropertyFromRow(reader, "languagecode");
                activity.CanonicalURL = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "CanonicalURL");
                activity.PickUpOption = (PickUpDropOffOptionType)Enum.Parse(typeof(PickUpDropOffOptionType), DbPropertyHelper.StringDefaultPropertyFromRow(reader, "PickupOptionID"));
                activity.ActualServiceUrl = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "actual_url");
                activity.ProductType = ProductType.Activity;
                activity.TotalReviews = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "customer_reviewcount");
                activity.WhereYouStay = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "WhereYouStay");
                activity.WhyYouLove = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "WhyYouLove");

                activity.IsNoIndex = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "IsNoIndex");
                activity.IsHighDefinationImages = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "IsHighDefinationImages");
                activity.IsFollow = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "IsFollow");
                activity.IsGoogleFeed = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "IsGoogleFeed");
                if (hbActivities?.Count(x => x.ID == activity.ID) == 0)
                    hbActivities.Add(activity);
                activity.Ratings = new List<Rating>();
                activity.RouteMaps = new List<RouteMap>();
                activity.LineOfBusinessId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "LINEOFBUSINESSID");
                activity.AttractionsCovered = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "ATTRACTIONSCOVERED");
                activity.ToDoOnArrival = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "TODOONARRIVAL");
                activity.WhyDoThis = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "WHYDOTHIS");
                activity.LiveOnDate = DbPropertyHelper.DateTimeNullablePropertyFromRow(reader, "LiveOnDate");
                activity.DurationDay = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "DurationDay");
                activity.DurationTime = (!String.IsNullOrEmpty(Convert.ToString(reader["DurationTime"]))) ? TimeSpan.Parse(Convert.ToString(reader["DurationTime"])) : TimeSpan.Zero;
                activity.WhereYouStay = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "WhereYouStay");
                activity.WhyYouLove = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "WhyYouLove");
                activity.DurationAdditionText = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "DurationAdditionText");
                activity.TourLaunchDate = DbPropertyHelper.DateTimeNullablePropertyFromRow(reader, "TourLaunchDate");
                activity.IsTimeBase = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "IsTimeBase");

                var cancellationSummary = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "Cancellation_Summary");
                activity.CancellationSummary = FilterCancellationPolicy(cancellationSummary);

                activity.DurationSummary = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "Duration_Summary");
                activity.StartTimeSummary = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "StartTime_Summary");
                activity.SupplierID = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "SupplierID");
                activity.isperpersonprice = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "isperpersonprice");
                activity.UnitText = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "UnitText");
                activity.IsPrivateTour = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "isprivatetour");
                activity.IsBundle = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "IsBundle");
                activity.SupplierName = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "SupplierName");
                activity.ISSHOWSUPPLIERVOUCHER = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "ISSHOWSUPPLIERVOUCHER");
                activity.AdyenStringentAccount = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "AdyenStringentAccount");
                activity.ServiceStatusID = DbPropertyHelper.Int32NullablePropertyFromRow(reader, "servicestatusid");
                activity.IsHideGatePrice = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "IsHideGatePrice");

                #endregion Service Details
            }

            #region Itineraries

            if (reader.NextResult())
            {
                LoadActivityItinerariesData(reader, hbActivities);
            }

            #endregion Itineraries

            #region Reasons To Book

            if (reader.NextResult())
            {
                LoadActivityReasonsToBookData(reader, hbActivities);
            }

            #endregion Reasons To Book

            #region Badges

            if (reader.NextResult())
            {
                LoadBadgesData(reader, hbActivities);
            }

            #endregion Badges

            #region Regions

            if (reader.NextResult())
            {
                LoadRegionData(reader, ref hbActivities);
            }

            #endregion Regions

            #region Categories

            if (reader.NextResult())
            {
                LoadCategoryData(reader, ref hbActivities);
            }

            #endregion Categories

            #region Images

            if (reader.NextResult())
            {
                LoadImageData(reader, ref hbActivities);
            }

            #endregion Images

            #region TripAdvisor

            reader.NextResult();

            #endregion TripAdvisor

            #region Top Reviews

            if (reader.NextResult())
            {
                LoadReviewData(reader, ref hbActivities);
            }

            #endregion Top Reviews

            #region Options

            if (reader.NextResult())
            {
                LoadOptionData(reader, ref hbActivities);
            }

            #endregion Options

            #region Issues

            if (reader.NextResult())
            {
                //NOT PROCESSED YET!
            }

            #endregion Issues

            #region YouTubeLink

            if (reader.NextResult())
            {
                while (reader.Read())
                {
                    var activityId = DbPropertyHelper.Int32PropertyFromRow(reader, "serviceid");
                    var listedProduct = hbActivities.Find(a => a.ID == activityId);
                    listedProduct.YouTubeLink = DbPropertyHelper.StringPropertyFromRow(reader, "videoURL");
                }
            }

            #endregion YouTubeLink

            #region Servicetype rating Ex. Attraction,Themepark

            if (reader.NextResult())
            {
                LoadServiceTypeData(reader, ref hbActivities);
            }

            #endregion Servicetype rating Ex. Attraction,Themepark

            #region Times of day

            if (reader.NextResult())
            {
                LoadTimesOfDay(reader, ref hbActivities);
            }

            #endregion Times of day

            #region Ratings

            if (reader.NextResult())
            {
                LoadRatings(reader, ref hbActivities);
            }

            #endregion Ratings

            //Download Links
            if (reader.NextResult())
            {
                LoadDownloadLinksData(reader, ref hbActivities);
            }
            //Activity Offers
            if (reader.NextResult())
            {
                LoadActivityOffersData(reader, ref hbActivities);
            }
            //API Contract Questions
            if (reader.NextResult())
            {
                LoadAPIContractQuestions(reader, ref hbActivities);
            }
            //API Contract Answers
            if (reader.NextResult())
            {
                LoadAPIContractAnswers(reader, ref hbActivities);
            }
            if (reader.NextResult())
            {
                LoadPickupLocationsData(reader, ref hbActivities);
            }
            if (reader.NextResult())
            {
                LoadRouteMapsData(reader, ref hbActivities);
            }
            return hbActivities;
        }

        /// <summary>
        /// Load availability for all the options.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="productOptions"></param>
        /// <returns></returns>
        public List<ProductOption> LoadAllOptionsAvailability(IDataReader reader, List<ProductOption> productOptions)
        {
            var updatedProductOptions = new List<ProductOption>();
            while (reader.Read())
            {
                var serviceOptionId = DbPropertyHelper.Int32PropertyFromRow(reader, "serviceoptionid");
                var availStatus = DbPropertyHelper.Int32PropertyFromRow(reader, "availablestate");
                var priceDate = DbPropertyHelper.DateTimePropertyFromRow(reader, "pricedate");
                var product =
                    productOptions.FirstOrDefault(e => e.Id == serviceOptionId);

                if (product != null)
                {
                    product.AvailabilityStatus = (AvailabilityStatus)availStatus;
                    product.IsCapacityCheckRequired = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "IsCapacityCheckRequired");
                    product.AllocationCapacity = DbPropertyHelper.Int32PropertyFromRow(reader, "Capacity");
                    var travelInfo = product.TravelInfo;
                    var startDate = travelInfo.StartDate;
                    var endDate = startDate.AddDays(travelInfo.NumberOfNights);

                    if (!updatedProductOptions.Any(x => x.Id == serviceOptionId)
                        && (priceDate >= startDate || priceDate <= endDate)
                        && ((AvailabilityStatus)availStatus != AvailabilityStatus.NOTAVAILABLE)
                     )
                    {
                        updatedProductOptions.Add(product);
                    }
                }
            }

            return updatedProductOptions;
        }

        /// <summary>
        /// Load availability for all the options.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="productOptions"></param>
        /// <returns></returns>
        public List<ProductOptionAvailabilty> LoadAllOptionsAvailabilities(IDataReader reader, List<ProductOption> productOptions)
        {
            var updatedProductOptions = new List<ProductOptionAvailabilty>();
            while (reader.Read())
            {
                var serviceOptionId = DbPropertyHelper.Int32PropertyFromRow(reader, "serviceoptionid");
                var availStatus = DbPropertyHelper.Int32PropertyFromRow(reader, "availablestate");
                var priceDate = DbPropertyHelper.DateTimePropertyFromRow(reader, "pricedate");
                var product =
                    productOptions.FirstOrDefault(e => e.Id == serviceOptionId);

                if (product != null)
                {
                    var productOptionAvailabilty = new ProductOptionAvailabilty
                    {
                        AvailableState = (AvailabilityStatus)availStatus,
                        Capacity = DbPropertyHelper.Int32PropertyFromRow(reader, "Capacity"),
                        IsCapacityCheckRequired = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "IsCapacityCheckRequired"),
                        PriceDate = priceDate,
                        PriceDateId = DbPropertyHelper.Int32PropertyFromRow(reader, "PriceDateId"),
                        ServiceId = DbPropertyHelper.Int32PropertyFromRow(reader, "serviceid"),
                        ServiceOptionId = serviceOptionId
                    };
                    product.AvailabilityStatus = (AvailabilityStatus)availStatus;
                    product.IsCapacityCheckRequired = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "IsCapacityCheckRequired");
                    product.AllocationCapacity = DbPropertyHelper.Int32PropertyFromRow(reader, "Capacity");
                    var travelInfo = product.TravelInfo;
                    var startDate = travelInfo.StartDate;
                    var endDate = startDate.AddDays(travelInfo.NumberOfNights);

                    if ((AvailabilityStatus)availStatus != AvailabilityStatus.NOTAVAILABLE)
                    {
                        updatedProductOptions.Add(productOptionAvailabilty);
                    }
                }
            }

            return updatedProductOptions;
        }

        public List<ProductOptionAvailabilty> LoadAllOptionsAvailabilitiesRedeam(IDataReader reader, List<ProductOption> productOptions)
        {
            var updatedProductOptions = new List<ProductOptionAvailabilty>();
            while (reader.Read())
            {
                var serviceOptionId = DbPropertyHelper.Int32PropertyFromRow(reader, "serviceoptionid");
                var availStatus = DbPropertyHelper.Int32PropertyFromRow(reader, "availablestate");
                var priceDate = DbPropertyHelper.DateTimePropertyFromRow(reader, "pricedate");

                if (productOptions.Any(x => x?.ServiceOptionId == serviceOptionId))
                {
                    var productOptionAvailabilty = new ProductOptionAvailabilty
                    {
                        AvailableState = (AvailabilityStatus)availStatus,
                        Capacity = DbPropertyHelper.Int32PropertyFromRow(reader, "Capacity"),
                        IsCapacityCheckRequired = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "IsCapacityCheckRequired"),
                        PriceDate = priceDate,
                        PriceDateId = DbPropertyHelper.Int32PropertyFromRow(reader, "PriceDateId"),
                        ServiceId = DbPropertyHelper.Int32PropertyFromRow(reader, "serviceid"),
                        ServiceOptionId = serviceOptionId
                    };
                    updatedProductOptions.Add(productOptionAvailabilty);
                }
            }

            return updatedProductOptions;
        }

        /// <summary>
        /// Load prices for all the options.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="productOptions"></param>
        /// <param name="conversionFactor"></param>
        /// <returns></returns>
        public List<ProductOption> LoadPrice(IDataReader reader, List<ProductOption> productOptions)
        {
            var optionId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "OptionID");
            var totalPrice = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "TotalPrice");
            var baseTotalPrice = DbPropertyHelper.DecimalDefaultNullablePropertyFromRow(reader, "Base_TotalPrice");
            var baseChildPrice = DbPropertyHelper.DecimalDefaultNullablePropertyFromRow(reader, "Base_ChildPrice");
            var baseCurrencyIsoCode = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "Base_CurrencyIsoCode");
            var availabilityStatus = (AvailabilityStatus)Enum.Parse(typeof(AvailabilityStatus), reader["StateName"].ToString(), true);
            var currentDate = DbPropertyHelper.DateTimePropertyFromRow(reader, "CDate");
            foreach (var productOption in productOptions)
            {
                if (productOption.Id == optionId)
                {
                    if (baseTotalPrice != 0 && baseCurrencyIsoCode != string.Empty)
                    {
                        productOption.BasePrice = CreatePriceandAvailability(baseTotalPrice, totalPrice, baseChildPrice, baseCurrencyIsoCode, baseCurrencyIsoCode, availabilityStatus, productOption, currentDate, optionId);
                    }
                    productOption.AvailabilityStatus = availabilityStatus;
                }
            }
            return productOptions;
        }

        public List<OptionDetail> MapOptionDetail(IDataReader reader)
        {
            var optionDetails = new List<OptionDetail>();
            while (reader.Read())
            {
                var optionDetail = new OptionDetail
                {
                    ServiceOptionId = DbPropertyHelper.Int32PropertyFromRow(reader, "serviceoptionid"),
                    CurrencyIsoCode = DbPropertyHelper.StringPropertyFromRow(reader, "currencyisocode"),
                    PriceType = DbPropertyHelper.Int32PropertyFromRow(reader, "PriceType"),
                    UnitType = DbPropertyHelper.Int32PropertyFromRow(reader, "UnitType"),
                    MaxCapacity = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "MaxCapacity"),
                    MaxQuantity = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "MaxQuantity"),
                    PaxPrices = new List<PaxPrice>()
                };
                optionDetails.Add(optionDetail);
            }
            if (reader.NextResult())
            {
                optionDetails = LoadPaxPrices(reader, optionDetails);
            }
            return optionDetails;
        }

        public List<WidgetMappedData> MapWidgetData(IDataReader reader)
        {
            var widgetDetails = new List<WidgetMappedData>();
            while (reader.Read())
            {
                var widgetData = new WidgetMappedData
                {
                    CSRegionName = DbPropertyHelper.StringPropertyFromRow(reader, "CSRegionName"),
                    languageid = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "languageid"),
                    Isangoregionid = DbPropertyHelper.Int32PropertyFromRow(reader, "Isangoregionid"),
                    languagecode = DbPropertyHelper.StringPropertyFromRow(reader, "languagecode"),
                    SEFURL = DbPropertyHelper.StringPropertyFromRow(reader, "SEFURL")
                };
                widgetDetails.Add(widgetData);
            }
            return widgetDetails;
        }

        /// <summary>
        /// Map Activity From DataSet
        /// </summary>
        /// <param name="dataSet"></param>
        /// <returns></returns>
        public List<Activity> MapActivityFromDataSet(DataSet dataSet)
        {
            var activities = new List<Activity>();
            // Get Activity
            LoadActivityFromDataSet(dataSet.Tables, ref activities);

            if (activities?.Count > 0)
            {
                //Get Itineraries
                LoadActivityItinerariesFromDataSet(dataSet.Tables, ref activities);

                //Get ReasonToBook
                LoadReasonToBookFromDataSet(dataSet.Tables, ref activities);

                //Get Badges
                LoadBadgesFromDataSet(dataSet.Tables, ref activities);

                //Get Regions
                LoadRegionsFromDataSet(dataSet.Tables, ref activities);

                //Get CategoryIDs
                LoadCategoryIDsFromDataSet(dataSet.Tables, ref activities);

                //Get Images
                LoadImagesFromDataSet(dataSet.Tables, ref activities);

                //Get Reviews
                LoadReviewsFromDataSet(dataSet.Tables, ref activities);

                //Get YouTubeLink
                LoadYouTubeLinkFromDataSet(dataSet.Tables, ref activities);

                //Get CategoryTypes
                LoadCategoryTypesFromDataSet(dataSet.Tables, ref activities);

                //Get Options
                LoadOptionsFromDataSet(dataSet.Tables, ref activities);

                //Get Times  of day
                LoadTimesOfDayFromDataSet(dataSet.Tables, ref activities);

                //Get Ratings
                LoadRatingsFromDataSet(dataSet.Tables, ref activities);

                //Get Close Out
                LoadCloseOutFromDataSet(dataSet.Tables, ref activities);

                //Get Download Links
                LoadDownloadLinksFromDataSet(dataSet.Tables, ref activities);

                //Get Activity Offers
                LoadActivityOffersFromDataSet(dataSet.Tables, ref activities);

                //Get Pickup Locations
                LoadPickupLocationsFromDataSet(dataSet.Tables, ref activities);

                //Get RouteMaps
                LoadRouteMapsFromDataSet(dataSet.Tables, ref activities);
            }
            return activities;
        }

        public AttractionActivityMapping GetCategoryServiceMapping(IDataReader reader)
        {
            var mapping = new AttractionActivityMapping
            {
                AttractionId = DbPropertyHelper.StringPropertyFromRow(reader, "attractionid"),
                ActivityId = DbPropertyHelper.StringPropertyFromRow(reader, "serviceids")
            };
            return mapping;
        }

        public CalendarAvailability GetCalendarMapping(IDataReader reader)
        {
            var calendar = new CalendarAvailability
            {
                Id = DbPropertyHelper.StringPropertyFromRow(reader, "uniqueStrID"),
                ActivityId = DbPropertyHelper.Int32PropertyFromRow(reader, "serviceID"),
                StartDate = DbPropertyHelper.DateTimePropertyFromRow(reader, "startDate"),
                EndDate = DbPropertyHelper.DateTimePropertyFromRow(reader, "endDate"),
                RegionId = DbPropertyHelper.Int32PropertyFromRow(reader, "regionID"),
                Currency = DbPropertyHelper.StringPropertyFromRow(reader, "currencyisocode"),
                AffiliateId = DbPropertyHelper.StringPropertyFromRow(reader, "affiliateid"),
                B2CBasePrice = DbPropertyHelper.DecimalPropertyFromRow(reader, "B2C_BaseSell"),
                B2BBasePrice = DbPropertyHelper.DecimalPropertyFromRow(reader, "B2B_BaseSell"),
                CostPrice = DbPropertyHelper.DecimalPropertyFromRow(reader, "CostPrice"),
                _ts = DateTimeOffset.Now.ToUnixTimeSeconds()
            };
            return calendar;
        }

        public Dictionary<string, int> LoadMaxPax(IDataReader reader)
        {
            var listMaxPaxDetails = new Dictionary<string, int>();
            while (reader.Read())
            {
                listMaxPaxDetails.Add("maxadults", DbPropertyHelper.Int32PropertyFromRow(reader, "maxadults"));
                listMaxPaxDetails.Add("maxpax", DbPropertyHelper.Int32PropertyFromRow(reader, "maxpax"));
            }
            return listMaxPaxDetails;
        }

        public List<ActivityChangeTracker> GetModifiedServices(IDataReader reader)
        {
            var activityChangesTracker = new List<ActivityChangeTracker>();
            while (reader.Read())
            {
                var serviceId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "ServiceID");
                bool isProcessed = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "IsModified");
                var modifiedOn = DbPropertyHelper.DateTimePropertyFromRow(reader, "ModifiedOn");
                var isHBProduct = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "IsHBProduct");
                var operationType = (OperationType)Enum.Parse(typeof(OperationType), DbPropertyHelper.StringDefaultPropertyFromRow(reader, "TypeOfOperation"), true);

                if (serviceId != 0)
                {
                    var activityChangeTracker = new ActivityChangeTracker
                    {
                        ActivityId = serviceId,
                        IsProcessed = isProcessed,
                        OperationType = operationType,
                        ProcessedDate = modifiedOn,
                        IsHbProduct = isHBProduct
                    };

                    activityChangesTracker.Add(activityChangeTracker);
                }
            }
            return activityChangesTracker;
        }

        /// <summary>
        /// Load the Product Sale Rules from the reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public ProductSaleRule GetProductSaleRule(IDataReader reader)
        {
            var rules = new ProductSaleRule
            {
                ProductSaleRulesByActivity = MapProductSaleRuleByActivity(reader)
            };

            if (reader.NextResult())
            {
                rules.ProductSaleRulesByOption = MapProductSaleRuleByOption(reader);
            }

            if (reader.NextResult())
            {
                rules.ProductSaleRulesByAffiliate = MapProductSaleRuleByAffiliate(reader);
            }

            if (reader.NextResult())
            {
                rules.ProductSaleRulesByCountry = MapProductSaleRuleByCountry(reader);
            }

            return rules;
        }

        /// <summary>
        /// Load the B2B Sale Rules from the reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public List<B2BSaleRule> GetB2BSaleRules(IDataReader reader)
        {
            var b2BSaleRules = new List<B2BSaleRule>();
            while (reader.Read())
            {
                var b2BSaleRule = new B2BSaleRule()
                {
                    AffiliateId = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "AffiliateID"),
                    SaleDescription = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "SALEDESC"),
                    B2BSaleOfferPercent = DbPropertyHelper.DecimalDefaultNullablePropertyFromRow(reader, "B2BSALEOFFERPERCENT"),
                    MinMarginCapPercent = DbPropertyHelper.DecimalDefaultNullablePropertyFromRow(reader, "MINMARGINCAPPERCENT"),
                    BookingFromDate = DbPropertyHelper.DateTimePropertyFromRow(reader, "BOOKINGFROMDATE"),
                    BookingToDate = DbPropertyHelper.DateTimePropertyFromRow(reader, "BOOKINGTODATE"),
                };
                b2BSaleRules.Add(b2BSaleRule);
            }
            return b2BSaleRules;
        }

        /// <summary>
        /// Load the B2B Net Rate Rules from the reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public List<B2BNetRateRule> GetB2BNetRateRules(IDataReader reader)
        {
            var b2BSaleRules = new List<B2BNetRateRule>();
            while (reader.Read())
            {
                var b2BSaleRule = new B2BNetRateRule()
                {
                    AffiliateId = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "AffiliateID"),
                    NetRatePercent = DbPropertyHelper.DecimalDefaultNullablePropertyFromRow(reader, "NetRatePercent"),
                    NetPriceType = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "NetPriceType"),
                    BookingFromDate = DbPropertyHelper.DateTimePropertyFromRow(reader, "BookingFromDate"),
                    BookingToDate = DbPropertyHelper.DateTimePropertyFromRow(reader, "BookingToDate"),
                };
                b2BSaleRules.Add(b2BSaleRule);
            }
            return b2BSaleRules;
        }

        /// <summary>
        /// Load the Supplier Sale Rules from the reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public SupplierSaleRule GetSupplierSaleRule(IDataReader reader)
        {
            var rules = new SupplierSaleRule
            {
                SupplierSaleRulesByActivity = MapSupplierSaleRuleByActivity(reader)
            };

            if (reader.NextResult())
            {
                rules.SupplierSaleRulesByOption = MapSupplierSaleRuleByOption(reader);
            }

            return rules;
        }

        /// <summary>
        /// Load passenger information from reader
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public List<Entities.Booking.PassengerInfo> GetPassengerinfo(IDataReader reader)
        {
            var passengerInfoList = new List<Entities.Booking.PassengerInfo>();
            while (reader.Read())
            {
                var serviceId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "ServiceID");
                bool independablePax = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "Independablepax");
                var fromAge = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "FromAge");
                var toAge = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "ToAge");
                var maxSize = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "MaxSize");
                var minSize = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "MinSize");
                var passengerTypeId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "PassengerTypeID");
                var paxDesc = DbPropertyHelper.StringPropertyFromRow(reader, "PaxName");
                var label = DbPropertyHelper.StringPropertyFromRow(reader, "Label");
                var measurementDesc = DbPropertyHelper.StringPropertyFromRow(reader, "MeasurementDesc");
                //Added for BackGround compatibility:ageGroupId, remove when JF1 is completely remove from everywhere.
                var ageGroupId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "AgeGroupID");
                var passengerInfo = new Entities.Booking.PassengerInfo
                {
                    FromAge = fromAge,
                    ToAge = toAge,
                    IndependablePax = independablePax,
                    MaxSize = maxSize,
                    MinSize = minSize,
                    PassengerTypeId = passengerTypeId,
                    PaxDesc = paxDesc,
                    ActivityId = serviceId,
                    Label = label,
                    MeasurementDesc = measurementDesc,
                    AgeGroupId = ageGroupId  //Added for BackGround compatibility:ageGroupId, remove when JF1 is completely remove from everywhere.
                };
                passengerInfoList.Add(passengerInfo);
            }

            return passengerInfoList;
        }

        /// <summary>
        /// Load Service Feeds Data
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public Entities.GoogleMaps.ServiceAvailabilityFeed GetServiceAvailabilityData(IDataReader reader)
        {
            return new Entities.GoogleMaps.ServiceAvailabilityFeed()
            {
                ActivityId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "ActivityId"),
                AvailableOn = DbPropertyHelper.DateTimePropertyFromRow(reader, "AvailableOn"),
                CommissionPercent = DbPropertyHelper.DecimalDefaultNullablePropertyFromRow(reader, "CommissionPercent"),
                Currency = DbPropertyHelper.StringPropertyFromRow(reader, "Currency"),
                Factsheetid = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "Factsheetid"),
                FromAge = DbPropertyHelper.StringPropertyFromRow(reader, "FromAge"),
                MinAdult = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "MinAdult"),
                Modality = DbPropertyHelper.StringPropertyFromRow(reader, "Modality"),
                PassengerTypeId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "PassengerTypeId"),
                Price = DbPropertyHelper.DecimalDefaultNullablePropertyFromRow(reader, "Price"),
                ProductClass = DbPropertyHelper.StringPropertyFromRow(reader, "ProductClass"),
                ProductCode = DbPropertyHelper.StringPropertyFromRow(reader, "ProductCode"),
                SellPrice = DbPropertyHelper.DecimalDefaultNullablePropertyFromRow(reader, "SellPrice"),
                Serviceoptionid = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "Serviceoptionid"),
                StartTime = DbPropertyHelper.TimeSpanPropertyFromRow(reader, "StartTime"),
                Status = DbPropertyHelper.StringPropertyFromRow(reader, "Status"),
                TicketOfficePrice = DbPropertyHelper.DecimalDefaultNullablePropertyFromRow(reader, "TicketOfficePrice"),
                ToAge = DbPropertyHelper.StringPropertyFromRow(reader, "ToAge"),
                UnitType = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "UnitType"),
                Variant = DbPropertyHelper.StringPropertyFromRow(reader, "Variant")
            };
        }

        #region Private Methods

        #region Private methods of LoadLiveHbActivityData

        /// <summary>
        /// Load service type data
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="hbActivities"></param>
        private void LoadServiceTypeData(IDataReader reader, ref List<Activity> hbActivities)
        {
            while (reader.Read())
            {
                var activityId = DbPropertyHelper.Int32PropertyFromRow(reader, "serviceid");
                var listedProduct = hbActivities.Find(a => a.ID == activityId);
                var categoryType = (ActivityCategoryType)Enum.Parse(typeof(ActivityCategoryType), DbPropertyHelper.StringPropertyFromRow(reader, "ServiceTyperatingId"));
                if (listedProduct.CategoryTypes == null)
                {
                    listedProduct.CategoryTypes = new List<ActivityCategoryType>() { { categoryType } };
                }
                else
                    listedProduct.CategoryTypes.Add(categoryType);
            }
        }

        /// <summary>
        /// Load review data
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="hbActivities"></param>
        private void LoadDownloadLinksData(IDataReader reader, ref List<Activity> hbActivities)
        {
            while (reader.Read())
            {
                var activityId = DbPropertyHelper.Int32PropertyFromRow(reader, "serviceid");
                var listedProduct = hbActivities.Find(a => a.ID == activityId);

                var downloadlink = new DownloadLinks
                {
                    ServiceId = DbPropertyHelper.Int32PropertyFromRow(reader, "serviceId"),
                    DownloadLink = DbPropertyHelper.StringPropertyFromRow(reader, "Downloadlink"),
                    DownloadText = DbPropertyHelper.StringPropertyFromRow(reader, "Downloadtext"),
                    StartDate = DbPropertyHelper.DateTimeNullablePropertyFromRow(reader, "Startdate"),
                    EndDate = DbPropertyHelper.DateTimeNullablePropertyFromRow(reader, "Enddate"),
                    DownloadId = DbPropertyHelper.Int32PropertyFromRow(reader, "Linkid")
                };
                if (listedProduct.DownloadLinks == null)
                {
                    listedProduct.DownloadLinks = new List<DownloadLinks>() { downloadlink };
                }
                else
                    listedProduct.DownloadLinks.Add(downloadlink);
            }
        }

        private void LoadPickupLocationsData(IDataReader reader, ref List<Activity> hbActivities)
        {
            while (reader.Read())
            {
                try
                {
                    var serviceId = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "Serviceid");
                    var activityPickups = new ActivityPickupLocations
                    {
                        ServiceID = DbPropertyHelper.Int32PropertyFromRow(reader, "Serviceid"),
                        Serviceoptionid = DbPropertyHelper.Int32NullablePropertyFromRow(reader, "Serviceoptionid"),
                        Pickuplocation = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "Pickuplocation"),
                        Languagecode = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "Languagecode"),
                        ID = DbPropertyHelper.Int32NullablePropertyFromRow(reader, "ID")
                    };

                    var IsServiceLevel = activityPickups.Serviceoptionid == null || activityPickups.Serviceoptionid == 0;

                    if (IsServiceLevel)
                    {
                        if (hbActivities?.FirstOrDefault(x => x.Id == serviceId)?.PickupLocations == null)
                        {
                            if (hbActivities.Any(x => x.Id == serviceId))
                                hbActivities.First(x => x.Id == serviceId).PickupLocations = new List<ActivityPickupLocations> { activityPickups };
                        }
                        else
                            hbActivities.FirstOrDefault(x => x.Id == serviceId)?.PickupLocations.Add(activityPickups);
                    }
                    else
                    {
                        if (hbActivities?.FirstOrDefault(x => x.Id == serviceId)?.ProductOptions?.FirstOrDefault(x => x.ServiceOptionId == activityPickups.Serviceoptionid)?.OptionPickupLocations == null)
                        {
                            if (hbActivities.FirstOrDefault(x => x.Id == serviceId)?.ProductOptions?.Any(x => x.ServiceOptionId == activityPickups.Serviceoptionid) ?? false)
                                hbActivities.First(x => x.Id == serviceId).ProductOptions.FirstOrDefault(x => x.ServiceOptionId == activityPickups.Serviceoptionid).OptionPickupLocations = new List<ActivityPickupLocations> { activityPickups };
                        }
                        else
                            hbActivities.First(x => x.Id == serviceId).ProductOptions.FirstOrDefault(x => x.ServiceOptionId == activityPickups.Serviceoptionid).OptionPickupLocations.Add(activityPickups);
                    }

                }
                catch
                {
                    //no need to handle the exception.
                }
            }
        }

        /// <summary>
        /// Load RouteMaps
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="hbActivities"></param>
        private void LoadRouteMapsData(IDataReader reader, ref List<Activity> hbActivities)
        {
            while (reader.Read())
            {
                var activityId = DbPropertyHelper.Int32PropertyFromRow(reader, "ServiceID");
                var listedProduct = hbActivities.Find(a => a.ID == activityId);

                var routeMap = new RouteMap
                {
                    RouteMapTitle = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "RouteMapTitle"),
                    RouteMaporder = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "RouteMaporder"),
                    ImagePath = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "ImagePath") ?? ""
                };

                if (listedProduct.RouteMaps == null)
                {
                    listedProduct.RouteMaps = new List<RouteMap>() { routeMap };
                }
                else
                    listedProduct.RouteMaps.Add(routeMap);
            }
        }

        /// <summary>
        /// Load activity offers data
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="hbActivities"></param>
        private void LoadActivityOffersData(IDataReader reader, ref List<Activity> hbActivities)
        {
            while (reader.Read())
            {
                var activityId = DbPropertyHelper.Int32PropertyFromRow(reader, "serviceid");
                var listedProduct = hbActivities.Find(a => a.ID == activityId);

                var activityOffers = new ActivityOffers
                {
                    OfferId = DbPropertyHelper.Int32PropertyFromRow(reader, "offerid"),
                    ServiceId = DbPropertyHelper.Int32PropertyFromRow(reader, "serviceid"),
                    OfferText = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "offertext"),
                    OfferOrder = DbPropertyHelper.Int32PropertyFromRow(reader, "offerorder"),
                    StartDate = DbPropertyHelper.DateTimeNullablePropertyFromRow(reader, "startdate"),
                    EndDate = DbPropertyHelper.DateTimeNullablePropertyFromRow(reader, "end_date")
                };

                if (listedProduct.ActivityOffers == null)
                {
                    listedProduct.ActivityOffers = new List<ActivityOffers>() { activityOffers };
                }
                else
                    listedProduct.ActivityOffers.Add(activityOffers);
            }
        }

        /// <summary>
        /// Load option data
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="hbActivities"></param>
        private void LoadOptionData(IDataReader reader, ref List<Activity> hbActivities)
        {
            var allOptions = new Dictionary<int, List<ProductOption>>();
            var options4Id = new List<ProductOption>();

            var prevId = 0;
            while (reader.Read())
            {
                var id = DbPropertyHelper.Int32PropertyFromRow(reader, "serviceid");

                var option = new ActivityOption
                {
                    Id = DbPropertyHelper.Int32PropertyFromRow(reader, "ServiceOptionID"),
                    Name = DbPropertyHelper.StringPropertyFromRow(reader, "ServiceOptionName"),
                    SupplierOptionCode = DbPropertyHelper.StringPropertyFromRow(reader, "SupplierOptionCode"),
                    Description = DbPropertyHelper.StringPropertyFromRow(reader, "Description"),
                    HotelPickUpLocation = DbPropertyHelper.StringPropertyFromRow(reader, "ScheduleHotelPickup"),
                    PickUpOption = (PickUpDropOffOptionType)Enum.Parse(typeof(PickUpDropOffOptionType), DbPropertyHelper.StringPropertyFromRow(reader, "PickUpOptionID")),
                    ScheduleReturnDetails = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "ScheduleReturnDetails"),
                    Margin = new Margin
                    {
                        Value = !string.IsNullOrEmpty(DbPropertyHelper.StringDefaultPropertyFromRow(reader, "marginAmount")) ? DbPropertyHelper.DecimalPropertyFromRow(reader, "marginAmount") : 0, //Added condition as sometimes db returns null value
                        IsPercentage = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "IsPercent")
                    },
                    PrefixServiceCode = DbPropertyHelper.StringPropertyFromRow(reader, "PrefixServiceCode"),
                    IsIsangoMarginApplicable = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "IsIsangoMarginApplicable")
                };

                if (prevId == id)
                    options4Id.Add(option);
                else
                {
                    if (!allOptions.ContainsKey(prevId))
                        allOptions.Add(prevId, options4Id);
                    options4Id = new List<ProductOption>() { option };
                    prevId = id;
                }
            }
            if (!allOptions.ContainsKey(prevId))
                allOptions.Add(prevId, options4Id);
            foreach (var options in allOptions)
            {
                if (options.Key != 0)
                {
                    var listedProduct = hbActivities.Find(a => a.ID == options.Key);
                    var tmpOptions = new List<ProductOption>();
                    foreach (var item in options.Value)
                    {
                        var opt = new ActivityOption()
                        {
                            Id = item.Id,
                            Name = item.Name,
                            Code = ((ActivityOption)item).Code,
                            Description = item.Description,
                            HotelPickUpLocation = ((ActivityOption)item).HotelPickUpLocation,
                            SupplierOptionCode = ((ActivityOption)item).SupplierOptionCode,
                            PickUpOption = ((ActivityOption)item).PickUpOption,
                            ScheduleReturnDetails = ((ActivityOption)item).ScheduleReturnDetails,
                            TravelInfo = GetDefaultTravelInfo(),
                            Margin = item.Margin,
                            PrefixServiceCode = item.PrefixServiceCode,
                            OptionOrder = item.OptionOrder,
                            IsIsangoMarginApplicable = item.IsIsangoMarginApplicable
                        };

                        tmpOptions.Add(opt);
                    }

                    //HACK: We know there is only 1 Occupancy Unit
                    listedProduct.ProductOptions = tmpOptions;
                }
            }
        }

        /// <summary>
        /// Load review data
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="hbActivities"></param>
        private void LoadReviewData(IDataReader reader, ref List<Activity> hbActivities)
        {
            while (reader.Read())
            {
                var activityId = DbPropertyHelper.Int32PropertyFromRow(reader, "serviceid");
                var listedProduct = hbActivities.Find(a => a.ID == activityId);

                var userReview = new Review
                {
                    Title = DbPropertyHelper.StringPropertyFromRow(reader, "title"),
                    Text = DbPropertyHelper.StringPropertyFromRow(reader, "reviewcomments"),
                    Rating = DbPropertyHelper.StringPropertyFromRow(reader, "overallrating"),
                    UserName = DbPropertyHelper.StringPropertyFromRow(reader, "firstname"),
                    Country = DbPropertyHelper.StringPropertyFromRow(reader, "country"),
                    IsFeefo = DbPropertyHelper.BoolPropertyFromRow(reader, "IsFeefoReview"),
                    SubmittedDate = DbPropertyHelper.DateTimePropertyFromRow(reader, "reviewdate")
                };
                if (listedProduct.Reviews == null)
                {
                    listedProduct.Reviews = new List<Review>() { userReview };
                }
                else
                    listedProduct.Reviews.Add(userReview);
            }
        }

        /// <summary>
        /// Load image data
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="hbActivities"></param>
        private void LoadImageData(IDataReader reader, ref List<Activity> hbActivities)
        {
            while (reader.Read())
            {
                var activityId = DbPropertyHelper.Int32PropertyFromRow(reader, "serviceid");
                var listedProduct = hbActivities.Find(a => a.ID == activityId);

                var productImage = new ProductImage
                {
                    Name = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "ImageFileName"),
                    Description = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "ImageDescription"),
                    ID = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "ImageID"),
                    ImageType = (ImageType)Enum.Parse(typeof(ImageType),
                        DbPropertyHelper.StringDefaultPropertyFromRow(reader, "ProductImageType")),
                    ImageSequence = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "IMAGESEQUENCE"),
                };

                if (reader["Thumbnail"] != DBNull.Value && Convert.ToBoolean(reader["Thumbnail"]))
                {
                    productImage.Thumbnail = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "Thumbnail");
                }

                if (listedProduct.Images == null)
                {
                    listedProduct.Images = new List<ProductImage>() { productImage };
                }
                else
                    listedProduct.Images.Add(productImage);
            }
        }

        /// <summary>
        /// Load category data
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="hbActivities"></param>
        private void LoadCategoryData(IDataReader reader, ref List<Activity> hbActivities)
        {
            while (reader.Read())
            {
                var activityId = DbPropertyHelper.Int32PropertyFromRow(reader, "serviceid");
                var listedProduct = hbActivities.Find(a => a.ID == activityId);

                var attractionId = DbPropertyHelper.Int32PropertyFromRow(reader, "attractionid");
                var sequence = DbPropertyHelper.Int32PropertyFromRow(reader, "sequence");
                if (listedProduct.CategoryIDs == null)
                {
                    listedProduct.CategoryIDs = new List<int> { attractionId };
                }
                else
                    listedProduct.CategoryIDs.Add(attractionId);

                if (listedProduct.PriorityWiseCategory == null && sequence > 0)
                {
                    listedProduct.PriorityWiseCategory = new Dictionary<int, int> { { attractionId, sequence } };
                }
                else if (sequence > 0)
                {
                    listedProduct.PriorityWiseCategory?.Add(attractionId, sequence);
                }
            }
        }

        /// <summary>
        /// Load region data
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="hbActivities"></param>
        /// <returns></returns>
        private void LoadRegionData(IDataReader reader, ref List<Activity> hbActivities)
        {
            while (reader.Read())
            {
                var activityId = DbPropertyHelper.Int32PropertyFromRow(reader, "serviceid");
                var listedProduct = hbActivities.Find(a => a.ID == activityId);

                if (listedProduct.Regions == null)
                    listedProduct.Regions = new List<Region>();

                listedProduct.Regions.Add(new Region
                {
                    Id = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "RegionID"),
                    Name = DbPropertyHelper.StringPropertyFromRow(reader, "RegionName"),
                    Type = (RegionType)Enum.Parse(typeof(RegionType),
                            DbPropertyHelper.StringPropertyFromRow(reader, "RLevel")),
                    ParentId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "ParentRegionID"),
                    IsoCode = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "ISOCode") ?? ""
                });
            }
        }

        /// <summary>
        /// Load times of day for product option of activity
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="hbActivities"></param>
        private void LoadTimesOfDay(IDataReader reader, ref List<Activity> hbActivities)
        {
            while (reader.Read())
            {
                var serviceId = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "SERVICEID");
                var serviceOptionID = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "ServiceOptionID");

                var timesOfDay = new TimesOfDay
                {
                    StartTime = (!String.IsNullOrEmpty(Convert.ToString(reader["StartTime"]))) ? TimeSpan.Parse(Convert.ToString(reader["StartTime"])) : TimeSpan.Zero,
                    EndTime = (!String.IsNullOrEmpty(Convert.ToString(reader["EndTime"]))) ? TimeSpan.Parse(Convert.ToString(reader["EndTime"])) : TimeSpan.Zero,
                    DurationDay = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "DurationDay"),
                    TotalDuration = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "TotalDuration"),
                    AppliedFromDate = (!String.IsNullOrEmpty(Convert.ToString(reader["AppliedFromDate"]))) ? DbPropertyHelper.DateTimePropertyFromRow(reader, "AppliedFromDate") : DateTime.MinValue,
                    AppliedToDate = (!String.IsNullOrEmpty(Convert.ToString(reader["AppliedEndDate"]))) ? DbPropertyHelper.DateTimePropertyFromRow(reader, "AppliedEndDate") : DateTime.MinValue,
                };

                var hbActivity = hbActivities.Find(x => x.Id == serviceId);

                if (hbActivity != null)
                {
                    var hbProductOption = hbActivity.ProductOptions?.Find(y => y.Id == serviceOptionID);
                    if (hbProductOption != null && hbProductOption.TimesOfDays == null)
                    {
                        hbProductOption.TimesOfDays = new List<TimesOfDay>();
                    }

                    hbProductOption?.TimesOfDays.Add(timesOfDay);
                }
            }
        }

        /// <summary>
        /// Load rating for each activity
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="hbActivities"></param>
        private void LoadRatings(IDataReader reader, ref List<Activity> hbActivities)
        {
            while (reader.Read())
            {
                var serviceId = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "serviceid");

                var rating = new Rating
                {
                    ServiceTypeRatingId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "servicetyperatingid"),
                    ServiceTypeRatingName = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "servicetyperatingname"),
                    ServiceTypeRatingTypeId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "servicetyperatingtypeid"),
                    ServiceTypeRatingTypeName = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "servicetyperatingtypename")
                };

                var hbActivity = hbActivities.Find(x => x.Id == serviceId);
                if (hbActivity != null)
                {
                    hbActivity.Ratings.Add(rating);
                }
            }
        }

        #endregion Private methods of LoadLiveHbActivityData

        /// <summary>
        /// Load Activity From DataSet
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="activities"></param>
        private void LoadActivityFromDataSet(DataTableCollection dataTable, ref List<Activity> activities)
        {
            var rows = dataTable[0].Rows;
            var rowCount = rows.Count;
            for (var i = 0; i < rowCount; i++)
            {
                var row = rows[i];
                var type = ActivityType.Undefined;
                int tempIntValue;
                if (!string.IsNullOrEmpty(DbPropertyHelper.StringDefaultPropertyFromRow(row, "ServiceTypeId")))
                {
                    int.TryParse(row["ServiceTypeId"].ToString(), out tempIntValue);
                    if (tempIntValue.Equals(1) || tempIntValue.Equals(2) || tempIntValue.Equals(3))
                    {
                        tempIntValue = 1;
                    }
                    type = (ActivityType)Enum.Parse(typeof(ActivityType), tempIntValue.ToString());
                }

                var durationInDaysList = new List<double>();
                var durationInDays = DbPropertyHelper.StringDefaultPropertyFromRow(row, "DurationInDays");
                if (!string.IsNullOrEmpty(durationInDays))
                {
                    var durationDays = durationInDays.Split(',');
                    foreach (var duration in durationDays)
                    {
                        durationInDaysList.Add(Convert.ToDouble(duration));
                    }
                }

                var activity = new Activity
                {
                    Id = DbPropertyHelper.StringDefaultPropertyFromRow(row, "ServiceID"),
                    ActivityType = type,
                    ID = DbPropertyHelper.Int32DefaultPropertyFromRow(row, "ServiceID"),
                    LanguageCode = DbPropertyHelper.StringPropertyFromRow(row, "languagecode"),
                    Name = DbPropertyHelper.StringDefaultPropertyFromRow(row, "ServiceName"),
                    ShortIntroduction = DbPropertyHelper.StringDefaultPropertyFromRow(row, "ShortDescription"),
                    MetaKeywords = DbPropertyHelper.StringDefaultPropertyFromRow(row, "MetaKeywords"),
                    MetaDescription = DbPropertyHelper.StringDefaultPropertyFromRow(row, "MetaDescription"),
                    Introduction = DbPropertyHelper.StringDefaultPropertyFromRow(row, "Introduction"),
                    DurationString = DbPropertyHelper.StringDefaultPropertyFromRow(row, "Duration"),
                    Schedule = DbPropertyHelper.StringDefaultPropertyFromRow(row, "Schedule"),
                    ScheduleOperates = DbPropertyHelper.StringDefaultPropertyFromRow(row, "ScheduleOperates"),
                    ScheduleUnavailableDates = DbPropertyHelper.StringDefaultPropertyFromRow(row, "ScheduleUnavailableDates"),
                    ScheduleLocation = DbPropertyHelper.StringDefaultPropertyFromRow(row, "ScheduleLocation"),
                    HotelPickUpLocation = DbPropertyHelper.StringDefaultPropertyFromRow(row, "ScheduleHotelPickup"),
                    ScheduleReturnDetails = DbPropertyHelper.StringDefaultPropertyFromRow(row, "ScheduleReturnDetails"),
                    Inclusions = DbPropertyHelper.StringDefaultPropertyFromRow(row, "Inclusions"),
                    Exclusions = DbPropertyHelper.StringDefaultPropertyFromRow(row, "Exclusions"),
                    DoDont = DbPropertyHelper.StringDefaultPropertyFromRow(row, "DoDont"),
                    PleaseNote = DbPropertyHelper.StringDefaultPropertyFromRow(row, "PleaseNote"),
                    ChildPolicy = DbPropertyHelper.StringDefaultPropertyFromRow(row, "ChildPolicy"),
                    CancellationPolicy = FilterCancellationPolicy(DbPropertyHelper.StringDefaultPropertyFromRow(row, "CancellationPolicy")),
                    AlertNote = DbPropertyHelper.StringDefaultPropertyFromRow(row, "AlertNote"),
                    AdditionalInfo = DbPropertyHelper.StringDefaultPropertyFromRow(row, "AdditionalInfo"),
                    CoOrdinates = DbPropertyHelper.StringDefaultPropertyFromRow(row, "CoOrdinates"),
                    BookingWindow = DbPropertyHelper.Int32DefaultPropertyFromRow(row, "BookingWindowDays"),
                    AdditionalMarkUp = DbPropertyHelper.FloatPropertyFromRow(row, "destinationmarkup"),
                    Title = DbPropertyHelper.StringDefaultPropertyFromRow(row, "PageTitle"),
                    IsServiceLevelPickUp = DbPropertyHelper.BoolDefaultPropertyFromRow(row, "IsPickUpServicelevel"),
                    PriceTypeId = (PriceTypeId)Enum.Parse(typeof(PriceTypeId), DbPropertyHelper.StringDefaultPropertyFromRow(row, "PriceTypeID")),
                    IsLivePrice = DbPropertyHelper.BoolDefaultPropertyFromRow(row, "IsLivePrice"),
                    CurrencyIsoCode = DbPropertyHelper.StringDefaultPropertyFromRow(row, "Sell_CurrencyIsoCode"),
                    CanonicalURL = DbPropertyHelper.StringDefaultPropertyFromRow(row, "CanonicalURL"),
                    IsPackage = DbPropertyHelper.BoolDefaultPropertyFromRow(row, "IsPackage"),
                    Itineraries = new List<ActivityItinerary>(),
                    ReasonToBook = new List<string>(),
                    Badges = new List<Badge>(),
                    Regions = new List<Region>(),
                    RouteMaps = new List<RouteMap>(),
                    CategoryIDs = new List<int>(),
                    PriorityWiseCategory = new Dictionary<int, int>(),
                    Images = new List<ProductImage>(),
                    Reviews = new List<Review>(),
                    ApiType = (APIType)Enum.Parse(typeof(APIType), DbPropertyHelper.StringPropertyFromRow(row, "APITypeId")),
                    CategoryTypes = new List<ActivityCategoryType>(),
                    IsPaxDetailRequired = (!string.IsNullOrEmpty(DbPropertyHelper.StringDefaultPropertyFromRow(row, "IsPaxDetailRequired")) &&
                                                Convert.ToInt32(DbPropertyHelper.StringDefaultPropertyFromRow(row, "IsPaxDetailRequired")) > 0),
                    IsReceipt = DbPropertyHelper.BoolDefaultPropertyFromRow(row, "IsReceipt"),
                    MeetingPointCoordinate = DbPropertyHelper.StringDefaultPropertyFromRow(row, "MeetingPointCoordinate"),
                    Priority = DbPropertyHelper.Int32DefaultPropertyFromRow(row, "Priority"),
                    Duration = durationInDaysList,
                    PickUpOption = (PickUpDropOffOptionType)Enum.Parse(typeof(PickUpDropOffOptionType), DbPropertyHelper.StringDefaultPropertyFromRow(row, "PickupOptionID")),
                    ActualServiceUrl = DbPropertyHelper.StringDefaultPropertyFromRow(row, "actual_url"),
                    OverAllRating = DbPropertyHelper.DoublePropertyFromRow(row, "customer_reviewavgrating"),
                    ProductType = ProductType.Activity,
                    TotalReviews = DbPropertyHelper.Int32DefaultPropertyFromRow(row, "customer_reviewcount"),
                    Ratings = new List<Rating>(),
                    LineOfBusinessId = DbPropertyHelper.Int32DefaultPropertyFromRow(row, "LINEOFBUSINESSID"),
                    AttractionsCovered = DbPropertyHelper.StringDefaultPropertyFromRow(row, "ATTRACTIONSCOVERED"),
                    ToDoOnArrival = DbPropertyHelper.StringDefaultPropertyFromRow(row, "TODOONARRIVAL"),
                    WhyDoThis = DbPropertyHelper.StringDefaultPropertyFromRow(row, "WHYDOTHIS"),
                    LiveOnDate = DbPropertyHelper.DateTimeNullablePropertyFromRow(row, "LiveOnDate"),
                    DurationDay = DbPropertyHelper.Int32DefaultPropertyFromRow(row, "DurationDay"),
                    DurationTime = (!String.IsNullOrEmpty(Convert.ToString(row["DurationTime"]))) ? TimeSpan.Parse(Convert.ToString(row["DurationTime"])) : TimeSpan.Zero,
                    DurationAdditionText = DbPropertyHelper.StringDefaultPropertyFromRow(row, "DurationAdditionText"),
                    WhereYouStay = DbPropertyHelper.StringDefaultPropertyFromRow(row, "WhereYouStay"),
                    WhyYouLove = DbPropertyHelper.StringDefaultPropertyFromRow(row, "WhyYouLove"),
                    IsNoIndex = DbPropertyHelper.BoolDefaultPropertyFromRow(row, "IsNoIndex"),
                    IsHighDefinationImages = DbPropertyHelper.BoolDefaultPropertyFromRow(row, "IsHighDefinationImages"),
                    IsFollow = DbPropertyHelper.BoolDefaultPropertyFromRow(row, "IsFollow"),
                    IsGoogleFeed = DbPropertyHelper.BoolDefaultPropertyFromRow(row, "IsGoogleFeed"),
                    TourLaunchDate = DbPropertyHelper.DateTimeNullablePropertyFromRow(row, "TourLaunchDate"),
                    IsTimeBase = DbPropertyHelper.BoolDefaultPropertyFromRow(row, "IsTimeBase"),
                    CancellationSummary = FilterCancellationPolicy(DbPropertyHelper.StringDefaultPropertyFromRow(row, "Cancellation_Summary")),
                    DurationSummary = DbPropertyHelper.StringDefaultPropertyFromRow(row, "Duration_Summary"),
                    StartTimeSummary = DbPropertyHelper.StringDefaultPropertyFromRow(row, "StartTime_Summary"),
                    SupplierID = DbPropertyHelper.Int32DefaultPropertyFromRow(row, "SupplierID"),
                    isperpersonprice = DbPropertyHelper.BoolDefaultPropertyFromRow(row, "isperpersonprice"),
                    UnitText = DbPropertyHelper.StringDefaultPropertyFromRow(row, "UnitText"),
                    IsPrivateTour = DbPropertyHelper.BoolDefaultPropertyFromRow(row, "isprivatetour"),
                    IsBundle = DbPropertyHelper.BoolDefaultPropertyFromRow(row, "IsBundle"),
                    SupplierName = DbPropertyHelper.StringDefaultPropertyFromRow(row, "SupplierName"),
                    ISSHOWSUPPLIERVOUCHER = DbPropertyHelper.BoolDefaultPropertyFromRow(row, "ISSHOWSUPPLIERVOUCHER"),
                    AdyenStringentAccount = DbPropertyHelper.BoolDefaultPropertyFromRow(row, "AdyenStringentAccount"),
                    ServiceStatusID = DbPropertyHelper.Int32NullablePropertyFromRow(row, "servicestatusid"),
                    IsHideGatePrice = DbPropertyHelper.BoolDefaultPropertyFromRow(row, "IsHideGatePrice"),

                };
                activities.Add(activity);
            }
        }

        /// <summary>
        /// Load Activity Itineraries From DataSet
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="activities"></param>
        private void LoadActivityItinerariesFromDataSet(DataTableCollection dataTable, ref List<Activity> activities)
        {
            var rows = dataTable[1].Rows;
            var rowCount = rows.Count;
            for (var i = 0; i < rowCount; i++)
            {
                var row = rows[i];
                var serviceId = DbPropertyHelper.StringDefaultPropertyFromRow(row, "serviceid");
                var activityItinerary = new ActivityItinerary
                {
                    Description = DbPropertyHelper.StringDefaultPropertyFromRow(row, "Description"),
                    Title = DbPropertyHelper.StringDefaultPropertyFromRow(row, "Title"),
                    Order = DbPropertyHelper.Int32DefaultPropertyFromRow(row, "ZOrder")
                };
                activities.FirstOrDefault(x => x.Id == serviceId)?.Itineraries.Add(activityItinerary);
            }
        }

        /// <summary>
        /// Load ReasonToBook From DataSet
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="activities"></param>
        private void LoadReasonToBookFromDataSet(DataTableCollection dataTable, ref List<Activity> activities)
        {
            var rows = dataTable[2].Rows;
            var rowCount = rows.Count;
            for (var i = 0; i < rowCount; i++)
            {
                var row = rows[i];
                var serviceId = DbPropertyHelper.StringDefaultPropertyFromRow(row, "serviceid");
                activities.FirstOrDefault(x => x.Id == serviceId)?.ReasonToBook.Add(DbPropertyHelper.StringDefaultPropertyFromRow(row, "TextDetails"));
            }
        }

        /// <summary>
        /// Load Badges From DataSet
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="activities"></param>
        private void LoadBadgesFromDataSet(DataTableCollection dataTable, ref List<Activity> activities)
        {
            var rows = dataTable[3].Rows;
            var rowCount = rows.Count;
            for (var i = 0; i < rowCount; i++)
            {
                var row = rows[i];
                var serviceId = DbPropertyHelper.StringDefaultPropertyFromRow(row, "serviceid");
                var badge = new Badge
                {
                    Id = DbPropertyHelper.Int32DefaultPropertyFromRow(row, "id"),
                    Name = DbPropertyHelper.StringDefaultPropertyFromRow(row, "TagName")
                };
                activities.FirstOrDefault(x => x.Id == serviceId)?.Badges.Add(badge);
            }
        }

        /// <summary>
        /// Load Regions From DataSet
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="activities"></param>
        private void LoadRegionsFromDataSet(DataTableCollection dataTable, ref List<Activity> activities)
        {
            var rows = dataTable[4].Rows;
            var rowCount = rows.Count;
            for (var i = 0; i < rowCount; i++)
            {
                var row = rows[i];
                var serviceId = DbPropertyHelper.StringDefaultPropertyFromRow(row, "serviceid");
                var region = new Region
                {
                    Id = DbPropertyHelper.Int32DefaultPropertyFromRow(row, "RegionID"),
                    Name = DbPropertyHelper.StringDefaultPropertyFromRow(row, "RegionName"),
                    Type = (RegionType)Enum.Parse(typeof(RegionType), DbPropertyHelper.StringDefaultPropertyFromRow(row, "RLevel")),
                    ParentId = DbPropertyHelper.Int32DefaultPropertyFromRow(row, "ParentRegionID"),
                    IsoCode = DbPropertyHelper.StringDefaultPropertyFromRow(row, "ISOCode") ?? ""
                };
                activities.FirstOrDefault(x => x.Id == serviceId)?.Regions.Add(region);
            }
        }

        /// <summary>
        /// Load RouteMaps
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="activities"></param>
        private void LoadRouteMapsFromDataSet(DataTableCollection dataTable, ref List<Activity> activities)
        {
            var rows = dataTable[19].Rows;
            var rowCount = rows.Count;
            for (var i = 0; i < rowCount; i++)
            {
                var row = rows[i];
                var serviceId = DbPropertyHelper.StringDefaultPropertyFromRow(row, "ServiceID");
                var routeMap = new RouteMap
                {
                    RouteMapTitle = DbPropertyHelper.StringDefaultPropertyFromRow(row, "RouteMapTitle"),
                    RouteMaporder = DbPropertyHelper.Int32DefaultPropertyFromRow(row, "RouteMaporder"),
                    ImagePath = DbPropertyHelper.StringDefaultPropertyFromRow(row, "ImagePath") ?? ""
                };
                activities.FirstOrDefault(x => x.Id == serviceId)?.RouteMaps.Add(routeMap);
            }
        }

        /// <summary>
        /// Load CategoryIDs From DataSet
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="activities"></param>
        private void LoadCategoryIDsFromDataSet(DataTableCollection dataTable, ref List<Activity> activities)
        {
            var rows = dataTable[5].Rows;
            var rowCount = rows.Count;
            for (var i = 0; i < rowCount; i++)
            {
                var row = rows[i];
                var serviceId = DbPropertyHelper.StringDefaultPropertyFromRow(row, "serviceid");
                var attractionId = DbPropertyHelper.Int32DefaultPropertyFromRow(row, "attractionid");
                var sequenceString = !string.IsNullOrEmpty(DbPropertyHelper.StringDefaultPropertyFromRow(row, "sequence"));
                var sequenceInt = DbPropertyHelper.Int32DefaultPropertyFromRow(row, "sequence");

                if (sequenceString && sequenceInt > 0)
                {
                    activities.FirstOrDefault(x => x.Id == serviceId)?.PriorityWiseCategory.Add(attractionId, sequenceInt);
                }
                activities.FirstOrDefault(x => x.Id == serviceId)?.CategoryIDs.Add(attractionId);
            }
        }

        /// <summary>
        /// Load Images From DataSet
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="activities"></param>
        private void LoadImagesFromDataSet(DataTableCollection dataTable, ref List<Activity> activities)
        {
            var rows = dataTable[6].Rows;
            var rowCount = rows.Count;
            for (var i = 0; i < rowCount; i++)
            {
                var row = rows[i];
                var serviceId = DbPropertyHelper.StringDefaultPropertyFromRow(row, "serviceid");
                var productImage = new ProductImage
                {
                    Name = DbPropertyHelper.StringDefaultPropertyFromRow(row, "ImageFileName"),
                    Description = DbPropertyHelper.StringDefaultPropertyFromRow(row, "ImageDescription"),
                    ID = DbPropertyHelper.Int32DefaultPropertyFromRow(row, "ImageID"),
                    ImageType = (ImageType)Enum.Parse(typeof(ImageType),
                    DbPropertyHelper.StringDefaultPropertyFromRow(row, "ProductImageType")),
                    ImageSequence = DbPropertyHelper.Int32DefaultPropertyFromRow(row, "IMAGESEQUENCE"),
                };

                if (row["Thumbnail"] != DBNull.Value && Convert.ToBoolean(row["Thumbnail"]))
                {
                    productImage.Thumbnail = DbPropertyHelper.BoolDefaultPropertyFromRow(row, "Thumbnail");
                }
                activities.FirstOrDefault(x => x.Id == serviceId)?.Images.Add(productImage);
            }
        }

        /// <summary>
        /// Load Reviews From DataSet
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="activities"></param>
        private void LoadReviewsFromDataSet(DataTableCollection dataTable, ref List<Activity> activities)
        {
            var rows = dataTable[7].Rows;
            var rowCount = rows.Count;
            for (var i = 0; i < rowCount; i++)
            {
                var row = rows[i];
                var serviceId = DbPropertyHelper.StringDefaultPropertyFromRow(row, "serviceid");
                var review = new Review
                {
                    Title = DbPropertyHelper.StringDefaultPropertyFromRow(row, "title"),
                    Text = DbPropertyHelper.StringDefaultPropertyFromRow(row, "reviewcomments"),
                    Rating = DbPropertyHelper.StringDefaultPropertyFromRow(row, "overallrating"),
                    UserName = DbPropertyHelper.StringDefaultPropertyFromRow(row, "firstname"),
                    Country = DbPropertyHelper.StringDefaultPropertyFromRow(row, "country"),
                    IsFeefo = DbPropertyHelper.BoolDefaultPropertyFromRow(row, "IsFeefoReview"),
                    SubmittedDate = DbPropertyHelper.DateTimePropertyFromRow(row, "reviewdate")
                };
                activities.FirstOrDefault(x => x.Id == serviceId)?.Reviews.Add(review);
            }
        }

        /// <summary>
        /// Get YouTubeLink From DataSet
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="activities"></param>
        private void LoadYouTubeLinkFromDataSet(DataTableCollection dataTable, ref List<Activity> activities)
        {
            var rows = dataTable[8].Rows;
            var rowCount = rows.Count;
            for (var i = 0; i < rowCount; i++)
            {
                var row = rows[i];
                var serviceId = DbPropertyHelper.StringDefaultPropertyFromRow(row, "serviceid");
                if (activities.Any(x => x.Id == serviceId))
                    activities.First(x => x.Id == serviceId).YouTubeLink = DbPropertyHelper.StringDefaultPropertyFromRow(row, "videoURL");
            }
        }

        /// <summary>
        /// Load CategoryTypes From DataSet
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="activities"></param>
        private void LoadCategoryTypesFromDataSet(DataTableCollection dataTable, ref List<Activity> activities)
        {
            var rows = dataTable[9].Rows;
            var rowCount = rows.Count;
            for (var i = 0; i < rowCount; i++)
            {
                var row = rows[i];
                var serviceId = DbPropertyHelper.StringDefaultPropertyFromRow(row, "serviceid");
                var categoryType = (ActivityCategoryType)Enum.Parse(typeof(ActivityCategoryType),
                    DbPropertyHelper.StringDefaultPropertyFromRow(row, "ServiceTyperatingId"));
                activities.FirstOrDefault(x => x.Id == serviceId)?.CategoryTypes.Add(categoryType);
            }
        }

        /// <summary>
        /// Load ProductOptions From DataSet
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="activities"></param>
        private void LoadOptionsFromDataSet(DataTableCollection dataTable, ref List<Activity> activities)
        {
            var rows = dataTable[10].Rows;
            var rowCount = rows.Count;
            for (var i = 0; i < rowCount; i++)
            {
                try
                {
                    var row = rows[i];
                    var serviceId = DbPropertyHelper.StringDefaultPropertyFromRow(row, "SERVICEID");

                    var option = new ActivityOption
                    {
                        Id = DbPropertyHelper.Int32PropertyFromRow(row, "serviceoptionid"),
                        ServiceOptionId = DbPropertyHelper.Int32PropertyFromRow(row, "serviceoptionid"),
                        Name = DbPropertyHelper.StringDefaultPropertyFromRow(row, "optionname"),
                        Description = DbPropertyHelper.StringDefaultPropertyFromRow(row, "optiondesc"),
                        SupplierName = DbPropertyHelper.StringDefaultPropertyFromRow(row, "SupplierOptionName"),
                        SupplierOptionCode = DbPropertyHelper.StringDefaultPropertyFromRow(row, "SupplierOptionCode").Trim(),
                        PickUpOption = (PickUpDropOffOptionType)Enum.Parse(typeof(PickUpDropOffOptionType), DbPropertyHelper.StringDefaultPropertyFromRow(row, "OptionPickupID", true)),
                        HotelPickUpLocation = DbPropertyHelper.StringDefaultPropertyFromRow(row, "Pickup"),
                        ScheduleReturnDetails = DbPropertyHelper.StringDefaultPropertyFromRow(row, "dropoff"),
                        Capacity = DbPropertyHelper.Int32DefaultPropertyFromRow(row, "occupancytypecapacity"),
                        Margin = new Margin
                        {
                            Value = !string.IsNullOrEmpty(DbPropertyHelper.StringDefaultPropertyFromRow(row, "marginAmount")) ? DbPropertyHelper.DecimalPropertyFromRow(row, "marginAmount") : 0, //Added condition as sometimes db returns null value
                            IsPercentage = DbPropertyHelper.BoolDefaultPropertyFromRow(row, "IsPercent")
                        },

                        ComponentOrder = DbPropertyHelper.Int32DefaultPropertyFromRow(row, "ComponentOrder_"),
                        ComponentServiceID = DbPropertyHelper.Int32DefaultPropertyFromRow(row, "ComponentServiceID"),
                        ComponentServiceName = DbPropertyHelper.StringDefaultPropertyFromRow(row, "ComponentServiceName"),
                        BundleOptionID = DbPropertyHelper.Int32DefaultPropertyFromRow(row, "BundleOptionID"),
                        PriceTypeID = DbPropertyHelper.Int32DefaultPropertyFromRow(row, "PriceTypeID"),
                        BundleOptionName = DbPropertyHelper.StringDefaultPropertyFromRow(row, "BundleOptionName"),
                        IsSameDayBookable = !string.IsNullOrEmpty(DbPropertyHelper.StringDefaultPropertyFromRow(row, "IsSameDayBookable")) && DbPropertyHelper.BoolDefaultPropertyFromRow(row, "IsSameDayBookable"),
                        IsIsangoMarginApplicable = DbPropertyHelper.BoolDefaultPropertyFromRow(row, "IsIsangoMarginApplicable")
                    };
                    if (activities.FirstOrDefault(x => x.Id == serviceId)?.ProductOptions == null)
                    {
                        if (activities.Any(x => x.Id == serviceId))
                            activities.First(x => x.Id == serviceId).ProductOptions = new List<ProductOption> { option };
                    }
                    else
                        activities.FirstOrDefault(x => x.Id == serviceId)?.ProductOptions.Add(option);
                }
                catch
                {
                    //no need to handle the exception.
                }
            }
        }

        /*
        /// <summary>
        /// Load DownloadLinks From DataSet
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="activities"></param>
        private void LoadDownloadLinksFromDataSet(DataTableCollection dataTable, ref List<Activity> activities)
        {
            var rows = dataTable[14].Rows;
            var rowCount = rows.Count;
            for (var i = 0; i < rowCount; i++)
            {
                try
                {
                    var row = rows[i];
                    var serviceId = DbPropertyHelper.StringDefaultPropertyFromRow(row, "serviceid");
                    var downloadLinks = new DownloadLinks
                    {
                        ServiceId = DbPropertyHelper.Int32PropertyFromRow(row, "serviceid"),
                        DownloadLink = DbPropertyHelper.StringDefaultPropertyFromRow(row, "Downloadlink"),
                        DownloadText = DbPropertyHelper.StringDefaultPropertyFromRow(row, "Downloadtext"),
                        StartDate = DbPropertyHelper.DateTimeNullablePropertyFromRow(row, "Startdate"),
                        EndDate = DbPropertyHelper.DateTimeNullablePropertyFromRow(row, "Enddate")
                    };

                    var activity = activities.Find(x => x.Id == serviceId);
                    if (activity != null)
                    {
                        activity.Ratings.Add(rating);
                    }
                }
                catch
                {
                }
            }
        }
        //*/

        /// <summary>
        /// Load times of day for product option of activity
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="activities"></param>
        private void LoadTimesOfDayFromDataSet(DataTableCollection dataTable, ref List<Activity> activities)
        {
            var rows = dataTable[11].Rows;
            var rowCount = rows.Count;
            for (var i = 0; i < rowCount; i++)
            {
                var row = rows[i];
                var serviceId = DbPropertyHelper.StringDefaultPropertyFromRow(row, "SERVICEID");
                var serviceOptionId = DbPropertyHelper.Int32DefaultPropertyFromRow(row, "ServiceOptionID");

                var timesOfDay = new TimesOfDay
                {
                    StartTime = (!String.IsNullOrEmpty(Convert.ToString(row["StartTime"]))) ? TimeSpan.Parse(Convert.ToString(row["StartTime"])) : TimeSpan.Zero,
                    EndTime = (!String.IsNullOrEmpty(Convert.ToString(row["EndTime"]))) ? TimeSpan.Parse(Convert.ToString(row["EndTime"])) : TimeSpan.Zero,
                    DurationDay = DbPropertyHelper.StringDefaultPropertyFromRow(row, "DurationDay"),
                    TotalDuration = DbPropertyHelper.StringDefaultPropertyFromRow(row, "TotalDuration"),
                    AppliedFromDate = (!String.IsNullOrEmpty(Convert.ToString(row["AppliedFromDate"]))) ? DbPropertyHelper.DateTimePropertyFromRow(row, "AppliedFromDate") : DateTime.MinValue,
                    AppliedToDate = (!String.IsNullOrEmpty(Convert.ToString(row["AppliedEndDate"]))) ? DbPropertyHelper.DateTimePropertyFromRow(row, "AppliedEndDate") : DateTime.MinValue,
                };

                var activity = activities.Find(x => x.Id == serviceId);

                if (activity != null)
                {
                    var productOption = activity.ProductOptions?.Find(y => y.Id == serviceOptionId);
                    if (productOption != null && productOption.TimesOfDays == null)
                    {
                        productOption.TimesOfDays = new List<TimesOfDay>();
                    }

                    productOption?.TimesOfDays.Add(timesOfDay);
                }
            }
        }

        /// <summary>
        /// Load rating for each activity
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="activities"></param>
        private void LoadRatingsFromDataSet(DataTableCollection dataTable, ref List<Activity> activities)
        {
            var rows = dataTable[12].Rows;
            var rowCount = rows.Count;
            for (int i = 0; i < rowCount; i++)
            {
                var row = rows[i];
                var serviceId = DbPropertyHelper.StringDefaultPropertyFromRow(row, "serviceid");

                var rating = new Rating
                {
                    ServiceTypeRatingId = DbPropertyHelper.Int32DefaultPropertyFromRow(row, "servicetyperatingid"),
                    ServiceTypeRatingName = DbPropertyHelper.StringDefaultPropertyFromRow(row, "servicetyperatingname"),
                    ServiceTypeRatingTypeId = DbPropertyHelper.Int32DefaultPropertyFromRow(row, "servicetyperatingtypeid"),
                    ServiceTypeRatingTypeName = DbPropertyHelper.StringDefaultPropertyFromRow(row, "servicetyperatingtypename")
                };

                var activity = activities.Find(x => x.Id == serviceId);
                if (activity != null)
                {
                    activity.Ratings.Add(rating);
                }
            }
        }

        private void LoadCloseOutFromDataSet(DataTableCollection dataTable, ref List<Activity> activities)
        {
            var rows = dataTable[13].Rows;
            var rowCount = rows.Count;
            for (int i = 0; i < rowCount; i++)
            {
                var row = rows[i];
                var serviceId = DbPropertyHelper.StringDefaultPropertyFromRow(row, "serviceid");
                var serviceOptionId = DbPropertyHelper.Int32DefaultPropertyFromRow(row, "serviceoptioninserviceid");

                var closeout = new CloseOut
                {
                    CloseOutMin = DbPropertyHelper.Int32DefaultPropertyFromRow(row, "CLOSEOUTMIN"),
                    AppliedFromDate = DbPropertyHelper.DateTimePropertyFromRow(row, "APPLIEDRULEFROMDATE"),
                    AppliedToDate = DbPropertyHelper.DateTimePropertyFromRow(row, "APPLIEDRULETODATE")
                };

                var activity = activities.Find(x => x.Id == serviceId);

                if (activity == null) continue;
                var productOption = activity.ProductOptions?.Find(y => y.Id == serviceOptionId);
                if (productOption != null && productOption.CloseOuts == null)
                {
                    productOption.CloseOuts = new List<CloseOut>();
                }

                productOption?.CloseOuts.Add(closeout);
            }
        }

        private void LoadPickupLocationsFromDataSet(DataTableCollection dataTable, ref List<Activity> activities)
        {
            var rows = dataTable[18].Rows;
            var rowCount = rows.Count;
            for (var i = 0; i < rowCount; i++)
            {
                try
                {
                    var row = rows[i];
                    var serviceId = DbPropertyHelper.StringDefaultPropertyFromRow(row, "Serviceid");
                    var activityPickups = new ActivityPickupLocations
                    {
                        ServiceID = DbPropertyHelper.Int32PropertyFromRow(row, "Serviceid"),
                        Serviceoptionid = DbPropertyHelper.Int32NullablePropertyFromRow(row, "Serviceoptionid"),
                        Pickuplocation = DbPropertyHelper.StringDefaultPropertyFromRow(row, "Pickuplocation"),
                        Languagecode = DbPropertyHelper.StringDefaultPropertyFromRow(row, "Languagecode"),
                        ID = DbPropertyHelper.Int32NullablePropertyFromRow(row, "ID")
                    };

                    var IsServiceLevel = activityPickups.Serviceoptionid == null || activityPickups.Serviceoptionid == 0;

                    if (IsServiceLevel)
                    {
                        if (activities?.FirstOrDefault(x => x.Id == serviceId)?.PickupLocations == null)
                        {
                            if (activities.Any(x => x.Id == serviceId))
                                activities.First(x => x.Id == serviceId).PickupLocations = new List<ActivityPickupLocations> { activityPickups };
                        }
                        else
                            activities.FirstOrDefault(x => x.Id == serviceId)?.PickupLocations.Add(activityPickups);
                    }
                    else
                    {
                        if (activities?.FirstOrDefault(x => x.Id == serviceId)?.ProductOptions?.FirstOrDefault(x => x.ServiceOptionId == activityPickups.Serviceoptionid)?.OptionPickupLocations == null)
                        {
                            if (activities.FirstOrDefault(x => x.Id == serviceId)?.ProductOptions?.Any(x => x.ServiceOptionId == activityPickups.Serviceoptionid) ?? false)
                                activities.First(x => x.Id == serviceId).ProductOptions.FirstOrDefault(x => x.ServiceOptionId == activityPickups.Serviceoptionid).OptionPickupLocations = new List<ActivityPickupLocations> { activityPickups };
                        }
                        else
                            activities.First(x => x.Id == serviceId).ProductOptions.FirstOrDefault(x => x.ServiceOptionId == activityPickups.Serviceoptionid).OptionPickupLocations.Add(activityPickups);
                    }

                }
                catch
                {
                    //no need to handle the exception.
                }
            }
        }

        /// <summary>
        /// Load DownloadLinks From DataSet
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="activities"></param>
        private void LoadActivityOffersFromDataSet(DataTableCollection dataTable, ref List<Activity> activities)
        {
            var rows = dataTable[15].Rows;
            var rowCount = rows.Count;
            for (var i = 0; i < rowCount; i++)
            {
                try
                {
                    var row = rows[i];
                    var serviceId = DbPropertyHelper.StringDefaultPropertyFromRow(row, "serviceid");
                    var activityOffers = new ActivityOffers
                    {
                        OfferId = DbPropertyHelper.Int32PropertyFromRow(row, "offerid"),
                        ServiceId = DbPropertyHelper.Int32PropertyFromRow(row, "serviceid"),
                        OfferText = DbPropertyHelper.StringDefaultPropertyFromRow(row, "offertext"),
                        OfferOrder = DbPropertyHelper.Int32PropertyFromRow(row, "offerorder"),
                        StartDate = DbPropertyHelper.DateTimeNullablePropertyFromRow(row, "startdate"),
                        EndDate = DbPropertyHelper.DateTimeNullablePropertyFromRow(row, "end_date")
                    };

                    if (activities.FirstOrDefault(x => x.Id == serviceId)?.ActivityOffers == null)
                    {
                        if (activities.Any(x => x.Id == serviceId))
                            activities.First(x => x.Id == serviceId).ActivityOffers = new List<ActivityOffers> { activityOffers };
                    }
                    else
                        activities.FirstOrDefault(x => x.Id == serviceId)?.ActivityOffers.Add(activityOffers);
                }
                catch
                {
                    //no need to handle the exception.
                }
            }
        }

        /// <summary>
        /// This method is used to get default travel information
        /// </summary>
        /// <returns></returns>
        private TravelInfo GetDefaultTravelInfo()
        {
            var travelInfo = new TravelInfo
            {
                NoOfPassengers = new Dictionary<PassengerType, int>
                {
                    {PassengerType.Adult, 1 },
                    {PassengerType.Child, 0 }
                },
                NumberOfNights = 1,
                StartDate = DateTime.Today.AddDays(1)
            };

            return travelInfo;
        }

        /// <summary>
        /// Load DownloadLinks From DataSet
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="activities"></param>
        private void LoadDownloadLinksFromDataSet(DataTableCollection dataTable, ref List<Activity> activities)
        {
            var rows = dataTable[14].Rows;
            var rowCount = rows.Count;
            for (var i = 0; i < rowCount; i++)
            {
                try
                {
                    var row = rows[i];
                    var serviceId = DbPropertyHelper.StringDefaultPropertyFromRow(row, "serviceid");
                    var downloadLinks = new DownloadLinks
                    {
                        ServiceId = DbPropertyHelper.Int32PropertyFromRow(row, "serviceid"),
                        DownloadLink = DbPropertyHelper.StringDefaultPropertyFromRow(row, "Downloadlink"),
                        DownloadText = DbPropertyHelper.StringDefaultPropertyFromRow(row, "Downloadtext"),
                        EndDate = DbPropertyHelper.DateTimeNullablePropertyFromRow(row, "Startdate"),
                        StartDate = DbPropertyHelper.DateTimeNullablePropertyFromRow(row, "Enddate"),
                        DownloadId = DbPropertyHelper.Int32PropertyFromRow(row, "Linkid")
                    };

                    if (activities.FirstOrDefault(x => x.Id == serviceId)?.DownloadLinks == null)
                    {
                        if (activities.Any(x => x.Id == serviceId))
                            activities.First(x => x.Id == serviceId).DownloadLinks = new List<DownloadLinks> { downloadLinks };
                    }
                    else
                        activities.FirstOrDefault(x => x.Id == serviceId)?.DownloadLinks.Add(downloadLinks);
                }
                catch
                {
                    //no need to handle the exception.
                }
            }
        }

        /// <summary>
        /// Create and return price model based on passed parameter
        /// </summary>
        /// <param name="baseTotalPrice"></param>
        /// <param name="totalPrice"></param>
        /// <param name="basechildPrice"></param>
        /// <param name="currencySymbol"></param>
        /// <param name="currencyIsoCode"></param>
        /// <param name="status"></param>
        /// <param name="productOption"></param>
        /// <param name="currentDate"></param>
        /// <param name="optionId"></param>
        /// <returns></returns>
        private Price CreatePriceandAvailability(decimal baseTotalPrice, decimal totalPrice, decimal basechildPrice, string currencySymbol
            , string currencyIsoCode, AvailabilityStatus status, ProductOption productOption, DateTime currentDate, int optionId)
        {
            Price price;
            var pricingUnits = new List<PricingUnit>();

            var adultPrice = new AdultPricingUnit
            {
                Price = baseTotalPrice,
                PriceType = PriceType.PerPerson,
                UnitType = UnitType.PerPerson
            };

            pricingUnits.Add(adultPrice);

            if (basechildPrice != 0)
            {
                var childPrice = new ChildPricingUnit
                {
                    Price = basechildPrice,
                    PriceType = PriceType.PerPerson,
                    UnitType = UnitType.PerPerson
                };

                pricingUnits.Add(childPrice);
            }

            var priceAndAvailability = new DefaultPriceAndAvailability
            {
                TotalPrice = totalPrice,
                AvailabilityStatus = status,
                IsSelected = false,
                PricingUnits = pricingUnits
            };

            if (productOption.Id.Equals(optionId) && productOption.BasePrice?.DatePriceAndAvailabilty != null)
            {
                price = productOption.BasePrice;
                if (currentDate.Equals(productOption.TravelInfo.StartDate) && ((status.Equals(AvailabilityStatus.AVAILABLE) || (status.Equals(AvailabilityStatus.ONREQUEST)))))
                {
                    if (price.Currency != null && !price.Currency.IsoCode.Equals(currencyIsoCode))
                    {
                        var currency = new Currency
                        {
                            Symbol = currencySymbol,
                            IsoCode = currencyIsoCode,
                            Name = currencyIsoCode
                        };
                        price.Currency = currency;
                    }
                }
                if (productOption.BasePrice.DatePriceAndAvailabilty.Count >= 7)
                {
                    var dateAndAvailabilty = new Dictionary<DateTime, PriceAndAvailability>
                    {
                        { currentDate, priceAndAvailability }
                    };
                    price.DatePriceAndAvailabilty = dateAndAvailabilty;
                }

                price.Amount = baseTotalPrice;

                if (productOption.BasePrice.DatePriceAndAvailabilty.ContainsKey(currentDate))
                {
                    productOption.BasePrice.DatePriceAndAvailabilty.Remove(currentDate);
                }
                productOption.BasePrice.DatePriceAndAvailabilty.Add(currentDate, priceAndAvailability);
            }
            else
            {
                price = new Price();
                var currency = new Currency
                {
                    Symbol = currencySymbol,
                    IsoCode = currencyIsoCode,
                    Name = currencyIsoCode
                };
                price.Currency = currency;

                if (productOption.BasePrice != null && productOption.BasePrice.DatePriceAndAvailabilty == null)
                {
                    var dateAndAvailabilty = new Dictionary<DateTime, PriceAndAvailability>
                    {
                        { currentDate, priceAndAvailability }
                    };
                    price.DatePriceAndAvailabilty = dateAndAvailabilty;
                }
            }

            return price;
        }

        private List<ProductSaleRuleByActivity> MapProductSaleRuleByActivity(IDataReader reader)
        {
            var productSaleRulesByActivity = new List<ProductSaleRuleByActivity>();
            while (reader.Read())
            {
                var productSaleRuleByActivity = new ProductSaleRuleByActivity()
                {
                    AppliedRuleId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "APPLIEDRULEID"),
                    RuleName = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "RULENAME"),
                    TravelFromDate = DbPropertyHelper.DateTimePropertyFromRow(reader, "TRAVELFROMDATE"),
                    TravelToDate = DbPropertyHelper.DateTimePropertyFromRow(reader, "TRAVELTODATE"),
                    BookingFromDate = DbPropertyHelper.DateTimePropertyFromRow(reader, "BOOKINGFROMDATE"),
                    BookingToDate = DbPropertyHelper.DateTimePropertyFromRow(reader, "BOOKINGTODATE"),
                    ServiceId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "SERVICEID"),
                    SaleRuleOfferPercent = DbPropertyHelper.DecimalPropertyFromRow(reader, "SALERULEOFFERPERCENT"),
                    ShowSale = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "SHOWSALE"),
                    SupplementRuleArriveOnMonday = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "SUPPLEMENTRULEARRIVEONMONDAY"),
                    SupplementRuleArriveOnTuesday = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "SUPPLEMENTRULEARRIEONTUESDAY"),
                    SupplementRuleArriveOnWednesday = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "SUPPLEMENTRULEARRIVEONWEDNESDAY"),
                    SupplementRuleArriveOnThursday = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "SUPPLEMENTRULEARRIVEONTHURSDAY"),
                    SupplementRuleArriveOnFriday = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "SUPPLEMENTRULEARRIVEONFRIDAY"),
                    SupplementRuleArriveOnSaturday = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "SUPPLEMENTRULEARRIVEONSATURDAY"),
                    SupplementRuleArriveOnSunday = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "SUPPLEMENTRULEARRIVEONSUNDAY"),
                    APPLIEDTOBUYRATES = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "APPLIEDTOBUYRATES"),
                    SUPPLEMENTRULEWEEKDAYONARRIVAL = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "SUPPLEMENTRULEWEEKDAYONARRIVAL")
                };
                productSaleRulesByActivity.Add(productSaleRuleByActivity);
            }

            return productSaleRulesByActivity;
        }

        private List<ProductSaleRuleByOption> MapProductSaleRuleByOption(IDataReader reader)
        {
            var productSaleRulesByOption = new List<ProductSaleRuleByOption>();
            while (reader.Read())
            {
                var productSaleRuleByOption = new ProductSaleRuleByOption
                {
                    AppliedRuleId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "APPLIEDRULEID"),
                    ServiceOptionInServiceId =
                        DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "SERVICEOPTIONINSERVICEID"),
                    PriorityOrder = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "PRIORITYORDER")
                };
                productSaleRulesByOption.Add(productSaleRuleByOption);
            }
            return productSaleRulesByOption;
        }

        private List<ProductSaleRuleByAffiliate> MapProductSaleRuleByAffiliate(IDataReader reader)
        {
            var productSaleRulesByAffiliate = new List<ProductSaleRuleByAffiliate>();
            while (reader.Read())
            {
                var productSaleRuleByAffiliate = new ProductSaleRuleByAffiliate()
                {
                    AppliedRuleId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "APPLIEDRULEID"),
                    AffiliateId =
                        DbPropertyHelper.StringDefaultPropertyFromRow(reader, "AFFILIATEID"),
                };
                productSaleRulesByAffiliate.Add(productSaleRuleByAffiliate);
            }
            return productSaleRulesByAffiliate;
        }

        private List<ProductSaleRuleByCountry> MapProductSaleRuleByCountry(IDataReader reader)
        {
            var productSaleRulesByCountry = new List<ProductSaleRuleByCountry>();
            while (reader.Read())
            {
                var productSaleRuleByCountry = new ProductSaleRuleByCountry()
                {
                    AppliedRuleId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "APPLIEDRULEID"),
                    CountryCode =
                        DbPropertyHelper.StringDefaultPropertyFromRow(reader, "COUNTRYISOCODE"),
                };
                productSaleRulesByCountry.Add(productSaleRuleByCountry);
            }
            return productSaleRulesByCountry;
        }

        private List<SupplierSaleRuleByActivity> MapSupplierSaleRuleByActivity(IDataReader reader)
        {
            var supplierSaleRulesByActivity = new List<SupplierSaleRuleByActivity>();
            while (reader.Read())
            {
                var supplierSaleRuleByActivity = new SupplierSaleRuleByActivity()
                {
                    AppliedRuleId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "APPLIEDRULEID"),
                    RuleName = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "RULENAME"),
                    TravelFromDate = DbPropertyHelper.DateTimePropertyFromRow(reader, "TRAVELFROMDATE"),
                    TravelToDate = DbPropertyHelper.DateTimePropertyFromRow(reader, "TRAVELTODATE"),
                    BookingFromDate = DbPropertyHelper.DateTimePropertyFromRow(reader, "BOOKINGFROMDATE"),
                    BookingToDate = DbPropertyHelper.DateTimePropertyFromRow(reader, "BOOKINGTODATE"),
                    ServiceId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "SERVICEID"),
                    SaleRuleOfferPercent = DbPropertyHelper.DecimalPropertyFromRow(reader, "SALERULEOFFERPERCENT"),
                    ShowSale = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "SHOWSALE")
                };
                supplierSaleRulesByActivity.Add(supplierSaleRuleByActivity);
            }

            return supplierSaleRulesByActivity;
        }

        private List<SupplierSaleRuleByOption> MapSupplierSaleRuleByOption(IDataReader reader)
        {
            var supplierSaleRulesByOption = new List<SupplierSaleRuleByOption>();
            while (reader.Read())
            {
                var supplierSaleRuleByOption = new SupplierSaleRuleByOption
                {
                    AppliedRuleId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "APPLIEDRULEID"),
                    ServiceOptionInServiceId =
                        DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "SERVICEOPTIONINSERVICEID"),
                    PriorityOrder = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "PRIORITYORDER")
                };
                supplierSaleRulesByOption.Add(supplierSaleRuleByOption);
            }
            return supplierSaleRulesByOption;
        }

        /// <summary>
        /// Load API Contract Questions
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="hbActivities"></param>
        private void LoadAPIContractQuestions(IDataReader reader, ref List<Activity> hbActivities)
        {
            while (reader.Read())
            {
                try
                {
                    var activityId = DbPropertyHelper.Int32PropertyFromRow(reader, "serviceid");
                    var listedProduct = hbActivities.Find(a => a.ID == activityId);
                    var apiContractQuestion = new APIContractQuestion
                    {
                        QuestionId = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "QuestionID"),
                        Label = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "Label"),
                        Status = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "Status"),
                        Required = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "Required"),
                        SelectFromOptions = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "selectFromOptions"),
                        Description = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "Description"),
                        Assignedservicequestionid = DbPropertyHelper.Int32PropertyFromRow(reader, "Assignedservicequestionid"),
                        Serviceid = DbPropertyHelper.Int32PropertyFromRow(reader, "Serviceid"),
                        ServiceOptionid = DbPropertyHelper.Int32PropertyFromRow(reader, "ServiceOptionid"),
                        ServiceOptionQuestionStatus = DbPropertyHelper.Int32PropertyFromRow(reader, "ServiceOptionQuestionStatus"),
                    };

                    if (listedProduct.APIContractQuestion == null)
                    {
                        listedProduct.APIContractQuestion = new List<APIContractQuestion>() { apiContractQuestion };
                    }
                    else
                        listedProduct.APIContractQuestion.Add(apiContractQuestion);
                }
                catch (Exception ex)
                {
                    //ignore
                }
            }
        }

        /// <summary>
        /// Load API Contract Answers
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="hbActivities"></param>
        private void LoadAPIContractAnswers(IDataReader reader, ref List<Activity> hbActivities)
        {
            while (reader.Read())
            {
                try
                {
                    var activityId = DbPropertyHelper.Int32PropertyFromRow(reader, "serviceid");
                    var listedProduct = hbActivities.Find(a => a.ID == activityId);
                    var aPIContractAnswers = new APIContractAnswers
                    {
                        Serviceid = DbPropertyHelper.Int32PropertyFromRow(reader, "Serviceid"),
                        ServiceOptionid = DbPropertyHelper.Int32PropertyFromRow(reader, "ServiceOptionid"),
                        QuestionId = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "QuestionID"),
                        Value = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "Value"),
                        Label = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "Label"),
                        AnswerStatus = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "AnswerStatus"),
                    };

                    if (listedProduct.APIContractAnswer == null)
                    {
                        listedProduct.APIContractAnswer = new List<APIContractAnswers>() { aPIContractAnswers };
                    }
                    else
                        listedProduct.APIContractAnswer.Add(aPIContractAnswers);
                }
                catch (Exception ex)
                {
                    //ignore
                }
            }
        }

        private string FilterCancellationPolicy(string cancellationPolicies)
        {
            var cancellationPolicy = string.Empty;
            try
            {
                if (!string.IsNullOrWhiteSpace(cancellationPolicies) && cancellationPolicies.Contains("#"))
                {
                    var split = cancellationPolicies.Split('#');
                    foreach (var c in split)
                    {
                        if (!string.IsNullOrWhiteSpace(c) && !cancellationPolicy.Contains(c))
                        {
                            cancellationPolicy += $"{c}\r\n";
                        }
                    }
                    cancellationPolicies = cancellationPolicy;
                }
            }
            catch
            {
            }
            return cancellationPolicies?.Trim();
        }

        #endregion Private Methods

        #region [Delta Activity]

        /// <summary>
        /// Load Review information from reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public Review GetReviewMapping(IDataReader reader)
        {
            var review = new Review
            {
                Title = DbPropertyHelper.StringPropertyFromRow(reader, "Title"),
                Rating = DbPropertyHelper.StringPropertyFromRow(reader, "Rating"),
                Text = DbPropertyHelper.StringPropertyFromRow(reader, "ReviewComments"),
                UserName = DbPropertyHelper.StringPropertyFromRow(reader, "Firstname"),
                Country = DbPropertyHelper.StringPropertyFromRow(reader, "Country"),
                SubmittedDate = DbPropertyHelper.DateTimePropertyFromRow(reader, "ReviewDate"),
                IsFeefo = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "IsFeefoReview"),
                ServiceId = DbPropertyHelper.StringPropertyFromRow(reader, "ServiceId")
            };
            return review;
        }

        /// <summary>
        /// Load Passenger information from reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public Entities.Booking.PassengerInfo GetPassengerMapping(IDataReader reader)
        {
            var passengerInfo = new Entities.Booking.PassengerInfo
            {
                ActivityId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "ServiceId"),
                FromAge = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "FromAge"),
                IndependablePax = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "IndependablePax"),
                MaxSize = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "MaxSize"),
                MinSize = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "MinSize"),
                PassengerTypeId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "PassengerTypeId"),
                PaxDesc = DbPropertyHelper.StringPropertyFromRow(reader, "PaxName"),
                ToAge = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "ToAge"),
                Label = DbPropertyHelper.StringPropertyFromRow(reader, "Label"),
                MeasurementDesc = DbPropertyHelper.StringPropertyFromRow(reader, "MeasurementDesc")
            };
            return passengerInfo;
        }

        /// <summary>
        /// /// Load ActivityIds information from reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public ActivityIds GetActivityIdsMapping(IDataReader reader)
        {
            var activityIds = new ActivityIds
            {
                Serviceid = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "ServiceId"),
                LanguageCode = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "LanguageCode"),
                ServiceStatusID = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "ServiceStatusID")
            };
            return activityIds;
        }

        /// <summary>
        /// Load price information from reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public ActivityMinPrice GetDeltaActivityPriceMapping(IDataReader reader)
        {
            var activityIds = new ActivityMinPrice
            {
                Serviceid = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "ServiceId"),
                AffiliateID = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "AffiliateID"),
                BasePrice = DbPropertyHelper.DecimalDefaultNullablePropertyFromRow(reader, "BasePrice"),
                Offer_Percent = DbPropertyHelper.DecimalDefaultNullablePropertyFromRow(reader, "Offer_Percent"),
                SellPrice = DbPropertyHelper.DecimalDefaultNullablePropertyFromRow(reader, "SellPrice")
            };
            return activityIds;
        }

        /// <summary>
        ///  Load available information from reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public ActivityAvailableDays GetDeltaActivityAvailableMapping(IDataReader reader)
        {
            var activityAvailableDays = new ActivityAvailableDays
            {
                Serviceid = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "ServiceId"),
                AvailableDays = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "AvailableDays")
            };
            return activityAvailableDays;
        }

        #endregion [Delta Activity]
    }
}