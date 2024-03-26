using Isango.Entities;
using Isango.Entities.Affiliate;
using Isango.Entities.Booking;
using Isango.Entities.Booking.ConfirmBooking;
using Isango.Entities.Booking.PartialRefund;
using Isango.Entities.Enums;
using Isango.Entities.Payment;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Util;

namespace Isango.Persistence.Data
{
    public class BookingData
    {
        #region Declaration

        private int _activityId;
        private int _optionId;

        #endregion Declaration

        public string GetBookingRefNo(IDataReader reader)
        {
            return DbPropertyHelper.StringDefaultPropertyFromRow(reader, "BookingRefNo");
        }

        public void LoadBookingData(IDataReader reader, ref Booking booking)
        {
            if (booking == null)
                booking = new Booking();

            while (reader.Read())
            {
                booking.User = new ISangoUser();
                booking.Affiliate = new Affiliate();
                booking.Language = new Language();
                booking.Currency = new Currency();

                // booking.Affiliate.AffiliateConfiguration.IsOfflineEmail = bool.Parse(ConfigurationManagerHelper.GetValuefromAppSettings("IsOffline"));
                booking.ReferenceNumber = DbPropertyHelper.StringPropertyFromRowWithTrim(reader, "bookingrefno");
                booking.Affiliate.Id = DbPropertyHelper.StringPropertyFromRowWithTrim(reader, "affiliateid");
                booking.BookingId = int.Parse(reader["bookingid"].ToString());
                booking.Currency.Symbol = DbPropertyHelper.StringPropertyFromRow(reader, "currencysymbol");                                //TODO: need discussion
                booking.User.FirstName = DbPropertyHelper.StringPropertyFromRowWithTrim(reader, "passengerfirstname");
                booking.User.LastName = DbPropertyHelper.StringPropertyFromRowWithTrim(reader, "passengerlastname");
                booking.Affiliate.TermsAndConditionLink = DbPropertyHelper.StringPropertyFromRowWithTrim(reader, "TermsAndConditionLink");
                booking.Affiliate.Name = DbPropertyHelper.StringPropertyFromRowWithTrim(reader, "AffiliateName");
                booking.Affiliate.AffiliateCompanyDetail = new AffiliateCompanyDetail
                {
                    CompanyEmail = DbPropertyHelper.StringPropertyFromRowWithTrim(reader, "FromMail"),
                    CompanyWebSite = DbPropertyHelper.StringPropertyFromRow(reader, "companywebsite")
                };
                booking.Affiliate.AffiliateBaseURL = DbPropertyHelper.StringPropertyFromRow(reader, "AffiliateBaseUrl");
                booking.Affiliate.GroupId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "AffiliateGroupID");                         //TODO: need discussion
                booking.VoucherEmailAddress = DbPropertyHelper.StringPropertyFromRowWithTrim(reader, "VoucherEmail");
                booking.Language.Code = DbPropertyHelper.StringPropertyFromRowWithTrim(reader, "languagecode");
            }
        }

