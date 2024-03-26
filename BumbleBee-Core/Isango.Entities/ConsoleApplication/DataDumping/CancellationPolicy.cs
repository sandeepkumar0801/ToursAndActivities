namespace Isango.Entities.ConsoleApplication.DataDumping
{
    public class CancellationPolicy : CustomTableEntity
    {
        public string TokenId { get; set; }
        public int ActivityId { get; set; }
        public int OptionId { get; set; }
        public string CancellationPrices { get; set; }
    }
}