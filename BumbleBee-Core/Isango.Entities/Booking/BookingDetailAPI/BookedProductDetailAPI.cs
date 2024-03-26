using System;
using System.Data;

namespace Isango.Entities.Booking.BookingDetailAPI
{
    public class BookedProductDetailAPI
    {
        //booked product detail data
        public string BookedOptionId { get; set; }

        public string BookedOptionStatusName { get; set; }

        public string BookedOptionStatusId { get; set; }

        public string ServiceName { get; set; }

        public string OptionName { get; set; }

        public string LeadPassengerName { get; set; }

        public string NoOfAdults { get; set; }

        public string ChildCount { get; set; }

        public string ScheduleLocation { get; set; }

        public string ServiceHotelPickup { get; set; }

        public DateTime FromDate { get; set; }

        public string ToDate { get; set; }

        public string Schedule { get; set; }

        public string Duration { get; set; }

        public string ScheduleReturnDetails { get; set; }

        public string PleaseNote { get; set; }

        public string GrosSellingAmount { get; set; }

        public string DiscountAmount { get; set; }

        public string MultisaveDiscount { get; set; }

        public string AmountOnWirecard { get; set; }

        public bool IsAudioGuide { get; set; }

        public string ServiceTypeId { get; set; }

        public string ServiceCheckinTime { get; set; }

        public string ServicecheckoutTime { get; set; }

        public string MealPlan { get; set; }

        public string IsPaxDetailRequired { get; set; }

        public string SupplierOptionCode { get; set; }

        public string ServiceID { get; set; }

        public string ServiceOptionInServiceID { get; set; }

        public string PickupOptionId { get; set; }

        public string ArrivalAirport { get; set; }

        public string ArrivalDate { get; set; }

        public string ArrivalFlight { get; set; }

        public string DepartureAirport { get; set; }

        public string DepartureDate { get; set; }

        public string DepartureFlight { get; set; }

        public string PickupLocation { get; set; } = string.Empty;

        public string SpecialRequest { get; set; } = string.Empty;

        public string TransferType { get; set; }

        public string RefundAmount { get; set; }

        // Added these properties so that Supplier can see the service and option name in English in Supplier Voucher
        public string ServiceNameEnglish { get; set; }

        public string OptionNameEnglish { get; set; }

        public string TsProductName { get; set; }

        public string SupplierCost { get; set; }

        public string SupplierCurrency { get; set; }

        public string SupplierId { get; set; }

        public string SupplierOptionName { get; set; }

        public string ContractComment { get; set; }

        public bool IsHotelBedsActivity { get; set; }

        public string FileNumber { get; set; }

        public string OfficeCode { get; set; }

        public string VatNumber { get; set; }

        public string Reason { get; set; }

        public string AlternativeTours { get; set; }

        public string AltervativeDateTime { get; set; }

        public int RegionId { get; set; }

        public string CancellationPolicy { get; set; }

