using Isango.Entities;
using Isango.Service.Contract;
using NSubstitute;
using NUnit.Framework;
using WebAPI.Controllers;
using WebAPI.Mapper;
using WebAPI.Models.RequestModels;
using WebAPI.Models.ResponseModels;

namespace Isango.WebAPI.Test
{
    [TestFixture]
    public class LandingControllerTest : BaseTest
    {
        private IManageIdentityService _manageIdentityServiceMock;
        private SubscribeController _landingControllerMock;
        private SubscribeMapper _subscribeMapper;

        [OneTimeSetUp]
        public void TestInitialise()
        {
            _manageIdentityServiceMock = Substitute.For<IManageIdentityService>();
            _subscribeMapper = Substitute.For<SubscribeMapper>();
            _landingControllerMock = new SubscribeController(_manageIdentityServiceMock, _subscribeMapper);
        }

        //[Test]
        //public void TestSubscribeUser_Success()
        //{
        //    var citeria = PrepareNewsLetterData();
        //    //var clientInfo = new ClientInfoRequest
        //    //{
        //    //    // F2DE3C6A-048D-4C7E-AAF0-86CF0D51DDEF // (B2BNetRateModule)
        //    //    // 0E03E273-5FE3-4C80-A80F-2100E9A26513 // (B2BSale Module)
        //    //    // E9B092AD-F8F7-4B15-A478-CB3D444A6D8A // (Product Sale Module)
        //    //    AffiliateId = "0E03E273-5FE3-4C80-A80F-2100E9A26513",
        //    //    AffiliateName = "B2C",
        //    //    IsB2BAffiliate = true,
        //    //    CountryIp = "gb",
        //    //    IsSupplementOffer = true,
        //    //    CurrencyIsoCode = "GBP"
        //    //};
        //    //var newsLetter = new NewsLetterRequest
        //    //{
        //    //    EmailId = "test@test.com",
        //    //    ClientInfo = clientInfo,
        //    //    Name = "Test"
        //    //};

        //    var newsLetterData = new NewsLetter
        //    {
        //        EmailId = "test@test.com",
        //        AffiliateId = "5BEEF089-3E4E-4F0F-9FBF-99BF1F350183",
        //        Name = "Test",
        //        LanguageCode = "en",
        //        CustomerOrigin = "GBP"
        //    };

        //    _manageIdentityServiceMock.SubscribeNewsLetterAsync(citeria).ReturnsForAnyArgs("subscribed");
        //    var result = _landingControllerMock.SubscribeNewsLetter(newsLetterData);

        //    Assert.IsNotEmpty(result);
        //    Assert.That(result.Content.Status.Equals("subscribed"));
        //}

        private NewsLetterCriteria PrepareNewsLetterCriteria()
        {
            return new NewsLetterCriteria
            {
                EmailId = "test@test.com",
                LanguageCode = "en",
                AffiliateId = "5BEEF089-3E4E-4F0F-9FBF-99BF1F350183",
                CountryId = 0,
                CountryName = "India",
                IsNbVerified = true,
                ConsentUser = true
            };
        }

        private NewsLetterData PrepareNewsLetterData()
        {
            return new NewsLetterData
            {
                EmailId = "test@test.com",
                LanguageCode = "en",
                AffiliateId = "5BEEF089-3E4E-4F0F-9FBF-99BF1F350183",
                Name = "test",
                CustomerOrigin = "GBP"
            };
        }
    }
}