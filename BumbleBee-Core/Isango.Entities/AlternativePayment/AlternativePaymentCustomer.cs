using Newtonsoft.Json;

namespace Isango.Entities.AlternativePayment
{
    public class AlternativePaymentCustomer
    {
        public AlternativePaymentCustomer()
        {
        }

        private AlternativePaymentCustomer(Builder builder)
        {
            FirstName = builder.FirstName;
            LastName = builder.LastName;
            Email = builder.Email;
            Country = builder.Country;
        }

        [JsonProperty(PropertyName = "firstName")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "lastName")]
        public string LastName { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "country")]
        public string Country { get; set; }

        public sealed class Builder
        {
            public Builder(string firstName, string lastName, string email, string country)
            {
                FirstName = firstName;
                LastName = lastName;
                Email = email;
                Country = country;
            }

            public string FirstName { get; set; }

            public string LastName { get; set; }

            public string Email { get; set; }

            public string Country { get; set; }

            public AlternativePaymentCustomer Build()
            {
                return new AlternativePaymentCustomer(this);
            }
        }
    }
}