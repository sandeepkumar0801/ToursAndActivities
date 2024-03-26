using Autofac;
using ServiceAdapters.GoCity.GoCity.Commands;
using ServiceAdapters.GoCity.GoCity.Commands.Contract;
using ServiceAdapters.GoCity.GoCity.Converters;
using ServiceAdapters.GoCity.GoCity.Converters.Contracts;

namespace ServiceAdapters.GoCity
{
    public class GoCityModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ProductsCmdHandler>().As<IProductsCommandHandler>()
                .InstancePerLifetimeScope();

            builder.RegisterType<BookingCmdHandler>().As<IBookingCommandHandler>()
                .InstancePerLifetimeScope();

            builder.RegisterType<BookingConverter>().As<IBookingConverter>()
                .InstancePerLifetimeScope();

            builder.RegisterType<CancellationCmdHandler>().As<ICancellationCommandHandler>()
                           .InstancePerLifetimeScope();

            builder.RegisterType<GoCityCancellationConverter>().As<IGoCityCancellationConverter>()
                .InstancePerLifetimeScope();
        }
    }
}