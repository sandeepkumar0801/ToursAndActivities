namespace Isango.Persistence.Constants
{
    public sealed class Constants
    {
        public const string IsangoDb = "IsangoDB";
        public const string IsangoLiveDb = "IsangoLiveDB";
        public const string API_Upload = "APIUpload";
        public const string PrimalIdentitiesDb = "DefaultConnection";
        public const string APIUploadDb = "APIUpload";
        public const string GetActivityDetailSp = "dbo.usp_get_Activity_detail";
        public const string ServiceId = "@ServiceID";
        public const string LanguageCode = "@LanguageCode";
        public const string LanguageCodeValue = "en";
        public const string SearchProductFullTextSp = "usp_get_search_product_fulltext";
        public const string Keyword = "@Keyword";
        public const string Region = "REGION";
        public const string RatingType = "RATINGTYPE";
        public const string Rating = "RATING";
        public const string GetAffiliateByDomain = "usp_get_affiliatebydomain";
        public const string LicenseKey = "@LicenseKey";
        public const string GetCurrencyForAffiliates = "dbo.usp_Get_CurrencyForAffiliates";
        public const string GetCurrencyfromCurrencyCode = "dbo.usp_GetCurrencyName_Code_Symbol_from_Currency";

        public const string GetCurrencyFromCountryCode =
            "dbo.usp_GetCurrencyCode_from_CurrencyToCountry_given_CountryCode";

        public const string AffiliateId = "@AffiliateID";
        public const string Categoryid = "@categoryids";

        public const string Currecycode = "@CurrencyCode";
        public const string CountryCode = "@CountryCode";
        public const string GetFilteredThemeparkTicketsSp = "usp_get_origin_service_restriction";
        public const string GetCountryServiceRestriction = "usp_get_country_service_restriction";
        public const string GetRegionIdFromGeotreeSp = "dbo.usp_get_liveregionbyserviceid";
        public const string GetCountriesSp = "dbo.usp_Get_Address_Countries";
        public const string RandomRefNoParam = "RandomRefNo";
        public const string TransFlowID = "@TransflowID";
        public const string PSPReference = "@PSPReference";
        public const string ReturnStatus = "@Returnstatus";
        public const string Reason = "@reason";
        public const string IsCustomerMailSent = "@IsCustomerMailSent";
        public const string IsSupplierMailSent = "@IsSupplierMailSent";
        public const string IsSuccess = "@isSuccess";

        public const string GetRegionCoordinatesSp = "usp_get_regioncoordinates";

        public const string CompanyWebSite = "@companywebsite";
        public const string Alias = "@alias";

        public const string GetAffiliateServiceNo = "dbo.usp_Get_AffiliateServiceNo";
        public const string GetCrossSale = "usp_get_cross_sale";
        public const string GetCrossSaleLogic = "usp_get_cross_sell_lob";
        public const string GetUnityUrLs = "IsangoLive.dbo.usp_get_UnityURLs";

        public const string GetMasterAutoSuggestDataSp = "usp_Get_AutoComplete_Text"; //usp_get_predectiveMaster
        public const string SiteType = "@SiteType";
        public const string GetWebsiteSiteMap = "dbo.usp_Website_SiteMap";
        public const string GetAttractionUrLtoRedirectList = "dbo.usp_get_AttractionURLtoRedirectList ";
        public const string GetExchangeRatesForChangeCurrency = "usp_Get_ExchangeRatesForChangeCurrency";

        public const string GetMicrositeCanonicalsURL = "dbo.usp_get_MicrositeCanonicalsURL";
        public const string GetBlogs = "dbo.usp_get_guidebookblogs";

        public const string GetBlogSqlCommand =
            "select id, title, link, description, publishDate, category, imagepath from dbo.[GuidebookBlogs]";

        public const string GetAffiliateFilter = "dbo.usp_Get_Affiliate_Authorization";

        public const string GetServiceAttractionsForSearch = "usp_Get_service_attractions4Search";
        public const string GetRegionMetadata = "usp_get_regionMetadata";
        public const string RegionId = "@RegionID";
        public const string AttractionId = "@attractionID";
        public const string LanguageCodeForLoadRegionMetaData = "@languagecode";

        public const string GetLicenseKeySp = "dbo.usp_Get_LicenseKey";

        public const string GetisangoLocaleMerchandising = "dbo.usp_get_isangoLocaleMerchandising";
        public const string SourceName = "sourcename";
        public const string SourceId = "sourceid";
        public const string Url = "url";
        public const string ImageName = "imagename";
        public const string ImageId = "imageid";
        public const string Locale = "locale";
        public const string TypeId = "typeid";
        public const string Language = "language";
        public const string ReaderAffiliateId = "affiliateId";

        public const string NewsLetterSubscription = "dbo.usp_Ins_Upd_NewsLetterSubscription";
        public const string EmailId = "@EmailID";
        public const string Subscribe = "@Subscribe";
        public const string Status = "@Status";
        public const string LanguageCodeParam = "@LanguageCode";
        public const string SubscriberName = "@SubscriberName";
        public const string Countryid = "@countryid";

        public const string NewsLetterSubscriptionLog = "dbo.usp_Ins_NewsLetterSubscriptionLog";
        public const string Issubscribed = "@ISSUBSCRIBED";
        public const string Customerorigin = "@CUSTOMERORIGIN";
        public const string IsNBverified = "@ISNBVERIFIED";

        public const string GetAffiliateRegionForCategorySp = "dbo.usp_get_affiliateregion4category";
        public const string AffiliateIdForRegion = "@affiliateid";
        public const string AttractionIdForRegion = "@attractionid";

        public const string CartId = "@CartID";
        public const string AddReviewSp = "dbo.usp_Ins_Review";
        public const string AddReviewImageSp = "dbo.usp_Ins_Upd_ReviewImage";
        public const string UserID = "@UserID";
        public const string Title = "@Title";
        public const string ReviewComments = "@ReviewComments";
        public const string OverallRating = "@OverallRating";
        public const string BookingReferenceID = "@BookingReferenceNumber";
        public const string StatusId = "@StatusID";
        public const string ReviewDate = "@ReviewDate";
        public const string ClientBrowser = "@ClientBrowser";
        public const string ReviewID = "@ReviewID";
        public const string MinSize = "@MinSize";
        public const string MaxSize = "@MaxSize";
        public const string Caption = "@Caption";
        public const string TotalRecords = "@TotalRecords";
        public const string ActivityId = "@ActivityID";
        public const string RecsPerPage = "@RecsPerPage";
        public const string PageNumber = "@PageNumber";
        public const string GetProductReviewsSp = "dbo.usp_get_Activity_Reviews";
        public const string GetReviewsSp = "dbo.usp_get_SiteReview";
        public const string GetAllProductReviewsDataSp = "dbo.Usp_Get_Review_Data";

        public const string GetSupportedLanguagesSp = "dbo.usp_get_supportedlanguages";
        public const string GetDisneyHotelsSp = "dbo.usp_get_disney_hotels";
        public const string GetHbActivitiesV1Sp = "dbo.usp_get_HB_ActivitiesV1";
        public const string GetLiveServicesSp = "dbo.usp_Get_LiveServices";
        public const string GetActivityByMultipleIds = "dbo.usp_get_Activities_detail";

        //public const string GetCalendarAvailability = "dbo.usp_ins_90DayOptionAvailabilityPrice_V1";
        public const string GetCalendarAvailability = "dbo.usp_get_90DayOptionAvailabilityPrice";
        public const string GetCalendarFlag = "dbo.usp_Get_AvaiabilityCacheRefresh";

        public const string ServiceIds = "@ServiceIDs";
        public const string ParamServiceId = "@ServiceID";
        public const string ParamTokenID = "@TokenID";
        public const string ParamAPIReferenceNumber = "@APIReferenceNumber";
        public const string GetAffiliateExtendedLanguageMappingSp = "usp_get_Affiliate_ExtendedLanguageMapping";
        public const string GetHbRegionMappingSp = "dbo.usp_get_HBRegionMapping";

        public const string GetAlternateWorkFlowCategory = "dbo.usp_get_alternativeWorkflow_category";
        public const string RegionIdReader = "regionid";
        public const string AttractionIdReader = "attractionid";
        public const string OrderReader = "order_";

        public const string GetProductWithAffiliateCommission = "usp_Get_ProductWithAffiliateCommission";
        public const string CacheAffiliateId = "AffiliateID";
        public const string CacheServiceId = "serviceid";
        public const string CommissionPercent = "CommissionPercent";
        public const string B2BNetPrice = "B2BNetPrice";
        public const string CurrencyIsoCode = "currencyisocode";
        public const string MaxSell = "MaxSell";
        public const string CostAmount = "CostAmount";
        public const string APITypeID = "APITypeID";

        public const string GetHBLiveService = "dbo.usp_get_HBLiveService";
        public const string CacheServiceCode = "ServiceCode";
        public const string CacheFactSheetId = "factsheetid";
        public const string CacheMinAdult = "MinAdult";

        public const string NoiseData =
            ",about,1,after,2,all,also,3,an,4,and,5,another,6,any,7,are,8,as,9,at,0,be,$,because,been,before,being,between,both,but,by,came,can,come,could,did,do,each,for,from,get,got,has,had,he,have,her,here,him,himself,his,how,if,in,into,is,it,like,make,many,me,might,more,most,much,must,my,never,now,of,on,only,or,other,our,out,over,said,same,see,should,since,some,still,such,take,than,that,the,their,them,then,there,these,they,this,those,through,to,too,under,up,very,was,way,we,well,were,what,where,which,while,who,with,would,you,your,a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z,";

        public const string GetGliAgeGroupsAllActivities = "usp_get_gli_agegroups_for_all_activities";
        public const string GetPrioAgeGroupsAllActivities = "usp_get_Prio_AgeGroups_For_All_Activities";
        public const string GetPrioHubAgeGroupsAllActivities = "usp_get_Prio_AgeGroups_For_All_Activities";
        public const string GetAotAgeGroupsAllActivities = "usp_get_AOT_AgeGroups_For_All_Activities";
        public const string CachedActivityId = "ActivityId";
        public const string AgeGroupId = "AgeGroupId";
        public const string FromAge = "FromAge";
        public const string ToAge = "ToAge";
        public const string Description = "Description";
        public const string MinimumSellingPrice = "MinimumSellingPrice";
        public const string OriginalPrice = "OriginalPrice";
        public const string CancellationNoteId = "Cancellation_Id";
        public const string CancellationNoteValue = "Cancellation_Text";

        public const string GetAutoCompleteText = "usp_Get_AutoComplete_Text";
        public const string CachedKeyword = "keyword";
        public const string DisplayName = "displayname";
        public const string Category = "Category";
        public const string Destinations = "Destinations";
        public const string TopAttractions = "Top Attractions";
        public const string ToursAndActivity = "Tours and Activity";
        public const string D = "D";
        public const string C = "C";
        public const string A = "A";
        public const string SeoUrl = "seourl";
        public const string CacheParentId = "ParentID";
        public const string RefId = "refid";
        public const string IsTop = "IsTop";
        public const string CacheLanguageCode = "languagecode";
        public const string GetActivityIdSp = "dbo.usp_get_RedirectedIDs";
        public const string ServiceIdForGetActivityId = "@serviceid";

        public const string GetPredectiveHotelPmbSp = "usp_get_predectivehotel_PMB";
        public const string InsertMissingRoomsSp = "usp_ins_MissingRooms";
        public const string GetHotelListSp = "usp_get_Hotelist";
        public const string GetHotelDistanceForShowSp = "usp_get_HotelDistance4Show";
        public const string GetHotelDetailsSp = "usp_get_hotel_detail";

        public const string GetFacilityHotelSp = "usp_GetFacility_Hotel";

        public const string GetComputeActivityPricePerDayForAllB2BNetPriceSp =
            "usp_Get_ComputeActivityPriceperday4all_B2BNetPrice";

        public const string GetAlloptionAvailabilitySp =
            "usp_service_Alloption_availabilityRange";

        public const string GetComputeActivityPricePerDayForAll = "usp_Get_ComputeActivityPriceperday4all";
        public const string OccupancyString = "@OccupancyString";

        public const string GetFareHarborCustomerPrototypeForAllActivitiesSp =
            "usp_get_FHB_customerPrototype_for_all_activities";

        public const string GetFareHarborAgeGroupForAllActivitiesSp = "usp_get_FHB_Agegroup_for_all_activities";
        public const string GetFareHarborKeySp = "usp_get_fareharbourKey";

        public const string GetBokunPriceCategoryForAllActivities =
            "dbo.usp_get_Bokun_PriceCategory_for_all_activities";

        public const string SPCheckActivityType = "dbo.CheckActivityType";
        public const string Flag = "@FLAG";
        public const string uspGetAffiliatesWidgetPartner = "usp_Get_Affiliates_WidgetPartner";
        public const string uspGetLivecitiesAPI = "usp_get_livecities_api";
        public const string uspGetLivecategoriesAPI = "usp_get_livecategories_api";
        public const string uspGetCategorytopservice = "usp_get_categorytopservice";

        public const string LoadMaxPaxQuery =
            "select maxadults,maxpax from [dbo].[AOT_MaxPaxDetails] where serviceid=@serviceid";

        public const string uspGetIsValidNewsletterSubscriber = "dbo.usp_Get_IsValidNewsletterSubscriber";

        public const string CompleteIsangoBookingAfterTransactionSp =
            "dbo.usp_upd_booking_afterAlternativePayment";

        public const string Guid = "@GUWID";
        public const string UpdateSofortChargeBackSp = "Update_Sofort_Chargeback";
        public const string TransId = "@transId";
        public const string ParamTransId = "@TransID";
        public const string ParamDate = "@Date";
        public const string SofortStatus = "@status";
        public const string ParamRequestXml = "@RequestXML";
        public const string ParamResponseXml = "@ResponseXML";
        public const string ParamRequestType = "@RequestType";
        public const string ParamTransGUWID = "@TransGUWID";
        public const string InsertWirecardXmlSp = "dbo.usp_Ins_WirecardXML";
        public const string GetBookingRefByTransIdSp = "usp_get_BookingbySofortTransaction";
        public const string TransactionId = "@transactionID";
        public const string BookingRefNo = "BookingRefNo";

        public const string BookingRefNoParam = "@BookingRefNo";
        public const string Bookingjsonparam = "@Bookingjson";
        public const string AffiliateIdParam = "@AffiliateID";
        public const string StatusParam = "@Status";
        public const string IsAlternativePaymentParam = "@IsAlternativePayment";
        public const string Is3DParam = "@Is3D";
        public const string CardTypeParam = "@CardType";
        public const string PaymentGatewayTypeIdParam = "@PaymentGatewayTypeId";

        public const string SpGetCustomerService = "dbo.usp_Get_CustomerService";
        public const string SpGetAffiliateReleaseTag = "dbo.usp_get_AffiliateReleaseTag";
        public const string SpUpdateCustomerService = "dbo.usp_upd_AffiliateReleaseTag";
        public const string IsForAllParam = "@IsForAll";
        public const string ReleaseTagParam = "@ReleaseTag";
        public const string ResultParam = "@Result";

        public const string GetActivityOptionDataSp = "dbo.usp_Get_ActivityOptions";
        public const string ServiceOptionId = "ServiceOptionID";
        public const string PriceId = "priceID";
        public const string FromDate = "FromDate";
        public const string ToDate = "ToDate";

        public const string SPGetPageURList = "dbo.usp_get_pageURList";
        public const string GetIsangoModifiedServicesSp = "dbo.usp_Get_IsangoModifiedServices";
        public const string UpdateIsangoModifiedServicesSp = "dbo.usp_Update_IsangoModifiedServices";

        public const string GetProductSaleRules = "dbo.usp_get_IsangoSaleRule";
        public const string GetProductCostSaleRules = "dbo.usp_get_IsangoCostSaleRule";

        public const string GetB2BNetRateRules = "dbo.usp_get_B2BNetRateRule";
        public const string GetB2BSaleRules = "dbo.usp_Get_B2BSaleRule";
        public const string GetSupplierSaleRules = "dbo.usp_get_SupplierSaleRule";

        public const string SPInsOptionAvailabilityOnlyCache = "dbo.usp_ins_OptionAvailabilityOnlyCache";
        public const string SPGetOptionAvailabilityOnlyCache = "dbo.usp_get_OptionAvailabilityOnlyCache";
        public const string GetPassengerInfoSp = "usp_Get_ActivityAgeGroup";
        public const string GetB2BWhileLabelSp = "usp_Get_B2bWhileLabel";
        public const string GetUsersAffiliateSp = "usp_get_UsersAffiliate";
        public const string GetModifiedAffiliatesSp = "usp_get_ModifiedAffiliates";

        public const string GetSightseeingItalyMappingSp = "usp_get_SightseeingItaly_Mapping";
        public const string ParamApiTypeId = "@apiTypeID";
        public const string ParamApiType = "@APIType";
        public const string ParamServiceOptionId = "@serviceoptionid";
        public const string ParamOptionID = "@OptionID";

        public const string BookingTrackerSp = "dbo.usp_ins_api_bookingtracker";
        public const string ParamApiRefNo = "@api_refno";
        public const string ParRequestXml = "@requestxml";
        public const string ParResponseXml = "@responsexml";
        public const string ParamBType = "@btype";
        public const string ParamSupplierId = "@supplierid";
        public const string ParIsangoBookingId = "@isango_bookingid";
        public const string ParamBookingRefNo = "@isango_refno";
        public const string DuplicateBookingSp = "dbo.usp_get_booking4duplicateCheck";
        public const string LogBookingFailureSp = "dbo.usp_ins_Failed_Booking_Logs";
        public const string ValidateBookingByEmailSp = "usp_validate_BookingbyEmail";
        public const string GetBookingTravelDateSp = "dbo.usp_get_booking_travelDate";
        public const string GetBookingDetailForVoucherSp = "Usp_get_booking_detail4voucher";
        public const string GetBookingDetailByStatusB2BSp = "dbo.usp_Get_BookingDetail_By_Status_b2b";
        public const string GetBookingDetailForMailBodySp = "usp_Get_booking_detail4mailbody";
        public const string GetBookingReferenceNumber = "IsangoLive.dbo.usp_validate_auto_Booking_reference_no";
        public const string UpdateAdyenWebhookinDB = "dbo.usp_ins_AdyenWebhook";
        public const string GetAdyenWebhookfromDB = "IsangoLive.dbo.usp_Get_AdyenWebhookStatus";
        public const string InsertTokenAvailability = "dbo.usp_ins_Token_Availability";
        public const string UpdateTokenAvailability = "dbo.usp_upd_Token_Availability_RefNo";
        public const string GetTokenAvailability = "dbo.usp_get_Token_Availability";


        public const string ParamFortokeID = "@tokenID";
        public const string ParamForAvailabilityRefID = "@AvailabilityRefID";
        public const string ParamBookingRefnoForMail = "@bookingrefno";
        public const string ParamSource = "@Source";
        public const string ParamForCustomer = "@ForCustomer";
        public const string ParamForBookedOptionId = "@bookedoptionid";
        public const string ParamUserEmailId = "@useremailid";
        public const string ParamIsValid = "@IsValid";
        public const string ParamSmcPassengerId = "@smcPassengerId";
        public const string ParamTravelDate = "@travelDate";
        public const string ParamServiceOptionIdForBooking = "@serviceOptionId";
        public const string ParamAdultCount = "@adultCount";
        public const string ParamLeadPaxName = "@leadPaxName";
        public const string ParamAffiliateIdForBooking = "@affiliateID";
        public const string ParamVoucherEmail = "@VoucherEmail";
        public const string CreateBooking = "dbo.usp_ins_CreateBooking";
        public const string GenerateBookingRefNo = "dbo.usp_Get_GenerateBookingRefNo";

        public const string CreateReversalTransactionB2BSp = "dbo.usp_Create_Reversal_Transaction_b2b";
        public const string ParamBookingID = "@BookingID";
        public const string TransPrice = "@TransPrice";
        public const string ClientIpAddress = "@ClientIPAddress";
        public const string ParamGuid = "@GuWID";
        public const string AuthorizationCode = "@AuthorizationCode";
        public const string ParamITransId = "@i_TransID";

        public const string GetReceiveDetailSp = "usp_get_receiveDetail";
        public const string FinancialBookingTransactionId = "@Financialbookingtransactionid";
        public const string GetPartialRefundDetailSp = "dbo.usp_get_partialRefundDetail";
        public const string InsReceiveDetailSp = "dbo.usp_ins_ReceiveDetail";
        public const string ParamFinancialBookingTransactionId = "@financialbookingtransactionid";
        public const string IPurchaseTransId = "@i_PurchaseTransID";
        public const string InsPartialRefundSp = "dbo.usp_ins_PartialRefund";
        public const string ParamRemarks = "@Remarks";
        public const string ParamActionBy = "@ActionBy";
        public const string GetHotelBedsCredentialsSp = "GetHotelBedsCredentials";
        public const string GetGliPickupLocationForAllActivitiesSp = "usp_get_gli_pickuplocation_for_all_activities";

        public const string GetPaxPrices = "dbo.usp_Get_PaxPrice";
        public const string GetCSRegionMapping = "dbo.usp_get_CS_RegionMapping";
        public const string CheckInDate = "@CheckIn";
        public const string CheckOutDate = "@CheckOut";
        public const string PaxDetail = "@Paxdetail";

        //MailRuleEngine
        public const string KayakoActionTypeId = "@kayako_ActionTypeid";

        public const string GetKayakoMailHeaderSp = "usp_get_KayakoMailHeader";

        public const string InsertPartialBookingSp = "dbo.usp_ins_PartialBookingItem";
        public const string AvailabilityReferenceId = "@AvailabilityReferenceId";
        public const string ItemStatus = "@Status";
        public const string SelectedProductId = "@SelectedProductId";
        public const string BookingReferenceNumber = "@bookingreferencenumber";
        public const string PartialBookingItemId = "@Id";

        public const string GetPassengerDetail4MyIsangoSp = "usp_get_passengerdetail4myisango";
        public const string GetBookingDetail4MyIsangoSp = "usp_get_bookingdetail4myisango";
        public const string UpdatePassengerDetail4MyIsangoSp = "usp_update_passengerdetail4myisango";
        public const string SaveEmailPreferncesForUserSp = "usp_save_emailPreferncesForUser";
        public const string GetPreference4MyIsangoSp = "usp_get_preference4myisango";
        public const string GetUserCreationDateSp = "usp_Get_UserCreationDate";

        public const string GetBookingDetailForConfirmationPage = "usp_Get_Booking_Detail4ConfirmationPage";
        public const string GetApiPassengerType = "usp_Get_APIPassengerType";
        public const string ServiceCancellationPolicy = "usp_Get_ServiceCancellationPolicy";

        //duplicate in merging
        //public const string BookedOptionId = "BookedOptionId";
        public const string GetReceiveDetail = "usp_get_receiveDetail";

        public const string UpdateBookingTransaction = "usp_Update_BookingTransaction";
        public const string CaptureTransID = "@CaptureTransID";
        public const string CaptureGuWID = "@CaptureGuWID";
        public const string CaptureAuthorizationCode = "@CaptureAuthorizationCode";
        public const string PreAuthTransID = "@PreAuthTransID";
        public const string PreAuthGuWID = "@PreAuthGuWID";
        public const string PreAuthAuthorizationCode = "@PreAuthAuthorizationCode";

        #region Voucher Persistence Constants

        public const string GetVoucherDetailSp = "dbo.usp_get_VoucherDetail";
        public const string GetDiscountCategoryConfigSp = "dbo.usp_get_PromoCategoryConfig";
        public const string ParamPromoCode = "@PromoCode";

        public const string UpdateAPISupplierBookingQRCode = "dbo.usp_upd_APISupplierBookingQRCode";
        public const string QRCodeValue = "@QRCodeValue";
        public const string QRCodeType = "@QRCodeType";
        public const string IsQRCodePerPax = "@IsQRCodePerPax";
        public const string MultiQRcode = "@multiQRcode";
        #endregion Voucher Persistence Constants

        #region ProfilePersistence

        public const string ParamPassengerId = "@smcpassengerid";
        public const string ParamAffiliateId = "@affiliateid";
        public const string ParamIsCancelledBooking = "@IsCancelledbooking";
        public const string ParamAgentName = "@AgentName";
        public const string ParamPassengerFirstName = "@smcpassengerfirstname";
        public const string ParamPassengerLastName = "@smcpassengerlastname";
        public const string ParamLoginId = "@loginid";
        public const string ParamLoginPassword = "@LoginPassword";
        public const string ParamAddressTelephoneNumber = "@addresstelephonenumber";
        public const string ParamUserId = "@UserId";
        public const string ParamAnswerIds = "@AnswerIds";
        public const string GetMyIsangoSp = "usp_get_myisangobookingdetail";
        public const string ParamEmailId = "@Email";
        public const string ParamIsAgent = "@IsAgent";

        #endregion ProfilePersistence

        #region Console Application

        //AOT
        public const string SyncAOTMappingSp = "usp_sync_AOT_Mapping";

        public const string InsertAOTGeneralInfoSp = "dbo.usp_ins_AOT_GeneralInfo";
        public const string ServiceCode = "@ServiceCode";
        public const string OptionName = "@Name";
        public const string ParamOptionName = "@OptionName";
        public const string ParamAvailabilityReferenceNumber = "@AvailabilityReferenceNumber";
        public const string ParamErrorMsg = "@ErrorMessage";
        public const string ParamErrorLvl = "@ErrorLevel";
        public const string SupplierCode = "@SupplierCode";
        public const string SupplierName = "@SupplierName";
        public const string Comment = "@Comment";
        public const string BeddingConfiguration = "@BeddingConfiguration";
        public const string MaxPaxDesc = "@MaxPaxDesc";
        public const string LocationId = "@LocationID";
        public const string LocationName = "@LocationName";
        public const string MinChildAge = "@MinChildAge";
        public const string MaxChildAge = "@MaxChildAge";
        public const string ChildPolicyDescription = "@ChildPolicyDescription";
        public const string InfantAgeFrom = "@InfantAgeFrom";
        public const string InfantAgeTo = "@InfantAgeTo";
        public const string ChildAgeFrom = "@ChildAgeFrom";
        public const string ChildAgeTo = "@ChildAgeTo";
        public const string AdultAgeFrom = "@AdultAgeFrom";
        public const string AdultAgeTo = "@AdultAgeTo";
        public const string MaxAdults = "@MaxAdults";
        public const string MaxPax = "@MaxPax";
        public const string Periods = "@Periods";
        public const string SType = "@SType";
        public const string Mpfcu = "@Mpfcu";
        public const string Scu = "@Scu";
        public const string MinScu = "@MinScu";
        public const string MaxScu = "@MaxScu";
        public const string Inclusions = "@Inclusions";
        public const string OptionImportantInfo = "@OptionImportantInfo";
        public const string Images = "@Images";
        public const string Description1 = "@Description1";
        public const string OptExtras = "@OptExtras";
        public const string CancellationPolicy = "@CancellationPolicy";
        public const string GetHBLiveOptionsSp = "dbo.usp_get_HBLiveOptions";
        public const string GetActivityIdFromOptionId = "dbo.usp_Get_ServiceId";
        public const string GetTokenByAvailabilityReferenceId = "dbo.usp_get_Token_Availability";
        public const string GetHBLiveOptionsApiTudeContentSp = "dbo.usp_get_HBLiveOptions_for_API_Request";

        //GrayLineIceLand
        public const string InsertUpdateAgeGroupsSp = "GLI_usp_Ins_Upd_AgeGroup";

        public const string InsertUpdateActivityAgeGroupSp = "GLI_usp_Ins_Upd_ActivityAgeGroup";
        public const string DeleteActivityAgeGroupByActivityIdSp = "GLI_usp_Del_ActivityAgeGroupsByActivityId";
        public const string InsertUpdatePickupLocationSp = "GLI_usp_Ins_Upd_PickupLocation";
        public const string InsertUpdateActivityPickupLocationSp = "GLI_usp_Ins_Upd_ActivityPickupLocation";
        public const string SyncGLIMasterDataSp = "usp_sync_GLI_MasterData";

        public const string DeleteActivityPickupLocationsByActivityIdSp =
            "GLI_usp_Del_ActivityPickupLocationsByActivityId";

        public const string GLIAgeGroupId = "@AgeGroupId";
        public const string Ticked = "@ticked";
        public const string GLIToAge = "@ToAge";
        public const string Id = "@Id";
        public const string ProductId = "@ProductId";
        public const string Address = "@Address";
        public const string GLIActivityId = "@ActivityId";
        public const string Lat = "@Lat";
        public const string Long = "@Long";
        public const string ZipCode = "@ZipCode";
        public const string City = "@City";
        public const string Price = "@Price";
        public const string IsCheckinLocation = "@isCheckinLocation";
        public const string PickupLocationId = "@PickupLocationId";
        public const string PickupTime = "@PickupTime";
        public const string TimePUMinutes = "@TimePUMinutes";

        //FareHarbor
        public const string SyncFareHarborMasterDataSp = "usp_sync_FHB_MasterData";

        public const string InsUpdCustomerPrototypeCustomerTypeSp = "FHB_usp_Ins_Upd_CustomerPrototypeCustomerType";
        public const string InsUpdCompaniesSp = "FHB_usp_Ins_Upd_Companies";
        public const string InsUpdCompaniesMappingSp = "FHB_usp_Ins_Upd_CompaniesMapping";
        public const string GetCompaniesProductsSp = "FHB_usp_GetCompaniesProducts";
        public const string GetFareHarborUserKeysSp = "dbo.usp_GetFareHarborUserKeys";
        public const string Pk = "@pk";
        public const string Total = "@total";
        public const string FareHarborDisplayName = "@display_name";
        public const string FareHarborServiceId = "@ServiceId";
        public const string CustomerTypePk = "@customerTypepk";
        public const string Singular = "@singular";
        public const string Plural = "@plural";
        public const string Note = "@note";
        public const string TourPk = "@tourpk";
        public const string StartAt = "@startat";
        public const string EndAt = "@endat";
        public const string Tourminpax = "@tourminpax";
        public const string Tourmaxpax = "@Tourmaxpax";
        public const string prototypeminpax = "@prototypeminpax";
        public const string prototypemaxpax = "@prototypemaxpax";
        public const string Currency = "@currency";
        public const string ShortName = "@shortname";
        public const string FareHarborSupplierName = "@name";
        public const string UserKey = "@userkey";
        public const string CompanyCancellationPolicy = "@cancellationpolicy";
        public const string CompanyCancellationPolicyHtml = "@cancellationpolicyhtml";
        public const string TourId = "@tourID";
        public const string Days2FetchForFHBData = "Days2FetchForFHBData";

        //Prio
        public const string SyncPrioTicketMappingSp = "usp_sync_PrioTicket_Mapping";

        public const string InsertPrioTicketAgeGroupSP = "PrioTicket_usp_ins_PrioTicket_AgeGroup";
        public const string PrioTicketId = "TicketId";
        public const string PrioDescription = "Description";
        public const string PrioFromAge = "FromAge";
        public const string PrioToAge = "ToAge";
        public const string PrioAgeGroupxml = "PrioAgeGroupxml";

        public const string InsertPrioTicketRouteMap = "prioTicket_usp_ins_RouteMap";
        public const string InsertPrioTicketDetails = "prioTicket_usp_ins_PriocontentDetail";
        public const string PrioTicketRouteMap = "@RouteMap";
        public const string PrioRouteMapTableType = "PrioRouteMapTableType";
        public const string PrioProductDetailTableType = "PrioTicketProduct";
        public const string InsertPrioTicketList = "prioTicket_usp_ins_Productdetail";
        public const string PrioTicketListParameter = "@PrioTicketProductdetail";
        public const string PrioTicketListMapTableType = "PrioTicketProductdetail";
        //Tiqets
        public const string InsertTiqetsPackageSP = "usp_Ins_InsertTiqetsPackage";
        public const string InsertTiqetsVariantsSP = "tiq_usp_Ins_TiqetsVariants";
        public const string InsertTiqetsProdDetailsSP = "tiq_usp_ins_tiqetproduct";
        public const string InsertTiqetsVariantsSPIntoTemp = "tiq_usp_Ins_TiqetsVariants_intoTiqetsVariantsTemp";


        public const string VariantId = "@variantid";
        public const string Label = "@label";
        public const string VariantDescription = "@description";
        public const string RequiresVisitorsDetails = "@requires_visitors_details";
        public const string MaxTickets = "@max_tickets";
        public const string Commission = "@distributor_commission_excl_vat";
        public const string RetailPrice = "@total_retail_price_incl_vat";
        public const string TicketValue = "@sale_ticket_value_incl_vat";
        public const string TiqetsBookingFee = "@booking_fee_incl_vat";
        public const string TiqetsProductId = "@productid";
        public const string ValidWithVariantIds = "@valid_with_variant_ids";
        public const string requireVisitor = "@requires_visitors_details";
        public const string TiqetsProductDetailTableType = "tiqProductList";
        public const string TiqetsProductDetailTableName = "TiqetsProductList";
        public const string Package_Title = "@Package_Title";
        public const string Product_ID = "@Product_ID";
        public const string Package_ID = "@Package_ID";


        public const string ProductIdTiqets = "@productid";
        public const string ServiceIdTiqets = "@serviceid";
        public const string StartingPoint = "@starting_point";
        public const string SkipTheLine = "@isskiptheline";
        public const string SmartPhoneticket = "@issmartphoneticket";
        public const string Duration = "@duration";
        public const string Venue = "@venue";
        public const string InstantTicket = "@instant_Ticket";

        //Service Availabilities
        public const string DeleteHBServiceDetailSp = "usp_del_HBServiceDetail";

        public const string SyncAPIPriceAvailabilitySp = "dbo.usp_sync_APIPriceAvailability";
        public const string InsertErrorLoggerSp = "dbo.usp_ins_ErrorLogger";
        public const string Message = "Message";
        public const string Destination = "Destination";
        public const string ErrorCheckInDate = "CheckInDate";
        public const string ErrorCheckOutDate = "CheckOutDate";
        public const string ErrorFactSheetIds = "FactSheetIds";
        public const string InsertTempHBServiceDetailSp = "dbo.usp_Ins_TempHBServiceDetail";
        public const string InsertTempHBServiceDetailByPaxSp = "dbo.usp_Ins_TempHBServiceDetailByPax";
        public const string InsertTempHBServiceDetailTiqetsByPaxSp = "dbo.usp_Ins_TempHBTiqetsServiceDetailsByPax";
        public const string AvailabilityParameter = "@tvAvailability";
        public const string AvailabilityTableType = "AvailabilityTableType";
        public const string PriceAvailabilityTableType = "PriceAvailabilityTableType";
        public const string QuestionsTableType = "APIQuestionsTableType";
        public const string AnswerOptionsTableType = "APIAnswerOptionsTableType";
        public const string APIPickUpLocationsTableType = "APIPickUpLocationsTableType ";
        public const string APIassignedPickUpLocationsTableType = "APIassignedPickUpLocationsTableType";
        public const string DropOffLocationsTableType = "APIDropofflocationtabletype";
        public const string APIAssignedDropofflocationtabletype = "APIAssignedDropofflocationtabletype";
        public const string PassengerType = "PassengerTypeID";
        public const string InsertAPIQuestions = "API_usp_ins_Question";
        public const string QuestionParameter = "@Question";
        public const string AssignedQuestionsTable = "APIAssignedQuestionsTableType";
        public const string AssignedQuestionsParameter = "@Assigned_question";
        public const string AnswerParameter = "@QuestionAnswer";
        public const string PickUpLocationParameter = "@PickupLocation";
        public const string AssignedPickUpLocationParameter = "@AssignedPickuplocation";
        public const string DropOffLocationParameter = "@DropLocation";
        public const string AssignedDropOffLocationParameter = "@assignedDroplocation";
        public const string InsertAPIPickUpLocations = "API_usp_ins_Pickuplocation";
        public const string InsertAPIDropOffLocations = "API_usp_ins_Droplocation";
        public const string SyncAPIQuestions = "dbo.usp_sync_ApiQuestions";

        // Golden Tours
        public const string InsertGoldenToursAgeGroupsSp = "usp_ins_GoldenTours_AgeGroup";

        public const string InsertGoldenToursProductDetailsSp = "GT_usp_ins_GoldenTours_ProductDetails";
        public const string InsertGoldenToursPricePeriodsSp = "GT_usp_ins_productcapacity";

        public const string ProductDetailsParameter = "@productDetails";
        public const string PricePeriodParameter = "@GTproductcapacity";
        public const string PriceUnitsParameter = "@priceUnits";
        public const string GoldenToursProductDetails = "GoldenToursProductDetails";
        public const string GoldenToursAgeGroups = "GoldenToursAgeGroups";
        public const string GoldenToursPricePeriods = "GoldenToursPricePeriods";

        //Ventrata
        public const string VentrataProductDetailsTbl = "Ventrata_product";
        public const string VentrataDestinationTbl = "Ventrata_Destination";
        public const string VentrataFaqTbl = "Ventrata_FAQ";
        public const string VentrataOptionTbl = "Ventrata_ProuductOption";
        public const string VentrataUnitForOptionTbl = "Ventrata_Units";

        public const string VentrataPackageInclude = "VentrataPackageInclude";

        public const string VentrataProductDetailParameter = "@VentrataProduct";
        public const string VentrataDestinationParameter = "@ventrataDestination";
        public const string VentrataFaqParameter = "@VentrataFAQ";
        public const string VentrataOptionParameter = "@VentrataProductOption";
        public const string VentrataUnitInOptionParameter = "@VentrataUnits";

        public const string VentrataPackagesIncludeParameter = "@VentrataPackagesInclude";


        public const string VentrataProductDetailsProcedure = "usp_ins_ventrataProduct";
        public const string VentrataDeleteProductDetailsProcedure = "usp_del_ventrataproduct";
        public const string VentrataDestinationProcedure = "dbo.usp_ins_ventrataDestination";
        public const string VentrataFaqProcedure = "dbo.usp_ins_VentrataFAQ";
        public const string VentrataOptionProcedure = "dbo.usp_ins_VentrataProductOption";
        public const string VentrataUnitsForOptionProcedure = "dbo.usp_ins_VentrataUnits";

        public const string VentrataPackagesIncludeProcedure = "dbo.Usp_ins_ventratapackagesInclude";

        public const string VentrataSupplierDetailsProcedure = "usp_get_ventratra_supplier";

        //Bokun

        public const string BokunProductDetailsTbl = "Bokun_Product";
        public const string InsertBokunProductDetailsSp = "Bokun_usp_ins_Product";
        public const string BokunProductParameter = "@BokunProduct";

        public const string BokunCancellationDetailsTbl = "Bokun_CancellationPolicy";
        public const string InsertBokunCancellationPolicySp = "Bokun_usp_ins_CancellationPolicy";
        public const string CancellationPolicyParameter = "@Cancellationpolicy";

        public const string BokunRatesTbl = "Bokun_Rate";
        public const string BokunInsertRatesSp = "Bokun_usp_ins_Rate";
        public const string BokunRatesParameter = "@BokunRates";

        public const string BookableExtraQuestionsTable = "Bokun_BookableExtraQuestions";
        public const string BookabeExtraTable = "Bokun_BookableExtra";
        public const string BokunInsertBookableExtraSp = "Bokun_usp_ins_Bokun_BookableExtra";
        public const string BokunBookableExtraParameter = "@Bokun_BookableExtra";
        public const string BokunInsertBookabelExtraQuesSp = "Bokun_usp_ins_Bokun_BookableExtraQuestions";
        public const string BokunBookableExtraQuesParameter = "@Bokun_BookableExtraQuestions";

        public const string InsertBokunSyncMappingSp = "usp_sync_Bokun_Mapping";

        //APITUDE
        public const string InsertApiTudeAgeGroupsSp = "hbt_usp_ins_ServiceAgeGroup";

        public const string InsertApiTudeQuestions = "hbt_usp_ins_Question";

        public const string HBTServiceAgeGroup = "HBTServiceAgeGroup";
        public const string HBTQuestionsTable = "HBTQuestion";
        public const string HBTServiceAgeGroupParameter = "@HBTServiceAgeGroup";

        public const string InsertApiTudeProductFactSheetdetail = "hbt_Usp_Insert_ProductFactSheetdetail";
        public const string HBTProductFactSheet = "dbo.HBTProductFactSheet";
        public const string HBTProductFactSheetParameter = "@ProductFactSheet";
        public const string HBTLanguageParameter = "@Language";
        public const string ServiceOptionIDParameter = "@ServiceOptionID";

        public const string InsertDestinationDetail = "hbt_Usp_Insert_Destination_Detail";
        public const string HBTDestination = "dbo.HBTDestination";
        public const string HBTDestinationParameter = "@HBTDestination";
        public const string HBTLanguageCodeParameter = "@LanguageCode";
        public const string HBTRequiredParameter = "@Required";
        public const string HBTQuestionParameter = "@Question";
        public const string HBTQuestionCodeParameter = "@QuestionCode";

        public const string InsertCountry = "hbt_usp_ins_country";
        public const string HBTCountry = "dbo.HBTCountry";
        public const string HBTCountryParameter = "@HBTCountry";

        public const string InsertLocation = "hbt_usp_ins_location";
        public const string HBTLocation = "dbo.HBTLocation";
        public const string HBTLocationParameter = "@HBTLocation";

        public const string InsertFeature = "hbt_usp_ins_feature_detail";
        public const string HBTFeature = "dbo.HBTfeature";
        public const string HBTFeatureParameter = "@HBTfeature";

        public const string InsertMedia = "hbt_usp_ins_Media";
        public const string InsertAPIImages = "API_usp_ins_Media";
        public const string HBTMedia = "dbo.HBTMedia";
        public const string HBTMediaParameter = "@HBTMedia";
        public const string APIMediaParameter = "@APIMedia";

        public const string InsertRoute = "hbt_usp_ins_routes";
        public const string HBTRoute = "dbo.HBTroutes";
        public const string HBTRouteParameter = "@HBTroutes";

        public const string InsertDescription = "HBT_usp_ins_detailinfo";
        public const string HBTDescriptionInfo = "dbo.HBTDetailinfo";
        public const string HBTDescritionParameter = "@HBTDetailinfo";

        public const string InsertRedeemInfo = "HBT_usp_ins_redeeminfo";
        public const string HBTRedeeminfo = "dbo.HBTRedeeminfo";
        public const string HBTRedeeminfoParameter = "@HBTRedeeminfo";

        public const string InsertScheduling = "HBT_usp_ins_Scheduling";
        public const string HBTScheduling = "dbo.HBTScheduling";
        public const string HBTSchedulingParameter = "@HBTScheduling";

        public const string GetProductFactSheet = "HBT_usp_get_productfactsheet";

        public const string InsertDuration = "HBT_usp_ins_activityDuration";
        public const string HBTDuration = "dbo.HBTActivityDuration";
        public const string HBTDurationParameter = "@HBTActivityDuration";

        public const string InsertHighLights = "HBT_usp_ins_activityhighlights";
        public const string HBTHighLights = "dbo.HBThighlight";
        public const string HBTHighLightsParameter = "@HBThighlight";

        public const string InsertOperationalDays = "HBT_usp_ins_operationdays";
        public const string HBTOperationalDays = "dbo.HBToperationalDays";
        public const string HBTOperationalDaysParameter = "@HBToperationalDays";

        // Redeam
        public const string InsertRedeamSuppliersSp = "Redeam_usp_ins_supplier";
        public const string InsertRedeamV12SuppliersSp = "RedeamV12_usp_ins_supplier";

        public const string InsertRedeamProductsSp = "Redeam_usp_ins_Product";
        public const string InsertRedeamV12ProductsSp = "RedeamV12_usp_ins_Product";
        public const string InsertRedeamPriceSp = "Redeam_usp_ins_Price";
        public const string InsertRedeamV12PriceSp = "RedeamV12_usp_ins_Price";
        public const string InsertRedeamRateSp = "Redeam_usp_ins_Rate";
        public const string InsertRedeamV12RateSp = "RedeamV12_usp_ins_Rate";
        public const string InsertRedeamTravelerTypeSp =
            "Redeam_usp_ins_PriceTravelType";
        public const string InsertRedeamV12TravelerTypeSp =
           "RedeamV12_usp_ins_PriceTravelType";

        public const string RedeamSuppliersParameter = "@supplier";
        public const string RedeamProductsParameter = "@Product";
        public const string RedeamPriceParameter = "@Price";
        public const string RedeamRateParameter = "@Rate";
        public const string RedeamTravelerTypeParameter = "@PriceTravelType";

        public const string RedeamSuppliers = "Supplier";
        public const string RedeamV12Suppliers = "RedeamV2Supplier";
        public const string RedeamProducts = "Product";
        public const string RedeamV12Products = "Redeam12Product";
        public const string RedeamPrice = "Price";
        public const string RedeamV12Price = "RedeamV12Price";
        public const string RedeamRate = "Rate";
        public const string RedeamV12Rate = "RedeamV12Rate";
        public const string RedeamTravelerType = "PriceTravelType";
        public const string RedeamV12TravelerType = "RedeamV12PriceTravelType";

        // Rezdy
        public const string InsertRezdyAgeGroupsSP = "Rezdy_usp_ins_AgeGroup";

        public const string InsertRezdyProductsSP = "Rezdy_usp_ins_Product";
        public const string InsertRezdyBookingFieldsSP = "Rezdy_usp_ins_booking";
        public const string InsertRezdyExtraDetailsSP = "Rezdy_usp_ins_bookableextra";
        public const string AgeGroupParameter = "@Agegroup";
        public const string BookableExtraParameter = "@RezdyBookableextra";

        public const string BookingFieldParameter = "@Booking";
        public const string RezdyProductDetailsParamter = "@Product";
        public const string GetSupplierSP = "Rezdy.usp_get_supplierID";
        public const string RezdyProductDetails = "RezdyProductDetails";
        public const string RezdyAgeGroup = "RezdyAgeGroup";
        public const string RezdybookingFields = "RezdybookingFields";
        public const string RezdyBookableExtra = "RezdyBookableextra";

        public const string AgeGroupDBType = "[dbo].[RezdyAgeGroup]";
        public const string ProductDetailsDBType = "[dbo].[RezdyProduct]";
        public const string BookingFieldsDBType = "[dbo].[RezdyBooking]";
        public const string RezdyExtraDBType = "[dbo].[RezdyBookableextra]";
        public const string RezdyLabelDetails = "echo.usp_get_apilabel_detail";

        //----- Begin GlobalTix -----
        public const string DB_APIUpload = "APIUpload";

        public const string DataTable_CountryCity = "CountryCityTable";

        public const string DBTable_CountryCity = "dbo.GlobalTix_CountryCity";
        public const string DBTable_Product = "dbo.GlobalTix_Product_Master";
        public const string DBTable_ProductList = "dbo.GlobalTixV3_ProductList";
        public const string DBTable_ProductOptionV3 = "dbo.GlobalTixV3_ProductOption";
        public const string DBTable_ProductOption = "dbo.GlobalTix_Product_Options";
        public const string DBTable_ProductOptionTicket = "dbo.GlobalTix_Product_Option_Tickets";
        public const string DBTable_ProductOptionTicketV3 = "dbo.GlobalTixV3_TicketType";
        public const string DBTable_ProductListV3 = "dbo.GlobalTixV3_ProductList";



        public const int DBTable_WriteBatchSize_CountryCity = 500;
        public const int DBTable_WriteBatchSize_Product = 500;
        public const int DBTable_WriteBatchSize_ProductOption = 500;
        public const int DBTable_WriteBatchSize_ProductOptionTicket = 500;

        public const string Column_ApplyCapacity = "Apply_Capacity";
        public const string Column_CityId = "City_Id";
        public const string Column_CityName = "City_Name";
        public const string Column_CountryId = "Country_Id";
        public const string Column_CountryName = "Country_Name";
        public const string Column_CurrencyCode = "CurrencyCode";
        public const string Column_HoursOfOp = "Hours_Of_Operation";
        public const string Column_IsPackage = "Is_Package";
        public const string Column_OptionDesc = "Option_Desc";
        public const string Column_OptionId = "Option_Id";
        public const string Column_OptionName = "Option_Name";
        public const string Column_PassengerType = "PAX_type";
        public const string Column_Price = "Price";
        public const string Column_ProductDesc = "Product_Desc";
        public const string Column_ProductId = "Product_Id";


        public const string Column_ProductTitle = "Product_Title";
        public const string Column_TicketId = "Ticket_Id";
        public const string Column_TicketName = "Ticket_Name";
        public const string Column_Latitude = "latitude";
        public const string Column_Longitude = "Longitude";

        public const int ColLen_OptionDesc = 4000;
        public const int ColLen_ProductDesc = 4000;
        public const int ColLen_HoursOfOp = 4000;
        //V3 GLobal tix productlist
        public const string Column_Name = "Name";
        public const string Column_Currency = "Currency";
        public const string Column_City = "City ";
        public const string Column_OriginalPrice = "OriginalPrice";
        public const string Column_Country = "Country";
        public const string Column_IsOpenDated = "IsOpenDated ";
        public const string Column_IsOwnContracted = "IsOwnContracted ";
        public const string Column_IsFavorited = "IsFavorited";
        public const string Column_IsInstantConfirmation = "IsInstantConfirmation";
        public const string Column_IsBestSeller = "IsBestSeller";
        public const string Column_Category = "Category";
        //GlonalTix ProductOPtion
        public const string Column_Description = "Description";
        public const string Column_TicketValidity = "TicketValidity";
        public const string Column_TicketFormat = "TicketFormat";
        public const string Column_IsCapacity = "IsCapacity";
        public const string Column_Type = "Type";
        public const string Column_TimeSlot = "TimeSlot";
        public const string Column_IsCancellable = "IsCancellable";
        public const string Column_DemandType = "DemandType";
        //ProductTicketV3 GLOBALTIX
        public const string Column_Id = "Id";
        public const string Column_Sku = "Sku";
        //public const string Column_TicketName = "TicketName";
        public const string Column_OriginalPriceGt = "OriginalPrice";
        public const string Column_AgeFrom = "AgeFrom";
        public const string Column_AgeTo = "AgeTo";
        public const string Column_NettPrice = "NettPrice";
        public const string Column_MinimumSellingPrice = "MinimumSellingPrice";
        public const string Column_minimumMerchantSellingPrice = "minimumMerchantSellingPrice";
        public const string Column_recommendedSellingPrice = "recommendedSellingPrice";




        public const string Cmd_TruncTable_CountryCity = "truncate table " + DBTable_CountryCity;
        public const string Cmd_SelActivities_Product = "select Product_Id from " + DBTable_Product + " where Is_Package = 0";
        public const string Cmd_SelPackages_Product = "select Product_Id from " + DBTable_Product + " where Is_Package = 1";

        public const string Cmd_DelActivities_Product = "delete from " + DBTable_Product + " where Is_Package = 0";
        public const string Cmd_DelActivities_ProductOption = "delete from " + DBTable_ProductOption + " where Product_Id in (" + Cmd_SelActivities_Product + ")";
        public const string Cmd_DelActivities_ProductOptionTicket = "delete from " + DBTable_ProductOptionTicket + " where Product_Id in (" + Cmd_SelActivities_Product + ")";

        public const string Cmd_DelActivities_ProductOptionTicketGlobalTixV3 = "truncate table " + DBTable_ProductOptionTicketV3;
        public const string Cmd_DelActivities_ProductOptionGlobalTixV3 = "truncate table " + DBTable_ProductOptionV3;
        public const string Cmd_DelActivities_ProductGlobalTixV3 = "truncate table " + DBTable_ProductListV3;

        public const string Cmd_DelPackages_Product = "delete from " + DBTable_Product + " where Is_Package = 1";
        public const string Cmd_DelPackages_ProductOption = "delete from " + DBTable_ProductOption + " where Product_Id in (" + Cmd_SelPackages_Product + ")";
        public const string Cmd_DelPackages_ProductOptionTicket = "delete from " + DBTable_ProductOptionTicket + " where Product_Id in (" + Cmd_SelPackages_Product + ")";

        public const char Package_Prefix = 'P';

        public const string AgeDumpingAPIsProcedure = "dbo.usp_Get_API4AgeDumping";
        public const string APIImagesProcedure = "dbo.usp_get_image_data_for_api_dumping";
        public const string APIImagesUploadResultProcedure = "dbo.usp_ins_isango_image_data_for_api";
        public const string APIImagesUploadResulttParameter = "@APIimagelist";
        public const string APIImagesDeleteResulttParameter = "@APIDeleteimagelist";

        //----- End GlobalTix -----

        #endregion Console Application

        #region [Master]

        public const string GetDeltaAttractions = "usp_Get_Attractions_DeltaChange";
        public const string GetDeltaRegionAttraction = "usp_get_RegionAttraction_DeltaChange";
        public const string GetDeltaAffiliate = "usp_Get_Allaffiliate_DeltaChange";
        public const string GetDeltaRegionSubAttraction = "usp_get_RegionSubAttraction_DeltaChange";

        public const string GetDeltaActivity = "usp_get_Activity_DeltaChange";
        public const string GetDeltaPassengerInfo = "usp_Get_ActivityAgeGroup_DeltaChange";
        public const string GetDeltaReview = "usp_Get_Review_DeltaChange";
        public const string GetDeltaActivityMinPrice = "usp_Get_ServiceMinPrice_Delta";
        public const string GetDeltaActivityAvailability = "usp_Get_ServiceAvailability";
        public const string TiqetsPackages = "dbo.usp_get_TiqetsPackage";

        public const string APIAgeGroupSp = "dbo.usp_Get_APIAgeGroup";
        public const string ventrataPackages = "dbo.usp_get_VentrataPackageInclude";
        public const string GetDeltaGeoDetails = "usp_get_GeoDetails_delta";
        public const string GetDeltaDestination = "usp_get_destination_Delta";
        public const string GetDeltaProductSupplier = "usp_get_supplier_info_detail_delta";
        public const string GetMasterCurrency = "usp_Get_AllCurrencies";
        public const string GetMasterLanguages = "usp_Get_AllLanguages";
        public const string GetMasterGeoDetails = "usp_get_GeoDetails";
        public const string GetMasterRegionWise = "usp_get_regionwise_live_product_Detail_for_affliate";
        public const string GetAffiliateWiseServiceMinPrice = "usp_get_AffiliateWiseServiceMinPrice";
        public const string Affliateid = "Affliateid";

        #endregion [Master]

        #region [Marketing]

        public const string GetMarketingCJFeed = "usp_get_CJ_Feed";
        public const string Currencyid = "@currencyid";
        public const string GetMarketingCriteoFeed = "usp_get_criteofeed";

        public const string ParentProductId = "@ParentProductId";
        public const string ParentOptionId = "@ParentOptionId";

        #endregion [Marketing]

        #region Feeds

        public const string GetMerchantFeed = "usp_get_Merchant_google";
        public const string GetServiceAvailabilityFeed = "usp_Get_PaxPrice_Availability_Google";
        public const string GetAssignedServiceMerchant = "dbo.usp_Get_Assigned_Service_Merchant";
        public const string GetAllPassengerType = "dbo.usp_Get_AllPassengerType";

        public static string AvailabilityReferenceIds = "@AvailabilityReferenceIds";

        #endregion Feeds

        #region CancelBooking

        public const string UserId = "@UserID";
        public const string StatusID = "@StatusID";

        public const string GetCancellationPolicyAmount = "usp_Get_Cancellation_PolicyAmount_For_Booking_V1";
        public const string BookedOptionId_ri = "@ri_BookedOptionID";
        public const string CurrencyISOCode = "@CurrencyISOCode";
        public const string SPID = "@SPID";

        public const string GetCancellableSuppliersCancellationData = "dbo.usp_GetBookingsForCancellation";
        public const string GetUserPermission = "dbo.usp_Get_UserID";
        public const string UserName = "@Username";

        public const string CreateCancelBooking = "dbo.[usp_create_booking_cancellation_JSON]";
        public const string CancelledByUser = "@CancelledByUser";
        public const string CancelledById = "@CancelledByID";
        public const string rjson = "@rjson";

        public const string GetAllCancellationStatus = "usp_Get_CancellationStepstatus";
        public const string InsertUpdateCancellationStatus = "usp_ins_usp_Get_CancellationStepstatus";
        public const string BookedOptionId = "@bookedoptionid";
        public const string IsangoCancelStatus = "@IsangoCancelStatus";
        public const string PaymentRefundStatus = "@PaymentRefundStatus";
        public const string SupplierCancelStatus = "@SupplierCancelStatus";

        #endregion CancelBooking

        //TourCMS
        public const string TourCMSChannelList = "TCMS_ChannelList";
        public const string InsertTourCMSChannelListSp = "[TCMS_usp_ins_TourCMS_ChannelList]";
        public const string ChannelListParameter = "@channelList";
        public const string InsertTourCMSRedemptionData = "dbo.usp_insert_TourCMSRedemptionData";
        public const string RedemptionJsonData = "@JsonData";
        public const string GetChannelId = "dbo.usp_get_TourcmschannelId";


        public const string TourCMSTourList = "TCMS_TourList";
        public const string InsertTourCMSTourListSp = "[TCMS_usp_ins_TourCMS_TourList]";
        public const string TourListParameter = "@TourList";

        public const string TourCMSTourRateList = "TCMS_TourRateList";
        public const string InsertTourCMSTourRateListSp = "[dbo].[TCMS_usp_ins_TourCMS_RateList]";
        public const string TourRateListParameter = "@TourRateList";
        public const string GetTourCMSChannelListSP = "TCMS_usp_get_TourCMS_ChannelList";
        #region Booking Confirmation

        public const string BookingConfirmation = "usp_create_booking_confirmation";
        public const string UpdateTransaction = "usp_Update_Transaction_b2b";
        public const string CheckSendMailToCustomer = "Usp_get_mailbookedproduct2customer ";

        public const string CompletionStatus = "@CompletionStatus";
        public const string TransID = "@TransID";
        public const string GuWID = "@GuWID";
        public const string ConfirmBookingAuthorizationCode = "@AuthorizationCode";

        #endregion Booking Confirmation

        #region Partial Refund

        public const string InsertPartialRefund = "usp_ins_PartialRefund";
        public const string Remarks = "@Remarks";
        public const string ActionBy = "@ActionBy";
        public const string GateWayID = "@GateWayID";
        #endregion Partial Refund

        public const string NewCitySightSeeingProducts = "NewCitySightSeeing_ProductList";
        public const string InsertNewCitySightSeeingProductsSP = "[dbo].[NewCitySightSeeing_usp_ins_CitySightSeeing_Product]";
        public const string NewCitySightSeeingProductParamter = "@CitySightSeeingProductList";
        public const string NewCitySightSeeingProductDBType = "[dbo].[NewCitySightSeeing_ProductList]";

        public const string NewCitySightSeeingProductsVariant = "NewCitySightSeeing_ProductVariant";
        public const string InsertNewCitySightSeeingProductsSPVariant = "[dbo].[CitySightSeeing_usp_ins_CitySightSeeing_ProductVariant]";
        public const string NewCitySightSeeingProductParamterVariant = "@CitySightSeeingProductVariant";
        public const string NewCitySightSeeingProductDBTypeVariant = "[dbo].[NewCitySightSeeing_ProductVariant]";

        public const string GoCityProducts = "GoCity_ProductsForSale";
        public const string InsertGoCityProductsSP = "[dbo].[GoCity_usp_ins_GoCity_Product]";
        public const string GoCityProductParamter = "@GoCityProductList";
        public const string GoCityProductDBType = "[dbo].[GoCity_ProductsForSale]";

        public const string GetElasticProducts = "usp_get_ES_Servicelist";
        public const string GetElasticDestinations = "usp_Get_ES_Affiliate_RegionLandingPageURL";
        public const string InsertGeneratePaymentLinkSp = "dbo.Usp_Insert_GeneratePaymentLink";
        public const string GeneratePaymentLinkCurrency = "@Currency";
        public const string GeneratePaymentLinkValue = "@Value";
        public const string GeneratePaymentLinkCountryCode = "@CountryCode";
        public const string GeneratePaymentLinkDescription = "@Description";
        public const string GeneratePaymentLinkExpiresAt = "@ExpiresAt";
        public const string GeneratePaymentLinkId = "@Id";
        public const string GeneratePaymentLinkMerchantAccount = "@MerchantAccount";
        public const string GeneratePaymentLinkReference = "@Reference";
        public const string GeneratePaymentLinkShopperLocale = "@ShopperLocale";
        public const string GeneratePaymentLinkShopperReference = "@ShopperReference";
        public const string GeneratePaymentLinkUrl = "@Url";
        public const string UpdateGeneratePaymentLink = "dbo.usp_upd_GeneratePaymentLink";
        public const string CustomerEmail = "@CustomerEmail";
        public const string CustomerLanguage = "@CustomerLanguage";

        public const string NewPrioProduct = "NewPrioProduct";
        public const string InsertNewPrioProductSp = "[dbo].[NewPrio_usp_ins_Product]";
        public const string NewPrioProductListParameter = "@NewPrioProductList";
        public const string NewPrioProductDBType = "[dbo].[NewPrioProduct]";

        public const string NewPrioProductTypeSeasons = "NewPrioProductTypeSeasons";
        public const string InsertNewPrioProductTypeSeasonsProductSp = "[dbo].[NewPrio_usp_ins_ProductTypeSeasons]";
        public const string NewPrioProductTypeSeasonsParameter = "@NewPrioProductTypeSeasons";
        public const string NewPrioProductTypeSeasonsDBType = "[dbo].[NewPrioProductTypeSeasons]";

        public const string NewPrioProductExtraOptionsSeasons = "NewPrioProductExtraOptions";
        public const string InsertNewPrioProductExtraOptionsProductSp = "[dbo].[NewPrio_usp_ins_ProductExtraOptions]";
        public const string NewPrioProductExtraOptionsParameter = "@NewPrioProductExtraOptions";
        public const string NewPrioProductExtraOptionsDBType = "[dbo].[NewPrioProductExtraOptions]";

        public const string NewPrioProductExtraOptionsValuesSeasons = "NewPrioProductExtraOptionsValues";
        public const string InsertNewPrioProductExtraOptionsValuesProductSp = "[dbo].[NewPrio_usp_ins_ProductExtraOptionsValues]";
        public const string NewPrioProductExtraOptionsValuesParameter = "@NewPrioProductExtraOptionsValues";
        public const string NewPrioProductExtraOptionsValuesDBType = "[dbo].[NewPrioProductExtraOptionsValues]";

        public const string NewPrioProductRoute = "@NewPrioProductRoute";
        public const string InsertNewPrioProductRouteProductSp = "[dbo].[NewPrio_usp_ins_ProductRoute]";
        public const string NewPrioProductRouteParameter = "@NewPrioProductRoute";
        public const string NewPrioProductRouteDBType = "[dbo].[NewPrioProductRoute]";

        public const string NewPrioProductRouteLocation = "@NewPrioProductRouteLocation";
        public const string InsertNewPrioProductRouteLocationProductSp = "[dbo].[NewPrio_usp_ins_ProductRouteLocations]";
        public const string NewPrioProductRouteLocationParameter = "@NewPrioProductRouteLocation";
        public const string NewPrioProductRouteLocationDBType = "[dbo].[NewPrioProductRouteLocation]";
        public const string AffiliateDetail = "[dbo].[usp_Get_All_Affiliates]";


        public const string TemporaryRefNo = "@TemporaryRefNo";
        public const string CustomerContact = "@CustomerContact";
        public const string ParamAPICancellationStatus = "@APICancellationStatus";
        public const string GetElasticAttraction = "usp_get_ES_Attractionlist";
        public const string GetElasticAffiliate = "usp_Get_ES_AffiliateList";
        public const string InsertMRVoucher = "usp_ins_MRVoucher";

        public const string RaynaCountryCity = "Rayna_CountryCity";
        public const string InsertRaynaCountryCitySP = "[dbo].[Rayna_usp_ins_CountryCity]";
        public const string RaynaCountryCityParameter = "@RaynaCountryCityList";
        public const string RaynaCountryCityDBType = "[dbo].[Rayna_CountryCity]";

        public const string RaynaTourStaticData = "Rayna_TourStaticData";
        public const string InsertTourStaticDataSP = "[dbo].[Rayna_usp_ins_TourStaticData]";
        public const string RaynaTourStaticDataParameter = "@RaynaTourStaticData";
        public const string RaynaTourStaticDataDBType = "Rayna_TourStaticData";


        public const string RaynaTourStaticDataById = "Rayna_TourStaticDataById";
        public const string InsertRaynaTourStaticDataByIdSP = "[dbo].[Rayna_usp_ins_TourStaticDataById]";
        public const string RaynaTourStaticDataByIdParameter = "@RaynaTourStaticDataById";
        public const string RaynaTourStaticDataByIdDBType = "Rayna_TourStaticDataById";


        public const string RaynaTourOptions = "Rayna_TourOptions";
        public const string InsertTourOptionsSP = "[dbo].[Rayna_usp_ins_TourOptions]";
        public const string RaynaTourOptionsParameter = "@RaynaTourOptions";
        public const string RaynaTourOptionsDBType = "Rayna_TourOptions";

        public const string RaynaTourOptionsTransferTime = "Rayna_TourOptionsTransferTime";
        public const string InsertTourOptionsTransferTimeSP = "[dbo].[Rayna_usp_ins_TourOptionsTransferTime]";
        public const string RaynaTourOptionsTransferTimeParameter = "@RaynaTourOptionsTransferTime";
        public const string RaynaTourOptionsTransferTimeDBType = "Rayna_TourOptionsTransferTime";

        public const string GetPersonTypeOptionCacheAvailability = "dbo.usp_get_OptionCacheAvailability";

        public const string InsertTokenAndRefIds = "dbo.usp_ins_Token_Availability";

        public const string InsertImageAltText = "dbo.usp_ins_imageAltText";

        public const string GetCvPoints = "dbo.usp_Get_AirVistaraPoints";

        public const string SaveAllCssExternalProducts = "dbo.usp_Ins_Upd_ExternalProducts";
        public const string CssProductOptionId = "@CssProductOptionId";
        public const string IsangoProductOptionId = "@IsangoProductOptionId";
        public const string productName = "@productName";
        public const string CssProductId = "@CssProductId";
        public const string supplierId = "@supplierId";

        public const string SaveAllCssBooking = "dbo.usp_Ins_Upd_CssBooking";
        public const string CssReferenceNumber = "@CssReferenceNumber";
        public const string IdempotancyKey = "@IdempotancyKey";
        public const string bookedOptionId = "@bookedOptionId";
        public const string Process = "@Process";


        public const string GetOptionId = "dbo.usp_Get_External_OptionId";
        public const string CssBookingRequired = "dbo.usp_Get_IsBookingDoneInCss";

        public const string GetCssCancellation = "usp_Get_CancellationForCss";
        public const string GetBookingData = "dbo.usp_get_Booking_Css";

        public const string GetpassengerDetail = "dbo.usp_Get_CssPassengerDetail";
        public const string GetCssRedemption = "dbo.usp_Get_RedemptionForCss";

        public const string InsertRedemptionData = "dbo.usp_ins_RedemptionApiData";
        public const string GetRedeamV12SupplierData = "usp_get_RedeamV12_SupplierData";
        public const string InsertApiErrorLog = "dbo.usp_ins_ApiErrorLog";

        public const string Update_GlobalV3TixData = "usp_update_GlobalV3TixData";


    }
}