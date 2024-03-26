using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace FeefoDownloader
{
    public class Helper
    {
        private  static IConfiguration _configuration;

        public Helper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public static string FeefoUrl => _configuration.GetValue<string>("AppSettings:FeefoUrl").Replace("&amp;", "&");

        public static string Since => _configuration.GetValue<string>("AppSettings:Since");

        public static string Limit => _configuration.GetValue<string>("AppSettings:Limit");

        public   string[] Modes => _configuration.GetValue<string>("AppSettings:Modes").Split(',');

        public static bool IsOfflineEmail => _configuration.GetValue<bool>("AppSettings:IsOfflineEmail");

        public static bool SendSupportMail(string message, string url, string errorMessage)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("**************************FEEFO DOWNLOAD ERROR********************************");
                stringBuilder.Append("<br>Message: " + message);
                if (!string.IsNullOrWhiteSpace(url))
                    stringBuilder.Append("<br>Url: " + url);
                stringBuilder.Append("<br>Exception: " + errorMessage);
                stringBuilder.Append("*******************************************************************************");
                // new MailGeneratorBase().SendMailAsHTML("Customer enquiry", "<RoundRobinEmails>", "hara.prasad@isango.com", "", "", stringBuilder.ToString(), Helper.IsOfflineEmail);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
