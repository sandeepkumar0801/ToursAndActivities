using ServiceAdapters.Rayna.Rayna.Entities;

namespace ServiceAdapters.Rayna.Rayna.Converters.Contracts
{
    public interface IConverterBase
    {
        object Convert(object apiResponse, MethodType methodType, object criteria = null);
    }
}