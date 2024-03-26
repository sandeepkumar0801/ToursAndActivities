using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models.RequestModels
{
    public class CancellationRequest
    {
        [Required] 
        public string BookingRefNo { get; set; }

        [Required] 
        public string UserName { get; set; }

        public string TokenId { get; set; }
        public CancellationParameters CancellationParameters { get; set; }

        public bool IsBookingManager { get; set; }
    }

    public class CancellationParameters
    {
        [Required] 
        public int BookedOptionId { get; set; }

        [Required] 
        public decimal UserRefundAmount { get; set; }

        [JsonIgnore]
        public decimal SupplierRefundAmount { get; set; }

        public string Reason { get; set; }
        public string AlternativeTours { get; set; }
        public string SupplierNotes { get; set; }
        public string CustomerNotes { get; set; }
        public List<string> AlternativeDates { get; set; }
    }
}