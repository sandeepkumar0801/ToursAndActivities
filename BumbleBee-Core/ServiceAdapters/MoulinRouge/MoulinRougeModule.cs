using Autofac;
using ServiceAdapters.MoulinRouge.MoulinRouge.Commands;
using ServiceAdapters.MoulinRouge.MoulinRouge.Commands.Contracts;
using ServiceAdapters.MoulinRouge.MoulinRouge.Converters;
using ServiceAdapters.MoulinRouge.MoulinRouge.Converters.Contracts;

namespace ServiceAdapters.MoulinRouge
{
    public class MoulinRougeModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AddToBasketCommandHandler>().As<IAddToBasketCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<AllocSeatsAutomaticCommandHandler>().As<IAllocSeatsAutomaticCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<CatalogDateGetDetailMultiCommandHandler>().As<ICatalogDateGetDetailMultiCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<CatalogDateGetListCommandHandler>().As<ICatalogDateGetListCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<CreateAccountCommandHandler>().As<ICreateAccountCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<GetOrderEticketCommandHandler>().As<IGetOrderEticketCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<OrderConfirmCommandHandler>().As<IOrderConfirmCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<ReleaseSeatsCommandHandler>().As<IReleaseSeatsCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<TempOrderGetDetailCommandHandler>().As<ITempOrderGetDetailCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<TempOrderGetSendingFeesCommandHandler>().As<ITempOrderGetSendingFeesCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<TempOrderSetSendingFeesCommandHandler>().As<ITempOrderSetSendingFeesCommandHandler>().InstancePerLifetimeScope();

            builder.RegisterType<AddToCartConverter>().As<IAddToBasketConverter>().InstancePerLifetimeScope();
            builder.RegisterType<AllocSeatsAutomaticConverter>().As<IAllocSeatsAutomaticConverter>().InstancePerLifetimeScope();
            builder.RegisterType<CatalogDateGetDetailMultiConverter>().As<ICatalogDateGetDetailMultiConverter>().InstancePerLifetimeScope();
            builder.RegisterType<CatalogDateGetListConverter>().As<ICatalogDateGetListConverter>().InstancePerLifetimeScope();
            builder.RegisterType<DateAndPriceConverter>().As<IDateAndPriceConverter>().InstancePerLifetimeScope();
            builder.RegisterType<TempOrderGetDetailConverter>().As<ITempOrderGetDetailConverter>().InstancePerLifetimeScope();
        }
    }
}