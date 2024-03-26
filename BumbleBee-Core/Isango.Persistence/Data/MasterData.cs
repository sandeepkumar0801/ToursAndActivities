using Isango.Entities;
using Isango.Entities.Bokun;
using Isango.Entities.ConsoleApplication.DataDumping;
using Isango.Entities.Enums;
using Isango.Entities.GlobalTixV3;
using Isango.Entities.Hotel;
using Isango.Entities.HotelBeds;
using Isango.Entities.Master;
using Isango.Entities.Region;
using Isango.Entities.Rezdy;
using Isango.Entities.SiteMap;
using Isango.Entities.Tiqets;
using Isango.Entities.TourCMS;
using Isango.Entities.v1Css;
using Isango.Entities.Ventrata;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using Util;
using Constant = Isango.Persistence.Constants.Constants;
using GoldenTours = Isango.Entities.GoldenTours;

namespace Isango.Persistence.Data
{
    public class MasterData
    {
        /// <summary>
        /// This method returns currency data mapping from reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public Currency GetCurrencyData(IDataReader reader)
        {
            var currency = new Currency
            {
                IsoCode = DbPropertyHelper.StringPropertyFromRow(reader, "CurrencyCode"),
                Name = DbPropertyHelper.StringPropertyFromRow(reader, "CurrencyName"),
                Symbol = DbPropertyHelper.StringPropertyFromRow(reader, "CurrencySymbol")
            };

            return currency;
        }

        /// <summary>
        /// This method returns country data mapping from reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public Region GetCountryData(IDataReader reader)
        {
            var region = new Region
            {
                Id = DbPropertyHelper.Int32PropertyFromRow(reader, "CountryID"),
                Name = DbPropertyHelper.StringPropertyFromRow(reader, "CountryName"),
                Url = DbPropertyHelper.StringPropertyFromRow(reader, "countrycode").ToUpper()
            };

            return region;
        }

        /// <summary>
        /// This method returns filtered theme part ticket data mapping from reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public Entities.TicketByRegion GetFilteredThemeParkTicketsData(IDataReader reader)
        {
            var filteredTicket = new Entities.TicketByRegion
            {
                CountryCode = DbPropertyHelper.StringPropertyFromRow(reader, "countrycode"),
                ThemeparkTicket = new Entities.ThemeparkTicket
                {
                    ProductId = DbPropertyHelper.Int32PropertyFromRow(reader, "serviceid"),
                    City = DbPropertyHelper.Int32PropertyFromRow(reader, "cityid"),
                    Region = DbPropertyHelper.Int32PropertyFromRow(reader, "regionid"),
                    Country = DbPropertyHelper.Int32PropertyFromRow(reader, "countryid")
                }
            };

            return filteredTicket;
        }

        /// <summary>
        /// This method returns auto suggest data mapping from reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public AutoSuggest GetMasterAutoSuggestData(IDataReader reader)
        {
            var autoSuggest = new AutoSuggest
            {
                Label = DbPropertyHelper.StringPropertyFromRow(reader, "keyword"),
                Display = DbPropertyHelper.StringPropertyFromRow(reader, "displayname"),
                Category = DbPropertyHelper.StringPropertyFromRow(reader, "Category") == "D" ||
                           DbPropertyHelper.StringPropertyFromRow(reader, "Category") == "C"
                    ? "Destinations"
                    : (DbPropertyHelper.StringPropertyFromRow(reader, "Category") == "A"
                        ? "Top Attractions"
                        : "Tours and Activity"),
                Type = DbPropertyHelper.StringPropertyFromRow(reader, "Category"),
                ParentId = DbPropertyHelper.StringPropertyFromRow(reader, "ParentID"),
                ReferenceId = DbPropertyHelper.StringPropertyFromRow(reader, "refid"),
                Languagecode = DbPropertyHelper.StringPropertyFromRow(reader, "languagecode")
            };
            return autoSuggest;
        }

        /// <summary>
        /// This method returns region data mapping from reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public LatLongVsurlMapping GetRegionData(IDataReader reader)
        {
            var latitude = DbPropertyHelper.DoublePropertyFromRow(reader, "latitudes");
            var longitude = DbPropertyHelper.DoublePropertyFromRow(reader, "longitudes");
            if (!string.IsNullOrEmpty(latitude.ToString(CultureInfo.InvariantCulture).Trim()) &&
                !string.IsNullOrEmpty(longitude.ToString(CultureInfo.InvariantCulture).Trim()))
            {
                var coordVsRegionUrlMapping = new LatLongVsurlMapping
                {
                    RegionId = DbPropertyHelper.Int32PropertyFromRow(reader, "regionid"),
                    Latitude = latitude,
                    Longitude = longitude,
                    RegionUrl = DbPropertyHelper.StringPropertyFromRowWithTrim(reader, "sefurl"),
                    CountryName = DbPropertyHelper.StringPropertyFromRowWithTrim(reader, "countryname"),
                    CityName = DbPropertyHelper.StringPropertyFromRowWithTrim(reader, "regionname")
                };
                return coordVsRegionUrlMapping;
            }

            return null;
        }

        /// <summary>
        /// This method returns support phone with country code data mapping from reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetSupportPhonesWithCountryCodeData(IDataReader reader)
        {
            var supportNumber = new Dictionary<string, string>
            {
                {
                    DbPropertyHelper.StringPropertyFromRow(reader, "Country"),
                    DbPropertyHelper.StringPropertyFromRow(reader, "ServiceNo")
                }
            };

            return supportNumber;
        }

