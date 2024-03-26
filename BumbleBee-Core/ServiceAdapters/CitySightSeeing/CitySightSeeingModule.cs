using Autofac;
using ServiceAdapters.CitySightSeeing.CitySightSeeing.Commands;
using ServiceAdapters.CitySightSeeing.CitySightSeeing.Commands.Contracts;
using ServiceAdapters.CitySightSeeing.CitySightSeeing.Converters;
using ServiceAdapters.CitySightSeeing.CitySightSeeing.Converters.Contracts;

namespace ServiceAdapters.CitySightSeeing
{
    public class CitySightSeeingModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ProductsCmdHandler>().As<IProductsCommandHandler>()
                .InstancePerLifetimeScope();
            builder.RegisterType<ProductsConverter>().As<IProductsConverter>().InstancePerDependency();


            builder.RegisterType<CreateBookingCommandHandler>().As<ICreateBookingCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<BookingConverter>().As<IBookingConverter>().InstancePerDependency();

            builder.RegisterType<ReservationCmdHandler>().As<IreservationCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<ReservationConverter>().As<IReservationConverter>().InstancePerLifetimeScope();

            builder.RegisterType<CancellationCommandHandler>().As<ICancellationCommandHandler>().InstancePerLifetimeScope();

            builder.RegisterType<RedemptionCmmdHandler>().As<IRedemptionCmmdHandler>().InstancePerLifetimeScope();



        }
    }
}
