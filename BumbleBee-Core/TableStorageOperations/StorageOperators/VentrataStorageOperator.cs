using Microsoft.WindowsAzure.Storage.Table;
using System;
using Isango.Entities.Activities;
using TableStorageOperations.Models.AdditionalPropertiesModels.Availabilities;
using Constant = TableStorageOperations.Constants.Constant;
using System.Collections.Generic;
using Isango.Entities.Ventrata;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util;

namespace TableStorageOperations.StorageOperators
{
    public class VentrataStorageOperator : BaseStorageOperator
    {
        /// <summary>
        /// Insert the additional properties on availabilites call in the table storage
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
                    ActivityOption actOpt = productOption as ActivityOption;
                    if (actOpt == null)
                    {
                        continue;
                    }

                    //Select opening hours of the date in loop
                    var openingHoursForThisDate = actOpt.OpeningHoursDetails?.ToList().FindAll(thisOpenHr => Convert.ToDateTime(thisOpenHr.Date).ToString(Constant.DateFormat).ToLowerInvariant()
                                                        .Equals(date.ToString(Constant.DateFormat).ToLowerInvariant()));

                    var basePriceAndAvailability = GetPriceAndAvailability(productOption.BasePrice, productOption.TravelInfo, date);
                    var costPriceAndAvailability = GetPriceAndAvailability(productOption.CostPrice, productOption.TravelInfo, date);
                    var gatePriceAndAvailability = GetPriceAndAvailability(productOption.GateBasePrice, productOption.TravelInfo, date);
                    productOption.TravelInfo.StartDate = date;
                    var rowKey = GetRowKey();
                    var gtEntity = new VentrataAvailabilities
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
                        TimeSlot = actOpt.StartTime.ToString(),
                        EndTimeSlot = actOpt.EndTime.ToString(),
                        LanguageCode = activity.LanguageCode,

                        //Ventrata specific properties
                        AvailabilityId = ((VentrataPriceAndAvailability)basePriceAndAvailability).AvailabilityId,
                        VentrataProductId = actOpt.VentrataProductId,
                        OfferCode = actOpt.OfferCode,
                        OfferTitle = actOpt.OfferTitle,
                        PickupPointsDetailsForVentrata = SerializeDeSerializeHelper.Serialize(actOpt.PickupPointsDetailsForVentrata),
                        MeetingPointDetails = SerializeDeSerializeHelper.Serialize(actOpt.MeetingPointDetails),
                        OpeningHoursDetails = SerializeDeSerializeHelper.Serialize(openingHoursForThisDate),

                        PartitionKey = tokenId,
                        RowKey = rowKey,
                        VentrataSupplierId= actOpt.VentrataSupplierId,
                        VentrataBaseURL= actOpt.VentrataBaseURL
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

            VentrataAvailabilities AvailabilitiesEntityResolver(string pk, string rk, DateTimeOffset ts, IDictionary<string, EntityProperty> props, string etag)
            {
                var resolvedEntity = new VentrataAvailabilities() { PartitionKey = pk, RowKey = rk, Timestamp = ts, ETag = etag };

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
