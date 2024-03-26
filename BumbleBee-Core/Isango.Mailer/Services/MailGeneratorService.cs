using Isango.Entities.Activities;
using Isango.Entities.Affiliate;
using Isango.Entities.Enums;
using Isango.Entities.Mailer;
using Isango.Entities.Mailer.Voucher;
using Isango.Mailer.Constants;
using Isango.Mailer.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web;
using Util;

namespace Isango.Mailer.Services
{
    public class MailGeneratorService : IMailGeneratorService
    {
        private string _preCurrency = string.Empty;
        private string _postCurrency = string.Empty;
        private string _storageAccount = string.Empty;

        public MailGeneratorService()
        {
            try
            {
                _storageAccount = ConfigurationManagerHelper
                    .GetValuefromConfig("AzureWebJobsStorage")?
                    .Split(';')?[1]
                    .Replace("AccountName=", "");
            }
            catch (Exception)
            {
                _storageAccount = string.Empty;
            }
        }

        /// <summary>
        /// Generate Mail Body
        /// </summary>
        /// <param name="templateContext"></param>
        /// <param name="isAlternativePayment"></param>
        /// <returns></returns>
        public string GenerateMailBody(TemplateContext templateContext, bool? isAlternativePayment = false, List<Activity> crossSellData = null, bool? isReceive = false, bool? isCancel = false, bool? isORtoConfirm = false)
        {
            //Load mail template
            var templateText = LoadTemplate(Path.Combine((Constant.CustomerTemplateBasePath),(templateContext.TemplateName)));

            //To generate mail body.
            var mailContent = CreateMailContent(templateText, templateContext.Data, isAlternativePayment, crossSellData, isReceive, isCancel, isORtoConfirm, templateContext.CVPoints);

            return mailContent;
        }

        /// <summary>
        /// Generate supplier mail body
        /// </summary>
        /// <param name="bookingDataOthers"></param>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        public string GenerateSupplierMailBody(BookingDataOthers bookingDataOthers, int serviceId)
        {
            //To generate mail body.
            if (bookingDataOthers.BookedProductDetailList == null ||
                bookingDataOthers.BookedProductDetailList.Count <= 0) return string.Empty;
            var selectedProduct =
                bookingDataOthers.BookedProductDetailList.Find(x => Convert.ToInt32(x.ServiceId).Equals(serviceId));
            var localPartner =
                bookingDataOthers.SupplierDataList.Find(x => x.BookedOptionId == selectedProduct.BookedOptionId);

            if (selectedProduct.ServiceTypeId.Equals("3") || selectedProduct.ServiceTypeId.Equals("5")
            ) // Tours and Activity
            {
                var content = new StringBuilder();
                string status = null;

                var _supPreCurrency = string.Empty;
                var _supPostCurrency = string.Empty;
                List<string> postCurrencies = null;

                if (ConfigurationManager.AppSettings[Constant.PostCurrencyLanguages] != null)
                    postCurrencies = ConfigurationManager.AppSettings[Constant.PostCurrencyLanguages].Split(',').ToList();

                if (postCurrencies != null && postCurrencies.Contains(bookingDataOthers.LanguageCode.ToLowerInvariant()))
                    _supPostCurrency = $"  {selectedProduct.SupplierCurrency}";
                else
                    _supPreCurrency = $"  {selectedProduct.SupplierCurrency}";

                // Getting the template
                if ((selectedProduct.BookedOptionStatusId == "2") || (selectedProduct.BookedOptionStatusId == "4"))
                {
                    var templateText = LoadTemplate(Path.Combine(Constant.SupplierConfirmationTemplateName,Constant.MailToSupplierForConfirmation));
                    content.Append(templateText);
                    content.Replace("#LinkText#", Constant.LinkTextForConfirmedProduct);
                    content.Replace("#BookingStatus#", Constant.Confirmed);
                    status = "Confirmed";
                }
                else if ((selectedProduct.BookedOptionStatusId == "1") || (selectedProduct.BookedOptionStatusId == "5"))
                {
                    var templateText = LoadTemplate(Path.Combine(Constant.SupplierOnRequestTemplateName,Constant.MailToSupplierForOnRequest));
                    content.Append(templateText);
                    content.Replace("#LinkText#", Constant.LinkTextForOnRequestProduct);
                    content.Replace("#BookingStatus#", Constant.OnRequest);
                    status = "On Request";
                }

                // replacing the values
                content.Replace("#VoucherNo#", bookingDataOthers.BookingRefNo);
                content.Replace("#SupplierEmail#", localPartner != null ? localPartner.SupportEmailId : string.Empty);
                content.Replace("#StartDate#", selectedProduct.FromDate.ToString(Constant.DateFormat));
                content.Replace("#LeadPxEmail#", bookingDataOthers.VoucherEmail);
                content.Replace("#LeadPxPhone#", bookingDataOthers.PhoneNumber);

                var transferInfo = string.Empty;

                if (selectedProduct.ServiceTypeId.Equals("5"))
                {
                    transferInfo = GetTransferInfo(selectedProduct);
                }

                content.Replace("#TransferInfo#", transferInfo != string.Empty ? transferInfo : "");

                content.Replace("#BookingDate#",
                    Convert.ToDateTime(bookingDataOthers.BookingDate).ToString(Constant.DateFormat));
                content.Replace("#ProductName#", selectedProduct.TsProductName);
                content.Replace("#OptionName#", selectedProduct.SupplierOptionName);
                content.Replace("#ProductCode#", selectedProduct.SupplierOptionCode);
                content.Replace("#OptionCode#", selectedProduct.SupplierOptionCode);
                content.Replace("#BookingStatus#", selectedProduct.BookedOptionStatusName);
                content.Replace("#StartTime#", selectedProduct.Schedule);
                content.Replace("#LeadPxName#", selectedProduct.LeadPassengerName);
                content.Replace("#NoOfAdultPx#", selectedProduct.NoOfAdults);
                var childAgeSection = string.Empty;
                var customers =
                    bookingDataOthers.Customers.FindAll(x => x.BookedOptionId == selectedProduct.BookedOptionId);

                childAgeSection = childAgeSection.Equals(string.Empty) ? string.Empty : $" ({childAgeSection} )";
                content.Replace("#NoOfChildPx#", selectedProduct.ChildCount + childAgeSection);
                content.Replace("#ServiceID#", serviceId.ToString());
                content.Replace("#SupplierCurrency#", selectedProduct.SupplierCurrency);
                content.Replace("#BookingCurrency#", bookingDataOthers.CurrencySymbol);

                content.Replace("#Amount#", selectedProduct.GrosSellingAmount);

                content.Replace("#CostPrice#", selectedProduct.SupplierCost);

                //if ((!string.IsNullOrEmpty(selectedProduct.PickupLocation)) &&
                //    String.Compare(selectedProduct.PickupLocation, "N/A", StringComparison.OrdinalIgnoreCase) != 0)
                //    content.Replace("#PickUpLocation#",
                //        selectedProduct.PickupLocation.Contains(Constant.Tell) ? "" : selectedProduct.PickupLocation);
                //else
                //    content.Replace("#PickUpLocation#", "N/A");

                content.Replace("#PickUpLocation#", string.IsNullOrEmpty(selectedProduct.PickupLocation) ? (string.IsNullOrEmpty(selectedProduct.ServiceHotelPickup) ? "N/A" : selectedProduct.ServiceHotelPickup)
                                                            : selectedProduct.PickupLocation?.Trim().Replace("<br />", "\n").Replace("<br>", "\n").Replace("<br/>", "\n").Replace("<b>", "").Replace("</b>", "").Replace("<p>", "").Replace("</p>", ""));

                content.Replace("#SpecialRequest#",
                    selectedProduct.SpecialRequest.Contains(Constant.SpecialRequest)
                        ? ""
                        : selectedProduct.SpecialRequest);
                if (!selectedProduct.SpecialRequest.Contains(Constant.ContractComment)
                    && string.IsNullOrWhiteSpace(selectedProduct.ContractComment))
                {
                    content.Replace("##displayContractComment##", "none");
                    content.Replace("##ContractComment##", "N/A");
                }
                else
                {
                    content.Replace("##displayContractComment##", "block");
                    content.Replace("#ContractComment#", selectedProduct.ContractComment);
                }

                content.Replace("#ClickHereToConfirm#",
                    GetSupplierURL(status, bookingDataOthers.BookingRefNo, selectedProduct.BookedOptionId,
                        selectedProduct.SupplierId));

                // Adding Passenger Details
                var passengerRows = new StringBuilder();
                bool onlyLeadPassenger = false;
                foreach (var customer in customers)
                {
                    var isChild = Convert.ToBoolean(customer.CustomerType);
                    if (!((customer.FirstName != null && customer.LastName != null &&
                           customer.FirstName.Equals(Constant.AdultFirstName) &&
                           customer.LastName.Equals(Constant.AdultLastName)) ||
                          (customer.FirstName != null && customer.LastName != null &&
                           customer.FirstName.Equals(Constant.ChildFirstName) &&
                           customer.LastName.Equals(Constant.ChildLastName))))
                    {
                        var supplierCost = bookingDataOthers.BookingAgeGroupList?.Where(x => x.BookedOptionId == customer.BookedOptionId && x.PassengerType.Contains(customer.PassengerType))?.FirstOrDefault()?.PaxSupplierCostAmount ?? 0;

                        if (supplierCost == 0)
                        {
                            supplierCost = bookingDataOthers.BookingAgeGroupList?.Where(x => x.BookedOptionId == customer.BookedOptionId && x.AgeGroupDesc.Contains(customer.PassengerType))?.FirstOrDefault()?.PaxSupplierCostAmount ?? 0;
                        }

                        if (supplierCost == 0 && bookingDataOthers?.BookingAgeGroupList?.Count == 1)
                        {
                            supplierCost = bookingDataOthers.BookingAgeGroupList?.FirstOrDefault(x => x.BookedOptionId == customer.BookedOptionId)?.PaxSupplierCostAmount ?? 0;
                        }

                        //passengerRows.Append(
                        //    $"<tr bgcolor='#ffffdd'><td><price class='MsoNormal' style='TEXT-ALIGN: center' align='center'><font face='Arial' size='2'><span style='FONT-SIZE: 10pt; FONT-FAMILY: Arial'>{customer.FirstName}</span></font></price></td><td><price class='MsoNormal' style='TEXT-ALIGN: center' align='center'><font face='Arial' size='2'><span style='FONT-SIZE: 10pt; FONT-FAMILY: Arial'>{customer.LastName}</span></font></price></td><td><price class='MsoNormal' style='TEXT-ALIGN: center' align='center'><font face='Arial' size='2'><span style='FONT-SIZE: 10pt; FONT-FAMILY: Arial'>{(!isChild ? "Adult" : "Child")}</span></font></price></td><td><price class='MsoNormal' style='TEXT-ALIGN: center' align='center'><font face='Arial' size='2'><span style='FONT-SIZE: 10pt; FONT-FAMILY: Arial'>{(!isChild ? "" : customer.Age)}</span></font></price></td></tr>");
                        passengerRows.Append(
                            $"<tr><td></td><td style='font-size: 12px;color: #333;font-family: Arial, Helvetica, sans-serif;vertical-align: top;'>{customer.PassengerType}</td><td style='font-size: 12px;color: #333;font-family: Arial, Helvetica, sans-serif;vertical-align: top;'>{customer.FirstName} {customer.LastName}</td><td style='font-size: 12px;color: #333;font-family: Arial, Helvetica, sans-serif;vertical-align: top;'>{customer.Age}</td><td style='font-size: 12px;color: #333;font-family: Arial, Helvetica, sans-serif;vertical-align: top;'>{_supPreCurrency} {supplierCost.ToString("0.00")} {_supPostCurrency}</td>"
                            + "</tr>");
                    }
                    else
                    {
                        onlyLeadPassenger = true;
                    }
                }

                content.Replace("#Passengerdetails#",
                    onlyLeadPassenger ? Constant.LeadPassengerDetails : Constant.PassengerDetails);
                content.Replace("#PxDetailRow#", passengerRows.ToString());
                content.Replace("##TotalPax##", customers.Count().ToString());
                return content.ToString().Replace("???", "");
            }

            return string.Empty;
        }

