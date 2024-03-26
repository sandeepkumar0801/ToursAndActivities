using Isango.Entities.RiskifiedPayment;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ServiceAdapters.RiskifiedPayment.RiskifiedPayment.Entities
{
    public class RiskifiedDecideRequest
    {
        [JsonProperty(PropertyName = "order")]
        public OrderData Order { get; set; }
    }

    public class OrderData
    {
        /// <summary>
        /// The unique identifier for the order
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        /// <summary>
        /// The customer's email address
        /// </summary>
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        /// <summary>
        /// The date and time when the order was first created
        /// </summary>
        [JsonProperty(PropertyName = "created_at")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// The three letter code (ISO 4217) for the currency used for the payment
        /// </summary>
        [JsonProperty(PropertyName = "currency")]
        public string Currency { get; set; }

        /// <summary>
        /// The payment gateway used
        /// </summary>
        [JsonProperty(PropertyName = "gateway")]
        public string Gateway { get; set; }

        /// <summary>
        /// The customer's browser IP address at the time of order checkout
        /// </summary>
        [JsonProperty(PropertyName = "browser_ip")]
        public string BrowserIp { get; set; }

        /// <summary>
        /// The sum of all the prices of all the items in the order, taxes and discounts included (must be positive)
        /// </summary>
        [JsonProperty(PropertyName = "total_price")]
        public float TotalPrice { get; set; }

        /// <summary>
        /// The total amount of the discounts to be applied to the price of the order
        /// </summary>
        //[JsonProperty(PropertyName = "total_discounts")]
        //public float TotalDiscount { get; set; }

        /// <summary>
        /// The webpage from which the customer accessed the shop
        /// </summary>
        [JsonProperty(PropertyName = "referring_site")]
        public string ReferringSite { get; set; }

        /// <summary>
        /// The session id that this order was created on,
        /// this value should match the session id value that is passed in the beacon JavaScript
        /// </summary>
        [JsonProperty(PropertyName = "cart_token")]
        public string CartToken { get; set; }

        /// <summary>
        /// The name of the selling vendor.
        /// isango.com/EN / isango.com/DE / isango.com/ES / hop-on-hop-off-bus.com
        /// </summary>
        [JsonProperty(PropertyName = "vendor_name")]
        public string VendorName { get; set; }

        /// <summary>
        /// An unique id representing the selling vendor
        /// </summary>
        [JsonProperty(PropertyName = "vendor_id")]
        public string VendorId { get; set; }

        /// <summary>
        /// A list of discount code objects, each one containing information about an item in the order
        /// </summary>
        [JsonProperty(PropertyName = "discount_codes")]
        public List<DiscountCode> DiscountCodes { get; set; }

        /// <summary>
        /// Customer Data
        /// </summary>
        [JsonProperty(PropertyName = "customer")]
        public CustomerData Customer { get; set; }  // Customer

        /// <summary>
        /// A list of payment details. Should be passed as an array of nested payment_details objects. In cases where several payment methods are sent in the array, the first payment method shall correspond to the first billing address, etc
        /// </summary>
        [JsonProperty(PropertyName = "payment_details")]
        public List<PaymentDetail> PaymentDetails { get; set; }

        /// <summary>
        /// The mailing address associated with the payment method
        /// </summary>
        [JsonProperty(PropertyName = "billing_address")]
        public Address BillingAddress { get; set; }

        /// <summary>
        /// A list of passenger objects, each one containing information about a passenger in the order.
        /// </summary>
        [JsonProperty(PropertyName = "passengers")]
        public List<Passenger> Paseengers { get; set; }

        /// <summary>
        /// A list of line item objects, each one containing information about an item in the order
        /// </summary>
        [JsonProperty(PropertyName = "line_items")]
        public List<LineItem> LineItems { get; set; }

        [JsonProperty(PropertyName = "source")]
        public string Source { get; set; }

        [JsonProperty(PropertyName = "client_details")]
        public ClientDetail ClientDetails { get; set; }
    }

    public class Address
    {
        [JsonProperty(PropertyName = "last_name")]
        public string LastName { get; set; }

        [JsonProperty(PropertyName = "first_name")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "address1")]
        public string Address1 { get; set; }

        [JsonProperty(PropertyName = "address2")]
        public string Address2 { get; set; }

        [JsonProperty(PropertyName = "city")]
        public string City { get; set; }

        [JsonProperty(PropertyName = "country")]
        public string Country { get; set; }

        [JsonProperty(PropertyName = "country_code")]
        public string CountryCode { get; set; }

        [JsonProperty(PropertyName = "zip")]
        public string Zip { get; set; }

        [JsonProperty(PropertyName = "phone")]
        public string Phone { get; set; }
    }

    public class LineItem
    {
        [JsonProperty(PropertyName = "price")]
        public float Price { get; set; }

        [JsonProperty(PropertyName = "requires_shipping")]
        public bool RequireShipping { get; set; }

        [JsonProperty(PropertyName = "quantity")]
        public int Quantity { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "product_id")]
        public string ProductId { get; set; }

        [JsonProperty(PropertyName = "category")]
        public string Category { get; set; }

        [JsonProperty(PropertyName = "product_type")]
        public string ProductType { get; set; }

        [JsonProperty(PropertyName = "section")]
        public string Section { get; set; }

        [JsonProperty(PropertyName = "event_date")]
        public DateTime EventDate { get; set; }

        [JsonProperty(PropertyName = "city")]
        public string City { get; set; }

        /// <summary>
        /// FR
        /// </summary>
        [JsonProperty(PropertyName = "country_code")]
        public string CountryCode { get; set; }

        [JsonProperty(PropertyName = "latitude")]
        public string Latitude { get; set; }

        [JsonProperty(PropertyName = "longitude")]
        public string Longitude { get; set; }
    }

    public class DiscountCode
    {
        /// <summary>
        /// The amount of the discount
        /// </summary>
        [JsonProperty(PropertyName = "amount")]
        public float Amount { get; set; }

        /// <summary>
        /// The code of the discount
        /// </summary>
        [JsonProperty(PropertyName = "code")]
        public string Code { get; set; }
    }

    public class Passenger
    {
        [JsonProperty(PropertyName = "first_name")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "last_name")]
        public string LastName { get; set; }

        [JsonProperty(PropertyName = "passenger_type")]
        public string PassengerType { get; set; }

        [JsonProperty(PropertyName = "nationality_code")]
        public string NationalityCode { get; set; }
    }

    public class ShippingLine
    {
        /// <summary>
        /// The price of the shipping method.
        /// </summary>
        [JsonProperty(PropertyName = "price")]
        public float Price { get; set; }

        /// <summary>
        /// The title of the shipping method.
        /// </summary>
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }
    }

    public class PaymentDetail
    {
        /// <summary>
        /// The issuer identification number (IIN), formerly known as bank identification number (BIN) of the customer's credit card.
        ///Made up of the first 6 digits of the credit card number.
        /// </summary>
        [JsonProperty(PropertyName = "credit_card_bin")]
        public string CreditCardBin { get; set; }

        [JsonProperty(PropertyName = "credit_card_number")]
        public string CreditCardNumber { get; set; } // "XXXX-XXXX-XXXX-4242"

        [JsonProperty(PropertyName = "credit_card_company")]
        public string CreditCardCompany { get; set; } // "Visa/Master"

        //[JsonProperty(PropertyName = "cardholder_name")]
        //public string CardholderName { get; set; }

        //Model describing why a checkout was denied authorization. (use when calling checkout_denied endpoint)
        [JsonProperty(PropertyName = "authorization_error")]
        public AuthorizationError AuthorizationErrorData { get; set; }
    }

    public class CustomerData
    {
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        /// <summary>
        /// Indicates whether the merchant verified the customer’s email.
        /// </summary>
        [JsonProperty(PropertyName = "verified_email")]
        public bool VerifiedEmail { get; set; }

        [JsonProperty(PropertyName = "first_name")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "last_name")]
        public string LastName { get; set; }

        /// <summary>
        /// The timestamp of the initial registration of the customer's account in the merchant's systems
        /// </summary>
        [JsonProperty(PropertyName = "created_at")]
        public DateTime? CreatedAt { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "orders_count")]
        public int OrdersCount { get; set; }

        [JsonProperty(PropertyName = "account_type")]
        public string AccountType { get; set; }
    }

    // Technical information regarding the customer's browsing session.
    public class ClientDetail
    {
        /// <summary>
        /// List of two-letter language codes sent from the client
        /// </summary>
        [JsonProperty(PropertyName = "accept_language")]
        public string AcceptLanguage { get; set; }

        /// <summary>
        /// The full User-Agent sent from the client
        /// </summary>
        [JsonProperty(PropertyName = "user_agent")]
        public string UserAgent { get; set; }
    }
}