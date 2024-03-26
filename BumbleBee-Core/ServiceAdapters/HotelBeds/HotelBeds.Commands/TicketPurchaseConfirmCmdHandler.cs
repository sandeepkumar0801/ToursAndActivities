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
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Util;
using IsangoEntities = Isango.Entities;

namespace ServiceAdapters.HotelBeds.HotelBeds.Commands
{
    public class TicketPurchaseConfirmCmdHandler : CommandHandlerBase, ITicketPurchaseConfirmCmdHandler
    {
        public TicketPurchaseConfirmCmdHandler(ILogger iLog) : base(iLog)
        {
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
                HydraEnumerationsRelease =
                    XmlOperations.GetInnerTextForElement(XmlNodes.HydraEnumerationsRelease, auditDataNode, 0, Parser),
                MerlinRelease = XmlOperations.GetInnerTextForElement(XmlNodes.MerlinRelease, auditDataNode, 0, Parser),
                ProcessTime =
                    int.Parse(XmlOperations.GetInnerTextForElement(XmlNodes.ProcessTime, auditDataNode, 0, Parser),
                        CultureInfo.InvariantCulture),
                RequestHost = XmlOperations.GetInnerTextForElement(XmlNodes.RequestHost, auditDataNode, 0, Parser),
                SchemaRelease = XmlOperations.GetInnerTextForElement(XmlNodes.SchemaRelease, auditDataNode, 0, Parser),
                ServerId = XmlOperations.GetInnerTextForElement(XmlNodes.ServerId, auditDataNode, 0, Parser),
                ServerName = XmlOperations.GetInnerTextForElement(XmlNodes.ServerName, auditDataNode, 0, Parser),
                Timestamp = DateTime.Parse(
                    XmlOperations.GetInnerTextForElement(XmlNodes.Timestamp, auditDataNode, 0, Parser),
                    CultureInfo.InvariantCulture)
            };
            purchaseData.AuditData = auditData;

            #endregion Audit Data

            purchaseData.EchoToken = XmlOperations.GetElementAttributeValue(XmlNodes.PurchaseConfirmRs, 0, Parser, XmlAttribs.EchoToken);

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

            #region ServiceList