        /// <summary>
        /// Create supplier cancellation email content
        /// </summary>
        /// <param name="bookingDataOthers"></param>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        public string CreateSupplierCancellationEmailContent(BookingDataOthers bookingDataOthers, int serviceId)
        {
            if (bookingDataOthers.BookedProductDetailList != null &&
                bookingDataOthers.BookedProductDetailList.Count > 0)
            {
                var selectedProduct =
                    bookingDataOthers.BookedProductDetailList.Find(x => Convert.ToInt32(x.ServiceId).Equals(serviceId));
                var localPartner =
                    bookingDataOthers.SupplierDataList.Find(x => x.BookedOptionId == selectedProduct.BookedOptionId);

                var _supPreCurrency = string.Empty;
                var _supPostCurrency = string.Empty;
                List<string> postCurrencies = null;

                if (ConfigurationManager.AppSettings[Constant.PostCurrencyLanguages] != null)
                    postCurrencies = ConfigurationManager.AppSettings[Constant.PostCurrencyLanguages].Split(',').ToList();

                if (postCurrencies != null && postCurrencies.Contains(bookingDataOthers.LanguageCode.ToLowerInvariant()))
                    _supPostCurrency = $"  {selectedProduct.SupplierCurrency}";
                else
                    _supPreCurrency = $"  {selectedProduct.SupplierCurrency}";

                var content = new StringBuilder();
                var templateText = LoadTemplate(Path.Combine(Constant.SupplierCancellationTemplateName,Constant.MailToSupplierForCancellation));
                content.Append(templateText);

                content.Replace("#EntertainedByWhom#", Constant.EntertainedByWhomText);
                content.Replace("#VoucherNo#", bookingDataOthers.BookingRefNo);
                content.Replace("#BookingDate#",
                    Convert.ToDateTime(bookingDataOthers.BookingDate).ToString(Constant.DateFormat));

                content.Replace("#ProductName#", selectedProduct.TsProductName);
                content.Replace("#OptionName#", selectedProduct.SupplierOptionName);
                content.Replace("#ProductCode#", selectedProduct.SupplierOptionCode);
                content.Replace("#OptionCode#", selectedProduct.SupplierOptionCode);
                content.Replace("#SupplierName#", localPartner.SupplierName);
                content.Replace("#BookingStatus#", selectedProduct.BookedOptionStatusName);
                content.Replace("#StartDate#", selectedProduct.FromDate.ToString(Constant.DateFormat));
                content.Replace("#StartTime#", selectedProduct.Schedule);
                content.Replace("#LeadPxName#", selectedProduct.LeadPassengerName);
                content.Replace("#NoOfAdultPx#", selectedProduct.NoOfAdults);

                //if ((!string.IsNullOrEmpty(selectedProduct.PickupLocation)) &&
                //    String.Compare(selectedProduct.PickupLocation, "N/A", StringComparison.OrdinalIgnoreCase) != 0)
                //    content.Replace("#PickUpLocation#",
                //        selectedProduct.PickupLocation.Contains(Constant.Tell) ? "" : selectedProduct.PickupLocation);
                //else
                //    content.Replace("#PickUpLocation#", "N/A");

                content.Replace("#PickUpLocation#", string.IsNullOrEmpty(selectedProduct.PickupLocation) ? (string.IsNullOrEmpty(selectedProduct.ServiceHotelPickup) ? "N/A" : selectedProduct.ServiceHotelPickup)
                                                            : selectedProduct.PickupLocation?.Trim().Replace("<br />", "\n").Replace("<br>", "\n").Replace("<br/>", "\n").Replace("<b>", "").Replace("</b>", "").Replace("<p>", "").Replace("</p>", ""));

                var childAgeSection = string.Empty;

                //Need to check this
                childAgeSection = string.IsNullOrEmpty(childAgeSection) ? string.Empty : $" ({childAgeSection} )";

                content.Replace("#NoOfChildPx#", selectedProduct.ChildCount + childAgeSection);
                content.Replace("#ServiceID#", serviceId.ToString());
                content.Replace("#Reason#", selectedProduct.Reason);
                content.Replace("#AlternativeDates#", selectedProduct.AltervativeDateTime);
                content.Replace("#AlternativeTours#", selectedProduct.AlternativeTours);
                content.Replace("#SupplierEmail#", localPartner.SupportEmailId);

                var customers =
                    bookingDataOthers.Customers.FindAll(x => x.BookedOptionId == selectedProduct.BookedOptionId);

                var passengerRows = new StringBuilder();
                bool onlyLeadPassenger = false;
                foreach (var customer in customers)
                {
                    var isChild = Convert.ToBoolean(customer.CustomerType);
                    if (!((customer.FirstName != null && customer.LastName != null &&
                           customer.FirstName.Equals(Constant.AdultFirstName) &&
                           customer.LastName.Equals(Constant.AdultLastName)) ||
                          (customer.FirstName != null && customer.LastName != null &&
                           customer.FirstName.Equals(Constant.ChildFirstName) &&
                           customer.LastName.Equals(Constant.ChildLastName))))
                    {
                        var supplierCost = bookingDataOthers.BookingAgeGroupList?.Where(x => x.BookedOptionId == customer.BookedOptionId && x.AgeGroupDesc.Contains(customer.PassengerType))?.FirstOrDefault()?.PaxSupplierCostAmount ?? 0;
                        //passengerRows.Append(
                        //    $"<tr bgcolor='#ffffdd'><td><price class='MsoNormal' style='TEXT-ALIGN: center' align='center'><font face='Arial' size='2'><span style='FONT-SIZE: 10pt; FONT-FAMILY: Arial'>{customer.FirstName}</span></font></price></td><td><price class='MsoNormal' style='TEXT-ALIGN: center' align='center'><font face='Arial' size='2'><span style='FONT-SIZE: 10pt; FONT-FAMILY: Arial'>{customer.LastName}</span></font></price></td><td><price class='MsoNormal' style='TEXT-ALIGN: center' align='center'><font face='Arial' size='2'><span style='FONT-SIZE: 10pt; FONT-FAMILY: Arial'>{(!isChild ? "Adult" : "Child")}</span></font></price></td><td><price class='MsoNormal' style='TEXT-ALIGN: center' align='center'><font face='Arial' size='2'><span style='FONT-SIZE: 10pt; FONT-FAMILY: Arial'>{(!isChild ? "" : customer.Age)}</span></font></price></td></tr>");
                        passengerRows.Append(
                            $"<tr><td style='font-size: 12px;color: #333;font-family: Arial, Helvetica, sans-serif;vertical-align: top;'>{customer.PassengerType}</td><td style='font-size: 12px;color: #333;font-family: Arial, Helvetica, sans-serif;vertical-align: top;'>{customer.FirstName} {customer.LastName}</td><td style='font-size: 12px;color: #333;font-family: Arial, Helvetica, sans-serif;vertical-align: top;'>{customer.Age}</td><td style='font-size: 12px;color: #333;font-family: Arial, Helvetica, sans-serif;vertical-align: top;'>{_supPreCurrency} {supplierCost.ToString("0.00")} {_supPostCurrency}</td>"
                            + "</tr>");
                    }
                    else
                    {
                        onlyLeadPassenger = true;
                    }
                }

                content.Replace("#Passengerdetails#",
                    onlyLeadPassenger ? Constant.LeadPassengerDetails : Constant.PassengerDetails);
                content.Replace("#PxDetailRow#", passengerRows.ToString());
                content.Replace("##TotalPax##", customers.Count().ToString());
                content.Replace("##CancellationFee##", bookingDataOthers.CancellationPricesList.Where(x => x.BookedOptionID.ToString() == selectedProduct.BookedOptionId)?.FirstOrDefault()?.CancellationAmount.ToString("0.00"));
                return content.ToString();
            }

            return string.Empty;
        }

        /// <summary>
        /// Generate Alert Mail Body
        /// </summary>
        /// <param name="affiliate"></param>
        /// <returns></returns>
        public string GenerateAlertMailBody(Affiliate affiliate)
        {
            var mailBodyContent =
                $"<b>Important:</b> Your account balance has dipped lower than the required threshold of GBP{affiliate.AffiliateCredit.ThresholdAmount}.</br>You will not be able to make new bookings until you deposit funds in our account. Request you to make a payment immediately, if you wish to continue making bookings on the website.<br></br> For any queries, please feel free to reach out to us at api@isango.com / finance@isango.com.</br></br>Thanks,</br>isango! Travel Experiences";

            return mailBodyContent;
        }

        /// <summary>
        /// Generate Failure Mail Body
        /// </summary>
        /// <param name="failureMailContextList"></param>
        /// <returns></returns>
        public string GenerateFailureMailBody(List<FailureMailContext> failureMailContextList)
        {
            //Load mail template
            var templateText = LoadTemplate(Path.Combine(Constant.FailureEmailTemplateName,Constant.FailureEmail));

            //To generate mail body.
            var mailContent = CreateFailureMailContent(templateText, failureMailContextList);

            return mailContent;
        }

        /// <summary>
        /// Generate Discount Mail Body
        /// </summary>
        /// <param name="remainingDiscountRows"></param>
        /// <returns></returns>
        public string GenerateDiscountMailBody(string remainingDiscountRows)
        {
            var templateText = LoadTemplate(Path.Combine(Constant.DiscountEmailTemplateName, Constant.ReminingDiscountEmail));
            var content = new StringBuilder();
            content.Append(templateText);

            content.Replace("##RemainingRows##", remainingDiscountRows);

            return content.ToString();
        }

        /// <summary>
        /// Generate Cancel Booking Mail Body
        /// </summary>
        /// <param name="cancellationMailContextList"></param>
        /// <returns></returns>
        public string GenerateCancelBookingMailBody(List<CancellationMailText> cancellationMailContextList)
        {
            //Load mail template for both success and failure case
            var templateText = LoadTemplate(Path.Combine(Constant.CancelBookingMailTemplateName,Constant.CancelBookingMail));

            //To generate mail body.
            var mailContent = CreateCancelBookingMailContent(templateText, cancellationMailContextList);

            return mailContent;
        }

