using ServiceAdapters.HB.HB.Entities;

namespace ServiceAdapters.HB.HB.Converters.Contracts
{
    public interface IConverterBase
    {
        object Convert(object apiResponse, MethodType methodType, object criteria = null);
    }
}