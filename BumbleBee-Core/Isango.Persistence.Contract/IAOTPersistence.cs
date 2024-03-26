using Isango.Entities.ConsoleApplication.AgeGroup.AOT;

namespace Isango.Persistence.Contract
{
    public interface IAOTPersistence
    {
        void SaveAllActivityAgeGroupsMapping(OptionGeneralInfoResponse activityAgeGroups,
            string cancellationPolicy);

        void SyncDataBetweenIsangoDataBases();
    }
}