using Isango.Entities;
using Isango.Entities.Affiliate;
using Isango.Entities.Booking;
using Isango.Entities.Mailer;
using Isango.Mailer.Contract;
using Isango.Mailer.ServiceContracts;
using Isango.Persistence.Contract;
using Isango.Service.Contract;
using Logger.Contract;
using System.Net.Mail;
using BookingDataOthers = Isango.Entities.Mailer.Voucher.BookingDataOthers;

namespace Isango.Service
{
    public class MailerService : IMailerService
    {
        private readonly IBookingPersistence _bookingPersistence;
        private readonly IMailer _mailer;
        private readonly IMasterService _masterService;
        private readonly ITemplateContextService _templateContextService;
        private readonly ILogger _log;
        private readonly IActivityService _activityService;

        public MailerService(IBookingPersistence bookingPersistence, IMailer mailer, IMasterService masterService,
            ITemplateContextService templateContextService, ILogger log, IActivityService activityService)
        {
            _bookingPersistence = bookingPersistence;
            _mailer = mailer;
            _masterService = masterService;
            _templateContextService = templateContextService;
            _log = log;
            _activityService = activityService;
        }

        /// <summary>
        /// Send FS/OR product mail to the customer
        /// </summary>
        /// <param name="bookingRef"></param>
        /// <param name="attachment"></param>
        /// <param name="isAlternativePayment"></param>
        /// <param name="isReceive"></param>
        /// <param name="supplierBookingReferenceNumber">to send email with supplier booking link in later step from booking web job</param>
        /// <param name="supplierVoucherLink">to send email with supplier booking link in later step from booking web job</param>
        public void SendMail(string bookingRef
            , List<Attachment> attachment = null
            , bool? isAlternativePayment = false
            , bool? isReceive = false
            , string supplierBookingReferenceNumber = null
            , string supplierVoucherLink = null
            , bool? isCancel = false
            , bool? isORtoConfirm = false
        )
        {
            try
            {
                var bookingData = _bookingPersistence.GetMailDataForReceive(bookingRef);
                var CVPoint = 0;
                try
                {
                     CVPoint = getCVPoints();
                }
                catch (Exception ex)
                { }
                //bookingData.SelectedProducts?.FirstOrDefault()?

                var clientInfo = new ClientInfo
                {
                    AffiliateId = bookingData?.Affiliate.Id,
                    LanguageCode = bookingData?.Language?.Code,
                    Currency = bookingData?.Currency
                };

                var CrossSaleData = _activityService.GetCrossSellProducts(bookingData?.SelectedProducts?.FirstOrDefault()?.LineOfBusiness ?? 0, bookingData?.Affiliate, clientInfo, bookingData?.SelectedProducts?.FirstOrDefault()?.RegionId)?.GetAwaiter().GetResult() ?? null;

                //if (!string.IsNullOrWhiteSpace(supplierBookingReferenceNumber)
                //    && !string.IsNullOrWhiteSpace(supplierVoucherLink)
                //)
                //{
                //    bookingData.SelectedProducts = bookingData.SelectedProducts.Where
                //            (p =>
                //                p.SupplierConfirmationData?.ToLower() == supplierBookingReferenceNumber?.ToLower()
                //            ).ToList();

                //    bookingData.SelectedProducts.ForEach(p =>
                //    {
                //        p.LinkType = "Link";
                //        p.LinkValue = supplierVoucherLink;
                //    });
                //}

                var numbers = _masterService
                    .GetSupportPhonesWithCountryCodeAsync(bookingData?.Affiliate?.Id, bookingData?.Language?.Code)?.Result;

                var templateContext = _templateContextService.GetEmailTemplateContext(bookingData, numbers, isReceive);
                templateContext.CVPoints = CVPoint;
                _mailer.SendMail(templateContext, attachment, isAlternativePayment, CrossSaleData, isReceive, isCancel, isORtoConfirm);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MailerService",
                    MethodName = "SendMail",
                    Params = $"{bookingRef}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Send FS/OR product mail to supplier
        /// </summary>
        /// <param name="bookingRef"></param>
        public bool SendSupplierMail(string bookingRef, string productids = "")
        {
            try
            {
                var returnStatus = false;
                var bookingData = _bookingPersistence.GetMailDataForReceive(bookingRef);
                var numbers = _masterService
                    .GetSupportPhonesWithCountryCodeAsync(bookingData.Affiliate.Id, bookingData.Language.Code).Result;
                var context = _templateContextService.GetEmailTemplateContext(bookingData, numbers);

                //Note: isSupplier is true as we want to retrieve data for supplier
                var bookingDetails = _bookingPersistence.GetBookingDataForMail(context.BookingRef, true);
                var bookingDataOthers = (BookingDataOthers)bookingDetails;

                if (bookingDataOthers?.BookedProductDetailList?.Count > 0)
                {
                    if (!String.IsNullOrEmpty(productids))
                    {
                        foreach (var item in bookingDataOthers.BookedProductDetailList)
                        {
                            //Sending supplier mail only for Isango and CitySightSeeing products
                            if (productids.Contains(item.ServiceId?.ToString()?.Trim()) && (item.ApiTypeId == "0" || item.ApiTypeId == "1" || item.ApiTypeId == "21"))
                            {
                                _mailer.SendSupplierMail(context, bookingDataOthers, Convert.ToInt32(item.ServiceId));
                            }
                        }

                    }
                    else
                    {
                        foreach (var item in bookingDataOthers.BookedProductDetailList)
                        {
                            //Sending supplier mail only for Isango and CitySightSeeing products
                            if (item.ApiTypeId == "0" || item.ApiTypeId == "1" || item.ApiTypeId == "21")
                            {
                                _mailer.SendSupplierMail(context, bookingDataOthers, Convert.ToInt32(item.ServiceId));
                            }
                        }
                    }
                    returnStatus = true;
                }
                return returnStatus;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MailerService",
                    MethodName = "SendSupplierMail",
                    Params = $"{bookingRef}"
                };
                _log.Error(isangoErrorEntity, ex);
                //throw;
                return false;
            }
        }

        /// <summary>
        /// Send cancellation mail to the customer
        /// </summary>
        /// <param name="bookingRef"></param>
        /// <param name="bookingDetail"></param>
        /// <returns></returns>
        public void SendCancelMail(string bookingRef, Booking bookingDetail = null)
        {
            try
            {
                var bookingData = bookingDetail ?? _bookingPersistence.GetMailDataForReceive(bookingRef);
                var CVPoint = getCVPoints();


                var numbers = _masterService
                    .GetSupportPhonesWithCountryCodeAsync(bookingData?.Affiliate.Id, bookingData?.Language.Code).Result;

                var templateContext = _templateContextService.GetCancelledEmailTemplateContext(bookingData, numbers);
                templateContext.CVPoints = CVPoint;

                //send cancellation mail to customer
                _mailer.SendMail(templateContext);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MailerService",
                    MethodName = "SendCancelMail",
                    AffiliateId = bookingDetail.Affiliate.Id,
                    Params = $"{bookingRef}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Send cancellation mail to supplier
        /// </summary>
        /// <param name="bookingRef"></param>
        /// <param name="bookingDetail"></param>
        /// <returns></returns>
        public void SendSupplierCancelMail(string bookingRef, Booking bookingDetail = null)
        {
            try
            {
                var bookingData = bookingDetail ?? _bookingPersistence.GetMailDataForReceive(bookingRef);

                var numbers = _masterService
                    .GetSupportPhonesWithCountryCodeAsync(bookingData?.Affiliate.Id, bookingData?.Language.Code).Result;

                var templateContext = _templateContextService.GetCancelledEmailTemplateContext(bookingData, numbers);

                //Note: isSupplier is true as we want to retrieve data for supplier
                var bookingDetails = _bookingPersistence.GetBookingDataForMail(templateContext.BookingRef, true);

                var bookingDataOthers = (BookingDataOthers)bookingDetails;

                if (bookingDataOthers?.BookedProductDetailList != null)
                {
                    foreach (var item in bookingDataOthers.BookedProductDetailList?.FindAll(x =>
                        x.BookedOptionStatusId == "3" && x.BookedOptionId == templateContext.BookedOptionId)
                    ) // retrieves latest cancelled product only
                    {
                        if (!item.IsHotelBedsActivity)
                        {
                            _mailer.SendCancellationMailToSupplier(templateContext, bookingDataOthers, item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MailerService",
                    MethodName = "SendSupplierCancelMail",
                    AffiliateId = bookingDetail.Affiliate.Id,
                    Params = $"{bookingRef}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Send alert mail
        /// </summary>
        /// <param name="affiliate"></param>
        /// <returns></returns>
        public void SendAlertMail(Affiliate affiliate)
        {
            try
            {
                _mailer.SendAlertMail(affiliate);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MailerService",
                    MethodName = "SendAlertMail",
                    AffiliateId = affiliate.Id,
                    Params = $"{affiliate.Id}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public void SendRemainingDiscountAmountMail(string discountRows)
        {
            try
            {
                _mailer.SendRemainingDiscountAmountMail(discountRows);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MailerService",
                    MethodName = "SendRemainingDiscountAmountMail"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Send Failed Booking to Customer Support
        /// </summary>
        /// <param name="failureMailContextList"></param>
        /// <returns></returns>
        public void SendFailureMail(List<FailureMailContext> failureMailContextList)
        {
            try
            {
                _mailer.SendFailureMail(failureMailContextList);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MailerService",
                    MethodName = "SendFailureMail"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Send Error Mail
        /// </summary>
        /// <param name="data"></param>
        public void SendErrorMail(List<Tuple<string, string>> data)
        {
            try
            {
                _mailer.SendErrorMail(data);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MailerService",
                    MethodName = "SendErrorMail",
                    Params = $"{data}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Send Cancel Booking Mail
        /// </summary>
        /// <param name="cancellationMailContextList"></param>
        public void SendCancelBookingMail(List<CancellationMailText> cancellationMailContextList)
        {
            try
            {
                _mailer.SendCancelBookingMail(cancellationMailContextList);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MailerService",
                    MethodName = "SendCancelBookingMail",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public void SendPDFErrorMail(string bookingRefNo)
        {
            _mailer.SendPDFErrorMail(bookingRefNo);
        }

        public void SendAdyenWebhookErrorMail(string bookingRefNo, string status, string pspReference, string reason)
        {
            _mailer.SendAdyenWebhookErrorMail(bookingRefNo, status, pspReference, reason);
        }

        public void SendAdyenGenerateLinkMail(string customerEmail, string generatedLink, string lang, string tempBookingRefNumber)
        {
            try
            {
                _mailer.SendAdyenGenerateLinkMail(customerEmail, generatedLink, lang, tempBookingRefNumber);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MailerService",
                    MethodName = "SendAdyenGenerateLinkMail"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public void SendAdyenReceivedLinkMail(string customerEmail, string price, string lang, string tempRef)
        {
            try
            {
                _mailer.SendAdyenReceivedLinkMail(customerEmail, price, lang, tempRef);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MailerService",
                    MethodName = "SendAdyenReceivedLinkMail"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public bool SendMailCustomer(string bookingRef
            , List<Attachment> attachment = null
            , bool? isAlternativePayment = false
            , bool? isReceive = false
            , string supplierBookingReferenceNumber = null
            , string supplierVoucherLink = null
        )
        {
            try
            {
                var returnStatus = false;
                var bookingData = _bookingPersistence.GetMailDataForReceive(bookingRef);
                var CVPoint = getCVPoints();

                //If there are all moulinRougeProduct then generate error, 
                //this endpoint use in booking manager
                var status = bookingData.SelectedProducts.All(x => x.APIType == Entities.Enums.APIType.Moulinrouge);
                if (!status)
                {
                    //bookingData.Language.Code = "es";
                    var clientInfo = new ClientInfo
                    {
                        AffiliateId = bookingData?.Affiliate.Id,
                        LanguageCode = bookingData?.Language?.Code,
                        Currency = bookingData?.Currency
                    };

                    var CrossSaleData = _activityService.GetCrossSellProducts(bookingData.SelectedProducts?.FirstOrDefault()?.LineOfBusiness ?? 0, bookingData.Affiliate, clientInfo, bookingData?.SelectedProducts?.FirstOrDefault()?.RegionId).GetAwaiter().GetResult() ?? null;

                    //if (!string.IsNullOrWhiteSpace(supplierBookingReferenceNumber)
                    //    && !string.IsNullOrWhiteSpace(supplierVoucherLink)
                    //)
                    //{
                    //    bookingData.SelectedProducts = bookingData.SelectedProducts.Where
                    //            (p =>
                    //                p.SupplierConfirmationData?.ToLower() == supplierBookingReferenceNumber?.ToLower()
                    //            ).ToList();

                    //    bookingData.SelectedProducts.ForEach(p =>
                    //    {
                    //        p.LinkType = "Link";
                    //        p.LinkValue = supplierVoucherLink;
                    //    });
                    //}

                    var numbers = _masterService
                        .GetSupportPhonesWithCountryCodeAsync(bookingData.Affiliate.Id, bookingData.Language.Code).Result;

                    var templateContext = _templateContextService.GetEmailTemplateContext(bookingData, numbers, isReceive);
                    templateContext.CVPoints = CVPoint;
                    _mailer.SendMail(templateContext, attachment, isAlternativePayment, CrossSaleData);
                    returnStatus = true;
                }
                return returnStatus;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MailerService",
                    MethodName = "SendMail",
                    Params = $"{bookingRef}"
                };
                _log.Error(isangoErrorEntity, ex);
                return false;
            }
        }
        public int getCVPoints()
        {
            DateTime currentDate = DateTime.Now;
            var cvdata = _bookingPersistence.LoadCVPointData();
            var filteredCVData = cvdata
                                 .SingleOrDefault(data => data.FromDate <= currentDate && currentDate <= data.ToDate);
            var cvPointsList = filteredCVData.CVPoints;

            return cvPointsList;
        }
    }
}