using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Enums;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Table.Protocol;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Util;
using Constant = TableStorageOperations.Constants.Constant;

namespace TableStorageOperations.StorageOperators
{
    public abstract class BaseStorageOperator
    {
        /// <summary>
        /// Insert the additional properties on availabilites call in the table storage
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        public abstract void InsertAvailabilitiesData(Activity activity, string tokenId);

        /// <summary>
        /// Retrieve the additional properties from the table storage
        /// </summary>
        /// <param name="referenceId"></param>
        /// <param name="partitionKey"></param>
        /// <returns></returns>
        public abstract TableEntity Retrieve(string referenceId, string partitionKey);

        /// <summary>
        /// Get the Azure Table Storage object by the table name
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        protected CloudTable GetTableReference(string tableName)
        {
            var connectionString = ConfigurationManagerHelper.GetValuefromConfig(Constant.StorageConnectionString);
            var cloudStorageAccount = CloudStorageAccount.Parse(connectionString);
            var client = cloudStorageAccount.CreateCloudTableClient();
            var table = client.GetTableReference(tableName);
            table.CreateIfNotExists();
            return table;
        }

        /// <summary>
        /// Get the GUID for the RowKey column
        /// </summary>
        /// <returns></returns>
        protected static string GetRowKey()
        {
            return Guid.NewGuid().ToString().ToLower(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Determines if the ProductSale is applied or not
        /// </summary>
        /// <param name="priceOffers"></param>
        /// <returns></returns>
        protected bool ProductSaleApplied(List<PriceOffer> priceOffers)
        {
            if (priceOffers == null) return false;

            var appliedModules = priceOffers.Select(x => x.ModuleName);
            return appliedModules.Contains(Constant.ProductDiscountModule);
        }

        /// <summary>
        /// Get Option Id on the basis of bundle product and normal product
        /// </summary>
        /// <param name="activityType"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        protected string GetOptionId(ActivityType activityType, ProductOption option)
        {
            return activityType == ActivityType.Bundle ? $"{Convert.ToString(option.Id)}|{Convert.ToString(option.BundleOptionID)}"
                                        : Convert.ToString(option.Id);
        }

        /// <summary>
        /// Get unit type of the given product option
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        protected string GetUnitType(ProductOption option)
        {
            var unitType = option.BasePrice?.DatePriceAndAvailabilty?.FirstOrDefault(x => x.Key.Date == option.TravelInfo.StartDate).Value?.PricingUnits.FirstOrDefault()?.UnitType;
            return unitType == null ? null : EnumNameResolver.GetNameFromEnum(typeof(UnitType), unitType);
        }

        /// <summary>
        /// Inserts the applied rules details for the given product option
        /// </summary>
        /// <param name="priceOffers"></param>
        /// <param name="availabilityReferenceId"></param>
        /// <returns></returns>
        protected string InsertPriceOfferData(List<PriceOffer> priceOffers, string availabilityReferenceId)
        {
            if (priceOffers == null) return null;

            var referenceIds = new List<string>();
            var batchOperation = new TableBatchOperation();
            foreach (var offer in priceOffers)
            {
                var referenceId = GetRowKey();
                var priceOffer = new Models.AdditionalPropertiesModels.AppliedPriceOffers.PriceOffer
                {
                    AppliedId = offer.Id,
                    ModuleName = offer.ModuleName,
                    RuleName = offer.RuleName,
                    OfferPercent = offer.OfferPercent,
                    SaleAmount = offer.SaleAmount,
                    CostAmount = offer.CostAmount,
                    PartitionKey = availabilityReferenceId,
                    RowKey = referenceId
                };
                batchOperation.Insert(priceOffer);
                referenceIds.Add(referenceId);
            }

            var table = GetTableReference(Constant.AppliedPriceOffersTable);
            Task.Run(() => InsertInBathces(table, batchOperation));

            return string.Join("|", referenceIds);
        }

        /// <summary>
        /// Get the PriceAndAvailability of the given price
        /// </summary>
        /// <param name="price"></param>
        /// <param name="travelInfo"></param>
        /// <param name="validDate"></param>
        /// <returns></returns>
        protected PriceAndAvailability GetPriceAndAvailability(Price price, TravelInfo travelInfo, DateTime? validDate = null)
        {
            //return price?.DatePriceAndAvailabilty.Where(x => x.Key == travelInfo.StartDate).Select(x => x.Value).FirstOrDefault();
            return validDate != null ? price?.DatePriceAndAvailabilty[validDate.Value] ?? price?.DatePriceAndAvailabilty[travelInfo.StartDate] : price?.DatePriceAndAvailabilty[travelInfo.StartDate];
        }

        /// <summary>
		/// Get the PriceAndAvailability of the given price
		/// </summary>
		/// <param name="price"></param>
		/// <param name="travelInfo"></param>
		/// <returns></returns>
		public PriceAndAvailability GetPriceAndAvailabilityRezdy(Price price, TravelInfo travelInfo)
        {
            return price?.DatePriceAndAvailabilty.Select(x => x.Value).FirstOrDefault();
        }

        /// <summary>
        /// Insert in batches keeping insert limit in check
        /// </summary>
        /// <param name="table"></param>
        /// <param name="batchOperation"></param>
        public void InsertInBathces(CloudTable table, TableBatchOperation batchOperation)
        {
            try
            {
                if (batchOperation.Count == 0)
                    return;
                if (batchOperation?.Count <= TableConstants.TableServiceBatchMaximumOperations)
                {
                    table.ExecuteBatch(batchOperation);
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
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BaseStorageOperator",
                    MethodName = "InsertInBathces",
                    Params = $"{SerializeDeSerializeHelper.Serialize(batchOperation)}"
                };
                //_log.Error(isangoErrorEntity, ex);
            }
        }
    }
}