using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Isango.Entities.Tiqets
{
    public class ProductDetails
    {
        public string ProductId { get; set; }
        public string ServiceId { get; set; }
        public string StartPointLatitude { get; set; }
        public string StartPointLongitude { get; set; }
        public string StartPointAddress { get; set; }
        public string StartPointCity { get; set; }

        public bool IsSkipTheLine { get; set; }
        public bool IsSmartPhoneTicket { get; set; }
        public string Duration { get; set; }
        public string VenueId { get; set; }
        public string VenueName { get; set; }
        public string VenueAddress { get; set; }
        public bool IsInstantTicket { get; set; }
        public string sale_status_expected_reopen { get; set; }
        public string sale_status_reason { get; set; }
        public string sale_status { get; set; }
		public bool is_package { get; set; }


		public string Language { get; set; }
		public string Title { get; set; }
		public string CountryName { get; set; }
		public string CityName { get; set; }
		public float Geolocation_Latitude { get; set; }
		public float Geolocation_Longitude { get; set; }
		public decimal Price { get; set; }
		public string Summary { get; set; }
		public string Included { get; set; }
		public string Excluded { get; set; }
		public double Display_Price { get; set; }

		public string Live_guide_languages { get; set; }
		public string Audio_guide_languages { get; set; }
		public string Starting_time { get; set; }

		public bool wheelchair_access { get; set; }

		public string Supplier_Name { get; set; }

		public string Supplier_City { get; set; }

        public string Marketing_Restrictions { get; set; }
        //public List<Image> Images { get; set; }
    }

    public class Image {
        public string Large { get; set; }
    }
}
