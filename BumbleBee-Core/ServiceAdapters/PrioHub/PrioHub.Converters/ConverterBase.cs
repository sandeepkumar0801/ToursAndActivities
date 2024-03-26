using Logger.Contract;
using ServiceAdapters.PrioHub.PrioHub.Entities;

namespace ServiceAdapters.PrioHub.PrioHub.Converters
{
    public abstract class ConverterBase
    {
        public ILogger _logger;

        public ConverterBase(ILogger logger)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            _logger = logger;
        }

        public MethodType Converter { get; set; }

        public abstract object Convert(object objectResult);
    }
}