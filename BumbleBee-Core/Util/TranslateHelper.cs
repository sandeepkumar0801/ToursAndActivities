using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Util
{
    public class TranslateHelper
    {
        private static readonly string key = "89d2bd642edb4a07b1e19decee48900e";
        private static readonly string endpoint = "https://api.cognitive.microsofttranslator.com/";
        private static readonly string location = "westeurope";

        public static string Translate(string text, string language)
        {
            try
            {
                if (string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text))
                {
                    return string.Empty;
                }
                // Input and output languages are defined as parameters.
                string route = $"/translate?api-version=3.0&from=en&to=" + language + "";
                object[] body = new object[] { new { Text = text } };
                var requestBody = JsonConvert.SerializeObject(body);

                using (var client = new HttpClient())
                using (var request = new HttpRequestMessage())
                {
                    // Build the request.
                    request.Method = HttpMethod.Post;
                    request.RequestUri = new Uri(endpoint + route);
                    request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                    request.Headers.Add("Ocp-Apim-Subscription-Key", key);
                    request.Headers.Add("Ocp-Apim-Subscription-Region", location);

                    // Send the request and get response.
                    HttpResponseMessage response = client.SendAsync(request)?.GetAwaiter().GetResult();
                    // Read response as a string.
                    string responseBody = response.Content.ReadAsStringAsync()?.GetAwaiter().GetResult();
                    var result = JsonConvert.DeserializeObject<TranslationResult[]>(responseBody);

                    //return result;;
                    var translatedString = result?.FirstOrDefault()?.translations?.FirstOrDefault()?.text?.ToString();

                    return translatedString;
                }
            }
            catch (Exception ex)
            {
                return text;
            }
        }
    }

    public class TranslationResult
    {
        public Translation[] translations { get; set; }
    }

    public class Translation
    {
        public string text { get; set; }
        public string to { get; set; }
    }
}
