using Logger.Contract;

namespace ServiceAdapters.Tiqets.Tiqets.Converters
{
    public abstract class ConverterBase
    {
        public ILogger _logger;

        protected ConverterBase(ILogger logger)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            _logger = logger;
        }

        public virtual object Convert<T>(T objectResult, object input)
        {
            return null;
        }
    }
}