using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Isango.Entities.Ventrata
{
    public class VentrataAvailabilityCriteria : Criteria
    {
        public string ProductId { get; set; }
        //public Dictionary<string, string> SupplierOptionCodesAndAvaillIdFetchedString  { get; set; }
        public Dictionary<string, List<string>> SupplierOptionCodesAndProductIdVsApiOptionIds { get; set; }
        public string SupplierBearerToken { get; set; }
        //public List<PassengerUnits> PassengerDetails { get; set; }

        public string VentrataBaseURL { get; set; }

        public bool IsSupplementOffer { get; set; }

        public List<VentrataPaxMapping> VentrataPaxMappings { get; set; }
    }

    public class PassengerUnits
    {
        public string Id { get; set; }
        public int Quantity { get; set; }
    }
}
