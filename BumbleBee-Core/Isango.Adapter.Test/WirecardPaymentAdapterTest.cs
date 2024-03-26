using Isango.Entities;
using Isango.Entities.Booking;
using Isango.Entities.Payment;
using Isango.Entities.WirecardPayment;
using NSubstitute;
using NUnit.Framework;
using ServiceAdapters.WirecardPayment;
using ServiceAdapters.WirecardPayment.WirecardPayment.Commands.Contracts;
using ServiceAdapters.WirecardPayment.WirecardPayment.Converters.Contracts;
using ServiceAdapters.WirecardPayment.WirecardPayment.Entities;
using System;
using System.Collections.Generic;

namespace Isango.Adapter.Test
{
    [TestFixture]
    public class WirecardPaymentAdapterTest : BaseTest
    {
        private WirecardPaymentAdapter _wirecardPaymentAdapterForMocking;
        private IProcessPaymentCommandHandler _processPaymentCommandHandlerForMocking;
        private IProcessPaymentConverter _processPaymentConverterForMocking;
        private IProcessPayment3DCommandHandler _processPayment3DCommandHandlerForMocking;
        private IProcessPayment3DConverter _processPayment3DConverterForMocking;
        private ICapturePreauthorize3DCommandHandler _capturePreauthorize3DCommandHandlerForMocking;
        private ICapturePreauthorize3DConverter _capturePreauthorize3DConverter;
        private IBookBackCommandHandler _bookBackCommandHandlerForMocking;
        private IBookBackConverter _bookBackConverterForMocking;
        private IEnrollmentCheckCommandHandler _enrollmentCheckCommandHandlerForMocking;
        private IEmiEnrollmentCheckCommandHandler _emiEnrollmentCheckCommandHandlerForMocking;
        private IRollBackCommandHandler _rollBackCommandHandlerForMocking;
        private ICapturePreauthorizeCommandHandler _capturePreauthorizeCommandHandlerForMocking;
        private IRollBackConverter _rollBackConverterForMocking;
        private ICapturePreauthorizeConverter _capturePreauthorizeConverterForMocking;
        private IEnrollmentCheckConverter _enrollmentCheckConverterForMocking;
        private IEmiEnrollmentCheckConverter _emiEnrollmentCheckConverterForMocking;

        [OneTimeSetUp]
        public void TestInitialize()
        {
            _bookBackCommandHandlerForMocking = Substitute.For<IBookBackCommandHandler>();
            _bookBackConverterForMocking = Substitute.For<IBookBackConverter>();
            _capturePreauthorizeCommandHandlerForMocking = Substitute.For<ICapturePreauthorizeCommandHandler>();
            _capturePreauthorizeConverterForMocking = Substitute.For<ICapturePreauthorizeConverter>();
            _processPaymentCommandHandlerForMocking = Substitute.For<IProcessPaymentCommandHandler>();
            _processPaymentConverterForMocking = Substitute.For<IProcessPaymentConverter>();
            _processPayment3DCommandHandlerForMocking = Substitute.For<IProcessPayment3DCommandHandler>();
            _processPayment3DConverterForMocking = Substitute.For<IProcessPayment3DConverter>();
            _rollBackCommandHandlerForMocking = Substitute.For<IRollBackCommandHandler>();
            _rollBackConverterForMocking = Substitute.For<IRollBackConverter>();
            _emiEnrollmentCheckCommandHandlerForMocking = Substitute.For<IEmiEnrollmentCheckCommandHandler>();
            _emiEnrollmentCheckConverterForMocking = Substitute.For<IEmiEnrollmentCheckConverter>();
            _capturePreauthorize3DCommandHandlerForMocking = Substitute.For<ICapturePreauthorize3DCommandHandler>();
            _capturePreauthorize3DConverter = Substitute.For<ICapturePreauthorize3DConverter>();
            _enrollmentCheckCommandHandlerForMocking = Substitute.For<IEnrollmentCheckCommandHandler>();
            _enrollmentCheckConverterForMocking = Substitute.For<IEnrollmentCheckConverter>();

            _wirecardPaymentAdapterForMocking = new WirecardPaymentAdapter(_bookBackCommandHandlerForMocking, _bookBackConverterForMocking, _capturePreauthorizeCommandHandlerForMocking, _capturePreauthorizeConverterForMocking, _processPaymentCommandHandlerForMocking, _processPaymentConverterForMocking, _processPayment3DCommandHandlerForMocking, _processPayment3DConverterForMocking, _rollBackCommandHandlerForMocking, _rollBackConverterForMocking, _emiEnrollmentCheckCommandHandlerForMocking, _capturePreauthorize3DCommandHandlerForMocking, _capturePreauthorize3DConverter, _emiEnrollmentCheckConverterForMocking, _enrollmentCheckCommandHandlerForMocking, _enrollmentCheckConverterForMocking);
        }

