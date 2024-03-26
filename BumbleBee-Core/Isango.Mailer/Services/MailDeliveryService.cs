using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Isango.Entities;
using Isango.Entities.Mailer;
using Isango.Mailer.ServiceContracts;
using Logger.Contract;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.IO;
using Util;
using Constant = Isango.Mailer.Constants.Constant;

namespace Isango.Mailer.Services
{
    public class MailDeliveryService : IMailDeliveryService
    {
        private SendGridClient _sendGridClient;

        private readonly ILogger _log;

        public MailDeliveryService(ILogger logger)
        {
            _log = logger;
        }

        /// <summary>
        /// Method used to send mails through SendGrid
        /// </summary>
        /// <param name="mailContext"></param>
        /// <param name="attachments"></param>
        public void SendMail(MailContext mailContext, List<System.Net.Mail.Attachment> attachments = null)
        {
            try
            {
                var mailType = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.MailTypeSMTP);
                if (mailType == "1")
                {
                    SMTPServerEmail(mailContext, attachments);
                }
                else
                {
                    if (mailContext != null)
                    {
                        if (_sendGridClient == null)
                        {
                            _sendGridClient = new SendGridClient(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.SendGridAppKey));
                        }

                        var sendGridMessage = CreateSendGridMessage(mailContext, attachments);
                        var result = _sendGridClient.SendEmailAsync(sendGridMessage).GetAwaiter().GetResult();
                        //var test = result.StatusCode.ToString();
                        _log.Info(
                            new IsangoErrorEntity
                            {
                                Params = $"{result?.StatusCode.ToString()} - Email Sent",
                                MethodName = nameof(SendMail),
                                ClassName = nameof(MailDeliveryService),
                                Token = "SendGrid"
                            });
                    }
                }
            }
            catch (Exception ex)
            {

                //throw;
            }
        }

        #region Private Methods

        private void SMTPServerEmail(MailContext mailContext, List<System.Net.Mail.Attachment> attachments)
        {
            try
            {
                var SMTPPort = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.SMTPPort);
                var SMTPHost = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.SMTPHost);

                var SMTPFromEmail = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.SMTPFromEmail);
                var SMTPPassword = string.Empty;
                var SMTPUserName = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.SMTPUserName);

                var message = new System.Net.Mail.MailMessage();
                var smtp = new System.Net.Mail.SmtpClient();

                try
                {
                    var isKeyVaultEnabled = ConfigurationManagerHelper.GetValuefromAppSettings("IsKeyVaultEnabled") == "1";
                    if (isKeyVaultEnabled)
                    {
                        var vaultName = ConfigurationManagerHelper.GetValuefromAppSettings("KeyVaultName");
                        var kvUri = $"https://" + vaultName + ".vault.azure.net";

                        var client = new SecretClient(new Uri(kvUri), new DefaultAzureCredential());
                        message.From = new System.Net.Mail.MailAddress(SMTPFromEmail);
                        try
                        {
                            if (mailContext.From.Email.ToLower().Contains("hop-on-hop-off"))
                            {
                                message.From = new System.Net.Mail.MailAddress(mailContext.From.Email);
                                var password = client.GetSecretAsync(Constant.SMTPPasswordHOHOKeyVault).GetAwaiter().GetResult();
                                SMTPPassword = password.Value.Value;
                                SMTPUserName = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.SMTPUserNameHOHO);
                            }
                            else if (mailContext.From.Email.ToLower().Contains("the-boat-tours"))
                            {
                                message.From = new System.Net.Mail.MailAddress(mailContext.From.Email);
                                var password = client.GetSecretAsync(Constant.SMTPPasswordBOATKeyVault).GetAwaiter().GetResult();
                                SMTPPassword = password.Value.Value;
                                SMTPUserName = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.SMTPUserNameBOAT);
                            }
                            else
                            {
                                var password = client.GetSecretAsync(Constant.SMTPPasswordKeyVault).GetAwaiter().GetResult();
                                SMTPPassword = password.Value.Value;
                            }
                        }
                        catch (Exception ex)
                        {
                            _log.Info(
                                       new IsangoErrorEntity
                                       {
                                           Params = Convert.ToString(ex.InnerException),
                                           MethodName = nameof(SendMail),
                                           ClassName = nameof(MailDeliveryService),
                                           Token = "SendGrid"
                                       });

                            message.From = new System.Net.Mail.MailAddress(SMTPFromEmail);
                            SMTPPassword = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.SMTPPassword);
                            SMTPUserName = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.SMTPUserName);
                        }
                    }
                    else
                    {
                        message.From = new System.Net.Mail.MailAddress(SMTPFromEmail);
                        try
                        {
                            if (mailContext.From.Email.ToLower().Contains("hop-on-hop-off"))
                            {
                                message.From = new System.Net.Mail.MailAddress(mailContext.From.Email);
                                SMTPPassword = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.SMTPPasswordHOHO);
                                SMTPUserName = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.SMTPUserNameHOHO);
                            }
                            else if (mailContext.From.Email.ToLower().Contains("the-boat-tours"))
                            {
                                message.From = new System.Net.Mail.MailAddress(mailContext.From.Email);
                                SMTPPassword = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.SMTPPasswordBOAT);
                                SMTPUserName = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.SMTPUserNameBOAT);
                            }
                            else
                            {
                                SMTPPassword = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.SMTPPassword);
                            }
                        }
                        catch (Exception ex)
                        {
                            _log.Info(
                                       new IsangoErrorEntity
                                       {
                                           Params = Convert.ToString(ex.InnerException),
                                           MethodName = nameof(SendMail),
                                           ClassName = nameof(MailDeliveryService),
                                           Token = "SendGrid"
                                       });

                            message.From = new System.Net.Mail.MailAddress(SMTPFromEmail);
                            SMTPPassword = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.SMTPPassword);
                            SMTPUserName = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.SMTPUserName);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _log.Info(
                               new IsangoErrorEntity
                               {
                                   Params = Convert.ToString(ex.InnerException),
                                   MethodName = nameof(SendMail),
                                   ClassName = nameof(MailDeliveryService),
                                   Token = "SendGrid"
                               });

                    message.From = new System.Net.Mail.MailAddress(SMTPFromEmail);
                    try
                    {
                        if (mailContext.From.Email.ToLower().Contains("hop-on-hop-off"))
                        {
                            message.From = new System.Net.Mail.MailAddress(mailContext.From.Email);
                            SMTPPassword = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.SMTPPasswordHOHO);
                            SMTPUserName = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.SMTPUserNameHOHO);
                        }
                        else if (mailContext.From.Email.ToLower().Contains("the-boat-tours"))
                        {
                            message.From = new System.Net.Mail.MailAddress(mailContext.From.Email);
                            SMTPPassword = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.SMTPPasswordBOAT);
                            SMTPUserName = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.SMTPUserNameBOAT);
                        }
                        else
                        {
                            SMTPPassword = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.SMTPPassword);
                        }
                    }
                    catch (Exception e)
                    {
                        message.From = new System.Net.Mail.MailAddress(SMTPFromEmail);
                        SMTPPassword = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.SMTPPassword);
                        SMTPUserName = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.SMTPUserName);

                        _log.Info(
                       new IsangoErrorEntity
                       {
                           Params = Convert.ToString(e.InnerException),
                           MethodName = nameof(SendMail),
                           ClassName = nameof(MailDeliveryService),
                           Token = "SendGrid"
                       });
                    }
                }
                
                var toCount = mailContext.To;
                if (!toCount.IsNullOrEmpty())
                {
                    foreach (var tt in toCount)
                    {
                        message.To.Add(new System.Net.Mail.MailAddress(tt));
                    }
                }

                var ccCount = mailContext.CC;
                if (!ccCount.IsNullOrEmpty())
                {
                    foreach (var cc in ccCount)
                    {
                        message.CC.Add(new System.Net.Mail.MailAddress(cc));
                    }
                }
                var bccCount = mailContext.BCC;
                if (!bccCount.IsNullOrEmpty())
                {
                    foreach (var bcc in bccCount)
                    {
                        message.Bcc.Add(new System.Net.Mail.MailAddress(bcc));
                    }
                }

                if (attachments != null && attachments.Count > 0)
                {
                    foreach (var attachment in attachments)
                    {
                        message.Attachments.Add(attachment);
                    }
                }

                message.Subject = mailContext.Subject;
                message.IsBodyHtml = true; //to make message body as html  
                message.Body = mailContext.HtmlContent;
                smtp.Port = Convert.ToInt32(SMTPPort);
                smtp.Host = SMTPHost; //for gmail host  
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential(SMTPUserName, SMTPPassword);
                smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                smtp.Send(message);

                _log.Info(
                        new IsangoErrorEntity
                        {
                            Params = "SMTPServerEmail - Email Sent",
                            MethodName = nameof(SMTPServerEmail),
                            ClassName = nameof(MailDeliveryService),
                            Token = "SendGrid"
                        });
                }
            catch (Exception ex)
            {
                _log.Info(
                       new IsangoErrorEntity
                       {
                           Params =Convert.ToString(ex.InnerException),
                           MethodName = nameof(SendMail),
                           ClassName = nameof(MailDeliveryService),
                           Token = "SendGrid"
                       });
            }
        }

        private SendGridMessage CreateSendGridMessage(MailContext mailContext, List<System.Net.Mail.Attachment> attachments = null)
        {
            var msg = new SendGridMessage
            {
                From = mailContext.From,
                Subject = mailContext.Subject,
                HtmlContent = mailContext.HtmlContent
            };

            //Add email Ids in To
            var toCount = mailContext.To;
            if (!toCount.IsNullOrEmpty())
            {
                foreach (var to in toCount)
                {
                    msg.AddTo(new EmailAddress(to));
                    msg.SetReplyTo(new EmailAddress(to));
                }
            }

            //Add email Ids in CC
            var ccCount = mailContext.CC;
            if (!ccCount.IsNullOrEmpty())
            {
                foreach (var cc in ccCount)
                {
                    msg.AddCc(new EmailAddress(cc));
                }
            }

            //Add email Ids in BCC
            var bccCount = mailContext.BCC;
            if (!bccCount.IsNullOrEmpty())
            {
                foreach (var bcc in bccCount)
                {
                    msg.AddBcc(new EmailAddress(bcc));
                }
            }

            if (attachments != null && attachments.Count > 0)
            {
                foreach (var attachment in attachments)
                {
                    msg.AddAttachment(GetSendGridAttachment(attachment));
                }
            }

            return msg;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="attachment"></param>
        /// <returns></returns>
        public Attachment GetSendGridAttachment(System.Net.Mail.Attachment attachment)
        {
            using (var stream = new MemoryStream())
            {
                try
                {
                    attachment.ContentStream.CopyTo(stream);
                    return new Attachment
                    {
                        Disposition = "attachment",
                        Type = attachment.ContentType.MediaType,
                        Filename = attachment.Name,
                        ContentId = attachment.ContentId,
                        Content = Convert.ToBase64String(stream.ToArray())
                    };
                }
                catch
                {
                    //Ignore
                }
                finally
                {
                    stream?.Close();
                }
                return null;
            }
        }

        #endregion Private Methods
    }
}