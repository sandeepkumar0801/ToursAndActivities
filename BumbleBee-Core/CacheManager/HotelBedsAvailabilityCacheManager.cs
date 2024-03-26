using CacheManager.Constants;
using CacheManager.Contract;
using Isango.Entities.Availability;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Util;
using System;
using CacheManager.Helper;
using MongoDB.Driver;

namespace CacheManager
{
    public class HotelBedsAvailabilityCacheManager : IHotelBedsActivitiesCacheManager
    {
        private readonly CollectionDataFactory<Availability> _collectionDataFactory;

        public HotelBedsAvailabilityCacheManager(CollectionDataFactory<Availability> cosmosHelper)
        {
            _collectionDataFactory = cosmosHelper;
        }

        public string LoadAvailabilityCache(List<Availability> availabilityList)
        {
            if (_collectionDataFactory.GetCollectionDataHelper().CheckIfCollectionExist(ConfigurationManagerHelper.GetValuefromAppSettings("HotelBedAvailabilityCollection")).Result)
            {
                _collectionDataFactory.GetCollectionDataHelper().DeleteCollection(ConfigurationManagerHelper.GetValuefromAppSettings("HotelBedAvailabilityCollection")).Wait();
            }

            if (_collectionDataFactory.GetCollectionDataHelper().CreateCollection(ConfigurationManagerHelper.GetValuefromAppSettings("HotelBedAvailabilityCollection"), Constant.HotelBedAvailabilityCollectionPartitionKey).Result)
            {
                return InsertDocuments(availabilityList);
            }

            return string.Empty;
        }

        public List<Availability> GetAvailability(string regionId, string productId)
        {
            var query = productId != string.Empty ? $"select VALUE M from M where M.RegionId={regionId} and M.ServiceId={productId}" : $"select VALUE M from M where M.RegionId={regionId}";
            var filter = Builders<Availability>.Filter.Eq("RegionId", regionId);
            if (!string.IsNullOrEmpty(productId))
            {
                filter = Builders<Availability>.Filter.And(
                                                           Builders<Availability>.Filter.Eq("RegionId", regionId),
                                                           Builders<Availability>.Filter.Eq("ServiceId", productId)
                                                           );
            }
            var result = _collectionDataFactory.GetCollectionDataHelper().GetResultList(ConfigurationManagerHelper.GetValuefromAppSettings("HotelBedAvailabilityCollection"), query, filter);
            return result;
        }

        /// <summary>
        /// This method is for performance testing.
        /// </summary>
        /// <returns></returns>
        public List<Availability> GetAllPriceAndAvailability()
        {
            var query = "select * from M";
            var filter = Builders<Availability>.Filter.Empty;
            var result = _collectionDataFactory.GetCollectionDataHelper().GetResultList(ConfigurationManagerHelper.GetValuefromAppSettings("HotelBedAvailabilityCollection"), query, filter);
            return result;
        }

        private string InsertDocuments(List<Availability> availabilityList)
        {
            var sb = new StringBuilder();
            var maxParallelThreadCount = MaxParallelThreadCountHelper.GetMaxParallelThreadCount("MaxParallelThreadCount");

            Parallel.ForEach(availabilityList, new ParallelOptions { MaxDegreeOfParallelism = maxParallelThreadCount }, (availability) =>
            {
                if (!_collectionDataFactory.GetCollectionDataHelper().InsertDocument(ConfigurationManagerHelper.GetValuefromAppSettings("HotelBedAvailabilityCollection"), availability).Result)
                {
                    sb.Append(sb.Length == 0 ? $"{availability.RegionId}-{availability.ServiceId}" : "," + $"{availability.RegionId}-{availability.ServiceId}");
                }
            });
            return sb.ToString();
        }
    }
}