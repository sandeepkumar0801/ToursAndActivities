using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.GlobalTixV3;
using ServiceAdapters.GlobalTixV3.GlobalTix.Entities;
using ServiceAdapters.GlobalTixV3.GlobalTix.Entities.RequestResponseModels;
using ServiceAdapters.GlobalTixV3.GlobalTix.Entities.RequestResponseModels.Categories;
using ServiceAdapters.GlobalTixV3.GlobalTix.Entities.RequestResponseModels.Cities;
using ServiceAdapters.GlobalTixV3.GlobalTix.Entities.RequestResponseModels.Countries;
using ServiceAdapters.GlobalTixV3.GlobalTix.Entities.RequestResponseModels.PackageOptions;
using ServiceAdapters.GlobalTixV3.GlobalTix.Entities.RequestResponseModels.ProductChanges;
using ServiceAdapters.GlobalTixV3.GlobalTix.Entities.RequestResponseModels.ProductInfo;
using ServiceAdapters.GlobalTixV3.GlobalTixV3.Commands.Contracts;
using ServiceAdapters.GlobalTixV3.GlobalTixV3.Converters.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Util;
using AgentAuthenticateDetails = ServiceAdapters.GlobalTixV3.GlobalTix.Entities.AgentAuthenticateDetails;
using InputContext = ServiceAdapters.GlobalTixV3.GlobalTix.Entities.InputContext;
using packageData = ServiceAdapters.GlobalTixV3.GlobalTix.Entities.RequestResponseModels.PackageOptions.PackageOptionsList;

namespace ServiceAdapters.GlobalTixV3
{
    class GlobalTixAdapterV3 : IGlobalTixAdapterV3, IAdapter
    {
        private ICityListCommandHandler _cityListCommandHandler;
        private ICountryListCommandHandler _countryListCommandHandler;
        private IProductListCommandHandler _productListCommandHandler;
        private IProductOptionCommandHandler _productOptionCommandHandler;
        private ICategoriesCommandHandler _categoriesCommandHandler;
        private IProductInfoCommandHandler _productInfoCommandHandler;
        private IProductChangesCommandHandler _productChangesCommandHandler;
        private IPackageOptionsCommandHandler _PackageOptionsCommandHandler;
        private IAvailabilitySeriesCommandHandler _availabilitySeriesCommandHandler;

        private ICancelReleaseCommandHandler _cancelReleaseCommandHandler;


        private readonly int _maxParallelThreadCount;
        private readonly IActivityInfoConverter _activityInfoConverter;

        private IReservationCommandHandler _iReservationCommandHandler;
        private IReservationConverter _iReservationConverter;

        private IBookingCommandHandler _iBookingCommandHandler;
        private IBookingConverter _iBookingConverter;
        private IBookingDetailCommandHandler _iBookingDetailCommandHandler;

        private IPackageOptionsConverter _iPackageOptionsConverter;

        public GlobalTixAdapterV3(ICityListCommandHandler cityListCommandHandler,
           ICountryListCommandHandler countryListCommandHandler, IProductListCommandHandler productListCommandHandler, IProductOptionCommandHandler productOptionCommandHandler,
           ICategoriesCommandHandler categoriesCommandHandler, IProductInfoCommandHandler productInfoCommandHandler,
           IAvailabilitySeriesCommandHandler availabilitySeriesCommandHandler, IActivityInfoConverter activityInfoConverter, IProductChangesCommandHandler productChangesCommandHandler,
            IPackageOptionsCommandHandler packageOptionsCommandHandler, IReservationCommandHandler iReservationCommandHandler,
           IReservationConverter iReservationConverter,
           IBookingCommandHandler iBookingCommandHandler,
           IBookingConverter iBookingConverter,
           IBookingDetailCommandHandler iBookingDetailCommandHandler,
           IPackageOptionsConverter iPackageOptionsConverter,
           ICancelReleaseCommandHandler cancelReleaseCommandHandler)
        {

            _cityListCommandHandler = cityListCommandHandler;
            _countryListCommandHandler = countryListCommandHandler;
            _productListCommandHandler = productListCommandHandler;
            _productOptionCommandHandler = productOptionCommandHandler;
            _categoriesCommandHandler = categoriesCommandHandler;
            _productInfoCommandHandler = productInfoCommandHandler;
            _availabilitySeriesCommandHandler = availabilitySeriesCommandHandler;
            _maxParallelThreadCount = MaxParallelThreadCountHelper.GetMaxParallelThreadCount("MaxParallelThreadCount");
            _activityInfoConverter = activityInfoConverter;
            _productChangesCommandHandler = productChangesCommandHandler;
            _PackageOptionsCommandHandler = packageOptionsCommandHandler;
            _iReservationCommandHandler = iReservationCommandHandler;
            _iReservationConverter = iReservationConverter;
            _iBookingCommandHandler = iBookingCommandHandler;
            _iBookingConverter = iBookingConverter;
            _iBookingDetailCommandHandler = iBookingDetailCommandHandler;
            _iPackageOptionsConverter = iPackageOptionsConverter;
            _cancelReleaseCommandHandler = cancelReleaseCommandHandler;
        }

