using Isango.Entities.Enums;
using ServiceAdapters.GlobalTix.GlobalTix.Entities.RequestResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAdapters.GlobalTix.GlobalTix.Entities
{
    public class ActivityInfoInputContext : InputContext
    {
        public string ActivityId { get; set; }
        public int? ServiceOptionID { get; set; }
        public int FactSheetId { get; set; }
        public int Days2Fetch { get; set; }
        public ActivityInfo ActivityInfo { get; set; }
        public List<TicketType> TicketTypes { get; set; }
		public Dictionary<int, TicketTypeDetail> TicketDetails { get; set; }
		public Dictionary<PassengerType, int> NoOfPassengers;
		public DateTime CheckinDate;
        public List<TicketTypeDetail> PaxTypeDetails { get; set; }

    }
}
