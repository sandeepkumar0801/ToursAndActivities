using Autofac;
using Isango.Mailer.ServiceContracts;
using Isango.Register;
using NUnit.Framework;

namespace Isango.Services.Test
{
    public class MailAttachmentServiceTest : BaseTest
    {
        private IMailAttachmentService _mailAttachmentService;

        [OneTimeSetUp]
        public void TestInitialise()
        {
            //var container = Startup._builder.Build();

            using (var scope = _container.BeginLifetimeScope())
            {
                _mailAttachmentService = scope.Resolve<IMailAttachmentService>();
            }
        }

        [Test]
        [Ignore("Getting Exception: IronBarCode must be licensed for  barcode creation outside of the Visual Studio development environment.")]
        public void GetVoucherForFreeSaleProduct_ENTest()
        {
            _mailAttachmentService.GetBookedVoucher("ISA757686");
        }

        [Test]
        [Ignore("Getting Exception: IronBarCode must be licensed for  barcode creation outside of the Visual Studio development environment.")]
        public void GetVoucherForFreeSaleAndOnRequestProduct_ENTest()
        {
            _mailAttachmentService.GetBookedVoucher("ISA755858");
        }

        [Test]
        public void GetCancelledVoucher_ENTest()
        {
            _mailAttachmentService.GetCancelledVoucher("ISA757371", "1181455");
        }

        [Test]
        public void GetCancelledVoucher_DETest()
        {
            _mailAttachmentService.GetCancelledVoucher("ISA757591", "1181811");
        }
    }
}