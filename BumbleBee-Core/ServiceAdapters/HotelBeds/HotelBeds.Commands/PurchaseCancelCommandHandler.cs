using Isango.Entities.Activities;
using Isango.Entities.Hotel;
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
    public class PurchaseCancelCommandHandler : CommandHandlerBase, IPurchaseCancelCommandHandler
    {
        public PurchaseCancelCommandHandler(ILogger iLog) : base(iLog)
        {
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
                purchaseCancelXml.Load($"{Directory.GetParent((Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName).FullName)).FullName}{Constant.PurchaseCancelRqXml}");
            else
                purchaseCancelXml.Load($"{AppDomain.CurrentDomain.BaseDirectory}{Constant.PurchaseCancelRqXml}");

            Parser.ResetParser();
            Parser.ParseXml(purchaseCancelXml.InnerXml);

            #region Credentials

            XmlOperations.SetInnerTextForElement(Constant.User, 0, Parser, inputContext.UserName);
            XmlOperations.SetInnerTextForElement(Constant.Password, 0, Parser, inputContext.Password);

            #endregion Credentials

            #region Purchase

            var purchaseNode = XmlOperations.GetElement(Constant.PurchaseReference, 0, Parser);
            XmlOperations.SetInnerTextForElement(XmlNodes.FileNumber, purchaseNode, 0, Parser, inputContext.HBSelectedProducts[0].FileNumber);
            if (inputContext.HBSelectedProducts[0].ProductOptions[0].GetType().Name.Equals(Constant.HotelOption))
            {
                var hotelOption = ((HotelOption)inputContext.HBSelectedProducts[0].ProductOptions.Find(x => x.IsSelected.Equals(true)));
                XmlOperations.SetElementAttributeValue(XmlNodes.IncomingOffice, 0, purchaseNode, Parser, XmlAttribs.Code, hotelOption.Contract.InComingOfficeCode);
            }
            else
            {
                var activityOption = ((ActivityOption)inputContext.HBSelectedProducts[0].ProductOptions.Find(x => x.IsSelected.Equals(true)));
                XmlOperations.SetElementAttributeValue(XmlNodes.IncomingOffice, 0, purchaseNode, Parser, XmlAttribs.Code, activityOption.Contract.InComingOfficeCode);
            }

            #endregion Purchase

            return Parser.XmlDocument.InnerXml;
        }

        public override object GetResults(object xmlResults)
        {
            return xmlResults;
        }

        protected override Task<object> GetXmlResultsAsync(object inputXml)
        {
            return null;
        }
    }
}