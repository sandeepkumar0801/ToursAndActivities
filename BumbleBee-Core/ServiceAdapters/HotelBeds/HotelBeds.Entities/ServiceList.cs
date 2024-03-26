using ServiceAdapters.HotelBeds.HotelBeds.Entities.Tickets;
using System;
using System.Collections.Generic;

namespace ServiceAdapters.HotelBeds.HotelBeds.Entities
{
    public class ServiceList : EntityBase
    {
        public string Spui { get; set; }
        public string FileNumber { get; set; }
        public string IncomingOfficeCode { get; set; }
        public string Status { get; set; }
        public HBSupplier Supplier { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public string Currency { get; set; }
        public decimal TotalAmount { get; set; }
        public Contract Contract { get; set; }
        public HotelInfo HotelInfo { get; set; }
        public TicketInfo TicketInfo { get; set; }
        public List<AdditionalCost> AdditionalCosts { get; set; }
        public List<string> ModificationPolicy { get; set; }
        public List<Comment> Comments { get; set; }
        public AvailableRoom AvailableRoom { get; set; }
        public AvailableModality AvailableModality { get; set; }
        public Paxes PassengerDetails { get; set; }
        public CancellationPolicy CancellationPolicy { get; set; }
        public List<CancellationPolicy> CancellationCharges { get; set; }
        public List<ServiceDetail> ServiceDetails { get; set; }
        public List<Seating> SeatingDetails { get; set; }
        public List<Voucher> VoucherDetails { get; set; }
    }
}