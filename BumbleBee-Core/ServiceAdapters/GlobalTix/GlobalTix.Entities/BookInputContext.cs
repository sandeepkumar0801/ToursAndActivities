using Isango.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAdapters.GlobalTix.GlobalTix.Entities
{
    public class BookInputContext : InputContext
    {
        public List<SelectedProduct> SelectedProducts { get; set; }
        public string BookingReferenceNumber { get; set; } 
    }
}
