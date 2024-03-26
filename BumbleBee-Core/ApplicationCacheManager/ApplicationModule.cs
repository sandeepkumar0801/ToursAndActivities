using ApplicationCacheManager.Contract;
using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCacheManager
{
    public class ApplicationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<B2BNetRateRuleCache>().As<IB2BNetRateRuleCache>().InstancePerLifetimeScope();
        }
    }
}
