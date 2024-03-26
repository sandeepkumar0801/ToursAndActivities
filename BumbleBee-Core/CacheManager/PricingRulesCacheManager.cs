using CacheManager.Constants;
using CacheManager.Contract;
using CacheManager.Helper;
using Isango.Entities;
using Isango.Entities.PricingRules;
using Isango.Entities.Wrapper;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using Util;

namespace CacheManager
{
    public class PricingRulesCacheManager : IPricingRulesCacheManager
    {
        private readonly CollectionDataFactory<CacheKey<ProductSaleRuleByActivity>> _collectionDataByActivity;
        private readonly CollectionDataFactory<CacheKey<ProductSaleRuleByOption>> _collectionDataByOption;
        private readonly CollectionDataFactory<CacheKey<ProductSaleRuleByAffiliate>> _collectionDataByAffiliate;
        private readonly CollectionDataFactory<CacheKey<ProductSaleRuleByCountry>> _collectionDataByCountry;
        private readonly CollectionDataFactory<CacheKey<B2BSaleRule>> _collectionDataB2BSale;
        private readonly CollectionDataFactory<CacheKey<B2BNetRateRule>> _collectionDataB2BNet;
        private readonly CollectionDataFactory<CacheKey<SupplierSaleRuleByActivity>> _collectionDataSupplierSaleByActivity;
        private readonly CollectionDataFactory<CacheKey<SupplierSaleRuleByOption>> _collectionDataSupplierSaleByOption;
        private readonly CollectionDataFactory<IsangoConfiguration> _collectionDataIsangoConfiguration;

        #region Constructor

        public PricingRulesCacheManager(CollectionDataFactory<CacheKey<ProductSaleRuleByActivity>> cosmosHelperByActivity, CollectionDataFactory<CacheKey<ProductSaleRuleByOption>> cosmosHelperByOption, CollectionDataFactory<CacheKey<ProductSaleRuleByAffiliate>> cosmosHelperByAffiliate, CollectionDataFactory<CacheKey<ProductSaleRuleByCountry>> cosmosHelperByCountry, CollectionDataFactory<CacheKey<B2BSaleRule>> cosmosHelperB2BSale, CollectionDataFactory<CacheKey<B2BNetRateRule>> cosmosHelperB2BNet, CollectionDataFactory<CacheKey<SupplierSaleRuleByActivity>> cosmosHelperSupplierSaleByActivity, CollectionDataFactory<CacheKey<SupplierSaleRuleByOption>> cosmosHelperSupplierSaleByOption, CollectionDataFactory<IsangoConfiguration> cosmosHelperIsangoConfiguration)
        {
            _collectionDataByActivity = cosmosHelperByActivity;
            _collectionDataByOption = cosmosHelperByOption;
            _collectionDataByAffiliate = cosmosHelperByAffiliate;
            _collectionDataByCountry = cosmosHelperByCountry;
            _collectionDataB2BSale = cosmosHelperB2BSale;
            _collectionDataB2BNet = cosmosHelperB2BNet;
            _collectionDataSupplierSaleByActivity = cosmosHelperSupplierSaleByActivity;
            _collectionDataSupplierSaleByOption = cosmosHelperSupplierSaleByOption;
            _collectionDataIsangoConfiguration = cosmosHelperIsangoConfiguration;
        }

        #endregion Constructor

        #region Product Sale Rules Operations

        public List<ProductSaleRuleByActivity> GetProductSaleRuleByActivity()
        {
            //for mongoDB
            var filter = Builders<CacheKey<ProductSaleRuleByActivity>>.Filter.Eq("_id", Constant.ProductSaleRuleByActivity);
            //end mongo
            var result = _collectionDataByActivity.GetCollectionDataHelper().GetResult(ConfigurationManagerHelper.GetValuefromAppSettings("PricingRulesCollection"), $"{Constant.GetPricingRules}'{Constant.ProductSaleRuleByActivity}'", filter);
            return result?.CacheValue;
        }

        public List<ProductSaleRuleByActivity> GetProductCostSaleRuleByActivity()
        {
            //for mongoDB
            var filter = Builders<CacheKey<ProductSaleRuleByActivity>>.Filter.Eq("_id", Constant.ProductCostSaleRuleByActivity);
            //end mongo
            var result = _collectionDataByActivity.GetCollectionDataHelper().GetResult(ConfigurationManagerHelper.GetValuefromAppSettings("PricingRulesCollection"), $"{Constant.GetPricingRules}'{Constant.ProductCostSaleRuleByActivity}'", filter);
            return result?.CacheValue;
        }

        public DateTime? GetExpirationTime()
        {
            //for mongoDB
            var filter = Builders<IsangoConfiguration>.Filter.Eq("_id", "PREExpirationTime");
            //end mongo
            var result = _collectionDataIsangoConfiguration.GetCollectionDataHelper().GetResult(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.IsangoConfiguration), Constant.PreExpirationTimeQuery, filter);
            return result?.ExpirationTime;
        }

