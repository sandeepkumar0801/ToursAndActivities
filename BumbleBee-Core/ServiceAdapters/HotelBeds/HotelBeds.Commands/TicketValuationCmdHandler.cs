using Isango.Entities.Activities;
using Isango.Entities.Enums;
using Logger.Contract;
using ServiceAdapters.HotelBeds.Constants;
using ServiceAdapters.HotelBeds.HotelBeds.Commands.Contracts;
using ServiceAdapters.HotelBeds.HotelBeds.Config;
using ServiceAdapters.HotelBeds.HotelBeds.Entities;
using ServiceAdapters.HotelBeds.HotelBeds.Entities.Tickets;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Util;

namespace ServiceAdapters.HotelBeds.HotelBeds.Commands
{
    public class TicketValuationCmdHandler : CommandHandlerBase, ITicketValuationCmdHandler
    {
        public TicketValuationCmdHandler(ILogger iLog) : base(iLog)
        {
        }

        public override object GetResults(object xmlResults)
        {
            Parser.ResetParser();
            Parser.ParseXml(xmlResults.ToString());
            var valuation = new TicketValuationRs();

            if (XmlOperations.GetElement(XmlNodes.ErrorList, 0, Parser) == null)
            {
                #region Audit Data

                var auditDataNode = Parser.GetElement(XmlNodes.AuditData, 0);

                var auditData = new AuditData
                {
                    HydraCoreRelease =
                        XmlOperations.GetInnerTextForElement(XmlNodes.HydraCoreRelease, auditDataNode, 0, Parser),
                    HydraEnumerationsRelease = XmlOperations.GetInnerTextForElement(XmlNodes.HydraEnumerationsRelease,
                        auditDataNode, 0, Parser),
                    MerlinRelease =
                        XmlOperations.GetInnerTextForElement(XmlNodes.MerlinRelease, auditDataNode, 0, Parser),
                    ProcessTime =
                        int.Parse(XmlOperations.GetInnerTextForElement(XmlNodes.ProcessTime, auditDataNode, 0, Parser),
                            CultureInfo.InvariantCulture),
                    RequestHost = XmlOperations.GetInnerTextForElement(XmlNodes.RequestHost, auditDataNode, 0, Parser),
                    SchemaRelease =
                        XmlOperations.GetInnerTextForElement(XmlNodes.SchemaRelease, auditDataNode, 0, Parser),
                    ServerId = XmlOperations.GetInnerTextForElement(XmlNodes.ServerId, auditDataNode, 0, Parser),
                    ServerName = XmlOperations.GetInnerTextForElement(XmlNodes.ServerName, auditDataNode, 0, Parser),
                    Timestamp = DateTime.Parse(
                        XmlOperations.GetInnerTextForElement(XmlNodes.Timestamp, auditDataNode, 0, Parser),
                        CultureInfo.InvariantCulture)
                };
                valuation.AuditData = auditData;

                #endregion Audit Data

                valuation.EchoToken = XmlOperations.GetElementAttributeValue(XmlNodes.TicketValuationRs, 0, Parser, XmlAttribs.EchoToken);

                var serviceNode = Parser.GetElement(XmlNodes.ServiceTicket, 0);
                var svcCntr = 0;

                var serviceForPurchase = new ServiceList
                {
                    Spui = XmlOperations.GetElementAttributeValue(XmlNodes.ServiceTicket, svcCntr, Parser,
                        XmlAttribs.Spui),
                    Status = XmlOperations.GetInnerTextForElement(XmlNodes.Status, serviceNode, 0, Parser)
                };

                #region Contract

                var contractListNode = XmlOperations.GetElement(XmlNodes.ContractList, serviceNode, 0, Parser);
                var contractNode = XmlOperations.GetElement(XmlNodes.Contract, contractListNode, 0, Parser);

                var contractIndex = 0;
                var hotelContract = new Contract
                {
                    Name = XmlOperations.GetInnerTextForElement(XmlNodes.Name, contractNode, contractIndex, Parser),
                    IncomingOffice =
                        XmlOperations.GetElementAttributeValue(XmlNodes.IncomingOffice, 0, Parser, XmlAttribs.Code),
                    Classification =
                        XmlOperations.GetElementAttributeValue(XmlNodes.Classification, 0, Parser, XmlAttribs.Code)
                };

                var comments = new List<Comment>();
                var comment = new Comment();
                var commentNodes = Parser.GetAllElements(serviceNode, XmlNodes.CommentList);
                var commentIndex = 0;

                foreach (var commentNode in commentNodes)
                {
                    comment.CommentText = XmlOperations.GetInnerTextForElement(XmlNodes.Comment, commentNode, commentIndex, Parser);
                    comment.Type = XmlOperations.GetElementAttributeValue(XmlNodes.Comment, commentIndex, Parser, XmlAttribs.Type);
                    comments.Add(comment);
                }
                hotelContract.Comments = comments;
                serviceForPurchase.Contract = hotelContract;

                #endregion Contract

                serviceForPurchase.Currency = XmlOperations.GetElementAttributeValue(XmlNodes.Currency, 0, serviceNode, Parser, XmlAttribs.Code);

                serviceForPurchase.DateFrom = DateTime.ParseExact(XmlOperations.GetElementAttributeValue(XmlNodes.DateFrom, 0, serviceNode, Parser, XmlAttribs.Date), Constant.CheckInOutFormat, CultureInfo.InvariantCulture);
                serviceForPurchase.DateTo = DateTime.ParseExact(XmlOperations.GetElementAttributeValue(XmlNodes.DateTo, 0, serviceNode, Parser, XmlAttribs.Date), Constant.CheckInOutFormat, CultureInfo.InvariantCulture);

                #region Additional Costs

                var additionalCostListNode = XmlOperations.GetElement(XmlNodes.AdditionalCostList, serviceNode, 0, Parser);
                var additionalCostNodes = Parser.GetAllElements(additionalCostListNode, XmlNodes.AdditionalCost);
                serviceForPurchase.AdditionalCosts = new List<AdditionalCost>();
                var addindexer = 0;

                foreach (var itemAcost in additionalCostNodes)
                {
                    var priceNode = XmlOperations.GetElement(XmlNodes.Price, itemAcost, 0, Parser);
                    var acost = new AdditionalCost
                    {
                        Type = XmlOperations.GetElementAttributeValue(XmlNodes.AdditionalCost, addindexer++, Parser,
                            XmlAttribs.Type)
                    };
                    decimal.TryParse(XmlOperations.GetInnerTextForElement(XmlNodes.Amount, priceNode, 0, Parser), out var acostAmount);
                    acost.Amount = acostAmount;
                    serviceForPurchase.AdditionalCosts.Add(acost);
                }

                #endregion Additional Costs

                #region Modification Policies

                var modPolicies = new List<string>();
                var modPolicyListNode = XmlOperations.GetElement(XmlNodes.ModificationPolicyList, 0, Parser);
                if (modPolicyListNode != null)
                {
                    var modPolicyNodes = Parser.GetAllElements(modPolicyListNode);

                    for (var policyIndex = 0; policyIndex < modPolicyNodes.Count; policyIndex++)
                    {
                        modPolicies.Add(XmlOperations.GetInnerTextForElement(XmlNodes.ModificationPolicy, policyIndex, Parser));
                    }

                    serviceForPurchase.ModificationPolicy = modPolicies;
                }

                #endregion Modification Policies

                decimal.TryParse(XmlOperations.GetInnerTextForElement(XmlNodes.TotalAmount, serviceNode, 0, Parser), out var amt);
                serviceForPurchase.TotalAmount = amt;

                #region Ticket Info

                var ticketIndex = 0;
                var ticketInfoNode = XmlOperations.GetElement(XmlNodes.TicketInfo, serviceNode, ticketIndex, Parser);
                var tInfo = new TicketInfo
                {
                    Code = XmlOperations.GetInnerTextForElement(XmlNodes.Code, ticketInfoNode, 0, Parser),
                    Name = XmlOperations.GetInnerTextForElement(XmlNodes.Name, ticketInfoNode, 0, Parser),
                    CompanyCode =
                        XmlOperations.GetInnerTextForElement(XmlNodes.CompanyCode, ticketInfoNode, 0, Parser)
                };

                char.TryParse(XmlOperations.GetInnerTextForElement(XmlNodes.TicketClass, ticketInfoNode, 0, Parser), out var tInfoTicketClass);
                tInfo.TicketClass = tInfoTicketClass;
                var dest = new Destination
                {
                    DestinationCode = XmlOperations.GetElementAttributeValue(XmlNodes.Destination, ticketIndex,
                        ticketInfoNode, Parser, XmlAttribs.Code)
                };

                var destinationNode = XmlOperations.GetElement(XmlNodes.Destination, ticketIndex, Parser);
                dest.DestinationName = XmlOperations.GetInnerTextForElement(XmlNodes.Name, destinationNode, 0, Parser);

                tInfo.Destination = dest;
                serviceForPurchase.TicketInfo = tInfo;

                #endregion Ticket Info

                #region Available Modality

                var avlmodalityNode = XmlOperations.GetElement(XmlNodes.AvailableModality, serviceNode, 0, Parser);
                var avModality = new AvailableModality
                {
                    Code = XmlOperations.GetElementAttributeValue(XmlNodes.AvailableModality, 0, Parser,
                        XmlAttribs.Code),
                    Name = XmlOperations.GetElement(XmlNodes.Name, avlmodalityNode, 0, Parser).InnerText
                };

                var contract = new Contract();
                var avlContractNode = XmlOperations.GetElement(XmlNodes.Contract, avlmodalityNode, 0, Parser);
                contract.Name = XmlOperations.GetInnerTextForElement(XmlNodes.Name, avlContractNode, 0, Parser);
                contract.IncomingOffice = XmlOperations.GetElementAttributeValue(XmlNodes.IncomingOffice, 0, avlContractNode, Parser, XmlAttribs.Code);
                avModality.Contract = contract;

                var priceListNode = XmlOperations.GetElement(XmlNodes.PriceList, avlmodalityNode, 0, Parser);
                var priceNodes = Parser.GetAllElements(priceListNode, XmlNodes.Price);

                var prices = new List<TicketPrice>();
                foreach (var itemPrice in priceNodes)
                {
                    var price = new TicketPrice();
                    decimal.TryParse(XmlOperations.GetInnerTextForElement(XmlNodes.Amount, itemPrice, 0, Parser), out var avlPrice);
                    price.Amount = avlPrice;
                    price.Description = XmlOperations.GetInnerTextForElement(XmlNodes.Description, itemPrice, 0, Parser);
                    prices.Add(price);
                }
                avModality.PriceList = prices;

                var priceRangeListNode = XmlOperations.GetElement(XmlNodes.PriceRangeList, avlmodalityNode, 0, Parser);
                var priceRangeNodes = Parser.GetAllElements(priceRangeListNode, XmlNodes.PriceRange);

                var priceRanges = new List<PriceRange>();

                for (var index = 0; index < priceRangeNodes.Count; index++)
                {
                    var priceRange = new PriceRange
                    {
                        Type = XmlOperations.GetElementAttributeValue(XmlNodes.PriceRange, index, priceRangeListNode,
                            Parser, XmlAttribs.Type)
                    };
                    decimal.TryParse(XmlOperations.GetElementAttributeValue(XmlNodes.PriceRange, index, priceRangeListNode, Parser, XmlAttribs.UnitPrice), out var unitPrice);
                    priceRange.UnitPrice = unitPrice;
                    priceRanges.Add(priceRange);
                }
                avModality.PriceRangeList = priceRanges;

                #endregion Available Modality

                serviceForPurchase.AvailableModality = avModality;

                #region Paxes

                var paxes = new Paxes();
                var paxesNode = XmlOperations.GetElement(XmlNodes.Paxes, serviceNode, 0, Parser);
                int.TryParse(XmlOperations.GetInnerTextForElement(XmlNodes.AdultCount, paxesNode, 0, Parser), out var adultCount);
                int.TryParse(XmlOperations.GetInnerTextForElement(XmlNodes.ChildCount, paxesNode, 0, Parser), out var childCount);
                paxes.AdultCount = adultCount;
                paxes.ChildCount = childCount;

                var guestList = new GuestList();

                var guestListNode = XmlOperations.GetElement(XmlNodes.GuestList, paxesNode, 0, Parser);
                var customers = new List<Customer>();
                if (guestListNode != null)
                {
                    var customerNodes = Parser.GetAllElements(guestListNode, XmlNodes.Customer);
                    var indexer = 0;
                    foreach (var itemCustomer in customerNodes)
                    {
                        var customer = new Customer();
                        int.TryParse(XmlOperations.GetInnerTextForElement(XmlNodes.Age, itemCustomer, 0, Parser), out var customerAge);
                        customer.Type = XmlOperations.GetElementAttributeValue(XmlNodes.Customer, indexer++, guestListNode, Parser, XmlAttribs.Type);
                        customer.Age = customerAge;
                        customers.Add(customer);
                    }
                    guestList.CustomerList = customers;
                }

                paxes.GuestList = guestList;
                serviceForPurchase.PassengerDetails = paxes;

                #endregion Paxes

                var cancelPolicyNode = XmlOperations.GetElement(Constant.CancellationPolicyList, serviceNode, 0, Parser);
                if (cancelPolicyNode != null)
                {
                    var cancellationPrices = Parser.GetAllElements(cancelPolicyNode, XmlNodes.Price);
                    var charges = cancellationPrices.Select(cancelPolicyPriceNode => new CancellationPolicy
                    {
                        Amount = decimal.Parse(XmlOperations.GetInnerTextForElement(XmlNodes.Amount, cancelPolicyPriceNode, 0, Parser), CultureInfo.InvariantCulture),
                        FromDate = DateTime.ParseExact(XmlOperations.GetElementAttributeValue(Constant.DateTimeFrom, 0, cancelPolicyPriceNode, Parser, XmlAttribs.Date), Constant.CheckInOutFormat, CultureInfo.InvariantCulture),
                        ToDate = DateTime.ParseExact(XmlOperations.GetElementAttributeValue(Constant.DateTimeTo, 0, cancelPolicyPriceNode, Parser, XmlAttribs.Date), Constant.CheckInOutFormat, CultureInfo.InvariantCulture)
                    })
                        .ToList();

                    serviceForPurchase.CancellationPolicy = charges[0];
                    serviceForPurchase.CancellationCharges = charges;
                }

                var serviceDetailListNode = XmlOperations.GetElement(Constant.ServiceDetailList, serviceNode, 0, Parser);
                if (serviceDetailListNode != null)
                {
                    var serviceDetailsElements = Parser.GetAllElements(serviceDetailListNode, XmlNodes.ServiceDetail);
                    var counter = 0;
                    var serviceDetails = serviceDetailsElements.Select(serviceDetailNode => new ServiceDetail
                    {
                        Code = XmlOperations.GetElementAttributeValue(Constant.ServiceDetail, counter++, serviceDetailListNode, Parser, XmlAttribs.Code),
                        Name = XmlOperations.GetInnerTextForElement(XmlNodes.Name, serviceDetailNode, 0, Parser)
                    })
                        .ToList();

                    serviceForPurchase.ServiceDetails = serviceDetails;
                }

                valuation.ServiceTicket = serviceForPurchase;

                return valuation;
            }

            return null;
        }

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
                return client.Post(values);
            }
            return null;
        }

        protected override object CreateInputRequest(InputContext inputContext)
        {
            var serviceAddXml = new XmlDocument();
            if (AppDomain.CurrentDomain.BaseDirectory.ToLowerInvariant().EndsWith(Constant.Release) || AppDomain.CurrentDomain.BaseDirectory.ToLowerInvariant().EndsWith(Constant.Debug))
                serviceAddXml.Load($"{Directory.GetParent((Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName).FullName)).FullName}{Constant.TicketValuationRqXml}");
            else
                serviceAddXml.Load($"{AppDomain.CurrentDomain.BaseDirectory}{Constant.TicketValuationRqXml}");

            var tkt2Add = inputContext.HBSelectedProducts[0];
            if (tkt2Add == null)
                throw new InvalidDataException(Constant.ExceptionMsg1);

            var activityOption = (ActivityOption)tkt2Add.ProductOptions.Find(op => op.IsSelected.Equals(true));

            var customers = activityOption.Customers;//.FindAll(cus => cus.PassengerType.Equals(PassengerType.Adult) || cus.PassengerType.Equals(PassengerType.Child));

            var avlToken = activityOption.AvailToken;

            Parser.ResetParser();
            Parser.ParseXml(serviceAddXml.InnerXml);

            XmlOperations.SetElementAttributeValue(XmlNodes.TicketValuationRq, 0, Parser, XmlAttribs.EchoToken, Constant.IsangoHB);

            #region Credentials

            XmlOperations.SetInnerTextForElement(Constant.User, 0, Parser, inputContext.UserName);
            XmlOperations.SetInnerTextForElement(Constant.Password, 0, Parser, inputContext.Password);

            #endregion Credentials

            if (inputContext.Language != null)
            {
                XmlOperations.SetInnerTextForElement(Constant.Language, 0, Parser, inputContext.Language.ToUpperInvariant());
            }

            XmlOperations.SetInnerTextForElement(Constant.AvailToken, 0, Parser, avlToken);

            #region Date

            var dateFrom = activityOption.TravelInfo.StartDate;
            var dateTo = dateFrom.AddDays(activityOption.TravelInfo.NumberOfNights);
            // DateFrom
            XmlOperations.SetElementAttributeValue(XmlNodes.DateFrom, 0, Parser, XmlAttribs.Date, dateFrom.ToString(Constant.CheckInOutFormat));
            // DateTo
            XmlOperations.SetElementAttributeValue(XmlNodes.DateTo, 0, Parser, XmlAttribs.Date, dateTo.ToString(Constant.CheckInOutFormat));

            #endregion Date

            XmlOperations.SetInnerTextForElement(XmlNodes.TicketCode, 0, Parser, tkt2Add.Code);
            XmlOperations.SetInnerTextForElement(XmlNodes.ModalityCode, 0, Parser, activityOption.Code);

            #region ServiceOccupancy

            var paxesNode = XmlOperations.GetElement(XmlNodes.ServiceOccupancy, 0, Parser);

            XmlOperations.SetInnerTextForElement(XmlNodes.AdultCount, paxesNode, 0, Parser, activityOption.TravelInfo.NoOfPassengers.FirstOrDefault(x => x.Key == PassengerType.Adult).Value.ToString());
            XmlOperations.SetInnerTextForElement(XmlNodes.ChildCount, paxesNode, 0, Parser, activityOption.TravelInfo.NoOfPassengers.Where(x => x.Key != PassengerType.Adult).Sum(y => y.Value).ToString());

            XmlOperations.SetInnerXmlForElement(Constant.GuestList, paxesNode, 0, Parser, GetXMLForCustomers(customers));

            #endregion ServiceOccupancy

            return Parser.XmlDocument.InnerXml;
        }

        protected override Task<object> GetXmlResultsAsync(object inputXml)
        {
            return Task.FromResult<object>(null);
        }
    }
}