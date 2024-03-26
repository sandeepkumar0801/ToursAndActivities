using Newtonsoft.Json;

namespace ServiceAdapters.Rezdy.Rezdy.Entities.Availability
{
    public class AvailabilityResponse
    {
        [JsonProperty("requestStatus")]
        public RequestStatus RequestStatus { get; set; }

        [JsonProperty("sessions")]
        public Session[] Sessions { get; set; }
    }

    public class RequestStatus
    {
        [JsonProperty("error")]
        public Error Error { get; set; }

        [JsonProperty("success")]
        public string Success { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("warning")]
        public Warning Warning { get; set; }
    }

    public class Error
    {
        [JsonProperty("errorCode")]
        public string ErrorCode { get; set; }

        [JsonProperty("errorMessage")]
        public string ErrorMessage { get; set; }
    }

    public class Warning
    {
        [JsonProperty("warningMessage")]
        public string WarningMessage { get; set; }
    }

    public class Session
    {
        [JsonProperty("allDay")]
        public string AllDay { get; set; }

        [JsonProperty("endTime")]
        public string EndTime { get; set; }

        [JsonProperty("endTimeLocal")]
        public string EndTimeLocal { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("priceOptions")]
        public PriceOption[] PriceOptions { get; set; }

        [JsonProperty("productCode")]
        public string ProductCode { get; set; }

        [JsonProperty("seats")]
        public string Seats { get; set; }

        [JsonProperty("seatsAvailable")]
        public string SeatsAvailable { get; set; }

        [JsonProperty("startTime")]
        public string StartTime { get; set; }

        [JsonProperty("startTimeLocal")]
        public string StartTimeLocal { get; set; }
    }

    public class PriceOption
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("maxQuantity")]
        public string MaxQuantity { get; set; }

        [JsonProperty("minQuantity")]
        public string MinQuantity { get; set; }

        [JsonProperty("price")]
        public string Price { get; set; }

        [JsonProperty("priceGroupType")]
        public string PriceGroupType { get; set; }

        [JsonProperty("productCode")]
        public string ProductCode { get; set; }

        [JsonProperty("seatsUsed")]
        public string SeatsUsed { get; set; }
    }
}