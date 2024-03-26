using Autofac;
using ServiceAdapters.GoogleMaps.GoogleMaps.Commands;
using ServiceAdapters.GoogleMaps.GoogleMaps.Commands.Contracts;

namespace ServiceAdapters.GoogleMaps
{
    public class GoogleMapsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            #region CommandHandlers

            builder.RegisterType<AvailabilityFeedCommandHandler>().As<IAvailabilityFeedCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<MerchantFeedCommandHandler>().As<IMerchantFeedCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<ServiceFeedCommandHandler>().As<IServiceFeedCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<OrderNotificationCommandHandler>()
                .As<IOrderNotificationCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<InventoryRealTimeUpdateCommandHandler>().As<IInventoryRealTimeUpdateCommandHandler>().InstancePerLifetimeScope();

            #endregion CommandHandlers
        }
    }
}