        public BookedProductDetailAPI(IDataReader result)
        {
            BookedOptionId = Convert.ToString(result["bookedoptionid"]).Trim();
            BookedOptionStatusName = Convert.ToString(result["bookedoptionstatusname"]).Trim();
            BookedOptionStatusId = Convert.ToString(result["bookedoptionstatusid"]).Trim();
            ServiceName = Convert.ToString(result["servicename"]).Trim();
            OptionName = Convert.ToString(result["optionname"]).Trim();
            LeadPassengerName = Convert.ToString(result["LeadPassengerName"]).Trim();
            NoOfAdults = Convert.ToString(result["noofadults"]).Trim();
            ChildCount = Convert.ToString(result["childcount"]).Trim();
            ScheduleLocation = Convert.ToString(result["schedulelocation"]).Trim();
            ServiceHotelPickup = Convert.ToString(result["servicehotelpickup"]).Trim();
            FromDate = Convert.ToDateTime(result["fromdate"]);
            ToDate = Convert.ToString(result["todate"]).Trim();
            Schedule = Convert.ToString(result["schedule"]).Trim();
            Duration = Convert.ToString(result["duration"]).Trim();
            ScheduleReturnDetails = Convert.ToString(result["ScheduleReturnDetails"]).Trim();
            PleaseNote = Convert.ToString(result["pleasenote"]).Trim();
            GrosSellingAmount = Convert.ToString(result["grossellingamount"]).Trim();
            DiscountAmount = Convert.ToString(result["DISCOUNT_AMOUNT"]).Trim();
            MultisaveDiscount = Convert.ToString(result["multisavediscount"]).Trim();
            AmountOnWirecard = Convert.ToString(result["amountonwirecard"]).Trim();
            IsAudioGuide = Convert.ToBoolean(result["IsAudioGuide"]);
            ServiceTypeId = Convert.ToString(result["servicetypeid"]).Trim();
            ServiceCheckinTime = Convert.ToString(result["servicecheckintime"]).Trim();
            ServicecheckoutTime = Convert.ToString(result["servicecheckouttime"]).Trim();
            MealPlan = Convert.ToString(result["MealPlan"]).Trim();
            IsPaxDetailRequired = Convert.ToString(result["IsPaxDetailRequired"]).Trim();
            SupplierOptionCode = Convert.ToString(result["SupplierOptionCode"]).Trim();
            ServiceID = Convert.ToString(result["serviceid"]).Trim();
            ServiceOptionInServiceID = Convert.ToString(result["serviceoptioninserviceid"]).Trim();
            PickupOptionId = Convert.ToString(result["pickupoptionid"]).Trim();
            ArrivalAirport = Convert.ToString(result["arrivalairport"]).Trim();
            ArrivalDate = Convert.ToString(result["arrivaldate"]).Trim();
            ArrivalFlight = Convert.ToString(result["arrivalflight"]).Trim();
            DepartureAirport = Convert.ToString(result["departureairport"]).Trim();
            DepartureDate = Convert.ToString(result["departuredate"]).Trim();
            DepartureFlight = Convert.ToString(result["departureflight"]).Trim();
            TransferType = Convert.ToString(result["transfertype"]).Trim();
            RefundAmount = Convert.ToString(result["UserRefundAmount"]).Trim();
            ServiceNameEnglish = Convert.ToString(result["servicename_en"]).Trim();
            OptionNameEnglish = Convert.ToString(result["optionname_en"]).Trim();
            TsProductName = Convert.ToString(result["TS_ProductName"]).Trim();
            SupplierCost = Convert.ToString(result["Cost_Suppliercrry"]).Trim();
            SupplierCurrency = Convert.ToString(result["SupplierCurrency"]).Trim();
            SupplierId = Convert.ToString(result["supplierID"]).Trim();
            SupplierOptionName = Convert.ToString(result["supplieroptionname"]);
            IsHotelBedsActivity = Convert.ToBoolean(result["IsHotelBedsAPIProduct"]);
            RegionId = Convert.ToInt32(result["regionid"]);

            if (result["VATNo"] != DBNull.Value)
            {
                VatNumber = Convert.ToString(result["VATNo"]).Trim();
            }

            if (result["supplierbookingreferencenumber"] != DBNull.Value)
            {
                FileNumber = Convert.ToString(result["supplierbookingreferencenumber"]);
                OfficeCode = Convert.ToString(result["officecode"]);
            }

            if (result["responsedatetime"] != DBNull.Value)
            {
                AltervativeDateTime = Convert.ToString(result["responsedatetime"]);
            }

            if (result["alternativetours"] != DBNull.Value)
            {
                AlternativeTours = Convert.ToString(result["alternativetours"]);
            }

            if (result["reason"] != DBNull.Value)
            {
                Reason = Convert.ToString(result["reason"]);
            }

            if (result["cancellationpolicy"] != DBNull.Value)
            {
                CancellationPolicy = Convert.ToString(result["cancellationpolicy"]);
            }
        }
    }
}