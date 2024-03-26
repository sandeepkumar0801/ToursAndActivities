using Isango.Entities.Booking;
using Isango.Entities.Enums;
using Isango.Entities.Mailer;
using Isango.Mailer.Constants;
using Isango.Mailer.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading;
using Util;

namespace Isango.Mailer.Services
{
    public class TemplateContextService : ITemplateContextService
    {
        /// <summary>
        /// Get FS/OR mail template context
        /// </summary>
        /// <param name="bookingDetail"></param>
        /// <param name="numbers"></param>
        /// <returns></returns>
        public TemplateContext GetEmailTemplateContext(Booking bookingDetail, Dictionary<string, string> numbers, bool? isReceive = false)
        {
            var resourceManager = new ResourceManager(Constant.ResourceManagerBaseName, Assembly.GetExecutingAssembly());
            var languageCode = bookingDetail?.Language?.Code?.ToLowerInvariant() ?? "en";

            var cultureInfo = GetCultureInfo(bookingDetail.Language.Code.ToLowerInvariant());
            var products = new List<MailProduct>();
            var mailContext = new MailHeader();
            double multiSaveDiscountAmount = 0;
            double discountAmount = 0;
            var count = 1;
            var url = bookingDetail.Affiliate.AffiliateBaseURL;
            var templateContext = new TemplateContext
            {
                AffiliateId = Guid.Parse(bookingDetail.Affiliate.Id),
                BookingRef = bookingDetail.ReferenceNumber,
                BookingID = bookingDetail.BookingId,
                Language = bookingDetail.Language.Code,
            };
            var mailBooking = new MailBooking
            {
                ReferenceNumber = bookingDetail.ReferenceNumber,
                CurrencySymbol = bookingDetail.Currency.Symbol,
                CustomerName = $"{bookingDetail.User.FirstName} {bookingDetail.User.LastName}",
                CompanyEmail = bookingDetail.Affiliate.AffiliateCompanyDetail.CompanyEmail,
                CustomerEmail = bookingDetail.VoucherEmailAddress,
                Language = bookingDetail.Language.Code,
                TermsAndConditionLink = bookingDetail?.Affiliate?.TermsAndConditionLink,
                AffiliateName = bookingDetail?.Affiliate?.Name,
                AffiliateURL = bookingDetail?.Affiliate?.AffiliateBaseURL,
                AffiliateGroupID = bookingDetail?.Affiliate?.GroupId
            };
            foreach (var product in bookingDetail.SelectedProducts)
            {
                if (product.Status == OptionBookingStatus.Failed)
                    continue;

                var mailProduct = new MailProduct
                {
                    Name = product.Name,
                    Price = product.ProductOptions.FirstOrDefault()?.SellPrice.Amount ?? 0.0M,
                    CurrencySymbol = bookingDetail.Currency.Symbol,
                    Multisave = product.MultisaveDiscountedPrice,
                    Discount = product.DiscountedPrice,
                    ServiceId = product.Id,
                    IsReceipt = product.GetIsReceipt(),
                    LinkType = product.LinkType,
                    LinkValue = product.LinkValue,
                    APIType = product.APIType,
                    LeadPaxName = $"{product.ProductOptions?.FirstOrDefault().Customers?.FirstOrDefault().FirstName} {product.ProductOptions?.FirstOrDefault().Customers?.FirstOrDefault().LastName}",
                    Sequence = count,
                    IsSmartPhoneVoucher = product.IsSmartPhoneVoucher,
                    IsShowSupplierVoucher = product.IsShowSupplierVoucher,
                    OptionName = product.ProductOptions?.FirstOrDefault()?.Name,
                    BookedOptionID = product.BundleOptionId
                };
                if (product.ProductOptions?.FirstOrDefault().TravelInfo != null)
                {
                    mailProduct.Adults = product.ProductOptions.FirstOrDefault().TravelInfo.NoOfPassengers.FirstOrDefault(x => x.Key.Equals(PassengerType.Adult)).Value;
                    mailProduct.Children = product.ProductOptions.FirstOrDefault().TravelInfo.NoOfPassengers.FirstOrDefault(x => x.Key.Equals(PassengerType.Child)).Value;
                    mailProduct.TravelDate = product.ProductOptions.FirstOrDefault().TravelInfo.StartDate.ToString(Constant.DateFormat);
                    mailProduct.AgeGroupDescription = product?.AgeGroupDescription;
                    mailProduct.PerPaxPdfDetails = product?.PaxWisePdfDetails;
                    mailProduct.CancellationPolicy = product?.CancellationPolicy;
                    mailProduct.BUNDLESERVICEID = product?.BUNDLESERVICEID;
                }

                //If pax wise pdf is available then dont show supplier voucher
                if (mailProduct.PerPaxPdfDetails != null && mailProduct.PerPaxPdfDetails.Count > 0 && string.IsNullOrEmpty(mailProduct.LinkType) && string.IsNullOrEmpty(mailProduct.LinkValue))
                {
                    mailProduct.IsShowSupplierVoucher = false;
                }

                //Need to discuss as in unity it is coming from cache
                mailProduct.Url = $"{Constant.IsangoBaseUrl}{mailProduct.ServiceId}";

                mailProduct.Message = string.Empty;
                if (bookingDetail.Language.Code == Constant.DE && product.IsReceipt)
                    mailProduct.Message = resourceManager.GetString("ReceiptMessage", cultureInfo);

                if (product.ProductOptions != null && product.ProductOptions.FirstOrDefault().AvailabilityStatus.Equals(AvailabilityStatus.ONREQUEST))
                {
                    mailProduct.Status = Constant.PendingConfirmation;
                    mailBooking.OrBookingText = $"<b>{resourceManager.GetString("Pleasenotes", cultureInfo)}</b>: {resourceManager.GetString("Forproducts", cultureInfo)}<b> {resourceManager.GetString("PendingConfirmation", cultureInfo)}</b>, {resourceManager.GetString("YouWillRecieve", cultureInfo)}.";
                }
                else if (product.ProductOptions != null && product.ProductOptions.FirstOrDefault().AvailabilityStatus.Equals(AvailabilityStatus.AVAILABLE))
                {
                    mailProduct.Status = Constant.Confirmed;
                }

                if (product.Status == OptionBookingStatus.Confirmed)
                {
                    mailProduct.Status = resourceManager.GetString("Confirmed", cultureInfo);
                    templateContext.MailActionType = ActionType.CustomerVoucherFS;
                }

                if (product.Status == OptionBookingStatus.Cancelled)
                {
                    mailProduct.Status = resourceManager.GetString("Cancelled", cultureInfo);
                }

                if (product.Status == OptionBookingStatus.Requested)
                {
                    mailProduct.Status = resourceManager.GetString("PendingConfirmation", cultureInfo);
                    templateContext.MailActionType = ActionType.CustomerVoucherOR;
                }
                products.Add(mailProduct);
                count++;
            }
            foreach (var currentProduct in products)
            {
                if (currentProduct.Status.Equals(resourceManager.GetString("PendingConfirmation", cultureInfo)))
                {
                    mailBooking.OrAmount += Math.Round(currentProduct.Price - (currentProduct.Multisave + currentProduct.Discount), 2);
                    multiSaveDiscountAmount += (double)currentProduct.Multisave;
                    discountAmount += (double)currentProduct.Discount;
                }
                else if (currentProduct.Status.Equals(resourceManager.GetString("Confirmed", cultureInfo)))
                {
                    mailBooking.TotalChargedAmount += Math.Round(currentProduct.Price - (currentProduct.Multisave + currentProduct.Discount), 2);
                    multiSaveDiscountAmount += (double)currentProduct.Multisave;
                    discountAmount += (double)currentProduct.Discount;
                }

                if (!currentProduct.Status.Equals(Constant.Cancelled))
                    mailBooking.BookingAmount += Math.Round(currentProduct.Price, 2);
            }
            mailBooking.Multisave = Math.Round(multiSaveDiscountAmount, 2);
            mailBooking.Discount = Math.Round(discountAmount, 2);

            mailBooking.VoucherLink = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.WebAPIBaseUrl) + "/api" + Constant.VoucherUrl + bookingDetail.ReferenceNumber;

