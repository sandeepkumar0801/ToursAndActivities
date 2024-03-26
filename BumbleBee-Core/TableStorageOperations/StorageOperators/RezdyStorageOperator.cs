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
    public class RezdyStorageOperator : BaseStorageOperator
    {
        //public override void InsertAvailabilitiesData(Activity activity, string tokenId)
        //{
        //    var referenceIds = new Dictionary<string, string>();
        //    var batchOperation = new TableBatchOperation();

        //    foreach (var productOption in activity.ProductOptions)
        //    {
        //        var guid = GetRowKey();
        //        if (!(productOption is ActivityOption)) throw new InvalidCastException(Constant.InvalidCastErrorMessage);
        //        var option = (ActivityOption)productOption;
        //        var basePriceAndAvailability = GetPriceAndAvailabilityRezdy(productOption.BasePrice, productOption.TravelInfo);
        //        var costPriceAndAvailability = GetPriceAndAvailabilityRezdy(productOption.CostPrice, productOption.TravelInfo);
        //        var gatePriceAndAvailability = GetPriceAndAvailabilityRezdy(productOption.GateBasePrice, productOption.TravelInfo);

        //        var rezdyEntity = new RezdyAvailabilities
        //        {
        //            Seats = option.Seats,
        //            SeatsAvailable = option.SeatsAvailable,
        //            StartTimeLocal = option.StartLocalTime,
        //            EndTimeLocal = option.EndLocalTime,
        //            PickUpId = option.PickUpId,

        //            BasePrice = productOption.BasePrice.Amount,
        //            CostPrice = productOption.CostPrice == null ? 0 : productOption.CostPrice.Amount,
        //            GateBasePrice = productOption.GateBasePrice.Amount,
        //            CurrencyCode = productOption.BasePrice?.Currency?.IsoCode,
        //            AvailabilityStatus = productOption.AvailabilityStatus.ToString(),
        //            SupplierOptionCode = productOption.SupplierOptionCode,
        //            OptionId = productOption.Id,
        //            ServiceOptionId = productOption.ServiceOptionId,
        //            OptionName = productOption.Name,
        //            ActivityId = activity.ID,
        //            ApiType = (int)activity.ApiType,
        //            Margin = productOption.Margin == null ? 0 : productOption.Margin.Value,
        //            OnSale = ProductSaleApplied(productOption.PriceOffers),
        //            UnitType = GetUnitType(productOption),
        //            PriceOfferReferenceId = InsertPriceOfferData(productOption.PriceOffers, guid),
        //            BasePricingUnits = SerializeDeSerializeHelper.Serialize(basePriceAndAvailability?.PricingUnits),
        //            GateBasePricingUnits = SerializeDeSerializeHelper.Serialize(gatePriceAndAvailability?.PricingUnits),
        //            CostPricingUnits = SerializeDeSerializeHelper.Serialize(costPriceAndAvailability?.PricingUnits),
        //            TravelInfo = SerializeDeSerializeHelper.Serialize(productOption.TravelInfo),

        //            BundleOptionID = productOption.BundleOptionID,
        //            BundleOptionName = productOption.BundleOptionName,
        //            ComponentOrder = productOption.ComponentOrder,
        //            ComponentServiceID = productOption.ComponentServiceID,
        //            IsSameDayBookable = productOption.IsSameDayBookable,
        //            PriceTypeID = productOption.PriceTypeID,

        //            PartitionKey = tokenId,
        //            RowKey = guid
        //        };
        //        basePriceAndAvailability.ReferenceId = guid;
        //        costPriceAndAvailability.ReferenceId = guid;
        //        gatePriceAndAvailability.ReferenceId = guid;
        //        batchOperation.Insert(rezdyEntity);
        //        //var optionId = GetOptionId(activity.ActivityType, productOption);
        //        //referenceIds.Add(optionId, guid);
        //    }
        //    var table = GetTableReference(Constant.Availabilties);
        //    table.ExecuteBatch(batchOperation);

        //    // return referenceIds;
        //}

        public override void InsertAvailabilitiesData(Activity activity, string tokenId)
        {
            var productOptions = activity.ProductOptions = activity?.ProductOptions?.OrderBy(x => x.TravelInfo.StartDate).ToList();
            if (productOptions?.Count > 0)
            {
                var batchOperation = new TableBatchOperation();

                foreach (var productOption in productOptions)
                {
                    var validDates = productOption.BasePrice.DatePriceAndAvailabilty.Keys;
                    foreach (var date in validDates)
                    {
                        if (!(productOption is ActivityOption)) throw new InvalidCastException(Constant.InvalidCastErrorMessage);

                        var rowKey = GetRowKey();
                        var option = (ActivityOption)productOption;

                        var validCancellationPolicies = option?.CancellationPrices?.Where(x => x.CancellationDateRelatedToOpreationDate.Date == date.Date)?.ToList();

                        var basePriceAndAvailability = GetPriceAndAvailability(productOption.BasePrice, productOption.TravelInfo, date);
                        var costPriceAndAvailability = GetPriceAndAvailability(productOption.CostPrice, productOption.TravelInfo, date);
                        var gatePriceAndAvailability = GetPriceAndAvailability(productOption.GateBasePrice, productOption.TravelInfo, date);

                        var rezdyEntity = new RezdyAvailabilities
                        {
                            Seats = option.Seats,
                            SeatsAvailable = option.SeatsAvailable,
                            StartTimeLocal = option.StartLocalTime,
                            EndTimeLocal = option.EndLocalTime,
                            PickUpId = option.PickUpId,

                            BasePrice = productOption.BasePrice.Amount,
                            CostPrice = productOption.CostPrice == null ? 0 : productOption.CostPrice.Amount,
                            GateBasePrice = productOption.GateBasePrice.Amount,
                            CurrencyCode = productOption.BasePrice?.Currency?.IsoCode,
                            AvailabilityStatus = productOption.AvailabilityStatus.ToString(),
                            SupplierOptionCode = productOption.SupplierOptionCode,
                            OptionId = productOption.Id,
                            ServiceOptionId = productOption.ServiceOptionId,
                            OptionName = productOption.Name,
                            ActivityId = activity.ID,
                            ApiType = (int)activity.ApiType,
                            Margin = productOption.Margin == null ? 0 : productOption.Margin.Value,
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
                            TimeSlot = productOption.StartTime.ToString(),
                            PartitionKey = tokenId,
                            RowKey = rowKey,
                            ApiCancellationPolicy = option.ApiCancellationPolicy,
                            CancellationText = option.CancellationText,
                            CancellationPrices = SerializeDeSerializeHelper.Serialize(validCancellationPolicies)
                        };
                        basePriceAndAvailability.ReferenceId = rowKey;
                        costPriceAndAvailability.ReferenceId = rowKey;
                        gatePriceAndAvailability.ReferenceId = rowKey;
                        batchOperation.Insert(rezdyEntity);
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

            RezdyAvailabilities AvailabilitiesEntityResolver(string pk, string rk, DateTimeOffset ts, IDictionary<string, EntityProperty> props, string etag)
            {
                var resolvedEntity = new RezdyAvailabilities() { PartitionKey = pk, RowKey = rk, Timestamp = ts, ETag = etag };

                foreach (var item in props.Where(p => p.Key.StartsWith(Constant.DecimalPrefix)))
                {
                    string realPropertyName = item.Key.Substring(Constant.DecimalPrefix.Length);
                    System.Reflection.PropertyInfo propertyInfo = resolvedEntity.GetType()?.GetProperty(realPropertyName);
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