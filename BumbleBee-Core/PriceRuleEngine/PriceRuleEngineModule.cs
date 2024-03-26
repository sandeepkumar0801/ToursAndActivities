using Autofac;
using PriceRuleEngine.Factory;
using PriceRuleEngine.Modules;

namespace PriceRuleEngine
{
    public class PriceRuleEngineModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<PricingController>().InstancePerLifetimeScope();
            builder.RegisterType<ModuleBuilderFactory>().InstancePerLifetimeScope();
            builder.RegisterType<ModuleDataSingleton>().SingleInstance();
        }
    }
}