using CacheManager.Constants;
using CacheManager.Contract;
using CacheManager.Helper;
using Isango.Entities;
using System.Collections.Generic;
using System.Text;
using Util;

namespace CacheManager
{
    public class HbProductMappingCacheManager : IHbProductMappingCacheManager
    {
        
        private readonly CollectionDataFactory<IsangoHBProductMapping> _collectionDataFactory;
        public HbProductMappingCacheManager(CollectionDataFactory<IsangoHBProductMapping> collectionDataFactory)
        {
            _collectionDataFactory = collectionDataFactory;
        }

        public string LoadCacheMapping(List<IsangoHBProductMapping> isangoHBProductMappingList)
        {
            if (_collectionDataFactory.GetCollectionDataHelper().CheckIfCollectionExist(ConfigurationManagerHelper.GetValuefromAppSettings("HBProductMappingCollection")).Result)
            {
                _collectionDataFactory.GetCollectionDataHelper().DeleteCollection(ConfigurationManagerHelper.GetValuefromAppSettings("HBProductMappingCollection")).Wait();
            }

            if (_collectionDataFactory.GetCollectionDataHelper().CreateCollection(ConfigurationManagerHelper.GetValuefromAppSettings("HBProductMappingCollection"), Constant.HBProductMappingCollectionPartitionKey).Result)
            {
                return InsertDocuments(isangoHBProductMappingList);
            }

            return string.Empty;
        }

        private string InsertDocuments(List<IsangoHBProductMapping> isangoHBProductMappingList)
        {
            var sb = new StringBuilder();
            foreach (var isangoHBProductMapping in isangoHBProductMappingList)
            {
                if (!_collectionDataFactory.GetCollectionDataHelper().InsertDocument(ConfigurationManagerHelper.GetValuefromAppSettings("HBProductMappingCollection"), isangoHBProductMapping).Result)
                {
                    sb.Append(sb.Length == 0 ? isangoHBProductMapping.IsangoRegionId.ToString() : "," + isangoHBProductMapping.IsangoRegionId);
                }
            }

            return sb.ToString();
        }
    }
}