using Isango.Entities;
using System;

namespace Logger.Contract
{
    public interface ILogger
    {
        void Debug(string message);

        void Debug(string message, Exception ex);

        void Debug(IsangoErrorEntity isangoErrorEntity);

        void Debug(IsangoErrorEntity isangoErrorEntity, Exception ex);

        void Warning(string message);

        void Warning(IsangoErrorEntity isangoErrorEntity);

        void Error(IsangoErrorEntity isangoErrorEntity, Exception ex);

        void Error(string message, Exception ex);

        void Info(string message);

        void Info(IsangoErrorEntity isangoErrorEntity);

        void Write(string request, string response, string methodName, string token, string apiName);

        void WriteTimer(string methodName, string token, string apiName, string elapsedTime);
    }
}