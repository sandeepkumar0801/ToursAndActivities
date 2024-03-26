using Isango.Entities;
using Isango.Entities.Booking;
using Isango.Entities.RedeamV12;
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
    public class SupplierBookingPersistence : PersistenceBase, ISupplierBookingPersistence
    {
        private readonly ILogger _log;
        public SupplierBookingPersistence(ILogger log)
        {
            _log = log;
        }
        /// <summary>
        /// Get SightSeeing mapping
        /// </summary>
        /// <param name="optionId"></param>
        /// <returns></returns>
        public List<CitySightseeingMapping> GetSightseeingMapping(int optionId)
        {
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetSightseeingItalyMappingSp))
                {
                    IsangoDataBaseLive.AddInParameter(command, Constant.ParamServiceOptionId, DbType.Int32, optionId);
                    using (var reader = IsangoDataBaseLive.ExecuteReader(command))
                    {
                        var supplierBookingData = new SupplierBookingData();
                        return supplierBookingData.GetCitySightseeingMappings(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "SupplierBookingPersistence",
                    MethodName = "GetSightseeingMapping",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public void LogPurchaseXML(LogPurchaseXmlCriteria criteria)
        {
            try
            {
                //Prepare command
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.BookingTrackerSp))
                {
                    // Prepare parameter collection
                    IsangoDataBaseLive.AddInParameter(command, Constant.ParamApiRefNo, DbType.String, criteria.ApiRefNumber);
                    IsangoDataBaseLive.AddInParameter(command, Constant.SofortStatus, DbType.String, criteria.Status);
                    IsangoDataBaseLive.AddInParameter(command, Constant.ParRequestXml, DbType.String, criteria.RequestXml);
                    IsangoDataBaseLive.AddInParameter(command, Constant.ParResponseXml, DbType.String, criteria.ResponseXml);
                    IsangoDataBaseLive.AddInParameter(command, Constant.ParamBType, DbType.String, criteria.Bookingtype);
                    IsangoDataBaseLive.AddInParameter(command, Constant.ParamSupplierId, DbType.Int32, criteria.SupplierId);
                    IsangoDataBaseLive.AddInParameter(command, Constant.ParIsangoBookingId, DbType.Int32, criteria.BookingId);
                    IsangoDataBaseLive.AddInParameter(command, Constant.ParamApiTypeId, DbType.Int32, Convert.ToInt32(criteria.APIType));
                    IsangoDataBaseLive.AddInParameter(command, Constant.ParamBookingRefNo, DbType.String, criteria.BookingReferenceNumber);
                    IsangoDataBaseLive.ExecuteNonQuery(command);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "SupplierBookingPersistence",
                    MethodName = "LogPurchaseXML",
                    Params = $"{SerializeDeSerializeHelper.Serialize(criteria)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Insert Reservation Request Details
        /// </summary>
        /// <param name="token"></param>
        /// <param name="availabilityRefID"></param>
        /// <returns></returns>
        public void InsertReserveRequest(string token, string availabilityRefID)
        {
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.InsertTokenAvailability))
                {
                    IsangoDataBaseLive.AddInParameter(command, Constant.ParamFortokeID, DbType.String, token);
                    IsangoDataBaseLive.AddInParameter(command, Constant.ParamForAvailabilityRefID, DbType.String, availabilityRefID);
                    IsangoDataBaseLive.ExecuteNonQuery(command);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingPersistence",
                    MethodName = "InsertReserveRequest",
                    Params = $"{token},{availabilityRefID}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Insert Reservation Request Details
        /// </summary>
        /// <param name="token"></param>
        /// <param name="availabilityRefID"></param>
        /// <returns></returns>
        public void UpdateReserveRequest(string token, string availabilityRefID, string bookingRefNo)
        {
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.UpdateTokenAvailability))
                {
                    IsangoDataBaseLive.AddInParameter(command, Constant.ParamFortokeID, DbType.String, token);
                    IsangoDataBaseLive.AddInParameter(command, Constant.ParamForAvailabilityRefID, DbType.String, availabilityRefID);
                    IsangoDataBaseLive.AddInParameter(command, Constant.BookingReferenceID, DbType.String, bookingRefNo);
                    IsangoDataBaseLive.ExecuteNonQuery(command);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "BookingPersistence",
                    MethodName = "UpdateReserveRequest",
                    Params = $"{token},{availabilityRefID},{bookingRefNo}"
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
                if (booking?.Errors?.Count > 0)
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

        public void SaveMoulinRougeTicketBytes(Entities.MoulinRouge.MoulinRougeSelectedProduct mulinRougeSelectedProduct)
        {
            int i = 0;
            foreach (var confirmedTicketBytes in mulinRougeSelectedProduct?.ConfirmedTicketBytes)
            {
                var eTicketGUID = mulinRougeSelectedProduct.ETicketGuiDs[i];
                var optionId = mulinRougeSelectedProduct?.ProductOptions?.FirstOrDefault()?.Id ?? 0;
                try
                {
                    //Prepare command
                    using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.InsertMRVoucher))
                    {
                        // Prepare parameter collection
                        IsangoDataBaseLive.AddInParameter(command, "@bookingrefno", DbType.String, mulinRougeSelectedProduct.IsangoBookingReferenceNumber);
                        IsangoDataBaseLive.AddInParameter(command, "@serviceoptionid", DbType.Int32, optionId);
                        IsangoDataBaseLive.AddInParameter(command, "@APIorderID", DbType.String, mulinRougeSelectedProduct.OrderId);
                        IsangoDataBaseLive.AddInParameter(command, "@APITicketGUWID", DbType.String, eTicketGUID);
                        IsangoDataBaseLive.AddInParameter(command, "@APIVoucherByte", DbType.Binary, confirmedTicketBytes);
                        IsangoDataBaseLive.ExecuteNonQuery(command);

                    }
                }
                catch (Exception ex)
                {
                    var isangoErrorEntity = new IsangoErrorEntity
                    {
                        ClassName = "SupplierBookingPersistence",
                        MethodName = "SaveMoulinRougeTicketBytes",
                        Params = $"{SerializeDeSerializeHelper.Serialize(mulinRougeSelectedProduct)}"
                    };
                    _log.Error(isangoErrorEntity, ex);
                    // throw;
                }
                i++;
            }
        }
        public List<SupplierData> GetRedeamV12SupplierData()
        {
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetRedeamV12SupplierData))
                {

                    using (var reader = IsangoDataBaseLive.ExecuteReader(command))
                    {
                        var supplierBookingData = new SupplierBookingData();
                        return supplierBookingData.GetRedeamV12SupplierData(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "SupplierBookingPersistence",
                    MethodName = "GetRedeamV12SupplierData",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }
    }
}