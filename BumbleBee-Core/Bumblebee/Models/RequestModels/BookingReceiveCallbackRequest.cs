using Isango.Entities.Booking.RequestModels;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models.RequestModels
{
    public class BookingReceiveCallbackRequest
    {
        [Required]
        public string GuWid { get; set; }

        [Required]
        public string PaRes { get; set; }

        [Required]
        public string TokenId { get; set; }

        [Required]
        public Card CardDetails { get; set; }
    }
}