using Isango.Entities.ConsoleApplication.AgeGroup.GrayLineIceLand;
using System.Collections.Generic;

namespace Isango.Persistence.Contract
{
    public interface IGrayLineIceLandPersistence
    {
        void SaveAllAgeGroups(List<AgeGroup> masterAgeGroups);

        void SaveAllActivityAgeGroupsMapping(List<ActivityAgeGroup> activityAgeGroups);

        void SaveAllPickupLocations(List<Pickuplocation> masterPickupLocations);

        void SaveAllPickupLocationsMapping(List<ActivityPickupLocation> activityPickupLocationsList);

        void SyncDataBetweenDataBases();
    }
}