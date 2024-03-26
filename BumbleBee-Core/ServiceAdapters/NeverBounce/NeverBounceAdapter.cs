using Logger.Contract;
using ServiceAdapters.NeverBounce.NeverBounce.Commands.Contracts;
using ServiceAdapters.NeverBounce.NeverBounce.Entities;
using Util;

namespace ServiceAdapters.NeverBounce
{
    public class NeverBounceAdapter : INeverBounceAdapter, IAdapter
    {
        #region "Private Members"

        private readonly IEmailVerificationCmdHandler _emailVerificationCmdHandler;
        private readonly ILogger _log;

        #endregion "Private Members"

        #region "Constructor"

        public NeverBounceAdapter(IAuthenticationCmdHandler authenticationCmdHandler,
            IEmailVerificationCmdHandler emailVerificationCmdHandler, ILogger log)
        {
            _emailVerificationCmdHandler = emailVerificationCmdHandler;
            _log = log;
        }

        #endregion "Constructor"

        public bool IsEmailNbVerified(string email, string token)
        {
            if (AgentAuthenticateDetails.Instance.IsAuthenticated)
            {
                var result = VerifyEmail(email);

                _log.Write(email, result?.ToString(), "IsEmailNbVerified", token, "NeverBounce");

                if (result != null && result.ToString() != "")
                {
                    var res = SerializeDeSerializeHelper.DeSerialize<EmailVerificationRs>((string)result);
                    return res.Result == 0 || res.Result == 3 || res.Result == 4;
                }
            }

            return false;
        }

        #region Private Methods

        private object VerifyEmail(string email)
        {
            return _emailVerificationCmdHandler.GetResults(AgentAuthenticateDetails.Instance.AuthRs.AccessToken, email);
        }

        #endregion Private Methods
    }
}