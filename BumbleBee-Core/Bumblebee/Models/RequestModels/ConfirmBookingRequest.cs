using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models.RequestModels
{
    public class ConfirmBookingRequest
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public int BookedOptionId { get; set; }

        [Required]
        public string TokenId { get; set; }
    }
}