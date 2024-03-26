using Microsoft.Extensions.PlatformAbstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Util
{
    public static class WebRootPath
    {
        public static string GetWebRootPath()
        {
            var appEnv = PlatformServices.Default.Application;
            var baseDirectory = AppContext.BaseDirectory;

            // Construct the path to the "wwwroot" folder
            var webRootPath = Path.Combine(baseDirectory, "wwwroot");

            return webRootPath;
        }

        public static string GetWebRoot()
        {
            var appEnv = PlatformServices.Default.Application;
            var baseDirectory = AppContext.BaseDirectory;
            var webRootPath = baseDirectory;

            return webRootPath;
        }
    }
}