        public List<CountryCityV3> GetCountryCityListV3(string token)
        {
            if (AgentAuthenticateDetails.Instance(true).IsAuthenticated == false)
            {
                return null;
            }

            var countryListRSRaw = _countryListCommandHandler.Execute(null, token);
            CountriesList countryListRS = SerializeDeSerializeHelper.DeSerialize<CountriesList>(countryListRSRaw.ToString());
            if (countryListRS == null && countryListRS.Success == false)
            {
                return null;
            }

            Dictionary<int, string> countryIdToName = new Dictionary<int, string>();
            foreach (var data in countryListRS.Data)
            {
                int countryId = data.Id;
                string countryName = data.Name;
                countryIdToName.Add(countryId, countryName);
            }


            var cityListRSRaw = _cityListCommandHandler.Execute(null, token);
            CitiesList cityListRS = SerializeDeSerializeHelper.DeSerialize<CitiesList>(cityListRSRaw.ToString());
            if (cityListRS == null && cityListRS.Success == false)
            {
                return null;
            }

            var countryCities = new List<CountryCityV3>();
            foreach (var city in cityListRS.Data)
            {
                string countryName = countryIdToName[city.CountryId] ?? "";
                countryCities.Add(
                    new CountryCityV3()
                    {
                        CountryId = city.CountryId.ToString(),
                        CountryName = countryIdToName[city.CountryId] ?? "",
                        CityId = city.Id.ToString(),
                        CityName = city.Name
                    }
                );
            }
            return countryCities;
        }


        public async Task<List<ProductList>> GetAllActivitiesV3Async(int country, string token,
            bool isNonThailandProduct)
        {
            if (AgentAuthenticateDetails.Instance(isNonThailandProduct).IsAuthenticated == false)
            {
                return null;
            }

            var availCtx = new ActivityInfoInputContext()
            {
                countryid = country,
				PageNumber=1,
                MethodType = MethodType.ProductList,
                isNonThailandProduct= isNonThailandProduct
            };

            bool hasMorePages = false;

            var  allActivities = new List<ProductList>();
            do
            {
                var actListRSRaw = _productListCommandHandler.Execute(availCtx, token);

                if (actListRSRaw == null)
                {
                    break;
                }

                GlobalTix.Entities.RequestResponseModels.ProductList.ProductList actListRS = SerializeDeSerializeHelper.DeSerialize<GlobalTix.Entities.RequestResponseModels.ProductList.ProductList>(actListRSRaw.ToString());

                var dataCollection = actListRS.Data;
                if (dataCollection.Count() == 0)
                {
                    break;
                }

                foreach (var productList in actListRS.Data)
                {
                    try
                    {
                        allActivities.Add(
                            new Isango.Entities.GlobalTixV3.ProductList()
                            {
                                Id = productList.Id,
                                Name = productList.Name,
                                Currency = productList.Currency,
                                City = productList.City,
                                OriginalPrice = productList.OriginalPrice,
                                Country = productList.Country,
                                IsOpenDated = productList.IsOpenDated,
                                IsOwnContracted = productList.IsOwnContracted,
                                IsFavorited = productList.IsFavorited,
                                IsBestSeller = productList.IsBestSeller,
                                IsInstantConfirmation = productList.IsInstantConfirmation,
                                Category = productList.Category,

                            }
                        );
                    }
                    catch(Exception e) { }

                }
				hasMorePages = (allActivities.Count < Convert.ToInt32(actListRS.Size));
				if (hasMorePages)
				{
					availCtx.PageNumber++;
				}
				
			}
            while (hasMorePages);

            return allActivities;
        }



