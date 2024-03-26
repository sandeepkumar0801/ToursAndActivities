using System;

namespace ServiceAdapters.RedeamV12.RedeamV12.Entities
{
    public class LogData
    {
        public MethodType MethodType;
        public string Token;
        public TimeSpan TimeElapsed;
        public object ResponseApi;
        public object InputRequest;
        public string ApiType;
    }
}