using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logger.Contract;
using ServiceAdapters.GlobalTixV3.Constants;
using ServiceAdapters.GlobalTixV3.GlobalTix.Entities;
using ServiceAdapters.GlobalTixV3.GlobalTixV3.Commands.Contracts;
using Util;

namespace ServiceAdapters.GlobalTixV3.GlobalTixV3.Commands
{
    public class AuthenticationCommandHandler : CommandHandlerBase, IAuthenticationCommandHandler
    {
        #region Constructors
        public AuthenticationCommandHandler(ILogger iLog) : base(iLog)
        {
        }
        #endregion
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
            else
            {
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

        protected override object GetResults(object input, string authString,bool v)
        {
            AuthRQ authReq = input as AuthRQ;
            if (authReq == null)
            {
                return null;
            }

            AsyncClient client = GetServiceClient(Constant.URL_Auth);
            return client.ConsumePostService(null, Constant.HttpHeader_ContentType_JSON, SerializeDeSerializeHelper.Serialize(authReq), Encoding.UTF8);
        }

        protected override Task<object> GetResultsAsync(object input, string authString, bool v)
        {
            throw new System.NotImplementedException();
        }
    }
}
