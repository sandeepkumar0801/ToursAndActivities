using Logger.Contract;
using ServiceAdapters.CitySightSeeing.CitySightSeeing.Entities;

namespace ServiceAdapters.CitySightSeeing.CitySightSeeing.Converters
{
    public class ConverterBase
    {
        protected readonly ILogger _logger;

        protected ConverterBase(ILogger logger)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            _logger = logger;
        }

        public MethodType Converter { get; set; }

        public virtual object Convert<T>(T objectResult, object criteria)
        {
            return null;
        }
    }
}
