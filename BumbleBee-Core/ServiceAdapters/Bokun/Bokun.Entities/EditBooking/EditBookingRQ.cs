using System.Collections.Generic;

namespace ServiceAdapters.Bokun.Bokun.Entities.EditBooking
{
    public class EditBookingRq
    {
        public string Type { get; set; }

        public int ActivityBookingId { get; set; }

        public bool DropOff { get; set; }

        public string Description { get; set; }

        public int PricingCategoryBookingId { get; set; }

        public int ExtraBookingId { get; set; }

        public int UnitCount { get; set; }

        public PricingCategoryBooking PricingCategoryBooking { get; set; }

        public int StartTimeId { get; set; }

        public int AnswerId { get; set; }

        public string Answer { get; set; }
    }

    public class PricingCategoryBooking
    {
        public int PricingCategoryId { get; set; }

        public int Age { get; set; }

        public bool LeadPassenger { get; set; }

        public PassengerInfo PassengerInfo { get; set; }

        public List<Extras> Extras { get; set; }
    }

    public class PassengerInfo
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }
    }

    public class Extras
    {
        public int ExtraId { get; set; }

        public int UnitCount { get; set; }

        public bool LeadPassenger { get; set; }

        public List<Answers> Answers { get; set; }
    }

    public class Answers
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Answer { get; set; }
        public int QuestionId { get; set; }
    }
}