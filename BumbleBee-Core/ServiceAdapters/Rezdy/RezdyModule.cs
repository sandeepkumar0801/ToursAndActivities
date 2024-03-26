using Autofac;

using ServiceAdapters.Rezdy.Rezdy.Commands;
using ServiceAdapters.Rezdy.Rezdy.Commands.Contracts;
using ServiceAdapters.Rezdy.Rezdy.Converters;
using ServiceAdapters.Rezdy.Rezdy.Converters.Contracts;

namespace ServiceAdapters.Rezdy
{
    public class RezdyModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            #region CommandHandlers

            builder.RegisterType<GetProductCommandHandler>()
                 .As<IGetProductCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<GetAvailabilityCommandHandler>()
              .As<IGetAvailabilityCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<CreateBookingCommandHandler>()
              .As<ICreateBookingCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<CancelBookingCommandHandler>()
              .As<ICancelBookingCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<GetAllProductsCommandHandler>()
              .As<IGetAllProductsCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<GetPickUpLocationCommandHandler>()
                .As<IGetPickUpLocationCommandHandler>().InstancePerLifetimeScope();

            #endregion CommandHandlers

            #region Converters

            builder.RegisterType<GetProductConverter>()
                  .As<IGetProductConverter>().InstancePerLifetimeScope();
            builder.RegisterType<GetAvailabilityConverter>()
              .As<IGetAvailabilityConverter>().InstancePerLifetimeScope();
            builder.RegisterType<CreateBookingConverter>()
              .As<ICreateBookingConverter>().InstancePerLifetimeScope();
            builder.RegisterType<CancelBookingConverter>()
              .As<ICancelBookingConverter>().InstancePerLifetimeScope();
            builder.RegisterType<GetAllProductsConverter>()
                .As<IGetAllProductsConverter>().InstancePerLifetimeScope();
            builder.RegisterType<GetPickUpLocationConverter>()
                .As<IGetPickUpLocationConverter>().InstancePerLifetimeScope();

            #endregion Converters

            #region Adapter

            builder.RegisterType<RezdyAdapter>()
                  .As<IRezdyAdapter>().InstancePerLifetimeScope();

            #endregion Adapter
        }
    }
}