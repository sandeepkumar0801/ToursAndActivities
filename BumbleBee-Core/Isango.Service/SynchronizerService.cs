using CacheManager.Contract;
using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Enums;
using Isango.Persistence.Contract;
using Isango.Service.Constants;
using Isango.Service.Contract;
using Logger.Contract;
using System.Configuration;
using System.Data;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Util;

namespace Isango.Service
{
    public class SynchronizerService : ISynchronizerService
    {
        private readonly IMasterService _masterService;
        private readonly IActivityPersistence _activityPersistence;
        private readonly IMailerService _mailerService;
        private readonly IActivityCacheManager _activityCacheManager;
        private readonly ILogger _log;
        private readonly int _maxParallelThreadCount;
        private readonly ISynchronizerCacheManager _synchronizerCacheManager;
        private readonly ICacheLoaderService _cacheLoaderService;
        private static bool _isCosmosInsertDeleteLogging;

        public SynchronizerService(IMasterService masterService, IActivityPersistence activityPersistence, ILogger log, IActivityCacheManager activityCacheManager,
            ISynchronizerCacheManager synchronizerCacheManager, ICacheLoaderService cacheLoaderService,IMailerService mailerService = null)
        {
            _masterService = masterService;
            _activityPersistence = activityPersistence;
            _log = log;
            _mailerService = mailerService;
            _maxParallelThreadCount = MaxParallelThreadCountHelper.GetMaxParallelThreadCount(Isango.Service.Constants.Constant.MaxParallelThreadCount);
            _activityCacheManager = activityCacheManager;
            _synchronizerCacheManager = synchronizerCacheManager;
            _cacheLoaderService = cacheLoaderService;
            try
            {
                _isCosmosInsertDeleteLogging = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.IsCosmosInsertDeleteLogging) == "1";
            }
            catch (Exception)
            {
                _isCosmosInsertDeleteLogging = false;
            }
        }

        /// <summary>
        /// Polls the database for changes.
        /// </summary>
        public async Task<bool> PollDatabaseForChangesAsync()
        {
            var mailErrorLog = new List<Tuple<string, string>>();
            var successfullyUpdatedServices = string.Empty;
            var activitiesChangeTracker = default(List<ActivityChangeTracker>);

            try
            {
                activitiesChangeTracker = await GetModifiedServicesAsync();

                if (activitiesChangeTracker.Count > 0)
                {
                    _log.Info($"SynchronizerService|GetModifiedServices|TotalModifiedServicesCount:{activitiesChangeTracker.Count}");
                    var activities = GetActivitiesForUpdate(activitiesChangeTracker);

                    if (activities.Item1.Count > 0)
                    {
                        _log.Info($"SynchronizerService|GetModifiedServices|updatedActivitiesCount:{activities.Item1.Count}");
                        successfullyUpdatedServices = UpdateOrRemoveCache(activities.Item1, OperationType.Update);
                    }
                    if (activities.Item2.Count > 0)
                    {
                        _log.Info($"SynchronizerService|GetModifiedServices|DeletedActivitiesCount:{activities.Item2.Count}");
                        successfullyUpdatedServices += (successfullyUpdatedServices == string.Empty ? "" : ",") + UpdateOrRemoveCache(activities.Item2, OperationType.Delete);
                    }
                    if (activities.Item3.Count > 0)
                    {
                        _log.Info($"SynchronizerService|GetModifiedServices|InsertedActivitiesCount:{activities.Item3.Count}");
                        successfullyUpdatedServices += (successfullyUpdatedServices == string.Empty ? "" : ",") + UpdateOrRemoveCache(activities.Item3, OperationType.Insert);
                    }

                    // Deleting CachingManager table
                    if (successfullyUpdatedServices != string.Empty)
                    {
                        await RemoveUpdatedServicesAsync(successfullyUpdatedServices);
                    }
                    mailErrorLog.Add(Tuple.Create("SynchronizerService|PollDatabaseForChanges", "Success"));
                    return true;
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "SynchronizerService",
                    MethodName = "PollDatabaseForChanges",
                    Params = $"{successfullyUpdatedServices}"
                };
                _log.Error(isangoErrorEntity, ex);
                mailErrorLog.Add(Tuple.Create("SynchronizerService|PollDatabaseForChanges", "Error:" + ex));
                throw;
            }
            finally
            {
                if (mailErrorLog != null && mailErrorLog.Count > 0 && ConfigurationManagerHelper.GetValuefromAppSettings("ErrorMail") == "1")
                {
                    SendMail(mailErrorLog);
                }
            }
            _cacheLoaderService.ClearActivityWebsiteCache(activitiesChangeTracker);
            return false;
        }
        
