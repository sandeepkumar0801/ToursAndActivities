using Autofac;
using ServiceAdapters.WirecardPayment.WirecardPayment.Commands;
using ServiceAdapters.WirecardPayment.WirecardPayment.Commands.Contracts;
using ServiceAdapters.WirecardPayment.WirecardPayment.Converters;
using ServiceAdapters.WirecardPayment.WirecardPayment.Converters.Contracts;

namespace ServiceAdapters.WirecardPayment
{
    public class WirecardPaymentModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<BookBackCommandHandler>()
                .As<IBookBackCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<CapturePreauthorizeCommandHandler>()
                .As<ICapturePreauthorizeCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<ProcessPaymentCommandHandler>()
                .As<IProcessPaymentCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<ProcessPayment3DCommandHandler>()
                .As<IProcessPayment3DCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<RollBackCommandHandler>()
                .As<IRollBackCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<CapturePreauthorize3DCommandHandler>()
                .As<ICapturePreauthorize3DCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<EmiEnrollmentCheckCommandHandler>()
                .As<IEmiEnrollmentCheckCommandHandler>().InstancePerLifetimeScope();
            builder.RegisterType<EnrollmentCheckCommandHandler>()
                .As<IEnrollmentCheckCommandHandler>().InstancePerLifetimeScope();

            builder.RegisterType<BookBackConverter>()
                .As<IBookBackConverter>().InstancePerLifetimeScope();
            builder.RegisterType<CapturePreauthorizeConverter>()
                .As<ICapturePreauthorizeConverter>().InstancePerLifetimeScope();
            builder.RegisterType<ProcessPaymentConverter>()
                .As<IProcessPaymentConverter>().InstancePerLifetimeScope();
            builder.RegisterType<ProcessPayment3DConverter>()
                .As<IProcessPayment3DConverter>().InstancePerLifetimeScope();
            builder.RegisterType<RollBackConverter>()
                .As<IRollBackConverter>().InstancePerLifetimeScope();
            builder.RegisterType<CapturePreauthorize3DConverter>()
                .As<ICapturePreauthorize3DConverter>().InstancePerLifetimeScope();
            builder.RegisterType<EmiEnrollmentCheckConverter>()
                .As<IEmiEnrollmentCheckConverter>().InstancePerLifetimeScope();
            builder.RegisterType<EnrollmentCheckConverter>()
                .As<IEnrollmentCheckConverter>().InstancePerLifetimeScope();
        }
    }
}