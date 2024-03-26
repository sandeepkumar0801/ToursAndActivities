using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Isango.Entities.Booking.RequestModels
{
    public class GeneratePaymentLinkRequest
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Amount { get; set; }
        [Required]
        public string Currency { get; set; }
        [Required]
        public string EmailLanguage { get; set; }
        public string CountryCode { get; set; }
        public string ShopperLocale { get; set; }
    }
}