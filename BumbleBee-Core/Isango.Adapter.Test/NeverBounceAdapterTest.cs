using Autofac;
using Isango.Register;
using NUnit.Framework;
using ServiceAdapters.NeverBounce;
using System;

namespace Isango.Adapter.Test
{
    [TestFixture]
    public class NeverBounceAdapterTest : BaseTest
    {
        private INeverBounceAdapter _neverBounceAdapter;

        [OneTimeSetUp]
        public void TestInitialise()
        {
            //var container = Startup._builder.Build();

            using (var scope = _container.BeginLifetimeScope())
            {
                _neverBounceAdapter = scope.Resolve<INeverBounceAdapter>();
            }
        }

        /// <summary>
        /// Test case to check if email NB verified or not
        /// </summary>
        /// <param name="emailId"></param>
        /// <param name="expected"></param>
        [Test]
        [TestCase("btijare.ext@isango.com", true)]
        public void IsEmailNbVerifiedTest(string emailId, bool expected)
        {
            var token = Guid.NewGuid();
            var result = _neverBounceAdapter.IsEmailNbVerified(emailId, token.ToString());
            Assert.AreEqual(expected, result);
        }
    }
}