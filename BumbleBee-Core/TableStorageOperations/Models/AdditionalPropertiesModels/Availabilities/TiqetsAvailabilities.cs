using System.Collections.Generic;

namespace TableStorageOperations.Models.AdditionalPropertiesModels.Availabilities
{
    public class TiqetsAvailabilities : BaseAvailabilitiesEntity
    {
        public int FactSheetId { get; set; } //Supplier Id
        public string RequiresVisitorsDetails { get; set; }
        //public string RequiresVisitorsDetailsWithVariant { get; set; }
    }
}