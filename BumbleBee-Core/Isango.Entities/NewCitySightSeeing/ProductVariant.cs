using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Isango.Entities.NewCitySightSeeing
{
	public class ProductVariant
    {
		public int Id { get; set; }
		public int ProductId { get; set; }
        public string Code { get; set; }
        public string VariantCode { get; set; }
		public string Title { get; set; }
        public string VariantName { get; set; }
        
    }
}
