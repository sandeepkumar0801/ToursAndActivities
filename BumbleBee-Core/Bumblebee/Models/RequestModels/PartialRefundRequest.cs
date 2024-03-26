using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models.RequestModels
{
    public class PartialRefundRequest
    {
        [Required]
        public int AmendmentId { get; set; }
        [Required]
        public string Remarks { get; set; }
        [Required]
        public string ActionBy { get; set; }
        [Required]
        public string TokenId { get; set; }
    }
}