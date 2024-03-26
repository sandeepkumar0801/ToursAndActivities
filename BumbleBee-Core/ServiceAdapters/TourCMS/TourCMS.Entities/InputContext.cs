using Isango.Entities;
using Isango.Entities.Booking;
using System;
using System.Collections.Generic;

namespace ServiceAdapters.TourCMS.TourCMS.Entities
{
    public class InputContext
    {
        public string ReservationReference { get; set; }
        public bool TicketPerPassenger { get; set; }

        public SelectedProduct SelectedProducts { get; set; }
        public string BookingReference { get; set; }

        public string LanguageCode { get; set; }
        public string VoucherEmailAddress { get; set; }

        public string VoucherPhoneNumber { get; set; }

        public int TotalCustomers { get; set; }
    }

    
}