using System;
using System.Collections.Generic;

namespace Isango.Entities.GoogleMaps
{
    public class ExtraDetail
    {
        public List<PaymentExtraDetail> PaymentExtraDetails { get; set; }
        public string TokenId { get; set; }
    }

    public class PaymentExtraDetail
    {
        public int ActivityId { get; set; }
        public int OptionId { get; set; }
        public string Variant { get; set; }
        public TimeSpan StartTime { get; set; }
        public List<Question> Questions { get; set; }
        public Dictionary<int, string> PickupLocations { get; set; }
        public Dictionary<int, string> DropoffLocations { get; set; }
    }

    public class Question
    {
        public string Id { get; set; }
        public string Label { get; set; }
        public bool Required { get; set; }
        public bool SelectFromOptions { get; set; }
        public string DataType { get; set; }
        public string DefaultValue { get; set; }
        public List<AnswerOption> AnswerOptions { get; set; }
        public string QuestionType { get; set; } // added this property to know where does this question belong in the Submitcheckout request...
    }

    public class AnswerOption
    {
        public string Value { get; set; }
        public string Label { get; set; }
    }
}