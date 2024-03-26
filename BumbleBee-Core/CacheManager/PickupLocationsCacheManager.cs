using CacheManager.Constants;
using CacheManager.Contract;
using CacheManager.Helper;
using Isango.Entities;
using MongoDB.Driver;
using System.Collections.Generic;
using Util;

namespace CacheManager
{
    public class PickupLocationsCacheManager : IPickupLocationsCacheManager
    {
        private readonly CollectionDataFactory<PickupLocation> _collectionDataFactory;

        public PickupLocationsCacheManager(CollectionDataFactory<PickupLocation> cosmosHelper)
        {
            _collectionDataFactory = cosmosHelper;
        }

        public bool CreateCollection()
        {
            var collectionName = ConfigurationManagerHelper.GetValuefromAppSettings("PickupLocationsCollection");
            if (_collectionDataFactory.GetCollectionDataHelper().CheckIfCollectionExist(collectionName).Result)
                _collectionDataFactory.GetCollectionDataHelper().DeleteCollection(collectionName).Wait();

            if (_collectionDataFactory.GetCollectionDataHelper().CreateCollection(collectionName, Constant.ActivityIdPartitionKey).Result)
                return true;

            return false;
        }

        /// <summary>
        /// Get calender availability
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public List<PickupLocation> GetPickupLocationsByActivity(int productId)
        {
            var query = $"select * from c where c.ActivityId = {productId}";
            var filter = Builders<PickupLocation>.Filter.Eq("ActivityId", productId);
            var result = _collectionDataFactory.GetCollectionDataHelper().GetResultList(ConfigurationManagerHelper.GetValuefromAppSettings("PickupLocationsCollection"), query, filter);
            return result;
        }

        public bool InsertDocuments(PickupLocation pickupLocation)
        {
            var collectionName = ConfigurationManagerHelper.GetValuefromAppSettings("PickupLocationsCollection");
            return _collectionDataFactory.GetCollectionDataHelper().InsertDocument(collectionName, pickupLocation).Result;
        }
    }
}