using Isango.Entities.Activities;
using Isango.Entities.ConsoleApplication.DataDumping;
using System.Collections.Generic;

namespace Isango.Entities.GoogleMaps
{
    public class MerchantActivitiesDto
    {
        public string MerchantId { get; set; }
        public List<ActivityDto> Activities { get; set; }
    }

    public class ActivityDto
    {
        public int ActivityId { get; set; }
        public List<AcitvityCollection> AcitvityCollections { get; set; }
       public List<StorageServiceDetail> ServiceDetails { get; set; }
      public List<ConsoleApplication.DataDumping.ExtraDetail> ExtraDetails { get; set; }
      public List<CancellationPolicy> CancellationPolicies { get; set; }
    }

    public class AcitvityCollection
    {
        public string LanguageCode { get; set; }
        public Activity Activity { get; set; }
    }
}