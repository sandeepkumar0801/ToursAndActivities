using System.Collections.Generic;

namespace Isango.Entities.Bokun
{
    public class Question
    {
        public string QuestionId { get; set; }
        public string Label { get; set; }
        public string DataType { get; set; }
        public bool Required { get; set; }
        public string DefaultValue { get; set; }
        public bool SelectFromOptions { get; set; }
        public bool SelectMultiple { get; set; }
        public string DataFormat { get; set; }
        public string QuestionCode { get; set; }
        public string Help { get; set; }
        public string Placeholder { get; set; }
        public string Pattern { get; set; }
        public string OriginalQuestion { get; set; }
        public List<string> Flags { get; set; }
        public List<string> Answers { get; set; }
        public List<AnswerOption> AnswerOptions { get; set; }
        public string QuestionType { get; set; } // added this property to know where does this question belong in the Submitcheckout request...
    }

    public class AnswerOption
    {
        public string Value { get; set; }
        public string Label { get; set; }
    }
}