        public List<ProductInfoV3> GetProductInfoV3(int id, string token, bool isNonThailandProduct)
        {
            if (AgentAuthenticateDetails.Instance(isNonThailandProduct).IsAuthenticated == false)
            {
                return null;
            }

            var availCtx = new ActivityInfoInputContext()
            {
                id = id,
                MethodType = MethodType.ProductInfo,
                isNonThailandProduct = isNonThailandProduct
            };

            var actProductListRSRaw = _productInfoCommandHandler.Execute(availCtx, token);
            ProductInfoList productListRS = SerializeDeSerializeHelper.DeSerialize<ProductInfoList>(actProductListRSRaw.ToString());

           var ProductInfo = new List<ProductInfoV3>();

            ProductInfo.Add(
                new ProductInfoV3()
                {
                    country = productListRS.data.Country,
                    originalPrice = productListRS.data.OriginalPrice,
                    blockedDate = productListRS.data.BlockedDate,
                    fromPrice = productListRS.data.FromPrice,
                    city = productListRS.data.City,
                    description = productListRS.data.Description,
                    countryId = productListRS.data.CountryId,
                    currency = productListRS.data.Currency,
                    id = productListRS.data.Id,
                    isGTRecommend = productListRS.data.IsGTRecommend,
                    isOpenDated = productListRS.data.IsOpenDated,
                    isOwnContracted = productListRS.data.IsOwnContracted,
                    isFavorited = productListRS.data.IsFavorited,
                    isBestSeller = productListRS.data.IsBestSeller,
                    fromReseller = productListRS.data.FromReseller,
                    name = productListRS.data.Name,
                    isInstantConfirmation = productListRS.data.IsInstantConfirmation,
                    location = productListRS.data.Location,
                    category = productListRS.data.Category
                }
            );

            return ProductInfo;
        }

        public List<GlobalTixCategoriesV3> GetCategoriesListV3(string token)
        {
            if (AgentAuthenticateDetails.Instance(true).IsAuthenticated == false)
            {
                return null;
            }

            var CategoriesListRSRaw = _categoriesCommandHandler.Execute(null, token);
            GlobalTixV3Categories CategoriesListRS = SerializeDeSerializeHelper.DeSerialize<GlobalTixV3Categories>(CategoriesListRSRaw.ToString());
            if (CategoriesListRS == null && CategoriesListRS.Success == false)
            {
                return null;
            }
            List<GlobalTixCategoriesV3> Categories = new List<GlobalTixCategoriesV3>();
            foreach (var category in CategoriesListRS.Data)
            {
                Categories.
                    Add(
                        new GlobalTixCategoriesV3()
                        {
                            Id = category.Id,
                            Name = category.Name
                        }
                        );
            }
            return Categories;
        }

        public Tuple<List<ProductOptionV3>, List<Tickettype>> GetProductOptionV3(string token,
            int productId, bool isNonThailandProduct)
        {
            var availCtx = new ActivityInfoInputContext()
            {
                id = productId,
                MethodType=MethodType.ProductOptions,
                isNonThailandProduct= isNonThailandProduct
            };

            bool hasMorePages = false;

            var allActivities = new List<ProductOptionV3>();
            var allTicketTypes = new List<Tickettype>();
            do
            {
                var actListRSRaw = _productOptionCommandHandler.Execute(availCtx, token);

                if (actListRSRaw == null)
                {
                    break;
                }

                GlobalTix.Entities.RequestResponseModels.ProductOption.ProductOption productOption = SerializeDeSerializeHelper.DeSerialize<GlobalTix.Entities.RequestResponseModels.ProductOption.ProductOption>(actListRSRaw.ToString());

                if (productOption == null || productOption.Success == false)
                {
                    break;
                }
                var packageTypeOptions = new List<Tickettype>();

                foreach (var productList in productOption.Datum)
                {
                    ProductOptionV3 productOptionV3 = new ProductOptionV3()
                    {
                        Id = productId,
                        Name = productList.Name,
                        Description = productList.Description,
                        Currency = productList.Currency,
                        TicketValidity = productList.TicketValidity,
                        TicketFormat = productList.TicketFormat,
                        IsCapacity = productList.IsCapacity,
                        TimeSlot = productList.TimeSlot,
                        IsCancellable = productList.IsCancellable,
                        Type = productList.Type,
                        DemandType = productList.DemandType,
                        OptionId= productList.Id,
                     };

                   
                        foreach (var ticketType in productList.TicketType)
                        {
                            //ServiceAdapters.GlobalTixV3.GlobalTix.Entities.RequestResponseModels.ProductOption.Tickettype ticketType = productList.TicketType[i];
                            packageTypeOptions.Add(
                                new Tickettype()
                                {
                                    ParentProductOptionId = ticketType.Id,//ticketType.Id,9183
                                    TicketType_Id = productId,
                                    Name = ticketType.Name,
                                    Sku = ticketType.Sku,
                                    AgeFrom = ticketType.AgeFrom,
                                    AgeTo = ticketType.AgeTo,
                                    NettPrice = ticketType.NettPrice,
                                    MinimumMerchantSellingPrice = ticketType.MinimumMerchantSellingPrice,
                                    MinimumSellingPrice = ticketType.MinimumSellingPrice,
                                    RecommendedSellingPrice = ticketType.RecommendedSellingPrice,
                                    OriginalPrice = ticketType.OriginalPrice,
                                    OriginalMerchantPrice = ticketType.OriginalMerchantPrice,
                                    MinPurchaseQty = ticketType.MinPurchaseQty,
                                    MaxPurchaseQty = ticketType.MinPurchaseQty,
                                    MerchantReference = ticketType.MerchantReference,
                                    NettMerchantPrice = ticketType.NettMerchantPrice,
                                    OptionId= productList.Id,  //productList.Id 13914
                                });

                        }  // Assign the value to the array element
                    
                    allActivities.Add(productOptionV3);
                    allTicketTypes.AddRange(packageTypeOptions);
                }

            }
            while (hasMorePages);

            return new Tuple<List<ProductOptionV3>, List<Tickettype>>(allActivities, allTicketTypes);
        }


