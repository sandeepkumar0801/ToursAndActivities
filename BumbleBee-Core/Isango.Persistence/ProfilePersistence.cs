using Isango.Entities;
using Isango.Entities.MyIsango;
using Isango.Persistence.Contract;
using Isango.Persistence.Data;
using Logger.Contract;
using System;
using System.Collections.Generic;
using System.Data;
using Constant = Isango.Persistence.Constants.Constants;

namespace Isango.Persistence
{
    public class ProfilePersistence : PersistenceBase, IProfilePersistence
    {
        private readonly ILogger _log;
        public ProfilePersistence(ILogger log)
        {
            _log = log;
        }
        /// <summary>
        /// Fetch user information
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ISangoUser FetchUserInfo(int userId)
        {
            try
            {
                if (userId > 0)
                {
                    using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetPassengerDetail4MyIsangoSp))
                    {
                        ISangoUser userInfo;
                        IsangoDataBaseLive.AddInParameter(command, Constant.ParamPassengerId, DbType.Int32, userId);
                        using (var reader = IsangoDataBaseLive.ExecuteReader(command))
                        {
                            var profileData = new ProfileData();
                            userInfo = profileData.GetUserInfo(reader);
                        }

                        return userInfo;
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ProfilePersistence",
                    MethodName = "FetchUserInfo",
                    Params = $"{userId}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return null;
        }

        /// <summary>
        /// Fetch user booking details
        /// </summary>
        /// <param name="emailId"></param>
        /// <param name="affiliateId"></param>
        /// <param name="isAgent"></param>
        /// <returns></returns>
        public List<MyBookingSummary> FetchUserBookingDetails(string emailId, string affiliateId, bool isAgent)
        {
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetMyIsangoSp))
                {
                    List<MyBookingSummary> userBookingDetailsList;
                    IsangoDataBaseLive.AddInParameter(command, Constant.ParamEmailId, DbType.String, emailId);
                    if (affiliateId != null)
                        IsangoDataBaseLive.AddInParameter(command, Constant.ParamAffiliateId, DbType.String, affiliateId);
                    IsangoDataBaseLive.AddInParameter(command, Constant.ParamIsAgent, DbType.Boolean, isAgent);

                    using (var reader = IsangoDataBaseLive.ExecuteReader(command))
                    {
                        userBookingDetailsList = GetBookingDetailsList(reader, false);
                    }

                    return userBookingDetailsList;
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ProfilePersistence",
                    MethodName = "FetchUserBookingDetails",
                    Params = $"{emailId}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }

        }

