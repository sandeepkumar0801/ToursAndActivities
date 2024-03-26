using Isango.Entities;

namespace Isango.Persistence.Contract
{
    public interface IManageIdentityPersistence
    {
        string SubscribeToNewsLetter(NewsLetterCriteria criteria);

        void LogForConsentUser(NewsLetterData criteria);

        bool IsValidNewsletterSubscriber(string emailId);
    }
}