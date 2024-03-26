using Autofac;
using ServiceAdapters.HotelBeds.HotelBeds.Commands;
using ServiceAdapters.HotelBeds.HotelBeds.Commands.Contracts;
using ServiceAdapters.HotelBeds.HotelBeds.Converters;
using ServiceAdapters.HotelBeds.HotelBeds.Converters.Contracts;

namespace ServiceAdapters.HotelBeds
{
    public class HotelBedsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CommonCartCommandHandler>().As<ICommonCartCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<PurchaseCancelCommandHandler>().As<IPurchaseCancelCommandHandler>()
                .InstancePerLifetimeScope();

            builder.RegisterType<PurchaseDetailCommandHandler>().As<IPurchaseDetailCommandHandler>()
                .InstancePerLifetimeScope();
            builder.RegisterType<ServiceRemoveCommandHandler>().As<IServiceRemoveCommandHandler>()
                .InstancePerLifetimeScope();
            builder.RegisterType<TicketAvailabilityCmdHandler>().As<ITicketAvailabilityCmdHandler>()
                .InstancePerLifetimeScope();
            builder.RegisterType<TicketPurchaseConfirmCmdHandler>().As<ITicketPurchaseConfirmCmdHandler>()
                .InstancePerLifetimeScope();
            builder.RegisterType<TicketValuationCmdHandler>().As<ITicketValuationCmdHandler>()
                .InstancePerLifetimeScope();
            builder.RegisterType<CartConverter>().As<ICartConverter>().InstancePerLifetimeScope();
            builder.RegisterType<PurchaseConfirmConverter>().As<IPurchaseConfirmConverter>().InstancePerLifetimeScope();
            builder.RegisterType<PurchaseDetailsConverter>().As<IPurchaseDetailsConverter>().InstancePerLifetimeScope();
            builder.RegisterType<TicketAvailabilityConverter>().As<ITicketAvailabilityConverter>()
                .InstancePerLifetimeScope();
            builder.RegisterType<TicketValuationConverter>().As<ITicketValuationConverter>().InstancePerLifetimeScope();
        }
    }
}