        [Test]
        public void ChargeWithRefIdTest()
        {
            var payment = GetPayment();
            var wirecardPaymentResponse = CreateWireCardPaymentResponse("CapturePreauthorize");
            var response = Tuple.Create(wirecardPaymentResponse, convertedReqXml, convertedResXml);
            payment.PaymentType = GetCreditCard();
            var token = Guid.NewGuid();
            payment.PaymentGatewayReferenceId = "323";
            _capturePreauthorizeCommandHandlerForMocking.Execute(new PaymentCardCriteria(), false, token.ToString())
                .ReturnsForAnyArgs(Tuple.Create(requestXml, responseXml));
            _capturePreauthorizeConverterForMocking.Convert(responseXml, null).ReturnsForAnyArgs(wirecardPaymentResponse);
            var result = _wirecardPaymentAdapterForMocking.Charge(payment, token.ToString());
            Assert.That(result, Is.EqualTo(response));
        }

        [Test]
        public void ChargeWithoutRefIdTest()
        {
            var payment = GetPayment();
            var wirecardPaymentResponse = CreateWireCardPaymentResponse("CapturePreauthorize");
            var response = Tuple.Create(wirecardPaymentResponse, convertedReqXml, convertedResXml);
            payment.PaymentType = GetCreditCard();
            var token = Guid.NewGuid();
            payment.PaymentGatewayReferenceId = null;
            _processPaymentCommandHandlerForMocking.Execute(new PaymentCardCriteria(), false, token.ToString())
                .ReturnsForAnyArgs(Tuple.Create(requestXml, responseXml));
            _processPaymentConverterForMocking.Convert(responseXml, payment).ReturnsForAnyArgs(wirecardPaymentResponse);
            var resultData = _wirecardPaymentAdapterForMocking.Charge(payment, token.ToString());
            Assert.That(resultData, Is.EqualTo(response));
        }

        [Test]
        public void Charge3DWithRefIdTest()
        {
            var payment = GetPayment();
            var booking = new Booking();
            var wirecardPaymentResponse = CreateWireCardPaymentResponse("CapturePreauthorize");
            var response = Tuple.Create(wirecardPaymentResponse, convertedReqXml, convertedResXml);
            payment.PaymentType = GetCreditCard();
            var token = Guid.NewGuid();
            payment.PaymentGatewayReferenceId = "323";
            _capturePreauthorizeCommandHandlerForMocking.Execute(new PaymentCardCriteria(), true, token.ToString())
                .ReturnsForAnyArgs(Tuple.Create(requestXml, responseXml));
            _capturePreauthorizeConverterForMocking.Convert(responseXml, null).ReturnsForAnyArgs(wirecardPaymentResponse);
            var result = _wirecardPaymentAdapterForMocking.Charge3D(payment, booking, token.ToString());
            Assert.That(result, Is.EqualTo(response));
        }

        [Test]
        public void Charge3DWithoutRefIdTest()
        {
            var payment = GetPayment();
            var booking = new Booking();
            var wirecardPaymentResponse = CreateWireCardPaymentResponse("CapturePreauthorize");
            var response = Tuple.Create(wirecardPaymentResponse, convertedReqXml, convertedResXml);
            payment.PaymentType = GetCreditCard();
            var token = Guid.NewGuid();
            payment.PaymentGatewayReferenceId = null;
            _processPayment3DCommandHandlerForMocking.Execute(new PaymentCardCriteria(), true, token.ToString()).ReturnsForAnyArgs(Tuple.Create(requestXml, responseXml));
            _processPayment3DConverterForMocking.Convert(responseXml, payment).ReturnsForAnyArgs(wirecardPaymentResponse);
            var resultData = _wirecardPaymentAdapterForMocking.Charge3D(payment, booking, token.ToString());
            Assert.That(resultData, Is.EqualTo(response));
        }