            templateContext.TemplateName = resourceManager.GetString("TemplateName", cultureInfo);
            mailBooking.BookingAmountLabel = resourceManager.GetString("BookingAmountLabel", cultureInfo);
            mailBooking.MultisaveDiscountLabel = resourceManager.GetString("MultisaveDiscountLabel", cultureInfo);
            mailBooking.DiscountCodeLabel = resourceManager.GetString("DiscountCodeLabel", cultureInfo);
            mailBooking.AfterDiscountLabel = resourceManager.GetString("AfterDiscountLabel", cultureInfo);
            mailBooking.ChargedAmountLabel = resourceManager.GetString("ChargedAmountLabel", cultureInfo);
            mailBooking.OrAmountLabel = resourceManager.GetString("OrAmountLabel", cultureInfo);

            mailBooking.LogoPath = $"{ConfigurationManagerHelper.GetValuefromAppSettings(Constant.CDN)}{Constant.ImgPath}" + bookingDetail?.Affiliate?.Id?.ToUpper() + ".png";

            mailContext.Subject = $"{Constant.BookingConfirmationSubject}{mailBooking.ReferenceNumber}";
            if (products.Count > 0)
            {
                var receiptProducts = products.FindAll(allProducts => allProducts.IsReceipt.Equals(true));
                if (receiptProducts.Count > 0)
                    mailContext.Subject = $"{Constant.BookingRefNo}{mailBooking.ReferenceNumber}";
            }

