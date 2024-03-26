using Google.Apis.Auth.OAuth2;
using Isango.Entities;
using Logger.Contract;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Renci.SshNet;
using ServiceAdapters.GoogleMaps.Constants;
using ServiceAdapters.GoogleMaps.GoogleMaps.Entities;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Util;

namespace ServiceAdapters.GoogleMaps.GoogleMaps.Commands
{
    public abstract class CommandHandlerBase
    {
        #region Properties

        private readonly ILogger _log;
        private readonly string _baseAddress;

        #endregion Properties

        #region ctr

        protected CommandHandlerBase(ILogger log)
        {
            _log = log;
            _baseAddress = ConfigurationManagerHelper.GetValuefromAppSettings("GMaps:RTU:Endpoint");
        }

        #endregion ctr

        #region Public Methods

        /// <summary>
        ///
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public virtual bool UploadToDropbox(string userName, string filePath)
        {
            try
            {
                var compressPath = CompressFeeds(filePath);
                var keyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GMapKey");
                keyPath = keyPath + @"\" + ConfigurationManagerHelper.GetValuefromAppSettings("GMaps:PrivateKey:File");
                var host = ConfigurationManagerHelper.GetValuefromAppSettings("GMaps:DropBox:Host");
                var port = Convert.ToInt32(ConfigurationManagerHelper.GetValuefromAppSettings("GMaps:DropBox:Port"));

                using (var sftpClient = new SftpClient(GetSftpConnection(host, userName, port, keyPath)))
                {
                    sftpClient.Connect();
                    using (var fs = new FileStream(compressPath, FileMode.Open))
                    {
                        sftpClient.BufferSize = 1024;
                        sftpClient.UploadFile(fs, Path.GetFileName(compressPath));
                    }
                    sftpClient.Dispose();
                }
                UploadFeedsToBlob(compressPath);
                return true;
            }
            catch (Exception e)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "GoogleMaps.Commands.CommandHandlerBase",
                    MethodName = "UploadToDropbox",
                    Params = $"{userName}"
                };
                _log.Error(isangoErrorEntity, e.GetBaseException());
                throw;
            }
        }

        /// <summary>
        /// To get the feed folder path
        /// </summary>
        /// <returns></returns>
        public string GetFeedPath()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Constant.FeedFolder);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }

        /// <summary>
        /// This method used to call the API and Log the JSON Files.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputContext"></param>
        /// <param name="methodType"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public virtual object Execute<T>(T inputContext, MethodType methodType)
        {
            var watch = Stopwatch.StartNew();
            var inputRequest = CreateInputRequest(inputContext);
            if (inputRequest == null) return null;

            var responseApi = GoogleMapsApiRequest(inputRequest);
            watch.Stop();
            _log.WriteTimer(methodType.ToString(), "", "GoogleMaps", watch.Elapsed.ToString());
            _log.Write(SerializeDeSerializeHelper.Serialize(inputRequest), responseApi.ToString(), methodType.ToString(), "", "GoogleMaps");
            return CheckIfErrorOccurred(responseApi) ? null : responseApi;
        }

        public virtual bool UploadFeed<T>(T inputContext, MethodType methodType)
        {
            var mapFeedJson = MapFeed(inputContext);
            var path = CreateFeedFile(methodType, mapFeedJson, out var username);
            return UploadToDropbox(username, path);
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// This method created a new Http Client instance and add the Authorization header in it.
        /// </summary>
        /// <returns></returns>
        protected HttpClient GetHttpClient()
        {
            var httpClient = new HttpClient { BaseAddress = new Uri(_baseAddress), Timeout = new TimeSpan(0, 5, 0) };
            var authToken = GetAuthToken();
            var token = $"Bearer {authToken}";
            httpClient.DefaultRequestHeaders.Add("Authorization", token);
            httpClient.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header
            return httpClient;
        }

        protected virtual object CreateInputRequest<T>(T inputContext)
        {
            return inputContext;
        }

        protected virtual object GoogleMapsApiRequest<T>(T inputContext)
        {
            return null;
        }

        protected virtual object MapFeed<T>(T inputContext)
        {
            return inputContext;
        }

        protected object ValidateApiResponse(HttpResponseMessage result)
        {
            var apiResponse = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            if (result.StatusCode != HttpStatusCode.OK)
                apiResponse = $"{Constant.ErrorWhileAPIHit} : {result.StatusCode.ToString()} \n-----------------------------------\n\n<Request Message> -----------------------------------\n{result.RequestMessage} \n<Response Message> -----------------------------------\n {result.Content.ReadAsStringAsync().Result}";

            return apiResponse;
        }

        protected string GetAuthToken()
        {
            var keyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GMapKey");
            keyPath = keyPath + @"\" + ConfigurationManagerHelper.GetValuefromAppSettings("GMaps:PrivateKey:JsonFile");
            string[] scopes = { "https://www.googleapis.com/auth/mapsbooking" }; //Read-only
            GoogleCredential credential;
            using (var stream = new FileStream(keyPath, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential
                    .FromStream(stream) // Loads key file
                    .CreateScoped(scopes); // Gathers scopes requested
            }

            var token = credential.UnderlyingCredential.GetAccessTokenForRequestAsync().GetAwaiter().GetResult();
            return token;
        }

        #endregion Protected Methods

        #region Private Methods

        /// <summary>
        ///
        /// </summary>
        /// <param name="host"></param>
        /// <param name="username"></param>
        /// <param name="port"></param>
        /// <param name="publicKeyPath"></param>
        /// <returns></returns>
        private ConnectionInfo GetSftpConnection(string host, string username, int port, string publicKeyPath)
        {
            return new ConnectionInfo(host, port, username, PrivateKeyObject(username, publicKeyPath));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="username"></param>
        /// <param name="publicKeyPath"></param>
        /// <returns></returns>
        private AuthenticationMethod[] PrivateKeyObject(string username, string publicKeyPath)
        {
            var privateKeyFile = new PrivateKeyFile(publicKeyPath);
            var privateKeyAuthenticationMethod = new PrivateKeyAuthenticationMethod(username, privateKeyFile);
            return new AuthenticationMethod[] { privateKeyAuthenticationMethod };
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private string CompressFeeds(string filePath)
        {
            var fileName = Path.GetFileName(filePath);
            var compressFileName = fileName?.Replace(".json", "") ?? "Feeds";
            var compressPath = filePath.Replace(@"\" + fileName, "") + @"\" + compressFileName + ".gz";
            if (File.Exists(compressPath))
            {
                File.Delete(compressPath);
            }
            var originalFileStream = File.OpenRead(filePath);
            using (FileStream compressedFileStream = File.Create(compressPath))
            {
                using (GZipStream compressionStream = new GZipStream(compressedFileStream,
                    CompressionMode.Compress))
                {
                    originalFileStream.CopyTo(compressionStream);
                }
            }
            return compressPath;
        }

        private void UploadFeedsToBlob(string path)
        {
            var cloudStorageAccount = CloudStorageAccount.Parse(ConfigurationManagerHelper.GetValuefromConfig("StorageConnectionString"));
            // Create the CloudBlobClient that represents the Blob storage endpoint for the storage account.
            var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();

            // Create a container //lowercase
            var container = cloudBlobClient.GetContainerReference("gmapfeeds");

            container.CreateIfNotExistsAsync().GetAwaiter().GetResult();

            // Set the permissions so the blobs are public.
            var permissions = new BlobContainerPermissions
            {
                PublicAccess = BlobContainerPublicAccessType.Blob
            };
            container.SetPermissionsAsync(permissions).GetAwaiter().GetResult();

            var fileName = DateTime.UtcNow.ToString("dd-MMM-yyyy HH:MM:ss tt") + "_" + Path.GetFileName(path);
            var blockBlob = container.GetBlockBlobReference(fileName);
            blockBlob.UploadFromFileAsync(path);
        }

        private string CreateFeedFile(MethodType methodType, object mapFeedsJson, out string username)
        {
            var path = GetFeedPath();
            username = string.Empty;
            switch (methodType)
            {
                case MethodType.AvailabilityFeed:
                    path = path + @"\" + Constant.AvailabilityFeedFileJson;
                    username = ConfigurationManagerHelper.GetValuefromAppSettings("GMaps:Availability:UserName");
                    break;

                case MethodType.MerchantFeed:
                    path = path + @"\" + Constant.MerchantFileJson;
                    username = ConfigurationManagerHelper.GetValuefromAppSettings("GMaps:Merchant:UserName");
                    break;

                case MethodType.ServiceFeed:
                    path = path + @"\" + Constant.ServiceFeedFileJson;
                    username = ConfigurationManagerHelper.GetValuefromAppSettings("GMaps:Service:UserName");
                    break;

                default:
                    path = string.Empty;
                    break;
            }

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            File.WriteAllText(path, mapFeedsJson.ToString());
            return path;
        }

        private bool CheckIfErrorOccurred(object responseApi)
        {
            return Convert.ToString(responseApi).Contains(Constant.ErrorWhileAPIHit);
        }

        #endregion Private Methods
    }
}