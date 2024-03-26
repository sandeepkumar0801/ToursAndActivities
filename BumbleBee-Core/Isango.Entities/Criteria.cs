using Isango.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Isango.Entities
{
    public class Criteria
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

        public bool IsPrivateTour { get; set; }

        public string CurrencyFromDataBase { get; set; }

    }
}