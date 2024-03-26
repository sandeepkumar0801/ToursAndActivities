using Isango.Entities.GlobalTixV3;
using ServiceAdapters.GlobalTix.GlobalTix.Entities.RequestResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using packageData = ServiceAdapters.GlobalTixV3.GlobalTix.Entities.RequestResponseModels.PackageOptions.PackageOptionsList;

namespace ServiceAdapters.GlobalTixV3.GlobalTix.Entities
{
    public class PackageInfoInputContext : InputContext
    {
        public string PackageId { get; set; }
        public int? ServiceOptionID { get; set; }
        public int FactSheetId { get; set; }
        public int Days2Fetch { get; set; }
        public packageData.Datum LinkedPackages { get; set; }
        public List<TicketType> TicketTypes { get; set; }
        public DateTime CheckinDate { get; set; }
        public List<GlobalTixV3Mapping> GlobalTixV3Mapping { get; set; }
        public bool isNonThailandProduct { get; set; }
    }
}
