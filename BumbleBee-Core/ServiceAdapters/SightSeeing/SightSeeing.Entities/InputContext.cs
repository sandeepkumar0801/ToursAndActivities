using Isango.Entities;
using System.Collections.Generic;

namespace ServiceAdapters.SightSeeing.SightSeeing.Entities
{
    public class InputContext
    {
        public List<SelectedProduct> SelectedProducts { get; set; }

        public string BookingReferenceNumber { get; set; }
    }
}