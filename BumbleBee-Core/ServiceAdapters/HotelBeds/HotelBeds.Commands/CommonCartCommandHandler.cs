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
using Comment = ServiceAdapters.HotelBeds.HotelBeds.Entities.Comment;

namespace ServiceAdapters.HotelBeds.HotelBeds.Commands
{
    public class CommonCartCommandHandler : CommandHandlerBase, ICommonCartCommandHandler
    {
        public CommonCartCommandHandler(ILogger iLog) : base(iLog)
        {
        }

        public override object GetResults(object xmlResults)
        {
            var lstSpui = new List<string>();
            Parser.ResetParser();
            Parser.ParseXml(xmlResults.ToString());

            var cartData = new ServiceAddRs();

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
            cartData.AuditData = auditData;

            #endregion Audit Data

            cartData.EchoToken = XmlOperations.GetElementAttributeValue(XmlNodes.ServiceAddRs, 0, Parser, XmlAttribs.EchoToken);

            var purchase = new Purchase
            {
                PurchaseToken = XmlOperations.GetElementAttributeValue(XmlNodes.Purchase, 0, Parser, XmlAttribs.PurchaseToken),
                TimeToExpiration =
                    int.Parse(
                        XmlOperations.GetElementAttributeValue(XmlNodes.Purchase, 0, Parser,
                            XmlAttribs.TimeToExpiration), CultureInfo.InvariantCulture)
            };

            var agencyNode = XmlOperations.GetElement(XmlNodes.Agency, 0, Parser);
            purchase.AgencyBranch = XmlOperations.GetInnerTextForElement(XmlNodes.Branch, agencyNode, 0, Parser);
            purchase.AgencyCode = XmlOperations.GetInnerTextForElement(XmlNodes.Code, agencyNode, 0, Parser);
            purchase.AgencyReference = "";
            purchase.Language = XmlOperations.GetInnerTextForElement(XmlNodes.Language, 0, Parser);
            purchase.Currency = XmlOperations.GetElementAttributeValue(XmlNodes.Currency, 0, Parser, XmlAttribs.Code);

            var purchaseNode = XmlOperations.GetElement(XmlNodes.Purchase, 0, Parser);
            purchase.Status = XmlOperations.GetInnerTextForElement(XmlNodes.Status, purchaseNode, 0, Parser);

            decimal.TryParse(XmlOperations.GetInnerTextForElement(XmlNodes.TotalPrice, purchaseNode, 0, Parser), out var totalAmount);
            purchase.TotalAmount = totalAmount;
            cartData.Purchase = purchase;

            #region ServiceList

            var servicesInResponse = new List<ServiceList>();
            var serviceListNode = XmlOperations.GetElement(XmlNodes.ServiceList, 0, Parser);
            var serviceNodes = Parser.GetAllElements(serviceListNode, XmlNodes.Service);
            var svcCntr = 0;
            foreach (var serviceNode in serviceNodes)
            {
                var serviceForPurchase = new ServiceList
                {
                    Spui = XmlOperations.GetElementAttributeValue(XmlNodes.Service, svcCntr, Parser,
                        XmlAttribs.Spui)
                };

                lstSpui.Add(serviceForPurchase.Spui);
                serviceForPurchase.Status = XmlOperations.GetInnerTextForElement(XmlNodes.Status, serviceNode, 0, Parser);

                #region Supplier

                var hbSupplier = new HBSupplier
                {
                    Name = XmlOperations.GetElementAttributeValue(XmlNodes.Supplier, 0, serviceNode, Parser,
                        XmlAttribs.Name),
                    VatNumber = XmlOperations.GetElementAttributeValue(XmlNodes.Supplier, 0, serviceNode, Parser,
                        XmlAttribs.VatNumber)
                };
                serviceForPurchase.Supplier = hbSupplier;

                #endregion Supplier

                #region Additional Costs

                var additionalCosts = new List<AdditionalCost>();
                var addCostListNode = XmlOperations.GetElement(XmlNodes.AdditionalCostList, 0, Parser);
                var addCostNodes = Parser.GetAllElements(addCostListNode, XmlNodes.AdditionalCost);

                var costIndexer = 0;
                foreach (var addCostNode in addCostNodes)
                {
                    var addCost = new AdditionalCost
                    {
                        Type = XmlOperations.GetElementAttributeValue(XmlNodes.AdditionalCost, costIndexer++,
                            Parser, XmlAttribs.Type)
                    };
                    var priceNode = XmlOperations.GetElement(XmlNodes.Price, addCostNode, 0, Parser);
                    if (decimal.TryParse(XmlOperations.GetInnerTextForElement(XmlNodes.Amount, priceNode, 0, Parser), out var tempDecimalValue))
                        addCost.Amount = tempDecimalValue;
                    additionalCosts.Add(addCost);
                }
                serviceForPurchase.AdditionalCosts = additionalCosts;

                #endregion Additional Costs

                #region ModPolicies

                var modPolicies = new List<string>();
                var modPolicyListNode = XmlOperations.GetElement(XmlNodes.ModificationPolicyList, 0, Parser);
                if (modPolicyListNode != null)
                {
                    var modPolicyNodes = Parser.GetAllElements(modPolicyListNode);
                    var policyIndex = 0;
                    modPolicies.AddRange(modPolicyNodes.Select(modPolicyNode => XmlOperations.GetInnerTextForElement(XmlNodes.ModificationPolicy, policyIndex++, Parser)));
                    serviceForPurchase.ModificationPolicy = modPolicies;
                }

                #endregion ModPolicies

                serviceForPurchase.DateFrom = DateTime.ParseExact(XmlOperations.GetElementAttributeValue(XmlNodes.DateFrom, 0, serviceNode, Parser, XmlAttribs.Date), Constant.CheckInOutFormat, CultureInfo.InvariantCulture);
                serviceForPurchase.DateTo = DateTime.ParseExact(XmlOperations.GetElementAttributeValue(XmlNodes.DateTo, 0, serviceNode, Parser, XmlAttribs.Date), Constant.CheckInOutFormat, CultureInfo.InvariantCulture);

                serviceForPurchase.Currency = XmlOperations.GetElementAttributeValue(XmlNodes.Currency, 0, serviceNode, Parser, XmlAttribs.Code);
                decimal.TryParse(XmlOperations.GetInnerTextForElement(XmlNodes.TotalAmount, serviceNode, 0, Parser), out var totalAmt);
                serviceForPurchase.TotalAmount = totalAmt;

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
                var dest = new Destination();
                var destinationNodeTkt = XmlOperations.GetElement(XmlNodes.Destination, ticketInfoNode, 0, Parser);
                if (destinationNodeTkt != null)
                {
                    dest.DestinationCode = XmlOperations.GetElementAttributeValue(XmlNodes.Destination, 0, ticketInfoNode, Parser, XmlAttribs.Code);
                    dest.DestinationName = XmlOperations.GetInnerTextForElement(XmlNodes.Name, destinationNodeTkt, 0, Parser);

                    tInfo.Destination = dest;
                }
                serviceForPurchase.TicketInfo = tInfo;

                #endregion Ticket Info

                var contractListNode = XmlOperations.GetElement(XmlNodes.ContractList, serviceNode, 0, Parser);
                var contractNode = XmlOperations.GetElement(XmlNodes.Contract, contractListNode, 0, Parser);

                var contractIndex = 0;
                var serviceContract = new Contract
                {
                    Name = XmlOperations.GetInnerTextForElement(XmlNodes.Name, contractNode, contractIndex, Parser),
                    IncomingOffice =
                        XmlOperations.GetElementAttributeValue(XmlNodes.IncomingOffice, 0, Parser, XmlAttribs.Code)
                };

                serviceForPurchase.Contract = serviceContract;

                #region Comment section

                var commentNodes = Parser.GetAllElements(serviceNode, XmlNodes.CommentList);
                serviceForPurchase.Comments = new List<Comment>();
                foreach (var itemComment in commentNodes)
                {
                    var comment = new Comment
                    {
                        CommentText =
                            XmlOperations.GetInnerTextForElement(XmlNodes.Comment, itemComment, 0, Parser),
                        Type = XmlOperations.GetElementAttributeValue(XmlNodes.Comment, 0, Parser, XmlAttribs.Type)
                    };
                    serviceForPurchase.Comments.Add(comment);
                }

                #endregion Comment section

                #region Available Modality

                var avlmodalityNode = XmlOperations.GetElement(XmlNodes.AvailableModality, serviceNode, 0, Parser);
                var avModality = new AvailableModality
                {
                    Code = XmlOperations.GetElementAttributeValue(XmlNodes.AvailableModality, 0, Parser,
                        XmlAttribs.Code),
                    Name = XmlOperations.GetInnerTextForElement(XmlNodes.Name, avlmodalityNode, 0, Parser)
                };

                var aVcontract = new Contract();
                var avlContractNode = XmlOperations.GetElement(XmlNodes.Contract, avlmodalityNode, 0, Parser);
                aVcontract.Name = XmlOperations.GetInnerTextForElement(XmlNodes.Name, avlContractNode, 0, Parser);
                aVcontract.IncomingOffice = XmlOperations.GetElementAttributeValue(XmlNodes.IncomingOffice, 0, avlContractNode, Parser, XmlAttribs.Code);
                avModality.Contract = aVcontract;

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

                var priceRangeListNode = XmlOperations.GetElement(Constant.PriceRangeList, avlmodalityNode, 0, Parser);
                var priceRangeNodes = Parser.GetAllElements(priceRangeListNode, XmlNodes.PriceRange);

                var priceRanges = new List<PriceRange>();

                for (var index = 0; index < priceRangeNodes.Count; index++)
                {
                    var priceRange = new PriceRange
                    {
                        Type = XmlOperations.GetElementAttributeValue(XmlNodes.PriceRange, index,
                            priceRangeListNode, Parser, XmlAttribs.Type)
                    };
                    decimal.TryParse(XmlOperations.GetElementAttributeValue(XmlNodes.PriceRange, index, priceRangeListNode, Parser, XmlAttribs.UnitPrice), out var unitPrice);
                    priceRange.UnitPrice = unitPrice;
                    priceRanges.Add(priceRange);
                }
                avModality.PriceRangeList = priceRanges;
                serviceForPurchase.AvailableModality = avModality;

                #endregion Available Modality

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
                    int customerAge;
                    foreach (var itemCustomer in customerNodes)
                    {
                        var customer = new Customer();

                        int.TryParse(XmlOperations.GetInnerTextForElement(XmlNodes.CustomerId, itemCustomer, 0, Parser), out var customerId);
                        int.TryParse(XmlOperations.GetInnerTextForElement(XmlNodes.Age, itemCustomer, 0, Parser), out customerAge);

                        customer.Id = customerId;
                        customer.Type = XmlOperations.GetElementAttributeValue(XmlNodes.Customer, indexer++, guestListNode, Parser, XmlAttribs.Type);
                        customer.Age = customerAge;
                        customer.Name = XmlOperations.GetInnerTextForElement(XmlNodes.Name, itemCustomer, 0, Parser);
                        customer.LastName = XmlOperations.GetInnerTextForElement(XmlNodes.LastName, itemCustomer, 0, Parser);
                        customers.Add(customer);
                    }
                    guestList.CustomerList = customers;
                }

                paxes.GuestList = guestList;
                serviceForPurchase.PassengerDetails = paxes;

                #endregion Paxes

                #region Cancellation Policy

                var cancelPolicyNodeTkt = XmlOperations.GetElement(Constant.CancellationPolicyList, serviceNode, 0, Parser);
                var cancellationPrices = new List<XmlElement>();

                if (cancelPolicyNodeTkt != null)
                {
                    cancellationPrices = Parser.GetAllElements(cancelPolicyNodeTkt, XmlNodes.Price);
                }
                else
                {
                    var cancelPolicySingle = XmlOperations.GetElement(Constant.CancellationPolicy, serviceNode, 0, Parser);
                    if (cancelPolicySingle != null)
                    {
                        cancellationPrices.Add(cancelPolicySingle);
                    }
                }
                var charges = new List<CancellationPolicy>();
                foreach (var cancelPolicyPriceNode in cancellationPrices)
                {
                    var cancellationPolicy = new CancellationPolicy
                    {
                        Amount = decimal.Parse(
                            XmlOperations.GetInnerTextForElement(XmlNodes.Amount, cancelPolicyPriceNode, 0,
                                Parser), CultureInfo.InvariantCulture),
                        FromDate = DateTime.ParseExact(
                            XmlOperations.GetElementAttributeValue(Constant.DateTimeFrom, 0,
                                cancelPolicyPriceNode, Parser, XmlAttribs.Date), Constant.CheckInOutFormat,
                            CultureInfo.InvariantCulture),
                        ToDate = DateTime.ParseExact(
                            XmlOperations.GetElementAttributeValue(Constant.DateTimeTo, 0,
                                cancelPolicyPriceNode, Parser, XmlAttribs.Date), Constant.CheckInOutFormat,
                            CultureInfo.InvariantCulture)
                    };
                    charges.Add(cancellationPolicy);
                }
                if (charges != null && charges.Count > 0)
                    serviceForPurchase.CancellationPolicy = charges[0];
                serviceForPurchase.CancellationCharges = charges;

                #endregion Cancellation Policy

                svcCntr++;
                servicesInResponse.Add(serviceForPurchase);
            }
            purchase.ServiceList = servicesInResponse;

