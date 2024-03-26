using System;
using System.Data;

namespace Isango.Entities.Booking.BookingDetailAPI
{
    public class ChildDetailAPI
    {
        //Child detail
        public string BookedOptionId { get; set; }

        public string BookedChildRateQuantity { get; set; }

        public string BookedChildRateAge { get; set; }

        public string BookedChildRateTotalSellAmount { get; set; }

        public ChildDetailAPI(IDataReader result)
        {
            BookedOptionId = Convert.ToString(result["bookedoptionid"]).Trim();
            BookedChildRateQuantity = Convert.ToString(result["bookedchildratequantity"]).Trim();
            BookedChildRateAge = Convert.ToString(result["bookedchildrateage"]).Trim();
            BookedChildRateTotalSellAmount = Convert.ToString(result["bookedchildratetotalsellamount"]).Trim();
        }
    }
}