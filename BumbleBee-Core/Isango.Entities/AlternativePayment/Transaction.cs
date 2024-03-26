using Newtonsoft.Json;

namespace Isango.Entities.AlternativePayment
{
    public class Transaction
    {
        public Transaction()
        {
        }

        private Transaction(Builder builder)
        {
            Customer = builder.Customer;
            Amount = builder.Amount;
            Currency = builder.Currency;
            RedirectUrls = builder.RedirectUrls;
            IpAddress = builder.IpAddress;
            Mode = builder.Mode;
        }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "customer")]
        public AlternativePaymentCustomer Customer { get; set; }

        [JsonProperty(PropertyName = "amount")]
        public decimal Amount { get; set; }

        [JsonProperty(PropertyName = "currency")]
        public string Currency { get; set; }

        [JsonProperty(PropertyName = "mode")]
        public string Mode { get; set; }

        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "redirectUrl")]
        public string RedirectUrl { get; set; }

        [JsonProperty(PropertyName = "redirectUrls")]
        public RedirectUrls RedirectUrls { get; set; }

        [JsonProperty(PropertyName = "ipAddress")]
        public string IpAddress { get; set; }

        public sealed class Builder
        {
            public Builder(AlternativePaymentCustomer customer, decimal amount, string currency, string ipAddress, string mode)
            {
                Customer = customer;
                Amount = amount;
                Currency = currency;
                IpAddress = ipAddress;
                Mode = mode;
            }

            public AlternativePaymentCustomer Customer { get; set; }
            public string Token { get; set; }
            public decimal Amount { get; set; }
            public string Currency { get; set; }

            public string IpAddress { get; set; }
            public string Mode { get; set; }

            public RedirectUrls RedirectUrls { get; set; }

            public void WithRedirectUrls(string returnUrl, string cancelUrl)
            {
                RedirectUrls = new RedirectUrls
                {
                    ReturnUrl = returnUrl,
                    CancelUrl = cancelUrl
                };
            }

            public Transaction Build()
            {
                return new Transaction(this);
            }
        }
    }
}