        public string GenerateCustomerTiqetsTicketMail(string ticketPdfUrl, string bookingRefNo)
        {
            var mailBodyContent = $"<table border =\"1\" style=\"border-collapse: collapse;width:25%;\"><tr style=\"border-collapse: collapse;\"> <td style=\"border-collapse: collapse; background-color:lightgray;\"><b>Booking Ref No<b></td> <td style=\"border-collapse: collapse; background-color:lightgray;\"><b> URL <b></td></tr><tr style=\"border-collapse: collapse;\"> <td style=\"border-collapse: collapse;\"> {bookingRefNo}</td> <td style=\"border-collapse: collapse;\"> <a href=\"{ticketPdfUrl}\" target=\"_blank\">Ticket Pdf</a></td></tr></table>";

            return mailBodyContent;
        }

        public string GenerateGetTicketFailureMail(string bookingRefNo)
        {
            var mailBodyContent = $"Booking Tickets of Tiqets supplier for Booking Reference number - <b> {bookingRefNo}</b> are not yet generated.";
            return mailBodyContent;
        }

        /// <summary>
        /// Generate Link
        /// </summary>
        /// <param name="generatedLink"></param>
        /// <returns></returns>
        public string GeneratePaymentLinkMailBody(string generatedLink, string lang, string tempBookingRefNumber)
        {
            if (string.IsNullOrEmpty(lang))
            {
                lang = "en";
            }
            var templateText = LoadTemplate(Path.Combine(Constant.GenerateLinkTemplateName,Constant.PaymentLink , lang));
            var content = new StringBuilder();
            content.Append(templateText);
            content.Replace("##TempRef##", tempBookingRefNumber);
            content.Replace("##GeneratedLink##", generatedLink);
            //content.Replace("#showlogo#", "<td style='width: 120px; text-align: right; border-bottom: 1px solid #EEE;height: 60px;vertical-align: middle;'><a href= 'https://www.isango.com' ><img height='42' width='99' alt='isango!' src= '{ConfigurationManagerHelper.GetValuefromAppSettings(Constant.CDN)}/logos/isango-cs.png' /></a></td>");
            content.Replace("#showlogo#", ConfigurationManagerHelper.GetValuefromAppSettings(Constant.CDN) + "/logos/isango-cs.png");
            var thumbGifData = File.ReadAllBytes($"{Path.Combine(WebRootPath.GetWebRootPath())}Images\\thumb.gif");
            var thumbGifUri = @"data:image/png;base64," + Convert.ToBase64String(thumbGifData);
            content.Replace("##ThumbGif##", thumbGifUri);

            var FacebookData = File.ReadAllBytes($"{Path.Combine(WebRootPath.GetWebRootPath())}Images\\facebook.png");
            var FacebookUri = @"data:image/png;base64," + Convert.ToBase64String(FacebookData);
            content.Replace("##Facebook##", FacebookUri);

            var TwitterData = File.ReadAllBytes($"{Path.Combine(WebRootPath.GetWebRootPath())}Images\\twitter.png");
            var TwitterUri = @"data:image/png;base64," + Convert.ToBase64String(TwitterData);
            content.Replace("##Twitter##", TwitterUri);

            var PinterestData = File.ReadAllBytes($"{Path.Combine(WebRootPath.GetWebRootPath())}Images\\pinterest.png");
            var PinterestUri = @"data:image/png;base64," + Convert.ToBase64String(PinterestData);
            content.Replace("##Pinterest##", PinterestUri);

            var InstagramData = File.ReadAllBytes($"{Path.Combine(WebRootPath.GetWebRootPath())}Images\\instagram.png");
            var InstagramUri = @"data:image/png;base64," + Convert.ToBase64String(InstagramData);
            content.Replace("##Instagram##", InstagramUri);
            content.Replace("#SiteName#", "isango.com");
            content.Replace("##SupportEmail##", "support@isango.com");
            content.Replace("#SupportEmail#", "support@isango.com");
            return content.ToString().Replace("???", "");
        }

        public string GeneratePaymentReceivedMailBody(string price, string lang, string tempRef)
        {
            if (string.IsNullOrEmpty(lang))
            {
                lang = "en";
            }
            var templateText = LoadTemplate(Path.Combine(Constant.ReceivedLinkTemplateName ,Constant.ReceivedPayment ,lang));
            var content = new StringBuilder();

            content.Append(templateText);
            content.Replace("##TempRef##", tempRef);
            //content.Replace("##ReceivedPrice##", price);
            content.Replace("#showlogo#", ConfigurationManagerHelper.GetValuefromAppSettings(Constant.CDN) + "/logos/isango-cs.png");
            var thumbGifData = File.ReadAllBytes($"{Path.Combine(WebRootPath.GetWebRootPath())}Images\\thumb.gif");
            var thumbGifUri = @"data:image/png;base64," + Convert.ToBase64String(thumbGifData);
            content.Replace("##ThumbGif##", thumbGifUri);

            var FacebookData = File.ReadAllBytes($"{Path.Combine(WebRootPath.GetWebRootPath())}Images\\facebook.png");
            var FacebookUri = @"data:image/png;base64," + Convert.ToBase64String(FacebookData);
            content.Replace("##Facebook##", FacebookUri);

            var TwitterData = File.ReadAllBytes($"{Path.Combine(WebRootPath.GetWebRootPath())}Images\\twitter.png");
            var TwitterUri = @"data:image/png;base64," + Convert.ToBase64String(TwitterData);
            content.Replace("##Twitter##", TwitterUri);

            var PinterestData = File.ReadAllBytes($"{Path.Combine(WebRootPath.GetWebRootPath())}Images\\pinterest.png");
            var PinterestUri = @"data:image/png;base64," + Convert.ToBase64String(PinterestData);
            content.Replace("##Pinterest##", PinterestUri);

            var InstagramData = File.ReadAllBytes($"{Path.Combine(WebRootPath.GetWebRootPath())}Images\\instagram.png");
            var InstagramUri = @"data:image/png;base64," + Convert.ToBase64String(InstagramData);
            content.Replace("##Instagram##", InstagramUri);
            content.Replace("#SiteName#", "isango.com");
            content.Replace("##SupportEmail##", "support@isango.com");
            content.Replace("#SupportEmail#", "support@isango.com");
            return content.ToString().Replace("???", "");
        }

        #region private Methods

        /// <summary>
        /// Get supplier URL
        /// </summary>
        /// <param name="status"></param>
        /// <param name="bookingRefNo"></param>
        /// <param name="serviceOptionInServiceId"></param>
        /// <param name="supplierId"></param>
        /// <returns></returns>
        private string GetSupplierURL(string status, string bookingRefNo, string serviceOptionInServiceId,
            string supplierId)
        {
            var b2BBaseUrl = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.B2bBaseUrl);
            var reconfirmationPageUrl =
                ConfigurationManagerHelper.GetValuefromAppSettings(Constant.ReconfirmationPageUrl);

            var queryString =
                $"SupplierID={supplierId}&BookingRefID={bookingRefNo}&OptionID={serviceOptionInServiceId}&BookingStatus={status}";
            return $"{b2BBaseUrl}{reconfirmationPageUrl}{Encrypt(queryString)}";
        }

        /// <summary>
        /// Get transfer info
        /// </summary>
        /// <param name="selectedProduct"></param>
        /// <returns></returns>
        private string GetTransferInfo(OthersBookedProductDetail selectedProduct)
        {
            var updatedHTML = new StringBuilder();
            if (!string.IsNullOrEmpty(selectedProduct.ArrivalDate) && !selectedProduct.ArrivalAirport.Contains("1900"))
                updatedHTML.Append(
                    $"<table cellspacing='0' cellpadding='0' border='0' style='margin-top: 5px; border-collapse: separate ! important; border-spacing: 1px; font-size: 11px;'><tbody><tr><td style='padding: 5px; width: 115px; font-weight: bold; font-size: 11px;'>Arrival</td><td style='padding: 5px; font-size: 11px;'>{selectedProduct.ArrivalDate}</td></tr></tbody></table>");

            if (!string.IsNullOrEmpty(selectedProduct.ArrivalFlight))
                updatedHTML.Append(
                    $"<table cellspacing='0' cellpadding='0' border='0' style='margin-top: 5px; border-collapse: separate ! important; border-spacing: 1px; font-size: 11px;'><tbody><tr><td style='padding: 5px; width: 115px; font-weight: bold; font-size: 11px;'>Arrival Flight</td><td style='padding: 5px; font-size: 11px;'>{selectedProduct.ArrivalFlight}</td></tr></tbody></table>");

            if (!string.IsNullOrEmpty(selectedProduct.ArrivalAirport))
                updatedHTML.Append(
                    $"<table cellspacing='0' cellpadding='0' border='0' style='margin-top: 5px; border-collapse: separate ! important; border-spacing: 1px; font-size: 11px;'><tbody><tr><td style='padding: 5px; width: 115px; font-weight: bold; font-size: 11px;'>Arrival Airport</td><td style='padding: 5px; font-size: 11px;'>{selectedProduct.ArrivalAirport}</td></tr></tbody></table>");

            if (!string.IsNullOrEmpty(selectedProduct.DepartureDate) && !selectedProduct.DepartureDate.Contains("1900"))
                updatedHTML.Append(
                    $"<table cellspacing='0' cellpadding='0' border='0' style='margin-top: 5px; border-collapse: separate ! important; border-spacing: 1px; font-size: 11px;'><tbody><tr><td style='padding: 5px; width: 115px; font-weight: bold; font-size: 11px;'>Departure</td><td style='padding: 5px; font-size: 11px;'>{selectedProduct.DepartureDate}</td></tr></tbody></table>");

            if (!string.IsNullOrEmpty(selectedProduct.DepartureFlight))
                updatedHTML.Append(
                    $"<table cellspacing='0' cellpadding='0' border='0' style='margin-top: 5px; border-collapse: separate ! important; border-spacing: 1px; font-size: 11px;'><tbody><tr><td style='padding: 5px; width: 115px; font-weight: bold; font-size: 11px;'>Departure Flight</td><td style='padding: 5px; font-size: 11px;'>{selectedProduct.DepartureFlight}</td></tr></tbody></table>");

            if (!string.IsNullOrEmpty(selectedProduct.DepartureAirport))
                updatedHTML.Append(
                    $"<table cellspacing='0' cellpadding='0' border='0' style='margin-top: 5px; border-collapse: separate ! important; border-spacing: 1px; font-size: 11px;'><tbody><tr><td style='padding: 5px; width: 115px; font-weight: bold; font-size: 11px;'>Departure Airport</td><td style='padding: 5px; font-size: 11px;'>{selectedProduct.DepartureAirport}</td></tr></tbody></table>");

            return updatedHTML.ToString();
        }

