using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models.RequestModels
{
    public class OnlyAPICancellationRequest
    {
        [Required] 
        public string BookingRefNo { get; set; }

        [Required]
        public int BookedOptionId { get; set; }

        [Required]
        public string UserName { get; set; }

    }
}