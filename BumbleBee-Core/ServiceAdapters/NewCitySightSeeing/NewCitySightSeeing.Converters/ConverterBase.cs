namespace ServiceAdapters.NewCitySightSeeing.NewCitySightSeeing.Converters
{
    public abstract class ConverterBase
    {
        public ConverterBase()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
        }
    }
}