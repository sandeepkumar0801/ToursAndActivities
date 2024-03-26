using Isango.Entities.Enums;
using Isango.Entities.GlobalTixV3;
using Isango.Entities.TourCMS;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Isango.Entities
{
    public class CanocalizationCriteria
    {
        public int ActivityId { get; set; }
        public DateTime CheckinDate { get; set; }

        public DateTime CheckoutDate { get; set; }

        [XmlIgnore]
        public Dictionary<PassengerType, int> NoOfPassengers { get; set; }

        [XmlIgnore]
        public Dictionary<PassengerType, int> Ages { get; set; }

        /// <summary>
        /// Required to filter out right prices from api response,
        /// This is master data of PassengerInfo.
        /// </summary>
        public List<Booking.PassengerInfo> PassengerInfo { get; set; }

        /// <summary>
        /// Session token
        /// </summary>
        public virtual string Token { get; set; }

        [XmlIgnore]
        public Dictionary<PassengerType, int> PassengerAgeGroupIds { get; set; } //Rezdy

        public string Language { get; set; }

        public bool IsBundle { get; set; }

        public decimal? ActivityMargin { get; set; }

        /// Redeam V1.2 Start
        public string SupplierId { get; set; }
        public string ProductId { get; set; }
        public List<string> RateIds { get; set; }
        public string RateId { get; set; }
        public string Quantity { get; set; }
        public Dictionary<string, string> RateIdAndType { get; set; }
        public int ServiceOptionId { get; set; }
        public string ApiToken { get; set; }
        /// Redeam V1.2 End
        ///globalTix Start
        public string ActivityIdStr { get; set; }
        public int FactSheetId { get; set; }
        public int Days2Fetch { get; set; }
        public int? ServiceOptionID { get; set; }
        ///globalTix end
        public List<GlobalTixV3Mapping> GlobalTixV3Mapping { get; set; }
        public string Currency { get; set; }
    }
}
 
 
 