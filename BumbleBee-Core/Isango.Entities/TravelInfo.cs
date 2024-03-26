using Isango.Entities.Enums;
using System;
using System.Collections.Generic;

namespace Isango.Entities
{
    public class TravelInfo
    {
        public DateTime StartDate { get; set; }
        public int NumberOfNights { get; set; }
        public Dictionary<PassengerType, int> NoOfPassengers { get; set; }
        public Dictionary<PassengerType, int> Ages { get; set; }
    }
}