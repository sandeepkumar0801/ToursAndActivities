using Isango.Entities;
using Isango.Entities.Affiliate;
using Isango.Entities.ApexxPayment;
using Isango.Entities.Booking;
using Isango.Entities.Payment;
using NSubstitute;
using NUnit.Framework;
//using ServiceAdapters.AlternativePayment.Constants;
using ServiceAdapters.Apexx;
using ServiceAdapters.Apexx.Apexx.Commands.Contracts;
using ServiceAdapters.Apexx.Apexx.Converters.Contracts;
using ServiceAdapters.Apexx.Apexx.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Util;

namespace Isango.Adapter.Test
{
    [TestFixture]
    public class ApexxAdapterTest : BaseTest
    {
        private IApexxAdapter _apexxAdapter;
        private IEnrollmentCheckCommandHandler _enrollmentCheckCommandHandlerMock;
        private IThreeDSVerifyCommandHandler _threeDsVerifyCommandHandlerMock;
        private ICaptureCommandHandler _captureCommandHandlerMock;
        private IRefundCommandHandler _refundCommandHandlerMock;
        private ICancelCommandHandler _cancelCommandHandlerMock;
        private ICancelCaptureCommandHandler _cancelCaptureCommandHandlerMock;
        private IEnrollmentCheckConverter _enrollmentCheckConverterMock;
        private IThreeDsVerifyConverter _threeDsVerifyConverterMock;
        private ICaptureConverter _captureConverterMock;
        private IRefundConverter _refundConverterMock;
        private ICancelConverter _cancelConverterMock;
        private ICancelCaptureConverter _cancelCaptureConverterMock;

        [OneTimeSetUp]
        public void TestInitialise()
        {
            _enrollmentCheckCommandHandlerMock = Substitute.For<IEnrollmentCheckCommandHandler>();
            _threeDsVerifyCommandHandlerMock = Substitute.For<IThreeDSVerifyCommandHandler>();
            _captureCommandHandlerMock = Substitute.For<ICaptureCommandHandler>();
            _refundCommandHandlerMock = Substitute.For<IRefundCommandHandler>();
            _cancelCommandHandlerMock = Substitute.For<ICancelCommandHandler>();
            _cancelCaptureCommandHandlerMock = Substitute.For<ICancelCaptureCommandHandler>();
            _enrollmentCheckConverterMock = Substitute.For<IEnrollmentCheckConverter>();
            _threeDsVerifyConverterMock = Substitute.For<IThreeDsVerifyConverter>();
            _captureConverterMock = Substitute.For<ICaptureConverter>();
            _refundConverterMock = Substitute.For<IRefundConverter>();
            _cancelConverterMock = Substitute.For<ICancelConverter>();
            _cancelCaptureConverterMock = Substitute.For<ICancelCaptureConverter>();
            _apexxAdapter = new ApexxAdapter(_enrollmentCheckCommandHandlerMock, _threeDsVerifyCommandHandlerMock, _captureCommandHandlerMock, _refundCommandHandlerMock, _cancelCommandHandlerMock,
                null, _enrollmentCheckConverterMock, _threeDsVerifyConverterMock, _captureConverterMock, _refundConverterMock, _cancelConverterMock, _cancelCaptureCommandHandlerMock,
                _cancelCaptureConverterMock,null);
        }

        [Test]
        public void EnrollmentCheckTest()
        {
            var booking = new Booking
            {
                Payment = new List<Payment>
                {
                    new Payment
                    {
                        PaymentType = new CreditCard
                        {
                            CardNumber = "4543059999999982",
                            SecurityCode = "110",
                            ExpiryMonth = "12",
                            ExpiryYear = "19"
                        }
                    }
                },
                Amount = 10.00M,
                IpAddress = "127.0.0.1",
                BookingUserAgent = new BookingUserAgent
                {
                    UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-GB;rv:1.9.2.13) Gecko/20101203 Firefox/3.6.13 (.NET CLR 3.5.30729)"
                },
                Affiliate = new Affiliate
                {
                    AffiliateBaseURL = "http://localhost:50532"
                }
            };

            var payment = booking.Payment.FirstOrDefault();
            var creditCard = (CreditCard)payment?.PaymentType;

            var billingAddress = new BillingAddress
            {
                Country = "IN"
            };

