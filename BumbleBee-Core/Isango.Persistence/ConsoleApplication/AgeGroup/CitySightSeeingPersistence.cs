using Isango.Entities.GlobalTix;
using Isango.Persistence.Contract;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util;
using ServiceAdapters.GlobalTix.Constants;

using PersistenceConstants = Isango.Persistence.Constants.Constants;
using Logger.Contract;
using Isango.Entities;
using Isango.Entities.NewCitySightSeeing;

namespace Isango.Persistence.ConsoleApplication.AgeGroup
{
    public class CitySightSeeingPersistence : PersistenceBase, ICitySightSeeingPersistence
    {
        private readonly ILogger _log;
        public CitySightSeeingPersistence(ILogger log)
        {
            _log = log;
        }
        private string _connString;

        public CitySightSeeingPersistence()
        {
            _connString = ConfigurationManagerHelper.GetValuefromConfig(PersistenceConstants.DB_APIUpload);
        }
        public void SaveProductList(List<Entities.NewCitySightSeeing.Product> products)
        {
            //try
            //{
            //    using (SqlConnection conn = new SqlConnection(_connString))
            //    {
            //        conn.Open();

            //        DeleteActivitiesDataFromTables(conn);

            //        DataTable prodTable = GetDataTableForProduct();
            //        DataTable prodOptTable = GetDataTableForProductOption();
            //        DataTable prodOptTktTable = GetDataTableForProductOptionTicket();


            //        foreach (GlobalTixActivity actData in gtActivities)
            //        {

            //            try
            //            {

            //                prodTable.Rows.Add(actData.Id, false, actData.Country.Id, actData.Country.Name, actData.City.Id, actData.City.Name, actData.Title,
            //                    (actData.Desc.Length <= PersistenceConstants.ColLen_ProductDesc) ? actData.Desc : actData.Desc.Substring(0, PersistenceConstants.ColLen_ProductDesc),
            //                    (actData.OpHours.Length <= PersistenceConstants.ColLen_HoursOfOp) ? actData.OpHours : actData.OpHours.Substring(0, PersistenceConstants.ColLen_HoursOfOp), actData.Latitude, actData.Longitude);
            //                try
            //                {
            //                    foreach (GlobalTixTicketTypeGroup tktGrp in actData.TicketTypeGroups)
            //                    {
            //                        prodOptTable.Rows.Add(actData.Id, tktGrp.Id, tktGrp.ApplyCapacity, tktGrp.Name,
            //                                            tktGrp.Desc != null ? ((tktGrp.Desc.Length <= PersistenceConstants.ColLen_OptionDesc) ? tktGrp.Desc : tktGrp.Desc.Substring(0, PersistenceConstants.ColLen_OptionDesc)) : string.Empty);
            //                        foreach (GlobalTixIdentifier tktId in tktGrp.Products)
            //                        {
            //                            GlobalTixTicketType tktType = actData?.TicketTypes?.FirstOrDefault(tkt => tkt.Id == tktId.Id);
            //                            if (tktType == null)
            //                            {
            //                                continue;
            //                            }
            //                            try
            //                            {
            //                                if (!string.IsNullOrEmpty(Convert.ToString(tktType.PaxType)))
            //                                {
            //                                    prodOptTktTable.Rows.Add(actData.Id, tktGrp.Id, tktType.Id, tktType.Name, tktType.PaxType, tktType.CurrencyCode, tktType.Price, tktType.ToAge, tktType.FromAge, tktType.OriginalPrice, tktType.MinimumSellingPrice, string.IsNullOrEmpty(tktType.CancellationNotesSetting.Id) ? string.Empty : tktType.CancellationNotesSetting.Id, string.IsNullOrEmpty(tktType.CancellationNotesSetting.Value) ? string.Empty : tktType.CancellationNotesSetting.Value);
            //                                }
            //                            }
            //                            catch (Exception ex)
            //                            {
            //                                var isangoErrorEntity = new IsangoErrorEntity
            //                                {
            //                                    ClassName = "GlobalTixPersistence",
            //                                    MethodName = "SaveAllActivities",
            //                                };
            //                                continue;
            //                            }
            //                        }
            //                    }
            //                }
            //                catch (Exception ex)
            //                {
            //                    var isangoErrorEntity = new IsangoErrorEntity
            //                    {
            //                        ClassName = "GlobalTixPersistence",
            //                        MethodName = "SaveAllActivities",
            //                    };
            //                    _log.Error(isangoErrorEntity, ex);

            //                    continue;
            //                }
            //            }
            //            catch (Exception ex)
            //            {
            //                var isangoErrorEntity = new IsangoErrorEntity
            //                {
            //                    ClassName = "GlobalTixPersistence",
            //                    MethodName = "SaveAllActivities",
            //                };
            //                _log.Error(isangoErrorEntity, ex);
            //                continue;
            //            }
            //        }
            //        WriteProductDataToDB(conn, prodTable, prodOptTable, prodOptTktTable);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    var isangoErrorEntity = new IsangoErrorEntity
            //    {
            //        ClassName = "GlobalTixPersistence",
            //        MethodName = "SaveAllActivities",
            //        Params = $"{SerializeDeSerializeHelper.Serialize(gtActivities)}"
            //    };
            //    _log.Error(isangoErrorEntity, ex);
            //    throw;
            //}
            throw new NotImplementedException();
        }

        public void SaveProductVariantList(List<ProductVariant> products)
        {
            throw new NotImplementedException();
        }
       
       
       

        #region Private Methods
        private DataTable GetDataTableForCountryCity()
        {
            DataTable countryCityTable = new DataTable();
            try
            {
                countryCityTable.Columns.Add(PersistenceConstants.Column_CountryId, typeof(string));
                countryCityTable.Columns.Add(PersistenceConstants.Column_CityId, typeof(string));
                countryCityTable.Columns.Add(PersistenceConstants.Column_CountryName, typeof(string));
                countryCityTable.Columns.Add(PersistenceConstants.Column_CityName, typeof(string));
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "GlobalTixPersistence",
                    MethodName = "GetDataTableForCountryCity",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return countryCityTable;
        }

     

        private void WriteProductDataToDB(SqlConnection conn, DataTable prodTable, DataTable prodOptTable, DataTable prodOptTktTable)
        {
            try
            {

                if (prodTable.Rows.Count <= 0)
                {
                    return;
                }

                using (SqlBulkCopy dbBulkCopy = new SqlBulkCopy(conn))
                {
                    try
                    {
                        dbBulkCopy.DestinationTableName = PersistenceConstants.DBTable_Product;
                        dbBulkCopy.BatchSize = PersistenceConstants.DBTable_WriteBatchSize_Product;
                        dbBulkCopy.WriteToServer(prodTable);

                        dbBulkCopy.DestinationTableName = PersistenceConstants.DBTable_ProductOption;
                        dbBulkCopy.BatchSize = PersistenceConstants.DBTable_WriteBatchSize_ProductOption;
                        dbBulkCopy.WriteToServer(prodOptTable);

                        dbBulkCopy.DestinationTableName = PersistenceConstants.DBTable_ProductOptionTicket;
                        dbBulkCopy.BatchSize = PersistenceConstants.DBTable_WriteBatchSize_ProductOptionTicket;
                        dbBulkCopy.WriteToServer(prodOptTktTable);
                    }
                    catch (Exception ex)
                    {
                        //logging
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "GlobalTixPersistence",
                    MethodName = "WriteProductDataToDB",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        #endregion Private Methods
    }
}
