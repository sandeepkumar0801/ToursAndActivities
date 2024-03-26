using Isango.Entities.Activities;
using Isango.Entities.Affiliate;
using Isango.Entities.Mailer;
using Isango.Entities.Mailer.Voucher;
using Isango.Mailer.Constants;
using Isango.Mailer.Contract;
using Isango.Mailer.ServiceContracts;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util;

namespace Isango.Mailer
{
    public class Mailer : IMailer
    {
        private readonly IMailRuleEngineService _mailRuleEngineService;
        private readonly IMailGeneratorService _mailGeneratorService;
        private readonly IMailDeliveryService _mailDeliveryService;
        private readonly string _storageAccount;

        public Mailer(IMailRuleEngineService mailRuleEngineService, IMailGeneratorService mailGeneratorService,
            IMailDeliveryService mailDeliveryService)
        {
            _mailRuleEngineService = mailRuleEngineService;
            _mailGeneratorService = mailGeneratorService;
            _mailDeliveryService = mailDeliveryService;
            try
            {
                _storageAccount = ConfigurationManagerHelper.GetValuefromConfig("AzureWebJobsStorage")?.Split(';')?[1]
                    .Replace("AccountName=", "");
            }
            catch
            {
                _storageAccount = string.Empty;
            }
        }

        /// <summary>
        /// Send Mail to Customer
        /// </summary>
        /// <param name="templateContext"></param>
        public void SendMail(TemplateContext templateContext, List<System.Net.Mail.Attachment> attachments = null, bool? isAlternativePayment = false, List<Activity> crossSellData = null, bool? isReceive = false, bool? isCancel = false, bool? isORtoConfirm = false)
        {
            //Generate mail body
            var mailBody = _mailGeneratorService.GenerateMailBody(templateContext, isAlternativePayment, crossSellData, isReceive, isCancel, isORtoConfirm);

            //Get and set mail headers
            MailBooking bookingData = null;
            if (templateContext.Data.ContainsKey("#BookingData#")) { bookingData = (MailBooking)templateContext.Data["#BookingData#"]; }
            var headers = _mailRuleEngineService.GetMailHeaders(templateContext);
            var mailHeaders = SetMailHeaders(headers, bookingData);

            //Send Mail
            foreach (var header in mailHeaders)
            {
                var mailContext = new MailContext
                {
                    HtmlContent = mailBody,
                    Subject = header?.Subject,
                    To = header?.To,
                    CC = header?.CC,
                    From = header?.From,
                };
                if (header?.BCC != null && header.BCC.Length > 0)
                {
                    var newBCCAddress = FindUniqueBCCAddress(header);
                    if (newBCCAddress != null && newBCCAddress.Length > 0)
                        mailContext.BCC = newBCCAddress.ToArray();
                }

                _mailDeliveryService.SendMail(mailContext, attachments);
            }
        }

        public void SendPDFErrorMail(string bookingRefNo)
        {
            var mailBody = $"There was an error Generating PDF for Booking Reference Number - {bookingRefNo}";
            var mailHeaders = new MailHeader
            {
                From = "support@isango.com",
                Subject = $"Error in PDF Generation - {bookingRefNo}",
                To = "hara.prasad@isango.com".Split(','),
                CC = "amalik@isango.com".Split(',')
            };
            var mailContext = new MailContext
            {
                HtmlContent = mailBody,
                Subject = mailHeaders.Subject,
                To = mailHeaders.To,
                CC = mailHeaders.CC,
                From = new EmailAddress(mailHeaders.From)
            };
            _mailDeliveryService.SendMail(mailContext);
        }

