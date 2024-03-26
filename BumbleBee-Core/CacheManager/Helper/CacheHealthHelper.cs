using CacheManager.Contract;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util;

namespace CacheManager.Helper
{
    public class CacheHealthHelper : ICacheHealthHelper
    {
        private readonly IMongoClient _client;

        public CacheHealthHelper()
        {
            _client = new MongoClient(ConfigurationManagerHelper.GetValuefromAppSettings("MongoDataBaseConnection"));
        }

        public async Task<bool> IsMongoHealthy()
        {
            try
            {
                await _client.ListDatabasesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
