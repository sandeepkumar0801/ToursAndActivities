using Autofac;
using ServiceAdapters.AlternativePayment.AlternativePayment.Commands;
using ServiceAdapters.AlternativePayment.AlternativePayment.Commands.Contracts;

namespace ServiceAdapters.AlternativePayment
{
    public class AlternativePaymentModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AlternativePaymentCommandHandler>().As<IAlternativePaymentCommandHandler>().InstancePerLifetimeScope();
        }
    }
}