namespace ServiceAdapters.Ventrata.Ventrata.Entities
{
    public class CancellationDetail
    {
        public object CancellationPolicy { get; set; }
        public string CancellationCutoff { get; set; }
        public string CancellationCutoffAmount { get; set; }
        public string CancellationCutoffUnit { get; set; }
        public bool IsCancellable { get; set; }
    }
}