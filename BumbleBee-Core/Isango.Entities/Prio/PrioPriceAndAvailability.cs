using System;

namespace Isango.Entities.Prio
{
    public class PrioPriceAndAvailability : PriceAndAvailability
    {
        public string Name { get; set; }
        public string FromDateTime { get; set; }
        public string ToDateTime { get; set; }
        public string Vacancies { get; set; }
    }
}