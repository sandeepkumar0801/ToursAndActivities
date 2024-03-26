using Isango.Entities.Rezdy;
using Isango.Persistence.Contract;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Util;
using Constant = Isango.Persistence.Constants.Constants;
using System.Linq;
using System;
using Isango.Entities;
using Logger.Contract;
using ServiceAdapters.PrioHub.PrioHub.Entities.ProductListResponse;
using Isango.Entities.PrioHub;
using ServiceAdapters.PrioHub.PrioHub.Entities.RouteResponse;

namespace Isango.Persistence.ConsoleApplication.AgeGroup
{
    public class NewPrioPersistence : PersistenceBase, INewPrioPersistence
    {
        private readonly ILogger _log;
        public NewPrioPersistence(ILogger log)
        {
            _log = log;
        }

        public void SavePrioHubProducts(List<Item> products)
        {
            var lstProduct = new List<NewPrioProduct>();
            var lstProductTypeSeasonDetails = new List<NewPrioProductTypeSeasons>();
            var lstProductOptions = new List<NewPrioProductExtraOptions>();
            var lstOptionValues = new List<NewPrioProductExtraOptionsValues>();
            if (products != null && products?.Count > 0)
            {
                foreach (var item in products)
                {
                    try
                    {
                        //1. Assign Data Product Start
                        var passData = new NewPrioProduct
                        {
                            ProductId = Convert.ToInt32(item?.ProductId),
                            ProductAddon = item.ProductAddon,
                            ProductAdmissionType = item?.ProductAdmissionType,
                            ProductAvailability = item.ProductAvailability,
                            ProductBookingStartDate = Convert.ToString(item?.ProductBookingStartDate),
                            productCancellationAllowed = item.productCancellationAllowed,
                            ProductCapacity = item.ProductCapacity,
                            ProductCapacityId = item?.ProductCapacityId,
                            ProductCapacityType = item?.ProductCapacityType,
                            productCatalogueStatus = item?.productCatalogueStatus,
                            ProductCluster = item.ProductCluster,
                            ProductCombi = item.ProductCombi,
                            ProductDailyPricing = item.ProductDailyPricing,
                            ProductDistributorId = item?.ProductDistributorId,
                            ProductDistributorName = item?.ProductDistributorName,
                            ProductDuration = item.ProductDuration,
                            ProductDynamicPricing = item.ProductDynamicPricing,
                            ProductEndDate = Convert.ToString(item?.ProductEndDate),
                            ProductFromPrice = item?.ProductFromPrice,
                            ProductInternalReference = item?.ProductInternalReference,
                            ProductoverBookingAllowed = item.ProductoverBookingAllowed,
                            ProductPickupPoint = item?.ProductPickupPoint,
                            ProductQuantityPricing = item.ProductQuantityPricing,
                            ProductrelationDetailsVisible = item.ProductrelationDetailsVisible,
                            ProductResellerId = item?.ProductResellerId,
                            ProductResellerName = item?.ProductResellerName,
                            ProductSeasonalPricing = item.ProductSeasonalPricing,
                            Productsourcename = item?.Productsourcename,
                            ProductStartDate = Convert.ToString(item?.ProductStartDate),
                            ProductStatus = item?.ProductStatus,
                            ProductSupplierId = item?.ProductSupplierId,
                            ProductThirdParty = item.ProductThirdParty,
                            ProductTimePickerVisible = item.ProductTimePickerVisible,
                            ProductTravelDateRequired = item.ProductTravelDateRequired,
                            ProductViewType = item?.Productviewtype,
                            ProductVoucherSettings = item?.productCodeSettings?.ProductVoucherSettings,
                            ProductcodeFormat = item?.productCodeSettings?.ProductcodeFormat,
                            ProductCodeSource = item?.productCodeSettings?.ProductCodeSource,
                            ProductCombiCode = item.productCodeSettings.ProductCombiCode,
                            ProductdurationText = item?.ProductContent.ProductdurationText,
                            ProductShortDescription = item?.ProductContent?.ProductShortDescription,
                            ProductEntryNotes = item?.ProductContent?.ProductEntryNotes,
                            ProductGroupCode = item.productCodeSettings.ProductGroupCode,
                            ProductLongDescription = item?.ProductContent?.ProductLongDescription,
                            ProductPaymentCurrency = item?.ProductPaymentDetail?.ProductPaymentCurrency?.CurrencyCode,
                            ProductAdditionalInformation = item?.ProductContent?.ProductAdditionalInformation,
                            ProductSupplierName = item?.ProductContent?.ProductSupplierName,
                            ProductTitle = item?.ProductContent?.ProductTitle,
                            Route = item?.ProductRoute != null ? string.Join(",", item?.ProductRoute) : "",
                            MinQuantity = item.MinQuantity,
                            MaxQuantity = item.MaxQuantity
                        };
                        lstProduct.Add(passData);
                        //AssigData Product End

                        // 2.Assign Data ProductTypeSeasons Start
                        if (item.ProductTypeSeasons != null && item.ProductTypeSeasons?.Count > 0)
                        {
                            foreach (var season in item.ProductTypeSeasons)
                            {
                                try
                                {
                                    if (season != null && season?.ProductTypeSeasonDetails?.Count > 0)
                                    {
                                        foreach (var seasonData in season?.ProductTypeSeasonDetails)
                                        {
                                            var passDataSeason = new NewPrioProductTypeSeasons
                                            {
                                                ProductType = seasonData?.ProductType,
                                                ProductTypeAgeFrom = seasonData.ProductTypeAgeFrom,
                                                ProductTypeAgeTo = seasonData.ProductTypeAgeTo,
                                                ProductTypeCapacity = seasonData.ProductTypeCapacity,
                                                ProductTypeClass = seasonData?.ProductTypeClass,
                                                ProductTypeId = seasonData?.ProductTypeId,
                                                ProductTypeLabel = seasonData?.ProductTypeLabel,
                                                ProductTypePax = seasonData.ProductTypePax,
                                                ProductTypePriceTaxid = seasonData?.ProductTypePriceTaxid,
                                                ProductTypePriceType = seasonData?.ProductTypePriceType,
                                                FeeAmount = seasonData?.ProductTypeFees?.FirstOrDefault().FeeAmount,
                                                FeePercentage = seasonData?.ProductTypeFees?.FirstOrDefault().FeePercentage,
                                                ProductId = Convert.ToInt32(item?.ProductId),
                                                ProductTypeDiscount = seasonData?.ProductTypePricing?.ProductTypeDiscount,
                                                ProductTypeDisplayPrice = seasonData.ProductTypePricing.ProductTypeDisplayPrice,
                                                ProductTypeListPrice = seasonData?.ProductTypePricing?.ProductTypeListPrice,
                                                ProductTyperesalePrice = seasonData?.ProductTypePricing?.ProductTyperesalePrice,
                                                ProductTypeSalesPrice = seasonData?.ProductTypePricing?.ProductTypeSalesPrice,
                                                ProductTypeSupplierPrice = seasonData?.ProductTypePricing?.ProductTypeSupplierPrice
                                            };
                                            lstProductTypeSeasonDetails.Add(passDataSeason);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                { }
                            }
                        }
                        // AssigData ProductTypeSeasons End

                        // 3.Assign Data Options Extras Start
                        if (item?.ProductOptions != null && item?.ProductOptions?.Count > 0)
                        {
                            foreach (var option in item?.ProductOptions)
                            {
                                try
                                {
                                    var passDataOption = new NewPrioProductExtraOptions
                                    {
                                        ProductId = Convert.ToInt32(item?.ProductId),
                                        OptionCounttype = option?.OptionCounttype,
                                        OptionDescription = option?.OptionDescription,
                                        OptionId = option?.OptionId,
                                        OptionlistType = option?.OptionlistType,
                                        OptionMandatory = option.OptionMandatory,
                                        optionName = option?.optionName,
                                        OptionpriceType = option?.OptionpriceType,
                                        OptionSelectiontype = option?.OptionSelectiontype,
                                        OptionType = option?.OptionType
                                    };
                                    lstProductOptions.Add(passDataOption);

                                    // 4.Assign Data Options Extras values Start
                                    foreach (var optionValue in option.OptionValues)
                                    {
                                        try
                                        {
                                            var passDataOptionValues = new NewPrioProductExtraOptionsValues
                                            {
                                                ProductId = Convert.ToInt32(item?.ProductId),
                                                OptionId = option?.OptionId,
                                                ValueiId = optionValue?.ValueiId,
                                                ValueName = optionValue?.ValueName,
                                                ValuePrice = optionValue?.ValuePrice,
                                                ValuePriceTaxId = optionValue?.ValuePriceTaxId
                                            };
                                            lstOptionValues.Add(passDataOptionValues);
                                        }
                                        catch (Exception ex)
                                        { }
                                    }
                                }
                                catch (Exception ex)
                                { }
                            }
                        }
                    }
                    catch (Exception ex)
                    { }
                    // Assign Data Options Extras End
                }

            }
            var productDataTable = SetProductDetails(lstProduct);
            var productDataTableSeason = SetProductDetailsSeason(lstProductTypeSeasonDetails);

            var productDataTableExtrasOptions = SetProductDetailsExtrasOptions(lstProductOptions);
            var productDataTableExtrasOptionsValues = SetProductDetailsExtrasOptionsValues(lstOptionValues);

            try
            {
                //1.) save data into NewPrioProduct Table
                using (var connection = new SqlConnection(ConfigurationManagerHelper.GetValuefromConfig(Constant.IsangoLiveDb)))
                {
                    var insertCommand = new SqlCommand(Constant.InsertNewPrioProductSp, connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    var param = insertCommand.Parameters.AddWithValue(Constant.NewPrioProductListParameter, productDataTable);
                    param.SqlDbType = SqlDbType.Structured;
                    param.TypeName = Constant.NewPrioProductDBType;
                    try
                    {
                        connection.Open();
                        insertCommand.ExecuteNonQuery();
                    }
                    finally
                    {
                        connection.Close();
                    }
                }

                //2.) save data into NewPrioProductTypeSeasons Table
                using (var connection = new SqlConnection(ConfigurationManagerHelper.GetValuefromConfig(Constant.IsangoLiveDb)))
                {
                    var insertCommand = new SqlCommand(Constant.InsertNewPrioProductTypeSeasonsProductSp, connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    var param = insertCommand.Parameters.AddWithValue(Constant.NewPrioProductTypeSeasonsParameter, productDataTableSeason);
                    param.SqlDbType = SqlDbType.Structured;
                    param.TypeName = Constant.NewPrioProductTypeSeasonsDBType;
                    try
                    {
                        connection.Open();
                        insertCommand.ExecuteNonQuery();
                    }
                    finally
                    {
                        connection.Close();
                    }
                }

                //3.) save data into NewPrioProductExtraOptions Table
                using (var connection = new SqlConnection(ConfigurationManagerHelper.GetValuefromConfig(Constant.IsangoLiveDb)))
                {
                    var insertCommand = new SqlCommand(Constant.InsertNewPrioProductExtraOptionsProductSp, connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    var param = insertCommand.Parameters.AddWithValue(Constant.NewPrioProductExtraOptionsParameter, productDataTableExtrasOptions);
                    param.SqlDbType = SqlDbType.Structured;
                    param.TypeName = Constant.NewPrioProductExtraOptionsDBType;
                    try
                    {
                        connection.Open();
                        insertCommand.ExecuteNonQuery();
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
                //4.) save data into NewPrioProductExtraOptionsValues Table
                using (var connection = new SqlConnection(ConfigurationManagerHelper.GetValuefromConfig(Constant.IsangoLiveDb)))
                {
                    var insertCommand = new SqlCommand(Constant.InsertNewPrioProductExtraOptionsValuesProductSp, connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    var param = insertCommand.Parameters.AddWithValue(Constant.NewPrioProductExtraOptionsValuesParameter, productDataTableExtrasOptionsValues);
                    param.SqlDbType = SqlDbType.Structured;
                    param.TypeName = Constant.NewPrioProductExtraOptionsValuesDBType;
                    try
                    {
                        connection.Open();
                        insertCommand.ExecuteNonQuery();
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "NewPrioPersistence",
                    MethodName = "SaveNewPrioProducts",
                    Params = $"{SerializeDeSerializeHelper.Serialize(products)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }


        public void SavePrioHubProductsRoutes(List<ItemRoute> products)
        {
            var lstNewPrioProductRoutes = new List<NewPrioProductRoutes>();
            var lstNewPrioProductRoutesLocations = new List<NewPrioProductRoutesLocations>();

            if (products != null && products?.Count > 0)
            {
                foreach (var item in products)
                {
                    try
                    {
                        //1. Assign Data ProductRoute Start
                        var passDataRoute = new NewPrioProductRoutes
                        {
                            RouteActive = item.RouteActive,
                            RouteAudioLanguages = item?.RouteAudioLanguages != null ? string.Join(",", item?.RouteAudioLanguages) : "",
                            RouteColor = item?.RouteColor,
                            RouteDuration = item.RouteDuration,
                            RouteEndTime = item?.RouteEndTime,
                            RouteFrequency = item.RouteFrequency,
                            RouteId = item?.RouteId,
                            RouteLiveLanguages = item?.RouteLiveLanguages != null ? string.Join(",", item?.RouteLiveLanguages) : "",
                            RouteName = item?.RouteName,
                            RouteProducts = item.RouteProducts != null ? string.Join(",", item.RouteProducts) : "",
                            RouteStartTime = item?.RouteStartTime,
                            RouteType = item?.RouteType
                        };
                        lstNewPrioProductRoutes.Add(passDataRoute);
                        //Assign Data ProductRoute End

                        //2. Locations Start
                        if (item?.RouteLocations != null && item?.RouteLocations.Count > 0)
                        {
                            foreach (var location in item.RouteLocations)
                            {
                                try
                                {
                                    var passDataRouteLocations = new NewPrioProductRoutesLocations
                                    {
                                        RouteId = item?.RouteId,
                                        RouteLocationActive = location.RouteLocationActive,
                                        RouteLocationId = location?.RouteLocationId,
                                        RouteLocationName = location?.RouteLocationName,
                                        RouteLocationStopOver = location.RouteLocationStopOver
                                    };
                                    //2. Locations End\
                                    lstNewPrioProductRoutesLocations.Add(passDataRouteLocations);
                                }
                                catch (Exception ex)
                                { }
                            }
                        }
                    }
                    catch (Exception ex)
                    { }
                }
                var productRouteDataTable = SetProductDetailsRoute(lstNewPrioProductRoutes);
                var productRouteLocationsDataTable = SetProductDetailsRouteLocation(lstNewPrioProductRoutesLocations);

                try
                {
                    //1.) save data into NewPrioRoute Table
                    using (var connection = new SqlConnection(ConfigurationManagerHelper.GetValuefromConfig(Constant.IsangoLiveDb)))
                    {
                        var insertCommand = new SqlCommand(Constant.InsertNewPrioProductRouteProductSp, connection)
                        {
                            CommandType = CommandType.StoredProcedure
                        };
                        var param = insertCommand.Parameters.AddWithValue(Constant.NewPrioProductRouteParameter, productRouteDataTable);
                        param.SqlDbType = SqlDbType.Structured;
                        param.TypeName = Constant.NewPrioProductRouteDBType;
                        try
                        {
                            connection.Open();
                            insertCommand.ExecuteNonQuery();
                        }
                        finally
                        {
                            connection.Close();
                        }
                    }
                    //2) save data into NewPrioRouteLocation Table
                    using (var connection = new SqlConnection(ConfigurationManagerHelper.GetValuefromConfig(Constant.IsangoLiveDb)))
                    {
                        var insertCommand = new SqlCommand(Constant.InsertNewPrioProductRouteLocationProductSp, connection)
                        {
                            CommandType = CommandType.StoredProcedure
                        };
                        var param = insertCommand.Parameters.AddWithValue(Constant.NewPrioProductRouteLocationParameter, productRouteLocationsDataTable);
                        param.SqlDbType = SqlDbType.Structured;
                        param.TypeName = Constant.NewPrioProductRouteLocationDBType;
                        try
                        {
                            connection.Open();
                            insertCommand.ExecuteNonQuery();
                        }
                        finally
                        {
                            connection.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    var isangoErrorEntity = new IsangoErrorEntity
                    {
                        ClassName = "NewPrioPersistence",
                        MethodName = "SaveNewPrioProductsRoutes",
                        Params = $"{SerializeDeSerializeHelper.Serialize(products)}"
                    };
                    _log.Error(isangoErrorEntity, ex);
                    throw;
                }
            }
        }
        #region Private methods


        private DataTable SetProductDetails(List<NewPrioProduct> products)
        {
            var dataTable = new DataTable { TableName = Constant.NewPrioProduct };
            try
            {
                foreach (var property in products[0].GetType().GetProperties())
                {
                    dataTable.Columns.Add(new DataColumn(property.Name, property.PropertyType));
                }

                foreach (var productDetail in products)
                {
                    var newRow = dataTable.NewRow();
                    foreach (var property in productDetail.GetType().GetProperties())
                    {
                        newRow[property.Name] = productDetail.GetType().GetProperty(property.Name)
                            ?.GetValue(productDetail, null);
                    }
                    dataTable.Rows.Add(newRow);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "NewPrioPersistence",
                    MethodName = "SetProductDetails",
                    Params = $"{SerializeDeSerializeHelper.Serialize(products)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return dataTable;
        }
        private DataTable SetProductDetailsSeason(List<NewPrioProductTypeSeasons> products)
        {
            var dataTable = new DataTable { TableName = Constant.NewPrioProductTypeSeasons };
            try
            {
                foreach (var property in products[0].GetType().GetProperties())
                {
                    dataTable.Columns.Add(new DataColumn(property.Name, property.PropertyType));
                }

                foreach (var productDetail in products)
                {
                    var newRow = dataTable.NewRow();
                    foreach (var property in productDetail.GetType().GetProperties())
                    {
                        newRow[property.Name] = productDetail.GetType().GetProperty(property.Name)
                            ?.GetValue(productDetail, null);
                    }
                    dataTable.Rows.Add(newRow);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "NewPrioPersistence",
                    MethodName = "SetProductDetailsSeason",
                    Params = $"{SerializeDeSerializeHelper.Serialize(products)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return dataTable;
        }
        private DataTable SetProductDetailsExtrasOptions(List<NewPrioProductExtraOptions> products)
        {
            var dataTable = new DataTable { TableName = Constant.NewPrioProductExtraOptionsSeasons };
            try
            {
                foreach (var property in products[0].GetType().GetProperties())
                {
                    dataTable.Columns.Add(new DataColumn(property.Name, property.PropertyType));
                }

                foreach (var productDetail in products)
                {
                    var newRow = dataTable.NewRow();
                    foreach (var property in productDetail.GetType().GetProperties())
                    {
                        newRow[property.Name] = productDetail.GetType().GetProperty(property.Name)
                            ?.GetValue(productDetail, null);
                    }
                    dataTable.Rows.Add(newRow);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "NewPrioPersistence",
                    MethodName = "SetProductDetailsExtrasOptions",
                    Params = $"{SerializeDeSerializeHelper.Serialize(products)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return dataTable;
        }
        private DataTable SetProductDetailsExtrasOptionsValues(List<NewPrioProductExtraOptionsValues> products)
        {
            var dataTable = new DataTable { TableName = Constant.NewPrioProductExtraOptionsValuesSeasons };
            try
            {
                foreach (var property in products[0].GetType().GetProperties())
                {
                    dataTable.Columns.Add(new DataColumn(property.Name, property.PropertyType));
                }

                foreach (var productDetail in products)
                {
                    var newRow = dataTable.NewRow();
                    foreach (var property in productDetail.GetType().GetProperties())
                    {
                        newRow[property.Name] = productDetail.GetType().GetProperty(property.Name)
                            ?.GetValue(productDetail, null);
                    }
                    dataTable.Rows.Add(newRow);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "NewPrioPersistence",
                    MethodName = "SetProductDetailsSeason",
                    Params = $"{SerializeDeSerializeHelper.Serialize(products)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return dataTable;
        }

        private DataTable SetProductDetailsRoute(List<NewPrioProductRoutes> products)
        {
            var dataTable = new DataTable { TableName = Constant.NewPrioProductRoute };
            try
            {
                foreach (var property in products[0].GetType().GetProperties())
                {
                    dataTable.Columns.Add(new DataColumn(property.Name, property.PropertyType));
                }

                foreach (var productDetail in products)
                {
                    var newRow = dataTable.NewRow();
                    foreach (var property in productDetail.GetType().GetProperties())
                    {
                        newRow[property.Name] = productDetail.GetType().GetProperty(property.Name)
                            ?.GetValue(productDetail, null);
                    }
                    dataTable.Rows.Add(newRow);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "NewPrioPersistence",
                    MethodName = "SetProductDetailsRoute",
                    Params = $"{SerializeDeSerializeHelper.Serialize(products)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return dataTable;
        }

        private DataTable SetProductDetailsRouteLocation(List<NewPrioProductRoutesLocations> products)
        {
            var dataTable = new DataTable { TableName = Constant.NewPrioProductRouteLocation };
            try
            {
                foreach (var property in products[0].GetType().GetProperties())
                {
                    dataTable.Columns.Add(new DataColumn(property.Name, property.PropertyType));
                }

                foreach (var productDetail in products)
                {
                    var newRow = dataTable.NewRow();
                    foreach (var property in productDetail.GetType().GetProperties())
                    {
                        newRow[property.Name] = productDetail.GetType().GetProperty(property.Name)
                            ?.GetValue(productDetail, null);
                    }
                    dataTable.Rows.Add(newRow);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "NewPrioPersistence",
                    MethodName = "SetProductDetailsRouteLocation",
                    Params = $"{SerializeDeSerializeHelper.Serialize(products)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return dataTable;
        }
        #endregion Private methods
    }
}
