using Newtonsoft.Json;

namespace ServiceAdapters.Apexx.Apexx.Entities
{
    public class EnrollmentCheckResponse
    {
        [JsonProperty(PropertyName = "_id")]
        public string TransactionId { get; set; }

        [JsonProperty(PropertyName = "created_at")]
        public string CreatedAt { get; set; }

        [JsonProperty(PropertyName = "three_ds")]
        public ThreeDs ThreeDs { get; set; }

        [JsonProperty(PropertyName = "reason_code")]
        public string ReasonCode { get; set; }

        [JsonProperty(PropertyName = "code")]
        public string Code { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }
    }

    public class ThreeDs
    {
        [JsonProperty(PropertyName = "three_ds_required")]
        public string ThreeDsRequired { get; set; }

        [JsonProperty(PropertyName = "acsURL")]
        public string AcsUrl { get; set; }

        [JsonProperty(PropertyName = "eci")]
        public string Eci { get; set; }

        [JsonProperty(PropertyName = "paReq")]
        public string PaReq { get; set; }

        [JsonProperty(PropertyName = "three_ds_enrollment")]
        public bool ThreeDsEnrollment { get; set; }

        [JsonProperty(PropertyName = "acq_id")]
        public string AcqId { get; set; }

        [JsonProperty(PropertyName = "psp_3d_id")]
        public string MD { get; set; }

        [JsonProperty(PropertyName = "acsURL_http_method")]
        public string AcsUrlHttpMethod { get; set; }
    }
}