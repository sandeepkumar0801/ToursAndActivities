namespace ServiceAdapters.HotelBeds.HotelBeds.Config
{
    public static class XmlNodes
    {
        public const string ErrorList = "ErrorList";

        #region HotelValuedAvailRQ

        public const string HotelValuedAvailRq = "HotelValuedAvailRQ";
        public const string HotelValuedAvailRs = "HotelValuedAvailRS";
        public const string Credentials = "Credentials";
        public const string User = "User";
        public const string Password = "Password";
        public const string PaginationData = "PaginationData";
        public const string CheckInDate = "CheckInDate";
        public const string CheckOutDate = "CheckOutDate";
        public const string Destination = "Destination";
        public const string ExtraParamList = "ExtraParamList";
        public const string ExtendedData = "ExtendedData";
        public const string Name = "Name";
        public const string Value = "Value";
        public const string OccupancyList = "OccupancyList";
        public const string HotelOccupancy = "HotelOccupancy";
        public const string RoomCount = "RoomCount";
        public const string Occupancy = "Occupancy";
        public const string Customer = "Customer";
        public const string AdultCount = "AdultCount";
        public const string ChildCount = "ChildCount";
        public const string Age = "Age";

        #endregion HotelValuedAvailRQ

        #region ServiceAddRQ

        public const string Service = "Service";
        public const string ServiceAddRq = "ServiceAddRQ";

        #endregion ServiceAddRQ

        #region ServiceAddRS

        public const string ServiceAddRs = "ServiceAddRS";
        public const string Purchase = "Purchase";
        public const string Status = "Status";
        public const string Agency = "Agency";
        public const string Branch = "Branch";
        public const string CommentList = "CommentList";
        public const string Comment = "Comment";
        public const string Supplier = "Supplier";
        public const string AdditionalCostList = "AdditionalCostList";
        public const string AdditionalCost = "AdditionalCost";
        public const string ModificationPolicyList = "ModificationPolicyList";
        public const string ModificationPolicy = "ModificationPolicy";

        #endregion ServiceAddRS

        #region AuditData

        public const string AuditData = "AuditData";
        public const string ProcessTime = "ProcessTime";
        public const string Timestamp = "Timestamp";
        public const string RequestHost = "RequestHost";
        public const string ServerName = "ServerName";
        public const string ServerId = "ServerId";
        public const string SchemaRelease = "SchemaRelease";
        public const string HydraCoreRelease = "HydraCoreRelease";
        public const string HydraEnumerationsRelease = "HydraEnumerationsRelease";
        public const string MerlinRelease = "MerlinRelease";

        #endregion AuditData

        #region ServiceHotel

        public const string ServiceHotel = "ServiceHotel";
        public const string DirectPayment = "DirectPayment";
        public const string DateFrom = "DateFrom";
        public const string DateTo = "DateTo";
        public const string Currency = "Currency";
        public const string PackageRate = "PackageRate";
        public const string TravelAgent = "TravelAgent";

        #endregion ServiceHotel

        #region Contract

        public const string ContractList = "ContractList";
        public const string Contract = "Contract";

        public const string IncomingOffice = "IncomingOffice";
        public const string Classification = "Classification";

        #endregion Contract

        #region ShoppingCart

        public const string SeatingShoppingCartId = "SeatingShoppingCartId";

        #endregion ShoppingCart

        #region Promotion

        public const string PromotionList = "PromotionList";
        public const string Promotion = "Promotion";

        public const string ShortName = "ShortName";
        public const string Observations = "Observations";

        #endregion Promotion

        #region Discount

        public const string DiscountList = "DiscountList";
        public const string Discount = "Discount";

        #endregion Discount

        #region HotelInfo

        public const string HotelInfo = "HotelInfo";
        public const string Code = "Code";

        public const string ImageList = "ImageList";
        public const string Image = "Image";
        public const string Type = "Type";
        public const string Order = "Order";
        public const string VisualizationOrder = "VisualizationOrder";
        public const string Url = "Url";
        public const string Category = "Category";

        public const string ZoneList = "ZoneList";
        public const string Zone = "Zone";
        public const string ChildAge = "ChildAge";
        public const string Position = "Position";

        #endregion HotelInfo

        #region AvailableRoom

        public const string AvailableRoom = "AvailableRoom";
        public const string HotelRoom = "HotelRoom";
        public const string Board = "Board";
        public const string RoomType = "RoomType";
        public const string Price = "Price";
        public const string Amount = "Amount";
        public const string SellingPrice = "SellingPrice";
        public const string NetPrice = "NetPrice";
        public const string Commission = "Commission";

        #endregion AvailableRoom

        #region PurchaseConfirm

        public const string PurchaseConfirmRs = "PurchaseConfirmRS";
        public const string Reference = "Reference";
        public const string FileNumber = "FileNumber";
        public const string Language = "Language";
        public const string CreationDate = "CreationDate";
        public const string Holder = "Holder";
        public const string LastName = "LastName";
        public const string AgencyReference = "AgencyReference";
        public const string ServiceList = "ServiceList";
        public const string TotalAmount = "TotalAmount";
        public const string GuestList = "GuestList";
        public const string CustomerId = "CustomerId";
        public const string CancellationPolicy = "CancellationPolicy";
        public const string DateTimeFrom = "DateTimeFrom";
        public const string DateTimeTo = "DateTimeTo";
        public const string TotalPrice = "TotalPrice";
        public const string PaymentData = "PaymentData";
        public const string InvoiceCompany = "InvoiceCompany";
        public const string RegistrationNumber = "RegistrationNumber";
        public const string Description = "Description";
        public const string PaymentType = "PaymentType";
        public const string ServiceDetailList = "ServiceDetailList";
        public const string SeatingList = "SeatingList";
        public const string Seat = "Seat";
        public const string Gate = "Gate";
        public const string Row = "Row";
        public const string EntranceDoor = "EntranceDoor";
        public const string PaxId = "PaxId";
        public const string VoucherList = "VoucherList";
        public const string Voucher = "Voucher";
        public const string MimeType = "MimeType";
        public const string StartDate = "StartDate";
        public const string EndDate = "EndDate";
        public const string LanguageCode = "LanguageCode";

        #endregion PurchaseConfirm

        #region Ticket Nodes

        #region TicketInfo

        public const string TicketInfo = "TicketInfo";

        #endregion TicketInfo

        #region AvailableModality

        public const string AvailableModality = "AvailableModality";
        public const string SupplierOption = "SupplierOption";

        #endregion AvailableModality

        #region TicketAvail

        public const string TicketAvailRs = "TicketAvailRS";
        public const string ServiceTicket = "ServiceTicket";
        public const string ContentSequence = "ContentSequence";
        public const string CheapestPriceAdult = "CheapestPriceAdult";
        public const string CheapestPriceChild = "CheapestPriceChild";
        public const string DescriptionList = "DescriptionList";
        public const string BarcodeImage = "BarcodeImage";
        public const string BarcodeImageList = "BarcodeImageList";
        public const string TicketPosition = "TicketPosition";
        public const string ContentFactSheet = "ContentFactSheet";
        public const string Town = "Town";
        public const string Street = "Street";
        public const string Zip = "Zip";
        public const string ShortDescription = "ShortDescription";
        public const string Segmentation = "Segmentation";
        public const string SegmentationGroup = "SegmentationGroup";
        public const string OperationDays = "OperationDays";
        public const string Day = "Day";
        public const string TicketFeature = "TicketFeature";
        public const string FeatureList = "FeatureList";
        public const string Feature = "Feature";
        public const string PriceRangeList = "PriceRangeList";
        public const string PriceRange = "PriceRange";
        public const string OperationDate = "OperationDate";
        public const string OperationDateList = "OperationDateList";
        public const string PriceList = "PriceList";
        public const string TicketGeneration = "TicketGeneration";
        public const string TicketClass = "TicketClass";
        public const string CompanyCode = "CompanyCode";
        public const string Mode = "Mode";
        public const string Percentage = "Percentage";
        public const string ServiceOccupancy = "ServiceOccupancy";

        #endregion TicketAvail

        public const string Paxes = "Paxes";

        #endregion Ticket Nodes

        #region TicketValuation

        public const string TicketValuationRq = "TicketValuationRQ";
        public const string TicketValuationRs = "TicketValuationRS";
        public const string TicketCode = "TicketCode";
        public const string ModalityCode = "ModalityCode";
        public const string ServiceDetail = "ServiceDetail";

        #endregion TicketValuation
    }
}