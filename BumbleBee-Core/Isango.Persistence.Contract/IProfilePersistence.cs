using Isango.Entities;
using Isango.Entities.MyIsango;
using System;
using System.Collections.Generic;

namespace Isango.Persistence.Contract
{
    public interface IProfilePersistence
    {
        ISangoUser FetchUserInfo(int userId);

        List<MyBookingSummary> FetchUserBookingDetails(string emailId, string affiliateId, bool isAgent);

        List<MyBookingSummary> FetchAgentBookingDetails(int userId, string affiliateId, bool onlyCancelledBooking, string agentName);

        bool UpdateUserInfo(UserInfo userInfo);

        bool SaveUserEmailPreferences(int userId, string affiliateId, string answerString);

        List<MyUserEmailPreferences> FetchUserEmailPreferences(int userId, string affiliateId, string languageCode);

        DateTime GetUserCreationDate(string emailID);
    }
}