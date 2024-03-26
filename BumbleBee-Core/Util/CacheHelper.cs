using Isango.Entities.Wrapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;

namespace Util
{
    public static class CacheHelper
    {
        private static double _defaultCacheExpirationMins;
        private static object _cacheLock;
        private static bool _isEnableCaching;
        private static IMemoryCache _memoryCache;
        private static List<string> _cacheKeys = new List<string>();
        public static void SetMemoryCache(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
        static CacheHelper()
        {
            _cacheLock = new object();
            _defaultCacheExpirationMins = 120;

            try
            {
                _defaultCacheExpirationMins = Convert.ToDouble(ConfigurationManagerHelper.GetValuefromAppSettings("CacheKeyExpirationTimeInMinutes"));
                _isEnableCaching = ConfigurationManagerHelper.GetValuefromAppSettings("IsEnableCaching") == "1";
            }
            catch (Exception ex)
            {
                _defaultCacheExpirationMins = 120;
                _isEnableCaching = false;
            }
        }

        public static void Set<T>(string key, T value)
        {
            key = key?.ToLower();
            if (string.IsNullOrWhiteSpace(key) || value == null)
                return;
            if (!Exists(key))
            {
                lock (_cacheLock)
                {
                    var clonedValue = DeepClone(value);
                    if (clonedValue != null && _isEnableCaching)
                    {
                        _defaultCacheExpirationMins = _defaultCacheExpirationMins > 0 ? _defaultCacheExpirationMins : 60;
                        _memoryCache.Set(key, clonedValue, TimeSpan.FromMinutes(_defaultCacheExpirationMins));
                        _cacheKeys.Add(key);
                    }
                }
            }
        }

        public static void Setstring<T>(string key, T value)
        {
            key = key?.ToLower();
            if (string.IsNullOrWhiteSpace(key) || value == null)
                return;
            if (!Exists(key))
            {
                lock (_cacheLock)
                {
                    if (value != null && _isEnableCaching)
                    {
                        _defaultCacheExpirationMins = _defaultCacheExpirationMins > 0 ? _defaultCacheExpirationMins : 60;
                        _memoryCache.Set(key, value, TimeSpan.FromMinutes(_defaultCacheExpirationMins));
                        _cacheKeys.Add(key);
                    }
                }
            }
        }

        public static void Set<T>(string key, T value, double expirationInMinutes)
        {
            key = key?.ToLower();

            if (string.IsNullOrWhiteSpace(key) || value == null)
                return;
            if (!Exists(key))
            {
                lock (_cacheLock)
                {
                    if (expirationInMinutes == 0)
                    {
                        expirationInMinutes = _defaultCacheExpirationMins > 0 ? _defaultCacheExpirationMins : 60;
                    }

                    var clonedValue = DeepClone(value);
                    if (clonedValue != null && _isEnableCaching)
                    {
                        _defaultCacheExpirationMins = _defaultCacheExpirationMins > 0 ? _defaultCacheExpirationMins : 60;
                        _memoryCache.Set(key, clonedValue, TimeSpan.FromMinutes(expirationInMinutes));
                        _cacheKeys.Add(key);
                    }
                }
            }
        }

        public static void Set<T>(string key, T value, double expirationInMinutes, double expirationSlidingMins)
        {
            key = key?.ToLower();

            if (string.IsNullOrWhiteSpace(key) || value == null)
                return;
            if (!Exists(key))
            {
                lock (_cacheLock)
                {
                    if (expirationInMinutes == 0)
                    {
                        expirationInMinutes = _defaultCacheExpirationMins > 0 ? _defaultCacheExpirationMins : 60;
                    }
                    var clonedValue = DeepClone(value);
                    if (clonedValue != null && _isEnableCaching)
                    {
                        _defaultCacheExpirationMins = _defaultCacheExpirationMins > 0 ? _defaultCacheExpirationMins : 60;
                        _memoryCache.Set(key, clonedValue, TimeSpan.FromMinutes(expirationInMinutes));
                        _cacheKeys.Add(key);
                    }
                }
            }
        }

        public static bool Exists(string key)
        {
            try
            {
                if (key != null)
                {
                    key = key.ToLower();
                    return _memoryCache.TryGetValue(key, out _);
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public static bool Remove(string key)
        {
            key = key?.ToLower();
            var result = false;
            if (Exists(key))
            {
                _memoryCache.Remove(key);
                _cacheKeys.Remove(key);
                result = true;
            }
            return result;
        }

        public static bool Get<T>(string key, out T value)
        {
            try
            {
                key = key?.ToLower();

                if (Exists(key))
                {
                    var valueFromCache = _memoryCache.Get<T>(key);
                    value = DeepClone(valueFromCache);
                    return true;
                }
            }
            catch (Exception ex)
            {
                // Ignored
            }
            value = default(T);
            return false;
        }
        public static void ClearAll()
        {
            _memoryCache.Dispose();
            _cacheKeys.Clear();
        }
        public static List<string> GetAllKeys()
        {
            return _cacheKeys;
        }
        public static int ItemsCount()
        {
            var result = GetAllKeys()?.Count ?? 0;
            return result;
        }
        // Rest of the methods remain the same...
        // ...

        private static T DeepClone<T>(T obj)
        {
            var result = default(T);
            try
            {
                if (_isEnableCaching)
                {
                    var cobjCopy = SerializeDeSerializeHelper.Serialize(obj);
                    result = SerializeDeSerializeHelper.DeSerialize<T>(cobjCopy);
                }
            }
            catch (Exception ex)
            {
                // Ignored
            }
            return result;
        }
    }
}
