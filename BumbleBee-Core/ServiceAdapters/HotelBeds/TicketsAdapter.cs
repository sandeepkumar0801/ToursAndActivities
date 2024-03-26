using Isango.Entities.Activities;
using Isango.Entities.Enums;
using Isango.Entities.HotelBeds;
using Isango.Entities.Ticket;
using ServiceAdapters.HotelBeds.HotelBeds.Commands.Contracts;
using ServiceAdapters.HotelBeds.HotelBeds.Converters.Contracts;
using ServiceAdapters.HotelBeds.HotelBeds.Entities;
using ServiceAdapters.HotelBeds.HotelBeds.Entities.Tickets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Util;

namespace ServiceAdapters.HotelBeds
{
    public class TicketsAdapter : ITicketAdapter
    {
        #region "Private Members"

        private readonly ICommonCartCommandHandler _commonCartCommandHandler;
        private readonly IPurchaseCancelCommandHandler _purchaseCancelCommandHandler;
        private readonly IPurchaseDetailCommandHandler _purchaseDetailCommandHandler;
        private readonly IServiceRemoveCommandHandler _serviceRemoveCommandHandler;
        private readonly ITicketAvailabilityCmdHandler _ticketAvailabilityCmdHandler;
        private readonly ITicketPurchaseConfirmCmdHandler _ticketPurchaseConfirmCmdHandler;
        private readonly ITicketValuationCmdHandler _ticketValuationCmdHandler;
        private readonly ICartConverter _cartConverter;
        private readonly IPurchaseConfirmConverter _purchaseConfirmConverter;
        private readonly IPurchaseDetailsConverter _purchaseDetailsConverter;
        private readonly ITicketAvailabilityConverter _ticketAvailabilityConverter;
        private readonly ITicketValuationConverter _ticketValuationConverter;

        #endregion "Private Members"

        #region "Parser"

        protected XmlParser MParser;

        protected virtual XmlParser Parser
        {
            get
            {
                if (MParser != null)
                    return MParser;

                MParser = XmlOperations.GetXmlParser();
                return MParser;
            }
        }

        #endregion "Parser"

        #region "Constructors"

        public TicketsAdapter(
            ICommonCartCommandHandler commonCartCommandHandler,
            IPurchaseCancelCommandHandler purchaseCancelCommandHandler,
            IPurchaseDetailCommandHandler purchaseDetailCommandHandler,
            IServiceRemoveCommandHandler serviceRemoveCommandHandler,
            ITicketAvailabilityCmdHandler ticketAvailabilityCmdHandler,
            ITicketPurchaseConfirmCmdHandler ticketPurchaseConfirmCmdHandler,
            ITicketValuationCmdHandler ticketValuationCmdHandler,
            ICartConverter cartConverter,
            IPurchaseConfirmConverter purchaseConfirmConverter,
            IPurchaseDetailsConverter purchaseDetailsConverter,
            ITicketAvailabilityConverter ticketAvailabilityConverter,
            ITicketValuationConverter ticketValuationConverter)
        {
            _commonCartCommandHandler = commonCartCommandHandler;
            _purchaseCancelCommandHandler = purchaseCancelCommandHandler;
            _purchaseDetailCommandHandler = purchaseDetailCommandHandler;
            _serviceRemoveCommandHandler = serviceRemoveCommandHandler;
            _ticketAvailabilityCmdHandler = ticketAvailabilityCmdHandler;
            _ticketPurchaseConfirmCmdHandler = ticketPurchaseConfirmCmdHandler;
            _ticketValuationCmdHandler = ticketValuationCmdHandler;
            _cartConverter = cartConverter;
            _purchaseConfirmConverter = purchaseConfirmConverter;
            _purchaseDetailsConverter = purchaseDetailsConverter;
            _ticketAvailabilityConverter = ticketAvailabilityConverter;
            _ticketValuationConverter = ticketValuationConverter;
        }

        #endregion "Constructors"

