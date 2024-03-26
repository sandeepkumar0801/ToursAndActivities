using System.Collections.Generic;
using System.Xml.Serialization;

namespace ServiceAdapters.TourCMS.TourCMS.Entities.NewBooking
{
    [XmlRoot(ElementName = "booking")]
    public class NewBookingRequest
    {
        [XmlElement(ElementName = "total_customers")]
        public int TotalCustomers { get; set; }

        [XmlElement(ElementName = "booking_key")]
        public string BookingKey { get; set; }

        [XmlElement(ElementName = "agent_ref")]
        public string AgentReference { get; set; }
        
        [XmlElement(ElementName = "components")]
        public BookingComponents Components { get; set; }

        [XmlElement(ElementName = "customers")]
        public BookingCustomers Customers { get; set; }
       
    }
    public  class BookingComponents
    {
        [XmlElement(ElementName = "component")]
        public BookingComponentsComponent component { get; set; }
    }
    public  class BookingComponentsComponent
    {
        [XmlElement(ElementName = "component_key")]
        public string ComponentKey { get; set; }
        [XmlElement(ElementName = "note")]
        public string Note { get; set; }

        [XmlElement(ElementName = "replies")]
        public List<Reply> Replies { get; set; }

        [XmlElement(ElementName = "pickup_key")]
        public string PickupKey { get; set; }

        [XmlElement(ElementName = "options")]
        public Option Options { get; set; }
    }
    public class Option
    {
        [XmlElement(ElementName = "option")]
        public OptionsComponentKey option { get; set; }
    }
    public class OptionsComponentKey
    {
        [XmlElement(ElementName = "component_key")]
        public string ComponentKey { get; set; }
    }
    public class Reply
    {
        [XmlElement(ElementName = "reply")]
        public List<ReplyAnswers> Answers { get; set; }
    }
    public class ReplyAnswers
    {
        [XmlElement(ElementName = "question_key")]
        public string QuestionKey { get; set; }

        [XmlElement(ElementName = "answers")]
        public Answer Answers { get; set; }
    }
    public class Answer
    {
        [XmlElement(ElementName = "answer")]
        public AnswerValue AnswerData { get; set; }
    }
    public class AnswerValue
    {
        [XmlElement(ElementName = "answer_value")]
        public string AnswerValueData { get; set; }
    }

    public  class BookingCustomers
    {
        [XmlElement(ElementName = "customer")]
        public BookingCustomersCustomer customer { get; set; }
    }
    public class BookingCustomersCustomer
    {
        [XmlElement(ElementName = "title")]
        public string Title { get; set; }
        [XmlElement(ElementName = "firstname")]
        public string FirstName { get; set; }
        [XmlElement(ElementName = "surname")]
        public string Surname { get; set; }
        [XmlElement(ElementName = "email")]
        public string Email { get; set; }
        [XmlElement(ElementName = "tel_home")]
        public long TelHome { get; set; }
    }
}