using Autofac;
using Isango.Entities.Enums;
using Isango.Service.Canocalization;
using Isango.Service.ConsoleApplication;
using Isango.Service.ConsoleApplication.CriteriaHandlers;
using Isango.Service.Contract;
using Isango.Service.Factory;
using Isango.Service.PaymentServices;
using Isango.Service.PriceRuleEngine;
using Isango.Service.SupplierServices;
using ServiceAdapters.GlobalTixV3.GlobalTixV3.Commands;
using ServiceAdapters.GlobalTixV3.GlobalTixV3.Commands.Contracts;

namespace Isango.Service
{
    public class ServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ActivityService>().As<IActivityService>().InstancePerDependency();
            builder.RegisterType<AffiliateService>().As<IAffiliateService>().InstancePerLifetimeScope();
            builder.RegisterType<MasterService>().As<IMasterService>().InstancePerDependency();
            builder.RegisterType<LandingService>().As<ILandingService>().InstancePerLifetimeScope();
            builder.RegisterType<ManageIdentityService>().As<IManageIdentityService>().InstancePerLifetimeScope();
            builder.RegisterType<CartService>().As<ICartService>().InstancePerDependency();
            builder.RegisterType<ReviewService>().As<IReviewService>().InstancePerLifetimeScope();
            builder.RegisterType<CacheLoaderService>().As<ICacheLoaderService>().InstancePerLifetimeScope();
            builder.RegisterType<AlternativePaymentService>().As<IAlternativePaymentService>().InstancePerLifetimeScope();
            builder.RegisterType<SynchronizerService>().As<ISynchronizerService>().InstancePerLifetimeScope();
            builder.RegisterType<PriceRuleEngineService>().As<IPriceRuleEngineService>().InstancePerLifetimeScope();
            builder.RegisterType<ProfileService>().As<IProfileService>().InstancePerLifetimeScope();
            builder.RegisterType<VoucherService>().As<IVoucherService>().InstancePerLifetimeScope();
            builder.RegisterType<SupplierBookingService>().As<ISupplierBookingService>().InstancePerLifetimeScope();
            builder.RegisterType<BookingService>().As<IBookingService>().InstancePerLifetimeScope();
            builder.RegisterType<ServiceAvailabilityService>().As<IServiceAvailabilityService>().InstancePerLifetimeScope();
            builder.RegisterType<AgeGroupService>().As<IAgeGroupService>().InstancePerLifetimeScope();
            builder.RegisterType<GrayLineIceLandCriteriaService>().As<IGrayLineIceLandCriteriaService>().InstancePerDependency();
            builder.RegisterType<PrioCriteriaService>().As<IPrioCriteriaService>().InstancePerDependency();
            builder.RegisterType<MoulinRougeCriteriaService>().As<IMoulinRougeCriteriaService>().InstancePerDependency();
            builder.RegisterType<FareHarborCriteriaService>().As<IFareHarborCriteriaService>().InstancePerDependency();
            builder.RegisterType<BokunCriteriaService>().As<IBokunCriteriaService>().InstancePerDependency();
            builder.RegisterType<AotCriteriaService>().As<IAotCriteriaService>().InstancePerDependency();
            builder.RegisterType<MailerService>().As<IMailerService>().InstancePerDependency();
            builder.RegisterType<TiqetsCriteriaService>().As<ITiqetsCriteriaService>().InstancePerDependency();
            builder.RegisterType<GoldenToursCriteriaService>().As<IGoldenToursCriteriaService>().InstancePerDependency();
            builder.RegisterType<VentrataCriteriaService>().As<IVentrataCriteriaService>().InstancePerDependency();
            builder.RegisterType<StorageOperationService>().As<IStorageOperationService>().InstancePerDependency();
            builder.RegisterType<CloudinaryService>().As<ICloudinaryService>().InstancePerDependency();

            builder.RegisterType<PaymentGatewayFactory>().InstancePerLifetimeScope();
            builder.RegisterType<SupplierFactory>().InstancePerLifetimeScope();

