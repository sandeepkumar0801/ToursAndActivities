using System;
using System.Collections.Generic;

namespace WebAPI.Models.ResponseModels
{
    public class BookingOptionsDetailsResponse
    {
        public List<BookingOptionDetail> BookingOptionsDetails { get; set; }
    }

    public class BookingOptionDetail
    {
        public int OptionId { get; set; }
        public string OptionName { get; set; }
        public string OptionStatus { get; set; }
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public DateTime TravelDate { get; set; }
        public string SellingPrice { get; set; }
    }
}