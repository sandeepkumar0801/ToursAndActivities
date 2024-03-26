using Logger.Contract;
using ServiceAdapters.Adyen.Adyen.Entities;
using ServiceAdapters.Adyen.Constants;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Util;

namespace ServiceAdapters.Adyen.Adyen.Commands
{
    public abstract class CommandHandlerBase
    {
        private ILogger _log;

        protected CommandHandlerBase(ILogger log)
        {
            _log = log;
        }

        public virtual string Execute(AdyenCriteria adyenCriteria, string token, int adyenMerchantType = 1)
        {
            var watch = Stopwatch.StartNew();
            var request = CreateInputRequest(adyenCriteria, adyenMerchantType);
            var jsonResponse = AdyenApiRequest(request, token);

            var response = (HttpResponseMessage)jsonResponse;
            var responseText = response.Content.ReadAsStringAsync().Result;

            watch.Stop();
            var saveInStorage = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.SaveInStorage);
            if (saveInStorage.Equals(Constant.SaveInStorageValue))
            {
                var requestLog = SerializeDeSerializeHelper.Serialize(request);
                var requestResponse = responseText;

                _log.WriteTimer(adyenCriteria.MethodType.ToString(), token, "Adyen", watch.Elapsed.ToString());
                _log.Write(requestLog, requestResponse, adyenCriteria.MethodType.ToString(), token, "Adyen");
            }

            return responseText;
        }

        protected abstract object CreateInputRequest(AdyenCriteria adyenCriteria, int adyenMerchantType = 1);

        protected abstract object AdyenApiRequest(object inputJson, string token);

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

        public string GetAdyenMerchantAccount(int adyenMerchantType = 1)
        {
            var returnMerchantAccount = string.Empty;
            switch (adyenMerchantType)
            {
                case 0:
                    returnMerchantAccount=ConfigurationManagerHelper.GetValuefromAppSettings(Constant.AdyenMerchantAccount);
                    break;
                case 1:
                    returnMerchantAccount=ConfigurationManagerHelper.GetValuefromAppSettings(Constant.AdyenMerchantAccountNew);
                    break;
                case 2:
                    returnMerchantAccount=ConfigurationManagerHelper.GetValuefromAppSettings(Constant.AdyenMerchantAccountStringent);
                    break;
                default:
                    returnMerchantAccount=ConfigurationManagerHelper.GetValuefromAppSettings(Constant.AdyenMerchantAccountNew);
                    break;
            }
            return returnMerchantAccount;
        }
    }
}