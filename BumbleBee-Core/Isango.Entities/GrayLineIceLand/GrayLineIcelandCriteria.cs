using Isango.Entities.Enums;
using System.Collections.Generic;

namespace Isango.Entities.GrayLineIceLand
{
    public class GrayLineIcelandCriteria : Criteria
    {
        public Dictionary<PassengerType, int> PaxAgeGroupIds { get; set; }
        public string ActivityCode { get; set; }
        public string Language { get; set; }
    }
}