        /// <summary>
        /// This method returns blog data mapping from reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public BlogData GetBlogData(IDataReader reader)
        {
            var blogData = new BlogData
            {
                Title = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "title"),
                Link = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "link"),
                Description = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "description"),
                PublishedDate = DbPropertyHelper.DateTimePropertyFromRow(reader, "publishDate"),
                Category = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "category"),
                ImageName = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "imagepath"),
                BlogId = DbPropertyHelper.Int32PropertyFromRow(reader, "id")
            };

            return blogData;
        }

        /// <summary>
        /// This method returns site map data mapping from reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public SiteMapData GetSiteMapData(IDataReader reader)
        {
            var siteMapRegion = new SiteMapData
            {
                RegionId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "regionid"),
                ParentId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "parentid"),
                Order = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "Order_"),
                RegionType = DbPropertyHelper.StringPropertyFromRow(reader, "NType"),
                RegionName = DbPropertyHelper.StringPropertyFromRow(reader, "regionname"),
                Url = DbPropertyHelper.StringPropertyFromRow(reader, "URL")
            };

            return siteMapRegion;
        }

        /// <summary>
        /// This method returns indexed attractions to region urls data mapping from reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public Dictionary<string, string> IndexedAttractionToRegionUrlsData(IDataReader reader)
        {
            var urlsDictionary = new Dictionary<string, string>
            {
                {
                    DbPropertyHelper.StringPropertyFromRowWithTrim(reader, "sourceurl"),
                    DbPropertyHelper.StringPropertyFromRowWithTrim(reader, "destinationurl")
                }
            };

            return urlsDictionary;
        }

        /// <summary>
        /// This method returns exchange range data mapping from reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public CurrencyExchangeRates ExchangeRangeData(IDataReader reader)
        {
            var currencyExchangeRate = new CurrencyExchangeRates
            {
                FromCurrencyCode = DbPropertyHelper.StringPropertyFromRow(reader, "from_cur"),
                ToCurrencyCode = DbPropertyHelper.StringPropertyFromRow(reader, "to_cur"),
                ExchangeRate = DbPropertyHelper.DecimalPropertyFromRow(reader, "exchangeratevalue")
            };

            return currencyExchangeRate;
        }

        /// <summary>
        /// This method returns supported language data mapping from reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public Language SupportedLanguageData(IDataReader reader)
        {
            var language = new Language
            {
                Code = DbPropertyHelper.StringPropertyFromRow(reader, "LanguageCode"),
                Description = DbPropertyHelper.StringPropertyFromRow(reader, "Name")
            };

            return language;
        }

        /// <summary>
        /// This method returns all facility data mapping from reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public Facility GetAllFacilityData(IDataReader reader)
        {
            var facility = new Facility
            {
                Id = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "FacilityID"),
                Name = DbPropertyHelper.StringPropertyFromRow(reader, "FacilityName"),
                Image = DbPropertyHelper.StringPropertyFromRow(reader, "ImageName")
            };

            return facility;
        }

        /// <summary>
        /// This method returns region category data mapping from reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public RegionCategorySimilarProducts GetRegionCategoryMappingData(IDataReader reader)
        {
            var regionCategoryMapping = new RegionCategorySimilarProducts()
            {
                RegionId = DbPropertyHelper.Int32PropertyFromRow(reader, Constant.RegionIdReader),
                AttractionId = DbPropertyHelper.Int32PropertyFromRow(reader, Constant.AttractionIdReader),
                Priority = DbPropertyHelper.Int32PropertyFromRow(reader, Constant.OrderReader)
            };

            return regionCategoryMapping;
        }

        /// <summary>
        /// This method returns net price master data mapping from reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public NetPriceMasterData LoadNetPriceMasterData(IDataReader reader)
        {
            var netPriceMasterData = new NetPriceMasterData
            {
                AffiliateId = DbPropertyHelper.StringPropertyFromRow(reader, Constant.CacheAffiliateId),
                ProductId = DbPropertyHelper.Int32PropertyFromRow(reader, Constant.CacheServiceId),
                CommisionPercentage =
                    DbPropertyHelper.DecimalPropertyFromRow(reader, Constant.CommissionPercent),
                NetPrice = DbPropertyHelper.DecimalPropertyFromRow(reader, Constant.B2BNetPrice),
                CurrencyCode = DbPropertyHelper.StringPropertyFromRow(reader, Constant.CurrencyIsoCode),
                MaxSellPrice = DbPropertyHelper.DecimalPropertyFromRow(reader, Constant.MaxSell),
                CostPrice = DbPropertyHelper.DecimalPropertyFromRow(reader, Constant.CostAmount),
                ApiType = (APIType)Enum.Parse(typeof(APIType),
                    DbPropertyHelper.StringPropertyFromRow(reader, Constant.APITypeID))
            };

            return netPriceMasterData;
        }

        /// <summary>
        /// This method returns fact sheet data mapping from reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public IsangoHBProductMapping LoadFactSheetMappingData(IDataReader reader)
        {
            var mapping = new IsangoHBProductMapping
            {
                IsangoHotelBedsActivityId = DbPropertyHelper.Int32PropertyFromRow(reader, Constant.CacheServiceId),

                HotelBedsActivityCode = DbPropertyHelper.StringPropertyFromRow(reader, Constant.CacheServiceCode),

                IsangoRegionId = DbPropertyHelper.Int32PropertyFromRow(reader, Constant.RegionIdReader),

                FactSheetId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, Constant.CacheFactSheetId),

                MinAdultCount = DbPropertyHelper.Int32PropertyFromRow(reader, Constant.CacheMinAdult),

                ApiType = (APIType)DbPropertyHelper.Int32PropertyFromRow(reader, Constant.APITypeID)
            };

            return mapping;
        }

        /// <summary>
        /// This method returns region vs destination data mapping from reader
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="mappedRegions"></param>
        /// <returns></returns>
        public void MappedRegionVsDestinationData(IDataReader reader, ref List<MappedRegion> mappedRegions)
        {
            var regionId = DbPropertyHelper.Int32PropertyFromRow(reader, "RegionID");

            if (!mappedRegions.Exists(m => m.RegionId == regionId))
            {
                mappedRegions.Add(new MappedRegion
                {
                    RegionId = regionId,
                    RegionName = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "RegionName"),
                    DestinationCode = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "Code")
                });
            }
        }

        /// <summary>
        /// This method returns language data mapping from reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public MappedLanguage LoadMappedLanguageData(IDataReader reader)
        {
            var mappedLanguage = new MappedLanguage
            {
                AffiliateId = DbPropertyHelper.StringPropertyFromRow(reader, "affiliateID"),
                IsangoLanguageCode = DbPropertyHelper.StringPropertyFromRow(reader, "isangolanguagecode"),
                SupplierLanguageCode = DbPropertyHelper.StringPropertyFromRow(reader, "HBlanguagecode"),
                GliLanguageCode = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "GLIlanguagecode")
            };

            return mappedLanguage;
        }

        /// <summary>
        /// This method returns GLI age group by activity data mapping from reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public AgeGroup GetGliAgeGroupByActivityData(IDataReader reader)
        {
            var ageGroup = new AgeGroup
            {
                Id = $"{DbPropertyHelper.Int32PropertyFromRow(reader, Constant.CachedActivityId)}-{DbPropertyHelper.Int32PropertyFromRow(reader, Constant.AgeGroupId)}",
                ApiType = Convert.ToInt32(APIType.Graylineiceland),
                AgeGroupId = DbPropertyHelper.Int32PropertyFromRow(reader, Constant.AgeGroupId),
                ActivityId = DbPropertyHelper.Int32PropertyFromRow(reader, Constant.CachedActivityId),
                FromAge = DbPropertyHelper.Int32PropertyFromRow(reader, Constant.FromAge),
                ToAge = DbPropertyHelper.Int32PropertyFromRow(reader, Constant.ToAge),
                Description = DbPropertyHelper.StringPropertyFromRow(reader, Constant.Description),
                PassengerType = (PassengerType)Enum.Parse(typeof(PassengerType),
                    DbPropertyHelper.StringPropertyFromRow(reader, Constant.PassengerType))
            };

            return ageGroup;
        }

        /// <summary>
        /// This method returns Prio age group by activity data mapping from reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public AgeGroup GetPrioAgeGroupByActivityData(IDataReader reader)
        {
            var ageGroup = new AgeGroup
            {
                Id = $"{DbPropertyHelper.Int32PropertyFromRow(reader, Constant.CachedActivityId)}-{DbPropertyHelper.Int32PropertyFromRow(reader, Constant.AgeGroupId)}",
                ApiType = Convert.ToInt32(APIType.Prio),
                AgeGroupId = DbPropertyHelper.Int32PropertyFromRow(reader, Constant.AgeGroupId),
                ActivityId = DbPropertyHelper.Int32PropertyFromRow(reader, Constant.CachedActivityId),
                FromAge = DbPropertyHelper.Int32PropertyFromRow(reader, Constant.FromAge),
                ToAge = DbPropertyHelper.Int32PropertyFromRow(reader, Constant.ToAge),
                Description = DbPropertyHelper.StringPropertyFromRow(reader, Constant.Description),
                PassengerType = (PassengerType)Enum.Parse(typeof(PassengerType),
                    DbPropertyHelper.StringPropertyFromRow(reader, Constant.PassengerType))
            };

            return ageGroup;
        }

        public AgeGroup GetPrioHubAgeGroupByActivityData(IDataReader reader)
        {
            var ageGroup = new AgeGroup
            {
                Id = $"{DbPropertyHelper.Int32PropertyFromRow(reader, Constant.CachedActivityId)}-{DbPropertyHelper.Int32PropertyFromRow(reader, Constant.AgeGroupId)}",
                ApiType = Convert.ToInt32(APIType.Prio),
                AgeGroupId = DbPropertyHelper.Int32PropertyFromRow(reader, Constant.AgeGroupId),
                ActivityId = DbPropertyHelper.Int32PropertyFromRow(reader, Constant.CachedActivityId),
                FromAge = DbPropertyHelper.Int32PropertyFromRow(reader, Constant.FromAge),
                ToAge = DbPropertyHelper.Int32PropertyFromRow(reader, Constant.ToAge),
                Description = DbPropertyHelper.StringPropertyFromRow(reader, Constant.Description),
                PassengerType = (PassengerType)Enum.Parse(typeof(PassengerType),
                    DbPropertyHelper.StringPropertyFromRow(reader, Constant.PassengerType))
            };

            return ageGroup;
        }

        /// <summary>
        /// This method returns AOT age group by activity data mapping from reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public AgeGroup GetAotAgeGroupByActivityData(IDataReader reader)
        {
            var ageGroup = new AgeGroup
            {
                Id = $"{DbPropertyHelper.Int32PropertyFromRow(reader, Constant.CachedActivityId)}-{DbPropertyHelper.Int32PropertyFromRow(reader, Constant.AgeGroupId)}",
                ApiType = Convert.ToInt32(APIType.Aot),
                ActivityId = DbPropertyHelper.Int32PropertyFromRow(reader, Constant.CachedActivityId),
                AgeGroupId = DbPropertyHelper.Int32PropertyFromRow(reader, Constant.AgeGroupId),
                FromAge = DbPropertyHelper.Int32PropertyFromRow(reader, Constant.FromAge),
                ToAge = DbPropertyHelper.Int32PropertyFromRow(reader, Constant.ToAge),
                Description = DbPropertyHelper.StringPropertyFromRow(reader, Constant.Description),
                PassengerType = (PassengerType)Enum.Parse(typeof(PassengerType),
                    DbPropertyHelper.StringPropertyFromRow(reader, Constant.PassengerType))
            };

            return ageGroup;
        }

        /// <summary>
        /// This method returns cross sell product data mapping from reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        //public Dictionary<int, List<CrossSellProduct>> GetCrossSellProductData(IDataReader reader)
        //{
        //    var allXProducts = new Dictionary<int, List<CrossSellProduct>>();
        //    var xSellProducts = new List<CrossSellProduct>();
        //    var product = new CrossSellProduct();
        //    var prevRegion = 0;

        //    while (reader.Read())
        //    {
        //        var region = DbPropertyHelper.Int32PropertyFromRow(reader, "regionid");

        //        if (!allXProducts.ContainsKey(prevRegion) && region != prevRegion)
        //        {
        //            allXProducts.Add(prevRegion, xSellProducts);
        //            xSellProducts = new List<CrossSellProduct>();
        //        }

        //        product.Id = DbPropertyHelper.Int32PropertyFromRow(reader, "SERVICEID");
        //        product.Priority = DbPropertyHelper.Int32PropertyFromRow(reader, "priority");
        //        var attractionList = DbPropertyHelper.StringPropertyFromRow(reader, "attractionlist");

        //        product.AttractionIDs = new List<int>();

        //        if (!string.IsNullOrEmpty(attractionList))
        //        {
        //            var attractions = attractionList.Split(',');
        //            foreach (var item in attractions)
        //            {
        //                product.AttractionIDs.Add(int.Parse(item));
        //            }
        //        }

        //        xSellProducts.Add(product);
        //        prevRegion = DbPropertyHelper.Int32PropertyFromRow(reader, "regionid");
        //    }

        //    return allXProducts;
        //}

        public Dictionary<int, List<CrossSellProduct>> GetCrossSellProductData(IDataReader reader)
        {
            var allXProducts = new Dictionary<int, List<CrossSellProduct>>();
            var xSellProducts = new List<CrossSellProduct>();
            var prevLob = 0;

            while (reader.Read())
            {
                var lineOfBusinessId = DbPropertyHelper.Int32PropertyFromRow(reader, "lineOfBusiness");
                var regionid = DbPropertyHelper.Int32PropertyFromRow(reader, "regionid");

                if (!allXProducts.ContainsKey(prevLob) && lineOfBusinessId != prevLob)
                {
                    allXProducts.Add(prevLob, xSellProducts);
                    xSellProducts = new List<CrossSellProduct>();
                }
                var product = new CrossSellProduct();
                product.Id = DbPropertyHelper.Int32PropertyFromRow(reader, "SERVICEID");
                product.Priority = DbPropertyHelper.Int32PropertyFromRow(reader, "priority");
                var attractionList = DbPropertyHelper.StringPropertyFromRow(reader, "attractionlist");

                product.AttractionIDs = new List<int>();

                if (!string.IsNullOrEmpty(attractionList))
                {
                    var attractions = attractionList.Split(',');
                    foreach (var item in attractions)
                    {
                        product.AttractionIDs.Add(int.Parse(item));
                    }
                }
                var attractionTicket = reader["HasAttractionTicket"]?.ToString();
                if (attractionTicket != null && attractionTicket == "4029")
                {
                    product.HasAttractionTicket = true;
                }
                xSellProducts.Add(product);
                prevLob = DbPropertyHelper.Int32PropertyFromRow(reader, "lineOfBusiness");
            }
            allXProducts.Remove(0);
            return allXProducts;
        }

        public List<CrossSellLogic> GetCrossSellLogicData(IDataReader reader)
        {
            var crossSellLogic = new List<CrossSellLogic>();

            while (reader.Read())
            {
                var logic = new CrossSellLogic();
                logic.LineofBusinessid = DbPropertyHelper.Int32PropertyFromRow(reader, "LineofBusinessid");
                logic.CrossSellBusiness1 = DbPropertyHelper.Int32PropertyFromRow(reader, "CrossSellBusiness1");
                logic.CrossSellBusiness2 = DbPropertyHelper.Int32PropertyFromRow(reader, "CrossSellBusiness2");
                logic.LINEOFBUSINESSNAME = DbPropertyHelper.StringPropertyFromRow(reader, "LINEOFBUSINESSNAME");

                crossSellLogic.Add(logic);
            }
            return crossSellLogic;
        }

        /// <summary>
        /// This method returns customer prototype by activity data mapping from reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public CustomerPrototype GetCustomerPrototypeByActivityData(IDataReader reader)
        {
            var ageGroupType = DbPropertyHelper.StringPropertyFromRow(reader, "AGEGROUPTYPE");
            var PassengerTypeID = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "PassengerTypeID");
            var PassengerTypeId = (PassengerType)(PassengerTypeID);
            var customerPrototype = new CustomerPrototype
            {
                CustomerPrototypeId = DbPropertyHelper.Int32PropertyFromRow(reader, "customerprototypeid"),
                ServiceOptionId = DbPropertyHelper.Int32PropertyFromRow(reader, "serviceoptionid"),
                ServiceId = DbPropertyHelper.Int32PropertyFromRow(reader, "serviceid"),
                AgeGroupId = DbPropertyHelper.Int32PropertyFromRow(reader, "AGEGROUPID"),
                PassengerType = PassengerTypeId,
                StartAt = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "startsat"),
                IsUnitPrice = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "ISUNITPRICE"),
                PassengersInUnitMinimum = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "UNITMIN"),
                PassengersInUnitMaximum = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "UNITMAX")
            };

            return customerPrototype;
        }

        /// <summary>
        /// This method returns age group by activity data mapping from reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public FareHarborAgeGroup GetFareHarborAgeGroupsByActivityData(IDataReader reader)
        {
            var ageGroup = new FareHarborAgeGroup
            {
                Id = $"{DbPropertyHelper.Int32PropertyFromRow(reader, "serviceid")}-{DbPropertyHelper.Int32PropertyFromRow(reader, Constant.AgeGroupId)}",
                ApiType = Convert.ToInt32(APIType.Fareharbor),
                ActivityId = DbPropertyHelper.Int32PropertyFromRow(reader, "serviceid"),
                AgeGroupId = DbPropertyHelper.Int32PropertyFromRow(reader, "AGEGROUPID"),
                DisplayName = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "AGEGROUPNAME"),
                Description = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "AGEGROUPDESC"),
                AgeGroupType = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "AGEGROUPTYPE"),
                FromAge = DbPropertyHelper.Int32PropertyFromRow(reader, "FROMAGE"),
                ToAge = DbPropertyHelper.Int32PropertyFromRow(reader, "TOAGE"),
                PassengerType = (PassengerType)Enum.Parse(typeof(PassengerType),
                    DbPropertyHelper.StringPropertyFromRow(reader, Constant.PassengerType))
            };

            return ageGroup;
        }

        /// <summary>
        /// This method returns fare harbor user keys data mapping from reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public FareHarborUserKey GetAllFareHarborUserKeysData(IDataReader reader)
        {
            var userKey = new FareHarborUserKey
            {
                CompanyShortName = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "SERVICESHORTNAME"),
                Currency = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "currency"),
                UserKey = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "userkey")
            };

            return userKey;
        }

        /// <summary>
        /// This method returns the pricing category data mapping from reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public PriceCategory GetPricingCategoryData(IDataReader reader)
        {
            var passengerTypeID = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "PassengerTypeID");
            var passengerType = (PassengerType)(passengerTypeID);
            return new PriceCategory
            {
                AgeGroupId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "AgeGroupID"),
                OptionId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "OptionID"),
                ServiceOptionCode = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "ServiceOptionCode"),
                ServiceId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "ServiceID"),
                PriceCategoryId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "PriceCategoryID"),
                Title = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "Title"),
                PassengerTypeId = passengerType
            };
        }

        /// <summary>
        /// This method returns age group data mapping from reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public AgeGroup LoadAgeGroupData(IDataReader reader)
        {
            return new AgeGroup
            {
                AgeGroupId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "AgeGroupId"),
                ActivityId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "ActivityId"),
                FromAge = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "FromAge"),
                ToAge = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "ToAge"),
                Description = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "FullTitle"),
                //PriceCategoryId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "")
            };
        }

        /// <summary>
        /// Load localized destinations
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public List<LocalizedDestinations> GetLocalizedDestinations(IDataReader reader)
        {
            var CitiesAndLanguages = new List<LocalizedDestinations>();
            while (reader.Read())
            {
                var City = new Entities.Destination
                {
                    Id = DbPropertyHelper.Int32PropertyFromRow(reader, "regionid"),
                    Name = DbPropertyHelper.StringPropertyFromRow(reader, "regionname").Trim()
                };
                var lang = DbPropertyHelper.StringPropertyFromRow(reader, "LanguageCode").Trim().ToLower();

                var City4CurrentLang = CitiesAndLanguages.Find(cNL => cNL.Language == lang);

                if (City4CurrentLang != null)
                {
                    City4CurrentLang.Destinations.Add(City);
                }
                else
                {
                    var CityBy1Lang = new LocalizedDestinations
                    {
                        Language = lang,
                        Destinations = new List<Entities.Destination>()
                    };

                    CityBy1Lang.Destinations.Add(City);
                    CitiesAndLanguages.Add(CityBy1Lang);
                }
            }
            return CitiesAndLanguages;
        }

        /// <summary>
        /// Load localized categories
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public List<LocalizedCategories> GetLocalizedCategories(IDataReader reader)
        {
            var CategoriesAndLanguages = new List<LocalizedCategories>();
            while (reader.Read())
            {
                var lang = reader["LanguageCode"].ToString().Trim().ToLower();

                var cat4CurrentLang = CategoriesAndLanguages.Find(cNL => cNL.Language == lang);
                var category = new Dictionary<int, string>();

                if (cat4CurrentLang != null)
                {
                    var thisRegion = DbPropertyHelper.Int32PropertyFromRow(reader, "regionid");
                    if (cat4CurrentLang.DestinationCategories.TryGetValue(thisRegion, out category))
                        category.Add(DbPropertyHelper.Int32PropertyFromRow(reader, "attractionid"), DbPropertyHelper.StringPropertyFromRow(reader, "attractionname").Trim());
                    else
                    {
                        category = new Dictionary<int, string>
                        {
                            {
                                DbPropertyHelper.Int32PropertyFromRow(reader, "attractionid"),
                                DbPropertyHelper.StringPropertyFromRow(reader, "attractionname").Trim()
                            }
                        }; //Because if not found TryGetValue sets category as null
                        cat4CurrentLang.DestinationCategories.Add(thisRegion, category);
                    }
                }
                else
                {
                    var cat4Language = new LocalizedCategories
                    {
                        Language = lang,
                        DestinationCategories = new Dictionary<int, Dictionary<int, string>>()
                    };

                    var thisRegion = DbPropertyHelper.Int32PropertyFromRow(reader, "regionid");
                    category.Add(DbPropertyHelper.Int32PropertyFromRow(reader, "attractionid"), DbPropertyHelper.StringPropertyFromRow(reader, "attractionname").Trim());
                    cat4Language.DestinationCategories.Add(thisRegion, category);
                    CategoriesAndLanguages.Add(cat4Language);
                }
            }
            return CategoriesAndLanguages;
        }

        /// <summary>
        /// Load Url and page Id mapping
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public UrlPageIdMapping GetUrlPageID(IDataReader reader)
        {
            var urlPageIdmapping = new UrlPageIdMapping
            {
                PageUrl = DbPropertyHelper.StringPropertyFromRow(reader, "pageurl"),
                AffiliateId = DbPropertyHelper.StringPropertyFromRow(reader, "affiliateid"),
                PageId = DbPropertyHelper.Int32PropertyFromRow(reader, "pageid"),
                RegionId = DbPropertyHelper.Int32PropertyFromRow(reader, "regionid"),
                CategoryId = DbPropertyHelper.Int32PropertyFromRow(reader, "categoryid"),
                LanguageCode = DbPropertyHelper.StringPropertyFromRow(reader, "languagecode"),
                PageName = DbPropertyHelper.StringPropertyFromRow(reader, "pagename")
            };

            return urlPageIdmapping;
        }

        /// <summary>
        /// Map Hotel Beds Credentials
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public HotelBedsCredentials GetHotelBedsCredentials(IDataReader reader)
        {
            var hotelBedsCredentials = new HotelBedsCredentials
            {
                AffiliateId = DbPropertyHelper.StringPropertyFromRow(reader, "AffiliateID"),
                Authentication = DbPropertyHelper.StringPropertyFromRow(reader, "Username") + "><" + DbPropertyHelper.StringPropertyFromRow(reader, "Password"),
                LanguageCode = DbPropertyHelper.StringPropertyFromRow(reader, "languagecode")
            };

            return hotelBedsCredentials;
        }

        /// <summary>
        /// Map pick up locations
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public PickupLocation GetPickupLocationsByActivity(IDataReader reader)
        {
            var pickupLocation = new PickupLocation
            {
                LocationId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "Id"),
                ActivityId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "ActivityId"),
                Description = DbPropertyHelper.StringPropertyFromRow(reader, "Description"),
                Name = DbPropertyHelper.StringPropertyFromRow(reader, "Name"),
                Id = DbPropertyHelper.StringPropertyFromRow(reader, "Id")
            };

            return pickupLocation;
        }

        public Entities.Ventrata.SupplierDetails GetVentrataSupplierDetail(IDataReader reader)
        {
            var supplierDetail = new Entities.Ventrata.SupplierDetails
            {
                SupplierName = DbPropertyHelper.StringPropertyFromRow(reader, "suppliername"),
                SupplierBearerToken = DbPropertyHelper.StringPropertyFromRow(reader, "supplierid"),
                BaseURL = DbPropertyHelper.StringPropertyFromRow(reader, "BaseURL"),
                IsPerPaxQRCode = DbPropertyHelper.StringPropertyFromRow(reader, "IsPerPaxQRCode")
            };

            return supplierDetail;
        }

        /// <summary>
        /// Load FactSheet Mappings
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public List<IsangoHBProductMapping> LoadLiveHBOptionsData(IDataReader reader)
        {
            var mappingCollection = new List<IsangoHBProductMapping>();

            while (reader.Read())
            {
                var mapping = new IsangoHBProductMapping
                {
                    IsangoHotelBedsActivityId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "serviceID"),
                    HotelBedsActivityCode = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "ServiceCode"),
                    IsangoRegionId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "regionid"),
                    FactSheetId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "factsheetid"),
                    MinAdultCount = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "MinAdult"),
                    ApiType = (APIType)Enum.Parse(typeof(APIType),
                        DbPropertyHelper.StringDefaultPropertyFromRow(reader, "APITypeID")),
                    CountryId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "CountryID"),

                    MarginAmount = DbPropertyHelper.DecimalDefaultNullablePropertyFromRow(reader, "MarginAmount"),
                    IsMarginPercent = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "IsMarginPercent"),
                    ServiceOptionInServiceid = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "ServiceOptionInServiceid"),
                    CurrencyISOCode = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "CurrencyISOCode"),
                    SupplierCode = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "SupplierCode"),
                    PrefixServiceCode = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "PrefixServiceCode"),
                    PriceTypeId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "PriceTypeID"),
                    IsIsangoMarginApplicable = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "IsIsangoMarginApplicable")
                };
                mappingCollection.Add(mapping);
            }

            return mappingCollection;
        }

        public int GetActivityIdByOptionId(IDataReader reader)
        {
            var serviceId = 0;
            while (reader.Read())
            {
                serviceId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "ServiceID");
            }

            return serviceId;
        }

        public List<string> GetTokenByAvailabilityReferenceId(IDataReader reader)
        {
            var result = new List<string>();
            while (reader.Read())
            {
                result.Add(DbPropertyHelper.StringDefaultPropertyFromRow(reader, "TokenID"));
            }

            return result;
        }

        /// <summary>
        /// Load FactSheet Mappings
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public List<IsangoHBProductMapping> LoadLiveHBOptionsDataApitudeContent(IDataReader reader)
        {
            var mappingCollection = new List<IsangoHBProductMapping>();

            while (reader.Read())
            {
                var mapping = new IsangoHBProductMapping
                {
                    IsangoHotelBedsActivityId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "serviceID"),
                    SupplierCode = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "servicecode"),
                    FactSheetId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "factsheetid"),
                    Language = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "languagecode"),
                    MinAdultCount = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "MinAdult"),
                    ApiType = APIType.Hotelbeds
                };
                mappingCollection.Add(mapping);
            }

            return mappingCollection;
        }

        /// <summary>
        /// This method returns Region Category Mapping data mapping from reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public RegionCategoryMapping RegionCategoryMappingData(IDataReader reader)
        {
            var regionCategoryMapping = new RegionCategoryMapping
            {
                RegionId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "RegionID"),
                RegionName = DbPropertyHelper.StringPropertyFromRow(reader, "RegionName"),
                CategoryId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "AttractionID"),
                CategoryName = DbPropertyHelper.StringPropertyFromRow(reader, "AttractionName"),
                Type_ = DbPropertyHelper.StringPropertyFromRow(reader, "Type_"),
                Order = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "Sequence"),
                IsVisibleOnSearch = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "IsVisibleOnSearch"),
                IsTopCategory = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "IsTopAttraction"),
                ImageId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "ImageID"),
                Languagecode = DbPropertyHelper.StringPropertyFromRow(reader, "LanguageCode"),
                Latitude = DbPropertyHelper.StringPropertyFromRow(reader, "Latitude"),
                Longitude = DbPropertyHelper.StringPropertyFromRow(reader, "Longitude")
            };
            return regionCategoryMapping;
        }

        /// <summary>
        /// This method returns Region Category Mapping data mapping from reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public RegionSubAttraction RegionSubAttractionData(IDataReader reader)
        {
            var regionSubAttraction = new RegionSubAttraction
            {
                RegionId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "RegionID"),
                RegionName = DbPropertyHelper.StringPropertyFromRow(reader, "RegionName"),
                ParentAttractionId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "ParentAttractionID"),
                ParentAttractionname = DbPropertyHelper.StringPropertyFromRow(reader, "ParentAttractionname"),
                Type = DbPropertyHelper.StringPropertyFromRow(reader, "Type_"),
                SubAttractionId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "SubAttractionID"),
                SubAttractionName = DbPropertyHelper.StringPropertyFromRow(reader, "SubAttractionName"),
                SubFilterOrder = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "SubFilterOrder"),
                IsVisible = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "IsVisible"),
            };
            return regionSubAttraction;
        }

        /// <summary>
        /// This method returns Attractions Mapping data mapping from reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public LocalizedMerchandising AttractionsData(IDataReader reader)
        {
            var attractions = new LocalizedMerchandising
            {
                Id = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "AttractionID"),
                Name = DbPropertyHelper.StringPropertyFromRow(reader, "AttractionName"),
                Language = DbPropertyHelper.StringPropertyFromRow(reader, "LanguageCode"),
                MultilingualName = DbPropertyHelper.StringPropertyFromRow(reader, "MultilingualName")
            };
            return attractions;
        }

        /// <summary>
        /// This method returns Affiliate Mapping data mapping from reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public List<AffiliateAPIData> AffiliateAPIData(IDataReader reader)
        {
            var affiliateAPICollection = new List<AffiliateAPIData>();
            while (reader.Read())
            {
                var affiliateAPIData = new AffiliateAPIData
                {
                    DisplayName = DbPropertyHelper.StringPropertyFromRow(reader, "DisplayName"),
                    AffiliateID = DbPropertyHelper.StringPropertyFromRow(reader, "AffiliateID"),
                    AffiliateName = DbPropertyHelper.StringPropertyFromRow(reader, "AffiliateName"),
                    SupportedLanguages = DbPropertyHelper.StringPropertyFromRow(reader, "SupportedLanguages"),
                    GoogleTrackerID = DbPropertyHelper.StringPropertyFromRow(reader, "GoogleTrackerID"),
                    Email = DbPropertyHelper.StringPropertyFromRow(reader, "Email"),
                    AffiliateGroupID = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "AffiliateGroupID"),
                    Alias = DbPropertyHelper.StringPropertyFromRow(reader, "Alias"),
                    IsmultiSave = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "IsmultiSave"),
                    MultiSavePercent = DbPropertyHelper.Int16PropertyFromRow(reader, "MultiSavePercent"),
                    Google_TagManager = DbPropertyHelper.StringPropertyFromRow(reader, "Google_TagManager"),
                    LOB = DbPropertyHelper.Int32PropertyFromRow(reader, "LOB"),
                    CompanyWebsite = DbPropertyHelper.StringPropertyFromRow(reader, "CompanyWebsite"),
                    B2BAffiliateID = DbPropertyHelper.StringPropertyFromRow(reader, "B2BAffiliateID"),
                    DiscountPercent = DbPropertyHelper.DoubleNullablePropertyFromRow(reader, "DiscountPercent"),
                    WhiteLabelPartner = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "WhiteLabelPartner")
                };
                affiliateAPICollection.Add(affiliateAPIData);
            }
            return affiliateAPICollection;
        }

        /// <summary>
        /// This method returns Affiliate Currency Mapping data mapping from reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public List<AffiliateCurrency> AffiliateCurrencyData(IDataReader reader)
        {
            var affiliateCurrencyCollection = new List<AffiliateCurrency>();
            while (reader.Read())
            {
                var affiliateCurrency = new AffiliateCurrency
                {
                    AffiliateID = DbPropertyHelper.StringPropertyFromRow(reader, "AffiliateID"),
                    CurrencyISOCode = DbPropertyHelper.StringPropertyFromRow(reader, "CurrencyISOCode"),
                };

                affiliateCurrencyCollection.Add(affiliateCurrency);
            }
            return affiliateCurrencyCollection;
        }

        /// <summary>
        /// This method returns Affiliate Email Mapping data mapping from reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public List<AffiliateEmail> AffiliateEmailData(IDataReader reader)
        {
            var affiliateEmailCollection = new List<AffiliateEmail>();
            while (reader.Read())
            {
                var affiliateEmail = new AffiliateEmail
                {
                    AffiliateID = DbPropertyHelper.StringPropertyFromRow(reader, "AffiliateID"),
                    LanguageCode = DbPropertyHelper.StringPropertyFromRow(reader, "LanguageCode"),
                    Val = DbPropertyHelper.StringPropertyFromRow(reader, "Val")
                };
                affiliateEmailCollection.Add(affiliateEmail);
            }
            return affiliateEmailCollection;
        }

        /// <summary>
        /// This method returns Affiliate Phone Mapping data mapping from reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public List<AffiliatePhone> AffiliatePhoneData(IDataReader reader)
        {
            var affiliatePhoneCollection = new List<AffiliatePhone>();
            while (reader.Read())
            {
                var affiliatePhone = new AffiliatePhone
                {
                    AffiliateID = DbPropertyHelper.StringPropertyFromRow(reader, "AffiliateID"),
                    SLanguage = DbPropertyHelper.StringPropertyFromRow(reader, "SLanguage"),
                    ServiceNo = DbPropertyHelper.StringPropertyFromRow(reader, "ServiceNo"),
                    Country = DbPropertyHelper.StringPropertyFromRow(reader, "Country"),
                    Sequence = DbPropertyHelper.Int16PropertyFromRow(reader, "Sequence")
                };
                affiliatePhoneCollection.Add(affiliatePhone);
            }
            return affiliatePhoneCollection;
        }

        /// <summary>
        /// This method returns Affiliate Service Mapping data mapping from reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public List<AffiliateServices> AffiliateServiceData(IDataReader reader)
        {
            var affiliateServicesCollection = new List<AffiliateServices>();
            while (reader.Read())
            {
                var affiliateServices = new AffiliateServices
                {
                    AffiliateID = DbPropertyHelper.StringPropertyFromRow(reader, "AffiliateID"),
                    ServiceId = DbPropertyHelper.Int32PropertyFromRow(reader, "ServiceId"),
                    LanguageCode = DbPropertyHelper.StringPropertyFromRow(reader, "languagecode")
                };
                affiliateServicesCollection.Add(affiliateServices);
            }
            return affiliateServicesCollection;
        }

        /// <summary>
        /// This method returns Region Category Mapping data mapping from reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public string CJFeedMappingData(IDataReader reader)
        {
            var sb = new StringBuilder();
            while (reader.Read())
            {
                //name, keywords, description, sku, buyurl, available, imageurl, price
                sb.Append("\n<product>\n");
                sb.AppendFormat("<id>{0}</id>\n", reader["sku"].ToString().Replace("\n", string.Empty).Replace("\r", string.Empty).Replace("\r\n", string.Empty));
                sb.AppendFormat("<title><![CDATA[{0}]]></title>\n", asAscii(reader["name"].ToString().Replace("\n", string.Empty).Replace("\r", string.Empty).Replace("\r\n", string.Empty)).Replace('|', '-'));
                sb.AppendFormat("<link><![CDATA[http://www.isango.com{0}]]></link>\n", asAscii(reader["buyurl"].ToString().Replace("\n", string.Empty).Replace("\r", string.Empty).Replace("\r\n", string.Empty)));
                sb.AppendFormat("<price>{0}</price>\n", reader["price"].ToString().Replace("\n", string.Empty).Replace("\r", string.Empty).Replace("\r\n", string.Empty));
                sb.AppendFormat("<sale_price>{0}</sale_price>\n", reader["price"].ToString().Replace("\n", string.Empty).Replace("\r", string.Empty).Replace("\r\n", string.Empty));
                sb.AppendFormat("<image_link><![CDATA[{0}]]></image_link>\n", reader["imageurl"].ToString().Replace("\n", string.Empty).Replace("\r", string.Empty).Replace("\r\n", string.Empty));
                sb.AppendFormat("<thumbnailImage><![CDATA[{0}]]></thumbnailImage>\n", reader["imageurl"].ToString().Replace("\n", string.Empty).Replace("\r", string.Empty).Replace("\r\n", string.Empty));

                sb.AppendFormat("<description><![CDATA[{0}]]></description>\n",
                    string.IsNullOrEmpty(reader["description"].ToString())
                        ? asAscii(reader["name"].ToString().Replace("\n", string.Empty).Replace("\r", string.Empty)
                            .Replace("\r\n", string.Empty))
                        : asAscii(Sanitizer.Sanitize(reader["description"].ToString()).Replace("\n", string.Empty)
                            .Replace("\r", string.Empty).Replace("\r\n", string.Empty).Replace('|', '-')));

                sb.AppendFormat("<availability>{0}</availability>\n", reader["available"].ToString().Replace("\n", string.Empty).Replace("\r", string.Empty).Replace("\r\n", string.Empty));
                sb.AppendFormat("<condition><![CDATA[{0}]]></condition>\n", asAscii(Sanitizer.Sanitize("New")));
                sb.AppendFormat("<custom_label_0><![CDATA[{0}]]></custom_label_0>\n", asAscii(Sanitizer.Sanitize(reader["merchandisetype"].ToString()).Replace("\n", string.Empty).Replace("\r", string.Empty).Replace("\r\n", string.Empty)));
                sb.AppendFormat("<identifier_exists><![CDATA[{0}]]></identifier_exists>\n", asAscii(Sanitizer.Sanitize("no")));

                //sb.AppendFormat("<keywords><![CDATA[{0}]]></keywords>\n", asAscii(Sanitizer.Sanitize(reader["keywords"].ToString()).Replace("\n", string.Empty).Replace("\r", string.Empty).Replace("\r\n", string.Empty)));
                //sb.AppendFormat("<currency>{0}</currency>\n", reader["currency"].ToString().Replace("\n", string.Empty).Replace("\r", string.Empty).Replace("\r\n", string.Empty));
                //sb.AppendFormat("<advertisercategory><![CDATA[{0}]]></advertisercategory >\n", asAscii(Sanitizer.Sanitize(reader["advertisercategory"].ToString()).Replace("\n", string.Empty).Replace("\r", string.Empty).Replace("\r\n", string.Empty)));
                //sb.AppendFormat("<country>{0}</country>\n", reader["thirdpartyid"].ToString().Replace("\n", string.Empty).Replace("\r", string.Empty).Replace("\r\n", string.Empty));
                //sb.AppendFormat("<city><![CDATA[{0}]]></city>\n", asAscii(Sanitizer.Sanitize(reader["thirdpartycategory"].ToString()).Replace("\n", string.Empty).Replace("\r", string.Empty).Replace("\r\n", string.Empty)));
                //sb.AppendFormat("<travel_type><![CDATA[{0}]]></travel_type>\n", asAscii(Sanitizer.Sanitize("activity")));
                //sb.AppendFormat("<availability><![CDATA[{0}]]></availability>\n", asAscii(Sanitizer.Sanitize("instock")));
                sb.Append("</product>");
            }
            sb.AppendLine("\n</product_catalog_data>");
            return sb.ToString();
        }

        /// <summary>
        /// as Ascii
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        private string asAscii(string inputString)
        {
            var asAscii = Encoding.ASCII.GetString(
                Encoding.Convert(
                    Encoding.UTF8,
                    Encoding.GetEncoding(
                        Encoding.ASCII.EncodingName,
                        new EncoderReplacementFallback(string.Empty),
                        new DecoderExceptionFallback()
                        ),
                    Encoding.UTF8.GetBytes(inputString)
                )
            );
            return asAscii;
        }

        /// <summary>
        /// This method returns CJ Feed data mapping from reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public CJFeedProduct CJFeedProductMappingData(IDataReader reader)
        {
            var CJFeedProductMapping = new CJFeedProduct
            {
                id = DbPropertyHelper.StringPropertyFromRow(reader, "id"),
                name = DbPropertyHelper.StringPropertyFromRow(reader, "name"),
                producturl = DbPropertyHelper.StringPropertyFromRow(reader, "producturl"),
                smallimage = DbPropertyHelper.StringPropertyFromRow(reader, "smallimage"),
                description = DbPropertyHelper.StringPropertyFromRow(reader, "description"),
                price = DbPropertyHelper.DecimalDefaultNullablePropertyFromRow(reader, "price"),
                instock = DbPropertyHelper.Int32PropertyFromRow(reader, "instock"),
                recommendable = (int.Parse(reader["recommendable"].ToString().Trim()) == 1 ? true : false),
                categoryid1 = DbPropertyHelper.StringPropertyFromRow(reader, "categoryid_1"),
                categoryid2 = DbPropertyHelper.StringPropertyFromRow(reader, "categoryid_2"),
                categoryid3 = DbPropertyHelper.StringPropertyFromRow(reader, "categoryid_3")
            };
            return CJFeedProductMapping;
        }

        /// <summary>
        /// This method returns Tiqets Pax Mapping data from reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public TiqetsPaxMapping TiqetsPaxMappingData(IDataReader reader)
        {
            var paxMapping = new TiqetsPaxMapping
            {
                ServiceOptionId = DbPropertyHelper.Int32PropertyFromRow(reader, "serviceoptionid"),
                AgeGroupId = DbPropertyHelper.Int32PropertyFromRow(reader, "AgeGroupID"),
                AgeGroupCode = DbPropertyHelper.StringPropertyFromRow(reader, "APIAgeGroupCode"),
                PassengerType = (PassengerType)Enum.Parse(typeof(PassengerType),
                    DbPropertyHelper.StringPropertyFromRow(reader, "PassengerTypeID")),
                APIType = (APIType)Enum.Parse(typeof(APIType),
                    DbPropertyHelper.StringPropertyFromRow(reader, "APITypeID"))
            };

            return paxMapping;
        }

        public VentrataPackages VentrataPackages(IDataReader reader)
        {
            var ventrataPackages = new VentrataPackages
            {
                PackageIncludeId = DbPropertyHelper.StringPropertyFromRow(reader, "PackageIncludeId"),
                PackageIncludeOptionId = DbPropertyHelper.StringPropertyFromRow(reader, "PackageIncludeOptionId"),
                PackageIncludeProductId = DbPropertyHelper.StringPropertyFromRow(reader, "PackageIncludeProductId")
            };
            return ventrataPackages;
        }

        public TiqetsPackage TiqetsPackages(IDataReader reader)
        {
            var TiqetsPackages = new TiqetsPackage
            {
                Product_Id = DbPropertyHelper.Int32PropertyFromRow(reader, "Product_Id"),
            };
            return TiqetsPackages;
        }

        /// <summary>
        /// Load passenger mapping data
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public List<GoldenTours.PassengerMapping> GetPassengerMappingForGoldenTours(IDataReader reader)
        {
            var passengerMappings = new List<GoldenTours.PassengerMapping>();
            while (reader.Read())
            {
                var passengerMapping = new GoldenTours.PassengerMapping
                {
                    PassengerTypeId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "PASSENGERTYPEID"),
                    SupplierPassengerTypeId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "APIPASSENGERTYPEID"),
                    ApiType = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "APITypeID"),
                };
                passengerMappings.Add(passengerMapping);
            }

            return passengerMappings;
        }

        /// <summary>
        /// This method returns Geo Details Mapping data mapping from reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public GeoDetails GeoDetailsData(IDataReader reader)
        {
            var geoDetails = new GeoDetails
            {
                ContinentRegionID = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "ContinentRegionID"),
                ContinentName = DbPropertyHelper.StringPropertyFromRow(reader, "ContinentName"),
                ContinentRegionCode = DbPropertyHelper.StringPropertyFromRow(reader, "ContinentRegionCode"),
                CountryRegionID = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "CountryRegionID"),
                CountryName = DbPropertyHelper.StringPropertyFromRow(reader, "CountryName"),
                CountryRegionCode = DbPropertyHelper.StringPropertyFromRow(reader, "CountryRegionCode"),
                DestinationRegionID = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "DestinationRegionID"),
                DestinationName = DbPropertyHelper.StringPropertyFromRow(reader, "DestinationName"),
                DestinationRegionCode = DbPropertyHelper.StringPropertyFromRow(reader, "DestinationRegionCode"),
                Latitudes = DbPropertyHelper.StringPropertyFromRow(reader, "Latitudes"),
                Longitudes = DbPropertyHelper.StringPropertyFromRow(reader, "Longitudes"),
                IsCountryChange = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "IsCountryChange")
            };
            return geoDetails;
        }

        /// <summary>
        /// This method returns Geo Details Mapping data mapping from reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public Entities.Destination GeoDestinationData(IDataReader reader)
        {
            var destination = new Entities.Destination
            {
                Id = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "destinationID"),
                Name = DbPropertyHelper.StringPropertyFromRow(reader, "destinationName"),
                CountryId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "CountryID"),
                CountryName = DbPropertyHelper.StringPropertyFromRow(reader, "CountryName"),
                LanguageCode = DbPropertyHelper.StringPropertyFromRow(reader, "languagecode"),
                IsCountryChange = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "IsCountryChange"),
                Latitudes = DbPropertyHelper.StringPropertyFromRow(reader, "Latitudes"),
                Longitudes = DbPropertyHelper.StringPropertyFromRow(reader, "Longitudes"),
            };
            return destination;
        }

        /// <summary>
        /// This method returns Product Supplier Mapping data mapping from reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public ProductSupplier GeoProductSupplierData(IDataReader reader)
        {
            var productSupplier = new ProductSupplier
            {
                SupplierId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "SupplierId"),
                SupplierName = DbPropertyHelper.StringPropertyFromRow(reader, "SupplierName"),
                Supplierinfo = DbPropertyHelper.StringPropertyFromRow(reader, "Supplierinfo"),
                Link = DbPropertyHelper.StringPropertyFromRow(reader, "Link"),
                Languageid = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "Languageid")
            };
            return productSupplier;
        }

        /// <summary>
		/// This method returns Currency Mapping data mapping from reader
		/// </summary>
		/// <param name="reader"></param>
		/// <returns></returns>
		public Currency CurrencyData(IDataReader reader)
        {
            var currency = new Currency
            {
                CurrencyID = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "CurrencyID"),
                Name = DbPropertyHelper.StringPropertyFromRow(reader, "CurrencyName"),
                IsoCode = DbPropertyHelper.StringPropertyFromRow(reader, "CurrencyIsoCode"),
                Symbol = DbPropertyHelper.StringPropertyFromRow(reader, "CurrencySymbol"),
                ShortSymbol = DbPropertyHelper.StringPropertyFromRow(reader, "CurrencyShortSymbol"),
            };
            return currency;
        }

        /// <summary>
        /// This method returns Language Mapping data mapping from reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public Language LanguageData(IDataReader reader)
        {
            var language = new Language
            {
                Code = DbPropertyHelper.StringPropertyFromRow(reader, "LanguageCode"),
                Description = DbPropertyHelper.StringPropertyFromRow(reader, "LanguageName"),
            };
            return language;
        }

        /// <summary>
        /// This method returns Geo Details master Mapping data mapping from reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public GeoDetails GeoDetailsMasterData(IDataReader reader)
        {
            var geoDetails = new GeoDetails
            {
                ContinentRegionID = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "ContinentRegionID"),
                ContinentName = DbPropertyHelper.StringPropertyFromRow(reader, "ContinentName"),
                ContinentRegionCode = DbPropertyHelper.StringPropertyFromRow(reader, "ContinentRegionCode"),
                CountryRegionID = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "CountryRegionID"),
                CountryName = DbPropertyHelper.StringPropertyFromRow(reader, "CountryName"),
                CountryRegionCode = DbPropertyHelper.StringPropertyFromRow(reader, "CountryRegionCode"),
                DestinationRegionID = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "DestinationRegionID"),
                DestinationName = DbPropertyHelper.StringPropertyFromRow(reader, "DestinationName"),
                DestinationRegionCode = DbPropertyHelper.StringPropertyFromRow(reader, "DestinationRegionCode"),
                Latitudes = DbPropertyHelper.StringPropertyFromRow(reader, "Latitudes"),
                Longitudes = DbPropertyHelper.StringPropertyFromRow(reader, "Longitudes")
            };
            return geoDetails;
        }

        /// <summary>
        /// This method returns regio Wise master Mapping data mapping from reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public RegionWiseProductDetails RegionWiseMasterData(IDataReader reader)
        {
            var regionWiseProductDetails = new RegionWiseProductDetails
            {
                RegionID = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "RegionID"),
                RegionName = DbPropertyHelper.StringPropertyFromRow(reader, "RegionName"),
                ActivityID = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "ActivityID"),
                ActivityName = DbPropertyHelper.StringPropertyFromRow(reader, "ActivityName"),
                PassengerTypeID = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "PassengerTypeID"),
                FromAge = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "FromAge"),
                ToAge = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "ToAge"),
                MinSize = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "MinSize"),
                MaxSize = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "MaxSize"),
                Label = DbPropertyHelper.StringPropertyFromRow(reader, "Label"),
                PassengerTypeName = DbPropertyHelper.StringPropertyFromRow(reader, "PassengerTypeName")
            };
            return regionWiseProductDetails;
        }

        public ServiceCancellationPolicy ServiceCancellationPolicyData(IDataReader reader)
        {
            var cancellationPolicy = new ServiceCancellationPolicy
            {
                ServiceId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "serviceid"),
                CutOffDays = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "CANCELLATIONPOLICYDETAILDAYS"),
                IsFixed = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "CANCELLATIONPOLICYDETAILFIXED"),
                CancellationAmount = DbPropertyHelper.DecimalDefaultNullablePropertyFromRow(reader, "CANCELLATIONPOLICYDETAILCANCELLATIONAMOUNT")
            };
            return cancellationPolicy;
        }

        public List<RezdyLabelDetail> GetLabelDetailsMappingForRezdy(IDataReader reader)
        {
            var labelDetailsMappings = new List<RezdyLabelDetail>();
            while (reader.Read())
            {
                var passengerMapping = new RezdyLabelDetail
                {
                    Label = DbPropertyHelper.StringPropertyFromRow(reader, "label"),
                    Value = DbPropertyHelper.StringPropertyFromRow(reader, "value"),
                    ApiTypeId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "ApiTypeid"),
                };
                labelDetailsMappings.Add(passengerMapping);
            }

            return labelDetailsMappings;
        }

        public RezdyPaxMapping RezdyPaxMappingData(IDataReader reader)
        {
            var paxMapping = new RezdyPaxMapping
            {
                ServiceOptionId = DbPropertyHelper.Int32PropertyFromRow(reader, "serviceoptionid"),
                AgeGroupId = DbPropertyHelper.Int32PropertyFromRow(reader, "AgeGroupID"),
                AgeGroupCode = DbPropertyHelper.StringPropertyFromRow(reader, "APIAgeGroupCode"),
                PassengerType = (PassengerType)Enum.Parse(typeof(PassengerType),
                    DbPropertyHelper.StringPropertyFromRow(reader, "PassengerTypeID")),
                APIType = (APIType)Enum.Parse(typeof(APIType),
                    DbPropertyHelper.StringPropertyFromRow(reader, "APITypeID")),
                SupplierCode = DbPropertyHelper.StringPropertyFromRow(reader, "SupplierCode")
            };

            return paxMapping;
        }

        public TourCMSMapping TourCMSPaxMappingData(IDataReader reader)
        {
            var paxMapping = new TourCMSMapping
            {
                ServiceOptionId = DbPropertyHelper.Int32PropertyFromRow(reader, "serviceoptionid"),
                AgeGroupId = DbPropertyHelper.Int32PropertyFromRow(reader, "AgeGroupID"),
                AgeGroupCode = DbPropertyHelper.StringPropertyFromRow(reader, "APIAgeGroupCode"),
                PassengerType = (PassengerType)Enum.Parse(typeof(PassengerType),
                    DbPropertyHelper.StringPropertyFromRow(reader, "PassengerTypeID")),
                APIType = (APIType)Enum.Parse(typeof(APIType),
                    DbPropertyHelper.StringPropertyFromRow(reader, "APITypeID")),
                SupplierCode = DbPropertyHelper.StringPropertyFromRow(reader, "SupplierCode")
            };

            return paxMapping;
        }
        public GlobalTixV3Mapping GlobalTixV3PaxMappingData(IDataReader reader)
        {
            var paxMapping = new GlobalTixV3Mapping
            {
                ServiceOptionId = DbPropertyHelper.Int32PropertyFromRow(reader, "serviceoptionid"),
                AgeGroupId = DbPropertyHelper.Int32PropertyFromRow(reader, "AgeGroupID"),
                AgeGroupCode = DbPropertyHelper.StringPropertyFromRow(reader, "APIAgeGroupCode"),
                PassengerType = (PassengerType)Enum.Parse(typeof(PassengerType),
                    DbPropertyHelper.StringPropertyFromRow(reader, "PassengerTypeID")),
                APIType = (APIType)Enum.Parse(typeof(APIType),
                    DbPropertyHelper.StringPropertyFromRow(reader, "APITypeID")),
                SupplierCode = DbPropertyHelper.StringPropertyFromRow(reader, "SupplierCode")
            };

            return paxMapping;
        }

        public VentrataPaxMapping VentrataPaxMappingData(IDataReader reader)
        {
            try
            {
                var paxMapping = new VentrataPaxMapping
                {
                    ServiceOptionId = DbPropertyHelper.Int32PropertyFromRow(reader, "serviceoptionid"),
                    AgeGroupId = DbPropertyHelper.Int32PropertyFromRow(reader, "AgeGroupID"),
                    AgeGroupCode = DbPropertyHelper.StringPropertyFromRow(reader, "APIAgeGroupCode"),
                    PassengerType = (PassengerType)Enum.Parse(typeof(PassengerType),
                    DbPropertyHelper.StringPropertyFromRow(reader, "PassengerTypeID")),
                    APIType = (APIType)Enum.Parse(typeof(APIType),
                    DbPropertyHelper.StringPropertyFromRow(reader, "APITypeID")),
                    SupplierCode = DbPropertyHelper.StringPropertyFromRow(reader, "SupplierCode")
                };

                return paxMapping;
            }
            catch (Exception ex)
            {
                return new VentrataPaxMapping()
                {
                    ServiceOptionId = 0,
                    AgeGroupCode = "0",
                    AgeGroupId = 0,
                    APIType = 0,
                    PassengerType = PassengerType.Adult,
                    SupplierCode = "0"
                };
            }
        }

        public List<BlogData> MapBlogData(DataSet dataSet)
        {
            var blogs = new List<BlogData>();
            LoadBlogData(dataSet.Tables, ref blogs);
            if (blogs.Count > 0)
            {
                LoadBlogDestinations(dataSet.Tables, ref blogs);
            }
            return blogs;
        }

        private List<BlogData> LoadBlogData(DataTableCollection dataTable, ref List<BlogData> blogs)
        {
            var rows = dataTable[0].Rows;
            var rowCount = rows.Count;
            for (var i = 0; i < rowCount; i++)
            {
                var row = rows[i];

                var Blog = new BlogData();

                if (!string.IsNullOrEmpty(row["title"].ToString().Trim()))
                {
                    Blog.Title = row["title"].ToString().Trim();
                }
                if (!string.IsNullOrEmpty(row["link"].ToString().Trim()))
                {
                    Blog.Link = row["link"].ToString().Trim();
                }
                if (!string.IsNullOrEmpty(row["description"].ToString().Trim()))
                {
                    Blog.Description = row["description"].ToString().Trim();
                }
                if (!string.IsNullOrEmpty(row["publishDate"].ToString().Trim()))
                {
                    Blog.PublishedDate = Convert.ToDateTime(row["publishDate"].ToString().Trim());
                }
                if (!string.IsNullOrEmpty(row["category"].ToString().Trim()))
                {
                    Blog.Category = row["category"].ToString().Trim();
                }
                if (!string.IsNullOrEmpty(row["imagepath"].ToString().Trim()))
                {
                    Blog.ImageName = row["imagepath"].ToString().Trim();
                }
                if (!string.IsNullOrEmpty(row["BlogId"].ToString().Trim()))
                {
                    Blog.BlogId = Convert.ToInt32(row["BlogId"].ToString().Trim());
                }
                Blog.Destinations = new List<string>();
                blogs.Add(Blog);
            }
            return blogs;
        }

        private void LoadBlogDestinations(DataTableCollection dataTable, ref List<BlogData> blogs)
        {
            var rows = dataTable[1].Rows;
            var rowCount = rows.Count;
            for (var i = 0; i < rowCount; i++)
            {
                var row = rows[i];
                var blogDestination = new BlogDestination();
                if (!string.IsNullOrEmpty(row["BlogId"].ToString().Trim()))
                {
                    blogDestination.BlogId = row["BlogId"].ToString().Trim();
                }
                if (!string.IsNullOrEmpty(row["Destinationid"].ToString().Trim()))
                {
                    blogDestination.DestinationId = row["Destinationid"].ToString().Trim();
                }
                if (!string.IsNullOrEmpty(row["DestinationName"].ToString().Trim()))
                {
                    blogDestination.DestinationName = row["DestinationName"].ToString().Trim();
                }
                if (!string.IsNullOrEmpty(blogDestination.BlogId) && !string.IsNullOrEmpty(blogDestination.DestinationName))
                {
                    blogs?.FirstOrDefault(x => x.BlogId == Convert.ToInt32(blogDestination.BlogId))?.Destinations.Add(blogDestination.DestinationName.Trim());
                }
            }
        }

        public string LoadAgeDumpngAPIs(IDataReader reader)
        {
            return DbPropertyHelper.StringPropertyFromRow(reader, "literalName");
        }

        public APIImages LoadAPIImages(IDataReader reader)
        {
            APIImages aPIImages = new APIImages
            {
                ServiceID = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "serviceid"),
                SupplierProductID = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "supplierproductid"),
                City = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "City"),
                Country = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "Country"),
                Continent = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "Continent"),
                Path = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "Path"),
                APITypeID = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "apitypeid"),
                ImageURL = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "imageurl"),
                Sequence = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "sequence")
            };

            return aPIImages;
        }

        public APIImages LoadAPIImagesToDelete(IDataReader reader)
        {
            APIImages aPIImages = new APIImages
            {
                ServiceID = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "serviceid"),
                SupplierProductID = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "supplierproductid"),
                City = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "City"),
                Country = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "Country"),
                Continent = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "Continent"),
                Path = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "Path"),
                APITypeID = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "apitypeid"),
                ImageURL = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "imageurl"),
                Sequence = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "sequence"),
                PublicID = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "Cloudinary_url"),
                ID = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "ID")
            };

            return aPIImages;
        }

        public List<TourCMSChannelList> GetTourCMSChannelList(IDataReader reader)
        {
            var tourCMSChannelList = new List<TourCMSChannelList>();

            while (reader.Read())
            {
                var mapping = new TourCMSChannelList
                {
                    channelId = DbPropertyHelper.StringPropertyFromRow(reader, "channelId"),
                    accountId = DbPropertyHelper.StringPropertyFromRow(reader, "accountId"),
                    channelname = DbPropertyHelper.StringPropertyFromRow(reader, "channelname")
                };
                tourCMSChannelList.Add(mapping);
            }

            return tourCMSChannelList;
        }

        public ElasticDestinations ElasticSearchDestinations(IDataReader reader)
        {
            var elasticDestinations = new ElasticDestinations
            {
                AffiliateKey = DbPropertyHelper.StringPropertyFromRow(reader, "AffiliateKey"),
                LanguageCode = DbPropertyHelper.StringPropertyFromRow(reader, "LanguageCode"),
                Location = DbPropertyHelper.StringPropertyFromRow(reader, "Location"),
                landingkeyword = DbPropertyHelper.StringPropertyFromRow(reader, "landingkeyword"),
                landingURL = DbPropertyHelper.StringPropertyFromRow(reader, "landingURL"),
                RegionId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "RegionId"),
                BookingCount = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "BookingCount"),
                BoosterCount = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "BoosterCount"),
                IsActive = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "IsActive")
            };
            return elasticDestinations;
        }

        public ElasticProducts ElasticSearchProducts(IDataReader reader)
        {
            var elasticProducts = new ElasticProducts
            {
                AffiliateKey = DbPropertyHelper.StringPropertyFromRow(reader, "AffiliateKey"),
                LanguageCode = DbPropertyHelper.StringPropertyFromRow(reader, "LanguageCode"),
                Location = DbPropertyHelper.StringPropertyFromRow(reader, "Location"),
                URL = DbPropertyHelper.StringPropertyFromRow(reader, "URL"),
                ServiceId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "ServiceId"),
                Offer_Percent = DbPropertyHelper.DecimalDefaultNullablePropertyFromRow(reader, "Offer_Percent"),
                RegionId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "RegionId"),
                BookingCount = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "BookingCount"),
                BoostingCount = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "BoostingCount"),
                IsActive = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "IsActive"),
                ImageURL = DbPropertyHelper.StringPropertyFromRow(reader, "ImageURL"),
                Serviceprice = DbPropertyHelper.DecimalDefaultNullablePropertyFromRow(reader, "Serviceprice"),
                Currencycode = DbPropertyHelper.StringPropertyFromRow(reader, "Currencycode"),
            };
            var regionName = DbPropertyHelper.StringPropertyFromRow(reader, "RegionName");
            var serviceName = DbPropertyHelper.StringPropertyFromRow(reader, "ServiceName");
            elasticProducts.RegionName = regionName;
            elasticProducts.ServiceName = serviceName;
            elasticProducts.ServiceNameRegionName = regionName + " " + serviceName;
            return elasticProducts;
        }

        public ElasticAttractions ElasticSearchAttractions(IDataReader reader)
        {
            var elasticAttractions = new ElasticAttractions
            {
                RegionId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "RegionId"),
                RegionName = DbPropertyHelper.StringPropertyFromRow(reader, "RegionName"),
                Categoryid = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "Categoryid"),
                CategoryName = DbPropertyHelper.StringPropertyFromRow(reader, "CategoryName"),

                AffiliateKey = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "AffiliateKey"),
                LanguageCode = DbPropertyHelper.StringPropertyFromRow(reader, "LanguageCode"),
                URL = DbPropertyHelper.StringPropertyFromRow(reader, "URL"),
                BoostingCount = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "BoostingCount"),

                BookingCount = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "BookingCount"),
                Location = DbPropertyHelper.StringPropertyFromRow(reader, "Location"),
                IsActive = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "IsActive"),
                AType = DbPropertyHelper.StringPropertyFromRow(reader, "AType"),
            };
            return elasticAttractions;
        }

        public AvailablePersonTypes ReadPersonTypeOptionCacheAvailability(IDataReader reader)
        {
            var results = new AvailablePersonTypes
            {
                AvailablePassengerTypes = new List<PersonTypeOptionDateRanges>(),
                AvailableDates = new List<PersonTypeOptionCacheAvailability>(),
            };

            while (reader.Read())
            {
                try
                {
                    var result = new PersonTypeOptionDateRanges
                    {
                        FromDate = DbPropertyHelper.DateTimeNullablePropertyFromRow(reader, "FromDate"),
                        ToDate = DbPropertyHelper.DateTimeNullablePropertyFromRow(reader, "ToDate"),
                        PassengerTypeID = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "PassengerTypeID"),
                        PassengerTypename = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "PassengerTypename"),
                        MeasurementDesc = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "MeasurementDesc"),
                        MaxSize = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "MaxSize"),
                        MinSize = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "MinSize"),
                        FromAge = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "FromAge"),
                        ToAge = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "ToAge"),
                    };
                    results.AvailablePassengerTypes.Add(result);
                }
                catch
                {
                }
            }
            if (reader.NextResult())
            {
                while (reader.Read())
                {
                    try
                    {
                        var result = new PersonTypeOptionCacheAvailability
                        {
                            ServiceId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "serviceId"),
                            ServiceOptionId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "serviceOptionId"),
                            Capacity = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "capacity"),
                            AvailableOn = DbPropertyHelper.DateTimeNullablePropertyFromRow(reader, "availableOn"),
                        };
                        results.AvailableDates.Add(result);
                    }
                    catch
                    {
                    }
                }
            }

            return results;
        }

        public ElasticAffiliate ElasticAffiliate(IDataReader reader)
        {
            var elasticAffiliate = new ElasticAffiliate
            {
                AffiliateKey = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "AffiliateKey"),
            };
            return elasticAffiliate;
        }
        /* public AffiliateDetails AffiliateDetail(IDataReader reader)
         {
             var affiliateDetail = new AffiliateDetails
             {
                 AffiliateKey = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "AffiliateKey"),
                 AffiliateID= DbPropertyHelper.StringPropertyFromRow(reader, "AffiliateID"),
                 AffiliateName = DbPropertyHelper.StringPropertyFromRow(reader, "AffiliateName"),

             };
             return affiliateDetail;
         }*/

        public List<CssBookingDatas> GetCssBookingDatas(IDataReader reader)
        {
            List<CssBookingDatas> result = new List<CssBookingDatas>();

            while (reader.Read())
            {
                CssBookingDatas cssBookingData = new CssBookingDatas
                {
                    BOOKEDOPTIONID = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "BOOKEDOPTIONID"),
                    LeadPassengerName = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "LeadPassengerName"),
                    languagecode = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "languagecode"),
                    voucheremail = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "voucheremail"),
                    bookingreferencenumber = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "bookingreferencenumber"),
                    affiliateid = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "affiliateid"),
                    Bookingdate = DbPropertyHelper.DateTimePropertyFromRow(reader, "Bookingdate"),
                    Traveldate = DbPropertyHelper.DateTimePropertyFromRow(reader, "TravelDate"),
                    serviceoptioninserviceid = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "serviceoptioninserviceid"),
                    CssProductId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "CssProductId"),
                    CssProductOptionId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "CssProductOptionId"),
                    SupplierId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "supplierId"),
                    optiontimeslot = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "optiontimeslot"),
                    OTAReferenceId = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "OTAReferenceId")
                };

                result.Add(cssBookingData);
            }

            return result;
        }


        public List<CssPassengerDetails> GetCssPassengerDetail(IDataReader reader)
        {
            var passengers = new List<CssPassengerDetails>();

            while (reader.Read())
            {
                CssPassengerDetails passenger = new CssPassengerDetails
                {
                    BOOKEDOPTIONID = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "BOOKEDOPTIONID"),
                    PASSENGERTYPEID = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "PASSENGERTYPEID"),
                    PaxCount = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "PaxCount"),
                    QRCODECOUNT = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "QRCODECOUNT"),
                    IsPerPaxQRCode = DbPropertyHelper.BoolPropertyFromRow(reader, "IsPerPaxQRCode")
                };

                passengers.Add(passenger);

            }
            return passengers;
        }

        public List<CssQrCode> GetCssQRCode(IDataReader reader)
        {
            var QRCodes = new List<CssQrCode>();

            while (reader.Read())
            {
                CssQrCode QRCode = new CssQrCode
                {
                    BOOKEDOPTIONID = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "BOOKEDOPTIONID"),
                    PASSENGERTYPEID = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "PASSENGERTYPEID"),
                    QRCODE = DbPropertyHelper.StringPropertyFromRow(reader, "QRCODE")
                };

                QRCodes.Add(QRCode);

            }
            return QRCodes;
        }
    }
}
