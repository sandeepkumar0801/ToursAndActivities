using AsyncBooking.HangFire;
using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Util;

namespace CacheLoader.HangFire
{
    public static class JobScheduler
    {
        public static void JobScheduler_Hangfire()
        {
            var isAsyncBooking = ConfigurationManagerHelper.GetValuefromAppSettings("isAsyncBooking");
            var isCacheLoader = ConfigurationManagerHelper.GetValuefromAppSettings("isCacheLoader");

            if (isCacheLoader == "1")
            {
                AddRecurringJobWithTimeZone<Functions>("CacheLoader_RegionDestinationMapping", functions => functions.RegionDestinationMapping(), "45 5 * * 1");
                AddRecurringJobWithTimeZone<Functions>("CacheLoader_LoadElasticDestination", functions => functions.LoadElasticDestination(), "05 3 * * *");
                AddRecurringJobWithTimeZone<Functions>("CacheLoader_RegionCategoryMapping", functions => functions.RegionCategoryMapping(), "35 5 * * *");
                AddRecurringJobWithTimeZone<Functions>("CacheLoader_LoadRegionCategoryMappingProducts", functions => functions.LoadRegionCategoryMappingProducts(), "40 5 * * 1");
                AddRecurringJobWithTimeZone<Functions>("CacheLoader_InsertOptionAvailability", functions => functions.InsertOptionAvailability(), "0 0 * * *");
                AddRecurringJobWithTimeZone<Functions>("CacheLoader_LoadAvailabilityCache", functions => functions.LoadAvailabilityCache(), "45 2 * * *");
                AddRecurringJobWithTimeZone<Functions>("CacheLoader_LoadCurrencyExchangeRates", functions => functions.LoadCurrencyExchangeRates(), "10 1 * * *");
                AddRecurringJobWithTimeZone<Functions>("CacheLoader_LoadAffiliateFilter", functions => functions.LoadAffiliateFilter(), "15 3 * * *");
                AddRecurringJobWithTimeZone<Functions>("CacheLoader_GetCustomerPrototypeByActivity", functions => functions.GetCustomerPrototypeByActivity(), "09 3 * * *");
                AddRecurringJobWithTimeZone<Functions>("CacheLoader_Synchronizer", functions => functions.Synchronizer(), "0 */5 * * *");
                AddRecurringJobWithTimeZone<Functions>("CacheLoader_LoadAffiliateDataByDomain", functions => functions.LoadAffiliateDataByDomain(), "0 3 * * *");
                AddRecurringJobWithTimeZone<Functions>("CacheLoader_PricingRules", functions => functions.PricingRules(), "30 8 * * *");
                AddRecurringJobWithTimeZone<Functions>("CacheLoader_LoadCalendarAvailability", functions => functions.LoadCalendarAvailability(), "0 1 * * *");
                AddRecurringJobWithTimeZone<Functions>("CacheLoader_LoadPickupLocationsData", functions => functions.LoadPickupLocationsData(), "30 5 * * 1");
                AddRecurringJobWithTimeZone<Functions>("CacheLoader_LoadMappedLanguageData", functions => functions.LoadMappedLanguageData(), "0 4 * * *");
                AddRecurringJobWithTimeZone<Functions>("CacheLoader_LoadFareHarborCustomerPrototypeData", functions => functions.LoadFareHarborCustomerPrototypeData(), "15 3 * * *");
                AddRecurringJobWithTimeZone<Functions>("CacheLoader_LoadFareHarborUserKeysData", functions => functions.LoadFareHarborUserKeysData(), "05 3 * * *");
                AddRecurringJobWithTimeZone<Functions>("CacheLoader_LoadElasticProducts", functions => functions.LoadElasticProducts(), "10 3 * * *");
                AddRecurringJobWithTimeZone<Functions>("CacheLoader_LoadElasticAttractions", functions => functions.LoadElasticAttractions(), "15 3 * * *");
                AddRecurringJobWithTimeZone<Functions>("CacheLoader_LoadElasticAffiliate", functions => functions.LoadElasticAffiliate(), "20 3 * * *");
                AddRecurringJobWithTimeZone<Functions>("CacheLoader_LoadElasticAffiliate", functions => functions.LoadElasticAffiliate(), "25 3 * * *");
                //BackgroundJob.Enqueue<Functions>(x => x.InitaliseMongoDb());
                //RecurringJob.AddOrUpdate<Functions>("InitaliseMongoDb", functions => functions.InitaliseMongoDb(), "30 3 * * *");
            }
            else
            {
                var jobNamesToRemove = new List<string>
                   {
                       "CacheLoader_RegionDestinationMapping",
                       "CacheLoader_LoadElasticDestination",
                       "CacheLoader_RegionCategoryMapping",
                       "CacheLoader_LoadRegionCategoryMappingProducts",
                       "CacheLoader_InsertOptionAvailability",
                       "CacheLoader_LoadAvailabilityCache",
                       "CacheLoader_LoadCurrencyExchangeRates",
                       "CacheLoader_LoadAffiliateFilter",
                       "CacheLoader_GetCustomerPrototypeByActivity",
                       "CacheLoader_Synchronizer",
                       "CacheLoader_LoadAffiliateDataByDomain",
                       "CacheLoader_PricingRules",
                       "CacheLoader_LoadCalendarAvailability",
                       "CacheLoader_LoadPickupLocationsData",
                       "CacheLoader_LoadMappedLanguageData",
                       "CacheLoader_LoadFareHarborCustomerPrototypeData",
                       "CacheLoader_LoadFareHarborUserKeysData",
                       "CacheLoader_LoadElasticProducts",
                       "CacheLoader_LoadElasticAttractions",
                       "CacheLoader_LoadElasticAffiliate",
                       //"InitaliseMongoDb"
                   };

                foreach (var jobName in jobNamesToRemove)
                {
                    RecurringJob.RemoveIfExists(jobName);
                }
            }
            if (isAsyncBooking == "1")
            {
                AddRecurringJobWithTimeZone<AyncFunctions>("AsyncBookingProcessIncompleteBooking", functions => functions.ProcessIncompleteBooking(), "*/15 * * * *");
                AddRecurringJobWithTimeZone<AyncFunctions>("AsyncBookingProcessCssIncompleteBooking", functions => functions.ProcessCssIncompleteBooking(), "*/15 * * * *");
                AddRecurringJobWithTimeZone<AyncFunctions>("AsyncBookingProcessIncompleteCancellation", functions => functions.ProcessIncompleteCancellation(), "*/20 * * * *");
                AddRecurringJobWithTimeZone<AyncFunctions>("AsyncBookingProcessIncompleteRedemption", functions => functions.ProcessIncompleteRedemption(), "*/25 * * * *");

            }
            else
            {
                RecurringJob.RemoveIfExists("AsyncBookingProcessIncompleteBooking");
                RecurringJob.RemoveIfExists("AsyncBookingProcessCssIncompleteBooking");
                RecurringJob.RemoveIfExists("AsyncBookingProcessIncompleteCancellation");
                RecurringJob.RemoveIfExists("AsyncBookingProcessIncompleteRedemption");


            }
        }

        public static void AddRecurringJobWithTimeZone<T>(string jobId, Expression<Action<T>> methodCall, string cronExpression, string timeZoneId = "India Standard Time")
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);

            RecurringJob.AddOrUpdate<T>(jobId, methodCall, cronExpression, new RecurringJobOptions
            {
                TimeZone = timeZone
            });
        }
    }
}
