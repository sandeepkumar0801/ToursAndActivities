using Logger.Contract;
using ServiceAdapters.HotelBeds.Constants;
using ServiceAdapters.HotelBeds.HotelBeds.Commands.Contracts;
using ServiceAdapters.HotelBeds.HotelBeds.Config;
using ServiceAdapters.HotelBeds.HotelBeds.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using Util;

namespace ServiceAdapters.HotelBeds.HotelBeds.Commands
{
    public class ServiceRemoveCommandHandler : CommandHandlerBase, IServiceRemoveCommandHandler
    {
        public ServiceRemoveCommandHandler(ILogger iLog) : base(iLog)
        {
        }

        public override object GetResults(object xmlResults)
        {
            return null;
        }

        protected override object GetXmlResults(object inputXml)
        {
            if (inputXml != null)
            {
                var client = new AsyncClient
                {
                    ServiceURL = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.HbServiceUrl)
                };
                var values = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>(Constant.XmlRequest, inputXml.ToString())
                };
                var xml = client.Post(values);
                return xml;
            }
            return null;
        }

        protected override object CreateInputRequest(InputContext inputContext)
        {
            var purchaseCancelXml = new XmlDocument();
            if (AppDomain.CurrentDomain.BaseDirectory.ToLowerInvariant().EndsWith(Constant.Release) || AppDomain.CurrentDomain.BaseDirectory.ToLowerInvariant().EndsWith(Constant.Debug))
                purchaseCancelXml.Load($"{Directory.GetParent(Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName).FullName).FullName}{Constant.ServiceRemoveRqXml}");
            else
                purchaseCancelXml.Load($"{AppDomain.CurrentDomain.BaseDirectory}{Constant.ServiceRemoveRqXml}");

            Parser.ResetParser();
            Parser.ParseXml(purchaseCancelXml.InnerXml);

            #region Credentials

            XmlOperations.SetInnerTextForElement(Constant.User, 0, Parser, inputContext.UserName);
            XmlOperations.SetInnerTextForElement(Constant.Password, 0, Parser, inputContext.Password);

            #endregion Credentials

            if (inputContext.Language != null)
            {
                XmlOperations.SetInnerTextForElement(Constant.Language, 0, Parser, inputContext.Language.ToUpperInvariant());
            }

            #region Purchase

            XmlOperations.SetElementAttributeValue(Constant.ServiceRemoveRq, 0, Parser, XmlAttribs.PurchaseToken, inputContext.HBSelectedProducts[0].PurchaseToken);
            XmlOperations.SetElementAttributeValue(Constant.ServiceRemoveRq, 0, Parser, XmlAttribs.Spui, inputContext.HBSelectedProducts[0].SPUI);

            #endregion Purchase

            return Parser.XmlDocument.InnerXml;
        }

        protected override async Task<object> GetXmlResultsAsync(object inputXml)
        {
            var client = new AsyncClient
            {
                ServiceURL = ConfigurationManagerHelper.GetValuefromAppSettings("HBServiceURL")
            };
            var values = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("xml_request", inputXml.ToString())
            };
            var xml = await client.PostAsync(values);
            return (object)xml;
        }
    }
}