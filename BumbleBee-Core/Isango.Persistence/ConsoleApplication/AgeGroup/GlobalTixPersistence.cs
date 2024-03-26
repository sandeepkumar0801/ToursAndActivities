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
using Isango.Entities.GlobalTixV3;
//using Isango.Entities.GlobalTixV3;

namespace Isango.Persistence.ConsoleApplication.AgeGroup
{
    public class GlobalTixPersistence : PersistenceBase, IGlobalTixPersistence
    {
        private readonly ILogger _log;
        private string _connString;
        public GlobalTixPersistence(ILogger log)
        {
            _log = log;
            _connString = ConfigurationManagerHelper.GetValuefromConfig(PersistenceConstants.DB_APIUpload);
        }

        public GlobalTixPersistence()
        {
            _connString = ConfigurationManagerHelper.GetValuefromConfig(PersistenceConstants.DB_APIUpload);
        }

        public void SaveAllActivities(List<GlobalTixActivity> gtActivities)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    conn.Open();

                    DeleteActivitiesDataFromTables(conn);

                    DataTable prodTable = GetDataTableForProduct();
                    DataTable prodOptTable = GetDataTableForProductOption();
                    DataTable prodOptTktTable = GetDataTableForProductOptionTicket();


                    foreach (GlobalTixActivity actData in gtActivities)
                    {

                        try
                        {

                            prodTable.Rows.Add(actData.Id, false, actData.Country.Id, actData.Country.Name, actData.City.Id, actData.City.Name, actData.Title,
                                   (actData?.Desc?.Length <= PersistenceConstants.ColLen_ProductDesc) ? actData?.Desc : actData?.Desc?.Substring(0, PersistenceConstants.ColLen_ProductDesc),
                                   (actData?.OpHours?.Length <= PersistenceConstants.ColLen_HoursOfOp) ? actData?.OpHours : actData?.OpHours?.Substring(0, PersistenceConstants.ColLen_HoursOfOp), actData?.Latitude, actData?.Longitude);
                            try
                            {
                                foreach (GlobalTixTicketTypeGroup tktGrp in actData.TicketTypeGroups)
                                {
                                    prodOptTable.Rows.Add(actData.Id, tktGrp.Id, tktGrp.ApplyCapacity, tktGrp.Name,
                                                        tktGrp.Desc != null ? ((tktGrp.Desc.Length <= PersistenceConstants.ColLen_OptionDesc) ? tktGrp.Desc : tktGrp.Desc.Substring(0, PersistenceConstants.ColLen_OptionDesc)) : string.Empty);
                                    foreach (GlobalTixIdentifier tktId in tktGrp.Products)
                                    {
                                        GlobalTixTicketType tktType = actData?.TicketTypes?.FirstOrDefault(tkt => tkt.Id == tktId.Id);
                                        if (tktType == null)
                                        {
                                            continue;
                                        }
                                        try
                                        {
                                            if (!string.IsNullOrEmpty(Convert.ToString(tktType.PaxType)))
                                            {
                                                prodOptTktTable.Rows.Add(actData.Id, tktGrp.Id, tktType.Id, tktType.Name, tktType.PaxType, tktType.CurrencyCode, tktType.Price, tktType.ToAge, tktType.FromAge, tktType.OriginalPrice, tktType.MinimumSellingPrice, string.IsNullOrEmpty(tktType.CancellationNotesSetting.Id) ? string.Empty : tktType.CancellationNotesSetting.Id, string.IsNullOrEmpty(tktType.CancellationNotesSetting.Value) ? string.Empty : tktType.CancellationNotesSetting.Value);
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            var isangoErrorEntity = new IsangoErrorEntity
                                            {
                                                ClassName = "GlobalTixPersistence",
                                                MethodName = "SaveAllActivities",
                                            };
                                            continue;
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                var isangoErrorEntity = new IsangoErrorEntity
                                {
                                    ClassName = "GlobalTixPersistence",
                                    MethodName = "SaveAllActivities",
                                };
                                _log.Error(isangoErrorEntity, ex);
                              
                                continue;
                            }
                        }
                        catch (Exception ex)
                        {
                            var isangoErrorEntity = new IsangoErrorEntity
                            {
                                ClassName = "GlobalTixPersistence",
                                MethodName = "SaveAllActivities",
                            };
                            _log.Error(isangoErrorEntity, ex);
                            continue;
                        }
                    }
                    //DataView viewOptTable = new DataView(prodOptTable);
                    //DataTable distinctprodOptTable = viewOptTable.ToTable(true, "Product_Id", "Option_Id");

                    //DataView viewOptTktTable = new DataView(prodOptTktTable);
                    //DataTable distinctprodOptTktTable = viewOptTktTable.ToTable(true, "Product_Id", "Option_Id", "Ticket_Id");

                    //foreach (DataRow row in distinctprodOptTktTable.Rows)
                    //{
                    //    foreach (DataColumn col in distinctprodOptTktTable.Columns)
                    //    {
                    //        if (row.IsNull(col) && col.DataType == typeof(string))
                    //            row.SetField(col, String.Empty);
                    //    }
                    //}
                    WriteProductDataToDB(conn, prodTable, prodOptTable, prodOptTktTable);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "GlobalTixPersistence",
                    MethodName = "SaveAllActivities",
                    Params = $"{SerializeDeSerializeHelper.Serialize(gtActivities)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }



        //public void SaveAllActivitiesV3(List<Isango.Entities.GlobalTixV3.ProductList> gtActivities, List<Isango.Entities.GlobalTixV3.ProductOptionV3> gtProductOption, List<Tickettype> tickettypes)
        //{
        //    try

        //    {
        //        using (SqlConnection conn = new SqlConnection(_connString))
        //        {
        //            conn.Open();

        //            //DeleteActivitiesDataFromTables(conn);

        //            DataTable prodTable = GetDataTableForProductListV3();
        //            DataTable prodOptTable = GetDataTableForProductOptionV3();
        //            DataTable prodOptTktTable = GetDataTableForProductOptionTicketV3();

        //            foreach (Isango.Entities.GlobalTixV3.ProductList actData in gtActivities)
        //            {

        //                try
        //                {

        //                    prodTable.Rows.Add(actData.Id, 
        //                        actData.Name, 
        //                        actData.Currency,
        //                        actData.City, 
        //                        actData.OriginalPrice, 
        //                        actData.Country,
        //                        actData.IsOpenDated,
        //                        actData.IsOwnContracted, 
        //                        actData.IsFavorited, 
        //                        actData.IsBestSeller, 
        //                        actData.IsInstantConfirmation, 
        //                        actData.Category);

        //            }
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
        //            try
        //            {
        //                foreach (Isango.Entities.GlobalTixV3.ProductOptionV3 optionData in gtProductOption)
        //                {
        //                    prodOptTable.Rows.Add(optionData.Id
        //                        , optionData.Name
        //                        , optionData.Description
        //                        , optionData.Currency
        //                        , optionData.TicketValidity
        //                        , optionData.TicketFormat
        //                        , optionData.IsCapacity
        //                        , optionData.TimeSlot
        //                        , optionData.IsCancellable
        //                        , optionData.Type
        //                        , optionData.DemandType);

        //                    try
        //                    {
        //                        foreach (var tt in tickettypes)
        //                        {
        //                            try
        //                            {
        //                                prodOptTktTable.Rows.Add(tt.Id
        //                                    , optionData.Id
        //                                    , tt.Name
        //                                    , tt.Sku
        //                                    , tt.AgeFrom
        //                                    , tt.AgeTo
        //                                    , tt.NettPrice
        //                                    , tt.MinimumMerchantSellingPrice
        //                                    , tt.MinimumSellingPrice
        //                                    , tt.RecommendedSellingPrice
        //                                    , tt.OriginalPrice);
        //                            }
        //                            catch(Exception ex)
        //                            {

        //                            }
        //                        }
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        var isangoErrorEntity = new IsangoErrorEntity
        //                        {
        //                            ClassName = "GlobalTixPersistence",
        //                            MethodName = "SaveAllActivities",
        //                        };
        //                        _log.Error(isangoErrorEntity, ex);

        //                        continue;
        //                    }
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

        //                throw;
        //            }
        //            WriteProductDataToDBV3(conn, prodTable, prodOptTable, prodOptTktTable);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        var isangoErrorEntity = new IsangoErrorEntity
        //        {
        //            ClassName = "GlobalTixPersistence",
        //            MethodName = "SaveAllActivities",
        //            Params = $"{SerializeDeSerializeHelper.Serialize(gtActivities)}"
        //        };
        //        _log.Error(isangoErrorEntity, ex);
        //        throw;
        //    }
        //}

        public void SaveAllActivitiesV3(List<Isango.Entities.GlobalTixV3.ProductList> gtActivities, List<Isango.Entities.GlobalTixV3.ProductOptionV3> gtProductOption, List<Tickettype> tickettypes)
        {
            try
            {
				

				using (SqlConnection conn = new SqlConnection(_connString))
                {
                    conn.Open();
					//Delete Existing Record form the DB First
					DeleteActivitiesDataFromTablesGlobalTixV3(conn);

					DataTable prodTable = GetDataTableForProductListV3();
                    DataTable prodOptTable = GetDataTableForProductOptionV3();
                    DataTable prodOptTktTable = GetDataTableForProductOptionTicketV3();

                    foreach (Isango.Entities.GlobalTixV3.ProductList actData in gtActivities)
                    {
                        try
                        {
                            prodTable.Rows.Add(actData.Id,
                                        actData.Name,
                                        actData.Currency,
                                        actData.City,
                                        actData.OriginalPrice,
                                        actData.Country,
                                        actData.IsOpenDated,
                                        actData.IsOwnContracted,
                                        actData.IsFavorited,
                                        actData.IsBestSeller,
                                        actData.IsInstantConfirmation,
                                        actData.Category);
                        }
                        catch (Exception ex)
                        {
                            var isangoErrorEntity = new IsangoErrorEntity
                            {
                                ClassName = "GlobalTixPersistence",
                                MethodName = "SaveAllActivities",
                            };
                            _log.Error(isangoErrorEntity, ex);
                            continue;
                        }
                    }

                    foreach (Isango.Entities.GlobalTixV3.ProductOptionV3 optionData in gtProductOption)
                    {
                        try
                        {
                            prodOptTable.Rows.Add(optionData.Id 
                                        , optionData.Name
                                        , optionData.Description
                                        , optionData.Currency
                                        , optionData.TicketValidity
                                        , optionData.TicketFormat
                                        , optionData.IsCapacity
                                        , optionData.TimeSlot
                                        , optionData.IsCancellable
                                        , optionData.Type
                                        , optionData.DemandType
                                        ,optionData.OptionId);




                        }
                        catch (Exception ex)
                        {
                            var isangoErrorEntity = new IsangoErrorEntity
                            {
                                ClassName = "GlobalTixPersistence",
                                MethodName = "SaveAllActivities",
                            };
                            _log.Error(isangoErrorEntity, ex);

                            
                        }
                    }

                    const int batchSize = 100; // Adjust batch size as needed
                    HashSet<int> uniqueIds = new HashSet<int>(); // Create a HashSet to track unique IDs


                    try
                    {
                        for (int i = 0; i < tickettypes.Count; i += batchSize)
                        {
                            int endIndex = Math.Min(i + batchSize, tickettypes.Count);

                            for (int j = i; j < endIndex; j++)
                            {
                                var tt = tickettypes[j];

                                // Check if the ID is unique before adding it to prodOptTktTable
                                if (!uniqueIds.Contains(tt.ParentProductOptionId))
                                {
                                    try
                                    {
                                        prodOptTktTable.Rows.Add(tt.ParentProductOptionId, // OptionId
                                            tt.TicketType_Id, // TicketType_Id
                                            tt.Sku,
                                            tt.Name,
                                            tt.OriginalPrice,
                                            tt.AgeFrom,
                                            tt.AgeTo,
                                            tt.NettPrice,
                                            tt.MinimumMerchantSellingPrice,
                                            tt.MinimumSellingPrice,
                                            tt.RecommendedSellingPrice,
                                            tt.OptionId); // done // ParentProductOptionId

                                        // Add the ID to the HashSet to mark it as processed
                                        uniqueIds.Add(tt.ParentProductOptionId);
                                    }
                                    catch (Exception ex)
                                    {
                                        _log.Error($"AgeGroupService|SaveGlobaltixV3AgeGroup_TicketType", ex);

                                    }
                                }
                            }

                            // Write prodOptTktTable data to the database after each batch
                            WriteProductTicketsDataToDBV3(conn, prodOptTktTable);
                            prodOptTktTable.Clear(); // Clear the DataTable to free up memory
                        }
                    }
                    catch (Exception ex)
                    {
                        _log.Error($"AgeGroupService|SaveGlobaltixV3AgeGroup_TicketType", ex);

                    }

                    // Write the remaining data to the database after loops complete
                    WriteProductDataToDBV3(conn, prodTable, prodOptTable);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "GlobalTixPersistence",
                    MethodName = "SaveAllActivities",
                    Params = $"{SerializeDeSerializeHelper.Serialize(gtActivities)}"
                };
                _log.Error(isangoErrorEntity, ex);
                
            }
        }


        public void SaveAllPackages(List<GlobalTixPackage> gtPackages)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    conn.Open();

                    DeletePackagesDataFromTables(conn);

                    DataTable prodTable = GetDataTableForProduct();
                    DataTable prodOptTable = GetDataTableForProductOption();
                    DataTable prodOptTktTable = GetDataTableForProductOptionTicket();

                    int idx = 0;
                    List<string> processedPackages = new List<string>();
                    while (idx > -1)
                    {
                        GlobalTixPackage pkg = gtPackages[idx];
                        processedPackages.Add(pkg.Id.ToString());
                        //string pfxPkgId = $"{PersistenceConstants.Package_Prefix}{pkg.Id}";
                        string pfxPkgId = $"{pkg.Id}";

                        // Packages do not have any country or city information. Therefore set columns at index 2,3,4,5 as empty
                        prodTable.Rows.Add(pfxPkgId, true, string.Empty, string.Empty, string.Empty, string.Empty, pkg.Name, pkg.Desc, string.Empty);
                        prodOptTable.Rows.Add(pfxPkgId, pkg.LinkId, false, pkg.Name, "");
                        prodOptTktTable.Rows.Add(pfxPkgId, pkg.LinkId, pkg.Id, pkg.Name, pkg.PaxType, pkg.CurrencyCode, (pkg.Price) ?? 0);


                        // Retrieve detailed package information to check related packages. In GlobalTix packages,
                        // there is one package for each pax type. These related packages are linked together using 
                        // package element 'linkId'.
                        if (pkg.RelatedPackages != null && pkg.RelatedPackages.Count > 0)
                        {
                            foreach (int relatedPackage in pkg.RelatedPackages)
                            {
                                GlobalTixPackage relPkg = gtPackages.FirstOrDefault(x => x.Id == relatedPackage);
                                if (relPkg != null)
                                {
                                    prodOptTktTable.Rows.Add(pfxPkgId, pkg.LinkId, relPkg.Id, relPkg.Name, relPkg.PaxType, relPkg.CurrencyCode, (relPkg.Price) ?? 0);
                                    processedPackages.Add(relPkg.Id.ToString());
                                }
                            }
                        }

                        // Get next index for processing. Any packages that are already processed will be skipped.
                        idx = gtPackages.FindIndex(x => processedPackages.Contains(x.Id.ToString()) == false);
                    }

                    // Write product data to database tables
                    WriteProductDataToDB(conn, prodTable, prodOptTable, prodOptTktTable);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "GlobalTixPersistence",
                    MethodName = "SaveAllPackages",
                    Params = $"{SerializeDeSerializeHelper.Serialize(gtPackages)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public void SaveCountryCityList(List<Entities.GlobalTix.CountryCity> countryCities)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    DataTable countryCityTable = GetDataTableForCountryCity();
                    foreach (Entities.GlobalTix.CountryCity countryCity in countryCities)
                    {
                        using (var command = APIUploadDb.GetStoredProcCommand(Constant.uspinsGlobalTixCountryCity))
                        {
                            APIUploadDb.AddInParameter(command, Constant.CountryId, DbType.String, countryCity.CountryId);
                            APIUploadDb.AddInParameter(command, Constant.CityId, DbType.String, countryCity.CityId);
                            APIUploadDb.AddInParameter(command, Constant.CountryName, DbType.String, countryCity.CountryName);
                            APIUploadDb.AddInParameter(command, Constant.CityName, DbType.String, countryCity.CityName);
                            APIUploadDb.ExecuteNonQuery(command);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "GlobalTixPersistence",
                    MethodName = "SaveCountryCityList",
                    Params = $"{SerializeDeSerializeHelper.Serialize(countryCities)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }
        public void SaveCountryCityListV3(List<Entities.GlobalTixV3.CountryCityV3> countryCities)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    DataTable countryCityTable = GetDataTableForCountryCity();
                    foreach (Entities.GlobalTixV3.CountryCityV3 countryCity in countryCities)
                    {
                        using (var command = APIUploadDb.GetStoredProcCommand(Constant.uspinsGlobalTixCountryCityV3))
                        {
                            APIUploadDb.AddInParameter(command, ServiceAdapters.GlobalTixV3.Constants.Constant.CountryId, DbType.String, countryCity.CountryId);
                            APIUploadDb.AddInParameter(command, ServiceAdapters.GlobalTixV3.Constants.Constant.CityId, DbType.String, countryCity.CityId);
                            APIUploadDb.AddInParameter(command, ServiceAdapters.GlobalTixV3.Constants.Constant.CountryName, DbType.String, countryCity.CountryName);
                            APIUploadDb.AddInParameter(command, ServiceAdapters.GlobalTixV3.Constants.Constant.CityName, DbType.String, countryCity.CityName);
                            APIUploadDb.ExecuteNonQuery(command);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "GlobalTixPersistence",
                    MethodName = "SaveCountryCityListV3",
                    Params = $"{SerializeDeSerializeHelper.Serialize(countryCities)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public void SaveProductInfoListV3(List<Entities.GlobalTixV3.ProductInfoV3> productInfoV3)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    //DataTable countryCityTable = GetDataTableForCountryCity();
                    foreach (Entities.GlobalTixV3.ProductInfoV3 productInfo in productInfoV3)
                    {
                        using (var command = APIUploadDb.GetStoredProcCommand(Constant.uspinsGlobalTixProductInfoV3))
                        {
                            APIUploadDb.AddInParameter(command, ServiceAdapters.GlobalTixV3.Constants.Constant.Country, DbType.String, productInfo.country);
                            APIUploadDb.AddInParameter(command, ServiceAdapters.GlobalTixV3.Constants.Constant.OriginalPrice, DbType.String, productInfo.originalPrice);
                            //APIUploadDb.AddInParameter(command, ServiceAdapters.GlobalTixV3.Constants.Constant.BlockeDate, DbType.String, productInfo.blockedDate.ToString());
                            List<object> blockedDateList = productInfo.blockedDate; // Assuming blockedDate is a List<object>

                            string blockedDateValue = blockedDateList.Count > 0 ? blockedDateList[0].ToString() : string.Empty;

                            APIUploadDb.AddInParameter(command, ServiceAdapters.GlobalTixV3.Constants.Constant.BlockeDate, DbType.String, blockedDateValue);

                            APIUploadDb.AddInParameter(command, ServiceAdapters.GlobalTixV3.Constants.Constant.FromPrice, DbType.String, productInfo.fromPrice);
                            APIUploadDb.AddInParameter(command, ServiceAdapters.GlobalTixV3.Constants.Constant.City, DbType.String, productInfo.city);
                            APIUploadDb.AddInParameter(command, ServiceAdapters.GlobalTixV3.Constants.Constant.Description, DbType.String, productInfo.description);
                            APIUploadDb.AddInParameter(command, ServiceAdapters.GlobalTixV3.Constants.Constant.CountryId, DbType.String, productInfo.countryId);
                            APIUploadDb.AddInParameter(command, ServiceAdapters.GlobalTixV3.Constants.Constant.Currency, DbType.String, productInfo.currency);
                            APIUploadDb.AddInParameter(command, ServiceAdapters.GlobalTixV3.Constants.Constant.Id, DbType.String, productInfo.id);
                            APIUploadDb.AddInParameter(command, ServiceAdapters.GlobalTixV3.Constants.Constant.IsGTRecommend, DbType.String, productInfo.isGTRecommend);
                            APIUploadDb.AddInParameter(command, ServiceAdapters.GlobalTixV3.Constants.Constant.IsOpenDated, DbType.String, productInfo.isOpenDated);
                            APIUploadDb.AddInParameter(command, ServiceAdapters.GlobalTixV3.Constants.Constant.IsOwnContracted, DbType.String, productInfo.isOwnContracted);
                            APIUploadDb.AddInParameter(command, ServiceAdapters.GlobalTixV3.Constants.Constant.IsFavorited, DbType.String, productInfo.isFavorited);
                            APIUploadDb.AddInParameter(command, ServiceAdapters.GlobalTixV3.Constants.Constant.IsBestSeller, DbType.String, productInfo.isBestSeller);
                            APIUploadDb.AddInParameter(command, ServiceAdapters.GlobalTixV3.Constants.Constant.FromReseller, DbType.String, productInfo.fromReseller);
                            APIUploadDb.AddInParameter(command, ServiceAdapters.GlobalTixV3.Constants.Constant.Name, DbType.String, productInfo.name);
                            APIUploadDb.AddInParameter(command, ServiceAdapters.GlobalTixV3.Constants.Constant.IsInstantConfirmation, DbType.String, productInfo.isInstantConfirmation);
                            APIUploadDb.AddInParameter(command, ServiceAdapters.GlobalTixV3.Constants.Constant.Location, DbType.String, productInfo.location);
                            APIUploadDb.AddInParameter(command, ServiceAdapters.GlobalTixV3.Constants.Constant.Category, DbType.String, productInfo.category);


                            APIUploadDb.ExecuteNonQuery(command);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "GlobalTixPersistence",
                    MethodName = "SaveProductInfoListV3",
                    Params = $"{SerializeDeSerializeHelper.Serialize(productInfoV3)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }
        public void SaveSaveGlobalTixCategoriesListV3(List<Entities.GlobalTixV3.GlobalTixCategoriesV3> categoriesV3List)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    conn.Open();

                    foreach (Entities.GlobalTixV3.GlobalTixCategoriesV3 category in categoriesV3List)
                    {
                        using (var command = APIUploadDb.GetStoredProcCommand(ServiceAdapters.GlobalTixV3.Constants.Constant.uspinsGlobalTixCategoriesV3))
                        {
                            APIUploadDb.AddInParameter(command, ServiceAdapters.GlobalTixV3.Constants.Constant.CategoryId, DbType.String, category.Id);
                            APIUploadDb.AddInParameter(command, ServiceAdapters.GlobalTixV3.Constants.Constant.CategoryName, DbType.String, category.Name);
                            APIUploadDb.ExecuteNonQuery(command);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "GlobalTixPersistence",
                    MethodName = "SaveSaveGlobalTixCategoriesListV3",
                    Params = $"{SerializeDeSerializeHelper.Serialize(categoriesV3List)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public void SaveGlobalTixProductChangesV3(List<Entities.GlobalTixV3.ProductChangesV3> ProductChangesV3List)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    conn.Open();

                    foreach (Entities.GlobalTixV3.ProductChangesV3 Product in ProductChangesV3List)
                    {
                        using (var command = APIUploadDb.GetStoredProcCommand(ServiceAdapters.GlobalTixV3.Constants.Constant.uspinsGlobalTixProductChangesV3))
                        {
                            APIUploadDb.AddInParameter(command, ServiceAdapters.GlobalTixV3.Constants.Constant.Id, DbType.String, Product.id);
                            APIUploadDb.AddInParameter(command, ServiceAdapters.GlobalTixV3.Constants.Constant.Name, DbType.String, Product.name);
                            APIUploadDb.AddInParameter(command, ServiceAdapters.GlobalTixV3.Constants.Constant.LastUpdated, DbType.DateTime, Product.lastUpdated);
                            APIUploadDb.AddInParameter(command, ServiceAdapters.GlobalTixV3.Constants.Constant.CityId, DbType.String, Product.cityId);
                            APIUploadDb.AddInParameter(command, ServiceAdapters.GlobalTixV3.Constants.Constant.CountryId, DbType.String, Product.countryId);
                            APIUploadDb.ExecuteNonQuery(command);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "GlobalTixPersistence",
                    MethodName = "SaveGlobalTixProductChangesV3",
                    Params = $"{SerializeDeSerializeHelper.Serialize(ProductChangesV3List)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public void SaveGlobalTixPackageOptionsV3(List<Entities.GlobalTixV3.PackageOptionsV3> packageOptionsV3List)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    conn.Open();

                    foreach (Entities.GlobalTixV3.PackageOptionsV3 Product in packageOptionsV3List)
                    {
                        using (var command = APIUploadDb.GetStoredProcCommand(ServiceAdapters.GlobalTixV3.Constants.Constant.uspinsGlobalTixPackageOptionsV3))
                        {
                            APIUploadDb.AddInParameter(command, ServiceAdapters.GlobalTixV3.Constants.Constant.Id, DbType.String, Product.id);
                            APIUploadDb.AddInParameter(command, ServiceAdapters.GlobalTixV3.Constants.Constant.Name, DbType.String, Product.name);
                            APIUploadDb.AddInParameter(command, ServiceAdapters.GlobalTixV3.Constants.Constant.Image, DbType.String, Product.image);
                            APIUploadDb.AddInParameter(command, ServiceAdapters.GlobalTixV3.Constants.Constant.Description, DbType.String, Product.description);
                            APIUploadDb.AddInParameter(command, ServiceAdapters.GlobalTixV3.Constants.Constant.TermsAndConditions, DbType.String, Product.termsAndConditions);
                            APIUploadDb.AddInParameter(command, ServiceAdapters.GlobalTixV3.Constants.Constant.Currency, DbType.String, Product.currency);
                            APIUploadDb.AddInParameter(command, ServiceAdapters.GlobalTixV3.Constants.Constant.PublishStart, DbType.String, Product.publishStart);
                            APIUploadDb.AddInParameter(command, ServiceAdapters.GlobalTixV3.Constants.Constant.publishEnd, DbType.String, Product.publishEnd);
                            APIUploadDb.AddInParameter(command, ServiceAdapters.GlobalTixV3.Constants.Constant.redeemStart, DbType.String, Product.redeemStart);
                            APIUploadDb.AddInParameter(command, ServiceAdapters.GlobalTixV3.Constants.Constant.redeemEnd, DbType.String, Product.redeemEnd);
                            APIUploadDb.AddInParameter(command, ServiceAdapters.GlobalTixV3.Constants.Constant.ticketValidity, DbType.String, Product.ticketValidity);
                            APIUploadDb.AddInParameter(command, ServiceAdapters.GlobalTixV3.Constants.Constant.ticketFormat, DbType.String, Product.ticketFormat);
                            APIUploadDb.AddInParameter(command, ServiceAdapters.GlobalTixV3.Constants.Constant.definedDuration, DbType.String, Product.definedDuration);
                            APIUploadDb.AddInParameter(command, ServiceAdapters.GlobalTixV3.Constants.Constant.IsFavorited, DbType.String, Product.isFavorited);
                            APIUploadDb.AddInParameter(command, ServiceAdapters.GlobalTixV3.Constants.Constant.FromReseller, DbType.String, Product.fromReseller);
                            APIUploadDb.AddInParameter(command, ServiceAdapters.GlobalTixV3.Constants.Constant.sourceName, DbType.String, Product.sourceName);
                            APIUploadDb.AddInParameter(command, ServiceAdapters.GlobalTixV3.Constants.Constant.sourceTitle, DbType.String, Product.sourceTitle);
                            APIUploadDb.AddInParameter(command, ServiceAdapters.GlobalTixV3.Constants.Constant.isAdditionalBookingInfo, DbType.String, Product.isAdditionalBookingInfo);
                            APIUploadDb.AddInParameter(command, ServiceAdapters.GlobalTixV3.Constants.Constant.keywords, DbType.String, Product.keywords);

                            APIUploadDb.ExecuteNonQuery(command);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "GlobalTixPersistence",
                    MethodName = "SaveGlobalTixPackageOptionsV3",
                    Params = $"{SerializeDeSerializeHelper.Serialize(packageOptionsV3List)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public void SaveGlobalTixPackageTypeIdV3(List<Entities.GlobalTixV3.PackageOptionsV3.PackageType> PackageTypeIdV3List)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    conn.Open();

                    foreach (Entities.GlobalTixV3.PackageOptionsV3.PackageType Product in PackageTypeIdV3List)
                    {
                        using (var command = APIUploadDb.GetStoredProcCommand(ServiceAdapters.GlobalTixV3.Constants.Constant.usp_ins_GlobalTix_PackageTypeIdV3))
                        {
                            APIUploadDb.AddInParameter(command, ServiceAdapters.GlobalTixV3.Constants.Constant.PackagetypeparentId, DbType.String, Product.PackagetypeparentId);
                            APIUploadDb.AddInParameter(command, ServiceAdapters.GlobalTixV3.Constants.Constant.PackageType_Id, DbType.String, Product.PackageType_id);
                            APIUploadDb.AddInParameter(command, ServiceAdapters.GlobalTixV3.Constants.Constant.PackageType_Sku, DbType.String, Product.PackageType_sku);
                            APIUploadDb.AddInParameter(command, ServiceAdapters.GlobalTixV3.Constants.Constant.PackageType_Name, DbType.String, Product.PackageType_name);
                            APIUploadDb.AddInParameter(command, ServiceAdapters.GlobalTixV3.Constants.Constant.PackageType_NettPrice, DbType.String, Product.PackageType_nettPrice);
                            APIUploadDb.AddInParameter(command, ServiceAdapters.GlobalTixV3.Constants.Constant.PackageType_SettlementRate, DbType.String, Product.PackageType_settlementRate);
                            APIUploadDb.AddInParameter(command, ServiceAdapters.GlobalTixV3.Constants.Constant.PackageType_OriginalPrice, DbType.String, Product.PackageType_originalPrice);
                            APIUploadDb.AddInParameter(command, ServiceAdapters.GlobalTixV3.Constants.Constant.PackageType_IssuanceLimit, DbType.String, Product.PackageType_issuanceLimit);


                            APIUploadDb.ExecuteNonQuery(command);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "GlobalTixPersistence",
                    MethodName = "SaveGlobalTixPackageTypeIdV3",
                    Params = $"{SerializeDeSerializeHelper.Serialize(PackageTypeIdV3List)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }

        }
        public List<Entities.GlobalTix.CountryCity> GetCountryCityList()
        {
            var countryCityList = new List<Entities.GlobalTix.CountryCity>();
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    using (var command = APIUploadDb.GetStoredProcCommand(Constant.uspgetGlobalTixCountryCity))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        using (var reader = APIUploadDb.ExecuteReader(command))
                        {
                            while (reader.Read())
                            {
                                countryCityList.Add(GetContryCityMappingData(reader));
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "GlobalTixPersistence",
                    MethodName = "GetCountryCityList",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return countryCityList;
        }

        public List<Entities.GlobalTixV3.CountryCityV3> GetCountryCityV3(int maxRecords)
        {
            var countryCityList = new List<Entities.GlobalTixV3.CountryCityV3>();
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    using (var command = APIUploadDb.GetStoredProcCommand(Constant.uspgetGlobalTixCountryCityV3))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        using (var reader = APIUploadDb.ExecuteReader(command))
                        {
                            int recordCount = 0;  // Keep track of retrieved records

                            while (reader.Read() && recordCount < maxRecords)
                            {
                                countryCityList.Add(GetContryCityMappingDataV3(reader));
                                recordCount++;
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "GlobalTixPersistence",
                    MethodName = "GetCountryCityList",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return countryCityList;
        }

        public Entities.GlobalTix.CountryCity GetContryCityMappingData(IDataReader reader)
        {
            var mapping = new Entities.GlobalTix.CountryCity();
            try
            {
                mapping = new Entities.GlobalTix.CountryCity
                {
                    CountryId = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "Country_Id"),
                    CityId = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "City_Id"),
                    CityName = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "City_Name"),
                    CountryName = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "Country_Name")
                };
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "GlobalTixPersistence",
                    MethodName = "GetContryCityMappingData",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return mapping;
        }

        public Entities.GlobalTixV3.CountryCityV3 GetContryCityMappingDataV3(IDataReader reader)
        {
            var mapping = new Entities.GlobalTixV3.CountryCityV3();
            try
            {
                mapping = new Entities.GlobalTixV3.CountryCityV3
                {
                    CountryId = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "Country_Id"),
                    CityId = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "City_Id"),
                    CityName = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "City_Name"),
                    CountryName = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "Country_Name")
                };
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "GlobalTixPersistence",
                    MethodName = "GetContryCityMappingData",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return mapping;
        }



        public void UpdateGlobalTixV3Data()
        {

            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    conn.Open();

                    using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.Update_GlobalV3TixData))
                    {
                        IsangoDataBaseLive.ExecuteNonQuery(command);
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "TiqetsPersistence",
                    MethodName = "UpdateGlobalTixV3Data",

                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
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

        private DataTable GetDataTableForProduct()
        {
            DataTable prodTable = new DataTable();
            try
            {
                prodTable.Columns.Add(PersistenceConstants.Column_ProductId, typeof(string));
                prodTable.Columns.Add(PersistenceConstants.Column_IsPackage, typeof(bool));
                prodTable.Columns.Add(PersistenceConstants.Column_CountryId, typeof(string));
                prodTable.Columns.Add(PersistenceConstants.Column_CountryName, typeof(string));
                prodTable.Columns.Add(PersistenceConstants.Column_CityId, typeof(string));
                prodTable.Columns.Add(PersistenceConstants.Column_CityName, typeof(string));
                prodTable.Columns.Add(PersistenceConstants.Column_ProductTitle, typeof(string));
                prodTable.Columns.Add(PersistenceConstants.Column_ProductDesc, typeof(string));
                prodTable.Columns.Add(PersistenceConstants.Column_HoursOfOp, typeof(string));
                prodTable.Columns.Add(PersistenceConstants.Column_Latitude, typeof(string));
                prodTable.Columns.Add(PersistenceConstants.Column_Longitude, typeof(string));
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "GlobalTixPersistence",
                    MethodName = "GetDataTableForProduct",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return prodTable;
        }

        private DataTable GetDataTableForProductOption()
        {
            DataTable prodOptTable = new DataTable();
            try
            {
                prodOptTable.Columns.Add(PersistenceConstants.Column_ProductId, typeof(string));
                prodOptTable.Columns.Add(PersistenceConstants.Column_OptionId, typeof(string));
                prodOptTable.Columns.Add(PersistenceConstants.Column_ApplyCapacity, typeof(bool));
                prodOptTable.Columns.Add(PersistenceConstants.Column_OptionName, typeof(string));
                prodOptTable.Columns.Add(PersistenceConstants.Column_OptionDesc, typeof(string));
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "GlobalTixPersistence",
                    MethodName = "GetDataTableForProductOption",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return prodOptTable;
        }

        private DataTable GetDataTableForProductListV3()
        {
            DataTable prodTable = new DataTable();
            try
            {
                prodTable.Columns.Add(PersistenceConstants.Column_ProductId, typeof(int));
                prodTable.Columns.Add(PersistenceConstants.Column_Name, typeof(string));
                prodTable.Columns.Add(PersistenceConstants.Column_Currency, typeof(string));
                prodTable.Columns.Add(PersistenceConstants.Column_City, typeof(string));
                prodTable.Columns.Add(PersistenceConstants.Column_OriginalPrice, typeof(float));
                prodTable.Columns.Add(PersistenceConstants.Column_Country, typeof(string));
                prodTable.Columns.Add(PersistenceConstants.Column_IsOpenDated, typeof(bool));
                prodTable.Columns.Add(PersistenceConstants.Column_IsOwnContracted, typeof(bool));
                prodTable.Columns.Add(PersistenceConstants.Column_IsFavorited, typeof(bool));
                prodTable.Columns.Add(PersistenceConstants.Column_IsBestSeller, typeof(bool));
                prodTable.Columns.Add(PersistenceConstants.Column_IsInstantConfirmation, typeof(bool));
                prodTable.Columns.Add(PersistenceConstants.Column_Category, typeof(string));
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "GlobalTixPersistence",
                    MethodName = "GetDataTableForProduct",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return prodTable;
        }

        private DataTable GetDataTableForProductOptionV3()
        {
            DataTable prodOptTable = new DataTable();
            try
            {
                prodOptTable.Columns.Add(PersistenceConstants.Column_ProductId, typeof(int));
                prodOptTable.Columns.Add(PersistenceConstants.Column_Name, typeof(string));
                prodOptTable.Columns.Add(PersistenceConstants.Column_Description, typeof(string));
                prodOptTable.Columns.Add(PersistenceConstants.Column_Currency, typeof(string));
                prodOptTable.Columns.Add(PersistenceConstants.Column_TicketValidity, typeof(string));
                prodOptTable.Columns.Add(PersistenceConstants.Column_TicketFormat, typeof(string));
                prodOptTable.Columns.Add(PersistenceConstants.Column_IsCapacity, typeof(bool));
                prodOptTable.Columns.Add(PersistenceConstants.Column_TimeSlot, typeof(string));
                prodOptTable.Columns.Add(PersistenceConstants.Column_IsCancellable, typeof(bool));
                prodOptTable.Columns.Add(PersistenceConstants.Column_Type, typeof(string));
                prodOptTable.Columns.Add(PersistenceConstants.Column_DemandType, typeof(string));
                prodOptTable.Columns.Add(PersistenceConstants.Column_OptionId, typeof(int));

            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "GlobalTixPersistence",
                    MethodName = "GetDataTableForProductOption",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return prodOptTable;
        }
        private DataTable GetDataTableForProductOptionTicketV3()
        {
            DataTable prodOptTktTable = new DataTable();
            try
            {
                //prodOptTktTable.Columns.Add(PersistenceConstants.Column_ProductId, typeof(string));
                //prodOptTktTable.Columns.Add(PersistenceConstants.Column_OptionId, typeof(string));
                prodOptTktTable.Columns.Add(PersistenceConstants.Column_Id, typeof(int));
                prodOptTktTable.Columns.Add(PersistenceConstants.Column_ProductId, typeof(int));
                prodOptTktTable.Columns.Add(PersistenceConstants.Column_TicketName, typeof(string));
                prodOptTktTable.Columns.Add(PersistenceConstants.Column_Sku, typeof(string));
                prodOptTktTable.Columns.Add(PersistenceConstants.Column_AgeFrom, typeof(int));
                prodOptTktTable.Columns.Add(PersistenceConstants.Column_AgeTo, typeof(int));
                prodOptTktTable.Columns.Add(PersistenceConstants.Column_NettPrice, typeof(decimal));
                prodOptTktTable.Columns.Add(PersistenceConstants.Column_minimumMerchantSellingPrice, typeof(decimal));
                prodOptTktTable.Columns.Add(PersistenceConstants.Column_MinimumSellingPrice, typeof(decimal));
                prodOptTktTable.Columns.Add(PersistenceConstants.Column_recommendedSellingPrice, typeof(decimal));
                prodOptTktTable.Columns.Add(PersistenceConstants.Column_OriginalPriceGt, typeof(decimal));
                prodOptTktTable.Columns.Add(PersistenceConstants.Column_OptionId, typeof(int));


            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "GlobalTixPersistence",
                    MethodName = "GetDataTableForProductOptionTicket",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return prodOptTktTable;
        }
        private DataTable GetDataTableForProductOptionTicket()
        {
            DataTable prodOptTktTable = new DataTable();
            try
            {
                prodOptTktTable.Columns.Add(PersistenceConstants.Column_ProductId, typeof(string));
                prodOptTktTable.Columns.Add(PersistenceConstants.Column_OptionId, typeof(string));
                prodOptTktTable.Columns.Add(PersistenceConstants.Column_TicketId, typeof(string));
                prodOptTktTable.Columns.Add(PersistenceConstants.Column_TicketName, typeof(string));
                prodOptTktTable.Columns.Add(PersistenceConstants.Column_PassengerType, typeof(string));
                prodOptTktTable.Columns.Add(PersistenceConstants.Column_CurrencyCode, typeof(string));
                prodOptTktTable.Columns.Add(PersistenceConstants.Column_Price, typeof(decimal));
                prodOptTktTable.Columns.Add(PersistenceConstants.ToAge, typeof(decimal));
                prodOptTktTable.Columns.Add(PersistenceConstants.FromAge, typeof(decimal));
                prodOptTktTable.Columns.Add(PersistenceConstants.OriginalPrice, typeof(decimal));
                prodOptTktTable.Columns.Add(PersistenceConstants.MinimumSellingPrice, typeof(string));
                prodOptTktTable.Columns.Add(PersistenceConstants.CancellationNoteId, typeof(string));
                prodOptTktTable.Columns.Add(PersistenceConstants.CancellationNoteValue, typeof(string));
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "GlobalTixPersistence",
                    MethodName = "GetDataTableForProductOptionTicket",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return prodOptTktTable;
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

        //GLOBALTIXV3
        private void WriteProductDataToDBV3(SqlConnection conn, DataTable prodTable, DataTable prodOptTable)
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
                        dbBulkCopy.DestinationTableName = PersistenceConstants.DBTable_ProductList;
                        dbBulkCopy.BatchSize = PersistenceConstants.DBTable_WriteBatchSize_Product;
                        dbBulkCopy.WriteToServer(prodTable);

                        dbBulkCopy.DestinationTableName = PersistenceConstants.DBTable_ProductOptionV3;
                        dbBulkCopy.BatchSize = PersistenceConstants.DBTable_WriteBatchSize_ProductOption;
                        dbBulkCopy.WriteToServer(prodOptTable);

                        //dbBulkCopy.DestinationTableName = PersistenceConstants.DBTable_ProductOptionTicketV3;
                        //dbBulkCopy.BatchSize = PersistenceConstants.DBTable_WriteBatchSize_ProductOptionTicket;
                        //dbBulkCopy.WriteToServer(prodOptTktTable);
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

        private void WriteProductTicketsDataToDBV3(SqlConnection conn, DataTable prodOptTktTable)
        {
            try
            {

                if (prodOptTktTable.Rows.Count <= 0)
                {
                    return;
                }

                using (SqlBulkCopy dbBulkCopy = new SqlBulkCopy(conn))
                {
                    try
                    {
                        //dbBulkCopy.DestinationTableName = PersistenceConstants.DBTable_ProductList;
                        //dbBulkCopy.BatchSize = PersistenceConstants.DBTable_WriteBatchSize_Product;
                        //dbBulkCopy.WriteToServer(prodTable);

                        //dbBulkCopy.DestinationTableName = PersistenceConstants.DBTable_ProductOptionV3;
                        //dbBulkCopy.BatchSize = PersistenceConstants.DBTable_WriteBatchSize_ProductOption;
                        //dbBulkCopy.WriteToServer(prodOptTable);

                        dbBulkCopy.DestinationTableName = PersistenceConstants.DBTable_ProductOptionTicketV3;
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

        private void DeleteActivitiesDataFromTables(SqlConnection conn)
        {
            try
            {
                SqlCommand cmdDelActs_ProdOptTkt = new SqlCommand(PersistenceConstants.Cmd_DelActivities_ProductOptionTicket, conn);
                cmdDelActs_ProdOptTkt.ExecuteNonQuery();

                SqlCommand cmdDelActs_ProdOpt = new SqlCommand(PersistenceConstants.Cmd_DelActivities_ProductOption, conn);
                cmdDelActs_ProdOpt.ExecuteNonQuery();

                SqlCommand cmdDelActs_Prod = new SqlCommand(PersistenceConstants.Cmd_DelActivities_Product, conn);
                cmdDelActs_Prod.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "GlobalTixPersistence",
                    MethodName = "DeleteActivitiesDataFromTables",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

		private void DeleteActivitiesDataFromTablesGlobalTixV3(SqlConnection conn)
		{
			try
			{
				SqlCommand cmdDelActs_ProdOptTkt = new SqlCommand(PersistenceConstants.Cmd_DelActivities_ProductOptionTicketGlobalTixV3, conn);
				cmdDelActs_ProdOptTkt.ExecuteNonQuery();

				SqlCommand cmdDelActs_ProdOpt = new SqlCommand(PersistenceConstants.Cmd_DelActivities_ProductOptionGlobalTixV3, conn);
				cmdDelActs_ProdOpt.ExecuteNonQuery();

				SqlCommand cmdDelActs_Prod = new SqlCommand(PersistenceConstants.Cmd_DelActivities_ProductGlobalTixV3, conn);
				cmdDelActs_Prod.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				var isangoErrorEntity = new IsangoErrorEntity
				{
					ClassName = "GlobalTixPersistence",
					MethodName = "DeleteActivitiesDataFromTablesGlobalTixV3",
				};
				_log.Error(isangoErrorEntity, ex);
				throw;
			}
		}

		private void DeleteCountryCityData(SqlConnection conn)
        {
            try
            {
                SqlCommand cmdDelCtryCity = new SqlCommand(PersistenceConstants.Cmd_TruncTable_CountryCity, conn);
                cmdDelCtryCity.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "GlobalTixPersistence",
                    MethodName = "DeleteCountryCityData",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private void DeletePackagesDataFromTables(SqlConnection conn)
        {
            try
            {
                SqlCommand cmdDelPkgs_ProdOptTkt = new SqlCommand(PersistenceConstants.Cmd_DelPackages_ProductOptionTicket, conn);
                cmdDelPkgs_ProdOptTkt.ExecuteNonQuery();

                SqlCommand cmdDelPkgs_ProdOpt = new SqlCommand(PersistenceConstants.Cmd_DelPackages_ProductOption, conn);
                cmdDelPkgs_ProdOpt.ExecuteNonQuery();

                SqlCommand cmdDelPkgs_Prod = new SqlCommand(PersistenceConstants.Cmd_DelPackages_Product, conn);
                cmdDelPkgs_Prod.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "GlobalTixPersistence",
                    MethodName = "DeletePackagesDataFromTables",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        #endregion Private Methods
    }
}
