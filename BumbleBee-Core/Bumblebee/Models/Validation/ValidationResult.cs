using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Linq;

namespace WebAPI.Models.Validation
{
	/// <summary>
	/// 
	/// </summary>
	public class ValidationResult
	{
		/// <summary>
		/// 
		/// </summary>
		public string Message { get; }

		/// <summary>
		/// 
		/// </summary>
		public List<ValidationError> Errors { get; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="modelState"></param>
		public ValidationResult(ModelStateDictionary modelState)
		{
			Message = "Validation Failed";
			Errors = modelState.Keys
				.SelectMany(key => modelState[key].Errors.Select(x => new ValidationError(key, x.ErrorMessage)))
				.ToList();
		}
	}
}