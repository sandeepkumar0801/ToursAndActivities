using System.Collections.Generic;

namespace Isango.Entities
{
    public class ActivityBookingCriteria
    {
        public Booking.Booking Booking { get; set; }
        public string Authentication { get; set; }
        public string Token { get; set; }
        public List<SelectedProduct> SelectedProducts;
    }
}