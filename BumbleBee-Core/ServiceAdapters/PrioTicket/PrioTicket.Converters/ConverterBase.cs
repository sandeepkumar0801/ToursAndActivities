using Logger.Contract;
using ServiceAdapters.PrioTicket.PrioTicket.Entities;

namespace ServiceAdapters.PrioTicket.PrioTicket.Converters
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