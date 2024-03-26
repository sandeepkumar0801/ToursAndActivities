using Logger.Contract;
using ServiceAdapters.MoulinRouge.MoulinRouge.Entities;
using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace ServiceAdapters.MoulinRouge.MoulinRouge.Converters
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

        public abstract object Convert<T>(T objectResult);

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
    }
}