        public List<Activity> GetTicketAvailability(TicketCriteria criteria, string authString, string token)
        {
            if (criteria != null)
            {
                var inputContext = new InputContext();
                if (!string.IsNullOrEmpty(authString))
                {
                    var auth = authString.Split('>', '<');
                    inputContext.UserName = auth[0];
                    inputContext.Password = auth[2];
                }
                inputContext.MethodType = MethodType.Ticketvaluedavailability;
                inputContext.Adults = criteria.NoOfPassengers.Where(x => x.Key == PassengerType.Adult).
                                            Select(x => x.Value).FirstOrDefault();
                inputContext.Children = criteria.NoOfPassengers.Where(x => x.Key == PassengerType.Infant).
                                            Select(x => x.Value).FirstOrDefault();

                inputContext.Children += criteria.NoOfPassengers.Where(x => x.Key == PassengerType.Child).
                                            Select(x => x.Value).FirstOrDefault();
                inputContext.Children += criteria.NoOfPassengers.Where(x => x.Key == PassengerType.Youth).
                                            Select(x => x.Value).FirstOrDefault();
                inputContext.CheckinDate = criteria.CheckinDate;
                inputContext.CheckoutDate = criteria.CheckoutDate;
                var InfantAges = criteria.Ages?.Where(x => x.Key == PassengerType.Infant).Select(x => Convert.ToInt32(x.Value)).ToList();
                var ChildAges = criteria.Ages?.Where(x => x.Key == PassengerType.Child).Select(x => Convert.ToInt32(x.Value)).ToList();
                var YouthAges = criteria.Ages?.Where(x => x.Key == PassengerType.Youth).Select(x => Convert.ToInt32(x.Value)).ToList();
                inputContext.ChildAges = InfantAges;
                if (ChildAges != null && ChildAges.Count > 0)
                {
                    inputContext?.ChildAges?.AddRange(ChildAges);
                }
                if (YouthAges != null && YouthAges.Count > 0)
                {
                    inputContext?.ChildAges?.AddRange(YouthAges);
                }
                inputContext.HotelIDs = criteria.HotelIds;
                inputContext.Destination = criteria.Destination;
                inputContext.FactsheetIDs = criteria.FactSheetIds;
                inputContext.Language = criteria.Language;
                inputContext.InputCriteria = criteria;

                var returnValue = _ticketAvailabilityCmdHandler.Execute(inputContext, token);
                if (returnValue != null)
                {
                    try
                    {
                        if (returnValue != null)
                        {
                            var result = (TicketAvailRs)returnValue;
                            result.InputCriteria = criteria;
                            returnValue = result;
                        }
                    }
                    catch
                    {
                        // ignored
                    }

                    returnValue = _ticketAvailabilityConverter.Convert(returnValue);
                    return (List<Activity>)returnValue;
                }
            }
            return new List<Activity>();
        }

        public async Task<List<Activity>> GetTicketAvailabilityAsync(TicketCriteria criteria, string authString, string token)
        {
            if (criteria != null)
            {
                var inputContext = new InputContext();
                if (!string.IsNullOrEmpty(authString))
                {
                    var auth = authString.Split('>', '<');
                    inputContext.UserName = auth[0];
                    inputContext.Password = auth[2];
                }
                inputContext.MethodType = MethodType.Ticketvaluedavailability;
                inputContext.Adults = criteria.NoOfPassengers?.Where(x => x.Key == PassengerType.Adult).Select(s => s.Value).FirstOrDefault() ?? 0;
                inputContext.CheckinDate = criteria.CheckinDate;
                inputContext.CheckoutDate = criteria.CheckoutDate;
                inputContext.Children = criteria.NoOfPassengers?.Where(x => x.Key == PassengerType.Child).Select(s => s.Value).FirstOrDefault() ?? 0;
                inputContext.ChildAges = criteria.Ages.Where(x => x.Key == PassengerType.Child).Select(x => Convert.ToInt32(x.Value)).ToList();
                inputContext.HotelIDs = criteria.HotelIds;
                inputContext.Destination = criteria.Destination;
                inputContext.FactsheetIDs = criteria.FactSheetIds;
                inputContext.Language = criteria.Language;

                var returnValue = await _ticketAvailabilityCmdHandler.ExecuteAsync(inputContext, token);

                if (returnValue != null)
                {
                    try
                    {
                        if (returnValue != null)
                        {
                            var result = (TicketAvailRs)returnValue;
                            result.InputCriteria = criteria;
                            returnValue = result;
                        }
                    }
                    catch
                    {
                        // ignored
                    }

                    returnValue = _ticketAvailabilityConverter.Convert(returnValue);
                    return (List<Activity>)returnValue;
                }
            }
            return null;
        }

