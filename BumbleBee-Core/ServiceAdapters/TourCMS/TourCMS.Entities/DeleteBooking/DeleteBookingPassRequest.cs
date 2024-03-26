using Newtonsoft.Json;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ServiceAdapters.TourCMS.TourCMS.Entities.DeleteBookingRequest
{
    
    public class DeleteBookingPassRequest
    {
        public int BookingId { get; set; }
        public string PrefixServiceCode { get; set; }

    }
}