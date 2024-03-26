using Isango.Mailer.ServiceContracts;
using Isango.Service.Contract;
using NSubstitute;
using NUnit.Framework;
using ServiceAdapters.MoulinRouge;
using System;
using System.IO;
using System.Net.Http;
using WebAPI.Controllers;

namespace Isango.WebAPI.Test
{
    [TestFixture]
    public class VoucherControllerTest
    {
        private IMailAttachmentService _mailAttachmentServiceMock;
        private VoucherController _voucherController;
        private IMailerService _mailerServiceMock;
        private IMoulinRougeAdapter _moulinRougeAdapter;

        [OneTimeSetUp]
        public void TestInitialise()
        {
            _mailAttachmentServiceMock = Substitute.For<IMailAttachmentService>();
            _mailerServiceMock = Substitute.For<IMailerService>();
            _moulinRougeAdapter = Substitute.For<IMoulinRougeAdapter>();
            _voucherController = new VoucherController(_mailAttachmentServiceMock, _mailerServiceMock, _moulinRougeAdapter);
        }

        [Test]
        public void GetBookedVoucherTest()
        {
            var byteArray = new Byte[]
            {
                10,
                37,
                45,
                90
            };

            var dataStream = new MemoryStream(byteArray);

            var response = new HttpRequestMessage
            {
                Content = new StreamContent(dataStream)
            };

            response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment") { FileName = "test" };
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

            //_mailAttachmentServiceMock.GetBookedVoucher("testBookingRef").ReturnsForAnyArgs(byteArray,"","");

            var result = _voucherController.GetBookedVoucher("testbookingred");

            Assert.AreEqual("OK", result);
        }

        [Test]
        public void GetCancelledVoucherTest()
        {
            var byteArray = new Byte[]
            {
                10,
                37,
                45,
                90
            };

            var dataStream = new MemoryStream(byteArray);

            var response = new HttpRequestMessage
            {
                Content = new StreamContent(dataStream)
            };

            response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment") { FileName = "test" };
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

            _mailAttachmentServiceMock.GetCancelledVoucher("testBookingRef", "").ReturnsForAnyArgs(byteArray);

            var result = true;// _voucherController.GetCancelledVoucher("testbookingred", "");

            Assert.AreEqual("OK", result);
        }
    }
}