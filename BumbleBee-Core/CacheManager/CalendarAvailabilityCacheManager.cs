using CacheManager.Constants;
using CacheManager.Contract;
using Isango.Entities;
using System.Collections.Generic;
using Util;
using CacheManager.Helper;
using MongoDB.Driver;

namespace CacheManager
{
    public class CalendarAvailabilityCacheManager : ICalendarAvailabilityCacheManager
    {
        private readonly CollectionDataFactory<CalendarAvailability> _collectionDataFactory;
        public CalendarAvailabilityCacheManager(CollectionDataFactory<CalendarAvailability> collectionDataFactory)
        {
            _collectionDataFactory = collectionDataFactory;
        }
        
        public bool LoadCalendarAvailability()
        {
            if (!_collectionDataFactory.GetCollectionDataHelper().CheckIfCollectionExist(ConfigurationManagerHelper.GetValuefromAppSettings("CalendarAvailabilityCollection")).Result)
            {
                //_collectionDataFactory.GetCollectionDataHelper().DeleteCollection(ConfigurationManagerHelper.GetValuefromAppSettings("CalendarAvailabilityCollection")).Wait();
                var result = _collectionDataFactory.GetCollectionDataHelper().CreateCollection(ConfigurationManagerHelper.GetValuefromAppSettings("CalendarAvailabilityCollection"), Constant.ActivityIdPartitionKey).Result;
                return result;
            }
            return true;
        }

        /// <summary>
        /// Get calender availability
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="affiliateId"></param>
        /// <returns></returns>
        public List<CalendarAvailability> GetCalendarAvailability(int productId, string affiliateId)
        {
            var query = $"select * from c WHERE c.ActivityId= {productId} and c.AffiliateId = '{affiliateId}'";
            //for mongoDB
            var filter = Builders<CalendarAvailability>.Filter.And(
                                                                  Builders<CalendarAvailability>.Filter.Eq("ActivityId", productId),
                                                                  Builders<CalendarAvailability>.Filter.Eq("AffiliateId", affiliateId)
                                                                  );
            //end mongo
            var result = _collectionDataFactory.GetCollectionDataHelper().GetResultList(ConfigurationManagerHelper.GetValuefromAppSettings("CalendarAvailabilityCollection"), query, filter);
            return result;
        }

        public List<CalendarAvailability> GetOldCalendarAvailability(string timestamp)
        {
            var query = $"select * from c WHERE c._ts <= {timestamp}";
            var result = _collectionDataFactory.GetCollectionDataHelper().GetResultList(ConfigurationManagerHelper.GetValuefromAppSettings("CalendarAvailabilityCollection"), query);
            return result;
        }

        public bool DeleteOldCalendarActivityFromCache(List<CalendarAvailability> calendarAvailabilities)
        {
            var alldeleted = true;
            foreach (var calendarAvailability in calendarAvailabilities)
            {
                var collectionName = ConfigurationManagerHelper.GetValuefromAppSettings("CalendarAvailabilityCollection");
                var result = _collectionDataFactory.GetCollectionDataHelper().DeleteDocument(collectionName, calendarAvailability.Id, calendarAvailability.ActivityId);
                result.Wait();
                alldeleted = result.Result;
            }
            return alldeleted;
        }

        public bool DeleteManyCalendarDocuments()
        {
            return _collectionDataFactory.GetCollectionDataHelper().DeleteCollection(ConfigurationManagerHelper.GetValuefromAppSettings("CalendarAvailabilityCollection")).Result;
        }

        public bool InsertCalendarDocuments(CalendarAvailability calendarAvailability)
        {
            return _collectionDataFactory.GetCollectionDataHelper().InsertDocument(ConfigurationManagerHelper.GetValuefromAppSettings("CalendarAvailabilityCollection"), calendarAvailability).Result;
        }

        public bool InsertManyCalendarDocuments(List<CalendarAvailability> calendarAvailabilities)
        {
            return _collectionDataFactory.GetCollectionDataHelper().InsertManyDocuments(ConfigurationManagerHelper.GetValuefromAppSettings("CalendarAvailabilityCollection"), calendarAvailabilities).Result;
        }
    }
}