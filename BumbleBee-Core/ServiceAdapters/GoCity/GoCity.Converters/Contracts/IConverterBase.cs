using ServiceAdapters.GoCity.GoCity.Entities;

namespace ServiceAdapters.GoCity.GoCity.Converters.Contracts
{
    public interface IConverterBase
    {
        object Convert(object apiResponse, MethodType methodType, object criteria = null);
    }
}