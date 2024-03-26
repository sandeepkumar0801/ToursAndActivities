using Isango.Entities.Enums;
using System;
using System.Collections.Generic;

namespace Isango.Entities.GoCity
{
    public class GoCitySelectedProduct : SelectedProduct
    {
        public string ProductCode { get; set; }
        public string VariantCode { get; set; }
        public DateTime ReservationDate { get; set; }

        public string Code { get; set; }

        public string Destination { get; set; }

        public string BookingReference { get; set; }

        public string ShortReference { get; set; }

        public string SupplierReferenceNumber { get; set; }

        public string BarcodedData { get; set; }
        public string OrderNumber { get; set; }
        public List<Passlist> Passlist { get; set; }

        public string GetPassUrl { get; set; }
        public string PrintPassesUrl { get; set; }
        public string MobilePassesUrl { get; set; }
        public string APIStatus { get; set; }
        public string CustomerEmail { get; set; }
    }
    public class Passlist
    {
        public string SkuCode { get; set; }
        public long ExpDate { get; set; }
        public long CreatedDate { get; set; }
        public string ConfirmationCode { get; set; }
    }

}
