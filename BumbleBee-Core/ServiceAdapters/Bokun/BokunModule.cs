using Autofac;
using ServiceAdapters.Bokun.Bokun.Commands;
using ServiceAdapters.Bokun.Bokun.Commands.Contracts;
using ServiceAdapters.Bokun.Bokun.Converters;
using ServiceAdapters.Bokun.Bokun.Converters.Contracts;

namespace ServiceAdapters.Bokun
{
    public class BokunModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CancelBookingCommandHandler>()
                .As<ICancelBookingCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<CheckAvailabilitiesCommandHandler>()
                .As<ICheckAvailabilitiesCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<CheckoutOptionsCommandHandler>()
                .As<ICheckoutOptionsCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<EditBookingCommandHandler>()
                .As<IEditBookingCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<GetActivityCommandHandler>()
                .As<IGetActivityCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<GetBookingCommandHandler>()
                .As<IGetBookingCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<GetActivityCommandHandler>()
                .As<IGetActivityCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<SubmitCheckoutCommandHandler>()
                .As<ISubmitCheckoutCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<GetPickupPlacesCommandHandler>().As<IGetPickupPlacesCommandHandler>()
                .InstancePerLifetimeScope();

            builder.RegisterType<CancelBookingConverter>()
                .As<ICancelBookingConverter>().InstancePerLifetimeScope();
            builder.RegisterType<CheckAvailabilitiesConverter>()
                .As<ICheckAvailabilitiesConverter>().InstancePerLifetimeScope();
            builder.RegisterType<CheckoutOptionsConverter>()
                .As<ICheckoutOptionsConverter>().InstancePerLifetimeScope();
            builder.RegisterType<EditBookingConverter>()
                .As<IEditBookingConverter>().InstancePerLifetimeScope();
            builder.RegisterType<GetActivityConverter>()
                .As<IGetActivityConverter>().InstancePerLifetimeScope();
            builder.RegisterType<GetBookingConverter>()
                .As<IGetBookingConverter>().InstancePerLifetimeScope();
            builder.RegisterType<SubmitCheckoutConverter>()
                .As<ISubmitCheckoutConverter>().InstancePerLifetimeScope();
            builder.RegisterType<GetPickupPlacesConverter>().As<IGetPickupPlacesConverter>().InstancePerLifetimeScope();
        }
    }
}