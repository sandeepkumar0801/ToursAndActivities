using System;
using Util;

namespace ServiceAdapters.Bokun.Bokun.Entities
{
    public class BokunAPIConfig
    {
        /// <summary>
        /// Provide instance that reads config values for BokunAPI
        /// </summary>
        public static BokunAPIConfig Instance { get; set; }

        public string BaseUri;
        public string SecretKey { get; set; }
        public string AccessKey { get; set; }

        public bool IsNotificationOn { get; set; }
        public bool IsSendNotificationToCustomer { get; set; }
        public string NotificationEmailAddressIsango { get; set; }
        public string SupportPhoneNumer { get; set; }
        public bool IsAppendBokunAPIDescription { get; set; }

        private BokunAPIConfig()
        {
            BaseUri = Convert.ToString(ConfigurationManagerHelper.GetValuefromAppSettings("BokunURI"));
            SecretKey = Convert.ToString(ConfigurationManagerHelper.GetValuefromAppSettings("BokunSecretKey"));
            AccessKey = Convert.ToString(ConfigurationManagerHelper.GetValuefromAppSettings("BokunAccessKey"));

            try
            {
                IsNotificationOn = Convert.ToString(ConfigurationManagerHelper.GetValuefromAppSettings("BokunIsNotificationOn")) == "1";
                IsSendNotificationToCustomer = Convert.ToString(ConfigurationManagerHelper.GetValuefromAppSettings("BokunIsSendNotificationToCustomer")) == "1";
                NotificationEmailAddressIsango = Convert.ToString(ConfigurationManagerHelper.GetValuefromAppSettings("BokunNotificationEmailAddressIsango"));
                SupportPhoneNumer = Convert.ToString(ConfigurationManagerHelper.GetValuefromAppSettings("SupportPhoneNumer"));
                IsAppendBokunAPIDescription = Convert.ToString(ConfigurationManagerHelper.GetValuefromAppSettings("IsAppendBokunAPIDescription")) == "1";
            }
            catch (Exception)
            {
                IsNotificationOn = true;
                IsSendNotificationToCustomer = false;
                NotificationEmailAddressIsango = "support@isango.com";
                SupportPhoneNumer = "+4402033551240";
                IsAppendBokunAPIDescription = false;
            }
        }

        public static BokunAPIConfig GetInstance()
        {
            return Instance ?? (Instance = new BokunAPIConfig());
        }
    }
}