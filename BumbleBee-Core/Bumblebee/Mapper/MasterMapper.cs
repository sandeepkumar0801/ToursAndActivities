using Isango.Entities;
using Isango.Entities.Master;
using Isango.Entities.Region;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using WebAPI.Models.ResponseModels;
using WebAPI.Models.ResponseModels.DeltaMaster;
using WebAPI.Models.ResponseModels.Master;

namespace WebAPI.Mapper
{
    public class MasterMapper
    {
        /// <summary>
        /// This method prepare the request model for Badge
        /// </summary>
        /// <param name="badgeRequest"></param>
        /// <returns></returns>
        public List<BadgeResponse> MapBadgeRequest(List<Badge> badgeRequest)
        {
            var badgeResponse = new List<BadgeResponse>();
            if (badgeRequest != null)
            {
                foreach (var badge in badgeRequest)
                {
                    badgeResponse.Add(new BadgeResponse
                    {
                        BadgeID = badge.Id,
                        BadgeName = badge.Name,
                        LanguageCode = badge.LanguageCode
                    });
                }
            }
            return badgeResponse;
        }

        /// <summary>
        /// This method prepare the request model for Currency
        /// </summary>
        /// <param name="currencyRequest"></param>
        /// <returns></returns>
        public List<CurrencyResponse> MapCurrencyRequest(List<Currency> currencyRequest)
        {
            var currencyResponse = new List<CurrencyResponse>();
            if (currencyRequest != null)
            {
                foreach (var currency in currencyRequest)
                {
                    currencyResponse.Add(new CurrencyResponse
                    {
                        CurrencyName = currency.Name,
                        CurrencyIsoCode = currency.IsoCode,
                        CurrencyShortSymbol = currency.ShortSymbol,
                        CurrencySymbol = currency.Symbol
                    });
                }
            }
            return currencyResponse;
        }

        /// <summary>
        /// This method prepare the request model for Language
        /// </summary>
        /// <param name="languageRequest"></param>
        /// <returns></returns>
        public Dictionary<string, string> MapLanguageRequest(List<Language> languageRequest)
        {
            var languageDict = new Dictionary<string, string>();
            if (languageRequest != null)
            {
                foreach (var language in languageRequest)
                {
                    languageDict.Add(language.Code, language.Description);
                }
            }
            return languageDict;
        }

        /// <summary>
        /// This method prepare the request model for Geo Details
        /// </summary>
        /// <param name="geoDetailsRequest"></param>
        /// <returns></returns>
        public List<GeoDetailsResponse> MapGeoDetailsRequest(List<GeoDetails> geoDetailsRequest)
        {
            var geoDetailsResponse = new List<GeoDetailsResponse>();
            if (geoDetailsRequest != null)
            {
                foreach (var geo in geoDetailsRequest)
                {
                    geoDetailsResponse.Add(new GeoDetailsResponse
                    {
                        ContinentRegionId = geo.ContinentRegionID,
                        ContinentName = geo.ContinentName,
                        ContinentRegionCode = geo.ContinentRegionCode,
                        CountryRegionId = geo.CountryRegionID,
                        CountryName = geo.CountryName,
                        CountryRegionCode = geo.CountryRegionCode,
                        DestinationRegionId = geo.DestinationRegionID,
                        DestinationName = geo.DestinationName,
                        DestinationRegionCode = geo.DestinationRegionCode,
                        Latitudes = geo.Latitudes,
                        Longitudes = geo.Longitudes,
                        IsCountryChange = geo.IsCountryChange
                    });
                }
            }
            return geoDetailsResponse;
        }

        /// <summary>
        /// This method prepare the request model for Exchange Rates Request Details
        /// </summary>
        /// <param name="exchangeRatesRequest"></param>
        /// <returns></returns>
        public List<ExchangeRateResponse> MapExchangeRateRequest(List<CurrencyExchangeRates> exchangeRatesRequest)
        {
            var exchangeRateResponse = new List<ExchangeRateResponse>();
            if (exchangeRatesRequest != null)
            {
                foreach (var exchange in exchangeRatesRequest)
                {
                    exchangeRateResponse.Add(new ExchangeRateResponse
                    {
                        ExchangeRateValue = exchange.ExchangeRate,
                        FromCurrency = exchange.FromCurrencyCode,
                        ToCurrency = exchange.ToCurrencyCode
                    });
                }
            }
            return exchangeRateResponse;
        }