        /// <summary>
        /// create mail content
        /// </summary>
        /// <param name="templateText"></param>
        /// <param name="data"></param>
        /// <param name="isAlternativePayment"></param>
        /// <returns></returns>
        private string CreateMailContent(string templateText, Dictionary<string, object> data,
            bool? isAlternativePayment = false, List<Activity> crossSellData = null, bool? isReceive = false, bool? isCancel = false, bool? isORtoConfirm = false, int CvPoints = 0)
        {

            try
            {
                //Create mail content by replacing placeholders with respective values.
                var mailBody = new StringBuilder(templateText);
                if (data.ContainsKey("#ProductData#") && data.ContainsKey("#BookingData#"))
                {
                    var products = (List<MailProduct>)data["#ProductData#"];
                    var anyNonReceiptProduct = products.Any(x => !x.IsReceipt);
                    var areProductsOtherThanVentrataInBooking = products.Any(prod => prod.APIType.Equals(APIType.Ventrata) == false);
                    var anyIsangoVoucherProducts = products.Any(x => !x.IsShowSupplierVoucher);// && areProductsOtherThanVentrataInBooking;

                    var bookingData = (MailBooking)data["#BookingData#"];
                    if (products.Count > 0 && bookingData != null)
                    {
                        var resourceManager = new ResourceManager(Constant.ResourceManagerBaseName,
                            Assembly.GetExecutingAssembly());
                        var cultureInfo = GetCultureInfo(bookingData.Language?.ToLowerInvariant());

                        if (anyIsangoVoucherProducts && anyNonReceiptProduct)
                        {
                            if (isAlternativePayment == false)
                            //if (bookingData.PaymentGateway != PaymentGatewayType.Alternative )
                            {
                                mailBody.Replace("#PrintMessage#",
                                    GeneratePrintMessageSection(products, resourceManager, cultureInfo));
                                mailBody.Replace("##Link##", bookingData.VoucherLink);
                            }
                            else
                            {
                                mailBody.Replace("#PrintMessage#",
                                    GeneratePrintMessageSectionForAlternative(products, resourceManager, cultureInfo));
                                mailBody.Replace("##Link##", string.Empty);
                            }
                        }
                        else
                        {
                            mailBody.Replace("##Link##", string.Empty);
                            mailBody.Replace("#PrintMessage#", string.Empty);
                        }

                        //string imagePath = $"{ConfigurationManagerHelper.GetValuefromAppSettings(Constant.WebAPIBaseUrl) + "/"}{Constant.IsangoLogoB2BUpdate}";
                        //var logo = bookingData.AffiliateName;

                        var DynamicMessage = string.Empty;
                        isCancel = products?.FirstOrDefault()?.Status == resourceManager.GetString("Cancelled", cultureInfo);
                        if ((isORtoConfirm ?? false) && products.Any(x => x.IsReceipt == true))
                        {
                            //code here for OR to Confirm
                            DynamicMessage = resourceManager.GetString("TwoStepConfirmed", cultureInfo);
                            DynamicMessage += resourceManager.GetString("ConfirmLine4", cultureInfo); // Happy travels and we hope you enjoy experiencing the world with us.
                            DynamicMessage = DynamicMessage.Replace("##BookingRefNo##", bookingData.ReferenceNumber);
                        }
                        else if (isORtoConfirm ?? false)
                        {
                            //code here for OR to Confirm
                            DynamicMessage = resourceManager.GetString("OrConfirmed", cultureInfo);
                            DynamicMessage += resourceManager.GetString("ConfirmLine4", cultureInfo); // Happy travels and we hope you enjoy experiencing the world with us.
                        }
                        else if (isReceive ?? false)
                        {
                            //code here for Amended Product
                            DynamicMessage = resourceManager.GetString("AmendedProduct", cultureInfo);
                        }
                        else if (isCancel ?? false)
                        {
                            //code here for cancelled product (needs discussion)
                            DynamicMessage = resourceManager.GetString("CancelledProductHtmlMsg", cultureInfo);
                            DynamicMessage = DynamicMessage.Replace("##BookingRefNo##", bookingData.ReferenceNumber);
                        }
                        else if (products.Any(x => x.Status.Equals(resourceManager.GetString("PendingConfirmation", cultureInfo))))
                        {
                            //code here for OR
                            //This is temporary changes, need to change as per IS-13507 in future
                            DynamicMessage = resourceManager.GetString("ORProduct", cultureInfo) + "<br /><br />";
                        }
                        else if (products.Any(x => x.Status.Equals(resourceManager.GetString("Confirmed", cultureInfo)) && x.IsReceipt == true))
                        {
                            //code here for 2step
                            //This is temporary changes, need to change as per IS-13507 in future
                            //DynamicMessage = resourceManager.GetString("ConfirmationTopMsg2", cultureInfo) + "<br />";
                            //DynamicMessage += resourceManager.GetString("ConfirmationTopMsg3", cultureInfo) + "<br /><br />";
                            //DynamicMessage += resourceManager.GetString("ConfirmationTopMsg5", cultureInfo);
                            DynamicMessage = resourceManager.GetString("ORProduct", cultureInfo) + "<br /><br />";
                        }
                        else
                        {
                            //code here for confirmed booking
                            DynamicMessage += resourceManager.GetString("ConfirmLine3", cultureInfo); //This is the acknowledgement of the booking made by you. You can download the booking voucher in a link below – please read through.
                            DynamicMessage += resourceManager.GetString("ConfirmLine4", cultureInfo); // Happy travels and we hope you enjoy experiencing the world with us.
                        }
                        var topMessage = new StringBuilder("");
                        // + "<tr><td class='tac' style='padding-top: 20px;padding-bottom: 20px;'>");
                        //if (isCancel == false)
                        //{
                        //    topMessage.Append(
                        //    $"{resourceManager.GetString("ConfirmLine1", cultureInfo).Replace("##WebsiteName##", bookingData.AffiliateName)}"
                        //    );
                        //}
                        //else
                        //{
                        //    topMessage.Append($"{resourceManager.GetString("CancelledProductHtmlMsgGreetings", cultureInfo)}");
                        //}
                        //topMessage.Append("</td></tr><tr>");

                        if (isORtoConfirm == false && isCancel == false && (products.Any(x => x.Status.Equals(resourceManager.GetString("Confirmed", cultureInfo)) && x.IsReceipt == true) || products.Any(x => x.Status.Equals(resourceManager.GetString("PendingConfirmation", cultureInfo)))))
                        {
                            // //code here for 2step 
                            topMessage.Append("<tr>"
                            + $"<td class='tac' style='color: #333333;font-size: 22px;border-bottom: 1px solid #70707033; height: 75px;'>{resourceManager.GetString("ThanksForYourBooking", cultureInfo)} <b style='font-weight:bold;'>##UserName##</b></td>"
                            + "</tr>");
                        }
                        else if (isCancel == false && !products.Any(x => x.Status.Equals(resourceManager.GetString("PendingConfirmation", cultureInfo))))
                        {
                            topMessage.Append("<tr>"
                            + $"<td class='tac' style='color: #333333;font-size: 22px;border-bottom: 1px solid #70707033; height: 75px;'>{resourceManager.GetString("ConfirmLine1", cultureInfo)} <b style='font-weight:bold;'>##UserName##</b></td>"
                            + "</tr>");
                        }
                        else if (isCancel ?? false)
                        {
                            topMessage.Append("<tr>"
                            + $"<td class='tac' style='color: #333333;font-size: 22px;border-bottom: 1px solid #70707033; height: 75px;'>{resourceManager.GetString("Dear", cultureInfo)} <b style='font-weight:bold;'>##UserName##</b></td>"
                            + "</tr>");
                        }

                        if (isCancel == false)
                        {
                            topMessage.Append($"<td style='padding: 20px 0px 5px;font-weight: bold;font-size: 18px;text-align: center;' >{resourceManager.GetString("ConfirmLine2", cultureInfo)}</td>");
                        }

                        topMessage.Append("</tr><tr>"
                         + "<td class='tac' style='padding: 10px 20px;font-size: 16px;border:1px solid #70707033;border-top: 0;'>"
                         + $"{DynamicMessage}"
                         + "</td></tr>");
                        //+ "<tr><td style='padding: 10px 0;' > Please note this is not your booking voucher and cannot be used to redeem a tour.</td></tr>"
                        //+ "<tr>"
                        //    + "<td class='tac' style='padding: 10px 0;'>"
                        //     + $"<b>{resourceManager.GetString("ConfirmLine4", cultureInfo)}"
                        //    + "</td></tr><tr>"
                        //+ "<td class='tac' style='padding: 10px 0;'>Happy travels and we hope you enjoy experiencing the world with us.</td>"
                        //+ "</tr>");

                        try
                        {
                            var thumbGifData = File.ReadAllBytes($"{Path.Combine(WebRootPath.GetWebRootPath())}Images\\thumb.gif");
                            var thumbGifUri = @"data:image/png;base64," + Convert.ToBase64String(thumbGifData);
                            mailBody.Replace("##ThumbGif##", thumbGifUri);

                            var FacebookData = File.ReadAllBytes($"{Path.Combine(WebRootPath.GetWebRootPath())}Images\\facebook.svg");
                            var FacebookUri = @"data:image/svg+xml;base64," + Convert.ToBase64String(FacebookData);
                            mailBody.Replace("##Facebook##", FacebookUri);

                            var TwitterData = File.ReadAllBytes($"{Path.Combine(WebRootPath.GetWebRootPath())}Images\\twitter.svg");
                            var TwitterUri = @"data:image/svg+xml;base64," + Convert.ToBase64String(TwitterData);
                            mailBody.Replace("##Twitter##", TwitterUri);

                            var PinterestData = File.ReadAllBytes($"{Path.Combine(WebRootPath.GetWebRootPath())}Images\\pintrest.svg");
                            var PinterestUri = @"data:image/svg+xml;base64," + Convert.ToBase64String(PinterestData);
                            mailBody.Replace("##Pinterest##", PinterestUri);

                            var YoutubeData = File.ReadAllBytes($"{Path.Combine(WebRootPath.GetWebRootPath())}Images\\youtube.svg");
                            var YoutubeUri = @"data:image/svg+xml;base64," + Convert.ToBase64String(YoutubeData);
                            mailBody.Replace("##Youtube##", YoutubeUri);

                            var InstagramData = File.ReadAllBytes($"{Path.Combine(WebRootPath.GetWebRootPath())}Images\\instagram.svg");
                            var InstagramUri = @"data:image/svg+xml;base64," + Convert.ToBase64String(InstagramData);
                            mailBody.Replace("##Instagram##", InstagramUri);
                        }
                        catch (Exception ex)
                        {
                            mailBody.Replace("##ThumbGif##", "");
                            mailBody.Replace("##Instagram##", "");
                            mailBody.Replace("##Twitter##", "");
                            mailBody.Replace("##Pinterest##", "");
                        }
                        mailBody.Replace("##TopMessage##", topMessage.ToString());
                        mailBody.Replace("##RefNo##", bookingData.ReferenceNumber);
                        mailBody.Replace("##UserName##", bookingData.CustomerName);
                        mailBody.Replace("#BookingData#", GenerateBookingSection(bookingData));
                        mailBody.Replace("##ProductData##", GenerateProductSection(products, resourceManager, cultureInfo, bookingData.VoucherLink));
                        mailBody.Replace("#ReceiptData#",
                            GenerateReceiptSection(products, bookingData, resourceManager, cultureInfo));
                        mailBody.Replace("##SupportEmail##", bookingData.CompanyEmail);
                        mailBody.Replace("#SupportEmail#", bookingData.CompanyEmail);
                        mailBody.Replace("#SiteName#", bookingData.AffiliateName);
                        mailBody.Replace("#SiteUrl#", bookingData.AffiliateURL);
                        mailBody.Replace("#cvInfoDisplay#", bookingData.AffiliateURL.Contains("vistara") ? "block" : "none !important;mso-hide:all");
                        mailBody.Replace("#cvHideClass#", bookingData.AffiliateURL.Contains("vistara") ? "" : "cv_hide hide");
                        mailBody.Replace("#TermsLink#", bookingData.TermsLink);
                        var isNotIsango = !string.IsNullOrEmpty(bookingData.LogoPath) && !bookingData.LogoPath.ToUpper().Contains("5BEEF089-3E4E-4F0F-9FBF-99BF1F350183") && !bookingData.LogoPath.ToUpper().Contains("7FAAF455-1C75-4FC1-88DC-0C08F79F1439") && !bookingData.LogoPath.ToUpper().Contains("B7951003-DA73-4AF9-A731-EF5064F6E343");
                        var isIsango = !string.IsNullOrEmpty(bookingData.LogoPath) && (bookingData.LogoPath.ToUpper().Contains("5BEEF089-3E4E-4F0F-9FBF-99BF1F350183") || bookingData.LogoPath.ToUpper().Contains("7FAAF455-1C75-4FC1-88DC-0C08F79F1439") || bookingData.LogoPath.ToUpper().Contains("B7951003-DA73-4AF9-A731-EF5064F6E343"));
                        var isBoatTours = !string.IsNullOrEmpty(bookingData.LogoPath) && (bookingData.LogoPath.ToUpper().Contains("F5DB7AA4-2740-4284-83D8-7CC66DDA6E33") || bookingData.LogoPath.ToUpper().Contains("9F9AE0DB-D4A1-43E3-AAA2-C821916F47E2"));
                        var isHotelbeds = !string.IsNullOrEmpty(bookingData.LogoPath) && (bookingData.LogoPath.ToUpper().Contains("BA3471FB-462F-4F93-BA70-5DC6B4FF1A1F"));
                        var isVistara = !string.IsNullOrEmpty(bookingData.LogoPath) && (bookingData.LogoPath.ToUpper().Contains("0969FEF8-12E5-4175-B46E-1E6C288BF996"));
                        var emailLogoHeight = isIsango ? "45" : isBoatTours || isHotelbeds ? "20" : isVistara ? "35" : "28";
                        var logoAlignment = "left";

                        if (bookingData.AffiliateGroupID == 7 || bookingData.AffiliateGroupID == 11
                            || bookingData.AffiliateGroupID == 12 || bookingData.AffiliateGroupID == 13)
                        {
                            if (isHotelbeds)
                            {
                                mailBody.Replace("#AffiliateNameLogo#", $"<td style='width:120px;text-align:left;height:60px;vertical-align:middle;'><a href='{bookingData.AffiliateURL}'><img width='auto' alt='' style='max-width:142px;height:{emailLogoHeight}px;' height='{emailLogoHeight}' src='{bookingData.LogoPath}' /></a></td>");
                                mailBody.Replace("#showlogo#", $"<td style='text-align:right; height: 60px;vertical-align: middle;'><a href= 'https://www.isango.com' ><img height='45' width='auto' alt='isango!' style='height: 45px;max-width:120px;float:right;' src= '{ConfigurationManagerHelper.GetValuefromAppSettings(Constant.CDN)}/logos/isango-cs.png' /></a></td>");
                            }
                            else
                            {
                                logoAlignment = isIsango ? "centter" : "left";
                                if (isIsango)
                                {
                                    mailBody.Replace("#AffiliateNameLogo#", $"<td style='text-align: {logoAlignment}; height: 60px;vertical-align: middle;'><a href= 'https://www.isango.com' ><img alt='isango!' height='45' style='height: 45px;max-width:120px;float:right;' src= '{ConfigurationManagerHelper.GetValuefromAppSettings(Constant.CDN)}/logos/isango-cs.png' /></a></td>");
                                    mailBody.Replace("#showlogo#", "");
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(bookingData.LogoPath) && bookingData.LogoPath.ToLower().Contains("58c11104-34e6-47ba-926d-e89e4242b962"))
                        {
                            mailBody.Replace("#AffiliateNameLogo#", $"<td style='width:100%;text-align:center;height:60px;vertical-align:middle;'><a href='{bookingData.AffiliateURL}'><img width='auto' alt='' style='max-width:130px;height:{emailLogoHeight}px;' height='{emailLogoHeight}' src='{bookingData.LogoPath}' /></a></td>");
                        }
                        else
                        {
                            logoAlignment = isIsango ? "center" : "left";
                            var logoMargin = isIsango ? "margin: auto" : "";
                            var classForLogo = isIsango ? "logoTd" : "";
                            mailBody.Replace("#AffiliateNameLogo#", $"<td class='{classForLogo}' style='width:120px;text-align:{logoAlignment};height:60px;vertical-align:middle;'><a href='{bookingData.AffiliateURL}'><img width='auto' alt='' style='{logoMargin};max-width:130px;height:{emailLogoHeight}px;' height='{emailLogoHeight}' src='{bookingData.LogoPath}' /></a></td>");
                        }
                        if (isNotIsango)
                        {
                            logoAlignment = "right";
                            mailBody.Replace("#showlogo#", $"<td style='text-align: {logoAlignment}; height: 60px;vertical-align: middle;'><a href= 'https://www.isango.com' ><img alt='isango!' height='45' style='height: 45px;max-width:120px;float:right;' src= '{ConfigurationManagerHelper.GetValuefromAppSettings(Constant.CDN)}/logos/isango-cs.png' /></a></td>");
                            //mailBody.Replace("#showlogo#", "block");
                        }
                        else
                        {
                            mailBody.Replace("#showlogo#", "");
                        }

                        var CV_Content = new StringBuilder("");
                        int Cancelledcount = products.Count(item => item.Status.ToLower() == "cancelled");
                        if (bookingData.AffiliateName.ToLowerInvariant().Contains("vistara") && products.Count != Cancelledcount)
                        {
                            CV_Content.Append("<tr>"
                            + "<td style='background-color: #f2f2f2;border: 1px solid #e7e7e7;overflow: hidden;padding: 8px;text-align: center;'>"
                                + "<table width='100%' cellpadding='0' cellspacing='0' border='0'>"
                                    + "<tr>"
                                        + "<td style='width: 5px;' width='5'>&nbsp;</td>"
                                        + "<td width='130' style='width: 130px;vertical-align: middle' valign='middle'>"
                                            + "<img src='https://marketing.isango.com/newsletters/emailers/booking/club_vistara.png' alt='CV' class='social' title='Vistara CV' width='120' style='display: block;' height='auto' />"
                                + "</td>"
                                + "<td style='font -size: 12px;line-height: 150%;text-align: left;' align='left'>"
                                            + "<span class='cv_txt'>You have earned total <b>#CvPoints# CV Points</b> <br /> CV Points will be awarded 10 days after the consumption of Tour.</span>"
                                        + "</td>"
                                    + "</tr>"
                                + "</table>"
                            + "</td>"
                        + "</tr>"
                        + "<tr><td height='20' style='height: 20px;font-size: 0px;line-height: 1px;'> &nbsp;</td></tr>");
                        }
                        var CV = Math.Floor((bookingData.TotalChargedAmount / 100) * CvPoints);
                        mailBody.Replace("#AffiliateName#", bookingData.AffiliateName);
                        mailBody.Replace("#PassengerEmail#", bookingData.CustomerEmail);
                        mailBody.Replace("#TermsandConditionLink#", bookingData.TermsAndConditionLink);
                        mailBody.Replace("##CV_Content##", CV_Content.ToString());
                        mailBody.Replace("#TotalAmount#", $"{_preCurrency} {bookingData.BookingAmount.ToString("0.00")} {_postCurrency}");
                        mailBody.Replace("#CvPoints#", $"{CV}");
                        mailBody.Replace("##AmountAfterConfirmation##", $"{_preCurrency} {bookingData.OrAmount.ToString("0.00")} {_postCurrency}");

                        if (bookingData.Multisave <= 0)
                        {
                            mailBody.Replace("#displayMultiSaveDiscount#", "none");
                        }

                        if (bookingData.Discount <= 0)
                        {
                            mailBody.Replace("#displayVoucherDiscount#", "none");
                        }

                        if (bookingData.OrAmount <= 0)
                        {
                            mailBody.Replace("##displayToBeChargedAmount##", "none");
                        }

                        var AfterDiscountAmount = bookingData.BookingAmount - Convert.ToDecimal(bookingData.Multisave + bookingData.Discount);

                        if (AfterDiscountAmount == bookingData.BookingAmount)
                        {
                            mailBody.Replace("#displayAmountAfterDiscount#", "none");
                        }
                        mailBody.Replace("##AmountAfterDiscount##", $"{_preCurrency} {AfterDiscountAmount.ToString("0.00")} {_postCurrency}");
                        mailBody.Replace("##MultisaveDiscount##", $"{_preCurrency} {bookingData.Multisave.ToString("0.00")} {_postCurrency}");
                        mailBody.Replace("##VoucherDiscount##", $"{_preCurrency} {bookingData.Discount.ToString("0.00")} {_postCurrency}");
                        //mailBody.Replace("##GoogleTripDiscount##", "GOOGLE TRIP DISCOUNT?");
                        mailBody.Replace("##TotalChargedAmount##", $"{_preCurrency} {bookingData.TotalChargedAmount.ToString("0.00")} {_postCurrency}");

                        if (bookingData.AffiliateGroupID == 11 || bookingData.AffiliateGroupID == 12 || bookingData.AffiliateGroupID == 13)
                        {
                            mailBody.Replace("##MyBookingsWebsite##", $"{bookingData.AffiliateURL}/Agent/Bookings");
                        }
                        else
                        {
                            mailBody.Replace("##MyBookingsWebsite##", $"{bookingData.AffiliateURL}/my");
                        }

                        mailBody.Replace("##BtnMyBooking##", $"<img src='{resourceManager.GetString("BtnMy", cultureInfo)}' alt='' width='180' style='margin: auto;' height='40' />");
                        mailBody.Replace("##invoicelink##", $"{ConfigurationManagerHelper.GetValuefromAppSettings(Constant.WebAPIBaseUrl) + "/"}api/voucher/invoice/{bookingData.ReferenceNumber}");
                        mailBody.Replace("##BtnDownloadInvoice##", $"<img src='{resourceManager.GetString("BtnInvoice", cultureInfo)}' alt='' width='180' height='40' style='margin: auto;' />");
                        mailBody.Replace("##BaseUrl##", $"{ConfigurationManagerHelper.GetValuefromAppSettings(Constant.WebAPIBaseUrl)}");
                        //mailBody.Replace("###logoimg###", boo)
                        if (data.ContainsKey("#SupportNumbers#"))
                            mailBody.Replace("#SupportNumbers#", data["#SupportNumbers#"].ToString());
                        //if (bookingData.PaymentGateway == PaymentGatewayType.Alternative)

                        // ReSharper disable once AssignNullToNotNullAttribute
                        mailBody.Replace("##HeadingConditionalAlt##",
                            isAlternativePayment == true
                                ? resourceManager.GetString("HeadingConditionalAlternative", cultureInfo)
                                : $"{resourceManager.GetString("HeadingConditionalNoAlternate", cultureInfo)} <span style='color: #139ab8;' >{bookingData.ReferenceNumber}</span>.");

                        //CrossSale
                        var CrossSellProductData = new StringBuilder();

                        if (crossSellData != null)
                        {
                            var count = 1;
                            foreach (var product in crossSellData)
                            {
                                decimal pricePercentage = 0;
                                decimal OfferAmount = 0;
                                var originalPrice = decimal.Parse(Convert.ToString(product.GateBaseMinPrice));
                                var sellPrice = decimal.Parse(Convert.ToString(product.SellMinPrice));

                                if (originalPrice != sellPrice)
                                {
                                    OfferAmount = originalPrice - sellPrice;
                                    pricePercentage = Math.Round((OfferAmount / originalPrice) * 100, 0);
                                }
                                var isOffVisible = pricePercentage == 0 ? "none !important" : "";
                                var OffText = $"<b class='off' style='display:{isOffVisible}'> &nbsp;{(pricePercentage > 0 ? pricePercentage.ToString() + "%" : "")} &nbsp;</b>";
                                var CrossSellProduct = new StringBuilder(""
                                    + "<td class='force-col' style='overflow: hidden;vertical-align:top;padding:0;width:48%'>"
                                    + "<table style='width: 100%;border:1px solid #EEE;margin:0;padding:0' cellpadding='0' cellspacing='0'>"
                                    + "<tr><td style='vertical-align:top;padding:0;'>"
                                    + $"<a href='{bookingData.AffiliateURL}{product.ActualServiceUrl}' class='pc-img' title='{product.Name}'>"
                                    + $"<img width='100%' height='auto' style='width: 100%;height: auto;display:block;' src='https://res.cloudinary.com/https-www-isango-com/image/upload/f_auto/t_m_Prod/{product.Images?.FirstOrDefault(x => x.Thumbnail == true)?.Name.Replace(" ", "%20").Replace("+", "%20") ?? product.Images?.FirstOrDefault()?.Name.Replace(" ", "%20").Replace("+", "%20") ?? "v1663043426/boat_home_about.jpg"}'"
                                                + "alt='{product.Name}'></a>"
                                    + "</td></tr>"
                                    + $"<tr><td style='font-size: 12px;vertical-align: top;padding-top:10px;padding-right:10px;padding-top:10px;padding-left:10px;'><a style='line-height: 120%;color: #333333;' href='{bookingData.AffiliateURL}{product.ActualServiceUrl}'>{product.Name}</a> &nbsp; {(pricePercentage > 0 ? OffText : "")}</td></tr>"
                                    + $"<tr><td style='vertical-align:top;padding-top:0;padding-right:10px;padding-bottom:10px;padding-left:10px;font-size: 12px;vertical-align: middle;'>"
                                    + $"<small style='color: #bdbdbd;font-size: 10px;'>from</small><del style='color: #bdbdbd !important;font-size:10px;'>{(OfferAmount == 0 ? "" : $"{_preCurrency} {originalPrice} {_postCurrency}")}</del><br>"
                                    + $"<b style='color: #333333;font-weight:bold;'>{_preCurrency} {product.SellMinPrice} {_postCurrency}</b> <small style='color: #bdbdbd;font-size: 10px;'>per person</small>"
                                    + "</td></tr></table>");

                                CrossSellProduct.Replace("##ProductTitle##", product.Name);
                                if (count == 2)
                                {
                                    CrossSellProductData.Append("<td width='4%' style='width:4%' class='vspcr hide'>&nbsp;</td>");
                                }
                                CrossSellProductData.Append(CrossSellProduct);
                                count++;
                            }
                        }
                        else
                        {
                            mailBody.Replace("##displayCrossSell##", "display: none");
                        }

                        mailBody.Replace("##CrossSellData##", CrossSellProductData.ToString());
                    }
                }
                else
                {
                    foreach (string key in data.Keys)
                    {
                        mailBody.Replace(key, data[key].ToString());
                    }
                }

                //var mailtext = $"'{mailBody.ToString()}'";
                //mailBody.Replace("##UserName##", mailtext);

                return mailBody.ToString().Replace("???", "");
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Generate print msg section
        /// </summary>
        /// <param name="products"></param>
        /// <param name="resourceManager"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        private string GeneratePrintMessageSection(List<MailProduct> products, ResourceManager resourceManager,
            CultureInfo cultureInfo)
        {
            var messageSection = new StringBuilder();
            var voucherProducts = products.FindAll(allProducts => allProducts.IsReceipt.Equals(false));
            if (voucherProducts.Count > 0)
            {
                messageSection.Append(
                    $"<p style='padding: 5px; margin-left: 6px; font-size: 11px; background:#f4f4f4;'> <strong style='font-size:16px; font-family:Arial, Helvetica, sans-serif; color:#2197b1;'>{resourceManager.GetString("immediately", cultureInfo)}</strong></p>");
                messageSection.Append(
                    $"<div style='margin-left:6px;'><a href='##Link##' style='color:#489b25; font-weight:bold; font-size:18px; font-family:Arial, Helvetica, sans-serif; margin-bottom:3px; margin-left:6px; padding-left:6px; display:block; clear:right;line-height:1.7;'>{resourceManager.GetString("VoucherLink", cultureInfo)}</a><br />");
                if (products.Count(x => x.IsSmartPhoneVoucher) <= 0)
                {
                    messageSection.Append(
                        $"<div style='padding-bottom: 0px;padding-top:10px; display:block; margin-left:6px; padding-left:6px; font-size: 12px; clear:left;color:#2197b1;font-family:Arial, Helvetica, sans-serif;'>{resourceManager.GetString("PrintTextSmartPhone", cultureInfo)}</div></div>");
                }
            }

            return messageSection.ToString();
        }

        /// <summary>
        /// Generate print msg section for alternative
        /// </summary>
        /// <param name="products"></param>
        /// <param name="resourceManager"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        private string GeneratePrintMessageSectionForAlternative(List<MailProduct> products,
            ResourceManager resourceManager, CultureInfo cultureInfo)
        {
            var messageSection = new StringBuilder();
            var voucherProducts = products.FindAll(allProducts => allProducts.IsReceipt.Equals(false));
            if (voucherProducts.Count > 0)
            {
                messageSection.Append(
                    $"<p style='padding: 5px; margin-left: 6px; font-size: 11px; background:#f4f4f4;'> <strong style='font-size:16px; font-family:Arial, Helvetica, sans-serif; color:#2197b1;'>{resourceManager.GetString("IgnoreIfAlready", cultureInfo)}</strong></p>");
            }

            return messageSection.ToString();
        }

        /// <summary>
        /// Generate booking section
        /// </summary>
        /// <param name="bookingData"></param>
        /// <returns></returns>
        private string GenerateBookingSection(MailBooking bookingData)
        {
            var bookingSection = new StringBuilder();
            List<string> postCurrencies = null;
            if (ConfigurationManager.AppSettings[Constant.PostCurrencyLanguages] != null)
                postCurrencies = ConfigurationManager.AppSettings[Constant.PostCurrencyLanguages].Split(',').ToList();

            if (postCurrencies != null && postCurrencies.Contains(bookingData.Language.ToLowerInvariant()))
                _postCurrency = $"  {bookingData.CurrencySymbol}";
            else
                _preCurrency = $"  {bookingData.CurrencySymbol}";

            bookingSection.Append(
                $"<tr><td width='84%'><strong style='color:#E0521B; font-size:15px;'>{bookingData.BookingAmountLabel}</strong></td>");
            bookingSection.Append("<td><strong style='color:#E0521B; font-size:15px;' width='16%'>");
            bookingSection.Append(_preCurrency);
            bookingSection.Append(bookingData.BookingAmount.ToString("0.00"));
            bookingSection.Append(_postCurrency);

            bookingSection.Append("</strong></td></tr>");
            if (bookingData.Multisave > 0)
            {
                bookingSection.Append($"<tr><td width='84%'>{bookingData.MultisaveDiscountLabel}<td width='16%'>");
                bookingSection.Append("-");
                bookingSection.Append(_preCurrency);
                bookingSection.Append(bookingData.Multisave.ToString("0.00"));
                bookingSection.Append(_postCurrency);
                bookingSection.Append("</td> </tr>");
            }

            if (bookingData.Discount > 0)
            {
                bookingSection.Append($"<tr><td width='84%'>{bookingData.DiscountCodeLabel}</td><td width='16%'>");
                bookingSection.Append("-");
                bookingSection.Append(_preCurrency);
                bookingSection.Append(bookingData.Discount.ToString("0.00"));
                bookingSection.Append(_postCurrency);
                bookingSection.Append("</td></tr>");
            }

            if (bookingData.AfterDiscount > 0)
            {
                bookingSection.Append($"<tr><td width='84%'>{bookingData.AfterDiscountLabel}</td>");
                bookingSection.Append("<td>");
                bookingSection.Append(_preCurrency);
                bookingSection.Append(bookingData.AfterDiscount.ToString("0.00"));
                bookingSection.Append(_postCurrency);
                bookingSection.Append("</td></tr>");
            }
            else if (bookingData.AfterDiscount < 0)
            {
                bookingSection.Append($"<tr><td width='84%'>{bookingData.AfterDiscountLabel}</td>");
                bookingSection.Append("<td>");
                bookingSection.Append(_preCurrency);
                bookingSection.Append("0.00");
                bookingSection.Append(_postCurrency);
                bookingSection.Append("</td></tr>");
            }

            if (bookingData.TotalChargedAmount < 0)
            {
                bookingSection.Append($"<tr><td width='84%'><b>{bookingData.ChargedAmountLabel}</b></td>");
                bookingSection.Append("<td><b>");
                bookingSection.Append(_preCurrency);
                bookingSection.Append("0.00");
                bookingSection.Append(_postCurrency);
                bookingSection.Append("</b></td></tr>");
            }
            else
            {
                bookingSection.Append($"<tr><td width='84%'><b>{bookingData.ChargedAmountLabel}</b></td>");
                bookingSection.Append("<td><b>");
                bookingSection.Append(_preCurrency);
                bookingSection.Append(bookingData.TotalChargedAmount.ToString("0.00"));
                bookingSection.Append(_postCurrency);
                bookingSection.Append("</b></td></tr>");
            }

            if (bookingData.OrAmount > 0)
            {
                bookingSection.Append(
                    $"<tr><td width='84%'><b>{bookingData.OrAmountLabel}</b></td><td width='16%'><b>");
                bookingSection.Append(_preCurrency);
                bookingSection.Append(bookingData.OrAmount.ToString("0.00"));
                bookingSection.Append(_postCurrency);
                bookingSection.Append("</b></td></tr>");
            }
            else if (bookingData.OrAmount < 0)
            {
                bookingSection.Append(
                    $"<tr><td width='84%'><b>{bookingData.OrAmountLabel}</b></td><td width='16%'><b>");
                bookingSection.Append(_preCurrency);
                bookingSection.Append("0.00");
                bookingSection.Append(_postCurrency);
                bookingSection.Append("</b></td></tr>");
            }

            if (!string.IsNullOrEmpty(bookingData.OrBookingText))
            {
                bookingSection.Append("<tr><td colspan='2'>");
                bookingSection.Append(bookingData.OrBookingText);
                bookingSection.Append("</td></tr>");
            }

            return bookingSection.ToString();
        }

        /// <summary>
        /// Generate product section
        /// </summary>
        /// <param name="products"></param>
        /// <param name="resourceManager"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        private string GenerateProductSection(List<MailProduct> products, ResourceManager resourceManager,
            CultureInfo cultureInfo, string voucherlink)
        {
            var productsSection = new StringBuilder();
            var statusImg = "";

            var bundles = products.GroupBy(x => x.BUNDLESERVICEID != 0 ? x.BUNDLESERVICEID : x.ServiceId).Select(grp => grp.ToList())?.ToList();

            foreach (var bundle in bundles)
            {
                //var paxString = bundle?.FirstOrDefault()?.AgeGroupDescription.Where(x => x.PassengerType.Contains(PassengerType.Adult.ToString())).ToList()?.FirstOrDefault()?.PaxCount + " " + PassengerType.Adult;

                //foreach (var paxInfo in bundle?.FirstOrDefault()?.AgeGroupDescription)
                //{
                //    if (!paxInfo.PassengerType.Contains(PassengerType.Adult.ToString()) && !paxString.Contains(paxInfo.PassengerType))
                //    {
                //        paxString = paxString + ", " + paxInfo.PaxCount + " " + paxInfo.PassengerType;
                //    }
                //}

                var paxString = string.Empty;

                productsSection.Append($"<tr><td style='font-size: 14px;font-weight:bold;color: #333;padding:7pt 3pt 7pt 7pt;line-height: 120%;border: 1px solid #EEE;height:45px;'>{bundle?.FirstOrDefault()?.Name}</td></tr>");

                var counter = 0;

                foreach (var product in bundle)
                {
                    paxString = string.Empty;
                    product.AgeGroupDescription = product.AgeGroupDescription.OrderBy(x => x.PassengerType).ThenBy(y => y.FromAge).ToList();

                    var index = 0;
                    foreach (var paxInfo in product.AgeGroupDescription)
                    {

                        paxString = paxString
                            + ((index > 0 && index != product.AgeGroupDescription.Count) ? ", " : string.Empty)

                            + paxInfo.PaxCount + " "
                            + (!string.IsNullOrWhiteSpace(paxInfo.Description) ? paxInfo.Description : paxInfo.PassengerType);

                        index++;
                    }
                    counter = counter + 1;
                    if (product.Status == resourceManager.GetString("Confirmed", cultureInfo))
                    {
                        statusImg = resourceManager.GetString("ImgConfirmed", cultureInfo);
                    }
                    else if (product.Status == resourceManager.GetString("Cancelled", cultureInfo))
                    {
                        statusImg = resourceManager.GetString("ImgCancelled", cultureInfo);
                    }
                    else
                    {
                        statusImg = resourceManager.GetString("ImgPending", cultureInfo);
                    }

                    //if(bundle?.Count > 1)
                    //{
                    //    productsSection.Append($"<tr><td height='5' style='height:5px;font-size:.0001em'>&nbsp;</td></tr>");
                    //    productsSection.Append($"<tr><td height='1' style='height:1px;font-size:.0001em;border-top: 1px solid #EEE;'>&nbsp;</td></tr>");
                    //    productsSection.Append($"<tr><td height='5' style='height:5px;font-size:.0001em'>&nbsp;</td></tr>");
                    //}

                    productsSection.Append("<tr><td style='border: 1px solid #EEE;border-top-width: 0px;vertical-align:top;'><table style='width: 100%;' cellspacing='0' cellpadding='0' border='0'><tr><td class='fl force-col bkg-l' style='width: 425px;vertical-align:top;'><table style='text-align:left;width: 100%;float:none !important;'>");
                    productsSection.Append($"<tr><td colspan='2' height='6' style='height:6px;font-size:.0001em'>&nbsp;</td><tr>");
                    productsSection.Append($"<tr><td width='100' style='width: 100px;color: #333;font-size: 14px;padding: 0 10px 5px 10px;font-weight: bold;vertical-align:top;'>{resourceManager.GetString("Tour", cultureInfo)}{(bundle?.Count > 1 ? " " + counter.ToString() : "")}: </td>");
                    productsSection.Append($"<td style='color: #555;font-size: 14px;padding: 0 10px 5px 10px;vertical-align:top;'>{product.OptionName}</td></tr>");
                    productsSection.Append($"<tr><td style='color: #333;font-size: 14px;padding: 0 10px 5px 10px;font-weight: bold;vertical-align:top;'>{resourceManager.GetString("Guests", cultureInfo)}:</td>");
                    productsSection.Append($"<td style='color: #555;font-size: 14px;padding: 0 10px 5px 10px;vertical-align:top;'>{paxString}</td></tr>");
                    productsSection.Append($"<tr><td style='color: #333;font-size: 14px;padding: 0 10px 5px 10px;font-weight: bold;vertical-align:top;'>{resourceManager.GetString("traveldate", cultureInfo)}:</td>");
                    productsSection.Append($"<td style='color: #555;font-size: 14px;padding: 0 10px 5px 10px;vertical-align:top;'>{product.TravelDate}</td></tr>");
                    //productsSection.Append("<tr><td style='color: #333;font-size: 11px;padding: 0 10px 10px 10px;font-weight: bold;'>Extra</td>");
                    //productsSection.Append("<td style='color: #555;font-size: 11px;padding: 0 10px 10px 10px'>Extra info goes here...</td></tr>");
                    productsSection.Append("</table></td><td class='fr tac force-col' style='width: 215px;padding: 10px 0;vertical-align: middle;border-left: 1px dashed #CCC;'><table style='width: 100%;'>");

                    productsSection.Append($"<tr><td style='padding: 10px 0;text-align:center;'><img src='{statusImg}' alt='' width='78' height='23' style='margin:auto;' /></td></tr>");
                    if (product.Status.Equals(resourceManager.GetString("PendingConfirmation", cultureInfo)))
                    {
                        productsSection.Append($"<tr><td style='padding: 5px;min-height: 60px;text-align:center;font-size: 8.5pt;color: #555;' class='bookingNotes'>{resourceManager.GetString("ORProduct", cultureInfo)}</td></tr>");
                    }
                    else if (product.Status.Equals(resourceManager.GetString("Confirmed", cultureInfo)) && product.IsReceipt == true)
                    {
                        productsSection.Append($"<tr><td style='padding: 5px;min-height: 60px;text-align:center;font-size: 8.5pt;color: #555;' class='bookingNotes'>{resourceManager.GetString("FS2Step", cultureInfo)}</td></tr>");
                    }
                    else if (product.Status.Equals(resourceManager.GetString("Confirmed", cultureInfo)))
                    {
                        if (product.IsSmartPhoneVoucher)
                        {
                            productsSection.Append($"<tr><td style='padding: 5px;min-height: 60px;text-align:center;font-size: 8.5pt;color: #555;' class='bookingNotes'>{resourceManager.GetString("EVoucherAccepted", cultureInfo)}</td></tr>");
                        }
                        else
                        {
                            productsSection.Append($"<tr><td style='padding: 5px;min-height: 60px;text-align:center;font-size: 8.5pt;color: #555;' class='bookingNotes'>{resourceManager.GetString("PrintedVoucher", cultureInfo)}</td></tr>");
                        }
                        productsSection.Append("<tr><td style='padding: 10px 0;text-align:center;' class='voucherLink'>");
                        productsSection.Append($"<a href='{voucherlink}/?source=3&bookedoptionid={product.BookedOptionID}{(product.Status == Constant.Cancelled ? "&iscancelled=true" : "")}' {(product.Status == Constant.PendingConfirmation || (product.Status == Constant.Confirmed && product.IsReceipt) ? "disabled" : "")}>");
                        productsSection.Append($"<img src='{resourceManager.GetString("BtnDownload", cultureInfo)}' alt='' width='165' style='margin:auto;' height='31' />");
                        productsSection.Append("</a></td></tr>");
                    }
                    //else if (product.Status?.ToLower()?.Equals("cancelled") == true)
                    //{
                    //    productsSection.Append($"<tr><td style='padding: 5px;min-height: 60px;text-align:center;font-size: 8.5pt;color: #555;' class='bookingNotes'>{resourceManager.GetString("Cancelled", cultureInfo)}</td></tr>");
                    //}
                    productsSection.Append($"</table></td></tr></table></td></tr>");
                }
                productsSection.Append($"<tr><td style='border: 1px solid #EEE;border-top-width: 0px;'><table style='width: 100%;' cellspacing='0' cellpadding='0' border='0'>");
                productsSection.Append($"<tr><td style='padding: 10px;color: #333;font-size: 18px;text-align:left;vertical-align: top;'>{resourceManager.GetString("Amount", cultureInfo)}: </td><td class='tar force-col' style='padding: 10px;color: #333;font-size: 18px;text-align:right;vertical-align: top;'><b>{_preCurrency} {bundle?.Sum(x => x.Price).ToString("0.00")} {_postCurrency}</b></td></tr>");
                productsSection.Append($"<tr><td style='text-align:left;padding: 5px 3px 8px 8px;vertical-align: top;border-top: 1px solid #EEE;' colspan='2'><b style='font-size: 12px;line-height: 125%;color:#999999'>{resourceManager.GetString("cancellationpolPay", cultureInfo)}</b>: <br /><span style='font-size:12px;display: inline-block;color:#999999'>{bundle?.FirstOrDefault()?.CancellationPolicy}</span></td></tr>");
                productsSection.Append($"</table></td></tr>");
                productsSection.Append($"<tr><td height='20' style='height:20px;font-size:.0001em'>&nbsp;</td></tr>");
            }

            return productsSection.ToString();
        }

        /// <summary>
        /// Generate reciept section
        /// </summary>
        /// <param name="products"></param>
        /// <param name="bookingData"></param>
        /// <param name="resourceManager"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        private string GenerateReceiptSection(List<MailProduct> products, MailBooking bookingData,
            ResourceManager resourceManager, CultureInfo cultureInfo)
        {
            bool isNonReceipt = false;
            var receiptsSection = new StringBuilder();
            var receiptProducts = products?.FindAll(allProducts => allProducts.IsReceipt.Equals(true));

            var nonReceiptProducts = products?.FindAll(allProducts => allProducts.IsReceipt.Equals(false));

            if (nonReceiptProducts?.Count > 0)
            {
                isNonReceipt = true;
            }

            if (receiptProducts?.Count > 0)
            {
                var guestName = receiptProducts[0].LeadPaxName?.Trim();

                if (string.IsNullOrEmpty(guestName))
                    guestName = bookingData.LeadGuestName?.Trim();

                if (string.IsNullOrEmpty(guestName))
                    guestName = bookingData.CustomerName;

                receiptProducts.RemoveAll(x => x.APIType == APIType.Tiqets);
                if (receiptProducts.Count > 0
                ) //As discussed with HP: Not showing this section in the mail for Tiqets product
                {
                    if (isNonReceipt.Equals(true))
                        receiptsSection.Append(
                            "<div style='border-top:2px dashed #888888;margin-top:10px;padding-top:20px;'>");

                    receiptsSection.Append(
                        "<p style='padding: 5px; margin-left: 6px; font-size: 11px; background:#f4f4f4;'> <strong style='font-size:16px; font-family:Arial, Helvetica, sans-serif; color:#2197b1;'>" +
                        resourceManager.GetString("withinin48", cultureInfo) + "</strong></p>");
                    receiptsSection.Append("<div style='padding:14px; margin-left: 9px;'>");
                    receiptsSection.Append("<p>" + resourceManager.GetString("redemption", cultureInfo) + "</p>");
                    receiptsSection.Append("<p>" + resourceManager.GetString("pleaseNote", cultureInfo) + "</p>");
                    receiptsSection.Append("<p style='font-family:Arial, Helvetica, sans-serif; font-size:15px; font-weight:bold;'>" +
                                           resourceManager.GetString("yourorder", cultureInfo) + "</p>");
                    receiptsSection.Append("<p>" + resourceManager.GetString("lp", cultureInfo) +
                                           ": <span style='font-weight:bold;'>" + guestName + "</span></p>");
                    receiptsSection.Append("<p style='border-bottom:1px solid;padding-bottom:14px;'>" +
                                           resourceManager.GetString("lpe", cultureInfo) +
                                           ": <span style='font-weight:bold;'>" + bookingData.CustomerEmail +
                                           "</span></p>");

                    foreach (var receiptProduct in receiptProducts)
                    {
                        receiptsSection.Append("<p>" + resourceManager.GetString("ProductName", cultureInfo) +
                                               ": <a href='" + receiptProduct.Url +
                                               "' style='font-family:Arial, Helvetica, sans-serif; font-size:14px; font-weight:bold; color:#489b25;'>" +
                                               receiptProduct.Name + "</a></p>");

                        if (receiptProduct.AgeGroupDescription != null)
                        {
                            foreach (var item in receiptProduct.AgeGroupDescription)
                            {
                                receiptsSection.Append("<p>" + resourceManager.GetString("Numberof", cultureInfo) +
                                                       " " + MultiLingualText(item.PassengerType, resourceManager,
                                                           cultureInfo) + " " + "(" + item.FromAge + "-" + item.ToAge +
                                                       ")" + ": <span style='font-weight:bold;'>" + item.PaxCount +
                                                       "</span></p>");
                            }
                        }

                        receiptsSection.Append("<p>" + resourceManager.GetString("traveldate", cultureInfo) +
                                               ": <span style='font-weight:bold;'>" + receiptProduct.TravelDate +
                                               "</span></p>");
                    }

                    receiptsSection.Append("</div>");

                    if (isNonReceipt.Equals(true))
                        receiptsSection.Append("</div>");
                }
            }

            return receiptsSection.ToString();
        }

