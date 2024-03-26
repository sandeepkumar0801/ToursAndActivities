using System;
using System.Collections.Generic;

namespace Isango.Entities
{
    /// <summary>
    ///  Moulin Rouge API Request Response Properties
    /// </summary>
    public class APIContextMoulinRouge
    {
        #region MoulinRougeAPI calls will be using these fields in different calls

        /// <summary>
        /// Used for MoulinRougeAPI Requests
        /// MoulinRouge Service Type
        /// 1) SHOW
        /// 2) DINNER
        /// </summary>
        public string ServiceType { get; set; }

        /// <summary>
        /// Used for MoulinRougeAPI Requests
        /// MoulinRouge Service Type Names
        /// Service type name can of one of the followings based on the rateID and contingentID  combination :-
        ///,"ShowWithDrinks" value="81622:82647"
        ///,"MistinguettMenu" value="81623:82646"
        ///,"VegetarianMenu" value="83664:82646"
        ///,"VeganMenu" value="83665:82646"
        ///,"ToulouseLautrecMenu" value="81624:82646"
        ///,"BelleEpoqueMenu" value="81625:82646"
        ///,"ChristmasDinner" value="81626:82646"
        ///,"ValentineDinner" value="81627:82646"
        /// </summary>
        public string ServiceTypeName { get; set; }

        /// <summary>
        /// Used for MoulinRougeAPI Requests
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// Used for MoulinRougeAPI Requests
        /// </summary>
        public int ContingentId { get; set; }

        /// <summary>
        /// Used for MoulinRougeAPI Requests
        /// </summary>
        public int BlocId { get; set; }

        /// <summary>
        /// Used for MoulinRougeAPI Requests
        /// </summary>
        public int RateId { get; set; }

        /// <summary>
        /// Used for MoulinRougeAPI Requests
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Used for MoulinRougeAPI Requests
        /// </summary>
        public int CatalogDateId { get; set; }

        /// <summary>
        /// Used for MoulinRougeAPI Requests
        /// Use to hold Integer list that can be used for various id in request response
        /// </summary>
        public List<int> Ids { get; set; }

        /// <summary>
        /// Used for MoulinRougeAPI Requests
        /// </summary>
        public int FloorId { get; set; }

        /// <summary>
        /// Used for MoulinRougeAPI Requests
        /// </summary>
        public decimal Amount { get; set; }

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
        /// </summary>
        public string FeeType { get; set; }

        /// <summary>
        /// Used for MoulinRougeAPI Requests
        /// </summary>
        public int SendingFeeId { get; set; }

        /// <summary>
        /// Used for MoulinRougeAPI Requests
        /// TempOrderSetSendingFeesRequest
        /// </summary>
        public decimal UnitAmount { get; set; }

        /// <summary>
        /// Used for MoulinRougeAPI Requests
        /// TempOrderSetSendingFeesRequest
        /// </summary>
        public decimal GlobalAmount { get; set; }

        /// <summary>
        /// Used for MoulinRougeAPI Requests
        /// TempOrderSetSendingFeesRequest
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Used for MoulinRougeAPI Requests
        /// TempOrderSetSendingFeesRequest
        /// </summary>
        public int TypeCalcul { get; set; }

        /// <summary>
        /// Used for MoulinRougeAPI Requests
        /// TempOrderSetSendingFeesRequest
        /// </summary>
        public int Nombre { get; set; }

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
        public string Guid { get; set; }

        /// <summary>
        /// Used for MoulinRougeAPI Requests
        /// Required for calling ACP_GetOrderEticketRequest
        /// This value is retrived from resposnse of ACP_OrderConfirmRequest
        /// </summary>
        public string OrderId { get; set; }

        /// <summary>
        /// Used for MoulinRougeAPI Requests
        /// </summary>
        public DateTime DateStart { get; set; }

        /// <summary>
        /// Used for MoulinRougeAPI Requests
        /// </summary>
        public DateTime DateEnd { get; set; }

        /// <summary>
        /// Used for Isango Service Id Mapping
        /// </summary>
        public int ServiceId { get; set; }

        /// <summary>
        /// Used for Isango Service Id Mapping
        /// </summary>
        public string ServiceOptionCode { get; set; }

        public APIContextMoulinRouge()
        {
            FeeType = string.Empty;
            FirstName = string.Empty;
            FullName = string.Empty;
            Guid = string.Empty;
            Ids = new List<int>();
            IsangoBookingReferenceNumber = string.Empty;
            OrderId = string.Empty;
            ServiceType = string.Empty;
            ServiceTypeName = string.Empty;
            TemporaryOrderId = string.Empty;
            TemporaryOrderRowId = string.Empty;
        }

        #endregion MoulinRougeAPI calls will be using these fields in different calls
    }
}