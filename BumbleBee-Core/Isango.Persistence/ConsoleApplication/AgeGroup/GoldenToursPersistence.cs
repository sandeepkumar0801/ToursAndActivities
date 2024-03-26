using Isango.Entities;
using Isango.Entities.GoldenTours;
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
    public class GoldenToursPersistence : PersistenceBase, IGoldenToursPersistence
    {
        private readonly ILogger _log;
        public GoldenToursPersistence(ILogger log)
        {
            _log = log;
        }
        /// <summary>
        /// Save Golden Tours product details in the database
        /// </summary>
        /// <param name="productDetails"></param>
        public void SaveGoldenToursProductDetails(List<ProductDetail> productDetails)
        {
            var productDetailsDataTable = SetProductDetails(productDetails);
            try
            {
                using (var connection = new SqlConnection(ConfigurationManagerHelper.GetValuefromConfig(Constant.APIUploadDb)))
                {
                    var insertCommand = new SqlCommand(Constant.InsertGoldenToursProductDetailsSp, connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    var tvpParam = insertCommand.Parameters.AddWithValue(Constant.ProductDetailsParameter, productDetailsDataTable);
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
                    ClassName = "GoldenToursPersistence",
                    MethodName = "SaveGoldenToursProductDetails",
                    Params = $"{SerializeDeSerializeHelper.Serialize(productDetails)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Save Golden Tours Age Groups in the database
        /// </summary>
        /// <param name="ageGroups"></param>
        public void SaveGoldenToursAgeGroups(List<Entities.GoldenTours.AgeGroup> ageGroups)
        {
            var ageGroupDataTable = SetAgeGroup(ageGroups);
            try
            {
                using (var connection = new SqlConnection(ConfigurationManagerHelper.GetValuefromConfig(Constant.APIUploadDb)))
                {
                    var insertCommand = new SqlCommand(Constant.InsertGoldenToursAgeGroupsSp, connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    var tvpAgeGroup = insertCommand.Parameters.AddWithValue(Constant.PriceUnitsParameter, ageGroupDataTable);
                    tvpAgeGroup.SqlDbType = SqlDbType.Structured;

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
                    ClassName = "GoldenToursPersistence",
                    MethodName = "SaveGoldenToursAgeGroups",
                    Params = $"{SerializeDeSerializeHelper.Serialize(ageGroups)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Save Golden Tours product details in the database
        /// </summary>
        /// <param name="productDetails"></param>
        public void SaveGoldenToursPricePeriods(List<Periods> pricePeriods)
        {
            var pricePeriodsDataTable = SetPricePeriodDT(pricePeriods);

            using (var connection = new SqlConnection(ConfigurationManagerHelper.GetValuefromConfig(Constant.APIUploadDb)))
            {
                var insertCommand = new SqlCommand(Constant.InsertGoldenToursPricePeriodsSp, connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                var tvpParam = insertCommand.Parameters.AddWithValue(Constant.PricePeriodParameter, pricePeriodsDataTable);
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

        #region Private Methods

        private DataTable SetPricePeriodDT(List<Periods> pricePeriods)
        {
            var dataTable = new DataTable { TableName = Constant.GoldenToursProductDetails };

            foreach (var property in pricePeriods[0].GetType().GetProperties())
            {
                dataTable.Columns.Add(new DataColumn(property.Name, property.PropertyType));
            }

            foreach (var pricePeriod in pricePeriods)
            {
                var newRow = dataTable.NewRow();
                foreach (var property in pricePeriod.GetType().GetProperties())
                {
                    newRow[property.Name] = pricePeriod.GetType().GetProperty(property.Name)
                        ?.GetValue(pricePeriod, null);
                }
                dataTable.Rows.Add(newRow);
            }
            return dataTable;
        }


        /// <summary>
        /// Prepare datatable for the Product Details
        /// </summary>
        /// <param name="productDetails"></param>
        /// <returns></returns>
        private DataTable SetProductDetails(List<ProductDetail> productDetails)
        {
            var dataTable = new DataTable { TableName = Constant.GoldenToursProductDetails };
            try
            {
                foreach (var property in productDetails[0].GetType().GetProperties())
                {
                    dataTable.Columns.Add(new DataColumn(property.Name, property.PropertyType));
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
                    ClassName = "GoldenToursPersistence",
                    MethodName = "SetProductDetails",
                    Params = $"{SerializeDeSerializeHelper.Serialize(productDetails)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return dataTable;
        }

        /// <summary>
        /// Prepare datatable for Age Group
        /// </summary>
        /// <param name="ageGroups"></param>
        /// <returns></returns>
        private DataTable SetAgeGroup(List<Entities.GoldenTours.AgeGroup> ageGroups)
        {
            var dataTable = new DataTable { TableName = Constant.GoldenToursAgeGroups };
            try
            {
                foreach (var property in ageGroups[0].GetType().GetProperties())
                {
                    dataTable.Columns.Add(new DataColumn(property.Name, property.PropertyType));
                }

                foreach (var ageGroup in ageGroups)
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
                    ClassName = "GoldenToursPersistence",
                    MethodName = "SetAgeGroup",
                    Params = $"{SerializeDeSerializeHelper.Serialize(ageGroups)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return dataTable;
        }

        #endregion Private Methods
    }
}