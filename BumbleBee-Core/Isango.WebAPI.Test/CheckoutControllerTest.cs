using Isango.Entities.Enums;
using Isango.Entities.HotelBeds;
using Isango.Service.Contract;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using TableStorageOperations.Contracts;
using TableStorageOperations.Models.AdditionalPropertiesModels.Availabilities;
using WebAPI.Controllers;
using WebAPI.Helper;
using WebAPI.Mapper;
using WebAPI.Models.RequestModels;
using WebAPI.Models.ResponseModels;
using PaymentExtraDetailRQ = WebAPI.Models.RequestModels.PaymentExtraDetail;

namespace Isango.WebAPI.Test
{
    [TestFixture]
    public class CheckoutControllerTest
    {
        private ICartService _cartService;
        private ITableStorageOperation _TableStorageOperations;
        private CheckoutController _checkoutControllerMock;
        private CheckoutHelper _checkoutHelper;

        [OneTimeSetUp]
        public void TestInitialise()
        {
            var checkoutMapper = new CheckoutMapper();
            _TableStorageOperations = Substitute.For<ITableStorageOperation>();
            _cartService = Substitute.For<ICartService>();
            _checkoutHelper = new CheckoutHelper(_cartService, _TableStorageOperations, checkoutMapper);
            _checkoutControllerMock = new CheckoutController(_TableStorageOperations, _checkoutHelper);
        }

        [Test]
        [Ignore("Ignore as it is not needed")]
        public void GetPaymentExtraDetailTest()
        {
            var paymentExtraDetails = new List<PaymentExtraDetailRQ>
            {
                new PaymentExtraDetailRQ
                {
                    ReferenceId="testRK",
                    //TravelInfo=new Entities.TravelInfo
                    //{
                    //    NoOfPassengers = new Dictionary<PassengerType, int>{{PassengerType.Adult,1}},
                    //    StartDate = DateTime.Now,
                    //    NumberOfNights = 1,
                    //    Ages = new Dictionary<PassengerType, int>{{PassengerType.Adult,25}}
                    //}
                }
            };
            var request = new PaymentExtraDetailsRequest
            {
                TokenId = "token",
                PaymentExtraDetails = paymentExtraDetails
            };

            PrepareMockDataForHotelBedsPaymentDetails();
            var response = _checkoutControllerMock.GetPaymentExtraDetails(request) as ObjectResult;

            var Content = response?.Value as PaymentExtraDetailsResponse;
            //Assert.IsNotNull(response);
            //Assert.IsTrue(response.Content.PaymentExtraDetails.Count > 0);
        }

        #region Private methods

        private void PrepareMockDataForHotelBedsPaymentDetails()
        {
            var baseAvailableEntity = new BaseAvailabilitiesEntity
            {
                ActivityId = 1001,
                ApiType = 3,
                AvailabilityStatus = "Available",
                //PartitionKey = "testPK",
               // RowKey = "testRK"
            };
            var ticketsAvailabilities = new TicketsAvailabilities
            {
                AvailToken = "availtoken",
                ModalityCode = "mod_code",
                TicketCode = "tocket_code",
                IsPaxDetailRequired = false,
                SupplierOptionCode = "test"
            };
            var hotelBedsSelectedProduct = new HotelBedsSelectedProduct
            {
                ContractQuestions = new List<Entities.ContractQuestion>
                {
                    new Entities.ContractQuestion
                    {
                        Code="code",
                        Description="description",
                        IsRequired=true,
                        Name="question"
                    }
                }
            };
            //_TableStorageOperations.RetrieveData<BaseAvailabilitiesEntity>("referenceId", "partitionKey").ReturnsForAnyArgs(baseAvailableEntity);
            //_TableStorageOperations.RetrieveData<TicketsAvailabilities>("referenceId", "partitionKey").ReturnsForAnyArgs(ticketsAvailabilities);
            _cartService.GetExtraDetailsForHotelBeds(new HotelBedsSelectedProduct(), "token").ReturnsForAnyArgs(hotelBedsSelectedProduct);
        }

        #endregion Private methods
    }
}