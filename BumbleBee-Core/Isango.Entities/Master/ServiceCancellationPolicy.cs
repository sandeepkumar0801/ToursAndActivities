namespace Isango.Entities.Master
{
    public class ServiceCancellationPolicy
    {
        public int ServiceId { get; set; }
        public int CutOffDays { get; set; }
        public bool IsFixed { get; set; }
        public decimal CancellationAmount { get; set; }
    }
}