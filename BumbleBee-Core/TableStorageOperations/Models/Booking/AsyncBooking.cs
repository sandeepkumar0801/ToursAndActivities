using TableStorageOperations.Models.AdditionalPropertiesModels;

namespace TableStorageOperations.Models.Booking
{
    public class AsyncBooking : CustomTableEntity
    {
        public string Id { get; set; }
        public int ApiType { get; set; }
        public string BookingReferenceNo { get; set; }
        public string Status { get; set; }
        public string OrderReferenceId { get; set; }
        public string LanguageCode { get; set; }
        public int RetryThreshold { get; set; }
        public int RetryCount { get; set; }
        public int RetryInterval { get; set; }
        public string NextProcessingTime { get; set; }
        public string CustomerEmail { get; set; }
        public string Token { get; set; }
        public string AvailabilityReferenceId { get; set; }
        public string OptionName { get; set; }
        public int ServiceOptionId { get; set; }
        public int BookedOptionId { get; set; }
        public string VoucherLink { get; set; }
        public int FailureEmailSentCount { get; set; }

        public string AffiliateId { get; set; }
        public string WebhookUrl { get; set; }
        public string WebhookRequest { get; set; }
        public string WebhookResponse { get; set; }
        public string IsWebhookSuccess { get; set; }
        public int? WebhookRetryCount { get; set; }

        public string ApiTypeMethod { get; set; }

        public string ApiDistributerId { get; set; }
    }
}