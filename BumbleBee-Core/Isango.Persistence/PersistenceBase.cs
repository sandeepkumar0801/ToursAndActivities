using Microsoft.Extensions.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Runtime.Caching;

namespace Isango.Persistence
{
    public abstract class PersistenceBase
    {
        private static readonly ObjectCache Cache = MemoryCache.Default;
        private static readonly string ConfigurationCacheKey = "ApplicationConfiguration";
        private static readonly CacheItemPolicy DefaultCacheItemPolicy = new CacheItemPolicy { SlidingExpiration = TimeSpan.FromMinutes(15) }; // Adjust expiration as needed

        public IConfigurationRoot GetConfiguration()
        {
            try
            {
                if (Cache.Contains(ConfigurationCacheKey))
                {
                    return (IConfigurationRoot)Cache[ConfigurationCacheKey];
                }
                var configuration = LoadConfiguration();
                Cache.Add(ConfigurationCacheKey, configuration, DefaultCacheItemPolicy);

                return configuration;
            }
            catch(Exception ex)
            {
                var configuration = LoadConfiguration();
                Cache.Add(ConfigurationCacheKey, configuration, DefaultCacheItemPolicy);

                return configuration;
            }
        }

        private IConfigurationRoot LoadConfiguration()
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
        
        /// <summary>
        /// Returns new database instance of IsangoLiveDB database
        /// </summary>
        protected Database IsangoDataBaseLive
        {
            get
            {
                try
                {
                    if (Cache.Contains(Constants.Constants.IsangoLiveDb))
                    {
                        var constringCache = (string)Cache[Constants.Constants.IsangoLiveDb];
                        return new SqlDatabase(constringCache);
                    }

                    var configuration = GetConfiguration();
                    var constring = configuration.GetConnectionString(Constants.Constants.IsangoLiveDb);
                    if (constring != null)
                    {
                        Cache.Add(Constants.Constants.IsangoLiveDb, constring, DefaultCacheItemPolicy);
                    }
                    return new SqlDatabase(constring);
                }
                catch
                {
                    var configuration = GetConfiguration();
                    var constring = configuration.GetConnectionString(Constants.Constants.IsangoLiveDb);
                    if (constring != null)
                    {
                        Cache.Add(Constants.Constants.IsangoLiveDb, constring, DefaultCacheItemPolicy);
                    }

                    return new SqlDatabase(constring);

                }
            }
        }

        /// <summary>
        /// Returns new database instance of PrimalIdentities database
        /// </summary>
        protected Database PrimalIdentitiesDb
        {
            get
            {
                var configuration = GetConfiguration();
                var constring = configuration.GetConnectionString(Constants.Constants.PrimalIdentitiesDb);
                return new SqlDatabase(constring);
            }
        }

        /// <summary>
        /// Returns new database instance of API_Upload database
        /// </summary>
        protected Database APIUploadDb
        {
            get
            {
                var configuration = GetConfiguration();
                var constring = configuration.GetConnectionString(Constants.Constants.IsangoLiveDb);
                return new SqlDatabase(constring);

                //var factory = new DatabaseProviderFactory();
                //return factory.Create(Constants.Constants.APIUploadDb);
            }
        }
    }
}