using HtmlAgilityPack;
using Isango.Entities;
using Isango.Entities.HotelBeds;
using ServiceAdapters.HB.HB.Converters.Contracts;
using ServiceAdapters.HB.HB.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ApiBooking = ServiceAdapters.HB.HB.Entities.Booking.BookingDetail;
using IsangoBooking = Isango.Entities.Booking.Booking;

namespace ServiceAdapters.HB.HB.Converters
{
    public class HBBookingConfirmConverter : ConverterBase, IHbBookingConfirmConverter
    {
        private string _languageCode = string.Empty;
        /// <summary>
        /// Convert API Result Entities to Isnago.Contract.Entities
        /// </summary>
        /// <param name="objectresult"></param>
        /// <returns></returns>

        public object Convert(object apiResponse, MethodType methodType, object criteria = null)
        {
            if (apiResponse != null)
            {
                var bookingConfirmRS = apiResponse as ApiBooking.BookingDetailRs;
                var booking = criteria as IsangoBooking;
                var selectedProducts = ConvertPurchaseResult(bookingConfirmRS, booking);
                return selectedProducts;
            }
            return null;
        }

        private List<HotelBedsSelectedProduct> ConvertPurchaseResult(ApiBooking.BookingDetailRs bookingConfirmRS, IsangoBooking booking)
        {
            if (bookingConfirmRS == null)
            {
                return null;
            }
            var hbsp = booking?.SelectedProducts?.FirstOrDefault(x => x.ProductOptions.FirstOrDefault(y => y.Id == booking.BookingId) != null) as HotelBedsSelectedProduct;

            _languageCode = hbsp?.Language?.ToLower() ?? booking?.Language?.Code?.ToLower();

            if (string.IsNullOrWhiteSpace(_languageCode))
            {
                _languageCode = "eng";
            }

            var selectedProducts = booking.SelectedProducts
                                   .Where(x => x.APIType == Isango.Entities.Enums.APIType.Hotelbeds
                                   && x.ProductOptions.FirstOrDefault(y => y.Id == booking.BookingId) != null
                                   )
                                   .ToList()
                                   .OfType<HotelBedsSelectedProduct>().ToList();

            if (selectedProducts?.Count > 0)
            {
                var isangoBookingData = booking.IsangoBookingData;
                var apiBooking = bookingConfirmRS?.Booking;
                var apiActivities = bookingConfirmRS?.Booking.activities;
                var bookingReference = bookingConfirmRS?.Booking?.reference;
                foreach (var selectedProduct in selectedProducts)
                {
                    var productoptions = selectedProduct?.ProductOptions;
                    if (productoptions?.Count > 0)
                    {
                        foreach (var po in productoptions)
                        {
                            var activityOption = ((Isango.Entities.Activities.ActivityOption)po);

                            var modalityCode = string.Empty;
                            try
                            {
                                var modalityCodeSplit = activityOption?.Code?.Split('#');
                                if (modalityCodeSplit?.Length > 1)
                                {
                                    modalityCode = modalityCodeSplit[1];
                                }
                            }
                            catch
                            {
                            }

                            var apiActivity = apiActivities.FirstOrDefault(x => x.code == po.PrefixServiceCode)
                                ?? apiActivities.FirstOrDefault(x => x.code == po.SupplierOptionCode
                                                                && x.modality?.code?.Split('@')?.FirstOrDefault() == modalityCode
                                                                )
                                ?? apiActivities.FirstOrDefault(x => x.code == po.SupplierOptionCode);

                            selectedProduct.EchoToken = bookingReference;// apiActivity?.activityReference;
                            selectedProduct.PurchaseToken = bookingReference;//apiActivity?.activityReference;

                            selectedProduct.OfficeCode = bookingReference/*;//apiActivity?.activityReference?*/
                                                        .Split('-')?.FirstOrDefault();

                            selectedProduct.FileNumber = bookingReference;//apiActivity?.activityReference;//?.Split('-')?.LastOrDefault();
                            selectedProduct.ServiceStatus = apiActivity?.status;
                            selectedProduct.CartStatus = apiActivity?.status;
                            selectedProduct.SPUI = apiActivity?.id;
                            selectedProduct.VatNumber = apiActivity?.supplier?.vatNumber;
                            selectedProduct.SupplierName = apiActivity?.supplier?.name;
                            selectedProduct.ProviderInformation = $"{ apiActivity?.providerInformation?.name ?? string.Empty} " +
                                $" {apiActivity?.providerInformation?.address ?? string.Empty}" +
                                $" {apiActivity?.providerInformation?.reference ?? string.Empty}";
                            selectedProduct.ProviderInformation = selectedProduct?.ProviderInformation?.Trim();
                            var invoicingCompany = bookingConfirmRS?.Booking?.paymentData?.invoicingCompany;
                            if (invoicingCompany != null)
                            {
                                selectedProduct.InvoicingCompany = $"{invoicingCompany.name}, {invoicingCompany.code}, {invoicingCompany.registrationNumber}";
                            }

                            //selectedProduct.Price = apiBookingActivity.TotalAmount,
                            //selectedProduct.SupplierCurrency = apiBookingActivity.Currency
                            activityOption.Contract.InComingOfficeCode = selectedProduct.OfficeCode;
                            activityOption.BookingStatus = Isango.Entities.Enums.OptionBookingStatus.Confirmed;
                            activityOption.OptionKey = apiActivity?.id;

                            if (apiActivity?.cancellationPolicies != null)
                            {
                                var cancellationCost = new List<CancellationPrice>();

                                activityOption.ApiCancellationPolicy = Util.SerializeDeSerializeHelper.Serialize(apiActivity?.cancellationPolicies);

                                foreach (var cancellationPolicy in apiActivity?.cancellationPolicies)
                                {
                                    DateTime fromDate = cancellationPolicy.dateFrom;
                                    DateTime toDate = cancellationPolicy.dateFrom;

                                    try
                                    {
                                        fromDate = System.Convert.ToDateTime(apiActivity.dateFrom).Date;
                                        toDate = System.Convert.ToDateTime(apiActivity.dateTo).Date;
                                    }
#pragma warning disable CS0168 // Variable is declared but never used
                                    catch (Exception ex)
#pragma warning restore CS0168 // Variable is declared but never used
                                    {
                                    }

                                    var cancellationPrice = new CancellationPrice
                                    {
                                        Percentage = GetCancellationPricePercentage(
                                                System.Convert.ToDecimal(apiBooking.total),
                                                System.Convert.ToDecimal(cancellationPolicy.amount)
                                                ),
                                        CancellationDateRelatedToOpreationDate = fromDate,
                                        CancellationFromdate = cancellationPolicy.dateFrom,
                                        CancellationAmount = System.Convert.ToDecimal(cancellationPolicy.amount),
                                        CancellationToDate = toDate
                                    };
                                    cancellationCost.Add(cancellationPrice);
                                }
                                po.CancellationPrices = cancellationCost;
                            }
                            selectedProduct.IsVocuherCustomizable = null;
                            if (apiActivity?.vouchers != null && apiActivity?.vouchers?.Count > 0)
                            {
                                try
                                {
                                    ProcessVoucher(selectedProduct, apiBooking, apiActivity);
                                }
                                catch (Exception)
                                {
                                    selectedProduct.IsVocuherCustomizable = null;
                                    selectedProduct.BookingVouchers = null;
                                }
                            }
                            try
                            {
                                //selectedProduct.Name = apiActivity?.name ?? selectedProduct.Name;
                                var optionNameFromApi = apiActivity?.name ?? selectedProduct?.Name ?? string.Empty;
                                optionNameFromApi = $"{ optionNameFromApi} - {apiActivity?.modality?.name?.ToUpper()}";

                                var rateDetails = apiActivity?.modality?.rates?.FirstOrDefault()?.rateDetails;
                                var rateDetailLanguage = rateDetails?.FirstOrDefault()?.languages?.FirstOrDefault()?.Code;
                                var rateDetailSesssion = rateDetails?.FirstOrDefault()?.sessions?.FirstOrDefault()?.Name;

                                if (!string.IsNullOrWhiteSpace(rateDetailLanguage))
                                {
                                    optionNameFromApi = $"{ optionNameFromApi} - {rateDetailLanguage?.ToUpper()}";
                                }
                                if (!string.IsNullOrWhiteSpace(rateDetailSesssion))
                                {
                                    optionNameFromApi = $"{ optionNameFromApi} - {rateDetailSesssion}";
                                }
                                activityOption.Name = optionNameFromApi;
                            }
                            catch
                            {
                                //ignored
                                //##TODO add logging here
                            }
                        }
                    }
                }

                //bookingConfirmRS?.Booking?.reference is same as 1st activity in api booking apiActivity?.activityReference
                //Still setting it as in case it may change then from 1st hbProduct we can cancel entire booking
                //var selectedProduct1st = selectedProducts?.FirstOrDefault();
                //var bookingReference = bookingConfirmRS?.Booking?.reference;
                //if (selectedProduct1st != null && !string.IsNullOrWhiteSpace(bookingReference))
                //{
                //    selectedProduct1st.EchoToken = bookingReference;
                //    selectedProduct1st.PurchaseToken = bookingReference;
                //    selectedProduct1st.OfficeCode = bookingReference?.Split('-')?.FirstOrDefault();
                //    selectedProduct1st.FileNumber = bookingReference;//?.Split('-')?.LastOrDefault();
                //}
            }

            return selectedProducts;
        }

