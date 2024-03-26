using Autofac;
using ServiceAdapters.Ventrata.Ventrata.Commands;
using ServiceAdapters.Ventrata.Ventrata.Commands.Contracts;
using ServiceAdapters.Ventrata.Ventrata.Converters;
using ServiceAdapters.Ventrata.Ventrata.Converters.Contracts;

namespace ServiceAdapters.Ventrata
{
    public class VentrataModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Command Handlers
            builder.RegisterType<AvailabilityCommandHandler>().As<IAvailabilityCommandHandler>().InstancePerDependency();
            builder.RegisterType<BookingReservationCommandHandler>().As<IBookingReservationCommandHandler>().InstancePerDependency();
            builder.RegisterType<BookingAndReservationCancellationCommandHandler>().As<IBookingAndReservationCancellationCommandHandler>().InstancePerDependency();
            builder.RegisterType<BookingConfirmationCommandHandler>().As<IBookingConfirmationCommandHandler>().InstancePerDependency();
            builder.RegisterType<GetAllProductsCommandHandler>().As<IGetAllProductsCommandHandler>().InstancePerDependency();
            builder.RegisterType<BookingConfirmationConverter>().As<IBookingConfirmationConverter>().InstancePerDependency();
            builder.RegisterType<AvailabilityConverter>().As<IAvailabilityConverter>().InstancePerDependency();
            builder.RegisterType<PackageReservationCommandHandler>().As<IPackageReservationCommandHandler>().InstancePerDependency();

            builder.RegisterType<CustomQuestionsCommandHandler>().As<ICustomQuestionsCommandHandler>().InstancePerDependency();
            builder.RegisterType<CustomQuestionsConverter>().As<ICustomQuestionsConverter>().InstancePerDependency();
        }
    }
}
