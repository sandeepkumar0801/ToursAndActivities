using System;
using System.Collections.Generic;

namespace Isango.Entities.MyIsango
{
    public class MyBookingSummary
    {
        public string BookingRefenceNumber { get; set; }

        public int BookingId { get; set; }

        public DateTime BookingDate { get; set; }

        public string BookingAmountCurrency { get; set; }

        public List<MyBookedProduct> BookedProducts { get; set; }
        public List<MyPaxPriceInfo> PaxPriceInfo { get; set; }

        public string GetBookingDate { get; set; }
    }

    public class MyBookingSummarySitecoreViewModel
    {
        public int BookingId { get; set; }
        public string BookingRefNo { get; set; }
        public DateTime BookingDate { get; set; }
        public string CurrencyShortSymbol { get; set; }
        public string AffiliateName { get; set; }
        public List<MyBookedProductSitecoreViewModel> BookingDetail { get; set; }
    }
}