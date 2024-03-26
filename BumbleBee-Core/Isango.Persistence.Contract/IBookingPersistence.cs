using Isango.Entities;
using Isango.Entities.Booking;
using Isango.Entities.Booking.BookingDetailAPI;
using Isango.Entities.Booking.ConfirmBooking;
using Isango.Entities.Booking.PartialRefund;
using Isango.Entities.Booking.RequestModels;
using Isango.Entities.Payment;
using System;
using System.Collections.Generic;

namespace Isango.Persistence.Contract
{
    public interface IBookingPersistence
    {
        bool InsertWirecardXml(WireCardXmlCriteria xmlCriteria);

        Tuple<bool, string> CheckDuplicateBooking(DuplicateBookingCriteria criteria);

        bool CreateIsangoBooking(IsangoBookingData isangoBooking, bool isAlternativePayment);

        #region Get Booking data

        Booking GetMailDataForReceive(string bookingRefNo);

        BookingDetailBase GetBookingDataForMail(string bookingRef, bool isSupplier, int value = 3, int? bookedOptionId = null);

        bool MatchBookingByEmailRef(string email, string bookingRef);

        #endregion Get Booking data

        bool SaveFailedBookingInDb(Guid affiliateId, string customerEmail, string selectedProductText, string hashKey,
            int saveInCartType);

        Booking UpdateIsangoBookingAgainstRefund(int amendmentId, string remarks, string actionBy);

        int UpdateIsangoBooking(int amendmentId, bool is3D, ref Payment purchasePayment, string cardType = "", int paymentGatewayTypeId = 4);

        PaymentBookingData GetPartialRefundData(int amendmentId);

        PaymentBookingData GetPaymentRelatedBookingData(int amendmentId);

        int InsertPartialBooking(PartialBooking partialBooking);

        ConfirmBookingDetail GetBookingData(string bookingReferenceNumber);

        string GenerateBookingRefNumber(string affiliateID, string currencyCode);

        BookedProductPaymentData ConfirmBookingUpdateStatusAndGetPaymentData(string userId, int bookedOptionId);

        List<BookedOptionMailData> CheckToSendmailToCustomer(int bookedOptionId);

        bool UpdatePaymentStatus(int transactionId, string guWId, string AuthorizationCode, string gateWayID="");

        PartialRefundPaymentData InsertPartialRefundAndGetPaymentInfo(int amendmentId, string remarks, string actionBy);

        ReceiveDetail GetReceiveDetail(int id);

        string BookingReferenceNumberfromDB(string affiliateID, string currencyISO, string randomString);

        #region Booking Details By StatusID

        List<BookingDetail> GetBookingDetails(string referenceNumber, string userId, string statusId);

        #endregion Booking Details By StatusID

        void UpdateReceiveBookingTransaction(Payment purchasePayment);

        /// <summary>
        int UpdateAPISupplierBookingQRCode(string bookingRefNo, int serviceOptionId,
        string availabilityRefId, string value, string qrCodeType = "", bool isQRCodePerPax = false, string multiQRcode = "");

        void InsertGeneratePaymentLink(GeneratePaymentLinkResponse generatePaymentLinkRequest);

        void LogBookingFailureInDB(Booking booking, string bookingRefNo, int? serviceID, string tokenID, string apiRefID, string custEmail, string custContact, int? ApiType, int? optionID, string optionName, string avlbltyRefID, string ErrorLevel);

        List<ReservationDBDetails> GetReservationData(string bookingRefNo);
        List<CVData> LoadCVPointData();
    }
}