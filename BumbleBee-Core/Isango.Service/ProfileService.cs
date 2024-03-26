using Isango.Entities;
using Isango.Entities.MyIsango;
using Isango.Persistence.Contract;
using Isango.Service.Contract;
using Logger.Contract;

namespace Isango.Service
{
    public class ProfileService : IProfileService
    {
        private readonly IProfilePersistence _profilePersistence;
        private readonly ILogger _log;

        public ProfileService(IProfilePersistence profilePersistence, ILogger log)
        {
            _profilePersistence = profilePersistence;
            _log = log;
        }

        /// <summary>
        /// This Operation is used to fetch Agent booking details
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="affiliateId"></param>
        /// <param name="onlyCancelledBooking"></param>
        /// <param name="agentName"></param>
        /// <returns></returns>
        public async Task<List<MyBookingSummary>> FetchAgentBookingDetailsAsync(int userId, string affiliateId, bool onlyCancelledBooking, string agentName = "")
        {
            try
            {
                var userBookingDetailsList = _profilePersistence.FetchAgentBookingDetails(userId, affiliateId, onlyCancelledBooking, agentName);
                return await Task.FromResult(userBookingDetailsList);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ProfileService",
                    MethodName = "FetchAgentBookingDetailsAsync",
                    AffiliateId = affiliateId,
                    Params = $"{userId}{affiliateId}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// This Operation is used to fetch User booking details
        /// </summary>
        /// <param name="emailId"></param>
        /// <param name="affiliateId"></param>
        /// <param name="isAgent"></param>
        /// <returns></returns>
        public async Task<List<MyBookingSummary>> FetchUserBookingDetailsAsync(string emailId, string affiliateId, bool isAgent)
        {
            try
            {
                var userBookingDetailsList = _profilePersistence.FetchUserBookingDetails(emailId, affiliateId, isAgent);
                return await Task.FromResult(userBookingDetailsList);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ProfileService",
                    MethodName = "FetchUserBookingDetailsAsync",
                    AffiliateId = affiliateId,
                    Params = $"{emailId}{affiliateId}{isAgent}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// This Operation is used to fetch User email preferences
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="affiliateId"></param>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public async Task<List<MyUserEmailPreferences>> FetchUserEmailPreferencesAsync(int userId, string affiliateId, string languageCode)
        {
            try
            {
                if (userId > 0)
                {
                    var userEmailPreferenceList = _profilePersistence.FetchUserEmailPreferences(userId, affiliateId, languageCode);
                    return await Task.FromResult(userEmailPreferenceList);
                }

                return null;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ProfileService",
                    MethodName = "FetchUserEmailPreferencesAsync",
                    AffiliateId = affiliateId,
                    Params = $"{userId}{affiliateId}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// This Operation is used to fetch user information
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<ISangoUser> FetchUserInfoAsync(int userId)
        {
            try
            {
                if (userId > 0)
                {
                    var userInfo = _profilePersistence.FetchUserInfo(userId);
                    return await Task.FromResult(userInfo);
                }

                return null;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ProfileService",
                    MethodName = "FetchUserInfoAsync",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// This Operation is used to save user email preferences
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="affiliateId"></param>
        /// <param name="answerString"></param>
        /// <returns></returns>
        public async Task<bool> SaveUserEmailPreferencesAsync(int userId, string affiliateId, string answerString)
        {
            try
            {
                if (userId > 0)
                {
                    var isSaved = _profilePersistence.SaveUserEmailPreferences(userId, affiliateId, answerString);
                    return await Task.FromResult(isSaved);
                }

                return await Task.FromResult(false);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ProfileService",
                    MethodName = "SaveUserEmailPreferencesAsync",
                    AffiliateId = affiliateId,
                    Params = $"{userId}{affiliateId}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// This Operation is used to update user information
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public async Task<bool> UpdateUserInfoAsync(UserInfo userInfo)
        {
            try
            {
                if (userInfo != null && userInfo.UserId > 0)
                {
                    var isUpdated = _profilePersistence.UpdateUserInfo(userInfo);
                    return await Task.FromResult(isUpdated);
                }

                return await Task.FromResult(false);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ProfileService",
                    MethodName = "UpdateUserInfoAsync",
                    Params = $"{userInfo}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="emailId"></param>
        /// <param name="affiliateId"></param>
        /// <param name="isAgent"></param>
        /// <returns></returns>
        public async Task<List<MyBookingSummarySitecoreViewModel>> FetchUserBookingDetailsSitecoreAsync(string emailId, string affiliateId, bool isAgent)
        {
            try
            {
                var userBookingDetailsList = _profilePersistence.FetchUserBookingDetails(emailId, affiliateId, isAgent);
                var siteCoreBookingDetails = new List<MyBookingSummarySitecoreViewModel>();
                foreach (var ub in userBookingDetailsList)
                {
                    try
                    {
                        var scBookingSummary = new MyBookingSummarySitecoreViewModel
                        {
                            AffiliateName = string.Empty,
                            BookingDate = ub.BookingDate,
                            BookingDetail = new List<MyBookedProductSitecoreViewModel>(),

                            BookingId = ub.BookingId,
                            BookingRefNo = ub.BookingRefenceNumber,
                            CurrencyShortSymbol = ub.BookingAmountCurrency
                        };
                        foreach (var b in ub?.BookedProducts)
                        {
                            try
                            {
                                var p = new MyBookedProductSitecoreViewModel
                                {
                                    BookingId = ub.BookingId,
                                    BookingDate = b.TravelDate,
                                    //AdultCount = b.NoOfAdults,
                                    //ChildCount = b.NoOfChildren,
                                    CurrencyShortSymbol = ub.BookingAmountCurrency,
                                    OptionStatusId = GetBookingStatusId(b.BookingStatus),
                                    OptionStatusName = b.BookingStatus,
                                    SellAmount = Convert.ToDouble(b.BookingAmountPaid),
                                    ServiceName = b.BookedProductName,
                                    TravelDate = b.TravelDate,
                                    PaxInfo = new List<MyPaxPriceInfoViewModel>(),
                                    AmountBeforeDiscount = Convert.ToDouble(b.AmountBeforeDiscount),
                                    TicketDetail = b.TicketDetail,
                                    ServiceId = b.ServiceId,
                                    BookedOptionId = b.BookedOptionId,
                                    IsReceipt = b.IsReceipt
                                };

                                var filterdata = ub?.PaxPriceInfo?.Where(x => x.BookedOptionId == p.BookedOptionId && x.BookingId == p.BookingId);

                                if (filterdata != null && filterdata.Count() > 0)
                                {
                                    foreach (var z in filterdata)
                                    {
                                        try
                                        {
                                            var GetPaxInfo = new MyPaxPriceInfoViewModel
                                            {
                                                PassengerCount = z.PassengerCount,
                                                PassengerOriginalSellAmount = z.BookedPassengerRateOriginalSellAmount,
                                                PassengerSellAmount = z.BookedPassengerRateSellAmount,
                                                PassengerType = z.PassengerType,
                                                PassengerTypeId = z.PassengerTypeId,
                                            };

                                            p.PaxInfo.Add(GetPaxInfo);
                                        }
                                        catch (Exception)
                                        {
                                            //throw;
                                        }
                                    }
                                }

                                scBookingSummary.BookingDetail.Add(p);
                            }
                            catch (Exception)
                            {
                                //throw;
                            }
                        }
                        siteCoreBookingDetails.Add(scBookingSummary);
                    }
                    catch (Exception ex)
                    {
                        var isangoErrorEntity = new IsangoErrorEntity
                        {
                            ClassName = "ProfileService",
                            MethodName = "FetchUserBookingDetailsSitecoreAsync_Insideloop",
                            AffiliateId = affiliateId,
                            Params = $"{emailId}{affiliateId}{isAgent}"
                        };
                        _log.Error(isangoErrorEntity, ex);
                        throw;
                    }
                }
                return await Task.FromResult(siteCoreBookingDetails);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ProfileService",
                    MethodName = "FetchUserBookingDetailsSitecoreAsync",
                    AffiliateId = affiliateId,
                    Params = $"{emailId}{affiliateId}{isAgent}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Proc To get user creation date using emailId
        /// </summary>
        /// <param name="emailID"></param>
        /// <returns></returns>
        public async Task<DateTime> GetUserCreationDate(string emailID)
        {
            try
            {
                var userBookingDetailsList = _profilePersistence.GetUserCreationDate(emailID);
                return await Task.FromResult(userBookingDetailsList);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ProfileService",
                    MethodName = "GetUserCreationDate",
                    Params = $"{emailID}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Convert from string booking status to int booking status
        /// </summary>
        /// <param name="BookingStatus"></param>
        /// <returns></returns>
        private int GetBookingStatusId(string BookingStatus)
        {
            int result = 0;
            BookingStatus = BookingStatus?.ToLower()?.Trim();
            switch (BookingStatus)
            {
                case "on request":
                    {
                        result = 1;
                        break;
                    }
                case "confirmed":
                    {
                        result = 2;
                        break;
                    }

                case "cancelled":
                    {
                        result = 3;
                        break;
                    }
                // ReSharper disable once RedundantEmptySwitchSection
                default:
                    break;
            }
            return result;
        }
    }
}