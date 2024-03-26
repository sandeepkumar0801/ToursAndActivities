using Isango.Entities.AdyenPayment;
using ServiceAdapters.Adyen.Adyen.Converters.Contracts;
using ServiceAdapters.Adyen.Adyen.Entities;
using Util;

namespace ServiceAdapters.Adyen.Adyen.Converters
{
    public class PaymentMethodsConverter : ConverterBase, IPaymentMethodsConverter
    {
        public override object Convert(string response, object inputObject)
        {
            var paymentMethodResponseAPI = SerializeDeSerializeHelper.DeSerialize<PaymentMethodResponse>(response);
            var paymentMethodsResponseIsango = new PaymentMethodsResponse
            {
                PaymentMethods = new List<Isango.Entities.AdyenPayment.Paymentmethods>()
            };
            if (paymentMethodResponseAPI?.PaymentMethods != null)
            {
                foreach (var itemAPI in paymentMethodResponseAPI?.PaymentMethods)
                {
                    var itemIsango = new Isango.Entities.AdyenPayment.Paymentmethods
                    {
                        Name = itemAPI?.Name,
                        Type = itemAPI?.Type,
                    };
                    itemIsango.Brands = new List<string>();
                    if (itemAPI?.Brands != null)
                    {
                        foreach (var itemBrandAPI in itemAPI?.Brands)
                        {
                            itemIsango.Brands.Add(itemBrandAPI);
                        }
                    }
                    if (itemAPI?.configuration != null)
                    {
                        itemIsango.Configuration = new Isango.Entities.AdyenPayment.Configuration
                        {
                            MerchantId = itemAPI?.configuration?.MerchantId,
                            MerchantName = itemAPI?.configuration?.MerchantName,
                            Intent= itemIsango.Type.ToLowerInvariant()=="paypal"? "authorize": itemAPI?.configuration.Intent
                        };
                    }
                    itemIsango.Details = new List<Isango.Entities.AdyenPayment.Detail>();
                    if (itemAPI?.Details != null)
                    {
                        foreach (var itemDetailAPI in itemAPI?.Details)
                        {
                            var item = new Isango.Entities.AdyenPayment.Detail
                            {
                                Key = itemDetailAPI?.Key,
                                Type = itemDetailAPI?.Type,
                                Optional = itemDetailAPI.Optional,
                            };

                            item.Items = new List<Isango.Entities.AdyenPayment.Item>();
                            if (itemDetailAPI.Items != null)
                            {
                                foreach (var getItem in itemDetailAPI?.Items)
                                {
                                    var items = new Isango.Entities.AdyenPayment.Item()
                                    {
                                        Id = getItem?.Id,
                                        Name = getItem?.Name
                                    };
                                    item.Items.Add(items);
                                }
                            }
                            itemIsango.Details.Add(item);
                        }
                    }
                    paymentMethodsResponseIsango.PaymentMethods.Add(itemIsango);
                }
            }
            return paymentMethodsResponseIsango;
        }
    }
}