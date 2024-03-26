using Isango.Entities.Activities;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Table.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TableStorageOperations.Models.AdditionalPropertiesModels.Availabilities;
using Util;
using Constant = TableStorageOperations.Constants.Constant;

namespace TableStorageOperations.StorageOperators
{
    public class TicketsStorageOperator : BaseStorageOperator
    {
        /// <summary>
        /// Insert the additional properties on availabilities call in the table storage
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        public override void InsertAvailabilitiesData(Activity activity, string tokenId)
        {
            var productOptions = activity?.ProductOptions?.ToList();
            if (productOptions?.Count > 0)
            {
                var batchOperation = new TableBatchOperation();

                foreach (var productOption in productOptions)
                {
                    if (!(productOption is ActivityOption))
                    {
                        throw new InvalidCastException(Constant.InvalidCastErrorMessage);
                    }
                    var option = (ActivityOption)productOption;
                    if (option == null)
                    {
                        continue;
                    }

                    var validDates = productOption.BasePrice.DatePriceAndAvailabilty.Keys;

                    foreach (var date in validDates)
                    {
                        var validCancellationPolicies = option?.CancellationPrices?.Where(x => x.CancellationDateRelatedToOpreationDate.Date == date.Date)?.ToList();

                        var basePriceAndAvailability = GetPriceAndAvailability(productOption.BasePrice, productOption.TravelInfo, date);
                        var costPriceAndAvailability = GetPriceAndAvailability(productOption.CostPrice, productOption.TravelInfo, date);
                        var gatePriceAndAvailability = GetPriceAndAvailability(productOption.GateBasePrice, productOption.TravelInfo, date);
                        productOption.TravelInfo.StartDate = date;
                        var rowKey = GetRowKey();
                        var hbEntity = new TicketsAvailabilities
                        {
                            AvailToken = option.AvailToken,
                            ModalityCode = option.Code,
                            TicketCode = activity.Code,
                            IsPaxDetailRequired = activity.IsPaxDetailRequired,
                            Language = activity.LanguageCode,
                            BasePrice = basePriceAndAvailability.TotalPrice,
                            CostPrice = costPriceAndAvailability.TotalPrice,
                            GateBasePrice = gatePriceAndAvailability.TotalPrice,
                            CurrencyCode = productOption.BasePrice?.Currency?.IsoCode,
                            AvailabilityStatus = basePriceAndAvailability.AvailabilityStatus.ToString(),
                            OptionId = productOption.Id,
                            ServiceOptionId = productOption.ServiceOptionId,
                            OptionName = productOption.Name,
                            SupplierOptionCode = productOption.SupplierOptionCode,
                            ActivityId = activity.ID,
                            ApiType = (int)activity.ApiType,
                            Margin = option.Margin?.Value ?? 0,
                            OnSale = ProductSaleApplied(option.PriceOffers),
                            UnitType = GetUnitType(productOption),
                            PriceOfferReferenceId = InsertPriceOfferData(productOption.PriceOffers, rowKey),

                            BasePricingUnits = SerializeDeSerializeHelper.Serialize(basePriceAndAvailability?.PricingUnits),
                            GateBasePricingUnits = SerializeDeSerializeHelper.Serialize(gatePriceAndAvailability?.PricingUnits),
                            CostPricingUnits = SerializeDeSerializeHelper.Serialize(costPriceAndAvailability?.PricingUnits),
                            TravelInfo = SerializeDeSerializeHelper.Serialize(productOption.TravelInfo),

                            BundleOptionID = productOption.BundleOptionID,
                            BundleOptionName = productOption.BundleOptionName,
                            ComponentOrder = productOption.ComponentOrder,
                            ComponentServiceID = productOption.ComponentServiceID,
                            IsSameDayBookable = productOption.IsSameDayBookable,
                            PriceTypeID = productOption.PriceTypeID,
                            ContractName = option.Contract.Name,
                            IncomingOfficeCode = option.Contract.InComingOfficeCode,
                            Destination = activity.RegionName,
                            PartitionKey = tokenId,
                            RowKey = rowKey,
                            ContractDetails = SerializeDeSerializeHelper.Serialize(option.Contract),
                            RateKey = option.RateKey,
                            ContractQuestions = SerializeDeSerializeHelper.Serialize(option?.ContractQuestions),
                            ApiCancellationPolicy = option.ApiCancellationPolicy,
                            CancellationText = option.CancellationText,
                            CancellationPrices = SerializeDeSerializeHelper.Serialize(validCancellationPolicies),
                            TimeSlot = productOption?.StartTime.ToString() ?? ""
                        };
                        basePriceAndAvailability.ReferenceId = rowKey;
                        costPriceAndAvailability.ReferenceId = rowKey;
                        gatePriceAndAvailability.ReferenceId = rowKey;
                        batchOperation.Insert(hbEntity);
                    }
                }
                var table = GetTableReference(Constant.Availabilties);
                //table.ExecuteBatchAsLimitedBatches(batchOperation, null);
                Task.Run(() => InsertInBathces(table, batchOperation));
            }
        }

