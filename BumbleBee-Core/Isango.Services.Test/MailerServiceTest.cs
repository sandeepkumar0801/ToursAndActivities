using Autofac;
using Isango.Entities.Affiliate;
using Isango.Entities.Mailer;
using Isango.Register;
using Isango.Service.Contract;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Isango.Services.Test
{
    public class MailerServiceTest : BaseTest
    {
        private IMailerService _mailerService;

        [OneTimeSetUp]
        public void TestInitialise()
        {
           // var container = Startup._builder.Build();

            using (var scope = _container.BeginLifetimeScope())
            {
                _mailerService = scope.Resolve<IMailerService>();
            }
        }

        #region Send Mail For 'EN' Language

        [Test]
        [Ignore("ignored because this method sends mail.")]
        public void SendMail_Customer_FreeSale_EN_Test()
        {
            _mailerService.SendMail("ISA757686");
        }

        [Test]
        [Ignore("ignored because this method sends mail.")]
        public void SendMail_Customer_OnRequest_EN_Test()
        {
            _mailerService.SendMail("ISA757618");
        }

        [Test]
        [Ignore("ignored because this method sends mail.")]
        public void SendMail_Customer_2Step_FreeSell_EN_Test()
        {
            _mailerService.SendMail("ISA757189");
        }

        [Test]
        [Ignore("ignored because this method sends mail.")]
        public void SendMail_Customer_2Step_OnRequest_EN_Test()
        {
            _mailerService.SendMail("ISA757684");
        }

        [Test]
        [Ignore("ignored because this method sends mail.")]
        public void SendMail_Customer_FreeSell_OnRequest_EN_Test()
        {
            _mailerService.SendMail("ISA755858");
        }

        [Test]
        [Ignore("ignored because this method sends mail.")]
        public void SendMail_Customer_Cancel_EN_Test()
        {
            _mailerService.SendCancelMail("ISA757371");
        }

        [Test]
        [Ignore("ignored because this method sends mail.")]
        public void SendMail_Supplier_Cancel_EN_Test()
        {
            //Not sending mail to supplier
            _mailerService.SendSupplierCancelMail("ISA757371");
        }

        [Test]
        [Ignore("ignored because this method sends mail.")]
        public void SendMail_Supplier_FreeSell_EN_Test()
        {
            _mailerService.SendSupplierMail("ISA757686");
        }

        [Test]
        [Ignore("ignored because this method sends mail.")]
        public void SendMail_Supplier_OnRequest_EN_Test()
        {
            _mailerService.SendSupplierMail("ISA757618");
        }

        [Test]
        [Ignore("ignored because this method sends mail.")]
        public void SendMail_Supplier_2Step_EN_Test()
        {
            _mailerService.SendSupplierMail("ISA757189");
        }

        #endregion Send Mail For 'EN' Language

        #region Send Mail For 'DE' Language

        [Test]
        [Ignore("ignored because this method sends mail.")]
        public void SendMail_Customer_FreeSell_DE_Test()
        {
            _mailerService.SendMail("ISA757495");
        }

        [Test]
        [Ignore("ignored because this method sends mail.")]
        public void SendMail_Customer_OnRequest_DE_Test()
        {
            _mailerService.SendMail("ISA757246");
        }

        [Test]
        [Ignore("ignored because this method sends mail.")]
        public void SendMail_Customer_2Step_FreeSell_DE_Test()
        {
            _mailerService.SendMail("ISA756859");
        }

        [Test]
        [Ignore("ignored because this method sends mail.")]
        public void SendMail_Customer_FreeSell_OnRequest_DE_Test()
        {
            _mailerService.SendMail("ISA747096");
        }

        [Test]
        [Ignore("ignored because this method sends mail.")]
        public void SendMail_Customer_Cancel_DE_Test()
        {
            _mailerService.SendCancelMail("ISA757591");
        }

        [Test]
        [Ignore("ignored because this method sends mail.")]
        public void SendMail_Supplier_Cancel_DE_Test()
        {
            _mailerService.SendSupplierCancelMail("ISA757591");
        }

        [Test]
        [Ignore("ignored because this method sends mail.")]
        public void SendMail_Supplier_FreeSell_DE_Test()
        {
            _mailerService.SendSupplierMail("ISA757686");
        }

        [Test]
        [Ignore("ignored because this method sends mail.")]
        public void SendMail_Supplier_OnRequest_DE_Test()
        {
            _mailerService.SendSupplierMail("ISA757246");
        }

        #endregion Send Mail For 'DE' Language

        #region Send Mail For 'ES' Language

        [Test]
        [Ignore("ignored because this method sends mail.")]
        public void SendMail_Customer_FreeSell_ES_Test()
        {
            _mailerService.SendMail("ISA757680");
        }

        [Test]
        [Ignore("ignored because this method sends mail.")]
        public void SendMail_Customer_2Step_FreeSell_ES_Test()
        {
            _mailerService.SendMail("ISA757681");
        }

        [Test]
        [Ignore("ignored because this method sends mail.")]
        public void SendMail_Customer_2Step_OnRequest_ES_Test()
        {
            _mailerService.SendMail("ISA743371");
        }

        [Test]
        [Ignore("ignored because this method sends mail.")]
        public void SendMail_Customer_FreeSell_OnRequest_ES_Test()
        {
            _mailerService.SendMail("ISA755750");
        }

        [Test]
        [Ignore("ignored because this method sends mail.")]
        public void SendMail_Customer_Cancel_ES_Test()
        {
            _mailerService.SendCancelMail("ISA755185");
        }

        [Test]
        [Ignore("ignored because this method sends mail.")]
        public void SendMail_Supplier_Cancel_ES_Test()
        {
            _mailerService.SendSupplierCancelMail("ISA755185");
        }

        [Test]
        [Ignore("ignored because this method sends mail.")]
        public void SendMail_Supplier_FreeSell_ES_Test()
        {
            _mailerService.SendSupplierMail("ISA757680");
        }

        #endregion Send Mail For 'ES' Language

        #region Alert Mail

        [Test]
        [Ignore("ignored because this method sends mail.")]
        public void SendMail_Alert_Test()
        {
            var affiliate = new Affiliate
            {
                AffiliateCompanyDetail = new AffiliateCompanyDetail
                {
                    CompanyEmail = "bharati.tijare@saviantconsulting.com"
                },
                AffiliateCredit = new AffiliateCredit
                {
                    ThresholdAmount = 10.00M
                }
            };

            _mailerService.SendAlertMail(affiliate);
        }

        #endregion Alert Mail

        #region Failure Mail

        [Test]
        //[Ignore("ignored because this method sends mail.")]
        public void SendMail_Failure_Test()
        {
            var failedList = new List<FailureMailContext>
            {
                new FailureMailContext
                {
                    BookingReferenceNumber = "1234",
                    ServiceId = 1234,
                    APIBookingReferenceNumber = "1234",
                    CustomerEmailId = "skumar@isango.com",
                    TravelDate = DateTime.Now,
                    ContactNumber = "1234567890",
                    APICancellationStatus = true,
                    TokenId = "1234"
                }
            };

            _mailerService.SendFailureMail(failedList);
        }

        #endregion Failure Mail
    }
}