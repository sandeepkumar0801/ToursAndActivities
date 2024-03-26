using System;
using System.Collections.Generic;

namespace Isango.Entities.PrioHub
{
    public class NewPrioProductRoutes
    {
        public string RouteId { get; set; }
        public bool RouteActive { get; set; }
        public string RouteName { get; set; }
        public string RouteColor { get; set; }
        public int RouteDuration { get; set; }
        public string RouteType { get; set; }
        public string RouteStartTime { get; set; }
        public string RouteEndTime { get; set; }
        public int RouteFrequency { get; set; }
        public string RouteAudioLanguages { get; set; }
        public string RouteLiveLanguages { get; set; }
        public string RouteProducts { get; set; }
       
    }
}