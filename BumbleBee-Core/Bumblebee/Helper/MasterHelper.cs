using Isango.Entities;
using Isango.Service.Contract;
using Logger.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using ILogger = Logger.Contract.ILogger;

namespace WebAPI.Helper
{
    public class MasterHelper
    {
        private readonly ICacheLoaderService _cacheLoaderService;
        private readonly ILogger _log;

        public MasterHelper(ICacheLoaderService cacheLoaderService, ILogger log)
        {
            _log = log;
            _cacheLoaderService = cacheLoaderService;
        }

        public async Task<bool> LoadSelectedActivities(string activityIds)
        {
            try
            {
                var result = _cacheLoaderService.LoadSelectedActivitiesAsync(activityIds).GetAwaiter().GetResult();
                return await Task.FromResult(result);

            }

            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "masterHelper",
                    MethodName = "LoadSelectedActivitiesAsync"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }
    }
    

    
}