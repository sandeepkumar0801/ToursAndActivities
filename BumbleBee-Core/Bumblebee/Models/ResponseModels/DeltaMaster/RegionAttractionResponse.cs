namespace WebAPI.Models.ResponseModels.DeltaMaster
{
    public class RegionAttractionResponse
    {
        public int RegionID { get; set; }
        public string RegionName { get; set; }
        public int AttractionID { get; set; }
        public string AttractionName { get; set; }
        public string Type_ { get; set; }
        public int Sequence { get; set; }
        public bool IsVisibleOnSearch { get; set; }
        public bool IsTopAttraction { get; set; }
        public int ImageID { get; set; }
        public string LanguageCode { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }
}