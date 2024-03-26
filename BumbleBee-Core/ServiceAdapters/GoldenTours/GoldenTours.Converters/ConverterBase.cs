using Logger.Contract;
using ServiceAdapters.GoldenTours.GoldenTours.Converters.Contracts;
using System.Xml.Serialization;

namespace ServiceAdapters.GoldenTours.GoldenTours.Converters
{
    public class ConverterBase : IConverterBase
    {
        protected readonly ILogger _logger;

        protected ConverterBase(ILogger logger)
        {
            _logger = logger;
        }

        public virtual object Convert<T>(T response)
        {
            return null;
        }

        public virtual object Convert<T>(T response, T request)
        {
            return null;
        }

        public virtual object Convert<T>(T response, T request, T extraRequest)
        {
            return null;
        }

        public virtual object Convert<T>(T response, T request, T extraRequest, T extraResponse)
        {
            return null;
        }

        public T DeSerializeXml<T>(string responseXmlString)
        {
            var returnValue = default(T);
            if (string.IsNullOrEmpty(responseXmlString))
                throw new ArgumentNullException();
            var serializer = new XmlSerializer(typeof(T));

            using (TextReader reader = new StringReader(responseXmlString))
            {
                try
                {
                    returnValue = (T)serializer.Deserialize(reader);
                }
                catch (Exception ex)
                {
                    // ignored
                }
            }
            return returnValue;
        }
    }
}