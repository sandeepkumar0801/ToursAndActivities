namespace WebAPI.Models.ResponseModels
{
	/// <summary>
	/// 
	/// </summary>
	public class ActivityDetailsResponse
	{
		/// <summary>
		/// 
		/// </summary>
		public ActivityDetails Activity { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public CalendarResponse CalendarAvailability { get; set; }
	}
}