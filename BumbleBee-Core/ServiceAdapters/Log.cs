using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using System.Globalization;
using Util;

namespace ServiceAdapters
{
    public class Log : ILog
    {
        private CloudStorageAccount _cloudStorageAccount;
        private int _azureTableLoggingLimitInKb;

        public Log()
        {
            _cloudStorageAccount = CloudStorageAccount.Parse(ConfigurationManagerHelper.GetValuefromConfig("StorageConnectionString"));
            try
            {
                int.TryParse(ConfigurationManagerHelper.GetValuefromAppSettings("AzureTableLoggingLimitInKb"), out var tempInt);
                _azureTableLoggingLimitInKb = tempInt;
            }
            catch (Exception)
            {
                _azureTableLoggingLimitInKb = 30;
            }
            if (_azureTableLoggingLimitInKb == 0 || _azureTableLoggingLimitInKb > 30)
            {
                _azureTableLoggingLimitInKb = 30;
            }
        }

        public void Write(string request, string response, string methodName, string token, string apiName)
        {
            var adapterLoggingEntity = new AdapterLoggingEntity
            {
                Method = methodName,
                Request = request,
                Response = response,
                PartitionKey = GetPartitionKey(),
                RowKey = GetRowKey(),
                Token = token,
                ApiName = apiName
            };
            int logggingSize = 0;
            try
            {
                logggingSize = (request + response + methodName + token + apiName).Length;
                if (logggingSize > 0)
                {
                    //Not fully accurate but gives size approx with variation up to 2kb
                    logggingSize /= 1024;
                }

                //if size is around 30 kb then try to log in azure table else save response in blob
                if (logggingSize < _azureTableLoggingLimitInKb)
                {
                    InsertDataInAzureTable("AdapterLogging", adapterLoggingEntity);
                }
                else
                {
                    var blobFileUri = UploadFileToBlob(response, token, adapterLoggingEntity);
                    adapterLoggingEntity.Response = blobFileUri;
                    InsertDataInAzureTable("AdapterLogging", adapterLoggingEntity);
                }
            }
            catch (AggregateException ex)
            {
                var blobFileUri = UploadFileToBlob(response, token, adapterLoggingEntity);
                adapterLoggingEntity.Response = blobFileUri;

                InsertDataInAzureTable("AdapterLogging", adapterLoggingEntity);
            }
        }

        private string GetRowKey()
        {
            return Guid.NewGuid().ToString().ToLower(CultureInfo.InvariantCulture);
        }

        private string GetPartitionKey()
        {
            return DateTime.UtcNow.ToString("dd_MMM_yyyy");
        }

        private CloudTable GetTableReference(string tableName)
        {
            var client = _cloudStorageAccount.CreateCloudTableClient();
            var table = client.GetTableReference(tableName);
            table.CreateIfNotExistsAsync();
            return table;
        }

        private /*Task<object>*/ void InsertDataInAzureTable(string tableName, ITableEntity tableEntity)
        {
            var table = GetTableReference(tableName);
            var insertOp = TableOperation.InsertOrReplace(tableEntity);
            var result = table.ExecuteAsync(insertOp);
            //Commenting it as wait for logging to complete is not required.
            result.Wait();
            //return Task.FromResult<object>(null);
        }

        public void WriteTimer(string methodName, string token, string apiName, string elapsedTime)
        {
            var timerLoggingEntity = new TimerLoggingEntity
            {
                Method = methodName,
                PartitionKey = GetPartitionKey(),
                RowKey = GetRowKey(),
                Token = token,
                ApiName = apiName,
                ElapsedTime = elapsedTime
            };

            Task.Run(() => InsertDataInAzureTable("TimerLogging", timerLoggingEntity));
        }

        /// <summary>
        /// Creates blob asynchronously and return blob file path.
        /// </summary>
        /// <param name="response"></param>
        /// <param name="token"></param>
        /// <param name="adapterLoggingEntity"></param>
        /// <returns>url for created blob.</returns>
        private string UploadFileToBlob(string response, string token, AdapterLoggingEntity adapterLoggingEntity = null)
        {
            var thislogtoken = Guid.NewGuid();
            // Create the CloudBlobClient that represents the Blob storage endpoint for the storage account.
            CloudBlobClient cloudBlobClient = _cloudStorageAccount.CreateCloudBlobClient();

            // Create a container //lowercase
            var container = cloudBlobClient.GetContainerReference("logging");

            container.CreateIfNotExistsAsync().GetAwaiter().GetResult();

            // Set the permissions so the blobs are public.
            BlobContainerPermissions permissions = new BlobContainerPermissions
            {
                PublicAccess = BlobContainerPublicAccessType.Blob
            };
            container.SetPermissionsAsync(permissions).GetAwaiter().GetResult();
            var apidetails = string.Empty;
            if (adapterLoggingEntity != null)
            {
                apidetails = $"{adapterLoggingEntity.ApiName}_{adapterLoggingEntity.Method}_";
            }
            var fileName = $"AdapterLogging/{apidetails}response_{token}_{thislogtoken}.json";
            var blockBlob = container.GetBlockBlobReference(fileName);

            //upload file without wait to complete uploading
            Task.Run(() => UploadFileToBlobAsync(response, blockBlob));

            return blockBlob.Uri.ToString();
        }

        /// <summary>
        /// upload to blob asynchronously
        /// </summary>
        /// <param name="response"></param>
        /// <param name="blockBlob"></param>
        /// <returns></returns>
        private Task<object> UploadFileToBlobAsync(string response, CloudBlockBlob blockBlob)
        {
            blockBlob.UploadTextAsync(response);
            return Task.FromResult<object>(null);
        }
    }
}