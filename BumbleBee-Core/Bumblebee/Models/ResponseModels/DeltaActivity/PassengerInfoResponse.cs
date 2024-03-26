namespace WebAPI.Models.ResponseModels.DeltaActivity
{
	public class PassengerInfoResponse
	{
		public int ServiceID { get; set; }
		public int PassengerTypeID { get; set; }
		public int FromAge { get; set; }
		public int ToAge { get; set; }
		public int MinSize { get; set; }
		public int MaxSize { get; set; }
		public string PaxDesc { get; set; }
		public bool IsIndependablePax { get; set; }
		public string Label { get; set; }
		public string MeasurementDesc { get; set; }
	}
}