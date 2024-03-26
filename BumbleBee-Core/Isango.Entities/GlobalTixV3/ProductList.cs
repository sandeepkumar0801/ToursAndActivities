using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Isango.Entities.GlobalTixV3
{
    public class ProductList
    {
        public string Country { get; set; }
        public float OriginalPrice { get; set; }
        //public string Keywords { get; set; }
       // public object FromPrice { get; set; }
        public string City { get; set; }
        public string Currency { get; set; }
        public int Id { get; set; }
        public bool IsOpenDated { get; set; }
        public bool IsOwnContracted { get; set; }
        public bool IsFavorited { get; set; }
        public bool IsBestSeller { get; set; }
        //public object FromReseller { get; set; }
        public string Name { get; set; }
        public bool IsInstantConfirmation { get; set; }
        public string Category { get; set; }
       // public List<Tickettype> Tickettype { get; set; }
        public bool success { get; set; }
    }
        public class Rootobject
        {
            public object Error { get; set; }
            public int Size { get; set; }
            public bool Success { get; set; }
        }

}
