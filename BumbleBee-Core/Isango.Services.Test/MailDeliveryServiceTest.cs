using Autofac;
using Isango.Entities.Mailer;
using Isango.Mailer.ServiceContracts;
using Isango.Register;
using NUnit.Framework;
using SendGrid.Helpers.Mail;

namespace Isango.Services.Test
{
    public class MailDeliveryServiceTest : BaseTest
    {
        private IMailDeliveryService _mailDeliveryService;

        [OneTimeSetUp]
        public void TestInitialise()
        {
            //var container = Startup._builder.Build();
            using (var scope = _container.BeginLifetimeScope())
            {
                _mailDeliveryService = scope.Resolve<IMailDeliveryService>();
            }
        }

        [Test]
        [Ignore("ignored because this method sends mail.")]
        public void SendMailTest()
        {
            var mailContext = new MailContext
            {
                To = new[] { "bharati.tijare@saviantconsulting.com" },
                From = new EmailAddress("sendgrid@gmail.com", "SendGrid"),
                Subject = "SendGrid Test Mail",
                //CC = new [] { "ketan.vidhate@saviantconsulting.com" },
                HtmlContent = "<b>Hi, This is test mail.</b>",
            };

            _mailDeliveryService.SendMail(mailContext);
        }
    }
}