using System;
using System.Collections.Generic;

namespace Isango.Entities.PrioHub
{
    public class NewPrioProductRoutesLocations
    {
        public string RouteId { get; set; }
        public string RouteLocationId { get; set; }
        public bool RouteLocationActive { get; set; }
        public string RouteLocationName { get; set; }
        public bool RouteLocationStopOver { get; set; }

    }
}