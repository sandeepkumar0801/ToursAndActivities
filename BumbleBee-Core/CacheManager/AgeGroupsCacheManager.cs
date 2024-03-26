using CacheManager.Constants;
using CacheManager.Contract;
using CacheManager.Helper;
using Isango.Entities;
using Logger.Contract;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Util;

namespace CacheManager
{
    public class AgeGroupsCacheManager : IAgeGroupsCacheManager
    {
        private readonly CollectionDataFactory<AgeGroup> _collectionDataFactory;
        private readonly CollectionDataFactory<FareHarborAgeGroup> _collectionDataFactoryFareHarbor;
        private readonly ILogger _logger;

        public AgeGroupsCacheManager(CollectionDataFactory<AgeGroup> collectionDataFactory, CollectionDataFactory<FareHarborAgeGroup> collectionDataFactoryFareHarbor, ILogger logger)
        {
            _collectionDataFactory = collectionDataFactory;
            _collectionDataFactoryFareHarbor = collectionDataFactoryFareHarbor;
            _logger = logger;
        }

        public string LoadAgeGroupByActivity(List<AgeGroup> ageGroupByActivityList)
        {
            if (!_collectionDataFactory.GetCollectionDataHelper().CheckIfCollectionExist(ConfigurationManagerHelper.GetValuefromAppSettings("AgeGroupByActivityCollection")).Result)
            {
                _collectionDataFactory.GetCollectionDataHelper().CreateCollection(ConfigurationManagerHelper.GetValuefromAppSettings("AgeGroupByActivityCollection"), Constant.ActivityIdPartitionKey);
            }

            return InsertDocuments(ageGroupByActivityList);
        }

        public string LoadFhAgeGroupByActivity(List<FareHarborAgeGroup> ageGroupByActivityList)
        {
            //if (_cosmosHelper.CheckIfCollectionExist(ConfigurationManagerHelper.GetValuefromAppSettings("AgeGroupByActivityCollection")).Result)
            //{
            //    _cosmosHelper.DeleteCollection(ConfigurationManagerHelper.GetValuefromAppSettings("AgeGroupByActivityCollection")).Wait();
            //}

            //if (_cosmosHelper.CreateCollection(ConfigurationManagerHelper.GetValuefromAppSettings("AgeGroupByActivityCollection"), Constant.ActivityIdPartitionKey).Result)
            //{
            //    return InsertFhDocuments(ageGroupByActivityList);
            //}

            if (!_collectionDataFactory.GetCollectionDataHelper().CheckIfCollectionExist(ConfigurationManagerHelper.GetValuefromAppSettings("AgeGroupByActivityCollection")).Result)
            {
                _collectionDataFactory.GetCollectionDataHelper().CreateCollection(ConfigurationManagerHelper.GetValuefromAppSettings("AgeGroupByActivityCollection"), Constant.ActivityIdPartitionKey);
            }

            return InsertFhDocuments(ageGroupByActivityList);
        }

        /// <summary>
        /// for Prio, Aot and GLI
        /// </summary>
        /// <param name="apiType"></param>
        /// <param name="activityID"></param>
        /// <returns></returns>
        public List<AgeGroup> GetAgeGroup(int apiType, int activityID)
        {
            var ageGroups = default(List<AgeGroup>);
            var query = string.Empty;
            try
            {
                query = $"select * from c where c.ApiType = {apiType} and c.ActivityId = {activityID}";
                //for mongoDB
                var filter = Builders<AgeGroup>.Filter.And(
                                                   Builders<AgeGroup>.Filter.Eq("ActivityId", activityID),
                                                   Builders<AgeGroup>.Filter.Eq("ApiType", apiType)
                                                            );

                //end mongo
                ageGroups = _collectionDataFactory.GetCollectionDataHelper().GetResultList(ConfigurationManagerHelper.GetValuefromAppSettings("AgeGroupByActivityCollection"), query, filter);
            }
            catch (System.Exception ex)
            {
                Task.Run(() =>
                        _logger.Error(
                                    new IsangoErrorEntity
                                    {
                                        ClassName = nameof(AgeGroupsCacheManager),
                                        MethodName = nameof(GetAgeGroup),
                                        Params = $"Collection: AgeGroupByActivityCollection, Query: {query}"
                                    }, ex
                            )
                    );
            }
            return ageGroups;
        }

        /// <summary>
        /// For Fare harbor
        /// </summary>
        /// <param name="apiType"></param>
        /// <param name="activityID"></param>
        /// <returns></returns>
        public List<FareHarborAgeGroup> GetFareHarborAgeGroup(int apiType, int activityID)
        {
            var query = $"select * from c where c.ApiType = {apiType} and c.ActivityId = {activityID}";
            //for mongoDB
            var filter = Builders<FareHarborAgeGroup>.Filter.And(
                                                                  Builders<FareHarborAgeGroup>.Filter.Eq("ActivityId", activityID),
                                                                  Builders<FareHarborAgeGroup>.Filter.Eq("ApiType", apiType)
                                                                  );
            //end mongo
            var result = _collectionDataFactoryFareHarbor.GetCollectionDataHelper().GetResultList(ConfigurationManagerHelper.GetValuefromAppSettings("AgeGroupByActivityCollection"), query, filter);
            return result;
        }

        private string InsertDocuments(List<AgeGroup> ageGroupByActivityList)
        {
            var sb = new StringBuilder();
            foreach (var ageGroupByActivity in ageGroupByActivityList)
            {
                if (!_collectionDataFactory.GetCollectionDataHelper().InsertDocument(ConfigurationManagerHelper.GetValuefromAppSettings("AgeGroupByActivityCollection"), ageGroupByActivity).GetAwaiter().GetResult())
                {
                    sb.Append(sb.Length == 0 ? ageGroupByActivity.AgeGroupId.ToString() : "," + ageGroupByActivity.AgeGroupId);
                }
            }
            return sb.ToString();
        }

        private string InsertFhDocuments(List<FareHarborAgeGroup> ageGroupByActivityList)
        {
            var sb = new StringBuilder();
            foreach (var fHAgeGroupByActivity in ageGroupByActivityList)
            {
                if (!_collectionDataFactoryFareHarbor.GetCollectionDataHelper().InsertDocument(ConfigurationManagerHelper.GetValuefromAppSettings("AgeGroupByActivityCollection"), fHAgeGroupByActivity).Result)
                {
                    sb.Append(sb.Length == 0 ? fHAgeGroupByActivity.AgeGroupId.ToString() : "," + fHAgeGroupByActivity.AgeGroupId);
                }
            }

            return sb.ToString();
        }
    }
}