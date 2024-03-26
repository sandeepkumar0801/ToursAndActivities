using ServiceAdapters.GlobalTix.GlobalTix.Entities.RequestResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAdapters.GlobalTix.GlobalTix.Entities
{
    public class PackageInfoInputContext : InputContext
    {
        public string PackageId { get; set; }
        public int? ServiceOptionID { get; set; }
        public int FactSheetId { get; set; }
        public int Days2Fetch { get; set; }
        public List<PackageInfo> LinkedPackages { get; set; }
        public List<TicketType> TicketTypes { get; set; }
        public DateTime CheckinDate { get; set; }
    }
}
