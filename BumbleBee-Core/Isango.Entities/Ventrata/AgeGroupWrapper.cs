using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Isango.Entities.Ventrata
{
    public class AgeGroupWrapperForVentrata
    {
        public List<ProductDetail> ProductDetails { get; set; }
        public List<Destination> Destinations { get; set; }
        public List<FAQ> Faqs { get; set; }
        public List<Option> Options { get; set; }
        public List<UnitsForOption> UnitDetailsForoptions { get; set; }

        public List<PackageInclude> PackageInclude { get; set; }
    }

    public class ProductDetail {
        public string ProductId { get; set; }
        public string Inclusions { get; set; }
        public string Exclusions { get; set; }
        public string CancellationPolicy { get; set; }
        public string ProductName { get; set; }
        public string Supplierid { get; set; }

        public bool isPackage { get; set; }
        
    }

    public class Destination {
        public string ProductId { get; set; }
        public string Country { get; set; }
        public string DestinationId { get; set; }

        public string DestinationName { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }

    }

    public class FAQ {
        public string ProductId { get; set; }

        public string Question { get; set; }
        public string Answer { get; set; }

    }

    public class Option {
        public string ProductId { get; set; }

        public string OptionId { get; set; }
        public bool IsDefault { get; set; }
        public string InternalName { get; set; }
        public string CancellationCutOff { get; set; }
        public int CancellationCutOffAmount { get; set; }
        public string CancellationCutOffUnit { get; set; }

    }

    public class UnitsForOption {
        public string ProductId { get; set; }
        public string OptionId { get; set; }
        public string UnitId { get; set; }
        public string InternalName { get; set; }
        public string Reference { get; set; }
        public string UnitType { get; set; }
        public int MinAge { get; set; }
        public int MaxAge { get; set; }
        public bool IdRequired { get; set; }
        public int MinQuantity { get; set; }
        public int MaxQuantity { get; set; }
        public int PaxCount { get; set; }
        public string AccompaniedBy { get; set; }

        public string SubTitle { get; set; }

    }
    public class PackageInclude
    {
        public string PackageIncludeId { get; set; }
        public string PackageIncludeOptionId { get; set; }

        public bool required { get; set; }
        public int limit { get; set; }
        public string PackageIncludeProductId { get; set; }

        
        public string ParentProductId { get; set; }
        public string ParentOptionId { get; set; }

        public string PackageIncludeTitle { get; set; }
        public int PackageIncludeCount { get; set; }
    }
}
