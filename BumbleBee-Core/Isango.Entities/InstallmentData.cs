using Isango.Entities.Enums;
using System;
using System.Collections.Generic;

namespace Isango.Entities
{
    public class InstallmentData
    {
        public bool IsValid { get; set; }
        public string ReferenceNumber { get; set; }
        public PassengerInfo PassengerInfo { get; set; }
        public List<InstallmentDate> InstallmentDates { get; set; }
        public List<EmiBookingData> BookingData { get; set; }
        public bool IsLastEmi { get; set; }
    }

    public class PassengerInfo
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string PostCode { get; set; }
        public string CountryId { get; set; }
        public string CountryName { get; set; }
        public DateTime BookingDate { get; set; }
        public int BookingId { get; set; }
        public string UserId { get; set; }
        public int InstallmetId { get; set; }
        public int InstallmetNo { get; set; }
        public decimal InstallmentAmount { get; set; }
    }

    public class InstallmentDate
    {
        public int InstallmentId { get; set; }
        public DateTime InstallmentDates { get; set; }
        public int TransId { get; set; }
        public decimal Amount { get; set; }
        public int InstallmentNo { get; set; }
    }

    public class EmiBookingData
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public int Adult { get; set; }
        public int Child { get; set; }
        public string CurrencyCode { get; set; }
        public decimal Amount { get; set; }
        public DateTime DepartureDate { get; set; }
        public ActivityType Type { get; set; }
    }

    public class Installment
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public decimal Amount { get; set; }
        public bool Is3D { get; set; }
        public string CurrencyCode { get; set; }
        public string UserId { get; set; }
        public string BookingRefNo { get; set; }
        public CreditCard Card { get; set; }
        public string IpAddress { get; set; }
    }
}