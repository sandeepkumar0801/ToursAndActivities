using Hangfire.Dashboard;
using System;
using Util;

namespace CacheLoader.HangFire
{

    public class ConsoleLogJob
    {
        public void LogToConsoleJob()
        {
            var data = ConfigurationManagerHelper.GetValuefromAppSettings("Environment"); // Replace with your actual key
            if (data == "")
            {
                data = "appsettings.json";
                Console.WriteLine("environment from job:" + data);
            }
            else
            {
                Console.WriteLine("environment from job:" + data);

            }
        }
    }



}
