using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Review;
using System;
using System.Collections.Generic;

namespace WebAPI.Models.ResponseModels
{
    public class ContractQuestions
    {
        public int ServiceOptionId { get; set; }
        public List<Questions> Questions { get; set; }
    }

    public class Questions
    {
        public string QuestionId { get; set; }
        public string Label { get; set; }
        public bool? Required { get; set; }
        public bool? SelectFromOptions { get; set; }
        public string Description { get; set; }
        public int Assignedservicequestionid { get; set; }
        
        public int ServiceOptionQuestionStatus { get; set; }
        public List<Answers> Answers { get; set; }
    }

    public class Answers
    {
        
        public string Value { get; set; }
        public string Label { get; set; }
       
    }
}