        /// <summary>
        /// This method prepare the request model for Region Attraction
        /// </summary>
        /// <param name="regionCategoryRequest"></param>
        /// <returns></returns>
        public List<RegionAttractionResponse> MapRegionAttractionRequest(List<RegionCategoryMapping> regionCategoryRequest)
        {
            var regionAttractionResponse = new List<RegionAttractionResponse>();
            if (regionCategoryRequest != null)
            {
                foreach (var regionCategory in regionCategoryRequest)
                {
                    regionAttractionResponse.Add(new RegionAttractionResponse
                    {
                        AttractionID = regionCategory.CategoryId,
                        AttractionName = regionCategory.CategoryName,
                        ImageID = regionCategory.ImageId,
                        IsTopAttraction = regionCategory.IsTopCategory,
                        IsVisibleOnSearch = regionCategory.IsVisibleOnSearch,
                        LanguageCode = regionCategory.Languagecode,
                        RegionID = regionCategory.RegionId,
                        RegionName = regionCategory.RegionName,
                        Sequence = regionCategory.Order,
                        Type_ = regionCategory.Type_,
                        Latitude = regionCategory.Latitude,
                        Longitude = regionCategory.Longitude
                    });
                }
            }
            return regionAttractionResponse;
        }

        /// <summary>
        /// This method prepare the request model for Region Sub Attraction
        /// </summary>
        /// <param name="regionSubAttractionRequest"></param>
        /// <returns></returns>
        public List<RegionSubAttractionResponse> MapRegionSubAttractionRequest(List<RegionSubAttraction> regionSubAttractionRequest)
        {
            var regionSubAttractionResponse = new List<RegionSubAttractionResponse>();
            if (regionSubAttractionRequest != null)
            {
                foreach (var regionCategory in regionSubAttractionRequest)
                {
                    regionSubAttractionResponse.Add(new RegionSubAttractionResponse
                    {
                        RegionId = regionCategory.RegionId,
                        RegionName = regionCategory.RegionName,
                        ParentAttractionId = regionCategory.ParentAttractionId,
                        ParentAttractionName = regionCategory.ParentAttractionname,
                        Type = regionCategory.Type,
                        SubAttractionId = regionCategory.SubAttractionId,
                        SubAttractionName = regionCategory.SubAttractionName,
                        SubFilterOrder = regionCategory.SubFilterOrder,
                        IsVisible = regionCategory.IsVisible
                    });
                }
            }
            return regionSubAttractionResponse;
        }

        /// <summary>
        /// This method prepare the request model for Attractions
        /// </summary>
        /// <param name="localizedMerchandisingRequest"></param>
        /// <returns></returns>
        public List<LocalizedMerchandisingResponse> MapAttractionRequest(List<LocalizedMerchandising> localizedMerchandisingRequest)
        {
            var localizedMerchandisingResponse = new List<LocalizedMerchandisingResponse>();
            if (localizedMerchandisingRequest != null)
            {
                foreach (var regionCategory in localizedMerchandisingRequest)
                {
                    localizedMerchandisingResponse.Add(new LocalizedMerchandisingResponse
                    {
                        AttractionID = regionCategory.Id,
                        AttractionName = regionCategory.Name,
                        LanguageCode = regionCategory.Language,
                        MultilingualName = regionCategory.MultilingualName
                    });
                }
            }
            return localizedMerchandisingResponse;
        }

        /// <summary>
        /// This method prepare the request model for Destination
        /// </summary>
        /// <param name="destinationRequest"></param>
        /// <returns></returns>
        public List<DestinationResponse> MapDestinationRequest(List<Destination> destinationRequest)
        {
            var destinationResponse = new List<DestinationResponse>();
            if (destinationRequest != null)
            {
                foreach (var destination in destinationRequest)
                {
                    destinationResponse.Add(new DestinationResponse
                    {
                        DestinationId = destination.Id,
                        DestinationName = destination.Name,
                        CountryId = destination.CountryId,
                        CountryName = destination.CountryName,
                        LanguageCode = destination.LanguageCode,
                        IsCountryChange = destination.IsCountryChange,
                        Latitudes = destination.Latitudes,
                        Longitudes = destination.Longitudes
                    });
                }
            }
            return destinationResponse;
        }

