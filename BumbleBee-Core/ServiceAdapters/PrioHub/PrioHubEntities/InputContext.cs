using Isango.Entities;
using Isango.Entities.PrioHub;
using System;
using System.Collections.Generic;

namespace ServiceAdapters.PrioHub.PrioHub.Entities
{
    public class InputContext
    {
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
        public string PickupPoints { get; set; }
        public string ReservationReference { get; set; }
        public int PrioTicketClass { get; set; }

        public List<PrioHubProductPaxMapping> PrioHubProductPaxMapping { get; set; }
        public int PrioHubProductTypeStatus { get; set; }
        public bool? PrioHubProductGroupCode { get; set; } //QrCode single or multiple
        public string PrioHubAvailabilityId { get; set; }

        public string BookingFirstName { get; set; }

        public string BookingLastName { get; set; }

        public string BookingOptionType { get; set; }

        public List<ProductCombiDetails> ProductCombiDetails { get; set; }

        public List<PrioHubComboSubProduct> PrioHubComboSubProduct { get; set; }

        public ProductCluster PrioHubClusterProduct { get; set; }

        public string PrioHubDistributerId { get; set; }

        public List<Customer> Customers { get; set; }

        public DateTime TourDate { get; set; }

    }

  
}