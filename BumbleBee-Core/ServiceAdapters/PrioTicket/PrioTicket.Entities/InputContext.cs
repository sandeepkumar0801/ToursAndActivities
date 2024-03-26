using System;
using System.Collections.Generic;

namespace ServiceAdapters.PrioTicket.PrioTicket.Entities
{
    public class InputContext
    {
        // For PrioTicket

        public string ActivityId { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public MethodType MethodType { get; set; }

        public int TicketId { get; set; }

        public List<string> TicketType { get; set; }

        public List<int> Count { get; set; }

        public string BookingName { get; set; }

        public string BookingEmail { get; set; }

        public string Street { get; set; }

        public string PostalCode { get; set; }

        public string City { get; set; }

        public string PhoneNumber { get; set; }

        public List<string> Notes { get; set; }

        public string Language { get; set; }

        public string DistributorReference { get; set; }

        public DateTime FromDateTime { get; set; }

        public string CheckInDate { get; set; }

        public string CheckOutDate { get; set; }

        public string BookingReference { get; set; }
        public string PickupPointId { get; set; }
        public string ReservationReference { get; set; }
        public int PrioTicketClass { get; set; }
    }
}