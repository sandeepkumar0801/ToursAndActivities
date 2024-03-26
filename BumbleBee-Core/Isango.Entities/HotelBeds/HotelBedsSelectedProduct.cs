using Isango.Entities.Enums;
using System;
using System.Collections.Generic;

namespace Isango.Entities.HotelBeds
{
    public class HotelBedsSelectedProduct : SelectedProduct
    {
        public string EchoToken { get; set; }

        public string PurchaseToken { get; set; }

        public DateTime TimeToExpiration { get; set; }

        public string SPUI { get; set; }

        public string SupplierName { get; set; }

        public string VatNumber { get; set; }

        public string CartStatus { get; set; }

        public string ServiceStatus { get; set; }

        public string FileNumber { get; set; }

        public string OfficeCode { get; set; }

        public string HolderFirstName { get; set; }

        public string HolderLastName { get; set; }

        public float StarRating { get; set; }

        public string HotelBedsId { get; set; }

        /// <summary>
        /// For use by HotelBeds.Ticket (Even at option level, use this property!)
        /// </summary>
        public Contract ServiceContract { get; set; }

        public string Language { get; set; }

        public List<ContractQuestion> ContractQuestions { get; set; }

        public int BookedOptionId { get; set; }

        public string ShoppingCartId { get; set; }

        public int ShoppingCartOperationId { get; set; }

        public List<BookedSeat> BookedSeats { get; set; }

        public List<BookingVoucher> BookingVouchers { get; set; }

        //Added for new HoletBedApi v3
        /// <summary>
        /// Booking reference at api end i.e 123-456789
        /// </summary>
        public string BookingReferenceAPI { get; set; }

        /// <summary>
        /// Booking reference that is passed while creating booking using api i.e ISangoTest001
        /// </summary>
        public string BookingReferenceCustomer { get; set; }

        public AvailabilityStatus AvailabilityStatus { get; set; }

        public string ShowTime { get; set; }

        public string Code { get; set; }

        public string Destination { get; set; }

        public int FactsheetId { get; set; }

        public string Inclusions { get; set; }

        public string Exclusions { get; set; }

        /// <summary>
        /// if true then isango voucher with HB QR Image.
        /// if false then HotelBeds Supplier PDF will be used as redeemable voucher.
        /// IF NULL then Isango can provide its desired voucher as pdf or html
        /// </summary>
        public bool? IsVocuherCustomizable { get; set; }

        public string ProviderInformation { get; set; }
    }
}