using Logger.Contract;
using ServiceAdapters.GrayLineIceLand.GrayLineIceLand.Entities;

namespace ServiceAdapters.GrayLineIceLand.GrayLineIceLand.Converters
{
    public abstract class ConverterBase
    {
        protected readonly ILogger _logger;

        public ConverterBase(ILogger logger)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            _logger = logger;
        }

        public MethodType Converter { get; set; }

        public abstract object Convert(object objectResult);

        public abstract object Convert(object objectResult, object input);
    }
}