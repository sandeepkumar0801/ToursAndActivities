using Autofac;
using ServiceAdapters.RiskifiedPayment.RiskifiedPayment.Commands;
using ServiceAdapters.RiskifiedPayment.RiskifiedPayment.Commands.Contracts;

namespace ServiceAdapters.RiskifiedPayment
{
    public class RiskifiedPaymentModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            #region commandHandler

            builder.RegisterType<DecideCommandHandler>().As<IDecideCommandHandler>().InstancePerLifetimeScope();

            builder.RegisterType<DecisionCommandHandler>().As<IDecisionCommandHandler>().InstancePerLifetimeScope();

            builder.RegisterType<CheckoutDeniedCommandHandler>().As<ICheckoutDeniedCommandHandler>().InstancePerLifetimeScope();

            builder.RegisterType<FulFillCommandHandler>().As<IFulFillCommandHandler>().InstancePerLifetimeScope();

            builder.RegisterType<FullRefundCommandHandler>().As<IFullRefundCommandHandler>().InstancePerLifetimeScope();

            builder.RegisterType<PartialRefundCommandHandler>().As<IPartialRefundCommandHandler>().InstancePerLifetimeScope();

            #endregion commandHandler

            #region Adapter

            builder.RegisterType<RiskifiedPaymentAdapter>().As<IRiskifiedPaymentAdapter>().InstancePerLifetimeScope();

            #endregion Adapter
        }
    }
}