        public List<HotelBedsSelectedProduct> AddTicket(HotelBedsSelectedProduct hotelBedsSelectedProduct, string authString, string token)
        {
            if (hotelBedsSelectedProduct != null)
            {
                var inputContext = new InputContext();
                var auth = authString.Split('>', '<');

                inputContext.UserName = auth[0];
                inputContext.Password = auth[2];
                inputContext.MethodType = MethodType.Shoppingcart;
                inputContext.Language = hotelBedsSelectedProduct.Language;
                if (inputContext.HBSelectedProducts == null)
                    inputContext.HBSelectedProducts = new List<HotelBedsSelectedProduct>();
                inputContext.HBSelectedProducts.Add(hotelBedsSelectedProduct);
                inputContext.Destination = hotelBedsSelectedProduct.Destination;

                var returnValue = _commonCartCommandHandler.Execute(inputContext, token);
                if (returnValue != null)
                {
                    try
                    {//Add input to result so that it can be used in convertor
                        var result = (ServiceAddRs)returnValue;
                        result.InputCritera = inputContext.HBSelectedProducts;
                        returnValue = result;
                    }
                    catch
                    {
                        //Ignore
                    }
                    returnValue = _cartConverter.Convert(returnValue);
                    return returnValue as List<HotelBedsSelectedProduct>;
                }
            }
            return new List<HotelBedsSelectedProduct>();
        }

        public async Task<HotelBedsSelectedProduct> AddTicketAsync(HotelBedsSelectedProduct hotelBedsSelectedProduct, string authString, string token)
        {
            if (hotelBedsSelectedProduct != null)
            {
                var inputContext = new InputContext();
                var auth = authString.Split('>', '<');
                inputContext.UserName = auth[0];
                inputContext.Password = auth[2];
                inputContext.MethodType = MethodType.Shoppingcart;
                inputContext.Language = hotelBedsSelectedProduct.Language;
                if (inputContext.HBSelectedProducts == null)
                    inputContext.HBSelectedProducts = new List<HotelBedsSelectedProduct>();
                inputContext.HBSelectedProducts.Add(hotelBedsSelectedProduct);
                inputContext.Destination = hotelBedsSelectedProduct.Destination;

                var returnValue = await _commonCartCommandHandler.ExecuteAsync(inputContext, token);
                if (returnValue != null)
                {
                    returnValue = _cartConverter.Convert(returnValue);
                    return returnValue as HotelBedsSelectedProduct;
                }
            }
            return new HotelBedsSelectedProduct();
        }

        public HotelBedsSelectedProduct GetTicketPrice(HotelBedsSelectedProduct hotelBedsSelectedProduct, string authString, string token)
        {
            if (hotelBedsSelectedProduct != null)
            {
                var inputContext = new InputContext();
                var auth = authString.Split('>', '<');

                inputContext.UserName = auth[0];
                inputContext.Password = auth[2];
                inputContext.MethodType = MethodType.Valuation;
                inputContext.Language = hotelBedsSelectedProduct.Language;
                if (inputContext.HBSelectedProducts == null)
                    inputContext.HBSelectedProducts = new List<HotelBedsSelectedProduct>();
                inputContext.HBSelectedProducts.Add(hotelBedsSelectedProduct);

                var returnValue = _ticketValuationCmdHandler.Execute(inputContext, token);
                if (returnValue != null)
                {
                    try
                    {
                        //Adding input to result so that can use in convector
                        var result = (TicketValuationRs)returnValue;
                        result.HotelBedsSelectedProduct = hotelBedsSelectedProduct;
                        returnValue = result;
                    }
                    catch
                    {
                        //Ignore
                    }

                    returnValue = _ticketValuationConverter.Convert(returnValue);
                    return returnValue as HotelBedsSelectedProduct;
                }
            }
            return null;
        }

        public async Task<HotelBedsSelectedProduct> GetTicketPriceAsync(HotelBedsSelectedProduct hotelBedsSelectedProduct, string authString, string token)
        {
            var auth = authString.Split('>', '<');
            var inputContext = new InputContext
            {
                UserName = auth[0],
                Password = auth[2],
                MethodType = MethodType.Valuation,
                Language = hotelBedsSelectedProduct.Language
            };
            if (inputContext.HBSelectedProducts == null)
                inputContext.HBSelectedProducts = new List<HotelBedsSelectedProduct>();
            inputContext.HBSelectedProducts.Add(hotelBedsSelectedProduct);

            //using (new PerfTimer("Execute - The customer enter details"))
            var returnValue = await _ticketValuationCmdHandler.ExecuteAsync(inputContext, token);
            if (returnValue != null)
            {
                returnValue = _ticketValuationConverter.Convert(returnValue);
                return returnValue as HotelBedsSelectedProduct;
            }
            return null;
        }

