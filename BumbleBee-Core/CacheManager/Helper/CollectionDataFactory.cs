using Autofac;
using CacheManager.Contract;
using Isango.Entities;
using Isango.Entities.Enums;
using Logger.Contract;
using System;
using Util;

namespace CacheManager.Helper
{
    public class CollectionDataFactory<T>
    {
        private ILifetimeScope _lifetimeScope;
        private ILogger _log;
        private static string collectionDatabase = ConfigurationManagerHelper.GetValuefromAppSettings("CollectionDataBase");
        public CollectionDataFactory(ILifetimeScope lifetimeScope,

            ILogger log
        )
        {
            _lifetimeScope = lifetimeScope;
            _log = log;
        }

        public CollectionDataFactory()
        {
            //Empty Constructor for test Case Initialisation
            //https://stackoverflow.com/questions/42254974/cannot-instantiate-proxy-of-class-error/42276752
        }


        public  ICollectionDataHelper<T> GetCollectionDataHelper()
        {
            try
            {
                Int32.TryParse(collectionDatabase, out int collectionType);
                switch (collectionType)
                {
                    case (int)CollectionType.CosmosDB:
                        {
                            return new CosmosHelper<T>(_log);
                        }
                    case (int)CollectionType.MongoDB:
                        {
                            return new MongoHelper<T>(_log);
                        }
                    default:
                        {
                            return new CosmosHelper<T>(_log);
                        }
                }
            }
            catch (System.Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CollectionDataFactory",
                    MethodName = "GetCollectionDataHelper",
                };
                _log.Error(isangoErrorEntity, ex);
                
            }
            return null;
        }
    }
}