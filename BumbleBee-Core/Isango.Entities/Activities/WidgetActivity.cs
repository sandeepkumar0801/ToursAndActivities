using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Isango.Entities.Activities
{
    public class WidgetActivity
    {
        public string ProductName { get; set; }
        public string ProductUrl { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public string ProductImage { get; set; }
    }

    public class WidgetMappedData
    {
        public string CSRegionName { get; set; }
        public int languageid { get; set; }
        public int Isangoregionid { get; set; }
        public string SEFURL { get; set; }
        public string languagecode { get; set; }
    }

    public class WidgetResult
    {
        public string RegionURL { get; set; }
        public List<WidgetActivity> Activities { get; set; }
    }
}
