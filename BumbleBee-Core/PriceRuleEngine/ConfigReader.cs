using Isango.Entities;
using Isango.Entities.PricingRules;
using Logger.Contract;
using PriceRuleEngine.Constants;
using PriceRuleEngine.Contracts;
using ServiceAdapters.Rayna.Rayna.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Util;

namespace PriceRuleEngine
{
    /// <summary>
    /// Reads the Module.config and returns the list of modules applicable for a category
    /// </summary>
    public class ConfigReader : IConfigReader
    {
        

        /// <summary>
        /// Gets the the list of modules applicable for a category
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public List<string> GetModuleNames(string category)
        {
            var xml = ReadXML(Constant.ModuleConfig);
            var moduleNames = new List<string>();
            if (xml == null) return moduleNames;

            var elements = from categoryNode in xml.Descendants(Constant.Categories)
                           select categoryNode?.Elements()?.Where(x => x.Attribute(Constant.Name)?.Value == category);
            moduleNames = elements?.Select(x => x.Descendants()?.Nodes()?.Select(y => y.Parent?.Value)?.ToList())?.FirstOrDefault();

            return moduleNames;
        }

        /// <summary>
        /// Gets the the list of modulesBuilders
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public string GetModuleBuilder(string category)
        {
            var xml = ReadXML(Constant.ModuleBuilderConfig);
            if (xml == null) return null;

            var moduleBuilder = (from element in xml.Descendants(Constant.ModuleBuilders)
                                 select element?.Elements()?.FirstOrDefault(x => x.Attribute(Constant.Name)?.Value == category))?.FirstOrDefault()?.Value;

            return string.IsNullOrEmpty(moduleBuilder) ? string.Empty : moduleBuilder;
        }

        #region Private Methods

        private XDocument ReadXML(string fileName)
        {
            var key = $"xml_{fileName}";
            XDocument xml;

            if (CacheHelper.Exists(key))
            {
                
                    if (CacheHelper.Get<XDocument>(key, out xml))
                    {
                        return xml;  // Return cached data
                    }
                
                
            }
            else
            {
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