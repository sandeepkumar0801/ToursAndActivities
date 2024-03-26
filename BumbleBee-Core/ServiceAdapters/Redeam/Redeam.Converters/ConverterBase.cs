using ServiceAdapters.Redeam.Redeam.Converters.Contracts;

namespace ServiceAdapters.Redeam.Redeam.Converters
{
    public class ConverterBase : IConverterBase
    {
        public virtual object Convert<T>(T response)
        {
            return null;
        }

        public virtual object Convert<T>(T response, T request)
        {
            return null;
        }

        public virtual object Convert<T>(T response, T request, T extraRequest)
        {
            return null;
        }
    }
}