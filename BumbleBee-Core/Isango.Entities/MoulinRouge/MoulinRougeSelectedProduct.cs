using Isango.Entities.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Mail;

namespace Isango.Entities.MoulinRouge
{
    public class MoulinRougeSelectedProduct : SelectedProduct
    {
        /// <summary>
        /// Used for holding ticket information returned by MoulinRouge API
        /// </summary>
        ///
        public List<ConfirmedTicket> MrConfirmedTickets { get; set; }

        [JsonIgnore]
        public List<Attachment> ConfirmedTicketAttachments { get; set; }

        [JsonIgnore]
        public List<byte[]> ConfirmedTicketBytes { get; set; }

        /// <summary>
        /// Used for MoulinRougeAPI Requests.
        /// </summary>
        public int CatalogDateId { get; set; }

        /// <summary>
        /// Used for MoulinRougeAPI Requests.
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// Used for MoulinRougeAPI Requests.
        /// </summary>
        public int ContingentId { get; set; }

        /// <summary>
        /// Used for MoulinRougeAPI Requests.
        /// </summary>
        public int BlocId { get; set; }

        /// <summary>
        /// Used for MoulinRougeAPI Requests.
        /// </summary>
        public int FloorId { get; set; }

        /// <summary>
        /// Used for MoulinRougeAPI Requests.
        /// </summary>
        public int RateId { get; set; }

        /// <summary>
        /// Used for MoulinRougeAPI Requests.
        /// This Temp Order ID is created when we block the seat using AllocSeatsAutomatic call.
        /// This is used in communication b/w diffrent calls, Until we hit OrderConfirm, Final booking is not made at MoulinRougeAPI.
        /// </summary>
        public string TemporaryOrderId { get; set; }

        /// <summary>
        /// Used for MoulinRougeAPI Requests
        /// </summary>
        public string TemporaryOrderRowId { get; set; }

        /// <summary>
        /// Used for MoulinRougeAPI Requests
        /// Use to hold Integer list that can be used for various IDs in request response
        /// </summary>
        public List<int> Ids { get; set; }

        /// <summary>
        /// Used for MoulinRougeAPI Requests
        /// Create Account
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Used for MoulinRougeAPI Requests
        /// Create Account
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Used for MoulinRougeAPI Requests
        /// Order Confirm
        /// </summary>
        public string IsangoBookingReferenceNumber { get; set; }

        /// <summary>
        /// Used for MoulinRougeAPI Requests
        /// Order Confirm
        /// Its value is retrived from call CreateAccount
        /// In Respnse there is a feild ID_Identity
        /// </summary>
        public int IdentityConsumerId { get; set; }

        /// <summary>
        /// Used for MoulinRougeAPI Requests
        /// ACP_GetOrderEticketRequest
        /// This value is retrived from resposnse of ACP_OrderConfirmRequest
        /// </summary>
        public List<string> ETicketGuiDs { get; set; }

        /// <summary>
        /// Used for MoulinRougeAPI Requests
        /// Required for calling ACP_GetOrderEticketRequest
        /// This value is retrived from resposnse of ACP_OrderConfirmRequest
        /// </summary>
        public string OrderId { get; set; }

        /// <summary>
        /// Used for MoulinRougeAPI Requests
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Used for MoulinRougeAPI Requests
        /// </summary>
        public DateTime DateStart { get; set; }

        /// <summary>
        /// Used for MoulinRougeAPI Requests
        /// </summary>
        public DateTime DateEnd { get; set; }

        /// <summary>
        /// Used for MoulinRougeAPI Requests
        /// Contains List Of seats retunred by MoulinRouge API after order Confirmation
        /// </summary>
        public List<ConfirmedTicket> ConfirmedTickets { get; set; }

        /// <summary>
        /// Used for MoulinRougeAPI Requests
        /// id_orderMR field retvied from ACP_OrderConfirm Result
        /// </summary>
        public int OrderMrid { get; set; }

        public AvailabilityStatus AvailabilityStatus { get; set; }

        public string Code { get; set; }

        public string Destination { get; set; }

        public int FactsheetId { get; set; }

        /// <summary>
        /// Used for MoulinRougeAPI Requests
        /// </summary>
        public int Quantity { get; set; }
    }
}