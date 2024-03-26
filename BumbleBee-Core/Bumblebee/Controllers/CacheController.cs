using Microsoft.AspNetCore.Mvc;
using System.Text;
using Util;
using WebAPI.Filters;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CacheController : ApiBaseController
    {
        //protected IActionResult GetResponseWithHttpActionResult<T>(T apiResponseObject)
        //{
        //    if (apiResponseObject == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(apiResponseObject);
        //}

        /// <summary>
        /// Get count of items in cache and their keys
        /// </summary>
        /// <returns></returns>
        [Route("Get")]

        [HttpGet]
        public IActionResult Get(string cacheGetToken)
        {
            var tokenInConfig = ConfigurationManagerHelper.GetValuefromAppSettings("CacheDeleteToken");
            if (tokenInConfig.ToLower() == cacheGetToken.ToLower())
            {
                var result = new List<string>();
                result.Add($"Total cached items : {CacheHelper.ItemsCount()}");
                result.AddRange(CacheHelper.GetAllKeys());
                return GetResponseWithActionResult(result);
            }
            else
            {
                return GetResponseWithActionResult("GetToken token doesn't match.");

            }
        }

        /// <summary>
        /// Get object from cahce based on passed key as input
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [Route("GetCachedItem")]

        [HttpGet]
        public IActionResult GetCachedItemwithCaching(string key, string cacheGetToken)
        {
            var tokenInConfig = ConfigurationManagerHelper.GetValuefromAppSettings("CacheDeleteToken");
            if (tokenInConfig.ToLower() == cacheGetToken.ToLower())
            {
                var result = string.Empty;
                if (CacheHelper.Exists(key))
                {
                    CacheHelper.Get<object>(key, out var resultobj);
                    result = Util.SerializeDeSerializeHelper.Serialize(resultobj);
                }
                else
                {
                    result = $"{DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss")} :  {key} key not found.";
                }
                return GetResponseWithActionResult(result);
            }
            else
            {
                return GetResponseWithActionResult("GetToken doesn't match.");

            }
        }

        /// <summary>
        /// Delete item from cache based on key.Pass all to clear all cahced items.
        /// </summary>
        /// <param name="key">Pass all to clear all cahced items.</param>
        /// <param name="cacheDeleteToken"> Token required to clear cache.Only isango team should clear cahce if needed.</param>
        /// <returns></returns>
        [Route("Delete")]

        [HttpGet]
        public IActionResult DeleteCacheItem(string key, string cacheDeleteToken)
        {
            var builder = new StringBuilder();
            var dateTime = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
            try
            {
                var tokenInConfig = ConfigurationManagerHelper.GetValuefromAppSettings("CacheDeleteToken");
                if (tokenInConfig.ToLower() == cacheDeleteToken.ToLower())
                {
                    var keyArray = key?.Split(',');
                    foreach (var cachekey in keyArray)
                    {
                        if (CacheHelper.Exists(cachekey))
                        {
                            CacheHelper.Get<object>(cachekey, out var resultobj);
                            var isDeleted = CacheHelper.Remove(cachekey);

                            builder.Append($"\n{dateTime} :  {cachekey} Cache Item Cleared, isDeleted : {isDeleted}\n");
                        }
                        else if (cachekey.ToLower() == "all")
                        {
                            CacheHelper.ClearAll();
                            builder.Append($"\n{dateTime} :  All Cache items Cleared");
                        }
                        else
                        {
                            builder.Append($"\n{dateTime} :  {cachekey} key not found.");
                        }
                    }
                }
                else
                {
                    builder.Append("Delete token doesn't match.");
                }
            }
            catch (Exception ex)
            {
                //ignored
            }
            return GetResponseWithActionResult(builder.ToString());
        }

        [Route("GetRedisCachedItem")]
        [HttpGet]
        public IActionResult GetRedisCachedItem()
        {
            try
            {
                var data = RedixManagement.GetAllKeysWithValuesAndTTL();
                return GetResponseWithActionResult(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while retrieving data from Redis.");
            }
        }

        [Route("DeleteRedisCacheByKey")]
        [HttpGet]
        public IActionResult DeleteRedisCacheByKey(string key, string cacheDeleteToken)
        {
            try
            {
                var tokenInConfig = ConfigurationManagerHelper.GetValuefromAppSettings("CacheDeleteToken");
                if (tokenInConfig.ToLower() == cacheDeleteToken.ToLower())
                {
                    var deletedCount = RedixManagement.DeleteKeysContaining(key);
                    return GetResponseWithActionResult($"{deletedCount} keys are deleted");
                }
                else
                {
                    return StatusCode(500, "Please Contact with administrator as cacheDeleteToken is wrong");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while retrieving data from Redis.");
            }
        }
    }
}