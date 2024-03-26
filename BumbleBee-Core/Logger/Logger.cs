using Isango.Entities;
using log4net;
using Logger.Contract;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Globalization;
using System.Threading.Tasks;
using Util;

namespace Logger
{
    public class Logger : ILogger
    {
        private static ILog _log;
        private static string _saveTimerInStorageConfigFlag;
        private static CloudStorageAccount _cloudStorageAccount;
        private static int _azureTableloggingLimitInkb;
        private static readonly IConfiguration _configuration;


        static Logger()
        {
            _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            _cloudStorageAccount = CloudStorageAccount.Parse(ConfigurationManagerHelper.GetValuefromConfig("StorageConnectionString"));
            _azureTableloggingLimitInkb = Convert.ToInt32(ConfigurationManagerHelper.GetValuefromAppSettings("AzureTableloggingLimitInkb"));
            _saveTimerInStorageConfigFlag = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.SaveTimerInStorage);
        }

        public void Debug(string message)
        {
            var iSangoErrorEntity = new IsangoErrorEntity
            {
                ClassName = GetClassName(message),
                MethodName = GetMethodName(message),
                Params = GetParam(message)
            };
            Task.Run(() => _log.Debug(iSangoErrorEntity));
        }

        public void Debug(string message, Exception ex)
        {
            var iSangoErrorEntity = new IsangoErrorEntity
            {
                ClassName = GetClassName(message),
                MethodName = GetMethodName(message),
                Params = GetParam(message)
            };
            Task.Run(() => _log.Debug(iSangoErrorEntity, ex));
        }

        public void Debug(IsangoErrorEntity isangoErrorEntity)
        {
            Task.Run(() => _log.Debug(isangoErrorEntity));
        }

        public void Debug(IsangoErrorEntity isangoErrorEntity, Exception ex)
        {
            Task.Run(() => _log.Debug(isangoErrorEntity, ex));
        }

        public void Error(IsangoErrorEntity isangoErrorEntity, Exception ex)
        {
            Task.Run(() => _log.Error(isangoErrorEntity, ex));
        }

        public void Error(string message, Exception ex)
        {
            var iSangoErrorEntity = new IsangoErrorEntity
            {
                ClassName = GetClassName(message),
                MethodName = GetMethodName(message),
                Params = GetParam(message)
            };
            Task.Run(() => _log.Error(iSangoErrorEntity, ex));
        }

        public void Warning(string message)
        {
            var iSangoErrorEntity = new IsangoErrorEntity
            {
                ClassName = GetClassName(message),
                MethodName = GetMethodName(message),
                Params = GetParam(message)
            };
            Task.Run(() => _log.Warn(iSangoErrorEntity));
        }

        public void Warning(IsangoErrorEntity isangoErrorEntity)
        {

            Task.Run(() => _log.Warn(isangoErrorEntity));
        }

        public void LogError(Exception ex)
        {
            Task.Run(() => _log.Error(ex));
        }

        public void Info(string message)
        {
            var iSangoErrorEntity = new IsangoErrorEntity
            {
                ClassName = GetClassName(message),
                MethodName = GetMethodName(message),
                Params = GetParam(message)
            };
            Task.Run(() => _log.Info(iSangoErrorEntity));
        }

        public void Info(IsangoErrorEntity isangoErrorEntity)
        {
            Task.Run(() => _log.Info(isangoErrorEntity));
        }

        public void Write(string request, string response, string methodName, string token, string apiName)
        {
            try
            {
                var saveInStorageConfigFlag = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.SaveInStorage);
                if (!saveInStorageConfigFlag.Equals(Constant.SaveInStorageValue))
                {
                    return;
                }
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

                var loggingSize = (request + response + methodName + token + apiName).Length;
                if (loggingSize > 0)
                {
                    loggingSize /= 1024;
                }
                if (loggingSize < _azureTableloggingLimitInkb)
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
            catch (Exception ex)
            {
                //ignore
            }
        }

        private static string GetMethodName(string message)
        {
            return message.Split('|')[1];
        }

        private static string GetClassName(string message)
        {
            return message.Split('|')[0];
        }

        private static string GetParam(string message)
        {
            var result = message.Split('|');
            var param = string.Empty;
            if (result.Length > 2)
            {
                var offset = message.IndexOf('|');
                offset = message.IndexOf('|', offset + 1);
                param = message.Substring(offset + 1);
            }
            return param;
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
            if(!table.Exists())
            {
                table.CreateIfNotExistsAsync();
            }
            return table;
        }

        private void InsertDataInAzureTable(string tableName, ITableEntity tableEntity)
        {
            try
            {
                var table = GetTableReference(tableName);
                var insertOp = TableOperation.InsertOrReplace(tableEntity);
                var result = table.ExecuteAsync(insertOp);
                result.Wait();
            }
            catch (Exception)
            {
                //throw;
            }
        }

        public void WriteTimer(string methodName, string token, string apiName, string elapsedTime)
        {
            Task.Run(() => WriteTimerAsync(methodName, token, apiName, elapsedTime));
        }

        /// <summary>
        /// To be called async in WriteTimer
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="token"></param>
        /// <param name="apiName"></param>
        /// <param name="elapsedTime"></param>
        private void WriteTimerAsync(string methodName, string token, string apiName, string elapsedTime)
        {
            if (!_saveTimerInStorageConfigFlag.Equals(Constant.SaveTimerInStorageValue))
            {
                return;
            }
            var timerLoggingEntity = new TimerLoggingEntity
            {
                Method = methodName,
                PartitionKey = GetPartitionKey(),
                RowKey = GetRowKey(),
                Token = token,
                ApiName = apiName,
                ElapsedTime = elapsedTime
            };
            InsertDataInAzureTable("TimerLogging", timerLoggingEntity);
        }

        private string UploadFileToBlob(string response, string token, AdapterLoggingEntity adapterLoggingEntity = null)
        {
            var thislogtoken = Guid.NewGuid();
            // Create the CloudBlobClient that represents the Blob storage endpoint for the storage account.
            var cloudBlobClient = _cloudStorageAccount.CreateCloudBlobClient();

            // Create a container //lowercase
            var container = cloudBlobClient.GetContainerReference("logging");

            container.CreateIfNotExistsAsync().GetAwaiter().GetResult();

            // Set the permissions so the blobs are public.
            var permissions = new BlobContainerPermissions
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
            blockBlob.UploadTextAsync(response);

            return blockBlob.Uri.ToString();
        }
    }
}