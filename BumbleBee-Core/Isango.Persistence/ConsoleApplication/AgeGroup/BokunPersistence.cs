using Isango.Entities;
using Isango.Entities.Bokun;
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
    public class BokunPersistence : PersistenceBase, IBokunPersistence
    {
        private readonly ILogger _log;
        public BokunPersistence(ILogger log)
        {
            _log = log;
        }
        /// <summary>
        /// Save Bokun product details in the database
        /// </summary>
        /// <param name="productDetails"></param>
        public void SaveBokunProductDetails(List<Entities.Bokun.Product> productDetails)
        {
            var productDetailsDataTable = SetProductDetails(productDetails);

            try
            {
                using (var connection = new SqlConnection(ConfigurationManagerHelper.GetValuefromConfig(Constant.APIUploadDb)))
                {
                    var insertCommand = new SqlCommand(Constant.InsertBokunProductDetailsSp, connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    var tvpParam = insertCommand.Parameters.AddWithValue(Constant.BokunProductParameter, productDetailsDataTable);
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
                    ClassName = "BokunPersistence",
                    MethodName = "SaveBokunProductDetails",
                    Params = $"{SerializeDeSerializeHelper.Serialize(productDetails)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Save Bokun SaveBokunCancellationPolicies in the database
        /// </summary>
        /// <param name="CancellationPolicy"></param>
        public void SaveBokunCancellationPolicies(List<CancellationPolicy> cancellationPolicies)
        {
            var cancellationDetailsDataTable = SetCancellationDetails(cancellationPolicies);
            try
            {
                using (var connection = new SqlConnection(ConfigurationManagerHelper.GetValuefromConfig(Constant.APIUploadDb)))
                {
                    var insertCommand = new SqlCommand(Constant.InsertBokunCancellationPolicySp, connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    var tvpcancellationDetails = insertCommand.Parameters.AddWithValue(Constant.CancellationPolicyParameter, cancellationDetailsDataTable);
                    tvpcancellationDetails.SqlDbType = SqlDbType.Structured;

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
                    ClassName = "BokunPersistence",
                    MethodName = "SaveBokunCancellationPolicies",
                    Params = $"{SerializeDeSerializeHelper.Serialize(cancellationPolicies)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public void BokunSyncCall()
        {
            try
            {
                using (var connection = new SqlConnection(ConfigurationManagerHelper.GetValuefromConfig(Constant.IsangoLiveDb)))
                {
                    var insertCommand = new SqlCommand(Constant.InsertBokunSyncMappingSp, connection)
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
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BokunPersistence",
                    MethodName = "BokunSyncCall",
                 };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Save Bokun SaveBokunRates in the database
        /// </summary>
        /// <param name="CancellationPolicy"></param>
        public void SaveBokunRates(List<Rate> rates)
        {
            var rateDataTable = GetDataTable(rates, Constant.BokunRatesTbl);
            try
            {
                using (var connection = new SqlConnection(ConfigurationManagerHelper.GetValuefromConfig(Constant.APIUploadDb)))
                {
                    var insertCommand = new SqlCommand(Constant.BokunInsertRatesSp, connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    var tvpcancellationDetails = insertCommand.Parameters.AddWithValue(Constant.BokunRatesParameter, rateDataTable);
                    tvpcancellationDetails.SqlDbType = SqlDbType.Structured;

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
                    ClassName = "BokunPersistence",
                    MethodName = "SaveBokunRates",
                    Params = $"{SerializeDeSerializeHelper.Serialize(rates)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public void SaveBookableExtras(List<BookableExtras> bookableExtras)
        {
            var BookabeExtraTable = new DataTable { TableName = Constant.BookabeExtraTable };
            var BookableExtraQuestionsTable = new DataTable { TableName = Constant.BookableExtraQuestionsTable };
            try
            {
                //Add Columns to BookableExtraTable
                BookabeExtraTable.Columns.Add(new DataColumn("Id", typeof(string)));
                BookabeExtraTable.Columns.Add(new DataColumn("SelectionType", typeof(string)));
                BookabeExtraTable.Columns.Add(new DataColumn("IsPricedPerPerson", typeof(bool)));
                BookabeExtraTable.Columns.Add(new DataColumn("ServiceID", typeof(int)));
                BookabeExtraTable.Columns.Add(new DataColumn("ServiceOptionID", typeof(int)));
                BookabeExtraTable.Columns.Add(new DataColumn("ExternalId", typeof(string)));
                BookabeExtraTable.Columns.Add(new DataColumn("Title", typeof(string)));
                BookabeExtraTable.Columns.Add(new DataColumn("Information", typeof(string)));
                BookabeExtraTable.Columns.Add(new DataColumn("Included", typeof(bool)));
                BookabeExtraTable.Columns.Add(new DataColumn("Free", typeof(bool)));
                BookabeExtraTable.Columns.Add(new DataColumn("ProductGroupId", typeof(string)));
                BookabeExtraTable.Columns.Add(new DataColumn("PricingType", typeof(string)));
                BookabeExtraTable.Columns.Add(new DataColumn("PricingTypeLabel", typeof(string)));
                BookabeExtraTable.Columns.Add(new DataColumn("Price", typeof(string)));
                BookabeExtraTable.Columns.Add(new DataColumn("IncreasesCapacity", typeof(bool)));
                BookabeExtraTable.Columns.Add(new DataColumn("MaxPerBooking", typeof(string)));
                BookabeExtraTable.Columns.Add(new DataColumn("LimitByPax", typeof(bool)));
                BookabeExtraTable.Columns.Add(new DataColumn("Flags", typeof(string)));

                //Add Columns to BookabelExtraQuestionsTable
                BookableExtraQuestionsTable.Columns.Add(new DataColumn("Id", typeof(string)));
                BookableExtraQuestionsTable.Columns.Add(new DataColumn("ServiceID", typeof(string)));
                BookableExtraQuestionsTable.Columns.Add(new DataColumn("ServiceOptionID", typeof(string)));
                BookableExtraQuestionsTable.Columns.Add(new DataColumn("Active", typeof(bool)));
                BookableExtraQuestionsTable.Columns.Add(new DataColumn("Label", typeof(string)));
                BookableExtraQuestionsTable.Columns.Add(new DataColumn("Type", typeof(string)));
                BookableExtraQuestionsTable.Columns.Add(new DataColumn("Options", typeof(string)));
                BookableExtraQuestionsTable.Columns.Add(new DataColumn("AnswerRequired", typeof(bool)));
                BookableExtraQuestionsTable.Columns.Add(new DataColumn("Flags", typeof(string)));


                foreach (var bookextra in bookableExtras)
                {
                    try
                    {
                        var BookabelExtraRow = BookabeExtraTable.NewRow();

                        BookabelExtraRow["ExternalId"] = bookextra.ExternalId;
                        BookabelExtraRow["Flags"] = bookextra.Flags;
                        BookabelExtraRow["Free"] = bookextra.Free ?? false;
                        BookabelExtraRow["Id"] = bookextra.Id;
                        BookabelExtraRow["Included"] = bookextra.Included ?? false;
                        BookabelExtraRow["IncreasesCapacity"] = bookextra.IncreasesCapacity ?? false;
                        BookabelExtraRow["Information"] = bookextra.Information;
                        BookabelExtraRow["IsPricedPerPerson"] = bookextra.IsPricedPerPerson ?? true;
                        BookabelExtraRow["LimitByPax"] = bookextra.LimitByPax ?? true;
                        BookabelExtraRow["MaxPerBooking"] = bookextra.MaxPerBooking;
                        BookabelExtraRow["Price"] = bookextra.Price;
                        BookabelExtraRow["PricingType"] = bookextra.PricingType;
                        BookabelExtraRow["PricingTypeLabel"] = bookextra.PricingTypeLabel;
                        BookabelExtraRow["ProductGroupId"] = bookextra.ProductGroupId;
                        BookabelExtraRow["SelectionType"] = bookextra.SelectionType;
                        BookabelExtraRow["ServiceID"] = bookextra.ServiceID;
                        BookabelExtraRow["ServiceOptionID"] = bookextra.ServiceOptionID;
                        BookabelExtraRow["Title"] = bookextra.Title;

                        BookabeExtraTable.Rows.Add(BookabelExtraRow);

                        if (bookextra?.Questions?.Count > 0)
                        {
                            foreach (var ques in bookextra?.Questions)
                            {
                                try
                                {
                                    var BookableExtraQuestionsRow = BookableExtraQuestionsTable.NewRow();

                                    BookableExtraQuestionsRow["Active"] = ques.Active ?? false;
                                    BookableExtraQuestionsRow["AnswerRequired"] = ques.AnswerRequired ?? false;
                                    BookableExtraQuestionsRow["Flags"] = ques.Flags;
                                    BookableExtraQuestionsRow["Id"] = ques.Id;
                                    BookableExtraQuestionsRow["Label"] = ques.Label;
                                    BookableExtraQuestionsRow["Options"] = ques.Options;
                                    BookableExtraQuestionsRow["Type"] = ques.Type;
                                    BookableExtraQuestionsRow["ServiceID"] = bookextra.ServiceID;
                                    BookableExtraQuestionsRow["ServiceOptionID"] = bookextra.ServiceOptionID;

                                    BookableExtraQuestionsTable.Rows.Add(BookableExtraQuestionsRow);
                                }
                                catch (Exception ex)
                                {
                                    //ignored, must not stop the whole process
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        //ignored, must not stop the whole process
                    }
                }

                using (var connection = new SqlConnection(ConfigurationManagerHelper.GetValuefromConfig(Constant.APIUploadDb)))
                {
                    var insertCommandBookableExtra = new SqlCommand(Constant.BokunInsertBookableExtraSp, connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    var bookableExtraDetails = insertCommandBookableExtra.Parameters.AddWithValue(Constant.BokunBookableExtraParameter, BookabeExtraTable);
                    bookableExtraDetails.SqlDbType = SqlDbType.Structured;

                    var insertCommandBookableExtraQuestions = new SqlCommand(Constant.BokunInsertBookabelExtraQuesSp, connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    var bookableExtraQuestionsDetails = insertCommandBookableExtraQuestions.Parameters.AddWithValue(Constant.BokunBookableExtraQuesParameter, BookableExtraQuestionsTable);
                    bookableExtraQuestionsDetails.SqlDbType = SqlDbType.Structured;

                    try
                    {
                        connection.Open();
                        insertCommandBookableExtra.ExecuteNonQuery();
                        insertCommandBookableExtraQuestions.ExecuteNonQuery();
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
                    ClassName = "BokunPersistence",
                    MethodName = "SaveBookableExtras",
                    Params = $"{SerializeDeSerializeHelper.Serialize(bookableExtras)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        #region Private Methods

        /// <summary>
        /// Prepare data-table for the Product Details
        /// </summary>
        /// <param name="productDetails"></param>
        /// <returns></returns>
        private DataTable SetProductDetails(List<Entities.Bokun.Product> productDetails)
        {
            var dataTable = new DataTable { TableName = Constant.BokunProductDetailsTbl };
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
                    ClassName = "BokunPersistence",
                    MethodName = "SetProductDetails",
                    Params = $"{SerializeDeSerializeHelper.Serialize(productDetails)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return dataTable;
        }

        /// <summary>
        /// Prepare datatable for CancellationDetails
        /// </summary>
        /// <param name="cancellationPolicies"></param>
        /// <returns></returns>
        private DataTable SetCancellationDetails(List<CancellationPolicy> cancellationPolicies)
        {
            var dataTable = new DataTable { TableName = Constant.BokunCancellationDetailsTbl };
            try
            {
                foreach (var property in cancellationPolicies[0].GetType().GetProperties())
                {
                    dataTable.Columns.Add(new DataColumn(property.Name, Nullable.GetUnderlyingType(
                property.PropertyType) ?? property.PropertyType));
                }

                foreach (var cancellationPollicie in cancellationPolicies)
                {
                    var newRow = dataTable.NewRow();
                    foreach (var property in cancellationPollicie.GetType().GetProperties())
                    {
                        newRow[property.Name] = cancellationPollicie.GetType().GetProperty(property.Name)
                            ?.GetValue(cancellationPollicie, null);
                    }
                    dataTable.Rows.Add(newRow);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BokunPersistence",
                    MethodName = "SetCancellationDetails",
                    Params = $"{SerializeDeSerializeHelper.Serialize(cancellationPolicies)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return dataTable;
        }

        /// <summary>
        /// Prepare datatable for rates
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private DataTable GetDataTable<T>(List<T> list, string tableName)
        {
            var dataTable = new DataTable { TableName = tableName };
            try
            {
                foreach (var property in list[0].GetType().GetProperties())
                {
                    dataTable.Columns.Add(new DataColumn(property.Name, Nullable.GetUnderlyingType(
                property.PropertyType) ?? property.PropertyType));
                }

                foreach (var rate in list)
                {
                    var newRow = dataTable.NewRow();
                    foreach (var property in rate.GetType().GetProperties())
                    {
                        newRow[property.Name] = rate.GetType().GetProperty(property.Name)
                            ?.GetValue(rate, null);
                    }
                    dataTable.Rows.Add(newRow);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BokunPersistence",
                    MethodName = "GetDataTable",
                 };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return dataTable;
        }

        #endregion Private Methods
    }
}