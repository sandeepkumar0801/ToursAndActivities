using ActivityWrapper.Helper;
using ActivityWrapper.Mapper;
using Autofac;

namespace ActivityWrapper
{
	public class WrapperModule : Module
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="builder"></param>
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterType<ActivityWrapperHelper>().InstancePerLifetimeScope();
			builder.RegisterType<ActivityWrapperMapper>().InstancePerLifetimeScope();
			builder.RegisterType<ActivityWrapperService>().InstancePerLifetimeScope();
		}
	}
}