        private string MultiLingualText(string text, ResourceManager resourceManager, CultureInfo cultureInfo)
        {
            if (text.ToLowerInvariant().Contains("adult"))
            {
                return resourceManager.GetString("adults", cultureInfo);
            }
            else if (text.ToLowerInvariant().Contains("child"))
            {
                return resourceManager.GetString("childs", cultureInfo);
            }
            else if (text.ToLowerInvariant().Contains("infant"))
            {
                return resourceManager.GetString("infants", cultureInfo);
            }
            else if (text.ToLowerInvariant().Contains("senior"))
            {
                return resourceManager.GetString("seniors", cultureInfo);
            }
            else if (text.ToLowerInvariant().Contains("youth"))
            {
                return resourceManager.GetString("youths", cultureInfo);
            }
            else if (text.ToLowerInvariant().Contains("family"))
            {
                return resourceManager.GetString("family", cultureInfo);
            }

            return string.Empty;
        }

        /// <summary>
        /// Create Failure Mail Content
        /// </summary>
        /// <param name="templateText"></param>
        /// <param name="failureMailContextList"></param>
        /// <returns></returns>
        private string CreateFailureMailContent(string templateText, List<FailureMailContext> failureMailContextList)
        {
            var mailBody = new StringBuilder(templateText);
            var rows = new StringBuilder();
            var errorDetails = failureMailContextList.LastOrDefault()?.BookingErrors;

            foreach (var failureMailContext in failureMailContextList)
            {
                rows.Append("<tr>");
                rows.Append(
                    $"<td>{(!string.IsNullOrEmpty(failureMailContext.BookingReferenceNumber) ? failureMailContext.BookingReferenceNumber : "N/A")}</td>");
                rows.Append(
                    $"<td>{(!string.IsNullOrEmpty(failureMailContext.ServiceId.ToString()) ? failureMailContext.ServiceId.ToString() : "N/A")}</td>");
                rows.Append(
                    $"<td>{(!string.IsNullOrEmpty(failureMailContext.TokenId) ? failureMailContext.TokenId : "N/A")}</td>");
                rows.Append(
                    $"<td>{(!string.IsNullOrEmpty(failureMailContext.APIBookingReferenceNumber) ? failureMailContext.APIBookingReferenceNumber : "N/A")}</td>");
                rows.Append(
                    $"<td>{(!string.IsNullOrEmpty(failureMailContext.TravelDate.ToString(CultureInfo.InvariantCulture)) ? failureMailContext.TravelDate.ToString("MM/dd/yyyy") : "N/A")}</td>");
                rows.Append(
                    $"<td>{(!string.IsNullOrEmpty(failureMailContext.CustomerEmailId) ? failureMailContext.CustomerEmailId : "N/A")}</td>");
                rows.Append(
                    $"<td>{string.Empty/*(!string.IsNullOrEmpty(failureMailContext.ContactNumber) ? failureMailContext.ContactNumber : "N/A")*/}</td>");
                rows.Append($"<td>{failureMailContext.APICancellationStatus}</td>");
                rows.Append($"<td>{failureMailContext.ApiTypeName}</td>");
                rows.Append($"<td>{failureMailContext.ServiceOptionId}</td>");
                rows.Append($"<td>{failureMailContext.OptionName}</td>");
                rows.Append($"<td>{failureMailContext.SupplierOptionCode}</td>");
                rows.Append($"<td>{failureMailContext.AvailabilityReferenceId}</td>");
                rows.Append($"<td>{_storageAccount}</td>");
                rows.Append($"<td>{failureMailContext.CustomerName}</td>");
                rows.Append($"<td>{failureMailContext.ApiErrorMessage}</td>");
                rows.Append("</tr>");
            }
            if (!string.IsNullOrWhiteSpace(errorDetails))
            {
                rows.Append("<tr>");
                rows.Append($"<td colspan='20'>{errorDetails}</td>");
                rows.Append("</tr>");
            }

            mailBody.Replace("##TableContent##", rows.ToString());

            return mailBody.ToString();
        }

