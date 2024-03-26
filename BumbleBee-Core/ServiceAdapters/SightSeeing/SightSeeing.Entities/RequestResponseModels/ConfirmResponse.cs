namespace ServiceAdapters.SightSeeing.SightSeeing.Entities.RequestResponseModels
{
    public class ConfirmResponse
    {
        public bool Success { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}