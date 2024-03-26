using Isango.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Isango.Entities.Rezdy
{
    public class RezdySelectedProduct : SelectedProduct
    {
        public string ProductCode { get; set; }
        public string OrderNumber { get; set; }
        public RezdyProduct RezdyProduct { get; set; }
        public int Seats { get; set; }
        public int SeatsAvailable { get; set; }
        public DateTime StartTimeLocal { get; set; }
        public DateTime EndTimeLocal { get; set; }
        public List<RezdyLabelDetail> RezdyLabelDetails { get; set; }
        public List<RezdyPaxMapping> PaxMappings { get; set; }
        public List<BookingQuestions> BookingQuestions { get; set; }
        public RezdyPickUpLocation RezdyPickUpLocation { get; set; }
        public int PickUpId { get; set; }

        public string ReferenceNumber { get; set; }
    }

    public class BookingQuestions
    {
        public string Question { get; set; }
        public bool Required { get; set; }
        public List<string> Answers { get; set; }
        public List<AnswerOption> AnswerOptions { get; set; }
    }

    public class ExtraDetailsForRezdy
    {
        public List<BookingQuestions> ExtraDetailsBookingQuestions { get; set; }
        public Dictionary<int, string> PickupDetails { get; set; }
        public List<RezdyPickUpLocation> RezdyPickUpLocations { get; set; }
        public int PickUpId { get; set; }
    }

    public class AnswerOption
    {
        public string Value { get; set; }
        public string Label { get; set; }
    }
}