using Isango.Entities.Enums;
using System;
using System.Collections.Generic;

namespace Isango.Entities.NewCitySightSeeing
{
    public class NewCitySightSeeingSelectedProduct : SelectedProduct
    {
        public string ProductCode { get; set; }
        public string VariantCode { get; set; }
        public DateTime ReservationDate { get; set; }

        public string Code { get; set; }

        public string Destination { get; set; }

        public string NewCitySightSeeingReservationId { get; set; }

        public string NewCitySightSeeingOrderCode { get; set; }

        public DateTime NewCitySightSeeingOrderDate { get; set; }

        public List<LineList> NewCitySightSeeingLines { get; set; }

        public string BookingReference { get; set; }

        public string ShortReference { get; set; }

        public string SupplierReferenceNumber { get; set; }

        public string BarcodedData { get; set; }
    }

    public class LineList
    {
        public string OrderLineCode { get; set; }
        public string Rate { get; set; }
        public int Quantity { get; set; }
        public string QrCode { get; set; }
    }
}
