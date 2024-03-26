using System.Collections.Generic;

namespace Isango.Entities.MyIsango
{
    public class MyUserEmailPreferences
    {
        public int QuestionId { get; set; }

        public string QuestionText { get; set; }

        public int QuestionOrder { get; set; }

        public int UserPreferredAnswer { get; set; }

        public List<MyUserAnswer> MyUserAnswers { get; set; }
    }
}