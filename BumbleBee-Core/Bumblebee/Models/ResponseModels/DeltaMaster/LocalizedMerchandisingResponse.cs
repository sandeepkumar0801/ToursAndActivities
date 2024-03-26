namespace WebAPI.Models.ResponseModels.DeltaMaster
{
	public class LocalizedMerchandisingResponse
	{
		public int AttractionID { get; set; }
		public string AttractionName { get; set; }

		public string LanguageCode { get; set; }
		public string MultilingualName { get; set; }
	}
}