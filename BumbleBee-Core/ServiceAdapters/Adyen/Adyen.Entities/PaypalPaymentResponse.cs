using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAdapters.Adyen.Adyen.Entities
{
    public class PaypalPaymentResponse
    {
        [JsonProperty(PropertyName = "orderID")]
        public string OrderID { get; set; }
        [JsonProperty(PropertyName = "payerID")]
        public string PayerID { get; set; }
        [JsonProperty(PropertyName = "paymentID")]
        public string PaymentID { get; set; }
        [JsonProperty(PropertyName = "billingToken")]
        public string BillingToken { get; set; }
        [JsonProperty(PropertyName = "facilitatorAccessToken")]
        public string FacilitatorAccessToken { get; set; }
    }
}