        /// <summary>
        /// Private method to create cancel booking mail content used by 'GenerateCancelBookingMailBody'
        /// </summary>
        /// <param name="templateText"></param>
        /// <param name="cancellationMailContextList"></param>
        /// <returns></returns>
        private string CreateCancelBookingMailContent(string templateText,
            List<CancellationMailText> cancellationMailContextList)
        {
            var mailBody = new StringBuilder(templateText);
            var rows = new StringBuilder();

            foreach (var cancellationMailContext in cancellationMailContextList)
            {
                rows.Append("<tr>");
                rows.Append(
                    $"<td>{(!string.IsNullOrEmpty(cancellationMailContext.BookingReferenceNumber) ? cancellationMailContext.BookingReferenceNumber : "N/A")}</td>");
                rows.Append(
                    $"<td>{(!string.IsNullOrEmpty(cancellationMailContext.TokenId) ? cancellationMailContext.TokenId : "N/A")}</td>");
                rows.Append(
                    $"<td>{(!string.IsNullOrEmpty(cancellationMailContext.ServiceId.ToString()) ? cancellationMailContext.ServiceId.ToString() : "N/A")}</td>");
                rows.Append(
                    $"<td>{(!string.IsNullOrEmpty(cancellationMailContext.ServiceName) ? cancellationMailContext.ServiceName : "N/A")}</td>");
                rows.Append(
                    $"<td>{(!string.IsNullOrEmpty(cancellationMailContext.OptionName) ? cancellationMailContext.OptionName : "N/A")}</td>");
                rows.Append(
                    $"<td>{(!string.IsNullOrEmpty(cancellationMailContext.APIBookingReferenceNumber) ? cancellationMailContext.APIBookingReferenceNumber : "N/A")}</td>");
                rows.Append(
                    $"<td>{(!string.IsNullOrEmpty(cancellationMailContext.TravelDate.ToString(CultureInfo.InvariantCulture)) ? cancellationMailContext.TravelDate.ToString("MM/dd/yyyy") : "N/A")}</td>");
                rows.Append(
                    $"<td>{(!string.IsNullOrEmpty(cancellationMailContext.CustomerEmailId) ? cancellationMailContext.CustomerEmailId : "N/A")}</td>");
                rows.Append(
                    $"<td>{(!string.IsNullOrEmpty(cancellationMailContext.ContactNumber) ? cancellationMailContext.ContactNumber : "N/A")}</td>");
                rows.Append($"<td>{cancellationMailContext.IsangoBookingCancellationStatus}</td>");
                rows.Append($"<td>{cancellationMailContext.PaymentRefundStatus}</td>");
                rows.Append($"<td>{(!string.IsNullOrEmpty(cancellationMailContext.PaymentRefundAmount) ? cancellationMailContext.PaymentRefundAmount : "")}</td>");
                rows.Append($"<td>{cancellationMailContext.SupplierCancellationStatus}</td>");
                rows.Append($"<td>{cancellationMailContext.ApiTypeName}</td>");
                rows.Append("</tr>");
            }

            mailBody.Replace("##TableContent##", rows.ToString());
            mailBody.Replace("???", "");

            return mailBody.ToString();
        }

