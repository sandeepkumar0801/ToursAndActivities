using System;
using System.Collections.Generic;

namespace ServiceAdapters.BigBus.BigBus.Entities
{
    public class InputContext
    {
        public string ReservationReference { get; set; }
        public bool TicketPerPassenger { get; set; }

        public List<Product> Products { get; set; }

        public string BookingReference { get; set; }
    }

    public class Product
    {
        public string ProductId { get; set; }
        public Dictionary<string, int> NoOfPassengers { get; set; }
        public DateTime DateOfTravel { get; set; }
    }
}