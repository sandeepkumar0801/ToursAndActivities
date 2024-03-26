using Autofac;
using ServiceAdapters.Apexx.Apexx.Commands;
using ServiceAdapters.Apexx.Apexx.Commands.Contracts;
using ServiceAdapters.Apexx.Apexx.Converters;
using ServiceAdapters.Apexx.Apexx.Converters.Contracts;

namespace ServiceAdapters.Apexx
{
    public class ApexxModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<EnrollmentCheckCommandHandler>().As<IEnrollmentCheckCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<ThreeDsVerifyCommandHandler>().As<IThreeDSVerifyCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<CaptureCommandHandler>().As<ICaptureCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<RefundCommandHandler>().As<IRefundCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<CancelCommandHandler>().As<ICancelCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<CancelCaptureCommandHandler>().As<ICancelCaptureCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<CreateCardTransactionCommandHandler>().As<ICreateCardTransactionCommandHandler>().InstancePerLifetimeScope();

            builder.RegisterType<EnrollmentCheckConverter>().As<IEnrollmentCheckConverter>().InstancePerLifetimeScope();
            builder.RegisterType<ThreeDsVerifyConverter>().As<IThreeDsVerifyConverter>().InstancePerLifetimeScope();
            builder.RegisterType<RefundConverter>().As<IRefundConverter>().InstancePerLifetimeScope();
            builder.RegisterType<CaptureConverter>().As<ICaptureConverter>().InstancePerLifetimeScope();
            builder.RegisterType<CancelConverter>().As<ICancelConverter>().InstancePerLifetimeScope();
            builder.RegisterType<CancelCaptureConverter>().As<ICancelCaptureConverter>().InstancePerLifetimeScope();
            builder.RegisterType<CreateCardTransactionConverter>().As<ICreateCardTransactionConverter>().InstancePerLifetimeScope();
        }
    }
}