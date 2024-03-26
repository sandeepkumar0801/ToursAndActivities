using ServiceAdapters.NeverBounce.Constants;
using ServiceAdapters.NeverBounce.NeverBounce.Commands.Contracts;
using Util;

namespace ServiceAdapters.NeverBounce.NeverBounce.Commands
{
    public class AuthenticationCmdHandler : IAuthenticationCmdHandler
    {
        public object GetResults()
        {
            var client = new AsyncClient
            {
                ServiceURL = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.NbAuth)
            };
            return client.PostNbData($"{Constant.UrlStart}{ConfigurationManagerHelper.GetValuefromAppSettings(Constant.ClientId)}&client_secret={ConfigurationManagerHelper.GetValuefromAppSettings(Constant.ClientSecret)}");
        }
    }
}