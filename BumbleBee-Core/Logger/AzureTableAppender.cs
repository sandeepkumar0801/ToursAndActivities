using log4net.Appender;
using log4net.Core;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Linq;

namespace Logger
{
    public class AzureTableAppender : BufferingAppenderSkeleton
    {
        private CloudStorageAccount _account;
        private CloudTableClient _client;
        private CloudTable _table;

        public string ConnectionString { get; set; }
        public string TableName { get; set; }

        protected override void SendBuffer(LoggingEvent[] events)
        {
            //Batched ops require single partition key, group
            //by loggername to obey requirment.
            var grouped = events.GroupBy(evt => evt.LoggerName);

            foreach (var group in grouped)
            {
                try
                {
                    var batchOperation = new TableBatchOperation();
                    foreach (var azureLoggingEvent in group.Select(@event => new AzureLoggingEventEntity(@event)))
                    {
                        batchOperation.Insert(azureLoggingEvent);
                    }
                    _table.ExecuteBatchAsync(batchOperation);
                }
                catch (System.Exception)
                {
                    //ignore
                }
            }
        }

        public override void ActivateOptions()
        {
            base.ActivateOptions();

            _account = CloudStorageAccount.Parse(ConnectionString);
            _client = _account.CreateCloudTableClient();
            _table = _client.GetTableReference(TableName);
            _table.CreateIfNotExistsAsync();
        }
    }
}