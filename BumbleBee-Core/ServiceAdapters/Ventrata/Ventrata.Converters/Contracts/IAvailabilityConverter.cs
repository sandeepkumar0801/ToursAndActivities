using Isango.Entities.Activities;

namespace ServiceAdapters.Ventrata.Ventrata.Converters.Contracts
{
    public interface IAvailabilityConverter : IConverterBase
    {
        List<Activity> ConvertAvailablityResult(object listOfOptionsFromAPI, object criteria);
    }
}
