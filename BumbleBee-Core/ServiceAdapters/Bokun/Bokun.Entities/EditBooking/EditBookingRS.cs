using System.Collections.Generic;

namespace ServiceAdapters.Bokun.Bokun.Entities.EditBooking
{
    public class EditBookingRs
    {
        public List<Action> Actions { get; set; }
        public string Status { get; set; }
    }

    public class Action
    {
        public string Type { get; set; }
        public string Status { get; set; }
        public string StatusMessage { get; set; }
        public object RequestCallbackUrl { get; set; }
        public object RequestDeadline { get; set; }
        public int ActivityBookingId { get; set; }
        public int PricingCategoryBookingId { get; set; }
    }
}