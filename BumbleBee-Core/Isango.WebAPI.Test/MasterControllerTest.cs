//using Isango.Entities;
//using Isango.Service.Contract;
//using NSubstitute;
//using NUnit.Framework;
//using System.Web.Http.Results;
//using WebAPI.Controllers;
//using WebAPI.Models.ResponseModels;

//namespace Isango.WebAPI.Test
//{
//    internal class MasterControllerTest : BaseTest
//    {
//        public IMasterService _masterServiceMock;
//        public MasterController _masterControllerMock;

//        [OneTimeSetUp]
//        public void TestInitialise()
//        {
//            _masterServiceMock = Substitute.For<IMasterService>();
//            _masterControllerMock = new MasterController(_masterServiceMock);
//        }

//        [Test]
//        public void GetConversionFactorTest()
//        {
//            var clientInfo = new ClientInfo()
//            {
//                Currency = new Currency()
//                {
//                    IsoCode = "INR",
//                    Name = "Indian Rupee"
//                },
//                LanguageCode = "en"
//            };

//            _masterServiceMock.GetConversionFactorAsync("INR", "USD").ReturnsForAnyArgs(69);
//            var result = _masterControllerMock.GetConversionFactor("AUD", "USD") as OkNegotiatedContentResult<CurrencyExchangeRateResponse>;
//            Assert.AreEqual(69, result?.Content.ExchangeRateValue);
//        }
//    }
//}