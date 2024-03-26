using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Enums;
using Isango.Entities.Region;
using System;
using System.Collections.Generic;

namespace WebAPI.Models.ResponseModels.DeltaActivity
{
    public class ActivityResponse
    {
        public List<Badge> Badges { get; set; }
        public bool OnSale { get; set; }
        public string ScheduleOperates { get; set; }
        public string ShortName { get; set; }
        public int Priority { get; set; }
        public decimal SellMinPrice { get; set; }
        public decimal BaseMinPrice { get; set; }
        public string CurrencyCode { get; set; }
        public int BookingWindow { get; set; }
        public string AdditionalInfo { get; set; }
        public string AlertNote { get; set; }
        public string CancellationPolicy { get; set; }
        public string ChildPolicy { get; set; }
        public string DoDont { get; set; }
        public string Exclusions { get; set; }
        public string HotelPickUpLocation { get; set; }
        public string Inclusions { get; set; }

        public List<ActivityItinerary> Itineraries { get; set; }
        public string PleaseNote { get; set; }

        public string Schedule { get; set; }
        public string ScheduleLocation { get; set; }
        public string ScheduleUnavailableDates { get; set; }
        public List<double> Duration { get; set; }
        public string CoOrdinates { get; set; }
        public List<string> ReasonToBook { get; set; }
        public bool IsPaxDetailRequired { get; set; }
        public bool IsReceipt { get; set; }
        public string ActualServiceURL { get; set; }
        public string ScheduleReturnDetails { get; set; }
        public List<int> CategoryIDs { get; set; }
        public ActivityType ActivityType { get; set; }
        public List<ReviewResponse> Reviews { get; set; }
        public double OverAllRating { get; set; }
        public int TotalReviews { get; set; }
        public Boolean IsServiceLevelPickUp { get; set; }
        public Dictionary<int, int> PriorityWiseCategory { get; set; }

        #region HotelBeds Activity Properties

        public string Code { get; set; }
        public int FactsheetID { get; set; }
        public string YouTubeLink { get; set; }

        #endregion HotelBeds Activity Properties

        public List<ActivityCategoryType> CategoryTypes { get; set; }
        public APIType APIType { get; set; }
        public string MeetingPointCoordinate { get; set; }

        #region Fareharbor

        public List<CustomerPrototype> CustomerPrototypes { get; set; }

        #endregion Fareharbor

        public ProductType ProductType { get; set; }
        public int ID { get; set; }
        public string Name { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string Introduction { get; set; }
        public string ShortIntroduction { get; set; }

        public List<ProductImage> Images { get; set; }
        public string Title { get; set; }

        public decimal OfferPercentage { get; set; }
        public bool IsPackage { get; set; }
        public String Availability { get; set; }
        public List<PassengerInfoResponse> PassengerInfo { get; set; }
        public string CanonicalURL { get; set; }    // Need this Value
        public List<BundleResponse> Bundle { get; set; }

        public String LanguageCode { get; set; }
        public bool Status { get; set; }

        public List<ProductOption> AllOptions { get; set; }

        public List<Region> Regions { get; set; }
        public Margin Margin { get; set; }
        public string DurationString { get; set; }
        public PickUpDropOffOptionType PickUpOption { get; set; }

        public string WhereYouStay { get; set; }
        public string WhyYouLove { get; set; }

        public List<DownloadLinks> DownloadLinks { get; set; }

        public List<ActivityOffers> ActivityOffers { get; set; }

        public bool? IsNoIndex { get; set; }
        public bool? IsFollow { get; set; }
        public bool? IsHighDefinationImages { get; set; }
        public bool IsGoogleFeed { get; set; }

        public List<Rating> Ratings { get; set; }

        public List<TimesOfDayResponse> TimesOfDaysOptionWise { get; set; }

        public int LineOfBusinessId { get; set; }
        public string AttractionsCovered { get; set; }
        public string ToDoOnArrival { get; set; }
        public string WhyDoThis { get; set; }
        public DateTime? LiveOnDate { get; set; }

        public int DurationDay { get; set; }
        public TimeSpan DurationTime { get; set; }
        public string DurationAdditionText { get; set; }

        public DateTime? TourLaunchDate { get; set; }

        public bool IsTimeBase { get; set; }

        public string CancellationSummary { get; set; }

        public string StartTimeSummary { get; set; }
        public string DurationSummary { get; set; }

        public int? SupplierID { get; set; }
    }
}