            var servicesInResponse = new List<ServiceList>();
            var serviceListNode = XmlOperations.GetElement(XmlNodes.ServiceList, 0, Parser);
            var serviceNodes = Parser.GetAllElements(serviceListNode, XmlNodes.Service);
            var svcCntr = 0;
            foreach (var serviceNode in serviceNodes)
            {
                var serviceForPurchase = new ServiceList
                {
                    Spui = XmlOperations.GetElementAttributeValue(XmlNodes.Service, svcCntr, Parser, XmlAttribs.Spui),
                    Status = XmlOperations.GetInnerTextForElement(XmlNodes.Status, serviceNode, 0, Parser)
                };

                var referenceNode = XmlOperations.GetElement(XmlNodes.Reference, serviceNode, 0, Parser);
                serviceForPurchase.FileNumber = XmlOperations.GetInnerTextForElement(XmlNodes.FileNumber, referenceNode, 0, Parser);
                serviceForPurchase.IncomingOfficeCode = XmlOperations.GetInnerTextForElement(XmlNodes.IncomingOffice, referenceNode, 0, Parser);
                serviceForPurchase.Status = XmlOperations.GetInnerTextForElement(XmlNodes.Status, serviceNode, 0, Parser);

                var contractListNodes = XmlOperations.GetElement(XmlNodes.ContractList, serviceNode, 0, Parser);
                var contractNodes = Parser.GetAllElements(contractListNodes, XmlNodes.Contract);
                foreach (var itemContract in contractNodes) // do we need to have a foreach Loop?? as serviceForPurchase has a contract only and NO ContractList!
                {
                    var contract = new Contract
                    {
                        Name = XmlOperations.GetInnerTextForElement(XmlNodes.Name, itemContract, 0, Parser),
                        IncomingOffice = XmlOperations.GetElementAttributeValue(XmlNodes.IncomingOffice, 0,
                            itemContract, Parser, XmlAttribs.Code)
                    };
                    serviceForPurchase.Contract = contract;
                }

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

                var nodes = Parser.GetAllElements(serviceNode, XmlNodes.CommentList);
                serviceForPurchase.Comments = new List<Comment>();

                foreach (var itemComment in nodes)
                {
                    var comment = new Comment
                    {
                        CommentText =
                            XmlOperations.GetInnerTextForElement(XmlNodes.Comment, itemComment, 0, Parser),
                        Type = XmlOperations.GetElementAttributeValue(XmlNodes.Comment, 0, Parser, XmlAttribs.Type)
                    };
                    serviceForPurchase.Comments.Add(comment);
                }

                serviceForPurchase.DateFrom = DateTime.ParseExact(XmlOperations.GetElementAttributeValue(XmlNodes.DateFrom, 0, serviceNode, Parser, XmlAttribs.Date), Constant.CheckInOutFormat, CultureInfo.InvariantCulture);
                serviceForPurchase.DateTo = DateTime.ParseExact(XmlOperations.GetElementAttributeValue(XmlNodes.DateTo, 0, serviceNode, Parser, XmlAttribs.Date), Constant.CheckInOutFormat, CultureInfo.InvariantCulture);

                serviceForPurchase.Currency = XmlOperations.GetElementAttributeValue(XmlNodes.Currency, 0, serviceNode, Parser, XmlAttribs.Code);
                decimal.TryParse(XmlOperations.GetInnerTextForElement(XmlNodes.TotalAmount, serviceNode, 0, Parser), out var totalAmt);
                serviceForPurchase.TotalAmount = totalAmt;

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
                var contractcomment = new Comment();
                var commentNodes = Parser.GetAllElements(serviceNode, XmlNodes.CommentList);
                var commentIndex = 0;

                foreach (var commentNode in commentNodes)
                {
                    contractcomment.CommentText = XmlOperations.GetInnerTextForElement(XmlNodes.Comment, commentNode, commentIndex, Parser);
                    contractcomment.Type = XmlOperations.GetElementAttributeValue(XmlNodes.Comment, commentIndex, Parser, XmlAttribs.Type);
                    comments.Add(contractcomment);
                    commentIndex++;
                }
                hotelContract.Comments = comments;
                serviceForPurchase.Contract = hotelContract;

                #endregion Contract

                serviceForPurchase.Currency = XmlOperations.GetElementAttributeValue(XmlNodes.Currency, 0, serviceNode, Parser, XmlAttribs.Code);

                serviceForPurchase.DateFrom = DateTime.ParseExact(XmlOperations.GetElementAttributeValue(XmlNodes.DateFrom, 0, serviceNode, Parser, XmlAttribs.Date), Constant.CheckInOutFormat, CultureInfo.InvariantCulture);
                serviceForPurchase.DateTo = DateTime.ParseExact(XmlOperations.GetElementAttributeValue(XmlNodes.DateTo, 0, serviceNode, Parser, XmlAttribs.Date), Constant.CheckInOutFormat, CultureInfo.InvariantCulture);

                #region Additional Costs

                var additionalCosts = new List<AdditionalCost>();
                var addCostListNode = XmlOperations.GetElement(XmlNodes.AdditionalCostList, 0, Parser);
                var addCostNodes = Parser.GetAllElements(addCostListNode, XmlNodes.AdditionalCost);

                var aCostindexer = 0;
                foreach (var addCostNode in addCostNodes)
                {
                    var addCost = new AdditionalCost
                    {
                        Type = XmlOperations.GetElementAttributeValue(XmlNodes.AdditionalCost, aCostindexer++, Parser,
                            XmlAttribs.Type)
                    };
                    var priceNode = XmlOperations.GetElement(XmlNodes.Price, addCostNode, 0, Parser);
                    if (decimal.TryParse(XmlOperations.GetInnerTextForElement(XmlNodes.Amount, priceNode, 0, Parser), out var tempDecimalValue))
                        addCost.Amount = tempDecimalValue;
                    additionalCosts.Add(addCost);
                }
                serviceForPurchase.AdditionalCosts = additionalCosts;

                #endregion Additional Costs

                if (serviceNode.GetAttribute(Constant.Type).ToLower().EndsWith(Constant.Hotel))
                {
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

                    var roomCount = 0;
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
                    var cancelPolicyNode = XmlOperations.GetElement(Constant.CancellationPolicy, cancelPoliciesNode ?? hotelRoomNode, 0, Parser);
                    var cancellationPricesHtl = Parser.GetAllElements(cancelPolicyNode, XmlNodes.Price);

                    //TODO: CancellationPolicy must now be handled like Tickets, i.e. return Cancellation Charges!!!
                    foreach (var cancelPolicyPriceNode in cancellationPricesHtl)
                    {
                        cancellationPolicy.Amount = decimal.Parse(XmlOperations.GetInnerTextForElement(XmlNodes.Amount, cancelPolicyPriceNode, 0, Parser), CultureInfo.InvariantCulture);
                        cancellationPolicy.FromDate = DateTime.ParseExact(XmlOperations.GetElementAttributeValue(Constant.DateTimeFrom, 0, cancelPolicyPriceNode, Parser, XmlAttribs.Date), Constant.CheckInOutFormat, CultureInfo.InvariantCulture);
                        cancellationPolicy.ToDate = DateTime.ParseExact(XmlOperations.GetElementAttributeValue(Constant.DateTimeTo, 0, cancelPolicyPriceNode, Parser, XmlAttribs.Date), Constant.CheckInOutFormat, CultureInfo.InvariantCulture);
                    }
                    room.CancellationPolicy = cancellationPolicy;
                    avRoom.HotelRoom = room;
                    serviceForPurchase.AvailableRoom = avRoom;

                    #endregion Available Room
                }
                else
                {
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

                    #endregion Available Modality

                    #region SeatingList

                    var lstSeating = new List<Seating>();
                    var seatingListNode = XmlOperations.GetElement(XmlNodes.SeatingList, 0, Parser);
                    if (seatingListNode != null)
                    {
                        var seatingNodes = Parser.GetAllElements(seatingListNode);
                        foreach (var seatingNode in seatingNodes)
                        {
                            var seating = new Seating { Seat = 0 };

                            if (!string.IsNullOrEmpty(XmlOperations.GetInnerTextForElement(XmlNodes.Seat, seatingNode, 0, Parser)))
                            {
                                seating.Seat = Convert.ToInt32(XmlOperations.GetInnerTextForElement(XmlNodes.Seat, seatingNode, 0, Parser));
                            }

                            seating.Gate = XmlOperations.GetInnerTextForElement(XmlNodes.Gate, seatingNode, 0, Parser) ?? "";
                            seating.Row = XmlOperations.GetInnerTextForElement(XmlNodes.Row, seatingNode, 0, Parser) ?? "";
                            seating.EntranceDoor = XmlOperations.GetInnerTextForElement(XmlNodes.EntranceDoor, seatingNode, 0, Parser) ?? "";
                            seating.PaxId = 0;
                            if (!string.IsNullOrEmpty(XmlOperations.GetInnerTextForElement(XmlNodes.PaxId, seatingNode, 0, Parser)))
                            {
                                seating.PaxId = Convert.ToInt32(XmlOperations.GetInnerTextForElement(XmlNodes.PaxId, seatingNode, 0, Parser));
                            }

                            lstSeating.Add(seating);
                        }
                        serviceForPurchase.SeatingDetails = lstSeating;
                    }

                    #endregion SeatingList

                    #region Voucher List

                    var lstVoucher = new List<Voucher>();
                    var voucherListNode = XmlOperations.GetElement(XmlNodes.VoucherList, 0, Parser);
                    if (voucherListNode != null)
                    {
                        var voucherNodes = Parser.GetAllElements(voucherListNode);
                        lstVoucher.AddRange(voucherNodes.Select(voucherNode => new Voucher
                        {
                            Code = XmlOperations.GetInnerTextForElement(XmlNodes.Code, voucherNode, 0, Parser) ?? "",
                            Url = XmlOperations.GetInnerTextForElement(XmlNodes.Url, voucherNode, 0, Parser) ?? "",
                            MimeType = XmlOperations.GetInnerTextForElement(XmlNodes.MimeType, voucherNode, 0, Parser) ?? "",
                            StartDate = XmlOperations.GetInnerTextForElement(XmlNodes.StartDate, voucherNode, 0, Parser) ?? "",
                            EndDate = XmlOperations.GetInnerTextForElement(XmlNodes.EndDate, voucherNode, 0, Parser) ?? "",
                            LanguageCode = XmlOperations.GetInnerTextForElement(XmlNodes.LanguageCode, voucherNode, 0, Parser) ?? ""
                        }));
                        serviceForPurchase.VoucherDetails = lstVoucher;
                    }

                    #endregion Voucher List

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
                    var charges = cancellationPrices.Select(cancelPolicyPriceNode => new CancellationPolicy
                    {
                        Amount = decimal.Parse(XmlOperations.GetInnerTextForElement(XmlNodes.Amount, cancelPolicyPriceNode, 0, Parser), CultureInfo.InvariantCulture),
                        FromDate = DateTime.ParseExact(XmlOperations.GetElementAttributeValue(Constant.DateTimeFrom, 0, cancelPolicyPriceNode, Parser, XmlAttribs.Date), Constant.CheckInOutFormat, CultureInfo.InvariantCulture),
                        ToDate = DateTime.ParseExact(XmlOperations.GetElementAttributeValue(Constant.DateTimeTo, 0, cancelPolicyPriceNode, Parser, XmlAttribs.Date), Constant.CheckInOutFormat, CultureInfo.InvariantCulture)
                    })
                        .ToList();
                    if (charges != null && charges.Count > 0)
                        serviceForPurchase.CancellationPolicy = charges[0];
                    serviceForPurchase.CancellationCharges = charges;

                    #endregion Cancellation Policy

                    #region Service Details

                    var lstServiceDetails = new List<ServiceDetail>();
                    var serviceDetailListNode = XmlOperations.GetElement(XmlNodes.ServiceDetailList, 0, Parser);
                    if (serviceDetailListNode != null)
                    {
                        var serviceDetailNodes = Parser.GetAllElements(serviceDetailListNode);
                        lstServiceDetails.AddRange(serviceDetailNodes.Select(serviceDetailNode => new ServiceDetail
                        {
                            Code = XmlOperations.GetElementAttributeValue(XmlNodes.ServiceDetail, 0, Parser, XmlAttribs.Code) ?? "",
                            Name = XmlOperations.GetInnerTextForElement(XmlNodes.Name, serviceDetailNode, 0, Parser) ?? "",
                            Description = XmlOperations.GetInnerTextForElement(XmlNodes.Description, serviceDetailNode, 0, Parser) ?? ""
                        }));
                        serviceForPurchase.ServiceDetails = lstServiceDetails;
                    }

                    #endregion Service Details
                }
                servicesInResponse.Add(serviceForPurchase);
                svcCntr++;
            }
            purchase.ServiceList = servicesInResponse;

