using Autofac;
using Isango.Persistence;
using Isango.Service;
using Logger;
using ServiceAdapters;
using ServiceAdapters.AlternativePayment;
using ServiceAdapters.Aot;
using ServiceAdapters.Apexx;
using ServiceAdapters.BigBus;
using ServiceAdapters.Bokun;
using ServiceAdapters.FareHarbor;
using ServiceAdapters.GoldenTours;
using ServiceAdapters.GrayLineIceLand;
using ServiceAdapters.HB;
using ServiceAdapters.HotelBeds;
using ServiceAdapters.MoulinRouge;
using ServiceAdapters.NeverBounce;
using ServiceAdapters.PrioTicket;
using ServiceAdapters.Tiqets;
using ServiceAdapters.WirecardPayment;
using ServiceAdapters.GoogleMaps;
using ServiceAdapters.Redeam;
using ServiceAdapters.RiskifiedPayment;
using ServiceAdapters.GlobalTix;
using ServiceAdapters.Rezdy;
using ServiceAdapters.Ventrata;
using ServiceAdapters.Adyen;
using ServiceAdapters.TourCMS;
using ServiceAdapters.NewCitySightSeeing;
using ServiceAdapters.GoCity;
using ServiceAdapters.PrioHub;
using ServiceAdapters.RaynaModule;
using Autofac.Core;
using Isango.Mailer;
using PriceRuleEngine;
using TableStorageOperations;
using ApplicationCacheManager;
using CacheManager;
using ActivityWrapper;
using DiscountRuleEngine;
using ServiceAdapters.CitySightSeeing;
using ServiceAdapters.SightSeeing;
using ServiceAdapters.RedeamV12;
using ServiceAdapters.GlobalTixV3;

namespace Isango.Register
{
    public class StartupModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Register Dependency
            builder.RegisterModule<WrapperModule>();

            // Register your Autofac modules and dependencies her
            builder.RegisterModule<ServiceModule>();
            //Persistence
            builder.RegisterModule<PersistenceModule>();
            //CacheManager
            builder.RegisterModule<CacheModule>();
            //Logger
            builder.RegisterModule<LoggerModule>();

            //Service Adapter
            builder.RegisterModule<AdapterModule>();

            //SightSeeing
            builder.RegisterModule<SightSeeingModule>();

            //Bokun
            builder.RegisterModule<BokunModule>();

            //AlternativePayment
            builder.RegisterModule<AlternativePaymentModule>();

            //NeverBounce
            builder.RegisterModule<NeverBounceModule>();

            //PrioTickets
            builder.RegisterModule<PrioTicketsModule>();
            //MoulinRouge
            builder.RegisterModule<MoulinRougeModule>();
            //Gray
            builder.RegisterModule<GrayLineIcelandModule>();
            //Hotelbeds
            builder.RegisterModule<HotelBedsModule>();
            //Wirecard Payment
            builder.RegisterModule<WirecardPaymentModule>();

            // Riskified Payment
            builder.RegisterModule<RiskifiedPaymentModule>();

            //FareHarbor
            builder.RegisterModule<FareHarborModule>();

            //AotModule
            builder.RegisterModule<AotModule>();

            // PriceRuleEngine
            builder.RegisterModule<PriceRuleEngineModule>();

            // DiscountRuleEngine
            builder.RegisterModule<DiscountRuleEngineModule>();

            // Mailing
            builder.RegisterModule<MailerModule>();

            //Tiqets
            builder.RegisterModule<TiqetsModule>();

            //Apexx
            builder.RegisterModule<ApexxModule>();

            builder.RegisterModule<BigBusModule>();
            //webjob

            // GoldenTours
            builder.RegisterModule<GoldenToursModule>();

            //HB
            builder.RegisterModule<HbModule>();

            // Redeam
            builder.RegisterModule<RedeamModule>();

            // Google Maps
            builder.RegisterModule<GoogleMapsModule>();

            builder.RegisterModule<TableStorageModule>();

            //Rezdy
            builder.RegisterModule<RezdyModule>();
            //GlobalTix
            builder.RegisterModule<GlobalTixModule>();
            builder.RegisterModule<GlobalTixModuleV3>();

            //Application Module
            builder.RegisterModule<ApplicationModule>();

            //Adyen
            builder.RegisterModule<AdyenModule>();

            //Ventrata
            builder.RegisterModule<VentrataModule>();

            //TOUR CMS
            builder.RegisterModule<TourCMSModule>();

            //NewCity SightSeeing
            builder.RegisterModule<NewCitySightSeeingModule>();

            //GoCity
            builder.RegisterModule<GoCityModule>();

            //NewPrio
            builder.RegisterModule<PrioHubModule>();

            //Rayna
            builder.RegisterModule<RaynaModule>();

            builder.RegisterModule<CitySightSeeingModule>();
            builder.RegisterModule<RedeamV12Module>();

        }
    }
}