        public Activity GetActivityInformation(CanocalizationCriteria gtCriteria, string token, bool isNonThailandProduct)
        {
            if (AgentAuthenticateDetails.Instance(isNonThailandProduct).IsAuthenticated)
            {
                Activity actInfo = GetActivityInfo(gtCriteria, token, isNonThailandProduct);
                return actInfo ?? GetPackageInfo(gtCriteria, token, isNonThailandProduct);
            }

            return null;
        }
        private Activity GetActivityInfo(CanocalizationCriteria gtCriteria, string token, bool isNonThailandProduct)
        {
            var inputContext = new ActivityInfoInputContext
            {
                MethodType = MethodType.ProductInfo,
                ServiceOptionID = gtCriteria?.ServiceOptionID ?? 0,
                ActivityId = gtCriteria.ActivityIdStr,
                FactSheetId = gtCriteria.FactSheetId,
                Days2Fetch = gtCriteria.Days2Fetch,
                NoOfPassengers = gtCriteria.NoOfPassengers,
                CheckinDate = gtCriteria.CheckinDate,
                id = gtCriteria.FactSheetId,
                GlobalTixV3Mapping= gtCriteria.GlobalTixV3Mapping,
                isNonThailandProduct= isNonThailandProduct
            };

            

            // 1 Step.) api ProductInfo Call  by passing productid
            var apiProductInfo = _productInfoCommandHandler.Execute(inputContext, token);
            if (apiProductInfo == null)
            {
                return null;
            }
            ProductInfoResponse productInfoRS = SerializeDeSerializeHelper.DeSerialize<ProductInfoResponse>(apiProductInfo.ToString());
            if (productInfoRS == null || productInfoRS.Success == false)
            {
                return null;
            }

            //// 2nd Step: api Product options call by passing productid
            inputContext.MethodType = MethodType.ProductOptions;
            var apiProductOptionsRSRaw = _productOptionCommandHandler.Execute(inputContext, token);
            if (apiProductOptionsRSRaw == null)
            {
                return null;
            }
            var apiProductOptionRS = SerializeDeSerializeHelper.DeSerialize<GlobalTix.Entities.RequestResponseModels.ProductOption.ProductOption>(apiProductOptionsRSRaw.ToString());
            if (apiProductOptionRS?.Datum == null || apiProductOptionRS?.Success == false)
            {
                return null;
            }
            //Remove not mapped options
            apiProductOptionRS.Datum.RemoveAll(x => !inputContext.ActivityId.Contains(x.Id.ToString()));

            
            //PAX types:ADULT,CHILD,Per Pax,pax,
            var dataGet = apiProductOptionRS?.Datum;

            var useAPIselectedPax = gtCriteria.GlobalTixV3Mapping.Where(x=> gtCriteria.NoOfPassengers.Keys.ToList().Contains(x.PassengerType))?.Select(x=>x.AgeGroupCode)?.ToList();


            var stringData = string.Join(",", useAPIselectedPax);
           
             var stringLowerData = stringData?.ToLower();
            
            dataGet.ForEach(u => {
                u.TicketType.RemoveAll(a => !stringLowerData.Contains(a.Name.ToLower()));
            });

            var gtPaxIdsToGetDetailsFor = dataGet?.ToList()?.SelectMany(x => x.TicketType)?.Select(x => x.Id)?.Distinct()?.ToList();
            if (gtPaxIdsToGetDetailsFor == null || gtPaxIdsToGetDetailsFor.Count == 0)
            {
                return null;
            }

            ////3rd Step:call schedule events, availability endpoint by passing tickettypeid and todate, from date
            var listOfGTPaxTypeDetailsForPricingUnitCreation = new List<ServiceAdapters.GlobalTixV3.GlobalTix.Entities.RequestResponseModels.DatumAvailability>();
            //Parallel.ForEach(gtPaxIdsToGetDetailsFor, new ParallelOptions { MaxDegreeOfParallelism = _maxParallelThreadCount }, (paxId) =>
            foreach (var paxId in gtPaxIdsToGetDetailsFor)
            {

                var inCtx = new ActivityInfoInputContext
                {
                    MethodType = MethodType.Availability,
                    TicketType = paxId.ToString(),
                    CheckinDate = gtCriteria.CheckinDate,
                    CheckOutDate = gtCriteria.CheckinDate.AddDays(inputContext.Days2Fetch + 1),
                    isNonThailandProduct = isNonThailandProduct
                 }; 

                
                var ticketTypeRSRaw = _availabilitySeriesCommandHandler.Execute(inCtx, token);
                if (ticketTypeRSRaw != null)
                {
                    var ticketTypeRS = SerializeDeSerializeHelper.DeSerialize<ServiceAdapters.GlobalTixV3.GlobalTix.Entities.RequestResponseModels.AvailabilitySeriesResponse>(ticketTypeRSRaw.ToString());
                    if (ticketTypeRS?.DataAvailability != null && ticketTypeRS.Success == true)
                    {
                        ticketTypeRS.DataAvailability.ToList().ForEach(cc => cc.ticketTypeID = paxId);
                        listOfGTPaxTypeDetailsForPricingUnitCreation.AddRange(ticketTypeRS.DataAvailability.ToList());
                    }
                }
            }
            //);

            inputContext.ActivityInfo = productInfoRS.Data; //ProductInfo
            inputContext.PaxTypeDetails = listOfGTPaxTypeDetailsForPricingUnitCreation;//Availability
            inputContext.ProductOption = apiProductOptionRS.Datum;//Product Options
            return (Activity)_activityInfoConverter.Convert(inputContext);
        }

