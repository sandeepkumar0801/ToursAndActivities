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
    public class RedeamV12StorageOperator : BaseStorageOperator
    {
        public override void InsertAvailabilitiesData(Activity activity, string tokenId)
        {
            var referenceIds = new Dictionary<string, string>();
            var productOptions = activity?.ProductOptions?.ToList();
            if (productOptions?.Count > 0)
            {
                var batchOperation = new TableBatchOperation();
                foreach (var productOption in productOptions)
                {
                    var validDates = productOption.BasePrice.DatePriceAndAvailabilty.Keys;
                    foreach (var date in validDates)
                    {
                        if (!(productOption is ActivityOption)) throw new InvalidCastException(Constant.InvalidCastErrorMessage);

                        var option = (ActivityOption)productOption;
                        var validCancellationPolicies = option?.CancellationPrices?.Where(x => x.CancellationDateRelatedToOpreationDate.Date == date.Date)?.ToList();

                        var basePriceAndAvailability = GetPriceAndAvailability(productOption.BasePrice, productOption.TravelInfo, date);
                        var costPriceAndAvailability = GetPriceAndAvailability(productOption.CostPrice, productOption.TravelInfo, date);
                        var gatePriceAndAvailability = GetPriceAndAvailability(productOption.GateBasePrice, productOption.TravelInfo, date);
                        productOption.TravelInfo.StartDate = date;
                        var rowKey = GetRowKey();

                        var splitData = basePriceAndAvailability.ReferenceId.Split('|');
                        var redeamAvailabilityId = string.Empty;
                        var redeamAvailabilityStart = string.Empty;
                        var splitCostPrice = string.Empty;
                        var splitBasePrice = string.Empty;
                        var splitGateBasePrice = string.Empty;
                        var dataPriceId = string.Empty;
                        if (splitData != null && splitData.Length>5)
                        {
                            redeamAvailabilityId = splitData[0];
                            redeamAvailabilityStart = splitData[1];
                            dataPriceId = splitData[2];
                            splitCostPrice = splitData[3];
                            splitBasePrice = splitData[4];
                            splitGateBasePrice = splitData[5];
                        }
                        var dataPriceIdDictionary = dataPriceId.Split(';');
                        var myDict = new Dictionary<string, string>();
                        
                        if (dataPriceIdDictionary != null)
                        {
                            foreach (var dict in dataPriceIdDictionary)
                            {
                                var dictItem = dict.Split('=');
                                if (!myDict.ContainsKey(dictItem[0]))
                                {
                                    myDict.Add(dictItem[0], dictItem[1]);
                                }
                            }
                        }
                        var redeamAvailabilities = new RedeamAvailabilities
                        {
                            #region Properties used for Redeam Product Booking request

                            Cancellable = option.Cancellable,
                            Holdable = option.Holdable,
                            Refundable = option.Refundable,
                            Type = option.Type,
                            HoldablePeriod = option.HoldablePeriod,
                            Time = option.Time,
                            RateId = option.RateId,
                            PriceId = SerializeDeSerializeHelper.Serialize(myDict),
                            SupplierId = option.SupplierId,

                            #endregion Properties used for Redeam Product Booking request

                            SupplierOptionCode = option.SupplierOptionCode,
                            BasePrice =System.Convert.ToDecimal(splitBasePrice),
                            CostPrice = System.Convert.ToDecimal(splitCostPrice),
                            GateBasePrice = System.Convert.ToDecimal(splitGateBasePrice),
                            CurrencyCode = productOption.BasePrice?.Currency?.IsoCode,
                            AvailabilityStatus = option.AvailabilityStatus.ToString(),
                            OptionId = option.Id,
                            ServiceOptionId = productOption.ServiceOptionId,
                            OptionName = option.Name,
                            ActivityId = activity.ID,
                            ApiType = (int)activity.ApiType,
                            Margin = option.Margin == null ? 0 : option.Margin.Value,
                            OnSale = ProductSaleApplied(option.PriceOffers),
                            UnitType = GetUnitType(option),
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
                            ApiCancellationPolicy = option.ApiCancellationPolicy,
                            CancellationText = option.CancellationText,
                            CancellationPrices = SerializeDeSerializeHelper.Serialize(validCancellationPolicies),
                            TimeSlot = productOption?.StartTime.ToString() ?? "",
                            RedeamAvailabilityId= redeamAvailabilityId,
                            RedeamAvailabilityStart= redeamAvailabilityStart,
                          
                        };
                        basePriceAndAvailability.ReferenceId = rowKey;
                        costPriceAndAvailability.ReferenceId = rowKey;
                        gatePriceAndAvailability.ReferenceId = rowKey;
                        batchOperation.Insert(redeamAvailabilities);
                    }
                }
                var table = GetTableReference(Constant.Availabilties);
                Task.Run(() => InsertInBathces(table, batchOperation));
            }
        }

        public override TableEntity Retrieve(string referenceId, string partitionKey)
        {
            var table = GetTableReference(Constant.Availabilties);

            RedeamAvailabilities AvailabilitiesEntityResolver(string pk, string rk, DateTimeOffset ts, IDictionary<string, EntityProperty> props, string etag)
            {
                var resolvedEntity = new RedeamAvailabilities() { PartitionKey = pk, RowKey = rk, Timestamp = ts, ETag = etag };

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