        /// <summary>
        /// This method prepare the request model for Affiliate
        /// </summary>
        /// <param name="affiliateRequest"></param>
        /// <returns></returns>
        public List<AffiliateResponse> MapAffiliateRequest(AffiliateAPI affiliateRequest)
        {
            var AffiliateAPIData = affiliateRequest?.AffiliateAPIData;
            var AffiliateCurrency = affiliateRequest?.AffiliateCurrency;
            var AffiliateEmail = affiliateRequest?.AffiliateEmail;
            var AffiliatePhone = affiliateRequest?.AffiliatePhone;
            var AffiliateServices = affiliateRequest?.AffiliateServices;

            var affiliateResponse = new List<AffiliateResponse>();
            if (AffiliateAPIData != null)
            {
                foreach (var a in AffiliateAPIData)
                {
                    if (AffiliateServices != null)
                        affiliateResponse.Add(new AffiliateResponse
                        {
                            DisplayName = a.DisplayName,
                            AffiliateID = a.AffiliateID,
                            AffiliateName = a.AffiliateName,
                            SupportedLanguages = a.SupportedLanguages,
                            GoogleTrackerID = a.GoogleTrackerID,
                            Email = a.Email,
                            AffiliateGroupID = a.AffiliateGroupID,
                            Alias = a.Alias,
                            IsmultiSave = a.IsmultiSave,
                            Google_TagManager = a.Google_TagManager,
                            LOB = a.LOB,
                            CompanyWebsite = a.CompanyWebsite,
                            DiscountPercent = a.DiscountPercent,
                            B2BAffiliateID = a.B2BAffiliateID,
                            MultiSavePercent = a.MultiSavePercent,
                            ServiceidLanguageCode = (AffiliateServices?.Where(x => x.AffiliateID == a.AffiliateID)
                                .GroupBy(x => x.LanguageCode)
                                .Select(y => new ServiceidLanguageCode
                                {
                                    ServiceId = string.Join(",", y.Select(x => x.ServiceId)),
                                    LanguageCode = y.Key
                                })).ToList(),

                            EmailPhoneLanguageWise = (from e in AffiliateEmail
                                                      join p in AffiliatePhone
                                                          on new { e.AffiliateID, e.LanguageCode } equals new
                                                          { p.AffiliateID, LanguageCode = p.SLanguage }
                                                      where e.AffiliateID == a.AffiliateID && p.AffiliateID == a.AffiliateID
                                                      group p by e
                                into g
                                                      select new EmailPhoneLanguageWiseResponse
                                                      {
                                                          LanguageCode = g.Key.LanguageCode,
                                                          Email = g.Key.Val,
                                                          Phone = g.Select((x => new PhoneResponse
                                                          { ServiceNo = x.ServiceNo, Country = x.Country, Sequence = x.Sequence })).ToList()
                                                      }).ToList(),
                            SupportedCurrency = AffiliateCurrency.Where(x => x.AffiliateID == a.AffiliateID)
                                .Select(X => X.CurrencyISOCode).ToList(),
                            WhiteLabelPartner = a.WhiteLabelPartner
                        });
                }
            }
            return affiliateResponse;
        }

        /// <summary>
        /// This method prepare the request model for cjFeed
        /// </summary>
        /// <param name="cjFeed"></param>
        /// <param name="currency"></param>
        /// <returns></returns>
        public string MapCjFeedRequest(string cjFeed, string currency)
        {
            return BuildHeader(currency) + cjFeed;
        }

