namespace ServiceAdapters.Redeam.Redeam.Converters.Contracts
{
    public interface IConverterBase
    {
        object Convert<T>(T response);

        object Convert<T>(T response, T request);

        object Convert<T>(T response, T request, T extraRequest);
    }
}