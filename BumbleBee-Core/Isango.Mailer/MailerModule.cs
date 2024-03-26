using Autofac;
using Isango.Mailer.Contract;
using Isango.Mailer.ServiceContracts;
using Isango.Mailer.Services;

namespace Isango.Mailer
{
	public class MailerModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterType<MailDeliveryService>().As<IMailDeliveryService>().InstancePerLifetimeScope();
			builder.RegisterType<MailAttachmentService>().As<IMailAttachmentService>().InstancePerLifetimeScope();
			builder.RegisterType<MailGeneratorService>().As<IMailGeneratorService>().InstancePerLifetimeScope();
			builder.RegisterType<MailRuleEngineService>().As<IMailRuleEngineService>().InstancePerLifetimeScope();
			builder.RegisterType<TemplateContextService>().As<ITemplateContextService>().InstancePerLifetimeScope();
			builder.RegisterType<Mailer>().As<IMailer>().InstancePerLifetimeScope();
		}
	}
}
