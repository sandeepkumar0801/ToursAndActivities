using Isango.Entities.Enums;
using System;

namespace Isango.Entities
{
    public class Customer
    {
        public int CustomerId { get; set; }
        public PassengerType PassengerType { get; set; }
        public int Age { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsLeadCustomer { get; set; }
        public DateTime ChildDob { get; set; }
        public string Title { get; set; }
        public string Email { get; set; }
        public string PassportNumber { get; set; }
        public string PassportNationality { get; set; }

        public string AgeSupplier { get; set; }
    }
}