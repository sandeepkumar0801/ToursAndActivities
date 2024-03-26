using ServiceAdapters.GlobalTixV3.GlobalTix.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAdapters.GlobalTixV3.GlobalTix.Entities
{
    internal class CancelInputContext : InputContext
    {
        public string BookingReference { get; set; }
        public string BookingNumber { get; set; }
        public bool isNonThailandProduct { get; set; }

    }
}
