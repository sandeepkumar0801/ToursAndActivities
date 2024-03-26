using Isango.Entities.Activities;

namespace ServiceAdapters.Ventrata.Ventrata.Converters.Contracts
{
    public interface ICustomQuestionsConverter : IConverterBase
    {
        List<Activity> ConvertAvailablityResult(object listOfOptionsFromAPI, object criteria);
    }
}
