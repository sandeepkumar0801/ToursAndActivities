using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Isango.Entities
{
   public class ExternalProducts
    {
        public int CssProductOptionId { get; set; }
        public string IsangoProductOptionId { get; set; }

        public string productName { get; set; }
        public int CssProductId { get; set; }
        public int supplierId { get; set; }

    }
}
