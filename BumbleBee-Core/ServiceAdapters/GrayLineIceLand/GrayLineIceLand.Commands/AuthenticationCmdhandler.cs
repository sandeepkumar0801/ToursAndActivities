using ServiceAdapters.GrayLineIceLand.Constants;
using ServiceAdapters.GrayLineIceLand.GrayLineIceLand.Commands.Contracts;
using Util;
using EntityConstant = ServiceAdapters.GrayLineIceLand.GrayLineIceLand.Entities.Constants;

namespace ServiceAdapters.GrayLineIceLand.GrayLineIceLand.Commands
{
    public class AuthenticationCmdHandler : IAuthenticationCmdhandler
    {
        private const string QueryString = Constant.QueryString;

        public object GetResults()
        {
            var client = new AsyncClient
            {
                ServiceURL = $"{ConfigurationManagerHelper.GetValuefromAppSettings(Constant.GrayLineBaseUrl)}{EntityConstant.AuthURL}"
            };
            return client.PostNbData(string.Format(QueryString, ConfigurationManagerHelper.GetValuefromAppSettings(Constant.GLAuthuser), ConfigurationManagerHelper.GetValuefromAppSettings(Constant.GLAuthpassword)));
        }
    }
}