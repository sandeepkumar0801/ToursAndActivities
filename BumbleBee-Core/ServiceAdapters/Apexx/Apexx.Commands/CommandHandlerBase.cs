using Logger.Contract;
using ServiceAdapters.Apexx.Apexx.Entities;
using ServiceAdapters.Apexx.Constants;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Util;

namespace ServiceAdapters.Apexx.Apexx.Commands
{
    public abstract class CommandHandlerBase
    {
        private ILogger _log;

        protected CommandHandlerBase(ILogger log)
        {
            _log = log;
        }

        public virtual string Execute(ApexxCriteria apexxCriteria, string token)
        {
            var watch = Stopwatch.StartNew();
            var request = CreateInputRequest(apexxCriteria);
            var jsonResponse = ApexxApiRequest(request, token);

            var response = (HttpResponseMessage)jsonResponse;
            var responseText = response.Content.ReadAsStringAsync().Result;

            watch.Stop();
            var saveInStorage = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.SaveInStorage);
            if (saveInStorage.Equals(Constant.SaveInStorageValue))
            {
                var requestLog = RemoveCardDetails(SerializeDeSerializeHelper.Serialize(request), (int)apexxCriteria.MethodType);
                var requestResponse = RemoveCardDetails(responseText, (int)apexxCriteria.MethodType);

                _log.WriteTimer(apexxCriteria.MethodType.ToString(), token, "Apexx", watch.Elapsed.ToString());
                _log.Write(requestLog, requestResponse, apexxCriteria.MethodType.ToString(), token, "Apexx");
            }

            return responseText;
        }

        protected abstract object CreateInputRequest(ApexxCriteria apexxCriteria);

        protected abstract object ApexxApiRequest(object inputJson, string token);

        private string RemoveCardDetails(string text, int condition)
        {
            var updatedText = text;
            if (!string.IsNullOrWhiteSpace(updatedText))
            {
                try
                {
                    if (condition == 1)// Enrollement Check Request Condition
                    {
                        Regex regex = new Regex(@"card(.*?)customer_ip");
                        updatedText = regex.Replace(text, @"customer_ip");
                    }
                    else if (condition == 2)// 3 DS Verify Response Condition
                    {
                        Regex regex = new Regex(@"card(.*?)payment_product");
                        updatedText = regex.Replace(text, @"payment_product");
                    }
                }
                catch (Exception ex)
                {
                    updatedText += ex.Message;
                }
            }
            return updatedText;
        }
    }
}