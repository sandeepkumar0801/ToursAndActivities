using Autofac;
using Isango.Persistence.ConsoleApplication.AgeGroup;
using Isango.Persistence.ConsoleApplication.ServiceAvailability;
using Isango.Persistence.Contract;
using Isango.Persistence.Mailer;
using Isango.Persistence.PriceRuleEngine;
using Logger.Contract;

namespace Isango.Persistence
{
    public class PersistenceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ActivityPersistence>().As<IActivityPersistence>().InstancePerLifetimeScope();
            builder.RegisterType<AffiliatePersistence>().As<IAffiliatePersistence>().InstancePerLifetimeScope();
            builder.RegisterType<MasterPersistence>().As<IMasterPersistence>().InstancePerLifetimeScope();
            builder.RegisterType<LandingPersistence>().As<ILandingPersistence>().InstancePerLifetimeScope();
            builder.RegisterType<ManageIdentityPersistence>().As<IManageIdentityPersistence>().InstancePerLifetimeScope();
            builder.RegisterType<ReviewPersistence>().As<IReviewPersistence>().InstancePerLifetimeScope();
            //builder.RegisterType<HotelPersistence>().As<IHotelPersistence>().InstancePerLifetimeScope();
            builder.RegisterType<BookingPersistence>().As<IBookingPersistence>().InstancePerLifetimeScope();
            builder.RegisterType<PriceRuleEnginePersistence>().As<IPriceRuleEnginePersistence>().InstancePerLifetimeScope();
            builder.RegisterType<ProfilePersistence>().As<IProfilePersistence>().InstancePerLifetimeScope();
            builder.RegisterType<VoucherPersistence>().As<IVoucherPersistence>().InstancePerLifetimeScope();
            builder.RegisterType<MailRuleEnginePersistence>().As<IMailRuleEnginePersistence>().InstancePerLifetimeScope();
            builder.RegisterType<SupplierBookingPersistence>().As<ISupplierBookingPersistence>().InstancePerLifetimeScope();
            builder.RegisterType<AOTPersistence>().As<IAOTPersistence>().InstancePerLifetimeScope();
            builder.RegisterType<GrayLineIceLandPersistence>().As<IGrayLineIceLandPersistence>().InstancePerLifetimeScope();
            builder.RegisterType<FareHarborPersistence>().As<IFareHarborPersistence>().InstancePerLifetimeScope();
            builder.RegisterType<PrioPersistence>().As<IPrioPersistence>().InstancePerLifetimeScope();
            builder.RegisterType<ServiceAvailabilityPersistence>().As<IServiceAvailabilityPersistence>().InstancePerLifetimeScope();
            builder.RegisterType<AlternativePaymentPersistence>().As<IAlternativePaymentPersistence>().InstancePerLifetimeScope();
            builder.RegisterType<TiqetsPersistence>().As<ITiqetsPersistence>().InstancePerLifetimeScope();
            builder.RegisterType<GoldenToursPersistence>().As<IGoldenToursPersistence>().InstancePerLifetimeScope();
            builder.RegisterType<ApiTudePersistence>().As<IApiTudePersistence>().InstancePerLifetimeScope();
            builder.RegisterType<RedeamPersistence>().As<IRedeamPersistence>().InstancePerLifetimeScope();
            builder.RegisterType<GoogleMapsPersistence>().As<IGoogleMapsPersistence>().InstancePerLifetimeScope();
            builder.RegisterType<BokunPersistence>().As<IBokunPersistence>().InstancePerLifetimeScope();
            builder.RegisterType<CancellationPersistence>().As<ICancellationPersistence>().InstancePerLifetimeScope();
            builder.RegisterType<ActivityDeltaPersistence>().As<IActivityDeltaPersistence>().InstancePerLifetimeScope();
            builder.RegisterType<RezdyPersistence>().As<IRezdyPersistence>().InstancePerLifetimeScope();
            builder.RegisterType<GlobalTixPersistence>().As<IGlobalTixPersistence>().InstancePerLifetimeScope();
            builder.RegisterType<AdyenPersistence>().As<IAdyenPersistence>().InstancePerLifetimeScope();
            builder.RegisterType<VentrataPersistence>().As<IVentrataPersistence>().InstancePerLifetimeScope();
            builder.RegisterType<Logger.Logger>().As<ILogger>().InstancePerDependency();
            builder.RegisterType<TourCMSPersistence>().As<ITourCMSPersistence>().InstancePerLifetimeScope();
            builder.RegisterType<NewCitySightSeeingPersistence>().As<INewCitySightSeeingPersistence>().InstancePerLifetimeScope();
            builder.RegisterType<GoCityPersistence>().As<IGoCityPersistence>().InstancePerLifetimeScope();
            builder.RegisterType<RaynaPersistence>().As<IRaynaPersistence>().InstancePerLifetimeScope();
            builder.RegisterType<NewPrioPersistence>().As<INewPrioPersistence>().InstancePerLifetimeScope();

            builder.RegisterType<RedemptionPersistance>().As<IRedemptionPersistance>().InstancePerLifetimeScope();

        }
    }
}