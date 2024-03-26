using ServiceAdapters.RedeamV12.RedeamV12.Converters.Contracts;

namespace ServiceAdapters.RedeamV12.RedeamV12.Converters
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
        public virtual object Convert<T>(T response, T request, T extraRequest, T pricing)
        {
            return null;
        }
    }
}