            builder.RegisterType<AotService>().Keyed<ISupplierService>(APIType.Aot).InstancePerLifetimeScope();
            builder.RegisterType<BokunService>().Keyed<ISupplierService>(APIType.Bokun).InstancePerLifetimeScope();
            builder.RegisterType<FareHarborService>().Keyed<ISupplierService>(APIType.Fareharbor).InstancePerLifetimeScope();
            builder.RegisterType<GoldenToursService>().Keyed<ISupplierService>(APIType.Goldentours).InstancePerLifetimeScope();
            builder.RegisterType<GrayLineIceLandService>().Keyed<ISupplierService>(APIType.Graylineiceland).InstancePerLifetimeScope();
            builder.RegisterType<MoulinRougeService>().Keyed<ISupplierService>(APIType.Moulinrouge).InstancePerLifetimeScope();
            builder.RegisterType<PrioService>().Keyed<ISupplierService>(APIType.Prio).InstancePerLifetimeScope();
            builder.RegisterType<TiqetsService>().Keyed<ISupplierService>(APIType.Tiqets).InstancePerLifetimeScope();
            builder.RegisterType<HBApitudeService>().Keyed<ISupplierService>(APIType.Hotelbeds).InstancePerLifetimeScope();
            builder.RegisterType<RedeamService>().Keyed<ISupplierService>(APIType.Redeam).InstancePerLifetimeScope();
            builder.RegisterType<GlobalTixService>().Keyed<ISupplierService>(APIType.GlobalTix).InstancePerLifetimeScope();
            builder.RegisterType<VentrataService>().Keyed<ISupplierService>(APIType.Ventrata).InstancePerLifetimeScope();


            builder.RegisterType<ApexxPaymentService>().Keyed<IPaymentService>(PaymentGatewayType.Apexx).InstancePerLifetimeScope();
            builder.RegisterType<WirecardPaymentService>().Keyed<IPaymentService>(PaymentGatewayType.WireCard).InstancePerLifetimeScope();

            builder.RegisterType<GoogleMapsDataDumpingService>().As<IGoogleMapsDataDumpingService>().InstancePerLifetimeScope();
            builder.RegisterType<GoogleMapsService>().As<IGoogleMapsService>().InstancePerLifetimeScope();

            //##Merging Check -- need to do as per rest of the api
            builder.RegisterType<ApiTudeCriteriaService>().As<IApiTudeCriteriaService>().InstancePerDependency();
            builder.RegisterType<RedeamCriteriaService>().As<IRedeamCriteriaService>().InstancePerDependency();
            builder.RegisterType<PaymentGatewayFactory>().InstancePerLifetimeScope();

            builder.RegisterType<AsyncBookingService>().As<IAsyncBookingService>().InstancePerDependency();
            builder.RegisterType<RiskifiedService>().As<IRiskifiedService>().InstancePerLifetimeScope();

            builder.RegisterType<CancellationService>().As<ICancellationService>().InstancePerLifetimeScope();
            builder.RegisterType<RezdyCriteriaService>().As<IRezdyCriteriaService>().InstancePerLifetimeScope();
            builder.RegisterType<RezdyService>().Keyed<ISupplierService>(APIType.Rezdy).InstancePerLifetimeScope();
            builder.RegisterType<ActivityDeltaService>().As<IActivityDeltaService>().InstancePerLifetimeScope();
            builder.RegisterType<UnusedActivityService>().As<IUnusedActivityService>().InstancePerLifetimeScope();
            builder.RegisterType<SearchService>().As<ISearchService>().InstancePerLifetimeScope();
            builder.RegisterType<GlobalTixCriteriaService>().As<IGlobalTixCriteriaService>().InstancePerLifetimeScope();
            builder.RegisterType<ApplicationService>().As<IApplicationService>().InstancePerLifetimeScope();
            builder.RegisterType<AdyenPaymentService>().Keyed<IPaymentService>(PaymentGatewayType.Adyen).InstancePerLifetimeScope();
            builder.RegisterType<TourCMSCriteriaService>().As<ITourCMSCriteriaService>().InstancePerLifetimeScope();
            builder.RegisterType<TourCMSService>().Keyed<ISupplierService>(APIType.TourCMS).InstancePerLifetimeScope();

            builder.RegisterType<NewCitySightSeeingCriteriaService>().As<INewCitySightSeeingCriteriaService>().InstancePerLifetimeScope();
            builder.RegisterType<NewCitySightSeeingService>().Keyed<ISupplierService>(APIType.NewCitySightSeeing).InstancePerLifetimeScope();

            builder.RegisterType<GoCityCriteriaService>().As<IGoCityCriteriaService>().InstancePerLifetimeScope();
            builder.RegisterType<GoCityService>().Keyed<ISupplierService>(APIType.GoCity).InstancePerLifetimeScope();

            builder.RegisterType<RaynaCriteriaService>().As<IRaynaCriteriaService>().InstancePerLifetimeScope();
            builder.RegisterType<RaynaService>().Keyed<ISupplierService>(APIType.Rayna).InstancePerLifetimeScope();
            builder.RegisterType<PrioHubCriteriaService>().As<IPrioHubCriteriaService>().InstancePerDependency();
            builder.RegisterType<PrioHubService>().Keyed<ISupplierService>(APIType.PrioHub).InstancePerLifetimeScope();
            builder.RegisterType<CssBookingService>().As<ICssBookingService>().InstancePerDependency();
            builder.RegisterType<CanocalizationService>().As<ICanocalizationService>().InstancePerDependency();
            builder.RegisterType<RedemptionService>().As<IRedemptionService>().InstancePerDependency();

        }
    }
}