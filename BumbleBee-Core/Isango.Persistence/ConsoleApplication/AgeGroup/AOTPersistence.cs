using Isango.Entities;
using Isango.Entities.ConsoleApplication.AgeGroup.AOT;
using Isango.Persistence.Contract;
using Logger.Contract;
using System;
using System.Data;
using System.Linq;
using Util;
using Constant = Isango.Persistence.Constants.Constants;

namespace Isango.Persistence.ConsoleApplication.AgeGroup
{
    public class AOTPersistence : PersistenceBase, IAOTPersistence
    {
        private readonly ILogger _log;
        public AOTPersistence(ILogger log)
        {
            _log = log;
        }
        /// <summary>
        /// Save all activity age groups mapping
        /// </summary>
        /// <param name="activityAgeGroups"></param>
        /// <param name="cancellationPolicy"></param>
        public void SaveAllActivityAgeGroupsMapping(OptionGeneralInfoResponse activityAgeGroups, string cancellationPolicy)
        {
            try
            {
                if (activityAgeGroups?.OptGeneralInfo?.Count > 0)
                {
                    var information = activityAgeGroups.OptGeneralInfo.FirstOrDefault();
                    if (information == null) return;
                    var images = SerializeDeSerializeHelper.Serialize(information.Images);
                    var optExtras = SerializeDeSerializeHelper.Serialize(information.OptExtras);

                    using (var command = APIUploadDb.GetStoredProcCommand(Constant.InsertAOTGeneralInfoSp))
                    {
                        APIUploadDb.AddInParameter(command, Constant.ServiceCode, DbType.String, information.Opt);
                        APIUploadDb.AddInParameter(command, Constant.OptionName, DbType.String, information.Name);
                        APIUploadDb.AddInParameter(command, Constant.SupplierCode, DbType.String, information.SupplierCode);
                        APIUploadDb.AddInParameter(command, Constant.SupplierName, DbType.String, information.SupplierName);
                        APIUploadDb.AddInParameter(command, Constant.Comment, DbType.String, information.Comment);
                        APIUploadDb.AddInParameter(command, Constant.BeddingConfiguration, DbType.String, information.BeddingConfiguration);
                        APIUploadDb.AddInParameter(command, Constant.MaxPaxDesc, DbType.String, information.MaxPaxDesc);
                        APIUploadDb.AddInParameter(command, Constant.LocationId, DbType.String, information.LocationId);
                        APIUploadDb.AddInParameter(command, Constant.LocationName, DbType.String, information.LocationName);
                        APIUploadDb.AddInParameter(command, Constant.MinChildAge, DbType.String, information.MinChildAge);
                        APIUploadDb.AddInParameter(command, Constant.MaxChildAge, DbType.String, information.MaxChildAge);
                        APIUploadDb.AddInParameter(command, Constant.ChildPolicyDescription, DbType.String, information.ChildPolicyDescription);
                        APIUploadDb.AddInParameter(command, Constant.InfantAgeFrom, DbType.String, information.InfantAgeFrom);
                        APIUploadDb.AddInParameter(command, Constant.InfantAgeTo, DbType.String, information.InfantAgeTo);
                        APIUploadDb.AddInParameter(command, Constant.ChildAgeFrom, DbType.String, information.ChildAgeFrom);
                        APIUploadDb.AddInParameter(command, Constant.ChildAgeTo, DbType.String, information.ChildAgeTo);
                        APIUploadDb.AddInParameter(command, Constant.AdultAgeFrom, DbType.String, information.AdultAgeFrom);
                        APIUploadDb.AddInParameter(command, Constant.AdultAgeTo, DbType.String, information.AdultAgeTo);
                        APIUploadDb.AddInParameter(command, Constant.MaxAdults, DbType.String, information.MaxAdults);
                        APIUploadDb.AddInParameter(command, Constant.MaxPax, DbType.String, information.MaxPax);
                        APIUploadDb.AddInParameter(command, Constant.Periods, DbType.String, information.Periods);
                        APIUploadDb.AddInParameter(command, Constant.SType, DbType.String, information.SType);
                        APIUploadDb.AddInParameter(command, Constant.Mpfcu, DbType.String, information.Mpfcu);
                        APIUploadDb.AddInParameter(command, Constant.Scu, DbType.String, information.Scu);
                        APIUploadDb.AddInParameter(command, Constant.MinScu, DbType.String, information.MinScu);
                        APIUploadDb.AddInParameter(command, Constant.MaxScu, DbType.String, information.MaxScu);
                        APIUploadDb.AddInParameter(command, Constant.Inclusions, DbType.String, information.Inclusions);
                        APIUploadDb.AddInParameter(command, Constant.OptionImportantInfo, DbType.String, information.OptionImportantInfo);
                        APIUploadDb.AddInParameter(command, Constant.Images, DbType.String, images);
                        APIUploadDb.AddInParameter(command, Constant.Description1, DbType.String, information.Description);
                        APIUploadDb.AddInParameter(command, Constant.OptExtras, DbType.String, optExtras);
                        APIUploadDb.AddInParameter(command, Constant.CancellationPolicy, DbType.String, cancellationPolicy);

                        APIUploadDb.ExecuteNonQuery(command);
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "AOTPersistence",
                    MethodName = "SaveAllActivityAgeGroupsMapping",
                    Params = $"{SerializeDeSerializeHelper.Serialize(activityAgeGroups)}"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        /// <summary>
        /// Data synchronization between Isango databases
        /// </summary>
        public void SyncDataBetweenIsangoDataBases()
        {
            try
            {
                using (var ageGroupCommand = IsangoDataBaseLive.GetStoredProcCommand(Constant.SyncAOTMappingSp))
                {
                    IsangoDataBaseLive.ExecuteNonQuery(ageGroupCommand);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "AOTPersistence",
                    MethodName = "SyncDataBetweenIsangoDataBases",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }
    }
}