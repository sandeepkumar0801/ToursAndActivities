using Isango.Entities;
using Isango.Entities.Rayna;
using Isango.Persistence.Contract;
using Logger.Contract;
using ServiceAdapters.Rayna.Rayna.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Util;
using Constant = Isango.Persistence.Constants.Constants;

namespace Isango.Persistence.ConsoleApplication.AgeGroup
{
    public class RaynaPersistence : PersistenceBase, IRaynaPersistence
    {
        private readonly ILogger _log;
        public RaynaPersistence(ILogger log)
        {
            _log = log;
        }

        public void SaveCountryCity(List<CountryCity> countrycityData)
        {
            var productDetailsDataTable = SetProductDetails(countrycityData);
            try
            {
                using (var connection = new SqlConnection(ConfigurationManagerHelper.GetValuefromConfig(Constant.IsangoLiveDb)))
                {
                    var insertCommand = new SqlCommand(Constant.InsertRaynaCountryCitySP, connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    var param = insertCommand.Parameters.AddWithValue(Constant.RaynaCountryCityParameter, productDetailsDataTable);
                    param.SqlDbType = SqlDbType.Structured;
                    param.TypeName = Constant.RaynaCountryCityDBType;
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
                    ClassName = "RaynaPersistence",
                    MethodName = "SaveRaynaProducts",
                    Params = $"{SerializeDeSerializeHelper.Serialize(countrycityData)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public void SaveTourList(List<ResultTour> tourData)
        {
            var tourDataTable = SetTour(tourData);
            try
            {
                using (var connection = new SqlConnection(ConfigurationManagerHelper.GetValuefromConfig(Constant.IsangoLiveDb)))
                {
                    var insertCommand = new SqlCommand(Constant.InsertTourStaticDataSP, connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    var param = insertCommand.Parameters.AddWithValue(Constant.RaynaTourStaticDataParameter, tourDataTable);
                    param.SqlDbType = SqlDbType.Structured;
                    param.TypeName = Constant.RaynaTourStaticDataDBType;
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
                    ClassName = "RaynaPersistence",
                    MethodName = "SaveTourList",
                    Params = $"{SerializeDeSerializeHelper.Serialize(tourData)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }
        public void SaveTourListById(List<ResultTourStaticDataById> tourDataById)
        {
            var tourDataTable = SetTourById(tourDataById);
            try
            {
                using (var connection = new SqlConnection(ConfigurationManagerHelper.GetValuefromConfig(Constant.IsangoLiveDb)))
                {
                    var insertCommand = new SqlCommand(Constant.InsertRaynaTourStaticDataByIdSP, connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    var param = insertCommand.Parameters.AddWithValue(Constant.RaynaTourStaticDataByIdParameter, tourDataTable);
                    param.SqlDbType = SqlDbType.Structured;
                    param.TypeName = Constant.RaynaTourStaticDataByIdDBType;
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
                    ClassName = "RaynaPersistence",
                    MethodName = "SaveTourListById",
                    Params = $"{SerializeDeSerializeHelper.Serialize(tourDataById)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }


        public void SaveTourOptions(List<Touroption> tourOption)
        {
            var tourOptionsDataTable = SetTourOptions(tourOption);
            try
            {
                using (var connection = new SqlConnection(ConfigurationManagerHelper.GetValuefromConfig(Constant.IsangoLiveDb)))
                {
                    var insertCommand = new SqlCommand(Constant.InsertTourOptionsSP, connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    var param = insertCommand.Parameters.AddWithValue(Constant.RaynaTourOptionsParameter, tourOptionsDataTable);
                    param.SqlDbType = SqlDbType.Structured;
                    param.TypeName = Constant.RaynaTourOptionsDBType;
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
                    ClassName = "RaynaPersistence",
                    MethodName = "SaveTourOptions",
                    Params = $"{SerializeDeSerializeHelper.Serialize(tourOption)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public void SaveTourOptionTransferTime(List<TransferTimeTourOption> transferTimeTourOption)
        {
            var tourOptionsDataTable = SetTourOptionsTransferTime(transferTimeTourOption);
            try
            {
                using (var connection = new SqlConnection(ConfigurationManagerHelper.GetValuefromConfig(Constant.IsangoLiveDb)))
                {
                    var insertCommand = new SqlCommand(Constant.InsertTourOptionsTransferTimeSP, connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    var param = insertCommand.Parameters.AddWithValue(Constant.RaynaTourOptionsTransferTimeParameter, tourOptionsDataTable);
                    param.SqlDbType = SqlDbType.Structured;
                    param.TypeName = Constant.RaynaTourOptionsTransferTimeDBType;
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
                    ClassName = "RaynaPersistence",
                    MethodName = "SaveTourListById",
                    Params = $"{SerializeDeSerializeHelper.Serialize(transferTimeTourOption)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }
        #region Private methods


        private DataTable SetProductDetails(List<Isango.Entities.Rayna.CountryCity> products)
        {
            var dataTable = new DataTable { TableName = Constant.RaynaCountryCity };
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
                    ClassName = "RaynaPersistence",
                    MethodName = "SetProductDetails",
                    Params = $"{SerializeDeSerializeHelper.Serialize(products)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return dataTable;
        }

        private DataTable SetTour(List<ResultTour> products)
        {
            var dataTable = new DataTable { TableName = Constant.RaynaTourStaticData };
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
                    ClassName = "RaynaPersistence",
                    MethodName = "SetProductDetails",
                    Params = $"{SerializeDeSerializeHelper.Serialize(products)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return dataTable;
        }
        private DataTable SetTourById(List<ResultTourStaticDataById> products)
        {
            var dataTable = new DataTable { TableName = Constant.RaynaTourStaticDataById };
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
                    ClassName = "RaynaPersistence",
                    MethodName = "SetTourById",
                    Params = $"{SerializeDeSerializeHelper.Serialize(products)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return dataTable;
        }
        private DataTable SetTourOptions(List<ResultTourStaticDataById> products)
        {
            var dataTable = new DataTable { TableName = Constant.RaynaTourStaticDataById };
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
                    ClassName = "RaynaPersistence",
                    MethodName = "SetTourOptions",
                    Params = $"{SerializeDeSerializeHelper.Serialize(products)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return dataTable;
        }

        private DataTable SetTourOptionsTransferTime(List<TransferTimeTourOption> products)
        {
            var dataTable = new DataTable { TableName = Constant.RaynaTourOptionsTransferTime };
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
                    ClassName = "RaynaPersistence",
                    MethodName = "SetTourOptions",
                    Params = $"{SerializeDeSerializeHelper.Serialize(products)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return dataTable;
        }
        private DataTable SetTourOptions(List<Touroption> products)
        {
            var dataTable = new DataTable { TableName = Constant.RaynaTourOptions };
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
                    ClassName = "RaynaPersistence",
                    MethodName = "SetTourById",
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
