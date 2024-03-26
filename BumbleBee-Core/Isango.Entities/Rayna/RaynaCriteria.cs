using Isango.Entities.Ticket;
using System;
using System.Collections.Generic;

namespace Isango.Entities.Rayna
{
    public class RaynaCriteria : Criteria
    {
        public string ModalityCode { get; set; }
        public string IsangoActivityId { get; set; }
        public List<IsangoHBProductMapping> ProductMapping { get; set; }
        public int FactSheetId { get; set; }
        public int Days2Fetch { get; set; }
        public int? ServiceOptionID { get; set; }
        public string ActivityId { get; set; }

        public DateTime PassDate { get; set; }

        public int TourOptionId { get; set; }
        public int TransferId { get; set; }
        public int TourId { get; set; }
        public string CurrencyIsoCode { get; set; }

        public bool IsCalendarDumping { get; set; }

        public List<string> SupplierOptionIds { get; set; }

    }
}