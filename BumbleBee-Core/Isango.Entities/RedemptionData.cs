using Isango.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Isango.Entities
{
    public class RedemptionData
    {
        public string SupplierBookingReferenceNumber { get; set; }
        public string IsangoReferenceNumber { get; set; }
        public APIType APIType { get; set; }
        public Redemption_Status Status { get; set; }
    }

    public enum Redemption_Status
    {
        CONFIRMED = 1,
        Redemeed = 2,
        Default = 0


    }
}
