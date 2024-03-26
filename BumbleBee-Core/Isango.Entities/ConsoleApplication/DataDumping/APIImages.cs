using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Isango.Entities.ConsoleApplication.DataDumping
{
    public class APIImages
    {
        public string ServiceID { get; set; }
        public string SupplierProductID { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Continent { get; set; }
        public string Path { get; set; }
        public int APITypeID { get; set; }
        public string ImageURL { get; set; }
        public int? Sequence { get; set; }
        public string PublicID { get; set; }
        public int? ID { get; set; }
    }
}
