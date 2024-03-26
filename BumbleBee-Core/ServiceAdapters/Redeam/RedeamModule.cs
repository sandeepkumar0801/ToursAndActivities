using Autofac;

using ServiceAdapters.Redeam.Redeam.Commands;
using ServiceAdapters.Redeam.Redeam.Commands.Contracts;
using ServiceAdapters.Redeam.Redeam.Converters;
using ServiceAdapters.Redeam.Redeam.Converters.Contracts;

namespace ServiceAdapters.Redeam
{
    public class RedeamModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            #region CommandHandlers

            builder.RegisterType<GetAvailabilitiesCommandHandler>()
                .As<IGetAvailabilitiesCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<GetRateCommandHandler>()
                .As<IGetRateCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<GetRatesCommandHandler>()
                .As<IGetRatesCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<GetAvailabilityCommandHandler>()
                .As<IGetAvailabilityCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<GetSuppliersCommandHandler>()
                .As<IGetSuppliersCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<GetProductsCommandHandler>()
                .As<IGetProductsCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<CreateHoldCommandHandler>()
                .As<ICreateHoldCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<DeleteHoldCommandHandler>()
                .As<IDeleteHoldCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<CreateBookingCommandHandler>()
                .As<ICreateBookingCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<CancelBookingCommandHandler>()
                .As<ICancelBookingCommandHandler>().InstancePerLifetimeScope();

            #endregion CommandHandlers

            #region Converters

            builder.RegisterType<GetAvailabilitiesConverter>()
                .As<IGetAvailabilitiesConverter>().InstancePerLifetimeScope();
            builder.RegisterType<GetSuppliersConverter>()
                .As<IGetSuppliersConverter>().InstancePerLifetimeScope();
            builder.RegisterType<GetProductsConverter>()
                .As<IGetProductsConverter>().InstancePerLifetimeScope();
            builder.RegisterType<GetRatesConverter>()
                .As<IGetRatesConverter>().InstancePerLifetimeScope();
            builder.RegisterType<CreateHoldConverter>()
                .As<ICreateHoldConverter>().InstancePerLifetimeScope();
            builder.RegisterType<CreateBookingConverter>()
                .As<ICreateBookingConverter>().InstancePerLifetimeScope();

            #endregion Converters

            #region Adapter

            builder.RegisterType<RedeamAdapter>()
                .As<IRedeamAdapter>().InstancePerLifetimeScope();

            #endregion Adapter
        }
    }
}