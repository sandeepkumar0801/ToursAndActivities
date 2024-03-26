namespace ServiceAdapters.Tiqets.Tiqets.Converters.Contracts
{
    public interface IConverterBase
    {
        object Convert<T>(T objectResult, object input);
    }
}