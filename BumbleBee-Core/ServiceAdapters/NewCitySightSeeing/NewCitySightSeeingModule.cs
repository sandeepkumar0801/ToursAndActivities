using Autofac;
using ServiceAdapters.NewCitySightSeeing.NewCitySightSeeing.Commands;
using ServiceAdapters.NewCitySightSeeing.NewCitySightSeeing.Commands.Contract;
using ServiceAdapters.NewCitySightSeeing.NewCitySightSeeing.Converters;
using ServiceAdapters.NewCitySightSeeing.NewCitySightSeeing.Converters.Contracts;

namespace ServiceAdapters.NewCitySightSeeing
{
    public class NewCitySightSeeingModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ProductsCmdHandler>().As<IProductsCommandHandler>()
                .InstancePerLifetimeScope();

            builder.RegisterType<NewCitySightSeeingCalendarHandler>().As<INewCitySightSeeingCalendarHandler>()
                           .InstancePerLifetimeScope();
            builder.RegisterType<NewCitySightSeeingCalendarConverter>().As<INewCitySightSeeingCalendarConverter>()
                           .InstancePerLifetimeScope();

            builder.RegisterType<NewCitySightSeeingAvailabilityHandler>().As<INewCitySightSeeingAvailabilityHandler>()
                           .InstancePerLifetimeScope();
            builder.RegisterType<NewCitySightSeeingAvailabilityConverter>().As<INewCitySightSeeingAvailabilityConverter>()
                           .InstancePerLifetimeScope();

            builder.RegisterType<ReservationCmdHandler>().As<IReservationCommandHandler>()
                          .InstancePerLifetimeScope();
            builder.RegisterType<NewCitySightSeeingReservationConverter>().As<INewCitySightSeeingReservationConverter>()
                           .InstancePerLifetimeScope();

            builder.RegisterType<BookingCmdHandler>().As<IBookingCommandHandler>()
                          .InstancePerLifetimeScope();
            builder.RegisterType<NewCitySightSeeingBookingConverter>().As<INewCitySightSeeingBookingConverter>()
                           .InstancePerLifetimeScope();

            builder.RegisterType<CancellationCmdHandler>().As<ICancellationCommandHandler>()
                         .InstancePerLifetimeScope();
            builder.RegisterType<NewCitySightSeeingCancellationConverter>().As<INewCitySightSeeingCancellationConverter>()
                           .InstancePerLifetimeScope();

        }
    }
}