        /// <summary>
        /// Load Template
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        private string LoadTemplate(string template)
        {
            //Get file path
            //@"D:\Repository\BumbleBee\BumbleBee\WebAPI\Templates\MailTemplates\CustomerTemplates\Email_en.html";
            var templatePath = Path.Combine(WebRootPath.GetWebRoot(), Constant.Template, Constant.MailTemplateBasePath, $"{template}.html");
            // Get template from blob.
            var fsInput = File.Open(templatePath, FileMode.Open);
            var fileContents = new byte[Convert.ToInt32(fsInput.Length)];
            //Read File contents into a Byte Array.
            fsInput.Read(fileContents, 0, Convert.ToInt32(fsInput.Length));
            fsInput.Close();
            //Load Byte Array into a Memory Stream Object.
            using (var oMemoryStream = new MemoryStream(fileContents))
            {
                return Encoding.ASCII.GetString(oMemoryStream.ToArray());
            }
        }

        /// <summary>
        /// Encrypt the plain text
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        private static string Encrypt(string plainText)
        {
            var keySize = 256;

            var initVectorBytes = Encoding.ASCII.GetBytes(Constant.InitVector);
            var saltValueBytes = Encoding.ASCII.GetBytes(Constant.SaltPhrase);

            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            var password = new Rfc2898DeriveBytes(Constant.PassPhrase, saltValueBytes, 1000); // Iteration count is set to 1000 (adjust it as needed)

            var keyBytes = password.GetBytes(keySize / 8);

            using var aes = Aes.Create();
            aes.Mode = CipherMode.CBC;

            var encryptor = aes.CreateEncryptor(keyBytes, initVectorBytes);

            using var memoryStream = new MemoryStream();
            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
            {
                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                cryptoStream.FlushFinalBlock();
            }

            var cipherTextBytes = memoryStream.ToArray();

            var cipherText = Convert.ToBase64String(cipherTextBytes);

            return cipherText;
        }
        /// <summary>
        /// Get culture information
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        private CultureInfo GetCultureInfo(string languageCode)
        {
            string culture;
            switch (languageCode)
            {
                case Constant.DE:
                    culture = Constant.Germany;
                    break;

                case Constant.FR:
                    culture = Constant.French;
                    break;

                case Constant.ES:
                    culture = Constant.Spanish;
                    break;

                default:
                    culture = string.Empty;
                    break;
            }

            return culture != string.Empty ? new CultureInfo(culture) : Thread.CurrentThread.CurrentCulture;
        }
        #endregion private Methods
    }
}