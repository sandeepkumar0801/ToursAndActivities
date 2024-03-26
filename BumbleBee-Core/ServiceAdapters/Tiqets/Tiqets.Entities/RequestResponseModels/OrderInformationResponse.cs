using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAdapters.Tiqets.Tiqets.Entities.RequestResponseModels
{
    public class OrderInformationResponse
    {
        [JsonProperty(PropertyName = "product_id")]
        public int ProductId { get; set; }

        [JsonProperty(PropertyName = "order_reference_id")]
        public string OrderReferenceId { get; set; }

        [JsonProperty(PropertyName = "day")]
        public string Day { get; set; }

        [JsonProperty(PropertyName = "timeslot")]
        public string TimeSlot { get; set; }

        [JsonProperty(PropertyName = "variants")]
        public List<OrderInfoVariant> Variant { get; set; }

        [JsonProperty(PropertyName = "customer_details")]
        public OrderInfoCustomerDetails CustomerDetails { get; set; }

        [JsonProperty(PropertyName = "order_status")]
        public string OrderStatus { get; set; }

        [JsonProperty(PropertyName = "is_currently_cancellable")]
        public bool IsCancellable { get; set; }

        [JsonProperty(PropertyName = "cancellation_deadline")]
        public string CancellationDeadline { get; set; }

        [JsonProperty(PropertyName = "external_reference")]
        public string ExternalReference { get; set; }
    }

    public class OrderInfoVariant
    {
        [JsonProperty(PropertyName = "count")]
        public int Count { get; set; }

        [JsonProperty(PropertyName = "variant_id")]
        public int VariantId { get; set; }
    }

    public class OrderInfoCustomerDetails
    {
        [JsonProperty(PropertyName = "firstname")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "lastname")]
        public string LastName { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "phone")]
        public string Phone { get; set; }
        
    }
}