        [Test]
        public void BookBackTest()
        {
            var payment = GetPayment();
            var wirecardPaymentResponse = CreateWireCardPaymentResponse("Bookback");
            var token = Guid.NewGuid();
            var response = Tuple.Create(wirecardPaymentResponse, convertedReqXml, convertedResXml);
            var bookBackData = Tuple.Create(requestXml, responseXml);
            _bookBackConverterForMocking.Convert(responseXml, null).ReturnsForAnyArgs(wirecardPaymentResponse);
            _bookBackCommandHandlerForMocking.Execute(new PaymentCardCriteria(), true, token.ToString())
                .ReturnsForAnyArgs(bookBackData);

            var result = _wirecardPaymentAdapterForMocking.BookBack(payment, token.ToString());
            Assert.That(result, Is.EqualTo(response));
        }

        [Test]
        public void RollBackTest()
        {
            var payment = GetPayment();
            payment.Is3D = true;
            var wirecardPaymentResponse = CreateWireCardPaymentResponse("Reversal");
            var response = Tuple.Create(wirecardPaymentResponse, convertedReqXml, convertedResXml);
            var token = Guid.NewGuid();
            _rollBackCommandHandlerForMocking.Execute(new PaymentCardCriteria(), true, token.ToString()).ReturnsForAnyArgs(Tuple.Create(requestXml, responseXml));
            _rollBackConverterForMocking.Convert(responseXml, null).ReturnsForAnyArgs(wirecardPaymentResponse);
            var result = _wirecardPaymentAdapterForMocking.Rollback(payment, token.ToString());
            Assert.That(result, Is.EqualTo(response));
        }

        [Test]
        public void CapturePreauthorizeTest()
        {
            var payment = GetPayment();
            var wirecardPaymentResponse = CreateWireCardPaymentResponse("CapturePreauthorize");
            var response = Tuple.Create(wirecardPaymentResponse, convertedReqXml, convertedResXml);
            var token = Guid.NewGuid();
            _capturePreauthorizeCommandHandlerForMocking.Execute(new PaymentCardCriteria(), false, token.ToString())
                .ReturnsForAnyArgs(Tuple.Create(requestXml, responseXml));
            _capturePreauthorizeConverterForMocking.Convert(responseXml, null).ReturnsForAnyArgs(wirecardPaymentResponse);
            var result = _wirecardPaymentAdapterForMocking.CapturePreauthorize(payment, token.ToString());
            Assert.That(result, Is.EqualTo(response));
        }

        [Test]
        public void PreAuthorizeTest()
        {
            var wirecardPaymentResponse = CreateWireCardPaymentResponse("PreAuthorize");
            var response = Tuple.Create(wirecardPaymentResponse, convertedReqXml, convertedResXml);
            var payment = GetPayment();
            var token = Guid.NewGuid();
            payment.PaymentType = GetCreditCard();
            _processPaymentCommandHandlerForMocking.Execute(new PaymentCardCriteria(), false, token.ToString())
                .ReturnsForAnyArgs(Tuple.Create(requestXml, responseXml));
            _processPaymentConverterForMocking.Convert(responseXml, payment).ReturnsForAnyArgs(wirecardPaymentResponse);
            var result =
                _wirecardPaymentAdapterForMocking.PreAuthorize(payment, token.ToString());
            Assert.That(result, Is.EqualTo(response));
        }

        [Test]
        public void EnrollmentCheckTest()
        {
            var booking = new Booking
            {
                Payment = new List<Payment> { GetPayment() },
                Currency = new Currency(),
                IpAddress = "",
                Amount = 0m
            };
            var wirecardPaymentResponse = CreateWireCardPaymentResponse("EnrollCheck");
            booking.Payment[0].PaymentType = GetCreditCard();
            var response = Tuple.Create(wirecardPaymentResponse, convertedReqXml, convertedResXml);
            var token = Guid.NewGuid();
            _enrollmentCheckCommandHandlerForMocking.Execute(new PaymentCardCriteria() { TagText = string.Empty }, true, string.Empty)
                .ReturnsForAnyArgs(Tuple.Create(requestXml, responseXml));
            _enrollmentCheckConverterForMocking.Convert(responseXml, null).ReturnsForAnyArgs(wirecardPaymentResponse);
            var result = _wirecardPaymentAdapterForMocking.EnrollmentCheck(booking, token.ToString());
            Assert.That(result, Is.EqualTo(response));
        }

