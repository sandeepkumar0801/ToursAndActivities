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
    public class NetPriceCacheManager : INetPriceCacheManager
    {
        private readonly CollectionDataFactory<NetPriceMasterData> _collectionDataFactory;

        public NetPriceCacheManager(CollectionDataFactory<NetPriceMasterData> cosmosHelper)
        {
            _collectionDataFactory = cosmosHelper;
        }

        public string LoadNetPriceMasterData(List<NetPriceMasterData> netPriceDataList)
        {
            if (_collectionDataFactory.GetCollectionDataHelper().CheckIfCollectionExist(ConfigurationManagerHelper.GetValuefromAppSettings("NetPriceDataCollection")).Result)
            {
                _collectionDataFactory.GetCollectionDataHelper().DeleteCollection(ConfigurationManagerHelper.GetValuefromAppSettings("NetPriceDataCollection")).Wait();
            }

            if (_collectionDataFactory.GetCollectionDataHelper().CreateCollection(ConfigurationManagerHelper.GetValuefromAppSettings("NetPriceDataCollection"), Constant.NetPriceDataCollectionPartitionKey).Result)
            {
                return InsertDocuments(netPriceDataList);
            }

            return string.Empty;
        }

        public List<NetPriceMasterData> GetNetPriceMasterData()
        {
            var query = $"select VALUE M from {ConfigurationManagerHelper.GetValuefromAppSettings("NetPriceDataCollection")} M";
            var filter = Builders<NetPriceMasterData>.Filter.Empty;
            var result = _collectionDataFactory.GetCollectionDataHelper().GetResultList(ConfigurationManagerHelper.GetValuefromAppSettings("NetPriceDataCollection"), query, filter);
            return result;
        }

        private string InsertDocuments(List<NetPriceMasterData> netPriceDataList)
        {
            var sb = new StringBuilder();
            foreach (var netPriceData in netPriceDataList)
            {
                if (!_collectionDataFactory.GetCollectionDataHelper().InsertDocument(ConfigurationManagerHelper.GetValuefromAppSettings("NetPriceDataCollection"), netPriceData).Result)
                {
                    sb.Append(sb.Length == 0 ? netPriceData.ProductId.ToString() : "," + netPriceData.ProductId);
                }
            }
            return sb.ToString();
        }
    }
}