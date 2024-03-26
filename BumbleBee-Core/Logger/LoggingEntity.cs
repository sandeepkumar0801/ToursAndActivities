using Microsoft.WindowsAzure.Storage.Table;

namespace Logger
{
    public class AdapterLoggingEntity : TableEntity
    {
        public string Request { get; set; }
        public string Response { get; set; }
        public string Method { get; set; }
        public string Token { get; set; }
        public string ApiName { get; set; }
    }

    public class TimerLoggingEntity : TableEntity
    {
        public string ElapsedTime { get; set; }
        public string Method { get; set; }
        public string Token { get; set; }
        public string ApiName { get; set; }
    }
}