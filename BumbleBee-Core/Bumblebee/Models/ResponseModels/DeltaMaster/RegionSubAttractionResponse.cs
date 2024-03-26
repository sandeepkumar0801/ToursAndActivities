namespace WebAPI.Models.ResponseModels.DeltaMaster
{
	public class RegionSubAttractionResponse
	{
		public int RegionId { get; set; }
		public string RegionName { get; set; }
		public int ParentAttractionId { get; set; }
		public string ParentAttractionName { get; set; }
		public string Type { get; set; }
		public int? SubAttractionId { get; set; }
		public string SubAttractionName { get; set; }
		public int SubFilterOrder { get; set; }
		public bool IsVisible { get; set; }
	}
}