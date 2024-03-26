using System;

namespace WebAPI.Models.ResponseModels.DeltaActivity
{
	public class ReviewResponse
	{
		public string Title { get; set; }
		public string Rating { get; set; }
		public string Text { get; set; }
		public string UserName { get; set; }
		public string Country { get; set; }
		public string ServiceId { get; set; }
		public DateTime SubmittedDate { get; set; }
		public bool IsFeefo { get; set; }
	}
}