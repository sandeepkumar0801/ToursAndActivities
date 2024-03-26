using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Isango.Entities
{
    public class CssExternalOption
    {
        public int CssProductId { get; set; }
        public int CssProductOptionId { get; set; }
        public int SupplierId { get; set; }

        public int IsangoOptionId { get; set; }

        public string timeslot { get; set; }


    }

}
