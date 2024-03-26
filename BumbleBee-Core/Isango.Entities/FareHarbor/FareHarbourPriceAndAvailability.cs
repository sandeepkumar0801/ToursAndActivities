using Isango.Entities.Enums;
using System;
using System.Collections.Generic;

namespace Isango.Entities.FareHarbor
{
    public class FareHarborPriceAndAvailability : PriceAndAvailability
    {
        public Dictionary<PassengerType, Int64> CustomerTypePriceIds { get; set; }
    }
}