        private Activity GetPackageInfo(CanocalizationCriteria gtCriteria, string token, bool isNonThailandProduct)
        {

            var inputContext = new PackageInfoInputContext
            {
                MethodType = MethodType.PackageAvailability,
                ServiceOptionID = gtCriteria.ServiceOptionID ?? 0,
                PackageId = gtCriteria.ActivityIdStr,
                FactSheetId = gtCriteria.FactSheetId,
                Days2Fetch = gtCriteria.Days2Fetch,
                CheckinDate = gtCriteria.CheckinDate,
                id= gtCriteria.FactSheetId,
                GlobalTixV3Mapping = gtCriteria.GlobalTixV3Mapping,
                isNonThailandProduct= isNonThailandProduct
            };

            // Call ActivityInfo command handler to invoke supplier API
            var packagesRSRaw = _PackageOptionsCommandHandler.Execute(inputContext, token);
            if (packagesRSRaw == null)
            {
                return null;
            }
            var packagesRSAll = SerializeDeSerializeHelper.DeSerialize<packageData>(packagesRSRaw.ToString());
            var packagesRS = packagesRSAll.data;

            var useAPIselectedPax = gtCriteria.GlobalTixV3Mapping.Where(x => gtCriteria.NoOfPassengers.Keys.ToList().Contains(x.PassengerType))?.Select(x => x.AgeGroupCode)?.ToList();
            var stringData = string.Join(",", useAPIselectedPax);
            var stringLowerData = stringData?.ToLower();
            packagesRS.ForEach(u => {
                u.PackageType.RemoveAll(a => !stringLowerData.Contains(a.PackageTypeName.ToLower()));
            });

            inputContext.LinkedPackages = packagesRS.ToList()?.FirstOrDefault();
            inputContext.TicketTypes = null;
            return (Activity)_iPackageOptionsConverter.Convert(inputContext);
        }


