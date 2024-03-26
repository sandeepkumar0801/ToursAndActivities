using Newtonsoft.Json;

namespace WebAPI.Models.Validation
{
	/// <summary>
	/// 
	/// </summary>
	public class ValidationError
	{
		/// <summary>
		/// 
		/// </summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string Field { get; }

		/// <summary>
		/// 
		/// </summary>
		public string Message { get; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="field"></param>
		/// <param name="message"></param>
		public ValidationError(string field, string message)
		{
			Field = field != string.Empty ? field : null;
			Message = message;
		}
	}

}