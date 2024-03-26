using CacheManager.Constants;
using CacheManager.Contract;
using CacheManager.Helper;
using Isango.Entities;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Text;
using Util;

namespace CacheManager
{
    public class AttractionActivityMappingCacheManager : IAttractionActivityMappingCacheManager
    {
        private readonly CollectionDataFactory<AttractionActivityMapping> _collectionDataFactoryFilter;

        public AttractionActivityMappingCacheManager(CollectionDataFactory<AttractionActivityMapping> collectionDataFactoryFilter)
        {
            _collectionDataFactoryFilter = collectionDataFactoryFilter;
        }

        public string LoadCachedAttractionActivity(List<AttractionActivityMapping> attractionActivityList)
        {
            if (_collectionDataFactoryFilter.GetCollectionDataHelper().CheckIfCollectionExist(ConfigurationManagerHelper.GetValuefromAppSettings("AttractonActivityMappingCollection")).Result)
            {
                _collectionDataFactoryFilter.GetCollectionDataHelper().DeleteCollection(ConfigurationManagerHelper.GetValuefromAppSettings("AttractonActivityMappingCollection")).Wait();
            }

            if (_collectionDataFactoryFilter.GetCollectionDataHelper().CreateCollection(ConfigurationManagerHelper.GetValuefromAppSettings("AttractonActivityMappingCollection"), Constant.AttractonActivityMappingCollectionParitionKey).Result)
            {
                return InsertDocuments(attractionActivityList);
            }

            return string.Empty;
        }

        public List<AttractionActivityMapping> GetAttractionActivityList(int attractionId)
        {
            var query = $"select VALUE M from {ConfigurationManagerHelper.GetValuefromAppSettings("AttractonActivityMappingCollection")} M where M.AttractionId='{attractionId}'";
            //for mongoDB
            var filter = Builders<AttractionActivityMapping>.Filter.Eq("AttractionId", attractionId.ToString());
            //end mongo
            var result = _collectionDataFactoryFilter.GetCollectionDataHelper().GetResultList(ConfigurationManagerHelper.GetValuefromAppSettings("AttractonActivityMappingCollection"), query, filter);
            return result;
        }

        public string InsertDocuments(List<AttractionActivityMapping> attractionActivityList)
        {
            var sb = new StringBuilder();
            foreach (var attractionActivity in attractionActivityList)
            {
                if (!_collectionDataFactoryFilter.GetCollectionDataHelper().InsertDocument(ConfigurationManagerHelper.GetValuefromAppSettings("AttractonActivityMappingCollection"), attractionActivity).Result)
                {
                    sb.Append(sb.Length == 0 ? attractionActivity.AttractionId : "," + attractionActivity.AttractionId);
                }
            }

            return sb.ToString();
        }
    }
}