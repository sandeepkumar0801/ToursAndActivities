using Isango.Entities.Enums;
using Isango.Entities.GrayLineIceLand;
using System;
using System.Collections.Generic;

namespace ServiceAdapters.GrayLineIceLand.GrayLineIceLand.Entities.RequestResponseModels
{
    public class InputContext
    {
        public string AuthToken { get; set; }

        public MethodType MethodType { get; set; }

        public DateTime DateFrom { get; set; }

        public DateTime DateTo { get; set; }

        public int Adults { get; set; }

        public int Youths { get; set; }

        public int Children { get; set; }

        public List<int> ChildAges { get; set; }

        public string TourNumber { get; set; }

        public int Language { get; set; }

        public List<GrayLineIceLandSelectedProduct> SelectedProducts { get; set; }

        public int AgentProfileId { get; set; }

        public string CurrencyCode { get; set; }

        public Boolean IsBookingTemp { get; set; }

        public string BookingNumber { get; set; }

        public Dictionary<PassengerType, int> PaxAgeGroupIds { get; set; }

        public string BookingReference { get; set; }
    }
}