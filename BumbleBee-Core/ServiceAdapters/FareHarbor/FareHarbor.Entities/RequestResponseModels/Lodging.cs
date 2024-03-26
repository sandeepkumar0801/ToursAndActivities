using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAdapters.FareHarbor.FareHarbor.Entities.RequestResponseModels
{
    public class Lodging
    {
        public int pk { get; set; }
        public string name { get; set; }
        public bool is_pickup_available { get; set; }
    }
}
