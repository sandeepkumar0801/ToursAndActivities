using Isango.Entities;
using Isango.Entities.MyIsango;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Isango.Service.Contract
{
    public interface IProfileService
    {
        Task<ISangoUser> FetchUserInfoAsync(int userId);

        Task<List<MyBookingSummary>> FetchUserBookingDetailsAsync(string emailId, string affiliateId, bool isAgent);

        Task<List<MyBookingSummarySitecoreViewModel>> FetchUserBookingDetailsSitecoreAsync(string emailId, string affiliateId, bool isAgent);

        Task<List<MyBookingSummary>> FetchAgentBookingDetailsAsync(int userId, string affiliateId,
            bool onlyCancelledBooking, string agentName = "");

        Task<List<MyUserEmailPreferences>> FetchUserEmailPreferencesAsync(int userId, string affiliateId,
            string languageCode);

        Task<bool> SaveUserEmailPreferencesAsync(int userId, string affiliateId, string answerString);

        Task<bool> UpdateUserInfoAsync(UserInfo userInfo);

        Task<DateTime> GetUserCreationDate(string emailID);
    }
}