            //Set booking mail subject
            if (bookingDetail.SelectedProducts.Any(x => x.Status == OptionBookingStatus.Confirmed))
            {
                mailContext.Subject = resourceManager.GetString("BookingConfirmationSubject", cultureInfo) + " " + mailBooking.ReferenceNumber;
                // $"{Constant.BookingConfirmationSubject}{mailBooking.ReferenceNumber}";
            }
            else if (bookingDetail.SelectedProducts.Any(x => x.Status == OptionBookingStatus.Requested))
            {
                mailContext.Subject = resourceManager.GetString("BookingOnRequestSubject", cultureInfo) + " " + mailBooking.ReferenceNumber;
                // $"{Constant.BookingOnRequestSubject}{mailBooking.ReferenceNumber}";
            }
            else
            {
                mailContext.Subject = resourceManager.GetString("CancelBookingMailSubject", cultureInfo) + " " + mailBooking.ReferenceNumber;
                //$"{Constant.CancelBookingMailSubject}{mailBooking.ReferenceNumber}";
            }

            if ((bookingDetail.SelectedProducts.Any(x => x.Status == OptionBookingStatus.Confirmed) ||
                bookingDetail.SelectedProducts.Any(x => x.Status == OptionBookingStatus.Requested))
                && bookingDetail.SelectedProducts.Any(x => x.Status == OptionBookingStatus.Cancelled))
            {
                mailContext.Subject = mailContext.Subject = resourceManager.GetString("AmendBookingSubject", cultureInfo) + " " + mailBooking.ReferenceNumber;
                // $"{Constant.AmendBookingSubject}{mailBooking.ReferenceNumber}";
            }

            List<MailProduct> specificReceiptProducts = null;
            if (products.Count > 0)
            {
                var simpleReceiptProducts = products.FindAll(allProducts => allProducts.IsReceipt.Equals(true));
                if (simpleReceiptProducts.Count > 0)
                {
                    specificReceiptProducts = simpleReceiptProducts.FindAll(allProducts => allProducts.IsReceiptException.Equals(true));
                }
            }

            if (specificReceiptProducts != null && specificReceiptProducts.Count > 0)
            {
                mailContext.Subject = mailContext.Subject = resourceManager.GetString("AmendBookingSubject", cultureInfo) + " " + mailBooking.ReferenceNumber;
                // $"{Constant.AmendBookingSubject}{mailBooking.ReferenceNumber}";
                //templateContext.TemplateName = Constant.AmendMailTemplateName;
            }

            //Condition: When customer send amend booking request and add extra pax, after customer payment, 
            //email send to them with this Subject.  
            if (isReceive == true)
            {
                mailContext.Subject = mailContext.Subject = resourceManager.GetString("AmendBookingSubject", cultureInfo) + " " + mailBooking.ReferenceNumber;
                //$"{Constant.AmendBookingSubject}{mailBooking.ReferenceNumber}";
            }

