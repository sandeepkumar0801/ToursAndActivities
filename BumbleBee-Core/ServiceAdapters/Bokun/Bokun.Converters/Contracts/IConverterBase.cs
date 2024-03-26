namespace ServiceAdapters.Bokun.Bokun.Converters.Contracts
{
    public interface IConverterBase
    {
        object Convert<T>(T objectResult, T criteria);
    }
}