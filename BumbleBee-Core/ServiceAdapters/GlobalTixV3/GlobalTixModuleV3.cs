using Autofac;
using ServiceAdapters.GlobalTixV3.GlobalTixV3.Commands;
using ServiceAdapters.GlobalTixV3.GlobalTixV3.Commands.Contracts;
using ServiceAdapters.GlobalTixV3.GlobalTixV3.Converters;
using ServiceAdapters.GlobalTixV3.GlobalTixV3.Converters.Contracts;

namespace ServiceAdapters.GlobalTixV3
{
    public class GlobalTixModuleV3: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CityListCommandHandler>().As<ICityListCommandHandler>().InstancePerDependency();
            builder.RegisterType<CountryListCommandHandler>().As<ICountryListCommandHandler>().InstancePerDependency();
            builder.RegisterType<ProductListCommandHandler>().As<IProductListCommandHandler>().InstancePerDependency();
            builder.RegisterType<CategoriesCommandHandler>().As<ICategoriesCommandHandler>().InstancePerDependency();
            builder.RegisterType<ProductOptionCommandHandler>().As<IProductOptionCommandHandler>().InstancePerDependency();
            builder.RegisterType<ProductInfoCommandHandler>().As<IProductInfoCommandHandler>().InstancePerDependency();
            builder.RegisterType<ProductChangesCommandHandler>().As<IProductChangesCommandHandler>().InstancePerDependency();
            builder.RegisterType<PackageOptionsCommandHandler>().As<IPackageOptionsCommandHandler>().InstancePerDependency();

            builder.RegisterType<AvailabilitySeriesCommandHandler>().As<IAvailabilitySeriesCommandHandler>().InstancePerDependency();
            builder.RegisterType<ReservationCommandHandler>().As<IReservationCommandHandler>().InstancePerDependency();
            builder.RegisterType<BookingCommandHandler>().As<IBookingCommandHandler>().InstancePerDependency();
            builder.RegisterType<BookingDetailCommandHandler>().As<IBookingDetailCommandHandler>().InstancePerDependency();

            builder.RegisterType<CancelReleaseBookingCommandHandler>().As<ICancelReleaseCommandHandler>().InstancePerDependency();

            //Converter
            builder.RegisterType<ActivityInfoConverter>().As<IActivityInfoConverter>().InstancePerDependency();
            builder.RegisterType<ReservationConverter>().As<IReservationConverter>().InstancePerDependency();
            builder.RegisterType<BookingConverter>().As<IBookingConverter>().InstancePerDependency();
            builder.RegisterType<PackageOptionsConverter>().As<IPackageOptionsConverter>().InstancePerDependency();
        }
    }
}
