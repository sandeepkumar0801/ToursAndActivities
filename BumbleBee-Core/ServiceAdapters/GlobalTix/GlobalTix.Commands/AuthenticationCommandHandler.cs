using Logger.Contract;
using ServiceAdapters.GlobalTix.Constants;
using ServiceAdapters.GlobalTix.GlobalTix.Commands.Contracts;
using ServiceAdapters.GlobalTix.GlobalTix.Entities;
using ServiceAdapters.GlobalTix.GlobalTix.Entities.RequestResponseModels;
using System.Text;
using Util;

namespace ServiceAdapters.GlobalTix.GlobalTix.Commands
{
    public class AuthenticationCommandHandler : CommandHandlerBase, IAuthenticationCommandHandler
    {
        #region Constructors
        public AuthenticationCommandHandler(ILogger iLog) : base(iLog)
        {
        }
        #endregion

        //public object GetResults()
        //{
        //    var client = new AsyncClient
        //    {
        //        ServiceURL = $"{ConfigurationManagerHelper.GetValuefromAppSettings(Constant.CfgParam_GlobalTixBaseUrl)}{Constant.URL_Auth}"
        //    };
        //
        //    AuthRQ authReq = new AuthRQ()
        //    {
        //        UserName = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.CfgParam_GTAuthuser),
        //        Password = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.CfgParam_GTAuthpassword)
        //    };
        //
        //    return client.ConsumePostService(null, Constant.HttpHeader_ContentType_JSON, SerializeDeSerializeHelper.Serialize(authReq), Encoding.UTF8);
        //}

        protected override object CreateInputRequest(InputContext inputContext, bool isNonThailandProduct)
        {
            if (isNonThailandProduct)
            {
                return
                   new AuthRQ()
                   {
                       UserName = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.CfgParam_GTAuthuser),
                       Password = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.CfgParam_GTAuthpassword)
                   };
            }
            else {
                return
                   new AuthRQ()
                   {
                       UserName = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.CfgParam_GTAuthuser_ForThailand),
                       Password = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.CfgParam_GTAuthpassword_ForThailand)
                   };
            }
        }

        protected override object CreateInputRequest(InputContext inputContext)
        {
            throw new System.NotImplementedException();
        }

        protected override object GetResults(object input, string authString)
        {
            AuthRQ authReq = input as AuthRQ;
            if (authReq == null)
            {
                return null;
            }

            AsyncClient client = GetServiceClient(Constant.URL_Auth);
            return client.ConsumePostService(null, Constant.HttpHeader_ContentType_JSON, SerializeDeSerializeHelper.Serialize(authReq), Encoding.UTF8);
        }

        protected override Task<object> GetResultsAsync(object input, string authString)
        {
            throw new System.NotImplementedException();
        }
    }
}