        private Tuple<List<ActivityChangeTracker>, List<ActivityChangeTracker>, List<ActivityChangeTracker>> GetActivitiesForUpdate(List<ActivityChangeTracker> activitiesChangeTracker)
        {
            _log.Info($"SynchronizerService|GetActivitiesForUpdate|Start");

            var updatedActivities = new List<ActivityChangeTracker>();
            var deletedActivities = new List<ActivityChangeTracker>();
            var insertActivities = new List<ActivityChangeTracker>();

            try
            {
                foreach (var activityChangeTracker in activitiesChangeTracker)
                {
                    if (activityChangeTracker.OperationType == OperationType.Update)
                        updatedActivities.Add(activityChangeTracker);
                    else if (activityChangeTracker.OperationType == OperationType.Insert)
                        insertActivities.Add(activityChangeTracker);
                    else
                        deletedActivities.Add(activityChangeTracker);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "SynchronizerService",
                    MethodName = "GetActivitiesForUpdate"
                };
                _log.Error(isangoErrorEntity, ex);
            }

            _log.Info("SynchronizerService|GetActivitiesForUpdate|End");

            var result = Tuple.Create(updatedActivities, deletedActivities, insertActivities);
            return result;
        }

        private string UpdateOrRemoveCache(List<ActivityChangeTracker> services, OperationType operationType)
        {
            _log.Info($"SynchronizerService|UpdateOrRemoveCache|start:{operationType}");

            var addInList = true;
            var result = new StringBuilder();
            var startDate = DateTime.Now;//As StartDate will be provided from UI, we are defining it as DateTime.Now

            try
            {
                var languages = _masterService.GetSupportedLanguagesAsync()?.GetAwaiter().GetResult();

                if (operationType != OperationType.Delete)
                {
                    var activityIds = string.Join(",", services.Where(x => x.OperationType == OperationType.Insert || x.OperationType == OperationType.Update).Select(x => x.ActivityId.ToString()));
                    var passengerInfoList = _activityPersistence.GetPassengerInfoDetails(activityIds);
                    var isangoAvailabilities = _activityPersistence.GetOptionAvailability();

                    Parallel.ForEach(languages, new ParallelOptions { MaxDegreeOfParallelism = _maxParallelThreadCount }, (language) =>
                    {
                        //    foreach (var language in languages)
                        //{
                        var collectionExist = CreateCollectionIfNotExist(language.Code).GetAwaiter().GetResult();
                        if (collectionExist)
                        {
                            foreach (var service in services)
                            {
                                try
                                {
                                    var passengerinfo = passengerInfoList.Where(x => x.ActivityId == service.ActivityId).ToList();

                                    var defaultClientInfo = GetDefaultClientInfo();
                                    defaultClientInfo.LanguageCode = language.Code;
                                    //Send a loaded activity object rather than id
                                    if (operationType == OperationType.Insert)
                                    {
                                        InsertActivityInCacheAsync(service.ActivityId, defaultClientInfo, startDate, passengerinfo, isangoAvailabilities).GetAwaiter().GetResult();
                                    }
                                    if (operationType == OperationType.Update)
                                        UpdateActivityInCacheAsync(service.ActivityId, defaultClientInfo, startDate, passengerinfo, isangoAvailabilities, service.IsHbProduct, languages).GetAwaiter().GetResult();
                                    if (addInList)
                                        result.Append(service.ActivityId + ",");
                                    Task.Run(() => RemoveActivityFromMemoryCache(service.ActivityId, language.Code)).GetAwaiter().GetResult();
                                }
                                catch (Exception ex)
                                {
                                    Task.Run(() =>
                                            _log.Error(new IsangoErrorEntity
                                            {
                                                ClassName = "SynchronizerService",
                                                MethodName = "UpdateOrRemoveCache",
                                                Params = $"{operationType}:{service.ActivityId}"
                                            }, ex)
                                    );
                                }
                            }
                            addInList = false;
                        }
                        //}
                    });
                }
                else if (operationType == OperationType.Delete)
                {
                    var updatedActivitiesIds = services?.Select(x => x.ActivityId).ToArray();
                    Task.Run(() => RemoveFromCacheAsync(updatedActivitiesIds, languages)).GetAwaiter().GetResult();
                    Task.Run(() => RemoveActivitiesFromMemoryCache(updatedActivitiesIds, languages)).GetAwaiter().GetResult();
                    result.Append(string.Join(",", updatedActivitiesIds) + ",");
                }
                else
                {
                    var isangoErrorEntity = new IsangoErrorEntity
                    {
                        ClassName = "SynchronizerService",
                        MethodName = "UpdateOrRemoveCache",
                        Token = "TOKEN-UNDEFINED",
                        Params = $"{"Operation Type to test activity removal: " + operationType.ToString()}"
                    };
                    _log.Info(isangoErrorEntity);
                }
            }
            catch (Exception ex)
            {
                Task.Run(() =>
                    _log.Error(new IsangoErrorEntity
                    {
                        ClassName = "SynchronizerService",
                        MethodName = "UpdateOrRemoveCache",
                    }, ex)
                );
            }
            Task.Run(() => _log.Info($"SynchronizerService|UpdateOrRemoveCache|End:{operationType}"));

            return result.ToString().TrimEnd(',');
        }