        [Test]
        public void EmiEnrollmentCheckTest()
        {
            var booking = new Booking
            {
                Payment = new List<Payment> { GetPayment() },
                Currency = new Currency()
            };
            var installment = new Installment
            {
                UserId = "1",
                BookingRefNo = "21",
                Amount = 33,
                CurrencyCode = "as",
                Card = GetCreditCard()
            };
            var wirecardPaymentResponse = CreateWireCardPaymentResponse("EnrollCheck");
            var response = Tuple.Create(wirecardPaymentResponse, convertedReqXml, convertedResXml);
            var token = Guid.NewGuid();
            _emiEnrollmentCheckCommandHandlerForMocking.Execute(new PaymentCardCriteria(), true, token.ToString()).ReturnsForAnyArgs(Tuple.Create(requestXml, responseXml));
            _emiEnrollmentCheckConverterForMocking.Convert(responseXml, null).ReturnsForAnyArgs(wirecardPaymentResponse);
            var result = _wirecardPaymentAdapterForMocking.EmiEnrollmentCheck(booking, installment, token.ToString());
            Assert.That(result, Is.EqualTo(response));
        }

        [Test]
        public void PreAuthorize3DTest()
        {
            var wirecardPaymentResponse = CreateWireCardPaymentResponse("PreAuthorize");
            var response = Tuple.Create(wirecardPaymentResponse, convertedReqXml, convertedResXml);
            var payment = GetPayment();
            var booking = new Booking();
            var token = Guid.NewGuid();
            payment.PaymentType = GetCreditCard();
            _processPayment3DCommandHandlerForMocking.Execute(new PaymentCardCriteria(), true, token.ToString()).ReturnsForAnyArgs(Tuple.Create(requestXml, responseXml));
            _processPayment3DConverterForMocking.Convert(responseXml, payment).ReturnsForAnyArgs(wirecardPaymentResponse);
            var result =
                _wirecardPaymentAdapterForMocking.PreAuthorize3D(payment, booking, token.ToString());
            Assert.That(result, Is.EqualTo(response));
        }

        [Test]
        public void PreAuthPurchase3DTest()
        {
            var preAuthPayment = GetPayment();
            preAuthPayment.PaymentType = GetCreditCard();
            var purchasePayment = GetPayment();
            var token = Guid.NewGuid();
            var wirecardPaymentResponse = CreateWireCardPaymentResponse("PreAuthorize");
            var response = Tuple.Create(wirecardPaymentResponse, convertedReqXml, convertedResXml);
            var booking = new Booking();
            _processPayment3DCommandHandlerForMocking.Execute(new PaymentCardCriteria(), true, token.ToString()).ReturnsForAnyArgs(Tuple.Create(requestXml, responseXml));
            _processPayment3DConverterForMocking.Convert(responseXml, preAuthPayment).ReturnsForAnyArgs(wirecardPaymentResponse);
            _capturePreauthorize3DCommandHandlerForMocking.Execute(new PaymentCardCriteria(), true, token.ToString()).ReturnsForAnyArgs(Tuple.Create(requestXml, responseXml));
            _capturePreauthorize3DConverter.Convert(responseXml, preAuthPayment).ReturnsForAnyArgs(wirecardPaymentResponse);
            var result = _wirecardPaymentAdapterForMocking.PreAuthPurchase3D(preAuthPayment, purchasePayment, booking, token.ToString());
            Assert.That(result, Is.EqualTo(response));
        }

        #region Private Methods

        private WirecardPaymentResponse CreateWireCardPaymentResponse(string type)
        {
            return new WirecardPaymentResponse
            {
                JobId = "4324",
                TransactionId = "234523",
                Status = "Success",
                RequestType = type,
                PaymentGatewayReferenceId = "2353",
                AuthorizationCode = "5234",
                RequestXml = requestXml
            };
        }

