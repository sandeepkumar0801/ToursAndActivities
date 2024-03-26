using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ServiceAdapters.PrioHub.PrioHub.Entities.ReservationRequest
{
    public class ReservationRequest
    {
        [JsonProperty("data")]
        public Data Data { get; set; }
    }
    public class Data
    {
        [JsonProperty("reservation")]
        public Reservation Reservation { get; set; }
    }
    public class Reservation
    {
        [JsonProperty("reservation_distributor_id")]
        public string ReservationDistributorId { get; set; }
        [JsonProperty("reservation_external_reference")]
        public string ReservationExternalReference { get; set; }
        [JsonProperty("reservation_details")]
        public List<ReservationDetails> ReservationDetails { get; set; }
    }
    public class ReservationDetails
    {
        [JsonProperty("booking_external_reference")]
        public string BookingExternalReference { get; set; }
        [JsonProperty("booking_language")]
        public string BookingLanguage { get; set; }
        [JsonProperty("product_id")]
        public string ProductId { get; set; }

        [JsonProperty("product_relation_id")]
        public string ProductRelationId { get; set; }

        [JsonProperty("product_availability_id")]
        public string ProductAvailabilityId { get; set; }
        [JsonProperty("product_type_details")]
        public List<ProductTypeDetails> ProductTypeDetails { get; set; }
        [JsonProperty("product_options")]
        public List<ProductOptions> ProductOptions { get; set; }
        [JsonProperty("product_pickup_point_id")]
        public string ProductPickupPointId { get; set; }

        [JsonProperty("product_pickup_point")]
        public ProductPickupPoint ProductPickupPoint { get; set; }

        [JsonProperty("product_combi_details")]
        public List<ProductCombiDetail> ProductCombiDetails { get; set; }
    }

    public class ProductOptions
    {
        [JsonProperty("option_id")]
        public string OptionId { get; set; }

        [JsonProperty("option_values")]
        public List<OptionsValues> OptionValues { get; set; }
    }

    public class OptionsValues
    {
        [JsonProperty("value_id")]
        public string ValueId { get; set; }

        [JsonProperty("value_count")]
        public string ValueCount { get; set; }
    }


        public class ProductCombiDetail
    {
        [JsonProperty("product_id")]
        public string ProductId { get; set; }

        [JsonProperty("product_availability_id")]
        public string ProductAvailabilityId { get; set; }

        //[JsonProperty("booking_travel_date")]
        //public string BookingTravelDate { get; set; }

       
    }

        public class ProductPickupPoint
    {
        [JsonProperty("pickup_point_id")]
        public string PickupPointId { get; set; }
        [JsonProperty("pickup_point_time")]
        public string PickupPointTime { get; set; }
    }
    public class ProductTypeDetails
    {
        [JsonProperty("product_type_id")]
        public string ProductTypeId { get; set; }
        [JsonProperty("product_type_count")]
        public int ProductTypeCount { get; set; }
    }
}