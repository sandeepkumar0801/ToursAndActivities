using Isango.Entities;
using Isango.Entities.HotelBeds;
using Isango.Persistence.Contract;
using Logger.Contract;
using ServiceAdapters.HB.HB.Entities.ContentMulti;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Util;
using Constant = Isango.Persistence.Constants.Constants;

namespace Isango.Persistence.ConsoleApplication.AgeGroup
{
    public class ApiTudePersistence : PersistenceBase, IApiTudePersistence
    {
        private readonly ILogger _log;
        public ApiTudePersistence(ILogger log)
        {
            _log = log;
        }
        /// <summary>
        /// Save ApiTude Age Group in the database
        /// </summary>
        /// <param name="ageGroups"></param>
        public void SaveApiTudeAgeGroups(List<ApiTudeAgeGroup> ageGroups)
        {
            try
            {
                if (ageGroups != null && ageGroups.Count > 0)
                {
                    //GroupBy ServiceID
                    var groupByServiceId = ageGroups?.OrderBy(x => x.ServiceID).GroupBy(x => x.ServiceID);
                    foreach (var groupItem in groupByServiceId)
                    {
                        var groupItemList = groupItem?.ToList();
                        var count = groupItemList?.Count ?? 0;

                        for (int itemCriteria = 0; itemCriteria < count; itemCriteria = (itemCriteria + count))
                        {
                            if (groupItemList[itemCriteria] != null && groupItemList?.Count > 0)
                            {
                                var criteriaRecordsAtTime = groupItemList?.Skip(itemCriteria)?.Take(count)?.ToList();
                                var ageGroupDataTable = SetAgeGroup(criteriaRecordsAtTime);

                                //try
                                //{
                                //    using (var command = IsangoDataBase.GetStoredProcCommand(Constant.SyncAPIPriceAvailabilitySp))
                                //    {
                                //        IsangoDataBase.ExecuteNonQuery(command);
                                //    }
                                //}
                                //catch (Exception ex)
                                //{
                                //    //ignored
                                //    //##TODO add logging here
                                //}
                                try
                                {
                                    using (var connection = new SqlConnection(ConfigurationManagerHelper.GetValuefromConfig(Constant.APIUploadDb)))
                                    {
                                        using (var insertCommand = new SqlCommand(Constant.InsertApiTudeAgeGroupsSp, connection))
                                        {
                                            insertCommand.CommandType = CommandType.StoredProcedure;

                                            var tvpAgeGroup = insertCommand.Parameters.AddWithValue(Constant.HBTServiceAgeGroupParameter, ageGroupDataTable);
                                            tvpAgeGroup.SqlDbType = SqlDbType.Structured;

                                            try
                                            {
                                                connection.Open();
                                                insertCommand.ExecuteNonQuery();
                                            }
                                            finally
                                            {
                                                if (connection.State != ConnectionState.Closed)
                                                    connection.Close();
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    //ignored
                                    //##TODO add logging here
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ApiTudePersistence",
                    MethodName = "SaveApiTudeAgeGroups",
                    Params = $"{SerializeDeSerializeHelper.Serialize(ageGroups)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }


        /// <summary>
        ///
        /// </summary>
        /// <param name="activitiesContent"></param>
        public void SaveApiTudeContent(List<ActivitiesContent> activitiesContent)
        {
            //    //Check LastUpdated Date of Product
            //    var GetContentLastUpdated = GetContentFactSheetDetail();

            //    //1.) Save ApiTude ContentFactSheetDetail //[HBT].[Usp_Insert_ProductFactSheetdetail , hbt_ProductFactSheet]
            //    var contentProduct = ContentFactSheetDetail(activitiesContent);
            //    var sendOnlyUpdatedDataFactSheet = GetContentLastUpdated.Count > 0
            //        ? (contentProduct.Where(x => !GetContentLastUpdated.
            //        Any(y => x.LastUpdate.Date == y.Lastupdate.Date &&
            //        x.LanguagCode.ToLowerInvariant().Trim() == y.LanguageCode.ToLowerInvariant().Trim() &&
            //        x.FactsheetId == y.FactSheetId.ToString())).ToList())
            //        : contentProduct;
            //    SaveApiTudeContentFactSheetDetail(sendOnlyUpdatedDataFactSheet);

            //    //2.) Save Destination Detail  //[HBT].[Usp_Insert_Destination_Detail , [HBT].[Destination] ]
            //    var contentDestination = ContentDestinations(activitiesContent);
            //    var sendOnlyUpdatedDataDestination = GetContentLastUpdated.Count > 0
            //        ? (contentDestination.Where(x => sendOnlyUpdatedDataFactSheet.
            //        Any(y => x.LanguagCode.ToLowerInvariant().Trim() == y.LanguagCode.ToLowerInvariant().Trim() &&
            //        x.FactsheetId.ToString() == y.FactsheetId.ToString())).ToList())
            //        : contentDestination;
            //    SaveApiTudeContentDestination(sendOnlyUpdatedDataDestination);

            //    //3.) Save Country //hbt_usp_ins_country , hbt_Country
            //    var contentCountry = ContentCountry(activitiesContent);
            //    var sendOnlyUpdatedDataCountry = GetContentLastUpdated.Count > 0
            //        ? (contentCountry.Where(x => sendOnlyUpdatedDataFactSheet.
            //        Any(y => x.LanguageCode.ToLowerInvariant().Trim() == y.LanguagCode.ToLowerInvariant().Trim() &&
            //        x.Factsheetid.ToString() == y.FactsheetId.ToString())).ToList())
            //        : contentCountry;
            //    SaveApiTudeContentCountry(sendOnlyUpdatedDataCountry);

            //    //4.) Save Location //hbt_usp_ins_location , dbo.HBTLocation
            //    var contentLocation = ContentLocation(activitiesContent);
            //    var sendOnlyUpdatedDataLocation = GetContentLastUpdated.Count > 0
            //        ? (contentLocation.Where(x => sendOnlyUpdatedDataFactSheet.
            //        Any(y => x.Language.ToLowerInvariant().Trim() == y.LanguagCode.ToLowerInvariant().Trim() &&
            //        x.Factsheetid.ToString() == y.FactsheetId.ToString())).ToList())
            //        : contentLocation;
            //    SaveApiTudeContentLocation(sendOnlyUpdatedDataLocation);

            //    //5.) Save Features //hbt_usp_ins_feature_detail, hbt_feature
            //    var contentFeature = ContentFeature(activitiesContent);
            //    var sendOnlyUpdatedDataFeature = GetContentLastUpdated.Count > 0
            //        ? (contentFeature.Where(x => sendOnlyUpdatedDataFactSheet.
            //        Any(y => x.Language.ToLowerInvariant().Trim() == y.LanguagCode.ToLowerInvariant().Trim() &&
            //        x.FactsheetId.ToString() == y.FactsheetId.ToString())).ToList())
            //        : contentFeature;
            //    SaveApiTudeContentFeature(sendOnlyUpdatedDataFeature);
            //    //6.) Save Media //hbt_usp_ins_Media
            //    var contentMedia = ContentMedia(activitiesContent);
            //    var sendOnlyUpdatedDataMedia = GetContentLastUpdated.Count > 0
            //        ? (contentMedia.Where(x => sendOnlyUpdatedDataFactSheet.
            //        Any(y => x.Language.ToLowerInvariant().Trim() == y.LanguagCode.ToLowerInvariant().Trim() &&
            //        x.Factsheetid.ToString() == y.FactsheetId.ToString())).ToList())
            //        : contentMedia;
            //    SaveApiTudeContentMedia(sendOnlyUpdatedDataMedia);
            //    //7.) Save Routes //hbt_usp_ins_routes, hbt_routes
            //    var contentRoute = ContentRoute(activitiesContent);
            //    var sendOnlyUpdatedDataRoute = GetContentLastUpdated.Count > 0
            //        ? (contentRoute.Where(x => sendOnlyUpdatedDataFactSheet.
            //        Any(y => x.Language.ToLowerInvariant().Trim() == y.LanguagCode.ToLowerInvariant().Trim() &&
            //        x.Factsheetid.ToString() == y.FactsheetId.ToString())).ToList())
            //        : contentRoute;
            //    SaveApiTudeContentRoute(sendOnlyUpdatedDataRoute);

            //    //8.) Save RedeemInfo [HBT].[usp_ins_redeeminfo]
            //    var contentRedeemInfo = ContentRedeeminfo(activitiesContent);
            //    var sendOnlyUpdatedDataRedeem = GetContentLastUpdated.Count > 0
            //        ? (contentRedeemInfo.Where(x => sendOnlyUpdatedDataFactSheet.
            //        Any(y => x.Language.ToLowerInvariant().Trim() == y.LanguagCode.ToLowerInvariant().Trim() &&
            //        x.Factsheetid.ToString() == y.FactsheetId.ToString())).ToList())
            //        : contentRedeemInfo;
            //    SaveApiTudeContentRedeemInfo(sendOnlyUpdatedDataRedeem);

            //    //8 ) Save Detail info //[HBT].[usp_ins_detailinfo]
            //    var contentDescriptionInfo = ContentDescription(activitiesContent);
            //    var sendOnlyUpdatedDataDetail = GetContentLastUpdated.Count > 0 ? (contentDescriptionInfo.Where(x => sendOnlyUpdatedDataFactSheet.
            //    Any(y => x.Language.ToLowerInvariant().Trim() == y.LanguagCode.ToLowerInvariant().Trim() &&
            //    x.FactsheetID.ToString() == y.FactsheetId.ToString())).ToList()) : contentDescriptionInfo;
            //    SaveApiTudeContentDescription(sendOnlyUpdatedDataDetail);

            //    //9.) Save Scheduling //[HBT].[usp_ins_Scheduling]
            //    var contentScheduling = ContentScheduling(activitiesContent);
            //    var sendOnlyUpdatedDataScheduling = GetContentLastUpdated.Count > 0 ? (contentScheduling.Where(x => sendOnlyUpdatedDataFactSheet.
            //    Any(y => x.Language.ToLowerInvariant().Trim() == y.LanguagCode.ToLowerInvariant().Trim() &&
            //    x.Factsheetid.ToString() == y.FactsheetId.ToString())).ToList()) : contentScheduling;
            //    SaveApiTudeContentScheduling(sendOnlyUpdatedDataScheduling);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="activitiesContent"></param>
        public void SaveApiTudeContentCalendar(List<ServiceAdapters.HB.HB.Entities.Calendar.Activity> activities)
        {
            try
            {
                var activitiesContent = activities.Where(x => x != null).Where(x => x.Content != null).Select(y => y.Content).ToList();
                //Check LastUpdated Date of Product
                var GetContentLastUpdated = GetContentFactSheetDetail();

                //1.) Save ApiTude ContentFactSheetDetail //[HBT].[Usp_Insert_ProductFactSheetdetail , hbt_ProductFactSheet]
                var contentProduct = ContentFactSheetDetail(activitiesContent);
                var sendOnlyUpdatedDataFactSheet = GetContentLastUpdated.Count > 0
                    ? (contentProduct.Where(x => !GetContentLastUpdated.
                    Any(y => x.LastUpdate.Date == y.Lastupdate.Date &&
                    x.LanguagCode.ToLowerInvariant().Trim() == y.LanguageCode.ToLowerInvariant().Trim() &&
                    x.FactsheetId == y.FactSheetId.ToString())).ToList())
                    : contentProduct;
                SaveApiTudeContentFactSheetDetail(sendOnlyUpdatedDataFactSheet);

                //2.) Save Destination Detail  //[HBT].[Usp_Insert_Destination_Detail , [HBT].[Destination] ]
                var contentDestination = ContentDestinations(activitiesContent);
                var sendOnlyUpdatedDataDestination = GetContentLastUpdated.Count > 0
                    ? (contentDestination.Where(x => sendOnlyUpdatedDataFactSheet.
                    Any(y => x.LanguagCode.ToLowerInvariant().Trim() == y.LanguagCode.ToLowerInvariant().Trim() &&
                    x.FactsheetId.ToString() == y.FactsheetId.ToString())).ToList())
                    : contentDestination;
                SaveApiTudeContentDestination(sendOnlyUpdatedDataDestination);

                //3.) Save Country //hbt_usp_ins_country , hbt_Country
                var contentCountry = ContentCountry(activitiesContent);
                var sendOnlyUpdatedDataCountry = GetContentLastUpdated.Count > 0
                    ? (contentCountry.Where(x => sendOnlyUpdatedDataFactSheet.
                    Any(y => x.LanguageCode.ToLowerInvariant().Trim() == y.LanguagCode.ToLowerInvariant().Trim() &&
                    x.Factsheetid.ToString() == y.FactsheetId.ToString())).ToList())
                    : contentCountry;
                SaveApiTudeContentCountry(sendOnlyUpdatedDataCountry);

                //4.) Save Location //hbt_usp_ins_location , dbo.HBTLocation
                var contentLocation = ContentLocation(activitiesContent);
                var sendOnlyUpdatedDataLocation = GetContentLastUpdated.Count > 0
                    ? (contentLocation.Where(x => sendOnlyUpdatedDataFactSheet.
                    Any(y => x.Language.ToLowerInvariant().Trim() == y.LanguagCode.ToLowerInvariant().Trim() &&
                    x.Factsheetid.ToString() == y.FactsheetId.ToString())).ToList())
                    : contentLocation;
                SaveApiTudeContentLocation(sendOnlyUpdatedDataLocation);

                //5.) Save Features //hbt_usp_ins_feature_detail, hbt_feature
                var contentFeature = ContentFeature(activitiesContent);
                var sendOnlyUpdatedDataFeature = GetContentLastUpdated.Count > 0
                    ? (contentFeature.Where(x => sendOnlyUpdatedDataFactSheet.
                    Any(y => x.Language.ToLowerInvariant().Trim() == y.LanguagCode.ToLowerInvariant().Trim() &&
                    x.FactsheetId.ToString() == y.FactsheetId.ToString())).ToList())
                    : contentFeature;
                SaveApiTudeContentFeature(sendOnlyUpdatedDataFeature);
                //6.) Save Media //hbt_usp_ins_Media
                var contentMedia = ContentMedia(activitiesContent);
                var sendOnlyUpdatedDataMedia = GetContentLastUpdated.Count > 0
                    ? (contentMedia.Where(x => sendOnlyUpdatedDataFactSheet.
                    Any(y => x.Language.ToLowerInvariant().Trim() == y.LanguagCode.ToLowerInvariant().Trim() &&
                    x.Factsheetid.ToString() == y.FactsheetId.ToString())).ToList())
                    : contentMedia;
                SaveApiTudeContentMedia(sendOnlyUpdatedDataMedia);
                SaveApiTudeMediImages(contentMedia);
                //7.) Save Routes //hbt_usp_ins_routes, hbt_routes
                var contentRoute = ContentRoute(activitiesContent);
                var sendOnlyUpdatedDataRoute = GetContentLastUpdated.Count > 0
                    ? (contentRoute.Where(x => sendOnlyUpdatedDataFactSheet.
                    Any(y => x.Language.ToLowerInvariant().Trim() == y.LanguagCode.ToLowerInvariant().Trim() &&
                    x.Factsheetid.ToString() == y.FactsheetId.ToString())).ToList())
                    : contentRoute;
                SaveApiTudeContentRoute(sendOnlyUpdatedDataRoute);

                //8.) Save RedeemInfo [HBT].[usp_ins_redeeminfo]
                var contentRedeemInfo = ContentRedeeminfo(activitiesContent);
                var sendOnlyUpdatedDataRedeem = GetContentLastUpdated.Count > 0
                    ? (contentRedeemInfo.Where(x => sendOnlyUpdatedDataFactSheet.
                    Any(y => x.Language.ToLowerInvariant().Trim() == y.LanguagCode.ToLowerInvariant().Trim() &&
                    x.Factsheetid.ToString() == y.FactsheetId.ToString())).ToList())
                    : contentRedeemInfo;
                SaveApiTudeContentRedeemInfo(sendOnlyUpdatedDataRedeem);

                //8 ) Save Detail info //[HBT].[usp_ins_detailinfo]
                var contentDescriptionInfo = ContentDescription(activitiesContent);
                var sendOnlyUpdatedDataDetail = GetContentLastUpdated.Count > 0 ? (contentDescriptionInfo.Where(x => sendOnlyUpdatedDataFactSheet.
                Any(y => x.Language.ToLowerInvariant().Trim() == y.LanguagCode.ToLowerInvariant().Trim() &&
                x.FactsheetID.ToString() == y.FactsheetId.ToString())).ToList()) : contentDescriptionInfo;
                SaveApiTudeContentDescription(sendOnlyUpdatedDataDetail);

                //9.) Save Scheduling //[HBT].[usp_ins_Scheduling]
                var contentScheduling = ContentScheduling(activitiesContent);
                var sendOnlyUpdatedDataScheduling = GetContentLastUpdated.Count > 0 ? (contentScheduling.Where(x => sendOnlyUpdatedDataFactSheet.
                Any(y => x.Language.ToLowerInvariant().Trim() == y.LanguagCode.ToLowerInvariant().Trim() &&
                x.Factsheetid.ToString() == y.FactsheetId.ToString())).ToList()) : contentScheduling;
                SaveApiTudeContentScheduling(sendOnlyUpdatedDataScheduling);

                //10.) Save HighLights //[HBT].[usp_ins_activityhighlights]
                var contentHighLights = ContentHighLights(activitiesContent);
                var sendOnlyUpdatedDataHighLights = GetContentLastUpdated.Count > 0 ? (contentHighLights.Where(x => sendOnlyUpdatedDataFactSheet.
                Any(y => x.Language.ToLowerInvariant().Trim() == y.LanguagCode.ToLowerInvariant().Trim() &&
                x.Factsheetid.ToString() == y.FactsheetId.ToString())).ToList()) : contentHighLights;
                SaveApiTudeContentHighLights(sendOnlyUpdatedDataHighLights);

                //11.) Save OperationalDays //[HBT].[usp_ins_operationdays]
                var contentOperationDays = ContentOperationalDays(activities);
                var sendOnlyUpdatedOperationalDays = GetContentLastUpdated.Count > 0 ? (contentOperationDays.Where(x => sendOnlyUpdatedDataFactSheet.
                Any(y => x.Language.ToLowerInvariant().Trim() == y.LanguagCode.ToLowerInvariant().Trim() &&
                x.Factsheetid.ToString() == y.FactsheetId.ToString())).ToList()) : contentOperationDays;
                SaveApiTudeContentOperationalDays(sendOnlyUpdatedOperationalDays);

                //12.) Save Duration //[HBT].[usp_ins_activityDuration]
                var contentDuration = ContentDuration(activities);
                var sendOnlyUpdatedDuration = GetContentLastUpdated.Count > 0 ? (contentDuration.Where(x => sendOnlyUpdatedDataFactSheet.
                Any(y => x.Language.ToLowerInvariant().Trim() == y.LanguagCode.ToLowerInvariant().Trim() &&
                x.Factsheetid.ToString() == y.FactsheetId.ToString())).ToList()) : contentDuration;
                SaveApiTudeContentDuration(sendOnlyUpdatedDuration);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "ApiTudePersistence",
                    MethodName = "SaveApiTudeContentCalendar",
                    Params = $"{SerializeDeSerializeHelper.Serialize(activities)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

            /// <summary>
            /// Assign Content FactSheet Detail
            /// </summary>
            /// <param name="activitiesContent"></param>
            /// <param name="language"></param>
            /// <returns></returns>
            private List<ContentProduct> ContentFactSheetDetail(List<ServiceAdapters.HB.HB.Entities.Calendar.Content> activitiesContent)
            {
                var contentProduct = new List<ContentProduct>();
                foreach (var contentItem in activitiesContent)
                {
                    var product = new ContentProduct();
                    try
                    {
                        product = new ContentProduct
                        {
                            Longitude = string.Empty,
                            Latitude = string.Empty,
                            FactsheetId = contentItem?.ContentId,
                            ProductName = contentItem?.Name,
                            Town = string.Empty,
                            Street = string.Empty,
                            Zip = string.Empty,
                            LanguagCode = contentItem?.Language,
                            DestinationCode = contentItem?.Countries?.FirstOrDefault().Destinations?.FirstOrDefault()?.Code,
                            ActivityFactsheetType = contentItem?.ActivityFactsheetType,
                            ActivityCode = contentItem?.ActivityCode,
                            Description = contentItem?.Description,
                            GuideType = contentItem?.GuidingOptions?.GuideType,
                            Included = contentItem?.GuidingOptions?.Included != null ? Convert.ToBoolean(contentItem?.GuidingOptions?.Included) : false,
                            LastUpdate = Convert.ToDateTime(contentItem?.LastUpdate)
                        };
                        contentProduct.Add(product);
                    }
                    catch (Exception ex)
                    {
                        var isangoErrorEntity = new IsangoErrorEntity
                        {
                            ClassName = "ApiTudePersistence",
                            MethodName = "ContentFactSheetDetail",
                            Params = $"{SerializeDeSerializeHelper.Serialize(product)}"
                        };
                        _log.Error(isangoErrorEntity, ex);

                    }
                }
                return contentProduct;
            }

            /// <summary>
            /// Assign Destinations
            /// </summary>
            /// <param name="activitiesContent"></param>
            /// <param name="language"></param>
            /// <returns></returns>
            private List<ContentDestination> ContentDestinations(List<ServiceAdapters.HB.HB.Entities.Calendar.Content> activitiesContent)
            {
                var contentDestination = new List<ContentDestination>();

                foreach (var contentItem in activitiesContent)
                {
                    try
                    {
                        if (contentItem != null && contentItem?.Countries != null)
                        {
                            foreach (var itemCountry in contentItem?.Countries)
                            {
                                if (itemCountry != null && itemCountry?.Destinations != null)
                                {
                                    foreach (var itemDestination in itemCountry?.Destinations)
                                    {
                                        var destination = new ContentDestination
                                        {
                                            DestinationCode = itemDestination?.Code,
                                            DestinationName = itemDestination?.Name,
                                            LanguagCode = contentItem?.Language,
                                            FactsheetId = Convert.ToInt32(contentItem?.ContentId)
                                        };
                                        contentDestination.Add(destination);
                                    }
                                }
                            }
                        }
                    }

                    catch (Exception ex)
                    {
                        var isangoErrorEntity = new IsangoErrorEntity
                        {
                            ClassName = "ApiTudePersistence",
                            MethodName = "ContentDestinations",
                            Params = $"{SerializeDeSerializeHelper.Serialize(contentItem)}"
                        };
                        _log.Error(isangoErrorEntity, ex);

                    }
                }


                return contentDestination;
            }

            /// <summary>
            /// Assign Country
            /// </summary>
            /// <param name="activitiesContent"></param>
            /// <param name="language"></param>
            /// <returns></returns>
            private List<ContentCountry> ContentCountry(List<ServiceAdapters.HB.HB.Entities.Calendar.Content> activitiesContent)
            {
                var contentCountry = new List<ContentCountry>();

                foreach (var contentItem in activitiesContent)
                {
                    try
                    {
                        if (contentItem != null && contentItem?.Countries != null)
                        {
                            foreach (var itemCountry in contentItem?.Countries)
                            {
                                if (itemCountry != null && itemCountry?.Destinations != null)
                                {
                                    foreach (var itemDestination in itemCountry?.Destinations)
                                    {
                                        var country = new ContentCountry
                                        {
                                            Countrycode = itemCountry?.Code,
                                            CountryName = itemCountry?.Name,
                                            Destinationcode = itemDestination?.Code,
                                            Factsheetid = Convert.ToInt32(contentItem?.ContentId),
                                            LanguageCode = contentItem?.Language
                                        };
                                        contentCountry.Add(country);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        var isangoErrorEntity = new IsangoErrorEntity
                        {
                            ClassName = "ApiTudePersistence",
                            MethodName = "ContentCountry",
                            Params = $"{SerializeDeSerializeHelper.Serialize(contentItem)}"
                        };
                        _log.Error(isangoErrorEntity, ex);

                    }

                }
                return contentCountry;
            }

            /// <summary>
            /// Assign Location
            /// </summary>
            /// <param name="activitiesContent"></param>
            /// <param name="language"></param>
            /// <returns></returns>
            private List<ContentLocation> ContentLocation(List<ServiceAdapters.HB.HB.Entities.Calendar.Content> activitiesContent)
            {
                var contentLocation = new List<ContentLocation>();

                foreach (var contentItem in activitiesContent)
                {
                    try
                    {
                        if (contentItem != null && contentItem.Location != null && contentItem?.Location?.StartingPoints != null)
                        {
                            foreach (var itemStartingPoint in contentItem?.Location?.StartingPoints)
                            {
                                if (itemStartingPoint != null && itemStartingPoint.MeetingPoint != null && itemStartingPoint.MeetingPoint.Country != null && itemStartingPoint?.MeetingPoint?.Country?.Destinations != null)
                                {
                                    foreach (var destination in itemStartingPoint?.MeetingPoint?.Country?.Destinations)
                                    {
                                        var country = new ContentLocation
                                        {
                                            Address = (itemStartingPoint?.MeetingPoint?.Address) ?? string.Empty,
                                            City = (destination?.Name) ?? string.Empty,
                                            CountryCode = itemStartingPoint?.MeetingPoint?.Country?.Code,
                                            CountryName = itemStartingPoint?.MeetingPoint?.Country?.Name,
                                            Description = itemStartingPoint?.MeetingPoint?.Description,
                                            Factsheetid = Convert.ToInt32(contentItem?.ContentId),
                                            Geolocation_latitude = Convert.ToString(itemStartingPoint?.MeetingPoint?.Geolocation?.Latitude),
                                            Geolocation_longitude = Convert.ToString(itemStartingPoint?.MeetingPoint?.Geolocation?.Longitude),
                                            Meetingpointtype = itemStartingPoint?.MeetingPoint?.Type,
                                            StartingPointstype = itemStartingPoint?.Type,
                                            Street = string.Empty,
                                            Zip = itemStartingPoint?.MeetingPoint?.Zip,
                                            Language = contentItem?.Language,
                                            Location_Type = "StartingPoint"
                                        };
                                        contentLocation.Add(country);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        var isangoErrorEntity = new IsangoErrorEntity
                        {
                            ClassName = "ApiTudePersistence",
                            MethodName = "ContentLocation",
                            Params = $"{SerializeDeSerializeHelper.Serialize(contentItem)}"
                        };
                        _log.Error(isangoErrorEntity, ex);

                    }
                }

                return contentLocation;
            }

            /// <summary>
            /// Assign Feature
            /// </summary>
            /// <param name="activitiesContent"></param>
            /// <param name="language"></param>
            /// <returns></returns>
            private List<ContentFeature> ContentFeature(List<ServiceAdapters.HB.HB.Entities.Calendar.Content> activitiesContent)
            {
                var contentFeature = new List<ContentFeature>();

                foreach (var contentItem in activitiesContent)
                {
                    try
                    {
                        if (contentItem != null && contentItem?.FeatureGroups != null)
                        {
                            foreach (var itemFeatureGroup in contentItem?.FeatureGroups)
                            {
                                if (itemFeatureGroup != null && itemFeatureGroup?.Included != null)
                                {
                                    foreach (var include in itemFeatureGroup?.Included)
                                    {
                                        var feature = new ContentFeature
                                        {
                                            Code = 0,
                                            Description = include?.Description,
                                            FactsheetId = Convert.ToInt32(contentItem?.ContentId),
                                            GroupCode = (itemFeatureGroup?.GroupCode) ?? string.Empty,
                                            Inc_Exc = "1",
                                            Language = (contentItem?.Language) ?? string.Empty,
                                            Name = (include?.FeatureType) ?? string.Empty
                                        };
                                        contentFeature.Add(feature);
                                    }
                                }
                                if (itemFeatureGroup != null && itemFeatureGroup?.Excluded != null)
                                {
                                    foreach (var include in itemFeatureGroup?.Excluded)
                                    {
                                        var feature = new ContentFeature
                                        {
                                            Code = 0,
                                            Description = (include?.Description) ?? string.Empty,
                                            FactsheetId = Convert.ToInt32(contentItem?.ContentId),
                                            GroupCode = (itemFeatureGroup?.GroupCode) ?? string.Empty,
                                            Inc_Exc = "0",
                                            Language = (contentItem?.Language) ?? string.Empty,
                                            Name = (include?.FeatureType) ?? string.Empty
                                        };
                                        contentFeature.Add(feature);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        var isangoErrorEntity = new IsangoErrorEntity
                        {
                            ClassName = "ApiTudePersistence",
                            MethodName = "ContentFeature",
                            Params = $"{SerializeDeSerializeHelper.Serialize(contentItem)}"
                        };
                        _log.Error(isangoErrorEntity, ex);

                    }
                }
                return contentFeature;
            }

            /// <summary>
            /// Content Media
            /// </summary>
            /// <param name="activitiesContent"></param>
            /// <returns></returns>
            private List<ContentMedia> ContentMedia(List<ServiceAdapters.HB.HB.Entities.Calendar.Content> activitiesContent)
            {
                var contentMedia = new List<ContentMedia>();

                foreach (var contentItem in activitiesContent)
                {
                    try
                    {
                        if (contentItem != null && contentItem?.Media != null)
                        {
                            if (contentItem?.Media?.Images != null)
                            {
                                foreach (var itemMedia in contentItem?.Media?.Images)
                                {
                                    if (itemMedia?.Urls != null)
                                    {
                                        foreach (var item in itemMedia.Urls)
                                        {
                                            if (item?.SizeType != null && item?.SizeType?.ToUpper() == "XLARGE")
                                            {
                                                var media = new ContentMedia
                                                {
                                                    Dpi = item.Dpi,
                                                    Duration = string.Empty,
                                                    Factsheetid = Convert.ToInt32(contentItem?.ContentId),
                                                    Height = Convert.ToInt32(item?.Height),
                                                    Image_order = Convert.ToInt32(itemMedia.VisualizationOrder),
                                                    Language = contentItem?.Language,
                                                    Mediatype = itemMedia?.MimeType,
                                                    SizeType = item?.SizeType,
                                                    Url = item?.Resource,
                                                    VisualizationOrder = Convert.ToInt32(itemMedia.VisualizationOrder),
                                                    Width = item.Width
                                                };
                                                contentMedia.Add(media);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        var isangoErrorEntity = new IsangoErrorEntity
                        {
                            ClassName = "ApiTudePersistence",
                            MethodName = "ContentMedia",
                            Params = $"{SerializeDeSerializeHelper.Serialize(contentItem)}"
                        };
                        _log.Error(isangoErrorEntity, ex);

                    }
                }
                return contentMedia;
            }

            /// <summary>
            /// Content Route
            /// </summary>
            /// <param name="activitiesContent"></param>
            /// <returns></returns>
            private List<ContentRoute> ContentRoute(List<ServiceAdapters.HB.HB.Entities.Calendar.Content> activitiesContent)
            {
                var contentRoute = new List<ContentRoute>();

                foreach (var contentItem in activitiesContent)
                {
                    try
                    {
                        if (contentItem != null && contentItem?.Routes != null)
                        {
                            foreach (var itemRoute in contentItem?.Routes)
                            {
                                if (itemRoute != null && itemRoute?.Points != null)
                                {
                                    foreach (var item in itemRoute?.Points)
                                    {
                                        var route = new ContentRoute
                                        {
                                            Address = item?.PointOfInterest?.Address,
                                            Description = item?.Description,
                                            Factsheetid = Convert.ToInt32(contentItem?.ContentId),
                                            GeoLocationLatitude = Convert.ToString(item.PointOfInterest?.Geolocation?.Latitude),
                                            GeoLocationLongitude = Convert.ToString(item.PointOfInterest?.Geolocation?.Longitude),
                                            Pointsorder = item.Order,
                                            Stop = item.Stop,
                                            Language = contentItem?.Language,
                                            City = item.PointOfInterest?.City,
                                            Type = item.PointOfInterest?.Type,
                                            Country = item.PointOfInterest?.Country?.Code,
                                            Duration = (itemRoute?.Duration != null) ? (itemRoute?.Duration?.Value + " " + itemRoute?.Duration?.Metric) : string.Empty,
                                            TimeFrom = itemRoute?.TimeFrom ?? string.Empty,
                                            TimeTo = itemRoute?.TimeTo ?? string.Empty,
                                            Zip = item?.PointOfInterest?.Zip ?? string.Empty,
                                            POI_description2 = item?.PointOfInterest?.Description
                                        };
                                        contentRoute.Add(route);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        var isangoErrorEntity = new IsangoErrorEntity
                        {
                            ClassName = "ApiTudePersistence",
                            MethodName = "ContentRoute",
                            Params = $"{SerializeDeSerializeHelper.Serialize(contentItem)}"
                        };
                        _log.Error(isangoErrorEntity, ex);

                    }
                }
                return contentRoute;
            }

            private List<ContentRedeeminfo> ContentRedeeminfo(List<ServiceAdapters.HB.HB.Entities.Calendar.Content> activitiesContent)
            {
                var redeem = new List<ContentRedeeminfo>();

                foreach (var contentItem in activitiesContent)
                {
                    try
                    {
                        if (contentItem != null && contentItem?.RedeemInfo != null)
                        {
                            foreach (var item in contentItem?.RedeemInfo?.Comments)
                            {
                                var redeeminfo = new ContentRedeeminfo
                                {
                                    Comments = item?.Description,
                                    DirectEntrance = (contentItem?.RedeemInfo?.DirectEntrance != null) ? Convert.ToBoolean(contentItem?.RedeemInfo?.DirectEntrance) : false,
                                    Factsheetid = Convert.ToInt32(contentItem?.ContentId),
                                    Language = contentItem?.Language,
                                    Type = contentItem?.RedeemInfo?.Type
                                };
                                redeem.Add(redeeminfo);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        var isangoErrorEntity = new IsangoErrorEntity
                        {
                            ClassName = "ApiTudePersistence",
                            MethodName = "ContentRedeeminfo",
                            Params = $"{SerializeDeSerializeHelper.Serialize(contentItem)}"
                        };
                        _log.Error(isangoErrorEntity, ex);

                    }
                }

                return redeem;
            }

            /// <summary>
            /// Content Detail Info
            /// </summary>
            /// <param name="activitiesContent"></param>
            /// <returns></returns>
            private List<ContentDescription> ContentDescription(List<ServiceAdapters.HB.HB.Entities.Calendar.Content> activitiesContent)
            {
                var contentDetailInfo = new List<ContentDescription>();

                foreach (var contentItem in activitiesContent)
                {
                    try
                    {
                        var advancedTips = String.Empty;
                        var highLights = String.Empty;
                        var importantInfo = String.Empty;
                        var detailedInfo = String.Empty;
                        if (contentItem.AdvancedTips != null)
                        {
                            advancedTips = string.Join("-", Array.ConvertAll<object, string>(contentItem?.AdvancedTips, Convert.ToString));
                        }

                        if (contentItem?.ImportantInfo != null)
                        {
                            importantInfo = string.Join(" ", Array.ConvertAll<object, string>(contentItem?.ImportantInfo, Convert.ToString));
                        }

                        if (contentItem?.DetailedInfo != null)
                        {
                            detailedInfo = string.Join(" ", Array.ConvertAll<object, string>(contentItem?.DetailedInfo, Convert.ToString));
                        }

                        var detail = new ContentDescription
                        {
                            FactsheetID = Convert.ToInt32(contentItem.ContentId),
                            DetailedInfo = detailedInfo ?? string.Empty,
                            Summary = (contentItem?.Summary) ?? string.Empty,
                            ImportantInfo = importantInfo ?? string.Empty,
                            Otherinfo = string.Empty,
                            AdvancedTips = advancedTips ?? string.Empty,
                            Language = (contentItem?.Language) ?? string.Empty,
                        };
                        contentDetailInfo.Add(detail);
                    }
                    catch (Exception ex)
                    {
                        var isangoErrorEntity = new IsangoErrorEntity
                        {
                            ClassName = "ApiTudePersistence",
                            MethodName = "ContentDescription",
                            Params = $"{SerializeDeSerializeHelper.Serialize(contentItem)}"
                        };
                        _log.Error(isangoErrorEntity, ex);

                    }
                }
                return contentDetailInfo;
            }

            private List<ContentScheduling> ContentScheduling(List<ServiceAdapters.HB.HB.Entities.Calendar.Content> activitiesContent)
            {
                var contentScheduling = new List<ContentScheduling>();

                foreach (var contentItem in activitiesContent)
                {
                    try
                    {
                        if (contentItem?.Scheduling != null && contentItem?.Scheduling?.Opened != null
                            && contentItem?.Scheduling?.Opened.Count > 0)
                        {
                            foreach (var scheduleItem in contentItem?.Scheduling?.Opened)
                            {
                                var schedule = new ContentScheduling
                                {
                                    Factsheetid = Convert.ToInt32(contentItem?.ContentId),
                                    OpeningTime = (scheduleItem?.OpeningTime) ?? string.Empty,
                                    CloseTime = (scheduleItem?.CloseTime) ?? string.Empty,
                                    Language = (contentItem?.Language) ?? string.Empty,
                                    ScheduleType = string.Empty,
                                    WeekDays = (scheduleItem?.WeekDays != null ? string.Join(",", scheduleItem?.WeekDays) : string.Empty),
                                    Duration = (contentItem?.Scheduling?.Duration != null) ? (contentItem?.Scheduling?.Duration?.Value + " " + contentItem?.Scheduling?.Duration?.Metric) : string.Empty
                                };
                                contentScheduling.Add(schedule);
                            }
                        }
                        else if (contentItem?.Scheduling != null && contentItem?.Scheduling?.Duration != null)
                        {
                            var schedule = new ContentScheduling
                            {
                                Factsheetid = Convert.ToInt32(contentItem?.ContentId),
                                OpeningTime = string.Empty,
                                CloseTime = string.Empty,
                                Language = (contentItem?.Language) ?? string.Empty,
                                ScheduleType = string.Empty,
                                WeekDays = string.Empty,
                                Duration = (contentItem?.Scheduling?.Duration != null) ? (contentItem?.Scheduling?.Duration?.Value + " " + contentItem?.Scheduling?.Duration?.Metric) : string.Empty
                            };
                            contentScheduling.Add(schedule);
                        }
                    }
                    catch (Exception ex)
                    {
                        var isangoErrorEntity = new IsangoErrorEntity
                        {
                            ClassName = "ApiTudePersistence",
                            MethodName = "ContentScheduling",
                            Params = $"{SerializeDeSerializeHelper.Serialize(contentItem)}"
                        };
                        _log.Error(isangoErrorEntity, ex);

                    }
                }
                return contentScheduling;
            }

            private List<ContentHighLight> ContentHighLights(List<ServiceAdapters.HB.HB.Entities.Calendar.Content> activitiesContent)
            {
                var contentHighLights = new List<ContentHighLight>();

                if (activitiesContent != null && activitiesContent.Count > 0)
                {
                    foreach (var contentItem in activitiesContent)
                    {
                        try
                        {
                            if (contentItem?.Highligths != null && contentItem?.Highligths?.Count > 0)
                            {
                                for (var highLightItem = 0; highLightItem < contentItem?.Highligths?.Count; highLightItem++)
                                {
                                    if (contentItem?.Highligths[highLightItem] != null)
                                    {
                                        var highLight = new ContentHighLight
                                        {
                                            Factsheetid = Convert.ToInt32(contentItem.ContentId),
                                            ActivityCode = (contentItem?.ActivityCode) ?? string.Empty,
                                            HighLightText = contentItem?.Highligths[highLightItem] ?? string.Empty,
                                            Orders = (highLightItem + 1),
                                            Language = (contentItem?.Language) ?? string.Empty
                                        };
                                        contentHighLights.Add(highLight);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            var isangoErrorEntity = new IsangoErrorEntity
                            {
                                ClassName = "ApiTudePersistence",
                                MethodName = "ContentHighLights",
                                Params = $"{SerializeDeSerializeHelper.Serialize(contentItem)}"
                            };
                            _log.Error(isangoErrorEntity, ex);

                        }
                    }
                }
                return contentHighLights;
            }

            private List<ContentOperationalDays> ContentOperationalDays(List<ServiceAdapters.HB.HB.Entities.Calendar.Activity> activities)
            {
                var contentOperationalDays = new List<ContentOperationalDays>();

                if (activities != null && activities.Count > 0)
                {
                    foreach (var activityItem in activities)
                    {
                        try
                        {
                            if (activityItem?.OperationDays != null && activityItem?.OperationDays?.Count > 0)
                            {
                                foreach (var itemOperational in activityItem?.OperationDays)
                                {
                                    var days = new ContentOperationalDays
                                    {
                                        Factsheetid = Convert.ToInt32(activityItem?.Content?.ContentId),
                                        ActivityCode = (activityItem?.ActivityCode) ?? string.Empty,
                                        Code = (itemOperational?.Code) ?? string.Empty,
                                        Name = (itemOperational?.Name) ?? string.Empty,
                                        Language = (activityItem?.Language) ?? string.Empty
                                    };
                                    contentOperationalDays.Add(days);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            var isangoErrorEntity = new IsangoErrorEntity
                            {
                                ClassName = "ApiTudePersistence",
                                MethodName = "ContentOperationalDays",
                                Params = $"{SerializeDeSerializeHelper.Serialize(activityItem)}"
                            };
                            _log.Error(isangoErrorEntity, ex);

                        }
                    }
                }
                return contentOperationalDays;
            }

            private List<ContentDuration> ContentDuration(List<ServiceAdapters.HB.HB.Entities.Calendar.Activity> activities)
            {
                var contentDuration = new List<ContentDuration>();

                if (activities != null && activities.Count > 0)
                {
                    foreach (var activityItem in activities)
                    {
                        if (activityItem?.Modalities != null && activityItem?.Modalities?.Count > 0)
                        {
                            try
                            {
                                foreach (var itemModality in activityItem?.Modalities)
                                {
                                    var duration = new ContentDuration
                                    {
                                        Factsheetid = Convert.ToInt32(activityItem?.Content?.ContentId),
                                        ActivityCode = (activityItem?.ActivityCode) ?? string.Empty,
                                        ModalityCode = (itemModality?.Code) ?? string.Empty,
                                        DurationValue = Convert.ToDecimal(itemModality?.Duration?.Value),
                                        DurationMetric = (itemModality?.Duration?.Metric) ?? string.Empty,
                                        Language = (activityItem?.Language) ?? string.Empty
                                    };
                                    contentDuration.Add(duration);
                                }
                            }
                            catch (Exception ex)
                            {
                                var isangoErrorEntity = new IsangoErrorEntity
                                {
                                    ClassName = "ApiTudePersistence",
                                    MethodName = "ContentDuration",
                                    Params = $"{SerializeDeSerializeHelper.Serialize(activityItem)}"
                                };
                                _log.Error(isangoErrorEntity, ex);

                            }
                        }
                    }
                }
                return contentDuration;
            }

            private List<ContentLastUpdated> GetContentFactSheetDetail()
            {
                var mappings = new List<ContentLastUpdated>();
                try
                {
                    using (var dbCommand = APIUploadDb.GetStoredProcCommand(Constant.GetProductFactSheet))
                    {
                        dbCommand.CommandType = CommandType.StoredProcedure;
                        using (var reader = APIUploadDb.ExecuteReader(dbCommand))
                        {
                            var contentLastUpdated = new ContentLastUpdated();
                            while (reader.Read())
                            {
                                mappings.Add(GetContentLastUpdatedData(reader));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    var isangoErrorEntity = new IsangoErrorEntity
                    {
                        ClassName = "ApiTudePersistence",
                        MethodName = "GetContentFactSheetDetail",
                    };
                    _log.Error(isangoErrorEntity, ex);
                    throw;
                }
                return mappings;
            }

            public ContentLastUpdated GetContentLastUpdatedData(IDataReader reader)
            {
                var mapping = new ContentLastUpdated();
                try
                {
                    mapping = new ContentLastUpdated
                    {
                        FactSheetId = DbPropertyHelper.Int32PropertyFromRow(reader, "FactsheetId"),
                        LanguageCode = DbPropertyHelper.StringPropertyFromRow(reader, "LanguagCode"),
                        Lastupdate = DbPropertyHelper.DateTimePropertyFromRow(reader, "lastupdate")
                    };
                }
                catch (Exception ex)
                {
                    var isangoErrorEntity = new IsangoErrorEntity
                    {
                        ClassName = "ApiTudePersistence",
                        MethodName = "GetContentLastUpdatedData",
                    };
                    _log.Error(isangoErrorEntity, ex);
                    throw;
                }
                return mapping;
            }

            /// <summary>
            /// Save ApiTude Content in the database
            /// </summary>
            /// <param name="ageGroups"></param>
            private void SaveApiTudeContentFactSheetDetail(List<ContentProduct> contentProduct)
            {
                try
                {
                    if (contentProduct != null && contentProduct.Count > 0)
                    {
                        var contentFactSheetDataTable = SetContent(contentProduct, "P");

                        using (var connection = new SqlConnection(ConfigurationManagerHelper.GetValuefromConfig(Constant.APIUploadDb)))
                        {
                            var insertCommand = new SqlCommand(Constant.InsertApiTudeProductFactSheetdetail, connection)
                            {
                                CommandType = CommandType.StoredProcedure
                            };
                            var tvpFactSheet = insertCommand.Parameters.AddWithValue(Constant.HBTProductFactSheetParameter, contentFactSheetDataTable);
                            tvpFactSheet.SqlDbType = SqlDbType.Structured;

                            try
                            {
                                connection.Open();
                                insertCommand.ExecuteNonQuery();
                            }
                            finally
                            {
                                connection.Close();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    var isangoErrorEntity = new IsangoErrorEntity
                    {
                        ClassName = "ApiTudePersistence",
                        MethodName = "SaveApiTudeContentFactSheetDetail",
                    };
                    _log.Error(isangoErrorEntity, ex);
                    throw;
                }

            }

            /// <summary>
            /// Save ApiTude Content in the database
            /// </summary>
            /// <param name="ageGroups"></param>
            public void SaveApiTudeContentDestination(List<ContentDestination> contentDestination)
            {
                try
                {
                    if (contentDestination != null && contentDestination.Count > 0)
                    {
                        var contentDestinationDataTable = SetContent(contentDestination, "D");

                        using (var connection = new SqlConnection(ConfigurationManagerHelper.GetValuefromConfig(Constant.APIUploadDb)))
                        {
                            var insertCommand = new SqlCommand(Constant.InsertDestinationDetail, connection)
                            {
                                CommandType = CommandType.StoredProcedure
                            };
                            var tvpFactSheet = insertCommand.Parameters.AddWithValue(Constant.HBTDestinationParameter, contentDestinationDataTable);
                            tvpFactSheet.SqlDbType = SqlDbType.Structured;

                            try
                            {
                                connection.Open();
                                insertCommand.ExecuteNonQuery();
                            }
                            finally
                            {
                                connection.Close();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    var isangoErrorEntity = new IsangoErrorEntity
                    {
                        ClassName = "ApiTudePersistence",
                        MethodName = "SaveApiTudeContentDestination",
                    };
                    _log.Error(isangoErrorEntity, ex);
                    throw;
                }
            }

            /// <summary>
            /// Save ApiTude Content in the database
            /// </summary>
            /// <param name="ageGroups"></param>
            public void SaveApiTudeContentCountry(List<ContentCountry> contentCountry)
            {
                try
                {
                    if (contentCountry != null && contentCountry.Count > 0)
                    {
                        var contentCountryDataTable = SetContent(contentCountry, "C");

                        using (var connection = new SqlConnection(ConfigurationManagerHelper.GetValuefromConfig(Constant.APIUploadDb)))
                        {
                            var insertCommand = new SqlCommand(Constant.InsertCountry, connection)
                            {
                                CommandType = CommandType.StoredProcedure
                            };
                            var tvpFactSheet = insertCommand.Parameters.AddWithValue(Constant.HBTCountryParameter, contentCountryDataTable);
                            tvpFactSheet.SqlDbType = SqlDbType.Structured;

                            try
                            {
                                connection.Open();
                                insertCommand.ExecuteNonQuery();
                            }
                            finally
                            {
                                connection.Close();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    var isangoErrorEntity = new IsangoErrorEntity
                    {
                        ClassName = "ApiTudePersistence",
                        MethodName = "SaveApiTudeContentCountry",
                    };
                    _log.Error(isangoErrorEntity, ex);
                    throw;
                }
            }

            /// <summary>
            /// Save ApiTude Content in the database
            /// </summary>
            /// <param name="ageGroups"></param>
            public void SaveApiTudeContentLocation(List<ContentLocation> contentLocation)
            {
                try
                {
                    if (contentLocation != null && contentLocation.Count > 0)
                    {
                        var contentLocationDataTable = SetContent(contentLocation, "L");

                        using (var connection = new SqlConnection(ConfigurationManagerHelper.GetValuefromConfig(Constant.APIUploadDb)))
                        {
                            var insertCommand = new SqlCommand(Constant.InsertLocation, connection)
                            {
                                CommandType = CommandType.StoredProcedure
                            };
                            var tvpFactSheet = insertCommand.Parameters.AddWithValue(Constant.HBTLocationParameter, contentLocationDataTable);
                            tvpFactSheet.SqlDbType = SqlDbType.Structured;

                            try
                            {
                                connection.Open();
                                insertCommand.ExecuteNonQuery();
                            }
                            finally
                            {
                                connection.Close();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    var isangoErrorEntity = new IsangoErrorEntity
                    {
                        ClassName = "ApiTudePersistence",
                        MethodName = "SaveApiTudeContentLocation",
                    };
                    _log.Error(isangoErrorEntity, ex);
                    throw;
                }
            }

            public void SaveApiTudeContentFeature(List<ContentFeature> contentFeature)
            {
                try
                {
                    if (contentFeature != null && contentFeature.Count > 0)
                    {
                        var contentFeatureDataTable = SetContent(contentFeature, "F");

                        using (var connection = new SqlConnection(ConfigurationManagerHelper.GetValuefromConfig(Constant.APIUploadDb)))
                        {
                            var insertCommand = new SqlCommand(Constant.InsertFeature, connection)
                            {
                                CommandType = CommandType.StoredProcedure
                            };
                            var tvpFactSheet = insertCommand.Parameters.AddWithValue(Constant.HBTFeatureParameter, contentFeatureDataTable);
                            tvpFactSheet.SqlDbType = SqlDbType.Structured;

                            try
                            {
                                connection.Open();
                                insertCommand.ExecuteNonQuery();
                            }
                            finally
                            {
                                connection.Close();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    var isangoErrorEntity = new IsangoErrorEntity
                    {
                        ClassName = "ApiTudePersistence",
                        MethodName = "SaveApiTudeContentFeature",
                    };
                    _log.Error(isangoErrorEntity, ex);
                    throw;
                }
            }

            public void SaveApiTudeContentMedia(List<ContentMedia> contentMedia)
            {
                try
                {
                    if (contentMedia != null && contentMedia.Count > 0)
                    {
                        var contentMediaDataTable = SetContent(contentMedia, "M");

                        using (var connection = new SqlConnection(ConfigurationManagerHelper.GetValuefromConfig(Constant.APIUploadDb)))
                        {
                            var insertCommand = new SqlCommand(Constant.InsertMedia, connection)
                            {
                                CommandType = CommandType.StoredProcedure
                            };
                            var tvpFactSheet = insertCommand.Parameters.AddWithValue(Constant.HBTMediaParameter, contentMediaDataTable);
                            tvpFactSheet.SqlDbType = SqlDbType.Structured;

                            try
                            {
                                connection.Open();
                                insertCommand.ExecuteNonQuery();
                            }
                            finally
                            {
                                connection.Close();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    var isangoErrorEntity = new IsangoErrorEntity
                    {
                        ClassName = "ApiTudePersistence",
                        MethodName = "SaveApiTudeContentMedia",
                    };
                    _log.Error(isangoErrorEntity, ex);
                    throw;
                }
            }

            public void SaveApiTudeMediImages(List<ContentMedia> contentMedia)
            {
                try
                {
                    if (contentMedia != null && contentMedia.Count > 0)
                    {
                        var dataTable = new DataTable { TableName = "dbo.APIMedia" };

                        dataTable.Columns.Add("APIproductcode", typeof(string));
                        dataTable.Columns.Add("isangoServiceid", typeof(int));
                        dataTable.Columns.Add("Apitypeid", typeof(int));
                        dataTable.Columns.Add("mediatype", typeof(string));
                        dataTable.Columns.Add("dpi", typeof(int));
                        dataTable.Columns.Add("height", typeof(int));
                        dataTable.Columns.Add("width", typeof(int));
                        dataTable.Columns.Add("language", typeof(string));
                        dataTable.Columns.Add("sizeType", typeof(string));
                        dataTable.Columns.Add("Image_order", typeof(int));
                        dataTable.Columns.Add("VisualizationOrder", typeof(int));
                        dataTable.Columns.Add("Url", typeof(string));
                        dataTable.Columns.Add("duration", typeof(string));
                        dataTable.Columns.Add("Cloudinaryurl", typeof(string));

                        foreach (var Media in contentMedia.Where(x => x.Language.ToLower() == "en"))
                        {
                            var dataTableRow = dataTable.NewRow();
                            dataTableRow["APIproductcode"] = Media.Factsheetid;
                            dataTableRow["isangoServiceid"] = 0;
                            dataTableRow["Apitypeid"] = 3;
                            dataTableRow["mediatype"] = Media.Mediatype;
                            dataTableRow["dpi"] = Media.Dpi;
                            dataTableRow["height"] = Media.Height;
                            dataTableRow["width"] = Media.Width;
                            dataTableRow["language"] = Media.Language;
                            dataTableRow["sizeType"] = Media.SizeType;
                            dataTableRow["Image_order"] = Media.Image_order;
                            dataTableRow["VisualizationOrder"] = Media.VisualizationOrder;
                            dataTableRow["Url"] = Media.Url;
                            dataTableRow["duration"] = Media.Duration;
                            dataTableRow["Cloudinaryurl"] = String.Empty;

                            dataTable.Rows.Add(dataTableRow);
                        }

                        using (var connection = new SqlConnection(ConfigurationManagerHelper.GetValuefromConfig(Constant.APIUploadDb)))
                        {
                            var insertCommand = new SqlCommand(Constant.InsertAPIImages, connection)
                            {
                                CommandType = CommandType.StoredProcedure
                            };
                            var tvpFactSheet = insertCommand.Parameters.AddWithValue(Constant.APIMediaParameter, dataTable);
                            tvpFactSheet.SqlDbType = SqlDbType.Structured;

                            try
                            {
                                connection.Open();
                                insertCommand.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {
                                //ignore
                            }
                            finally
                            {
                                connection.Close();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    var isangoErrorEntity = new IsangoErrorEntity
                    {
                        ClassName = "ApiTudePersistence",
                        MethodName = "SaveApiTudeMediImages",
                    };
                    _log.Error(isangoErrorEntity, ex);
                    throw;
                }
            }

            public void SaveApiTudeContentRoute(List<ContentRoute> contentRoute)
            {
                try
                {
                    if (contentRoute != null && contentRoute.Count > 0)
                    {
                        var contentRouteDataTable = SetContent(contentRoute, "R");

                        using (var connection = new SqlConnection(ConfigurationManagerHelper.GetValuefromConfig(Constant.APIUploadDb)))
                        {
                            var insertCommand = new SqlCommand(Constant.InsertRoute, connection)
                            {
                                CommandType = CommandType.StoredProcedure
                            };
                            var tvpFactSheet = insertCommand.Parameters.AddWithValue(Constant.HBTRouteParameter, contentRouteDataTable);
                            tvpFactSheet.SqlDbType = SqlDbType.Structured;

                            try
                            {
                                connection.Open();
                                insertCommand.ExecuteNonQuery();
                            }
                            finally
                            {
                                connection.Close();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    var isangoErrorEntity = new IsangoErrorEntity
                    {
                        ClassName = "ApiTudePersistence",
                        MethodName = "SaveApiTudeContentRoute",
                    };
                    _log.Error(isangoErrorEntity, ex);
                    throw;
                }
            }

            public void SaveApiTudeContentRedeemInfo(List<ContentRedeeminfo> contentRedeeminfo)
            {
                try
                {
                    if (contentRedeeminfo != null && contentRedeeminfo.Count > 0)
                    {
                        var contentRedeeminfoDataTable = SetContent(contentRedeeminfo, "I");

                        using (var connection = new SqlConnection(ConfigurationManagerHelper.GetValuefromConfig(Constant.APIUploadDb)))
                        {
                            var insertCommand = new SqlCommand(Constant.InsertRedeemInfo, connection)
                            {
                                CommandType = CommandType.StoredProcedure
                            };
                            var tvpFactSheet = insertCommand.Parameters.AddWithValue(Constant.HBTRedeeminfoParameter, contentRedeeminfoDataTable);
                            tvpFactSheet.SqlDbType = SqlDbType.Structured;

                            try
                            {
                                connection.Open();
                                insertCommand.ExecuteNonQuery();
                            }
                            finally
                            {
                                connection.Close();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    var isangoErrorEntity = new IsangoErrorEntity
                    {
                        ClassName = "ApiTudePersistence",
                        MethodName = "SaveApiTudeContentRedeemInfo",
                    };
                    _log.Error(isangoErrorEntity, ex);
                    throw;
                }
            }

            public void SaveApiTudeContentDescription(List<ContentDescription> contentRoute)
            {
                try
                {
                    if (contentRoute != null && contentRoute.Count > 0)
                    {
                        var contentRouteDataTable = SetContent(contentRoute, "S");

                        using (var connection = new SqlConnection(ConfigurationManagerHelper.GetValuefromConfig(Constant.APIUploadDb)))
                        {
                            var insertCommand = new SqlCommand(Constant.InsertDescription, connection)
                            {
                                CommandType = CommandType.StoredProcedure
                            };
                            var tvpFactSheet = insertCommand.Parameters.AddWithValue(Constant.HBTDescritionParameter, contentRouteDataTable);
                            tvpFactSheet.SqlDbType = SqlDbType.Structured;

                            try
                            {
                                connection.Open();
                                insertCommand.ExecuteNonQuery();
                            }
                            finally
                            {
                                connection.Close();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    var isangoErrorEntity = new IsangoErrorEntity
                    {
                        ClassName = "ApiTudePersistence",
                        MethodName = "SaveApiTudeContentDescription",
                    };
                    _log.Error(isangoErrorEntity, ex);
                    throw;
                }
            }

            public void SaveApiTudeContentScheduling(List<ContentScheduling> contentScheduling)
            {
                try
                {
                    if (contentScheduling != null && contentScheduling.Count > 0)
                    {
                        var contentSchedulingDataTable = SetContent(contentScheduling, "E");

                        using (var connection = new SqlConnection(ConfigurationManagerHelper.GetValuefromConfig(Constant.APIUploadDb)))
                        {
                            var insertCommand = new SqlCommand(Constant.InsertScheduling, connection)
                            {
                                CommandType = CommandType.StoredProcedure
                            };
                            var tvpFactSheet = insertCommand.Parameters.AddWithValue(Constant.HBTSchedulingParameter, contentSchedulingDataTable);
                            tvpFactSheet.SqlDbType = SqlDbType.Structured;

                            try
                            {
                                connection.Open();
                                insertCommand.ExecuteNonQuery();
                            }
                            finally
                            {
                                connection.Close();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    var isangoErrorEntity = new IsangoErrorEntity
                    {
                        ClassName = "ApiTudePersistence",
                        MethodName = "SaveApiTudeContentScheduling",
                    };
                    _log.Error(isangoErrorEntity, ex);
                    throw;
                }
            }

            public void SaveApiTudeContentDuration(List<ContentDuration> contentDuration)
            {
                try
                {
                    if (contentDuration != null && contentDuration.Count > 0)
                    {
                        var contentDurationDataTable = SetContent(contentDuration, "Dt");

                        using (var connection = new SqlConnection(ConfigurationManagerHelper.GetValuefromConfig(Constant.APIUploadDb)))
                        {
                            var insertCommand = new SqlCommand(Constant.InsertDuration, connection)
                            {
                                CommandType = CommandType.StoredProcedure
                            };
                            var tvpFactSheet = insertCommand.Parameters.AddWithValue(Constant.HBTDurationParameter, contentDurationDataTable);
                            tvpFactSheet.SqlDbType = SqlDbType.Structured;

                            try
                            {
                                connection.Open();
                                insertCommand.ExecuteNonQuery();
                            }
                            finally
                            {
                                connection.Close();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    var isangoErrorEntity = new IsangoErrorEntity
                    {
                        ClassName = "ApiTudePersistence",
                        MethodName = "SaveApiTudeContentDuration",
                    };
                    _log.Error(isangoErrorEntity, ex);
                    throw;
                }
            }

            public void SaveApiTudeContentHighLights(List<ContentHighLight> contentHighLight)
            {
                try
                {
                    if (contentHighLight != null && contentHighLight.Count > 0)
                    {
                        var contentHighLightDataTable = SetContent(contentHighLight, "Ht");

                        using (var connection = new SqlConnection(ConfigurationManagerHelper.GetValuefromConfig(Constant.APIUploadDb)))
                        {
                            var insertCommand = new SqlCommand(Constant.InsertHighLights, connection)
                            {
                                CommandType = CommandType.StoredProcedure
                            };
                            var tvpFactSheet = insertCommand.Parameters.AddWithValue(Constant.HBTHighLightsParameter, contentHighLightDataTable);
                            tvpFactSheet.SqlDbType = SqlDbType.Structured;

                            try
                            {
                                connection.Open();
                                insertCommand.ExecuteNonQuery();
                            }
                            finally
                            {
                                connection.Close();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    var isangoErrorEntity = new IsangoErrorEntity
                    {
                        ClassName = "ApiTudePersistence",
                        MethodName = "SaveApiTudeContentHighLights",
                    };
                    _log.Error(isangoErrorEntity, ex);
                    throw;
                }
            }

            public void SaveApiTudeContentOperationalDays(List<ContentOperationalDays> contentOperationalDays)
            {
                try
                {
                    if (contentOperationalDays != null && contentOperationalDays.Count > 0)
                    {
                        var contentOpeartionalDataTable = SetContent(contentOperationalDays, "Od");

                        using (var connection = new SqlConnection(ConfigurationManagerHelper.GetValuefromConfig(Constant.APIUploadDb)))
                        {
                            var insertCommand = new SqlCommand(Constant.InsertOperationalDays, connection)
                            {
                                CommandType = CommandType.StoredProcedure
                            };
                            var tvpFactSheet = insertCommand.Parameters.AddWithValue(Constant.HBTOperationalDaysParameter, contentOpeartionalDataTable);
                            tvpFactSheet.SqlDbType = SqlDbType.Structured;

                            try
                            {
                                connection.Open();
                                insertCommand.ExecuteNonQuery();
                            }
                            finally
                            {
                                connection.Close();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    var isangoErrorEntity = new IsangoErrorEntity
                    {
                        ClassName = "ApiTudePersistence",
                        MethodName = "SaveApiTudeContentOperationalDays",
                    };
                    _log.Error(isangoErrorEntity, ex);
                    throw;
                }
            }

            /// <summary>
            /// Prepare datatable for Age Group
            /// </summary>
            /// <param name="ageGroups"></param>
            /// <returns></returns>
            private DataTable SetAgeGroup(List<ApiTudeAgeGroup> ageGroups)
            {
                var dataTable = new DataTable { TableName = Constant.HBTServiceAgeGroup };
                try
                {
                    if (ageGroups != null && ageGroups.Count > 0)
                    {
                        foreach (var property in ageGroups[0].GetType().GetProperties())
                        {
                            dataTable.Columns.Add(new DataColumn(property.Name, property.PropertyType));
                        }

                        foreach (var ageGroup in ageGroups)
                        {
                            var newRow = dataTable.NewRow();
                            foreach (var property in ageGroup.GetType().GetProperties())
                            {
                                newRow[property.Name] = ageGroup.GetType().GetProperty(property.Name)
                                    ?.GetValue(ageGroup, null);
                            }
                            dataTable.Rows.Add(newRow);
                        }
                    }
                }
                catch (Exception ex)
                {
                    var isangoErrorEntity = new IsangoErrorEntity
                    {
                        ClassName = "ApiTudePersistence",
                        MethodName = "SetAgeGroup",
                    };
                    _log.Error(isangoErrorEntity, ex);
                    throw;
                }
                return dataTable;
            }


            private DataTable SetContent<T>(List<T> content, string type)
            {
                var datatableName = string.Empty;
                try
                {
                    switch (type)
                    {
                        case "P":
                            datatableName = Constant.HBTProductFactSheet;
                            break;

                        case "D":
                            datatableName = Constant.HBTDestination;
                            break;

                        case "C":
                            datatableName = Constant.HBTCountry;
                            break;

                        case "L":
                            datatableName = Constant.HBTLocation;
                            break;

                        case "F":
                            datatableName = Constant.HBTFeature;
                            break;

                        case "M":
                            datatableName = Constant.HBTMedia;
                            break;

                        case "R":
                            datatableName = Constant.HBTRoute;
                            break;

                        case "S":
                            datatableName = Constant.HBTDescriptionInfo;
                            break;

                        case "I":
                            datatableName = Constant.HBTRedeeminfo;
                            break;

                        case "E":
                            datatableName = Constant.HBTScheduling;
                            break;

                        case "Dt":
                            datatableName = Constant.HBTDuration;
                            break;

                        case "Ht":
                            datatableName = Constant.HBTHighLights;
                            break;

                        case "Od":
                            datatableName = Constant.HBTOperationalDays;
                            break;
                    }
                    var dataTable = new DataTable { TableName = datatableName };

                    foreach (var property in content[0].GetType().GetProperties())
                    {
                        dataTable.Columns.Add(new DataColumn(property.Name, property.PropertyType));
                    }

                    foreach (var activities in content)
                    {
                        var newRow = dataTable.NewRow();
                        foreach (var property in activities.GetType().GetProperties())
                        {
                            newRow[property.Name] = activities.GetType().GetProperty(property.Name)
                                ?.GetValue(activities, null);
                        }
                        dataTable.Rows.Add(newRow);
                    }
                    return dataTable;
                }
                catch (Exception ex)
                {
                    var isangoErrorEntity = new IsangoErrorEntity
                    {
                        ClassName = "ApiTudePersistence",
                        MethodName = "SetContent",
                    };
                    _log.Error(isangoErrorEntity, ex);
                    throw;
                }
            }
        }
    }
