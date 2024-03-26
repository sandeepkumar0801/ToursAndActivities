using System.Collections.Generic;

namespace Isango.Entities.Redeam
{
    public class RatesWrapper
    {
        public List<RateData> Rates { get; set; }
        public List<PriceData> Prices { get; set; }
        public List<PassengerTypeData> TravelerTypes { get; set; }
    }
}