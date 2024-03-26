using System.Collections.Generic;

namespace ServiceAdapters.HotelBeds.HotelBeds.Entities
{
    public class HotelRoom : EntityBase
    {
        public Board Board { get; set; }
        public RoomType RoomType { get; set; }
        public string Shrui { get; set; }
        public decimal Amount { get; set; }
        public int AvailCount { get; set; }
        public string Status { get; set; }
        public CancellationPolicy CancellationPolicy { get; set; }
        public List<Discount> DiscountList { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal NetPrice { get; set; }
        public decimal Commission { get; set; }
    }
}