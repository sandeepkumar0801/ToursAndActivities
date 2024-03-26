using Isango.Entities.Enums;
using Isango.Entities.Mailer.Voucher;
using Isango.Mailer.Constants;
using Isango.Mailer.ServiceContracts;
using Isango.Persistence.Contract;
using Logger.Contract;
using RealObjects.PDFreactor.Webservice.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading;
using Util;
using Result = RealObjects.PDFreactor.Webservice.Client.Result;
using QRCoder;
using System.Security.Principal;
using ZXing.PDF417.Internal;
using ZXing;
using SkiaSharp;
using ZXing.SkiaSharp;
using ZXing.QrCode;

namespace Isango.Mailer.Services
{
    public class MailAttachmentService : IMailAttachmentService
    {
        private readonly IBookingPersistence _bookingPersistence;
        private readonly ILogger _log;
        private readonly string letsettleFilePath;
        private readonly string isangoFilePath;
        private readonly string goGiffilePath;


        public MailAttachmentService(IBookingPersistence bookingPersistence, ILogger logger)
        {
            _bookingPersistence = bookingPersistence;
            _log = logger;
            letsettleFilePath = Path.Combine(WebRootPath.GetWebRootPath(), Constant.Image, Constant.LetSettleImage);
            isangoFilePath = Path.Combine(WebRootPath.GetWebRootPath(), Constant.Image, Constant.IsangoLogo);
            goGiffilePath = Path.Combine(WebRootPath.GetWebRootPath(), Constant.Image, Constant.WatermarkImage);
        }


        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        /// <summary>
        /// Get voucher for FS and OR product
        /// </summary>
        /// <param name="bookingRefNo"></param>
        /// <returns></returns>
        public Tuple<byte[], string, string> GetBookedVoucherNew(string bookingRefNo, bool isPDFVoucher = false, int Source = 3, int? bookedoptionid = null, bool? iscancelled = false)
        {
#pragma warning disable S1168 // Empty arrays and collections should be returned instead of null
            if (bookingRefNo == null) { return null; }
#pragma warning restore S1168 // Empty arrays and collections should be returned instead of null

            // Get Booking Details.
            //Note: We are setting isSupplier as false for retrieving data of customers
            var bookingData = _bookingPersistence.GetBookingDataForMail(bookingRefNo, false, Source, bookedoptionid);
            var bookingDataOthers = ((BookingDataOthers)bookingData);

            //Initialize resource manager
            var resourceManager = new ResourceManager(Constant.ResourceManagerBaseName, Assembly.GetExecutingAssembly());
            var cultureInfo = GetCultureInfo(bookingData.LanguageCode?.ToLowerInvariant());
            var bookedoption = bookingDataOthers?.BookedProductDetailList?.FirstOrDefault(x => x.BookedOptionId == bookedoptionid?.ToString())
                ??
                bookingDataOthers?.BookedProductDetailList?.FirstOrDefault();

            var isMR = bookedoption?.ApiTypeId == "4" && bookingDataOthers?.MoulinRougePDFBytes?.Count > 0;
            //var isShowSupplierVoucher = (bookedoption?.QRCodeType?.ToUpper() == "LINK" && !string.IsNullOrWhiteSpace(bookedoption?.QrCode))
            //|| isMR;
            var isShowSupplierVoucher = (bookedoption?.QRCodeType?.ToUpper() == Constant.IsangoLink && !string.IsNullOrWhiteSpace(bookedoption?.QrCode))
            || isMR;

            if (bookedoption.BookedOptionStatusName.ToLower().Contains("cancel"))
            {
                iscancelled = true;
            }

            if ((iscancelled ?? false) || (!isShowSupplierVoucher && ((BookingDataOthers)bookingData)?.BookedProductDetailList?.Count <= 1))
            {
                // Generate the attachment.
                var attachment = GeneratePdfForBookingNew((BookingDataOthers)bookingData, resourceManager, cultureInfo, isPDFVoucher, iscancelled ?? false);

                // Convert HTML template to PDF.
                var byteArray = ConvertHtmlToByteArrayNew(attachment?.Item1, resourceManager, cultureInfo, attachment?.Item2, attachment?.Item3, isPDFVoucher, false, null, iscancelled);

                return Tuple.Create(byteArray, bookedoption?.ApiTypeId, bookedoption?.QrCode);
            }
            else if (isShowSupplierVoucher)
            {
                byte[] byteArray = null;
                if (isMR)
                {
                    var query = bookingDataOthers?.MoulinRougePDFBytes?.Select(x => x.ApiVoucherByte)?.ToArray();
                    byteArray = Combine(query);
                }
                else
                {
                    var pdfFilePath = $"{Path.Combine(WebRootPath.GetWebRootPath())}{Guid.NewGuid()}.pdf";

                    var filename = pdfFilePath.Split('\\').LastOrDefault();
                    var path = Path.GetDirectoryName(pdfFilePath);
                    var pdfFilePath_local = $"{path}\\{Path.GetFileNameWithoutExtension(filename)}{Path.GetExtension(filename)}";
                    try
                    {
                        if (File.Exists(pdfFilePath_local)) { File.Delete(pdfFilePath_local); }

                        using (WebClient client = new WebClient())
                        {
                            client.DownloadFile(bookedoption?.QrCode, pdfFilePath_local);
                        }
                    }
                    catch (Exception)
                    {
                        byteArray = ConvertHtmlToByteArrayNew(string.Empty, resourceManager, cultureInfo, string.Empty, string.Empty, isPDFVoucher, true, (BookingDataOthers)bookingData, iscancelled);
                        return Tuple.Create(byteArray, bookedoption?.ApiTypeId, bookedoption?.QrCode);
                    }
                    byteArray = File.ReadAllBytes(pdfFilePath_local);
                }
                return Tuple.Create(byteArray, bookedoption?.ApiTypeId, bookedoption?.QrCode);
            }
            else
            {
                var byteArray = ConvertHtmlToByteArrayNew(string.Empty, resourceManager, cultureInfo, string.Empty, string.Empty, isPDFVoucher, true, (BookingDataOthers)bookingData, iscancelled);

                return Tuple.Create(byteArray, bookedoption?.ApiTypeId, bookedoption?.QrCode);
            }
        }

