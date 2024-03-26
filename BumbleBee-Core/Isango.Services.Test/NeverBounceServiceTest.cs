using Isango.Register;
using Isango.Service;
using Isango.Service.Contract;
using NSubstitute;
using NUnit.Framework;
using ServiceAdapters.NeverBounce;

namespace Isango.Services.Test
{
    [TestFixture]
    public class NeverBounceServiceTest : BaseTest
    {
        private INeverBounceService _neverBounceService;
        private INeverBounceAdapter _neverBounceAdapter;

        [OneTimeSetUp]
        public void TestInitialise()
        {
            //var container = Startup._builder.Build();
            _neverBounceAdapter = Substitute.For<INeverBounceAdapter>();
            _neverBounceService = new NeverBounceService(_neverBounceAdapter);
        }

        [Test]
        public void IsEmailVerifiedTest()
        {
            _neverBounceAdapter.IsEmailNbVerified("test@gmail.com", "").ReturnsForAnyArgs(true);

            var result = _neverBounceService.IsEmailVerified("test@gmail.com", "");
            Assert.IsTrue(result);
        }
    }
}