        private float GetCancellationPricePercentage(decimal totalPrice, decimal cancellationAmount)
        {
            var result = 0.0F;
            try
            {
                if (totalPrice > 0)
                {
                    var p = (cancellationAmount / totalPrice) * 100;
                    float.TryParse(p.ToString(), out var f);
                    result = f;
                }
            }
            catch (Exception ex)
            {
                //ignored
                //#TODO add logging
                result = 0.0F;
            }
            return result;
        }

        #region Apitude Json Version Changes for Customized HTMLVoucher / HB Default PDFVouhers

        private void ProcessVoucher(HotelBedsSelectedProduct hotelBedsSelectedProduct, ApiBooking.Booking apiBooking, ApiBooking.Activity apiBookingActivity)
        {
            var apiBookingActivityVouchers = apiBookingActivity?.vouchers;

            if (apiBookingActivityVouchers == null)
            {
                return;
            }

            try
            {
                hotelBedsSelectedProduct.BookingVouchers = new List<BookingVoucher>();

                //var voucher = apiBookingActivity?.vouchers?
                //    .Where(cv =>
                //                cv.mimeType.Equals("text/html")
                //          //&& cv?.language?.ToLowerInvariant().Equals(apiBookingActivity.Purchase.Language.ToLowerInvariant())
                //          ).FirstOrDefault();

                //if (voucher == null)
                //{
                var vouchers = apiBookingActivity?.vouchers?
                          .Where(cv =>
                                    cv?.mimeType?.Equals("text/html") == true
                                    &&
                                    cv?.language?.ToLowerInvariant()?.Contains(_languageCode) == true
                                )
                          .ToList();

                if (vouchers?.Count == 0 && _languageCode != "eng")
                {
                    vouchers = apiBookingActivity?.vouchers?
                          .Where(cv =>
                                    cv?.mimeType?.Equals("text/html") == true
                                    &&
                                    cv?.language?.ToLowerInvariant()?.Contains("eng") == true
                                )
                          .ToList();
                }

                //}
                if (vouchers?.Count > 0)
                {
                    foreach (var voucher in vouchers)
                    {
                        if (voucher != null)
                        {
                            #region Download Multiple QrCodes Image  for each pax from Html file and use it in isango voucher

                            var bookingVoucher = new BookingVoucher();
                            bookingVoucher.Language = voucher.language;
                            bookingVoucher.Url = voucher.url;
                            bookingVoucher.Type = BookingVoucherType.HTML;
                            //bookingVoucher.APIType = APIType.HOTELBEDS;

                            List<Tuple<string, string, string, string>> voucherTpls = null;
                            try
                            {
                                voucherTpls = DownloadAndParseHtml(voucher.url, apiBooking.clientReference);
                                var error = voucherTpls?.FirstOrDefault(x =>
                                            x?.Item1?.IndexOf("error", StringComparison.InvariantCultureIgnoreCase) >= 0);

                                if (error == null && voucherTpls?.Any() == true)
                                {
                                    bookingVoucher.DownloadFileNames = voucherTpls.Select(y => new BookingVoucherHtmlORpdf
                                    {
                                        Url = y.Item1,
                                        CodeValue = y.Item2,
                                        DownloadedFile = y.Item3,
                                        QRCodeType = BookingVoucherQRCodeType.HTMLIMAGE,
                                        CodeType = y.Item4
                                    }).ToList();
                                }
                            }
                            catch (Exception ex)
                            {
                                //ignored
                                //#TODO Add logging here
                                bookingVoucher = null; // issue in downloading voucher;
                            }

                            if (bookingVoucher != null && bookingVoucher?.DownloadFileNames != null)
                                hotelBedsSelectedProduct.BookingVouchers.Add(bookingVoucher);

                            #endregion Download Multiple QrCodes Image  for each pax from Html file and use it in isango voucher
                        }
                    }
                }

                #region Check if QrCode Images were found from html if not then try to download pdf file.

                //if QR Code not found so download PDF file .
                bool isCusomizableVoucherFound = false;
                if (hotelBedsSelectedProduct?.BookingVouchers?.Count > 0)
                {
                    isCusomizableVoucherFound = hotelBedsSelectedProduct.BookingVouchers
                                                .Any(x => x.DownloadFileNames != null
                                                && x.DownloadFileNames?.Any(
                                                            y => !string.IsNullOrEmpty(y.DownloadedFile)) == true
                                                        );
                    if (isCusomizableVoucherFound)
                    {
                        hotelBedsSelectedProduct.IsVocuherCustomizable = true;
                    }
                }

                // non customizable pdf voucher as per language
                if (isCusomizableVoucherFound == false)
                {
                    //voucher = apiBookingActivity.vouchers
                    //          .FirstOrDefault(cv =>
                    //                    cv.mimeType.Equals("application/pdf")
                    //                //&& cv.LanguageCode.ToLowerInvariant().Equals(apiBookingActivity.Purchase.Language.ToLowerInvariant())
                    //                );

                    //if (voucher == null)
                    //{
                    //non customizable pdf voucher as per eng language (Use eng PDF as default)
                    vouchers = apiBookingActivity.vouchers
                                .Where(cv => cv.mimeType.Equals("application/pdf")
                                    &&
                                    cv?.language?.ToLowerInvariant()?.Contains(_languageCode) == true
                                )
                                .ToList();
                    //}

                    if (vouchers?.Count == 0 && _languageCode != "eng")
                    {
                        vouchers = apiBookingActivity.vouchers
                               .Where(cv => cv.mimeType.Equals("application/pdf")
                                   &&
                                   cv?.language?.ToLowerInvariant()?.Contains("eng") == true
                               )
                               .ToList();
                    }

                    foreach (var voucher in vouchers)
                    {
                        if (voucher != null)
                        {
                            #region Download single pdf file for all paxes and use this pdf as voucher in confirmation email and confirmation page

                            var bookingVoucher = new BookingVoucher();
                            bookingVoucher.Language = voucher.language;
                            bookingVoucher.Url = voucher.url;
                            bookingVoucher.Type = BookingVoucherType.PDF;
                            //bookingVoucher.APIType = APIType.HOTELBEDS;
                            try
                            {
                                bookingVoucher.DownloadFileNames = new List<BookingVoucherHtmlORpdf>();
                                var PDFFile = SavePDFVoucher(apiBooking?.clientReference, voucher.url, apiBookingActivity.activityReference);

                                if (!string.IsNullOrWhiteSpace(PDFFile))
                                {
                                    bookingVoucher.DownloadFileNames.Add(new BookingVoucherHtmlORpdf { DownloadedFile = PDFFile, CodeValue = string.Empty, Url = voucher.url, QRCodeType = BookingVoucherQRCodeType.PDF });
                                }
                            }
                            catch (Exception)
                            {
                                bookingVoucher = null;
                                // issue in downloading voucher;
                            }
                            if (bookingVoucher != null && bookingVoucher?.DownloadFileNames != null)
                            {
                                hotelBedsSelectedProduct.BookingVouchers = new List<BookingVoucher>();
                                hotelBedsSelectedProduct.BookingVouchers.Add(bookingVoucher);
                                hotelBedsSelectedProduct.IsVocuherCustomizable = false;
                            }

                            #endregion Download single pdf file for all paxes and use this pdf as voucher in confirmation email and confirmation page
                        }
                    }
                }

                #endregion Check if QrCode Images were found from html if not then try to download pdf file.
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Download html file from given url and parse it for QrImageURLs and QrCodes.
        /// </summary>
        /// <param name="url">String url for downloading html file.</param>
        /// <param name="bookingReference">Booking Reference number.</param>
        /// <returns>Tuple(Image Source URL, QrCode).</returns>
        public List<Tuple<string, string, string, string>> DownloadAndParseHtml(string url, string bookingReference)
        {
            var html = new HtmlDocument();
            var QrCodes = new List<Tuple<string, string, string, string>>();
            try
            {
                // html.Load(@"C:\HtmlDocs\test.html"); // load a file
                using (WebClient client = new WebClient())
                {
                    html.LoadHtml(client.DownloadString(url));
                }
                var root = html.DocumentNode;
                var QrImagesHtmlTags = root.Descendants("img").Where(n => n.ParentNode.GetAttributeValue("class", "").Equals("qr__image"));
                if (QrImagesHtmlTags.Any())
                {
                    foreach (var ImgTag in QrImagesHtmlTags)
                    {
                        try
                        {
                            var QrCode = GetParsedImgAndQrCode(ImgTag.OuterHtml, bookingReference);
                            if (QrCode != null)
                                QrCodes.Add(QrCode);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
                //Else Voucher is not customizable, use default pdf provided by HB
            }
            catch (Exception)
            {
                QrCodes = null;//.Add(Tuple.Create("Error Occured", Ex.Message, Ex.StackTrace));
            }
            return QrCodes;
        }

        /// <summary>
        /// Get QRCode image source url and QRCode value from html image tag.
        /// </summary>
        /// <param name="hbImgTagOuterHtml">img src="https://activitiesbank.voucher-service.com/barser/rest/generateBarcode/GUE-19088721-25459938/QRCODE/0/1/80/PIXEL/80/P/PNG" alt="QRCODE GUE-19088721-25459938"</param>
        /// <param name="bookingReference">Booking Reference number.</param>
        /// <returns></returns>
        private Tuple<string, string, string, string> GetParsedImgAndQrCode(string hbImgTagOuterHtml, string bookingReference)
        {
            var imgSrc = string.Empty;
            var codeValue = string.Empty;
            var codeType = string.Empty;
            var fileName = string.Empty;
            Tuple<string, string, string, string> Result = null;
            try
            {
                if (!string.IsNullOrWhiteSpace(hbImgTagOuterHtml))
                {
                    imgSrc = GetAttributeValueFromHtmlTag(hbImgTagOuterHtml, "src");
                    codeValue = GetAttributeValueFromHtmlTag(hbImgTagOuterHtml, "alt");
                    if (!string.IsNullOrWhiteSpace(codeValue) && codeValue.Contains(" "))
                    {
                        codeType = codeValue?.Split(' ')?.FirstOrDefault()?.ToLower() ?? string.Empty;
                        codeType = codeType?.Contains("bar") == true ? "BAR_CODE" : codeType;
                        codeType = codeType?.Contains("qr") == true ? "QR_CODE" : codeType;
                        codeValue = codeValue?.Split(' ')?.LastOrDefault() ?? string.Empty;
                    }
                    if (!string.IsNullOrWhiteSpace(imgSrc) && !string.IsNullOrWhiteSpace(codeValue))
                    {
                        fileName = SaveQrCodeImage(bookingReference: bookingReference, imageUrl: imgSrc, fileName: codeType + "_" + codeValue);
                    }
                    Result = new Tuple<string, string, string, string>(imgSrc, codeValue, fileName, codeType);
                }
            }
            catch (Exception)
            {
                return null;// Tuple.Create("Error Occurred", Ex.Message, Ex.StackTrace);
            }
            return Result;
        }

        /// <summary>
        /// Get given Attribute value from html tag.
        /// </summary>
        /// <param name="htmlText">img src="https://activitiesbank.voucher-service.com/barser/rest/generateBarcode/GUE-19088721-25459938/QRCODE/0/1/80/PIXEL/80/P/PNG" alt="QRCODE GUE-19088721-25459938"</param>
        /// <param name="attribute">src or alt</param>
        ///  <param name="bookingReference">Booking Reference number.</param>
        /// <returns>https://activitiesbank.voucher-service.com/barser/rest/generateBarcode/GUE-19088721-25459938/QRCODE/0/1/80/PIXEL/80/P/PNG or QRCODE GUE-19088721-25459938 </returns>
        private string GetAttributeValueFromHtmlTag(string htmlText, string attribute)
        {
            try
            {
                return Regex.Match(htmlText, @"(?<=\b" + attribute + @"="")[^""]*").Value;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// It will download image from the given URL and save as per given parameters.
        /// </summary>
        ///  <param name="bookingReference">Booking Reference number.</param>
        /// <param name="imageUrl">URL from which the image would be downloaded.</param>
        /// <param name="fileName">Name of image file</param>
        /// <param name="targetDir">Folter path where the file will be saved (Default "./QrCodes"</param>
        /// <param name="format">Extension of file(Default PNG)</param>
        /// <returns>Saved file name</returns>
        private string SaveQrCodeImage(string bookingReference, string imageUrl, string fileName = null, string targetDir = "QrCodes", ImageFormat format = null)
        {
            var updatedFileName = string.Empty;
            try
            {
                var folder = targetDir;

                var webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                targetDir = Path.Combine(webRootPath, targetDir);

                if (!Directory.Exists(targetDir))
                    Directory.CreateDirectory(targetDir);

                bookingReference = !string.IsNullOrWhiteSpace(bookingReference) ? bookingReference + "_" : string.Empty;
                fileName = $"{bookingReference}{fileName}.{format?.ToString()?.ToLowerInvariant()}";

                updatedFileName = $"/{folder}/{fileName}";

                Task.Run(() => SaveQrImage(imageUrl, targetDir, fileName));
            }
            catch (Exception)
            {
                return null; // Ex.Message;
            }
            return updatedFileName;
        }

        /// <summary>
        /// It will download image from the given URL and save as per given parameters.
        /// </summary>
        /// /// <param name="pdfUrl">URL from which the pdf would be downloaded.</param>
        ///  <param name="bookingReference">Booking Reference number.</param>
        /// <param name="fileName">Name of pdf file</param>
        /// <param name="targetDir">Folder path where the file will be saved (Default "./QrCodes"</param>
        /// <returns>Saved file name</returns>
        private string SavePDFVoucher(string bookingReference, string pdfUrl, string fileName = null, string targetDir = "QrCodes")
        {
            var folder = targetDir;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                    targetDir = Path.Combine(webRootPath, targetDir);

                    if (!Directory.Exists(targetDir))
                        Directory.CreateDirectory(targetDir);

                    bookingReference = !string.IsNullOrWhiteSpace(bookingReference) ? bookingReference + "_" : string.Empty;
                    fileName = !string.IsNullOrWhiteSpace(fileName) ? fileName : string.Empty;
                    fileName = bookingReference + fileName /*+ "-" + DateTime.Now.ToString("yyyyMMdd-HHmmss")*/ + ".pdf";

                    var filePath = Path.Combine(targetDir, fileName);
                    var response = client.GetAsync(pdfUrl).Result;

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        response.Content.CopyToAsync(fileStream).Wait();
                    }
                }
            }
            catch (Exception)
            {
                return null; // "Error Occurred " + ex.Message;
            }
            return $"/{folder}/{fileName}";
        }
        /// <summary>
        /// Save image after downloading from given url in to target directory as given filename.
        /// </summary>
        /// <param name="imageUrl"></param>
        /// <param name="targetDir"></param>
        /// <param name="fileName"></param>
        private void SaveQrImage(string imageUrl, string targetDir, string fileName)
        {
            try
            {
                var format = ImageFormat.Png;

                using (WebClient client = new WebClient())
                {
                    var stream = client.OpenRead(imageUrl);
                    var bitmap = new Bitmap(stream);

                    if (bitmap != null)
                    {
                        bitmap.Save(targetDir + fileName, format);
                    }

                    stream.Flush();
                    stream.Close();
                }
            }
            catch
            {
            }
        }

        #endregion Apitude Json Version Changes for Customized HTMLVoucher / HB Default PDFVouhers
    }
}