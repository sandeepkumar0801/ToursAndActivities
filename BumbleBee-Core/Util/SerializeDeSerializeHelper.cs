using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Util
{
    public static class SerializeDeSerializeHelper
    {
        #region Json

        public static T DeSerialize<T>(string value)
        {
            try
            {
              
                return JsonConvert.DeserializeObject<T>(value);
            }
            catch (System.Exception ex)
            {
                //ignored
            }

            return default(T);
        }

        public static string Serialize(object value)
        {
            var result = string.Empty;
            if (value != null)
            {
                try
                {
                    var getType = value.GetType().ToString();
                    var isString = getType == "System.String";
                    var data= "System.Collections.Generic.List`1[Isango.Entities.PricingUnit]";
                    if (isString)
                    {
                        result = value.ToString();
                    }
                    else if (getType == data)
                    {


                        var castedObject = (System.Collections.Generic.List<Isango.Entities.PricingUnit>)value;
                        try
                        {
                            var lstPerson = new System.Collections.Generic.List<Isango.Entities.PerPersonPricingUnit>();

                            foreach (var item in castedObject)
                            {
                                var puType = item.GetType().ToString().ToLowerInvariant();

                                var person =(Isango.Entities.PerPersonPricingUnit)item;
                                if (person != null)
                                {
                                    lstPerson.Add(person);
                                }
                            }
                            result = JsonConvert.SerializeObject(lstPerson);
                        }
                        catch (Exception ex)
                        {
                            result = JsonConvert.SerializeObject(castedObject);
                        }
                      }

                    else
                    {
                        result = JsonConvert.SerializeObject(value);
                    }
                    
                   
                }
                catch (System.Exception)
                {
                    //ignore
                }
            }
            return result;
        }

        public static string SerializeWithContractResolver(object value)
        {
            var settings = new JsonSerializerSettings()
            {
                ContractResolver = new LowercaseContractResolver()
            };

            return JsonConvert.SerializeObject(value, Newtonsoft.Json.Formatting.Indented, settings);
        }

        public static string SerializeWithNullValueHandling(object value)
        {
            var settings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            return JsonConvert.SerializeObject(value, Newtonsoft.Json.Formatting.Indented, settings);
        }

        public static T DeSerializeWithNullValueHandling<T>(string value)
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            return JsonConvert.DeserializeObject<T>(value, settings);
        }

        public static T DeSerializeWithIsoDateTime<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value,
                new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-ddThh:mm:ss" });
        }

        #endregion Json

        #region Xml

        public static string SerializeXml<T>(T requestObject)
        {
            if (requestObject == null)
                return null;

            var serializer = new XmlSerializer(requestObject.GetType());
            var memoryStream = new MemoryStream();
            var xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
            var utf8Encoding = new UTF8Encoding();

            var xmlnsEmpty = new XmlSerializerNamespaces();
            xmlnsEmpty.Add("", "");

            xmlTextWriter.Formatting = System.Xml.Formatting.Indented;
            xmlTextWriter.Indentation = 4;

            serializer.Serialize(xmlTextWriter, requestObject, xmlnsEmpty);
            var newMemStream = (MemoryStream)xmlTextWriter.BaseStream;
            var xmlString = utf8Encoding.GetString(newMemStream.ToArray());
            return xmlString;
        }

        public static T DeSerializeXml<T>(string responseXmlString)
        {
            if (responseXmlString == null)
                return default(T);

            var serializer = new XmlSerializer(typeof(T));
            using (TextReader reader = new StringReader(responseXmlString))
            {
                var deSerializedObject = (T)serializer.Deserialize(reader);
                return deSerializedObject;
            }
        }

        #endregion Xml

        #region Bae64Encode Base64Decode

        /// <summary>
        /// Base64Decode
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string Base64Decode(string data)
        {
            string result = string.Empty;
            try
            {
                var encoder = new System.Text.UTF8Encoding();
                var utf8Decode = encoder.GetDecoder();
                byte[] todecode_byte = Convert.FromBase64String(data);
                int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
                char[] decoded_char = new char[charCount];
                utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
                result = new String(decoded_char);
            }
            catch (Exception ex)
            {
                //ignored
            }
            return result;
        }

        /// <summary>
        /// Base64Encode
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string Base64Encode(string data)
        {
            string encodedData = string.Empty;
            try
            {
                byte[] encData_byte = new byte[data.Length];
                encData_byte = System.Text.Encoding.UTF8.GetBytes(data);
                encodedData = Convert.ToBase64String(encData_byte);
                return encodedData;
            }
            catch (Exception ex)
            {
            }
            return encodedData;
        }

        #endregion Bae64Encode Base64Decode
    }
}