using System;
using System.Xml.Serialization;
using Util;

namespace ServiceAdapters.MoulinRouge.MoulinRouge.Entities
{
    [SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://tempuri.org/")]
    [XmlRootAttribute(Namespace = "http://tempuri.org/", IsNullable = false, ElementName = "context")]
    public class RequestCredentialsContext
    {
        private RequestCredentialsContext()
        {
            KEY_Database = Convert.ToString(ConfigurationManagerHelper.GetValuefromAppSettings("KEY_Database"));
            Login = Convert.ToString(ConfigurationManagerHelper.GetValuefromAppSettings("Login"));
            Password = Convert.ToString(ConfigurationManagerHelper.GetValuefromAppSettings("MoulinPassword"));
            Interface = Convert.ToInt32(ConfigurationManagerHelper.GetValuefromAppSettings("Interface"));
            ID_WitheLabel = Convert.ToInt32(ConfigurationManagerHelper.GetValuefromAppSettings("ID_WitheLabel"));
        }

        [XmlIgnore]
        private static RequestCredentialsContext ContextInstance { get; set; }

        #region Properties

        /// <remarks/>
        public string KEY_Database { get; set; }

        /// <remarks/>
        public string Login { get; set; }

        /// <remarks/>
        public string Password { get; set; }

        /// <remarks/>
        public int Interface { get; set; }

        /// <remarks/>
        public int ID_WitheLabel { get; set; }

        #endregion Properties

        public static RequestCredentialsContext GetContextInstance()
        {
            return ContextInstance ?? (ContextInstance = new RequestCredentialsContext());
        }
    }
}