        private Payment GetPayment()
        {
            return new Payment
            {
                JobId = "123",
                TransactionId = "12334",
                Guwid = "3231",
                ChargeAmount = 12,
                Is3D = true
            };
        }

        private CreditCard GetCreditCard()
        {
            return new CreditCard
            {
                SecurityCode = "003",
                CardHoldersCountryName = "Test",
                CardHoldersAddress1 = "TestAdd",
                CardHoldersCity = "TestCity",
                CardHoldersZipCode = "411057",
                CardHoldersState = "MH",
                CardHoldersEmail = "test@test.com",
                ExpiryYear = "2019",
                ExpiryMonth = "1",
                CardHoldersName = "Test test",
                CardNumber = "4012000300001003"
            };
        }

        #endregion Private Methods

        #region Private xml variables

        private string requestXml =
            "<?xml version='1.0' encoding='UTF-8'?><WIRECARD_BXML xmlns:xsi='http://www.w3.org/1999/XMLSchema-instance' xsi:noNamespaceSchemaLocation='wirecard.xsd'><W_REQUEST><W_JOB><JobID>621129_SGI622525</JobID><BusinessCaseSignature>0000003164DF5F22</BusinessCaseSignature><FNC_CC_PREAUTHORIZATION> <FunctionID>Preauthorization 1</FunctionID><CC_TRANSACTION><TransactionID>1240829</TransactionID><GuWID>C640478154340408724695</GuWID><CREDIT_CARD_DATA><CVC2>003</CVC2></CREDIT_CARD_DATA><THREE-D_SECURE><PARes>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</PARes></THREE-D_SECURE></CC_TRANSACTION></FNC_CC_PREAUTHORIZATION></W_JOB></W_REQUEST></WIRECARD_BXML>";

        private readonly string responseXml = "<?xml version='1.0' encoding='UTF-8'?><WIRECARD_BXML xmlns:xsi='http://www.w3.org/1999/XMLSchema-instance' xsi:noNamespaceSchemaLocation='wirecard.xsd'><W_RESPONSE><W_JOB><JobID>621129_SGI622525</JobID><FNC_CC_PREAUTHORIZATION><FunctionID>Preauthorization 1</FunctionID><CC_TRANSACTION><TransactionID>1240829</TransactionID><PROCESSING_STATUS><GuWID>C786456154340412255830</GuWID><AuthorizationCode>263030</AuthorizationCode><CVCResponseCode>P</CVCResponseCode><StatusType>Y</StatusType><FunctionResult>ACK</FunctionResult><AVS><ResultCode>U</ResultCode><Message>AVS Unavailable.</Message><AuthorizationEntity>5</AuthorizationEntity><AuthorizationEntityMessage>Response provided by issuer processor.</AuthorizationEntityMessage><ProviderResultCode>I</ProviderResultCode><ProviderResultMessage>Address information is unavailable, or the Issuer does not support AVS. Acquirer has representment rights.</ProviderResultMessage></AVS><TimeStamp>2018-11-28 12:22:02</TimeStamp></PROCESSING_STATUS></CC_TRANSACTION></FNC_CC_PREAUTHORIZATION></W_JOB></W_RESPONSE></WIRECARD_BXML>";

