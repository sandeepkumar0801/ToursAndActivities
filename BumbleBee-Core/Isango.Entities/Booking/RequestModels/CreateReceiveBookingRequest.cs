using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Isango.Entities.Booking.RequestModels
{
    public class CreateReceiveBookingRequest : CreateBookingRequest
    {
        [Required]
        public int AmendmentId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public string BookingReferenceNumber { get; set; }

        [Required]
        public decimal ChargeAmount { get; set; }

        public CreateReceiveBookingRequest()
        {
            SelectedProducts = new List<SelectedProduct>();
        }

        public string AdyenMerchantAccout { get; set; }
    }
}