using Autofac;
using CacheManager.Contract;
using CacheManager.FareHarborCacheManagers;
using CacheManager.Helper;
using CacheManager.TiqetsCacheManager;
using Isango.Entities.Enums;

namespace CacheManager
{
    public class CacheModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ActivityCacheManager>().As<IActivityCacheManager>().InstancePerLifetimeScope();
            builder.RegisterType<AffiliateCacheManager>().As<IAffiliateCacheManager>().InstancePerLifetimeScope();
            builder.RegisterType<MasterCacheManager>().As<IMasterCacheManager>().InstancePerLifetimeScope();
            builder.RegisterType<LandingCacheManager>().As<ILandingCacheManager>().InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(CosmosHelper<>)).As(typeof(ICosmosHelper<>)).InstancePerLifetimeScope();
            builder.RegisterType<SimilarProductsRegionAttractionCacheManager>().As<ISimilarProductsRegionAttractionCacheManager>().InstancePerLifetimeScope();
            builder.RegisterType<NetPriceCacheManager>().As<INetPriceCacheManager>().InstancePerLifetimeScope();
            builder.RegisterType<HbProductMappingCacheManager>().As<IHbProductMappingCacheManager>().InstancePerLifetimeScope();
            builder.RegisterType<AgeGroupsCacheManager>().As<IAgeGroupsCacheManager>().InstancePerLifetimeScope();
            builder.RegisterType<HotelBedsAvailabilityCacheManager>().As<IHotelBedsActivitiesCacheManager>().InstancePerLifetimeScope();
            builder.RegisterType<MemCache>().As<IMemCache>().InstancePerLifetimeScope();
            builder.RegisterType<FareHarborCustomerPrototypesCacheManager>().As<IFareHarborCustomerPrototypesCacheManager>().InstancePerLifetimeScope();
            builder.RegisterType<FareHarborCustomerTypesCacheManager>().As<IFareHarborCustomerTypesCacheManager>().InstancePerLifetimeScope();
            builder.RegisterType<FareHarborUserKeysCacheManager>().As<IFareHarborUserKeysCacheManager>().InstancePerLifetimeScope();
            builder.RegisterType<AttractionActivityMappingCacheManager>().As<IAttractionActivityMappingCacheManager>().InstancePerLifetimeScope();
            builder.RegisterType<PricingRulesCacheManager>().As<IPricingRulesCacheManager>().InstancePerLifetimeScope();
            builder.RegisterType<CalendarAvailabilityCacheManager>().As<ICalendarAvailabilityCacheManager>().InstancePerLifetimeScope();
            builder.RegisterType<PickupLocationsCacheManager>().As<IPickupLocationsCacheManager>().InstancePerLifetimeScope();
            builder.RegisterType<TiqetsPaxMappingCacheManager>().As<ITiqetsPaxMappingCacheManager>().InstancePerLifetimeScope();
            builder.RegisterType<IsangoDocumentClient>().SingleInstance().InstancePerLifetimeScope();
            builder.RegisterType<SynchronizerCacheManager>().As<ISynchronizerCacheManager>().InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(CosmosHelper<>)).As(typeof(ICollectionDataHelper<>)).InstancePerDependency();
            builder.RegisterGeneric(typeof(MongoHelper<>)).As(typeof(IMongoHelper<>)).InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(MongoHelper<>)).As(typeof(ICollectionDataHelper<>)).InstancePerDependency();
            builder.RegisterGeneric(typeof(CollectionDataFactory<>)).InstancePerDependency();
            builder.RegisterType<CacheHealthHelper>().As<ICacheHealthHelper>().InstancePerLifetimeScope();

        }
    }
}