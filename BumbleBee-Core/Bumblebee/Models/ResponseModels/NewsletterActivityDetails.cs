using Isango.Entities;
using Isango.Entities.Review;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.ResponseModels
{
    public class NewsletterActivityDetails
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string ActivityUrls { get; set; }

        public decimal BaseMinPrice { get; set; }
        public decimal GateBaseMinPrice { get; set; }
        public string CurrencyIsoCode { get; set; }

        public List<ProductImage> Images { get; set; }
        public double OverAllRating { get; set; }
        public string ReasonToBook { get; set; }





    }
}