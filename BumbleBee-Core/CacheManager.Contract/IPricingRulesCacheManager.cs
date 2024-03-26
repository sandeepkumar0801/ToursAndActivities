using Isango.Entities;
using Isango.Entities.PricingRules;
using Isango.Entities.Wrapper;
using System;
using System.Collections.Generic;

namespace CacheManager.Contract
{
    public interface IPricingRulesCacheManager
    {
        List<ProductSaleRuleByActivity> GetProductSaleRuleByActivity();

        List<ProductSaleRuleByOption> GetProductSaleRuleByOption();

        List<ProductSaleRuleByAffiliate> GetProductSaleRuleByAffiliate();

        List<ProductSaleRuleByCountry> GetProductSaleRuleByCountry();

        List<B2BSaleRule> GetB2BSaleRules();

        List<B2BNetRateRule> GetB2BNetRules();

        List<SupplierSaleRuleByActivity> GetSupplierSaleRuleByActivity();

        List<SupplierSaleRuleByOption> GetSupplierSaleRuleByOption();

        DateTime? GetExpirationTime();

        bool InsertDocuments(CacheKey<ProductSaleRuleByActivity> rules);

        bool InsertDocuments(CacheKey<ProductSaleRuleByOption> rules);

        bool InsertDocuments(CacheKey<ProductSaleRuleByAffiliate> rules);

        bool InsertDocuments(CacheKey<ProductSaleRuleByCountry> rules);

        bool InsertDocuments(CacheKey<B2BSaleRule> rules);

        bool InsertDocuments(CacheKey<B2BNetRateRule> rules);

        bool InsertDocuments(CacheKey<SupplierSaleRuleByActivity> rules);

        bool InsertDocuments(CacheKey<SupplierSaleRuleByOption> rules);

        bool InsertExpirationTime(IsangoConfiguration configuration);

        bool DeleteAndCreateCollection();

        bool CreateCollectionIfNotExist();

        List<ProductSaleRuleByActivity> GetProductCostSaleRuleByActivity();

        List<ProductSaleRuleByOption> GetProductCostSaleRuleByOption();

        List<ProductSaleRuleByAffiliate> GetProductCostSaleRuleByAffiliate();

        List<ProductSaleRuleByCountry> GetProductCostSaleRuleByCountry();
    }
}