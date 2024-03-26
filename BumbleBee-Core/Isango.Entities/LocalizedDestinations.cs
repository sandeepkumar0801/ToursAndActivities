using System.Collections.Generic;

namespace Isango.Entities
{
    public class LocalizedDestinations
    {
        public string Language { get; set; }
        public List<Destination> Destinations { get; set; }
    }
}