        private readonly string convertedReqXml =
            "<?xml version=\\\"1.0\\\" encoding=\\\"UTF-8\\\"?><WIRECARD_BXML xmlns:xsi=\\\"http://www.w3.org/1999/XMLSchema-instance\\\" xsi:noNamespaceSchemaLocation=\\\"wirecard.xsd\\\"><W_REQUEST><W_JOB><JobID>621129_SGI622525</JobID><BusinessCaseSignature>0000003164DF5F22</BusinessCaseSignature><FNC_CC_PREAUTHORIZATION> <FunctionID>Preauthorization 1</FunctionID><CC_TRANSACTION><TransactionID>1240829</TransactionID><GuWID>C640478154340408724695</GuWID><CREDIT_CARD_DATA><CVC2>003</CVC2></CREDIT_CARD_DATA><THREE-D_SECURE><PARes>eJydV1lzqsoWfvdXWDmPnr2ZRHGX8VQziKggyCDwxgzKoMzw6y9qsk+yk4d7b1el0r366zX1t3rJ8p82ice1lxdRlr6+ID/hl7GXOpkbpcHri6qsfxAv/6yWSph7Hi17TpV7q9F4GEveKwor8MaR+/qyQKZTAkUwDJsh6BzHEYIgXp64B1YER694IHF0AU9hZI6jGIJM0Rk2RWYfkA/0mzerwZmf6BJ6X34G8V7uhFZafhY/tiznRnLCCkGxKT5bQm/Lr7jEyzl6hcMwjAx/8BJ6Cr4CUyvxVqco9xwrd8e0l2RjOcyuS+ix8RXvZFVa5t2KQAfz74uvsCqPV2FZXn9BUNM0P5s3Az+dLFlC983PEUPfh7wUq7u4+M6RNnJXRu90FrrG3VMMywnenVK+cS7a1WSz1yV0R3w951qlt0JhhEAQlBgj6C8U+YXOl9BD/k3Ck3uMK5T4iQ2gt9VX2PXNUfAOv6P/EH6TzCrPB0a+Z/N99RXotdcs9e6Kl9Dv+R8p/D5Xy6uVrtoPY+ADNrhm/Uk6Rf/GbhklX5KFD8cf8q/worTKqlgZS+ht9k3IVl2vAAAkDX0e4DmGe3tAvsmBE61gfIh/+P+9XhAHWR6VYXJP02fBH7n6GOyQuXsJv5W+HAXp4HrujYfHIy1eXz6yGPuZ5QGEDgUFwQtoALhFFPz1snqc8lwu9bPVkrLSLI0cK456qxyKm/fKMHPHv335TqVyvGtFoCND/RjU/nCQafrjLoExBP/rNByjsiQZLr14gVb/+vjfqP7T27ywfhShhdwVHT3fu1POG6tH7vXlr+9fsKWSW2nhZ3lSfJj/b1a9tPbi7Oq5P4p35+8OQB9V01HgFeX/E9N7PE8NmhVX3ipKZv4e7WhUF7aof5xbJWZ4XQ0HQB049hG5hH7nYZh/vMrfiX4CAw2mtd2ilVn3oEhWW4aYUBFIhrnxNBADfzZrnYt6AxvjuF8fYaoMenbn1tdTojkGdpm686bTGXeLBvJhxJM4o4m7rgjEXaZdPUIlKfuk1YIHHyxRx1DM95xFGM60cOK7pstwRprPs8xeN8jGc4tOvO6nQS0pyGU26uMJw3c+i5x3skL5tr0w7cne7wWDyIISVvHbgm+P4h4xxRl1K252t71oDdo2ZaJBtDIXYUbOhtAvln0emeu6ZuvrvF87XXCxcw+VZ2ueOMSTyJa2B7ELmP05XavAAso0RhF/FzstdfA2N20Stdhls9mlmh+1hHC6jUj3TBfnIncS+kojkAHxfdgivLpLCYyx26hzRfEEXl+fif+Q7OXO6563oOPwgrZK6zmTK/vsOaVwb0+U8Iq5xafu8vf4oL7+7meAHdav+BSbwdgveXiTvDFppZdx5o95qxr4FVXFeF+6f4+pV15dQn8aeFikvLyM/KGihwbBcxzj9BQFkCoADUeCgKO4ndHQkrHdZSYX1o4AJGZNSqAJzKalerAlA0EjgaGAWFN4mWk2jUFrkrQbMW0oGLoAm/qWNvTthWPiymW1zk7WsHVaVMapafkzaJ4KCmWnIqGDHWPnIoROIgXyCb+frQz9eB3ZKB7aFKmYJwFxEo220WPMMW5sR6DlFaMRwMMqzXTIZ6NrITYwrTNVoTZZNZBOQjsaGmpn6lygYlpkY9uLdcJD974JD+6HjsDTxuAZM+UVrucVpj/dZQrf8D03yEAvKCp8OpM5LxXNiJIellmm2QqawuQ8dWEBojJUyEeCIvXCGfSHWICHMAaPt6GdFIGNGoFyWg+RxLClaz3HtLBJM+pQNtzzcMvvNXTw8vTMhaQ6LasA/ZmrwSdauDgdjln6NjbQdWdS+NlG4SYImIgHMEvJN3YkczZGSwwJJBWAKUfSDbgDdiAbblailbjFieN5Bu3RqD0bctr6/anHar5E8eOFxPqivGzt0waxRN8c5S4rxuGMzdRNePGOzblJAwGm+CLdyRHFKhNvCsitwu0DrI7iOQkInlevBK4HKLs2mwYqJrraT3enDidGKNFdThzWa+K6kGEBCsoAyizcjic4dj7fZuR8oxWIse5DUWHWfU3J3FZLiVvUq5Cj0pND4C4uJYAmxu4wCg2Xj2up2W9dN9JrQa+UrTTZ9od1wx7kQ02ffeVctFcrN9T4yK+nly5Tr2mm84E8L4Vwu0tchVtTGamVo2ijUip1sGNtUtOStK54YO44n2QvrBsARmIT0Re1+exMrIltNfWcOaPBlBIudHmOy8nWuNUKCHgSAPY8CrxpMzzEzYMiR1ghAdcAGhzut7iRCBL4xHA7PEVOQcM0D0xMgqahAoPbNQZJSupmKMMmsAN2NNDC5RqZLGiKzC2WvM5k8jrMM4XZ8+DJOTLkKU3jW04B/pMqmcIyX8tq9F5Xj7IaioanwTvvrgMx+3udSg9ekVs7HQ4n9wqJ66F2iz9LanREtYqjmfbQA/ydoPzls1UV1c7uQFYJXVxMmZTtoRdzjBA76fFqJvH56cw0GEm6BlvsorP0I34n6zub38lMvpEZSIMxjDsxWUYdN82OXGBlapydgtwop/6IiqaojsAsmKfqEbuGw/OSzW2foRAxryBF6XmWml91nshPfidL3Tnsc6FB8fVBnRQkPmEvDhzIbXWR3UvQy2BXjuRdtLXNCKocbWPWECled21GGhvnZs5FEubQvaMwtQ+DmlacSpmErX/rbpJjNi4F5ZvtzYfStHe8DM8vI0GIdHVeTOsbCnDkhizqlk/8PiCieXeqTcI0fYXQ+WTfrxmWarQqProios+BiaAQnbFD85snFFcPD7U80hI+m9WwvtCoENc90a+AJYtZu9hcUfIgwGqrzA/C/BhU3qmutIlYzBSmJTZ5MquKdrKlwkndYYmO9kE7SvGYe7aKj53hKXn2Keh37/q3q63uHzmPz8rVaPjd8/Gj8z9qVnzw</PARes></THREE-D_SECURE></CC_TRANSACTION></FNC_CC_PREAUTHORIZATION></W_JOB></W_REQUEST></WIRECARD_BXML>";

