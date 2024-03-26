using System.Collections.Generic;

namespace ServiceAdapters.HotelBeds.HotelBeds.Entities.Tickets
{
    public class GuestList : EntityBase
    {
        public List<Customer> CustomerList { get; set; }
    }
}