        public byte[] Combine(params byte[][] pdfByteContent)
        {
            try
            {
                var pdfFilePath = $"{Path.Combine(WebRootPath.GetWebRootPath())}{Guid.NewGuid()}.pdf";
                try
                {
                    if (File.Exists(pdfFilePath)) { File.Delete(pdfFilePath); }
                }
                catch (Exception)
                {
                }

                PDFreactor pdfReactor = new PDFreactor(string.IsNullOrEmpty(ConfigurationManagerHelper.GetValuefromAppSettings("PDFReactorURL")) ? "http://192.168.0.228:9423/service/rest" : ConfigurationManagerHelper.GetValuefromAppSettings("PDFReactorURL"))
                {
                    Timeout = 0
                };

                var config = new RealObjects.PDFreactor.Webservice.Client.Configuration
                {
                    LicenseKey = "<license><licensee><name>Technical-contact@isango.com</name></licensee><product>PDFreactor</product><licensetype>Personal</licensetype><purchasedate>2020-11-05</purchasedate><options><option>pdf</option></options><signatureinformation><signdate>2020-11-05 05:16</signdate><signature>302c02140d096d4ca982620c199c9db47ac65526995cc49e02143110341083ffc9d8c18b484cb49f5458e21c251f</signature><checksum>1490</checksum></signatureinformation></license>",
                };

                config.Document = "<html>";

                config.MergeMode = MergeMode.OVERLAY;

                config.MergeDocuments = new List<Resource>();
                foreach (var item in pdfByteContent)
                {
                    var resource = new Resource { Data = item };
                    config.MergeDocuments.Add(resource);
                }

                var task = Task.Run(() => pdfReactor.Convert(config));
                if (task.Wait(TimeSpan.FromSeconds(30)))
                {
                    //Delete created PDF file
                    try
                    {
                        Result data = task.Result;
                        File.Delete(pdfFilePath);
                        return data.Document;
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
                //Result data = pdfReactor.Convert(config);
                //BinaryWriter binWriter = new BinaryWriter(new FileStream("test.pdf",
                //    FileMode.Create,
                //    FileAccess.Write));
                //binWriter.Write(data.Document);
                //binWriter.Close();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public Tuple<byte[], string, string> GetBookedVoucher(string bookingRefNo, bool isPDFVoucher = false, int Source = 3)
        {
#pragma warning disable S1168 // Empty arrays and collections should be returned instead of null
            if (bookingRefNo == null) { return null; }
#pragma warning restore S1168 // Empty arrays and collections should be returned instead of null

            // Get Booking Details.
            //Note: We are setting isSupplier as false for retrieving data of customers
            var bookingData = _bookingPersistence.GetBookingDataForMail(bookingRefNo, false, Source);

            //Initialize resource manager
            var resourceManager = new ResourceManager(Constant.ResourceManagerBaseName, Assembly.GetExecutingAssembly());
            var cultureInfo = GetCultureInfo(bookingData.LanguageCode?.ToLowerInvariant());

            // Generate the attachment.
            var attachment = GeneratePdfForBooking((BookingDataOthers)bookingData, resourceManager, cultureInfo, isPDFVoucher);

            // Convert HTML template to PDF.
            var byteArray = ConvertHtmlToByteArray(attachment?.Item1, resourceManager, cultureInfo, attachment?.Item2, attachment?.Item3, isPDFVoucher);

            return Tuple.Create(byteArray, "", "");
        }

        /// <summary>
        /// Get voucher for FS and OR product NEW
        /// </summary>
        /// <param name="bookingRefNo"></param>
        /// <param name="bookingOptionId"></param>
        /// <returns></returns>
        public byte[] GetCancelledVoucher(string bookingRefNo, string bookingOptionId, bool isPDFVoucher = false)
        {
#pragma warning disable S1168 // Empty arrays and collections should be returned instead of null
            if (string.IsNullOrEmpty(bookingRefNo) || string.IsNullOrEmpty(bookingOptionId)) { return null; }
#pragma warning restore S1168 // Empty arrays and collections should be returned instead of null

            // Get Booking Details.
            //Note: We are setting isSupplier as false for retrieving data of customers
            var bookingData = _bookingPersistence.GetBookingDataForMail(bookingRefNo, false);

            //Initialize resource manager
            var resourceManager = new ResourceManager(Constant.ResourceManagerBaseName, Assembly.GetExecutingAssembly());
            var cultureInfo = GetCultureInfo(bookingData.LanguageCode?.ToLowerInvariant());

            // Generate the attachment.
            var attachment = GeneratePdfForCancelledBooking((BookingDataOthers)bookingData, bookingOptionId);

            // Convert HTML template to PDF.
            var byteArray = ConvertHtmlToByteArray(attachment, resourceManager, cultureInfo, string.Empty, string.Empty, isPDFVoucher);

            return byteArray;
        }

        private Tuple<string, string, string> GenerateInvoiceForBooking(BookingDataOthers bookingDataOthers, ResourceManager resourceManager, CultureInfo cultureInfo, bool isPDFVoucher = false)
        {
            var discountAmount = 0.0M;
            var multiSaveAmount = 0.0M;
            var grossSellAmount = 0.0M;
            var totalToBeChargedAmount = 0.0M;
            var totalRefundAmount = 0.0M;

            var paymentSummaryBuilder = new StringBuilder();
            var templateName = "Invoice_" + bookingDataOthers?.LanguageCode.ToUpper();
            var paymentSummaryText = LoadTemplate($"{templateName}");
            string primaryLogoPath = string.Empty;
            string secondaryLogoPath = string.Empty;
            string imagePath = $"{ConfigurationManagerHelper.GetValuefromAppSettings(Constant.CDN) + Constant.ImgPath}";
            string language = bookingDataOthers.LanguageCode.ToLowerInvariant();
            var isangoDefaultLogoUri = $"{imagePath}{ConfigurationManager.AppSettings["IsangoAffiliateId"]}{".png"}";
            var htmlHeader = string.Empty;
            paymentSummaryBuilder.Append(paymentSummaryText);

            var productDetailBuilder = new StringBuilder();
            var letSettleData = File.ReadAllBytes(letsettleFilePath);
            var letSettleUri = @"data:image/png;base64," + Convert.ToBase64String(letSettleData);
            var isangoLogoData = File.ReadAllBytes(isangoFilePath);
            var isangoLogoUri = @"data:image/png;base64," + Convert.ToBase64String(isangoLogoData);

            var bookedProductDetailList = bookingDataOthers.BookedProductDetailList;
            var LeadpaxName = bookedProductDetailList?.FirstOrDefault()?.LeadPassengerName;
            var logoHeight = bookingDataOthers.CompanyName.ToString().Contains("boat") || bookingDataOthers.CompanyName.ToString().Contains("hotelbeds") || bookingDataOthers.AffiliateId.ToString().Contains("f7feee76-7b67-4886-90ab-07488cb7a167") ? "25px" : "42px";

            PrePostCurrency(bookingDataOthers, out var pre, out var post);

            if (bookingDataOthers.IsPrimaryLogo == true)
            {
                if (language.Equals("en"))
                    primaryLogoPath = imagePath + bookingDataOthers.AffiliateId + ".png";
                else
                {
                    primaryLogoPath = imagePath + bookingDataOthers.AffiliateId + ".png";
                    if (!File.Exists(primaryLogoPath))
                        primaryLogoPath = imagePath + bookingDataOthers.AffiliateId + ".png";
                }
                secondaryLogoPath = isangoDefaultLogoUri;
            }
            else
            {
                primaryLogoPath = isangoDefaultLogoUri;

                if (language.Equals("en"))
                    secondaryLogoPath = imagePath + bookingDataOthers.AffiliateId + ".png";
                else
                {
                    secondaryLogoPath = imagePath + bookingDataOthers.AffiliateId + ".png";
                    if (!File.Exists(primaryLogoPath))
                        secondaryLogoPath = imagePath + bookingDataOthers.AffiliateId + ".png";
                }
            }

            if (bookingDataOthers.AffiliateName.ToLower().Contains("isango"))
            {
                if (!isPDFVoucher)//Html Voucher
                {
                    htmlHeader = $"<tr>"
                        //+ $"<td style='color: #333;font-size: 36px;text-align: left;'>{resourceManager.GetString("Invoice", cultureInfo)}</td>"
                        + "<td style='text-align: center;'>"
                        + $"<img height='42' alt='isango!' src='https://hohobassets.isango.com/phoenix/images/isango-cs.png' />"
                        + "</td>"
                        + "</tr>";
                }
                else //PDF Voucher
                {
                    htmlHeader = $"<tr>"
                        //+ $"<td style='color: #333;font-size: 36px;text-align: left;'>{resourceManager.GetString("Invoice", cultureInfo)}</td>"
                        + "<td style='text-align: center;'>"
                        + $"<img height='42' width='99' alt='isango!' src='https://hohobassets.isango.com/phoenix/images/isango-cs.png' />"
                        + "</td>"
                        + "</tr>";
                }
            }

            if (!bookingDataOthers.AffiliateName.ToLower().Contains("isango") && (primaryLogoPath.ToLower() != secondaryLogoPath.ToLower()) && (URLExists(secondaryLogoPath)))
            {
                if (!isPDFVoucher)//Html Voucher
                {
                    htmlHeader = $"<tr>"
                    + "<td style='text-align: left;'>"
                    + $"<img style='max-height:{logoHeight};max-width: 130px;' alt='isango!' src='{primaryLogoPath}' />"
                    + "</td>"
                    //+ $"<td style='color: #333;font-size: 36px;text-align: center;'>{resourceManager.GetString("Invoice", cultureInfo)}</td>"
                    + "<td style='text-align: right;'>"
                    + $"<img height='42' alt='isango!' src='https://hohobassets.isango.com/phoenix/images/isango-cs.png' />"
                    + "</td>"
                    + "</tr>";
                }
                else//PDF Voucher
                {
                    htmlHeader =
                        $"<tr>"
                        + "<td style='text-align: left;'>"
                        + $"<img style='max-height:{logoHeight};max-width: 130px;' alt='isango!' src='{primaryLogoPath}' />"
                        + "</td>"
                        //+ $"<td style='color: #333;font-size: 36px;text-align: center;'></td>"
                        + "<td style='text-align: right;'>"
                        + $"<img height='42'  alt='isango!' src='https://hohobassets.isango.com/phoenix/images/isango-cs.png' />"
                        + "</td>"
                        + "</tr>";
                }
            }

            paymentSummaryBuilder.Replace("##InvoiceHeader##", htmlHeader);

            //paymentSummaryBuilder.Replace("##TermsAndConditionLink##", bookingDataOthers?.TermsAndConditionLink?.Trim() ?? "N/A");
            if (!isPDFVoucher)
            {
                string htmlHead = $"<div style='text-align:left;padding-bottom: 7.5pt;margin:22.5 2.25pt 0pt'><a target='_blank' href=\"https://www.isango.com/\"><img width=\"150\" alt='' src=\"{isangoLogoUri}\"></a></div>";
                string htmlFoot = $"<div style='text-align:center;padding-top: 7.5pt;margin:0 2.25pt'><span style='font-size:6.75pt;color:#333'>{resourceManager.GetString("FooterText", cultureInfo)}!</span></center><center><a target=\"_blank\" href={Constant.FooterUrl}><img width=\"70\" style=\"margin:0;padding:0;display:block;\" src=\"{isangoLogoUri}\"></a></div>";

                paymentSummaryBuilder.Replace("##HeaderSet##", htmlHead);
                paymentSummaryBuilder.Replace("##FooterSet##", htmlFoot);
            }
            else
            {
                paymentSummaryBuilder.Replace("##HeaderSet##", string.Empty);
                paymentSummaryBuilder.Replace("##FooterSet##", string.Empty);
            }

            var showDiscount = false;
            var showCancell = false;
            var productCount = 1;

            foreach (var bookedProductDetail in bookedProductDetailList)
            {
                discountAmount += Convert.ToDecimal(string.IsNullOrEmpty(bookedProductDetail?.DiscountAmount?.Trim()) ? "0.0" : bookedProductDetail?.DiscountAmount?.Trim());
                multiSaveAmount += Convert.ToDecimal(bookedProductDetail?.MultisaveDiscount?.Trim());
                grossSellAmount += Convert.ToDecimal(bookedProductDetail?.GrosSellingAmount?.Trim());

                if (bookedProductDetail.BookedOptionStatusName.Equals("On Request", StringComparison.OrdinalIgnoreCase))
                {
                    totalToBeChargedAmount += Convert.ToDecimal(bookedProductDetail.AmountOnWirecard?.Trim());
                }

                var ProductStatus = bookedProductDetail.BookedOptionStatusName.ToLower().Contains("confirm") ? resourceManager.GetString("Confirmed", cultureInfo)
                                    : bookedProductDetail.BookedOptionStatusName.ToLower().Contains("cancel") ? resourceManager.GetString("Cancelled", cultureInfo)
                                    : bookedProductDetail.BookedOptionStatusName;


                //productDetailBuilder.Append($"<tr><td> {productCount++}) {bookedProductDetail.TsProductName?.Trim()} ({bookedProductDetail.BookedOptionStatusName?.Trim()})</td><td class=\"tar\">{pre + Convert.ToDecimal(bookedProductDetail.GrosSellingAmount?.Trim()).ToString("00.00") + post}</td></tr>");

                var AgeGroups = bookingDataOthers.BookingAgeGroupList.Where(x => x.BookedOptionId == bookedProductDetail.BookedOptionId)?.ToList();
                var AdultPaxCount = AgeGroups.Where(x => x.PassengerType.Contains(PassengerType.Adult.ToString())).ToList()?.FirstOrDefault()?.PaxCount;

                var paxStringwithPrice = AdultPaxCount == null ? string.Empty : pre + " " + ((AgeGroups.Where(x => x.PassengerType.Contains(PassengerType.Adult.ToString())).ToList()?.FirstOrDefault()?.PaxSellAmount * AdultPaxCount) ?? 0).ToString("0.00")
                    + " " +
                    post + "(" + AgeGroups.Where(x => x.PassengerType.Contains(PassengerType.Adult.ToString())).ToList()?.FirstOrDefault()?.PaxCount + "x" + resourceManager.GetString("adult", cultureInfo) + ")";

                var checkFamilies = AgeGroups?.Where(x => x.PassengerType.ToLower().Contains(PassengerType.Family.ToString()?.ToLower()))?.ToList()?.FirstOrDefault();

                foreach (var paxInfo in AgeGroups)
                {
                    if (!paxInfo.PassengerType.Contains(PassengerType.Adult.ToString()) && !paxStringwithPrice.Contains(paxInfo.PassengerType))
                    {
                        paxStringwithPrice = paxStringwithPrice + "<br>" + pre + " " + ((paxInfo.PaxSellAmount * paxInfo.PaxCount) ?? 0).ToString("0.00") + " " + post + "(" + paxInfo.PaxCount + "x" + paxInfo.AgeGroupDesc + ")";
                    }
                }


                var paxString = AdultPaxCount == null ? string.Empty : AgeGroups.Where(x => x.PassengerType.Contains(PassengerType.Adult.ToString())).ToList()?.FirstOrDefault()?.PaxCount + " " + resourceManager.GetString("adult", cultureInfo);

                foreach (var paxInfo in AgeGroups)
                {
                    if (!paxInfo.PassengerType.Contains(PassengerType.Adult.ToString()) && !paxString.Contains(paxInfo.PassengerType))
                    {
                        paxString = paxString + "<br>" + paxInfo.PaxCount + " " + paxInfo.AgeGroupDesc;
                    }
                }

                if (Convert.ToDecimal(bookedProductDetail?.DiscountAmount?.Trim() ?? "0.0") > 0)
                {
                    showDiscount = true;
                }

                decimal cancelledAmount = 0;

                if (bookedProductDetail.BookedOptionStatusName == Constant.Cancelled)
                {
                    showCancell = true;
                    cancelledAmount = Convert.ToDecimal(string.IsNullOrEmpty(bookedProductDetail.RefundAmount) ? "0.00" : bookedProductDetail.RefundAmount);
                    totalRefundAmount = totalRefundAmount + cancelledAmount;
                }

                productDetailBuilder.Append($"<tr><td style='font-size: 12pt;color: #333;text-align: left;vertical-align: top;'>{productCount}</td><td style='font-size: 12pt;color: #333;font-weight: bold;text-align: left;vertical-align: top;'>{bookedProductDetail.TsProductName?.Trim()} ({ProductStatus})<br><small style='font-weight: normal;'>{bookedProductDetail.OptionName?.Trim()}</small>");
                productDetailBuilder.Append($"</td><td style='font-size: 12pt;color: #333;text-align: left;vertical-align: top;'><small style='font-weight: normal;'>{paxString}</small></td>");
                productDetailBuilder.Append($"<td style='font-size: 12pt;color: #333;text-align: left;vertical-align: top;'><small style='font-weight: normal;'>{paxStringwithPrice}</small></td>");
                productDetailBuilder.Append($"<td style='font-size: 12pt;color: #333;text-align: left;vertical-align: top;display:##displayDiscount##;'>{pre} {bookedProductDetail.DiscountAmount} {post}</td>");
                productDetailBuilder.Append($"<td style='font-size: 12pt;color: #333;text-align: left;vertical-align: top;display:##displayCancel##;'>{pre} {cancelledAmount.ToString("0.00")} {post}</td></tr>");

                productCount = productCount + 1;
            }

            paymentSummaryBuilder.Replace("##LetSettle##", letSettleUri);
            paymentSummaryBuilder.Replace("##IsangoLogo##", isangoLogoUri);
            //paymentSummaryBuilder.Replace("##GoGif##", goGifUri);
            paymentSummaryBuilder.Replace("##BookingVoucherNumber##", bookingDataOthers.BookingRefNo?.Trim() ?? "N/A");
            paymentSummaryBuilder.Replace("##ProductData##", productDetailBuilder.ToString()?.Trim());
            paymentSummaryBuilder.Replace("##UserName##", LeadpaxName);
            paymentSummaryBuilder.Replace("##MyBookingWebsite##", bookingDataOthers?.CompanyWebSite);
            //paymentSummaryBuilder.Replace("##TotalBookingAmount##", pre + grossSellAmount.ToString("00.00") + post);
            //show hide payment fields on condition

            var amountAfterDiscount = grossSellAmount - discountAmount - multiSaveAmount - totalRefundAmount;
            amountAfterDiscount = amountAfterDiscount < 0 ? 0 : amountAfterDiscount;

            if (!showCancell)
            {
                paymentSummaryBuilder.Replace("##displayCancel##", "none");
            }

            if (!showDiscount)
            {
                paymentSummaryBuilder.Replace("##displayDiscount##", "none");
            }

            if (discountAmount == 0)
            {
                paymentSummaryBuilder.Replace("##displayDiscountVoucher##", "none");
            }
            if (multiSaveAmount == 0)
            {
                paymentSummaryBuilder.Replace("##displayMultisaveDiscount##", "none");
            }
            //if (discountAmount == 0 && multiSaveAmount == 0)
            //{
            //    paymentSummaryBuilder.Replace("##displayAmountAfterDiscount##", "none");
            //}
            var TotalDiscount = discountAmount + multiSaveAmount;
            paymentSummaryBuilder.Replace("##DiscountVoucher##", pre + discountAmount.ToString("00.00") + post);
            paymentSummaryBuilder.Replace("##MultisaveDiscount##", pre + multiSaveAmount.ToString("00.00") + post);
            paymentSummaryBuilder.Replace("##AmountAfterDiscount##", pre + amountAfterDiscount.ToString("00.00") + post);
            paymentSummaryBuilder.Replace("##SavingsAmount##", pre + TotalDiscount.ToString("00.00") + post);
            paymentSummaryBuilder.Replace("##ToBeChargedAmount##", pre + totalToBeChargedAmount.ToString("00.00") + post);

            if (TotalDiscount <= 0)
            {
                paymentSummaryBuilder.Replace("##displaySavingsAmount##", "none");
            }
            if (totalToBeChargedAmount <= 0)
            {
                paymentSummaryBuilder.Replace("##displayToBeChargedAmount##", "none");
            }

            //paymentSummaryBuilder.Replace("##TotalChargedAmount##", pre + totalChargedAmount.ToString("00.00") + post);

            //paymentSummaryBuilder.Replace("##ToBeChargedAmount##", bookingDataOthers.BookedProductDetailList.Any(e =>
            //e.BookedOptionStatusName.Equals("On Request", StringComparison.OrdinalIgnoreCase))
            //#pragma warning disable S3358 // Ternary operators should not be nested
            //? !languageCode.Equals("EN") ? $"<tr><td><b> {resourceManager.GetString("ToBeChargedAmount", cultureInfo)}</b> /</br> <em> To be Charged on Confirmation of Pending Booking Requests</em></td><td class=\"tar\"><b>{pre}{totalToBeChargedAmount:00.00}{post}</b></td></tr>" : $"<tr><td><b> {resourceManager.GetString("ToBeChargedAmount", cultureInfo)} </b></td><td class=\"tar\"><b>{pre}{totalToBeChargedAmount:00.00}{post}</b></td></tr>"
            //#pragma warning restore S3358 // Ternary operators should not be nested
            //: "");

            //paymentSummaryBuilder.Replace("##BookingNumber##", bookingDataOthers?.BookingRefNo?.Trim() ?? "N/A");

            return Tuple.Create(paymentSummaryBuilder.ToString().Replace("???", ""), string.Empty, string.Empty);
        }

        public byte[] GetBookedInvoice(string bookingRefNo, bool isPDFVoucher = false, int Source = 4)
        {
#pragma warning disable S1168 // Empty arrays and collections should be returned instead of null
            if (bookingRefNo == null) { return null; }
#pragma warning restore S1168 // Empty arrays and collections should be returned instead of null

            // Get Booking Details.
            //Note: We are setting isSupplier as false for retrieving data of customers
            var bookingData = _bookingPersistence.GetBookingDataForMail(bookingRefNo, false, Source);

            //Initialize resource manager
            var resourceManager = new ResourceManager(Constant.ResourceManagerBaseName, Assembly.GetExecutingAssembly());
            var cultureInfo = GetCultureInfo(bookingData.LanguageCode?.ToLowerInvariant());

            // Generate the attachment.
            var attachment = GenerateInvoiceForBooking((BookingDataOthers)bookingData, resourceManager, cultureInfo, isPDFVoucher);

            // Convert HTML template to PDF.
            var byteArray = ConvertHtmlToByteArrayNew(attachment?.Item1, resourceManager, cultureInfo, attachment?.Item2, attachment?.Item3, isPDFVoucher);

            return byteArray;
        }

        #region Private Methods

        /// <summary>
        /// Load template
        /// </summary>
        /// <param name="templateName"></param>
        /// <returns></returns>
        private static string LoadTemplate(string templateName)
        {
            //Get file path
            var templatePath = Path.Combine(WebRootPath.GetWebRoot(), Constant.Template, Constant.VoucherTemplateBasePath, $"{templateName}.html");

            if (!File.Exists(templatePath))
                throw new FileNotFoundException($"{Constant.FileNotFound} {templatePath}");

            //Open File Object to deserialize.
            using (var fsInput = File.Open(templatePath, FileMode.Open))
            {
                var fileContents = new byte[Convert.ToInt32(fsInput.Length)];

                //Read File contents into a Byte Array.
                fsInput.Read(fileContents, 0, Convert.ToInt32(fsInput.Length));
                fsInput.Close();

                //Load Byte Array into a Memory Stream Object.
                var oMemoryStream = new MemoryStream(fileContents);
                return Encoding.ASCII.GetString(oMemoryStream.ToArray());
            }
        }

        /// <summary>
        /// Get Template Name
        /// </summary>
        /// <param name="bookedOptionStatusName"></param>
        /// <param name="languageCode"></param>
        /// <returns></returns>
		private string GetTemplateName(string bookedOptionStatusName, string languageCode)
        {
            var templateName = string.Empty;
            switch (bookedOptionStatusName)
            {
                case Constant.Confirmed:
                case Constant.ConfirmedFromAllocation:
                    {
                        templateName = Constant.FreeSaleTemplateName;
                        break;
                    }
                case Constant.OnRequest:
                    {
                        templateName = Constant.OnRequestTemplateName;
                        break;
                    }
                case Constant.Cancelled:
                    {
                        templateName = Constant.CancelTemplateName;
                        break;
                    }
                case Constant.WholeBookingCancelled:
                    {
                        templateName = Constant.WholeBookingCancelledTemplateName;
                        break;
                    }
                default:
                    {
                        break;// throw new Exception("Unexpected Case");
                    }
            }

            templateName = templateName + "_" + languageCode;
            return templateName;
        }

        private string GetTemplateNameNew(string languageCode)
        {
            var templateName = "PDF";
            templateName = templateName + "_" + languageCode + "_V2";
            return templateName;
        }

        /// <summary>
        /// /// Generate QR?barcode Code
        /// </summary>
        /// <param name="qrCodeValue"></param>
        /// <param name="imageName"></param>
        /// <param name="codeType">codeType.ToLower().Contains("code") || codeType.ToLower().Contains("bar") then it vbarcode will be generated</param>
        public bool GenerateQrCode(string qrCodeValue, string imageName, string codeType = "")
        {
            //Get Image Path
            var imagePath = Path.Combine(Constant.root,Constant.QRCodeBasePath, $"{imageName}.png");
            var isBarcode = IsBarcode(codeType);
            try
            {
                if (File.Exists(imagePath)) { File.Delete(imagePath); }
            }
            catch (Exception ex)
            {
                Task.Run(() =>
                   _log.Error(
                                   new Entities.IsangoErrorEntity
                                   {
                                       ClassName = "MailGeneratorService",
                                       MethodName = "GenerateMultipleQRCode",
                                       Params = qrCodeValue
                                   }, ex
                             )
                   );
            }

            if (!isBarcode)
            {
                //Using qrcode-imagesharp as we have to deploy bumblebeecore on linux environment as with another packages it is dependendent on system.drawing
               // which is WindowsAccountType dependent.
                QRCodeData qrCodeData;
                using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
                {
                    qrCodeData = qrGenerator.CreateQrCode(qrCodeValue, QRCodeGenerator.ECCLevel.Q);
                }

                // Image Format
                var imgType = Base64QRCode.ImageType.Png;

                var qrCode = new Base64QRCode(qrCodeData);
                // Base64 Format
                string qrCodeImageAsBase64 = qrCode.GetGraphic(20, SixLabors.ImageSharp.Color.Black, SixLabors.ImageSharp.Color.White, true, imgType);

                // Convert Base64 string to byte array
                byte[] qrCodeImageBytes = Convert.FromBase64String(qrCodeImageAsBase64);

                // Save the image to a file
                File.WriteAllBytes(imagePath, qrCodeImageBytes);

            }
            else
            {
                try
                {
                    //codeType = (codeType == "bar" ? "EAN_13" : "CODE_128");
                    
                    codeType = (codeType?.ToLower() == Constant.IsangoBarcode?.ToLower() || codeType == "bar" ? "EAN_13" : "CODE_128");
                   
                    if (Enum.TryParse(codeType, out BarcodeFormat barcodeFormat))
                    {
                        try
                        {
                            var writer = new BarcodeWriter();
                            writer.Format = barcodeFormat;
                            var barcodeBitmap = writer.Write(qrCodeValue);
                            using (var skImage = SKImage.FromBitmap(barcodeBitmap))
                            {
                                using (var skData = skImage.Encode(SKEncodedImageFormat.Png, 80))
                                {
                                    using (var fileStream = File.OpenWrite(imagePath))
                                    {
                                        skData.SaveTo(fileStream);
                                    }
                                }
                            }

                            isBarcode = true;
                        }
                        catch
                        {
                            barcodeFormat = BarcodeFormat.CODE_128;
                            var writer = new BarcodeWriter();
                            writer.Format = barcodeFormat;

                            var barcodeBitmap = writer.Write(qrCodeValue);

                            using (var skImage = SKImage.FromBitmap(barcodeBitmap))
                            {
                                using (var skData = skImage.Encode(SKEncodedImageFormat.Png, 80))
                                {
                                    using (var fileStream = File.OpenWrite(imagePath))
                                    {
                                        skData.SaveTo(fileStream);
                                    }
                                }
                            }

                            isBarcode = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Task.Run(() =>
                    _log.Error(
                                new Entities.IsangoErrorEntity
                                {
                                    ClassName = "MailGeneratorService",
                                    MethodName = "GenerateMultipleQRCode",
                                    Params = qrCodeValue
                                }, ex
                              )
                    );
                }
            }

            return isBarcode;
        }

        public bool IsBarcode(string codeType)
        {
            var isBarcode = false;

            if (string.IsNullOrEmpty(codeType) || codeType.ToLower().Contains("qr"))
            {
                isBarcode = false;
            }
            else if (codeType?.ToLower()?.Contains("code") == true || codeType?.ToLower()?.Contains("bar") == true)
            {
                try
                {
                    // Use Code128 scheme to generate barcode if it's of Code128 type or Barcode Type
                    //codeType = (codeType == "bar" ? "EAN_13" : "CODE_128");
                    codeType = (codeType?.ToLower() == Constant.IsangoBarcode?.ToLower() || codeType?.ToLower() == "bar" ? "EAN_13" : "CODE_128");
                    if (Enum.TryParse(codeType, out BarcodeFormat barcodeFormat))
                    {
                        // Adjust based on supported barcode formats in ZXing.Net
                        if (barcodeFormat == BarcodeFormat.CODE_128 || barcodeFormat == BarcodeFormat.EAN_13)
                        {
                            isBarcode = true;
                        }
                    }
                    else
                    {
                        // Handle the case when the barcode format is not recognized
                    }
                }
                catch (Exception ex)
                {
                    Task.Run(() =>
                        _log.Error(
                            new Entities.IsangoErrorEntity
                            {
                                ClassName = "MailGeneratorService",
                                MethodName = "IsBarcode",
                                Params = codeType
                            }, ex
                        )
                    );
                }
            }

            return isBarcode;
        }



        public string FilterIllegalCharacterFromPath(string filename)
        {
            var result = filename;
            var invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            invalid = invalid + "!" + "|" + "+";
            foreach (char c in invalid)
            {
                result = result.Replace(c.ToString(), string.Empty);
            }

            return result;
        }

        private byte[] ConvertHtmlToByteArray(string htmlString, ResourceManager resourceManager, CultureInfo cultureInfo, string header, string footer, bool isPDFVoucher = false)
        {
#pragma warning disable S1168 // Empty arrays and collections should be returned instead of null
            if (string.IsNullOrEmpty(htmlString)) { return null; }
#pragma warning restore S1168 // Empty arrays and collections should be returned instead of null

            //Get Pdf path

            var isangoLogoData = File.ReadAllBytes(letsettleFilePath);
            var isangoLogoUri = @"data:image/png;base64," + Convert.ToBase64String(isangoLogoData);

            if (!isPDFVoucher)
            {
                var bytes = Encoding.UTF8.GetBytes(htmlString);
                return bytes;
            }
            else
            {
                try
                {
                    var pdfFilePath = $"{Path.Combine(AppDomain.CurrentDomain.BaseDirectory)}{Guid.NewGuid()}.pdf";
                    try
                    {
                        if (File.Exists(pdfFilePath)) { File.Delete(pdfFilePath); }
                    }
                    catch (Exception)
                    {
                    }

                    PDFreactor pdfReactor = new PDFreactor(string.IsNullOrEmpty(ConfigurationManagerHelper.GetValuefromAppSettings("PDFReactorURL")) ? "http://192.168.0.228:9423/service/rest" : ConfigurationManagerHelper.GetValuefromAppSettings("PDFReactorURL"))
                    {
                        Timeout = 0
                    };

                    var config = new RealObjects.PDFreactor.Webservice.Client.Configuration
                    {
                        LicenseKey = "<license><licensee><name>Technical-contact@isango.com</name></licensee><product>PDFreactor</product><licensetype>Personal</licensetype><purchasedate>2020-11-05</purchasedate><options><option>pdf</option></options><signatureinformation><signdate>2020-11-05 05:16</signdate><signature>302c02140d096d4ca982620c199c9db47ac65526995cc49e02143110341083ffc9d8c18b484cb49f5458e21c251f</signature><checksum>1490</checksum></signatureinformation></license>",
                        Document = htmlString
                    };

                    //To Enable Hyperlinks in Voucher
                    config.AddLinks = true;
                    config.UserStyleSheets.Add(new Resource
                    {
                        Content = "footer {position: running(footer);}"
                        + "@page {@bottom-center {content: element(footer);}}"
                        + "@page {margin: 20mm 5mm 20mm 5mm;}"
                        + "header {position: running(header);}"
                        + "@page {@top-center {content: element(header);}}"
                    });
                    Result data = pdfReactor.Convert(config);
                    //BinaryWriter binWriter = new BinaryWriter(new FileStream("test.pdf",
                    //    FileMode.Create,
                    //    FileAccess.Write));
                    //binWriter.Write(data.Document);
                    //binWriter.Close();

                    //Delete created PDF file
                    try
                    {
                        File.Delete(pdfFilePath);
                    }
                    catch (Exception)
                    {
                        //ignored
                    }
                    return data.Document;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        private byte[] ConvertHtmlToByteArrayNew(string htmlString, ResourceManager resourceManager, CultureInfo cultureInfo, string header, string footer, bool isPDFVoucher = false, bool isShowSupplierVoucher = false, BookingDataOthers bookingDataOthers = null, bool? iscancelled = false)
        {
#pragma warning disable S1168 // Empty arrays and collections should be returned instead of null
            if (string.IsNullOrEmpty(htmlString) && bookingDataOthers == null) { return null; }
#pragma warning restore S1168 // Empty arrays and collections should be returned instead of null

            var isangoLogoData = File.ReadAllBytes(isangoFilePath);
            
            var isangoLogoUri = @"data:image/png;base64," + Convert.ToBase64String(isangoLogoData);

            if (!isPDFVoucher)
            {
                var bytes = Encoding.UTF8.GetBytes(htmlString);
                return bytes;
            }
            else
            {
                try
                {
                    var pdfFilePath = $"{Path.Combine(WebRootPath.GetWebRoot())}{Guid.NewGuid()}.pdf";
                    try
                    {
                        if (File.Exists(pdfFilePath)) { File.Delete(pdfFilePath); }
                    }
                    catch (Exception)
                    {
                    }

                    PDFreactor pdfReactor = new PDFreactor(string.IsNullOrEmpty(ConfigurationManagerHelper.GetValuefromAppSettings("PDFReactorURL")) ? "http://192.168.0.228:9423/service/rest" : ConfigurationManagerHelper.GetValuefromAppSettings("PDFReactorURL"))
                    {
                        Timeout = 0
                    };

                    var config = new RealObjects.PDFreactor.Webservice.Client.Configuration
                    {
                        LicenseKey = "<license><licensee><name>Technical-contact@isango.com</name></licensee><product>PDFreactor</product><licensetype>Personal</licensetype><purchasedate>2020-11-05</purchasedate><options><option>pdf</option></options><signatureinformation><signdate>2020-11-05 05:16</signdate><signature>302c02140d096d4ca982620c199c9db47ac65526995cc49e02143110341083ffc9d8c18b484cb49f5458e21c251f</signature><checksum>1490</checksum></signatureinformation></license>",
                        Document = htmlString
                    };

                    if (isShowSupplierVoucher && bookingDataOthers != null)
                    {
                        config.Document = "<html>";

                        config.MergeMode = MergeMode.OVERLAY;

                        if ((bookingDataOthers?.BookedProductDetailList?.FirstOrDefault()?.IsQRCodePerPax ?? false) && bookingDataOthers?.BarCodeList != null)
                        {
                            config.MergeDocuments = new List<Resource>();
                            foreach (var barCode in bookingDataOthers?.BarCodeList)
                            {
                                config.MergeDocuments.Add(new Resource { Uri = string.IsNullOrEmpty(barCode.CodeValue) ? barCode.BarCode : barCode.CodeValue });
                            }
                        }
                        else
                        {
                            config.MergeDocuments = new List<Resource> { new Resource { Uri = bookingDataOthers?.BookedProductDetailList?.FirstOrDefault()?.QrCode } };
                        }
                    }
                    //else if(bookingDataOthers != null)
                    //{
                    //    config.Document = "<html>";
                    //    config.MergeDocuments = new List<Resource>();
                    //    foreach (var product in bookingDataOthers?.BookedProductDetailList)
                    //    {
                    //        config.MergeDocuments.Add(new Resource { Uri = $"{ConfigurationManagerHelper.GetValuefromAppSettings("WebAPIBaseUrl")}/api/voucher/book/{bookingDataOthers?.BookingRefNo}?source=3&bookedoptionid={product.BookedOptionId}" }); //&bookedoptionid={product.BookedOptionId}
                    //    }
                    //}

                    //To Enable Hyperlinks in Voucher
                    config.AddLinks = true;
                    //debugging off
                    //config.DebugSettings = new DebugSettings() { All= true};
                    config.UserStyleSheets.Add(new Resource
                    {
                        Content = "footer {position: running(footer);}"
                        + "@page {@bottom-center {content: element(footer);}}"
                        + "@page {margin: 15mm 5mm 25mm 5mm;}"
                        + "header {position: running(header);}"
                        + "@page {@top-center {content: element(header);}}"
                    });

                    if (iscancelled ?? false)
                    {
                        config.UserStyleSheets.Add(new Resource
                        {
                            Content = "@page {@top-left-corner {content: 'Cancelled'; z-index: 1;font-family: 'Poppins', sans-serif;font-size: 80pt;"
                        + "font-weight: bold; color: rgb(243, 101, 101); text-align: center; text-transform: uppercase; transform: translateY(50-ro-ph) translateY(-50%) translateX(50-ro-pw) translateX(-50%) rotate(-54.7deg);"
                        + "transform-origin: center center;position: absolute; left: 0; top: 0; width: 100%; height: 100%; }"
                        });
                    }

                    //Result data = pdfReactor.Convert(config);
                    //BinaryWriter binWriter = new BinaryWriter(new FileStream("test.pdf",
                    //    FileMode.Create,
                    //    FileAccess.Write));
                    //binWriter.Write(data.Document);
                    //binWriter.Close();

                    var task = Task.Run(() => pdfReactor.Convert(config));
                    if (task.Wait(TimeSpan.FromSeconds(30)))
                    {
                        //Delete created PDF file
                        try
                        {
                            Result data = task.Result;
                            File.Delete(pdfFilePath);
                            return data.Document;
                        }
                        catch (Exception)
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }
                    //Result data = pdfReactor.Convert(config);
                    //BinaryWriter binWriter = new BinaryWriter(new FileStream("test.pdf",
                    //    FileMode.Create,
                    //    FileAccess.Write));
                    //binWriter.Write(data.Document);
                    //binWriter.Close();
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        //        /// <summary>
        //        /// Covert HTML to PDF
        //        /// </summary>
        //        /// <param name="htmlString"></param>
        //        /// <param name="resourceManager"></param>
        //        /// <param name="cultureInfo"></param>
        //        /// <returns></returns>
        //        private byte[] ConvertHtmlToByteArray(string htmlString, ResourceManager resourceManager, CultureInfo cultureInfo, string header, string footer)
        //        {
        //#pragma warning disable S1168 // Empty arrays and collections should be returned instead of null
        //            if (string.IsNullOrEmpty(htmlString)) { return null; }
        //#pragma warning restore S1168 // Empty arrays and collections should be returned instead of null

        //            //Get Pdf path

        //            var isangoLogoData = File.ReadAllBytes($"{Path.Combine(AppDomain.CurrentDomain.BaseDirectory)}{Constant.IsangoLogo}");
        //            var isangoLogoUri = @"data:image/png;base64," + Convert.ToBase64String(isangoLogoData);

        //            if (ConfigurationManagerHelper.GetValuefromAppSettings("VoucherPDF") == "0")
        //            {
        //                var bytes = Encoding.UTF8.GetBytes(htmlString);
        //                return bytes;
        //            }
        //            else
        //            {
        //                var pdfFilePath = $"{Path.Combine(AppDomain.CurrentDomain.BaseDirectory)}{Guid.NewGuid()}.pdf";
        //                try
        //                {
        //                    if (File.Exists(pdfFilePath)) { File.Delete(pdfFilePath); }
        //                }
        //                catch (Exception)
        //                {
        //                }

        //                var renderer = new HtmlToPdf
        //                {
        //                    PrintOptions =
        //                        {
        //                            //Set header of pdf
        //                             Header=new HtmlHeaderFooter()
        //                            {
        //                                Height = 25,
        //                                HtmlFragment = header,
        //                            },

        //                            //Set footer of pdf
        //                            Footer = new HtmlHeaderFooter()
        //                            {
        //                                Height = 15,
        //                                HtmlFragment = footer,
        //                            },
        //                            MarginLeft=12,
        //                            MarginRight=12,
        //                            MarginTop=0,
        //                            DPI=300,
        //                            PrintHtmlBackgrounds=true,
        //                        }
        //                };
        //                var pdf = renderer.RenderHtmlAsPdf(htmlString);

        //                var goGifData = File.ReadAllBytes($"{Path.Combine(AppDomain.CurrentDomain.BaseDirectory)}Images\\go_png.png");
        //                var goGifUri = @"data:image/png;base64," + Convert.ToBase64String(goGifData);

        //                HtmlStamp htmlStamp = new HtmlStamp();
        //                htmlStamp.Html = "<img src=" + goGifUri + " style='position:relative;z-index:1;' width='255' height='442' alt=''>";
        //                htmlStamp.Opacity = 100;
        //                htmlStamp.Bottom = 0;
        //                htmlStamp.Right = 0;
        //                htmlStamp.Rotation = 0;
        //                htmlStamp.ZIndex = 0;

        //                pdf.StampHTML(htmlStamp);
        //                pdf.SaveAs(pdfFilePath);
        //                //Read PDF file and convert it into Base64
        //                var bytes = File.ReadAllBytes(pdfFilePath);

        //                //Delete created PDF file
        //                try
        //                {
        //                    File.Delete(pdfFilePath);
        //                }
        //                catch (Exception)
        //                {
        //                    //ignored
        //                }
        //                return bytes;
        //            }
        //        }

        /// <summary>
        /// Replace placeholders with their values for cancelled product voucher
        /// </summary>
        /// <param name="bookingDataOthers"></param>
        /// <param name="bookingOptionId"></param>
        /// <returns></returns>
        private string GeneratePdfForCancelledBooking(BookingDataOthers bookingDataOthers, string bookingOptionId)
        {
            var languageCode = bookingDataOthers?.LanguageCode?.ToUpperInvariant();
            PrePostCurrency(bookingDataOthers, out var pre, out var post);

            var bookedProductDetail = bookingDataOthers?.BookedProductDetailList?.FirstOrDefault(x => x.BookedOptionId.Equals(bookingOptionId));

            if (bookedProductDetail == null) { return null; }

            var templateName = GetTemplateName(bookedProductDetail.BookedOptionStatusName, languageCode);
            var productDetailText = LoadTemplate($"{templateName}");
            var attachmentDetails = new StringBuilder(productDetailText);

            #region Replace the placeholders with values

            var supplierDetails = bookingDataOthers.SupplierDataList?.FirstOrDefault(x => x.BookedOptionId == bookedProductDetail.BookedOptionId);

            attachmentDetails.Replace("##UserName##", bookedProductDetail.LeadPassengerName ?? "N/A");
            attachmentDetails.Replace("##BookingReferenceNumber##", bookingDataOthers.BookingRefNo ?? "N/A");
            attachmentDetails.Replace("##NumberOfAdults##", string.IsNullOrEmpty(bookedProductDetail.NoOfAdults) ? "0" : bookedProductDetail.NoOfAdults);
            attachmentDetails.Replace("##NumberOfChildren##", string.IsNullOrEmpty(bookedProductDetail.ChildCount) ? "None" : $"{bookedProductDetail.ChildCount} Child");
            attachmentDetails.Replace("##OptionName##", bookedProductDetail.OptionName ?? "N/A");
            attachmentDetails.Replace("##DateOfTour##", bookedProductDetail.FromDate.ToString("dd MMM yyyy"));
            var optionCode = bookedProductDetail?.SupplierOptionCode?.Trim()?.Split('~')?.FirstOrDefault() ?? "N/A";
            attachmentDetails.Replace("##OptionCode##", optionCode);

            if (!string.IsNullOrWhiteSpace(bookedProductDetail?.VatNumber))
            {
                attachmentDetails.Replace("##VATNumber##", bookedProductDetail?.VatNumber);
            }
            else
            {
                attachmentDetails.Replace("##displayVATNumber##", "none");
            }
            var affiliateId = bookingDataOthers.AffiliateId.ToUpperInvariant();
            attachmentDetails.Replace("##SpecialRequest##", string.IsNullOrEmpty(bookedProductDetail.SpecialRequest) ? "N/A" : bookedProductDetail.SpecialRequest);
            attachmentDetails.Replace("##ContractComment##", string.IsNullOrEmpty(bookedProductDetail.ContractComment) ? "N/A" : bookedProductDetail.ContractComment);
            if (affiliateId.Equals("413DE577-1630-42C0-82E8-F298187CB984") || affiliateId.Equals("4A1B7E59-8FEA-442A-AF56-D5C1C16D0826") || affiliateId.Equals("7E8277D2-EF03-492A-A882-8199272828F9") || affiliateId.Equals("1A23137F-81B9-4E93-8391-19C2662848DB") || affiliateId.Equals("405F5611-1387-4E19-9840-03A4671BB6A5") || affiliateId.Equals("A80BEC17-148B-46C3-B157-01AA0ECAD983") || affiliateId.Equals("D78748D4-70E3-487E-A6BA-3C5D4BD842BD") || affiliateId.Equals("F7FEEE76-7B67-4886-90AB-07488CB7A167") || affiliateId.Equals("5BEEF089-3E4E-4F0F-9FBF-99BF1F350183") || affiliateId.Equals("E1DFF808-13D0-4BEB-80EA-1247E38CFE9C") || affiliateId.Equals("C0E714F0-2864-40BC-BD82-63D2198384C1") || affiliateId.Equals("A3300403-CE50-4193-8AB0-70562B424ABE") || affiliateId.Equals("85BB0CD2-7CF2-4674-82A7-AE645AA3E974") || affiliateId.Equals("BF24CBCE-2E87-409B-BDA8-F717D16A8261") || affiliateId.Equals("9A082700-E9D1-449C-99A4-F62A561DA929"))
            {
                attachmentDetails.Replace("##ProductWithProductPageLink##", $"{bookingDataOthers.CompanyWebSite}{Constant.ActivityDetailUrl}{bookedProductDetail.ServiceId}");

                attachmentDetails.Replace("##ProductPageName##", bookedProductDetail.ServiceName ?? "N/A");
            }
            else
            {
                attachmentDetails.Replace("##ProductWithProductPageLink##", $"{bookingDataOthers.CompanyWebSite}/product/{bookedProductDetail.ServiceId}?affiliateid={bookingDataOthers.AffiliateId}&Language=EN");

                attachmentDetails.Replace("##ProductPageName##", bookedProductDetail.ServiceName ?? "N/A");
            }

            attachmentDetails.Replace("##TotalTourPrice##", pre + Convert.ToDouble(bookedProductDetail.AmountOnWirecard).ToString("00.00") + post);
            attachmentDetails.Replace("##CancellationCharges##", pre + Convert.ToDouble(bookingDataOthers.CancellationPricesList?.FirstOrDefault()?.CancellationAmount).ToString("00.00") + post);
            attachmentDetails.Replace("##NetAmountToBeRefund##", pre + Convert.ToDouble(bookedProductDetail.RefundAmount).ToString("00.00") + post);

            attachmentDetails.Replace("##Duration##", string.IsNullOrEmpty(bookedProductDetail.Duration) ? "N/A" : bookedProductDetail.Duration.Replace("<br />", "\n").Replace("<br>", "\n").Replace("<br/>", "\n").Replace("<b>", "").Replace("</b>", ""));

            attachmentDetails.Replace("##Schedule##", string.IsNullOrEmpty(bookedProductDetail.Schedule) ? "N/A" : bookedProductDetail.Schedule.Replace("<br />", "\n").Replace("<br>", "\n").Replace("<br/>", "\n").Replace("<b>", "").Replace("</b>", ""));

            var goGifData = File.ReadAllBytes($"{Path.Combine(WebRootPath.GetWebRootPath())}Images\\go_png.png");
            var goGifUri = @"data:image/png;base64," + Convert.ToBase64String(goGifData);
            attachmentDetails.Replace("##GoGif##", goGifUri);

            var isangoLogoData = File.ReadAllBytes(isangoFilePath);
            var isangoLogoUri = @"data:image/png;base64," + Convert.ToBase64String(isangoLogoData);
            attachmentDetails.Replace("##IsangoLogo##", isangoLogoUri);

            attachmentDetails.Replace("##CustomerSupportNumbers##", string.IsNullOrEmpty(bookingDataOthers.CustomerServiceNo) ? "N/A" : bookingDataOthers.CustomerServiceNo);
            attachmentDetails.Replace("##SupportEmail##", bookingDataOthers?.CompanyEmail ?? "N/A");
            attachmentDetails.Replace("???", "");

            #endregion Replace the placeholders with values

            return attachmentDetails.ToString();
        }

        /// <summary>
        /// Replace place holders with their values for FreeSale Voucher
        /// </summary>
        /// <param name="bookingDataOthers"></param>
        /// <param name="resourceManager"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        private Tuple<string, string, string> GeneratePdfForBooking(BookingDataOthers bookingDataOthers, ResourceManager resourceManager, CultureInfo cultureInfo, bool isPDFVoucher = false)
        {
            var attachmentBody = new StringBuilder();
            var productDetailBuilder = new StringBuilder();
            var productCount = 1;
            var wireCardAmount = 0.0M;
            var discountAmount = 0.0M;
            var multiSaveAmount = 0.0M;
            var grossSellAmount = 0.0M;
            var totalToBeChargedAmount = 0.0M;
            var languageCode = bookingDataOthers.LanguageCode?.ToUpperInvariant();
            var paymentSummaryBuilder = new StringBuilder();
            //var bigBusAttachment = new StringBuilder();

            var bookedProductDetailList = bookingDataOthers.BookedProductDetailList;

            var bookedProductCount = bookedProductDetailList?.Count(x => x.BookedOptionStatusName != "Failed");

            if (bookedProductDetailList == null || bookedProductCount == 0) { return null; }

            PrePostCurrency(bookingDataOthers, out var pre, out var post);

            var letSettleData = File.ReadAllBytes(letsettleFilePath);
            var letSettleUri = @"data:image/png;base64," + Convert.ToBase64String(letSettleData);

            var goGifData = File.ReadAllBytes(goGiffilePath);
            var goGifUri = @"data:image/png;base64," + Convert.ToBase64String(goGifData);

            var isangoLogoData = File.ReadAllBytes(isangoFilePath);
            var isangoLogoUri = @"data:image/png;base64," + Convert.ToBase64String(isangoLogoData);

            //Remove Products that are Cancelled
            if (bookedProductDetailList != null && bookedProductDetailList.Count > 0)
            {
                bookedProductDetailList = bookedProductDetailList.Where(x => x.BookedOptionStatusName != Constant.Cancelled).ToList();
            }

            //If all products are Cancel in the booking
            if (bookedProductDetailList == null || bookedProductDetailList.Count == 0)
            {
                var templateName = GetTemplateName(Constant.WholeBookingCancelled, languageCode);
                var productDetailText = LoadTemplate($"{templateName}");
                var attachmentDetails = new StringBuilder(productDetailText);
                attachmentDetails.Replace("##BookingVoucherNumber##", bookingDataOthers.BookingRefNo?.Trim() ?? "N/A");
                attachmentDetails.Replace("##BaseURL##", ConfigurationManagerHelper.GetValuefromAppSettings(Constant.WebAPIBaseUrl));
                string htmlHead = string.Empty;
                string htmlFoot = string.Empty;
                if (!isPDFVoucher)
                {
                    htmlHead = $" <div style='text-align:left;padding-bottom: 7.5pt;margin:22.5 2.25pt 0pt'><a target='_blank' href=\"https://www.isango.com/\"><img width=\"150\" alt='' src=\"{isangoLogoUri}\"></a></div>";
                    htmlFoot = $"<div style='text-align:center;padding-top: 7.5pt;margin:0 2.25pt'><span style='font-size:6.75pt;color:#333'>{resourceManager.GetString("FooterText", cultureInfo)}!</span></center><center><a target=\"_blank\" href={Constant.FooterUrl}><img width=\"70\" style=\"margin:0;padding:0;display:block;\" src=\"{isangoLogoUri}\"></a></div>";

                    attachmentDetails.Replace("##HeaderSet##", htmlHead);
                    attachmentDetails.Replace("##FooterSet##", htmlFoot);
                }
                else
                {
                    attachmentDetails.Replace("##HeaderSet##", string.Empty);
                    attachmentDetails.Replace("##FooterSet##", string.Empty);
                }
                attachmentBody.Append(attachmentDetails);
                attachmentBody.Replace("???", "");
                return Tuple.Create(attachmentBody.ToString(), htmlHead, htmlFoot);
            }
            //Big Bus Voucher is removed - Code Commented

            #region Big bus Voucher

            //var bigBusProducts = bookedProductDetailList.Where(x => x.ApiTypeId == Convert.ToInt32(APIType.BigBus).ToString()).ToList();
            //if (bigBusProducts.Count > 0)
            //{
            //    bigBusAttachment = GetBigBusVoucher(bigBusProducts, bookingDataOthers);
            //}

            //if (bigBusProducts.Count < bookedProductDetailList.Count)
            //{
            //    var paymentSummaryText = LoadTemplate($"{Constant.PaymentTemplate}{languageCode}");
            //    paymentSummaryBuilder.Append(paymentSummaryText);
            //}

            #endregion Big bus Voucher

            var paymentSummaryText = LoadTemplate($"{Constant.PaymentTemplate}{languageCode}");
            paymentSummaryBuilder.Append(paymentSummaryText);

            foreach (var bookedProductDetail in bookedProductDetailList)
            {
                if (bookedProductDetail.BookedOptionStatusName == Constant.Failed || bookedProductDetail.IsShowSupplierVoucher)
                    continue;

                //if (bookedProductDetail.ApiTypeId != ((int)APIType.BigBus).ToString())
                //if (bigBusProducts.Count < bookedProductDetailList.Count)
                //{
                if (isPDFVoucher && bookedProductDetailList.IndexOf(bookedProductDetail) > 0)
                {
                    attachmentBody.Append("<div style='clear: both;page-break-before: auto;'></div>");
                }
                var templateName = GetTemplateName(bookedProductDetail.BookedOptionStatusName, languageCode);
                var productDetailText = LoadTemplate($"{templateName}");

                var attachmentDetails = new StringBuilder(productDetailText);
                var supplierDetails = bookingDataOthers.SupplierDataList?.FirstOrDefault(x => x.BookedOptionId == bookedProductDetail.BookedOptionId);
                var endSupplierDetails = bookingDataOthers.SupplierOrHotelAddressList?.FirstOrDefault(x => x.BookedOptionId == bookedProductDetail.BookedOptionId);

                if (languageCode.Equals(Constant.DE.ToUpperInvariant()))
                {
                   var parisTourLogoFilePath = Path.Combine(WebRootPath.GetWebRootPath(), Constant.Image, Constant.ParisTourLogo);
                    var parisTourLogoData = File.ReadAllBytes(parisTourLogoFilePath);
                    var parisTourLogoUri = @"data:image/png;base64," + Convert.ToBase64String(parisTourLogoData);
                    attachmentDetails.Replace("##ParisTourLogo##", parisTourLogoUri);
                }

                #region Only for Free Sale Products

                var isQrPerPaxApplicable = bookedProductDetail.IsQRCodePerPax;

               
                if (!string.IsNullOrEmpty(bookedProductDetail?.QrCode)
                   && bookedProductDetail?.QRCodeType?.ToUpper() != Constant.IsangoLink
                   )
                {
                    //Generate QR Code
                    var codes = bookedProductDetail?.QrCode?.Split('~');
                    if (codes?.Length > 1)
                    {
                        var codeType = string.IsNullOrWhiteSpace(codes[0]) ? string.Empty : codes[0].ToLower();
                        var groupCodes = string.IsNullOrWhiteSpace(codes[1]) ? string.Empty : codes[1];
                        /*
                       var generatedQRCode = string.Empty;

                       if (codeType.Contains("qr") && !string.IsNullOrWhiteSpace(groupCodes))
                       {
                           generatedQRCode = GenerateMultipleQRCode(groupCodes, bookingDataOthers.BookingRefNo?.Trim());
                           attachmentDetails.Replace("##QRCodeTable##", generatedQRCode);
                       }
                       */

                        var qrcodes = new List<BarCodeData>();
                        var qdata = new BarCodeData
                        {
                            BarCode = groupCodes,
                            IsResourceApply = false,
                            BookedOptionId = bookedProductDetail.BookedOptionId,
                            CodeType = codeType,
                            CodeValue = groupCodes,
                        };
                        qrcodes.Add(qdata);
                        var generatedQRCode = GenerateMultipleQRCodePerPaxType(qrcodes, bookingDataOthers.BookingRefNo?.Trim());
                        attachmentDetails.Replace("##QRCodeTable##", generatedQRCode);
                    }
                    else
                    {
                        bookedProductDetail.QrCode = bookedProductDetail?.QrCode?.Split('~')?.LastOrDefault() ?? bookedProductDetail?.QrCode;
                        var isbarcode = GenerateQrCode(bookedProductDetail.QrCode, $"{bookingDataOthers.BookingRefNo?.Trim()}{productCount}");

                        if (!isbarcode)
                        {
                            attachmentDetails.Replace("##QRCode##", $"<img src=\"{ConfigurationManagerHelper.GetValuefromAppSettings(Constant.WebAPIBaseUrl)}/QRCodes/{bookingDataOthers.BookingRefNo?.Trim()}{productCount}.png\" width=\"100%\" style=\"max-height: 100px;margin:auto;\">");
                            attachmentDetails.Replace("##QRCodeValue##", bookedProductDetail?.QrCode);
                        }

                        var qrcodes = new List<BarCodeData>();
                        var qdata = new BarCodeData
                        {
                            BarCode = bookedProductDetail.QrCode,
                            IsResourceApply = false,
                            BookedOptionId = bookedProductDetail.BookedOptionId
                        };
                        qrcodes.Add(qdata);
                        var generatedQRCode = GenerateMultipleQRCodePerPaxType(qrcodes, bookingDataOthers.BookingRefNo?.Trim());
                        attachmentDetails.Replace("##QRCodeTable##", generatedQRCode);
                    }
                }
                //condition for multiple qr code for per pax type for redeam, not for other products
                else if (bookingDataOthers?.BarCodeList != null
                && isQrPerPaxApplicable
                )
                {
                    var barCodeList = bookingDataOthers.BarCodeList.Where(x => x.BookedOptionId == bookedProductDetail.BookedOptionId).ToList();
                    var generatedQRCode = GenerateMultipleQRCodePerPaxType(barCodeList, bookingDataOthers.BookingRefNo?.Trim());
                    attachmentDetails.Replace("##QRCodeTable##", generatedQRCode);
                }
                //condition for multiple qr code for per passenger type for Redeam, not for other products
                //Commented during merge ##TODO Check if upper block is able to handle it or not
                //else if (bookingDataOthers?.BarCodeList != null && bookedProductDetail?.QRCodeType?.ToUpper() != "LINK")
                //{
                //    var barCodeList = bookingDataOthers.BarCodeList.Where(x => x.BookedOptionId == bookedProductDetail.BookedOptionId).ToList();
                //    var generatedQRCode = GenerateMultipleQRCodePerPaxType(barCodeList, bookingDataOthers.BookingRefNo?.Trim());
                //    attachmentDetails.Replace("##QRCodeTable##", generatedQRCode);
                //}
                else
                {
                    attachmentDetails.Replace("##QRCodeTable##", string.Empty);
                    attachmentDetails.Replace("##QRCode##", string.Empty);
                    attachmentDetails.Replace("##displayQR##", "none");
                    attachmentDetails.Replace("##displayForNoQR##", "display:flex");
                    attachmentDetails.Replace("##w50##", "width: 49%;margin-right: 1%");
                    attachmentDetails.Replace("##leftBorder##", "border-left: 1px dashed #CCC;");
                    attachmentDetails.Replace("##paddingIfQr##", "padding-left:15pt;");
                }

                if (!string.IsNullOrEmpty(bookedProductDetail?.FileNumber))
                {
                    var supplierRefNumber = bookedProductDetail?.FileNumber;
                    if (!string.IsNullOrEmpty(bookedProductDetail?.OfficeCode)
                        && bookedProductDetail?.ApiTypeId == "3")
                    {
                        var supplierLineNumber = string.Empty;

                        try
                        {
                            var supplierLineNumberSplit = bookedProductDetail?.SupplierLineNumber?.Split('#');
                            var supplierOptionCode1stChar = bookedProductDetail?
                                                            .SupplierOptionCode?
                                                            .Split('-')?
                                                            .FirstOrDefault();
                            if (supplierLineNumberSplit?.Length >= 3)
                            {
                                supplierLineNumber = $"{supplierLineNumberSplit[1]}{supplierOptionCode1stChar}{supplierLineNumberSplit[2]}";
                            }
                        }
                        catch (Exception ex)
                        {
                            Task.Run(() =>
                              _log.Error(
                                              new Entities.IsangoErrorEntity
                                              {
                                                  ClassName = @"MailAttachementService",
                                                  MethodName = @"GeneratePdfForBooking",
                                                  Params = bookedProductDetail?.SupplierLineNumber
                                              }, ex
                                        )
                              );
                        }

                        if (!string.IsNullOrWhiteSpace(bookedProductDetail.OfficeCode))
                        {
                            if (supplierRefNumber?.Split('-')?.FirstOrDefault() != bookedProductDetail?.OfficeCode)
                            {
                                supplierRefNumber = $"{bookedProductDetail.OfficeCode}-{supplierRefNumber}";
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(supplierLineNumber))
                        {
                            supplierRefNumber = $"{supplierRefNumber}-{supplierLineNumber}";
                        }
                    }
                    attachmentDetails.Replace("##SupplierBookingReference##", supplierRefNumber);
                }
                else
                {
                    attachmentDetails.Replace("##displaySupplierBookingReference##", "none");
                }

                #endregion Only for Free Sale Products

                attachmentDetails.Replace("##UserName##", bookedProductDetail.LeadPassengerName?.Trim() ?? "N/A");
                attachmentDetails.Replace("##SupplierName##", supplierDetails?.SupplierName?.Trim() ?? "N/A");
                attachmentDetails.Replace("##BookingRequestNumber##", bookingDataOthers.BookingRefNo?.Trim() ?? "N/A");

                var yrsText = resourceManager.GetString("yrs", cultureInfo);
                var noOf = resourceManager.GetString("noOf", cultureInfo);

                //OLD Voucher Functionality
                if (bookingDataOthers.BookingAgeGroupList == null || bookingDataOthers.BookingAgeGroupList.Count == 0)
                {
                    var filterCustomerDataByOptionId = bookingDataOthers.Customers.Where(x => x.BookedOptionId == bookedProductDetail.BookedOptionId).ToList();
                    var ChildAges = String.Empty;
                    if (filterCustomerDataByOptionId != null)
                    {
                        foreach (var item in filterCustomerDataByOptionId)
                        {
                            if (item.CustomerType == "True" && Convert.ToInt32(item.Age) > 0)
                            {
                                ChildAges += item.Age + yrsText + ", ";
                            }
                        }
                        if (!string.IsNullOrEmpty(ChildAges))
                        {
                            ChildAges = ChildAges.Substring(0, ChildAges.Length - 2);
                            ChildAges = " (" + ChildAges + ")";
                        }
                    }
                    attachmentDetails.Replace("##DynamicAgeGroup##", "");
                    attachmentDetails.Replace("##NumberOfAdults##", string.IsNullOrEmpty(bookedProductDetail.NoOfAdults) ? "None" : bookedProductDetail.NoOfAdults?.Trim());
                    attachmentDetails.Replace("##NumberOfChildren##", string.IsNullOrEmpty(bookedProductDetail.ChildCount) ? "None" : $"{bookedProductDetail.ChildCount?.Trim()} Child" + ChildAges);
                }
                //New Voucher Functionality
                else
                {
                    attachmentDetails.Replace("##displayNoofAdults##", "none");
                    attachmentDetails.Replace("##displayNoOfChilds##", "none");

                    var filterDataByOptionId = bookingDataOthers.BookingAgeGroupList.Where(x => x.BookedOptionId == bookedProductDetail.BookedOptionId).ToList().OrderBy(x => x.AgeGroupDesc);
                    var dynamicAgeGroup = new StringBuilder();
                    if (filterDataByOptionId != null)
                    {
                        foreach (var item in filterDataByOptionId)
                        {
                            dynamicAgeGroup.Append("<tr>");
                            dynamicAgeGroup.Append("<td class='col1'><b>" + " " + noOf + " " + PaxInfoMultiLingualWithS(resourceManager, cultureInfo, item.AgeGroupDesc) + "</b></td>");
                            dynamicAgeGroup.Append("<td class='spcr'>&nbsp;</td>");
                            dynamicAgeGroup.Append("<td>" + item.PaxCount + " " + PaxInfoMultiLingual(resourceManager, cultureInfo, item.AgeGroupDesc) + "</td>");
                            dynamicAgeGroup.Append("</tr>");
                        }
                    }
                    attachmentDetails.Replace("##DynamicAgeGroup##", dynamicAgeGroup.ToString());
                }

                attachmentDetails.Replace("##OptionName##", bookedProductDetail.OptionName?.Trim() ?? "N/A");
                attachmentDetails.Replace("##DateOfTour##", bookedProductDetail.FromDate.ToString("dd MMM yyyy"));
                var optionCode = bookedProductDetail?.SupplierOptionCode?.Trim()?.Split('~')?.FirstOrDefault() ?? "N/A";

                if (bookedProductDetail.ApiTypeId == "3")
                    attachmentDetails.Replace("##OptionCode##", "N/A");
                else
                    attachmentDetails.Replace("##OptionCode##", optionCode);

                var htmlCode =
                        "<tr><td class=\"col1\"><b>##key##</b> <br>##em##</td>" +
                        "<td class=\"spcr\">&nbsp;</td>	<td>##value##</td></tr>";

                if (bookedProductDetail.ApiTypeId == "3" && !string.IsNullOrEmpty(bookedProductDetail?.SupplierName1))
                {
                    attachmentDetails = UpdateNode("Supplier Name", "##SupplierName1##", bookedProductDetail?.SupplierName1, attachmentDetails, htmlCode, cultureInfo);

                    attachmentDetails = UpdateNode("Provider Information", "##ProviderInformation##", bookedProductDetail?.ProviderInformation, attachmentDetails, htmlCode, cultureInfo);
                }
                else
                {
                    attachmentDetails.Replace("##SupplierName1##", string.Empty);
                    attachmentDetails.Replace("##ProviderInformation##", string.Empty);
                }

                if (!string.IsNullOrWhiteSpace(bookedProductDetail?.VatNumber))
                {
                    attachmentDetails.Replace("##VATNumber##", bookedProductDetail?.VatNumber);
                }
                else
                {
                    attachmentDetails.Replace("##displayVATNumber##", "none");
                }
                var affiliateId = bookingDataOthers.AffiliateId?.Trim().ToUpperInvariant();
                attachmentDetails.Replace("##SpecialRequest##", string.IsNullOrEmpty(bookedProductDetail.SpecialRequest) ? "N/A" : bookedProductDetail.SpecialRequest?.Trim());
                if (string.IsNullOrEmpty(bookedProductDetail.ContractComment))
                {
                    attachmentDetails.Replace("##displayContractComment##", "none");
                    attachmentDetails.Replace("##ContractComment##", "N/A");
                }
                else
                {
                    //Contract commnet to be displayed in Important Instructions section
                    attachmentDetails.Replace("##displayContractComment##", "none");
                    attachmentDetails.Replace("##ContractComment##", bookedProductDetail.ContractComment);
                }
                if (affiliateId.Equals("413DE577-1630-42C0-82E8-F298187CB984") || affiliateId.Equals("4A1B7E59-8FEA-442A-AF56-D5C1C16D0826") || affiliateId.Equals("7E8277D2-EF03-492A-A882-8199272828F9") || affiliateId.Equals("1A23137F-81B9-4E93-8391-19C2662848DB") || affiliateId.Equals("405F5611-1387-4E19-9840-03A4671BB6A5") || affiliateId.Equals("A80BEC17-148B-46C3-B157-01AA0ECAD983") || affiliateId.Equals("D78748D4-70E3-487E-A6BA-3C5D4BD842BD") || affiliateId.Equals("F7FEEE76-7B67-4886-90AB-07488CB7A167") || affiliateId.Equals("5BEEF089-3E4E-4F0F-9FBF-99BF1F350183") || affiliateId.Equals("E1DFF808-13D0-4BEB-80EA-1247E38CFE9C") || affiliateId.Equals("C0E714F0-2864-40BC-BD82-63D2198384C1") || affiliateId.Equals("A3300403-CE50-4193-8AB0-70562B424ABE") || affiliateId.Equals("85BB0CD2-7CF2-4674-82A7-AE645AA3E974") || affiliateId.Equals("BF24CBCE-2E87-409B-BDA8-F717D16A8261") || affiliateId.Equals("9A082700-E9D1-449C-99A4-F62A561DA929"))
                {
                    attachmentDetails.Replace("##ProductWithProductPageLink##", $"{bookingDataOthers.CompanyWebSite?.Trim()}{Constant.ActivityDetailUrl}{bookedProductDetail.ServiceId?.Trim()}");

                    attachmentDetails.Replace("##ProductPageName##", bookedProductDetail.ServiceName?.Trim() ?? "N/A");
                }
                else
                {
                    attachmentDetails.Replace("##ProductWithProductPageLink##", $"{bookingDataOthers.CompanyWebSite?.Trim()}/product/{bookedProductDetail.ServiceId?.Trim()}?affiliateid={affiliateId}&Language=EN");

                    attachmentDetails.Replace("##ProductPageName##", bookedProductDetail.ServiceName?.Trim() ?? "N/A");
                }
                if (bookingDataOthers.ShowTermsAndCondition == false)
                {
                    attachmentDetails.Replace("##displayTermsC##", "none");
                }
                if (bookedProductDetail?.PickupOptionId == "1")
                {
                    attachmentDetails.Replace("##displayPickPoint##", "none");
                    attachmentDetails.Replace("##displayDropPoint##", "none");
                    attachmentDetails.Replace("##displaySchedule##", "none");
                }
                else
                {
                    attachmentDetails.Replace("##displayMeetingPoint##", "none");
                    attachmentDetails.Replace("##displayEndPoint##", "none");
                    attachmentDetails.Replace("##displayStartTime##", "none");
                }

                if (isPDFVoucher) //PDF
                {
                    attachmentDetails.Replace("##displayPrintButton##", "none");
                }

                attachmentDetails.Replace("##TotalTourPrice##", pre + Convert.ToDecimal(bookedProductDetail.AmountOnWirecard).ToString("00.00") + post);
                attachmentDetails.Replace("##ProductPrice##", pre + Convert.ToDecimal(bookedProductDetail.AmountOnWirecard).ToString("00.00") + post);
                attachmentDetails.Replace("##PickupPoint##", string.IsNullOrEmpty(bookedProductDetail.PickupLocation) ? (string.IsNullOrEmpty(bookedProductDetail.ServiceHotelPickup) ? "N/A" : bookedProductDetail.ServiceHotelPickup)
                                                            : bookedProductDetail.PickupLocation?.Trim().Replace("<br />", "\n").Replace("<br>", "\n").Replace("<br/>", "\n").Replace("<b>", "").Replace("</b>", "").Replace("<p>", " ").Replace("</p>", " "));

                attachmentDetails.Replace("##Duration##", string.IsNullOrEmpty(bookedProductDetail.Duration) ? "N/A" : bookedProductDetail.Duration?.Trim().Replace("<br />", "\n").Replace("<br>", "\n").Replace("<br/>", "\n").Replace("<b>", "").Replace("</b>", "").Replace("<p>", " ").Replace("</p>", " "));

                attachmentDetails.Replace("##DropOffPoint##", string.IsNullOrEmpty(bookedProductDetail.ScheduleReturnDetails) ? "N/A" : bookedProductDetail.ScheduleReturnDetails?.Trim().Replace("<p>", " ").Replace("</p>", " "));
                attachmentDetails.Replace("##Schedule##", string.IsNullOrEmpty(bookedProductDetail.Schedule) ? "N/A" : bookedProductDetail.Schedule?.Trim().Replace("<br />", "\n").Replace("<br>", "\n").Replace("<br/>", "\n").Replace("<b>", "").Replace("</b>", "").Replace("<p>", " ").Replace("</p>", " "));

                attachmentDetails.Replace("##ImportantInstructions##", string.IsNullOrEmpty(bookedProductDetail.PleaseNote) ? "N/A" : SetInstructions(bookedProductDetail.PleaseNote?.Trim()
                    + $"\n{bookedProductDetail.ContractComment}"
                    ));
                attachmentDetails.Replace("##ProductName##", $"{bookedProductDetail.TsProductName?.Trim()} ({bookedProductDetail.BookedOptionStatusName?.Trim()})");
                attachmentDetails.Replace("##IsangoLogo##", isangoLogoUri);
                attachmentDetails.Replace("##GoGif##", goGifUri);
                attachmentDetails.Replace("##BookingVoucherNumber##", bookingDataOthers.BookingRefNo?.Trim() ?? "N/A");
                attachmentDetails.Replace("##BaseURL##", ConfigurationManagerHelper.GetValuefromAppSettings(Constant.WebAPIBaseUrl));

                if (!isPDFVoucher)
                {
                    string htmlH = $" <div style='text-align:left;padding-bottom: 7.5pt;margin:22.5pt 3pt 0'><a target='_blank' href=\"https://www.isango.com/\"><img width=\"150\" alt='' src=\"{isangoLogoUri}\"></a></div>";
                    string htmlF = $"<div style='text-align:center;padding-top: 7.5pt;margin:0 3pt'><span style='font-size:6.75pt;color:#333'>{resourceManager.GetString("FooterText", cultureInfo)}!</span></center><center><a target=\"_blank\" href={Constant.FooterUrl}><img width=\"70\" style=\"margin:0;padding:0;display:block;\" src=\"{isangoLogoUri}\"></a></div>";

                    attachmentDetails.Replace("##HeaderSet##", htmlH);
                    attachmentDetails.Replace("##FooterSet##", htmlF);
                }
                else
                {
                    attachmentDetails.Replace("##HeaderSet##", string.Empty);
                    attachmentDetails.Replace("##FooterSet##", string.Empty);
                }

                #region Only for Free Sale Products

                attachmentDetails.Replace("##IsangoPartner##", bookingDataOthers.AffiliateName?.Trim() ?? "N/A");
                attachmentDetails.Replace("##IsangoPartnerAddress##", $"{supplierDetails?.AddressLine1?.Trim()} {supplierDetails?.AddressLine2?.Trim()} {supplierDetails?.AddressLine3?.Trim()}" ?? "N/A"); //Note: Took first address
                attachmentDetails.Replace("##TelephoneNumber##", supplierDetails?.TelephoneNumber?.Trim() ?? "N/A");
                attachmentDetails.Replace("##EmergencyNumber##", supplierDetails?.EmergencyTelephoneNumber?.Trim() ?? "N/A");
                attachmentDetails.Replace("##Fax##", supplierDetails?.FaxNumber?.Trim() ?? "N/A");
                attachmentDetails.Replace("##Email##", supplierDetails?.EmailAddress?.Trim() ?? "N/A");

                attachmentDetails.Replace("##DestinationAssistance##",
                    endSupplierDetails != null
#pragma warning disable S3358 // Ternary operators should not be nested
                        ? $"<div class=\"contact2\"><h3>{resourceManager.GetString("AtDestinationAssistance", cultureInfo)}{(!languageCode.Equals("EN") ? "<br><em style=\"font-weight: normal\">At destination assistance</ em>" : "")}</h3><p><b class=\"ltBlue\">{resourceManager.GetString("ActivityOperator", cultureInfo)}</b>{(!languageCode.Equals("EN") ? "<br><em>Activity operator</em>" : "")}<br>{supplierDetails?.SupplierName?.Trim()}</p><p><b>{resourceManager.GetString("TelephoneNumber", cultureInfo)}</b><br>{supplierDetails?.TelephoneNumber?.Trim()}</p></div>"
#pragma warning restore S3358 // Ternary operators should not be nested
                        : "");

                #endregion Only for Free Sale Products

                attachmentDetails.Replace("##CustomerSupportNumbers##", bookingDataOthers.CustomerServiceNo?.Trim() ?? "N/A");
                attachmentDetails.Replace("##SupportEmail##", bookingDataOthers?.CompanyEmail?.Trim() ?? "N/A");
                attachmentDetails.Replace("##CancellationPolicy##", bookedProductDetail?.CancellationPolicy?.Trim() ?? "N/A");
                attachmentDetails.Replace("##TermsAndConditionLink##", bookingDataOthers?.TermsAndConditionLink?.Trim() ?? "N/A"); // Added to fix TnC Link issue in Voucher - Sanjay

                //Payment Summary
                productDetailBuilder.Append($"<tr><td> {productCount++}) {bookedProductDetail.TsProductName?.Trim()} ({bookedProductDetail.BookedOptionStatusName?.Trim()})</td><td class=\"tar\">{pre + Convert.ToDecimal(bookedProductDetail.GrosSellingAmount?.Trim()).ToString("00.00") + post}</td></tr>");
                wireCardAmount += Convert.ToDecimal(bookedProductDetail?.AmountOnWirecard?.Trim());

                discountAmount += Convert.ToDecimal(string.IsNullOrEmpty(bookedProductDetail?.DiscountAmount?.Trim()) ? "0.0" : bookedProductDetail?.DiscountAmount?.Trim());
                multiSaveAmount += Convert.ToDecimal(bookedProductDetail?.MultisaveDiscount?.Trim());
                grossSellAmount += Convert.ToDecimal(bookedProductDetail?.GrosSellingAmount?.Trim());
                if (bookedProductDetail.BookedOptionStatusName.Equals("On Request", StringComparison.OrdinalIgnoreCase))
                {
                    totalToBeChargedAmount += Convert.ToDecimal(bookedProductDetail.AmountOnWirecard?.Trim());
                }
                paymentSummaryBuilder.Replace("##TermsAndConditionLink##", bookingDataOthers?.TermsAndConditionLink?.Trim() ?? "N/A");
                paymentSummaryBuilder.Replace("##CancellationPolicy##", bookedProductDetail?.CancellationPolicy?.Trim() ?? "N/A");

                if (!isPDFVoucher)
                {
                    string htmlHead = $" <div style='text-align:left;padding-bottom: 7.5pt;margin:22.5pt 2.25pt 0'><a target='_blank' href=\"https://www.isango.com/\"><img width=\"150\" alt='' src=\"{isangoLogoUri}\"></a></div>";
                    string htmlFoot = $"<div style='text-align:center;padding-top: 7.5pt;margin:0 2.25pt'><span style='font-size:6.75pt;color:#333'>{resourceManager.GetString("FooterText", cultureInfo)}!</span></center><center><a target=\"_blank\" href={Constant.FooterUrl}><img width=\"70\" style=\"margin:0;padding:0;display:block;\" src=\"{isangoLogoUri}\"></a></div>";

                    paymentSummaryBuilder.Replace("##HeaderSet##", htmlHead);
                    paymentSummaryBuilder.Replace("##FooterSet##", htmlFoot);
                }
                else
                {
                    paymentSummaryBuilder.Replace("##HeaderSet##", string.Empty);
                    paymentSummaryBuilder.Replace("##FooterSet##", string.Empty);
                }

                attachmentBody.Append(attachmentDetails);
                //}
                //else
                //{
                //    productDetailBuilder.Append($"<tr><td> {productCount++}) {bookedProductDetail.TsProductName?.Trim()} ({bookedProductDetail.BookedOptionStatusName?.Trim()})</td><td class=\"tar\">{pre + Convert.ToDecimal(bookedProductDetail.GrosSellingAmount?.Trim()).ToString("00.00") + post}</td></tr>");
                //    wireCardAmount += Convert.ToDecimal(bookedProductDetail?.AmountOnWirecard?.Trim());

                //    discountAmount += Convert.ToDecimal(string.IsNullOrEmpty(bookedProductDetail?.DiscountAmount?.Trim()) ? "0.0" : bookedProductDetail?.DiscountAmount?.Trim());
                //    multiSaveAmount += Convert.ToDecimal(bookedProductDetail?.MultisaveDiscount?.Trim());
                //    grossSellAmount += Convert.ToDecimal(bookedProductDetail?.GrosSellingAmount?.Trim());
                //    if (bookedProductDetail.BookedOptionStatusName.Equals("On Request", StringComparison.OrdinalIgnoreCase))
                //    {
                //        totalToBeChargedAmount += Convert.ToDecimal(bookedProductDetail.AmountOnWirecard?.Trim());
                //    }
                //}
            }

            //Set payment summary details
            var totalChargedAmount = (wireCardAmount - totalToBeChargedAmount);
            var amountAfterDiscount = grossSellAmount - discountAmount - multiSaveAmount;
            totalChargedAmount = totalChargedAmount < 0 ? 0 : totalChargedAmount;
            amountAfterDiscount = amountAfterDiscount < 0 ? 0 : amountAfterDiscount;
            string htmlHeader = string.Empty;
            string htmlFooter = string.Empty;

            //In case of only big bus product booking, no need to add following block in attachment.
            //if (bigBusProducts.Count < bookedProductDetailList.Count)
            //{
            paymentSummaryBuilder.Replace("##LetSettle##", letSettleUri);
            paymentSummaryBuilder.Replace("##IsangoLogo##", isangoLogoUri);
            paymentSummaryBuilder.Replace("##GoGif##", goGifUri);
            paymentSummaryBuilder.Replace("##BookingVoucherNumber##", bookingDataOthers.BookingRefNo?.Trim() ?? "N/A");
            paymentSummaryBuilder.Replace("##ProductDetails##", productDetailBuilder.ToString()?.Trim());
            paymentSummaryBuilder.Replace("##TotalBookingAmount##", pre + grossSellAmount.ToString("00.00") + post);
            //show hide payment fields on condition
            if (discountAmount == 0)
            {
                paymentSummaryBuilder.Replace("##displayDiscountVoucher##", "none");
            }
            if (multiSaveAmount == 0)
            {
                paymentSummaryBuilder.Replace("##displayMultisaveDiscount##", "none");
            }
            if (discountAmount == 0 && multiSaveAmount == 0)
            {
                paymentSummaryBuilder.Replace("##displayAmountAfterDiscount##", "none");
            }
            paymentSummaryBuilder.Replace("##DiscountVoucher##", pre + " " + discountAmount.ToString("00.00") + " " + post);
            paymentSummaryBuilder.Replace("##MultisaveDiscount##", pre + " " + multiSaveAmount.ToString("00.00") + " " + post);
            paymentSummaryBuilder.Replace("##AmountAfterDiscount##", pre + " " + amountAfterDiscount.ToString("00.00") + " " + post);

            paymentSummaryBuilder.Replace("##TotalChargedAmount##", pre + " " + totalChargedAmount.ToString("00.00") + " " + post);

            paymentSummaryBuilder.Replace("##ToBeChargedAmount##", bookingDataOthers.BookedProductDetailList.Any(e =>
                e.BookedOptionStatusName.Equals("On Request", StringComparison.OrdinalIgnoreCase))
#pragma warning disable S3358 // Ternary operators should not be nested
                ? !languageCode.Equals("EN") ? $"<tr><td><b> {resourceManager.GetString("ToBeChargedAmount", cultureInfo)}</b> /</br> <em> To be Charged on Confirmation of Pending Booking Requests</em></td><td class=\"tar\"><b>{pre}{totalToBeChargedAmount:00.00}{post}</b></td></tr>" : $"<tr><td><b> {resourceManager.GetString("ToBeChargedAmount", cultureInfo)} </b></td><td class=\"tar\"><b>{pre}{totalToBeChargedAmount:00.00}{post}</b></td></tr>"
#pragma warning restore S3358 // Ternary operators should not be nested
                : "");

            paymentSummaryBuilder.Replace("##BookingNumber##", bookingDataOthers?.BookingRefNo?.Trim() ?? "N/A");
            string finalHTML = "";

            //string isBigBus = "";
            //if (!string.IsNullOrEmpty(bigBusAttachment.ToString()))
            //{
            //    isBigBus = "<link href=\"" + ConfigurationManagerHelper.GetValuefromAppSettings(Constant.WebAPIBaseUrl) + "/CSS/bigBus.css\" rel=\"stylesheet\" />";
            //    attachmentBody.Append(bigBusAttachment);
            //}

            //start html header B2B and B2C
            //PDF Header
            string primaryLogoPath = string.Empty;
            string secondaryLogoPath = string.Empty;
            string language = bookingDataOthers.LanguageCode.ToLowerInvariant();
            string imagePath = $"{ConfigurationManagerHelper.GetValuefromAppSettings(Constant.CDN) + Constant.ImgPath}";
            var isangoDefaultLogoUri = $"{imagePath}{ConfigurationManager.AppSettings["IsangoAffiliateId"]}{".png"}";

            //string footerHTML = $"<div style='text-align:center;padding-top: 7.5pt;margin:0 2.25pt'><span style='font-size:6.75pt;color:#333'>{resourceManager.GetString("FooterText", cultureInfo)}!</span></center><center><a target=\"_blank\" href={Constant.FooterUrl}><img width=\"70\" style=\"margin:0;padding:0;display:block;\" src=\"{isangoLogoUri}\"></a></div>";
            if (bookingDataOthers.IsPrimaryLogo == true)
            {
                if (language.Equals("en"))
                    primaryLogoPath = imagePath + bookingDataOthers.AffiliateId + ".png";
                else
                {
                    primaryLogoPath = imagePath + language + "/" + bookingDataOthers.AffiliateId + ".png";
                    if (!File.Exists(primaryLogoPath))
                        primaryLogoPath = imagePath + bookingDataOthers.AffiliateId + ".png";
                }
                secondaryLogoPath = isangoDefaultLogoUri;
            }
            else
            {
                primaryLogoPath = isangoDefaultLogoUri;

                if (language.Equals("en"))
                    secondaryLogoPath = imagePath + bookingDataOthers.AffiliateId + ".png";
                else
                {
                    secondaryLogoPath = imagePath + language + "/" + bookingDataOthers.AffiliateId + ".png";
                    if (!File.Exists(primaryLogoPath))
                        secondaryLogoPath = imagePath + bookingDataOthers.AffiliateId + ".png";
                }
            }

            if (URLExists(primaryLogoPath))
            {
                if (!isPDFVoucher)//Html Voucher
                {
                    htmlHeader = $"<header class=\"header\"><a target='_blank' href=\"https://www.isango.com/\"><img width=\"85\" alt='' src=\"{primaryLogoPath}\"></a></header>";
                }
                else //PDF Voucher
                {
                    htmlHeader =
                   $"<header style='height:40pt !important;'>" +
                   $"<a target='_blank' href=\"https://www.isango.com/\">" +
                   $"<center><img style='float:right;max-height:40pt;max-width:63pt;margin:7px;' alt='' " + $"src=\"{primaryLogoPath}\"></center>" +
                   $"</a>" +
                   $"</header>";
                }
            }

            if ((primaryLogoPath.ToLower() != secondaryLogoPath.ToLower()) && (URLExists(secondaryLogoPath)))
            {
                var logoHeight = bookingDataOthers.CompanyName.ToString().Contains("BoatTours") ? "25pt" : "40pt";

                if (!isPDFVoucher)//Html Voucher
                {
                    htmlHeader =
                    $"<header class=\"header\" style='text-align:left;float:left;padding:0;'>" +
                    $"<a target='_blank' href=\"https://www.isango.com/\">" +
                    $"<img style='margin: 0 0 10pt;max-height:{logoHeight};max-width:63pt' alt='' " + $"src=\"{secondaryLogoPath}\">" +
                    $"</a>" +
                    $"<a target = '_blank' href =\"https://www.isango.com/\" >" +
                    $"<img style='float:right; margin: 0 0 10pt 0;max-height:22.5pt;max-width:75pt' alt='' src=\"{primaryLogoPath}\">" +
                    $"</a>" +
                    $"</header>";
                }
                else//PDF Voucher
                {
                    htmlHeader =
                   $"<header style='height:40pt !important;'>" +
                   $"<a target='_blank' href=\"https://www.isango.com/\" >" +
                   $"<img style='float:left;margin: 5pt 0;max-height:{logoHeight};max-width:63pt;' alt='' " + $"src=\"{secondaryLogoPath}\">" +
                   $"</a>" +
                   $"<a target = '_blank' href =\"https://www.isango.com/\">" +
                   $"<img style='float:right;margin:5pt 0;max-height:22.5pt;max-width:65pt;' alt='' src=\"{primaryLogoPath}\">" +
                   $"</a>" +
                   $"</header>";
                }
            }
            //end html header B2B and B2C

            if (isPDFVoucher)//PDF Voucher
            {
                htmlFooter = $"<footer style='text-align:center;'><span style='text-align:center;font-size:6.75pt;color:#333'>{resourceManager.GetString("FooterText", cultureInfo)}!</span></center><center><a target=\"_blank\" href={Constant.FooterUrl}><img width=\"70\" style=\"margin:0 auto;padding:0;display:block;\" src=\"{isangoLogoUri}\"></a></footer>";
            }
            else
            {
                htmlFooter = $"<footer class=\"footer\"><span style='font-size:6.75pt;color:#333'>{resourceManager.GetString("FooterText", cultureInfo)}!</span></center><center><a target=\"_blank\" href={Constant.FooterUrl}><img width=\"70\" style=\"margin:0 auto;padding:0;display:block;\" src=\"{isangoLogoUri}\"></a></footer>";
            }

            finalHTML = "<html lang=\"en\">"
                                + "<head>"
                                    + "<meta charset=\"UTF-8\">"
                                    + "<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">"
                                    + "<meta http-equiv=\"X-UA-Compatible\" content=\"ie=edge\">"
                                    + "<title>isango! - experience the world with us</title>"
                                    + "<link href=\"" + ConfigurationManagerHelper.GetValuefromAppSettings(Constant.WebAPIBaseUrl) + "/CSS/Voucher.css\" rel=\"stylesheet\" />"
                                + "</head>"
                                + "<body>"
                                    + (isPDFVoucher ? htmlFooter : string.Empty)
                                    + (isPDFVoucher ? htmlHeader : string.Empty)
                                    + "<div class=\"mainBody\"><table>"
                                    + "<thead><tr><td> <div style=\"height:"
                                    + (isPDFVoucher ? "0pt;" : "115pt;")
                                    + "width:100%;\" class=\"headerSpace\">&nbsp;</div></td></tr></thead>"
                                    + "<tbody><tr><td>"
                                    + attachmentBody
                                    + (isPDFVoucher ? "<div  style='clear: both;page-break-before: always;'></div>" : string.Empty)
                                    + paymentSummaryBuilder
                                    + "</td></tr></tbody>"
                                    + "<tfoot><tr><td> <div style=\"height:"
                                    + (isPDFVoucher ? "0pt;" : "100pt;")
                                    + "width:100%;\" class=\"footerSpace\">&nbsp;</div></td></tr></tfoot></table></div>"
                                    + (isPDFVoucher ? htmlFooter : string.Empty)
                                + "</body>"
                            + "</html>";

            _log.Warning($"MailAttachmentService|GeneratePdfForBooking|{finalHTML}");

            return Tuple.Create(finalHTML.Replace("???", ""), htmlHeader, htmlFooter);

            //}

            //if (!string.IsNullOrEmpty(bigBusAttachment.ToString()))
            //{
            //    attachmentBody.Append(bigBusAttachment);
            //}

            //attachmentBody.Replace("???", "");
            //return Tuple.Create(attachmentBody.ToString(), htmlHeader, htmlFooter);
        }

        private Tuple<string, string, string> GeneratePdfForBookingNew(BookingDataOthers bookingDataOthers, ResourceManager resourceManager, CultureInfo cultureInfo, bool isPDFVoucher = false, bool iscancelled = false)
        {
            var attachmentBody = new StringBuilder();
            var productDetailBuilder = new StringBuilder();
            var productCount = 1;
            string htmlHeader = string.Empty;
            string htmlFooter = string.Empty;
            var languageCode = bookingDataOthers.LanguageCode?.ToUpperInvariant();
            var paymentSummaryBuilder = new StringBuilder();
            var isQrAvailable = false;
            //var bigBusAttachment = new StringBuilder();

            var bookedProductDetailList = bookingDataOthers.BookedProductDetailList;

            var bookedProductCount = bookedProductDetailList?.Count(x => x.BookedOptionStatusName != "Failed");

            if (bookedProductDetailList == null || bookedProductCount == 0) { return null; }

            PrePostCurrency(bookingDataOthers, out var pre, out var post);
            
            var letSettleData = File.ReadAllBytes(letsettleFilePath);
            var letSettleUri = @"data:image/png;base64," + Convert.ToBase64String(letSettleData);

            var goGifData = File.ReadAllBytes(goGiffilePath);
            var goGifUri = @"data:image/png;base64," + Convert.ToBase64String(goGifData);


            var isangoLogoData = File.ReadAllBytes(isangoFilePath);
            var isangoLogoUri = @"data:image/png;base64," + Convert.ToBase64String(isangoLogoData);

            //Remove Products that are Cancelled
            //if (bookedProductDetailList != null && bookedProductDetailList.Count > 0)
            //{
            //    bookedProductDetailList = bookedProductDetailList.Where(x => x.BookedOptionStatusName != Constant.Cancelled).ToList();
            //}

            ////If all products are Cancel in the booking
            //if (bookedProductDetailList == null || bookedProductDetailList.Count == 0)
            //{
            //    var templateName = GetTemplateName(Constant.WholeBookingCancelled, languageCode);
            //    var productDetailText = LoadTemplate($"{templateName}");
            //    var attachmentDetails = new StringBuilder(productDetailText);
            //    attachmentDetails.Replace("##BookingVoucherNumber##", bookingDataOthers.BookingRefNo?.Trim() ?? "N/A");
            //    attachmentDetails.Replace("##BaseURL##", ConfigurationManagerHelper.GetValuefromAppSettings(Constant.WebAPIBaseUrl));
            //    string htmlHead = string.Empty;
            //    string htmlFoot = string.Empty;
            //    if (!isPDFVoucher)
            //    {
            //        htmlHead = $" <div style='text-align:left;padding-bottom: 7.5pt;margin:22.5 2.25pt 0pt'><a target='_blank' href=\"https://www.isango.com/\"><img width=\"150\" alt='' src=\"{isangoLogoUri}\"></a></div>";
            //        htmlFoot = $"<div style='text-align:center;padding-top: 7.5pt;margin:0 2.25pt'><span style='font-size:6.75pt;color:#333'>{resourceManager.GetString("FooterText", cultureInfo)}!</span></center><center><a target=\"_blank\" href={Constant.FooterUrl}><img width=\"70\" style=\"margin:0;padding:0;display:block;\" src=\"{isangoLogoUri}\"></a></div>";

            //        attachmentDetails.Replace("##HeaderSet##", htmlHead);
            //        attachmentDetails.Replace("##FooterSet##", htmlFoot);
            //    }
            //    else
            //    {
            //        attachmentDetails.Replace("##HeaderSet##", string.Empty);
            //        attachmentDetails.Replace("##FooterSet##", string.Empty);
            //    }
            //    attachmentBody.Append(attachmentDetails);
            //    attachmentBody.Replace("???", "");
            //    return Tuple.Create(attachmentBody.ToString(), htmlHead, htmlFoot);
            //}

            var bookedProductDetail = bookedProductDetailList?.FirstOrDefault();

            var bookedProductUsefulDownloads = bookingDataOthers?.UsefulDowloads?
                                                .Where(x =>
                                                            x.BookedOptionId == bookedProductDetail?.BookedOptionId?.ToInt()
                                                            && x != null
                                                            && !string.IsNullOrWhiteSpace(x.DownloadLink)
                                                        )?.OrderBy(y => y.BookedOptionId)
                                                        ?.ThenBy(y => y.LinkOrder)
                                                        ?.ToList();

            //#TODO UsefulDownloads it should not be executed as above condition should return links
            /*
            if (bookedProductUsefulDownloads?.Any() == false && bookingDataOthers?.UsefulDowloads?.Any() == true)
            {
                var firstService = bookingDataOthers?.UsefulDowloads?.FirstOrDefault();
                bookedProductUsefulDownloads = bookingDataOthers?.UsefulDowloads?.Where(x =>
                                                            x.BookedOptionId == firstService?.BookedOptionId
                                                            && x != null
                                                            && !string.IsNullOrWhiteSpace(x.DownloadLink)
                                                        )?.OrderBy(y => y.BookedOptionId)
                                                        ?.ThenBy(y => y.LinkOrder)
                                                        ?.ToList();
            }
            */

            if (bookedProductDetail != null)
            {
                var templateName = GetTemplateNameNew(languageCode);
                var VoucherText = LoadTemplate($"{templateName}");
                var attachmentDetails = new StringBuilder(VoucherText);
                string language = bookingDataOthers.LanguageCode.ToLowerInvariant();
                var primaryLogoPath = string.Empty;
                var secondaryLogoPath = string.Empty;
                string imagePath = $"{ConfigurationManagerHelper.GetValuefromAppSettings(Constant.CDN) + Constant.ImgPath}";

                var isangoDefaultLogoUri = $"{imagePath}{ConfigurationManagerHelper.GetValuefromAppSettings("IsangoAffiliateId")}{".png"}";

                if (imagePath.Contains("http://localhost:5266"))
                {
                    imagePath = imagePath.Replace("http://localhost:5266", "https://isango.com");
                    isangoDefaultLogoUri = isangoDefaultLogoUri.Replace("http://localhost:5266", "https://isango.com");
                }
                if (bookingDataOthers.IsPrimaryLogo == true)
                {
                    if (language.Equals("en"))
                        primaryLogoPath = imagePath + bookingDataOthers.AffiliateId + ".png";
                    else
                    {
                        primaryLogoPath = imagePath + language + "/" + bookingDataOthers.AffiliateId + ".png";
                        if (!File.Exists(primaryLogoPath))
                            primaryLogoPath = imagePath + bookingDataOthers.AffiliateId + ".png";
                    }
                    secondaryLogoPath = isangoDefaultLogoUri;
                }
                else
                {
                    primaryLogoPath = isangoDefaultLogoUri;

                    if (language.Equals("en"))
                        secondaryLogoPath = imagePath + bookingDataOthers.AffiliateId + ".png";
                    else
                    {
                        secondaryLogoPath = imagePath + language + "/" + bookingDataOthers.AffiliateId + ".png";
                        if (!File.Exists(primaryLogoPath))
                            secondaryLogoPath = imagePath + bookingDataOthers.AffiliateId + ".png";
                    }
                }

                if (bookingDataOthers.AffiliateName.ToLower().Contains("isango"))
                {
                    if (!isPDFVoucher)//Html Voucher
                    {
                        htmlHeader = $" <header class=\"header\"><a target='_blank' href=\"https://www.isango.com/\"><img width=\"85\" alt='' src=\"{primaryLogoPath}\"></a></header>";
                    }
                    else //PDF Voucher
                    {
                        htmlHeader =
                       $"<header style='height:30pt !important';>" +
                       "<table width='100%' style='width: 100%;'>" +
                       "<tr>" +
                       "<td style=\"text-align:center;width:100%\">" +
                       $"<a target='_blank' href =\"https://www.isango.com/\">" +
                       $"<img style='max-height:22.5pt;max-width:63pt;margin:2pt auto 5pt;' alt='' src=\"{primaryLogoPath}\">" +
                       $"</a>" +
                       "</td>" +
                       //"<td style='text-align:left;margin-top:11.25pt;'>" +
                       //$"<span class='tac' style='font-size: 12pt;'><b>{resourceManager.GetString("BookingRefNo", cultureInfo)}: {bookingDataOthers.BookingRefNo?.Trim().ToUpper() ?? "N/A"}</b></span>" +
                       //"</td>" +
                       "</tr>" +
                       "</table>" +
                       $"</header>";
                    }
                }

                if (!bookingDataOthers.AffiliateName.ToLower().Contains("isango") && (primaryLogoPath.ToLower() != secondaryLogoPath.ToLower()) && (URLExists(secondaryLogoPath)))
                {
                    if (!isPDFVoucher)//Html Voucher
                    {
                        htmlHeader =
                        $"<header class=\"header\" style='text-align:left;float:left;padding:0;height:30pt !important;'>" +
                        $"<a target='_blank' href=\"https://www.isango.com/\" style=\"margin: 2pt 0 5pt; \">" +
                        $"<img style='max-height:22.5pt;max-width:63pt' alt='' " + $"src=\"{primaryLogoPath}\">" +
                        $"</a>" +
                        $"<a target = '_blank' href =\"https://www.isango.com/\" style=\"float:right; margin: 2pt 0 5pt; \">" +
                        $"<img style='max-height:22.5pt;max-width:63pt' alt='' src=\"{secondaryLogoPath}\">" +
                        $"</a>" +
                        $"</header>";
                    }
                    else//PDF Voucher
                    {
                        htmlHeader =
                       $"<header style='height:30pt !important';>" +
                       "<table width='100%' style='width: 100%;'>" +
                       "<tr>" +
                       "<td style=\"text-align:left;\">" +
                       $"<a target = '_blank' href =\"https://www.isango.com/\">" +
                       $"<img style='margin: 2pt 0 5pt;max-height:22.5pt;max-width:63pt;' alt='' src=\"{primaryLogoPath}\">" +
                       $"</a>" +
                       "</td>" +
                       //"<td style='text-align:center;'>" +
                       //$"<span class='tac' style='font-size: 12pt;'><b>{resourceManager.GetString("BookingRefNo", cultureInfo)}: {bookingDataOthers.BookingRefNo?.Trim().ToUpper() ?? "N/A"}</b></span>" +
                       //"</td>" +
                       "<td style=\"text-align:right;\">" +
                       $"<a target = '_blank' href =\"https://www.isango.com/\">" +
                       $"<img style='margin: 2pt 0 5px;max-height:22.5pt;max-width:63pt;' alt='' src=\"{secondaryLogoPath}\">" +
                       $"</a>" +
                       "</td>" +
                       "</tr>" +
                       "</table>" +
                       $"</header>";
                    }
                }

                var isQrPerPaxApplicable = bookedProductDetail.IsQRCodePerPax
                                           || (bookingDataOthers?.BarCodeList?.Count > 0 && bookedProductDetail.ApiTypeId == "3");

                if (bookingDataOthers?.BarCodeList != null
                    && isQrPerPaxApplicable)
                {
                    var productDetailsSection = new StringBuilder();
                    var counterForPageBreak = 0;
                    var counterforPaxType = 0;
                    foreach (var barCode in bookingDataOthers?.BarCodeList)
                    {
                        barCode.PassengerType = barCode?.PassengerType?.Replace("rate", "")?.Trim()?.ToLower();
                        var paxFound = false;
                        var values = Enum.GetValues(typeof(PassengerType));
                        foreach (var item in values)
                        {
                            var barcodePax = !string.IsNullOrWhiteSpace(barCode?.FiscalNumber?.ToLower()) ?
                                barCode?.FiscalNumber?.ToLower()
                                : (barCode?.PassengerType?.ToLower() ?? string.Empty);

                            var mappingPax = item?.ToString().ToLower();
                            if (string.IsNullOrWhiteSpace(barcodePax))
                            {
                                barcodePax = mappingPax;
                            }
                            var ageGroupDesc =

                                        bookingDataOthers?.BookingAgeGroupList?.FirstOrDefault(
                                                x => ((PassengerType)x.PassengerTypeId).ToString()?.ToLower() == barcodePax
                                            );

                            if (!string.IsNullOrEmpty(barcodePax))
                            {
                                if (ageGroupDesc == null)
                                {
                                    if (barcodePax.Contains(mappingPax)
                                        ||
                                       mappingPax.Contains(barcodePax))
                                    {
                                        ageGroupDesc = bookingDataOthers?.BookingAgeGroupList?.FirstOrDefault(
                                                        x => x.AgeGroupDesc?.ToLower()?.Contains(mappingPax) == true
                                                    );
                                    }
                                }
                                if (ageGroupDesc != null)
                                {
                                    barCode.PassengerType = ageGroupDesc?.AgeGroupDesc?.ToString()
                                               ?? item?.ToString();

                                    if (bookedProductDetail?.ApiTypeId == "16" || bookedProductDetail?.ApiTypeId == "17" || bookingDataOthers?.BarCodeList?.Count == 1)
                                    {
                                        barCode.PassengerType = (ageGroupDesc?.PaxCount ?? barCode?.PassengerCount ?? 1) + " " + barCode?.PassengerType;
                                    }
                                    else
                                    {
                                        barCode.PassengerType = 1 + " " + barCode?.PassengerType;
                                    }

                                    paxFound = true;

                                    break;
                                }
                            }
                        }
                        if (!paxFound)
                        {
                            if (barCode?.PassengerCount != null && barCode?.PassengerCount > 0)
                            {
                                barCode.PassengerType = barCode?.PassengerCount + " " + (!string.IsNullOrWhiteSpace(barCode?.FiscalNumber?.ToLower()) ?
                                barCode?.FiscalNumber?.ToLower()
                                : (barCode?.PassengerType?.ToLower() ?? "Adult"));
                            }
                            else
                            {
                                try
                                {
                                    try
                                    {
                                        barCode.PassengerType = (bookingDataOthers?.BookingAgeGroupList?[counterforPaxType].PaxCount ?? 1) + " " + (bookingDataOthers?.BookingAgeGroupList?[counterforPaxType].AgeGroupDesc ?? "Adult");
                                    }
                                    catch (Exception ex)
                                    {
                                        barCode.PassengerType = (bookingDataOthers?.BookingAgeGroupList?.FirstOrDefault(x => x?.PaxCount > 1)?.PaxCount ?? 1) + " " + (bookingDataOthers?.BookingAgeGroupList?.FirstOrDefault(x => x?.PaxCount > 1)?.AgeGroupDesc ?? bookingDataOthers?.BookingAgeGroupList?.FirstOrDefault()?.AgeGroupDesc ?? "Adult");
                                    }
                                }
                                catch (Exception e)
                                {
                                    barCode.PassengerType = "Adult";
                                }
                            }
                        }
                        counterforPaxType = counterforPaxType + 1;
                        //if (bookedProductDetail?.ApiTypeId == "16") //TourCMS =16
                        //{
                        //    if (string.IsNullOrEmpty(barCode.PassengerType))
                        //    {
                        //        barCode.PassengerType = barCode.FiscalNumber;
                        //    }
                        //    var bookingDataOtherList = bookingDataOthers?.BookingAgeGroupList.Select(x => x.AgeGroupDesc.ToLower()).ToList();
                        //    var barcodeList = bookingDataOthers?.BarCodeList;
                        //    var checkPaxQtyListData = barcodeList?.Where(x => x.PassengerType.ToLower().Trim().Contains(paxTypeCompare)).ToList();
                        //    var checkPaxQtyList = bookingDataOthers?.BookingAgeGroupList?.Where(x => x.AgeGroupDesc.ToLower().Contains(paxTypeCompare)).ToList();
                        //    if (checkPaxQtyListData.Count > 1)//multiple components -multiple items
                        //    {
                        //        //var paxcount = checkPaxQtyList?.Select(x=>x.PaxCount)?.FirstOrDefault();
                        //        //var checkPaxQty = checkPaxQtyListData?.Count/ paxcount;
                        //        var checkPaxQty = 1;
                        //        if (!string.IsNullOrEmpty(barCode.PassengerType))
                        //        {
                        //            barCode.PassengerType = checkPaxQty + " x " + barCode.PassengerType;
                        //        }
                        //    }
                        //    else //multiple components -single items
                        //    {
                        //        var checkPaxQty = checkPaxQtyList?.Select(x => x.PaxCount)?.FirstOrDefault();
                        //        if (checkPaxQty == null || checkPaxQty == 0)
                        //        {
                        //            barCode.PassengerType = barCode.PassengerType;
                        //        }
                        //        if (!string.IsNullOrEmpty(barCode.PassengerType))
                        //        {
                        //            barCode.PassengerType = checkPaxQty + " x " + barCode.PassengerType;
                        //        }
                        //    }
                        //}
                        var paxString = barCode.PassengerType;

                        //var productDetails = new StringBuilder();
                        var productDetailsText = GeneratePDFProductSection(paxString, resourceManager, cultureInfo);

                        if (bookedProductDetail.ApiTypeId == (Convert.ToInt32(APIType.Hotelbeds).ToString()))
                        {
                            var providerInformationSection = ""//"<tr><td>&nbsp;</td></tr>"
                                                        + $"<tr><td class='font20'> <b>{resourceManager.GetString("ProviderInformation", cultureInfo)}: </b>"
                                                         + $"<span style='margin-left:50; !important;'>{bookedProductDetail.ProviderInformation}</span>"
                                                        + "</td></tr>";
                            productDetailsText = productDetailsText.Replace("###ProviderInformationSection###", providerInformationSection);

                            var bookableAndPayableBySection = "<tr><td style='text-align: center;padding: 30px 0 20px;'>"
                                           + $"<b>{resourceManager.GetString("BookableAndPayableBy", cultureInfo)} *: </b> <span>{bookedProductDetail.SupplierName1} | {resourceManager.GetString("VatNumber", cultureInfo)}: {bookedProductDetail.VatNumber}</span>"
                                           + "</td></tr>";
                            productDetailsText = productDetailsText.Replace("###BookableAndPayableBySection###", bookableAndPayableBySection);
                        }
                        else
                        {
                            productDetailsText.Replace("###ProviderInformationSection###", String.Empty);
                            productDetailsText.Replace("###BookableAndPayableBySection###", String.Empty);
                        }

                        if (counterForPageBreak > 0)
                        {
                            productDetailsSection.Append("<tr style='clear: both;page-break-before: always;' ></tr>");
                        }
                        productDetailsSection.Append(productDetailsText);

                        counterForPageBreak = counterForPageBreak + 1;

                        if (bookedProductDetail?.ApiTypeId == "16") //TourCMS =16
                        {
                            var tourCMSQrCode = String.Empty;
                            var tourCMSSupplierCode = string.Empty;

                            var splitData = barCode.BarCode.Split(new string[] { "!!!" }, StringSplitOptions.None);
                            if (splitData.Count() > 1)
                            {
                                tourCMSSupplierCode = splitData[0];
                                tourCMSQrCode = splitData[1];
                                barCode.BarCode = tourCMSSupplierCode;
                                barCode.ShowValueOnly = tourCMSQrCode;
                                //barCode.CodeType = barCode.CodeType.ToLower() == "string" ? "" : barCode.CodeType;
                                barCode.CodeType = (barCode.CodeType?.ToLower() == Constant.IsangoQrCode?.ToLower() || barCode.CodeType?.ToLower() == "string")
                                ? "" : barCode.CodeType;
                            }
                            else
                            {
                                tourCMSSupplierCode = splitData[0];
                                barCode.BarCode = tourCMSSupplierCode;
                                barCode.ShowValueOnly = "";
                                //barCode.CodeType = barCode.CodeType.ToLower() == "string" ? "" : barCode.CodeType;
                                barCode.CodeType = (barCode.CodeType.ToLower() == Constant.IsangoQrCode.ToLower() ||
                                    barCode.CodeType.ToLower() == "string" ? "" : barCode.CodeType);
                           }
                        }

                        if (iscancelled)
                        {
                            productDetailsSection.Replace("##QRCodeTable##", GenerateCancelledQRCode());
                        }
                        else
                        {
                            var barCodeList = new List<BarCodeData>() { barCode };
                            var generatedQRCode = GenerateMultipleQRCodePerPaxType(barCodeList, bookingDataOthers.BookingRefNo?.Trim());
                            productDetailsSection.Replace("##QRCodeTable##", generatedQRCode);
                        }
                    }

                    attachmentDetails.Replace("##ProductData##", productDetailsSection.ToString());
                }
                else
                {
                    var AgeGroups = bookingDataOthers.BookingAgeGroupList.Where(x => x.BookedOptionId == bookedProductDetail.BookedOptionId)?.ToList();

                    var paxString = string.Empty;// PassengerType.Adult;

                    foreach (var paxInfo in AgeGroups)
                    {
                        if (string.IsNullOrWhiteSpace(paxString))
                        {
                            paxString = paxInfo.PaxCount + " " + paxInfo.AgeGroupDesc;
                        }
                        else
                        {
                            paxString = paxString + ", " + paxInfo.PaxCount + " " + paxInfo.AgeGroupDesc;
                        }
                    }
                    //var productDetails = new StringBuilder();
                    var productDetailsText = GeneratePDFProductSection(paxString, resourceManager, cultureInfo);

                    if (bookedProductDetail.ApiTypeId == (Convert.ToInt32(APIType.Hotelbeds).ToString()))
                    {
                        var providerInformationSection = ""//"<tr><td>&nbsp;</td></tr>"
                                                    + $"<tr><td class='font20'> <b>{resourceManager.GetString("ProviderInformation", cultureInfo)}: </b>"
                                                     + $"<span style='margin-left:50; !important;'>{bookedProductDetail.ProviderInformation}</span>"
                                                    + "</td></tr>";
                        productDetailsText = productDetailsText.Replace("###ProviderInformationSection###", providerInformationSection);

                        var bookableAndPayableBySection = "<tr><td style='text-align: center;padding: 30px 0 20px;'>"
                                       + $"<b>{resourceManager.GetString("BookableAndPayableBy", cultureInfo)} *: </b> <span>{bookedProductDetail.SupplierName1} | {resourceManager.GetString("VatNumber", cultureInfo)}: {bookedProductDetail.VatNumber}</span>"
                                       + "</td></tr>";
                        productDetailsText = productDetailsText.Replace("###BookableAndPayableBySection###", bookableAndPayableBySection);
                    }
                    else
                    {
                        productDetailsText.Replace("###ProviderInformationSection###", String.Empty);
                        productDetailsText.Replace("###BookableAndPayableBySection###", String.Empty);
                    }

                    attachmentDetails.Replace("##ProductData##", productDetailsText);

                   
                    if (!string.IsNullOrEmpty(bookedProductDetail?.QrCode)
                         && bookedProductDetail?.QRCodeType?.ToUpper() != Constant.IsangoLink)
                    {
                        //Generate QR Code
                        var codes = bookedProductDetail?.QrCode?.Split('~');
                        if (codes?.Length > 1)
                        {
                            var codeType = string.IsNullOrWhiteSpace(codes[0]) ? string.Empty : codes[0].ToLower();
                            var groupCodes = string.IsNullOrWhiteSpace(codes[1]) ? string.Empty : codes[1];
                            /*
                           var generatedQRCode = string.Empty;

                           if (codeType.Contains("qr") && !string.IsNullOrWhiteSpace(groupCodes))
                           {
                               generatedQRCode = GenerateMultipleQRCode(groupCodes, bookingDataOthers.BookingRefNo?.Trim());
                               attachmentDetails.Replace("##QRCodeTable##", generatedQRCode);
                           }
                           */

                            if (iscancelled)
                            {
                                attachmentDetails.Replace("##QRCodeTable##", GenerateCancelledQRCode());
                            }
                            else
                            {
                                var qrcodes = new List<BarCodeData>();
                                var qdata = new BarCodeData
                                {
                                    BarCode = groupCodes,
                                    IsResourceApply = false,
                                    BookedOptionId = bookedProductDetail.BookedOptionId,
                                    CodeType = codeType,
                                    CodeValue = groupCodes,
                                };
                                qrcodes.Add(qdata);
                                var generatedQRCode = GenerateMultipleQRCodePerPaxType(qrcodes, bookingDataOthers.BookingRefNo?.Trim());
                                attachmentDetails.Replace("##QRCodeTable##", generatedQRCode);
                            }
                        }
                        else
                        {
                            if (bookedProductDetail?.ApiTypeId == "16") //TourCMS =16
                            {
                                var tourCMSQrCode = String.Empty;
                                var tourCMSSupplierCode = string.Empty;

                                var splitData = bookedProductDetail.QrCode.Split(new string[] { "!!!" }, StringSplitOptions.None);
                                if (splitData.Count() > 1)
                                {
                                    tourCMSSupplierCode = splitData[0];
                                    tourCMSQrCode = splitData[1];
                                }
                                else
                                {
                                    tourCMSSupplierCode = splitData[0];
                                }
                                var barCode = tourCMSSupplierCode?.Split('~')?.LastOrDefault() ?? tourCMSSupplierCode;
                                //var barcodeCheck = bookedProductDetail?.QRCodeType?.ToUpper() == "BAR" ? bookedProductDetail?.QRCodeType?.ToLower() : string.Empty;
                                var barcodeCheck = bookedProductDetail?.QRCodeType?.ToUpper() == Constant.IsangoBarcode || bookedProductDetail?.QRCodeType?.ToUpper() == "BAR" ? bookedProductDetail?.QRCodeType?.ToLower() : string.Empty;
                                var isbarcode = GenerateQrCode(tourCMSSupplierCode, $"{bookingDataOthers.BookingRefNo?.Trim()}{productCount}", barcodeCheck);

                                if (!isbarcode)
                                {
                                    attachmentDetails.Replace("##QRCode##", $"<img src=\"{ConfigurationManagerHelper.GetValuefromAppSettings(Constant.WebAPIBaseUrl)}/wwwroot/QRCodes/{bookingDataOthers.BookingRefNo?.Trim()}{productCount}.png\" width=\"105\" height=\"105\">");
                                    attachmentDetails.Replace("##QRCodeValue##", tourCMSSupplierCode);
                                }
                            }
                            else
                            {
                                bookedProductDetail.QrCode = bookedProductDetail?.QrCode?.Split('~')?.LastOrDefault() ?? bookedProductDetail?.QrCode;

                                var isbarcode = GenerateQrCode(bookedProductDetail.QrCode, $"{bookingDataOthers.BookingRefNo?.Trim()}{productCount}");
                                if (!isbarcode)
                                {
                                    attachmentDetails.Replace("##QRCode##", $"<img src=\"{ConfigurationManagerHelper.GetValuefromAppSettings(Constant.WebAPIBaseUrl)}/QRCodes/{bookingDataOthers.BookingRefNo?.Trim()}{productCount}.png\" width=\"105\" height=\"105\">");
                                    attachmentDetails.Replace("##QRCodeValue##", bookedProductDetail?.QrCode);
                                }
                            }

                            if (iscancelled)
                            {
                                attachmentDetails.Replace("##QRCodeTable##", GenerateCancelledQRCode());
                            }
                            else
                            {
                                string generatedQRCode = string.Empty;
                                var qrcodes = new List<BarCodeData>();
                                var qdata = new BarCodeData
                                {
                                    BarCode = bookedProductDetail.QrCode,
                                    IsResourceApply = false,
                                    BookedOptionId = bookedProductDetail.BookedOptionId
                                };
                                if (bookedProductDetail?.ApiTypeId == "16") //TourCMS =16
                                {
                                    //qdata.CodeType = bookedProductDetail.QRCodeType?.ToUpper() == "STRING" ? "" : bookedProductDetail.QRCodeType;
                                    qdata.CodeType = bookedProductDetail.QRCodeType?.ToUpper() == Constant.IsangoQrCode || bookedProductDetail.QRCodeType?.ToUpper() == "STRING" ? "" : bookedProductDetail.QRCodeType;
                                }
                                qrcodes.Add(qdata);

                                if (qrcodes.Any(x => IsBarcode(x.CodeType) != true))
                                {
                                    if (bookedProductDetail?.ApiTypeId == "16") //TourCMS =16
                                    {
                                        generatedQRCode = GenerateMultipleQRCodePerPaxTypeTourCMS(qrcodes, bookingDataOthers.BookingRefNo?.Trim());
                                    }
                                    else
                                    {
                                        generatedQRCode = GenerateMultipleQRCodePerPaxType(qrcodes, bookingDataOthers.BookingRefNo?.Trim());
                                    }
                                    attachmentDetails.Replace("##QRCodeTable##", generatedQRCode);
                                    attachmentDetails.Replace("##BarcodeRowSection##", string.Empty);
                                }
                                else
                                {
                                    attachmentDetails.Replace("##QRCodeTable##", string.Empty);
                                    var generatedBarCodeHtml = new StringBuilder();
                                    var supRefNumber = (bookedProductDetail?.FileNumber)?.Trim() ?? string.Empty;
                                    var html = GetBarcodeHtmlRows(qrcodes, bookingDataOthers, resourceManager, cultureInfo);
                                    generatedBarCodeHtml.Append(html);
                                    if (string.IsNullOrWhiteSpace(supRefNumber))
                                    {
                                        attachmentDetails.Replace("##BarcodeRowSection##", string.Empty);
                                    }
                                    else
                                    {
                                        attachmentDetails.Replace("##BarcodeRowSection##", generatedBarCodeHtml.ToString());
                                        attachmentDetails.Replace("##w100IfBarCode##", "width:100% !iportant");
                                        attachmentDetails.Replace("##displayQR##", "none");
                                        attachmentDetails.Replace("##displayForNoQR##", "display:flex");
                                        attachmentDetails.Replace("##w50##", "width: 49%;margin-right: 1%");
                                        attachmentDetails.Replace("##leftBorder##", "border-left: 1px dashed #CCC;");
                                        attachmentDetails.Replace("##paddingIfQr##", "padding-left:15pt;");
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        attachmentDetails.Replace("##QRCodeTable##", string.Empty);
                        attachmentDetails.Replace("##QRCode##", "N/A");
                        attachmentDetails.Replace("##displayQR##", "none");
                        attachmentDetails.Replace("##displayForNoQR##", "display:flex");
                        attachmentDetails.Replace("##leftBorder##", "border-left: 1px dashed #CCC;");
                        attachmentDetails.Replace("##paddingIfQr##", "padding-left:15pt;");
                    }
                }

                var ProductStatus = bookedProductDetail.BookedOptionStatusName.ToLower().Contains("confirm") ? resourceManager.GetString("Confirmed", cultureInfo)
                                    : bookedProductDetail.BookedOptionStatusName.ToLower().Contains("cancel") ? resourceManager.GetString("Cancelled", cultureInfo)
                                    : bookedProductDetail.BookedOptionStatusName;

                if (bookedProductUsefulDownloads?.Count > 0)
                {
                    var usefulDownloadsLabel = $"{resourceManager.GetString("UsefulDownload", cultureInfo)}";

                    //##Test code #TODO UsefulDownloads Remove it
                    /*
                    bookedProductUsefulDownloads.Add(bookedProductUsefulDownloads.FirstOrDefault());
                    bookedProductUsefulDownloads.Add(bookedProductUsefulDownloads.FirstOrDefault());
                    bookedProductUsefulDownloads.Add(bookedProductUsefulDownloads.FirstOrDefault());
                    bookedProductUsefulDownloads.Add(bookedProductUsefulDownloads.FirstOrDefault());
                    */

                    if (bookedProductUsefulDownloads.Count > 1)
                    {
                        usefulDownloadsLabel = $"{resourceManager.GetString("UsefulDownloads", cultureInfo)}";
                    }

                    var usefulDownloadsSection = $"<tr><td width='130' style='width: 100pt;vertical-align:top;width:120pt;font-size: 9.75pt;line-height:15px !important;'><b>{usefulDownloadsLabel}:</b></td><td>";

                    var i = 0;

                    var separator = "";

                    foreach (var item in bookedProductUsefulDownloads)
                    {
                        if (i > 0)
                        {
                            separator = "<span style='width:1px;height:15px;display:inline-block;margin-left: 15px;margin-right: 15px;border-left: 1px solid #ccc;vertical-align:middle;'>&nbsp;</span>";
                            //style = "style='text-decoration: underline;font-family: Roboto,san-sarif; font-size: 14px; line-height:17px; color: #19A4C2; display:inline-block; margin-right: 15px; border-left: 1px solid #ccc; padding-left: 15px;'";
                        }

                        var arr = item?.DownloadLink?.Split('/');
                        if (arr?.Length > 1)
                        {
                            var last = item.DownloadLink.Split('/').LastOrDefault();
                            var urlEncoded = last?.Replace(" ", "%20") ?? string.Empty;
                            var updatedurlEncoded = item.DownloadLink.Replace(last, urlEncoded);
                            item.DownloadLink = updatedurlEncoded;
                        }
                        usefulDownloadsSection += $"{separator} <a style='font-family: Arial,san-sarif;font-size: 9.75pt; line-height:17px; color: #19A4C2; display:inline-block;vertical-align:middle;' href='{item.DownloadLink}'><img src='https://marketing.isango.com/newsletters/2022/common/download.png' style='display:inline-block;vertical-align:middle;margin-right: 10px;' width='14' height='14' alt='' /> {item.DownloadText}</a>";
                        i++;
                    }

                    usefulDownloadsSection += "</td></tr>";

                    attachmentDetails.Replace("##GoogleMap##", String.Empty);
                    attachmentDetails.Replace("##UsefulDownloadsSection##", usefulDownloadsSection);
                }
                else
                {
                    attachmentDetails.Replace("##UsefulDownloadsSection##", String.Empty);
                    attachmentDetails.Replace("##GoogleMap##", String.Empty);

                    /*
                    if (!string.IsNullOrEmpty(bookedProductDetail.Coordinates))
                    {
                        attachmentDetails.Replace("##GoogleMap##", $"<a href='https://maps.google.com/?q={bookedProductDetail.Coordinates}'><img style='width:100% !important;height:auto;' width='100%' height='auto' src='https://maps.googleapis.com/maps/api/staticmap?zoom=13&size=300x300&maptype=roadmap&markers=color:red%7Clabel:C%7C{bookedProductDetail.Coordinates}&key=AIzaSyC_ap_EGp_182vL00x9KkOn48OUDtxifUw' alt=''>");
                    }
                    else
                    {
                        attachmentDetails.Replace("##GoogleMap##", "");
                    }
                    */
                }

                if (!string.IsNullOrEmpty(bookedProductDetail?.FileNumber))
                {
                    var supplierRefNumber = bookedProductDetail?.FileNumber;
                    if (!string.IsNullOrEmpty(bookedProductDetail?.OfficeCode)
                        && bookedProductDetail?.ApiTypeId == "3")
                    {
                        var supplierLineNumber = string.Empty;

                        try
                        {
                            var supplierLineNumberSplit = bookedProductDetail?.SupplierLineNumber?.Split('#');
                            var supplierOptionCode1stChar = bookedProductDetail?
                                                            .SupplierOptionCode?
                                                            .Split('-')?
                                                            .FirstOrDefault();
                            if (supplierLineNumberSplit?.Length >= 3)
                            {
                                supplierLineNumber = $"{supplierLineNumberSplit[1]}{supplierOptionCode1stChar}{supplierLineNumberSplit[2]}";
                            }
                        }
                        catch (Exception ex)
                        {
                            Task.Run(() =>
                              _log.Error(
                                              new Entities.IsangoErrorEntity
                                              {
                                                  ClassName = @"MailAttachementService",
                                                  MethodName = @"GeneratePdfForBooking",
                                                  Params = bookedProductDetail?.SupplierLineNumber
                                              }, ex
                                        )
                              );
                        }

                        if (!string.IsNullOrWhiteSpace(bookedProductDetail.OfficeCode))
                        {
                            if (supplierRefNumber?.Split('-')?.FirstOrDefault() != bookedProductDetail?.OfficeCode)
                            {
                                supplierRefNumber = $"{bookedProductDetail.OfficeCode}-{supplierRefNumber}";
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(supplierLineNumber))
                        {
                            supplierRefNumber = $"{supplierRefNumber}-{supplierLineNumber}";
                        }
                    }

                    if (!string.IsNullOrEmpty(supplierRefNumber))
                    {
                        attachmentDetails.Replace("##SupplerReferenceDiv##", "<span>Supplier Reference Id</span><div style='font-size:12px;line-height:16px;padding:10px 0;'><strong>##BookingSupplierVoucherNumber##</strong></div>");
                    }
                    attachmentDetails.Replace("##BookingSupplierVoucherNumber##", supplierRefNumber);
                }
                else
                {
                    attachmentDetails.Replace("##SupplerReferenceDiv##", "");
                }

                if (bookedProductDetail?.PickupOptionId == "1")
                {
                    attachmentDetails.Replace("##displayPickPoint##", "none");
                    attachmentDetails.Replace("##displayDropPoint##", "none");
                }
                else
                {
                    attachmentDetails.Replace("##displayMeetingPoint##", "none");
                    attachmentDetails.Replace("##displayEndPoint##", "none");
                }

                attachmentDetails.Replace("##BookingStatus##", $"({ProductStatus.ToUpperInvariant()})");
                attachmentDetails.Replace("##ProductPageName##", $"{bookedProductDetail.ServiceName?.Trim()}");
                attachmentDetails.Replace("##OptionName##", bookedProductDetail?.OptionName ?? "N/A");
                var trOptionCodeRow = string.Empty;
                var trSupplierCodeRow = string.Empty;

                if (bookedProductDetail.ApiTypeId == (Convert.ToInt32(APIType.Hotelbeds).ToString()))
                {
                    trOptionCodeRow = trOptionCodeRow + "<tr>"
                                                        + $"<td style='vertical-align:top;font-size: 9.75pt;'><b>{resourceManager.GetString("SupplierReferenceNo", cultureInfo)}: </b></td>"
                                                        + "<td style='font-size: 9.75pt;'>##OptionCode##</td>"
                                                    + "</tr>";

                    attachmentDetails.Replace("###OptionCodeRow###", trOptionCodeRow);
                    attachmentDetails.Replace("##OptionCode##", (bookedProductDetail?.OfficeCode + "-" + bookedProductDetail?.FileNumber)?.Trim() ?? "N/A");
                }
                else
                {
                    trOptionCodeRow = trOptionCodeRow + "<tr>"
                                                        + $"<td style='vertical-align:top;font-size: 9.75pt;'><b>{resourceManager.GetString("OptionCode", cultureInfo)}</b></td>"
                                                        + "<td style='font-size: 9.75pt;'>##OptionCode##</td>"
                                                    + "</tr>";

                    attachmentDetails.Replace("###OptionCodeRow###", trOptionCodeRow);
                    attachmentDetails.Replace("##OptionCode##", bookedProductDetail?.SupplierOptionCode?.Trim()?.Split('~')?.FirstOrDefault() ?? "N/A");

                    if (!string.IsNullOrEmpty(bookedProductDetail?.FileNumber))
                    {
                        trSupplierCodeRow = trSupplierCodeRow + "<tr>"
                                                        + $"<td style='vertical-align:top;font-size: 9.75pt;line-height:15px !important;'><b>{resourceManager.GetString("SupplierReferenceNo", cultureInfo)}: </b></td>"
                                                        + "<td style='font-size: 9.75pt;vertical-align:top;'>##SupplierRefCode##</td>"
                                                    + "</tr>";

                        attachmentDetails.Replace("###SupplierRefCodeRow###", trSupplierCodeRow);
                        attachmentDetails.Replace("##SupplierRefCode##", (bookedProductDetail?.FileNumber)?.Trim() ?? "N/A");
                    }
                    else
                    {
                        attachmentDetails.Replace("###SupplierRefCodeRow###", "");
                    }
                }

                attachmentDetails.Replace("##UserName##", bookedProductDetail?.LeadPassengerName ?? "N/A");
                attachmentDetails.Replace("##DateOfTour##", bookedProductDetail?.FromDate.ToString("dd MMM yyyy") ?? "N/A");
                attachmentDetails.Replace("##Schedule##", string.IsNullOrEmpty(bookedProductDetail.Schedule) ? "N/A" : bookedProductDetail.Schedule.Replace("<br />", "\n").Replace("<br>", "\n").Replace("<br/>", "\n").Replace("<b>", "").Replace("</b>", ""));
                attachmentDetails.Replace("##PickupPoint##", string.IsNullOrEmpty(bookedProductDetail.PickupLocation) ? (string.IsNullOrEmpty(bookedProductDetail.ServiceHotelPickup) ? "N/A" : bookedProductDetail.ServiceHotelPickup)
                                             : bookedProductDetail.PickupLocation?.Trim().Replace("<br />", "\n").Replace("<br>", "\n").Replace("<br/>", "\n").Replace("<b>", "").Replace("</b>", "").Replace("<p>", " ").Replace("</p>", " "));
                attachmentDetails.Replace("##DropOffPoint##", string.IsNullOrEmpty(bookedProductDetail.ScheduleReturnDetails) ? "N/A" : bookedProductDetail.ScheduleReturnDetails?.Trim().Replace("<p>", " ").Replace("</p>", " "));
                attachmentDetails.Replace("##Duration##", string.IsNullOrEmpty(bookedProductDetail.Duration) ? "N/A" : bookedProductDetail.Duration.Replace("<br />", "\n").Replace("<br>", "\n").Replace("<br/>", "\n").Replace("<b>", "").Replace("</b>", ""));

                attachmentDetails.Replace("##ImportantInstructions##",
                    string.IsNullOrEmpty(bookedProductDetail.PleaseNote) ?
                    "N/A" :
                    SetInstructions(
                            bookedProductDetail.PleaseNote?.Trim()
                            + $"\r\n{bookedProductDetail.ContractComment}"

                        )
                    );

                var supplierDetails = bookingDataOthers.SupplierDataList?.FirstOrDefault(x => x.BookedOptionId == bookedProductDetail.BookedOptionId);
                var endSupplierDetails = bookingDataOthers.SupplierOrHotelAddressList?.FirstOrDefault(x => x.BookedOptionId == bookedProductDetail.BookedOptionId);

                if (endSupplierDetails != null)
                {
                    attachmentDetails.Replace("##ForOfficialUseOnlyContent##", resourceManager.GetString("ForOfficialUseOnlyContent2", cultureInfo).Replace("##EndSupplierName##", endSupplierDetails.SupplierName));
                }
                else
                {
                    attachmentDetails.Replace("##ForOfficialUseOnlyContent##", resourceManager.GetString("ForOfficialUseOnlyContent", cultureInfo));
                }

                attachmentDetails.Replace("##SupplierName##", supplierDetails?.SupplierName?.Trim() ?? "");
                attachmentDetails.Replace("##IsangoPartnerAddress##", $"{supplierDetails?.AddressLine1?.Trim()} {supplierDetails?.AddressLine2?.Trim()} {supplierDetails?.AddressLine3?.Trim()}" ?? ""); //Note: Took first address
                attachmentDetails.Replace("##DestinationAssistance##",
                    endSupplierDetails != null
#pragma warning disable S3358 // Ternary operators should not be nested
                        ? $"<div class=\"contact2\"><h3>{resourceManager.GetString("AtDestinationAssistance", cultureInfo)}{(!languageCode.Equals("EN") ? "<br><em style=\"font-weight: normal\">At destination assistance</ em>" : "")}</h3><p><b class=\"ltBlue\">{resourceManager.GetString("ActivityOperator", cultureInfo)}</b>{(!languageCode.Equals("EN") ? "<br><em>Activity operator</em>" : "")}<br>{endSupplierDetails?.SupplierName?.Trim() ?? supplierDetails?.SupplierName?.Trim()}</p><p><b>{resourceManager.GetString("TelephoneNumber", cultureInfo)}</b><br>{endSupplierDetails?.TelephoneNumber?.Trim() ?? supplierDetails?.TelephoneNumber?.Trim()}</p></div>"
#pragma warning restore S3358 // Ternary operators should not be nested
                        : "");
                attachmentDetails.Replace("##TelephoneNumber##", endSupplierDetails?.TelephoneNumber?.Trim() ?? supplierDetails?.TelephoneNumber?.Trim() ?? "N/A");
                attachmentDetails.Replace("##EmergencyNumber##", endSupplierDetails?.EmergencyTelephoneNumber?.Trim() ?? supplierDetails?.EmergencyTelephoneNumber?.Trim() ?? "N/A");
                attachmentDetails.Replace("##Fax##", endSupplierDetails?.FaxNumber?.Trim() ?? supplierDetails?.FaxNumber?.Trim() ?? "N/A");
                attachmentDetails.Replace("##Email##", endSupplierDetails?.EmailAddress?.Trim() ?? supplierDetails?.EmailAddress?.Trim() ?? "N/A");
                attachmentDetails.Replace("##CustomerSupportNumbers##", string.IsNullOrEmpty(bookingDataOthers.CustomerServiceNo) ? "N/A" : bookingDataOthers.CustomerServiceNo);
                attachmentDetails.Replace("##SupportEmail##", bookingDataOthers?.CompanyEmail ?? "N/A");
                attachmentDetails.Replace("???", "");
                attachmentDetails.Replace("##TermsAndConditionLink##", bookingDataOthers?.TermsAndConditionLink?.Trim() ?? "N/A"); // Added to fix TnC Link issue in Voucher - Sanjay

                if (string.IsNullOrEmpty(supplierDetails?.TelephoneNumber) && string.IsNullOrEmpty(supplierDetails?.EmergencyTelephoneNumber))
                {
                    attachmentDetails.Replace("##displaySupportAtDestination##", "none !important");
                }
                else if (string.IsNullOrEmpty(supplierDetails?.TelephoneNumber))
                {
                    attachmentDetails.Replace("##displayTelephoneNumber##", "none !important");
                }
                else if (string.IsNullOrEmpty(supplierDetails?.EmergencyTelephoneNumber))
                {
                    attachmentDetails.Replace("##displayEmergencyNumber##", "none !important");
                }
                attachmentDetails.Replace("###BookableAndPayableBySection###", string.Empty);
                attachmentDetails.Replace("###ProviderInformationSection###", string.Empty);
                attachmentDetails.Replace("###SupplierRefCodeRow###", string.Empty);
                attachmentDetails.Replace("##BarcodeRowSection##", string.Empty);
                attachmentDetails.Replace("##BookingVoucherNumber##", bookingDataOthers?.BookingRefNo);

                string finalHTML = "";

                finalHTML = "<html lang=\"en\">"
                                + "<head>"
                                    + "<meta charset=\"UTF-8\">"
                                    + "<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">"
                                    + "<meta http-equiv=\"X-UA-Compatible\" content=\"ie=edge\">"
                                    + "<title>isango! - experience the world with us</title>"
                                    + "<link href=\"" + ConfigurationManagerHelper.GetValuefromAppSettings(Constant.WebAPIBaseUrl) + "/CSS/Voucher_V2.css\" rel=\"stylesheet\" />"
                                + "</head>"
                                + "<body>"
                                    + (isPDFVoucher ? htmlFooter : string.Empty)
                                    + (isPDFVoucher ? htmlHeader : string.Empty)
                                    + "<div class=\"mainBody\"><table>"
                                    + "<thead><tr><td> <div style=\"height:"
                                    + (isPDFVoucher ? "0pt;" : "115pt;")
                                    + "width:100%;\" class=\"headerSpace\">&nbsp;</div></td></tr></thead>"
                                    + "<tbody><tr><td>"
                                    + attachmentDetails
                                    //+ (isPDFVoucher ? "<div  style='clear: both;page-break-before: always;'></div>" : string.Empty)
                                    //+ paymentSummaryBuilder
                                    + "</td></tr></tbody>"
                                    //+ "<tfoot><tr><td> <div style=\"height:"
                                    //+ (isPDFVoucher ? "0pt;" : "100pt;")
                                    //+ "width:100%;\" class=\"footerSpace\">&nbsp;</div></td></tr></tfoot>
                                    + "</table></div>"
                                //+ (isPDFVoucher ? htmlFooter : string.Empty)
                                + "</body>"
                            + "</html>";

                _log.Warning($"MailAttachmentService|GeneratePdfForBooking|{finalHTML}");

                return Tuple.Create(finalHTML.Replace("???", ""), htmlHeader, htmlFooter);
            }
            else
            {
                return Tuple.Create(string.Empty, htmlHeader, htmlFooter);
            }
        }

        private string GeneratePDFProductSection(string paxString, ResourceManager resourceManager, CultureInfo cultureInfo)
        {
            var text = "<tr>"
                    + "<td height='7pt' style='height: 7pt;border-top: 1px solid #CCC'></td>"
                    + "</tr>"
                    + "<tr>"
                    + "<td style='vertical-align:top;'>"
                        + "<table width='100%' style='width:100%;border-collapse: separate !important;overflow:hidden;'>"
                        + "##BarcodeRowSection##"
                            + "<tr><td height='7pt' style='height: 7pt;></td></tr>"
                            + "<tr>"
                                + "<td width='65%' style='padding: 0;vertical-align:top;width:65%;##w100IfBarCode##'>"
                                     + "<table width='100%' style='width:100%;'>"
                                        //+ "<tr>"
                                        //    + "<td class='vspcr'>&nbsp;</td>"
                                        //+ "</tr>"
                                        //+ "<tr>"
                                        //    + "<td height='24' style='height: 13pt;text-transform: uppercase;vertical-align: top;'>"
                                        //         + "<span class='txt-primary' style='font-size: 12pt;'>##BookingVoucherNumber##</span>"
                                        //     + "</td>"
                                        //+ "</tr>"
                                        + "<tr>"
                                            + "<td style='vertical-align:top;padding:0 20px 16px 0;'>"
                                                + $"<h3 style='color: #333;font-size: 11pt;line-height:120%'><b style='color: #21212180;'>{resourceManager.GetString("BookingRefNo", cultureInfo) + ": " + "<span style='color:#19A4C2;text-transform: uppercase;'>##BookingVoucherNumber##</span>"}</b> <b>##BookingStatus##</b></h3>"
                                              + "</td>"
                                        + "</tr>"
                                        + "<tr>"
                                            + "<td style='vertical-align:top;padding:0 20px 0 0;'>"
                                                + "<h1 style='color: #333;font-size: 13pt;line-height:120%;'>##ProductPageName##</h1>"
                                            + "</td>"
                                        + "</tr>"
                                        + "<tr>"
                                            + "<td class='vat' style='box-sizing:border-box; padding: 10px 20px 0 0'>"
                                                + "<div style='width:100%;##displayForNoQR##' class='fw'>"
                                                    + "<table class='pinfo font14 fw' style='##w50##'>"
                                                        + "<tr>"
                                                            + $"<td width='130' style='width: 100pt;vertical-align:top;font-size: 9.75pt;'><b>{resourceManager.GetString("OptionBooked", cultureInfo)}</b></td>"
                                                             + "<td style='font-size: 9.75pt;'>##OptionName##</td>"
                                                        + "</tr>"
                                                        + "###OptionCodeRow###"
                                                        + "###SupplierRefCodeRow###"
                                                    + "</table>"
                                                    + "<div class='border-bot-dashed' style='width: 100%; margin: 10px 0;display: ##displayQR##;font-size:0.001'></div>"
                                                    + "<table class='pinfo font14 fw' style='##w50##;##leftBorder##'>"
                                                        + "<tr>"
                                                            + $"<td style='vertical-align:top;font-size: 9.75pt;##paddingIfQr##'><b>{resourceManager.GetString("LeadTraveller", cultureInfo)}</b></td>"
                                                            + "<td style='font-size: 9.75pt;'>##UserName##</td>"
                                                        + "</tr>"
                                                        + "<tr>"
                                                            + $"<td width='130' style='width: 100pt;vertical-align:top;font-size: 9.75pt;##paddingIfQr##'><b>{resourceManager.GetString("Guests", cultureInfo)}</b></td>"
                                                             + $"<td style='font-size: 9.75pt;'>{paxString}</td>"
                                                        + "</tr>"
                                                        + "<tr>"
                                                            + $"<td style='vertical-align:top;font-size: 9.75pt;##paddingIfQr##'><b>{resourceManager.GetString("DateOfTour", cultureInfo)}</b></td>"
                                                            + "<td style='font-size: 9.75pt;'>##DateOfTour##</td>"
                                                        + "</tr>"
                                                    //+ "<tr>"
                                                    //    + "<td><b>Extras:</b></td>"
                                                    //    + "<td>##OptionName##</td>"
                                                    //+ "</tr>"
                                                    //+ "<tr>"
                                                    //    + "<td ><b>Special request:</b></td>"
                                                    //    + "<td>##SpecialRequest##</td>"
                                                    //+ "</tr>"
                                                    //+ "<tr>"
                                                    //    + "<td ></td>"
                                                    //+ "</tr>"
                                                    + "</table>"
                                                + "</div>"
                                            + "</td>"
                                        + "</tr>"
                                    + "</table>"
                                + "</td>"
                                + "<td width='35%' style='width: 35%;display: ##displayQR##;box-sizing:border-box;padding-top: 7px;vertical-align:top;text-align:center;'>"
                                     + "<div>##QRCodeTable##</div>"
                                    + "</div>"
                                + "</td>"
                            + "</tr>"
                        + "</table>"
                    + "</td>"
                + "</tr>"
                + "<tr>"
                    + "<td height='25' style='height: 19pt;'></td>"
                + "</tr>"
                 + "<tr>"
                    + "<td height='20' style='height: 15pt;border-top:1px solid #EEE'></td>"
                + "</tr>"
                + "<tr>"
                    + "<td width='100%'>"
                        + "<table width='100%' callpadding='0' cellspacing='0'>"

                            + "<tr>"
                                + "<td height='30' style='height: 23pt;vertical-align: top;font-size:13pt !important;line-height: 100%;' class='tal'>"
                                    + $"<span><b class='txt-secondary'>{resourceManager.GetString("TourSchedule", cultureInfo)}</b></span>"
                                + "</td>"
                            + "</tr>"

                            + "<tr>"
                                + "<td class='vat' style='vertical-align:top;'>"
                                     + "<table class='pinfo font14' callpadding='0' cellspacing='0' style='border-collapse: separate !important;width:100%;overflow:hidden;padding:0'>"

                                        + "<tr>"
                                            + "<td style='vertical-align:top;width:65%;padding: 0 20px 0 0;box-sizing:border-box;' width='65%' class='infoClass'>"
                                                + "<table style='width:100%;' width='100%'>"
                                                + "##UsefulDownloadsSection##"
                                                + "<tr style='display: ##displayStartTime##;' >"
                                                        + $"<td style='vertical-align:top;font-size: 9.75pt;line-height:175% !important;'><b>{resourceManager.GetString("StartTime", cultureInfo)}</b></td>"
                                                        + "<td style='vertical-align:top;font-size: 9.75pt;line-height:175% !important;'>##Schedule##</td>"
                                                    + "</tr>"

                                                    + "<tr style='display: ##displayPickPoint##;'>"
                                                        + $"<td width='130' style='width: 100pt;vertical-align:top;width:120pt;font-size: 9.75pt;line-height:175% !important;'><b>{resourceManager.GetString("PickupPoint", cultureInfo)}</b></td>"
                                                        + "<td style='font-size: 9.75pt;line-height:175% !important;vertical-align:top;' class='pickupColor'>##PickupPoint##</td>"
                                                    + "</tr>"
                                                    + "<tr style='display: ##displayDropPoint##;'>"
                                                        + $"<td style='vertical-align:top;font-size: 9.75pt;line-height:175% !important;'><b>{resourceManager.GetString("DropOffPoint", cultureInfo)}</b></td>"
                                                        + "<td style='font-size: 9.75pt;line-height:175% !important;vertical-align:top;' class='pickupColor'>##DropOffPoint##</td>"
                                                    + "</tr>"
                                                    + "<tr style='display: ##displayMeetingPoint##;'>"
                                                        + $"<td style='vertical-align:top;width:120pt;font-size: 9.75pt;line-height:175% !important;'><b>Meeting Point:</b></td>"
                                                        + "<td style='font-size: 9.75pt;line-height:175% !important;vertical-align:top;' class='pickupColor'>##PickupPoint##</td>"
                                                    + "</tr>"
                                                    + "<tr style='display: ##displayEndPoint##;'>"
                                                        + $"<td style='vertical-align:top;font-size: 9.75pt;line-height:175% !important;'><b>Ending Point:</b></td>"
                                                        + "<td style='font-size: 9.75pt;line-height:175% !important;vertical-align:top;' class='pickupColor'>##DropOffPoint##</td>"
                                                    + "</tr>"
                                                    + "<tr>"
                                                        + $"<td style='vertical-align:top;font-size: 9.75pt;line-height:175% !important;'><b>{resourceManager.GetString("Duration", cultureInfo)}</b></td>"
                                                        + "<td style='font-size: 9.75pt;line-height:175% !important;vertical-align:top;'>##Duration##</td>"
                                                    + "</tr>"

                                                + "</table>"
                                            + "</td>"
                                        /*
                                        + "<td width='35%' style='vertical-align:top;width:35%;padding: 7px 0 0'>"
                                            + "<table callpadding='0' cellspacing='0' width='100%' style='border-collapse: separate !important;width:100%;'>"
                                                + "<tr>"
                                                    + "<td width='100%' style='width:100%;background:#F5F5F5 !important;overflow:hidden;border-radius:12px;box-sizing:border-box;padding:0 !important;'>"
                                                    + "##GoogleMap##"
                                                    + "</td>"
                                                + "</tr>"
                                            + "</table>"
                                        + "</td>"
                                        */
                                        + "</tr>"
                                    + "</table>"
                                + "</td>"
                                + "</tr>"
                        + "</table>"
                    + "</td>"
                + "</tr>"
                + "<tr>"
                    + "<td height='25' style='height: 19pt;'></td>"
                + "</tr>"
                 + "<tr>"
                    + "<td height='20' style='height: 15pt;border-top:1px solid #EEE'></td>"
                + "</tr>"
                + "<tr>"
                  + "<td width='100%' style='width:100%;' style='padding-bottom: 10px;'>"
                    + "<table width='100%' style='width:100%;' callpadding='0' cellspacing='0'>"
                        + "<tr>"
                            + "<td height='30' style='height: 23pt;vertical-align: top;font-size:13pt !important;line-height: 100%;' class='tal'>"
                                + $"<span><b class='txt-secondary'>{resourceManager.GetString("ImportantInstructions", cultureInfo)}</b></span>"
                            + "</td>"
                        + "</tr>"
                        + "<tr>"
                            + "<td width='100%' style='width:100%;vertical-align:top;padding:0;'>"
                                + "<table width='100%' style='border-collapse: separate !important;width: 100%;' callpadding='0' cellspacing='0'>"
                                    + "<tr>"
                                        + "<td style='width:65%;padding: 0 20px 0 0;box-sizing:border-box;vertical-align:top;' width='65%'>"
                                            + "<table class='pinfo' width='100%' style='width:100%;' callpadding='0' cellspacing='0'>"
                                            + "<tr>"
                                                + $"<td class='font20' style='vertical-align:top;'><b style='line-height:175% !important'>{resourceManager.GetString("Pleasenotes", cultureInfo)}</b></td>"
                                            + "</tr>"
                                                + "<tr>"
                                                    + "<td style='line-height:175% !important;vertical-align:top;'>"
                                                        + "<table class='font10' style='line-height:175% !important' callpadding='0' cellspacing='0'>"
                                                            + "##ImportantInstructions##"
                                                        + "</table>"
                                                    + "</td>"
                                                + "</tr>"
                                                + "<tr>"
                                                    + "<td style='height: 7pt' height='7pt'></td>"
                                                + "</tr>"
                                                + "###ProviderInformationSection###"
                                                + "<tr>"
                                                    + $"<td class='font20'><b style='line-height:175% !important'>{resourceManager.GetString("TermsAndConditions", cultureInfo)}</b></td>"
                                                + "</tr>"
                                                + "<tr>"
                                                    + "<td class='font10' style='line-height:175% !important'>"
                                                        + $"{resourceManager.GetString("TermsAndConditionsContent", cultureInfo)}"
                                                    + "</td>"
                                                + "</tr>"
                                                + "<tr>"
                                                    + $"<td class='font10'><b>{resourceManager.GetString("ForOfficialUseOnly", cultureInfo)}</b> </td>"
                                                + "</tr>"
                                                + "<tr>"
                                                    + "<td class='font10'>"
                                                        + "##ForOfficialUseOnlyContent##"
                                                    + "</td>"
                                                + "</tr>"
                                            //+ "<tr>"
                                            //    + "<td class='font10'>"
                                            //        + $"{resourceManager.GetString("ForOfficialUseOnlyContent2", cultureInfo)}"
                                            //    + "</td>"
                                            //+ "</tr>"
                                            + "</table>"
                                        + "</td>"
                                        + "<td class='vat map' style='width: 35%;padding: 7px 0 0;vertical-align:top;' width='35%;'>"
                                            + "<div style='background:#F5F5F5 !important;border-radius:12px;overflow:hidden;vertical-align:top;padding: 10px 20px;width: 100%;'>"
                                                + "<table style='border-collapse: separate !important;width: 100%;' callpadding='0' cellspacing='0'>"
                                                    + "<tr><td style='vertical-align:top;'>"
                                                        + "<table class='asst' callpadding='0' cellspacing='0'>"
                                                            + "<tr>"
                                                                + $"<td class='font20'><b style='line-height:175% !important'>{resourceManager.GetString("NeedAssistanceHeading", cultureInfo)}</b></td>"
                                                                + "</tr>"
                                                            + "<tr>"
                                                                + "<td class='vspcr' style='height: 7px;font-size: 1px;line-height:1px' height='7'>&nbsp;</td>"
                                                            + "</tr>"
                                                            + "<tr>"
                                                                + $"<td class='font16' style='line-height:175% !important;color:#19A4C2;'>{resourceManager.GetString("SupportAtDestination", cultureInfo)}</td>"
                                                                + "</tr>"
                                                            + "<tr style='display:##displaySupportAtDestination##'>"
                                                                + "<td class='font16'>"
                                                                    + "<p style='margin:0;padding:0;line-height:175% !important;display:##displayTelephoneNumber##' class='font16'>"
                                                                        + $"<b>{resourceManager.GetString("TelephoneNumber", cultureInfo)} - </b><span> ##TelephoneNumber##</span>"
                                                                    + "</p>"
                                                                    + "<p style='margin:0;padding:0;line-height:175% !important;margin-top:1pt; display:##displayEmergencyNumber##' class='font16'>"
                                                                            + $"<b>{resourceManager.GetString("EmergencyNumber", cultureInfo)} - </b>"
                                                                            + "##EmergencyNumber##"
                                                                    + "</p>"
                                                                + "</td>"
                                                            + "</tr>"
                                                            + "<tr>"
                                                                + "<td class='vspcr' style='height: 15px;font-size: 1px;line-height:1px' height='15'>&nbsp;</td>"
                                                            + "</tr>"
                                                            + "<tr>"
                                                                + $"<td class='font16' style='line-height:175% !important;color:#19A4C2;'>{resourceManager.GetString("NeedFurtherAssistance", cultureInfo)}</td>"
                                                            + "</tr>"
                                                            + "<tr>"
                                                                + "<td>"
                                                                    + "<p style='line-height:175% !important' class='font16'>"
                                                                        + $"<strong class='db'>{resourceManager.GetString("IsangoCustomerSupport", cultureInfo)}</strong>"
                                                                            + "##CustomerSupportNumbers##"
                                                                    + "</p>"
                                                                + "</td>"
                                                            + "</tr>"
                                                            + "<tr>"
                                                                + "<td class='vspcr' style='height: 7px;font-size: 1px;line-height:1px' height='7'>&nbsp;</td>"
                                                            + "</tr>"
                                                            + "<tr>"
                                                                + "<td class='font16' style='line-height:175% !important'><strong class='db'>Email:</strong> ##SupportEmail##</td>"
                                                            + "</tr>"
                                                        + "</table>"
                                                    + "</td>"
                                                + "</tr>"
                                            + "</table>"
                                        + "</div>"
                                    + "</td>"
                                + "</tr>"
                        + "</table>"
                    + "</td>"
                + "</tr>"
            + "<tr>"
                + "<td class='vspcr' style='height: 20px;font-size: 1px;line-height:1px' height='20'>&nbsp;</td>"
            + "</tr>"
            + "###BookableAndPayableBySection###"
            ;
            return text;
        }

        private bool URLExists(string url)
        {
            bool urlexists = true;
            var urlCheck = new Uri(url);
            var request = WebRequest.Create(urlCheck);
            request.Timeout = 15000;

            WebResponse response;
            try
            {
                response = request.GetResponse();
            }
            catch (Exception)
            {
                urlexists = false; //url does not exist
            }
            return urlexists;
        }

        /// <summary>
        /// Add currency symbol
        /// </summary>
        /// <param name="bookingDataOthers"></param>
        /// <param name="preCurrency"></param>
        /// <param name="postCurrency"></param>
        private static void PrePostCurrency(BookingDataOthers bookingDataOthers, out string preCurrency, out string postCurrency)
        {
            List<string> postCurrencies = null;
            preCurrency = string.Empty;
            postCurrency = string.Empty;

            if (ConfigurationManagerHelper.GetValuefromAppSettings(Constant.PostCurrencyLanguages) != null)
                postCurrencies = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.PostCurrencyLanguages).Split(',').ToList();

            if (postCurrencies != null && postCurrencies.Contains(bookingDataOthers?.LanguageCode?.ToLowerInvariant()))
                postCurrency = $" {bookingDataOthers?.CurrencySymbol}"; //Note: Don't remove space, it is needed
            else
                preCurrency = $" {bookingDataOthers?.CurrencySymbol}";
        }

        /// <summary>
        /// Set instructions
        /// </summary>
        /// <param name="instructions"></param>
        /// <returns></returns>
        private string SetInstructions(string instructions)
        {
            var instructionStringBuilder = new StringBuilder();

            string[] str = { "\n", "\r\n" };
            var notes = instructions.Replace("<br>", "\n").Replace("<br/>", "\n").Replace("<br />", "\n").Replace("<b>", "").Replace("</b>", "").Replace("<p>", " ").Replace("</p>", " ").Trim().TrimStart('\r', '\n').TrimEnd('\r', '\n').Split(str, StringSplitOptions.RemoveEmptyEntries);
            var bullet = 1;

            foreach (var note in notes)
            {
                if (note.Trim().Length > 0 && note.Trim() != "")
                {
                    instructionStringBuilder.Append($"<tr><td style=\"width:14pt;vertical-align:top !important;padding-top: 5px;\"><i class=\"bullet\" style=\"margin:5px 0 0 !important;display:inline-block; font-weight:bold;font-style:normal;\">&#8226;</i></td> <td style=\"vertical-align:middle !important;line-height:175% !important;\">  <span>{note}</span></td></tr>");
                }
                bullet++;
            }

            return instructionStringBuilder.ToString();
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

        private string PaxInfoMultiLingual(ResourceManager resourceManager, CultureInfo cultureInfo, string text)
        {
            var returnPax = string.Empty;
            var ageband = text?.Split('(')?.LastOrDefault();

            if (!string.IsNullOrWhiteSpace(ageband) && !ageband.Contains('(') && ageband.Contains(')'))
            {
                ageband = $"({ageband}";
            }

            if (text.ToLowerInvariant().Contains("adult"))
            {
                returnPax = resourceManager.GetString("adult", cultureInfo);
            }
            else if (text.ToLowerInvariant().Contains("child"))
            {
                returnPax = resourceManager.GetString("child", cultureInfo);
            }
            else if (text.ToLowerInvariant().Contains("infant"))
            {
                returnPax = resourceManager.GetString("infant", cultureInfo);
            }
            else if (text.ToLowerInvariant().Contains("senior"))
            {
                returnPax = resourceManager.GetString("senior", cultureInfo);
            }
            else if (text.ToLowerInvariant().Contains("youth"))
            {
                returnPax = resourceManager.GetString("youth", cultureInfo);
            }
            else if (text.ToLowerInvariant().Contains("family"))
            {
                returnPax = resourceManager.GetString("family", cultureInfo);
            }
            else
            {
                returnPax = text;
            }

            if (!string.IsNullOrWhiteSpace(ageband))
            {
                returnPax += " " + ageband;
            }
            return returnPax;
        }

        private string PaxInfoMultiLingualWithS(ResourceManager resourceManager, CultureInfo cultureInfo, string text)
        {
            var returnPax = string.Empty;
            if (text.ToLowerInvariant().Contains("adult"))
            {
                returnPax = resourceManager.GetString("adults", cultureInfo);
            }
            else if (text.ToLowerInvariant().Contains("child"))
            {
                returnPax = resourceManager.GetString("childs", cultureInfo);
            }
            else if (text.ToLowerInvariant().Contains("infant"))
            {
                returnPax = resourceManager.GetString("infants", cultureInfo);
            }
            else if (text.ToLowerInvariant().Contains("senior"))
            {
                returnPax = resourceManager.GetString("seniors", cultureInfo);
            }
            else if (text.ToLowerInvariant().Contains("youth"))
            {
                returnPax = resourceManager.GetString("youths", cultureInfo);
            }
            else if (text.ToLowerInvariant().Contains("family"))
            {
                returnPax = resourceManager.GetString("family", cultureInfo);
            }
            else
            {
                returnPax = text;
            }
            return returnPax;
        }

        private StringBuilder GetBigBusVoucher(List<OthersBookedProductDetail> othersBookedProductDetails, BookingDataOthers bookingDataOthers)
        {
            var voucher = new StringBuilder();
            var bookedProductCount = 1;
            var languageCode = bookingDataOthers?.LanguageCode ?? "en";
            var bigBusVoucherName = $"BigBusVoucher_{languageCode}";
            var templateText = LoadTemplate($"{bigBusVoucherName}");

            var pre = string.Empty;
            var post = string.Empty;
            PrePostCurrency(bookingDataOthers, out pre, out post);

            #region New BigBusApiSupport

            var customerDetails = bookingDataOthers.Customers;
            var adultCount = customerDetails.Where(x => x.IsChild == 0)?.ToList()?.Count ?? 0;
            var childCount = customerDetails.Where(x => x.IsChild == 1)?.ToList()?.Count ?? 0;
            var adultString = adultCount == 0 ? string.Empty : (adultCount > 1 ? $"{adultCount} Adults" : $"{adultCount} Adult");
            var childString = childCount == 0 ? string.Empty : (childCount > 1 ? $"{childCount} Children" : $"{childCount} Child");
            var customerString = $"{adultString} {childString}".TrimEnd();

            #endregion New BigBusApiSupport

            for (var i = 0; i < bookingDataOthers.BarCodeList.Count; i++)
            {
                var barCodeData = bookingDataOthers.BarCodeList[i];

                if (barCodeData?.FiscalNumber != null)
                {
                    //old big bus api support
                    if (barCodeData?.FiscalNumber?.Trim().ToLower() != "all")
                    {
                        customerString = "1";
                    }
                    var isChild = barCodeData.FiscalNumber.Trim().ToLowerInvariant() == "Child".ToLowerInvariant() ? 1 : 0;
                    var othersBookedProductDetail = othersBookedProductDetails.FirstOrDefault(x => x.BookedOptionId == barCodeData.BookedOptionId);
                    if (othersBookedProductDetail != null)
                    {
                        var customerDetail = customerDetails.FirstOrDefault(x => x.BookedOptionId == bookingDataOthers.BarCodeList[i].BookedOptionId && x.IsChild == isChild);
                        customerDetails.Remove(customerDetail);

                        if (othersBookedProductDetail == null)
                        {
                            continue;
                        }
                        var supplierDetails = bookingDataOthers.SupplierDataList?.FirstOrDefault(x => x.BookedOptionId == othersBookedProductDetail.BookedOptionId);
                        var productDetailBuilder = $"{othersBookedProductDetail.ServiceName?.Trim()} - {othersBookedProductDetail.OptionName?.Trim()} ({othersBookedProductDetail.BookedOptionStatusName?.Replace("from Allocation", "").Trim()})";

                        voucher.Append(new StringBuilder(templateText));
                        var qrcodename = $"{bookingDataOthers.BookingRefNo?.Trim()}_{i}";

                        var isbarcode = GenerateQrCode(barCodeData.BarCode, qrcodename);

                        if (!isbarcode)
                        {
                            voucher.Replace("##QRCode##", $"<img src=\"{ConfigurationManagerHelper.GetValuefromAppSettings(Constant.WebAPIBaseUrl)}/QRCodes/{bookingDataOthers.BookingRefNo?.Trim()}_{i}.png\" width=\"105\" height=\"105\">");
                        }
                        voucher.Replace("##SupplierName##", supplierDetails?.SupplierName?.Trim() ?? "N/A");
                        voucher.Replace("##CustomerName##", $"{customerDetail?.FirstName} {customerDetail?.LastName} " ?? "N/A");
                        voucher.Replace("##ProductName##", productDetailBuilder.ToString().Trim() ?? "N/A");
                        voucher.Replace("##DateOfBooking##", Convert.ToDateTime(bookingDataOthers.BookingDate).ToString(Constant.DateFormat));
                        voucher.Replace("##DateOfTravel##", othersBookedProductDetail.FromDate.ToString("dd MMM yyyy"));
                        voucher.Replace("##StartTime##", othersBookedProductDetail.Schedule.Trim() ?? "N/A");
                        var BigBusTourfilepath = Path.Combine(WebRootPath.GetWebRootPath(), Constant.Image, Constant.BigBusTourLogo);
                        var BigBusLogoData = File.ReadAllBytes(BigBusTourfilepath);
                        var BigBusLogoUri = @"data:image/png;base64," + Convert.ToBase64String(BigBusLogoData);
                        voucher.Replace("##BigBusTourLogo##", BigBusLogoUri);

                        var isangoLogoData = File.ReadAllBytes(isangoFilePath);
                        var isangoLogoUri = @"data:image/png;base64," + Convert.ToBase64String(isangoLogoData);
                        voucher.Replace("##IsangoLogo##", isangoLogoUri);

                        var bigBusBannerFilePath = Path.Combine(WebRootPath.GetWebRootPath(), Constant.Image, Constant.BigBusAppBanner);
                        var bigBusBannerData = File.ReadAllBytes(bigBusBannerFilePath);
                        var bigBusBannerUri = @"data:image/png;base64," + Convert.ToBase64String(bigBusBannerData);
                        voucher.Replace("##BigBusAppBanner##", bigBusBannerUri);

                        voucher.Replace("##Category##", barCodeData.FiscalNumber.Trim() ?? "N/A");
                        voucher.Replace("##Quantity##", customerString);
                        voucher.Replace("##ShortRef##", othersBookedProductDetail.SupplierLineNumber ?? "N/A");
                    }
                }
            }

            bookedProductCount += 1;

            return voucher;
        }

        /// <summary>
        /// Generate multiple QRcode  per passenger type
        /// </summary>
        /// <param name="barCodeDataList"></param>
        /// <param name="bookingReferenceNumber"></param>
        /// <returns></returns>
        private string GenerateMultipleQRCodePerPaxType(List<BarCodeData> barCodeDataList, string bookingReferenceNumber)
        {
            var barCodeList = barCodeDataList;
            var qrCodeLabelString = new StringBuilder();
            var i = 0;
            var generatedQRCodes = new StringBuilder();

            /*
              "<div class='qrTd' style='background:#F5F5F5 !important;border-radius:12px;overflow:hidden;vertical-align:middle;padding: 15pt 0;width: 100%;text-align:center;'>"
                        + "<span>Supplier Reference Id</span>"
                        +"<div style='font-size:12px;line-height:16px;padding:10px 0;'><strong>##BookingSupplierVoucherNumber##</strong></div>"
                        */
            generatedQRCodes.Append("<div class='qrTd' style='background:#F5F5F5 !important;border-radius:12px;overflow:hidden;vertical-align:middle;padding: 15pt 0;width: 100%;text-align:center;'/>"
                        + "<span>Supplier Reference Id</span>"
                        + $"<div style='padding:10px 0;text-align:center;'><strong style='font-size:12px;line-height:125%;display:inline-block'>{bookingReferenceNumber}</strong></div>"
                        );
            generatedQRCodes.Append("<table style='width: 100%;border: 0 none' cellpadding='0' cellspacing='0'>");

            var tableTr =
                       "<tr><td style='text-align:center;width: 100%;vertical-align: top;'>##QRCode##</td></tr><tr><td style='text-align:center;width: 100%;font-size:12px' valign=\"top\"><b style='text-align: center;'>##QRCodeValue##</b></td></tr>";
            var spacerTr = "<tr><td class=\"spcr\">&nbsp;</td></tr>";

            foreach (var barCodeData in barCodeList)
            {
                if (barCodeList.Count > 1)
                {
                    ++i;
                    qrCodeLabelString.Append(" " + i);
                }
                var fileName = FilterIllegalCharacterFromPath(barCodeData.BarCode);
                var generatedQRCodeImageFile = $"{bookingReferenceNumber}_{barCodeData.BookedOptionId}_{fileName}";

                generatedQRCodes.Append(tableTr);
                if (barCodeData.IsResourceApply)
                {
                    generatedQRCodes.Replace("##QRCode##", $"<img src=\"{barCodeData.ResouceRemoteUrl}\" width=\"auto\" height=\"auto\">");
                }
                else
                {
                    //var ct = (barCodeData?.CodeType?.ToLower() == "string" ? "" : barCodeData?.CodeType);
                    var ct = (barCodeData?.CodeType?.ToLower() == Constant.IsangoQrCode?.ToLower() || barCodeData?.CodeType?.ToLower() == "string" ? "" : barCodeData?.CodeType);
                    if (String.IsNullOrEmpty(ct))
                    {
                        ct = string.Empty;
                    }
                    var isBarcode = GenerateQrCode(barCodeData.BarCode, generatedQRCodeImageFile, ct);
                    if (isBarcode)
                    {
                        generatedQRCodes.Replace("##QRCode##", $"<img src=\"{ConfigurationManagerHelper.GetValuefromAppSettings(Constant.WebAPIBaseUrl)}/QRCodes/{generatedQRCodeImageFile}.png\" width=\"300\" height=\"200\">");
                    }
                    else
                    {
                        generatedQRCodes.Replace("##QRCode##", $"<img src=\"{ConfigurationManagerHelper.GetValuefromAppSettings(Constant.WebAPIBaseUrl)}/QRCodes/{generatedQRCodeImageFile}.png\" width=\"105\" height=\"105\">");
                    }
                }
                var codeType = (string.IsNullOrWhiteSpace(barCodeData.CodeType) ? "QR CODE" : barCodeData.CodeType.Replace("_", " "));

                var codeTypeAndValueHtml = $"{codeType} {qrCodeLabelString}" + @"
                                            <table style='overflow-wrap:break-word;word-break:break-word;text-align: center;width: 90%;margin: auto;'>
                                                <tbody>
                                                        <tr>
                                                            <td><div style='overflow-wrap: anywhere;word-break: break-all;padding: 5px 0;'>" +
                                                            (string.IsNullOrWhiteSpace(barCodeData.CodeValue) ? barCodeData.BarCode : barCodeData.CodeValue)
                                                             + "<br/>" + (string.IsNullOrWhiteSpace(barCodeData.ShowValueOnly) ? "" : barCodeData.ShowValueOnly) + @"</div></td>
                                                        </tr>
                                                         <tr>
                                                            <td>" +
                                                                (string.IsNullOrWhiteSpace(barCodeData.PassengerType) ?
                                                                    barCodeData.FiscalNumber : barCodeData.PassengerType)
                                                                + @"
                                                            </td>
                                                        </tr>
                                                    </tbody>
                                            </table> ";

                //generatedQRCodes.Replace("##QRCodeValue##", $"{codeType} {qrCodeLabelString}"
                //    + "<br>" +
                //    (string.IsNullOrWhiteSpace(barCodeData.CodeValue) ? barCodeData.BarCode : barCodeData.CodeValue)
                //    + "<br>" +
                //    (string.IsNullOrWhiteSpace(barCodeData.PassengerType) ? barCodeData.FiscalNumber : barCodeData.PassengerType)

                //    );
                generatedQRCodes.Replace("##QRCodeValue##", codeTypeAndValueHtml);
                generatedQRCodes.Append(spacerTr);
                qrCodeLabelString.Clear();
            }
            generatedQRCodes.Append("</table>");

            return generatedQRCodes.ToString();
        }

        private string GenerateCancelledQRCode()
        {
            var qrCodeLabelString = new StringBuilder();
            var i = 0;
            var generatedQRCodes = new StringBuilder();

            generatedQRCodes.Append("<table style='width: 100%;border: 0 none'><tr><td valign=\"middle\"><td class=\"spcr\">&nbsp;</td></td></tr>");

            var tableTr = $"<tr><td style='text-align:center;width: 100%'><img src='{ConfigurationManagerHelper.GetValuefromAppSettings(Constant.WebAPIBaseUrl)}/Images/cancelled_qr.jpg' width='105' height='105'></td></tr>";
            var spacerTr = "<tr><td class=\"spcr\">&nbsp;</td></tr>";

            generatedQRCodes.Append(tableTr);
            generatedQRCodes.Append(spacerTr);
            qrCodeLabelString.Clear();
            generatedQRCodes.Append("</table>");

            return generatedQRCodes.ToString();
        }

        public StringBuilder UpdateNode(string displayLable, string key, string value, StringBuilder attachmentDetails, string htmlcode, CultureInfo cultureInfo)
        {
            var em = cultureInfo.TwoLetterISOLanguageName?.ToLower() == "en" ? string.Empty : $"<em>{displayLable}</em>";
            if (!string.IsNullOrWhiteSpace(value))
            {
                htmlcode = htmlcode.Replace($"##em##", em);
                htmlcode = htmlcode.Replace($"##key##", displayLable);
                htmlcode = htmlcode.Replace($"##value##", value);
                attachmentDetails.Replace(key, htmlcode);
            }
            else
            {
                htmlcode = string.Empty;
                attachmentDetails.Replace($"##{key}##", htmlcode);
            }
            return attachmentDetails;
        }

        /// <summary>
        /// Split based on , logic
        /// </summary>
        /// <param name="qrCodes"></param>
        /// <param name="bookingRefNo"></param>
        /// <returns></returns>
        private string GenerateMultipleQRCode(string qrCodes, string bookingRefNo)
        {
            var qrCodeList = qrCodes?.Split(',')?.Where(y => !string.IsNullOrEmpty(y)).Select(x => x.Trim())?.ToArray();
            var generatedQRCodes = new StringBuilder();
            generatedQRCodes.Append("<table>");
            var tableTr = "<tr><td>##QRCode##</td></tr><tr><td valign=\"middle\"><b>QR Code</b></td></tr><tr><td valign=\"top\"><b> ##QRCodeValue##</b></td></tr>";
            var spacerTr = "<tr><td class=\"spcr\">&nbsp;</td></tr>";
            for (var i = 0; i < qrCodeList.Length; i++)
            {
                if (qrCodeList[i].Length > 0)
                {
                    var isbarcode = GenerateQrCode(qrCodeList[i], $"{bookingRefNo}_{i}");

                    if (!isbarcode)
                    {
                        generatedQRCodes.Append(tableTr);
                        generatedQRCodes.Replace("##QRCode##", $"<img src=\"{ConfigurationManagerHelper.GetValuefromAppSettings(Constant.WebAPIBaseUrl)}/QRCodes/{bookingRefNo}_{i}.png\" width=\"105\" height=\"105\">");
                        generatedQRCodes.Replace("##QRCodeValue##", qrCodeList[i]);
                        generatedQRCodes.Append(spacerTr);
                    }
                }
            }

            generatedQRCodes.Append("</table>");

            return generatedQRCodes.ToString();
        }

        //Commmented code from Redeam Branch need to veridy if exiting is same or not
        ///// <summary>
        ///// Generate multiple QRcode  per pax type
        ///// </summary>
        ///// <param name="bookingDataOthers"></param>
        ///// <returns></returns>
        //private string GenerateMultipleQRCodePerPaxType(List<BarCodeData> barCodeDataList, string bookingReferenceNumber)
        //{
        //    var barCodeList = barCodeDataList;
        //    var qrCodeLabelString = new StringBuilder();
        //    int i = 0;
        //    var generatedQRCodes = new StringBuilder();
        //    generatedQRCodes.Append("<table><tr><td class=\"col1\" valign=\"middle\"><td class=\"spcr\">&nbsp;</td></td></tr>");
        //    var tableTr = "<tr><td class=\"col1\" valign=\"middle\"><table valign=\"top\"><tr><td class=\"col1\"  valign=\"top\"><b> ##QRCodeValue##</b></td></tr></table></td><td class=\"spcr\">&nbsp;</td><td>##QRCode##</td></tr>";
        //    var spacerTr = "<tr><td class=\"spcr\">&nbsp;</td></tr>";
        //    foreach (var barCodeData in barCodeList)
        //    {
        //        if (barCodeList.Count > 1)
        //        {
        //            ++i;
        //            qrCodeLabelString.Append(" " + i);
        //        }

        //        GenerateQrCode(barCodeData.BarCode, $"{bookingReferenceNumber}_{i}");
        //        generatedQRCodes.Append(tableTr);
        //        generatedQRCodes.Replace("##QRCode##", $"<img src=\"{ConfigurationManagerHelper.GetValuefromAppSettings(Constant.WebAPIBaseUrl)}/QRCodes/{bookingReferenceNumber}_{i}.png\" width=\"105\" height=\"105\">");
        //        generatedQRCodes.Replace("##QRCodeValue##", $"QR Code{qrCodeLabelString}" + "<br>" + barCodeData.BarCode + "<br>" + barCodeData.FiscalNumber);
        //        generatedQRCodes.Append(spacerTr);
        //        qrCodeLabelString.Clear();
        //    }
        //    generatedQRCodes.Append("</table>");

        //    return generatedQRCodes.ToString();
        //}

        /// <summary>
        /// Generate multiple QRcode  per passenger type
        /// </summary>
        /// <param name="barCodeDataList"></param>
        /// <param name="bookingReferenceNumber"></param>
        /// <returns></returns>
        private string GenerateMultipleQRCodePerPaxTypeTourCMS(List<BarCodeData> barCodeDataList, string bookingReferenceNumber)
        {
            var barCodeList = barCodeDataList;
            var qrCodeLabelString = new StringBuilder();
            var i = 0;
            var generatedQRCodes = new StringBuilder();

            generatedQRCodes.Append("<table style='width: 100%;border: 0 none'>");

            var tableTr = "<tr><td style='text-align:center;width: 100%;vertical-align: top;'>##QRCode##</td></tr><tr><td style='text-align:center;width: 100%;font-size:12px' valign=\"top\"><b style='text-align: center;'>##QRCodeValue##</b></td></tr>";
            var spacerTr = "<tr><td class=\"spcr\">&nbsp;</td></tr>";

            foreach (var barCodeData in barCodeList)
            {
                var tourCMSQrCode = String.Empty;
                var tourCMSSupplierCode = string.Empty;

                var splitData = barCodeData.BarCode.Split(new string[] { "!!!" }, StringSplitOptions.None);
                if (splitData.Count() > 1)
                {
                    tourCMSSupplierCode = splitData[0];
                    tourCMSQrCode = splitData[1];
                }
                else
                {
                    tourCMSSupplierCode = splitData[0];
                }

                if (barCodeList.Count > 1)
                {
                    ++i;
                    qrCodeLabelString.Append(" " + i);
                }
                var fileName = FilterIllegalCharacterFromPath(tourCMSSupplierCode);
                var generatedQRCodeImageFile = $"{bookingReferenceNumber}_{barCodeData.BookedOptionId}_{fileName}";

                generatedQRCodes.Append(tableTr);
                if (barCodeData.IsResourceApply)
                {
                    generatedQRCodes.Replace("##QRCode##", $"<img src=\"{barCodeData.ResouceRemoteUrl}\" width=\"auto\" height=\"auto\">");
                }
                else
                {
                    var ct = barCodeData?.CodeType?.ToLower() ?? string.Empty;
                    var isBarcode = GenerateQrCode(tourCMSSupplierCode, generatedQRCodeImageFile, ct);
                    if (isBarcode)
                    {
                        generatedQRCodes.Replace("##QRCode##", $"<img src=\"{ConfigurationManagerHelper.GetValuefromAppSettings(Constant.WebAPIBaseUrl)}/QRCodes/{generatedQRCodeImageFile}.png\" width=\"300\" height=\"200\">");
                    }
                    else
                    {
                        generatedQRCodes.Replace("##QRCode##", $"<img src=\"{ConfigurationManagerHelper.GetValuefromAppSettings(Constant.WebAPIBaseUrl)}/QRCodes/{generatedQRCodeImageFile}.png\" width=\"105\" height=\"105\">");
                    }
                }
                var codeType = (string.IsNullOrWhiteSpace(barCodeData.CodeType) ? "QR CODE" : barCodeData.CodeType.Replace("_", " "));

                var codeTypeAndValueHtml = $"{codeType} {qrCodeLabelString}" + @"
                                            <table style='overflow-wrap:break-word;word-break:break-word;text-align: center;width: 90%;margin: auto;'>
                                                <tbody>
                                                        <tr>
                                                            <td><div style='overflow-wrap: anywhere;word-break: break-all;padding: 5px 0;'>" +
                                                            (string.IsNullOrWhiteSpace(barCodeData.CodeValue)
                                                            ?
                                                           string.IsNullOrEmpty(tourCMSSupplierCode) ? tourCMSSupplierCode : tourCMSSupplierCode + "<br>" + tourCMSQrCode
                                                            :
                                                            barCodeData.CodeValue
                                                            )
                                                             + @"</div></td>
                                                        </tr>
                                                        <tr>
                                                            <td>" +
                                                                (string.IsNullOrWhiteSpace(barCodeData.PassengerType) ?
                                                                    barCodeData.FiscalNumber : barCodeData.PassengerType)
                                                                + @"
                                                            </td>
                                                        </tr>
                                                    </tbody>
                                            </table> ";

                generatedQRCodes.Replace("##QRCodeValue##", codeTypeAndValueHtml);
                generatedQRCodes.Append(spacerTr);
                qrCodeLabelString.Clear();
            }
            generatedQRCodes.Append("</table>");

            return generatedQRCodes.ToString();
        }

        public string GetBarcodeHtmlRows(List<BarCodeData> barCodeDataList
            , BookingDataOthers bookingDataOthers
            , ResourceManager resourceManager
            , CultureInfo cultureInfo
        )
        {
            try
            {
                var bookedProductDetail = bookingDataOthers?.BookedProductDetailList?.FirstOrDefault();

                string bookingReferenceNumber = bookingDataOthers.BookingRefNo;
                string supRefNumber = bookedProductDetail.FileNumber;
                var barCodeList = barCodeDataList;
                var qrCodeLabelString = new StringBuilder();
                var i = 0;
                var generatedBarCodeHtml = new StringBuilder();

                #region Barcode html

                var row = "<tr>" +
                "	<td height='7pt' style='height: 7pt;'></td>" +
                "</tr>" +
                "" +
                "<!-- Barcode start -->" +
                "<tr>" +
                "	<td>" +
                "		<table width='100%' style='width:100%;border-collapse: separate;border-radius: 16px;background-color: #F5F5F5;' cellpadding='0' cellspacing='0'>" +
                "			<tr>" +
                "				<td height='25' style='height: 19pt;'>&nbsp;</td>" +
                "			</tr>" +
                "			<tr>" +
                "				<td style='font-size: 20px; font-family: Roboto;letter-spacing: 0px;color: #212121;text-align: center;padding: 3px;'>##Barcode_SupplierReferenceId_Label##</td>" +
                "			</tr>" +
                "			<tr>" +
                "				<td style='font-size: 24px; font-family: Roboto;letter-spacing: 0px;color: #212121;text-align: center;padding: 3px;font-weight: bold'> ##Barcode_SupplierReferenceId_Value##</td>" +
                "			</tr>" +
                "			<tr>" +
                "				<td height='15' style='height: 10pt;'>&nbsp;</td>" +
                "			</tr>" +
                "			<tr>" +
                "				<td style='text-align: center;'><img src='##Barcode_Image_Path##' width='auto' height='auto' style='max-height: 70px;' alt=''></td>" +
                "			</tr>" +
                "			<tr>" +
                "				<td height='15' style='height: 10pt;'>&nbsp;</td>" +
                "			</tr>" +
                "			<tr>" +
                "				<td style='font-size: 20px; font-family: Roboto;letter-spacing: 0px;color: #212121;text-align: center;padding: 2px;'>##Barcode_Title_Label##</td>" +
                "			</tr>" +
                "			<tr>" +
                "				<td style='font-size: 24px; font-family: Roboto;letter-spacing: 0px;color: #212121;text-align: center;padding: 3x;font-weight: bold;'>##Barcode_Title_Value##</td>" +
                "			</tr>" +
                "			<tr>" +
                "				<td height='25' style='height: 19pt;'>&nbsp;</td>" +
                "			</tr>" +
                "		</table>" +
                "	</td>" +
                "</tr>" +
                "<!-- /Barcode end -->" +
                "" +
                "<tr>" +
                "	<td height='15' style='height: 10pt;'>&nbsp;</td>" +
                "</tr>";

                #endregion Barcode html

                foreach (var barCodeData in barCodeList)
                {
                    if (barCodeList.Count > 1)
                    {
                        ++i;
                        qrCodeLabelString.Append(" " + i);
                    }
                    var fileName = FilterIllegalCharacterFromPath(barCodeData.BarCode);
                    var generatedQRCodeImageFile = $"{bookingReferenceNumber}_{barCodeData.BookedOptionId}_{fileName}";

                    generatedBarCodeHtml.Append(row);

                    //var ct = (barCodeData?.CodeType?.ToLower() == "string" ? "" : barCodeData?.CodeType);
                    var ct = (barCodeData?.CodeType?.ToLower() == Constant.IsangoQrCode?.ToLower() || barCodeData?.CodeType?.ToLower() == "string" ? "" : barCodeData?.CodeType);
                    if (String.IsNullOrEmpty(ct))
                    {
                        ct = string.Empty;
                    }
                    var isBarcode = GenerateQrCode(barCodeData.BarCode, generatedQRCodeImageFile, ct);
                    var fullPathCodeImageFile = $"{ConfigurationManagerHelper.GetValuefromAppSettings(Constant.WebAPIBaseUrl)}/QRCodes/{generatedQRCodeImageFile}.png";
                    if (isBarcode)
                    {
                        if (string.IsNullOrWhiteSpace(supRefNumber))
                        {
                            generatedBarCodeHtml.Replace("##Barcode_SupplierReferenceId_Label##", string.Empty);
                            generatedBarCodeHtml.Replace("##Barcode_SupplierReferenceId_Value##", string.Empty);
                            generatedBarCodeHtml.Replace("##Barcode_Title_Label##", string.Empty);
                            generatedBarCodeHtml.Replace("##Barcode_Title_Value##", string.Empty);
                        }
                        else
                        {
                            generatedBarCodeHtml.Replace("##Barcode_SupplierReferenceId_Label##", resourceManager.GetString("SupplierReferenceNo", cultureInfo) + ": ");
                            generatedBarCodeHtml.Replace("##Barcode_SupplierReferenceId_Value##", (bookedProductDetail?.FileNumber)?.Trim());

                            generatedBarCodeHtml.Replace("##Barcode_Image_Path##", fullPathCodeImageFile);

                            generatedBarCodeHtml.Replace("##Barcode_Title_Label##", resourceManager.GetString("Barcode_Title", cultureInfo));
                            generatedBarCodeHtml.Replace("##Barcode_Title_Value##", bookedProductDetail.QrCode);
                        }
                    }
                    else
                    {
                    }

                    qrCodeLabelString.Clear();
                }

                return generatedBarCodeHtml.ToString();
            }
            catch (Exception)
            {
            }
            return string.Empty;
        }

        #endregion Private Methods
    }
}