        /// <summary>
        /// Fetch agent booking details
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="affiliateId"></param>
        /// <param name="onlyCancelledBooking"></param>
        /// <param name="agentName"></param>
        /// <returns></returns>
        public List<MyBookingSummary> FetchAgentBookingDetails(int userId, string affiliateId, bool onlyCancelledBooking, string agentName)
        {
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetBookingDetail4MyIsangoSp))
                {
                    List<MyBookingSummary> userBookingDetailsList;
                    IsangoDataBaseLive.AddInParameter(command, Constant.ParamPassengerId, DbType.Int32, userId);
                    if (affiliateId != null)
                        IsangoDataBaseLive.AddInParameter(command, Constant.ParamAffiliateId, DbType.String, affiliateId);
                    IsangoDataBaseLive.AddInParameter(command, Constant.ParamIsCancelledBooking, DbType.Boolean, onlyCancelledBooking);
                    IsangoDataBaseLive.AddInParameter(command, Constant.ParamAgentName, DbType.String, agentName);

                    using (var reader = IsangoDataBaseLive.ExecuteReader(command))
                    {
                        userBookingDetailsList = GetBookingDetailsList(reader, true);
                    }

                    return userBookingDetailsList;
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ProfilePersistence",
                    MethodName = "FetchAgentBookingDetails",
                    Params = $"{userId}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Update user information
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public bool UpdateUserInfo(UserInfo userInfo)
        {
            try
            {
                if (userInfo.UserId > 0)
                {
                    using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.UpdatePassengerDetail4MyIsangoSp))
                    {
                        IsangoDataBaseLive.AddInParameter(command, Constant.ParamPassengerId, DbType.Int32, userInfo.UserId);
                        IsangoDataBaseLive.AddInParameter(command, Constant.ParamPassengerFirstName, DbType.String, userInfo.FirstName);
                        IsangoDataBaseLive.AddInParameter(command, Constant.ParamPassengerLastName, DbType.String, userInfo.LastName);
                        IsangoDataBaseLive.AddInParameter(command, Constant.ParamLoginId, DbType.String, userInfo.EmailId);
                        IsangoDataBaseLive.AddInParameter(command, Constant.ParamLoginPassword, DbType.String, userInfo.Password);
                        IsangoDataBaseLive.AddInParameter(command, Constant.ParamAddressTelephoneNumber, DbType.String, userInfo.ContactNumber);

                        var isUpdated = IsangoDataBaseLive.ExecuteNonQuery(command);

                        return isUpdated != -1;
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ProfilePersistence",
                    MethodName = "UpdateUserInfo",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return false;
        }

        /// <summary>
        /// Save user email preferences
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="affiliateId"></param>
        /// <param name="answerString"></param>
        /// <returns></returns>
        public bool SaveUserEmailPreferences(int userId, string affiliateId, string answerString)
        {
            try
            {
                if (userId > 0)
                {
                    using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.SaveEmailPreferncesForUserSp))
                    {
                        IsangoDataBaseLive.AddInParameter(command, Constant.ParamUserId, DbType.Int32, userId);
                        IsangoDataBaseLive.AddInParameter(command, Constant.ParamAnswerIds, DbType.String, answerString);
                        IsangoDataBaseLive.AddInParameter(command, Constant.AffiliateId, DbType.String, affiliateId);

                        var noOfRowsAffected = IsangoDataBaseLive.ExecuteNonQuery(command);

                        return noOfRowsAffected != -1;
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ProfilePersistence",
                    MethodName = "SaveUserEmailPreferences",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return false;
        }

        /// <summary>
        /// Fetch user email preferences
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="affiliateId"></param>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public List<MyUserEmailPreferences> FetchUserEmailPreferences(int userId, string affiliateId,
            string languageCode)
        {
            try
            {
                if (userId > 0)
                {
                    using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetPreference4MyIsangoSp))
                    {
                        List<MyUserEmailPreferences> userEmailPreferenceList = null;
                        IsangoDataBaseLive.AddInParameter(command, Constant.ParamAffiliateId, DbType.String, affiliateId);
                        IsangoDataBaseLive.AddInParameter(command, Constant.ParamPassengerId, DbType.Int32, userId);
                        IsangoDataBaseLive.AddInParameter(command, Constant.LanguageCodeForLoadRegionMetaData, DbType.String, languageCode);

                        using (var reader = IsangoDataBaseLive.ExecuteReader(command))
                        {
                            var profileData = new ProfileData();
                            profileData.LoadUserEmailPrefQuestion(reader, ref userEmailPreferenceList);
                            profileData.LoadUserPrefAnswer(reader, ref userEmailPreferenceList);
                        }

                        return userEmailPreferenceList;
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ProfilePersistence",
                    MethodName = "FetchUserEmailPreferences",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }

            return null;
        }

        /// <summary>
        /// Proc To get user creation date using emailId
        /// </summary>
        /// <param name="emailID"></param>
        /// <returns></returns>
        public DateTime GetUserCreationDate(string emailID)
        {
            var userCreationDate = DateTime.Now;
            try
            {
                if (!string.IsNullOrEmpty(emailID))
                {
                    using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.GetUserCreationDateSp))
                    {
                        IsangoDataBaseLive.AddInParameter(command, "@UserEmailID", DbType.String, emailID);

                        using (var reader = IsangoDataBaseLive.ExecuteReader(command))
                        {
                            while (reader.Read())
                            {
                                userCreationDate = reader["SMCPASSENGERCREATIONDATE"] != DBNull.Value ? Convert.ToDateTime(reader["SMCPASSENGERCREATIONDATE"]) : userCreationDate;
                            }
                        }

                        return userCreationDate;
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ProfilePersistence",
                    MethodName = "GetUserCreationDate",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return userCreationDate;
        }

        #region Private Methods

        /// <summary>
        /// Get list of booking details
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="isAgent"></param>
        /// <returns></returns>
        private List<MyBookingSummary> GetBookingDetailsList(IDataReader reader, bool isAgent)
        {
            List<MyBookingSummary> userBookingDetailsList = null;
            try
            {
               
                var profileData = new ProfileData();

                profileData.LoadUserBookingDetails(reader, ref userBookingDetailsList, isAgent);
                profileData.LoadBookedProductDetails(reader, ref userBookingDetailsList, isAgent);
                profileData.LoadBookedPaxProductPrice(reader, ref userBookingDetailsList, isAgent);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ProfilePersistence",
                    MethodName = "GetBookingDetailsList",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
            return userBookingDetailsList;
        }

        #endregion Private Methods
    }
}