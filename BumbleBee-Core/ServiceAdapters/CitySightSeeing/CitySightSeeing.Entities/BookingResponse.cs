using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAdapters.CitySightSeeing.CitySightSeeing.Entities
{
    public class BookingTicket
    {
        public bool Mobile { get; set; }
        public string Reference { get; set; }
        public string Barcode { get; set; }
    }

    public class CssBookingResponseResult
    {
        public object Booking { get; set; }
        public List<BookingTicket> Tickets { get; set; }
    }

    public class CssBookingStatus
    {
        public DateTime Instant { get; set; }
        public string Description { get; set; }
    }

}
