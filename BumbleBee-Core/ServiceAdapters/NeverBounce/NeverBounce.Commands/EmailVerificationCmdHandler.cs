using ServiceAdapters.NeverBounce.Constants;
using ServiceAdapters.NeverBounce.NeverBounce.Commands.Contracts;
using Util;

namespace ServiceAdapters.NeverBounce.NeverBounce.Commands
{
    public class EmailVerificationCmdHandler : IEmailVerificationCmdHandler
    {
        public object GetResults(string authString, string email)
        {
            var client = new AsyncClient
            {
                ServiceURL = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.EmailVerification)
            };
            return client.PostNbData($"{Constant.AccessToken}{authString}{Constant.Email}{email}");
        }
    }
}