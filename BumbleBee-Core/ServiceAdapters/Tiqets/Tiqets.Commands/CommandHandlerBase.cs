using Logger.Contract;
using ServiceAdapters.Tiqets.Constants;
using ServiceAdapters.Tiqets.Tiqets.Entities;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using Util;
using System.Security.Cryptography.X509Certificates;
using Jose;
using Azure.Security.KeyVault.Certificates;
using Azure.Identity;
using System.Runtime.Caching;
using Azure.Security.KeyVault.Secrets;
using System.Web;
using System.Threading.Tasks;
using System.Net;
using System.Security.Cryptography;

namespace ServiceAdapters.Tiqets.Tiqets.Commands
{

    public abstract class CommandHandlerBase
    {
        private readonly ILogger _log;
        private readonly bool _isMock;
        private readonly string _path;
        private readonly string _mockingPath;
        private object _obj;
        private readonly string _FileCacheTime;
        private readonly string _affilateID;
        protected CommandHandlerBase(ILogger log)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            _log = log;
            _obj = new object();
            _isMock = false;
            _FileCacheTime = "1";
            try
            {
                _path = AppDomain.CurrentDomain.BaseDirectory;
                _mockingPath = $"{_path}Mock-Samples\\Tiqets";
                _isMock = Util.ConfigurationManagerHelper.GetValuefromAppSettings("isMock_MR") == "1";
                _FileCacheTime = Util.ConfigurationManagerHelper.GetValuefromAppSettings("TiqetStaticFileCacheTime");
                _affilateID = Util.ConfigurationManagerHelper.GetValuefromAppSettings("CitySightSeeingAffiliateID");
            }
            catch
            {
                _affilateID = null;
            }
        }

        public virtual object Execute<T>(T inputContext, MethodType methodType, string token)
        {
            var watch = Stopwatch.StartNew();
            var httpStatusData = new HttpResponseMessage();
            var inputRequest = CreateInputRequest(inputContext);
            var responseObject = default(object);
            string request = string.Empty;
            string response = string.Empty;
            if (_isMock)
            {
                try
                {
                    var resFilePath = $"{_mockingPath}\\{methodType.ToString()}_res.json";
                    if (System.IO.File.Exists(resFilePath))
                    {
                        responseObject = File.ReadAllText(resFilePath);
                        response = responseObject?.ToString();
                    }
                }
                catch
                {
                }
            }
            else
            {
                var httpResponse = TiqetsApiRequest(inputRequest);
                if (httpResponse.GetType() == typeof(HttpResponseMessage))
                {
                    httpStatusData = ((HttpResponseMessage)httpResponse);
                    responseObject = httpStatusData.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    inputRequest = SerializeDeSerializeHelper.Serialize(httpStatusData?.RequestMessage);
                }
                else
                {
                    responseObject = TiqetsApiRequest(inputRequest);
                }
            }

            watch.Stop();
            _log.WriteTimer(methodType.ToString(), token, Constant.Tiqets, watch.Elapsed.ToString());
            if (responseObject != null)
            {
                _log.Write(SerializeDeSerializeHelper.Serialize(inputRequest), responseObject.ToString(), methodType.ToString(), token, Constant.Tiqets);
            }
            return responseObject;
        }

        public virtual object Execute<T>(T inputContext, string token, MethodType methodType,
            out string request, out string response, out HttpStatusCode httpStatusCode)
        {
            var watch = Stopwatch.StartNew();
            var inputRequest = CreateInputRequest(inputContext);
            var httpStatusData = new HttpResponseMessage();
            var responseObject = default(object);
            request = string.Empty;
            response = string.Empty;
            if (_isMock)
            {
                try
                {
                    var resFilePath = $"{_mockingPath}\\{methodType.ToString()}_res.json";
                    if (System.IO.File.Exists(resFilePath))
                    {
                        responseObject = File.ReadAllText(resFilePath);
                        response = responseObject?.ToString();
                    }
                }
                catch
                {
                }
            }
            else
            {
                var httpResponse = TiqetsBookingApiRequest(inputRequest);
                httpStatusData = ((HttpResponseMessage)httpResponse);
                responseObject = httpStatusData.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            }

            watch.Stop();
            request = SerializeDeSerializeHelper.Serialize(inputContext);
            response = responseObject?.ToString();
            _log.WriteTimer(methodType.ToString(), token, Constant.Tiqets, watch.Elapsed.ToString());
            _log.Write(SerializeDeSerializeHelper.Serialize(inputRequest), response, methodType.ToString(), token, Constant.Tiqets);
            httpStatusCode = httpStatusData.StatusCode;
            return responseObject;
        }

