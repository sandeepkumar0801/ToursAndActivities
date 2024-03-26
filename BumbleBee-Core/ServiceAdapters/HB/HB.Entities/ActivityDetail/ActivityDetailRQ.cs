using Newtonsoft.Json;
using System.Collections.Generic;

namespace ServiceAdapters.HB.HB.Entities.ActivityDetail
{
    public class ActivityRq
    {
        public ActivityRq()
        {
            Paxes = new List<Pax>();
        }

        public ActivityRq(string activityCode, string fromDate, string toDate, string language, List<Pax> lstPaxes, string modalityCode = null)
        {
            Code = activityCode;
            From = fromDate;
            To = toDate;
            Language = language;
            Paxes = lstPaxes;
            ModalityCode = modalityCode;
        }

        /// <summary>
        /// Activity Code i.e "E-E10-HIGHARTIST"
        /// </summary>
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("from")]
        public string From { get; set; }

        [JsonProperty("to")]
        public string To { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty("paxes")]
        public List<Pax> Paxes { get; set; }

        /// <summary>
        /// Modality Code i.e "CAT2"
        /// </summary>
        [JsonProperty("modalityCode")]
        public string ModalityCode { get; set; }
    }

    public class Pax
    {
        public Pax()
        {
        }

        public Pax(int age)
        {
            Age = age;
        }

        [JsonProperty("age")]
        public int Age { get; set; }
    }
}