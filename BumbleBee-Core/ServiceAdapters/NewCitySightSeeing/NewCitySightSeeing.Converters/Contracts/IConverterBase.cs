using ServiceAdapters.NewCitySightSeeing.NewCitySightSeeing.Entities;

namespace ServiceAdapters.NewCitySightSeeing.NewCitySightSeeing.Converters.Contracts
{
    public interface IConverterBase
    {
        object Convert(object apiResponse, MethodType methodType, object criteria = null);
    }
}