using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAdapters.Tiqets.Tiqets.Entities.RequestResponseModels
{
	public class PackageOrderRequest
	{
		public string package_product_id { get; set; }

		[JsonProperty(PropertyName = "day")]
		public string day { get; set; }

		[JsonProperty(PropertyName = "timeslot")]
		public string timeslot { get; set; }

		[JsonProperty(PropertyName = "customer_details")]
		public CustomerDetail CustomerDetail { get; set; }

		[JsonProperty(PropertyName = "visitors_details")]
		public List<VisitorsDetail> VisitorsDetails { get; set; }

		[JsonProperty(PropertyName = "variants")]
		public List<PackageVariant> Variants { get; set; }

	}
	public class CustomerDetail
	{
		public string email { get; set; }
		public string firstname { get; set; }
		public string lastname { get; set; }

		[JsonProperty(PropertyName = "phone")]
		public string phone { get; set; }
	}
	public class PackageDetails
	{
		public int package_id { get; set; }
		public List<PackageVariant> package_variants { get; set; }
		public List<PackageProduct> package_products { get; set; }
	}

	public class PackageProduct
	{
		[JsonProperty(PropertyName = "package_product_id")]
		public string package_product_id { get; set; }

		[JsonProperty(PropertyName = "day")]
		public string day { get; set; }

		[JsonProperty(PropertyName = "timeslot")]
		public string timeslot { get; set; }
	}

	public class PackageVariant
	{
		public int package_variant_id { get; set; }
		public int count { get; set; }
		public List<VisitorsDetail> visitors_details { get; set; }
	}

	public class RootTiqets
	{
		public PackageDetails package_details { get; set; }
		public CustomerDetail customer_details { get; set; }
        public List<PackageVariant> visitors_details { get; set; }
    }

	public class VisitorsDetail
	{
		public string full_name { get; set; }
	}
}
