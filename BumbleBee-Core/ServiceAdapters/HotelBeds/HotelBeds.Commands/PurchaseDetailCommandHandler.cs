using Logger.Contract;
using ServiceAdapters.HotelBeds.Constants;
using ServiceAdapters.HotelBeds.HotelBeds.Commands.Contracts;
using ServiceAdapters.HotelBeds.HotelBeds.Config;
using ServiceAdapters.HotelBeds.HotelBeds.Entities;
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
    public class PurchaseDetailCommandHandler : CommandHandlerBase, IPurchaseDetailCommandHandler
    {
        public PurchaseDetailCommandHandler(ILogger iLog) : base(iLog)
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
                purchaseCancelXml.Load($"{Directory.GetParent((Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName).FullName)).FullName}{Constant.PurchaseDetailRqXml}");
            else
                purchaseCancelXml.Load($"{AppDomain.CurrentDomain.BaseDirectory}{Constant.PurchaseDetailRqXml}");

            Parser.ResetParser();
            Parser.ParseXml(purchaseCancelXml.InnerXml);

            #region Credentials

            XmlOperations.SetInnerTextForElement(Constant.User, 0, Parser, inputContext.UserName);
            XmlOperations.SetInnerTextForElement(Constant.Password, 0, Parser, inputContext.Password);

            if (inputContext.Language != null)
            {
                XmlOperations.SetInnerTextForElement(Constant.Language, 0, Parser, inputContext.Language.ToUpperInvariant());
            }

            #endregion Credentials

            #region Purchase

            var purchaseNode = XmlOperations.GetElement(Constant.PurchaseReference, 0, Parser);
            XmlOperations.SetInnerTextForElement(XmlNodes.FileNumber, purchaseNode, 0, Parser, inputContext.HBSelectedProducts[0].FileNumber);
            XmlOperations.SetElementAttributeValue(XmlNodes.IncomingOffice, 0, purchaseNode, Parser, XmlAttribs.Code, inputContext.HBSelectedProducts[0].OfficeCode);

            #endregion Purchase

            return Parser.XmlDocument.InnerXml;
        }

        public override object GetResults(object xmlResults)
        {
            Parser.ResetParser();
            Parser.ParseXml(xmlResults.ToString());

            var purchaseData = new PurchaseConfirmRs();
            var purchase = new Purchase();

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

            purchaseData.AuditData = auditData;

            #endregion Audit Data

            purchase.TimeToExpiration = long.Parse(XmlOperations.GetElementAttributeValue(XmlNodes.Purchase, 0, Parser, XmlAttribs.TimeToExpiration), CultureInfo.InvariantCulture);
            purchase.PurchaseToken = XmlOperations.GetElementAttributeValue(XmlNodes.Purchase, 0, Parser, XmlAttribs.PurchaseToken);

            var agencyNode = XmlOperations.GetElement(XmlNodes.Agency, 0, Parser);
            purchase.AgencyBranch = XmlOperations.GetInnerTextForElement(XmlNodes.Branch, agencyNode, 0, Parser);
            purchase.AgencyCode = XmlOperations.GetInnerTextForElement(XmlNodes.Code, agencyNode, 0, Parser);

            purchase.AgencyReference = XmlOperations.GetInnerTextForElement(XmlNodes.AgencyReference, 0, Parser);
            purchase.Language = XmlOperations.GetInnerTextForElement(XmlNodes.Language, 0, Parser);

            purchase.Currency = XmlOperations.GetElementAttributeValue(XmlNodes.Currency, 0, Parser, XmlAttribs.Code);

            var purchaseNode = XmlOperations.GetElement(XmlNodes.Purchase, 0, Parser);

            purchase.Status = XmlOperations.GetInnerTextForElement(XmlNodes.Status, purchaseNode, 0, Parser);
            purchase.CreationDate = DateTime.ParseExact(XmlOperations.GetElementAttributeValue(XmlNodes.CreationDate, 0, purchaseNode, Parser, XmlAttribs.Date), Constant.CheckInOutFormat, CultureInfo.InvariantCulture);

            var purchaseReferenceNode = XmlOperations.GetElement(XmlNodes.Reference, purchaseNode, 0, Parser);
            purchase.FileNumber = XmlOperations.GetInnerTextForElement(XmlNodes.FileNumber, purchaseReferenceNode, 0, Parser);
            purchase.IncomingOfficeCode = XmlOperations.GetElementAttributeValue(XmlNodes.IncomingOffice, 0, purchaseReferenceNode, Parser, XmlAttribs.Code);

            var purchaseHolderNode = XmlOperations.GetElement(XmlNodes.Holder, purchaseNode, 0, Parser);
            purchase.HolderName = XmlOperations.GetInnerTextForElement(XmlNodes.Name, purchaseHolderNode, 0, Parser);
            purchase.HolderLastName = XmlOperations.GetInnerTextForElement(XmlNodes.LastName, purchaseHolderNode, 0, Parser);

            var servicesInResponse = new List<ServiceList>();
            var serviceListNode = XmlOperations.GetElement(XmlNodes.ServiceList, 0, Parser);
            var serviceNodes = Parser.GetAllElements(serviceListNode, XmlNodes.Service);

            const int svcCntr = 0;

            foreach (var serviceNode in serviceNodes)
            {
                var serviceForPurchase = new ServiceList
                {
                    Spui = XmlOperations.GetElementAttributeValue(XmlNodes.Service, svcCntr, Parser, XmlAttribs.Spui),
                    Status = XmlOperations.GetInnerTextForElement(XmlNodes.Status, serviceNode, 0, Parser)
                };

                #region Contract

                var contractListNode = XmlOperations.GetElement(XmlNodes.ContractList, serviceNode, 0, Parser);
                var contractNode = XmlOperations.GetElement(XmlNodes.Contract, contractListNode, 0, Parser);

                const int contractIndex = 0;

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
                var commentNodes = Parser.GetAllElements(contractNode, Constant.CommentList);

                for (var commentIndex = 0; commentIndex < commentNodes.Count; commentIndex++)
                {
                    comment.CommentText = XmlOperations.GetInnerTextForElement(XmlNodes.Comment, commentIndex, Parser);
                    comment.Type = XmlOperations.GetElementAttributeValue(XmlNodes.Comment, commentIndex, Parser, XmlAttribs.Type);
                    comments.Add(comment);
                }

                hotelContract.Comments = comments;
                serviceForPurchase.Contract = hotelContract;

                #endregion Contract

                #region Supplier

                var supplier = new HBSupplier
                {
                    Name = XmlOperations.GetElementAttributeValue(XmlNodes.Supplier, 0, serviceNode, Parser,
                        XmlAttribs.Name),
                    VatNumber = XmlOperations.GetElementAttributeValue(XmlNodes.Supplier, 0, serviceNode, Parser,
                        XmlAttribs.VatNumber)
                };
                serviceForPurchase.Supplier = supplier;

                #endregion Supplier

                serviceForPurchase.Currency = XmlOperations.GetElementAttributeValue(XmlNodes.Currency, 0, serviceNode, Parser, XmlAttribs.Code);
                serviceForPurchase.DateFrom = DateTime.ParseExact(XmlOperations.GetElementAttributeValue(XmlNodes.DateFrom, 0, serviceNode, Parser, XmlAttribs.Date), Constant.CheckInOutFormat, CultureInfo.InvariantCulture);
                serviceForPurchase.DateTo = DateTime.ParseExact(XmlOperations.GetElementAttributeValue(XmlNodes.DateTo, 0, serviceNode, Parser, XmlAttribs.Date), Constant.CheckInOutFormat, CultureInfo.InvariantCulture);

                #region Additional Costs

                var additionalCosts = new List<AdditionalCost>();
                var addCostListNode = XmlOperations.GetElement(XmlNodes.AdditionalCostList, 0, Parser);  // As there will always be only one hotel service to be added. Therefore no parent/chaining necessary.
                var addCostNodes = Parser.GetAllElements(addCostListNode, Constant.AdditionalCost);

                foreach (var addCostNode in addCostNodes)
                {
                    var addCost = new AdditionalCost
                    {
                        Type = XmlOperations.GetElementAttributeValue(XmlNodes.AdditionalCost, 0, Parser,
                            XmlAttribs.Type)
                    };
                    var priceNode = XmlOperations.GetElement(XmlNodes.Price, addCostNode, 0, Parser);
                    if (decimal.TryParse(XmlOperations.GetInnerTextForElement(XmlNodes.Amount, priceNode, 0, Parser), out var tempDecimalValue))
                        addCost.Amount = tempDecimalValue;
                    additionalCosts.Add(addCost);
                }
                serviceForPurchase.AdditionalCosts = additionalCosts;

                #endregion Additional Costs

                #region Modification Policies

                var modPolicies = new List<string>();
                var modPolicyListNode = XmlOperations.GetElement(XmlNodes.ModificationPolicyList, 0, Parser);  // As there will always be only one hotel service to be added. Therefore no parent/chaining necessary.
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

                var commentServiceListNodes = Parser.GetAllElements(serviceNode, Constant.CommentList);

                var serviceComments = new List<Comment>();

                var serviceCommentIndex = 0;

                foreach (var commentNode in commentServiceListNodes)
                {
                    var serviceComment = new Comment
                    {
                        CommentText = XmlOperations.GetInnerTextForElement(XmlNodes.Comment, commentNode,
                            serviceCommentIndex, Parser),
                        Type = XmlOperations.GetElementAttributeValue(XmlNodes.Comment, serviceCommentIndex,
                            commentNode, Parser, XmlAttribs.Type)
                    };
                    serviceComments.Add(serviceComment);
                    serviceCommentIndex++;
                }

                serviceForPurchase.Comments = serviceComments;

                var serviceReferenceNode = XmlOperations.GetElement(XmlNodes.Reference, serviceNode, 0, Parser);
                serviceForPurchase.FileNumber = XmlOperations.GetInnerTextForElement(XmlNodes.FileNumber, serviceReferenceNode, 0, Parser);
                serviceForPurchase.IncomingOfficeCode = XmlOperations.GetElementAttributeValue(XmlNodes.IncomingOffice, 0, serviceReferenceNode, Parser, XmlAttribs.Code);

                serviceForPurchase.TotalAmount = 0;

                #region Hotel Info

                var hInfo = new HotelInfo { Code = XmlOperations.GetInnerTextForNode(XmlNodes.Code, 0, Parser) };
                var hotelIndex = 0; // Only one hotel!
                                    //HOWTO: SCOPED Node Operations
                var hotelInfoNode = XmlOperations.GetElement(XmlNodes.HotelInfo, hotelIndex, Parser);
                hInfo.Name = XmlOperations.GetInnerTextForElement(XmlNodes.Name, hotelInfoNode, 0, Parser);

                var cat = new Category
                {
                    Code = XmlOperations.GetElementAttributeValue(XmlNodes.Category, hotelIndex, Parser,
                        XmlAttribs.Code),
                    Name = XmlOperations.GetInnerTextForNode(XmlNodes.Category, hotelIndex, Parser),
                    ShortName = XmlOperations.GetElementAttributeValue(XmlNodes.Category, hotelIndex, Parser,
                        XmlAttribs.Shortname)
                };
                hInfo.Category = cat;

                var dest = new Destination
                {
                    DestinationCode = XmlOperations.GetElementAttributeValue(XmlNodes.Destination, hotelIndex,
                        Parser, XmlAttribs.Code)
                };
                var destinationNode = XmlOperations.GetElement(XmlNodes.Destination, hotelIndex, Parser);
                dest.DestinationName = XmlOperations.GetInnerTextForElement(XmlNodes.Name, destinationNode, 0, Parser);
                dest.ZoneCode = XmlOperations.GetElementAttributeValue(XmlNodes.Zone, hotelIndex, Parser, XmlAttribs.Code);
                var zoneListNode = XmlOperations.GetElement(XmlNodes.ZoneList, hotelIndex, Parser);
                dest.ZoneName = XmlOperations.GetInnerTextForElement(XmlNodes.Zone, zoneListNode, 0, Parser);
                hInfo.Destination = dest;

                var pos = new Position
                {
                    Latitude = XmlOperations.GetElementAttributeValue(XmlNodes.Position, hotelIndex, Parser,
                        XmlAttribs.Latitude),
                    Longitude = XmlOperations.GetElementAttributeValue(XmlNodes.Position, hotelIndex, Parser,
                        XmlAttribs.Longitude)
                };
                hInfo.Position = pos;

                serviceForPurchase.HotelInfo = hInfo;

                #endregion Hotel Info

                #region Available Room

                const int roomCount = 0;
                var avlRoom = XmlOperations.GetElement(XmlNodes.AvailableRoom, 0, Parser);
                var avRoom = new AvailableRoom();
                var hOccupancy = new HotelOccupancy();

                var hotelOccupancyNode = XmlOperations.GetElement(XmlNodes.HotelOccupancy, roomCount, Parser);
                var occupancyNode = XmlOperations.GetElement(XmlNodes.Occupancy, roomCount, Parser);
                if (int.TryParse(XmlOperations.GetInnerTextForElement(XmlNodes.RoomCount, hotelOccupancyNode, 0, Parser), out var parsedInt))
                    hOccupancy.RoomCount = parsedInt;
                if (int.TryParse(XmlOperations.GetInnerTextForElement(XmlNodes.AdultCount, occupancyNode, 0, Parser), out parsedInt))
                    hOccupancy.AdultCount = parsedInt;
                if (int.TryParse(XmlOperations.GetInnerTextForElement(XmlNodes.ChildCount, occupancyNode, 0, Parser), out parsedInt))
                    hOccupancy.ChildCount = parsedInt;

                var guestListNode = XmlOperations.GetElement(Constant.GuestList, occupancyNode, 0, Parser);
                var customerNodes = Parser.GetAllElements(guestListNode, Constant.Customer);
                const int customerIndex = 0;

                var guests = customerNodes.Select(customerNode => new Guest
                {
                    Age = int.Parse(XmlOperations.GetInnerTextForElement(Constant.Age, customerNode, customerIndex, Parser), CultureInfo.InvariantCulture),
                    Id = int.Parse(XmlOperations.GetInnerTextForElement(Constant.CustomerId, customerNode, customerIndex, Parser), CultureInfo.InvariantCulture),
                    Type = XmlOperations.GetElementAttributeValue(Constant.Customer, customerIndex, guestListNode, Parser, XmlAttribs.Type)
                })
                    .ToList();
                hOccupancy.Guests = guests;
                avRoom.HotelOccupancy = hOccupancy;

                var room = new HotelRoom
                {
                    Shrui = XmlOperations.GetElementAttributeValue(XmlNodes.HotelRoom, roomCount, avlRoom, Parser,
                        XmlAttribs.Shrui),
                    AvailCount =
                        int.Parse(
                            XmlOperations.GetElementAttributeValue(XmlNodes.HotelRoom, roomCount, avlRoom, Parser,
                                XmlAttribs.AvailCount), CultureInfo.InvariantCulture),
                    Status = XmlOperations.GetElementAttributeValue(XmlNodes.HotelRoom, roomCount, avlRoom, Parser,
                        Constant.OnRequest)
                };

                var board = new Board();
                var hotelRoomNode = XmlOperations.GetElement(XmlNodes.HotelRoom, avlRoom, roomCount, Parser);
                board.Code = XmlOperations.GetElementAttributeValue(XmlNodes.Board, roomCount, hotelRoomNode, Parser, XmlAttribs.Code);

                board.Name = XmlOperations.GetInnerTextForElement(XmlNodes.Board, hotelRoomNode, roomCount, Parser);
                board.ShortName = XmlOperations.GetElementAttributeValue(XmlNodes.Board, roomCount, hotelRoomNode, Parser, XmlAttribs.Shortname);
                room.Board = board;

                var price = XmlOperations.GetElement(XmlNodes.Price, hotelRoomNode, roomCount, Parser);
                room.Amount = decimal.Parse(XmlOperations.GetInnerTextForElement(XmlNodes.Amount, price, roomCount, Parser), CultureInfo.InvariantCulture);

                var roomType = new RoomType
                {
                    Characteristic = XmlOperations.GetElementAttributeValue(XmlNodes.RoomType, roomCount,
                        hotelRoomNode, Parser, XmlAttribs.Characteristic),
                    Code = XmlOperations.GetElementAttributeValue(XmlNodes.RoomType, roomCount, hotelRoomNode,
                        Parser, XmlAttribs.Code),
                    Name = XmlOperations.GetInnerTextForElement(XmlNodes.RoomType, hotelRoomNode, roomCount, Parser)
                };
                room.RoomType = roomType;

                room.CancellationPolicy = null;
                var cancellationPolicy = new CancellationPolicy();
                var cancelPoliciesNode = XmlOperations.GetElement(Constant.CancellationPolicies, hotelRoomNode, 0, Parser);

                cancellationPolicy.Amount = decimal.Parse(XmlOperations.GetElementAttributeValue(Constant.CancellationPolicy, 0, cancelPoliciesNode, Parser, "amount"), CultureInfo.InvariantCulture);
                cancellationPolicy.FromDate = DateTime.ParseExact(XmlOperations.GetElementAttributeValue(Constant.CancellationPolicy, 0, cancelPoliciesNode, Parser, Constant.DateFrom), Constant.CheckInOutFormat, CultureInfo.InvariantCulture);
                cancellationPolicy.FromTime = int.Parse(XmlOperations.GetElementAttributeValue(Constant.CancellationPolicy, 0, cancelPoliciesNode, Parser, Constant.Time), CultureInfo.InvariantCulture);
                room.CancellationPolicy = cancellationPolicy;
                avRoom.HotelRoom = room;
                serviceForPurchase.AvailableRoom = avRoom;

                #endregion Available Room

                servicesInResponse.Add(serviceForPurchase);
            }
            purchase.ServiceList = servicesInResponse;

            #region PaymentData

            var paymentData = new PaymentData();
            var paymentDataNode = XmlOperations.GetElement(XmlNodes.PaymentData, purchaseNode, 0, Parser);
            var invoiceNode = XmlOperations.GetElement(XmlNodes.InvoiceCompany, paymentDataNode, 0, Parser);

            paymentData.InvoiceCompanyName = XmlOperations.GetInnerTextForElement(XmlNodes.Name, invoiceNode, 0, Parser);
            paymentData.InvoiceCompanyCode = XmlOperations.GetInnerTextForElement(XmlNodes.Code, invoiceNode, 0, Parser);
            paymentData.InvoiceCompanyRegistrationNumber = XmlOperations.GetInnerTextForElement(XmlNodes.RegistrationNumber, invoiceNode, 0, Parser);
            paymentData.Description = XmlOperations.GetInnerTextForElement(XmlNodes.Description, paymentDataNode, 0, Parser);
            paymentData.PaymentType = XmlOperations.GetElementAttributeValue(XmlNodes.PaymentType, 0, paymentDataNode, Parser, XmlAttribs.Code);
            purchase.PaymentData = paymentData;

            #endregion PaymentData

            purchaseData.Purchase = purchase;
            return purchaseData;
        }

        protected override Task<object> GetXmlResultsAsync(object inputXml)
        {
            return null;
        }
    }
}