        /// <summary>
        /// This method adds the headers and Base address to Http Client.
        /// </summary>
        protected HttpClient AddRequestHeadersAndAddressToApi(string affiliateIDGet = "")
        {
            var httpClient = new HttpClient();
            if (httpClient.BaseAddress == null)
                httpClient.Timeout = TimeSpan.FromMinutes(3);
            if (_affilateID != null && _affilateID?.ToUpper() == affiliateIDGet?.ToUpper()) //if citysightseeing affiliate
            {
                httpClient.BaseAddress = new Uri(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.TiqetUriCitySightSeeing));
            }
            else
            {
                httpClient.BaseAddress = new Uri(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.TiqetUri));
            }
            if (httpClient.DefaultRequestHeaders.Any())
                httpClient.DefaultRequestHeaders.Clear();

            if (_affilateID != null && _affilateID?.ToUpper() == affiliateIDGet?.ToUpper())//if citysightseeing affiliate
            {
                httpClient.DefaultRequestHeaders.Add(Constant.TiqetsAuthorizationKey, ConfigurationManagerHelper.GetValuefromAppSettings(Constant.TiqetsAuthorizationCitySightSeeing));
            }
            else
            {
                httpClient.DefaultRequestHeaders.Add(Constant.TiqetsAuthorizationKey, ConfigurationManagerHelper.GetValuefromAppSettings(Constant.TiqetsAuthorizationValue));
            }

            httpClient.DefaultRequestHeaders.Add(Constant.UserAgentKey, Constant.UserAgentDefaultValue);
            return httpClient;
        }

        /// <summary>
        /// Create Input Request
        /// </summary>
        /// <param name="inputContext"></param>
        /// <returns></returns>
        protected virtual object CreateInputRequest<T>(T inputContext)
        {
            return inputContext;
        }

        protected virtual object TiqetsApiRequest<T>(T inputContext)
        {
            return null;
        }

        protected virtual object TiqetsBookingApiRequest<T>(T inputContext)
        {
            return null;
        }


        public string GetSignedPayload(string payload, string affiliateIDGet = "")
        {
            try
            {
                var certificateName = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.TiqetsCertificateNameKeyVault);
                var vaultName = ConfigurationManagerHelper.GetValuefromAppSettings("KeyVaultName");
                var kvUri = $"https://" + vaultName + ".vault.azure.net";

                if (_affilateID != null && _affilateID?.ToUpper() == affiliateIDGet?.ToUpper())//if citysightseeing affiliate
                {
                    certificateName = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.TiqetsCertificateNameKeyVaultCitySightSeeing);
                    vaultName = ConfigurationManagerHelper.GetValuefromAppSettings("KeyVaultNameCitySightSeeing");
                    kvUri = $"https://" + vaultName + ".vault.azure.net";
                }

                var certClient = new CertificateClient(new Uri(kvUri), new DefaultAzureCredential());
                var secretClient = new SecretClient(new Uri(kvUri), new DefaultAzureCredential());
                var certificate = certClient.GetCertificateAsync(certificateName)?.GetAwaiter().GetResult();

                var signedPayload = string.Empty;
                string[] segments = certificate.Value.SecretId.AbsolutePath.Split('/');
                var errorMsg = string.Empty;
                string secretName = segments[2];
                string secretVersion = segments[3];
                var secret = secretClient.GetSecretAsync(secretName, secretVersion)?.GetAwaiter().GetResult();
                var retryCount = 0;
                while (retryCount < 3
                    && (string.IsNullOrWhiteSpace(errorMsg) || errorMsg?.ToLower()?.Contains("keyset does not exist") == true)

                    )
                {
                    try
                    {
                        //var certificateFilePath = $"{Path.Combine(AppDomain.CurrentDomain.BaseDirectory)}{Constant.TiqetsCertificate}\\{ConfigurationManagerHelper.GetValuefromAppSettings(Constant.TiqetsCertificateName)}";
                        var password = string.Empty;

                        //lock (_obj)
                        //{
                        try
                        {
                            using (var x509Certificate = new X509Certificate2(Convert.FromBase64String(secret.Value.Value), password, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.Exportable))
                            {
                                //var x509Certificate = new X509Certificate2(certificateFilePath, password, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);
                                using (var privateKey = x509Certificate.GetRSAPrivateKey())
                                {
                                    //var privateKey = x509Certificate.PrivateKey;
                                    signedPayload = JWT.Encode(payload, privateKey, JwsAlgorithm.RS256);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            try
                            {
                                var x509Certificate1 = new X509Certificate2(Convert.FromBase64String(secret.Value.Value), password, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.Exportable);
                                using (var privateKey = x509Certificate1.GetRSAPrivateKey())
                                {
                                    //var privateKey = x509Certificate.PrivateKey;
                                    signedPayload = JWT.Encode(payload, privateKey, JwsAlgorithm.RS256);
                                }
                            }
                            catch (Exception ex)
                            {
                                throw;
                            }
                        }
                        //}
                        if (!string.IsNullOrWhiteSpace(signedPayload))
                        {
                            errorMsg = string.Empty;
                            retryCount = 3;
                        }

                    }
                    catch (Exception ex)
                    {
                        if (errorMsg?.ToLower()?.Contains("keyset does not exist") == true)
                        {
                            errorMsg = ex?.Message?.ToLower();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    retryCount++;
                }

                return signedPayload;
            }
            catch (Exception ex)
            {
                try
                {
                    var certificateFilePath = $"{Path.Combine(AppDomain.CurrentDomain.BaseDirectory)}{Constant.TiqetsCertificate}\\{ConfigurationManagerHelper.GetValuefromAppSettings(Constant.TiqetsCertificateName)}";
                    if (_affilateID != null && _affilateID?.ToUpper() == affiliateIDGet?.ToUpper())//if citysightseeing affiliate
                    {
                        certificateFilePath = $"{Path.Combine(AppDomain.CurrentDomain.BaseDirectory)}{Constant.TiqetsCertificate}\\{ConfigurationManagerHelper.GetValuefromAppSettings(Constant.TiqetsCertificateNameCitySightSeeing)}";
                    }


                    var password = string.Empty;
                    var signedPayload = string.Empty;
                    var certificateKey = $"{certificateFilePath}:{password}";

                    RSA? privateKey;
                    if (CacheHelper.Get(certificateKey, out privateKey))
                    {
                        signedPayload = JWT.Encode(payload, privateKey, JwsAlgorithm.RS256);
                    }
                    else
                    {
                        using (var x509Certificate = new X509Certificate2(certificateFilePath, password, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.Exportable))
                        {
                            using (privateKey = x509Certificate.GetRSAPrivateKey())
                            {
                                CacheHelper.Set(certificateKey, privateKey, 15);

                                signedPayload = JWT.Encode(payload, privateKey, JwsAlgorithm.RS256);
                            }
                        }
                    }
                    return signedPayload;
                }
                catch (Exception e)
                {
                    var certificateFilePath = $"{Path.Combine(AppDomain.CurrentDomain.BaseDirectory)}{Constant.TiqetsCertificate}\\{ConfigurationManagerHelper.GetValuefromAppSettings(Constant.TiqetsCertificateName)}";
                    if (_affilateID != null && _affilateID?.ToUpper() == affiliateIDGet?.ToUpper())//if citysightseeing affiliate
                    {
                        certificateFilePath = $"{Path.Combine(AppDomain.CurrentDomain.BaseDirectory)}{Constant.TiqetsCertificate}\\{ConfigurationManagerHelper.GetValuefromAppSettings(Constant.TiqetsCertificateNameCitySightSeeing)}";
                    }

                    var password = string.Empty;
                    var signedPayload = string.Empty;

                    var certificateKey = $"{certificateFilePath}:{password}";

                    RSA? privateKey;
                    if (CacheHelper.Get(certificateKey, out privateKey))
                    {
                        signedPayload = JWT.Encode(payload, privateKey, JwsAlgorithm.RS256);
                    }
                    else
                    {
                        var x509Certificate = new X509Certificate2(certificateFilePath, password, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.Exportable);
                        privateKey = x509Certificate.GetRSAPrivateKey();
                        CacheHelper.Set(certificateKey, privateKey, 15);

                        signedPayload = JWT.Encode(payload, privateKey, JwsAlgorithm.RS256);
                    }

                    return signedPayload;
                }
            }
        }
    }
}