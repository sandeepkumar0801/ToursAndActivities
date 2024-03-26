using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.ConsoleApplication.DataDumping;
using Isango.Entities.GoogleMaps;
using Isango.Service.Constants;
using Isango.Service.Contract;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Table.Protocol;
using Util;
using ContractLogger = Logger.Contract;
using ExtraDetail = Isango.Entities.ConsoleApplication.DataDumping.ExtraDetail;

namespace Isango.Service.ConsoleApplication
{
    public class StorageOperationService : IStorageOperationService
    {
        private readonly ContractLogger.ILogger _log;

        public StorageOperationService(ContractLogger.ILogger log)
        {
            _log = log;
        }

        /// <summary>
        /// Insert the extra details in the Table Storage
        /// </summary>
        /// <param name="extraDetail"></param>
        public void InsertExtraDetail(Entities.GoogleMaps.ExtraDetail extraDetail)
        {
            try
            {
                var batchOperation = new TableBatchOperation();
                foreach (var paymentExtraDetail in extraDetail.PaymentExtraDetails)
                {
                    // Continue if payment extra details is null or all the three details are empty
                    if (CheckIfInvalid(paymentExtraDetail)) continue;
                    var request = new ExtraDetail
                    {
                        PartitionKey = GetPartitionKey(),
                        RowKey = Guid.NewGuid().ToString(),
                        TokenId = extraDetail.TokenId,
                        ActivityId = paymentExtraDetail.ActivityId,
                        OptionId = paymentExtraDetail.OptionId,
                        Variant = paymentExtraDetail.Variant,
                        StartTime = paymentExtraDetail.StartTime,
                        Questions = SerializeDeSerializeHelper.Serialize(paymentExtraDetail.Questions),
                        PickUpLocations = SerializeDeSerializeHelper.Serialize(paymentExtraDetail.PickupLocations),
                        DropOffLocations = SerializeDeSerializeHelper.Serialize(paymentExtraDetail.DropoffLocations)
                    };
                    batchOperation.InsertOrMerge(request);
                }
                InsertData(batchOperation, Constant.ExtraDetails);
            }
            catch (StorageException ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "StorageOperationService",
                    MethodName = "InsertExtraDetail",
                    Params = $"{SerializeDeSerializeHelper.Serialize(extraDetail)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "StorageOperationService",
                    MethodName = "InsertExtraDetail",
                    Params = $"{SerializeDeSerializeHelper.Serialize(extraDetail)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Insert service details in the Table Storage
        /// </summary>
        /// <param name="storageServiceDetails"></param>
        public void InsertServiceDetails(List<StorageServiceDetail> storageServiceDetails)
        {
            try
            {
                var batchOperation = new TableBatchOperation();
                var partitionKey = GetPartitionKey();
                foreach (var storageServiceDetail in storageServiceDetails)
                {
                    if (storageServiceDetail != null)
                    {
                        storageServiceDetail.PartitionKey = partitionKey;
                        batchOperation.InsertOrMerge(storageServiceDetail);
                    }
                }
                InsertData(batchOperation, Constant.PriceAndAvailabilityDumping);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "StorageOperationService",
                    MethodName = "InsertExtraDetail",
                    Params = $"{SerializeDeSerializeHelper.Serialize(storageServiceDetails)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Insert Cancellation Policies in the Table Storage
        /// </summary>
        /// <param name="cancellationPolicies"></param>
        public void InsertCancellationPolicy(List<GoogleCancellationPolicy> cancellationPolicies)
        {
            try
            {
                var batchOperation = new TableBatchOperation();
                foreach (var cancellationPolicy in cancellationPolicies)
                {
                    // Continue if Cancellation price is null or empty
                    if (cancellationPolicy.CancellationPrices == null || cancellationPolicy.CancellationPrices.Count == 0) continue;
                    var request = new CancellationPolicy
                    {
                        PartitionKey = GetPartitionKey(),
                        RowKey = Guid.NewGuid().ToString(),
                        ActivityId = cancellationPolicy.ActivityId,
                        OptionId = cancellationPolicy.OptionId,
                        CancellationPrices = SerializeDeSerializeHelper.Serialize(cancellationPolicy.CancellationPrices)
                    };
                    batchOperation.InsertOrMerge(request);
                }
                InsertData(batchOperation, Constant.CancellationPolicy);
            }
            catch (StorageException ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "StorageOperationService",
                    MethodName = "InsertCancellactionPolicy",
                    Params = $"{SerializeDeSerializeHelper.Serialize(cancellationPolicies)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "StorageOperationService",
                    MethodName = "InsertCancellactionPolicy",
                    Params = $"{SerializeDeSerializeHelper.Serialize(cancellationPolicies)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Retrieve the Service Details from the Table Storage
        /// </summary>
        /// <param name="partitionKey"></param>
        /// <returns></returns>
        public List<StorageServiceDetail> GetServiceDetails(string partitionKey)
        {
            try
            {
                var table = GetTableReference(Constant.PriceAndAvailabilityDumping);
                var queryString = TableQuery.GenerateFilterCondition(
                        Constant.PartitionKey, QueryComparisons.Equal, partitionKey
                    );

                var query = new TableQuery<StorageServiceDetail>().Where(queryString);
                StorageServiceDetail EntityResolver(string pk, string rk, DateTimeOffset ts, IDictionary<string, EntityProperty> props, string etag)
                {
                    var resolvedEntity = new StorageServiceDetail() { PartitionKey = pk, RowKey = rk, Timestamp = ts, ETag = etag };

                    foreach (var item in props.Where(p => p.Key.StartsWith(Constant.DecimalPrefix)))
                    {
                        var realPropertyName = item.Key.Substring(Constant.DecimalPrefix.Length);
                        var propertyInfo = resolvedEntity.GetType()?.GetProperty(realPropertyName);
                        propertyInfo?.SetValue(resolvedEntity, Convert.ChangeType(item.Value.StringValue, propertyInfo.PropertyType), null);
                    }
                    resolvedEntity.ReadEntity(props, null);
                    return resolvedEntity;
                }

                var result = table.ExecuteQuery(query, EntityResolver).ToList();
                return result;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "StorageOperationService",
                    MethodName = "GetServiceDetails",
                    Params = $"{partitionKey}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Retrieve the Extra Details from the Table Storage
        /// </summary>
        /// <param name="partitionKey"></param>
        /// <returns></returns>
        public List<ExtraDetail> GetExtraDetails(string partitionKey)
        {
            try
            {
                var table = GetTableReference(Constant.ExtraDetails);
                var queryString = TableQuery.GenerateFilterCondition(
                    Constant.PartitionKey, QueryComparisons.Equal, partitionKey
                );
                var query = new TableQuery<ExtraDetail>().Where(queryString);
                var result = table.ExecuteQuery(query).ToList();
                return result;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "StorageOperationService",
                    MethodName = "GetExtraDetails",
                    Params = $"{partitionKey}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Retrieve the Cancellation Policies from the Table Storage
        /// </summary>
        /// <param name="partitionKey"></param>
        /// <returns></returns>
        public List<CancellationPolicy> GetCancellationPolicies(string partitionKey)
        {
            try
            {
                var table = GetTableReference(Constant.CancellationPolicy);
                var queryString = TableQuery.GenerateFilterCondition(
                    Constant.PartitionKey, QueryComparisons.Equal, partitionKey
                );
                var query = new TableQuery<CancellationPolicy>().Where(queryString);
                var result = table.ExecuteQuery(query).ToList();
                return result;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "StorageOperationService",
                    MethodName = "GetCancellationPolicies",
                    Params = $"{partitionKey}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Add the message in queue
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="message"></param>
        public void AddMessageToQueue(string queueName, string message)
        {
            try
            {
                var queue = GetQueueReference(queueName);
                var cloudMesssage = new CloudQueueMessage(message);
                queue.AddMessage(cloudMesssage);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "StorageOperationService",
                    MethodName = "AddMessageToQueue",
                    Params = $"{queueName}, {message}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Get all Cancelled Orders which are not notified to Google
        /// </summary>
        /// <returns></returns>
        public List<OrderResponse> GetCancelledOrders()
        {
            var table = GetTableReference(Constant.GoogleMapsOrder);
            var queryString = TableQuery.CombineFilters(TableQuery.GenerateFilterCondition(
                Constant.BookingStatus, QueryComparisons.Equal, Constant.CancelledBookingStatus
            ), TableOperators.And, TableQuery.GenerateFilterCondition(
                Constant.IsNotifiedToGoogle, QueryComparisons.Equal, "false"
            ));

            var query = new TableQuery<OrderResponse>().Where(queryString);
            var orderResponses = table.ExecuteQuery(query).ToList();
            return orderResponses;
        }

        /// <summary>
        /// Updates the IsNotifiedToGoogle flag in the storage
        /// </summary>
        /// <param name="orderResponses"></param>
        public void UpdateNotifiedOrders(List<OrderResponse> orderResponses)
        {
            try
            {
                var batchOperation = new TableBatchOperation();
                foreach (var orderResponse in orderResponses)
                {
                    orderResponse.IsNotifiedToGoogle = true;
                    batchOperation.InsertOrMerge(orderResponse);
                }
                InsertData(batchOperation, Constant.GoogleMapsOrder);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "StorageOperationService",
                    MethodName = "UpdateNotifiedOrders",
                    Params = $"{SerializeDeSerializeHelper.Serialize(orderResponses)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        #region Private Methods

        /// <summary>
        /// Get the Azure Table Storage object by the table name
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private CloudTable GetTableReference(string tableName)
        {
            var connectionString = ConfigurationManagerHelper.GetValuefromConfig(Constant.StorageConnectionString);
            var cloudStorageAccount = CloudStorageAccount.Parse(connectionString);
            var client = cloudStorageAccount.CreateCloudTableClient();
            var table = client.GetTableReference(tableName);
            table.CreateIfNotExists();
            return table;
        }

        /// <summary>
        /// Get the formatted date as partition key
        /// </summary>
        /// <returns></returns>
        private string GetPartitionKey()
        {
            return DateTime.UtcNow.ToString("dd_MMM_yyyy");
        }

        /// <summary>
        /// Check Questions, PickupLocations and DropoffLocations are not empty
        /// </summary>
        /// <param name="paymentExtraDetail"></param>
        /// <returns></returns>
        private bool CheckIfInvalid(Entities.GoogleMaps.PaymentExtraDetail paymentExtraDetail)
        {
            return (paymentExtraDetail.Questions == null || paymentExtraDetail.Questions?.Count == 0) && (paymentExtraDetail.PickupLocations == null || paymentExtraDetail.PickupLocations?.Count == 0) &&
                     (paymentExtraDetail.DropoffLocations == null || paymentExtraDetail.DropoffLocations?.Count == 0);
        }

        /// <summary>
        /// Insert the data in the storage using the TableBatchOperation
        /// </summary>
        /// <param name="batchOperation"></param>
        /// <param name="tableName"></param>
        private void InsertData(TableBatchOperation batchOperation, string tableName)
        {
            if (batchOperation.Count == 0)
                return;
            var table = GetTableReference(tableName);
            if (batchOperation.Count <= TableConstants.TableServiceBatchMaximumOperations)
            {
                try
                {
                    table.ExecuteBatch(batchOperation);
                }
                catch (Exception ex)
                {
                    var isangoErrorEntity = new IsangoErrorEntity
                    {
                        ClassName = "StorageOperationService",
                        MethodName = "InsertData",
                        Params = $"{SerializeDeSerializeHelper.Serialize(batchOperation)}"
                    };
                    _log.Error(isangoErrorEntity, ex);
                    //ignored - should not stop whole dumping process
                }

            }
            else
            {
                var limitedBatchList = batchOperation.ChunkBy(TableConstants.TableServiceBatchMaximumOperations);
                var batchCount = 0;

                foreach (var limitedBatch in limitedBatchList)
                {
                    try
                    {
                        var batch = new TableBatchOperation();
                        foreach (var item in limitedBatch)
                        {
                            batch.Add(item);
                        }
                        table.ExecuteBatch(batch);
                        batchCount++;
                    }
                    catch (StorageException ex)
                    {
                        var a = batchCount;
                        batchCount++;
                    }
                }
            }
        }

        /// <summary>
        /// Get the Azure Queue object by the queue name
        /// </summary>
        /// <param name="queueName"></param>
        /// <returns></returns>
        private CloudQueue GetQueueReference(string queueName)
        {
            var connectionString = ConfigurationManagerHelper.GetValuefromConfig(Constant.StorageConnectionString);
            var cloudStorageAccount = CloudStorageAccount.Parse(connectionString);
            var client = cloudStorageAccount.CreateCloudQueueClient();
            var queue = client.GetQueueReference(queueName);
            queue.CreateIfNotExists();
            return queue;
        }

        #endregion Private Methods
    }
}