using System;

namespace Util
{
    public static class MaxParallelThreadCountHelper
    {
        private static int _processorCount = (int)Math.Ceiling((Environment.ProcessorCount * .5) * 1.0);

        public static int GetMaxParallelThreadCount(string key = "MaxParallelThreadCount")
        {
            string keyValue = string.Empty;
            try
            {
                keyValue = ConfigurationManagerHelper.GetValuefromAppSettings(key);
            }
            catch  // key not found in config
            {
                keyValue = "0.75";
            }
            try
            {
                double.TryParse(keyValue, out double processorCountFromConfig);

                if (processorCountFromConfig > 0.75 || processorCountFromConfig <= 0)
                {
                    processorCountFromConfig = 0.75;
                }

                _processorCount = Convert.ToInt32(Math.Ceiling((Environment.ProcessorCount * processorCountFromConfig) * 1.0));
            }
            catch
            {
                _processorCount = 1;
            }
            return _processorCount;
        }
    }
}