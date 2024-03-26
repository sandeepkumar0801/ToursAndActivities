using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Threading;
using Constant = Util.CommonUtilConstant;

namespace Util

{
    public class CommonResourceManager
    {
        private static ResourceManager _resourceManager;

        static CommonResourceManager()
        {
            _resourceManager = new System.Resources.ResourceManager(Constant.ResourceManagerBaseName, Assembly.GetExecutingAssembly());
        }

        private static CultureInfo GetCultureInfo(string languageCode)
        {
            string culture;
            switch (languageCode)
            {
                case Constant.DE:
                    culture = Constant.Germany;
                    break;

                case Constant.FR:
                    culture = Constant.French;
                    break;

                case Constant.ES:
                    culture = Constant.Spanish;
                    break;

                default:
                    culture = string.Empty;
                    break;
            }

            return culture != string.Empty ? new CultureInfo(culture) : Thread.CurrentThread.CurrentCulture;
        }

        private static ResourceManager GetResourceManager()
        {
            if (_resourceManager == null)
            {
                _resourceManager = new System.Resources.ResourceManager(Constant.ResourceManagerBaseName, Assembly.GetExecutingAssembly());
            }
            return _resourceManager;
        }

        /// <summary>
        /// Get text from resource file for the key based on the language code.
        /// </summary>
        /// <param name="languageCode"></param>
        /// <param name="resourceKey"></param>
        /// <returns></returns>
        public static string GetString(string languageCode, string resourceKey)
        {
            _resourceManager = GetResourceManager();
            var cultureInfo = GetCultureInfo(languageCode);
            return _resourceManager.GetString(resourceKey, cultureInfo);
        }
    }
}