using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAdapters.CitySightSeeing.CitySightSeeing.Entities.Products
{
    
    public class ProductResponse
    {
        public int id { get; set; }
        public string externalId { get; set; }
        public string name { get; set; }
        public string supplier { get; set; }
        public int supplierId { get; set; }
        public List<Option> options { get; set; }
    }

    public class Option
    {
        public int id { get; set; }
        public string externalId { get; set; }
        public string name { get; set; }
    }

}
