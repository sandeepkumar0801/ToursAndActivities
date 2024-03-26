namespace WebAPI.Models.ResponseModels.DeltaMaster
{
	public class GeoDetailsResponse
	{
		public int? ContinentRegionId { get; set; }
		public string ContinentName { get; set; }
		public string ContinentRegionCode { get; set; }
		public int? CountryRegionId { get; set; }
		public string CountryName { get; set; }
		public string CountryRegionCode { get; set; }
		public int DestinationRegionId { get; set; }
		public string DestinationName { get; set; }
		public string DestinationRegionCode { get; set; }
		public string Latitudes { get; set; }
		public string Longitudes { get; set; }
		public bool IsCountryChange { get; set; }
	}
}