using Autofac;
using TableStorageOperations.TableStorageOperations;
using TableStorageOperations.Contracts;

namespace TableStorageOperations
{
    public class TableStorageModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<TableStorageOperation>().As<ITableStorageOperation>().InstancePerLifetimeScope();
            builder.RegisterType<ConfigReader>().As<IConfigReader>().InstancePerLifetimeScope();
        }
    }
}