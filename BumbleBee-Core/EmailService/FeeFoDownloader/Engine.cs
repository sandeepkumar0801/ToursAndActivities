using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;
using Microsoft.Extensions.Configuration;

namespace FeefoDownloader
{
    public class Engine
    {
        private static IConfiguration _configuration;
        private readonly DataAccess dataobj;
        //public readonly Helper _helper; 
        public Engine(DataAccess dataAccess, IConfiguration configuration)
        {
            dataobj = dataAccess;
            _configuration = configuration;
            //_helper= helper;
            
        }


        public void Process()
        {
            try
            {
                Helper h1 = new Helper(_configuration);
                foreach (Affiliate affiliate in dataobj.GetAffiliates())
                {
                    foreach (string mode in h1.Modes)
                    {
                        FEEDBACKLIST feed = GetFeed(GetUrl(FilterWebUrl(affiliate.CompanyWebsite), affiliate.FeefoKey, mode), affiliate.AffiliateID.ToString(), mode);
                        if (feed != null && feed.FEEDBACK != null && feed.FEEDBACK.Any<FEEDBACKLISTFEEDBACK>())
                            dataobj.FeefoReviews_Insert(feed.FEEDBACK, mode, affiliate.AffiliateID);
                    }
                }
                dataobj.FinalizeData();
            }
            catch (Exception ex)
            {
                //Helper.SendSupportMail("Some critical error found, Please refer to stack trace", null, $"Message: {ex.Message}{Environment.NewLine}Stack trace: {ex.StackTrace}");
                throw new InvalidOperationException("Encountered Process error ", ex);

            }
        }

        private string FilterWebUrl(string weburl) => weburl.Replace("https://", string.Empty).Replace("http://", string.Empty).TrimEnd('/');

        private string GetUrl(string logon, string password, string mode) => string.Format(Helper.FeefoUrl, logon, password, mode, Helper.Since, Helper.Limit);

        private FEEDBACKLIST GetFeed(string path, string affiliatedId, string mode)
        {
            try
            { 
                var isloggingFeefo = _configuration.GetValue<string>("AppSettings:isloggingFeefo");
                string myFilePath = _configuration.GetValue<string>("AppSettings:FeefoFilePath");
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(path);
                if (isloggingFeefo == "1")
                {
                    string filePath = Path.Combine(myFilePath, $"{affiliatedId}_{mode}.xml");

                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }

                    xmlDocument.Save(filePath);
                }

                using (StreamReader streamReader = new StreamReader(GenerateStreamFromString(xmlDocument.InnerXml)))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(FEEDBACKLIST));
                    return (FEEDBACKLIST)serializer.Deserialize(streamReader);
                }
            }
            catch (Exception ex)
            {
                //Helper.SendSupportMail("Could not parse XML", path, ex.Message);
                throw new InvalidOperationException("Encountered Process error ", ex);

                return null;
            }
        }

        private Stream GenerateStreamFromString(string s)
        {
            MemoryStream streamFromString = new MemoryStream();
            StreamWriter streamWriter = new StreamWriter(streamFromString);
            streamWriter.Write(s);
            streamWriter.Flush();
            streamFromString.Position = 0L;
            return streamFromString;
        }

    }
}
