using Isango.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Isango.Entities.Bokun
{
    public class BokunSelectedProduct : SelectedProduct
    {
        public AvailabilityStatus AvailabilityStatus { get; set; }

        [XmlIgnore]
        public List<int> PricingCategoryIds { get; set; }

        [XmlIgnore]
        public int StartTimeId { get; set; }

        [XmlIgnore]
        public string EditType { get; set; }

        // [XmlIgnore]
        public List<Question> Questions { get; set; }

        public DateTime DateStart { get; set; }

        [XmlIgnore]
        public string ConfirmationCode { get; set; }

        [XmlIgnore]
        public string ProductConfirmationCode { get; set; }

        public int RateId { get; set; }

        public string QrCode { get; set; }

        public PickupPlaceDetails PickupLocation { get; set; }

        public string Code { get; set; }

        public string Destination { get; set; }

        public int FactsheetId { get; set; }

        public int Quantity { get; set; }

        public string BookingReferenceNumber { get; set; }
        public string Token { get; set; }
    }

    public class ExtraDetailsForBokun
    {
        public Dictionary<int, string> PickupDetails { get; set; }
        public Dictionary<int, string> DropoffDetails { get; set; }
        public List<Question> Questions { get; set; }
    }
}