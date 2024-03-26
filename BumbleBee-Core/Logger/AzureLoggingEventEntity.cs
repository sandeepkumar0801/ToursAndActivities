using Isango.Entities;
using log4net.Core;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections;
using System.Globalization;
using System.Text;

namespace Logger
{
    internal sealed class AzureLoggingEventEntity : TableEntity
    {
        public AzureLoggingEventEntity(LoggingEvent e)
        {
            var isangoErrorEntity = (IsangoErrorEntity)e.MessageObject;
            PartitionKey = MakePartitionKey();
            RowKey = MakeRowKey();
            Class = isangoErrorEntity.ClassName;
            Method = isangoErrorEntity.MethodName;
            Params = isangoErrorEntity.Params;
            Level = e.Level.DisplayName;
            StackTrace = e.ExceptionObject?.StackTrace;
            UserName = e.UserName;
            var sb = new StringBuilder(e.Properties.Count);
            foreach (DictionaryEntry entry in e.Properties)
            {
                sb.AppendFormat("{0}:{1}", entry.Key, entry.Value);
                sb.AppendLine();
            }
            Properties = sb.ToString();
            LoggerName = e.LoggerName;
            Domain = e.Domain;
            Identity = e.Identity;
            ThreadName = e.ThreadName;
            ErrorMessage = e.ExceptionObject?.GetBaseException().Message;
            Token = isangoErrorEntity.Token;
            AffiliateId = isangoErrorEntity.AffiliateId;
        }

        private static string MakeRowKey()
        {
            return Guid.NewGuid().ToString().ToLower(CultureInfo.InvariantCulture);
        }

        private static string MakePartitionKey()
        {
            return DateTime.UtcNow.ToString("dd_MMM_yyyy");
        }

        public string UserName { get; set; }

        public string ThreadName { get; set; }

        public string Properties { get; set; }

        public string LoggerName { get; set; }

        public string Level { get; set; }

        public string Identity { get; set; }

        public string Domain { get; set; }

        public string Class { get; set; }

        public string Method { get; set; }

        public string StackTrace { get; set; }

        public string Params { get; set; }
        public string ErrorMessage { get; set; }

        public string Token { get; set; }

        public string AffiliateId { get; set; }
    }
}