        private readonly string convertedResXml =
            "<?xml version=\\\"1.0\\\" encoding=\\\"UTF-8\\\"?><WIRECARD_BXML xmlns:xsi=\\\"http://www.w3.org/1999/XMLSchema-instance\\\" xsi:noNamespaceSchemaLocation=\\\"wirecard.xsd\\\"><W_RESPONSE><W_JOB><JobID>621129_SGI622525</JobID><FNC_CC_PREAUTHORIZATION><FunctionID>Preauthorization 1</FunctionID><CC_TRANSACTION><TransactionID>1240829</TransactionID><PROCESSING_STATUS><GuWID>C786456154340412255830</GuWID><AuthorizationCode>263030</AuthorizationCode><CVCResponseCode>P</CVCResponseCode><StatusType>Y</StatusType><FunctionResult>ACK</FunctionResult><AVS><ResultCode>U</ResultCode><Message>AVS Unavailable.</Message><AuthorizationEntity>5</AuthorizationEntity><AuthorizationEntityMessage>Response provided by issuer processor.</AuthorizationEntityMessage><ProviderResultCode>I</ProviderResultCode><ProviderResultMessage>Address information is unavailable, or the Issuer does not support AVS. Acquirer has representment rights.</ProviderResultMessage></AVS><TimeStamp>2018-11-28 12:22:02</TimeStamp></PROCESSING_STATUS></CC_TRANSACTION></FNC_CC_PREAUTHORIZATION></W_JOB></W_RESPONSE></WIRECARD_BXML>";

        #endregion Private xml variables
    }
}