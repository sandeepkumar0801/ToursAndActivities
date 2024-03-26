using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using TableStorageOperations.Contracts;
using Util;
using Constant = TableStorageOperations.Constants.Constant;

namespace TableStorageOperations
{
    /// <summary>
    /// Reads the AvailabilitiesEntity.config and returns the list of entity builder class for given entity
    /// </summary>
    public class ConfigReader : IConfigReader
    {
        /// <summary>
        /// Fetches the availability entity for the given entityName
        /// </summary>
        /// <param name="entityName"></param>
        /// <returns></returns>
        public string AvailabilityEntity(string entityName)
        {
            var xml = ReadXML(Constant.AvailabilitiesEntityConfig);
            if (xml == null) return null;

            var entity = (from element in xml.Descendants(Constant.AvailabilityEntities)
                          select element?.Elements()?.FirstOrDefault(x => x.Attribute(Constant.Name)?.Value?.ToUpperInvariant() == entityName?.ToUpperInvariant()))?.FirstOrDefault()?.Value;

            return entity;
        }

        #region Private Methods

        

        private XDocument ReadXML(string fileName)
        {
            var key = $"xml_TableStorage_{fileName}";
            XDocument xml;

            if (CacheHelper.Get<XDocument>(key, out xml))
            {
                return xml;  // Return cached data
            }

            var directory = WebRootPath.GetWebRoot();
            var file = Path.Combine(directory, fileName);

            try
            {
                if (!File.Exists(file))
                {
                    file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
                    if (!File.Exists(file))
                    {
                        LogException($"File not found: {file}");
                        return null;
                    }
                }

                xml = XDocument.Load(file);
                CacheHelper.Set(key, xml);
            }
            catch (Exception ex)
            {
                LogException(ex.Message);
                return null;
            }

            return xml;
        }


        private void LogException(string message)
        {
            Console.WriteLine($"Exception: {message}");
        }


            #endregion Private Methods
        }
    }