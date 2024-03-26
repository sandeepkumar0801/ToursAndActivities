using ServiceAdapters.Rezdy.Rezdy.Converters.Contracts;

namespace ServiceAdapters.Rezdy.Rezdy.Converters
{
    public class ConverterBase : IConverterBase
    {
        public ConverterBase()
        {
        }

        public virtual object Convert(string response)
        {
            return null;
        }

        public virtual object Convert<T>(string response, T request)
        {
            return null;
        }

        public virtual object Convert<T,A>(string response, T request,A apiResponse)
        {
            return null;
        }
    }
}