        public List<ProductChangesV3> GetProductChangesV3(string token, int Countryid, bool isNonThailandProduct)
        {
            if (AgentAuthenticateDetails.Instance(isNonThailandProduct).IsAuthenticated == false)
            {
                return null;
            }
            DateTime currentDate = DateTime.Now;
            DateTime sevenDaysAgo = currentDate.AddDays(-7);
            var availCtx = new ActivityInfoInputContext()
            {

                countryid = Countryid,
                dateFrom = sevenDaysAgo,
                dateTo = currentDate,
                MethodType = MethodType.ProductChanges,
                isNonThailandProduct= isNonThailandProduct
            };

            var actProductChangesRSRaw = _productChangesCommandHandler.Execute(availCtx, token);
            ProductChangesList productChangesRS = SerializeDeSerializeHelper.DeSerialize<ProductChangesList>(actProductChangesRSRaw.ToString());

            if (productChangesRS == null || productChangesRS.Success == false)
            {
                return null;
            }
            List<ProductChangesV3> ProductChanges = new List<ProductChangesV3>();
            foreach (var product in productChangesRS.Data)
            {
                ProductChanges.Add(
                        new ProductChangesV3()
                        {
                            id = product.Id,
                            name = product.Name,
                            cityId = product.CityId,
                            countryId = product.CountryId,
                            lastUpdated = product.LastUpdated
                        }
                   );
            }
            return ProductChanges;
        }

        public Tuple<List<PackageOptionsV3>, List<PackageOptionsV3.PackageType>> GetPackageOptionsV3(string token, int? id)
        {

            if (AgentAuthenticateDetails.Instance(true).IsAuthenticated == false)
            {
                return null;
            }

            InputContext availCtx = new InputContext()
            {
                id = id ?? 0,
                MethodType= MethodType.PackageOptions
            };

            var actPackageOptionsRSRaw = _PackageOptionsCommandHandler.Execute(availCtx, token);
            PackageOptionsList PackageOptionsRS = SerializeDeSerializeHelper.DeSerialize<PackageOptionsList>(actPackageOptionsRSRaw.ToString());
            if (PackageOptionsRS == null || PackageOptionsRS.Success == false)
            {
                return null;
            }
            List<PackageOptionsV3> packageOptions = new List<PackageOptionsV3>();

            List<Isango.Entities.GlobalTixV3.PackageOptionsV3.PackageType> packageTypeOptions = new List<Isango.Entities.GlobalTixV3.PackageOptionsV3.PackageType>();

            foreach (var product in PackageOptionsRS.data)
            {
                packageOptions.Add(
                        new PackageOptionsV3()
                        {
                            id = product.Id,
                            name = product.Name,
                            image = product.Image,
                            description = product.Description,
                            termsAndConditions = product.TermsAndConditions,
                            currency = product.Currency,
                            publishStart = product.PublishStart,
                            publishEnd = product.PublishEnd,
                            redeemStart = product.RedeemStart,
                            redeemEnd = product.RedeemEnd,
                            ticketValidity = product.TicketValidity,
                            ticketFormat = product.TicketFormat,
                            definedDuration = product.DefinedDuration,
                            isFavorited = product.IsFavorited,
                            fromReseller = product.FromReseller,
                            sourceName = product.SourceName,
                            sourceTitle = product.SourceTitle,
                            isAdditionalBookingInfo = product.IsAdditionalBookingInfo


                        }
                        );

            }




            foreach (var packagetype in PackageOptionsRS.data)
            {
                InputContext availCtxs = new InputContext()
                {
                    id = packagetype.Id,
                    MethodType = MethodType.PackageOptions

                };

                var actPackageOptionsIdRSRaw = _PackageOptionsCommandHandler.Execute(availCtxs, token);
                PackageOptionsList PackageOptionsIdRS = SerializeDeSerializeHelper.DeSerialize<PackageOptionsList>(actPackageOptionsIdRSRaw.ToString());
				if (PackageOptionsIdRS == null || PackageOptionsIdRS.Success == false)
				{
					return null;
				}
				foreach (var product in PackageOptionsIdRS.data)
                {
                    foreach (var productType in product.PackageType)
                    {
                        packageTypeOptions.Add(
                                new Isango.Entities.GlobalTixV3.PackageOptionsV3.PackageType()
                                {
                                    PackagetypeparentId = packagetype.Id,
                                    PackageType_id = productType.PackageTypeId,
                                    PackageType_sku = productType.PackageTypeSku,
                                    PackageType_name = productType.PackageTypeName,
                                    PackageType_nettPrice = productType.PackageTypeNettPrice,
                                    PackageType_settlementRate = productType.PackageTypeSettlementRate,
                                    PackageType_originalPrice = productType.PackageTypeOriginalPrice,
                                    PackageType_issuanceLimit = productType.PackageTypeIssuanceLimit
                                }
                        );
                    }



                }


                //return new Tuple<List<PackageOptionsV3>, List<PackageOptionsV3.PackageType>>(packageOptions, packageTypeOptions);
            }

            return new Tuple<List<PackageOptionsV3>, List<PackageOptionsV3.PackageType>>(packageOptions, packageTypeOptions);


        }