        public bool InsertDocuments(CacheKey<ProductSaleRuleByActivity> rules)
        {
            var result = _collectionDataByActivity.GetCollectionDataHelper().InsertDocument(ConfigurationManagerHelper.GetValuefromAppSettings("PricingRulesCollection"), rules);
            return result.Result;
        }

        public List<ProductSaleRuleByOption> GetProductSaleRuleByOption()
        {
            //for mongoDB
            var filter = Builders<CacheKey<ProductSaleRuleByOption>>.Filter.Eq("_id", Constant.ProductSaleRuleByOption);
            //end mongo
            var result = _collectionDataByOption.GetCollectionDataHelper().GetResult(ConfigurationManagerHelper.GetValuefromAppSettings("PricingRulesCollection"), $"{Constant.GetPricingRules}'{Constant.ProductSaleRuleByOption}'", filter);
            return result?.CacheValue;
        }

        public List<ProductSaleRuleByOption> GetProductCostSaleRuleByOption()
        {
            //for mongoDB
            var filter = Builders<CacheKey<ProductSaleRuleByOption>>.Filter.Eq("_id", Constant.ProductCostSaleRuleByOption);
            //end mongo
            var result = _collectionDataByOption.GetCollectionDataHelper().GetResult(ConfigurationManagerHelper.GetValuefromAppSettings("PricingRulesCollection"), $"{Constant.GetPricingRules}'{Constant.ProductCostSaleRuleByOption}'", filter);
            return result?.CacheValue;
        }

        public bool InsertDocuments(CacheKey<ProductSaleRuleByOption> rules)
        {
            var result = _collectionDataByOption.GetCollectionDataHelper().InsertDocument(ConfigurationManagerHelper.GetValuefromAppSettings("PricingRulesCollection"), rules);
            return result.Result;
        }

        public List<ProductSaleRuleByAffiliate> GetProductSaleRuleByAffiliate()
        {
            //for mongoDB
            var filter = Builders<CacheKey<ProductSaleRuleByAffiliate>>.Filter.Eq("_id", Constant.ProductSaleRuleByAffiliate);
            //end mongo
            var result = _collectionDataByAffiliate.GetCollectionDataHelper().GetResult(ConfigurationManagerHelper.GetValuefromAppSettings("PricingRulesCollection"), $"{Constant.GetPricingRules}'{Constant.ProductSaleRuleByAffiliate}'", filter);
            return result?.CacheValue;
        }

        public List<ProductSaleRuleByAffiliate> GetProductCostSaleRuleByAffiliate()
        {
            //for mongoDB
            var filter = Builders<CacheKey<ProductSaleRuleByAffiliate>>.Filter.Eq("_id", Constant.ProductCostSaleRuleByAffiliate);
            //end mongo
            var result = _collectionDataByAffiliate.GetCollectionDataHelper().GetResult(ConfigurationManagerHelper.GetValuefromAppSettings("PricingRulesCollection"), $"{Constant.GetPricingRules}'{Constant.ProductCostSaleRuleByAffiliate}'", filter);
            return result?.CacheValue;
        }

        public bool InsertDocuments(CacheKey<ProductSaleRuleByAffiliate> rules)
        {
            var result = _collectionDataByAffiliate.GetCollectionDataHelper().InsertDocument(ConfigurationManagerHelper.GetValuefromAppSettings("PricingRulesCollection"), rules);
            return result.Result;
        }

        public List<ProductSaleRuleByCountry> GetProductSaleRuleByCountry()
        {
            //for mongoDB
            var filter = Builders<CacheKey<ProductSaleRuleByCountry>>.Filter.Eq("_id", Constant.ProductSaleRuleByCountry);
            //end mongo
            var result = _collectionDataByCountry.GetCollectionDataHelper().GetResult(ConfigurationManagerHelper.GetValuefromAppSettings("PricingRulesCollection"), $"{Constant.GetPricingRules}'{Constant.ProductSaleRuleByCountry}'", filter);
            return result?.CacheValue;
        }

        public List<ProductSaleRuleByCountry> GetProductCostSaleRuleByCountry()
        {
            //for mongoDB
            var filter = Builders<CacheKey<ProductSaleRuleByCountry>>.Filter.Eq("_id", Constant.ProductCostSaleRuleByCountry);
            //end mongo
            var result = _collectionDataByCountry.GetCollectionDataHelper().GetResult(ConfigurationManagerHelper.GetValuefromAppSettings("PricingRulesCollection"), $"{Constant.GetPricingRules}'{Constant.ProductCostSaleRuleByCountry}'", filter);
            return result?.CacheValue;
        }

        public bool InsertDocuments(CacheKey<ProductSaleRuleByCountry> rules)
        {
            var result = _collectionDataByCountry.GetCollectionDataHelper().InsertDocument(ConfigurationManagerHelper.GetValuefromAppSettings("PricingRulesCollection"), rules);
            return result.Result;
        }

        #endregion Product Sale Rules Operations

        #region B2B Sale Rules Operations

