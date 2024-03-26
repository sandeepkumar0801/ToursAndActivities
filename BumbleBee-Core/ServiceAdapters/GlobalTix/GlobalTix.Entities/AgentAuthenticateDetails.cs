using Logger.Contract;
using ServiceAdapters.GlobalTix.GlobalTix.Commands;
using System;
using Util;

namespace ServiceAdapters.GlobalTix.GlobalTix.Entities
{
    public class AgentAuthenticateDetails
    {
        private static readonly Lazy<AgentAuthenticateDetails> lazy = new Lazy<AgentAuthenticateDetails>(() => new AgentAuthenticateDetails());

        private static AgentAuthenticateDetails _instance;

        public AuthRS AuthRs { get; set; }

        public DateTime ExpirationTime { get; set; }

        public bool IsAuthenticated { get; private set; }
        public bool? isNonThailandProduct { get; set; } = null;
        private AgentAuthenticateDetails()
        {
        }

        public static AgentAuthenticateDetails Instance(bool isNonThailandProduct)
        {
           //get
            //{
            lock (lazy)
            {
                _instance = lazy.Value;

                //_instance.AuthRs = null;
                if (((_instance.AuthRs == null) || _instance.ExpirationTime < DateTime.Now)
                 || (_instance.AuthRs != null && _instance.isNonThailandProduct != isNonThailandProduct))
                {
                    _instance.IsAuthenticated = _instance.GetAuthData(isNonThailandProduct);
                }
                return _instance;
            }
            //}
        }

        private bool GetAuthData(bool isNonThailandProduct)
        {
            try
            {

                var cmd = new AuthenticationCommandHandler(new Logger.Logger());
                //var result = cmd.GetResults();
                InputContext inCtx = new InputContext
                {
                    AuthToken = string.Empty,
                    MethodType = MethodType.Authentication
                };
                var result = cmd.Execute(inCtx, string.Empty, isNonThailandProduct);

                if (result != null)
                {
                    this.AuthRs = SerializeDeSerializeHelper.DeSerialize<AuthRS>((string)result);
                    if (this.AuthRs != null && this.AuthRs.IsSuccess)
                    {
                        this.ExpirationTime = DateTime.Now.AddSeconds(this.AuthRs.Data.ExpiresIn).AddSeconds(-2);
                        this.isNonThailandProduct = isNonThailandProduct;
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }

            return false;
        }

        public string GetAccessTokenType()
        {
            return (this.AuthRs?.Data?.TokenType) ?? null;
        }

        public string GetAccessToken()
        {
            return (this.AuthRs?.Data?.AccessToken) ?? null;
        }

        public string GetHttpAuthorizationHeader()
        {
            return $"{GetAccessTokenType()} {GetAccessToken()}";
        }
    }
}