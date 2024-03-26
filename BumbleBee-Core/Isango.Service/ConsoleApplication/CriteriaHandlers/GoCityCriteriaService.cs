using CacheManager.Contract;
using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.ConsoleApplication.ServiceAvailability;
using Isango.Entities.Enums;
using Isango.Entities.NewCitySightSeeing;
using Isango.Entities.Rezdy;
using Isango.Persistence.Contract;
using Isango.Service.Contract;
using Logger.Contract;
using ServiceAdapters.NewCitySightSeeing;
using ServiceAdapters.Rezdy;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Isango.Service.ConsoleApplication.CriteriaHandlers
{
    public class GoCityCriteriaService : IGoCityCriteriaService
    {
        
        private readonly ILogger _log;

        public List<Activity> GetAvailability(Entities.ConsoleApplication.ServiceAvailability.Criteria criteria)
        {
            return null;
            
        }
        public List<TempHBServiceDetail> GetServiceDetails(List<Activity> activities,
            List<IsangoHBProductMapping> mappedProducts)
        {
            return null;
        }
    }
}