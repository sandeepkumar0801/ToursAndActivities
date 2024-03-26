using System;

namespace Isango.Entities.TheatreShow
{
    public class Seat
    {
        public string Row { get; set; }
        public string Key { get; set; }
        public int From { get; set; }
        public int To { get; set; }
        public string ShoppingCartId { get; set; }
        public int ShoppingCartOperationId { get; set; }
        public int CountOfSeats { get; set; }
        public decimal Amount { get; set; }
        public string CurrencySymbol { get; set; }
        public string SeatModality { get; set; }
        public decimal FacePrice { get; set; }
        public DateTime AvailabilityDate { get; set; }
        public decimal BaseAmount { get; set; }
    }
}