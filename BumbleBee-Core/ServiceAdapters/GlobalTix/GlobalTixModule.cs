using Autofac;
using ServiceAdapters.GlobalTix.GlobalTix.Commands;
using ServiceAdapters.GlobalTix.GlobalTix.Commands.Contracts;
using ServiceAdapters.GlobalTix.GlobalTix.Converters;
using ServiceAdapters.GlobalTix.GlobalTix.Converters.Contracts;

namespace ServiceAdapters.GlobalTix
{
    public class GlobalTixModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Command Handlers
            builder.RegisterType<ActivityInfoCommandHandler>().As<IActivityInfoCommandHandler>().InstancePerDependency();
            builder.RegisterType<ActivityTicketTypeCommandHandler>().As<IActivityTicketTypeCommandHandler>().InstancePerDependency();
            builder.RegisterType<ActivityTicketTypesCommandHandler>().As<IActivityTicketTypesCommandHandler>().InstancePerDependency();
            builder.RegisterType<ActivityListCommandHandler>().As<IActivityListCommandHandler>().InstancePerDependency();
            builder.RegisterType<AuthenticationCommandHandler>().As<IAuthenticationCommandHandler>().InstancePerDependency();
            builder.RegisterType<CancelByTicketCommandHandler>().As<ICancelByTicketCommandHandler>().InstancePerDependency();
            builder.RegisterType<CancelByBookingCommandHandler>().As<ICancelByBookingCommandHandler>().InstancePerDependency();
            builder.RegisterType<CityListCommandHandler>().As<ICityListCommandHandler>().InstancePerDependency();
            builder.RegisterType<CountryListCommandHandler>().As<ICountryListCommandHandler>().InstancePerDependency();
            builder.RegisterType<CreateBookingCommandHandler>().As<ICreateBookingCommandHandler>().InstancePerDependency();
            builder.RegisterType<PackageInfoCommandHandler>().As<IPackageInfoCommandHandler>().InstancePerDependency();
            builder.RegisterType<PackageListCommandHandler>().As<IPackageListCommandHandler>().InstancePerDependency();

            // Converters
            builder.RegisterType<ActivityEntityConverter>().As<IActivityEntityConverter>().InstancePerDependency();
            builder.RegisterType<ActivityInfoConverter>().As<IActivityInfoConverter>().InstancePerDependency();
            builder.RegisterType<ActivityListConverter>().As<IActivityListConverter>().InstancePerDependency();
            builder.RegisterType<BookingConverter>().As<IBookingConverter>().InstancePerDependency();
            builder.RegisterType<PackageEntityConverter>().As<IPackageEntityConverter>().InstancePerDependency();
            builder.RegisterType<PackageInfoConverter>().As<IPackageInfoConverter>().InstancePerDependency();
        }
    }
}