using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Isango.Entities.Booking
{
    public class CssCancellation
    {
        public string bookedoptionid { get; set; }
        public string CssReferenceNumber { get; set; }
        public bool IsangoCancelStatus { get; set; }

        public string Barcode { get; set; }
        public int SupplierId { get; set; }
        public int isangoserviceoptionid { get; set; }

    }
}
