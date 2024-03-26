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

namespace Isango.Persistence.ConsoleApplication.AgeGroup
{
    public class RezdyPersistence : PersistenceBase, IRezdyPersistence
    {
        private readonly ILogger _log;
        public RezdyPersistence(ILogger log)
        {
            _log = log;
        }
        public void SaveRezdyAgeGroup(List<AgeGroupMapping> ageGroupMappings)
        {
            var ageGroupDataTable = SetAgeGroup(ageGroupMappings);

            try
            {
                using (var connection = new SqlConnection(ConfigurationManagerHelper.GetValuefromConfig(Constant.APIUploadDb)))
                {
                    var insertCommand = new SqlCommand(Constant.InsertRezdyAgeGroupsSP, connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    var tvpAgeGroup = insertCommand.Parameters.AddWithValue(Constant.AgeGroupParameter, ageGroupDataTable);
                    tvpAgeGroup.SqlDbType = SqlDbType.Structured;
                    tvpAgeGroup.TypeName = Constant.AgeGroupDBType;
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
                    ClassName = "RezdyPersistence",
                    MethodName = "SaveRezdyAgeGroup",
                    Params = $"{SerializeDeSerializeHelper.Serialize(ageGroupMappings)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public void SaveRezdyProducts(List<RezdyProductDetail> rezdyProductDetails)
        {
            var productDetailsDataTable = SetProductDetails(rezdyProductDetails);
            try
            {
                using (var connection = new SqlConnection(ConfigurationManagerHelper.GetValuefromConfig(Constant.APIUploadDb)))
                {
                    var insertCommand = new SqlCommand(Constant.InsertRezdyProductsSP, connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    var param = insertCommand.Parameters.AddWithValue(Constant.RezdyProductDetailsParamter, productDetailsDataTable);
                    param.SqlDbType = SqlDbType.Structured;
                    param.TypeName = Constant.ProductDetailsDBType;
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
                    ClassName = "RezdyPersistence",
                    MethodName = "SaveRezdyProducts",
                    Params = $"{SerializeDeSerializeHelper.Serialize(rezdyProductDetails)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public void SaveBookingFields(List<BookingFieldMapping> bookingFieldMappings)
        {
            var bookingFieldDataTable = SetBookingField(bookingFieldMappings);
            try
            {
                using (var connection = new SqlConnection(ConfigurationManagerHelper.GetValuefromConfig(Constant.APIUploadDb)))
                {
                    var insertCommand = new SqlCommand(Constant.InsertRezdyBookingFieldsSP, connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    var tvpAgeGroup = insertCommand.Parameters.AddWithValue(Constant.BookingFieldParameter, bookingFieldDataTable);
                    tvpAgeGroup.SqlDbType = SqlDbType.Structured;
                    tvpAgeGroup.TypeName = Constant.BookingFieldsDBType;
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
                    ClassName = "RezdyPersistence",
                    MethodName = "SaveRezdyProducts",
                    Params = $"{SerializeDeSerializeHelper.Serialize(bookingFieldMappings)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public void SaveExtraDetailsFields(List<ProductWiseExtraDetails> extraDetailsMappings)
        {
            var extraDetailsDataTable = SetExtraDetails(extraDetailsMappings);

            using (var connection = new SqlConnection(ConfigurationManagerHelper.GetValuefromConfig(Constant.APIUploadDb)))
            {
                var insertCommand = new SqlCommand(Constant.InsertRezdyExtraDetailsSP, connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                var tvpAgeGroup = insertCommand.Parameters.AddWithValue(Constant.BookableExtraParameter, extraDetailsDataTable);
                tvpAgeGroup.SqlDbType = SqlDbType.Structured;
                tvpAgeGroup.TypeName = Constant.RezdyExtraDBType;
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

        public List<int> GetAllRezdySupportedSuppliers()
        {
            var rezdySuppliersId = new List<int>();
            try
            {
                using (var command = APIUploadDb.GetStoredProcCommand(Constant.GetSupplierSP))
                {
                    using (var reader = APIUploadDb.ExecuteReader(command))
                    {
                        while (reader.Read())
                        {
                            rezdySuppliersId.Add(DbPropertyHelper.Int32PropertyFromRow(reader, "supplierID"));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "RezdyPersistence",
                    MethodName = "GetAllRezdySupportedSuppliers",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return rezdySuppliersId;
        }

        #region Private methods
        private DataTable SetAgeGroup(List<AgeGroupMapping> ageGroupMappings)
        {
            var dataTable = new DataTable { TableName = Constant.RezdyAgeGroup };
            try
            {
                foreach (var property in ageGroupMappings[0].GetType().GetProperties())
                {
                    dataTable.Columns.Add(new DataColumn(property.Name, property.PropertyType));
                }

                foreach (var ageGroup in ageGroupMappings)
                {
                    var newRow = dataTable.NewRow();
                    foreach (var property in ageGroup.GetType().GetProperties())
                    {
                        newRow[property.Name] = ageGroup.GetType().GetProperty(property.Name)
                            ?.GetValue(ageGroup, null);

                    }
                    dataTable.Rows.Add(newRow);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "RezdyPersistence",
                    MethodName = "SetAgeGroup",
                    Params = $"{SerializeDeSerializeHelper.Serialize(ageGroupMappings)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return dataTable;
        }

        private DataTable SetProductDetails(List<RezdyProductDetail> rezdyProductDetails)
        {
            var dataTable = new DataTable { TableName = Constant.RezdyProductDetails };
            try
            {
                foreach (var property in rezdyProductDetails[0].GetType().GetProperties())
                {
                    dataTable.Columns.Add(new DataColumn(property.Name, property.PropertyType));
                }

                foreach (var productDetail in rezdyProductDetails)
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
                    ClassName = "RezdyPersistence",
                    MethodName = "SetProductDetails",
                    Params = $"{SerializeDeSerializeHelper.Serialize(rezdyProductDetails)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return dataTable;
        }

        private DataTable SetBookingField(List<BookingFieldMapping> bookingFieldMappings)
        {
            var dataTable = new DataTable { TableName = Constant.RezdybookingFields };
            try
            {
                foreach (var property in bookingFieldMappings[0].GetType().GetProperties())
                {
                    dataTable.Columns.Add(new DataColumn(property.Name, property.PropertyType));
                }

                foreach (var bookingFieldMapping in bookingFieldMappings)
                {
                    var newRow = dataTable.NewRow();
                    foreach (var property in bookingFieldMapping.GetType().GetProperties())
                    {
                        newRow[property.Name] = bookingFieldMapping.GetType().GetProperty(property.Name)?.GetValue(bookingFieldMapping, null);
                    }
                    dataTable.Rows.Add(newRow);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "RezdyPersistence",
                    MethodName = "SetBookingField",
                    Params = $"{SerializeDeSerializeHelper.Serialize(bookingFieldMappings)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return dataTable;
        }

        private DataTable SetExtraDetails(List<ProductWiseExtraDetails> extraDetailsMappings)
        {
            var dataTable = new DataTable { TableName = Constant.RezdyBookableExtra };

            foreach (var property in extraDetailsMappings[0].GetType().GetProperties())
            {
                dataTable.Columns.Add(new DataColumn(property.Name, property.PropertyType));
            }

            foreach (var extraDetailsMapping in extraDetailsMappings)
            {
                var newRow = dataTable.NewRow();
                foreach (var property in extraDetailsMapping.GetType().GetProperties())
                {
                    newRow[property.Name] = extraDetailsMapping.GetType().GetProperty(property.Name)?.GetValue(extraDetailsMapping, null);
                }
                dataTable.Rows.Add(newRow);
            }
            return dataTable;
        }
        #endregion Private methods

    }
}
