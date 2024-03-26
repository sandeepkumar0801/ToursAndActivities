using Isango.Entities;
using Isango.Entities.AdyenPayment;
using Isango.Entities.Booking;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Isango.Service.Contract
{
    public interface IBookingService
    {
        BookingResult Book(Booking booking, string token);

        BookingResult Create3DBooking(Booking booking, string token);

        bool LogTransaction(WireCardXmlCriteria wireCardXmlCriteria);

        string GetAuthString(string affiliateId);

        ConfirmBookingDetail GetBookingData(string bookingReferenceNumber);

        #region Get Booking

        bool MatchBookingByEmailRef(string email, string bookingRef);

        Tuple<bool, string> ConfirmBooking(int bookedOptionId, string userId, string token, bool isCalledByAsyncJob = false);

        #endregion Get Booking

        #region Failed Booking

        string SaveFailedBookingInDb(Booking failedBooking);

        #endregion Failed Booking

        #region Amend Booking

        bool AmendBooking(Booking booking, int amendmentId, string token);

        bool RefundService(int amendmentId, string remarks, string actionBy, string token);

        PaymentBookingData ReceivePaymentDataService(int amendmentId);

        PaymentBookingData PartialRefundDataService(int amendmentId);

        #endregion Amend Booking

        #region AlternativePayment

        void ProcessTransactionSuccess(Booking booking, bool? isAlternativePayment);

        void ProcessTransactionFail(Booking booking);

        void ProcessTransactionWebHook(Booking booking, string jsonPostedData, string absoluteUri, string token);

        #endregion AlternativePayment

        #region Cancellation API related methods

        Task<List<BookingDetail>> GetBookingDetailAsync(string referenceNumber, string userId, string statusId);

        Task<bool> CancelSupplierBookingAsync(Booking booking, string token,bool IsBookingManager=false);

        Task<Dictionary<string, string>> GetOptionAndServiceNameAsync(string bookingRefNo, bool isSupplier,
            string bookedOptionId);

        #endregion Cancellation API related methods

        Tuple<bool, string> ProcessPartialRefund(int amendmentId, string remarks, string actionBy, string token);

        string GenerateBookingRefNumber(string affiliateID, string currencyCode);

        ReceiveDetail GetReceiveDetail(int id);

        BookingResult BookReceive(Booking booking, string token);

        BookingResult CreateReceive3DBooking(Booking booking, string token);

        Tuple<bool, string, AuthorizationResponse> CreateReceiveBooking(Booking booking, bool isEnrollmentCheck, string token);

        string GetReferenceNumberfromDB(string affiliateID, string currencyISO);

        void ProcessAdyenWebhook(int flowName, string bookingReference, string status, string pspReference, string reason = "", bool? success = true);

        bool GeneratePaymentIsangoResponse(string countryCode
     , string shopperLocale, string amount, string currency, string emailLanguage, string customerEmail);

        void ProcessAdyenWebhookGeneratePaymentLink(string paymentGenerateLinkId="",string pspReference="");

        void LogBookingFailureInDB(Booking failedBooking, string bookingRefNo, int? serviceID, string tokenID, string apiRefID, string custEmail, string custContact, int? ApiType, int? optionID, string optionName, string avlbltyRefID, string ErrorLevel);

        ReservationResponse Reserve(Booking booking, string token);

        ReservationResponse CancelReservation(Booking booking, string token);

        List<ReservationDBDetails> GetResrvationDetailsFromDB(string bookingRef);
    }
}