        /// <summary>
        /// This method prepare the request model for Destination
        /// </summary>
        /// <param name="cjFeedProductRequest"></param>
        /// <returns></returns>
        public RootCriteo MapCJFeedProductRequest(List<CJFeedProduct> cjFeedProductRequest)
        {
            var cjFeedProductResponse = new List<CJFeedProductResponse>();
            if (cjFeedProductRequest != null)
            {
                foreach (var cjFeed in cjFeedProductRequest)
                {
                    cjFeedProductResponse.Add(new CJFeedProductResponse
                    {
                        categoryid1 = cjFeed.categoryid1,
                        categoryid2 = cjFeed.categoryid2,
                        categoryid3 = cjFeed.categoryid3,
                        description = cjFeed.description,
                        discount = cjFeed.discount,
                        id = cjFeed.id,
                        instock = cjFeed.instock,
                        name = cjFeed.name,
                        price = cjFeed.price,
                        producturl = cjFeed.producturl,
                        recommendable = cjFeed.recommendable,
                        retailprice = cjFeed.retailprice,
                        smallimage = cjFeed.smallimage
                    });
                }
            }

            /* Create an instance of the group to serialize, and set
             its properties. */
            var rootCriteo = new RootCriteo
            {
                products = new ProductsList()
            };
            var s = new XmlSerializer(typeof(RootCriteo));
            rootCriteo.products.productList = new List<CJFeedProductResponse>();
            rootCriteo.products.productList = cjFeedProductResponse;
            return rootCriteo;
        }

        private string BuildHeader(string currency)
        {
            string aID;
            var process = "<processtype>OVERWRITE</processtype>";
            switch (currency)
            {
                case "EUR":
                    aID = @"<subid>94848</subid>
<datefmt>DD/MM/YYYY</datefmt>
" + process + @"
<aid>10703104</aid>
</header>" + "\n";
                    break;

                case "USD":
                    aID = @"<subid>94849</subid>
<datefmt>DD/MM/YYYY</datefmt>
" + process + @"
<aid>10703102</aid>
</header>" + "\n";
                    break;

                default:
                    aID = @"<subid>92493</subid>
<datefmt>DD/MM/YYYY</datefmt>
" + process + @"
<aid>10590653</aid>
</header>" + "\n";
                    break;
            }

            return @"<?xml version=""1.0"" encoding=""UTF-8""?>
<!DOCTYPE product_catalog_data SYSTEM ""http://www.jdoqocy.com/content/dtd/product_catalog_data_1_1.dtd"">
<product_catalog_data>
<header>
<cid>1878825</cid>
" + aID;
        }

        /// <summary>
        /// This method prepare the request model for Currency
        /// </summary>
        /// <param name="currencyRequest"></param>
        /// <returns></returns>
        public List<CurrencyMasterResponse> MapCurrencyMasterRequest(List<Currency> currencyRequest)
        {
            var currencyMasterResponse = new List<CurrencyMasterResponse>();
            if (currencyRequest != null)
            {
                foreach (var currency in currencyRequest)
                {
                    currencyMasterResponse.Add(new CurrencyMasterResponse
                    {
                        ID = currency.CurrencyID,
                        Name = currency?.Name,
                        IsoCode = currency?.IsoCode,
                        Symbol = currency?.Symbol,
                        ShortSymbol = currency?.ShortSymbol,
                    });
                }
            }
            return currencyMasterResponse;
        }

        /// <summary>
        /// This method prepare the request model for Currency
        /// </summary>
        /// <param name="languageRequest"></param>
        /// <returns></returns>
        public List<LanguageMasterResponse> MapLanguageMasterRequest(List<Language> languageRequest)
        {
            var languageMasterResponse = new List<LanguageMasterResponse>();
            if (languageRequest != null)
            {
                foreach (var language in languageRequest)
                {
                    languageMasterResponse.Add(new LanguageMasterResponse
                    {
                        Code = language?.Code,
                        Name = language?.Description
                    });
                }
            }
            return languageMasterResponse;
        }

