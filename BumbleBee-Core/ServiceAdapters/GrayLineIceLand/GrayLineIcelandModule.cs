using Autofac;
using ServiceAdapters.GrayLineIceLand.GrayLineIceLand.Commands;
using ServiceAdapters.GrayLineIceLand.GrayLineIceLand.Commands.Contracts;
using ServiceAdapters.GrayLineIceLand.GrayLineIceLand.Converters;
using ServiceAdapters.GrayLineIceLand.GrayLineIceLand.Converters.Contracts;

namespace ServiceAdapters.GrayLineIceLand
{
    public class GrayLineIcelandModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CancelBookingCommandHandler>().As<ICancelBookingCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<CreateBookingCmdHandler>().As<ICreateBookingCmdHandler>().InstancePerLifetimeScope();
            builder.RegisterType<GetAvailabilityCommandHandler>().As<IGetAvailabilityCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<AuthenticationCmdHandler>().As<IAuthenticationCmdhandler>().InstancePerLifetimeScope();
            builder.RegisterType<GetAgeGroupsCommandHandler>().As<IGetAgeGroupsCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<GetPickupLocationsCommandHandler>().As<IGetPickupLocationsCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<AvailabilityConverter>().As<IAvailabilityConverter>().InstancePerLifetimeScope();
            builder.RegisterType<BookingConverter>().As<IBookingConverter>().InstancePerLifetimeScope();
        }
    }
}