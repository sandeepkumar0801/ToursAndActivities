using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Isango.Entities.Rezdy
{
    public class RezdyPickUpLocation
    {
        public int Id { get; set; }
        public string LocationName { get; set; }
        public string Address { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public int MinutesPrior { get; set; }
        public string AdditionalInstructions { get; set; }
    }
}
