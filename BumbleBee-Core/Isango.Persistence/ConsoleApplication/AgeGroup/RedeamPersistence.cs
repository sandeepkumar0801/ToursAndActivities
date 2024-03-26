using Isango.Entities;
using Isango.Entities.Redeam;
using Isango.Persistence.Contract;
using Logger.Contract;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Util;
using Constant = Isango.Persistence.Constants.Constants;

namespace Isango.Persistence.ConsoleApplication.AgeGroup
{
    public class RedeamPersistence : PersistenceBase, IRedeamPersistence
    {
        private readonly ILogger _log;
        public RedeamPersistence(ILogger log)
        {
            _log = log;
        }
        /// <summary>
        /// Save Redeam Suppliers in the database
        /// </summary>
        /// <param name="supplierData"></param>
        public void SaveSuppliers(List<SupplierData> supplierData)
        {
            try
            {
                // Calling the generic method to save the data in DB, passing the required parameters
                SaveData(supplierData, Constant.InsertRedeamSuppliersSp, Constant.RedeamSuppliers, Constant.RedeamSuppliersParameter);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "RedeamPersistence",
                    MethodName = "SaveAllActivityAgeGroupsMapping",
                    Params = $"{SerializeDeSerializeHelper.Serialize(supplierData)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }
        public void SaveSuppliersV12(List<Isango.Entities.RedeamV12.SupplierData> supplierData)
        {
            try
            {
                // Calling the generic method to save the data in DB, passing the required parameters
                SaveDataV12(supplierData, Constant.InsertRedeamV12SuppliersSp, Constant.RedeamV12Suppliers, Constant.RedeamSuppliersParameter);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "RedeamPersistence",
                    MethodName = "SaveSuppliersV12",
                    Params = $"{SerializeDeSerializeHelper.Serialize(supplierData)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }
        /// <summary>
        /// Save Redeam Products in the database
        /// </summary>
        /// <param name="productData"></param>
        public void SaveProducts(List<ProductData> productData)
        {
            try
            {
                // Calling the generic method to save the data in DB, passing the required parameters
                SaveData(productData, Constant.InsertRedeamProductsSp, Constant.RedeamProducts, Constant.RedeamProductsParameter);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "RedeamPersistence",
                    MethodName = "SaveProducts",
                    Params = $"{SerializeDeSerializeHelper.Serialize(productData)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }
        public void SaveProductsV12(List<Isango.Entities.RedeamV12.ProductData> productData)
        {
            try
            {
                // Calling the generic method to save the data in DB, passing the required parameters
                SaveData(productData, Constant.InsertRedeamV12ProductsSp, Constant.RedeamV12Products, Constant.RedeamProductsParameter);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "RedeamPersistence",
                    MethodName = "SaveProductsV12",
                    Params = $"{SerializeDeSerializeHelper.Serialize(productData)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Save Redeam Rates in the database
        /// </summary>
        /// <param name="rateData"></param>
        public void SaveRates(List<RateData> rateData)
        {
            try
            {
                // Calling the generic method to save the data in DB, passing the required parameters
                SaveData(rateData, Constant.InsertRedeamRateSp, Constant.RedeamRate, Constant.RedeamRateParameter);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "RedeamPersistence",
                    MethodName = "SaveRates",
                    Params = $"{SerializeDeSerializeHelper.Serialize(rateData)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }
        public void SaveRatesV12(List<Isango.Entities.RedeamV12.RateData> rateData)
        {
            try
            {
                // Calling the generic method to save the data in DB, passing the required parameters
                SaveData(rateData, Constant.InsertRedeamV12RateSp, Constant.RedeamV12Rate, Constant.RedeamRateParameter);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "RedeamPersistence",
                    MethodName = "SaveRates",
                    Params = $"{SerializeDeSerializeHelper.Serialize(rateData)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Save Redeam Prices in the database
        /// </summary>
        /// <param name="priceData"></param>
        public void SavePrices(List<PriceData> priceData)
        {
            try
            {
                // Calling the generic method to save the data in DB, passing the required parameters
                SaveData(priceData, Constant.InsertRedeamPriceSp, Constant.RedeamPrice, Constant.RedeamPriceParameter);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "RedeamPersistence",
                    MethodName = "SavePrices",
                    Params = $"{SerializeDeSerializeHelper.Serialize(priceData)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }
        public void SavePricesV12(List<Isango.Entities.RedeamV12.PriceData> priceData)
        {
            try
            {
                // Calling the generic method to save the data in DB, passing the required parameters
                SaveData(priceData, Constant.InsertRedeamV12PriceSp, Constant.RedeamV12Price, Constant.RedeamPriceParameter);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "RedeamPersistence",
                    MethodName = "SavePrices",
                    Params = $"{SerializeDeSerializeHelper.Serialize(priceData)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }
        /// <summary>
        /// Save TravelerTypes in the database
        /// </summary>
        /// <param name="passengerTypeData"></param>
        public void SaveAgeGroups(List<PassengerTypeData> passengerTypeData)
        {
            try
            {
                // Calling the generic method to save the data in DB, passing the required parameters
                SaveData(passengerTypeData, Constant.InsertRedeamTravelerTypeSp, Constant.RedeamTravelerType, Constant.RedeamTravelerTypeParameter);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "RedeamPersistence",
                    MethodName = "SaveAgeGroups",
                    Params = $"{SerializeDeSerializeHelper.Serialize(passengerTypeData)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }
        public void SaveAgeGroupsV12(List<Isango.Entities.RedeamV12.PassengerTypeData> passengerTypeData)
        {
            try
            {
                // Calling the generic method to save the data in DB, passing the required parameters
                SaveData(passengerTypeData, Constant.InsertRedeamV12TravelerTypeSp, Constant.RedeamV12TravelerType, Constant.RedeamTravelerTypeParameter);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "RedeamPersistence",
                    MethodName = "SaveAgeGroups",
                    Params = $"{SerializeDeSerializeHelper.Serialize(passengerTypeData)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }
        #region Private Methods

        /// <summary>
        /// Generic method to save the data in the database
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="storedProcedureName"></param>
        /// <param name="tableName"></param>
        /// <param name="parameterName"></param>
        public void SaveData<T>(List<T> data, string storedProcedureName, string tableName, string parameterName)
        {
            var dataTable = CreateDataTable(data, tableName);
            try
            {
                using (var connection = new SqlConnection(ConfigurationManagerHelper.GetValuefromConfig(Constant.APIUploadDb)))
                {
                    var insertCommand = new SqlCommand(storedProcedureName, connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    var tvpParam = insertCommand.Parameters.AddWithValue(parameterName, dataTable);
                    tvpParam.SqlDbType = SqlDbType.Structured;

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
                    ClassName = "RedeamPersistence",
                    MethodName = "SaveData",
                    Params = $"{SerializeDeSerializeHelper.Serialize(storedProcedureName)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }
        public void SaveDataV12<T>(List<T> data, string storedProcedureName, string tableName, string parameterName)
        {
            var dataTable = CreateDataTable(data, tableName);
            try
            {
                using (var connection = new SqlConnection(ConfigurationManagerHelper.GetValuefromConfig(Constant.APIUploadDb)))
                {
                    var insertCommand = new SqlCommand(storedProcedureName, connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    var tvpParam = insertCommand.Parameters.AddWithValue(parameterName, dataTable);
                    tvpParam.SqlDbType = SqlDbType.Structured;

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
                    ClassName = "RedeamPersistence",
                    MethodName = "SaveDataV12",
                    Params = $"{SerializeDeSerializeHelper.Serialize(storedProcedureName)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Prepare data table for the given type
        /// </summary>
        /// <param name="data"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private DataTable CreateDataTable<T>(List<T> data, string tableName)
        {
            var dataTable = new DataTable { TableName = tableName };
            try
            {
                foreach (var property in data[0].GetType().GetProperties())
                {
                    dataTable.Columns.Add(new DataColumn(property.Name, property.PropertyType));
                }

                foreach (var value in data)
                {
                    var newRow = dataTable.NewRow();
                    foreach (var property in value.GetType().GetProperties())
                    {
                        newRow[property.Name] = value.GetType().GetProperty(property.Name)
                            ?.GetValue(value, null);
                    }
                    dataTable.Rows.Add(newRow);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "RedeamPersistence",
                    MethodName = "CreateDataTable",
                    Params = $"{SerializeDeSerializeHelper.Serialize(tableName)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }

            return dataTable;
        }

        #endregion Private Methods
    }
}