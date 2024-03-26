using System;
using System.Collections.Generic;

namespace ServiceAdapters.HotelBeds.HotelBeds.Entities
{
    public class Purchase : EntityBase
    {
        public string PurchaseToken { get; set; }
        public long TimeToExpiration { get; set; }
        public string FileNumber { get; set; }
        public string IncomingOfficeCode { get; set; }
        public string Status { get; set; }
        public string AgencyCode { get; set; }
        public string AgencyBranch { get; set; }
        public string Language { get; set; }
        public DateTime CreationDate { get; set; }
        public string AgencyReference { get; set; }
        public string Currency { get; set; }
        public decimal TotalAmount { get; set; } // a.k.a TotalPrice in RSxml
        public List<ServiceList> ServiceList { get; set; }
        public PaymentData PaymentData { get; set; }
        public string HolderType { get; set; }
        public string HolderName { get; set; }
        public string HolderLastName { get; set; }
    }
}