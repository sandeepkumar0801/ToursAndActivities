using Isango.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAdapters.GlobalTixV3.GlobalTix.Entities
{
    public class BookInputContextV3 : InputContext
    {
        public List<SelectedProduct> SelectedProducts { get; set; }
        public string BookingReferenceNumber { get; set; } 

        public bool isNonThailandProduct { get; set; }
    }
}
