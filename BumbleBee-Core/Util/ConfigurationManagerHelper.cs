using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using System.Runtime.Caching;

namespace Util
{
    public static class ConfigurationManagerHelper
    {
        private static readonly ObjectCache Cache = MemoryCache.Default;
        private static readonly string ConfigurationCacheKey = "ApplicationConfiguration";
        private static readonly CacheItemPolicy DefaultCacheItemPolicy = new CacheItemPolicy { SlidingExpiration = TimeSpan.FromMinutes(15) }; //
        public static IConfigurationRoot GetConfiguration()
        {
            try
            {
                if (Cache.Contains(ConfigurationCacheKey))
                {
                    return (IConfigurationRoot)Cache[ConfigurationCacheKey];
                }

                var configuration = LoadConfiguration_util();
                Cache.Add(ConfigurationCacheKey, configuration, DefaultCacheItemPolicy);

                return configuration;
            }
            catch (Exception ex)
            {
                var configuration = LoadConfiguration_util();
                Cache.Add(ConfigurationCacheKey, configuration, DefaultCacheItemPolicy);
                return configuration;

            }
        }

        private static IConfigurationRoot LoadConfiguration_util()
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            Console.WriteLine("Environment: " + environment);

            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory());

            if (environment != null)
            {
                builder.AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true);
            }
            else
            {
                builder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            }

            return builder.Build();
        }

        public static string GetValuefromConfig(string key)
        {
            try
            {
                
                var configuration = GetConfiguration();
                var constring = configuration.GetConnectionString(key);
                return constring;
            }
            catch (Exception ex)
            {
                var configuration = GetConfiguration();
                var constring = configuration.GetConnectionString(key);
                return constring;
            }
        }

        public static string GetValuefromAppSettings(string key)
        {
            try
            {
                var configuration = GetConfiguration();
                var configValue = configuration.GetSection("AppSettings")[key];
                return configValue;
            }
            catch
            {
                var configuration = GetConfiguration();
                return configuration.GetSection("AppSettings")[key];

            }
        }
    }
}