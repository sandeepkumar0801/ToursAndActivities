using System;

namespace Isango.Entities.ConsoleApplication.AgeGroup.FareHarbor
{
    public class CustomerProtoTypeCustomerType
    {
        public int Pk { get; set; }
        public decimal Total { get; set; }
        public string DisplayName { get; set; }
        public int ServiceId { get; set; }
        public string Note { get; set; }
        public int CustomerTypePk { get; set; }
        public string Plural { get; set; }
        public string Singular { get; set; }
        public int TourPk { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public int? MinPartySize { get; set; }
        public int? MaxPartySize { get; set; }
        public int? TourMinPartySize { get; set; }
        public int? TourMaxPartySize { get; set; }

        public CustomerProtoTypeCustomerType()
        {
        }

        public CustomerProtoTypeCustomerType(CustomerPrototype customerPrototypes, CustomerType customerType, int serviceId, int tourPk, DateTime startAt, DateTime endAt)
        {
            ServiceId = serviceId;
            Total = customerPrototypes.Total;
            Pk = customerPrototypes.Pk;
            DisplayName = customerPrototypes.DisplayName;
            CustomerTypePk = customerType.Pk;
            Note = customerType.Note;
            Plural = customerType.Plural;
            Singular = customerType.Singular;
            TourPk = tourPk;
            StartAt = startAt;
            EndAt = endAt;
        }
    }
}