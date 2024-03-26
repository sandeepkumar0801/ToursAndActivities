using Newtonsoft.Json;
using System.Collections.Generic;

namespace Isango.Entities.Tiqets
{
    public class ProductVariantIdName
    {
        public int Id { get; set; }
        public List<string> ProductVariantName { get; set; }
    }
}