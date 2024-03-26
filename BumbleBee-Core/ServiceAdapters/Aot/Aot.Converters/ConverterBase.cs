using Logger.Contract;
using ServiceAdapters.Aot.Aot.Converters.Contracts;
using ServiceAdapters.Aot.Aot.Entities;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace ServiceAdapters.Aot.Aot.Converters
{
    public class ConverterBase : IConverterBase
    {
        public ILogger _logger;

        public ConverterBase(ILogger logger)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            _logger = logger;
        }

        public MethodType Converter { get; set; }

        public virtual object Convert<T>(T objectresult)
        {
            return null;
        }

        public virtual object Convert<T>(T objectresult, T inputRequest, T request)
        {
            return null;
        }

        public T DeSerializeXml<T>(string responseXmlString)
        {
            if (string.IsNullOrEmpty(responseXmlString))
                throw new ArgumentNullException();

            var utf8Encoding = new UTF8Encoding();
            var byteArray = utf8Encoding.GetBytes(responseXmlString);

            var memoryStream = new MemoryStream(byteArray);
            var deSerializer = new XmlSerializer(typeof(T));

            var xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);

            var deSerializedObject = (T)deSerializer.Deserialize(xmlTextWriter.BaseStream);
            return deSerializedObject;
        }

        public virtual object Convert<T>(T inputContext, T inputRequest)
        {
            return null;
        }
    }
}