using Isango.Entities;
using Isango.Entities.Payment;
using System.Xml.Serialization;

namespace ServiceAdapters.WirecardPayment.WirecardPayment.Entities
{
    public class PaymentCardCriteria
    {
        //Payment
        public string JobId { get; set; }

        public string BusinessCaseSignature_Wirecard { get; set; }
        public string TransactionId { get; set; }
        public string PaymentGatewayReferenceId { get; set; }
        public decimal ChargeAmount { get; set; }
        public string CurrencyCode { get; set; }
        public string IpAddress { get; set; }
        public string Guwid { get; set; }
        public string AuthorizationCode { get; set; }
        public PaymentStatus PaymentStatus { get; set; }

        //Credit Card
        public string CardHoldersName { get; set; }

        public string CardHoldersAddress1 { get; set; }
        public string CardHoldersCity { get; set; }
        public string CardHoldersZipCode { get; set; }
        public string CardHoldersState { get; set; }
        public string CardHoldersCountryName { get; set; }
        public string CardHoldersEmail { get; set; }

        [XmlIgnore]
        public string CardNumber { get; set; }

        [XmlIgnore]
        public string SecurityCode { get; set; }

        public string ExpiryYear { get; set; }
        public string ExpiryMonth { get; set; }

        //Booking
        public string PaRes { get; set; }

        public DeviceCategory DeviceCategory { get; set; }
        public decimal Amount { get; set; }
        public string AcceptHeader { get; set; }
        public string UserAgent { get; set; }

        //
        public string TagText { get; set; }

        public string TagValue { get; set; }
        public string LogText { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Status { get; set; }
        public string RequestType { get; set; }
        public string RequestXml { get; set; }
        public string InstallmentAmount { get; set; }
        public string UserId { get; set; }
        public string BookingRefNo { get; set; }

        //Method Type
        public MethodType MethodType { get; set; }

        public string BaseUrl { get; set; }
    }
}