using Autofac;
using DiscountRuleEngine.Contracts;
using DiscountRuleEngine.Factory;

namespace DiscountRuleEngine
{
	public class DiscountRuleEngineModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterType<DiscountProcessor>().As<IDiscountProcessor>().InstancePerLifetimeScope();
			builder.RegisterType<MultiSaveDiscountModule>().InstancePerLifetimeScope();
			builder.RegisterType<VoucherValidator>().As<IVoucherValidator>().InstancePerLifetimeScope();
			builder.RegisterType<DiscountValidator>().As<IDiscountValidator>().InstancePerLifetimeScope();
			builder.RegisterType<DiscountModuleFactory>().InstancePerLifetimeScope();
			builder.RegisterType<DiscountEngine>().As<IDiscountEngine>().InstancePerLifetimeScope();
		}
	}
}
