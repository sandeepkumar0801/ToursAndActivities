namespace ServiceAdapters
{
    public interface ILog
    {
        void Write(string request, string response, string methodName, string token, string apiName);
        void WriteTimer(string methodName, string token, string apiName, string elapsedTime);
    }
}
