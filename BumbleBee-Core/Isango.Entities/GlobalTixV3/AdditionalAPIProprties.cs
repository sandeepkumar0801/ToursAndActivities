using Isango.Entities.Bokun;
using System;
using System.Collections.Generic;

namespace Isango.Entities.GlobalTixV3
{
    public class PickupPointsForGlobalTix3
    {
        public int Id { get; set; }
        public string Options { get; set; }
        public string Question { get; set; }
        public bool IsRequired { get; set; }
    }

    public class ContractQuestionsForGlobalTix3
    {
        public string Id { get; set; }
        public List<AnswerOptionForGlobalTix3> AnswerOptions { get; set; }
        public bool Optional { get; set; }
        public string Question { get; set; }
        public string Type { get; set; }
        public string Code { get; set; }
     }
    public class AnswerOptionForGlobalTix3
    {
        public string Value { get; set; }
        public string Label { get; set; }
    }
}