using Isango.Entities.Activities;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TableStorageOperations.Models.AdditionalPropertiesModels.Availabilities;
using Util;
using Constant = TableStorageOperations.Constants.Constant;

namespace TableStorageOperations.StorageOperators
{
    public class AotStorageOperator : BaseStorageOperator
    {
        /// <summary>
        /// Insert the additional properties on availabilites call in the table storage
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
                    var validDates = productOption.BasePrice.DatePriceAndAvailabilty.Keys;

                    foreach (var date in validDates)
                    {
                        var validCancellationPolicies = productOption?.CancellationPrices?.Where(x => x?.CancellationDateRelatedToOpreationDate.Date == date.Date)?.ToList();

                        if (!(productOption is ActivityOption)) throw new InvalidCastException(Constant.InvalidCastErrorMessage);
                        var option = (ActivityOption)productOption;
                        var basePriceAndAvailability = GetPriceAndAvailability(productOption.BasePrice, productOption.TravelInfo, date);
                        var costPriceAndAvailability = GetPriceAndAvailability(productOption.CostPrice, productOption.TravelInfo, date);
                        var gatePriceAndAvailability = GetPriceAndAvailability(productOption.GateBasePrice, productOption.TravelInfo, date);
                        productOption.TravelInfo.StartDate = date;
                        var rowKey = GetRowKey();
                        var aotEntity = new AotAvailabilities
                        {
                            OptionType = option.OptionType,
                            RoomType = option.RoomType,
                            ServiceType = option.ServiceType,
                            SupplierOptionCode = option.SupplierOptionCode,
                            BasePrice = basePriceAndAvailability.TotalPrice,
                            CostPrice = costPriceAndAvailability.TotalPrice,
                            GateBasePrice = gatePriceAndAvailability.TotalPrice,
                            CurrencyCode = productOption.BasePrice?.Currency?.IsoCode,
                            AvailabilityStatus = basePriceAndAvailability?.AvailabilityStatus.ToString(),
                            OptionId = option.Id,
                            ServiceOptionId = productOption.ServiceOptionId,
                            OptionName = option.Name,
                            ActivityId = activity.ID,
                            ApiType = (int)activity.ApiType,
                            Margin = option.Margin?.Value ?? 0,
                            OnSale = ProductSaleApplied(option.PriceOffers),
                            UnitType = GetUnitType(option),
                            PriceOfferReferenceId = InsertPriceOfferData(productOption.PriceOffers, rowKey),
                            BasePricingUnits = SerializeDeSerializeHelper.Serialize(basePriceAndAvailability?.PricingUnits),
                            GateBasePricingUnits = SerializeDeSerializeHelper.Serialize(gatePriceAndAvailability?.PricingUnits),
                            CostPricingUnits = SerializeDeSerializeHelper.Serialize(costPriceAndAvailability?.PricingUnits),
                            TravelInfo = SerializeDeSerializeHelper.Serialize(productOption.TravelInfo),
                            SupplierCancellationPolicy = SerializeDeSerializeHelper.Serialize(productOption.CancellationPrices),
                            BundleOptionID = productOption.BundleOptionID,
                            BundleOptionName = productOption.BundleOptionName,
                            ComponentOrder = productOption.ComponentOrder,
                            ComponentServiceID = productOption.ComponentServiceID,
                            IsSameDayBookable = productOption.IsSameDayBookable,
                            PriceTypeID = productOption.PriceTypeID,
                            PartitionKey = tokenId,
                            RowKey = rowKey,
                            ApiCancellationPolicy = option.ApiCancellationPolicy,
                            CancellationText = option.CancellationText,
                            CancellationPrices = SerializeDeSerializeHelper.Serialize(validCancellationPolicies),
                            TimeSlot = option?.StartTime.ToString() ?? ""
                        };
                        basePriceAndAvailability.ReferenceId = rowKey;
                        costPriceAndAvailability.ReferenceId = rowKey;
                        gatePriceAndAvailability.ReferenceId = rowKey;
                        batchOperation.Insert(aotEntity);
                    }
                }
                var table = GetTableReference(Constant.Availabilties);
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

            AotAvailabilities AvailabilitiesEntityResolver(string pk, string rk, DateTimeOffset ts, IDictionary<string, EntityProperty> props, string etag)
            {
                var resolvedEntity = new AotAvailabilities() { PartitionKey = pk, RowKey = rk, Timestamp = ts, ETag = etag };

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
}