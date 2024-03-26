using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Booking;
using Isango.Entities.Booking.RequestModels;
using Isango.Entities.Enums;
using Isango.Entities.GoogleMaps;
using Isango.Entities.GoogleMaps.BookingServer;
using Logger.Contract;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TableStorageOperations.Contracts;
using TableStorageOperations.Models;
using TableStorageOperations.Models.AdditionalPropertiesModels;
using TableStorageOperations.Models.AdditionalPropertiesModels.Availabilities;
using TableStorageOperations.Models.AdditionalPropertiesModels.Booking;
using TableStorageOperations.Models.Booking;
using TableStorageOperations.StorageOperators;
using Util;
using APIPriceOffer = TableStorageOperations.Models.AdditionalPropertiesModels.AppliedPriceOffers.PriceOffer;
using Constant = TableStorageOperations.Constants.Constant;

using RezdyPickUpLocation = TableStorageOperations.Models.AdditionalPropertiesModels.PickUpLocation;

namespace TableStorageOperations.TableStorageOperations
{
    public class TableStorageOperation : ITableStorageOperation
    {
        private readonly ILogger _log;
        private readonly IConfigReader _configReader;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="log"></param>
        /// <param name="configReader"></param>
        public TableStorageOperation(ILogger log, IConfigReader configReader)
        {
            _log = log;
            _configReader = configReader;
        }

        /// <summary>
        /// Inserts the additional availability properties for the given type
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        public void InsertData(Activity activity, string tokenId)
        {
            try
            {
                var entityBuilderName = _configReader.AvailabilityEntity(activity.ApiType.ToString());
                if (entityBuilderName != null)
                {
                    var type = Type.GetType(entityBuilderName);
                    if (type != null)
                    {
                        var instance = Activator.CreateInstance(type);
                        var baseEntityBuilder = instance as BaseStorageOperator;
                        baseEntityBuilder?.InsertAvailabilitiesData(activity, tokenId);
                    }
                }
            }
            catch (StorageException ex)
            {
                Task.Run(() =>
                    _log.Error(new IsangoErrorEntity
                    {
                        ClassName = "TableStorageOperations",
                        MethodName = "InsertData",
                        Token = tokenId,
                        Params = $"{SerializeDeSerializeHelper.Serialize(activity)}"
                    }, ex)
                );
                throw;
            }
            catch (Exception ex)
            {
                Task.Run(() =>
                     _log.Error(new IsangoErrorEntity
                     {
                         ClassName = "TableStorageOperations",
                         MethodName = "InsertData",
                         Token = tokenId,
                         Params = $"{SerializeDeSerializeHelper.Serialize(activity)}"
                     }, ex)
                 );
                throw;
            }
        }