            var apexxCriteria = new ApexxCriteria
            {
                //Account = "b5ca22022ff0479d922df91c78dbc8cf", //Instead of Account we will use Organisation and Currency
                Organisation = ConfigurationManagerHelper.GetValuefromAppSettings("c43a76370fd64f2da4be9b32bc9c1f8b"),
                Currency = "INR",
                Amount = (booking.Amount * 100).ToString("0"),
                CaptureNow = false,
                CustomerIp = booking.IpAddress,
                DynamicDescriptor = booking?.Affiliate?.Name,
                MerchantReference = "ISANGO0001" + "_" + Guid.NewGuid().ToString(),
                UserAgent = booking.BookingUserAgent.UserAgent,
                CardNumber = creditCard?.CardNumber,
                ExpiryMonth = creditCard?.ExpiryMonth,
                ExpiryYear = creditCard?.ExpiryYear,
                SecurityCode = creditCard?.SecurityCode,
                BaseUrl = booking.Affiliate.AffiliateBaseURL,
                ThreeDsRequired = "true",
                BillingAddress = billingAddress
            };

            var apexxPaymentResponse = new ApexxPaymentResponse
            {
                AcsRequest = "<html>test</html>",
                TransactionID = Guid.NewGuid().ToString()
            };

            var responseString = string.Empty;

            _enrollmentCheckCommandHandlerMock.Execute(apexxCriteria, "e2c78a83-89f7-46a3-a999-c969cb7df873").ReturnsForAnyArgs(responseString);
            _enrollmentCheckConverterMock.Convert(responseString, apexxCriteria).ReturnsForAnyArgs(apexxPaymentResponse);

            var result = _apexxAdapter.EnrollmentCheck(booking, "e2c78a83-89f7-46a3-a999-c969cb7df873");
            Assert.IsNotNull(result);
        }

        [Test]
        public void ThreeDSVerifyTest()
        {
            var transactionId = "579ac58f2f474175818043c0482b4dfd";
            var html = "<html><head><meta HTTP-EQUIV='Content-Type' content='text/html; charset=UTF-8'/><meta HTTP-EQUIV='Cache-Control' CONTENT='no cache'/><meta HTTP-EQUIV='Pragma' CONTENT='no cache'/><meta HTTP-EQUIV='Expires' CONTENT='0'/></head><body OnLoad='AutoSubmitForm();'><form name='downloadForm' action='https://acs.3ds-pit.com/' method='POST'><input type='hidden' name='PaReq' value='eJxVUsluwjAQ/RXEnXjJAkaDpdBINAcoKtwrx5lC1CaBLA3t19cmodCRLM2b8bPfLLA/VojRDnVboYQ11rU64ChLF2M/RZq+J4FOfM7HErbhK54lfGFVZ2UhmUMdDuQGDbXSR1U0EpQ+L+ONZNz1/ADIACHHKo4kNSaEIfYQCpWjzGpVHEogVwC6bIum+pauZd8AtNWnPDbNqZ4T0nWd03McXeYEiE0CuUvYttarzWOXLJXrKOwezyb66F72Mdv8hAsg9gakqkHJKROMcTGiYk6nc+oBucZB5VaFXC23I+47M6N+iMDJfhT2gPs28xgB09UKC/0tZ9zWckOAl1NZoOUA+fMhxVrL3Sqeudxl4i3wU8XQFROtOE68KeqJCBhOmMYAE0zcxBVGoSUBuVf89GxHoBvTXc/3XOqLwa66+4QVkJmumnp7BRYAsVQyzJkMu2C8fzvyCy6nteo='/><input type='hidden' name='TermUrl' value='https://isango.local/checkout/SecurePayment'/><input type='hidden' name='MD' value='579ac58f2f474175818043c0482b4dfd'/><SCRIPT LANGUAGE='Javascript'>function AutoSubmitForm(){ document.downloadForm.submit();}</SCRIPT><input type='submit' name='continue' value='Continue'/></form></body></html>";
            var apexxCriteria = new ApexxCriteria
            {
                TransactionId = transactionId,
                Pares = html,
            };

            var responseString = string.Empty;

            var apexxPaymentResponse = new ApexxPaymentResponse
            {
                Pares = html,
                TransactionID = transactionId
            };

            _threeDsVerifyCommandHandlerMock.Execute(apexxCriteria, "b9483c29-fa6c-4b28-ad09-a10fbd32isssd")
                .ReturnsForAnyArgs(responseString);

            _threeDsVerifyConverterMock.Convert(responseString, apexxCriteria).ReturnsForAnyArgs(apexxPaymentResponse);

            var result = _apexxAdapter.ThreeDSVerify(transactionId, "<html><head><meta HTTP-EQUIV='Content-Type' content='text/html; charset=UTF-8'/><meta HTTP-EQUIV='Cache-Control' CONTENT='no cache'/><meta HTTP-EQUIV='Pragma' CONTENT='no cache'/><meta HTTP-EQUIV='Expires' CONTENT='0'/></head><body OnLoad='AutoSubmitForm();'><form name='downloadForm' action='https://acs.3ds-pit.com/' method='POST'><input type='hidden' name='PaReq' value='eJxVUsluwjAQ/RXEnXjJAkaDpdBINAcoKtwrx5lC1CaBLA3t19cmodCRLM2b8bPfLLA/VojRDnVboYQ11rU64ChLF2M/RZq+J4FOfM7HErbhK54lfGFVZ2UhmUMdDuQGDbXSR1U0EpQ+L+ONZNz1/ADIACHHKo4kNSaEIfYQCpWjzGpVHEogVwC6bIum+pauZd8AtNWnPDbNqZ4T0nWd03McXeYEiE0CuUvYttarzWOXLJXrKOwezyb66F72Mdv8hAsg9gakqkHJKROMcTGiYk6nc+oBucZB5VaFXC23I+47M6N+iMDJfhT2gPs28xgB09UKC/0tZ9zWckOAl1NZoOUA+fMhxVrL3Sqeudxl4i3wU8XQFROtOE68KeqJCBhOmMYAE0zcxBVGoSUBuVf89GxHoBvTXc/3XOqLwa66+4QVkJmumnp7BRYAsVQyzJkMu2C8fzvyCy6nteo='/><input type='hidden' name='TermUrl' value='https://isango.local/checkout/SecurePayment'/><input type='hidden' name='MD' value='579ac58f2f474175818043c0482b4dfd'/><SCRIPT LANGUAGE='Javascript'>function AutoSubmitForm(){ document.downloadForm.submit();}</SCRIPT><input type='submit' name='continue' value='Continue'/></form></body></html>",
                "579ac58f2f474175818043c0482b4dfd");
            Assert.IsNotNull(result);
        }

