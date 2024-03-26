using Isango.Entities.PrioHub;
using System.Collections.Generic;

namespace TableStorageOperations.Models.AdditionalPropertiesModels.Availabilities
{
    public class PrioHubAvailabilities : BaseAvailabilitiesEntity
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string Vacancies { get; set; }
        public int PrioTicketClass { get; set; }
        public string PickupPoints { get; set; }
        public string PickupPointsDetails { get; set; }
        public int PrioHubProductTypeStatus { get; set; }
        public string PrioHubProductPaxMapping { get; set; }
        public bool? PrioHubProductGroupCode { get; set; } //QrCode single or multiple
        public string PrioHubAvailabilityId { get; set; }

        public string ProductCombiDetails { get; set; }

        public string PrioHubComboSubProduct { get; set; }

        public string PrioHubClusterProduct { get; set; }

        public string PrioHubDistributerId { get; set; }

    }
}