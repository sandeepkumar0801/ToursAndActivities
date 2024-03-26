using Microsoft.Extensions.Configuration;

namespace Util
{
    public static class ConfigurationManagerHelperWebjob
    {
        private static IConfiguration Configuration { get; }

        static ConfigurationManagerHelperWebjob()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.WebJob.json", optional: true, reloadOnChange: true);

            Configuration = builder.Build();
        }

        /// <summary>
        /// Get Method to get value from App Settings
        /// </summary>
        /// <param name="key">key for which the value required</param>
        /// <returns>returns value for the provided key</returns>
        //public static string GetValuefromAppSettings(string key)
        //{
        //    try
        //    {
        //        return Configuration[key];
        //    }
        //    catch (System.Exception)
        //    {
        //        return string.Empty;
        //    }
        //}

        public static string GetValuefromAppSettings(string key)
        {
            try
            {
                var appSection = Configuration.GetSection("AppSettings");
                return appSection[key];
            }
            catch (System.Exception)
            {
                return string.Empty;
            }
        }


        /// <summary>
        /// Get Method to get value from configuration Settings
        /// </summary>
        /// <param name="key">key for which the value required</param>
        /// <returns>returns value for the provided key</returns>
        public static string GetValuefromConfig(string key)
        {
            return Configuration.GetConnectionString(key);
        }
    }
}
