namespace WebAPI.Models.ResponseModels.DeltaActivity
{
	public class BundleResponse
	{
		public int ServiceId { get; set; }
		public string ServiceName { get; set; }
		public bool IsSameDayBookable { get; set; }
	}
}