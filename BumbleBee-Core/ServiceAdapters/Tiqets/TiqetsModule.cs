using Autofac;
using ServiceAdapters.Tiqets.Tiqets.Commands;
using ServiceAdapters.Tiqets.Tiqets.Commands.Contracts;
using ServiceAdapters.Tiqets.Tiqets.Converters;
using ServiceAdapters.Tiqets.Tiqets.Converters.Contracts;

namespace ServiceAdapters.Tiqets
{
    public class TiqetsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //Register Command Handlers
            builder.RegisterType<GetAvailabilityByIdCommandHandler>().As<IGetAvailabilityByIdCommandHandler>()
                .InstancePerLifetimeScope();

            builder.RegisterType<ConfirmOrderCommandHandler>().As<IConfirmOrderCommandHandler>()
                .InstancePerLifetimeScope();

            builder.RegisterType<CreateOrderCommandHandler>().As<ICreateOrderCommandHandler>()
                .InstancePerLifetimeScope();

            builder.RegisterType<GetVariantsCommandHandler>().As<IGetVariantsCommandHandler>()
                .InstancePerLifetimeScope();

            builder.RegisterType<GetAvailableDaysCommandHandler>().As<IGetAvailableDaysCommandHandler>()
                .InstancePerLifetimeScope();

            builder.RegisterType<GetAvailableTimeSlotsCommandHandler>().As<IGetAvailableTimeSlotsCommandHandler>()
                .InstancePerLifetimeScope();

            builder.RegisterType<GetTicketCommandHandler>().As<IGetTicketCommandHandler>()
                .InstancePerLifetimeScope();

            builder.RegisterType<GetProductDetailsCommandHandler>().As<IGetProductDetailsCommandHandler>()
                .InstancePerLifetimeScope();

            builder.RegisterType<GetBulkAvailabilityCommandHandler>().As<IGetBulkAvailabilityCommandHandler>()
                .InstancePerLifetimeScope();

            builder.RegisterType<GetBulkVariantsAvailabilityCommandHandler>().As<IGetBulkVariantsAvailabilityCommandHandler>()
                .InstancePerLifetimeScope();

            builder.RegisterType<OrderInfoCommandHandler>().As<IOrderInformationCmdHandler>()
                .InstancePerLifetimeScope();
            builder.RegisterType<CancelOrderCommandHandler>().As<ICancelOrderCommandHandler>()
                .InstancePerLifetimeScope();

            //Register Converters
            builder.RegisterType<AvailabilityConverter>().As<IAvailabilityConverter>()
                .InstancePerLifetimeScope();

            builder.RegisterType<BookingConverter>().As<IBookingConverter>()
                .InstancePerLifetimeScope();

            builder.RegisterType<GetProductFilterCommandHandler>().As<IGetProductFilterCommandHandler>()
               .InstancePerLifetimeScope();

            builder.RegisterType<TiqetsPackageCommandHandler>().As<ITiqetsPackageCommandHandler>()
             .InstancePerLifetimeScope();

        }
    }
}