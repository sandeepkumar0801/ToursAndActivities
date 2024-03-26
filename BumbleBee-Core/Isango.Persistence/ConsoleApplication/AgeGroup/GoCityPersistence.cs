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
using ServiceAdapters.GoCity.GoCity.Entities.Product;

namespace Isango.Persistence.ConsoleApplication.AgeGroup
{
    public class GoCityPersistence : PersistenceBase, IGoCityPersistence
    {
        private readonly ILogger _log;
        public GoCityPersistence(ILogger log)
        {
            _log = log;
        }

        public void SaveGoCityProducts(ProductResponse products)
        {
            var lstProduct = new List<Isango.Entities.GoCity.Product>();
            //AssigData
            if (products != null && products.ProductResponseList.Count > 0)
            { 
                foreach (var item in products.ProductResponseList)
                {
                    var passData = new Isango.Entities.GoCity.Product
                    {
                        ProductSkuCodeName= item.ProductSkuCodeName,
                        RequestProductSkuCode= item.RequestProductSkuCode,
                        Type=item.Type
                    };
                    lstProduct.Add(passData);
                 }
            }
            var productDetailsDataTable = SetProductDetails(lstProduct);
            try
            {
                using (var connection = new SqlConnection(ConfigurationManagerHelper.GetValuefromConfig(Constant.IsangoLiveDb)))
                {
                    var insertCommand = new SqlCommand(Constant.InsertGoCityProductsSP, connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    var param = insertCommand.Parameters.AddWithValue(Constant.GoCityProductParamter, productDetailsDataTable);
                    param.SqlDbType = SqlDbType.Structured;
                    param.TypeName = Constant.GoCityProductDBType;
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
                    ClassName = "GoCityPersistence",
                    MethodName = "SaveGoCityProducts",
                    Params = $"{SerializeDeSerializeHelper.Serialize(products)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }


        #region Private methods


        private DataTable SetProductDetails(List<Isango.Entities.GoCity.Product> products)
        {
            var dataTable = new DataTable { TableName = Constant.GoCityProducts };
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
                    ClassName = "GoCityPersistence",
                    MethodName = "SetProductDetails",
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
