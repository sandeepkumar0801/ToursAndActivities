using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheLoader.HangFire
{
    public static class HangfireJobConfiguration
    {
        public static void ConfigureJobs(Func<Functions> functionsFactory)
        {
            RecurringJob.AddOrUpdate("RegionDestinationMapping", () => functionsFactory().RegionDestinationMapping(), Cron.Daily);
        }
    }
}
