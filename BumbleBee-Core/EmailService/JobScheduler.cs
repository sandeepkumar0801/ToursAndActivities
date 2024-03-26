using DataDumpingQueue.Hangfire;
using Hangfire;
using System.Linq.Expressions;
using Util;


namespace EmailSuitConsole
{
    public static class JobScheduler
    {
        public static void JobScheduler_Hangfire()
        {
            AddRecurringJobWithTimeZone<JobSchedulingService>("TiqetLongTermStatusEmail", functions => functions.SendtiqetStatusEmail(), "05 9 * * *");
            AddRecurringJobWithTimeZone<JobSchedulingService>("PrioLongTermStatusEmail", functions => functions.SendPrioStatusEmail(), "05 9 * * *");
            AddRecurringJobWithTimeZone<JobSchedulingService>("TiqetStatusEmail", functions => functions.SendTiqetChange(), "05 9 * * *");
            AddRecurringJobWithTimeZone<JobSchedulingService>("TiqetVariantEmail", functions => functions.SendVariantChange(), "05 9 * * *");


            AddRecurringJobWithTimeZone<JobSchedulingService>("TourCMSStatusEmail", functions => functions.SendTourCMSChange(), "05 9 * * *");
            AddRecurringJobWithTimeZone<JobSchedulingService>("RaynaStatusEmail", functions => functions.SendRaynaChange(), "05 9 * * *");
            AddRecurringJobWithTimeZone<JobSchedulingService>("RaynaOptionsChange", functions => functions.SendRaynaOptionsChange(), "05 9 * * *");
            AddRecurringJobWithTimeZone<JobSchedulingService>("GlobalTixV3StatusEmail", functions => functions.SendGlobalTixV3Change(), "05 9 * * *");
            AddRecurringJobWithTimeZone<JobSchedulingService>("GlobalTixV3OptionsChange", functions => functions.SendGlobalTixV3OptionsChange(), "05 9 * * *");
            //AddRecurringJobWithTimeZone<JobSchedulingService>("TourCMSPaxEmail", functions => functions.SendTourCMSPaxChange(), "05 9 * * *");

            AddRecurringJobWithTimeZone<JobSchedulingService>("FeefoDownloaderReviews", functions => functions.FeefoDownloaderReviews(), "35 3 * * *");
            //RecurringJob.AddOrUpdate<JobSchedulingService>("PreDepartureMail", functions => functions.PreDepartureMailSave(), "35 3 * * *");


            var isDataDumpingQueue = ConfigurationManagerHelper.GetValuefromAppSettings("isDataDumpingQueue");
            if (isDataDumpingQueue == "1")
            {
                AddRecurringJobWithTimeZone<DataDumpingQueue_Function>("AgeGroup_AOT", functions => functions.LoadAgeGroupForFareHarbor(), "30 3 * * *");
                AddRecurringJobWithTimeZone<DataDumpingQueue_Function>("AgeGroup_FareHarbor", functions => functions.LoadAgeGroupForFareHarbor(), "30 3 * * *");
                AddRecurringJobWithTimeZone<DataDumpingQueue_Function>("AgeGroup_GrayLineIceLand", functions => functions.LoadAgeGroupForGrayLineIceLand(), "30 3 * * *");
                AddRecurringJobWithTimeZone<DataDumpingQueue_Function>("AgeGroup_Prio", functions => functions.LoadAgeGroupForPrio(), "30 3 * * *");
                AddRecurringJobWithTimeZone<DataDumpingQueue_Function>("AgeGroup_HBAuthorization", functions => functions.LoadHBAuthorizationData(), "30 3 * * *");
                AddRecurringJobWithTimeZone<DataDumpingQueue_Function>("AgeGroup_RedeamV12", functions => functions.LoadRedeamV12Data(), "30 3 * * *");

                AddRecurringJobWithTimeZone<DataDumpingQueue_Function>("AgeGroup_Tiqets", functions => functions.LoadTiqetsVariants(), "30 3 * * *");
                AddRecurringJobWithTimeZone<DataDumpingQueue_Function>("AgeGroup_GoldenTours", functions => functions.LoadAgeGroupForGoldenTours(), "30 3 * * *");
                AddRecurringJobWithTimeZone<DataDumpingQueue_Function>("AgeGroup_ApiTude", functions => functions.LoadContentForApiTude(), "30 3 * * *");
                AddRecurringJobWithTimeZone<DataDumpingQueue_Function>("AgeGroup_RedeamV12", functions => functions.LoadRedeamV12Data(), "30 3 * * *");
                AddRecurringJobWithTimeZone<DataDumpingQueue_Function>("AgeGroup_Redeam", functions => functions.LoadRedeamData(), "30 3 * * *");
                AddRecurringJobWithTimeZone<DataDumpingQueue_Function>("AgeGroup_Bokun", functions => functions.LoadAgeGroupForBokun(), "30 3 * * *");
                AddRecurringJobWithTimeZone<DataDumpingQueue_Function>("AgeGroup_Rezdy", functions => functions.LoadAgeGroupForRezdy(), "30 3 * * *");
                AddRecurringJobWithTimeZone<DataDumpingQueue_Function>("AgeGroup_GlobalTixV3", functions => functions.LoadAgeGroupForGlobalTixV3(), "30 3 * * *");
                //AddRecurringJobWithTimeZone<DataDumpingQueue_Function>("AgeGroup_GlobalTix", functions => functions.LoadAgeGroupForGlobalTix(), "30 3 * * *");
                AddRecurringJobWithTimeZone<DataDumpingQueue_Function>("AgeGroup_Ventrata", functions => functions.LoadAgeGroupForVentrata(), "30 3 * * *");




                AddRecurringJobWithTimeZone<DataDumpingQueue_Function>("AgeGroup_TourCMS", functions => functions.LoadTourCMS(), "30 3 * * *");
                AddRecurringJobWithTimeZone<DataDumpingQueue_Function>("AgeGroup_Sightseeing_SRL", functions => functions.LoadNewCitySightSeeingData(), "0 */1 * * *");
                //AddRecurringJobWithTimeZone<DataDumpingQueue_Function>("AgeGroup_GoCity", functions => functions.LoadGoCityData(), "30 3 * * *");
                AddRecurringJobWithTimeZone<DataDumpingQueue_Function>("AgeGroup_PrioHub", functions => functions.LoadPrioHub(), "30 3 * * *");
                AddRecurringJobWithTimeZone<DataDumpingQueue_Function>("AgeGroup_Rayna", functions => functions.LoadRaynaData(), "30 3 * * *");
                AddRecurringJobWithTimeZone<DataDumpingQueue_Function>("AgeGroup_CacheAgeRefresh", functions => functions.LoadAgeGroupsToCache(), "30 3 * * *");
                AddRecurringJobWithTimeZone<DataDumpingQueue_Function>("AgeGroup_CssExternallProduct", functions => functions.LoadCssExternalProductData(), "30 3 * * *");
                AddRecurringJobWithTimeZone<DataDumpingQueue_Function>("AgeGroup_TourCMSRedemptionData", functions => functions.LoadTourCmsRedemptionData(), "*/10 */10 * * *");
                //RecurringJob.AddOrUpdate<DataDumpingQueue_Function>("Queue_InitiateGoogleFeeds", functions => functions.ProcessMerchantFeed(), "0 */12 * * *");
            }

            else
            {
                RecurringJob.RemoveIfExists("AgeGroup_AOT");
                RecurringJob.RemoveIfExists("AgeGroup_FareHarbor");
                RecurringJob.RemoveIfExists("AgeGroup_GrayLineIceLand");
                RecurringJob.RemoveIfExists("AgeGroup_Prio");
                RecurringJob.RemoveIfExists("AgeGroup_HBAuthorization");
                RecurringJob.RemoveIfExists("AgeGroup_Tiqets");
                //RecurringJob.RemoveIfExists("AgeGroup_GoldenTours");
                RecurringJob.RemoveIfExists("AgeGroup_ApiTude");
                RecurringJob.RemoveIfExists("AgeGroup_Redeam");
                RecurringJob.RemoveIfExists("AgeGroup_Bokun");
                RecurringJob.RemoveIfExists("AgeGroup_Rezdy");
                RecurringJob.RemoveIfExists("AgeGroup_GlobalTix");
                RecurringJob.RemoveIfExists("AgeGroup_Ventrata");
                RecurringJob.RemoveIfExists("AgeGroup_TourCMS");
                RecurringJob.RemoveIfExists("AgeGroup_Sightseeing_SRL");
                RecurringJob.RemoveIfExists("AgeGroup_GoCity");
                RecurringJob.RemoveIfExists("AgeGroup_PrioHub");
                RecurringJob.RemoveIfExists("AgeGroup_Rayna");
                RecurringJob.RemoveIfExists("AgeGroup_RedeamV12");
                RecurringJob.RemoveIfExists("AgeGroup_GlobalTixV3");

                RecurringJob.RemoveIfExists("AgeGroup_CacheAgeRefresh");
                RecurringJob.RemoveIfExists("AgeGroup_CssExternallProduct");
                RecurringJob.RemoveIfExists("AgeGroup_TourCMSRedemptionData");


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
