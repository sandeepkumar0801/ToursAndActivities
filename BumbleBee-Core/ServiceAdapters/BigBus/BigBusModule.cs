using Autofac;
using ServiceAdapters.BigBus.BigBus.Commands;
using ServiceAdapters.BigBus.BigBus.Commands.Contracts;
using ServiceAdapters.BigBus.BigBus.Converters;
using ServiceAdapters.BigBus.BigBus.Converters.Contracts;

namespace ServiceAdapters.BigBus
{
    public class BigBusModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CreateBookingCommandHandler>().As<ICreateBookingCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<CancelBookingCommandHandler>().As<ICancelBookingCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<CreateReservationCommandHandler>().As<ICreateReservationCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<CancelReservationCommandHandler>().As<ICancelReservationCommandHandler>().InstancePerLifetimeScope();

            builder.RegisterType<CreateBookingConverter>().As<ICreateBookingConverter>().InstancePerLifetimeScope();
            builder.RegisterType<CancelBookingConverter>().As<ICancelBookingConverter>().InstancePerLifetimeScope();
            builder.RegisterType<CreateReservationConverter>().As<ICreateReservationConverter>().InstancePerLifetimeScope();
            builder.RegisterType<CancelReservationConverter>().As<ICancelReservationConverter>().InstancePerLifetimeScope();
        }
    }
}