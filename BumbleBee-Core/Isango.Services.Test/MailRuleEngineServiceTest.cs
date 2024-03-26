using Autofac;
using Isango.Entities.Enums;
using Isango.Entities.Mailer;
using Isango.Mailer.ServiceContracts;
using Isango.Register;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Isango.Services.Test
{
    public class MailRuleEngineServiceTest : BaseTest
    {
        private IMailRuleEngineService _mailRuleEngineService;

        [OneTimeSetUp]
        public void TestInitialise()
        {
            //var container = Startup._builder.Build();

            using (var scope = _container.BeginLifetimeScope())
            {
                _mailRuleEngineService = scope.Resolve<IMailRuleEngineService>();
            }
        }

        [Test]
        public void GetMailHeadersTest()
        {
            //First Scenario
            var result = _mailRuleEngineService.GetMailHeaders(
                new TemplateContext
                {
                    AffiliateId = Guid.Parse("278B6450-0A6B-49D5-8A54-47B3BF6A57B5"),
                    Language = "en",
                    MailActionType = ActionType.SupplierVoucherFS
                });
            Assert.Greater(result.Count, 0);

            //Second Scenario
            result = _mailRuleEngineService.GetMailHeaders(
                new TemplateContext
                {
                    AffiliateId = Guid.Parse("4D22F371-139C-4C37-B605-69696D1C9C93"),
                    Language = "en",
                    MailActionType = ActionType.SupplierVoucherFS
                });
            Assert.Greater(result.Count, 0);

            //Third Scenario
            result = _mailRuleEngineService.GetMailHeaders(
                new TemplateContext
                {
                    AffiliateId = Guid.Parse("278B6450-0A6B-49D5-8A54-47B3BF6A57B5"),
                    Language = "en",
                    MailActionType = ActionType.SupplierVoucherFS,
                    MailContextList = new List<MailHeader>
                    {
                        new MailHeader
                        {
                            From = "from@gmail.com",
                            CC =  new []{"cc@gmail.com"},
                            To = new []{"to@gmail.com"},
                            BCC = new []{"bcc@gmail.com"},
                            Subject = "Test Subject"
                        }
                    }
                });
            Assert.Greater(result.Count, 0);
        }

        [Test]
        public void GetMailHeaders_Negative_Test()
        {
            //Null Check Scenario
            var result = _mailRuleEngineService.GetMailHeaders(null);
            Assert.IsNull(result);
        }
    }
}