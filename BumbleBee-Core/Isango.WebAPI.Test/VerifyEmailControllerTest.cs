using Isango.Service.Contract;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NUnit.Framework;
using WebAPI.Controllers;

namespace Isango.WebAPI.Test
{
    [TestFixture]
    public class VerifyEmailControllerTest
    {
        private INeverBounceService _neverBounceService;
        private VerifyEmailController _verifyEmailController;

        [OneTimeSetUp]
        public void TestInitialise()
        {
            _neverBounceService = Substitute.For<INeverBounceService>();
            _verifyEmailController = new VerifyEmailController(_neverBounceService);
        }

        [Test]
        public void VerifyEmailTest()
        {
            _neverBounceService.IsEmailVerified("test@gmail.com", "test").ReturnsForAnyArgs(true);
            var result = _verifyEmailController.VerifyEmail("test@gmail.com", "test") as ObjectResult;

            var isVerified = result?.Value as bool?;
          
            Assert.IsNotNull(result);
            Assert.IsTrue(isVerified);
        }
    }
}