using Isango.Entities.Enums;
using Isango.Entities.TourCMS;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Isango.Entities.TourCMS
{
    public class TourCMSSelectedProduct : SelectedProduct
    {
        public AvailabilityStatus AvailabilityStatus { get; set; }

        [XmlIgnore]
        public int StartTimeId { get; set; }

        [XmlIgnore]
        public string EditType { get; set; }

        public List<QuestionsForTourCMS> Questions { get; set; }

        public DateTime DateStart { get; set; }

        public int RateId { get; set; }

        public string QrCode { get; set; }

        public PickupPlaceDetails PickupLocation { get; set; }

        public string Code { get; set; }

        public string Destination { get; set; }

        public int FactsheetId { get; set; }

        public int Quantity { get; set; }

        public string BookingReferenceNumber { get; set; }
        public string Token { get; set; }
        public string RateKey { get; set; }

        public List<ContractQuestion> ContractQuestions { get; set; }

        public int BookingId { get; set; }

        public string BookingStatus { get; set; }
        public string ReservationReference { get; set; }
        public bool TicketPerPassenger { get; set; }
        public string BookingReference { get; set; }
        public string ShortReference { get; set; }
        public string SupplierReferenceNumber { get; set; }

        public List<TourCMSTicket> TourCMSTicket { get; set; }
        public string BarcodedData { get; set; }

        public List<PickupPointsForTourCMS> PickupPointsForTourCMS { get; set; }
    }
    public class PickupPlaceDetails
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }
    public class ExtraDetailsForTourCMS
    {
        public Dictionary<int, string> PickupDetails { get; set; }
        public List<QuestionsForTourCMS> Questions { get; set; }
    }

    public class TourCMSTicket
    {
        public string TicketType { get; set; }

        public string TicketBarCode { get; set; }
        public string PassengerType { get; set; }
        public string Quantity { get; set; }

        public string BarcodeSymbology { get; set; }
    }
}