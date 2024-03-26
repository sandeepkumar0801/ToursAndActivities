using Isango.Entities.GlobalTixV3;
using System.Collections.Generic;

namespace TableStorageOperations.Models.AdditionalPropertiesModels.Availabilities
{
    public class GlobalTixV3Availabilities : BaseAvailabilitiesEntity
    {
        public string ContractQuestionsForGlobalTix3 { get; set; }

        public string RateKey { get; set; }
        
        public int TourDepartureId { get; set; }

        public string TicketTypeIds { get; set; }
        public string ContractQuestions { get; set; }

    }
}