        private ClientInfo GetDefaultClientInfo()
        {
            var currency = new Currency
            {
                IsoCode = "GBP",
                Symbol = "£",
                Name = "GBP"
            };

            var clientInfo = new ClientInfo
            {
                Currency = currency,
                LanguageCode = "EN",
                CountryIp = "127.1.1.1",
                AffiliateId = ConfigurationManager.AppSettings["DefaultAffiliateID"],
                B2BAffiliateId = ConfigurationManager.AppSettings["DefaultAffiliateID"]
            };

            return clientInfo;
        }

        /// <summary>
        /// Helper Method for Sending Success and Failure Mail
        /// </summary>
        /// <param name="data"></param>
        private void SendMail(List<Tuple<string, string>> data)
        {
            if (Convert.ToString(ConfigurationManager.AppSettings["ErrorMail"]) == "1")
            {
                _mailerService?.SendErrorMail(data);
            }
        }

        private void RemoveActivityFromMemoryCache(int activityId, string languageCode)
        {
            try
            {
                var key = $"activity_{activityId}_{languageCode}";
                RemoveActivitiesFromMemoryCacheUsingEndPoint(key);
            }
            catch (Exception ex)
            {
                //throw;
            }
        }

        private void RemoveActivitiesFromMemoryCache(int[] activityIds, List<Language> languages)
        {
            try
            {
                var keyBuider = new StringBuilder();
                foreach (var language in languages)
                {
                    foreach (var activityId in activityIds)
                    {
                        try
                        {
                            var key = $"Activity_{activityId}_{language.Code}";
                            keyBuider.Append(key);
                            keyBuider.Append(",");
                        }
                        catch (Exception ex)
                        {
                            //ignored
                            //throw;
                        }
                    }
                }
                var keys = keyBuider.Remove(keyBuider.Length - 1, 1).ToString();
                RemoveActivitiesFromMemoryCacheUsingEndPoint(keys);
            }
            catch (Exception ex)
            {
                //throw;
            }
        }

        /// <summary>
        /// Remove cahed item from bumble bee instance
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private string RemoveActivitiesFromMemoryCacheUsingEndPoint(string key)
        {
            var result = string.Empty;
            try
            {
                using (var client = new HttpClient())
                {
                    var baseAddress = ConfigurationManagerHelper.GetValuefromAppSettings(Constants.Constant.WebAPIBaseUrl);
                    var cacheDeleteToken = ConfigurationManagerHelper.GetValuefromAppSettings(Constants.Constant.CacheDeleteToken);
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                    client.BaseAddress = new Uri(baseAddress);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var requestUri = new Uri(baseAddress + $"/api/Cache/delete?key={key}&cacheDeleteToken={cacheDeleteToken}");
                    var request = new HttpRequestMessage(HttpMethod.Delete, requestUri)
                    {
                        Content = new StringContent(string.Empty, Encoding.UTF8, "application/json")
                    };
                    var response = client.SendAsync(request)?.GetAwaiter().GetResult();
                    if (response.IsSuccessStatusCode)
                    {
                        result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    }
                }
            }
            catch (Exception ex)
            {
                result = $"{ex.Message}\n{ex.StackTrace}";
            }
            return result;
        }

