using ServiceAdapters.Bokun.Bokun.Entities;

namespace ServiceAdapters.Bokun.Bokun.Converters.Contracts
{
    public interface ICheckAvailabilitiesConverter : IConverterBase
    {
        object Convert<T>(object inputContext, MethodType methodType, T inputRequest, Isango.Entities.Activities.Activity activity);
    }
}