using Isango.Entities;
using Isango.Entities.Enums;
using System.Collections.Generic;

namespace WebAPI.Models.ResponseModels
{
	public class SearchDetails
	{
		public int ActivityId { get; set; }
		public string Name { get; set; }
		public string ShortIntroduction { get; set; }
		public string ScheduleOperates { get; set; }
		public double OverAllRating { get; set; }
		public DayBadge DayBadge { get; set; }
		public ProductType ProductType { get; set; }
		public ActivityType ActivityType { get; set; }
		public List<ProductImage> Images { get; set; }
		public decimal BaseMinPrice { get; set; }
		public decimal GateBaseMinPrice { get; set; }
		public decimal OfferPercentage { get; set; }
		public string ActualServiceUrl { get; set; }
		public string Destination { get; set; }
		public List<int> Themes { get; set; }
		public List<string> BulletPoints { get; set; }
		public string Length { get; set; }
		public string Coordinates { get; set; }
		public List<Badge> Badges { get; set; }
		public int TotalReviews { get; set; }
		public string CurrencyIsoCode { get; set; }

	}
}