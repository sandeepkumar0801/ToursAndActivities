using System;

namespace Isango.Entities
{
    public class APIContractQuestion
    {
        
        public string QuestionId { get; set; }
        public string Label { get; set; }
        public bool? Status { get; set; }
        public bool? Required { get; set; }
        public bool? SelectFromOptions { get; set; }
        public string Description { get; set; }
        public int Assignedservicequestionid { get; set; }
        public int Serviceid { get; set; }
        public int ServiceOptionid { get; set; }
        public int ServiceOptionQuestionStatus { get; set; }
     }
}