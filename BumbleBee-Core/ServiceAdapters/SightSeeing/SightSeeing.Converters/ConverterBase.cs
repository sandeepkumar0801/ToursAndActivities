using ServiceAdapters.SightSeeing.SightSeeing.Entities;

namespace ServiceAdapters.SightSeeing.SightSeeing.Converters
{
    public class ConverterBase
    {
        protected ConverterBase()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
        }

        public MethodType Converter { get; set; }

        public virtual object Convert(object objectResult)
        {
            return null;
        }

        public virtual object Convert<T>(string response, T request)
        {
            return null;
        }
    }
}