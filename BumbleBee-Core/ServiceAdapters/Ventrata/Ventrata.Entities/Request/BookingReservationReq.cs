using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAdapters.Ventrata.Ventrata.Entities.Request
{
    public class BookingReservationReq
    {
        [JsonProperty(PropertyName = "productId")]
        public string ProductId { get; set; }

        [JsonProperty(PropertyName = "optionId")]
        public string OptionId { get; set; }

        [JsonProperty(PropertyName = "availabilityId")]
        public string AvailabilityId { get; set; }

        [JsonProperty(PropertyName = "pickupRequested")]
        public bool IsPickUpRequested { get; set; }

        [JsonProperty(PropertyName = "pickupPointId")]
        public string PickUpPointId { get; set; }

        [JsonProperty(PropertyName = "unitItems")]
        public List<Unit> Units { get; set; }

        [JsonProperty(PropertyName = "questionAnswers")]
        public List<QuestionAnswer> QuestionAnswers { get; set; }
    }

    public class Unit
    {
        [JsonProperty(PropertyName = "unitId")]
        public string UnitId { get; set; }
        [JsonProperty(PropertyName = "questionAnswers")]
        public List<QuestionAnswer> QuestionAnswers { get; set; }
    }

    public class QuestionAnswer
    {
        [JsonProperty(PropertyName = "questionId")]
        public string QuestionId { get; set; }
        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }
    }

}









