using System;
using Util;
using Constant = ServiceAdapters.RedeamV12.Constants.Constant;

namespace ServiceAdapters.RedeamV12.RedeamV12.Entities
{
    public class RedeamV12APIConfig
    {
        /// <summary>
        /// Provide instance that reads config values for BokunAPI
        /// </summary>
        public static RedeamV12APIConfig Instance { get; set; }

        public string BaseUri;
        public string SecretKey { get; set; }
        public string AccessKey { get; set; }

      
        public string NotificationEmailAddressIsango { get; set; }

        public string SupportPhoneNumer { get; set; }
        

        private RedeamV12APIConfig()
        {
            BaseUri = Convert.ToString(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.BaseAddressV12));
            SecretKey = Convert.ToString(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.ApiSecretKeyV12));
            AccessKey = Convert.ToString(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.ApiKeyV12));
            try
            {

                NotificationEmailAddressIsango = Convert.ToString(ConfigurationManagerHelper.GetValuefromAppSettings("BokunNotificationEmailAddressIsango"));
                SupportPhoneNumer = Convert.ToString(ConfigurationManagerHelper.GetValuefromAppSettings("SupportPhoneNumer"));
            }
            catch (Exception)
            {
               NotificationEmailAddressIsango = "support@isango.com";
               SupportPhoneNumer = "0124 4148173";
            }
        }

        public static RedeamV12APIConfig GetInstance()
        {
            return Instance ?? (Instance = new RedeamV12APIConfig());
        }
    }
}