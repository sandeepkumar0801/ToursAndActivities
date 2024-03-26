using Logger.Contract;
using System.IO.Compression;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using Util;

// ReSharper disable AssignNullToNotNullAttribute

namespace ServiceAdapters
{

    public class AsyncClient
    {
        private byte[] _bytes;
        private ILogger _log;
        public AsyncClient()
        {
            _log = new Logger.Logger();
        }
        
        public string ServiceURL { get; set; }
        public Uri ServiceUri { get; set; }
        public static string ExternalIpAddress { get; set; } = ExternalIPAddress();
        public static string ApiKey { get; set; } = Convert.ToString(ConfigurationManagerHelper.GetValuefromAppSettings("ApiKey"));
        public static string Secret { get; set; } = Convert.ToString(ConfigurationManagerHelper.GetValuefromAppSettings("ApiSecret"));
        public static bool IsUncompressedStagingApi { get; set; } = ConfigurationManagerHelper.GetValuefromAppSettings("IsUncompressedStagingAPI") != null && Convert.ToBoolean(ConfigurationManagerHelper.GetValuefromAppSettings("IsUncompressedStagingAPI"));

        public static string ApexxApiKey { get; set; } = Convert.ToString(ConfigurationManagerHelper.GetValuefromAppSettings("ApexxApiKey"));
        public static string PrioUrls { get; set; } = Convert.ToString(ConfigurationManagerHelper.GetValuefromAppSettings("PrioUrls"));
        public string AdyenApiKey { get; set; } = Convert.ToString(ConfigurationManagerHelper.GetValuefromAppSettings("AdyenApiKey").Replace("||", "&"));

        /// <summary>
        /// The Body of Post request must be pre-formatted as expected by the endpoint.
        /// For example: In case of HotelBeds it must be prefixed by "xml_request=".
        /// In case of Jacob it should be the XML itself.
        /// </summary>
        /// <returns></returns>
        public async Task<string> PostAsync(List<KeyValuePair<string, string>> values)
        {
            var client = new HttpClient { Timeout = TimeSpan.FromMinutes(3) };
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Accept", "text/html");
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");

            HttpResponseMessage xmlRes;

            if (!string.IsNullOrEmpty(values[0].Key))
            {
                var content = new FormUrlEncodedContent(values);
                xmlRes = await client.PostAsync(ServiceURL, content);
            }
            else
            {
                xmlRes = await client.PostAsync(ServiceURL, new StringContent(values[0].Value));
            }

            var res = string.Empty;
            if (!xmlRes.IsSuccessStatusCode)
            {
                var hdr = "Headers:\n";
                foreach (var h in xmlRes.Headers)
                {
                    hdr = $"{hdr}{h.Key} ~ ";
                    foreach (var v in h.Value)
                    {
                        hdr = $"{hdr}\t\t{v}";
                    }
                    hdr = $"{hdr}\n";
                }
                hdr = $"{hdr}\n{xmlRes.RequestMessage}\n";
                res = $"Error: Server Responded with \n{xmlRes.ReasonPhrase}{hdr}\n\n\n";
            }

            if (IsUncompressedStagingApi)
            {
                using (var responseStream = await xmlRes.Content.ReadAsStreamAsync())
                using (var streamReader = new StreamReader(responseStream))
                {
                    return $"{res}{streamReader.ReadToEnd()}";
                }
            }
            else
            {
                using (var responseStream = await xmlRes.Content.ReadAsStreamAsync())
                using (var decompressedStream = new GZipStream(responseStream, CompressionMode.Decompress))
                using (var streamReader = new StreamReader(decompressedStream))
                {
                    return $"{res}{streamReader.ReadToEnd()}";
                }
            }
        }

