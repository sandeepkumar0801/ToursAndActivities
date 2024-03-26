using Logger.Contract;

namespace ServiceAdapters.HotelBeds.HotelBeds.Converters
{
    public abstract class ConverterBase
    {
        protected readonly ILogger _logger;

        public ConverterBase(ILogger logger)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            _logger = logger;
        }
    }
}