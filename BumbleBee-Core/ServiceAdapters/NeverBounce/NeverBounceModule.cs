using Autofac;
using ServiceAdapters.NeverBounce.NeverBounce.Commands;
using ServiceAdapters.NeverBounce.NeverBounce.Commands.Contracts;

namespace ServiceAdapters.NeverBounce
{
    public class NeverBounceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AuthenticationCmdHandler>()
                .As<IAuthenticationCmdHandler>()
                .InstancePerLifetimeScope();
            builder.RegisterType<EmailVerificationCmdHandler>().As<IEmailVerificationCmdHandler>()
                .InstancePerLifetimeScope();
        }
    }
}