using Isango.Entities.Enums;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Isango.Entities.Activities
{
    [BsonIgnoreExtraElements]
    public class Activity : ActivityLite
    {
        public Activity()
        {
            this.Errors = new List<Error>();
        }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        public string LanguageCode { get; set; }
        public int BookingWindow { get; set; }
        public string AdditionalInfo { get; set; }
        public string AlertNote { get; set; }
        public string ChildPolicy { get; set; }
        public string DoDont { get; set; }
        public string Exclusions { get; set; }
        public string HotelPickUpLocation { get; set; }
        public string Inclusions { get; set; }
        public string PleaseNote { get; set; }
        public string Schedule { get; set; }
        public string ScheduleLocation { get; set; }
        public string ScheduleUnavailableDates { get; set; }
        public List<double> Duration { get; set; }
        public List<int> Time { get; set; }
        public bool IsPaxDetailRequired { get; set; }
        public bool IsReceipt { get; set; }
        public string ScheduleReturnDetails { get; set; }
        public int TotalReviews { get; set; }
        public bool IsServiceLevelPickUp { get; set; }
        public List<ActivityItinerary> Itineraries { get; set; }
        public List<Booking.PassengerInfo> PassengerInfo { get; set; }
        public string CurrencyIsoCode { get; set; }

        #region HotelBeds Activity Properties

        public string YouTubeLink { get; set; }

        #endregion HotelBeds Activity Properties

        public string MeetingPointCoordinate { get; set; }
        public List<Review.Review> Reviews { get; set; }
        public List<ActivityCategoryType> CategoryTypes { get; set; }

        #region Fareharbor

        public List<CustomerPrototype> CustomerPrototypes { get; set; }

        #endregion Fareharbor

        public string CanonicalURL { get; set; }
        public PickUpDropOffOptionType PickUpOption { get; set; }
        public List<ActivityPickupLocations> PickupLocations { get; set; }
        public string WhereYouStay { get; set; }
        public string WhyYouLove { get; set; }

        public List<DownloadLinks> DownloadLinks { get; set; }

        public List<ActivityOffers> ActivityOffers { get; set; }

        public bool? IsNoIndex { get; set; }
        public bool? IsFollow { get; set; }
        public bool? IsHighDefinationImages { get; set; }

        public bool IsGoogleFeed { get; set; }

        public int LineOfBusinessId { get; set; }
        public string AttractionsCovered { get; set; }
        public string ToDoOnArrival { get; set; }
        public string WhyDoThis { get; set; }
        public DateTime? LiveOnDate { get; set; }
        public int DurationDay { get; set; }
        public TimeSpan DurationTime { get; set; }
        public string DurationAdditionText { get; set; }
        public List<Rating> Ratings { get; set; }

        public DateTime? TourLaunchDate { get; set; }

        public bool IsTimeBase { get; set; }

        public string CancellationSummary { get; set; }

        public string DurationSummary { get; set; }
        public string StartTimeSummary { get; set; }

        public int? SupplierID { get; set; }
        public List<APIContractQuestion> APIContractQuestion { get; set; }
        public List<APIContractAnswers> APIContractAnswer { get; set; }

        public string CountryCode { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public bool isperpersonprice { get; set; }

        public string UnitText { get; set; }

        public List<Error> Errors { get; set; }

        public bool? IsPrivateTour { get; set; }
        public int MinNoOfPax { get; set; }
        public int MaxNoOfPax { get; set; }
        public bool IsBundle { get; set; }
        public string SupplierName { get; set; }
        public bool ISSHOWSUPPLIERVOUCHER { get; set; }
        public bool IsHideGatePrice { get; set; }
        public int? ServiceStatusID { get; set; }
        public bool AdyenStringentAccount { get; set; }
    }
}