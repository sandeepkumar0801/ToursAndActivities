namespace Isango.Entities.Booking
{
    public class TransactionResponse
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorCode { get; set; }
        public bool? IsAdyen { get; set; }
        public bool? IsWebHookRecieved { get; set; }
    }
}