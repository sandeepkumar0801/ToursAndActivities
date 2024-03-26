using Isango.Entities;
using Isango.Persistence.Contract;
using Isango.Service.Contract;
using Logger.Contract;
using Util;

namespace Isango.Service
{
    public class ManageIdentityService : IManageIdentityService
    {
        private readonly IManageIdentityPersistence _manageIdentityPersistence;
        private readonly ILogger _log;

        public ManageIdentityService(IManageIdentityPersistence manageIdentityPersistence, ILogger log)
        {
            _manageIdentityPersistence = manageIdentityPersistence;
            _log = log;
        }

        /// <summary>
        /// This method subscribes the user for the news letters.
        /// </summary>
        /// <returns></returns>
        public async Task<string> SubscribeNewsLetterAsync(NewsLetterData criteria)
        {
            try
            {
                _manageIdentityPersistence.LogForConsentUser(criteria);
                return await Task.FromResult("subscribed");
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ManageIdentityService",
                    MethodName = "SubscribeNewsLetterAsync",
                    AffiliateId = criteria.AffiliateId,
                    Params = $"{SerializeDeSerializeHelper.Serialize(criteria)}"
                };
                _log.Error(isangoErrorEntity, ex);
                return await Task.FromResult("error");
            }
        }

        public async Task<bool> IsValidNewsletterSubscriberAsync(string emailId)
        {
            try
            {
                return await Task.FromResult(_manageIdentityPersistence.IsValidNewsletterSubscriber(emailId));
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ManageIdentityService",
                    MethodName = "IsValidNewsletterSubscriberAsync",

                    Params = $"{emailId}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        #region Private Methods

        /// <summary>
        /// This method is called from SubscribeNewsletter to add the consent user in log.
        /// </summary>
        private void LogForConsentUser(NewsLetterData criteria)
        {
            _manageIdentityPersistence.LogForConsentUser(criteria);
        }

        #endregion Private Methods
    }
}