using Isango.Persistence.Contract;
using Isango.Entities.Activities;
using Isango.Entities.Enums;
using Isango.Entities.Master;
using Isango.Entities.Region;
using Isango.Entities.Review;
using Isango.Persistence.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml;
using Util;
using Constant = Isango.Persistence.Constants.Constants;

namespace Isango.Persistence
{
    public class AdyenPersistence : PersistenceBase, IAdyenPersistence
    {
        public Tuple<string, bool> GetAdyenWebhookRepsonse(string bookingRefNo, int transFlowID)
        {
            try
            {
                var eventCode = string.Empty;
                var status = false;
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetAdyenWebhookfromDB))
                {
                    // Prepare parameter collection
                    IsangoDataBaseLive.AddInParameter(command, Constant.BookingRefNoParam.ToLower(), DbType.String, bookingRefNo);
                    IsangoDataBaseLive.AddInParameter(command, Constant.TransFlowID.ToLower(), DbType.Int32, transFlowID);
                    
                    using (IDataReader reader = IsangoDataBaseLive.ExecuteReader(command))
                    {
                        while (reader.Read())
                        {
                            eventCode = reader["Returnstatus"].ToString();
                            status = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "isSuccess");
                        }
                    }
                }
                return new Tuple<string, bool>(eventCode, status);
            }
            catch (Exception ex)
            {
                return new Tuple<string, bool>("Fail", false);
            }
        }

        public Tuple<string,string,string,string,string> UpdatePaymentLinkData(string id,string pspReference)
        {
            try
            {
                var customerEmail = String.Empty;
                var customerLang = String.Empty;
                var currency = String.Empty;
                var value = String.Empty;
                var temporaryRefNo= String.Empty;
                //Prepare command
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.UpdateGeneratePaymentLink))
                {
                    // Prepare parameter collection
                    command.CommandTimeout = 300;
                    IsangoDataBaseLive.AddInParameter(command, Constant.Id, DbType.String, id);
                    IsangoDataBaseLive.AddInParameter(command, Constant.PSPReference, DbType.String, pspReference);
                    using (IDataReader reader = IsangoDataBaseLive.ExecuteReader(command))
                    {
                        while (reader.Read())
                        {
                            customerEmail = Convert.ToString(reader["CustomerEmail"]);
                            customerLang = Convert.ToString(reader["CustomerLanguage"]);
                            currency = Convert.ToString(reader["currency"]);
                            value = Convert.ToString(reader["value"]);
                            temporaryRefNo = Convert.ToString(reader["TemporaryRefNo"]);
                        }
                    }
                    return new Tuple<string, string,string,string,string>
                        (customerEmail, customerLang, currency, value, temporaryRefNo);
                }
            }
            catch (Exception ex)
            {
                return new Tuple<string, string, string, string,string>("", "","","","");
                throw;
            }
        }
        public void UpdateWebhookStatusinDB(int flowName, string bookingReference, string status, string pspReference, string reason, bool isCustomerMailSent, bool isSupplierMailSent, bool? isSuccess)
        {
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.UpdateAdyenWebhookinDB))
                {
                    // Prepare parameter collection
                    IsangoDataBaseLive.AddInParameter(command, Constant.TransFlowID, DbType.Int32, flowName);
                    IsangoDataBaseLive.AddInParameter(command, Constant.BookingRefNoParam.ToLower(), DbType.String, bookingReference);
                    IsangoDataBaseLive.AddInParameter(command, Constant.PSPReference, DbType.String, pspReference);
                    IsangoDataBaseLive.AddInParameter(command, Constant.ReturnStatus, DbType.String, status);
                    IsangoDataBaseLive.AddInParameter(command, Constant.Reason, DbType.String, reason);
                    IsangoDataBaseLive.AddInParameter(command, Constant.IsCustomerMailSent, DbType.Boolean, isCustomerMailSent);
                    IsangoDataBaseLive.AddInParameter(command, Constant.IsSupplierMailSent, DbType.Boolean, isSupplierMailSent);
                    IsangoDataBaseLive.AddInParameter(command, Constant.IsSuccess, DbType.Boolean, isSuccess);
                    using (IDataReader reader = IsangoDataBaseLive.ExecuteReader(command))
                    {
                        //do nothing - let the data save and the reader will be closed automatically
                    }
                }
            }
            catch (Exception ex)
            {
                //ignore - do not stop the flow  need to return response to webhook within 10 secs
            }
        }
    }
}
