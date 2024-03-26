using Autofac;
using ServiceAdapters.Aot.Aot.Commands;
using ServiceAdapters.Aot.Aot.Commands.Contracts;
using ServiceAdapters.Aot.Aot.Converters;
using ServiceAdapters.Aot.Aot.Converters.Contracts;

namespace ServiceAdapters.Aot
{
    public class AotModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CancelEntireBookingCommandHandler>()
                .As<ICancelEntireBookingCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<CancelSingleServiceBookingCommandHandler>()
                .As<ICancelSingleServiceBookingCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<CommandHandlerBase>()
                .As<ICommandHandlerBase>().InstancePerLifetimeScope();
            builder.RegisterType<CreateBookingCommandHandler>()
                .As<ICreateBookingCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<GetBookingCommandHandler>()
                .As<IGetBookingCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<GetBookingListCommandHandler>()
                .As<IGetBookingListCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<GetBulkPricingAvailabilityDetailsCommandHandler>()
                .As<IGetBulkPricingAvailabilityDetailsCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<GetDetailedPricingAvailabilityCommandHandler>()
                .As<IGetDetailedPricingAvailabilityCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<GetLocationCommandHandler>()
                .As<IGetLocationCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<GetProductDetailsCommandHandler>()
                .As<IGetProductDetailsCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<GetSupplierCommandHandler>()
                .As<IGetSupplierCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<UpdateBookingCommandHandler>()
                .As<IUpdateBookingCommandHandler>().InstancePerLifetimeScope();

            builder.RegisterType<CancelEntireBookingConverter>()
                .As<ICancelEntireBookingConverter>().InstancePerLifetimeScope();
            builder.RegisterType<CancelSingleServiceBookingConverter>()
                .As<ICancelSingleServiceBookingConverter>().InstancePerLifetimeScope();
            builder.RegisterType<ConverterBase>()
                .As<IConverterBase>().InstancePerLifetimeScope();
            builder.RegisterType<CreateBookingConverter>()
                .As<ICreateBookingConverter>().InstancePerLifetimeScope();
            builder.RegisterType<GetBookingConverter>()
                .As<IGetBookingConverter>().InstancePerLifetimeScope();
            builder.RegisterType<GetBookingListConverter>()
                .As<IGetBookingListConverter>().InstancePerLifetimeScope();
            builder.RegisterType<GetBulkPricingAvailabilityDetailsConverter>()
                .As<IGetBulkPricingAvailabilityDetailsConverter>().InstancePerLifetimeScope();
            builder.RegisterType<GetDetailedPricingAvailabilityConverter>()
                .As<IGetDetailedPricingAvailabilityConverter>().InstancePerLifetimeScope();
            builder.RegisterType<GetLocationConverter>()
                .As<IGetLocationConverter>().InstancePerLifetimeScope();
            builder.RegisterType<GetProductDetailsConverter>()
                .As<IGetProductDetailsConverter>().InstancePerLifetimeScope();
            builder.RegisterType<GetSupplierConverter>()
                .As<IGetSupplierConverter>().InstancePerLifetimeScope();
            builder.RegisterType<UpdateBookingConverter>()
                .As<IUpdateBookingConverter>().InstancePerLifetimeScope();
            //            builder.RegisterType<CountryType>().As<CountryType>().InstancePerLifetimeScope();
        }
    }
}