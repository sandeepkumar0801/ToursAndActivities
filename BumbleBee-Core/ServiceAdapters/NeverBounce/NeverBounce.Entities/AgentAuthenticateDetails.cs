using ServiceAdapters.NeverBounce.NeverBounce.Commands;
using System;
using Util;

namespace ServiceAdapters.NeverBounce.NeverBounce.Entities
{
    public class AgentAuthenticateDetails
    {
        private AgentAuthenticateDetails()
        {
        }

        private static readonly object Lock = new object();

        private static AgentAuthenticateDetails _instance;

        public AuthRs AuthRs { get; set; }

        public DateTime ExpirationTime { get; set; }

        public bool IsAuthenticated { get; private set; }

        public static AgentAuthenticateDetails Instance
        {
            get
            {
                lock (Lock)
                {
                    if (_instance == null)
                    {
                        _instance = new AgentAuthenticateDetails();
                    }
                    if ((_instance.AuthRs == null) || _instance.ExpirationTime < DateTime.Now)
                    {
                        _instance.IsAuthenticated = _instance.GetAuthData();
                    }
                    return _instance;
                }
            }
        }

        private bool GetAuthData()
        {
            var agentAuthenticateDetails = new AgentAuthenticateDetails();
            var result = agentAuthenticateDetails.Authenticate();
            if (result != null)
            {
                var authRs = SerializeDeSerializeHelper.DeSerialize<AuthRs>((string)result);
                agentAuthenticateDetails.AuthRs = authRs;
                agentAuthenticateDetails.ExpirationTime = DateTime.Now.AddSeconds(authRs.ExpiresIn).AddSeconds(-5);
                agentAuthenticateDetails.IsAuthenticated = true;

                _instance = agentAuthenticateDetails;
                return true;
            }
            return false;
        }

        private object Authenticate()
        {
            var authenticationCmdHandler = new AuthenticationCmdHandler();
            return authenticationCmdHandler.GetResults();
        }
    }
}