        public ReservationRS CreateReservation(SelectedProduct selectedProduct, string bookingReference,
            string token, out string requestJson, out string responseJson, out HttpStatusCode httpStatusCode)
        {
            var selectedProducts = new List<SelectedProduct>
            {
                selectedProduct
            };
            var reservationRS = new ReservationRS();
            requestJson = string.Empty;
            responseJson = string.Empty;
            httpStatusCode = HttpStatusCode.BadGateway;

            //Code area for separating Thailand and NonThailand products as well as hitting api end point with region based credentials.
            var thailandProducts = new List<SelectedProduct>();
            var nonThailandProducts = new List<SelectedProduct>();

            bool isNonThailandProduct;

            thailandProducts = selectedProducts.FindAll(thisProduct => thisProduct.Regions?.Where(x => x.Type == Isango.Entities.Enums.RegionType.Country)?.FirstOrDefault()?.Name?.ToLower() == "thailand")?.ToList();
            nonThailandProducts = selectedProducts.FindAll(thisProduct => thisProduct.Regions?.Where(x => x.Type == Isango.Entities.Enums.RegionType.Country)?.FirstOrDefault()?.Name?.ToLower() != "thailand")?.ToList();

            if (thailandProducts != null && thailandProducts.Count > 0)
            {
                isNonThailandProduct = false;
                if (AgentAuthenticateDetails.Instance(isNonThailandProduct).IsAuthenticated)
                {
                    reservationRS = (ReservationRS)GetReservationResponse(selectedProducts, bookingReference, token, isNonThailandProduct, out requestJson, out responseJson, out  httpStatusCode);
                }
            }
            if (nonThailandProducts != null && nonThailandProducts.Count > 0)
            {
                isNonThailandProduct = true;
                if (AgentAuthenticateDetails.Instance(isNonThailandProduct).IsAuthenticated)
                {
                    reservationRS = (ReservationRS)GetReservationResponse(selectedProducts, bookingReference, token, isNonThailandProduct, out requestJson, out responseJson, out  httpStatusCode);
                }
            }

            //Logic to create final booking response from Thailand and NonThailand product bookings
            if (reservationRS != null && reservationRS.Success==true)
            {
                return (ReservationRS)reservationRS;
            }
            return null;
        }

        private object GetReservationResponse(List<SelectedProduct> selectedProducts, string bookingReference, 
            string token,bool isNonThailandProduct, out string requestJson, out string responseJson, out HttpStatusCode httpStatusCode)
        {
            requestJson = string.Empty;
            responseJson = string.Empty;
            var inputContext = new BookInputContextV3
            {
                MethodType = MethodType.Reservation,
                SelectedProducts = selectedProducts,
                BookingReferenceNumber = bookingReference,
                isNonThailandProduct= isNonThailandProduct
            };
            var response = _iReservationCommandHandler.Execute(inputContext, token, out requestJson, out responseJson, out  httpStatusCode);
            var reservationRS = SerializeDeSerializeHelper.DeSerialize<ReservationRS>(response.ToString());
            return reservationRS;
        }


