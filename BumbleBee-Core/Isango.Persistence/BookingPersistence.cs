using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Booking;
using Isango.Entities.Booking.BookingDetailAPI;
using Isango.Entities.Booking.ConfirmBooking;
using Isango.Entities.Booking.PartialRefund;
using Isango.Entities.Booking.RequestModels;
using Isango.Entities.Enums;
using Isango.Entities.Mailer.Voucher;
using Isango.Entities.Payment;
using Isango.Persistence.Contract;
using Isango.Persistence.Data;
using Logger.Contract;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Util;
using Constant = Isango.Persistence.Constants.Constants;

namespace Isango.Persistence
{
    public class BookingPersistence : PersistenceBase, IBookingPersistence
    {
        private readonly ILogger _log;

        public BookingPersistence(ILogger log)
        {
            _log = log;
        }

        /// <summary>
        /// Store Request & Response for each wirecard transaction.
        /// Credit card holder details are stripped from the request XML before storing it in the database.
        /// </summary>
        /// <param name="xmlCriteria"></param>
        public bool InsertWirecardXml(WireCardXmlCriteria xmlCriteria)
        {
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.InsertWirecardXmlSp))
                {
                    // Prepare parameter collection
                    IsangoDataBaseLive.AddInParameter(command, Constant.CartId, DbType.String, xmlCriteria.JobId);
                    IsangoDataBaseLive.AddInParameter(command, Constant.ParamTransId, DbType.Int32, xmlCriteria.TransId);
                    IsangoDataBaseLive.AddInParameter(command, Constant.ParamDate, DbType.DateTime, xmlCriteria.TransDate);
                    IsangoDataBaseLive.AddInParameter(command, Constant.Status, DbType.String, xmlCriteria.Status);
                    IsangoDataBaseLive.AddInParameter(command, Constant.ParamRequestXml, DbType.String,
                        xmlCriteria.Request);
                    IsangoDataBaseLive.AddInParameter(command, Constant.ParamResponseXml, DbType.String,
                        xmlCriteria.Response);
                    IsangoDataBaseLive.AddInParameter(command, Constant.ParamRequestType, DbType.String,
                        xmlCriteria.RequestType);
                    IsangoDataBaseLive.AddInParameter(command, Constant.ParamTransGUWID, DbType.String,
                        xmlCriteria.TransGuWId);

                    //Execute the Stored procedure to update the booking details.
                    IsangoDataBaseLive.ExecuteNonQuery(command);
                    return true;
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingPersistence",
                    MethodName = "InsertWirecardXml",
                    Params = $"{SerializeDeSerializeHelper.Serialize(xmlCriteria)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        #region Get Booking data

        /// <summary>
        /// Get Mail Data
        /// </summary>
        /// <param name="bookingRefNo"></param>
        /// <returns></returns>
        public Booking GetMailDataForReceive(string bookingRefNo)
        {
            Booking booking = null;
            bookingRefNo = bookingRefNo?.Trim();
            try
            {
                using (var dbCommand = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetBookingDetailForMailBodySp))
                {
                    IsangoDataBaseLive.AddInParameter(dbCommand, Constant.ParamBookingRefnoForMail, DbType.String,
                        bookingRefNo);

                    using (IDataReader reader = IsangoDataBaseLive.ExecuteReader(dbCommand))
                    {
                        var bookingData = new BookingData();
                        bookingData.LoadBookingData(reader, ref booking);
                        bookingData.LoadProductData(reader, ref booking);
                        bookingData.LoadAgeDetailData(reader, ref booking);

                        //One more data set gets returned for Ventrata. So fetch the ventrata products and update the pdf details for per passenger
                        var ventrataSelectedProduct = booking.SelectedProducts.FindAll(thisProd => thisProd.APIType.Equals(APIType.Ventrata));
                        if (ventrataSelectedProduct != null && ventrataSelectedProduct.Count > 0)
                        {
                            bookingData.LoadPerPaxPdfData(reader, ref booking);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingPersistence",
                    MethodName = "GetMailDataForReceive",
                    Params = $"{bookingRefNo}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }

            return booking;
        }

        /// <summary>
        /// Get booking details
        /// </summary>
        /// <param name="referenceNumber"></param>
        /// <returns></returns>
        public List<BookingDetail> GetBookingDetail(string referenceNumber)
        {
            var bookingDetails = new List<BookingDetail>();
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetBookingDetailByStatusB2BSp))
                {
                    // Prepare parameter collection
                    IsangoDataBaseLive.AddInParameter(command, Constant.BookingRefNoParam, DbType.String, referenceNumber);
                    using (IDataReader reader = IsangoDataBaseLive.ExecuteReader(command))
                    {
                        while (reader.Read())
                        {
                            var bookingData = new BookingData();
                            bookingDetails = bookingData.LoadBookingDetail(reader);
                        }
                    }

                    return bookingDetails;
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingPersistence",
                    MethodName = "GetBookingDetail",
                    Params = $"{referenceNumber}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Get booking data
        /// </summary>
        /// <param name="bookingRef"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public BookingDetailBase GetBookingDataForMail(string bookingRef, bool isSupplier, int value = 3, int? bookedOptionId = null)
        {
            var type = 0;
            BookingDetailBase bookingData;
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetBookingDetailForVoucherSp))
                {
                    command.CommandTimeout = 300;
                    IsangoDataBaseLive.AddInParameter(command, Constant.ParamBookingRefnoForMail, DbType.String,
                        bookingRef);
                    IsangoDataBaseLive.AddInParameter(command, Constant.ParamSource, DbType.Int16, value);

                    IsangoDataBaseLive.AddInParameter(command, Constant.ParamForCustomer, DbType.Boolean, 1);
                    //if (isSupplier)
                    //IsangoDataBaseLive.AddInParameter(command, Constant.ParamForCustomer, DbType.Boolean, 0);

                    if (bookedOptionId != null)
                        IsangoDataBaseLive.AddInParameter(command, Constant.ParamForBookedOptionId, DbType.Int32, bookedOptionId);

                    using (var reader = IsangoDataBaseLive.ExecuteReader(command))
                    {
                        while (reader.Read())
                        {
                            type = reader.GetInt32(0);
                        }

                        switch (type)
                        {
                            case 3:
                                bookingData = new BookingDataOthers(reader);
                                break;

                            default:
                                throw new NotImplementedException();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingPersistence",
                    MethodName = "GetBookingDataForMail",
                    Params = $"{bookingRef},{isSupplier}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return bookingData;
        }

        /// <summary>
        /// Get booking travel date
        /// </summary>
        /// <param name="bookingRefno"></param>
        /// <returns></returns>
        public string GetBookingTravelDate(string bookingRefno)
        {
            var returnValue = string.Empty;
            try
            {
                var dbCommand = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetBookingTravelDateSp);
                dbCommand.CommandType = CommandType.StoredProcedure;

                IsangoDataBaseLive.AddInParameter(dbCommand, Constant.ParamBookingRefnoForMail, DbType.String,
                    bookingRefno);
                using (IDataReader reader = IsangoDataBaseLive.ExecuteReader(dbCommand))
                {
                    while (reader.Read())
                    {
                        var bookingData = new BookingData();
                        returnValue = bookingData.GetTravelDate(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingPersistence",
                    MethodName = "GetBookingTravelDate",
                    Params = $"{bookingRefno}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return returnValue;
        }

        /// <summary>
        /// Get booking travel date
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public List<ReservationDBDetails> GetReservationData(string bookingRefNo)
        {
            var ReservationDetails = new List<ReservationDBDetails>();
            try
            {
                var dbCommand = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetTokenAvailability);
                dbCommand.CommandType = CommandType.StoredProcedure;

                IsangoDataBaseLive.AddInParameter(dbCommand, Constant.BookingReferenceID, DbType.String,
                    bookingRefNo);
                using (IDataReader reader = IsangoDataBaseLive.ExecuteReader(dbCommand))
                {
                    while (reader.Read())
                    {
                        var detail = new ReservationDBDetails()
                        {
                            Token = DbPropertyHelper.StringPropertyFromRow(reader, "TokenID"),
                            AvailabilityRefNo = DbPropertyHelper.StringPropertyFromRow(reader, "AvailabilityRefID")
                        };
                        ReservationDetails.Add(detail);
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingPersistence",
                    MethodName = "GetReservationData",
                    Params = $"{bookingRefNo}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return ReservationDetails;
        }

        /// <summary>
        /// Match booking by email reference
        /// </summary>
        /// <param name="email"></param>
        /// <param name="bookingRef"></param>
        /// <returns></returns>
        public bool MatchBookingByEmailRef(string email, string bookingRef)
        {
            bool isValid;
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.ValidateBookingByEmailSp))
                {
                    // Prepare parameter collection
                    IsangoDataBaseLive.AddInParameter(command, Constant.ParamBookingRefnoForMail, DbType.String,
                        bookingRef);
                    IsangoDataBaseLive.AddInParameter(command, Constant.ParamUserEmailId, DbType.String, email);
                    IsangoDataBaseLive.AddOutParameter(command, Constant.ParamIsValid, DbType.Boolean, 0);
                    IsangoDataBaseLive.ExecuteNonQuery(command);
                    isValid = bool.Parse(command.Parameters[Constant.ParamIsValid].Value.ToString());
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingPersistence",
                    MethodName = "MatchBookingByEmailRef",
                    Params = $"{email},{bookingRef}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return isValid;
        }
        
        public bool SaveFailedBookingInDb(Guid affiliateId, string customerEmail, string selectedProductText,
            string hashKey,
            int saveInCartType)
        {
            var sendEmail = false;
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand("dbo.usp_ins_CartAbundant"))
                {
                    IsangoDataBaseLive.AddInParameter(command, "@Affiliateid", DbType.Guid, affiliateId);
                    IsangoDataBaseLive.AddInParameter(command, "@CustomerEmail", DbType.String, customerEmail);
                    IsangoDataBaseLive.AddInParameter(command, "@AbundantLogText", DbType.String, selectedProductText);
                    IsangoDataBaseLive.AddInParameter(command, "@AbundantHashKey", DbType.String, hashKey);
                    IsangoDataBaseLive.AddInParameter(command, "@AbundantType", DbType.Int32, saveInCartType);

                    IsangoDataBaseLive.AddOutParameter(command, "@SendEmail", DbType.Boolean, 1);
                    IsangoDataBaseLive.ExecuteNonQuery(command);
                    var bitFromDb = IsangoDataBaseLive.GetParameterValue(command, "@SendEmail") == DBNull.Value
                        ? 0
                        : Convert.ToInt32(IsangoDataBaseLive.GetParameterValue(command, "@SendEmail"));
                    if (bitFromDb == 1)
                    {
                        sendEmail = true;
                    }

                    return sendEmail;
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingPersistence",
                    MethodName = "SaveFailedBookingInDb",
                    Params = $"{affiliateId},{customerEmail},{selectedProductText}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Get the booking data for the given booking reference number
        /// </summary>
        /// <param name="bookingReferenceNumber"></param>
        public ConfirmBookingDetail GetBookingData(string bookingReferenceNumber)
        {
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetBookingDetailForConfirmationPage))
                {
                    IsangoDataBaseLive.AddInParameter(command, Constant.BookingReferenceNumber, DbType.String,
                        bookingReferenceNumber);
                    using (var reader = IsangoDataBaseLive.ExecuteReader(command))
                    {
                        var bookingData = new BookingData();
                        var confirmBookingDetail = bookingData.MapBookingDetail(reader);
                        //setting same BookingReferenceNumber in the object, as we are not getting it back from database
                        confirmBookingDetail.BookingReferenceNumber = bookingReferenceNumber;
                        return confirmBookingDetail;
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingPersistence",
                    MethodName = "GetBookingData",
                    Params = $"{bookingReferenceNumber}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Get the Receive Detail for the given amendmentId
        /// </summary>
        /// <param name="id"></param>
        public ReceiveDetail GetReceiveDetail(int id)
        {
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetReceiveDetailSp))
                {
                    IsangoDataBaseLive.AddInParameter(command, Constant.FinancialBookingTransactionId, DbType.Int32, id);
                    using (var reader = IsangoDataBaseLive.ExecuteReader(command))
                    {
                        var bookingData = new BookingData();
                        var confirmBookingDetail = bookingData.MapReceiveDetail(reader);
                        return confirmBookingDetail;
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingPersistence",
                    MethodName = "GetReceiveDetail",
                    Params = $"{id}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        #endregion Get Booking data

        /// <summary>
        /// Operation used to validate duplicate booking
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public Tuple<bool, string> CheckDuplicateBooking(DuplicateBookingCriteria criteria)
        {
            var isBookingAlreadyExist = false;
            var bookingRefNo = string.Empty;
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.DuplicateBookingSp))
                {
                    IsangoDataBaseLive.AddInParameter(command, Constant.ParamSmcPassengerId, DbType.Int32,
                        criteria.SmcPasswordId);
                    IsangoDataBaseLive.AddInParameter(command, Constant.ParamTravelDate, DbType.DateTime,
                        criteria.TravelDate);
                    IsangoDataBaseLive.AddInParameter(command, Constant.ParamServiceOptionIdForBooking, DbType.Int32,
                        criteria.ServiceOptionId);
                    IsangoDataBaseLive.AddInParameter(command, Constant.ParamAdultCount, DbType.Int32, criteria.AdultCount);
                    IsangoDataBaseLive.AddInParameter(command, Constant.ParamLeadPaxName, DbType.String, criteria.LeadPaxName);
                    IsangoDataBaseLive.AddInParameter(command, Constant.ParamAffiliateIdForBooking, DbType.String, criteria.AffiliateId);
                    IsangoDataBaseLive.AddInParameter(command, Constant.ParamVoucherEmail, DbType.String, criteria.UserEmailId);
                    IsangoDataBaseLive.AddInParameter(command, Constant.AvailabilityReferenceIds, DbType.String, criteria.AvailabilityReferenceIds);

                    using (var reader = IsangoDataBaseLive.ExecuteReader(command))
                    {
                        while (reader.Read())
                        {
                            var bookingData = new BookingData();
                            bookingRefNo = bookingData.GetBookingRefNo(reader).Trim();
                            if (!string.IsNullOrEmpty(bookingRefNo))
                            {
                                isBookingAlreadyExist = true;
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingPersistence",
                    MethodName = "CheckDuplicateBooking",
                    Params = $"{SerializeDeSerializeHelper.Serialize(criteria)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return new Tuple<bool, string>(isBookingAlreadyExist, bookingRefNo);
        }

        /// <summary>
        /// Create isango and HB products booking without wire card step
        /// </summary>
        /// <param name="booking"></param>
        /// <param name="isangoBooking"></param>
        /// <param name="preauthPayment"></param>
        /// <param name="purchasePayment"></param>
        /// <param name="is3D"></param>
        /// <param name="isAlternativePayment"></param>
        /// <returns></returns>
        public bool CreateIsangoBooking(IsangoBookingData isangoBooking, bool isAlternativePayment)
        {
            // Step 1: Create Booking in Isango Database
            var isAlternativePaymentParam = Convert.ToInt32(isAlternativePayment);
            var dbjson = string.Empty;
            try
            {
                dbjson = SerializeDeSerializeHelper.Serialize(isangoBooking);
                //Prepare command
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.CreateBooking))
                {
                    // Prepare parameter collection
                    command.CommandTimeout = 300;
                    IsangoDataBaseLive.AddInParameter(command, Constant.Bookingjsonparam, DbType.String, dbjson
                        );
                    IsangoDataBaseLive.AddInParameter(command, Constant.IsAlternativePaymentParam, DbType.Boolean,
                        isAlternativePaymentParam);
                    IsangoDataBaseLive.AddOutParameter(command, Constant.StatusParam, DbType.Boolean, 1);

                    IsangoDataBaseLive.ExecuteNonQuery(command);
                    var returnStatus = Convert.ToBoolean(IsangoDataBaseLive.GetParameterValue(command, Constant.Status));
                    return returnStatus;
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingPersistence",
                    MethodName = "CreateIsangoBooking",
                    Params = $"{SerializeDeSerializeHelper.Serialize(isangoBooking)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            // End Step 1
        }

        public void CreateReversal(int bookingId, float transPrice, string currencyCode, string ipAddress, string Guid,
            string authCode, int transId)
        {
            try
            {
                // Prepare command
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.CreateReversalTransactionB2BSp))
                {
                    // Prepare parameter collection
                    IsangoDataBaseLive.AddInParameter(command, Constant.ParamBookingID, DbType.Int32, bookingId);
                    IsangoDataBaseLive.AddInParameter(command, Constant.TransPrice, DbType.Double, transPrice);
                    IsangoDataBaseLive.AddInParameter(command, Constant.Currecycode, DbType.String, currencyCode);
                    IsangoDataBaseLive.AddInParameter(command, Constant.ClientIpAddress, DbType.String, ipAddress);
                    IsangoDataBaseLive.AddInParameter(command, Constant.ParamGuid, DbType.String, Guid);
                    IsangoDataBaseLive.AddInParameter(command, Constant.AuthorizationCode, DbType.String, authCode);
                    IsangoDataBaseLive.AddInParameter(command, Constant.ParamITransId, DbType.Int32, transId);

                    //Execute the Stored procedure to update the temp booking detail.
                    IsangoDataBaseLive.ExecuteNonQuery(command);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingPersistence",
                    MethodName = "CreateReversal",
                    Params = $"{Guid},{transId}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        #region Confirm Booking API

        //change output and input as per sp
        public BookedProductPaymentData ConfirmBookingUpdateStatusAndGetPaymentData(string userId, int bookedOptionId)
        {
            var bookedProductPaymentData = new BookedProductPaymentData();
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.BookingConfirmation))
                {
                    command.CommandTimeout = 300;
                    IsangoDataBaseLive.AddInParameter(command, Constant.UserID, DbType.String, userId);
                    IsangoDataBaseLive.AddInParameter(command, Constant.BookedOptionId, DbType.Int32, bookedOptionId);
                    IsangoDataBaseLive.AddOutParameter(command, Constant.CompletionStatus, DbType.Boolean, 1);
                    using (var reader = IsangoDataBaseLive.ExecuteReader(command))
                    {
                        while (reader.Read())
                        {
                            var bookingData = new BookingData();
                            bookedProductPaymentData = bookingData.MapBookedProductPaymentData(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingPersistence",
                    MethodName = "ConfirmBookingUpdateStatusAndGetPaymentData",
                    Params = $"{userId},{bookedOptionId}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return bookedProductPaymentData;
        }

        public bool UpdatePaymentStatus(int transactionId, string guWId, string AuthorizationCode, string gateWayID = "")
        {
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.UpdateTransaction))
                {
                    command.CommandTimeout = 300;

                    IsangoDataBaseLive.AddInParameter(command, Constant.TransId, DbType.Int32, transactionId);
                    IsangoDataBaseLive.AddInParameter(command, Constant.GuWID, DbType.String, guWId);
                    IsangoDataBaseLive.AddInParameter(command, Constant.AuthorizationCode, DbType.String, AuthorizationCode);
                    IsangoDataBaseLive.AddInParameter(command, Constant.GateWayID, DbType.String, gateWayID);
                    IsangoDataBaseLive.ExecuteNonQuery(command);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingPersistence",
                    MethodName = "UpdatePaymentStatus",
                    Params = $"{transactionId},{guWId}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return true;
        }

        public List<BookedOptionMailData> CheckToSendmailToCustomer(int bookedOptionId)
        {
            var bookedOptionMailData = new List<BookedOptionMailData>();
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.CheckSendMailToCustomer))
                {
                    command.CommandTimeout = 300;
                    IsangoDataBaseLive.AddInParameter(command, Constant.BookedOptionId, DbType.Int32, bookedOptionId);
                    using (var reader = IsangoDataBaseLive.ExecuteReader(command))
                    {
                        var bookingData = new BookingData();
                        bookedOptionMailData = bookingData.MapBookedProductMailData(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingPersistence",
                    MethodName = "CheckToSendmailToCustomer",
                    Params = $"{bookedOptionId}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return bookedOptionMailData;
        }

        #endregion Confirm Booking API

        #region Amend Booking

        public Booking UpdateIsangoBookingAgainstRefund(int amendmentId, string remarks, string actionBy)
        {
            Booking amendedBooking;
            try
            {
                //Prepare command
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.InsPartialRefundSp))
                {
                    IsangoDataBaseLive.AddInParameter(command, Constant.ParamFinancialBookingTransactionId, DbType.Int32,
                        amendmentId);
                    IsangoDataBaseLive.AddInParameter(command, Constant.ParamRemarks, DbType.String, remarks);
                    IsangoDataBaseLive.AddInParameter(command, Constant.ParamActionBy, DbType.String, actionBy);

                    using (var reader = IsangoDataBaseLive.ExecuteReader(command))
                    {
                        var bookingData = new BookingData();
                        amendedBooking = bookingData.GetAmendedBooking(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingPersistence",
                    MethodName = "UpdateIsangoBookingAgainstRefund",
                    Params = $"{amendmentId}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }

            return amendedBooking;
        }

        public int UpdateIsangoBooking(int amendmentId, bool is3D, ref Payment purchasePayment, string cardType = "", int paymentGatewayTypeId = 4)
        {
            try
            {
                //Prepare command
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.InsReceiveDetailSp))
                {
                    IsangoDataBaseLive.AddInParameter(command, Constant.ParamFinancialBookingTransactionId, DbType.Int32,
                        amendmentId);
                    IsangoDataBaseLive.AddInParameter(command, Constant.Is3DParam, DbType.Boolean, is3D);
                    IsangoDataBaseLive.AddInParameter(command, Constant.CardTypeParam, DbType.String, cardType);
                    IsangoDataBaseLive.AddInParameter(command, Constant.PaymentGatewayTypeIdParam, DbType.Int32, paymentGatewayTypeId);
                    IsangoDataBaseLive.AddOutParameter(command, Constant.IPurchaseTransId, DbType.Int32, int.MaxValue);
                    IsangoDataBaseLive.ExecuteNonQuery(command);
                    var purchaseTransId = IsangoDataBaseLive.GetParameterValue(command, Constant.IPurchaseTransId);
                    var transactionId = purchaseTransId == DBNull.Value ? 0 : Convert.ToInt32(purchaseTransId);

                    return transactionId;
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingPersistence",
                    MethodName = "UpdateIsangoBooking",
                    Params = $"{amendmentId},{SerializeDeSerializeHelper.Serialize(purchasePayment)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public void UpdateReceiveBookingTransaction(Payment purchasePayment)
        {
            try
            {
                //Prepare command
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.UpdateBookingTransaction))
                {
                    IsangoDataBaseLive.AddInParameter(command, Constant.CaptureTransID, DbType.Int32, purchasePayment.TransactionId);
                    IsangoDataBaseLive.AddInParameter(command, Constant.CaptureGuWID, DbType.String, purchasePayment.PaymentGatewayReferenceId);
                    IsangoDataBaseLive.AddInParameter(command, Constant.CaptureAuthorizationCode, DbType.String, purchasePayment.AuthorizationCode);
                    //IsangoDataBaseLive.AddInParameter(command, Constant.PreAuthTransID, DbType.String, "");
                    IsangoDataBaseLive.AddInParameter(command, Constant.PreAuthGuWID, DbType.String, purchasePayment.Guwid);
                    //IsangoDataBaseLive.AddInParameter(command, Constant.PreAuthAuthorizationCode, DbType.String, "");
                    IsangoDataBaseLive.ExecuteNonQuery(command);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingPersistence",
                    MethodName = "UpdateReceiveBookingTransaction",
                    Params = $"{SerializeDeSerializeHelper.Serialize(purchasePayment)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public PaymentBookingData GetPartialRefundData(int amendmentId)
        {
            var recPaymentData = new PaymentBookingData();
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetPartialRefundDetailSp))
                {
                    IsangoDataBaseLive.AddInParameter(command, Constant.FinancialBookingTransactionId, DbType.Int32,
                        amendmentId);
                    using (IDataReader reader = IsangoDataBaseLive.ExecuteReader(command))
                    {
                        while (reader.Read())
                        {
                            var bookingData = new BookingData();
                            recPaymentData = bookingData.GetPaymentBookingData(reader, false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingPersistence",
                    MethodName = "GetPartialRefundData",
                    Params = $"{amendmentId}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return recPaymentData;
        }

        ///<summary>
        ///Persistence method for fetching data of the booking for which the Payment receivable
        ///process is initiated. Data fetched will be shown on the page dedicated for receiving post amendment amount(View Name-- Receive Payment).
        /// </summary>
        public PaymentBookingData GetPaymentRelatedBookingData(int amendmentId)
        {
            PaymentBookingData recPaymentData;
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetReceiveDetailSp))
                {
                    IsangoDataBaseLive.AddInParameter(command, Constant.ParamFinancialBookingTransactionId, DbType.Int32,
                        amendmentId);
                    using (IDataReader reader = IsangoDataBaseLive.ExecuteReader(command))
                    {
                        var bookingData = new BookingData();
                        recPaymentData = bookingData.GetPaymentBookingData(reader, true);
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingPersistence",
                    MethodName = "GetPaymentRelatedBookingData",
                    Params = $"{amendmentId}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return recPaymentData;
        }

        /// <summary>
        /// call to insert partial booking details
        /// </summary>
        /// <param name="partialBooking"></param>
        /// <returns></returns>
        public int InsertPartialBooking(PartialBooking partialBooking)
        {
            var partialBookingItemId = 0;
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.InsertPartialBookingSp))
                {
                    // Prepare parameter collection
                    IsangoDataBaseLive.AddInParameter(command, Constant.PartialBookingItemId, DbType.Int32,
                        partialBooking.Id);
                    IsangoDataBaseLive.AddInParameter(command, Constant.AvailabilityReferenceId, DbType.String,
                        partialBooking.AvailabilityReferenceId);
                    IsangoDataBaseLive.AddInParameter(command, Constant.ItemStatus, DbType.Int16,
                        partialBooking.ItemStatus);
                    IsangoDataBaseLive.AddInParameter(command, Constant.SelectedProductId, DbType.Int32,
                        partialBooking.SelectedProductId);
                    IsangoDataBaseLive.AddInParameter(command, Constant.BookingReferenceID, DbType.String,
                        partialBooking.BookingReferenceNumber);

                    using (var reader = IsangoDataBaseLive.ExecuteReader(command))
                    {
                        while (reader.Read())
                        {
                            partialBookingItemId = DbPropertyHelper.Int32PropertyFromRow(reader, "Id");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingPersistence",
                    MethodName = "InsertPartialBooking",
                    Params = $"{SerializeDeSerializeHelper.Serialize(partialBooking)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return partialBookingItemId;
        }

        #endregion Amend Booking

        #region Partial Refund API

        public PartialRefundPaymentData InsertPartialRefundAndGetPaymentInfo(int amendmentId, string remarks, string actionBy)
        {
            var partialRefundPaymentData = new PartialRefundPaymentData();
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.InsertPartialRefund))
                {
                    command.CommandTimeout = 300;
                    IsangoDataBaseLive.AddInParameter(command, Constant.FinancialBookingTransactionId, DbType.Int32, amendmentId);
                    IsangoDataBaseLive.AddInParameter(command, Constant.Remarks, DbType.String, remarks);
                    IsangoDataBaseLive.AddInParameter(command, Constant.ActionBy, DbType.String, actionBy);
                    using (var reader = IsangoDataBaseLive.ExecuteReader(command))
                    {
                        while (reader.Read())
                        {
                            var bookingData = new BookingData();
                            partialRefundPaymentData = bookingData.MapPartialRefundData(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingPersistence",
                    MethodName = "InsertPartialRefundAndGetPaymentInfo",
                    Params = $"{amendmentId}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return partialRefundPaymentData;
        }

        #endregion Partial Refund API


        public void InsertGeneratePaymentLink(GeneratePaymentLinkResponse generatePaymentLinkRequest)
        {
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.InsertGeneratePaymentLinkSp))
                {
                    // Prepare parameter collection
                    IsangoDataBaseLive.AddInParameter(command, Constant.GeneratePaymentLinkCurrency, DbType.String,
                    generatePaymentLinkRequest.Currency);
                    IsangoDataBaseLive.AddInParameter(command, Constant.GeneratePaymentLinkValue, DbType.String,
                        generatePaymentLinkRequest.Value);
                    IsangoDataBaseLive.AddInParameter(command, Constant.GeneratePaymentLinkCountryCode, DbType.String,
                        generatePaymentLinkRequest.CountryCode);
                    IsangoDataBaseLive.AddInParameter(command, Constant.GeneratePaymentLinkDescription, DbType.String,
                        generatePaymentLinkRequest.Description);
                    IsangoDataBaseLive.AddInParameter(command, Constant.GeneratePaymentLinkExpiresAt, DbType.String,
                       generatePaymentLinkRequest.ExpiresAt);
                    IsangoDataBaseLive.AddInParameter(command, Constant.GeneratePaymentLinkId, DbType.String,
                    generatePaymentLinkRequest.Id);
                    IsangoDataBaseLive.AddInParameter(command, Constant.GeneratePaymentLinkMerchantAccount, DbType.String,
                    generatePaymentLinkRequest.MerchantAccount);
                    IsangoDataBaseLive.AddInParameter(command, Constant.GeneratePaymentLinkReference, DbType.String,
                    generatePaymentLinkRequest.Reference);
                    IsangoDataBaseLive.AddInParameter(command, Constant.GeneratePaymentLinkShopperLocale, DbType.String,
                    generatePaymentLinkRequest.ShopperLocale);
                    IsangoDataBaseLive.AddInParameter(command, Constant.GeneratePaymentLinkShopperReference, DbType.String,
                    generatePaymentLinkRequest.ShopperReference);
                    IsangoDataBaseLive.AddInParameter(command, Constant.GeneratePaymentLinkUrl, DbType.String,
                    generatePaymentLinkRequest.Url);
                    IsangoDataBaseLive.AddInParameter(command, Constant.CustomerEmail, DbType.String,
                    generatePaymentLinkRequest.CustomerEmail);

                    IsangoDataBaseLive.AddInParameter(command, Constant.CustomerLanguage, DbType.String,
                    generatePaymentLinkRequest.CustomerLanguage);

                    IsangoDataBaseLive.AddInParameter(command, Constant.TemporaryRefNo, DbType.String,
                    generatePaymentLinkRequest.TemporaryRefNo);

                    IsangoDataBaseLive.ExecuteNonQuery(command);

                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingPersistence",
                    MethodName = "InsertGeneratePaymentLink",
                    Params = $"{SerializeDeSerializeHelper.Serialize(generatePaymentLinkRequest)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }
        
        public string GenerateBookingRefNumber(string affiliateID, string currencyCode)
        {
            try
            {
                //Prepare command
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.GenerateBookingRefNo))
                {
                    // Prepare parameter collection
                    command.CommandTimeout = 300;
                    IsangoDataBaseLive.AddInParameter(command, Constant.AffiliateId, DbType.String, affiliateID);
                    IsangoDataBaseLive.AddInParameter(command, Constant.Currecycode, DbType.String, currencyCode);
                    IsangoDataBaseLive.AddOutParameter(command, Constant.BookingRefNoParam, DbType.String, 20);

                    IsangoDataBaseLive.ExecuteNonQuery(command);
                    var result = Convert.ToString(IsangoDataBaseLive.GetParameterValue(command, Constant.BookingRefNoParam));
                    return result;
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingPersistence",
                    MethodName = "GenerateBookingRefNumber",
                    Params = $"{affiliateID}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Get booking details from db by booking reference number , user id and status id
        /// Passing statusId= null as we want to have all the products i.e. confirmed or on-request.
        /// </summary>
        /// <param name="referenceNumber"></param>
        /// <param name="userId"></param>
        /// <param name="statusId"></param>
        /// <returns></returns>
        public List<BookingDetail> GetBookingDetails(string referenceNumber, string userId, string statusId = null)
        {
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetBookingDetailByStatusB2BSp))
                {
                    // Prepare parameter collection
                    IsangoDataBaseLive.AddInParameter(command, Constant.BookingRefNoParam, DbType.String, referenceNumber);
                    IsangoDataBaseLive.AddInParameter(command, Constant.UserId, DbType.String, userId);
                    IsangoDataBaseLive.AddInParameter(command, Constant.StatusID, DbType.String, statusId);
                    var bookingDetails = new List<BookingDetail>();
                    using (IDataReader reader = IsangoDataBaseLive.ExecuteReader(command))
                    {
                        var bookingData = new BookingData();
                        bookingDetails = bookingData.LoadBookingDetail(reader);
                    }
                    return bookingDetails;
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingPersistence",
                    MethodName = "GetBookingDetails",
                    Params = $"{referenceNumber},{userId}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Update qrCode/VoucherLink in DB
        /// </summary>
        /// <param name="bookingRefNo"></param>
        /// <param name="serviceOptionId"></param>
        /// <param name="availabilityRefId"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public int UpdateAPISupplierBookingQRCode(string bookingRefNo, int serviceOptionId, 
            string availabilityRefId, string value,
            string qrCodeType="",bool isQRCodePerPax=false,string multiQRcode="")
        {
            var bookedOptionId = 0;
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.UpdateAPISupplierBookingQRCode))
                {
                    // Prepare parameter collection
                    IsangoDataBaseLive.AddInParameter(command, Constant.BookingRefNoParam, DbType.String, bookingRefNo);
                    IsangoDataBaseLive.AddInParameter(command, Constant.ParamServiceOptionId, DbType.Int32, serviceOptionId);
                    IsangoDataBaseLive.AddInParameter(command, Constant.AvailabilityReferenceId, DbType.String, availabilityRefId ?? string.Empty);
                    IsangoDataBaseLive.AddInParameter(command, Constant.QRCodeValue, DbType.String, value);
                    IsangoDataBaseLive.AddInParameter(command, Constant.QRCodeType, DbType.String, qrCodeType);
                    IsangoDataBaseLive.AddInParameter(command, Constant.IsQRCodePerPax, DbType.Boolean, isQRCodePerPax);
                    IsangoDataBaseLive.AddInParameter(command, Constant.MultiQRcode, DbType.String, multiQRcode);

                    IsangoDataBaseLive.AddOutParameter(command, Constant.BookedOptionId, DbType.Int32, 20);

                    IsangoDataBaseLive.ExecuteNonQuery(command);
                    bookedOptionId = Convert.ToInt32(IsangoDataBaseLive.GetParameterValue(command, Constant.BookedOptionId));
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingPersistence",
                    MethodName = "UpdateAPISupplierBookingQRCode",
                    Params = $"{bookingRefNo},{serviceOptionId}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return bookedOptionId;
        }

        public string BookingReferenceNumberfromDB(string affiliateID, string CurrencyISO, string randomString)
        {
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetBookingReferenceNumber))
                {
                    // Prepare parameter collection
                    IsangoDataBaseLive.AddInParameter(command, Constant.AffiliateId.ToLower(), DbType.String, affiliateID);
                    IsangoDataBaseLive.AddInParameter(command, Constant.Currecycode, DbType.String, CurrencyISO);
                    IsangoDataBaseLive.AddInParameter(command, Constant.RandomRefNoParam, DbType.String, randomString);
                    string RefNo = string.Empty;
                    RefNo = IsangoDataBaseLive.ExecuteReader(command).ToString();
                    using (IDataReader reader = IsangoDataBaseLive.ExecuteReader(command))
                    {
                        while (reader.Read())
                        {
                            RefNo = reader["status"].ToString();
                        }
                    }
                    return RefNo;
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingPersistence",
                    MethodName = "BookingReferenceNumberfromDB",
                    Params = $"{affiliateID},{randomString}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public void LogBookingFailureInDB(Booking booking, string bookingRefNo, int? serviceID, string tokenID, string apiRefID, string custEmail, string custContact, int? ApiType, int? optionID, string optionName, string avlbltyRefID, string ErrorLevel)
        {
            try
            {
                var errorMsg = "Error Occured";
                //if (!booking.Errors.Any(x => x.IsLoggedInDB))
                //{
                if(booking?.Errors?.Count > 0)
                {
                    errorMsg = booking?.Errors?.FirstOrDefault()?.Message;
                }
                    using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.LogBookingFailureSp))
                    {
                        IsangoDataBaseLive.AddInParameter(command, Constant.BookingReferenceID, DbType.String,
                            bookingRefNo);
                        IsangoDataBaseLive.AddInParameter(command, Constant.ParamServiceId, DbType.Int32,
                            serviceID);
                        IsangoDataBaseLive.AddInParameter(command, Constant.ParamTokenID, DbType.String,
                            tokenID);
                        IsangoDataBaseLive.AddInParameter(command, Constant.ParamAPIReferenceNumber, DbType.String, apiRefID);
                        IsangoDataBaseLive.AddInParameter(command, Constant.CustomerEmail, DbType.String, custEmail);
                        IsangoDataBaseLive.AddInParameter(command, Constant.CustomerContact, DbType.String, custContact);
                        IsangoDataBaseLive.AddInParameter(command, Constant.ParamAPICancellationStatus, DbType.Boolean, false);
                        IsangoDataBaseLive.AddInParameter(command, Constant.ParamApiType, DbType.Int32, ApiType ?? 0);
                        IsangoDataBaseLive.AddInParameter(command, Constant.ParamOptionID, DbType.Int32, optionID);
                        IsangoDataBaseLive.AddInParameter(command, Constant.ParamOptionName, DbType.String, optionName);
                        IsangoDataBaseLive.AddInParameter(command, Constant.ParamAvailabilityReferenceNumber, DbType.String, avlbltyRefID);
                        IsangoDataBaseLive.AddInParameter(command, Constant.ParamErrorMsg, DbType.String, errorMsg);
                        IsangoDataBaseLive.AddInParameter(command, Constant.ParamErrorLvl, DbType.String, ErrorLevel);
                        IsangoDataBaseLive.ExecuteNonQuery(command);
                    }
                //}
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingPersistence",
                    MethodName = "CheckDuplicateBooking",
                    Params = $"{SerializeDeSerializeHelper.Serialize(booking)}"
                };
                _log.Error(isangoErrorEntity, ex);
                //throw;
            }
        }
        public List<CVData> LoadCVPointData()
        {
            var CvDataList = new List<CVData>();
            using (var dbCmd = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetCvPoints))
            {
                dbCmd.CommandType = CommandType.StoredProcedure;

                using (var reader = IsangoDataBaseLive.ExecuteReader(dbCmd))
                {
                    while (reader.Read())
                    {
                        var masterData = new CVData();
                        masterData.CVPoints = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "CVPoints");
                        masterData.FromDate = DbPropertyHelper.DateTimePropertyFromRow(reader, "FromDate");
                        masterData.ToDate = DbPropertyHelper.DateTimePropertyFromRow(reader, "ToDate");

                        CvDataList.Add(masterData);
                    }
                }
            }
            return CvDataList;
        }


    }
}