        /// <summary>
        /// Retrieve the additional properties from the table storage
        /// </summary>
        /// <param name="referenceId"></param>
        /// <param name="partitionKey"></param>
        /// <returns></returns>
        public override TableEntity Retrieve(string referenceId, string partitionKey)
        {
            var table = GetTableReference(Constant.Availabilties);

            TicketsAvailabilities AvailabilitiesEntityResolver(string pk, string rk, DateTimeOffset ts, IDictionary<string, EntityProperty> props, string etag)
            {
                var resolvedEntity = new TicketsAvailabilities() { PartitionKey = pk, RowKey = rk, Timestamp = ts, ETag = etag };

                foreach (var item in props.Where(p => p.Key.StartsWith(Constant.DecimalPrefix)))
                {
                    var realPropertyName = item.Key.Substring(Constant.DecimalPrefix.Length);
                    var propertyInfo = resolvedEntity.GetType()?.GetProperty(realPropertyName);
                    propertyInfo?.SetValue(resolvedEntity, Convert.ChangeType(item.Value.StringValue, propertyInfo.PropertyType), null);
                }
                resolvedEntity.ReadEntity(props, null);
                return resolvedEntity;
            }

            var retrieveOperation = TableOperation.Retrieve(partitionKey, referenceId, AvailabilitiesEntityResolver);

            var retrievedResult = table.Execute(retrieveOperation);
            return (TableEntity)retrievedResult.Result;
        }
    }

    public static class TicketsStorageOperatorExtentions
    {
        public static IList<TableResult> ExecuteBatchAsLimitedBatches(this CloudTable table,
                                                              TableBatchOperation batch,
                                                              TableRequestOptions requestOptions)
        {
            if (IsBatchCountUnderSupportedOperationsLimit(batch))
            {
                return table.ExecuteBatch(batch, requestOptions);
            }

            var result = new List<TableResult>();
            var limitedBatchOperationLists = GetLimitedBatchOperationLists(batch);
            foreach (var limitedBatchOperationList in limitedBatchOperationLists)
            {
                var limitedBatch = CreateLimitedTableBatchOperation(limitedBatchOperationList);
                var limitedBatchResult = table.ExecuteBatch(limitedBatch, requestOptions);
                result.AddRange(limitedBatchResult);
            }

            return result;
        }

        private static bool IsBatchCountUnderSupportedOperationsLimit(TableBatchOperation batch)
        {
            return batch.Count <= TableConstants.TableServiceBatchMaximumOperations;
        }

        private static IEnumerable<List<TableOperation>> GetLimitedBatchOperationLists(TableBatchOperation batch)
        {
            return batch.ChunkBy(TableConstants.TableServiceBatchMaximumOperations);
        }

        private static TableBatchOperation CreateLimitedTableBatchOperation(IEnumerable<TableOperation> limitedBatchOperationList)
        {
            var limitedBatch = new TableBatchOperation();
            foreach (var limitedBatchOperation in limitedBatchOperationList)
            {
                limitedBatch.Add(limitedBatchOperation);
            }

            return limitedBatch;
        }
    }
}