using System;

namespace Isango.Entities
{
    public class APIContractAnswers
    {
        public int Serviceid { get; set; }
        public int ServiceOptionid { get; set; }
        public string QuestionId { get; set; }
        public string Value { get; set; }
        public string Label { get; set; }
        public bool? AnswerStatus { get; set; }
    }
}