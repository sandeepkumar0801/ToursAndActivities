using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Net;

namespace WebAPI.Models.Validation
{
	/// <summary>
	/// 
	/// </summary>
	public class ValidationFailedResult
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="modelState"></param>
		public ValidationFailedResult(ModelStateDictionary modelState)
		{
			StatusCode = HttpStatusCode.BadRequest;
			Value = new ValidationResult(modelState);
		}

		public HttpStatusCode StatusCode { get; set; }

		public ValidationResult Value { get; set; }
	}
}