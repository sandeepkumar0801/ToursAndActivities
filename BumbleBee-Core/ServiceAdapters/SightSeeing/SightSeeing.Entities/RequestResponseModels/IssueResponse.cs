namespace ServiceAdapters.SightSeeing.SightSeeing.Entities.RequestResponseModels
{
    public class IssueResponse
    {
        public bool Success { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public Response[] Response { get; set; }
    }
}