        /// <summary>
        /// Returns the Modified Services for caching service
        /// </summary>
        /// <returns></returns>
        public async Task<List<ActivityChangeTracker>> GetModifiedServicesAsync()
        {
            try
            {
                var result = _activityPersistence.GetModifiedServices();
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "GetModifiedServicesAsync",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Remove Updated Services from Persistence cache manager table
        /// </summary>
        /// <param name="servicedIds"></param>
        /// <returns></returns>
        public async Task<int> RemoveUpdatedServicesAsync(string servicedIds)
        {
            try
            {
                var result = _activityPersistence.RemoveUpdatedServices(servicedIds);
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "RemoveUpdatedServices",
                    Params = $"{servicedIds}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        ///  Remove activity from collection
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        public async Task<bool> CreateCollectionIfNotExist(string language)
        {
            try
            {
                var result = _synchronizerCacheManager.CreateCollectionIfNotExist(language);
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "CreateCollectionIfNotExist",
                    Params = $"{language}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        ///  Insert activity in collection
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="clientInfo"></param>
        /// <param name="startDate"></param>
        /// <param name="passengerInfo"></param>
        /// <param name="availability"></param>
        /// <returns></returns>
        public async Task<bool> InsertActivityInCacheAsync(int activityId, ClientInfo clientInfo, DateTime startDate, List<Entities.Booking.PassengerInfo> passengerInfo, DataTable availability)
        {
            try
            {
                var activity = _activityPersistence.GetActivitiesByActivityIds(activityId.ToString(), clientInfo.LanguageCode).FirstOrDefault();
                if (activity == null)
                    return false;

                activity.PassengerInfo = passengerInfo;
                var updatedActivity = UpdateActivityPrice(activity, availability);

                return await Task.FromResult(_activityCacheManager.InsertActivity(updatedActivity));
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "ActivityInsertCache",
                    Token = clientInfo.ApiToken,
                    AffiliateId = clientInfo.AffiliateId,
                    Params = $"{activityId}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        private Activity UpdateActivityPrice(Activity activity, DataTable availability)
        {
            for (var i = 0; i < activity?.ProductOptions?.Count; i++)
            {
                try
                {
                    var result = availability.Select(activity.ActivityType == ActivityType.Bundle
                        ? $"{Constant.ServiceId} = {activity.ProductOptions[i].ComponentServiceID} and {Constant.ServiceOptionId} = {activity.ProductOptions[i].Id}"
                        : $"{Constant.ServiceId} = {activity.Id} and {Constant.ServiceOptionId} = {activity.ProductOptions[i].Id}");

                    if (result?.Length > 0)
                    {
                        var baseValue = Convert.ToDecimal(result.Min(x => x[Constant.GatePrice]));

                        // if multiple rows exists having same gate price then find min of cost price.
                        var costValue = Convert.ToDecimal(result.Where(x => Convert.ToDecimal(x[Constant.GatePrice]) == baseValue).Select(x => x[Constant.CostPrice]).Count() > 1 ? result.Where(x => Convert.ToDecimal(x[Constant.GatePrice]) == baseValue).Min(x => x[Constant.CostPrice]) : result.Where(x => Convert.ToDecimal(x[Constant.GatePrice]) == baseValue).Select(x => x[Constant.CostPrice]).First());

                        //As currency for the ServiceID is same for any duration, so taking currency of the first row if multiple rows exists.
                        var currency = new Currency()
                        {
                            IsoCode = result.Where(x => Convert.ToDecimal(x[Constant.GatePrice]) == baseValue).Select(x => x[Constant.CurrencyISOCode]).First().ToString()
                        };

                        var costPrice = new Price()
                        {
                            Currency = currency,
                            Amount = costValue
                        };

                        var basePrice = new Price()
                        {
                            Currency = currency,
                            Amount = baseValue
                        };

                        var gateBasePrice = new Price()
                        {
                            Currency = currency,
                            Amount = baseValue
                        };

                        activity.ProductOptions[i].CostPrice = costPrice;
                        activity.ProductOptions[i].BasePrice = basePrice;
                        activity.ProductOptions[i].GateBasePrice = gateBasePrice;
                    }
                }
                catch
                {
                    //ignored
                }
            }

            return activity;
        }

        /// <summary>
        /// Update activity in collection
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="clientInfo"></param>
        /// <param name="startDate"></param>
        /// <param name="passengerInfo"></param>
        /// <param name="availability"></param>
        /// <returns></returns>
        public async Task<bool> UpdateActivityInCacheAsync(int activityId, ClientInfo clientInfo, DateTime startDate, List<Entities.Booking.PassengerInfo> passengerInfo, DataTable availability, bool isAPIProduct = false, List<Language> languagesResult = null)
        {
            try
            {
                if (languagesResult == null)
                {
                    languagesResult = _masterService.GetSupportedLanguagesAsync()?.GetAwaiter().GetResult();
                }
                var languages = languagesResult.Where(x => x.Code == clientInfo.LanguageCode)?.ToList();

                var activity = isAPIProduct ?
                                _activityPersistence.LoadLiveHbActivities(activityId, clientInfo.LanguageCode)?.FirstOrDefault()
                                : _activityPersistence.GetActivitiesByActivityIds(activityId.ToString(), clientInfo.LanguageCode)?.FirstOrDefault();

                if (activity == null)
                {
                    var isangoErrorEntity = new IsangoErrorEntity
                    {
                        ClassName = "ActivityService",
                        MethodName = "UpdateActivityInCacheAsync",
                        Token = clientInfo.ApiToken,
                        AffiliateId = clientInfo.AffiliateId,
                        Params = $"{"activity is null for DB: " + activityId + " Language:" + clientInfo.LanguageCode}"
                    };
                    _log.Info(isangoErrorEntity);
                    return false;
                }

                activity.PassengerInfo = passengerInfo;
                var updatedActivity = UpdateActivityPrice(activity, availability);

                _synchronizerCacheManager.DeleteActivityFromCache(activity.ID, languages);
                if (_isCosmosInsertDeleteLogging)
                {
                    var isangoErrorEntityDelete = new IsangoErrorEntity
                    {
                        ClassName = "ActivityService",
                        MethodName = "UpdateActivityInCacheAsync",
                        Token = clientInfo.ApiToken,
                        AffiliateId = clientInfo.AffiliateId,
                        Params = $"Deleted Activity: {activity.ID},{languages?.FirstOrDefault()}"
                    };
                    _log.Info(isangoErrorEntityDelete);
                }

                var result = _activityCacheManager.InsertActivity(updatedActivity, clientInfo.LanguageCode);
                if (_isCosmosInsertDeleteLogging)
                {
                    var isangoErrorEntityInsert = new IsangoErrorEntity
                    {
                        ClassName = "ActivityService",
                        MethodName = "UpdateActivityInCacheAsync",
                        Token = clientInfo.ApiToken,
                        AffiliateId = clientInfo.AffiliateId,
                        Params = $"Insert Activity: {activity.ID},{clientInfo.LanguageCode},{result}"
                    };
                    _log.Info(isangoErrorEntityInsert);
                }
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "UpdateActivityInCacheAsync",
                    Token = clientInfo.ApiToken,
                    AffiliateId = clientInfo.AffiliateId,
                    Params = $"{activityId}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        ///  Remove activity from collection
        /// </summary>
        /// <param name="activityIds"></param>
        /// <param name="languages"></param>
        /// <returns></returns>
        public async Task<List<int>> RemoveFromCacheAsync(int[] activityIds, List<Language> languages)
        {
            try
            {
                var result = _synchronizerCacheManager.RemoveFromCache(activityIds, languages);
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ActivityService",
                    MethodName = "RemoveFromCache",
                    Params = $"{activityIds}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }
    }
}