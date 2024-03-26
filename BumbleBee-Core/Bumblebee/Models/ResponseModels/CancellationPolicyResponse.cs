using System;

namespace WebAPI.Models.ResponseModels
{
	public class CancellationPolicyResponse
	{
		public string CancellationPolicy { get; set; }
		public decimal Amount { get; set; }
		public DateTime FromDate { get; set; }
		public DateTime ToDate { get; set; }
		public DateTime TravelDate { get; set; }
		public bool IsCancel { get; set; }
	}
}