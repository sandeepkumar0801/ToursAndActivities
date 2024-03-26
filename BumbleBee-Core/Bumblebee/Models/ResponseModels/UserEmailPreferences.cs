using Isango.Entities.MyIsango;
using System.Collections.Generic;

namespace WebAPI.Models.ResponseModels
{
	public class UserEmailPreferences
	{
		public string QuestionText { get; set; }

		public int QuestionOrder { get; set; }

		public int UserPreferredAnswer { get; set; }

		public List<MyUserAnswer> MyUserAnswers { get; set; }
	}
}