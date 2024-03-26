using Isango.Entities;
using Isango.Entities.Ventrata;
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
    public class VentrataPersistence : PersistenceBase, IVentrataPersistence
    {
        private readonly ILogger _log;

        public VentrataPersistence(ILogger log)
        {
            _log = log;
        }

        public void SaveProductDetails(List<ProductDetail> productDetails)
        {
            deleteExistingData();
            var chunkedData = productDetails?.ChunkBy(100)?.ToList();
            foreach (var item in chunkedData)
            {
                var productDetailsDataTable = SetProductDetails(item);
                try
                {
                    using (var connection = new SqlConnection(ConfigurationManagerHelper.GetValuefromConfig(Constant.APIUploadDb)))
                    {
                        var insertCommand = new SqlCommand(Constant.VentrataProductDetailsProcedure, connection)
                        {
                            CommandType = CommandType.StoredProcedure
                        };
                        var tvpParam = insertCommand.Parameters.AddWithValue(Constant.VentrataProductDetailParameter, productDetailsDataTable);
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
                        ClassName = "VentrataPersistence",
                        MethodName = "SaveProductDetails",
                        Params = $"{SerializeDeSerializeHelper.Serialize(item)}"
                    };
                    _log.Error(isangoErrorEntity, ex);
                }
            }
        }

        private void deleteExistingData()
        {
            try
            {
                using (var connection = new SqlConnection(ConfigurationManagerHelper.GetValuefromConfig(Constant.APIUploadDb)))
                {
                    var insertCommand = new SqlCommand(Constant.VentrataDeleteProductDetailsProcedure, connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
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
                //do nothing
            }
        }

        private DataTable SetProductDetails(List<ProductDetail> productDetails)
        {
            var dataTable = new DataTable { TableName = Constant.VentrataProductDetailsTbl };
            try
            {
                foreach (var property in productDetails[0].GetType().GetProperties())
                {
                    dataTable.Columns.Add(new DataColumn(property.Name, Nullable.GetUnderlyingType(
                property.PropertyType) ?? property.PropertyType));
                }

                foreach (var productDetail in productDetails)
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
                    ClassName = "VentrataPersistence",
                    MethodName = "SetProductDetails",
                    Params = $"{SerializeDeSerializeHelper.Serialize(productDetails)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return dataTable;
        }

        public void SaveDestinationDetails(List<Entities.Ventrata.Destination> destinations)
        {
            var destinationDataTable = SetDestinationDetails(destinations);
            try
            {
                using (var connection = new SqlConnection(ConfigurationManagerHelper.GetValuefromConfig(Constant.APIUploadDb)))
                {
                    var insertCommand = new SqlCommand(Constant.VentrataDestinationProcedure, connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    var tvpParam = insertCommand.Parameters.AddWithValue(Constant.VentrataDestinationParameter, destinationDataTable);
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
                    ClassName = "VentrataPersistence",
                    MethodName = "SaveDestinationDetails",
                    Params = $"{SerializeDeSerializeHelper.Serialize(destinations)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private DataTable SetDestinationDetails(List<Entities.Ventrata.Destination> destinations)
        {
            var dataTable = new DataTable { TableName = Constant.VentrataDestinationTbl };
            try
            {
                foreach (var property in destinations[0].GetType().GetProperties())
                {
                    dataTable.Columns.Add(new DataColumn(property.Name, Nullable.GetUnderlyingType(
                property.PropertyType) ?? property.PropertyType));
                }

                foreach (var destination in destinations)
                {
                    var newRow = dataTable.NewRow();
                    foreach (var property in destination.GetType().GetProperties())
                    {
                        newRow[property.Name] = destination.GetType().GetProperty(property.Name)
                            ?.GetValue(destination, null);
                    }
                    dataTable.Rows.Add(newRow);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "VentrataPersistence",
                    MethodName = "SetDestinationDetails",
                    Params = $"{SerializeDeSerializeHelper.Serialize(destinations)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return dataTable;
        }

        public void SaveFaqs(List<FAQ> faqs)
        {
            var faqDataTable = SetFaqDetails(faqs);
            try
            {
                using (var connection = new SqlConnection(ConfigurationManagerHelper.GetValuefromConfig(Constant.APIUploadDb)))
                {
                    var insertCommand = new SqlCommand(Constant.VentrataFaqProcedure, connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    var tvpParam = insertCommand.Parameters.AddWithValue(Constant.VentrataFaqParameter, faqDataTable);
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
                    ClassName = "VentrataPersistence",
                    MethodName = "SaveFaqs",
                    Params = $"{SerializeDeSerializeHelper.Serialize(faqs)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private DataTable SetFaqDetails(List<FAQ> faqs)
        {
            var dataTable = new DataTable { TableName = Constant.VentrataFaqTbl };
            try
            {
                foreach (var property in faqs[0].GetType().GetProperties())
                {
                    dataTable.Columns.Add(new DataColumn(property.Name, Nullable.GetUnderlyingType(
                property.PropertyType) ?? property.PropertyType));
                }

                foreach (var faq in faqs)
                {
                    var newRow = dataTable.NewRow();
                    foreach (var property in faq.GetType().GetProperties())
                    {
                        newRow[property.Name] = faq.GetType().GetProperty(property.Name)
                            ?.GetValue(faq, null);
                    }
                    dataTable.Rows.Add(newRow);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "VentrataPersistence",
                    MethodName = "SetFaqDetails",
                    Params = $"{SerializeDeSerializeHelper.Serialize(faqs)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return dataTable;
        }

        public void SaveOptionDetails(List<Option> options)
        {
            var optionDataTable = SetOptionDetails(options);
            try
            {
                using (var connection = new SqlConnection(ConfigurationManagerHelper.GetValuefromConfig(Constant.APIUploadDb)))
                {
                    var insertCommand = new SqlCommand(Constant.VentrataOptionProcedure, connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    var tvpParam = insertCommand.Parameters.AddWithValue(Constant.VentrataOptionParameter, optionDataTable);
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
                    ClassName = "VentrataPersistence",
                    MethodName = "SaveOptionDetails",
                    Params = $"{SerializeDeSerializeHelper.Serialize(options)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private DataTable SetOptionDetails(List<Option> options)
        {
            var dataTable = new DataTable { TableName = Constant.VentrataOptionTbl };
            try
            {
                foreach (var property in options[0].GetType().GetProperties())
                {
                    dataTable.Columns.Add(new DataColumn(property.Name, Nullable.GetUnderlyingType(
                property.PropertyType) ?? property.PropertyType));
                }

                foreach (var option in options)
                {
                    var newRow = dataTable.NewRow();
                    foreach (var property in option.GetType().GetProperties())
                    {
                        newRow[property.Name] = option.GetType().GetProperty(property.Name)
                            ?.GetValue(option, null);
                    }
                    dataTable.Rows.Add(newRow);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "VentrataPersistence",
                    MethodName = "SetOptionDetails",
                    Params = $"{SerializeDeSerializeHelper.Serialize(options)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return dataTable;
        }

        public void SaveUnitsDetailsOfOption(List<UnitsForOption> unitDetailsOfOption)
        {
            var unitOfOptionTable = SetUnitsInOptionDetails(unitDetailsOfOption);
            try
            {
                using (var connection = new SqlConnection(ConfigurationManagerHelper.GetValuefromConfig(Constant.APIUploadDb)))
                {
                    var insertCommand = new SqlCommand(Constant.VentrataUnitsForOptionProcedure, connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    var tvpParam = insertCommand.Parameters.AddWithValue(Constant.VentrataUnitInOptionParameter, unitOfOptionTable);
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
                    ClassName = "VentrataPersistence",
                    MethodName = "SaveUnitsDetailsOfOption",
                    Params = $"{SerializeDeSerializeHelper.Serialize(unitDetailsOfOption)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public void SavePackagesInclude(List<PackageInclude> package)
        {
            var packageTable = SetPackageInclude(package);
            try
            {
                using (var connection = new SqlConnection(ConfigurationManagerHelper.GetValuefromConfig(Constant.APIUploadDb)))
                {
                    var insertCommand = new SqlCommand(Constant.VentrataPackagesIncludeProcedure, connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    var tvpParam = insertCommand.Parameters.AddWithValue(Constant.VentrataPackagesIncludeParameter, packageTable);
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
                    ClassName = "VentrataPersistence",
                    MethodName = "SavePackagesInclude",
                    Params = $"{SerializeDeSerializeHelper.Serialize(package)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private DataTable SetUnitsInOptionDetails(List<UnitsForOption> unitsForOptions)
        {
            var dataTable = new DataTable { TableName = Constant.VentrataUnitForOptionTbl };
            try
            {
                foreach (var property in unitsForOptions[0].GetType().GetProperties())
                {
                    dataTable.Columns.Add(new DataColumn(property.Name, Nullable.GetUnderlyingType(
                property.PropertyType) ?? property.PropertyType));
                }

                foreach (var unitInOption in unitsForOptions)
                {
                    var newRow = dataTable.NewRow();
                    foreach (var property in unitInOption.GetType().GetProperties())
                    {
                        newRow[property.Name] = unitInOption.GetType().GetProperty(property.Name)
                            ?.GetValue(unitInOption, null);
                    }
                    dataTable.Rows.Add(newRow);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "VentrataPersistence",
                    MethodName = "SetUnitsInOptionDetails",
                    Params = $"{SerializeDeSerializeHelper.Serialize(unitsForOptions)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return dataTable;
        }
        private DataTable SetPackageInclude(List<PackageInclude> packageInclude)
        {
            var dataTable = new DataTable { TableName = Constant.VentrataPackageInclude };
            try
            {
                foreach (var property in packageInclude[0].GetType().GetProperties())
                {
                    dataTable.Columns.Add(new DataColumn(property.Name, Nullable.GetUnderlyingType(
                property.PropertyType) ?? property.PropertyType));
                }

                foreach (var packageIncludeData in packageInclude)
                {
                    var newRow = dataTable.NewRow();
                    foreach (var property in packageIncludeData.GetType().GetProperties())
                    {
                        newRow[property.Name] = packageIncludeData.GetType().GetProperty(property.Name)
                            ?.GetValue(packageIncludeData, null);
                    }
                    dataTable.Rows.Add(newRow);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "VentrataPersistence",
                    MethodName = "packageInclude",
                    Params = $"{SerializeDeSerializeHelper.Serialize(packageInclude)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return dataTable;
        }
    }
}