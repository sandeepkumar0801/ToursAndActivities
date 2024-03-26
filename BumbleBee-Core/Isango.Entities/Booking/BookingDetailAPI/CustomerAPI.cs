using System;
using System.Data;

namespace Isango.Entities.Booking.BookingDetailAPI
{
    public class CustomerAPI
    {
        public string BookedOptionId { get; set; }

        public string PassengerType { get; set; }

        public string Age { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string IsLeadCustomer { get; set; }

        public CustomerAPI(IDataReader result)
        {
            PassengerType = Convert.ToString(result["ischild"]).Trim();
            Age = Convert.ToString(result["passengerage"]).Trim();
            FirstName = Convert.ToString(result["passengerfirstname"]).Trim();
            LastName = Convert.ToString(result["passengerlastname"]).Trim();
            IsLeadCustomer = Convert.ToString(result["isleadpassenger"]).Trim();
            BookedOptionId = Convert.ToString(result["bookedoptionid"]).Trim();
        }
    }
}