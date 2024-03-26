using Isango.Entities;
using Isango.Entities.ConsoleApplication.AgeGroup.FareHarbor;
using Isango.Persistence.Contract;
using Isango.Persistence.Data;
using Logger.Contract;
using System;
using System.Collections.Generic;
using System.Data;
using Util;
using Constant = Isango.Persistence.Constants.Constants;
using Product = Isango.Entities.ConsoleApplication.AgeGroup.FareHarbor.Product;

namespace Isango.Persistence.ConsoleApplication.AgeGroup
{
    public class FareHarborPersistence : PersistenceBase, IFareHarborPersistence
    {
        private readonly ILogger _log;
        public FareHarborPersistence(ILogger log)
        {
            _log = log;
        }
        /// <summary>
        /// Save all customer prototypes
        /// </summary>
        /// <param name="customerPrototypes"></param>
        public void SaveAllCustomerProtoTypes(List<CustomerProtoTypeCustomerType> customerPrototypes)
        {
            try
            {
                if (customerPrototypes?.Count > 0)
                {
                    foreach (var customerPrototype in customerPrototypes)
                    {
                        
                            //Prepare Command
                            using (var command =
                            APIUploadDb.GetStoredProcCommand(Constant.InsUpdCustomerPrototypeCustomerTypeSp))
                            {
                                // Prepare parameter collection
                                APIUploadDb.AddInParameter(command, Constant.Pk, DbType.Int32, customerPrototype.Pk);
                                APIUploadDb.AddInParameter(command, Constant.Total, DbType.Decimal, customerPrototype.Total);
                                APIUploadDb.AddInParameter(command, Constant.FareHarborDisplayName, DbType.String,
                                    customerPrototype.DisplayName);
                                APIUploadDb.AddInParameter(command, Constant.FareHarborServiceId, DbType.Int32, customerPrototype.ServiceId);
                                APIUploadDb.AddInParameter(command, Constant.CustomerTypePk, DbType.Int32,
                                    customerPrototype.CustomerTypePk);
                                APIUploadDb.AddInParameter(command, Constant.Singular, DbType.String, customerPrototype.Singular);
                                APIUploadDb.AddInParameter(command, Constant.Plural, DbType.String, customerPrototype.Plural);
                                APIUploadDb.AddInParameter(command, Constant.Note, DbType.String, customerPrototype.Note);
                                APIUploadDb.AddInParameter(command, Constant.TourPk, DbType.Int32, customerPrototype.TourPk);
                                APIUploadDb.AddInParameter(command, Constant.StartAt, DbType.DateTime, customerPrototype.StartAt);
                                APIUploadDb.AddInParameter(command, Constant.EndAt, DbType.DateTime, customerPrototype.EndAt);
                                APIUploadDb.AddInParameter(command, Constant.Tourminpax, DbType.Int32, customerPrototype.TourMinPartySize);
                                APIUploadDb.AddInParameter(command, Constant.Tourmaxpax, DbType.Int32, customerPrototype.TourMaxPartySize);
                                APIUploadDb.AddInParameter(command, Constant.prototypeminpax, DbType.Int32, customerPrototype.MinPartySize);
                                APIUploadDb.AddInParameter(command, Constant.prototypemaxpax, DbType.Int32, customerPrototype.MaxPartySize);
                                APIUploadDb.ExecuteNonQuery(command);
                            }
                        
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "FareHarborPersistence",
                    MethodName = "SaveAllCustomerProtoTypes",
                    Params = $"{SerializeDeSerializeHelper.Serialize(customerPrototypes)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Save all companies
        /// </summary>
        /// <param name="suppliers"></param>
        public void SaveAllCompanies(List<Supplier> suppliers)
        {
            try
            {
                if (suppliers?.Count > 0)
                {
                    foreach (var supplier in suppliers)
                    {
                        //Prepare Command
                        using (var command = APIUploadDb.GetStoredProcCommand(Constant.InsUpdCompaniesSp))
                        {
                            // Prepare parameter collection
                            APIUploadDb.AddInParameter(command, Constant.Currency, DbType.String, supplier.Currency);
                            APIUploadDb.AddInParameter(command, Constant.ShortName, DbType.String, supplier.ShortName);
                            APIUploadDb.AddInParameter(command, Constant.FareHarborSupplierName, DbType.String, supplier.Name);
                            APIUploadDb.AddInParameter(command, Constant.UserKey, DbType.String, supplier.UserKey);

                            APIUploadDb.ExecuteNonQuery(command);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "FareHarborPersistence",
                    MethodName = "SaveAllCompanies",
                    Params = $"{SerializeDeSerializeHelper.Serialize(suppliers)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Save all company mappings
        /// </summary>
        /// <param name="companiesMapping"></param>
        public void SaveAllCompanyMappings(List<CompaniesMapping> companiesMapping)
        {
            try
            {
                if (companiesMapping?.Count > 0)
                {
                    foreach (var supplier in companiesMapping)
                    {
                        //Prepare Command
                        using (var command = APIUploadDb.GetStoredProcCommand(Constant.InsUpdCompaniesMappingSp))
                        {
                            // Prepare parameter collection
                            APIUploadDb.AddInParameter(command, Constant.ShortName, DbType.String, supplier.ShortName);
                            APIUploadDb.AddInParameter(command, Constant.TourId, DbType.Int32, supplier.TourId);
                            APIUploadDb.AddInParameter(command, Constant.FareHarborSupplierName, DbType.String, supplier.Name);
                            APIUploadDb.AddInParameter(command, Constant.CompanyCancellationPolicy, DbType.String,
                                supplier.CancellationPolicy);
                            APIUploadDb.AddInParameter(command, Constant.CompanyCancellationPolicyHtml, DbType.String,
                                supplier.CancellationPolicyHtml);

                            APIUploadDb.ExecuteNonQuery(command);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "FareHarborPersistence",
                    MethodName = "SaveAllCompanyMappings",
                    Params = $"{SerializeDeSerializeHelper.Serialize(companiesMapping)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Load products
        /// </summary>
        /// <returns></returns>
        public List<Product> LoadProducts()
        {
            var products = new List<Product>();
            try
            {
                using (var cmdStatement = APIUploadDb.GetStoredProcCommand(Constant.GetCompaniesProductsSp))
                {
                    using (var reader = APIUploadDb.ExecuteReader(cmdStatement))
                    {
                        var fareHarborData = new FareHarborData();
                        while (reader.Read())
                        {
                            products.Add(fareHarborData.LoadProductsData(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "FareHarborPersistence",
                    MethodName = "LoadProducts",
                  
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return products;
        }

        /// <summary>
        /// Get FareHarbor user keys
        /// </summary>
        /// <returns></returns>
        public List<FareHarborUserKey> GetUserKeys()
        {
            var keys = new List<FareHarborUserKey>();
            try
            {
                using (var cmdStatement = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetFareHarborUserKeysSp))
                {
                    using (var reader = IsangoDataBaseLive.ExecuteReader(cmdStatement))
                    {
                        var fareHarborData = new FareHarborData();
                        while (reader.Read())
                        {
                            keys.Add(fareHarborData.GetUserKeysData(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "FareHarborPersistence",
                    MethodName = "GetUserKeys",

                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }

            return keys;
        }

        /// <summary>
        ///  Data synchronization between Isango databases
        /// </summary>
        public void SyncDataBetweenDataBases()
        {
            try
            {
                using (var ageGroupCommand = IsangoDataBaseLive.GetStoredProcCommand(Constant.SyncFareHarborMasterDataSp))
                {
                    IsangoDataBaseLive.ExecuteNonQuery(ageGroupCommand);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "FareHarborPersistence",
                    MethodName = "SyncDataBetweenDataBases",

                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }
    }
}