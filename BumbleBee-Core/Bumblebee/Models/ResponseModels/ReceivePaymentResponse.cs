using Isango.Entities;
using Isango.Entities.Enums;

namespace WebAPI.Models.ResponseModels
{
    public class ReceivePaymentResponse
    {
        public string Message { get; set; }
        public string Status { get; set; }
        public string ReferenceId { get; set; }
        public GatewayDetail GatewayDetail { get; set; }
        public string Guwid { get; set; }
    }
}