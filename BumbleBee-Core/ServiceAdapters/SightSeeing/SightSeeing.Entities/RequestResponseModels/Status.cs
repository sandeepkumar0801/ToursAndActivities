namespace ServiceAdapters.SightSeeing.SightSeeing.Entities.RequestResponseModels
{
    public class Status
    {
        public bool Success { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}