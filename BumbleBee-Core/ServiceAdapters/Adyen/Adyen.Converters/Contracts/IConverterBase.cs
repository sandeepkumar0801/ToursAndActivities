namespace ServiceAdapters.Adyen.Adyen.Converters.Contracts
{
    public interface IConverterBase
    {
        object Convert(string response, object inputObject);
    }
}