        public void LoadProductData(IDataReader reader, ref Booking booking)
        {
            reader.NextResult();
            booking.SelectedProducts = new List<SelectedProduct>();
            while (reader.Read())
            {
                //1	On Request
                //2	Confirmed
                //3	Canceled
                //4	Confirmed
                //5	On Request
                var option = new ProductOption();
                var product = new SelectedProduct { ProductOptions = new List<ProductOption> { option } };
                option.Customers = new List<Customer> { new Customer() };
                var bookedStatusId = DbPropertyHelper.Int32PropertyFromRow(reader, "bookedoptionstatusid");
                if (bookedStatusId == 2 || bookedStatusId == 4)
                {
                    option.BookingStatus = OptionBookingStatus.Confirmed;
                    option.AvailabilityStatus = AvailabilityStatus.AVAILABLE;
                }
                else if (bookedStatusId == 3)
                {
                    option.BookingStatus = OptionBookingStatus.Cancelled;
                }
                else if (bookedStatusId == 1 || bookedStatusId == 5)
                {
                    option.BookingStatus = OptionBookingStatus.Requested;
                    option.AvailabilityStatus = AvailabilityStatus.ONREQUEST;
                }

                //product.Sequence = count;     //TODO: need discussion
                product.Name = DbPropertyHelper.StringPropertyFromRow(reader, "servicename");

                var price = new Price
                {
                    Amount = DbPropertyHelper.DecimalPropertyFromRow(reader, "grossellingamount")
                };

                product.ProductOptions.First().SellPrice = price;

                product.ProductOptions.First().Name = DbPropertyHelper.StringPropertyFromRow(reader, "ServiceOptionName");

                //product.CurrencySymbol = bookingData.CurrencySymbol;      //TODO: need discussion
                product.MultisaveDiscountedPrice = DbPropertyHelper.DecimalPropertyFromRow(reader, "multisavediscount");
                if (reader["DISCOUNT_AMOUNT"] != DBNull.Value)
                    product.DiscountedPrice = DbPropertyHelper.DecimalPropertyFromRow(reader, "DISCOUNT_AMOUNT");
                product.Id = DbPropertyHelper.Int32PropertyFromRow(reader, "serviceid");
                if (reader["IsReceipt"] != DBNull.Value)
                    product.SetIsReceipt(DbPropertyHelper.BoolPropertyFromRow(reader, "IsReceipt"));

                if (product.ProductOptions.First().Customers != null)
                    product.ProductOptions.First().Customers[0].IsLeadCustomer = true;

                product.ProductOptions.First().TravelInfo = new TravelInfo()
                {
                    NoOfPassengers = new Dictionary<PassengerType, int>
                    {
                        { PassengerType.Adult, DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "Adults")},
                        { PassengerType.Child, DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "Children")}
                    },
                    StartDate = DbPropertyHelper.DateTimePropertyFromRow(reader, "TravelDate")
                };

                var customerName = DbPropertyHelper.StringPropertyFromRow(reader, "LeadPaxName").Split(' ');
                if (customerName.Length > 0 && product.ProductOptions.First().Customers != null)
                {
                    option.Customers[0].FirstName = customerName[0];
                    for (var surNameSubParts = 1; surNameSubParts < customerName.Length; surNameSubParts++)
                    {
                        var lastName = product.ProductOptions.First().Customers[0].LastName;
                        if (lastName != null)
                            option.Customers[0].LastName += customerName[surNameSubParts];
                    }
                }

