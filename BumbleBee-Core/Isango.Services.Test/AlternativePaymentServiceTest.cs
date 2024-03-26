using Autofac;
using Isango.Persistence.Contract;
using Isango.Register;
using Isango.Service;
using Isango.Service.Contract;
using Logger.Contract;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using ServiceAdapters.AlternativePayment;
using System;

namespace Isango.Services.Test
{
    [TestFixture]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class AlternativePaymentServiceTest : BaseTest
    {
        private IAlternativePaymentService _alternativePaymentService;
        private AlternativePaymentService alternativePaymentService;

        [OneTimeSetUp]
        public void TestInitialise()
        {
            var builder = new ContainerBuilder(); // Create a new ContainerBuilder
            builder.RegisterModule<StartupModule>();
            var  container = builder.Build(); // Build the container
           // var container = Startup._builder.Build();
            var gatewayLogger = Substitute.For<ILogger>();
            var gatewayPaymentAdapter = Substitute.For<IAlternativePaymentAdapter>();
            var gatewayAlternativePersistence = Substitute.For<IAlternativePaymentPersistence>();

            alternativePaymentService = new AlternativePaymentService(gatewayLogger, gatewayPaymentAdapter,
                 gatewayAlternativePersistence);

            using (var scope = container.BeginLifetimeScope())
            {
                _alternativePaymentService = scope.Resolve<IAlternativePaymentService>();
            }
        }

        //[Test]
        //[Ignore("CreateTransactionTest")]
        //public void CreateTransactionTest()
        //{
        //    var booking = new Booking { };
        //    var successUrl = string.Empty;
        //    var failureurl = string.Empty;
        //    var token = Guid.NewGuid().ToString();

        //    var result = _alternativePaymentService.CreateTransaction(booking, successUrl, failureurl, token);

        //    Assert.IsTrue(result.Result.Amount > 0);
        //}

        //[Test]
        //[Ignore("CreateTransactionExceptionTest")]
        //public void CreateTransactionExceptionTest()
        //{
        //    //Catch block scenario
        //    alternativePaymentService.CreateTransaction(null, null, null, null).Throws(new Exception());
        //    Assert.Throws<Exception>(() =>
        //        alternativePaymentService.CreateTransaction(null, null, null, null).GetAwaiter().GetResult());
        //}

        [Test]
        [Ignore("Ignore as it hits the live API call")]
        public void GetAlternativeTransactionTest()
        {
            var transactionId = string.Empty;
            var apiKey = string.Empty;
            var token = Guid.NewGuid().ToString();
            var baseUrl = string.Empty;

            var result = _alternativePaymentService.GetAlternativeTransaction(transactionId, apiKey, token, baseUrl);

            Assert.AreEqual(result.Result.Id, transactionId);
        }

        [Test]
        [Ignore("Ignore as it hits the live API call")]
        public void GetAlternativeTransactionExceptionTest()
        {
            //Catch block scenario
            alternativePaymentService.GetAlternativeTransaction(null, null, null).Throws(new Exception());
            Assert.Throws<Exception>(() =>
                alternativePaymentService.GetAlternativeTransaction(null, null, null).GetAwaiter().GetResult());
        }

        [Test]
        [Ignore("Ignore as it hits the live API call")]
        public void CompleteTransactionAfterBookingTest()
        {
            alternativePaymentService.CompleteTransactionAfterBookingAsync("BRef01", "B001", Guid.NewGuid().ToString()).ReturnsForAnyArgs(true);
            var result = _alternativePaymentService.CompleteTransactionAfterBookingAsync("BRef01", "B001", Guid.NewGuid().ToString()).Result;
            Assert.IsTrue(result);
        }

        [Test]
        [Ignore("Ignore as it hits the live API call")]
        public void CompleteTransactionAfterBookingExceptionTest()
        {
            alternativePaymentService.CompleteTransactionAfterBookingAsync("BRef01", "B001", Guid.NewGuid().ToString()).ThrowsForAnyArgs(new Exception());
            Assert.ThrowsAsync<Exception>(() => _alternativePaymentService.CompleteTransactionAfterBookingAsync("BRef01", "B001", Guid.NewGuid().ToString()));
        }

        [Test]
        [Ignore("Ignore as it hits the live API call")]
        public void GetBookingRefByTransIdTest()
        {
            alternativePaymentService.GetBookingRefByTransIdAsync("19076").ReturnsForAnyArgs("success");
            var result = _alternativePaymentService.GetBookingRefByTransIdAsync("19076").Result;
            Assert.AreEqual("success", result);
        }

        [Test]
        [Ignore("Ignore as it hits the live API call")]
        public void GetBookingRefByTransIdExceptionTest()
        {
            alternativePaymentService.GetBookingRefByTransIdAsync("19076").ThrowsForAnyArgs(new Exception());
            Assert.ThrowsAsync<Exception>(() => _alternativePaymentService.GetBookingRefByTransIdAsync("19076"));
        }

        [Test]
        [Ignore("Ignore as it hits the live API call")]
        public void UpdateSofortChargeBackTest()
        {
            alternativePaymentService.UpdateSofortChargeBackAsync("B001", "active").ReturnsForAnyArgs(true);
            var result = _alternativePaymentService.UpdateSofortChargeBackAsync("B001", "active").Result;
            Assert.IsTrue(result);
        }

        [Test]
        [Ignore("Ignore as it hits the live API call")]
        public void UpdateSofortChargeBackExceptionTest()
        {
            alternativePaymentService.UpdateSofortChargeBackAsync("B001", "active").ThrowsForAnyArgs(new Exception());
            Assert.ThrowsAsync<Exception>(() => _alternativePaymentService.UpdateSofortChargeBackAsync("B001", "active"));
        }
    }
}