        /// <summary>
        /// Retrieves the API specific additional availability properties for the given referenceId and partitionKey
        /// </summary>
        /// <param name="referenceId"></param>
        /// <param name="partitionKey"></param>
        /// <param name="apiTypeId"></param>
        /// <returns></returns>
        public TableEntity Retrieve(string referenceId, string partitionKey, int apiTypeId)
        {
            try
            {
                var apiType = (APIType)apiTypeId;
                var entityBuilderName = _configReader.AvailabilityEntity(apiType.ToString());

                var type = Type.GetType(entityBuilderName);
                if (type == null) return null;
                var instance = Activator.CreateInstance(type);
                var baseEntityBuilder = (BaseStorageOperator)instance;

                return baseEntityBuilder.Retrieve(referenceId, partitionKey);
            }
            catch (StorageException ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "TableStorageOperations",
                    MethodName = "Retrieve",
                    Params = $"{referenceId},{partitionKey},{apiTypeId}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "TableStorageOperations",
                    MethodName = "Retrieve",
                    Params = $"{referenceId},{partitionKey},{apiTypeId}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Retrieves the additional availability properties for the given referenceId and partitionKey
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="referenceId"></param>
        /// <param name="partitionKey"></param>
        /// <returns></returns>
        public T RetrieveData<T>(string referenceId, string partitionKey) where T : ITableEntity, new()
        {
            return RetrieveData<T>(partitionKey, referenceId, Constant.Availabilties);
        }

        /// <summary>
        /// Inserts the booking request model in the table storage
        /// </summary>
        /// <param name="request"></param>
        /// <param name="bookingGuid"></param>
        /// <param name="bookingReferenceNumber"></param>
        /// <returns></returns>
        public void InsertBookingRequest(CreateBookingRequest request, string bookingGuid,
            string bookingReferenceNumber, BookingResponse bookingResponse = null, string error = null,
            string statusCode = "500", string token = "")
        {
            try
            {
                Enum.TryParse(request?.PaymentDetail?.PaymentGateway, true, out PaymentGatewayType gatewayType);
                if (gatewayType == PaymentGatewayType.WireCard || gatewayType == PaymentGatewayType.Apexx || gatewayType == PaymentGatewayType.Adyen)
                    request.PaymentDetail.CardDetails = null;  // Remove the security code before storing
                var bookingRequest = new BookingRequest
                {
                    PartitionKey = GetPartitionKey(),
                    RowKey = !string.IsNullOrEmpty(bookingGuid) ? bookingGuid?.Replace("/", "") : DateTime.Now.ToString("yyMMddHHmmssff"),
                    BookingReferenceNumber = bookingReferenceNumber,
                    CreateBookingRequest = SerializeDeSerializeHelper.Serialize(request),
                    CreateBookingResponse = String.IsNullOrEmpty(error) ? SerializeDeSerializeHelper.Serialize(bookingResponse) : error,
                    Status = String.IsNullOrEmpty(error) ? bookingResponse?.Status : "error",
                    StatusCode = statusCode,
                    Token = token
                };

                var table = GetTableReference(Constant.BookingRequest);
                var tableOperation = TableOperation.InsertOrReplace(bookingRequest);
                table.Execute(tableOperation);
            }
            catch (StorageException ex)
            {
                Task.Run(() =>
                        _log.Error(new IsangoErrorEntity
                        {
                            ClassName = "TableStorageOperations",
                            MethodName = "InsertBookingRequest",
                            Token = request.TokenId,
                            AffiliateId = request.AffiliateId,
                            Params = $"{SerializeDeSerializeHelper.Serialize(request)}"
                        }, ex)
                );
                throw;
            }
            catch (Exception ex)
            {
                Task.Run(() =>
                    _log.Error(new IsangoErrorEntity
                    {
                        ClassName = "TableStorageOperations",
                        MethodName = "InsertBookingRequest",
                        Token = request.TokenId,
                        AffiliateId = request.AffiliateId,
                        Params = $"{SerializeDeSerializeHelper.Serialize(request)}"
                    }, ex)
                );
                throw;
            }
        }

        public void InsertReservationRequest(CreateBookingRequest request, string bookingGuid,
            string bookingReferenceNumber, ReservationResponse reservationResponse = null, string error = null,
            string statusCode = "500", string token = "")
        {
            try
            {
                Enum.TryParse(request?.PaymentDetail?.PaymentGateway, true, out PaymentGatewayType gatewayType);
                if (gatewayType == PaymentGatewayType.WireCard || gatewayType == PaymentGatewayType.Apexx || gatewayType == PaymentGatewayType.Adyen)
                    request.PaymentDetail.CardDetails = null;  // Remove the security code before storing
                var bookingRequest = new BookingRequest
                {
                    PartitionKey = GetPartitionKey(),
                    RowKey = DateTime.Now.ToString("yyMMddHHmmssff"),
                    BookingReferenceNumber = bookingReferenceNumber,
                    CreateBookingRequest = SerializeDeSerializeHelper.Serialize(request),
                    CreateBookingResponse = String.IsNullOrEmpty(error) ? SerializeDeSerializeHelper.Serialize(reservationResponse) : error,
                    Status = String.IsNullOrEmpty(error) ? reservationResponse?.Success.ToString() : "error",
                    StatusCode = statusCode,
                    Token = token
                };

                var table = GetTableReference(Constant.BookingRequest);
                var tableOperation = TableOperation.InsertOrReplace(bookingRequest);
                table.Execute(tableOperation);
            }
            catch (StorageException ex)
            {
                Task.Run(() =>
                        _log.Error(new IsangoErrorEntity
                        {
                            ClassName = "TableStorageOperations",
                            MethodName = "InsertReservationRequest",
                            Token = request.TokenId,
                            AffiliateId = request.AffiliateId,
                            Params = $"{SerializeDeSerializeHelper.Serialize(request)}"
                        }, ex)
                );
                throw;
            }
            catch (Exception ex)
            {
                Task.Run(() =>
                    _log.Error(new IsangoErrorEntity
                    {
                        ClassName = "TableStorageOperations",
                        MethodName = "InsertReservationRequest",
                        Token = request.TokenId,
                        AffiliateId = request.AffiliateId,
                        Params = $"{SerializeDeSerializeHelper.Serialize(request)}"
                    }, ex)
                );
                throw;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="bookingGuid"></param>
        /// <param name="bookingReferenceNumber"></param>
        /// <param name="status"></param>
        /// <param name="bookingResponse"></param>
        /// <param name="token"></param>
        public void InsertBookingCallBackRequest(string bookingGuid, string bookingReferenceNumber, string status,
            BookingResponse bookingResponse, string token, string error = null,
            string statusCode = "500")
        {
            try
            {
                var bookingCallBackRequest = new BookingCallBackRequest
                {
                    PartitionKey = GetPartitionKey(),
                    RowKey = bookingGuid,
                    BookingReferenceNumber = bookingReferenceNumber,
                    Status = String.IsNullOrEmpty(error) ? status : "error",
                    BookingResponse = String.IsNullOrEmpty(error) ? SerializeDeSerializeHelper.Serialize(bookingResponse) : error,
                    Token = token,
                    StatusCode = statusCode
                };
                var table = GetTableReference(Constant.BookingCallBackRequest);
                var tableOperation = TableOperation.InsertOrReplace(bookingCallBackRequest);
                table.Execute(tableOperation);
            }
            catch (StorageException ex)
            {
                _log.Error($"TableStorageOperations|InsertBookingCallBackRequest|{SerializeDeSerializeHelper.Serialize(bookingResponse)}", ex);

                throw;
            }
            catch (Exception ex)
            {
                _log.Error($"TableStorageOperations|InsertBookingCallBackRequest|{SerializeDeSerializeHelper.Serialize(bookingResponse)}", ex);

                throw;
            }
        }

        /// <summary>
        /// Retrieves the booking request for the given referenceId and partitionKey
        /// </summary>
        /// <param name="bookingGuid"></param>
        /// <returns></returns>
        public Tuple<string, CreateBookingRequest> RetrieveBookingRequest(string bookingGuid)
        {
            try
            {
                BookingRequest request;

                var table = GetTableReference(Constant.BookingRequest);
                BookingRequest AvailabilitiesEntityResolver(string pk, string rk, DateTimeOffset ts, IDictionary<string, EntityProperty> props, string etag)
                {
                    var resolvedEntity = new BookingRequest() { PartitionKey = pk, RowKey = rk, Timestamp = ts, ETag = etag };

                    foreach (var item in props.Where(p => p.Key.StartsWith(Constant.DecimalPrefix)))
                    {
                        var realPropertyName = item.Key.Substring(Constant.DecimalPrefix.Length);
                        var propertyInfo = resolvedEntity.GetType()?.GetProperty(realPropertyName);
                        propertyInfo?.SetValue(resolvedEntity, Convert.ChangeType(item.Value.StringValue, propertyInfo.PropertyType), null);
                    }
                    resolvedEntity.ReadEntity(props, null);
                    return resolvedEntity;
                }
                var retrieveOperation = TableOperation.Retrieve(GetPartitionKey(), bookingGuid, AvailabilitiesEntityResolver);
                var retrievedResult = table.Execute(retrieveOperation).Result;

                if (retrievedResult is BookingRequest)
                {
                    request = (BookingRequest)retrievedResult;
                }
                else
                {
                    var query = new TableQuery<BookingRequest>().Where(
                        TableQuery.GenerateFilterCondition(Constant.RowKey, QueryComparisons.Equal, bookingGuid));
                    request = table.ExecuteQuery(query).FirstOrDefault();
                }

                if (request == null) return null;
                var bookingReferenceNumber = request.BookingReferenceNumber;
                var bookingRequest = SerializeDeSerializeHelper.DeSerialize<CreateBookingRequest>(request.CreateBookingRequest);

                if (bookingRequest == null) return null;

                //Check for Receive Payment
                if (bookingRequest.SelectedProducts == null || bookingRequest.SelectedProducts.Count == 0)
                {
                    try
                    {
                        var receiveRequest = SerializeDeSerializeHelper.DeSerialize<CreateReceiveBookingRequest>(request.CreateBookingRequest);
                        if (receiveRequest.AmendmentId > 0)
                        {
                            bookingRequest = receiveRequest;
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }

                var bookingData = Tuple.Create(bookingReferenceNumber, bookingRequest);
                return bookingData;
            }
            catch (StorageException ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "TableStorageOperations",
                    MethodName = "RetrieveBookingRequest",
                    Params = $"{bookingGuid}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            catch (Exception)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "TableStorageOperations",
                    MethodName = "RetrieveBookingRequest",
                    Params = $"{bookingGuid}"
                };
                throw;
            }
        }

        public Tuple<string, BookingResponse> RetrieveBookingCallBackRequest(string bookingGuid)
        {
            try
            {
                BookingCallBackRequest request = null;
                BookingResponse bookingResponse = null;

                var table = GetTableReference(Constant.BookingCallBackRequest);
                BookingCallBackRequest AvailabilitiesEntityResolver(string pk, string rk, DateTimeOffset ts, IDictionary<string, EntityProperty> props, string etag)
                {
                    var resolvedEntity = new BookingCallBackRequest() { PartitionKey = pk, RowKey = rk, Timestamp = ts, ETag = etag };

                    foreach (var item in props.Where(p => p.Key.StartsWith(Constant.DecimalPrefix)))
                    {
                        string realPropertyName = item.Key.Substring(Constant.DecimalPrefix.Length);
                        System.Reflection.PropertyInfo propertyInfo = resolvedEntity.GetType()?.GetProperty(realPropertyName);
                        propertyInfo?.SetValue(resolvedEntity, Convert.ChangeType(item.Value.StringValue, propertyInfo.PropertyType), null);
                    }
                    resolvedEntity.ReadEntity(props, null);
                    return resolvedEntity;
                }
                var retrieveOperation = TableOperation.Retrieve(GetPartitionKey(), bookingGuid, AvailabilitiesEntityResolver);
                var retrievedResult = table.Execute(retrieveOperation).Result;

                if (retrievedResult != null && (retrievedResult is BookingCallBackRequest))
                {
                    request = (BookingCallBackRequest)retrievedResult;
                }
                else
                {
                    var query = new TableQuery<BookingCallBackRequest>().Where(
                        TableQuery.GenerateFilterCondition(Constant.RowKey, QueryComparisons.Equal, bookingGuid));
                    request = table.ExecuteQuery(query).FirstOrDefault();
                }

                if (request == null) return null;

                bookingResponse = SerializeDeSerializeHelper.DeSerialize<BookingResponse>(request.BookingResponse);

                string status = request?.Status;
                var bookingData = Tuple.Create(status, bookingResponse);
                return bookingData;
            }
            catch (StorageException ex)
            {
                _log.Error($"TableStorageOperations|RetrieveBookingRequest|{bookingGuid}", ex);
                throw;
            }
            catch (Exception ex)
            {
                _log.Error($"TableStorageOperations|RetrieveBookingRequest|{bookingGuid}", ex);
                throw;
            }
        }

        /// <summary>
        /// Retrieves the price offers for the given referenceId
        /// </summary>
        /// <param name="rowKey"></param>
        /// <param name="partitionKey"></param>
        /// <returns></returns>
        public List<PriceOffer> RetrievePriceOfferData(string partitionKey, string rowKey)
        {
            var table = GetTableReference(Constant.AppliedPriceOffersTable);

            var priceOffers = new List<PriceOffer>();
            var splittedKeys = rowKey.Split(Constant.PipeSeparator);

            foreach (var key in splittedKeys)
            {
                APIPriceOffer AvailabilitiesEntityResolver(string pk, string rk, DateTimeOffset ts, IDictionary<string, EntityProperty> props, string etag)
                {
                    var resolvedEntity = new APIPriceOffer() { PartitionKey = pk, RowKey = rk, Timestamp = ts, ETag = etag };

                    foreach (var item in props.Where(p => p.Key.StartsWith(Constant.DecimalPrefix)))
                    {
                        var realPropertyName = item.Key.Substring(Constant.DecimalPrefix.Length);
                        var propertyInfo = resolvedEntity.GetType()?.GetProperty(realPropertyName);
                        propertyInfo?.SetValue(resolvedEntity, Convert.ChangeType(item.Value.StringValue, propertyInfo.PropertyType), null);
                    }
                    resolvedEntity.ReadEntity(props, null);
                    return resolvedEntity;
                }

                var retrieveOperation = TableOperation.Retrieve(partitionKey, key, AvailabilitiesEntityResolver);

                var retrievedResult = table.Execute(retrieveOperation).Result;
                var apiPriceOffer = (APIPriceOffer)retrievedResult;

                if (apiPriceOffer == null) continue;
                var priceOffer = new PriceOffer
                {
                    Id = apiPriceOffer.AppliedId,
                    RuleName = apiPriceOffer.RuleName,
                    OfferPercent = apiPriceOffer.OfferPercent,
                    ModuleName = apiPriceOffer.ModuleName,
                    SaleAmount = apiPriceOffer.SaleAmount,
                    CostAmount = apiPriceOffer.CostAmount

                };
                priceOffers.Add(priceOffer);
            }
            return priceOffers;
        }

        /// <summary>
        /// To retrive the data from storage
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="partitionKey"></param>
        /// <param name="rowKey"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public T RetrieveData<T>(string partitionKey, string rowKey, string tableName) where T : ITableEntity, new()
        {
            try
            {
                var table = GetTableReference(tableName);
                T AvailabilitiesEntityResolver(string pk, string rk, DateTimeOffset ts, IDictionary<string, EntityProperty> props, string etag)
                {
                    var resolvedEntity = new T { PartitionKey = pk, RowKey = rk, Timestamp = ts, ETag = etag };

                    foreach (var item in props.Where(p => p.Key.StartsWith(Constant.DecimalPrefix)))
                    {
                        var realPropertyName = item.Key.Substring(Constant.DecimalPrefix.Length);
                        var propertyInfo = resolvedEntity.GetType()?.GetProperty(realPropertyName);
                        propertyInfo?.SetValue(resolvedEntity, Convert.ChangeType(item.Value.StringValue, propertyInfo.PropertyType), null);
                    }
                    resolvedEntity.ReadEntity(props, null);
                    return resolvedEntity;
                }

                var retrieveOperation = TableOperation.Retrieve(partitionKey, rowKey, AvailabilitiesEntityResolver);

                var retrievedResult = table.Execute(retrieveOperation);
                if (retrievedResult.Result is T)
                    return (T)retrievedResult.Result;

                try
                {
                    return (T)Convert.ChangeType(retrievedResult.Result, typeof(T));
                }
                catch (InvalidCastException)
                {
                    return default(T);
                }
            }
            catch (StorageException ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "TableStorageOperations",
                    MethodName = "RetrieveData",
                    Params = $"{tableName}-{partitionKey}-{rowKey}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "TableStorageOperations",
                    MethodName = "RetrieveData",
                    Params = $"{tableName}-{partitionKey}-{rowKey}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public void InsertAsyncBookingDetails(AsyncBooking asyncBooking)
        {
            try
            {
                asyncBooking.RowKey = asyncBooking.OrderReferenceId;
                asyncBooking.PartitionKey = GetPartitionKey();
                asyncBooking.Id = Guid.NewGuid().ToString();

                var table = GetTableReference(Constant.AsyncBooking);
                var tableOperation = TableOperation.Insert(asyncBooking);
                table.Execute(tableOperation);
            }
            catch (StorageException ex)
            {
                _log.Error($"APIStorageOperation|InsertAsyncBookingDetails|{SerializeDeSerializeHelper.Serialize(asyncBooking)}", ex);
                throw;
            }
            catch (Exception ex)
            {
                _log.Error($"APIStorageOperation|InsertAsyncBookingDetails|{SerializeDeSerializeHelper.Serialize(asyncBooking)}", ex);
                throw;
            }
        }

        public void InsertReservationDetails(SupplierBookingReservationResponse reservationDetails)
        {
            try
            {
                reservationDetails.RowKey = $"{reservationDetails.BookingReferenceNo}-{reservationDetails.AvailabilityReferenceId}";
                reservationDetails.PartitionKey = GetPartitionKey();
                reservationDetails.Id = Guid.NewGuid().ToString();

                var table = GetTableReference(Constant.Reservations);
                var tableOperation = TableOperation.Insert(reservationDetails);
                table.Execute(tableOperation);
            }
            catch (StorageException ex)
            {
                _log.Error($"APIStorageOperation|InsertReservationDetails|{SerializeDeSerializeHelper.Serialize(reservationDetails)}", ex);
                //throw;
            }
            catch (Exception ex)
            {
                _log.Error($"APIStorageOperation|InsertReservationDetails|{SerializeDeSerializeHelper.Serialize(reservationDetails)}", ex);
                //throw;
            }
        }

        public SupplierBookingReservationResponse RetrieveReservationData(string rowKey)
        {
            try
            {
                var partitionKey = GetPartitionKey();
                var table = GetTableReference(Constant.Reservations);
                SupplierBookingReservationResponse AvailabilitiesEntityResolver(string pk, string rk, DateTimeOffset ts, IDictionary<string, EntityProperty> props, string etag)
                {
                    var resolvedEntity = new SupplierBookingReservationResponse { PartitionKey = pk, RowKey = rk, Timestamp = ts, ETag = etag };

                    foreach (var item in props.Where(p => p.Key.StartsWith(Constant.DecimalPrefix)))
                    {
                        var realPropertyName = item.Key.Substring(Constant.DecimalPrefix.Length);
                        var propertyInfo = resolvedEntity.GetType()?.GetProperty(realPropertyName);
                        propertyInfo?.SetValue(resolvedEntity, Convert.ChangeType(item.Value.StringValue, propertyInfo.PropertyType), null);
                    }
                    resolvedEntity.ReadEntity(props, null);
                    return resolvedEntity;
                }

                var retrieveOperation = TableOperation.Retrieve(partitionKey, rowKey, AvailabilitiesEntityResolver);

                var retrievedResult = table.Execute(retrieveOperation);
                if (retrievedResult.Result is SupplierBookingReservationResponse)
                    return (SupplierBookingReservationResponse)retrievedResult.Result;

                try
                {
                    return (SupplierBookingReservationResponse)Convert.ChangeType(retrievedResult.Result, typeof(SupplierBookingReservationResponse));
                }
                catch (InvalidCastException)
                {
                    return default(SupplierBookingReservationResponse);
                }
            }
            catch (StorageException ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "TableStorageOperations",
                    MethodName = "RetrieveData",
                    Params = $"{Constant.Reservations}-{GetPartitionKey()}-{rowKey}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "TableStorageOperations",
                    MethodName = "RetrieveData",
                    Params = $"{Constant.Reservations}-{GetPartitionKey()}-{rowKey}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Get Data from azure table for the given api excluding given column value. To Filter out records where processing not required.
        /// </summary>
        /// <param name="apiType"></param>
        /// <param name="azureTableColumnName">Case Sensitive Column</param>
        /// <param name="azureTableColumnValue">Case Sensitive Value</param>
        /// <returns></returns>
        public List<AsyncBooking> RetrieveAsyncBooking(int apiType, string azureTableColumnName, string azureTableColumnValue)
        {
            var asyncBookingData = new List<AsyncBooking>();
            try
            {
                var bookingReferenceNumber = string.Empty;

                var table = GetTableReference(Constant.AsyncBooking);

                var query1 = new TableQuery<AsyncBooking>();

                var query = new TableQuery<AsyncBooking>()
                        .Where
                        (
                            TableQuery.CombineFilters
                            (
                                TableQuery.GenerateFilterCondition
                                        (
                                            azureTableColumnName, QueryComparisons.NotEqual, azureTableColumnValue
                                        ),

                                TableOperators.And,

                                TableQuery.GenerateFilterConditionForInt
                                        (
                                            "ApiType", QueryComparisons.Equal, apiType
                                        )
                            )
                    )
                    ;

                var result = table.ExecuteQuery(query1)?.ToList();

                if (result.Any())
                {
                    asyncBookingData.AddRange(result);
                }
            }
            catch (StorageException ex)
            {
                _log.Error($"APIStorageOperation|RetrieveTiqetsGetTicketsData", ex);
                //throw;
            }
            catch (Exception ex)
            {
                _log.Error($"APIStorageOperation|RetrieveTiqetsGetTicketsData", ex);
                //throw;
            }
            return asyncBookingData;
        }

        /// <summary>
        /// Get Data from azure table for the given api excluding given column value. To Filter out records where processing not required.
        /// </summary>
        /// <param name="apiType"></param>
        /// <param name="azureTableColumnName">Case Sensitive Column</param>
        /// <param name="azureTableColumnValue">Case Sensitive Value</param>
        /// <returns></returns>
        public List<AsyncBooking> RetrieveAsyncBookingRowKey(int apiType, string azureTableColumnName, string azureTableColumnValue)
        {
            var asyncBookingData = new List<AsyncBooking>();
            try
            {
                var bookingReferenceNumber = string.Empty;

                var table = GetTableReference(Constant.AsyncBooking);

                var query = new TableQuery<AsyncBooking>()
                        .Where
                        (
                            TableQuery.CombineFilters
                            (
                                TableQuery.GenerateFilterCondition
                                        (
                                            azureTableColumnName, QueryComparisons.Equal, azureTableColumnValue
                                        ),

                                TableOperators.And,

                                TableQuery.GenerateFilterConditionForInt
                                        (
                                            "ApiType", QueryComparisons.Equal, apiType
                                        )
                            )
                    )
                    ;

                var result = table.ExecuteQuery(query)?.ToList();

                if (result.Any())
                {
                    asyncBookingData.AddRange(result);
                }
            }
            catch (StorageException ex)
            {
                _log.Error($"APIStorageOperation|RetrieveAsyncBookingRowKey", ex);
                //throw;
            }
            catch (Exception ex)
            {
                _log.Error($"APIStorageOperation|RetrieveAsyncBookingRowKey", ex);
                //throw;
            }
            return asyncBookingData;
        }

        public List<AsyncBooking> RetrieveFailedWebhookAsyncBooking(int apiType, string azureTableColumnName, string azureTableColumnValue)
        {
            var asyncBookingData = new List<AsyncBooking>();
            try
            {
                var bookingReferenceNumber = string.Empty;

                var table = GetTableReference(Constant.AsyncBooking);

                var query1 = new TableQuery<AsyncBooking>();

                var query = new TableQuery<AsyncBooking>()
                        .Where
                        (
                            TableQuery.CombineFilters
                            (
                                TableQuery.GenerateFilterCondition
                                        (
                                            azureTableColumnName, QueryComparisons.Equal, azureTableColumnValue
                                        ),

                                TableOperators.And,

                                TableQuery.GenerateFilterConditionForInt
                                        (
                                            "ApiType", QueryComparisons.Equal, apiType
                                        )
                            )
                    )
                    ;

                var result = table.ExecuteQuery(query1)?.ToList();

                if (result.Any())
                {
                    asyncBookingData.AddRange(result);
                }
            }
            catch (StorageException ex)
            {
                _log.Error($"APIStorageOperation|RetrieveFailedWebhookAsyncBooking", ex);
                //throw;
            }
            catch (Exception ex)
            {
                _log.Error($"APIStorageOperation|RetrieveFailedWebhookAsyncBooking", ex);
                //throw;
            }
            return asyncBookingData;
        }

        public void UpdateTiqetsGetTicketData(TiqetsTableStorageData tiqetsTableStorageData, AsyncBooking asyncBooking)
        {
            try
            {
                asyncBooking.PartitionKey = asyncBooking.PartitionKey;
                asyncBooking.RowKey = tiqetsTableStorageData.OrderReferenceId;
                asyncBooking.RetryCount = tiqetsTableStorageData.RetryCount;
                asyncBooking.NextProcessingTime = tiqetsTableStorageData.NextProcessingTime;
                asyncBooking.AvailabilityReferenceId = tiqetsTableStorageData.AvailabilityReferenceId;
                asyncBooking.OptionName = tiqetsTableStorageData.OptionName;
                asyncBooking.ServiceOptionId = tiqetsTableStorageData.ServiceOptionId;
                asyncBooking.VoucherLink = tiqetsTableStorageData.VoucherLink;
                asyncBooking.BookedOptionId = tiqetsTableStorageData.BookedOptionId;
                asyncBooking.Status = tiqetsTableStorageData.Status;
                asyncBooking.WebhookUrl = tiqetsTableStorageData.WebhookUrl;
                asyncBooking.WebhookRequest = tiqetsTableStorageData.WebhookRequest;
                asyncBooking.WebhookResponse = tiqetsTableStorageData.WebhookResponse;
                asyncBooking.IsWebhookSuccess = tiqetsTableStorageData.IsWebhookSuccess;
                asyncBooking.WebhookRetryCount = tiqetsTableStorageData.WebhookRetryCount;

                var table = GetTableReference(Constant.AsyncBooking);
                var tableOperation = TableOperation.Replace(asyncBooking);
                table.Execute(tableOperation);
            }
            catch (StorageException ex)
            {
                _log.Error($"APIStorageOperation|RetrieveTiqetsGetTicketsData|{SerializeDeSerializeHelper.Serialize(tiqetsTableStorageData)}|{SerializeDeSerializeHelper.Serialize(asyncBooking)}", ex);
                //throw;
            }
            catch (Exception ex)
            {
                _log.Error($"APIStorageOperation|RetrieveTiqetsGetTicketsData|{SerializeDeSerializeHelper.Serialize(tiqetsTableStorageData)}|{SerializeDeSerializeHelper.Serialize(asyncBooking)}", ex);
                //throw;
            }
        }

        public void InsertTiqetsGetTicketDetailsLog(string asyncBookingId, string orderReferenceId, string status, string processedDatetime, string request, string response, string requestType)
        {
            try
            {
                var asyncBookingLog = new AsyncBookingLog
                {
                    PartitionKey = GetPartitionKey(),
                    RowKey = orderReferenceId,
                    Id = Guid.NewGuid().ToString(),
                    AsyncBookingId = asyncBookingId,
                    Status = status,
                    ProcessedDateTime = processedDatetime,
                    Request = request,
                    Response = response,
                    RequestType = requestType
                };

                var table = GetTableReference(Constant.AsyncBookingLog);
                var tableOperation = TableOperation.Insert(asyncBookingLog);
                table.Execute(tableOperation);
            }
            catch (StorageException ex)
            {
                _log.Error($"APIStorageOperation|InsertTiqetsGetTicketDetailsLog|{asyncBookingId}|{orderReferenceId}|{status}|{processedDatetime}", ex);
                //throw;
            }
            catch (Exception ex)
            {
                _log.Error($"APIStorageOperation|InsertTiqetsGetTicketDetailsLog|{asyncBookingId}|{orderReferenceId}|{status}|{processedDatetime}", ex);
                //throw;
            }
        }

        /// <summary>
        /// Inserts the booking request model in the table storage
        /// </summary>
        /// <param name="rezdyPickUpLocation"></param>
        /// <param name="pickUpId"></param>
        /// <returns></returns>
        public void InsertRezdyPickUpLocations(List<Isango.Entities.Rezdy.RezdyPickUpLocation> rezdyPickUpLocations, int pickUpId)
        {
            try
            {
                var batchOperation = new TableBatchOperation();

                foreach (var rezdyPickUpLocation in rezdyPickUpLocations)
                {
                    var pickUpLocation = new RezdyPickUpLocation.PickUpLocation
                    {
                        PartitionKey = pickUpId.ToString(),
                        RowKey = $"{pickUpId}_{rezdyPickUpLocation.Id}",
                        PickUpLocations = SerializeDeSerializeHelper.Serialize(rezdyPickUpLocation)
                    };

                    batchOperation.Insert(pickUpLocation);
                }

                var table = GetTableReference(Constant.PickUpLocation);
                if (batchOperation.Count > 0)
                {
                    table.ExecuteBatch(batchOperation);
                }
            }
            catch (StorageException ex)
            {
                _log.Error($"APIStorageOperation|InsertBookingRequest|{SerializeDeSerializeHelper.Serialize(rezdyPickUpLocations)},{pickUpId}", ex);
                throw;
            }
            catch (Exception ex)
            {
                _log.Error($"APIStorageOperation|InsertBookingRequest|{SerializeDeSerializeHelper.Serialize(rezdyPickUpLocations)},{pickUpId}", ex);
                throw;
            }
        }

        /// <summary>
        /// Retrieves the booking request for the given referenceId and partitionKey
        /// </summary>
        /// <param name="pickUpId"></param>
        /// <param name="pickUpLocationId"></param>
        /// <returns></returns>
        public Isango.Entities.Rezdy.RezdyPickUpLocation RetrievePickUpLocation(int pickUpId, string pickUpLocationId)
        {
            try
            {
                RezdyPickUpLocation.PickUpLocation location = null;

                var table = GetTableReference(Constant.PickUpLocation);
                RezdyPickUpLocation.PickUpLocation AvailabilitiesEntityResolver(string pk, string rk, DateTimeOffset ts, IDictionary<string, EntityProperty> props, string etag)
                {
                    var resolvedEntity = new RezdyPickUpLocation.PickUpLocation() { PartitionKey = pk, RowKey = rk, Timestamp = ts, ETag = etag };

                    foreach (var item in props.Where(p => p.Key.StartsWith(Constant.DecimalPrefix)))
                    {
                        string realPropertyName = item.Key.Substring(Constant.DecimalPrefix.Length);
                        System.Reflection.PropertyInfo propertyInfo = resolvedEntity.GetType()?.GetProperty(realPropertyName);
                        propertyInfo?.SetValue(resolvedEntity, Convert.ChangeType(item.Value.StringValue, propertyInfo.PropertyType), null);
                    }
                    resolvedEntity.ReadEntity(props, null);
                    return resolvedEntity;
                }
                var retrieveOperation = TableOperation.Retrieve(pickUpId.ToString(), $"{pickUpId}_{pickUpLocationId}", AvailabilitiesEntityResolver);
                var retrievedResult = table.Execute(retrieveOperation).Result;

                if (retrievedResult is RezdyPickUpLocation.PickUpLocation rezdyPickUplocation)
                {
                    location = rezdyPickUplocation;
                }
                else
                {
                    var query = new TableQuery<RezdyPickUpLocation.PickUpLocation>().Where(
                        TableQuery.GenerateFilterCondition(Constant.RowKey, QueryComparisons.Equal, $"{pickUpId}_{pickUpLocationId}"));
                    location = table.ExecuteQuery(query).FirstOrDefault();
                }

                return SerializeDeSerializeHelper.DeSerialize<Isango.Entities.Rezdy.RezdyPickUpLocation>(location?.PickUpLocations);
            }
            catch (StorageException ex)
            {
                _log.Error($"APIStorageOperation|RetrieveBookingRequest|{pickUpId}", ex);
                throw;
            }
            catch (Exception ex)
            {
                _log.Error($"APIStorageOperation|RetrieveBookingRequest|{pickUpId}", ex);
                throw;
            }
        }


        public void UpdateGlobalTixData(GlobalTixAvailabilities globalTixAvailabilities)
        {
            try
            {
                var table = GetTableReference(Constant.Availabilties);
                var tableOperation = TableOperation.Replace(globalTixAvailabilities);
                table.Execute(tableOperation);
            }
            catch (StorageException ex)
            {
                _log.Error($"APIStorageOperation|UpdateGlobalTixData|{SerializeDeSerializeHelper.Serialize(globalTixAvailabilities)}", ex);
                throw;
            }
            catch (Exception ex)
            {
                _log.Error($"APIStorageOperation|UpdateGlobalTixData|{SerializeDeSerializeHelper.Serialize(globalTixAvailabilities)}", ex);
                throw;
            }
        }

        #region GoogleMaps Booking Server

        /// <summary>
        /// Inserts the Order Response in the table storage
        /// </summary>
        /// <param name="order"></param>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        public void InsertOrderResponse(Order order, string tokenId)
        {
            try
            {
                var orderResponse = new OrderResponse
                {
                    PartitionKey = DateTime.UtcNow.ToString("MMM_yyyy"),
                    RowKey = tokenId,
                    OrderId = order.OrderId,
                    UserId = order.UserInformation.UserId,
                    Order = SerializeDeSerializeHelper.Serialize(order),
                    BookingStatus = Isango.Entities.GoogleMaps.BookingStatus.Confirmed.ToString(),
                    IsNotifiedToGoogle = true
                };

                var table = GetTableReference(Constant.GoogleMapsOrder);
                var tableOperation = TableOperation.InsertOrReplace(orderResponse);
                table.Execute(tableOperation);
            }
            catch (StorageException ex)
            {
                var errorEntity = new IsangoErrorEntity
                {
                    ClassName = "TableStorageOperations",
                    MethodName = "InsertOrderResponse",
                    Params = $"{SerializeDeSerializeHelper.Serialize(order)}"
                };
                _log.Error(errorEntity, ex);
                throw;
            }
            catch (Exception ex)
            {
                var errorEntity = new IsangoErrorEntity
                {
                    ClassName = "TableStorageOperations",
                    MethodName = "InsertOrderResponse",
                    Params = $"{SerializeDeSerializeHelper.Serialize(order)}"
                };
                _log.Error(errorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Retrieves the Order Response Data for the given orderId
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="queryParameter"></param>
        /// <returns></returns>
        public Order GetOrders(string columnName, string queryParameter)
        {
            var table = GetTableReference(Constant.GoogleMapsOrder);
            var queryString = TableQuery.GenerateFilterCondition(
                columnName, QueryComparisons.Equal, queryParameter
            );
            var query = new TableQuery<OrderResponse>().Where(queryString);
            var orderResponse = table.ExecuteQuery(query).FirstOrDefault();
            if (orderResponse == null) return null;

            var order = SerializeDeSerializeHelper.DeSerialize<Order>(orderResponse.Order);
            return order;
        }

        #endregion GoogleMaps Booking Server

        #region Private Methods

        /// <summary>
        /// Get the Azure Table Storage object by the table name
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public CloudTable GetTableReference(string tableName)
        {
            var connectionString = ConfigurationManagerHelper.GetValuefromConfig(Constant.StorageConnectionString);
            var cloudStorageAccount = CloudStorageAccount.Parse(connectionString);
            var client = cloudStorageAccount.CreateCloudTableClient();
            var table = client.GetTableReference(tableName);
            table.CreateIfNotExists();
            return table;
        }

        public CloudTable GetTableReferenceNet(string tableName)
        {
            var connectionString = ConfigurationManagerHelper.GetValuefromConfig(Constant.StorageConnectionStringNet);
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

        public void UpdatePrioHubWebhookData(string webhookResponse,
            int webhookRetryCount, string webhookURL, AsyncBooking asyncBooking, string isWebhookSuccess)

        {
            try
            {
                asyncBooking.IsWebhookSuccess = isWebhookSuccess;
                asyncBooking.WebhookRetryCount = webhookRetryCount;
                asyncBooking.WebhookUrl = webhookURL;
                asyncBooking.WebhookResponse = webhookResponse;
                var table = GetTableReference(Constant.AsyncBooking);
                var tableOperation = TableOperation.Replace(asyncBooking);
                table.Execute(tableOperation);
            }
            catch (StorageException ex)
            {
                _log.Error($"APIStorageOperation|UpdatePrioHubWebhookData|{SerializeDeSerializeHelper.Serialize(webhookResponse)}|{SerializeDeSerializeHelper.Serialize(asyncBooking)}", ex);
                //throw;
            }
            catch (Exception ex)
            {
                _log.Error($"APIStorageOperation|UpdatePrioHubWebhookData|{SerializeDeSerializeHelper.Serialize(webhookResponse)}|{SerializeDeSerializeHelper.Serialize(asyncBooking)}", ex);
                //throw;
            }
        }
        public  List<AvailabilityModelResponse> RetrieveAdapterLoggingData(string methodName, string apiName, string partitionKey)
        {
            var adapterLoggingData = new List<AvailabilityModelResponse>();

            try
            {
                var table = GetTableReferenceNet("AdapterLogging");


                var query = new TableQuery<AvailabilityModelResponse>()
                    .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey));

                var result = table.ExecuteQuery(query)?.ToList();
                if (result != null && result.Any())
                {
                    adapterLoggingData.AddRange(result);
                }
                adapterLoggingData = adapterLoggingData
                                   .Where(entry => entry.Method == methodName && entry.ApiName == apiName)
                                   .ToList();
            }
            catch (StorageException ex)
            {
                // Handle StorageException
                throw;
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                throw;
            }

            return adapterLoggingData;
        }

        #endregion Private Methods
    }
}