        public SelectedProduct CreateBooking(SelectedProduct selectedProduct, string bookingReference,
            string token, out string requestJson, out string responseJson, out HttpStatusCode httpStatusCode)
        {
            var selectedProducts = new List<SelectedProduct>
            {
                selectedProduct
            };
            var selectedRS = new SelectedProduct();
            requestJson = string.Empty;
            responseJson = string.Empty;
            httpStatusCode = HttpStatusCode.BadGateway;

            //Code area for separating Thailand and NonThailand products as well as hitting api end point with region based credentials.
            var thailandProducts = new List<SelectedProduct>();
            var nonThailandProducts = new List<SelectedProduct>();

            bool isNonThailandProduct;

            thailandProducts = selectedProducts.FindAll(thisProduct => thisProduct.Regions?.Where(x => x.Type == Isango.Entities.Enums.RegionType.Country)?.FirstOrDefault()?.Name?.ToLower() == "thailand")?.ToList();
            nonThailandProducts = selectedProducts.FindAll(thisProduct => thisProduct.Regions?.Where(x => x.Type == Isango.Entities.Enums.RegionType.Country)?.FirstOrDefault()?.Name?.ToLower() != "thailand")?.ToList();

            if (thailandProducts != null && thailandProducts.Count > 0)
            {
                isNonThailandProduct = false;
                if (AgentAuthenticateDetails.Instance(isNonThailandProduct).IsAuthenticated)
                {
                    selectedRS = (SelectedProduct)GetCreateBookingResponse(selectedProducts, bookingReference, token, isNonThailandProduct, out requestJson, out responseJson, out  httpStatusCode);
                }
            }
            if (nonThailandProducts != null && nonThailandProducts.Count > 0)
            {
                isNonThailandProduct = true;
                if (AgentAuthenticateDetails.Instance(isNonThailandProduct).IsAuthenticated)
                {
                    selectedRS = (SelectedProduct)GetCreateBookingResponse(selectedProducts, bookingReference, token, isNonThailandProduct, out requestJson, out responseJson, out  httpStatusCode);
                }
            }

            //Logic to create final booking response from Thailand and NonThailand product bookings
            if (selectedRS != null )
            {
                return (SelectedProduct)selectedRS;
            }
            return null;
        }
        private object GetCreateBookingResponse(List<SelectedProduct> selectedProducts, string apiBookingReference, 
            string token,bool isNonThailandProduct, out string requestJson, out string responseJson,
            out HttpStatusCode httpStatusCode)
        {
            requestJson = string.Empty;
            responseJson = string.Empty;
           
            //1st Step
            var inputContext = new BookInputContextV3
            {
                MethodType = MethodType.CreateBooking,
                SelectedProducts = selectedProducts,
                BookingReferenceNumber = apiBookingReference,
                isNonThailandProduct= isNonThailandProduct
            };
            var result = _iBookingCommandHandler.Execute(inputContext, token, out requestJson, out responseJson, out httpStatusCode);
            var bookingDataCheck = SerializeDeSerializeHelper.DeSerialize<BookingRS>(responseJson.ToString());

            if (result == null || bookingDataCheck.Success == false)
            {
                return null;
            }

            //2nd step
            var inputContextGetDetail = new BookInputContextV3
            {
                MethodType = MethodType.BookingDetail,
                BookingReferenceNumber = apiBookingReference,
                isNonThailandProduct = isNonThailandProduct
            };
            var resultBookingDetail = _iBookingDetailCommandHandler.Execute(inputContextGetDetail, token, out requestJson, out responseJson, out httpStatusCode);

            var bookingDataGet = SerializeDeSerializeHelper.DeSerialize<BookingDetailsResponse>(responseJson.ToString());

            if (resultBookingDetail == null || bookingDataGet.success == false)
            {
                return null;
            }

            var response = _iBookingConverter.Convert(result, resultBookingDetail, selectedProducts);
            return response as SelectedProduct;
        }
        public bool CancelByBooking(string supplierReferenceNumber, string token, out string requestJson,
            out string responseJson, out HttpStatusCode httpStatusCode, bool isNonThailandProduct)
        {
            requestJson = string.Empty;
            responseJson = string.Empty;
            if (AgentAuthenticateDetails.Instance(isNonThailandProduct).IsAuthenticated)
            {
                var inputContext = new BookInputContextV3
                {
                    MethodType = MethodType.CancelBooking,
                    BookingReferenceNumber = supplierReferenceNumber,
                    isNonThailandProduct = isNonThailandProduct
                };
                
                var returnValue = _cancelReleaseCommandHandler.Execute(inputContext, token, out requestJson, out responseJson, out  httpStatusCode);
                var cancelRS = SerializeDeSerializeHelper.DeSerialize<CancelByBookingRS>(returnValue.ToString());
                return cancelRS.IsSuccess;
            }
            httpStatusCode = HttpStatusCode.BadGateway;
            return false;
        }
    }
}
