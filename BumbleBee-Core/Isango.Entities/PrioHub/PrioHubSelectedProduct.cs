using Isango.Entities.PrioHub;
using System.Collections.Generic;

namespace Isango.Entities.PrioHub
{
    public class PrioHubSelectedProduct : SelectedProduct
    {
        public PrioHubAPITicket PrioHubApiConfirmedBooking { get; set; }

        public string PrioReservationReference { get; set; }

        public string PrioDistributorReference { get; set; }

        public string PrioBookingStatus { get; set; }

        public int PrioTicketClass { get; set; }

        public string PickupPoints { get; set; }

        public List<PickUpPointForPrioHub> PickupPointDetails { get; set; }

        public string ReservationExpiry { get; set; }

        public string Code { get; set; }

        public string Destination { get; set; }

        public int FactsheetId { get; set; }

        public List<ProductCombiDetails> ProductCombiDetails { get; set; }
        public List<PrioHubProductPaxMapping> PrioHubProductPaxMapping { get; set; }
        public int PrioHubProductTypeStatus { get; set; }
        public bool? PrioHubProductGroupCode { get; set; } //QrCode single or multiple
        public string PrioHubAvailabilityId { get; set; }
        public string PrioHubReservationStatus { get; set; }
        public List<PickUpPointForPrioHub> PickUpPointForPrioHub { get; set; }
        public List<PrioHubComboSubProduct> PrioHubComboSubProduct { get; set; }

        public ProductCluster Cluster { get; set; }

        public string PrioHubDistributerId { get; set; }
    }
}