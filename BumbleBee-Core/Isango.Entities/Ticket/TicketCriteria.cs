using System.Collections.Generic;

namespace Isango.Entities.Ticket
{
    public class TicketCriteria : Criteria
    {
        public List<int> HotelIds { get; set; }

        /// <summary>
        /// Required to filter out right modality from new Hotelbeds Apitude Api
        /// </summary>
        public string Destination { get; set; }

        public List<int> FactSheetIds { get; set; }
    }
}