            var data = new Dictionary<string, object>
            {
                { "#ProductData#", products },
                { "#BookingData#", mailBooking },
                { "#SupportNumbers#", GetSupportNumbers(numbers) }
            };
            templateContext.Data = data;
            mailContext.To = new[] { mailBooking.CustomerEmail };
            mailContext.CC = new[] { mailBooking.CompanyEmail };
            mailContext.From = mailBooking.CompanyEmail;
            templateContext.IsOfflineEmail = true;

            var mailContexts = new List<MailHeader>
            {
                mailContext
            };
            templateContext.MailContextList = mailContexts;

            return templateContext;
        }

        /// <summary>
        /// Get cancelled booking email template context
        /// </summary>
        /// <param name="bookingDetail"></param>
        /// <param name="numbers"></param>
        /// <returns></returns>
        public TemplateContext GetCancelledEmailTemplateContext(Booking bookingDetail, Dictionary<string, string> numbers)
        {
            var templateContext = new TemplateContext
            {
                IsOfflineEmail = true,
                AffiliateId = new Guid(bookingDetail.Affiliate.Id),
                BookingRef = bookingDetail.ReferenceNumber,
                BookedOptionId = bookingDetail.SelectedProducts.FirstOrDefault()?.BundleOptionId.ToString()
            };

            var url = bookingDetail.Affiliate.AffiliateBaseURL;
            var languageCode = bookingDetail.Language.Code.ToLower();
            var resourceManager = new ResourceManager(Constant.ResourceManagerBaseName, Assembly.GetExecutingAssembly());
            var cultureInfo = GetCultureInfo(languageCode);

            templateContext.TemplateName = resourceManager.GetString("CancelTemplateName", cultureInfo);

            var bookingData = new MailBooking
            {
                CustomerEmail = bookingDetail.VoucherEmailAddress,
                ReferenceNumber = bookingDetail.ReferenceNumber,
                VoucherLink = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.WebAPIBaseUrl) + "/api" + Constant.CancelVoucherUrl + bookingDetail.ReferenceNumber + "/" + bookingDetail.SelectedProducts.FirstOrDefault()?.BundleOptionId
                //$"{url?.Trim()}{Constant.CancelVoucherUrl}{bookingDetail.ReferenceNumber}/{bookingDetail.SelectedProducts.FirstOrDefault()?.BundleOptionId}"
            };

            var data = new Dictionary<string, object>
            {
                {"#Name#", bookingDetail.User.FirstName},
                {"#RefNo#", bookingDetail.ReferenceNumber},
                {"#FromEmail#", bookingDetail.Affiliate.AffiliateCompanyDetail.CompanyEmail},
                {"#BookingData#", bookingData},
                { "#SupportNumbers#", GetSupportNumbers(numbers) },
                { "#Link#", bookingData.VoucherLink }
            };

            bookingData.LogoPath = $"{ConfigurationManagerHelper.GetValuefromAppSettings(Constant.CDN)}{Constant.ImgPath}" + bookingDetail?.Affiliate?.Id?.ToUpper() + ".png";

            templateContext.Data = data;
            var mailContext = new MailHeader();
            templateContext.MailActionType = ActionType.CoustomerVoucherCancellation;
            templateContext.Language = bookingDetail.Language.Code;
            mailContext.To = new[] { bookingDetail.VoucherEmailAddress };
            mailContext.Subject = resourceManager.GetString("CancelSubject", cultureInfo) + bookingDetail.ReferenceNumber;
            mailContext.CC = new[] { bookingDetail.Affiliate.AffiliateCompanyDetail.CompanyEmail };
            mailContext.From = bookingDetail.Affiliate.AffiliateCompanyDetail.CompanyEmail;

            var mailContexts = new List<MailHeader> { mailContext };
            templateContext.MailContextList = mailContexts;
            return templateContext;
        }

        /// <summary>
        /// Get support numbers
        /// </summary>
        /// <param name="numbers"></param>
        /// <returns></returns>
        private string GetSupportNumbers(Dictionary<string, string> numbers)
        {
            var supportNumbers = new StringBuilder();
            if (numbers == null) return supportNumbers.ToString();
            foreach (var number in numbers)
            {
                supportNumbers.Append(number.Value);
                supportNumbers.Append("<br>");
            }
            return supportNumbers.ToString();
        }

        /// <summary>
        /// Get Culture Information
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
    }
}