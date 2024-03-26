using System;
using System.Collections.Generic;

namespace Isango.Entities.PrioHub
{
    public class PrioHubProductPaxMapping
    {
        public string ProductTypeId { get; set; }
        public string ProductType { get; set; } //adult,child
        public string ProductTypeLabel { get; set; }
        public string ProductTypeClass { get; set; }//STANDARD
        public int ProductTypeAgeFrom { get; set; }
        public int ProductTypeAgeTo { get; set; }
        public string ProductTypePriceType { get; set; }//INDIVIDUAL
    }
}