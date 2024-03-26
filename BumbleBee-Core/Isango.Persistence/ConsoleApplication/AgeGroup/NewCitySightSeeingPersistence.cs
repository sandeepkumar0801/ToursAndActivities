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
using ServiceAdapters.NewCitySightSeeing.NewCitySightSeeing.Entities.Product;

namespace Isango.Persistence.ConsoleApplication.AgeGroup
{
    public class NewCitySightSeeingPersistence : PersistenceBase, INewCitySightSeeingPersistence
    {
        private readonly ILogger _log;
        public NewCitySightSeeingPersistence(ILogger log)
        {
            _log = log;
        }

        public void SaveNewCitySightSeeingProducts(List<Products> products)
        {
            var lstProduct = new List<Isango.Entities.NewCitySightSeeing.Product>();
            var lstProductVariant = new List<Isango.Entities.NewCitySightSeeing.ProductVariant>();
            //AssigData
            if (products != null && products.Count > 0)
            { 
                foreach (var item in products)
                {
                    try
                    {
                        if (item.AvailableDays != null)
                        {
                            var passData = new Isango.Entities.NewCitySightSeeing.Product
                            {
                                address = item?.MeetingPoints?.FirstOrDefault()?.Address,
                                availableDays = item == null ? null : item?.AvailableDays == null ? "" : string.IsNullOrEmpty(item?.AvailableDays) ? null : item?.AvailableDays,
                                cancellationPolicy = item?.CancellationPolicy,
                                content = item?.Content,
                                description = item?.Description,
                                DestinationSubTitle = item?.Destination?.SubTitle,
                                DestinationTitle = item?.Destination?.Title,
                                duration = item?.Duration,
                                Id = item.Id,
                                lat = item.Destination.GeoData.Lat,
                                lon = item.Destination.GeoData.Lon,
                                notes = item?.AvailableDays == null ? null : string.IsNullOrEmpty(item?.Notes) ? null : item.Notes,
                                ShortDescription = item?.Destination?.ShortDescription,
                                sku = item?.Sku,
                                title = item?.Title,
                                zoom = item.Destination.GeoData.Zoom
                            };
                            lstProduct.Add(passData);
                        }
                    }
                    catch(Exception ex)
                    {
                        //throw ex;

                    }
                   
                    if (item?.Variants != null && item.Variants.Count > 0)
                    {
                        foreach (var itemVaraint in item?.Variants)
                        {
                            var passDataVariant = new Isango.Entities.NewCitySightSeeing.ProductVariant
                            {
                                Id = itemVaraint.Id,
                                Title = itemVaraint?.Title,
                                Code = itemVaraint?.Code,
                                ProductId = item.Id,
                                VariantCode = itemVaraint?.VariantCode,
                                VariantName = itemVaraint?.VariantName
                            };
                            lstProductVariant.Add(passDataVariant);
                        }
                    }
                   
                }
        }
            var productDetailsDataTable = SetProductDetails(lstProduct);
            var productDetailsDataTableVariant = SetProductDetailsVariant(lstProductVariant);
            try
            {
                using (var connection = new SqlConnection(ConfigurationManagerHelper.GetValuefromConfig(Constant.IsangoLiveDb)))
                {
                    var insertCommand = new SqlCommand(Constant.InsertNewCitySightSeeingProductsSP, connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    var param = insertCommand.Parameters.AddWithValue(Constant.NewCitySightSeeingProductParamter, productDetailsDataTable);
                    param.SqlDbType = SqlDbType.Structured;
                    param.TypeName = Constant.NewCitySightSeeingProductDBType;
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

                using (var connection = new SqlConnection(ConfigurationManagerHelper.GetValuefromConfig(Constant.IsangoLiveDb)))
                {
                    var insertCommand = new SqlCommand(Constant.InsertNewCitySightSeeingProductsSPVariant, connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    var param = insertCommand.Parameters.AddWithValue(Constant.NewCitySightSeeingProductParamterVariant, productDetailsDataTableVariant);
                    param.SqlDbType = SqlDbType.Structured;
                    param.TypeName = Constant.NewCitySightSeeingProductDBTypeVariant;
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
                    ClassName = "NewCitySightSeeingPersistence",
                    MethodName = "SaveNewCitySightSeeingProducts",
                    Params = $"{SerializeDeSerializeHelper.Serialize(products)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }


        #region Private methods


        private DataTable SetProductDetails(List<Isango.Entities.NewCitySightSeeing.Product> products)
        {
            var dataTable = new DataTable { TableName = Constant.NewCitySightSeeingProducts };
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
                    ClassName = "NewCitySightSeeingPersistence",
                    MethodName = "SetProductDetails",
                    Params = $"{SerializeDeSerializeHelper.Serialize(products)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return dataTable;
        }
        private DataTable SetProductDetailsVariant(List<Isango.Entities.NewCitySightSeeing.ProductVariant> products)
        {
            var dataTable = new DataTable { TableName = Constant.NewCitySightSeeingProductsVariant };
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
                    ClassName = "NewCitySightSeeingPersistence",
                    MethodName = "SetProductDetailsVariant",
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