        public void SendAdyenWebhookErrorMail(string bookingRefNo, string status, string pspReference, string reason)
        {
            var mailBody = $"There was an error in Webhook for Booking Reference Number - {bookingRefNo} - status = {status} - pspReference =  {pspReference} - Reason = {reason}";
            var mailHeaders = new MailHeader
            {
                From = "support@isango.com",
                Subject = $"Error in Webhook - {bookingRefNo}",
                To = "hara.prasad@isango.com".Split(','),
                CC = "amalik@isango.com,dhananjay.dubey@isango.com,tabassum.karigar@isango.com,finance@isango.com,bgera@isango.com,shubham.khatri@isango.com".Split(',')
            };
            var mailContext = new MailContext
            {
                HtmlContent = mailBody,
                Subject = mailHeaders.Subject,
                To = mailHeaders.To,
                CC = mailHeaders.CC,
                From = new EmailAddress(mailHeaders.From)
            };
            _mailDeliveryService.SendMail(mailContext);
        }

        /// <summary>
        /// Send Supplier Mail
        /// </summary>
        /// <param name="templateContext"></param>
        /// <param name="bookingDataOthers"></param>
        /// <param name="serviceId"></param>
        public void SendSupplierMail(TemplateContext templateContext, BookingDataOthers bookingDataOthers, int serviceId)
        {
            //Generate mail body
            var mailBody = _mailGeneratorService.GenerateSupplierMailBody(bookingDataOthers, serviceId);

            //Get Supplier Email
            var supplierEmail = GetSupplierEmail(bookingDataOthers, serviceId);
            if (!String.IsNullOrEmpty(supplierEmail))
            {
                //Get and set mail headers
                MailBooking bookingData = null;
                if (templateContext.Data.ContainsKey("#BookingData#")) { bookingData = (MailBooking)templateContext.Data["#BookingData#"]; }
                var headers = _mailRuleEngineService.GetMailHeaders(templateContext);
                var mailHeaders = SetMailHeaders(headers, bookingData);

                var code = bookingDataOthers.BookedProductDetailList.FirstOrDefault()?.SupplierOptionCode.Trim();
                code = code != string.Empty ? $", code: {code}" : string.Empty;
                var subject = $"{bookingData?.ReferenceNumber.Trim()}{code}, date: {bookingDataOthers.BookedProductDetailList.FirstOrDefault()?.FromDate.ToString(Constant.DateFormat)}";

                //Send Mail
                foreach (var header in mailHeaders)
                {
                    var mailContext = new MailContext
                    {
                        HtmlContent = mailBody,
                        Subject = header?.Subject.Replace(Constant.BookingRefId, subject),
                        To = supplierEmail.Split(','),
                        CC = header?.CC,
                        From = header?.From
                    };
                    if (header?.BCC != null && header.BCC.Length > 0)
                    {
                        var newBCCAddress = FindUniqueBCCAddress(header);
                        if (newBCCAddress != null && newBCCAddress.Length > 0)
                            mailContext.BCC = newBCCAddress.ToArray();
                    }

                    _mailDeliveryService.SendMail(mailContext);
                }
            }
        }

        /// <summary>
        /// Send Cancellation Mail To Supplier
        /// </summary>
        /// <param name="templateContext"></param>
        /// <param name="bookingDataOthers"></param>
        /// <param name="bookedProductDetail"></param>
        public void SendCancellationMailToSupplier(TemplateContext templateContext, BookingDataOthers bookingDataOthers, OthersBookedProductDetail bookedProductDetail)
        {
            var mailBody = _mailGeneratorService.CreateSupplierCancellationEmailContent(bookingDataOthers, Convert.ToInt32(bookedProductDetail.ServiceId));

            //Get Supplier Email
            var supplierEmail = GetSupplierEmail(bookingDataOthers, Convert.ToInt32(bookedProductDetail.ServiceId));

            //Get and set mail headers
            MailBooking bookingData = null;
            if (templateContext.Data.ContainsKey("#BookingData#")) { bookingData = (MailBooking)templateContext.Data["#BookingData#"]; }
            var headers = _mailRuleEngineService.GetMailHeaders(templateContext);
            var mailHeaders = SetMailHeaders(headers, bookingData);
            var subject = $"{Constant.SupplierCancellationSubject}{bookingDataOthers.BookingRefNo.Trim()})";

            //Send Mail
            foreach (var header in mailHeaders)
            {
                var mailContext = new MailContext
                {
                    HtmlContent = mailBody,
                    Subject = subject,
                    To = header?.To.Select(x => x = supplierEmail).ToArray(),
                    CC = header?.CC,
                    From = header?.From
                };
                if (header?.BCC != null && header.BCC.Length > 0)
                {
                    var newBCCAddress = FindUniqueBCCAddress(header);
                    if (newBCCAddress != null && newBCCAddress.Length > 0)
                        mailContext.BCC = newBCCAddress.ToArray();
                }

                _mailDeliveryService.SendMail(mailContext);
            }
        }

