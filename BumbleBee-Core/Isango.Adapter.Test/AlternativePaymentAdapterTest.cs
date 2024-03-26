using Autofac;
using Isango.Register;
using NUnit.Framework;
using ServiceAdapters.AlternativePayment;
using System;

namespace Isango.Adapter.Test
{
    [TestFixture]
    internal class AlternativePaymentAdapterTest : BaseTest
    {
        private IAlternativePaymentAdapter _alternativePaymentAdapter;

        [OneTimeSetUp]
        public void TestInitialise()
        {
            //var container = Startup._builder.Build();

            using (var scope = _container.BeginLifetimeScope())
            {
                _alternativePaymentAdapter = scope.Resolve<IAlternativePaymentAdapter>();
            }
        }

        /// <summary>
        /// Test case to create payment
        /// </summary>
        [Test]
        public void CreatePaymentTest()
        {
            var authToken = "c2tfdGVzdF9MZUF6YnptMmkwZTBxNjhSVWRVelJzZDNaNFoxR3poUlg0dHFLU0x1OkF1dGhJbmZv";
            var apiUrl = "https://api.alternativepayments.com/api/transactions/hosted";
            var transaction = "{\"Customer\":{\"FirstName\":\"test\",\"LastName\":\"test\",\"" +
                                 "Email\":\"test@email.com\",\"Country\":\"GB\"},\"Amount\":6035.0,\"" +
                                 "Currency\":\"GBP\",\"Mode\":\"Test\",\"RedirectUrls\":{\"returnUrl\":\"" +
                                 "http://localhost:64032//Checkout/TransactionSuccess\",\"cancelUrl\":\"" +
                                 "http://localhost:64032//Checkout/TransactionFail\"},\"IpAddress\":\"" +
                                 "127.0.0.1\"}";
            var token = Guid.NewGuid();
            var result = _alternativePaymentAdapter.Create(authToken, apiUrl, transaction, token.ToString()).GetAwaiter();

            Assert.NotNull(result);
        }

        /// <summary>
        /// Test case to get transaction
        /// </summary>
        [Test]
        public void GetTransactionTest()
        {
            var authToken = "c2tfdGVzdF9MZUF6YnptMmkwZTBxNjhSVWRVelJzZDNaNFoxR3poUlg0dHFLU0x1OkF1dGhJbmZv";
            var apiUrl = "https://api.alternativepayments.com/api/transactions/hosted";
            var transaction = "{\"Customer\":{\"FirstName\":\"test\",\"LastName\":\"test\",\"" +
                              "Email\":\"test@email.com\",\"Country\":\"GB\"},\"Amount\":6035.0,\"" +
                              "Currency\":\"GBP\",\"Mode\":\"Test\",\"RedirectUrls\":{\"returnUrl\":\"" +
                              "http://localhost:64032//Checkout/TransactionSuccess\",\"cancelUrl\":\"" +
                              "http://localhost:64032//Checkout/TransactionFail\"},\"IpAddress\":\"" +
                              "127.0.0.1\"}";
            var token = Guid.NewGuid();
            var result = _alternativePaymentAdapter.GetTransaction(authToken, apiUrl, transaction, token.ToString()).GetAwaiter();
            Assert.NotNull(result);
        }
    }
}