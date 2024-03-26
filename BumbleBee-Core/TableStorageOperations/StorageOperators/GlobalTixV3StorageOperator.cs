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
    public class GlobalTixV3StorageOperator : BaseStorageOperator
    {
        /// <summary>
        /// Insert the additional properties on availabilities call in the table storage
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        public override void InsertAvailabilitiesData(Activity activity, string tokenId)
        {
            var referenceIds = new Dictionary<string, string>();
            var batchOperation = new TableBatchOperation();

            foreach (var productOption in activity.ProductOptions)
            {
                var validDates = productOption.BasePrice.DatePriceAndAvailabilty.Keys;
                foreach (var date in validDates)
                {
                    var option = productOption as ActivityOption;
                    if (option == null)
                    {
                        continue;
                    }
                    var validCancellationPolicies = option?.CancellationPrices?.Where(x => x.CancellationDateRelatedToOpreationDate.Date == date.Date)?.ToList();

                    var basePriceAndAvailability = GetPriceAndAvailability(productOption.BasePrice, productOption.TravelInfo, date);
                    var costPriceAndAvailability = GetPriceAndAvailability(productOption.CostPrice, productOption.TravelInfo, date);
                    var gatePriceAndAvailability = GetPriceAndAvailability(productOption.GateBasePrice, productOption.TravelInfo, date);
                    productOption.TravelInfo.StartDate = date;
                    var rowKey = GetRowKey();
                    var gtEntity = new GlobalTixV3Availabilities
                    {
                        SupplierOptionCode = productOption.SupplierOptionCode,
                        BasePrice = productOption.BasePrice.Amount,
                        CostPrice = productOption.CostPrice.Amount,
                        GateBasePrice = productOption.GateBasePrice.Amount,
                        CurrencyCode = productOption.BasePrice?.Currency?.IsoCode,
                        AvailabilityStatus = productOption.AvailabilityStatus.ToString(),
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

                        PartitionKey = tokenId,
                        RowKey = rowKey,

                        //----- GlobalTix specific elements
                        TourDepartureId = basePriceAndAvailability.TourDepartureId,
                        RateKey = option.RateKey,
                        TicketTypeIds = option.TicketTypeIds,
                        ContractQuestions = SerializeDeSerializeHelper.Serialize(option.ContractQuestions),
                        ContractQuestionsForGlobalTix3= SerializeDeSerializeHelper.Serialize(option.ContractQuestionForGlobalTix3),
                        ApiCancellationPolicy = option.ApiCancellationPolicy,
                        CancellationText = option.CancellationText,
                        CancellationPrices = SerializeDeSerializeHelper.Serialize(validCancellationPolicies),
                        TimeSlot = option?.StartTime.ToString() ?? "",
                       
                    };
                    basePriceAndAvailability.ReferenceId = rowKey;
                    costPriceAndAvailability.ReferenceId = rowKey;
                    gatePriceAndAvailability.ReferenceId = rowKey;
                    batchOperation.Insert(gtEntity);
                }
            }
            var table = GetTableReference(Constant.Availabilties);
            Task.Run(() => InsertInBathces(table, batchOperation));
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

            GlobalTixV3Availabilities AvailabilitiesEntityResolver(string pk, string rk, DateTimeOffset ts, IDictionary<string, EntityProperty> props, string etag)
            {
                var resolvedEntity = new GlobalTixV3Availabilities() { PartitionKey = pk, RowKey = rk, Timestamp = ts, ETag = etag };

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