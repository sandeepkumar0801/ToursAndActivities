using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Isango.Entities.AdyenPayment
{
    public class AdyenWebhookResponse
    {
        [JsonProperty(PropertyName = "live")]
        public bool Live { get; set; }
        [JsonProperty(PropertyName = "notificationItems")]
        public List<NotificationItems> NotificationItems { get; set; }
    }

    public class NotificationItems
    {
        [JsonProperty(PropertyName = "notificationRequestItem")]
        public NotificationRequestItem NotificationRequestItem { get; set; }
    }

    public class NotificationRequestItem
    {
        [JsonProperty(PropertyName = "additionalData")]
        public AdditionalData AdditionalData { get; set; }
        [JsonProperty(PropertyName = "amount")]
        public Amount Amount { get; set; }
        [JsonProperty(PropertyName = "eventCode")]
        public string EventCode { get; set; }
        [JsonProperty(PropertyName = "eventDate")]
        public DateTime EventDate { get; set; }
        [JsonProperty(PropertyName = "merchantAccountCode")]
        public string MerchantAccountCode { get; set; }
        [JsonProperty(PropertyName = "originalReference")]
        public string OriginalReference { get; set; }
        [JsonProperty(PropertyName = "merchantReference")]
        public string MerchantReference { get; set; }
        [JsonProperty(PropertyName = "paymentMethod")]
        public string PaymentMethod { get; set; }
        [JsonProperty(PropertyName = "pspReference")]
        public string PspReference { get; set; }
        [JsonProperty(PropertyName = "reason")]
        public string Reason { get; set; }
        [JsonProperty(PropertyName = "success")]
        public bool Success { get; set; }
    }

    public class Amount
    {
        [JsonProperty(PropertyName = "currency")]
        public string Currency { get; set; }
        [JsonProperty(PropertyName = "value")]
        public int Value { get; set; }
    }

    public class AdditionalData
    {
        [JsonProperty(PropertyName = "hmacSignature")]
        public string HmacSignature { get; set; }

        [JsonProperty(PropertyName = "paymentLinkId")]
        public string PaymentLinkId { get; set; }
    }

    public class AdyenNotificationResponse
    {
        [JsonProperty(PropertyName = "notificationResponse")]
        public string NotificationResponse { get; set; }
    }
}
