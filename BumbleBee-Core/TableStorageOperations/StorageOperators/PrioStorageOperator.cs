using Isango.Entities.Activities;
using Isango.Entities.Prio;
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
    public class PrioStorageOperator : BaseStorageOperator
    {
        /// <summary>
        /// Insert the additional properties on availabilites call in the table storage
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        public override void InsertAvailabilitiesData(Activity activity, string tokenId)
        {
            var batchOperation = new TableBatchOperation();

            try
            {
                var productOptions = activity?.ProductOptions?.ToList();
                if (productOptions?.Count > 0)
                {
                    foreach (var productOption in productOptions)
                    {
                        var validDates = productOption.BasePrice.DatePriceAndAvailabilty.Keys;
                        foreach (var date in validDates)
                        {
                            if (!(productOption is ActivityOption)) throw new InvalidCastException(Constant.InvalidCastErrorMessage);
                            var option = (ActivityOption)productOption;
                            var basePriceAndAvailability = (PrioPriceAndAvailability)GetPriceAndAvailability(productOption.BasePrice, productOption.TravelInfo, date);
                            var costPriceAndAvailability = GetPriceAndAvailability(productOption.CostPrice, productOption.TravelInfo, date);
                            var gatePriceAndAvailability = GetPriceAndAvailability(productOption.GateBasePrice, productOption.TravelInfo, date);
                            productOption.TravelInfo.StartDate = date;
                            var rowKey = GetRowKey();
                            var prioEntity = new PrioAvailabilities
                            {
                                PrioTicketClass = option.PrioTicketClass,
                                PickupPoints = option.PickupPoints,
                                FromDate = basePriceAndAvailability?.FromDateTime,
                                ToDate = basePriceAndAvailability?.ToDateTime,
                                Vacancies = basePriceAndAvailability?.Vacancies,
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
                                BundleOptionID = productOption.BundleOptionID,
                                BundleOptionName = productOption.BundleOptionName,
                                ComponentOrder = productOption.ComponentOrder,
                                ComponentServiceID = productOption.ComponentServiceID,
                                IsSameDayBookable = productOption.IsSameDayBookable,
                                PriceTypeID = productOption.PriceTypeID,
                                BasePricingUnits = SerializeDeSerializeHelper.Serialize(basePriceAndAvailability?.PricingUnits),
                                GateBasePricingUnits = SerializeDeSerializeHelper.Serialize(gatePriceAndAvailability?.PricingUnits),
                                CostPricingUnits = SerializeDeSerializeHelper.Serialize(costPriceAndAvailability?.PricingUnits),
                                TravelInfo = SerializeDeSerializeHelper.Serialize(productOption.TravelInfo),
                                PartitionKey = tokenId,
                                RowKey = rowKey,
                                TimeSlot = productOption?.StartTime.ToString() ?? ""
                            };
                            basePriceAndAvailability.ReferenceId = rowKey;
                            costPriceAndAvailability.ReferenceId = rowKey;
                            gatePriceAndAvailability.ReferenceId = rowKey;
                            batchOperation.Insert(prioEntity);
                        }
                    }
                    var table = GetTableReference(Constant.Availabilties);
                    Task.Run(() => InsertInBathces(table, batchOperation));
                }
            }
            catch (Exception ex)
            {
                //Task.Run(() =>
                //            _logger.Error(new Isango.Entities.IsangoErrorEntity
                //            {
                //                ClassName = "ActivityService",
                //                MethodName = "FilterProductOptionsBasedOnCloseOut",
                //                Params = SerializeDeSerializeHelper.Serialize(activity),
                //                Token = tokenId
                //            }, ex)
                //        );
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

            PrioAvailabilities AvailabilitiesEntityResolver(string pk, string rk, DateTimeOffset ts, IDictionary<string, EntityProperty> props, string etag)
            {
                var resolvedEntity = new PrioAvailabilities() { PartitionKey = pk, RowKey = rk, Timestamp = ts, ETag = etag };

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