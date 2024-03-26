using EmailSuitConsole.Data;
using EmailSuitConsole.Models;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace EmailSuitConsole.Service
{
    public class EmailConfig
    {
       
        private readonly SMTPConfigModel _smtpConfig;
        private readonly IsangoDataBaseLive _isangoDataBaseLive;
        private readonly EmailTemplateSettings _emailTemplateSettings;
        public EmailConfig(IOptions<SMTPConfigModel> smtpConfig, IsangoDataBaseLive isangoDataBaseLive, IOptions<EmailTemplateSettings> emailTemplateSettings)
        {
            _smtpConfig = smtpConfig.Value;
            _isangoDataBaseLive = isangoDataBaseLive;
            _emailTemplateSettings = emailTemplateSettings.Value;
        }
        public async Task SendEmail(UserEmailOptions userEmailOptions)
        {
            try
            {
                MailMessage mail = new MailMessage
                {
                    Subject = userEmailOptions.Subject,
                    Body = userEmailOptions.Body,
                    From = new MailAddress(_smtpConfig.SenderAddress, _smtpConfig.SenderDisplayName),
                    IsBodyHtml = _smtpConfig.IsBodyHTML
                };

                foreach (var toEmail in userEmailOptions.ToEmails)
                {
                    mail.To.Add(toEmail);
                }

              
                NetworkCredential networkCredential = new NetworkCredential(_smtpConfig.UserName, _smtpConfig.Password);

                SmtpClient smtpClient = new SmtpClient
                {
                    Host = _smtpConfig.Host,
                    Port = _smtpConfig.Port,
                    EnableSsl = _smtpConfig.EnableSSL,
                    UseDefaultCredentials = _smtpConfig.UseDefaultCredentials,
                    Credentials = networkCredential
                };
                mail.BodyEncoding = Encoding.Default;
                await smtpClient.SendMailAsync(mail);
                Console.WriteLine("Email sent successfully.");

            }
            catch (Exception ex)
            {
                // Handle the exception appropriately
            }
        }


        public List<Product> GetProductData(string storedProcedureName)
        {
            
                List<Product> products = _isangoDataBaseLive.ExecuteReader(storedProcedureName);
            

            return products;
        }
        public List<TiqetsProduct> GetProductData1(string storedProcedureName)
        {

            List<TiqetsProduct> results = _isangoDataBaseLive.ExecuteReader_Get(storedProcedureName);


            return results;
        }

        
        public List<Variant> GetProductData_Variant(string storedProcedureName)
        {

            List<Variant> results = _isangoDataBaseLive.ExecuteReader_VariantData(storedProcedureName);


            return results;
        }

        public List<TourCMSPax> GetProductData_TourCMSPax(string storedProcedureName)
        {

            List<TourCMSPax> results = _isangoDataBaseLive.ExecuteReader_TourCMSPaxData(storedProcedureName);


            return results;
        }


        public List<TourCMSProduct> GetProductDetai1TourCMS(string storedProcedureName)
        {

            List<TourCMSProduct> results = _isangoDataBaseLive.ExecuteReader_GetTourCMS(storedProcedureName);


            return results;
        }

        public List<RaynaProduct> GetProductDetai1Rayna(string storedProcedureName)
        {

            List<RaynaProduct> results = _isangoDataBaseLive.ExecuteReader_GetRayna(storedProcedureName);


            return results;
        }
        public List<GlobalTixV3Product> GetProductDetai1GlobalTixV3(string storedProcedureName)
        {

            List<GlobalTixV3Product> results = _isangoDataBaseLive.ExecuteReader_GetGlobalTixV3(storedProcedureName);


            return results;
        }

        public List<RaynaProductOptions> GetProductDetai1RaynaOptions(string storedProcedureName)
        {

            List<RaynaProductOptions> results = _isangoDataBaseLive.ExecuteReader_GetRaynaOptions(storedProcedureName);


            return results;
        }
        public List<GlobalTixV3ProductOptions> GetProductDetai1GlobalTixV3Options(string storedProcedureName)
        {

            List<GlobalTixV3ProductOptions> results = _isangoDataBaseLive.ExecuteReader_GetGlobalTixV3Options(storedProcedureName);


            return results;
        }

        public async Task SendEmail(UserEmailOptions userEmailOptions, string emailSubject, string emailBodyTemplate = null, StringBuilder productsTable = null, StringBuilder productsTable_active = null) 
        {
            userEmailOptions.Subject = emailSubject;
            if (emailBodyTemplate != null)
            {
                var body = GetEmailBody(emailBodyTemplate, this);

                if ((productsTable_active != null && productsTable_active.Length > 0) ||
                 (productsTable != null && productsTable.Length > 0))
                {
                    if (productsTable != null && productsTable.Length > 0)
                    {
                        body = body.Replace("{{ProductsTable}}", productsTable.ToString());
                    }
                    else
                    {
                        string pattern = @"<div class=""offline"">.*?</div>";
                        body = Regex.Replace(body, pattern, "", RegexOptions.Singleline);
                    }
                    if (productsTable_active != null && productsTable_active.Length > 0)
                    {
                        body = body.Replace("{{ProductsTable_active}}", productsTable_active.ToString());
                    }
                    else
                    {
                        string pattern = @"<div class=""online"">.*?</div>";
                        body = Regex.Replace(body, pattern, "", RegexOptions.Singleline);
                    }
                    userEmailOptions.Body = body;
                    await SendEmail(userEmailOptions);
                }
                else
                {
                    Console.WriteLine("No Product Recorded today");
                }

            }
            else
            {
                await SendEmail(userEmailOptions);

            }



        }

         
        public static string GetEmailBody(string templateName, EmailConfig emailConfig)
        {
            var templatePath = emailConfig._emailTemplateSettings.TemplatePath;

            if (string.IsNullOrEmpty(templatePath))
            {
                // Handle the case when templatePath is null or empty
                throw new ArgumentException("Template path is null or empty.");
            }

            var body = File.ReadAllText(string.Format(templatePath, templateName));
            return body;
        }

    }
}
