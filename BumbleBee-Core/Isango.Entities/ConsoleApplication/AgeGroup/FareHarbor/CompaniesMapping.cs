using Isango.Entities.Activities;

namespace Isango.Entities.ConsoleApplication.AgeGroup.FareHarbor
{
    public class CompaniesMapping
    {
        public string ShortName { get; set; }
        public int TourId { get; set; }
        public string Name { get; set; }
        public string CancellationPolicy { get; set; }
        public string CancellationPolicyHtml { get; set; }

        public CompaniesMapping()
        {
        }

        public CompaniesMapping(Activity activity, string shortName)
        {
            ShortName = shortName;
            Name = activity.Name;
            TourId = activity.FactsheetId;
            CancellationPolicy = activity.CancellationPolicy;
            CancellationPolicyHtml = activity.AdditionalInfo;
        }
    }
}