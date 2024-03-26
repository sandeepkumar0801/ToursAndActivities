using Isango.Entities.Enums;
using Isango.Entities.GlobalTixV3;
using ServiceAdapters.GlobalTix.GlobalTix.Entities.RequestResponseModels;
using ServiceAdapters.GlobalTixV3.GlobalTix.Entities.RequestResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAdapters.GlobalTixV3.GlobalTix.Entities
{
    public class ActivityInfoInputContext : InputContext
    {
        public string ActivityId { get; set; }
        public int? ServiceOptionID { get; set; }
        public int FactSheetId { get; set; }
        public int Days2Fetch { get; set; }
        public Data ActivityInfo { get; set; }
        public List<TicketType> TicketTypes { get; set; }
		public Dictionary<int, TicketTypeDetail> TicketDetails { get; set; }
		public Dictionary<PassengerType, int> NoOfPassengers;
		public DateTime CheckinDate;
        public List<ServiceAdapters.GlobalTixV3.GlobalTix.Entities.RequestResponseModels.DatumAvailability> PaxTypeDetails { get; set; }

        public List<RequestResponseModels.ProductOption.Datum> ProductOption { get; set; }
        public List<GlobalTixV3Mapping> GlobalTixV3Mapping { get; set; }

        public bool isNonThailandProduct { get; set; }

        public string TicketType { get; set; }
        
        public DateTime CheckOutDate { get; set; }

    }
}