        /// <summary>
        /// This method prepare the request model for Geo Details
        /// </summary>
        /// <param name="geoDetailsRequest"></param>
        /// <returns></returns>
        public List<GeoDetailsMasterResponse> MapGeoDetailsMasterRequest(List<GeoDetails> geoDetailsRequest)
        {
            var geoDetailsResponse = new List<GeoDetailsMasterResponse>();
            if (geoDetailsRequest != null)
            {
                foreach (var geo in geoDetailsRequest)
                {
                    geoDetailsResponse.Add(new GeoDetailsMasterResponse
                    {
                        ContinentRegionID = geo?.ContinentRegionID,
                        ContinentName = geo?.ContinentName,
                        ContinentRegionCode = geo?.ContinentRegionCode,
                        CountryRegionID = geo?.CountryRegionID,
                        CountryName = geo?.CountryName,
                        CountryRegionCode = geo?.CountryRegionCode,
                        DestinationRegionID = geo.DestinationRegionID,
                        DestinationName = geo?.DestinationName,
                        DestinationRegionCode = geo?.DestinationRegionCode,
                        Latitudes = geo?.Latitudes,
                        Longitudes = geo?.Longitudes
                    });
                }
            }
            return geoDetailsResponse;
        }

        /// <summary>
        /// This method prepare the request model for region wise
        /// </summary>
        /// <param name="regionWiseProductDetailsRequest"></param>
        /// <returns></returns>
        public List<RegionWiseMasterResponse> MapRegionWiseMasterRequest(List<RegionWiseProductDetails> regionWiseProductDetailsRequest)
        {
            var regionWiseMasterResponse = new List<RegionWiseMasterResponse>();
            if (regionWiseProductDetailsRequest != null)
            {
                //1.) Group by Region ID
                var groupByRegionId = regionWiseProductDetailsRequest.GroupBy(u => u.RegionID).ToList();
                foreach (var item in groupByRegionId)
                {
                    var regionWiseMaster = new RegionWiseMasterResponse
                    {
                        RegionID = item.FirstOrDefault().RegionID,
                        RegionName = item?.FirstOrDefault()?.RegionName?.Trim()
                    };
                    //2.) Group by ActivityId according to above RegionId (one region multiple activity combination)
                    var groupByActivityId = item.GroupBy(u => u.ActivityID).ToList();
                    var activityMasterList = new List<ActivityMasterResponse>();
                    foreach (var itemInner in groupByActivityId)
                    {
                        var passengerTypeMasterList = new List<PassengerTypeMasterResponse>();
                        foreach (var itemCommon in itemInner)
                        {
                            var passengerTypeMaster = new PassengerTypeMasterResponse
                            {
                                
                                Name = itemCommon?.PassengerTypeName?.Trim(),
                                minAge = itemCommon.FromAge,
                                maxAge = itemCommon.ToAge,
                                minSize = itemCommon.MinSize,
                                maxSize = itemCommon.MaxSize,
                                Label = itemCommon.Label,
                               passengerTypeID = itemCommon.PassengerTypeID
                            };
                            if (!String.IsNullOrEmpty(passengerTypeMaster.Name))
                            {
                                passengerTypeMasterList.Add(passengerTypeMaster);
                            }
                        }
                        var activityMasterResponse = new ActivityMasterResponse
                        {
                            ID = itemInner.FirstOrDefault().ActivityID,
                            Name = itemInner?.FirstOrDefault()?.ActivityName?.Trim(),
                            PassengerTypeMasterResponse = passengerTypeMasterList
                        };
                        if (activityMasterResponse != null)
                        {
                            activityMasterList.Add(activityMasterResponse);
                        }
                    }
                    regionWiseMaster.ActivityMasterResponse = activityMasterList;
                    if (regionWiseMaster != null)
                    {
                        regionWiseMasterResponse.Add(regionWiseMaster);
                    }
                }
            }
            return regionWiseMasterResponse;
        }

        /// <summary>
        /// This method prepare the request model for Destination
        /// </summary>
        /// <param name="productSupplierRequest"></param>
        /// <returns></returns>
        public List<ProductSupplierResponse> MapProductSupplierRequest(List<ProductSupplier> productSupplierRequest)
        {
            var productSupplierResponse = new List<ProductSupplierResponse>();
            if (productSupplierRequest != null)
            {
                foreach (var productSupplier in productSupplierRequest)
                {
                    productSupplierResponse.Add(new ProductSupplierResponse
                    {
                        SupplierId = productSupplier.SupplierId,
                        SupplierName = productSupplier.SupplierName,
                        Supplierinfo = productSupplier.Supplierinfo,
                        Link = productSupplier.Link,
                        Languageid = productSupplier.Languageid
                    });
                }
            }
            return productSupplierResponse;
        }
    }
}