        [Test]
        public void CaptureCardTransactionTest()
        {
            var transactionId = Guid.NewGuid().ToString();

            var apexxCriteria = new ApexxCriteria()
            {
                Amount = "100",
                CaptureRefernce = "ISANGO",
                TransactionId = transactionId
            };

            var responseString = string.Empty;

            var apexxPaymentResponse = new ApexxPaymentResponse
            {
                TransactionID = transactionId,
                Pares = "ISANGO",
                Status = "AUTHORIZED",
                AuthorizationCode = "TEST"
            };

            _captureCommandHandlerMock.Execute(apexxCriteria, "token").ReturnsForAnyArgs(responseString);
            _captureConverterMock.Convert(responseString, apexxCriteria).ReturnsForAnyArgs(apexxPaymentResponse);
            var result = _apexxAdapter.CaptureCardTransaction(100m, "ISANGO", transactionId, "token");
            Assert.IsNotNull(result);
        }

        [Test]
        public void RefundCardTransactionTest()
        {
            var transactionId = Guid.NewGuid().ToString();

            var apexxCriteria = new ApexxCriteria()
            {
                Amount = "100",
                Reason = "testing",
                TransactionId = transactionId
            };
            var responseString = string.Empty;

            var apexxPaymentResponse = new ApexxPaymentResponse
            {
                TransactionID = transactionId,
                Status = "REFUNDED"
            };

            _refundCommandHandlerMock.Execute(apexxCriteria, "token").ReturnsForAnyArgs(responseString);
            _refundConverterMock.Convert(responseString, apexxCriteria).ReturnsForAnyArgs(apexxPaymentResponse);

            var result = _apexxAdapter.RefundCardTransaction(100m, "testing", transactionId, "token","");
            Assert.IsNotNull(result);
        }

        [Test]
        public void CancelCardTransactionTest()
        {
            var transactionId = Guid.NewGuid().ToString();

            var apexxCriteria = new ApexxCriteria()
            {
                TransactionId = transactionId
            };
            var responseString = string.Empty;

            var apexxPaymentResponse = new ApexxPaymentResponse
            {
                TransactionID = transactionId,
                Status = "CANCELLED"
            };

            _cancelCommandHandlerMock.Execute(apexxCriteria, "token").ReturnsForAnyArgs(responseString);
            _cancelConverterMock.Convert(responseString, apexxCriteria).ReturnsForAnyArgs(apexxPaymentResponse);

            var result = _apexxAdapter.CancelCardTransaction(transactionId, "token");
            Assert.IsNotNull(result);
        }
    }
}