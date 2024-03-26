using ServiceAdapters.Adyen.Adyen.Converters.Contracts;
using ServiceAdapters.Adyen.Adyen.Entities;
using Util;

namespace ServiceAdapters.Adyen.Adyen.Converters
{
    public class GeneratePaymentConverter : ConverterBase, IGeneratePaymentConverter
    {
        public override object Convert(string response, object inputObject)
        {
            var generatePaymentResponseAPI = SerializeDeSerializeHelper.DeSerialize<GeneratePaymentResponse>(response);
            var generatePaymentIsango = new Isango.Entities.AdyenPayment.GeneratePaymentIsangoResponse
            {
                CountryCode = generatePaymentResponseAPI?.CountryCode,
                Description = generatePaymentResponseAPI?.Description,
                ExpiresAt = generatePaymentResponseAPI?.ExpiresAt,
                Id = generatePaymentResponseAPI?.Id,
                MerchantAccount = generatePaymentResponseAPI?.MerchantAccount,
                Reference = generatePaymentResponseAPI?.Reference,
                ShopperLocale = generatePaymentResponseAPI?.ShopperLocale,
                ShopperReference = generatePaymentResponseAPI?.ShopperReference,
                Url = generatePaymentResponseAPI?.Url
            };
            var generatePaymentAmountRS = new Isango.Entities.AdyenPayment.GeneratePaymentAmountRS
            {
                Value = generatePaymentResponseAPI?.GeneratePaymentAmount?.Value,
                Currency= generatePaymentResponseAPI?.GeneratePaymentAmount?.Currency
            };
            generatePaymentIsango.GeneratePaymentAmount = generatePaymentAmountRS;
            return generatePaymentIsango;
        }
    }
}