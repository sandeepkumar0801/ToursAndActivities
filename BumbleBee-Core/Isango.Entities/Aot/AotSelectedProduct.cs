// ReSharper disable All
namespace Isango.Entities.Aot
{
    public class AotSelectedProduct : SelectedProduct
    {
        public string ServiceLineId { get; set; }
        public new string Status { get; set; }
        public string Currency { get; set; }
        public string SequenceNumber { get; set; }
        public decimal Amount { get; set; }
        public string OptCode { get; set; }
        public string RoomType { get; set; }
        public string AotOptionType { get; set; }
        public string PuRemark { get; set; }
        public string PuTime { get; set; }
        public string Code { get; set; }
        public string Destination { get; set; }
        public int FactsheetId { get; set; }
    }
}