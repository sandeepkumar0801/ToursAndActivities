using System;

namespace TableStorageOperations.Models.AdditionalPropertiesModels.Availabilities
{
    public class RezdyAvailabilities : BaseAvailabilitiesEntity
    {
        public int Seats { get; set; }
        public int SeatsAvailable { get; set; }
        public DateTime StartTimeLocal { get; set; }
        public DateTime EndTimeLocal { get; set; }
        public int PickUpId { get; set; }
    }

}