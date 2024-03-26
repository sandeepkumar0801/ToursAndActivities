using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAdapters.GlobalTix.GlobalTix.Entities.RequestResponseModels
{
    public class TicketTypeRS
    {
        [JsonProperty(PropertyName = "data")]
        public TicketTypeDetail Data { get; set; }
		[JsonProperty(PropertyName = "error")]
		public Error Error { get; set; }
		[JsonProperty(PropertyName = "success")]
		public bool IsSuccess { get; set; }
	}

	public class TicketTypeDetail : IdentifierWithName
    {
		[JsonProperty(PropertyName = "questions")]
		public List<Question> Questions { get; set; }

        [JsonProperty(PropertyName = "toAge")]
        public int ToAge { get; set; }

        [JsonProperty(PropertyName = "fromAge")]
        public int FromAge { get; set; }

        [JsonProperty(PropertyName = "originalPrice")]
        public decimal OriginalPrice { get; set; }

        [JsonProperty(PropertyName = "payableAmount")]
        public decimal PayableAmount { get; set; }

        [JsonProperty(PropertyName = "currency")]
        public string Currency { get; set; }

        [JsonProperty(PropertyName = "variation")]
        public EnumValue Variation { get; set; }

        [JsonProperty(PropertyName = "minimumSellingPrice")]
        public string MinimumSellingPrice { get; set; }

        [JsonProperty(PropertyName = "cancellationNotesSettings")]
        public List<CancellationNotes> CancellationNotesSettings { get; set; }

    }

    public class CancellationNotes
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "isActive")]
        public bool IsActive { get; set; }

        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }

    }

}
