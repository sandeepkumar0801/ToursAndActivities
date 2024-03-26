
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Util
{
    public static class RedixManagement
    {
        private static IDatabase _cache;
        //private static  IMongoClient _client;

        public static void Initalize()
        {
            try
            {
                var connectionMultiplexer = ConnectionMultiplexer.Connect(ConfigurationManagerHelper.GetValuefromConfig("RedisConnectionString"));
                _cache = connectionMultiplexer.GetDatabase();
            }
            catch(Exception ex)
            {
                _cache = null;
            }
        }

        public static async Task<bool> IsRedisHealthy()
        {
            try
            {
                TimeSpan timeout = TimeSpan.FromSeconds(2);
                var pingTask = _cache.PingAsync();

                var completedTask = await Task.WhenAny(pingTask, Task.Delay(timeout));

                if (completedTask == pingTask)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static void RemoveInactiveSessions()
        {
            var server = _cache.Multiplexer.GetServer(_cache.Multiplexer.GetEndPoints()[0]);
            var now = DateTime.UtcNow;
            var keysToRemove = new List<RedisKey>();

            foreach (var key in server.Keys())
            {
                var idleTime = _cache.Execute("OBJECT", "IDLETIME", key); // Get idle time in seconds
                if (idleTime != null && TimeSpan.FromSeconds((long)idleTime) >= TimeSpan.FromHours(8))
                {
                    keysToRemove.Add(key);
                }
            }

            if (keysToRemove.Any())
            {
                _cache.KeyDelete(keysToRemove.ToArray());
            }
        }



        public static void RemoveKeysCreatedLastDay()
        {
            var server = _cache.Multiplexer.GetServer(_cache.Multiplexer.GetEndPoints()[0]);
            var now = DateTime.UtcNow;
            var keysToRemove = new List<RedisKey>();

            foreach (var key in server.Keys())
            {
                var timeToLive = _cache.KeyTimeToLive(key);
                if (timeToLive != null && timeToLive <= TimeSpan.FromHours(24))
                {
                    keysToRemove.Add(key);
                }
            }

            if (keysToRemove.Any())
            {
                _cache.KeyDelete(keysToRemove.ToArray());
            }
        }


        public static Dictionary<string, Tuple<string, TimeSpan?>> GetAllKeysWithValuesAndTTL()
        {
            var server = _cache.Multiplexer.GetServer(_cache.Multiplexer.GetEndPoints()[0]);
            var keysWithValuesAndTTL = new Dictionary<string, Tuple<string, TimeSpan?>>();

            foreach (var key in server.Keys())
            {
                var value = _cache.StringGet(key); 

                TimeSpan? ttl = _cache.KeyTimeToLive(key);
                keysWithValuesAndTTL[key] = new Tuple<string, TimeSpan?>(value, ttl);
            }

            return keysWithValuesAndTTL;
        }

        public static long DeleteKeysContaining(string search)
        {
            if (string.IsNullOrEmpty(search))
            {
                throw new ArgumentException("Search string is required.");
            }

            var keysToDelete = new List<RedisKey>();
            var server = _cache.Multiplexer.GetServer(_cache.Multiplexer.GetEndPoints()[0]);

            foreach (var key in server.Keys())
            {
                if (key.ToString().Contains(search))
                {
                    keysToDelete.Add(key);
                }
            }

            var deletedCount = _cache.KeyDelete(keysToDelete.ToArray());
            return deletedCount;
        }

    }
}
