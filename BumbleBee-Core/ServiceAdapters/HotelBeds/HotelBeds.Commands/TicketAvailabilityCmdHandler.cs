using Logger.Contract;
using ServiceAdapters.HotelBeds.Constants;
using ServiceAdapters.HotelBeds.HotelBeds.Commands.Contracts;
using ServiceAdapters.HotelBeds.HotelBeds.Config;
using ServiceAdapters.HotelBeds.HotelBeds.Entities;
using ServiceAdapters.HotelBeds.HotelBeds.Entities.Tickets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Util;

namespace ServiceAdapters.HotelBeds.HotelBeds.Commands
{
    public class TicketAvailabilityCmdHandler : CommandHandlerBase, ITicketAvailabilityCmdHandler
    {
        public TicketAvailabilityCmdHandler(ILogger iLog) : base(iLog)
        {
        }

        #region Command Specific Operations

        public override object GetResults(object xmlResults)
        {
            return ((string)xmlResults).Replace(Constant.ProductTicket, string.Empty).ConvertToObject<TicketAvailRs>();
        }

        #endregion Command Specific Operations

        protected override object GetXmlResults(object inputXml)
        {
            // Create Input XML
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
            var ticketCriteria = (Isango.Entities.Ticket.TicketCriteria)inputContext.InputCriteria;
            var childAges = Convert.ToInt32((ticketCriteria?.Ages?.Where(x => x.Key == Isango.Entities.Enums.PassengerType.Child)?.FirstOrDefault())?.Value);
            var infantAges = Convert.ToInt32((ticketCriteria?.Ages?.Where(x => x.Key == Isango.Entities.Enums.PassengerType.Infant)?.FirstOrDefault())?.Value);
            var youthAges = Convert.ToInt32((ticketCriteria?.Ages?.Where(x => x.Key == Isango.Entities.Enums.PassengerType.Youth)?.FirstOrDefault())?.Value);

            //In HB infant,child, youth are child
            var noOfChildren = ticketCriteria?.NoOfPassengers?.Where(x => x.Key != Isango.Entities.Enums.PassengerType.Adult)?.ToList();

            var availXml = new XmlDocument();

            if (AppDomain.CurrentDomain.BaseDirectory.ToLowerInvariant().EndsWith(Constant.Release) || AppDomain.CurrentDomain.BaseDirectory.ToLowerInvariant().EndsWith(Constant.Debug))
                availXml.Load($"{Directory.GetParent((Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName).FullName)).FullName}{Constant.TicketAvailRqXml}");
            else
                availXml.Load($"{AppDomain.CurrentDomain.BaseDirectory}{Constant.TicketAvailRqXml}");

            var namespaceManager = new XmlNamespaceManager(availXml.NameTable);
            namespaceManager.AddNamespace(Constant.Hb, inputContext.NameSpace);

            var rq = availXml.SelectSingleNode(Constant.TicketAvailRq, namespaceManager);
            if (rq != null)
            {
                if (rq.Attributes != null)
                {
                    rq.Attributes[XmlAttribs.EchoToken].Value = Constant.IsangoHB;
                    rq.Attributes[XmlAttribs.SessionId].Value = DateTime.Now.ToString(Constant.RqDateFormat);

                    var language = availXml.SelectSingleNode(Constant.TicketAvailRqLanguage, namespaceManager);
                    if (language != null) language.InnerXml = inputContext.Language;

                    var usr = availXml.SelectSingleNode(Constant.TicketAvailRqCredentialsUser, namespaceManager);
                    if (usr != null) usr.InnerXml = inputContext.UserName;

                    var pwd = availXml.SelectSingleNode(Constant.TicketAvailRqCredentialsPassword,
                        namespaceManager);
                    if (pwd != null) pwd.InnerXml = inputContext.Password;

                    var chkIn = availXml.SelectSingleNode(Constant.TicketAvailRqDateFrom, namespaceManager);
                    if (chkIn?.Attributes != null)
                        chkIn.Attributes["date"].Value = inputContext.CheckinDate.ToString(Constant.CheckInOutFormat);

                    var chkOut = availXml.SelectSingleNode(Constant.TicketAvailRqDateTo, namespaceManager);
                    if (chkOut?.Attributes != null)
                        chkOut.Attributes["date"].Value = inputContext.CheckoutDate.ToString(Constant.CheckInOutFormat);

                    var destination = availXml.SelectSingleNode(Constant.TicketAvailRqDestination, namespaceManager);
                    if (destination?.Attributes != null)
                        destination.Attributes["code"].Value = inputContext.Destination;
                }

                if (inputContext.FactsheetIDs != null && inputContext.FactsheetIDs.Count > 0)
                {
                    var sb = new StringBuilder();
                    var factsheet = availXml.SelectSingleNode(Constant.TicketAvailRqFactsheetFilter, namespaceManager);
                    foreach (var fs in inputContext.FactsheetIDs)
                    {
                        sb.AppendFormat("{0}{1}{2}", Constant.StartFactsheet, fs, Constant.EndFactsheet);
                    }

                    if (factsheet != null) factsheet.InnerXml = sb.ToString();
                }
                else
                    // ReSharper disable once AssignNullToNotNullAttribute
                    rq.RemoveChild(availXml.SelectSingleNode(Constant.TicketAvailRqFactsheetFilter, namespaceManager));
            }

            var adults = availXml.SelectSingleNode(Constant.TicketAvailRqServiceOccupancyAdultCount, namespaceManager);
            if (adults != null) adults.InnerText = inputContext.Adults.ToString();

            var children = availXml.SelectSingleNode(Constant.TicketAvailRqServiceOccupancyChildCount, namespaceManager);
            if (children != null) children.InnerText = inputContext.Children.ToString();

            if (inputContext.Children > 0)
            {
                var rootNode = availXml.SelectSingleNode(Constant.TicketAvailRqServiceOccupancy, namespaceManager);
                var guestList = availXml.CreateNode(XmlNodeType.Element, Constant.GuestList, inputContext.NameSpace);

                for (var i = 0; i < inputContext.Adults; i++)
                {
                    var xEle = availXml.CreateElement(XmlNodes.Customer, inputContext.NameSpace);
                    xEle.SetAttribute(XmlAttribs.Type, Constant.Ad);
                    var xChild = availXml.CreateElement(XmlNodes.Age, inputContext.NameSpace);
                    xChild.InnerText = Constant.ChildInnerText;
                    xEle.AppendChild(xChild);
                    guestList.AppendChild(xEle);
                }

                if (noOfChildren != null)
                {
                    foreach (var item in noOfChildren)
                    {
                        for (int c = 0; c < item.Value; c++)
                        {
                            try
                            {
                                if (item.Key == Isango.Entities.Enums.PassengerType.Infant)
                                {
                                    AddChildWithAges(availXml, guestList, inputContext, infantAges.ToString());
                                }

                                if (item.Key == Isango.Entities.Enums.PassengerType.Child)
                                {
                                    AddChildWithAges(availXml, guestList, inputContext, childAges.ToString());
                                }

                                if (item.Key == Isango.Entities.Enums.PassengerType.Youth)
                                {
                                    AddChildWithAges(availXml, guestList, inputContext, youthAges.ToString());
                                }
                            }
                            catch
                            {
                                //Ignore
                            }
                        }
                    }
                }

                rootNode?.AppendChild(guestList);
            }

            return availXml.InnerXml;
        }

        protected override async Task<object> GetXmlResultsAsync(object inputXml)
        {
            // Create Input XML
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
                var xml = await client.PostAsync(values);
                return xml;
            }
            return null;
        }

        private static void AddChildWithAges(XmlDocument availXml, XmlNode guestList, InputContext inputContext, string age)
        {
            var xEle = availXml.CreateElement(XmlNodes.Customer, inputContext.NameSpace);
            xEle.SetAttribute(XmlAttribs.Type, Constant.Ch);
            var xChild = availXml.CreateElement(XmlNodes.Age, inputContext.NameSpace);
            xChild.InnerText = age;
            xEle.AppendChild(xChild);
            guestList.AppendChild(xEle);
        }
    }
}