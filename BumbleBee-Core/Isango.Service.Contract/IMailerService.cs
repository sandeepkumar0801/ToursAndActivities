using Isango.Entities.Affiliate;
using Isango.Entities.Booking;
using Isango.Entities.Mailer;
using System;
using System.Collections.Generic;
using System.Net.Mail;

namespace Isango.Service.Contract
{
    public interface IMailerService
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="bookingRef"></param>
        /// <param name="attachment"></param>
        /// <param name="isAlternativePayment"></param>
        /// <param name="isReceive"></param>
        /// <param name="supplierBookingReferenceNumber">to send email with supplier booking link in later step from booking web job</param>
        /// <param name="supplierVoucherLink">to send email with supplier booking link in later step from booking web job</param>
        void SendMail(string bookingRef
            , List<Attachment> attachment = null
            , bool? isAlternativePayment = false
            , bool? isReceive = false
            , string supplierBookingReferenceNumber = null
            , string supplierVoucherLink = null
            , bool? isCancel = false
            , bool? isORtoConfirm = false);

        bool SendSupplierMail(string bookingRef,string productids="");

        void SendCancelMail(string bookingRef, Booking bookingDetail = null);

        void SendSupplierCancelMail(string bookingRef, Booking bookingDetail = null);

        void SendAlertMail(Affiliate affiliate);

        void SendFailureMail(List<FailureMailContext> failureMailContextList);

        void SendRemainingDiscountAmountMail(string discountRows);

        void SendErrorMail(List<Tuple<string, string>> data);

        void SendCancelBookingMail(List<CancellationMailText> cancellationMailContextList);

        bool SendMailCustomer(string bookingRef
           , List<Attachment> attachment = null
           , bool? isAlternativePayment = false
           , bool? isReceive = false
           , string supplierBookingReferenceNumber = null
           , string supplierVoucherLink = null
       );

        void SendPDFErrorMail(string bookingRefNo);

        void SendAdyenWebhookErrorMail(string bookingRefNo, string status, string pspReference, string reason);
        void SendAdyenGenerateLinkMail(string customerEmail, string generatedLink, string lang,string tempBookingRefNumber);

        void SendAdyenReceivedLinkMail(string customerEmail, string price, string lang,string temporaryRefNo);
    }
}