using Autofac;
using DiscountRuleEngine.Contracts;
using DiscountRuleEngine.Model;
using Isango.Register;
using NUnit.Framework;
using System.Collections.Generic;

namespace Isango.Services.Test
{
    [TestFixture]
    public class DiscountRuleEngineTest : BaseTest
    {
        private IDiscountEngine _discountEngine;

        [OneTimeSetUp]
        public void TestInitialise()
        {
           // var container = Startup._builder.Build();
            using (var scope = _container.BeginLifetimeScope())
            {
                _discountEngine = scope.Resolve<IDiscountEngine>();
            }
        }

        [Test]
        public void DiscountEngineProcessTest()
        {
            var selectedProduct = new List<DiscountSelectedProduct>();
            var product = new DiscountSelectedProduct
            {
                SellPrice = 1000,
                CurrencyIsoCode = "INR"
            };
            var testProduct = new DiscountSelectedProduct
            {
                SellPrice = 1000,
                CurrencyIsoCode = "INR"
            };
            var testSelectedProduct = new DiscountSelectedProduct
            {
                SellPrice = 1000,
                CurrencyIsoCode = "INR"
            };
            var vouchers = new List<VoucherInfo>();
            var voucher = new VoucherInfo
            {
                VoucherCode = "GV11D162QOENYE"
            };
            vouchers.Add(voucher);
            selectedProduct.Add(product);
            selectedProduct.Add(testProduct);
            selectedProduct.Add(testSelectedProduct);
            var discountModel = new DiscountModel
            {
                AffiliateId = "b76bcc30-11d1-42db-9d75-a2c3e38f91d8",
                Cart = new DiscountCart
                {
                    SelectedProducts = selectedProduct,
                    TotalPrice = 3000,
                    CurrencyIsoCode = "INR"
                },
                Vouchers = vouchers
            };
            var result = _discountEngine.Process(discountModel);
            Assert.IsNotNull(result);
        }
    }
}