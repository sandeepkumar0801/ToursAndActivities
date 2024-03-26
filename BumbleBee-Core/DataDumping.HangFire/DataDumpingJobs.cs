using Hangfire;
using System.Linq.Expressions;

namespace DataDumping.HangFire
{
    public static class DataDumpingJobs
    {
        public static void DataDumpingJobs_Hangfire()
        {
            AddRecurringJobWithTimeZone<DataDumpingFunctions>("DeleteExistingAvailabilityDetails", functions => functions.DeleteExistingAvailabilityDetails(), "5 0 * * *");
            AddRecurringJobWithTimeZone<DataDumpingFunctions>("MoulinRouge", functions => functions.LoadMoulinRougeServiceAvailabilities(), "15 0 * * *");
            AddRecurringJobWithTimeZone<DataDumpingFunctions>("Aot", functions => functions.LoadAotServiceAvailabilities(), "15 0 * * *");
            AddRecurringJobWithTimeZone<DataDumpingFunctions>("Prio", functions => functions.LoadPrioServiceAvailabilities(), "15 0 * * *");
            AddRecurringJobWithTimeZone<DataDumpingFunctions>("TourCMS", functions => functions.LoadTourCMSServiceAvailabilities(), "15 0 * * *");
            AddRecurringJobWithTimeZone<DataDumpingFunctions>("HBApitude", functions => functions.LoadHBApitudeServiceAvailabilities(), "10 0 * * *");
            AddRecurringJobWithTimeZone<DataDumpingFunctions>("RedeamV12", functions => functions.LoadRedeamV12ServiceAvailabilities(), "15 0 * * *");
            AddRecurringJobWithTimeZone<DataDumpingFunctions>("Bokun", functions => functions.LoadBokunServiceAvailabilities(), "15 0 * * *");

            AddRecurringJobWithTimeZone<DataDumpingFunctions>("LoadAPIImagesToCloudinary", functions => functions.LoadAPIImagesToCloudinary(), "30 0 * * *");

            AddRecurringJobWithTimeZone<DataDumpingFunctions>("Isango", functions => functions.LoadIsangoServiceAvailabilities(), "35 0 * * *");
            AddRecurringJobWithTimeZone<DataDumpingFunctions>("LoadCancellationPolicies", functions => functions.LoadCancellationPolicies(), "40 0 * * *");
            AddRecurringJobWithTimeZone<DataDumpingFunctions>("Rezdy", functions => functions.LoadRezdyServiceAvailabilities(), "10 0 * * *");
            AddRecurringJobWithTimeZone<DataDumpingFunctions>("GlobalTixV3", functions => functions.LoadGlobalTixV3ServiceAvailabilities(), "10 0 * * *");


            AddRecurringJobWithTimeZone<DataDumpingFunctions>("Ventrata", functions => functions.LoadVentrataAvailabilities(), "20 0 * * *");
            AddRecurringJobWithTimeZone<DataDumpingFunctions>("NewCitySightSeeing", functions => functions.LoadNewCitySightSeeingServiceAvailabilities(), "10 0 * * *");
            AddRecurringJobWithTimeZone<DataDumpingFunctions>("PrioHub", functions => functions.LoadPrioHubServiceAvailabilities(), "11 0 * * *");
            AddRecurringJobWithTimeZone<DataDumpingFunctions>("Rayna", functions => functions.LoadRaynaServiceAvailabilities(), "11 0 * * *");
            AddRecurringJobWithTimeZone<DataDumpingFunctions>("FareHarbor", functions => functions.LoadFareHarborServiceAvailabilities(), "25 0 * * *");

            AddRecurringJobWithTimeZone<DataDumpingFunctions>("DataDumpingInitiateOrderNotificationRealTimeUpdate", functions => functions.InitiateOrderNotificationRealTimeUpdate(), "0 */12 * * *");
            AddRecurringJobWithTimeZone<DataDumpingFunctions>("Tiqets", functions => functions.LoadTiqetsServiceAvailabilities(), "50 0 * * *");
            AddRecurringJobWithTimeZone<DataDumpingFunctions>("SendAbandonCartEmails", functions => functions.SendAbandonCartEmails(), "10 2 * * *");
            AddRecurringJobWithTimeZone<DataDumpingFunctions>("ElasticProductDataSave", functions => functions.ElasticProductDataSave(), "10 2 * * *");

            //RecurringJob.AddOrUpdate<DataDumpingFunctions>("DataDumpingLoadCancellationPolicies", functions => functions.LoadCancellationPolicies(), "0 */12 * * *");
            //RecurringJob.AddOrUpdate<DataDumpingFunctions>("DataDumpingInitiateLoadIsangoServiceAvailabilities", functions => functions.InitiateLoadIsangoServiceAvailabilities(), "0 */12 * * *");
            //RecurringJob.AddOrUpdate<DataDumpingFunctions>("DataDumpingInitiateGoogleFeeds", functions => functions.InitiateGoogleFeeds(), "0 */12 * * *");
            //RecurringJob.AddOrUpdate<DataDumpingFunctions>("DataDumpingInitiateAgeDumping", functions => functions.InitiateAgeDumping(), "0 */12 * * *");
            //AddRecurringJobWithTimeZone<DataDumpingFunctions>("DataDumpingLoadGlobalTixServiceAvailabilities", functions => functions.LoadGlobalTixServiceAvailabilities(), "10 0 * * *");
            //RecurringJob.AddOrUpdate<DataDumpingFunctions>("DataDumpingLoadHBApitudeServiceAvailabilities", functions => functions.LoadHBApitudeServiceAvailabilities(), "0 */12 * * *");

        }

        public static void AddRecurringJobWithTimeZone<T>(string jobId, Expression<Action<T>> methodCall, string cronExpression)
        {
            RecurringJob.AddOrUpdate<T>(jobId, methodCall, cronExpression);
        }


    }
}
