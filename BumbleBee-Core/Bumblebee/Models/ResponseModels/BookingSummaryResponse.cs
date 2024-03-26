using Isango.Entities.MyIsango;
using System;
using System.Collections.Generic;

namespace WebAPI.Models.ResponseModels
{
	public class BookingSummaryResponse
	{
		public string BookingRefenceNumber { get; set; }

		public int BookingId { get; set; }

		public DateTime BookingDate { get; set; }

		public string BookingAmountCurrency { get; set; }

		public List<MyBookedProduct> BookedProducts { get; set; }

		public string PickUpTravelDate { get; set; }
		public string Type { get; set; }
	}
}