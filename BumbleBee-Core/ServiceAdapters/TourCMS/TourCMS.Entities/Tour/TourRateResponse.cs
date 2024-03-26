using System.Collections.Generic;
using System.Xml.Serialization;

namespace ServiceAdapters.TourCMS.TourCMS.Entities.ChannelListResponse
{

    public class TourRateResponse
    {
        public int ChannelId { get; set; }
        public int AccountId { get; set; }
        public int TourId { get; set; }

        public string label_1 { get; set; }
        public string label_2 { get; set; }
        public string minimum { get; set; }

        public string maximum { get; set; }
        public string rate_id { get; set; }
        public string agecat { get; set; }
        public string agerange_min { get; set; }
        public string agerange_max { get; set; }
        public string from_price { get; set; }
        public string from_price_display { get; set; }
    }
}