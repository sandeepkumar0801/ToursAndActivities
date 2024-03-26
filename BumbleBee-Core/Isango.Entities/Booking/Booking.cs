using Isango.Entities.Enums;
using Isango.Entities.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Isango.Entities.Booking
{
    public class Booking : ErrorList
    {
        public string Id { get; set; }

        public int BookingId { get; set; }

        public string SessionId { get; set; }

        public string ReferenceNumber { get; set; }
        private BookingStatus _status;

        public BookingStatus Status
        {
            get
            {
                var anyFailedOrCancelledProduct = SelectedProducts.Any(x => x.Status == OptionBookingStatus.Failed || x.Status == OptionBookingStatus.Cancelled);
                var anyORProduct = SelectedProducts.Any(x => x.Status == OptionBookingStatus.Requested);

                if (anyFailedOrCancelledProduct)
                {
                    if (SelectedProducts.All(x => x.Status == OptionBookingStatus.Failed))
                        _status = BookingStatus.Failed;
                    else if (SelectedProducts.All(x => x.Status == OptionBookingStatus.Cancelled))
                        _status = BookingStatus.Cancelled;
                    else
                        _status = BookingStatus.Partial;
                }
                else
                {
                    _status = anyORProduct ? BookingStatus.Requested : BookingStatus.Confirmed;
                }
                return _status;
            }
        }

        public ISangoUser User { get; set; }

        public List<Payment.Payment> Payment { get; set; }

        public bool isRiskifiedEnabled { get; set; }

        public PaymentMethodType PaymentMethodType { get; set; }

        public PaymentGatewayType PaymentGateway { get; set; }

        public List<SelectedProduct> SelectedProducts { get; set; }

        public ClientDetail ClientDetail { get; set; }

        public decimal Amount { get; set; }

        public BookingType BookingType { get; set; }

        /// <summary>
        /// Provides the tour booking date
        /// </summary>
        public DateTime Date { get; set; }

        public DateTime BookingTime { get; set; }

        public Affiliate.Affiliate Affiliate { get; set; }

        public Language Language { get; set; }
        public string TiquetsLanguageCode { get; set; }


        public string VoucherEmailAddress { get; set; }

        public string VoucherPhoneNumber { get; set; }

        public string BookingAgent { get; set; }

        public decimal MultisaveAmountOnBooking { get; set; }

        public decimal TotalDiscount { get; set; }

        [XmlIgnore]
        public string IpAddress { get; set; }

        [XmlIgnore]
        public string ActualIP { get; set; }

        [XmlIgnore]
        public string OriginCountry { get; set; }

        [XmlIgnore]
        public string OriginCity { get; set; }

        public string Town { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Street { get; set; }
        public string State { get; set; }

        public string PaRes { get; set; }

        public string Guwid { get; set; }

        public BookingUserAgent BookingUserAgent { get; set; }
        public Currency Currency { get; set; }

        public PaymentOptionType PaymentOption { get; set; }

        public IsangoBookingData IsangoBookingData { get; set; }

        #region Encore

        public int TransactionReference { get; set; }

        public string TransactionPassword { get; set; }

        #endregion Encore

        #region Generic for Booking Ids

        public Dictionary<APIType, string> ApiBookingIds { get; set; }

        #endregion Generic for Booking Ids

        /// <summary>
        /// Set error if any.
        /// </summary>
        public string ErrorMessage { get; set; }

        public string AdyenPaymentData { get; set; }
        public string FallbackFingerPrint { get; set; }
        public BrowserInfo BrowserInfo { get; set; }

        public bool IsReservation { get; set; }

        public string AdyenMerchantAccount { get; set; }

        public string AdyenMerchantAccountCancelRefund { get; set; }
    }

    public class BrowserInfo
    {
        public string UserAgent { get; set; }
        public string AcceptHeader { get; set; }
        public string Language { get; set; }
        public string ScreenHeight { get; set; }
        public string ScreenWidth { get; set; }
        public string ColorDepth { get; set; }
        public string TimeZoneOffset { get; set; }
        public bool JavaEnabled { get; set; }
    }
}