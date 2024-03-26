using Logger.Contract;

namespace ServiceAdapters.TourCMS.TourCMS.Converters
{
    public abstract class ConverterBase
    {
        public ILogger _logger;
        protected ConverterBase(ILogger logger)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            _logger = logger;
        }
        
        //Use in calendar 
        public abstract object Convert(object objectResponse, object criteria);

        public abstract object Convert<T>(string response, T request);
    }
}