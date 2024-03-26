using System;
using System.Data;

namespace Isango.Entities.Mailer.Voucher
{
    public class OthersChildDetail
    {
        //Child detail
        internal string BookedOptionId { get; set; }

        internal string BookedChildRateQuantity { get; set; }
        internal string BookedChildRateAge { get; set; }
        internal string BookedChildRateTotalSellAmount { get; set; }

        internal OthersChildDetail(IDataReader result)
        {
            BookedOptionId = Convert.ToString(result["bookedoptionid"]).Trim();
            BookedChildRateQuantity = Convert.ToString(result["bookedchildratequantity"]).Trim();
            BookedChildRateAge = Convert.ToString(result["bookedchildrateage"]).Trim();
            BookedChildRateTotalSellAmount = Convert.ToString(result["bookedchildratetotalsellamount"]).Trim();
        }
    }
}