using Isango.Entities;
using Isango.Entities.Tiqets;
using Isango.Persistence.Contract;
using Logger.Contract;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Util;
using Constant = Isango.Persistence.Constants.Constants;

namespace Isango.Persistence.ConsoleApplication.AgeGroup
{
    public class TiqetsPersistence : PersistenceBase, ITiqetsPersistence
    {
        private readonly ILogger _log;
        public TiqetsPersistence(ILogger log)
        {
            _log = log;
        }
        /// <summary>
        /// Save All Variants in the database
        /// </summary>
        public void SaveAllVariants(Dictionary<int, List<ProductVariant>> products)
        {
            try
            {
                if (products?.Count > 0)
                {
                    foreach (var product in products)
                    {
                        var productId = product.Key;
                        var variants = product.Value;
                        foreach (var variant in variants)
                        {
                            try
                            {
                                using (var command = APIUploadDb.GetStoredProcCommand(Constant.InsertTiqetsVariantsSP))
                                {
                                    // Prepare parameter collection
                                    APIUploadDb.AddInParameter(command, Constant.VariantId, DbType.Int32, variant.Id);
                                    APIUploadDb.AddInParameter(command, Constant.Label, DbType.String, variant.Label);
                                    APIUploadDb.AddInParameter(command, Constant.VariantDescription, DbType.String, variant.Description);
                                    APIUploadDb.AddInParameter(command, Constant.MaxTickets, DbType.Int32, variant.MaxTickets);
                                    APIUploadDb.AddInParameter(command, Constant.Commission, DbType.Decimal, variant.PriceComponentsEur?.DistributorCommissionExclVat);
                                    APIUploadDb.AddInParameter(command, Constant.RetailPrice, DbType.Decimal, variant.PriceComponentsEur?.TotalRetailPriceIncVat);
                                    APIUploadDb.AddInParameter(command, Constant.TicketValue, DbType.Decimal, variant.PriceComponentsEur?.SaleTicketValueIncVat);
                                    APIUploadDb.AddInParameter(command, Constant.TiqetsBookingFee, DbType.Decimal, variant.PriceComponentsEur?.BookingFeeIncVat);
                                    APIUploadDb.AddInParameter(command, Constant.TiqetsProductId, DbType.String, productId.ToString());

                                    var validWithVariantIds = String.Join(",", variant.ValidWithVariantIds.Select(p => p.ToString()).ToArray());
                                    APIUploadDb.AddInParameter(command, Constant.ValidWithVariantIds, DbType.String, validWithVariantIds);

                                    APIUploadDb.AddInParameter(command, Constant.requireVisitor, DbType.String, string.Join(",", variant.RequiresVisitorsDetails));

                                    APIUploadDb.ExecuteNonQuery(command);
                                }

                            }
                            catch (Exception ex)
                            { }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "TiqetsPersistence",
                    MethodName = "SaveAllVariants",
                    Params = $"{SerializeDeSerializeHelper.Serialize(products)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public void SaveTiqetsPackage(List<Isango.Entities.Tiqets.PackageProducts> PackageIDs)
        {
            try
            {
                if (PackageIDs?.Count > 0)
                {
                    foreach (var packageid in PackageIDs)
                    {
                        try
                        {
                            using (var command = APIUploadDb.GetStoredProcCommand(Constant.InsertTiqetsPackageSP))
                            {
                                APIUploadDb.AddInParameter(command, Constant.Package_Title, DbType.String, packageid.Package_Title);
                                APIUploadDb.AddInParameter(command, Constant.Product_ID, DbType.Int32, packageid.Product_ID);
                                APIUploadDb.AddInParameter(command, Constant.Package_ID, DbType.String, packageid.Package_ID);

                                APIUploadDb.ExecuteNonQuery(command);
                            }

                        }
                        catch (Exception ex)
                        { }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "TiqetsPersistence",
                    MethodName = "SaveTiqetsPackage",
                    Params = $"{SerializeDeSerializeHelper.Serialize(PackageIDs)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }
        public void SaveAllDetails(List<ProductDetails> productDetails)
        {
            if (productDetails?.Count > 0)
            {
                var productDetailsTable = SetProductDetail(productDetails);
                using (var connection = new SqlConnection(ConfigurationManagerHelper.GetValuefromConfig(Constant.API_Upload)))
                {
                    SqlCommand insertCommand = new SqlCommand(Constant.InsertTiqetsProdDetailsSP, connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    SqlParameter tvpParam = insertCommand.Parameters.AddWithValue(Constant.TiqetsProductDetailTableType, productDetailsTable);
                    tvpParam.SqlDbType = SqlDbType.Structured;
                    try
                    {
                        connection.Open();
                        insertCommand.ExecuteNonQuery();
                    }
                    catch(Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        connection?.Close();
                    }
                }
            }
        }
        private DataTable SetProductDetail(List<ProductDetails> productDetails)
        {
            var productDetailTable = new DataTable { TableName = Constant.TiqetsProductDetailTableName };

            foreach (var property in productDetails[0].GetType().GetProperties())
            {
                productDetailTable.Columns.Add(new DataColumn(property.Name, property.PropertyType));
            }
            foreach (var productDetail in productDetails)
            {
                var newRow = productDetailTable.NewRow();
                foreach (var property in productDetail.GetType().GetProperties())
                {
                    newRow[property.Name] = productDetail.GetType().GetProperty(property.Name)?.GetValue(productDetail, null);
                }

                productDetailTable.Rows.Add(newRow);
            }
            return productDetailTable;
        }

        public void SaveMediaImages(List<ContentMedia> contentMedia)
        {
            try
            {
                if (contentMedia != null && contentMedia.Count > 0)
                {
                    var dataTable = new DataTable { TableName = "dbo.APIMedia" };

                    dataTable.Columns.Add("APIproductcode", typeof(string));
                    dataTable.Columns.Add("isangoServiceid", typeof(int));
                    dataTable.Columns.Add("Apitypeid", typeof(int));
                    dataTable.Columns.Add("mediatype", typeof(string));
                    dataTable.Columns.Add("dpi", typeof(int));
                    dataTable.Columns.Add("height", typeof(int));
                    dataTable.Columns.Add("width", typeof(int));
                    dataTable.Columns.Add("language", typeof(string));
                    dataTable.Columns.Add("sizeType", typeof(string));
                    dataTable.Columns.Add("Image_order", typeof(int));
                    dataTable.Columns.Add("VisualizationOrder", typeof(int));
                    dataTable.Columns.Add("Url", typeof(string));
                    dataTable.Columns.Add("duration", typeof(string));
                    dataTable.Columns.Add("Cloudinaryurl", typeof(string));

                    foreach (var Media in contentMedia.Where(x => x.Language.ToLower() == "en"))
                    {
                        var dataTableRow = dataTable.NewRow();
                        dataTableRow["APIproductcode"] = Media.Factsheetid;
                        dataTableRow["isangoServiceid"] = Media.IsangoProductId;
                        dataTableRow["Apitypeid"] = 9;
                        dataTableRow["mediatype"] = Media.Mediatype;
                        dataTableRow["dpi"] = Media.Dpi;
                        dataTableRow["height"] = Media.Height;
                        dataTableRow["width"] = Media.Width;
                        dataTableRow["language"] = Media.Language;
                        dataTableRow["sizeType"] = Media.SizeType;
                        dataTableRow["Image_order"] = Media.Image_order;
                        dataTableRow["VisualizationOrder"] = Media.VisualizationOrder;
                        dataTableRow["Url"] = Media.Url;
                        dataTableRow["duration"] = Media.Duration;
                        dataTableRow["Cloudinaryurl"] = String.Empty;

                        dataTable.Rows.Add(dataTableRow);
                    }

                    using (var connection = new SqlConnection(ConfigurationManagerHelper.GetValuefromConfig(Constant.APIUploadDb)))
                    {
                        var insertCommand = new SqlCommand(Constant.InsertAPIImages, connection)
                        {
                            CommandType = CommandType.StoredProcedure
                        };
                        var tvpFactSheet = insertCommand.Parameters.AddWithValue(Constant.APIMediaParameter, dataTable);
                        tvpFactSheet.SqlDbType = SqlDbType.Structured;

                        try
                        {
                            connection.Open();
                            insertCommand.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            //ignore
                        }
                        finally
                        {
                            connection.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ApiTudePersistence",
                    MethodName = "SaveApiTudeMediImages",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }
        public void SyncDataInVariantTemp()
        {
            try
            {
                using (var variantCommand = IsangoDataBaseLive.GetStoredProcCommand(Constant.InsertTiqetsVariantsSPIntoTemp))
                {
                    IsangoDataBaseLive.ExecuteNonQuery(variantCommand);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "TiqetsPersistence",
                    MethodName = "SyncDataInVariantTemp",

                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }
       
    }
}