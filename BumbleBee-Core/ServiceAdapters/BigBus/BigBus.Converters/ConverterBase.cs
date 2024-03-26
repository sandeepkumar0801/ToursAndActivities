using Logger.Contract;
using ServiceAdapters.BigBus.BigBus.Converters.Contracts;

namespace ServiceAdapters.BigBus.BigBus.Converters
{
    public class ConverterBase : IConverterBase
    {
        protected readonly ILogger _logger;

        protected ConverterBase(ILogger logger)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            _logger = logger;
        }

        public virtual object Convert(string objectResult)
        {
            return null;
        }

        public virtual object Convert<T>(string response, T request)
        {
            return null;
        }
    }
}