using Isango.Entities.Activities;
using Isango.Entities.MoulinRouge;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using Util;
using TableStorageOperations.Models.AdditionalPropertiesModels.Availabilities;
using Constant = TableStorageOperations.Constants.Constant;
using System.Threading.Tasks;

namespace TableStorageOperations.StorageOperators
{
    public class MoulinRougeStorageOperator : BaseStorageOperator
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
                        var basePriceAndAvailability = (MoulinRougePriceAndAvailability)GetPriceAndAvailability(productOption.BasePrice, productOption.TravelInfo, date);
                        var costPriceAndAvailability = GetPriceAndAvailability(productOption.CostPrice, productOption.TravelInfo, date);
                        var gatePriceAndAvailability = GetPriceAndAvailability(productOption.GateBasePrice, productOption.TravelInfo, date);
                        productOption.TravelInfo.StartDate = date;
                        var rowKey = GetRowKey();
                        var mrEntity = new MoulinRougeAvailabilities
                        {
                            BlocId = basePriceAndAvailability?.MoulinRouge?.BlocId,
                            CatalogDateId = basePriceAndAvailability?.MoulinRouge?.CatalogDateId,
                            CategoryId = basePriceAndAvailability?.MoulinRouge?.CategoryId,
                            ContingentId = basePriceAndAvailability?.MoulinRouge?.ContingentId,
                            FloorId = basePriceAndAvailability?.MoulinRouge?.FloorId,
                            RateId = basePriceAndAvailability?.MoulinRouge?.RateId,
                            BasePrice = basePriceAndAvailability.TotalPrice,
                            CostPrice = costPriceAndAvailability.TotalPrice,
                            GateBasePrice = costPriceAndAvailability.TotalPrice,
                            CurrencyCode = productOption.BasePrice?.Currency?.IsoCode,
                            AvailabilityStatus = basePriceAndAvailability?.AvailabilityStatus.ToString(),
                            SupplierOptionCode = productOption.SupplierOptionCode,
                            OptionId = productOption.Id,
                            ServiceOptionId = productOption.ServiceOptionId,
                            OptionName = productOption.Name,
                            ActivityId = activity.ID,
                            ApiType = (int)activity.ApiType,
                            Margin = productOption.Margin?.Value ?? 0,
                            OnSale = ProductSaleApplied(productOption.PriceOffers),
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
                            PartitionKey = tokenId,
                            RowKey = rowKey,
                            TimeSlot = productOption?.StartTime.ToString() ?? ""
                        };
                        basePriceAndAvailability.ReferenceId = rowKey;
                        costPriceAndAvailability.ReferenceId = rowKey;
                        gatePriceAndAvailability.ReferenceId = rowKey;
                        batchOperation.Insert(mrEntity);
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

            MoulinRougeAvailabilities AvailabilitiesEntityResolver(string pk, string rk, DateTimeOffset ts, IDictionary<string, EntityProperty> props, string etag)
            {
                var resolvedEntity = new MoulinRougeAvailabilities() { PartitionKey = pk, RowKey = rk, Timestamp = ts, ETag = etag };

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