        public string Post(List<KeyValuePair<string, string>> values)
        {
            var request = (HttpWebRequest)WebRequest.Create(ServiceURL);

            request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "text/html";
            request.Headers.Add("Accept-Encoding", "gzip, deflate");
            request.Method = "POST";
            request.Timeout = 90000;

            //HB Way
            if (!string.IsNullOrEmpty(values[0].Key))
            {
                var postData = $"xml_request={Uri.EscapeDataString(values[0].Value)}";
                _bytes = Encoding.UTF8.GetBytes(postData);
            }

            //Jacob Way
            else
            {
                _bytes = Encoding.UTF8.GetBytes(values[0].Value);
            }

            // send the Post
            request.ContentLength = _bytes.Length;   //Count bytes to send
            var os = request.GetRequestStream();
            os.Write(_bytes, 0, _bytes.Length);         //Send it

            if (IsUncompressedStagingApi)
            {
                var hdr = "<!--\n";
                using (WebResponse response = request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    var responseBody = reader.ReadToEnd();
                    for (var i = 0; i < response.Headers.Count; i++)
                    {
                        hdr = $"{hdr}{response.Headers.GetKey(i)} ~ ";
                        hdr = $"{hdr}\t\t{response.Headers.Get(i)}\n";
                    }
                    return $"{responseBody}\n\n{hdr}\n-->";
                }
            }
            else
            {
                var hdr = "<!--\n";
                using (WebResponse response = request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (var decompressedStream = new GZipStream(stream, CompressionMode.Decompress))
                using (StreamReader reader = new StreamReader(decompressedStream))
                {
                    var responseBody = reader.ReadToEnd();
                    for (int i = 0; i < response.Headers.Count; i++)
                    {
                        hdr = $"{hdr}{response.Headers.GetKey(i)} ~ ";
                        hdr = $"{hdr}\t\t{response.Headers.Get(i)}\n";
                    }
                    return $"{responseBody}\n\n{hdr}\n-->";
                }
            }
        }

        public object PostPrioJsonAsync(object jsonObject, string authToken)
        {
            var baseAddress = ConfigurationManagerHelper.GetValuefromAppSettings("PrioUrls");
            var client = new HttpClient { Timeout = TimeSpan.FromMinutes(3) };
            var content = new StringContent(jsonObject.ToString());
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            //Create Request authentication key
            //1.) Create a string with format
            var xRequestIdentifier = Convert.ToString(Guid.NewGuid());
            var identifierToken = $"{xRequestIdentifier}:{authToken}"; //<x-request-identifier>:<apikeytoken>
                                                                       //2.) Convert the string from step 1 to byte array using UTF - 8 encoding.
                                                                       //3.) Compute the SHA-256 hash for the byte array from step 2. The result will be a byte array.
                                                                       //4.) Base64 encode the byte array as computed in step 3.This string will be the x - request - authentication key for this request.
            var xRequestAuthenticationKey = GetSha256Hash(identifierToken);
            client.DefaultRequestHeaders.TryAddWithoutValidation("X-REQUEST-AUTHENTICATION", xRequestAuthenticationKey);
            client.DefaultRequestHeaders.TryAddWithoutValidation("X-REQUEST-IDENTIFIER", xRequestIdentifier);

            var response = client.PostAsync(baseAddress, content).GetAwaiter().GetResult();
            return response;
        }

        public async Task<object> PostPrioJsonAsyncV2(object jsonObject, string authToken)
        {
            try
            {
                var baseAddress = PrioUrls;
                using (var client = new HttpClient() { Timeout = TimeSpan.FromMinutes(3) })
                {
                    var content = new StringContent(jsonObject.ToString());
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    //Create Request authentication key
                    //1.) Create a string with format
                    var xRequestIdentifier = Convert.ToString(Guid.NewGuid());
                    var identifierToken = $"{xRequestIdentifier}:{authToken}"; //<x-request-identifier>:<apikeytoken>
                                                                               //2.) Convert the string from step 1 to byte array using UTF - 8 encoding.
                                                                               //3.) Compute the SHA-256 hash for the byte array from step 2. The result will be a byte array.
                                                                               //4.) Base64 encode the byte array as computed in step 3.This string will be the x - request - authentication key for this request.
                    var xRequestAuthenticationKey = GetSha256Hash(identifierToken);
                    client.DefaultRequestHeaders.TryAddWithoutValidation("X-REQUEST-AUTHENTICATION", xRequestAuthenticationKey);
                    client.DefaultRequestHeaders.TryAddWithoutValidation("X-REQUEST-IDENTIFIER", xRequestIdentifier);

                    var response = await client.PostAsync(baseAddress, content);
                    return response;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public object PostApexxJsonAsync(string inputString, string ServiceUrl)
        {
            var client = new HttpClient { Timeout = TimeSpan.FromMinutes(3) };
            var content = new StringContent(inputString);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
            client.DefaultRequestHeaders.TryAddWithoutValidation("X-APIKEY", ApexxApiKey);
            var response = client.PostAsync(ServiceUrl, content).Result;
            return response;
        }
        public object PostAdyenJsonAsync(string inputString, string ServiceUrl)
        {
            var client = new HttpClient { Timeout = TimeSpan.FromMinutes(3) };
            var content = new StringContent(inputString);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
            client.DefaultRequestHeaders.TryAddWithoutValidation("x-api-key", AdyenApiKey);
            var response = client.PostAsync(ServiceUrl, content).Result;
            return response;
        }
        public static string GetSha256Hash(string input)
        {
            var data = Encoding.UTF8.GetBytes(input);
            using (HashAlgorithm sha = new SHA256Managed())
            {
                sha.TransformFinalBlock(data, 0, data.Length);
                return Convert.ToBase64String(sha.Hash);
            }
        }

        public async Task<object> PostJsonAsync<TIn>(string authToken, TIn parameter)
        {
            using (var handler = new HttpClientHandler())
            {
                handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
                using (var client = new HttpClient(handler))
                {
                    client.Timeout = TimeSpan.FromMinutes(3);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                    var response = await client.PostAsJsonAsync(ServiceURL, parameter);

                    if (response.IsSuccessStatusCode)
                    {
                        return response;
                    }
                }
            }

            return null;
        }
        public async Task<object> PostJsonAsyncWithoutAuth<TIn>(TIn parameter)
        {
            using (var handler = new HttpClientHandler())
            {
                handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
                using (var client = new HttpClient(handler))
                {
                    client.Timeout = TimeSpan.FromMinutes(3);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("Accept", "application/json");

                    var response = await client.PostAsJsonAsync(ServiceURL, parameter);

                    if (response.IsSuccessStatusCode)
                    {
                        return response;
                    }
                }
            }

            return null;
        }


        public async Task<string> GetAuthStringAsync(string userName, string password)
        {
            using (var handler = new HttpClientHandler())
            {
                handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
                using (var client = new HttpClient(handler))
                {
                    client.Timeout = TimeSpan.FromMinutes(3);
                    client.DefaultRequestHeaders.Clear();
                    var byteArray = Encoding.ASCII.GetBytes($"{userName}:{password}");
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                    var response = await client.GetStringAsync(ServiceURL);
                    return response;
                }
            }
        }

        public async Task<string> GetAuthString(string userName, string password)
        {
            using (var handler = new HttpClientHandler())
            {
                handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
                using (var client = new HttpClient(handler))
                {
                    client.Timeout = TimeSpan.FromMinutes(3);
                    client.DefaultRequestHeaders.Clear();
                    var byteArray = Encoding.ASCII.GetBytes($"{userName}:{password}");
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                    var response = await client.GetStringAsync(ServiceURL);
                    return response;
                }
            }
        }
        public string PostNbData(string postData)
        {
            // Didn't remove try-catch block as catch block has some conditions
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(ServiceURL);

                request.ContentType = "application/x-www-form-urlencoded";
                request.Method = "POST";
                request.Timeout = 5000;//5 seconds

                var data = $"{postData}";
                _bytes = Encoding.UTF8.GetBytes(data);

                request.ContentLength = _bytes.Length;
                var os = request.GetRequestStream();
                os.Write(_bytes, 0, _bytes.Length);

                using (WebResponse response = request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    var responseBody = reader.ReadToEnd();
                    return responseBody;
                }
            }
            catch (WebException wex)
            {
                if (wex.Response != null)
                {
                    using (var errorResponse = (HttpWebResponse)wex.Response)
                    {
                        using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                        {
                            //TODO: use JSON.net to parse this string and look at the error message
                        }
                    }
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Compute the signature to be used in the API call (combined key + secret + timestamp in seconds)
        /// </summary>
        /// <returns></returns>
        public static string GetSignature()
        {
            string signature;
            using (var sha = SHA256.Create())
            {
                long ts = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds / 1000;
                Console.WriteLine($"Timestamp: {ts}");
                var computedHash = sha.ComputeHash(Encoding.UTF8.GetBytes($"{ApiKey}{Secret}{ts}"));
                signature = BitConverter.ToString(computedHash).Replace("-", "");
            }
            return signature;
        }

        public static string ExternalIPAddress()
        {
            // Didn't remove try-catch block as catch block has some conditions
            try
            {
                new WebClient().DownloadString("http://icanhazip.com");
            }
            catch (Exception)
            {
                return "127.0.0.1";
            }
            return string.Empty.Replace("\n", "");
        }

        public async Task<object> PutJsonAsync<Tin>(Tin parameter)
        {
            string response;
            var signature = GetSignature();

            var uploadString = SerializeDeSerializeHelper.Serialize(parameter);
            using (var client = new WebClient())
            {
                // Request configuration
                client.Headers.Add("X-Signature", signature);
                client.Headers.Add("Api-Key", ApiKey);
                client.Headers.Add("Content-Type", "application/json");
                client.Headers.Add("X-Originating-Ip", ExternalIpAddress);
                client.Headers.Add("Accept", "application/json");
                response = await client.UploadStringTaskAsync(ServiceURL, "PUT", uploadString);
            }

            return response;
        }

        public async Task<object> DelJsonAsync<Tin>(Tin parameter, string apiKey, string secretKey)
        {
            ApiKey = apiKey;
            Secret = secretKey;
            string response;
            var signature = GetSignature();

            var uploadString = SerializeDeSerializeHelper.Serialize(parameter);
            using (var client = new WebClient())
            {
                client.Headers.Add("Accept", "application/json");
                client.Headers.Add("Api-Key", ApiKey);
                client.Headers.Add("X-Signature", signature);
                client.Headers.Add("X-Originating-Ip", ExternalIpAddress);
                client.Headers.Add("Content-Type", "application/json");
                response = await client.UploadStringTaskAsync(ServiceURL, "DELETE", uploadString);
            }

            return response;
        }

        public async Task<object> DelJsonAsync<Tin>(string authToken, Tin parameter)
        {
            string response;

            var uploadString = SerializeDeSerializeHelper.Serialize(parameter);
            using (var client = new WebClient())
            {
                // Request configuration
                client.Headers.Clear();
                client.Headers.Add("Content-Type", "application/json");
                client.Headers.Add("Accept", "application/json");
                client.Headers.Add("Authorization", $"Bearer {authToken}");

                response = await client.UploadStringTaskAsync(ServiceUri, "DELETE", uploadString);
            }

            return response;
        }

        public object DelJson<Tin>(string authToken, Tin parameter)
        {
            string response;

            var uploadString = SerializeDeSerializeHelper.Serialize(parameter);
            using (var client = new WebClient())
            {
                // Request configuration
                client.Headers.Clear();
                client.Headers.Add("Content-Type", "application/json");
                client.Headers.Add("Accept", "application/json");
                client.Headers.Add("Authorization", $"Bearer {authToken}");

                response = client.UploadString(ServiceURL, "DELETE", uploadString);
            }

            return response;
        }

        /// <summary>
        /// Get resposne prom url in xml and json. You can also pass headers from calling module.
        /// </summary>
        /// <param name="headers"></param>
        /// <returns></returns>

        public async Task<object> GetJsonAsync(Dictionary<string, string> headers)
        {
            string response;

            using (var client = new WebClient())
            {
                // Request configuration
                foreach (var item in headers)
                {
                    client.Headers.Add(item.Key, item.Value);
                }

                response = await client.DownloadStringTaskAsync(ServiceURL);
            }

            return response;
        }

        public object PostJsonWithHeader<Tin>(Tin data, Dictionary<string, string> headers)
        {
            string uploadString;
            string response;

            // Didn't remove try-catch block as catch block has some conditions
            try
            {
                uploadString = SerializeDeSerializeHelper.Serialize(data);
                using (var client = new WebClient())
                {
                    // Request configuration
                    foreach (var item in headers)
                    {
                        client.Headers.Add(item.Key, item.Value);
                    }
                    response = client.UploadString(ServiceURL, uploadString);
                }
            }
            catch (Exception ex)
            {
                response = $"Error :- {ex.Message}";
            }
            return response;
        }


        public async Task<Tuple<bool, string>> SendPostRequest(string authToken, string body)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            var client = new HttpClient { Timeout = TimeSpan.FromMinutes(3) };
            var content = new StringContent(body);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);
            var response = client.PostAsync(ServiceURL, content).Result;
            var responseText = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            return new Tuple<bool, string>(response.IsSuccessStatusCode, responseText);
        }

        public async Task<string> SendGetRequest(string authToken, string transId)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            var client = new HttpClient { Timeout = TimeSpan.FromMinutes(3) };
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);
            var response = client.GetAsync(ServiceURL + transId).Result;
            var responseText = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            if (response.IsSuccessStatusCode)
                return responseText;
            return "Unsuccessful API response.";
        }

        public object PostJsonWithHeader<Tin>(Tin data, Dictionary<string, string> headers, out string requestJson, out string responseJson)
        {
            var uploadString = string.Empty;
            string response;
            try
            {
                uploadString = SerializeDeSerializeHelper.Serialize(data);
                using (var client = new WebClient())
                {
                    // Request configuration
                    foreach (var item in headers)
                    {
                        client.Headers.Add(item.Key, item.Value);
                    }
                    response = client.UploadString(ServiceURL, uploadString);
                }
            }
            catch (WebException ex)
            {
                var statusCode = ((HttpWebResponse)ex.Response).StatusCode;
                response = "Error :- " + ex.Message;
            }
            catch (Exception ex)
            {
                response = "Error :- " + ex.Message;
            }
            requestJson = uploadString;
            responseJson = response;
            return response;
        }

        public async Task<object> PostJsonAsync<TIn, TOut>(string authToken, TIn parameter)
        {
            HttpResponseMessage response;

            using (var handler = new HttpClientHandler())
            {
                handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
                using (var client = new HttpClient(handler))
                {
                    client.Timeout = TimeSpan.FromMinutes(5);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                    response = await client.PostAsJsonAsync(ServiceURL, parameter);
                }
            }

            if (response.IsSuccessStatusCode)
            {
                return response;
            }

            return null;
        }

        /// <summary>
        /// HttpClient Upload data and sets header. Default POST, Possible Values are "POST,PUT"
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uploadData"> Data object </param>
        /// <param name="headers">Header to be set</param>
        /// <param name="MethodType">Default POST, Possible Values are "POST,PUT"</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> PostJsonWithHeadersAsync<T>(T uploadData, Dictionary<string, string> headers, string MethodType = "POST")
        {
            var response = default(HttpResponseMessage);

            // Didn't remove try-catch block as catch block has some conditions
            try
            {
                var uploadString = string.Empty;

                if (uploadData != null)
                {
                    var isString = uploadData.GetType().ToString() == "System.String";
                    if (isString)
                    {
                        uploadString = uploadData?.ToString();
                    }
                    else
                    {
                        uploadString = SerializeDeSerializeHelper.Serialize(uploadData);
                    }
                }

                var content = new StringContent(uploadString);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                using (var client = new HttpClient())
                {
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                    client.Timeout = TimeSpan.FromMinutes(3);
                    // Request configuration
                    foreach (var item in headers)
                    {
                        client.DefaultRequestHeaders.TryAddWithoutValidation(item.Key, item.Value);
                    }
                    if (MethodType == "POST")
                    {
                        response = client.PostAsync(ServiceURL, content)?.GetAwaiter().GetResult();
                    }
                    else if (MethodType == "PUT")
                    {
                        #region ### Read Static file to avoid actual booking

                        //string dataFromFile = string.Empty;
                        //using (StreamReader r = new StreamReader(@"C:\inetpub\wwwroot\BumbleBee\tempData\BookingConfirmRS_E-U10-NYCITYPASS.json"))
                        //{
                        //    dataFromFile = r.ReadToEnd();
                        //}
                        //content = new StringContent(dataFromFile);
                        //var retvlaue = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(dataFromFile, System.Text.Encoding.UTF8, "application/json") };
                        //return retvlaue;

                        #endregion ### Read Static file to avoid actual booking

                        client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
                        response = client.PutAsync(ServiceURL, content)?.GetAwaiter().GetResult();
                    }
                    else if (MethodType == "DELETE")
                    {
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
                        response =  client.DeleteAsync(ServiceURL)?.GetAwaiter().GetResult();
                    }
                    else if (MethodType == "GET")
                    {
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
                        response = await client.GetAsync(ServiceURL);
                    }
                }
            }
            catch (Exception ex)
            {
                response = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Content = new StringContent(ex.Message),
                };
            }
            return response;
        }
        /// <summary>
        /// ConsumeGetService
        /// </summary>
        /// <param name="httpHeaders"></param>
        /// <param name="queryParams"></param>
        /// <returns></returns>
        public object ConsumeGetService(IDictionary<string, string> httpHeaders, IDictionary<string, string> queryParams)
        {
            using (HttpRequestMessage reqMsg = new HttpRequestMessage(HttpMethod.Get, AddQueryString(ServiceURL, queryParams)))
            {
                AddCustomHttpHeaders(reqMsg, httpHeaders);
                return ConsumeService(reqMsg);
            }
        }

        public object ConsumeGetServicePrioHub(IDictionary<string, string> httpHeaders, IDictionary<string, string> queryParams)
        {
            using (HttpRequestMessage reqMsg = new HttpRequestMessage(HttpMethod.Get, AddQueryString(ServiceURL, queryParams)))
            {
                AddCustomHttpHeaders(reqMsg, httpHeaders);
                return ConsumeServicePrioHub(reqMsg);
            }
        }
        private void AddCustomHttpHeaders(HttpRequestMessage reqMsg, IDictionary<string, string> httpHeaders)
        {
            if (reqMsg == null || httpHeaders == null || httpHeaders?.Count == 0)
            {
                return;
            }

            foreach (KeyValuePair<string, string> httpHeader in httpHeaders)
            {
                try
                {
                    reqMsg.Headers.Add(httpHeader.Key, httpHeader.Value);
                }
                catch
                {
                    //throw
                }
            }
        }
        private string AddQueryString(string svcURL, IDictionary<string, string> queryParams)
        {
            StringBuilder urlBuilder = new StringBuilder(ServiceURL);
            if (queryParams != null && queryParams.Count > 0)
            {
                urlBuilder.Append("?");
                foreach (KeyValuePair<string, string> queryParam in queryParams)
                {
                    urlBuilder.Append($"{queryParam.Key}={queryParam.Value}&");
                }

                urlBuilder.Length = urlBuilder.Length - 1;
            }

            return urlBuilder.ToString();
        }
        private string ConsumeService(HttpRequestMessage reqMsg)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                httpClient.Timeout = TimeSpan.FromMinutes(10);
                using (HttpResponseMessage resMsg = httpClient.SendAsync(reqMsg)?.GetAwaiter().GetResult())
                {
                    var responseString = resMsg.Content.ReadAsStringAsync()?.GetAwaiter().GetResult() ?? null;
                    //var reqDetails = string.Empty;
                    //try
                    //{
                    //    reqDetails =  SerializeDeSerializeHelper.Serialize(resMsg.RequestMessage ?? reqMsg);
                    //}
                    //catch  
                    //{
                    //}
                    //if (_log == null)
                    //{
                    //    _log = new Logger.Logger();
                    //}
                    //else
                    //{
                    //    _log.Write(reqDetails?.ToString(), responseString, "Priohub", "Priohub", "Priohub");
                    //}


                    return responseString;
                    //return
                    //    (resMsg.StatusCode == HttpStatusCode.OK)
                    //        ? resMsg.Content.ReadAsStringAsync().Result
                    //        : null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private HttpResponseMessage ConsumeServicePrioHub(HttpRequestMessage reqMsg)
        {
            var reqDetails = SerializeDeSerializeHelper.Serialize(reqMsg);
            var responseString = string.Empty;
            try
            {
                HttpClient httpClient = new HttpClient();
                httpClient.Timeout = TimeSpan.FromMinutes(10);
                HttpResponseMessage resMsg = httpClient.SendAsync(reqMsg)?.GetAwaiter().GetResult();
                    //responseString = resMsg.Content.ReadAsStringAsync()?.GetAwaiter().GetResult() ?? null;
                    
                    //try
                    //{
                    //    reqDetails = SerializeDeSerializeHelper.Serialize(resMsg.RequestMessage ?? reqMsg);
                    //}
                    //catch
                    //{
                    //}
                    //if (_log == null)
                    //{
                    //    _log = new Logger.Logger();
                    //}
                    //else
                    //{
                    //    _log.Write(reqDetails?.ToString(), responseString, "Priohub", "Priohub", "Priohub");
                    //}


                    return resMsg;
                    //return
                    //    (resMsg.StatusCode == HttpStatusCode.OK)
                    //        ? resMsg.Content.ReadAsStringAsync().Result
                    //        : null;
            }
            catch (Exception ex)
            {
                if (_log == null)
                {
                    _log = new Logger.Logger();
                }
                _log.Write(reqDetails?.ToString(), responseString, "Priohub", "Priohub", "Priohub");
                return null;
            }
        }
        public Task<object> ConsumeGetServiceAsync(IDictionary<string, string> httpHeaders, IDictionary<string, string> queryParams)
        {
            using (HttpRequestMessage reqMsg = new HttpRequestMessage(HttpMethod.Get, AddQueryString(ServiceURL, queryParams)))
            {
                AddCustomHttpHeaders(reqMsg, httpHeaders);
                return ConsumeServiceAsync(reqMsg);
            }
        }
        private Task<object> ConsumeServiceAsync(HttpRequestMessage reqMsg)
        {
            HttpClient httpClient = new HttpClient();
            using (HttpResponseMessage resMsg = httpClient.SendAsync(reqMsg).Result)
            {
                // TODO: Check Http status
                return resMsg.Content.ReadAsAsync(typeof(object));
            }
        }
        public string ConsumePostService(IDictionary<string, string> httpHeaders, MediaTypeHeaderValue contentType, string payload, Encoding payloadEncoding)
        {
            using (HttpContent reqContent = new StringContent(payload, payloadEncoding))
            using (HttpRequestMessage reqMsg = new HttpRequestMessage(HttpMethod.Post, ServiceURL))
            {
                AddCustomHttpHeaders(reqMsg, httpHeaders);
                reqContent.Headers.ContentType = contentType;
                reqMsg.Content = reqContent;
                return ConsumeService(reqMsg);
            }
        }

        public HttpResponseMessage ConsumePostServiceHttpResponse(IDictionary<string, string> httpHeaders, MediaTypeHeaderValue contentType, string payload, Encoding payloadEncoding)
        {
            using (HttpContent reqContent = new StringContent(payload, payloadEncoding))
            using (HttpRequestMessage reqMsg = new HttpRequestMessage(HttpMethod.Post, ServiceURL))
            {
                AddCustomHttpHeaders(reqMsg, httpHeaders);
                reqContent.Headers.ContentType = contentType;
                reqMsg.Content = reqContent;
                return ConsumeServiceHttpResponse(reqMsg);
            }
        }

        private HttpResponseMessage ConsumeServiceHttpResponse(HttpRequestMessage reqMsg)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.Timeout = TimeSpan.FromMinutes(10);
                    HttpResponseMessage resMsg = httpClient.SendAsync(reqMsg)?.GetAwaiter().GetResult();
                    //var responseString = resMsg.Content.ReadAsStringAsync()?.GetAwaiter().GetResult() ?? null;
                    return resMsg;

                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}