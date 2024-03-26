using Isango.Entities;
using Isango.Entities.Booking;
using Isango.Entities.Booking.ConfirmBooking;
using Isango.Entities.Cancellation;
using Isango.Persistence.Contract;
using Isango.Persistence.Data;
using Logger.Contract;
using System;
using System.Collections.Generic;
using System.Data;
using Util;
using Constant = Isango.Persistence.Constants.Constants;

namespace Isango.Persistence
{
    public class CancellationPersistence : PersistenceBase, ICancellationPersistence
    {
        private readonly ILogger _log;
        public CancellationPersistence(ILogger log)
        {
            _log = log;
        }
        #region public methods

        /// <summary>
        /// Get cancellation policy details from db
        /// </summary>
        /// <param name="bookingRefNo"></param>
        /// <param name="bookedOptionId"></param>
        /// <param name="currencyIsoCode"></param>
        /// <param name="spId"></param>
        /// <returns></returns>
        public Isango.Entities.Cancellation.CancellationPolicyDetail GetCancellationPolicyDetail(string bookingRefNo, int bookedOptionId,
            string currencyIsoCode, int spId)
        {
            var cancellationPolicyAmountDetail = new Isango.Entities.Cancellation.CancellationPolicyDetail();
            try
            {
                 cancellationPolicyAmountDetail =
                    GetCancellationPolicyAmountDetails(bookingRefNo, bookedOptionId, currencyIsoCode, spId);
                
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CancellationPersistence",
                    MethodName = "GetCancellationPolicyDetail",
                  
                    Params = $"{bookingRefNo}, {bookedOptionId}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return cancellationPolicyAmountDetail;
        }

        /// <summary>
        /// Get supplier cancellation data from db by booking reference number
        /// </summary>
        /// <param name="bookingRefNo"></param>
        /// <returns></returns>
        public SupplierCancellationData GetSupplierCancellationData(string bookingRefNo)
        {
            try
            {
                var suppliersCancellationData = GetAllSupplierCancellationData(bookingRefNo);
                return suppliersCancellationData;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CancellationPersistence",
                    MethodName = "GetSupplierCancellationData",
                    Params = $"{bookingRefNo}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }
        public List<SupplierCancellationData> GetSupplierCancellationDataList(string bookingRefNo)
        {
            try
            {
                var suppliersCancellationData = GetAllSupplierCancellationDataList(bookingRefNo);
                return suppliersCancellationData;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CancellationPersistence",
                    MethodName = "GetSupplierCancellationDataList",
                    Params = $"{bookingRefNo}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }
        /// <summary>
        /// Get user id and permission value to modify refund amount
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public UserCancellationPermission GetUserPermissionForCancellation(string userName)
        {
            try
            {
                var userPermitData = GetUserPermissionForCancellationData(userName);
                return userPermitData;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CancellationPersistence",
                    MethodName = "GetUserPermissionForCancellation",
                    Params = $"{userName}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// cancel booking for single product in db
        /// </summary>
        /// <param name="cancellation"></param>
        /// <param name="bookingRefNo"></param>
        /// <param name="cancelledById"></param>
        /// <param name="cancelledByUser"></param>
        /// <returns></returns>
        public ConfirmCancellationDetail CreateCancelBooking(Cancellation cancellation, string bookingRefNo,
            string cancelledById, int cancelledByUser)
        {
            try
            {
                var confirmCancellationDetail =
                    GetConfirmedCancellationDetail(cancellation, bookingRefNo, cancelledById, cancelledByUser);
                return confirmCancellationDetail;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CancellationPersistence",
                    MethodName = "CreateCancelBooking",
                    Params = $"{bookingRefNo}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Get cancellation status of all steps from db
        /// </summary>
        /// <param name="bookedOptionId"></param>
        /// <returns></returns>
        public CancellationStatus GetCancellationStatus(int bookedOptionId)
        {
            try
            {
                var cancellationStatus = GetAllCancellationStatus(bookedOptionId);
                return cancellationStatus;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CancellationPersistence",
                    MethodName = "GetCancellationStatus",
                    Params = $"{bookedOptionId}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// insert or update cancellation status in db
        /// </summary>
        /// <param name="cancellationStatus"></param>
        public void InsertOrUpdateCancellationStatus(CancellationStatus cancellationStatus)
        {
            try
            {
                using (var dbCmd = IsangoDataBaseLive.GetStoredProcCommand(Constant.InsertUpdateCancellationStatus))
                {
                    IsangoDataBaseLive.AddInParameter(dbCmd, Constant.BookedOptionId, DbType.Int32,
                        cancellationStatus.BookedOptionId);
                    IsangoDataBaseLive.AddInParameter(dbCmd, Constant.IsangoCancelStatus, DbType.Int16,
                        cancellationStatus.IsangoCancelStatus);
                    IsangoDataBaseLive.AddInParameter(dbCmd, Constant.PaymentRefundStatus, DbType.Int16,
                        cancellationStatus.PaymentRefundStatus);
                    IsangoDataBaseLive.AddInParameter(dbCmd, Constant.SupplierCancelStatus, DbType.Int16,
                        cancellationStatus.SupplierCancelStatus);
                    IsangoDataBaseLive.ExecuteNonQuery(dbCmd);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CancellationPersistence",
                    MethodName = "InsertOrUpdateCancellationStatus",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        #endregion public methods

        #region private methods

        /// <summary>
        /// Private method to get cancellation policy amount details used by 'GetCancellationPolicyDetails' method
        /// </summary>
        /// <param name="bookingRefNo"></param>
        /// <param name="bookedOptionId"></param>
        /// <param name="currencyIsoCode"></param>
        /// <param name="spId"></param>
        /// <returns></returns>
        private Isango.Entities.Cancellation.CancellationPolicyDetail GetCancellationPolicyAmountDetails(string bookingRefNo,
            int bookedOptionId, string currencyIsoCode, int spId = 0)
        {
            Isango.Entities.Cancellation.CancellationPolicyDetail cancellationPolicyAmountDetail = null;
            try
            {
                using (var dbCmd = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetCancellationPolicyAmount))
                {
                    IsangoDataBaseLive.AddInParameter(dbCmd, Constant.BookingRefNo, DbType.String, bookingRefNo);
                    IsangoDataBaseLive.AddInParameter(dbCmd, Constant.BookedOptionId_ri, DbType.Int32, bookedOptionId);
                    //IsangoDataBaseLive.AddInParameter(dbCmd, Constant.CurrencyISOCode, DbType.String, currencyIsoCode);
                    //IsangoDataBaseLive.AddInParameter(dbCmd, Constant.SPID, DbType.Int32, spId);

                    using (var reader = IsangoDataBaseLive.ExecuteReader(dbCmd))
                    {
                        while (reader.Read())
                        {
                            var cancelBookingData = new CancellationData();
                            cancellationPolicyAmountDetail = cancelBookingData.MapCancellationPolicyData(reader);
                        }
                    }
                }
            }
             catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CancellationPersistence",
                    MethodName = "GetCancellationPolicyAmountDetails",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return cancellationPolicyAmountDetail;
        }

        /// <summary>
        /// Private method to get supplier cancellation data used by 'GetSupplierCancellationData' method
        /// </summary>
        /// <param name="bookingRefNo"></param>
        /// <returns></returns>
        private SupplierCancellationData GetAllSupplierCancellationData(string bookingRefNo)
        {
            SupplierCancellationData suppliersCancellationData = null;
            try
            {
                using (var dbCmd =
      IsangoDataBaseLive.GetStoredProcCommand(Constant.GetCancellableSuppliersCancellationData))
                {
                    IsangoDataBaseLive.AddInParameter(dbCmd, Constant.BookingRefNoParam, DbType.String, bookingRefNo);

                    using (var reader = IsangoDataBaseLive.ExecuteReader(dbCmd))
                    {
                        while (reader.Read())
                        {
                            var suppliersData = new CancellationData();
                            suppliersCancellationData = suppliersData.MapSupplierCancellationData(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CancellationPersistence",
                    MethodName = "GetAllSupplierCancellationData",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return suppliersCancellationData;
        }


        /// <summary>
        /// Private method to get supplier cancellation data used by 'GetSupplierCancellationData' method
        /// </summary>
        /// <param name="bookingRefNo"></param>
        /// <returns></returns>
        private List<SupplierCancellationData> GetAllSupplierCancellationDataList(string bookingRefNo)
        {
            var suppliersCancellationDataList = new List<SupplierCancellationData>();
            try
            {
                using (var dbCmd =
      IsangoDataBaseLive.GetStoredProcCommand(Constant.GetCancellableSuppliersCancellationData))
                {
                    IsangoDataBaseLive.AddInParameter(dbCmd, Constant.BookingRefNoParam, DbType.String, bookingRefNo);

                    using (var reader = IsangoDataBaseLive.ExecuteReader(dbCmd))
                    {
                        while (reader.Read())
                        {
                            var suppliersData = new CancellationData();
                            suppliersCancellationDataList.Add(suppliersData.MapSupplierCancellationData(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CancellationPersistence",
                    MethodName = "suppliersCancellationDataList",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return suppliersCancellationDataList;
        }

        /// <summary>
        /// private method to get user id and permission value used by 'GetUserPermissionForCancellation' method
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        private UserCancellationPermission GetUserPermissionForCancellationData(string userName)
        {
            UserCancellationPermission userPermitData = null;
            try
            {
                using (var dbCmd = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetUserPermission))
                {
                    IsangoDataBaseLive.AddInParameter(dbCmd, Constant.UserName, DbType.String, userName);

                    using (var reader = IsangoDataBaseLive.ExecuteReader(dbCmd))
                    {
                        while (reader.Read())
                        {
                            var userData = new CancellationData();
                            userPermitData = userData.MapUserPermissionData(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CancellationPersistence",
                    MethodName = "GetUserPermissionForCancellationData",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return userPermitData;
        }

        /// <summary>
        /// Private method to get confirmed cancellation detail used by 'CreateCancelBooking' method
        /// </summary>
        /// <param name="cancellation"></param>
        /// <param name="bookingRefNo"></param>
        /// <param name="cancelledById"></param>
        /// <param name="cancelledByUser"></param>
        /// <returns></returns>
        private ConfirmCancellationDetail GetConfirmedCancellationDetail(Cancellation cancellation, string bookingRefNo,
            string cancelledById, int cancelledByUser)
        {
            ConfirmCancellationDetail confirmedDetail;
            try
            {
                var cancelJsonString = SerializeDeSerializeHelper.Serialize(cancellation.CancellationParameters);
                using (var dbCmd = IsangoDataBaseLive.GetStoredProcCommand(Constant.CreateCancelBooking))
                {
                    IsangoDataBaseLive.AddInParameter(dbCmd, Constant.rjson, DbType.String, cancelJsonString);
                    IsangoDataBaseLive.AddInParameter(dbCmd, Constant.BookingRefNo, DbType.String, bookingRefNo);
                    IsangoDataBaseLive.AddInParameter(dbCmd, Constant.CancelledByUser, DbType.String, cancelledByUser);
                    IsangoDataBaseLive.AddInParameter(dbCmd, Constant.CancelledById, DbType.String, cancelledById);
                    using (var reader = IsangoDataBaseLive.ExecuteReader(dbCmd))
                    {
                        var confirmedCancellationData = new CancellationData();
                        confirmedDetail = confirmedCancellationData.MapConfirmedCancellationData(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CancellationPersistence",
                    MethodName = "GetConfirmedCancellationDetail",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return confirmedDetail;
        }

        /// <summary>
        /// Check whether to send cancel mail to customer
        /// </summary>
        /// <param name="bookedOptionId"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Public method to update cancellation detail (Bookback call and Reversal Call Guwid)
        /// </summary>
        /// <param name="transId"></param>
        /// <param name="paymentGatewayResponse"></param>
        /// <returns></returns>
        public void UpdateCancelBooking(int transId, PaymentGatewayResponse paymentGatewayResponse)
        {
            try
            {
                using (var dbCmd = IsangoDataBaseLive.GetStoredProcCommand(Constant.UpdateTransaction))
                {
                    IsangoDataBaseLive.AddInParameter(dbCmd, Constant.TransID, DbType.Int32, transId);
                    IsangoDataBaseLive.AddInParameter(dbCmd, Constant.GuWID, DbType.String, paymentGatewayResponse.Guwid);
                    IsangoDataBaseLive.AddInParameter(dbCmd, Constant.AuthorizationCode, DbType.String, paymentGatewayResponse.AuthorizationCode);
                    //IsangoDataBaseLive.ExecuteReader(dbCmd);
                    using (var reader = IsangoDataBaseLive.ExecuteReader(dbCmd))
                    {
                        //Logic to close the reader automatically after execution. Other wise throwing error
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CancellationPersistence",
                    MethodName = "UpdateCancelBooking",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Private methods to get cancellation status used by 'GetCancellationStatus' method
        /// </summary>
        /// <param name="bookedOptionId"></param>
        /// <returns></returns>
        private CancellationStatus GetAllCancellationStatus(int bookedOptionId)
        {
            CancellationStatus cancellationStatus;
            try
            {
                using (var dbCmd = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetAllCancellationStatus))
                {
                    IsangoDataBaseLive.AddInParameter(dbCmd, Constant.BookedOptionId, DbType.Int32, bookedOptionId);
                    using (var reader = IsangoDataBaseLive.ExecuteReader(dbCmd))
                    {
                        var allCancellationStatus = new CancellationData();
                        cancellationStatus = allCancellationStatus.MapAllCancellationStatus(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CancellationPersistence",
                    MethodName = "GetAllCancellationStatus",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }

            return cancellationStatus;
        }

        #endregion private methods
    }
}