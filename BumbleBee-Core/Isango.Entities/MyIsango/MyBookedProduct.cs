using System;
using System.Collections.Generic;

namespace Isango.Entities.MyIsango
{
    public class MyBookedProduct
    {
        public string BookedProductName { get; set; }

        public DateTime TravelDate { get; set; }

        public Decimal BookingAmountPaid { get; set; }

        public string BookingStatus { get; set; }

        public int NoOfAdults { get; set; }

        public int NoOfChildren { get; set; }

        public string GetTravelDate { get; set; }

        public string TicketDetail { get; set; }

        public Decimal AmountBeforeDiscount { get; set; }

        public int ServiceId { get; set; }

        public int BookedOptionId { get; set; }
        public bool IsReceipt { get; set; }
    }

    public class MyBookedProductSitecoreViewModel
    {
        //public string BookedProductName { get; set; }
        //public DateTime TravelDate { get; set; }
        //public Decimal BookingAmountPaid { get; set; }
        //public string BookingStatus { get; set; }
        //public int NoOfAdults { get; set; }
        //public int NoOfChildren { get; set; }

        //public string GetTravelDate { get; set; }

        public int BookingId { get; set; }
        public string ServiceName { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime TravelDate { get; set; }
        public string CurrencyShortSymbol { get; set; }
        public double SellAmount { get; set; }
        public int OptionStatusId { get; set; }
        public string OptionStatusName { get; set; }

        //public int AdultCount { get; set; }
        //public int ChildCount { get; set; }
        public List<MyPaxPriceInfoViewModel> PaxInfo { get; set; }

        public double AmountBeforeDiscount { get; set; }
        public string TicketDetail { get; set; }

        public int ServiceId { get; set; }

        public int BookedOptionId { get; set; }
        public bool IsReceipt { get; set; }
    }

    public class MyPaxPriceInfoViewModel
    {
        public decimal PassengerSellAmount { get; set; }
        public decimal PassengerOriginalSellAmount { get; set; }
        public string PassengerType { get; set; }

        public int PassengerTypeId { get; set; }
        public int PassengerCount { get; set; }
    }
}