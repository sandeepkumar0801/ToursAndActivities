namespace WebAPI.Models.ResponseModels.DeltaMaster
{
	public class DestinationResponse
	{
		public int DestinationId { get; set; }
		public string DestinationName { get; set; }
		public int CountryId { get; set; }
		public string CountryName { get; set; }
		public string LanguageCode { get; set; }
		public bool IsCountryChange { get; set; }

		public string Latitudes { get; set; }
		public string Longitudes { get; set; }
	}
}