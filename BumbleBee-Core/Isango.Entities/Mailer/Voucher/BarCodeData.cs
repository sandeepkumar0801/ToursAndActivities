namespace Isango.Entities.Mailer.Voucher
{
    public class BarCodeData
    {
        public string BookedOptionId { get; set; }
        public string BarCode { get; set; }
        public string FiscalNumber { get; set; }
        public string CodeType { get; set; }
        public string CodeValue { get; set; }
        public string ResourceType { get; set; }
        public string ResouceRemoteUrl { get; set; }
        public string ResourceLocal { get; set; }
        public string PassengerType { get; set; }
        public int? PassengerCount { get; set; }
        public bool IsResourceApply { get; set; }

        public string ShowValueOnly { get; set; }
    }
}