        /// <summary>
        /// Send Alert Mail
        /// </summary>
        /// <param name="affiliate"></param>
        public void SendAlertMail(Affiliate affiliate)
        {
            //Generate mail body
            var mailBodyContents = _mailGeneratorService.GenerateAlertMailBody(affiliate);
            var isangoFinanceEmailId = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.IsangoFinanceEmailId)?.Split(',');

            var mailContext = new MailContext
            {
                Subject = Constant.AlertMailSubject,
                HtmlContent = mailBodyContents,
                From = new EmailAddress(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.CustomerSupportFrom), ConfigurationManagerHelper.GetValuefromAppSettings(Constant.AlternativeName)),
                To = new[] { affiliate.AffiliateCompanyDetail?.CompanyEmail },
                CC = isangoFinanceEmailId
            };

            _mailDeliveryService.SendMail(mailContext);
        }

        /// <summary>
        /// Send Failure Mail
        /// </summary>
        /// <param name="failureMailContextList"></param>
        public void SendFailureMail(List<FailureMailContext> failureMailContextList)
        {
            //Generate mail body
            /*
            var mailBodyContents = _mailGeneratorService.GenerateFailureMailBody(failureMailContextList);

            var mailContext = new MailContext
            {
                Subject = Constant.FailureMailSubject,
                HtmlContent = mailBodyContents,
                From = new EmailAddress(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.CustomerSupportFrom), ConfigurationManagerHelper.GetValuefromAppSettings(Constant.AlternativeName)),
                To = new[] { ConfigurationManagerHelper.GetValuefromAppSettings(Constant.CustomerSupportTo) },
            };

            _mailDeliveryService.SendMail(mailContext);
            */
            SendFailureMailAsync(failureMailContextList);
        }

        /// <summary>
        /// Send Cancel Booking Mail
        /// </summary>
        /// <param name="cancellationMailTexts"></param>
        public void SendCancelBookingMail(List<CancellationMailText> cancellationMailTexts)
        {
            SendCancelBookingMailAsync(cancellationMailTexts);
        }

        /// <summary>
        /// Send Remaining Discount Amount Mail
        /// </summary>
        /// <param name="remainingRows"></param>
        public void SendRemainingDiscountAmountMail(string remainingRows)
        {
            var remainingDiscountRows = _mailGeneratorService.GenerateDiscountMailBody(remainingRows);

            var customerSupport = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.CustomerSupportTo)?.Split(',');

            var isangoFinanceEmailId = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.IsangoFinanceEmailId)?.Split(',');

            var mailContext = new MailContext
            {
                Subject = Constant.RemainingDiscountAmountMailSubject,
                HtmlContent = remainingDiscountRows,
                From = new EmailAddress(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.CustomerSupportFrom), ConfigurationManagerHelper.GetValuefromAppSettings(Constant.AlternativeName)), // need to check
                To = customerSupport,
                CC = isangoFinanceEmailId
            };

            _mailDeliveryService.SendMail(mailContext);
        }

        /// <summary>
        /// Send Error Mail
        /// </summary>
        /// <param name="templateContext"></param>
        public void SendErrorMail(List<Tuple<string, string>> data)
        {
            var htmlContent = new StringBuilder();
            htmlContent.Append("<table style='width:100%;border:1px solid #000;border-collapse: collapse'>");
            htmlContent.Append("<tr style='border:1px solid #000;border-collapse: collapse'>");
            htmlContent.Append("<td style='text-align:left;width:15%;border:1px solid #000;border-collapse: collapse;font-weight: bold;'> Method Name </td>");
            htmlContent.Append("<td style='text-align:left;border:1px solid #000;border-collapse: collapse;font-weight: bold;'> Success/Error </ td >");
            htmlContent.Append("</tr>");

            foreach (var item in data)
            {
                htmlContent.Append("<tr>");
                htmlContent.Append("<td style = 'width:15%;text-align:left;border:1px solid #000;border-collapse: collapse;'>" + item.Item1 + "</td>");
                htmlContent.Append("<td style = 'text-align:left;border:1px solid #000;border-collapse: collapse;'> " + item.Item2 + " </ td >");
                htmlContent.Append("</tr>");
            }

            htmlContent.Append("</table>");

            var mailContext = new MailContext
            {
                Subject = "BumbleBee Cosmos Collection:(" + ConfigurationManagerHelper.GetValuefromAppSettings(Constant.ErrorMailEnvironment) + ") " + data[0].Item1,
                HtmlContent = Convert.ToString(htmlContent),
                From = new EmailAddress(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.CustomerSupportFrom), ConfigurationManagerHelper.GetValuefromAppSettings(Constant.AlternativeName)),
                To = new[] { ConfigurationManagerHelper.GetValuefromAppSettings(Constant.ErrorSendEmail) },
                CC = new[] { ConfigurationManagerHelper.GetValuefromAppSettings(Constant.ErrorSendEmailCC) },
                BCC = new[] { ConfigurationManagerHelper.GetValuefromAppSettings(Constant.ErrorSendEmailBCC) },
            };

            _mailDeliveryService.SendMail(mailContext, null);
        }

        public void SendTiqetsBookingTicket(string ticketPdfPath, string bookingRefNo, string customerEmail)
        {
            //Generate mail body
            var mailBodyContents = _mailGeneratorService.GenerateCustomerTiqetsTicketMail(ticketPdfPath, bookingRefNo);

            var isangoFinanceEmailId = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.IsangoFinanceEmailId)?.Split(',');

            var mailContext = new MailContext
            {
                Subject = Constant.TiqetsCustomerMail,
                HtmlContent = mailBodyContents,
                From = new EmailAddress(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.CustomerSupportFrom), ConfigurationManagerHelper.GetValuefromAppSettings(Constant.AlternativeName)),
                To = new[] { customerEmail },
                CC = isangoFinanceEmailId
            };

            _mailDeliveryService.SendMail(mailContext);
        }

        public void SendGetTicketFailureMail(string bookingRefNo)
        {
            var customerSupport = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.CustomerSupportTo)?.Split(',');

            var isangoFinanceEmailId = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.IsangoFinanceEmailId)?.Split(',');

            var getTicketFailure = _mailGeneratorService.GenerateGetTicketFailureMail(bookingRefNo);
            var mailContext = new MailContext
            {
                Subject = Constant.RemainingDiscountAmountMailSubject,
                HtmlContent = getTicketFailure,
                From = new EmailAddress(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.CustomerSupportFrom), ConfigurationManagerHelper.GetValuefromAppSettings(Constant.AlternativeName)), // need to check
                To = customerSupport,
                CC = isangoFinanceEmailId
            };

            _mailDeliveryService.SendMail(mailContext);
        }

        public void SendVoucherDownloadFailureMail(string bookingRefNo, string apiType = null, string request = null, string response = null)
        {
            string[] customerSupport = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.CustomerSupportTo)?.Split(',');

            var from = new EmailAddress(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.CustomerSupportFrom), ConfigurationManagerHelper.GetValuefromAppSettings(Constant.AlternativeName));

            var subject = string.Format(Constant.ApiBookingVoucherDownloadFailed, bookingRefNo);
            if (!string.IsNullOrWhiteSpace(apiType))
            {
                subject = $"{subject} || ApiType {apiType}";
            }

            var emailBody = $"<p>Dear Sir / Ma’am,</p><p>This is in reference to the&nbsp; booking (No. {bookingRefNo}) with isango. Regrettably, it won’t be possible for us to confirm the booking for&nbsp; the mentioned date.<br/> The downloading of voucher failed after trying at least 3 times. Contact Dev/Qa teams so that they can verify adapter logs for voucher request and response.</p><p>###request### ###response### Please connect with the Tiquet Customer Suppport team ticket_agent@tiqets.com for voucher of this booking and send it to customer. If voucher can't be downloaded then cancel the booking and initiate refund to customer.</p><p>Kind Regards<br/>isango Technology Team</p>";

            if (!string.IsNullOrWhiteSpace(request))
            {
                emailBody = emailBody.Replace("###request###", $"<p>Request:<br/>{request}</p>");
            }
            else
            {
                emailBody = emailBody.Replace("###request###", string.Empty);
            }

            if (!string.IsNullOrWhiteSpace(response))
            {
                emailBody = emailBody.Replace("###response###", $"<p>Response:<br/>{response}</p>");
            }
            else
            {
                emailBody = emailBody.Replace("###response###", string.Empty);
            }

            var mailContext = new MailContext
            {
                Subject = subject,
                HtmlContent = emailBody,
                From = from, // need to check
                To = customerSupport,
                //CC = new[] { ConfigurationManagerHelper.GetValuefromAppSettings(Constant.IsangoFinanceEmailId) }
            };

            _mailDeliveryService.SendMail(mailContext);
        }


        public void SendCancellationFailureMail(string bookingRefNo, string apiType = null, string request = null, string response = null)
        {
            string[] customerSupport = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.CustomerSupportTo)?.Split(',');

            var from = new EmailAddress(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.CustomerSupportFrom), ConfigurationManagerHelper.GetValuefromAppSettings(Constant.AlternativeName));

            var subject = string.Format(Constant.ApiBookingCancellationFailed, bookingRefNo);
            if (!string.IsNullOrWhiteSpace(apiType))
            {
                subject = $"{subject} || ApiType {apiType}";
            }

            var emailBody = $"<p>Dear Sir / Ma’am,</p><p>This is in reference to the&nbsp; booking (No. {bookingRefNo}) with isango. Regrettably, it won’t be possible for us to cancel the booking for&nbsp; the mentioned date.<br/>  after trying at least 3 times. Contact Dev/Qa teams so that they can verify adapter logs for cancel request and response.</p><p>###request### ###response### Please connect with the PrioHub Customer Suppport team support@prioticket.com, support@priohub.com for this booking.</p><p>Kind Regards<br/>isango Technology Team</p>";

            if (!string.IsNullOrWhiteSpace(request))
            {
                emailBody = emailBody.Replace("###request###", $"<p>Request:<br/>{request}</p>");
            }
            else
            {
                emailBody = emailBody.Replace("###request###", string.Empty);
            }

            if (!string.IsNullOrWhiteSpace(response))
            {
                emailBody = emailBody.Replace("###response###", $"<p>Response:<br/>{response}</p>");
            }
            else
            {
                emailBody = emailBody.Replace("###response###", string.Empty);
            }

            var mailContext = new MailContext
            {
                Subject = subject,
                HtmlContent = emailBody,
                From = from, // need to check
                To = customerSupport,
            };

            _mailDeliveryService.SendMail(mailContext);
        }

        public void SendCancellationSuccessMail(string bookingRefNo, string apiType = null, string request = null, string response = null)
        {
            string[] customerSupport = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.CustomerSupportTo)?.Split(',');

            var from = new EmailAddress(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.CustomerSupportFrom), ConfigurationManagerHelper.GetValuefromAppSettings(Constant.AlternativeName));

            var subject = string.Format(Constant.ApiBookingCancellationSuccess, bookingRefNo);
            if (!string.IsNullOrWhiteSpace(apiType))
            {
                subject = $"{subject} || ApiType {apiType}";
            }

            var emailBody = $"<p>Dear Sir / Ma’am,</p><p>This is in reference to the&nbsp; booking (No. {bookingRefNo}) with isango.<p>Booking is cancelled for&nbsp; the mentioned date.<br/>.</p><p>###request### ###response###.</p><p>Kind Regards<br/>isango Technology Team</p>";

            if (!string.IsNullOrWhiteSpace(request))
            {
                emailBody = emailBody.Replace("###request###", $"<p>Request:<br/>{request}</p>");
            }
            else
            {
                emailBody = emailBody.Replace("###request###", string.Empty);
            }

            if (!string.IsNullOrWhiteSpace(response))
            {
                emailBody = emailBody.Replace("###response###", $"<p>Response:<br/>{response}</p>");
            }
            else
            {
                emailBody = emailBody.Replace("###response###", string.Empty);
            }

            var mailContext = new MailContext
            {
                Subject = subject,
                HtmlContent = emailBody,
                From = from, // need to check
                To = customerSupport,
            };

            _mailDeliveryService.SendMail(mailContext);
        }


        /// <summary>
        /// Send Remaining Discount Amount Mail
        /// </summary>
        /// <param name="remainingRows"></param>
        public void SendAdyenGenerateLinkMail(string customerEmail, string generatedLink,string lang,string tempBookingRefNumber)
        {
            var htmlData = _mailGeneratorService.GeneratePaymentLinkMailBody(generatedLink, lang, tempBookingRefNumber);
            var subject = "Payment Link";
            if (lang == "de")
            {
                subject = "Zahlungslink";
            }
            else if (lang == "es")
            {
                subject = "Enlace de pago";
            }
            var mailContext = new MailContext
            {
                Subject = subject,
                HtmlContent = htmlData,
                From = new EmailAddress(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.CustomerSupportFrom), ConfigurationManagerHelper.GetValuefromAppSettings(Constant.AlternativeName)), // need to check
                To = new string[]{customerEmail},
            };

            _mailDeliveryService.SendMail(mailContext);
        }

        public void SendAdyenReceivedLinkMail(string customerEmail, string price,string lang,string tempRef)
        {
            var htmlData = _mailGeneratorService.GeneratePaymentReceivedMailBody(price, lang, tempRef);
            var subject = "Received Payment";
            if (lang == "de")
            {
                subject = "Zahlung erhalten";
            }
            else if (lang == "es")
            {
                subject = "Pago recibido";
            }
            var mailContext = new MailContext
            {
                Subject = subject,
                HtmlContent = htmlData,
                From = new EmailAddress(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.CustomerSupportFrom), ConfigurationManagerHelper.GetValuefromAppSettings(Constant.AlternativeName)), // need to check
                To = new string[] { customerEmail },
                BCC = new string[] { ConfigurationManagerHelper.GetValuefromAppSettings(Constant.MailFrom) }
            };

            _mailDeliveryService.SendMail(mailContext);
        }


        #region Private Methods

        /// <summary>
        /// Set Mail Headers
        /// </summary>
        /// <param name="contexts"></param>
        /// <param name="bookingData"></param>
        /// <returns></returns>
        private List<MailContext> SetMailHeaders(List<MailHeader> contexts, MailBooking bookingData)
        {
            var mailContextList = new List<MailContext>();
            if (contexts != null)
            {
                foreach (MailHeader mailHeader in contexts)
                {
                    var mailContext = new MailContext
                    {
                        From = new EmailAddress(mailHeader.From.Replace(Constant.Customer, bookingData?.CustomerEmail), ConfigurationManagerHelper.GetValuefromAppSettings(Constant.AlternativeName)),
                        Subject = mailHeader.Subject.Replace(Constant.BookingRefId, bookingData?.ReferenceNumber),
                        To = mailHeader.To.Select(x => x.Replace(Constant.Customer, bookingData?.CustomerEmail)).ToArray(),
                        CC = mailHeader.CC?.Select(x => x.Replace(Constant.BookingArchive,
                            ConfigurationManagerHelper.GetValuefromAppSettings(Constant.MailCc))).ToArray(),
                        BCC = mailHeader.BCC?.Select(x => x.Replace(Constant.BookingArchive,
                            ConfigurationManagerHelper.GetValuefromAppSettings(Constant.MailBcc))).ToArray()
                    };

                    mailContextList.Add(mailContext);
                }
            }

            return mailContextList;
        }

        /// <summary>
        /// Get Supplier Email
        /// </summary>
        /// <param name="bookingDataOthers"></param>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        private string GetSupplierEmail(BookingDataOthers bookingDataOthers, int serviceId)
        {
            var supplierEmail = String.Empty;
            var selectedProduct = bookingDataOthers.BookedProductDetailList.Find(x => Convert.ToInt32(x.ServiceId).Equals(serviceId));
            var localPartner = bookingDataOthers.SupplierDataList.Find(x => x.BookedOptionId == selectedProduct.BookedOptionId);

            if (localPartner != null)
            {
                supplierEmail = localPartner.SupportEmailId?.Trim();
            }

            return supplierEmail;
        }

        /// <summary>
        /// Find unique BCC Address
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        private static string[] FindUniqueBCCAddress(MailContext header)
        {
            var commonAddress = header?.CC?.Intersect(header.BCC).ToList();
            if (commonAddress == null)
                return header?.BCC;

            var uniqueBCC = (header?.BCC).Where(email => !commonAddress.Contains(email)).ToList();
            return uniqueBCC.Any() ? uniqueBCC.ToArray() : null;
        }

        private Task<object> SendFailureMailAsync(List<FailureMailContext> failureMailContextList)
        {
            //Generate mail body
            var mailBodyContents = _mailGeneratorService.GenerateFailureMailBody(failureMailContextList);
            string[] customerSupport = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.CustomerSupportTo)?.Split(',');
            var mailContext = new MailContext
            {
                Subject = $"{_storageAccount} || {Constant.FailureMailSubject}",
                HtmlContent = mailBodyContents,
                From = new EmailAddress(
                    ConfigurationManagerHelper.GetValuefromAppSettings(Constant.CustomerSupportFrom),
                    ConfigurationManagerHelper.GetValuefromAppSettings(Constant.AlternativeName)),
                To = customerSupport,
            };

            _mailDeliveryService.SendMail(mailContext);

            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// Private method to send cancel booking mail used by 'SendCancelBookingMail' method
        /// </summary>
        /// <param name="cancellationMailContextList"></param>
        /// <returns></returns>
        private Task<object> SendCancelBookingMailAsync(List<CancellationMailText> cancellationMailContextList)
        {
            //Generate mail body
            var mailBodyContents = _mailGeneratorService.GenerateCancelBookingMailBody(cancellationMailContextList);
            string[] customerSupport = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.BookingManagerCustomerSupportTo)?.Split(',');
            var mailContext = new MailContext
            {
                Subject = Constant.CancelBookingMailSubject,
                HtmlContent = mailBodyContents,
                From = new EmailAddress(
                    ConfigurationManagerHelper.GetValuefromAppSettings(Constant.CustomerSupportFrom),
                    ConfigurationManagerHelper.GetValuefromAppSettings(Constant.AlternativeName)),
                To = customerSupport,
            };

            _mailDeliveryService.SendMail(mailContext);

            return Task.FromResult<object>(null);
        }

        #endregion Private Methods
    }
}