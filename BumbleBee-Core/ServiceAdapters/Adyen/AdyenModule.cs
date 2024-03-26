using Autofac;
using ServiceAdapters.Adyen.Adyen.Commands;
using ServiceAdapters.Adyen.Adyen.Commands.Contracts;
using ServiceAdapters.Adyen.Adyen.Converters;
using ServiceAdapters.Adyen.Adyen.Converters.Contracts;

namespace ServiceAdapters.Adyen
{
    public class AdyenModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<EnrollmentCheckCommandHandler>().As<IEnrollmentCheckCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<EnrollmentCheckConverter>().As<IEnrollmentCheckConverter>().InstancePerLifetimeScope();
            builder.RegisterType<PaymentMethodsCommandHandler>().As<IPaymentMethodsCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<PaymentMethodsConverter>().As<IPaymentMethodsConverter>().InstancePerLifetimeScope();
            builder.RegisterType<ThreeDsVerifyCommandHandler>().As<IThreeDSVerifyCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<ThreeDsVerifyConverter>().As<IThreeDsVerifyConverter>().InstancePerLifetimeScope();
            builder.RegisterType<CaptureCommandHandler>().As<ICaptureCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<CaptureConverter>().As<ICaptureConverter>().InstancePerLifetimeScope();
            builder.RegisterType<CancelCommandHandler>().As<ICancelCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<CancelConverter>().As<ICancelConverter>().InstancePerLifetimeScope();
            builder.RegisterType<RefundCommandHandler>().As<IRefundCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<RefundConverter>().As<IRefundConverter>().InstancePerLifetimeScope();
            builder.RegisterType<GeneratePaymentCommandHandler>().As<IGeneratePaymentCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<GeneratePaymentConverter>().As<IGeneratePaymentConverter>().InstancePerLifetimeScope();
        }
    }
}