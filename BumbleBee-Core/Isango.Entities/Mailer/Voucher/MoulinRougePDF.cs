namespace Isango.Entities.Mailer.Voucher
{
    public class MoulinRougePDF
    {
        public string BookingreRerenceNumber { get; set; }
        public int ServiceOptionId { get; set; }
        public string ApiOrderId { get; set; }
        public string ApiTicketGUWID { get; set; }
        public byte[] ApiVoucherByte { get; set; }
    }
}