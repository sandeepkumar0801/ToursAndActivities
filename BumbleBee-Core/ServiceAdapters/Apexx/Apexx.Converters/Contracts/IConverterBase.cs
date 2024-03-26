namespace ServiceAdapters.Apexx.Apexx.Converters.Contracts
{
    public interface IConverterBase
    {
        object Convert(string response, object inputObject);
    }
}