        public List<HotelBedsSelectedProduct> PurchaseConfirm(List<HotelBedsSelectedProduct> selectedProducts,
            string authString, string token, out string requestXml, out string responseXml)
        {
            var inputContext = new InputContext();
            var auth = authString.Split('>', '<');
            inputContext.UserName = auth[0];
            inputContext.Password = auth[2];
            inputContext.MethodType = MethodType.Purchaseconfirm;
            inputContext.HBSelectedProducts = selectedProducts;

            requestXml = string.Empty;
            responseXml = string.Empty;

            if (selectedProducts?.Count > 0)
            {
                inputContext.Language = selectedProducts?.FirstOrDefault()?.Language;
                //using (new PerfTimer("Execute - The customer enter details"))
                var returnValue = _ticketPurchaseConfirmCmdHandler.Execute(inputContext, token, out requestXml, out responseXml);
                if (returnValue != null)
                {
                    //#TODO Adding cancellation after confirmation so that it can be cancelled
                    //FO not Check it in
                    try
                    {
                        returnValue = _purchaseConfirmConverter.Convert(returnValue);
                        //var ConfimredTicket = returnValue as List<HotelBedsSelectedProduct>;
                        //this.PurchaseCancel(ConfimredTicket, authString, token);
                    }
                    catch (Exception)
                    {
                        //Ignore
                    }
                    return returnValue as List<HotelBedsSelectedProduct>;
                }
            }
            return new List<HotelBedsSelectedProduct>();
        }

        public List<HotelBedsSelectedProduct> PurchaseDetails(List<HotelBedsSelectedProduct> selectedProducts, out string requestXml, out string responseXml,
            string authString, string token)
        {
            var inputContext = new InputContext();
            var auth = authString.Split('>', '<');
            inputContext.UserName = auth[0];
            inputContext.Password = auth[2];
            inputContext.MethodType = MethodType.Purchasedetail;
            inputContext.HBSelectedProducts = selectedProducts;
            inputContext.Language = selectedProducts[0].Language;
            requestXml = string.Empty;
            responseXml = string.Empty;

            if (selectedProducts.Count > 0)
            {
                var returnValue = _purchaseDetailCommandHandler.Execute(inputContext, token, out requestXml, out responseXml);
                if (returnValue != null)
                {
                    returnValue = _purchaseDetailsConverter.Convert(returnValue);

                    //#TODO Adding cancellation after confirmation so that it can be cancelled
                    //FO not Check it in
                    try
                    {
                        var ConfimredTicket = returnValue as List<HotelBedsSelectedProduct>;
                        PurchaseCancel(ConfimredTicket, authString, out requestXml, out responseXml, token);
                    }
                    catch (Exception)
                    {
                        //Ignore
                    }

                    return returnValue as List<HotelBedsSelectedProduct>;
                }
            }
            return new List<HotelBedsSelectedProduct>();
        }

        public bool ServiceRemove(List<HotelBedsSelectedProduct> selectedProducts, string authString, string token)
        {
            var response = false;
            var inputContext = new InputContext();
            var auth = authString.Split('>', '<');

            inputContext.UserName = auth[0];
            inputContext.Password = auth[2];
            inputContext.MethodType = MethodType.Serviceremove;
            inputContext.HBSelectedProducts = selectedProducts;

            if (selectedProducts != null && selectedProducts.Count > 0)
            {
                inputContext.Language = selectedProducts[0].Language;

                response = (bool)_serviceRemoveCommandHandler.ExecuteWithoutResponse(inputContext, token);
            }
            return response;
        }

        public bool PurchaseCancel(List<HotelBedsSelectedProduct> selectedProducts, string authString, string token)
        {
            var response = false;
            try
            {
                var inputContext = new InputContext();
                var auth = authString.Split('>', '<');
                inputContext.UserName = auth[0];
                inputContext.Password = auth[2];
                inputContext.MethodType = MethodType.Purchasecancel;
                inputContext.HBSelectedProducts = selectedProducts;
                if (selectedProducts?.Count > 0)
                {
                    inputContext.Language = selectedProducts.FirstOrDefault()?.Language;
                    response = (bool)_purchaseCancelCommandHandler.ExecuteWithoutResponse(inputContext, token);
                }
            }
            catch (Exception)
            {
                response = false;
            }
            return response;
        }

        public bool PurchaseCancel(List<HotelBedsSelectedProduct> selectedProducts, string authString, out string requestXml, out string responceXml, string token)
        {
            requestXml = "";
            responceXml = "";

            var response = false;
            var inputContext = new InputContext();
            var auth = authString.Split('>', '<');
            inputContext.UserName = auth[0];
            inputContext.Password = auth[2];
            inputContext.MethodType = MethodType.Purchasecancel;
            inputContext.HBSelectedProducts = selectedProducts;
            if (selectedProducts != null && selectedProducts.Count > 0)
            {
                inputContext.Language = selectedProducts[0].Language;
                response = (bool)_purchaseCancelCommandHandler.Execute(inputContext, token, out requestXml, out responceXml);
            }
            return response;
        }
    }
}