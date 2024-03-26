using Autofac;
using Logger.Contract;
using ServiceAdapters.Adyen;
using ServiceAdapters.AlternativePayment;
using ServiceAdapters.Aot;
using ServiceAdapters.Apexx;
using ServiceAdapters.BigBus;
using ServiceAdapters.Bokun;
using ServiceAdapters.FareHarbor;
using ServiceAdapters.GlobalTix;
using ServiceAdapters.GoogleMaps;
using ServiceAdapters.GrayLineIceLand;
using ServiceAdapters.HB;
using ServiceAdapters.HotelBeds;
using ServiceAdapters.MoulinRouge;
using ServiceAdapters.NeverBounce;
using ServiceAdapters.PrioTicket;
using ServiceAdapters.Redeam;
using ServiceAdapters.Rezdy;

//using ServiceAdapters.Seatings;
//using ServiceAdapters.SEMrush;
using ServiceAdapters.SightSeeing;
using ServiceAdapters.Tiqets;
using ServiceAdapters.WirecardPayment;
using ServiceAdapters.Ventrata;
using ServiceAdapters.TourCMS;

using ServiceAdapters.NewCitySightSeeing;
using ServiceAdapters.GoCity;
using ServiceAdapters.Rayna;
using ServiceAdapters.PrioHub;
using ServiceAdapters.CitySightSeeing;
using ServiceAdapters.CitySightSeeing.CitySightSeeing.Commands.Contracts;
using ServiceAdapters.GlobalTixV3;

namespace ServiceAdapters
{
    public class AdapterModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<SightSeeingAdapter>().As<ISightSeeingAdapter>().InstancePerDependency();
            builder.RegisterType<GrayLineIceLandAdapter>().As<IGrayLineIceLandAdapter>().InstancePerDependency();
            //builder.RegisterType<HotelBedsAdapter>().As<IHotelBedsAdapter>().InstancePerDependency();
            builder.RegisterType<NeverBounceAdapter>().As<INeverBounceAdapter>().InstancePerDependency();
            builder.RegisterType<MoulinRougeAdapter>().As<IMoulinRougeAdapter>().InstancePerDependency();
            builder.RegisterType<PrioAdapter>().As<IPrioTicketAdapter>().InstancePerDependency();
            builder.RegisterType<VentrataAdapter>().As<IVentrataAdapter>().InstancePerDependency();
            //builder.RegisterType<SEMrushAdapter>().As<ISEMrushAdapter>().InstancePerDependency();
            builder.RegisterType<AlternativePaymentAdapter>().As<IAlternativePaymentAdapter>().InstancePerDependency();
            builder.RegisterType<HBAdapter>().As<IHBAdapter>().InstancePerDependency();
            //builder.RegisterType<SeatingsAdapter>().As<ISeatingsAdapter>().InstancePerDependency();
            builder.RegisterType<BokunAdapter>().As<IBokunAdapter>().InstancePerDependency();
            builder.RegisterType<TicketsAdapter>().As<ITicketAdapter>().InstancePerDependency();
            //builder.RegisterType<EncoreAdapter>().As<IEncoreAdapter>().InstancePerDependency();
            builder.RegisterType<WirecardPaymentAdapter>().As<IWirecardPaymentAdapter>().InstancePerDependency();
            builder.RegisterType<AotAdapter>().As<IAotAdapter>().InstancePerDependency();
            builder.RegisterType<FareHarborAdapter>().As<IFareHarborAdapter>().InstancePerDependency();
            builder.RegisterType<TiqetsAdapter>().As<ITiqetsAdapter>().InstancePerDependency();
            builder.RegisterType<ApexxAdapter>().As<IApexxAdapter>().InstancePerDependency();
            builder.RegisterType<BigBusAdapter>().As<IBigBusAdapter>().InstancePerDependency();
            builder.RegisterType<RedeamAdapter>().As<IRedeamAdapter>().InstancePerDependency();
            //builder.RegisterType<Log>().As<ILog>().InstancePerDependency();
            builder.RegisterType<Logger.Logger>().As<ILogger>().InstancePerDependency();
            builder.RegisterType<HBAdapter>().As<IHBAdapter>().InstancePerDependency();
            builder.RegisterType<GoogleMapsAdapter>().
                As<IGoogleMapsAdapter>().InstancePerDependency();
            builder.RegisterType<RezdyAdapter>().As<IRezdyAdapter>().InstancePerLifetimeScope();
            builder.RegisterType<GlobalTixAdapter>().As<IGlobalTixAdapter>().InstancePerLifetimeScope();
            builder.RegisterType<AdyenAdapter>().As<IAdyenAdapter>().InstancePerLifetimeScope();
            builder.RegisterType<TourCMSAdapter>().As<ITourCMSAdapter>().InstancePerLifetimeScope();
            builder.RegisterType<NewCitySightSeeingAdapter>().As<INewCitySightSeeingAdapter>().InstancePerLifetimeScope();
            builder.RegisterType<GoCityAdapter>().As<IGoCityAdapter>().InstancePerLifetimeScope();
            builder.RegisterType<PrioHubAdapter>().As<IPrioHubAdapter>().InstancePerLifetimeScope();
            builder.RegisterType<RaynaAdapter>().As<IRaynaAdapter>().InstancePerLifetimeScope();
            builder.RegisterType<GlobalTixAdapterV3>().As<IGlobalTixAdapterV3>().InstancePerLifetimeScope();
            builder.RegisterType<CitySightSeeingAdapter>().As<ICitySightSeeingAdapter>().InstancePerLifetimeScope();

        }
    }
}