            #endregion ServiceList

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

            decimal.TryParse(XmlOperations.GetInnerTextForElement(XmlNodes.TotalPrice, purchaseNode, 0, Parser), out var purchaseAmount);
            purchase.TotalAmount = purchaseAmount;
            purchaseData.Purchase = purchase;

            return purchaseData;
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

                //#region ### Read Static file to avoid actualbooking

                //string xml = string.Empty;
                //using (StreamReader r = new StreamReader(@"D:\temp\PurchaseConfirmRS.xml"))
                //{
                //    xml = r.ReadToEnd();
                //}

                //#endregion Read Static file to avoid actualbooking

                var xml = client.Post(values);
                return xml;
            }
            return null;
        }

        protected override object CreateInputRequest(InputContext inputContext)
        {
            var bookingHotelXml = new XmlDocument();
            if (AppDomain.CurrentDomain.BaseDirectory.ToLowerInvariant().EndsWith(Constant.Release) || AppDomain.CurrentDomain.BaseDirectory.ToLowerInvariant().EndsWith(Constant.Debug))
                bookingHotelXml.Load($"{Directory.GetParent((Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName).FullName)).FullName}{Constant.PurchaseConfirmRqTicketXml}");
            else
                bookingHotelXml.Load($"{AppDomain.CurrentDomain.BaseDirectory}{Constant.PurchaseConfirmRqTicketXml}");

            var namespaceManager = new XmlNamespaceManager(bookingHotelXml.NameTable);
            namespaceManager.AddNamespace(Constant.Hb, inputContext.NameSpace);

            var echo = bookingHotelXml.SelectSingleNode(Constant.HBPurchaseConfirmRq, namespaceManager);
            if (echo?.Attributes != null)
                echo.Attributes[XmlAttribs.EchoToken].Value = inputContext.HBSelectedProducts[0].EchoToken;

            if (inputContext.Language != null)
            {
                var language = bookingHotelXml.SelectSingleNode(Constant.PurchaseConfirmRqLanguage, namespaceManager);
                if (language != null) language.InnerXml = inputContext.Language;
            }

            var usr = bookingHotelXml.SelectSingleNode(Constant.PurchaseConfirmRqUser, namespaceManager);
            if (usr != null) usr.InnerXml = inputContext.UserName;

            var pwd = bookingHotelXml.SelectSingleNode(Constant.PurchaseConfirmRqPassword, namespaceManager);
            if (pwd != null) pwd.InnerXml = inputContext.Password;

            var token = bookingHotelXml.SelectSingleNode(Constant.PurchaseConfirmRqConfirmationData, namespaceManager);
            if (token?.Attributes != null)
                token.Attributes[XmlAttribs.PurchaseToken].Value = inputContext.HBSelectedProducts[0].PurchaseToken;

            #region WARNING - Change this region when actually calling on Payment page.

            var customer = inputContext.HBSelectedProducts[0].ProductOptions.Find(f => f.IsSelected.Equals(true))
                .Customers.FirstOrDefault(x => x.IsLeadCustomer);
            var holderFirstName = bookingHotelXml.SelectSingleNode(Constant.HbPurchaseConfirmRqHbConfirmatioDataHolderName, namespaceManager);
            if (holderFirstName != null)
                holderFirstName.InnerXml = customer?.FirstName ?? "";

            var holderLastName = bookingHotelXml.SelectSingleNode(Constant.HbPurchaseConfirmRqHbConfirmatioDataHolderLastName, namespaceManager);
            var lastName = string.IsNullOrWhiteSpace(customer?.LastName) ? Constant.LastName : customer.LastName;
            if (holderLastName != null) holderLastName.InnerXml = lastName;

            #endregion WARNING - Change this region when actually calling on Payment page.

            var services = bookingHotelXml.SelectSingleNode(Constant.PurchaseConfirmRqConfirmationServiceDataList, namespaceManager);
            if (services != null) services.InnerXml = GetServices(inputContext.HBSelectedProducts);

            return bookingHotelXml.InnerXml;
        }

        private string GetServices(List<Isango.Entities.HotelBeds.HotelBedsSelectedProduct> products)
        {
            var serviceNodes = new StringBuilder();

            foreach (var item in products)
            {
                if (item.ProductType.Equals(ProductType.Activity))
                {
                    serviceNodes.AppendLine($"{Constant.PurchaseConfirmRqServiceData}{item.SPUI}\">");
                }
                else if (item.ProductType.Equals(ProductType.Hotel))
                {
                    serviceNodes.AppendLine($"{Constant.PurchaseConfirmRqServiceDataHotel}{item.SPUI}\">");
                }
                else
                {
                    serviceNodes.AppendLine($"{Constant.PurchaseConfirmRqServiceDataHotel}\">");
                }

                serviceNodes.AppendLine(Constant.StartCustomerList);
                var customers = item?.ProductOptions?.FirstOrDefault()?.Customers;//.FindAll(cus => cus.PassengerType.Equals(PassengerType.Adult) || cus.PassengerType.Equals(PassengerType.Child));
                serviceNodes.Append("\n" + GetXMLForCustomers(customers));
                serviceNodes.AppendLine(Constant.EndCustomerList);

                if (item.ContractQuestions != null && item.ContractQuestions.Count > 0)
                {
                    serviceNodes.AppendLine(Constant.StartServiceDetailList);
                    serviceNodes.Append($"\n{GetXMLForContractQuestions(item.ContractQuestions)}");
                    serviceNodes.AppendLine(Constant.EndServiceDetailList);
                }

                serviceNodes.AppendLine(Constant.ServiceData);
            }
            return serviceNodes.ToString();
        }

        private string GetXMLForContractQuestions(List<IsangoEntities.ContractQuestion> contractQuestions)
        {
            var contractXml = new StringBuilder();

            foreach (var contractQuestion in contractQuestions)
            {
                contractXml.Append($"{Constant.ServiceDetailCode}{contractQuestion.Code}\">");
                contractXml.Append($"{Constant.NameStart}{contractQuestion.Name}{Constant.NameEnd}");
                contractXml.Append($"{Constant.StartDescription}{contractQuestion.Description}{Constant.EndDescription}");
                contractXml.Append(Constant.EndServiceDeatail);
            }
            return contractXml.ToString();
        }

        protected override Task<object> GetXmlResultsAsync(object inputXml)
        {
            return null;
        }

        public override string GetXMLForCustomers(List<Isango.Entities.Customer> customers)
        {
            var customerXml = new StringBuilder();
            customers = customers.OrderByDescending(x => x.IsLeadCustomer)
                .ThenBy(x => x.CustomerId)
                .ThenBy(x => x.PassengerType)
                .ToList();
            foreach (var customer in customers)
            {
                var age = !string.IsNullOrWhiteSpace(customer.Age.ToString()) ? customer.Age.ToString() : "30";
                var customerType = customer.PassengerType.Equals(PassengerType.Adult) ? Constant.Ad : Constant.Ch;
                customerXml.Append($"<Customer type=\"{customerType}\">");
                customerXml.Append($"{Constant.CustomerIdStart}{customer.CustomerId}{Constant.CustomerIdEnd}");
                customerXml.Append($"{Constant.AgeStart}{age}{Constant.AgeEnd}");
                customerXml.Append($"{Constant.NameStart}{customer.FirstName}{Constant.NameEnd}");
                customerXml.Append($"{Constant.LastNameStart}{customer.LastName}{Constant.LastNameEnd}");
                customerXml.Append(Constant.CustomerTypeEnd);
            }
            return customerXml.ToString();
        }
    }
}