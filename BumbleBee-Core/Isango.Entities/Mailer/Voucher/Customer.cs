using System;
using System.Data;

namespace Isango.Entities.Mailer.Voucher
{
    public class Customer
    {
        public string BookedOptionId { get; set; }
        public string CustomerType { get; set; }
        public string Age { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string IsLeadCustomer { get; set; }
        public int IsChild { get; set; }
        public string PassengerType { get; set; }

        public Customer(IDataReader result)
        {
            CustomerType = Convert.ToString(result["ischild"]).Trim();
            Age = Convert.ToString(result["passengerage"]).Trim();
            FirstName = Convert.ToString(result["passengerfirstname"]).Trim();
            LastName = Convert.ToString(result["passengerlastname"]).Trim();
            IsLeadCustomer = Convert.ToString(result["isleadpassenger"]).Trim();
            BookedOptionId = Convert.ToString(result["bookedoptionid"]).Trim();
            PassengerType = Convert.ToString(result["passengerType"]).Trim();
            IsChild = Convert.ToInt32(result["ischild"]);
        }
    }
}