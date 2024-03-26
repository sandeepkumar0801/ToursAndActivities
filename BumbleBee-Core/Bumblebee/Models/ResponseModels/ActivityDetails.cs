using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Review;
using System;
using System.Collections.Generic;

namespace WebAPI.Models.ResponseModels
{
    public class ActivityDetails
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<string> ReasonToBook { get; set; }
        public string ScheduleOperates { get; set; }
        public string DurationString { get; set; }
        public string Schedule { get; set; }
        public string HotelPickUpLocation { get; set; }
        public string MeetingPointCoordinate { get; set; }
        public string ScheduleReturnDetails { get; set; }
        public List<ProductImage> Images { get; set; }
        public string Introduction { get; set; }
        public List<ActivityItinerary> Itineraries { get; set; }
        public string Inclusions { get; set; }
        public string Exclusions { get; set; }
        public string PleaseNote { get; set; }
        public string AdditionalInfo { get; set; }
        public List<Review> Reviews { get; set; }
        public int TotalReviews { get; set; }
        public string CancellationPolicy { get; set; }
        //public APIType ApiType { get; set; }
        public decimal BaseMinPrice { get; set; }
        public decimal GateBaseMinPrice { get; set; }
        public List<Isango.Entities.Booking.PassengerInfo> PassengerInfo { get; set; }
        public List<ComponentService> ComponentServices { get; set; }
        public string CurrencyIsoCode { get; set; }
        public int LineOfBusinessId { get; set; }
        public string AttractionsCovered { get; set; }
        public string ToDoOnArrival { get; set; }
        public string WhyDoThis { get; set; }
        public DateTime? LiveOnDate { get; set; }
        public int DurationDay { get; set; }
        public TimeSpan DurationTime { get; set; }
        public string DurationAdditionText { get; set; }
        public List<ContractQuestions> ContractQuestions { get; set; }
        public int MinNoOfPassengers { get; set; }
        public int MaxNoOfPassengers { get; set; }
        public List<DownloadLinks> DownloadLinks { get; set; }
        public bool IsHideGatePrice { get; set; }



    }



    public class ComponentService
	{
		public int Id { get; set; }
		public string Name { get; set; }
	}
}