        public List<B2BSaleRule> GetB2BSaleRules()
        {
            //for mongoDB
            var filter = Builders<CacheKey<B2BSaleRule>>.Filter.Eq("_id", Constant.B2BSaleRules);
            //end mongo
            var result = _collectionDataB2BSale.GetCollectionDataHelper().GetResult(ConfigurationManagerHelper.GetValuefromAppSettings("PricingRulesCollection"), $"{Constant.GetPricingRules}'{Constant.B2BSaleRules}'", filter);
            return result?.CacheValue;
        }

        public bool InsertDocuments(CacheKey<B2BSaleRule> rules)
        {
            var result = _collectionDataB2BSale.GetCollectionDataHelper().InsertDocument(ConfigurationManagerHelper.GetValuefromAppSettings("PricingRulesCollection"), rules);
            return result.Result;
        }

        #endregion B2B Sale Rules Operations

        #region B2B Sale Rules Operations

        public List<B2BNetRateRule> GetB2BNetRules()
        {
            //for mongoDB
            var filter = Builders<CacheKey<B2BNetRateRule>>.Filter.Eq("_id", Constant.B2BNetRateRules);
            //end mongo
            var result = _collectionDataB2BNet.GetCollectionDataHelper().GetResult(ConfigurationManagerHelper.GetValuefromAppSettings("PricingRulesCollection"), $"{Constant.GetPricingRules}'{Constant.B2BNetRateRules}'", filter);
            return result?.CacheValue;
        }

        public bool InsertDocuments(CacheKey<B2BNetRateRule> rules)
        {
            var result = _collectionDataB2BNet.GetCollectionDataHelper().InsertDocument(ConfigurationManagerHelper.GetValuefromAppSettings("PricingRulesCollection"), rules);
            return result.Result;
        }

        #endregion B2B Sale Rules Operations

        #region Supplier Sale Rules Operations

        public List<SupplierSaleRuleByActivity> GetSupplierSaleRuleByActivity()
        {
            //for mongoDB
            var filter = Builders<CacheKey<SupplierSaleRuleByActivity>>.Filter.Eq("_id", Constant.SupplierSaleRuleByActivity);
            //end mongo
            var result = _collectionDataSupplierSaleByActivity.GetCollectionDataHelper().GetResult(ConfigurationManagerHelper.GetValuefromAppSettings("PricingRulesCollection"), $"{Constant.GetPricingRules}'{Constant.SupplierSaleRuleByActivity}'", filter);
            return result?.CacheValue;
        }

        public bool InsertDocuments(CacheKey<SupplierSaleRuleByActivity> rules)
        {
            var result = _collectionDataSupplierSaleByActivity.GetCollectionDataHelper().InsertDocument(ConfigurationManagerHelper.GetValuefromAppSettings("PricingRulesCollection"), rules);
            return result.Result;
        }

        public List<SupplierSaleRuleByOption> GetSupplierSaleRuleByOption()
        {
            //for mongoDB
            var filter = Builders<CacheKey<SupplierSaleRuleByOption>>.Filter.Eq("_id", Constant.SupplierSaleRuleByOption);
            //end mongo
            var result = _collectionDataSupplierSaleByOption.GetCollectionDataHelper().GetResult(ConfigurationManagerHelper.GetValuefromAppSettings("PricingRulesCollection"), $"{Constant.GetPricingRules}'{Constant.SupplierSaleRuleByOption}'", filter);
            return result?.CacheValue;
        }

        public bool InsertDocuments(CacheKey<SupplierSaleRuleByOption> rules)
        {
            var result = _collectionDataSupplierSaleByOption.GetCollectionDataHelper().InsertDocument(ConfigurationManagerHelper.GetValuefromAppSettings("PricingRulesCollection"), rules);
            return result.Result;
        }

        public bool InsertExpirationTime(IsangoConfiguration configuration)
        {
            var result = _collectionDataIsangoConfiguration.GetCollectionDataHelper().InsertDocument(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.IsangoConfiguration), configuration);
            return result.Result;
        }

        #endregion Supplier Sale Rules Operations

        #region Delete and Create Collection

        public bool DeleteAndCreateCollection()
        {
            if (_collectionDataByActivity.GetCollectionDataHelper().CheckIfCollectionExist(ConfigurationManagerHelper.GetValuefromAppSettings("PricingRulesCollection")).Result)
            {
                _collectionDataByActivity.GetCollectionDataHelper().DeleteCollection(ConfigurationManagerHelper.GetValuefromAppSettings("PricingRulesCollection")).Wait();
            }

            return _collectionDataByActivity.GetCollectionDataHelper().CreateCollection(ConfigurationManagerHelper.GetValuefromAppSettings("PricingRulesCollection"), Constant.PartitionKey).Result;
        }

        public bool CreateCollectionIfNotExist()
        {
            return _collectionDataIsangoConfiguration.GetCollectionDataHelper().CheckIfCollectionExist(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.IsangoConfiguration)).Result
                   || _collectionDataIsangoConfiguration.GetCollectionDataHelper().CreateCollection(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.IsangoConfiguration), Constant.PartitionKey).Result;
        }

        #endregion Delete and Create Collection
    }
}