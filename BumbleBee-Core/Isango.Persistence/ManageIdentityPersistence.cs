using Isango.Entities;
using Isango.Persistence.Contract;
using Logger.Contract;
using System;
using System.Data;
using Constant = Isango.Persistence.Constants.Constants;

namespace Isango.Persistence
{
    public class ManageIdentityPersistence : PersistenceBase, IManageIdentityPersistence
    {
        private readonly ILogger _log;
        public ManageIdentityPersistence(ILogger log)
        {
            _log = log;
        }
        /// <summary>
        /// This method will subscribe the sent user to the news letter.
        /// </summary>
        /// <returns> if successful then "Subscribed" </returns>
        public string SubscribeToNewsLetter(NewsLetterCriteria criteria)
        {
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.NewsLetterSubscription))
                {
                    IsangoDataBaseLive.AddInParameter(command, Constant.EmailId, DbType.String, criteria.EmailId);
                    IsangoDataBaseLive.AddInParameter(command, Constant.Subscribe, DbType.Boolean, 1);
                    IsangoDataBaseLive.AddOutParameter(command, Constant.Status, DbType.String, 20);
                    IsangoDataBaseLive.AddInParameter(command, Constant.LanguageCodeParam, DbType.String, criteria.LanguageCode);
                    if (!string.IsNullOrWhiteSpace(criteria.UserName))
                    {
                        IsangoDataBaseLive.AddInParameter(command, Constant.SubscriberName, DbType.String, criteria.UserName);
                    }
                    IsangoDataBaseLive.AddInParameter(command, Constant.AffiliateId, DbType.String, criteria.AffiliateId);
                    IsangoDataBaseLive.AddInParameter(command, Constant.Countryid, DbType.Int32, criteria.CountryId);

                    IsangoDataBaseLive.ExecuteNonQuery(command);

                    return command.Parameters[Constant.Status].Value.ToString();
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ManageIdentityPersistence",
                    MethodName = "SubscribeToNewsLetter",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// This method add the user in the subscription log which are subscribed for the news letter.
        /// </summary>
        /// <param name="criteria"></param>
        public void LogForConsentUser(NewsLetterData criteria)
        {
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.NewsLetterSubscriptionLog))
                {
                    IsangoDataBaseLive.AddInParameter(command, Constant.EmailId, DbType.String, criteria.EmailId);
                    IsangoDataBaseLive.AddInParameter(command, Constant.Issubscribed, DbType.Boolean, 1);
                    IsangoDataBaseLive.AddInParameter(command, Constant.LanguageCode, DbType.String, criteria.LanguageCode);
                    IsangoDataBaseLive.AddInParameter(command, Constant.AffiliateId, DbType.String, criteria.AffiliateId);
                    IsangoDataBaseLive.AddInParameter(command, Constant.Customerorigin, DbType.String, criteria.CustomerOrigin);

                    if (!string.IsNullOrWhiteSpace(criteria.Name))
                    {
                        IsangoDataBaseLive.AddInParameter(command, Constant.SubscriberName, DbType.String, criteria.Name);
                    }

                    IsangoDataBaseLive.AddInParameter(command, Constant.IsNBverified, DbType.Boolean, 1);
                    try
                    {
                        IsangoDataBaseLive.ExecuteNonQuery(command);
                    }
                    catch (Exception ex)
                    {
                        throw new ApplicationException(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ManageIdentityPersistence",
                    MethodName = "LogForConsentUser",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// check whether the subscriber is valid or not
        /// </summary>
        /// <param name="emailId"></param>
        /// <returns></returns>
        public bool IsValidNewsletterSubscriber(string emailId)
        {
            try
            {
                using (var command = IsangoDataBaseLive.GetStoredProcCommand(Constant.uspGetIsValidNewsletterSubscriber))
                {
                    IsangoDataBaseLive.AddInParameter(command, Constant.EmailId, DbType.String, emailId);

                    return (Convert.ToByte(IsangoDataBaseLive.ExecuteScalar(command)) == 1);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ManageIdentityPersistence",
                    MethodName = "IsValidNewsletterSubscriber",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }
    }
}