                if (DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "IsHotelBedsAPIProduct"))
                {
                    product.APIType = APIType.Hotelbeds;
                }

                //TODO: Need to discuss
                product.BundleOptionId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "bookedoptionid");

                product.LinkType = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "QRCodeType");
                product.LinkValue = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "QRCode");
                product.APIType = (APIType)Enum.Parse(typeof(APIType), DbPropertyHelper.StringDefaultPropertyFromRow(reader, "APITYPEID"));

                //((HotelBedsSelectedProduct)product).VatNumber = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "VATNo").Trim();
                //((HotelBedsSelectedProduct)product).OfficeCode = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "officecode").Trim();
                product.IsSmartPhoneVoucher = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "IsSmartPhoneVoucher");
                product.LinkType = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "QRCodeType");
                product.LinkValue = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "QRCode");
                product.IsShowSupplierVoucher = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "IsShowSupplierVoucher");
                product.AvailabilityReferenceId = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "AvailabilityReferenceId");
                product.SupplierConfirmationData = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "supplierbookingreferencenumber");
                product.CancellationPolicy = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "CANCELLATIONPOLICY");
                product.LineOfBusiness = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "LINEOFBUSINESSID");
                product.RegionId = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "REGIONID");
                product.BUNDLESERVICEID = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "BUNDLESERVICEID");
                booking.SelectedProducts.Add(product);
            }
        }

        public void LoadPerPaxPdfData(IDataReader reader, ref Booking booking)
        {
            var paxWisePdfDetails = new List<PaxWisePdfEntity>();
            reader.NextResult();
            while (reader.Read())
            {
                var paxWisePdf = new PaxWisePdfEntity
                {
                    BookedOptionId = DbPropertyHelper.Int32PropertyFromRow(reader, "bookedoptionid"),
                    CodeValue = DbPropertyHelper.StringPropertyFromRow(reader, "BarCode"),
                    PassengerType = DbPropertyHelper.StringPropertyFromRow(reader, "PassengerType")
                };
                paxWisePdfDetails.Add(paxWisePdf);
            }

            if (paxWisePdfDetails != null && paxWisePdfDetails.Count > 0)
            {
                foreach (var item in booking.SelectedProducts)
                {
                    item.PaxWisePdfDetails = new List<PaxWisePdfEntity>();
                    item.PaxWisePdfDetails = paxWisePdfDetails.Where(x => x.BookedOptionId == item.BundleOptionId).ToList();
                }
            }
        }

        public void LoadAgeDetailData(IDataReader reader, ref Booking booking)
        {
            var ageGroupDescription = new List<AgeGroupDescription>();
            reader.NextResult();
            while (reader.Read())
            {
                var ageGroup = new AgeGroupDescription
                {
                    Id = DbPropertyHelper.Int32PropertyFromRow(reader, "bookedoptionid"),
                    Description = DbPropertyHelper.StringPropertyFromRow(reader, "AgeGroupDesc"),
                    PaxCount = DbPropertyHelper.Int32PropertyFromRow(reader, "PaxCount"),
                    PassengerType = DbPropertyHelper.StringPropertyFromRow(reader, "PassengerType"),
                    FromAge = DbPropertyHelper.Int32PropertyFromRow(reader, "FromAge"),
                    ToAge = DbPropertyHelper.Int32PropertyFromRow(reader, "ToAge"),
                    PaxSellAmount = DbPropertyHelper.DecimalNullablePropertyFromRow(reader, "BOOKEDPASSENGERRATESELLAMOUNT"),
                    PaxSupplierCostAmount = DbPropertyHelper.DecimalNullablePropertyFromRow(reader, "suppliercostamount")
                };
                ageGroupDescription.Add(ageGroup);
            }
            if (ageGroupDescription != null && ageGroupDescription.Count > 0)
            {
                foreach (var item in booking.SelectedProducts)
                {
                    item.AgeGroupDescription = new List<AgeGroupDescription>();
                    item.AgeGroupDescription = ageGroupDescription.Where(x => x.Id == item.BundleOptionId).ToList();
                }
            }
        }

        public List<BookingDetail> LoadBookingDetail(IDataReader reader)
        {
            var bookingDetails = new List<BookingDetail>();
            while (reader.Read())
            {
                var bookingDetail = new BookingDetail
                {
                    BookingDetailId = DbPropertyHelper.Int32PropertyFromRow(reader, "bookingdetailid"),
                    ServiceName = DbPropertyHelper.StringPropertyFromRow(reader, "servicename"),
                    ServiceId = DbPropertyHelper.Int32PropertyFromRow(reader, "serviceid"),
                    BookingCurrency = DbPropertyHelper.StringPropertyFromRow(reader, "bookingcurrency"),
                    SupplierCurrency = DbPropertyHelper.StringPropertyFromRow(reader, "suppliercurrency"),
                    ServiceOptionId = DbPropertyHelper.Int32PropertyFromRow(reader, "serviceoptionid"),
                    PassengerId = DbPropertyHelper.Int32PropertyFromRow(reader, "passengerid"),
                    IsHotelbed = DbPropertyHelper.BoolPropertyFromRow(reader, "IsHotelbedsProduct"),
                    HbReferenceNo =
                        DbPropertyHelper.StringDefaultPropertyFromRow(reader, "supplierbookingreferencenumber"),
                    OfficeCode = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "officecode"),
                    LanguageCode = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "LanguageCode"),
                    TrakerStatusId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "trakerstatusid")
                };
                bookingDetails.Add(bookingDetail);
            }
            return bookingDetails;
        }

        public string GetTravelDate(IDataReader reader)
        {
            return
                $"{DbPropertyHelper.StringPropertyFromRow(reader, "AFFILIATEID")}^{DbPropertyHelper.StringPropertyFromRow(reader, "TravelDate")}^{DbPropertyHelper.StringPropertyFromRow(reader, "CusotmerEmail")}";
        }

        public Booking GetAmendedBooking(IDataReader reader)
        {
            var amendedBooking = new Booking { Payment = new List<Payment>() };
            var noOfRecords = 0;
            while (reader.Read())
            {
                amendedBooking.Payment.Add(new Payment());
                amendedBooking.User = new ISangoUser();
                amendedBooking.Payment[noOfRecords].AuthorizationCode = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "AuthorizationCode").Trim();
                amendedBooking.Payment[noOfRecords].ChargeAmount = DbPropertyHelper.DecimalPropertyFromRow(reader, "TransPrice");
                amendedBooking.Payment[noOfRecords].CurrencyCode = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "CurrencyCode").Trim();
                amendedBooking.Payment[noOfRecords].Guwid = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "GuWID").Trim();
                amendedBooking.Payment[noOfRecords].TransactionId = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "BookbackTransID").Trim();
                amendedBooking.Payment[noOfRecords].Is3D = DbPropertyHelper.BoolPropertyFromRow(reader, "Is3DSecure");
                if (noOfRecords == 0)
                {
                    amendedBooking.ReferenceNumber = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "BookingRefNo").Trim();
                    amendedBooking.User.UserId = DbPropertyHelper.Int32PropertyFromRow(reader, "UserID");
                }
                noOfRecords++;
            }

            return amendedBooking;
        }

        public PaymentBookingData GetPaymentBookingData(IDataReader reader, bool isPaymentRelatedBookingData)
        {
            var recPaymentData = new PaymentBookingData();
            while (reader.Read())
            {
                recPaymentData.BookingRefNo = DbPropertyHelper.StringPropertyFromRowWithTrim(reader, "bookingReferenceNumber");
                recPaymentData.AffiliateId = DbPropertyHelper.StringPropertyFromRowWithTrim(reader, "affiliateID");
                recPaymentData.LanguageCode = DbPropertyHelper.StringPropertyFromRowWithTrim(reader, "languageCode");
                recPaymentData.CustomerEmailAdd = DbPropertyHelper.StringPropertyFromRowWithTrim(reader, "voucherEmail");
                recPaymentData.BookedOptionId = DbPropertyHelper.Int32PropertyFromRow(reader, "bookedOptionID");
                recPaymentData.CustomerPhoneNo = DbPropertyHelper.StringPropertyFromRowWithTrim(reader, "PhoneNumber");
                recPaymentData.ChargedAmount = DbPropertyHelper.DecimalPropertyFromRow(reader, "chargeAmount");
                recPaymentData.CurrencyName = DbPropertyHelper.StringPropertyFromRowWithTrim(reader, "sellCurrency");
                recPaymentData.CurrencySymbol = DbPropertyHelper.StringPropertyFromRowWithTrim(reader, "currencySymbol");
                recPaymentData.BookedProductName = DbPropertyHelper.StringPropertyFromRowWithTrim(reader, "serviceName");
                recPaymentData.BookedOptionName = DbPropertyHelper.StringPropertyFromRowWithTrim(reader, "serviceOptionName");
                recPaymentData.CSRemarks = DbPropertyHelper.StringPropertyFromRowWithTrim(reader, "Remarks");
                recPaymentData.UserId = DbPropertyHelper.Int32PropertyFromRow(reader, "userid");
                recPaymentData.CompanyWebsite = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "CompanyWebsite").Trim();
                recPaymentData.AffiliateName = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "AffiliateName").Trim();
                recPaymentData.CustomerName = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "LeadPaxName").Trim();
                if (isPaymentRelatedBookingData)
                {
                    recPaymentData.CustomerCountryId = DbPropertyHelper.Int32PropertyFromRow(reader, "CountryID");
                }
            }

            return recPaymentData;
        }

        public ConfirmBookingDetail MapBookingDetail(IDataReader reader)
        {
            var confirmBookingDetail = new ConfirmBookingDetail();
            while (reader.Read())
            {
                confirmBookingDetail.AffiliateId =
                    DbPropertyHelper.StringDefaultPropertyFromRow(reader, "affiliateid");
                confirmBookingDetail.BookingDate =
                    DbPropertyHelper.DateTimePropertyFromRow(reader, "bookingdate");
                confirmBookingDetail.BookingId =
                    DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "bookingid");
                confirmBookingDetail.CurrencyIsoCode =
                    DbPropertyHelper.StringDefaultPropertyFromRow(reader, "currencyisocode");
                confirmBookingDetail.LanguageCode =
                    DbPropertyHelper.StringDefaultPropertyFromRow(reader, "languagecode");
                confirmBookingDetail.VoucherEmail =
                    DbPropertyHelper.StringDefaultPropertyFromRow(reader, "voucheremail");
            }
            if (reader.NextResult())
            {
                confirmBookingDetail.BookedOptions = new List<BookedOption>();
                while (reader.Read())
                {
                    var bookedOption = new BookedOption
                    {
                        BookedOptionId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "BOOKEDOPTIONID"),
                        ServiceId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "SERVICEID"),
                        BookedOptionStatusId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "BOOKEDOPTIONSTATUSID"),
                        LeadPaxName = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "LeadPaxName"),
                        ServiceName = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "ServiceName"),
                        ServiceStatus = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "ServiceStatus"),
                        TravelDate = DbPropertyHelper.DateTimePropertyFromRow(reader, "TravelDate"),
                        ChargedAmount = DbPropertyHelper.DecimalDefaultNullablePropertyFromRow(reader, "ChargedAmount"),
                        DiscountAmount = DbPropertyHelper.DecimalDefaultNullablePropertyFromRow(reader, "DiscountAmount"),
                        SellAmount = DbPropertyHelper.DecimalDefaultNullablePropertyFromRow(reader, "SellAmount"),
                        MutliSaveDiscount = DbPropertyHelper.DecimalDefaultNullablePropertyFromRow(reader, "MultiSaveDiscount"),
                        IsReceipt = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "IsReceipt"),
                        IsQRCodePerPax = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "IsQRCodePerPax"),
                        LinkType = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "QRCodeType"),
                        LinkValue = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "QRCode"),
                        AvailabilityReferenceId = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "AvailabilityReferenceId"),
                        IsShowSupplierVoucher = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "IsShowSupplierVoucher"),
                        BookedOptionName = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "BookedOptionName"),
                        SupplierCode = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "SupplierCode"),
                        BookedPassengerDetails = new List<BookedPassengerDetail>(),
                        ApiType = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "ApiTypeId")
                    };
                    confirmBookingDetail.BookedOptions.Add(bookedOption);
                }
            }
            if (reader.NextResult())
            {
                while (reader.Read())
                {
                    var bookedOptionId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "BOOKEDOPTIONID");
                    var bookedPassengerDetail = new BookedPassengerDetail
                    {
                        AgeGroupDescription = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "AgeGroupDesc"),
                        PaxCount = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "PaxCount"),
                        QRCodeDetail = new List<BookedPassengerQRCodeDetail>()
                    };

                    try
                    {
                        bookedPassengerDetail.PassengerTypeId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "PassengerTypeId");
                    }
                    catch (Exception ex)
                    {
                    }

                    confirmBookingDetail.BookedOptions.FirstOrDefault(x => x.BookedOptionId == bookedOptionId)?.BookedPassengerDetails.Add(bookedPassengerDetail);
                }
            }
            if (reader.NextResult())
            {
                while (reader.Read())
                {
                    var bookedOptionId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "BOOKEDOPTIONID");
                    var bookedpaxQRdetail = new BookedPassengerQRCodeDetail
                    {
                        BookedOptionID = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "BOOKEDOPTIONID"),
                        BarCode = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "BarCode"),
                        PassengerType = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "PassengerType")
                    };
                    var passengerDetailsForThisPax = confirmBookingDetail.BookedOptions.FirstOrDefault(x => x.BookedOptionId == bookedOptionId)?.BookedPassengerDetails.Find(thisPass => thisPass.AgeGroupDescription.Contains(bookedpaxQRdetail.PassengerType));
                    if (passengerDetailsForThisPax != null)
                    {
                        passengerDetailsForThisPax.QRCodeDetail.Add(bookedpaxQRdetail);
                    }
                    else
                    {
                        confirmBookingDetail.BookedOptions.FirstOrDefault(x => x.BookedOptionId == bookedOptionId)?.BookedPassengerDetails.FirstOrDefault()?.QRCodeDetail.Add(bookedpaxQRdetail);
                    }
                }
            }
            return confirmBookingDetail;
        }

        public PartialRefundPaymentData MapPartialRefundData(IDataReader reader)
        {
            var partialRefundPaymentData = new PartialRefundPaymentData
            {
                Amount = DbPropertyHelper.DecimalPropertyFromRow(reader, "TransPrice"),
                CurrencyCode = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "CurrencyCode"),
                Is3DSecure = DbPropertyHelper.BoolPropertyFromRow(reader, "Is3DSecure"),
                GuWId = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "GuWID"),
                AuthorizationCode = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "AuthorizationCode"),
                BookBackTransactionId = DbPropertyHelper.Int32PropertyFromRow(reader, "BookbackTransID"),
                BookingReferenceNumber = DbPropertyHelper.StringPropertyFromRow(reader, "BookingRefNo"),
                UserId = DbPropertyHelper.Int32PropertyFromRow(reader, "UserID"),
                PaymentGateway = (PaymentGatewayType)Enum.Parse(typeof(PaymentGatewayType), DbPropertyHelper.StringPropertyFromRow(reader, "PaymentgatewayTypeID")),
                BookingDate = DbPropertyHelper.DateTimeNullablePropertyFromRow(reader, "bookingDate"),
                AdyenMerchantAccout = DbPropertyHelper.StringPropertyFromRow(reader, "adyenMerchantAccout"),
            };

            return partialRefundPaymentData;
        }

        public BookedProductPaymentData MapBookedProductPaymentData(IDataReader reader)
        {
            var bookedProductPaymentData = new BookedProductPaymentData();

            bookedProductPaymentData.Amount = DbPropertyHelper.DecimalPropertyFromRow(reader, "amount");
            bookedProductPaymentData.GuWId = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "guwid");
            bookedProductPaymentData.AuthorizationCode = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "authorizationcode");
            bookedProductPaymentData.TransactionId = DbPropertyHelper.Int32PropertyFromRow(reader, "transid");
            bookedProductPaymentData.CaptureTransactionId = DbPropertyHelper.Int32PropertyFromRow(reader, "captransid");
            bookedProductPaymentData.CurrencyCode = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "currencycode");
            bookedProductPaymentData.Is3D = DbPropertyHelper.BoolPropertyFromRow(reader, "Is3D");
            bookedProductPaymentData.CardType = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "CardType");
            bookedProductPaymentData.PaymentGateway = (PaymentGatewayType)Enum.Parse(typeof(PaymentGatewayType), DbPropertyHelper.StringPropertyFromRow(reader, "PaymentGatewayTypeID"));
            bookedProductPaymentData.BookingReferenceNumber = DbPropertyHelper.StringPropertyFromRow(reader, "BookingReferencenumber");
            bookedProductPaymentData.BookingDate = DbPropertyHelper.DateTimeNullablePropertyFromRow(reader, "BookingDate");
            bookedProductPaymentData.AdyenMerchantAccount = DbPropertyHelper.StringPropertyFromRow(reader, "adyenMerchantAccout");
            return bookedProductPaymentData;
        }

        public List<BookedOptionMailData> MapBookedProductMailData(IDataReader reader)
        {
            var bookedOptoinMailDatas = new List<BookedOptionMailData>();
            while (reader.Read())
            {
                var bookedOptoinMailData = new BookedOptionMailData
                {
                    BookedOptionId = DbPropertyHelper.Int32PropertyFromRow(reader, "bookedoptionid"),
                    BookedOptionStatusId = DbPropertyHelper.Int32PropertyFromRow(reader, "bookedoptionstatusid"),
                    BookedOptionStatusName = DbPropertyHelper.StringPropertyFromRow(reader, "bookedoptionstatusname"),
                };
                bookedOptoinMailDatas.Add(bookedOptoinMailData);
            }

            return bookedOptoinMailDatas;
        }

        /// <summary>
        /// Map Receive Detail
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public ReceiveDetail MapReceiveDetail(IDataReader reader)
        {
            var receiveDetail = new ReceiveDetail();
            while (reader.Read())
            {
                receiveDetail.AffiliateId = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "affiliateID");
                receiveDetail.AffiliateName = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "AffiliateName");
                receiveDetail.BookedOptionId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "bookedOptionID");
                receiveDetail.BookingReferenceNumber = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "bookingReferenceNumber");
                receiveDetail.ChargeAmount = DbPropertyHelper.DecimalDefaultNullablePropertyFromRow(reader, "chargeAmount");
                receiveDetail.CompanyWebsite = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "CompanyWebsite");
                receiveDetail.CountryID = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "CountryID");
                receiveDetail.CurrencySymbol = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "currencySymbol");
                receiveDetail.FinancialBookingTransactionId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "financialbookingtransactionid");
                receiveDetail.LanguageCode = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "languageCode");
                receiveDetail.LeadPaxName = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "LeadPaxName");
                receiveDetail.PassengerFirstName = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "passengerFirstName");
                receiveDetail.PassengerLastName = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "passengerLastName");
                receiveDetail.PhoneNumber = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "PhoneNumber");
                receiveDetail.Remarks = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "Remarks");
                receiveDetail.SellCurrency = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "sellCurrency");
                receiveDetail.ServiceName = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "serviceName");
                receiveDetail.ServiceOptionName = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "serviceOptionName");
                receiveDetail.UserId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "userid");
                receiveDetail.VoucherEmail = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "voucherEmail");
                receiveDetail.PaymentGatwayType = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "PaymentGatwayType");
                receiveDetail.AdyenMerchantAccout = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "adyenMerchantAccout");
            }
            return receiveDetail;
        }


        #region Private methods

        /// <summary>
        /// Predicate for BookedProduct for provided activityId and BookedOptionID.
        /// </summary>
        /// <param name="selectedProduct"></param>
        /// <returns></returns>
        private bool FilterSelectedForActivityOption(SelectedProduct selectedProduct)
        {
            var allOption = selectedProduct.ProductOptions.FindAll(FilterSelectedOption);
            return selectedProduct.Id == _activityId && allOption[0].Id == _optionId;
        }

        private bool FilterSelectedOption(ProductOption option)
        {
            return option.IsSelected;
        }

        /// <summary>
        /// Predicate: Find selected product
        /// </summary>
        /// <param name="selectedProduct"></param>
        /// <returns></returns>
        private bool SelectedProductPredicate(SelectedProduct selectedProduct)
        {
            return selectedProduct.ProductId == _activityId || selectedProduct.Id == _activityId;
            //return false;
        }

        #endregion Private methods
    }
}