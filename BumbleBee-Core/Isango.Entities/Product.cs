using Isango.Entities.Activities;
using Isango.Entities.Enums;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Isango.Entities
{
    public class Product
    {
        public ProductType ProductType { get; set; }

        [JsonProperty(PropertyName = "activityId")]
        public int ID { get; set; }

        public string Name { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string Introduction { get; set; }
        public string ShortIntroduction { get; set; }
        public List<ProductImage> Images { get; set; }
        public string ThumbNailImage { get; set; }
        public string Title { get; set; }
        public List<ProductOption> ProductOptions { get; set; }
        public string RegionName { get; set; }
        public float AdditionalMarkUp { get; set; }
        public List<Region.Region> Regions { get; set; }

        public List<RouteMap> RouteMaps { get; set; }

        //Not removed as Min Margin discussion is pending which is required for the MatchAffiliate functions.
        public Margin Margin { get; set; }

        private decimal _OfferPercentage;

        public decimal OfferPercentage
        {
            get
            {
                _OfferPercentage = (BaseMinPrice != 0 && GateBaseMinPrice != 0) ? ((GateBaseMinPrice - BaseMinPrice) / BaseMinPrice) * 100 : 0;
                return _OfferPercentage;
            }
            set => _OfferPercentage = value;
        }

        public bool IsPackage { get; set; }
        public List<Badge> Badges { get; set; }
        public APIType ApiType { get; set; }
        public decimal SellMinPrice { get; set; }
        public decimal BaseMinPrice { get; set; }
        public decimal CostMinPrice { get; set; }
        public decimal GateBaseMinPrice { get; set; }
        public decimal GateSellMinPrice { get; set; }
        public string CancellationPolicy { get; set; }
        public PriceTypeId PriceTypeId { get; set; }
        public bool IsLivePrice { get; set; }
        public bool ShowSale { get; set; }
    }
}