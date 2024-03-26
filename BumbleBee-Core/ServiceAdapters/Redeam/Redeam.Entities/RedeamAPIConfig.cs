using System;
using Util;
using Constant = ServiceAdapters.Redeam.Constants.Constant;

namespace ServiceAdapters.Redeam.Redeam.Entities
{
    public class RedeamAPIConfig
    {
        /// <summary>
        /// Provide instance that reads config values for BokunAPI
        /// </summary>
        public static RedeamAPIConfig Instance { get; set; }

        public string BaseUri;
        public string SecretKey { get; set; }
        public string AccessKey { get; set; }

        //public bool IsNotificationOn { get; set; }
        //public bool IsSendNotificationToCustomer { get; set; }
        public string NotificationEmailAddressIsango { get; set; }

        public string SupportPhoneNumer { get; set; }
        //public bool IsAppendBokunAPIDescription { get; set; }

        private RedeamAPIConfig()
        {
            BaseUri = Convert.ToString(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.BaseAddress));
            SecretKey = Convert.ToString(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.ApiSecretKey));
            AccessKey = Convert.ToString(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.ApiKey));

            try
            {
                //IsNotificationOn = Convert.ToString(ConfigurationManagerHelper.GetValuefromAppSettings("BokunIsNotificationOn")) == "1";
                //IsSendNotificationToCustomer = Convert.ToString(ConfigurationManagerHelper.GetValuefromAppSettings("BokunIsSendNotificationToCustomer")) == "1";
                NotificationEmailAddressIsango = Convert.ToString(ConfigurationManagerHelper.GetValuefromAppSettings("BokunNotificationEmailAddressIsango"));
                SupportPhoneNumer = Convert.ToString(ConfigurationManagerHelper.GetValuefromAppSettings("SupportPhoneNumer"));
                //IsAppendBokunAPIDescription = Convert.ToString(ConfigurationManagerHelper.GetValuefromAppSettings("IsAppendBokunAPIDescription")) == "1";
            }
            catch (Exception)
            {
                //IsNotificationOn = true;
                //IsSendNotificationToCustomer = false;
                NotificationEmailAddressIsango = "support@isango.com";
                SupportPhoneNumer = "0124 4148173";
                //IsAppendBokunAPIDescription = false;
            }
        }

        public static RedeamAPIConfig GetInstance()
        {
            return Instance ?? (Instance = new RedeamAPIConfig());
        }
    }
}