            #endregion ServiceList

            cartData.Purchase = purchase;
            return cartData;
        }

        protected override object CreateInputRequest(InputContext inputContext)
        {
            #region Ticket

            var serviceAddXml = new XmlDocument();
            if (AppDomain.CurrentDomain.BaseDirectory.ToLowerInvariant().EndsWith(Constant.Release) || AppDomain.CurrentDomain.BaseDirectory.ToLowerInvariant().EndsWith(Constant.Debug))
                serviceAddXml.Load($"{Directory.GetParent((Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName).FullName)).FullName}{Constant.ServiceAddTicketXml}");
            else
                serviceAddXml.Load($"{AppDomain.CurrentDomain.BaseDirectory}{Constant.ServiceAddTicketXml}");

            var tkt2Add = inputContext.HBSelectedProducts[0];
            if (tkt2Add == null)
                throw new InvalidDataException(Constant.ExceptionMsg1);

            #region Find Ticket To Be Added

            var cartId = string.Empty;
            if (!string.IsNullOrEmpty(tkt2Add.PurchaseToken))
            {
                cartId = tkt2Add.PurchaseToken;
            }

            #endregion Find Ticket To Be Added

            var activityOption = (ActivityOption)tkt2Add.ProductOptions.Find(op => op.IsSelected.Equals(true));

            var customers = activityOption.Customers;//.FindAll(cus => cus.PassengerType.Equals(PassengerType.Adult) || cus.PassengerType.Equals(PassengerType.Child));

            Parser.ResetParser();
            Parser.ParseXml(serviceAddXml.InnerXml);

            XmlOperations.SetElementAttributeValue(XmlNodes.ServiceAddRq, 0, Parser, XmlAttribs.EchoToken, Constant.IsangoHB);

            if (!string.IsNullOrEmpty(cartId))
            {
                XmlOperations.SetElementAttributeValue(XmlNodes.ServiceAddRq, 0, Parser, XmlAttribs.PurchaseToken, cartId);
            }
            else
            {
                XmlOperations.RemoveAttribute(XmlNodes.ServiceAddRq, 0, Parser, XmlAttribs.PurchaseToken);
            }

            #region Credentials

            XmlOperations.SetInnerTextForElement(Constant.User, 0, Parser, inputContext.UserName);
            XmlOperations.SetInnerTextForElement(Constant.Password, 0, Parser, inputContext.Password);

            #endregion Credentials

            if (inputContext.Language != null)
            {
                XmlOperations.SetInnerTextForElement(Constant.Language, 0, Parser, inputContext.Language.ToUpperInvariant());
            }

            XmlOperations.SetElementAttributeValue(XmlNodes.Service, 0, Parser, XmlAttribs.AvailToken, activityOption.AvailToken);

            #region Contract

            var contractNode = XmlOperations.GetElement(XmlNodes.Contract, 0, Parser);
            XmlOperations.SetInnerTextForElement(XmlNodes.Name, contractNode, 0, Parser, activityOption.Contract.Name);
            XmlOperations.SetElementAttributeValue(XmlNodes.IncomingOffice, 0, contractNode, Parser, XmlAttribs.Code, activityOption.Contract.InComingOfficeCode);

            var contractInModalityNode = XmlOperations.GetElement(XmlNodes.Contract, 1, Parser);
            XmlOperations.SetInnerTextForElement(XmlNodes.Name, contractInModalityNode, 0, Parser, activityOption.Contract.Name);
            XmlOperations.SetElementAttributeValue(XmlNodes.IncomingOffice, 0, contractInModalityNode, Parser, XmlAttribs.Code, activityOption.Contract.InComingOfficeCode);

            #endregion Contract

            #region shoppingCartID

            var namespaceManager = new XmlNamespaceManager(serviceAddXml.NameTable);
            namespaceManager.AddNamespace(Constant.Hb, inputContext.NameSpace);
            if (tkt2Add.ActivityType.Equals(ActivityType.Theatre))
            {
                var seatingShoppingCartId = XmlOperations.GetElement(XmlNodes.SeatingShoppingCartId, 0, Parser);
                if (seatingShoppingCartId != null)
                {
                    XmlOperations.SetInnerTextForElement(XmlNodes.SeatingShoppingCartId, 0, Parser, tkt2Add.ShoppingCartId);
                }
            }
            else
            {
                var serviceNode = serviceAddXml.SelectSingleNode(Constant.ServiceAddRqHbService, namespaceManager);
                var shoppingCartId = serviceNode?.SelectSingleNode(Constant.SeatingShoppingCartId, namespaceManager);
                if (shoppingCartId != null)
                {
                    serviceNode.RemoveChild(shoppingCartId);
                    XmlOperations.SetInnerXmlForElement(XmlNodes.Service, 0, Parser, serviceNode.InnerXml);
                }
            }

            #endregion shoppingCartID

            #region Date

            var dateFrom = activityOption.TravelInfo.StartDate;
            var dateTo = dateFrom.AddDays(activityOption.TravelInfo.NumberOfNights);
            // DateFrom
            XmlOperations.SetElementAttributeValue(XmlNodes.DateFrom, 0, Parser, XmlAttribs.Date, dateFrom.ToString(Constant.CheckInOutFormat));
            // DateTo
            XmlOperations.SetElementAttributeValue(XmlNodes.DateTo, 0, Parser, XmlAttribs.Date, dateTo.ToString(Constant.CheckInOutFormat));

            #endregion Date

            #region Service

            var ticketInfoNode = XmlOperations.GetElement(XmlNodes.TicketInfo, 0, Parser);
            XmlOperations.SetInnerTextForElement(XmlNodes.Code, ticketInfoNode, 0, Parser, tkt2Add.Code);
            XmlOperations.SetElementAttributeValue(XmlNodes.Destination, 0, ticketInfoNode, Parser, XmlAttribs.Code, tkt2Add.Destination);

            #endregion Service

            #region AvailableModality

            var avlModalityNode = XmlOperations.GetElement(XmlNodes.AvailableModality, 0, Parser);
            var paxesNode = XmlOperations.GetElement(XmlNodes.Paxes, 0, Parser);

            XmlOperations.SetInnerTextForElement(XmlNodes.Name, avlModalityNode, 0, Parser, activityOption.Name);
            XmlOperations.SetElementAttributeValue(XmlNodes.AvailableModality, 0, Parser, XmlAttribs.Code, activityOption.Code);

            XmlOperations.SetInnerTextForElement(XmlNodes.AdultCount, paxesNode, 0, Parser, activityOption.TravelInfo.NoOfPassengers?.Where(x => x.Key == PassengerType.Adult).Select(s => s.Value).FirstOrDefault().ToString());

            XmlOperations.SetInnerTextForElement(XmlNodes.ChildCount, paxesNode, 0, Parser, activityOption.TravelInfo.NoOfPassengers?.Where(x => x.Key != PassengerType.Adult).Sum(y => y.Value).ToString());

            XmlOperations.SetInnerXmlForElement(Constant.GuestList, paxesNode, 0, Parser, GetXMLForCustomers(customers));

            #endregion AvailableModality

            return Parser.XmlDocument.InnerXml;

            #endregion Ticket
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

        protected override Task<object> GetXmlResultsAsync(object inputXml)
        {
            return null;
        }
    }
}