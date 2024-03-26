using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.MoulinRouge;
using ServiceAdapters.MoulinRouge.Constants;
using ServiceAdapters.MoulinRouge.MoulinRouge.Commands.Contracts;
using ServiceAdapters.MoulinRouge.MoulinRouge.Converters.Contracts;
using ServiceAdapters.MoulinRouge.MoulinRouge.Entities;
using ServiceAdapters.MoulinRouge.MoulinRouge.Entities.AllocSeatsAutomatic;
using ServiceAdapters.MoulinRouge.MoulinRouge.Entities.CatalogDateGetDetailMulti;
using ServiceAdapters.MoulinRouge.MoulinRouge.Entities.CatalogDateGetList;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using AllocSeatsAutomatic = ServiceAdapters.MoulinRouge.MoulinRouge.Entities.AllocSeatsAutomatic;
using CatalogDateGetDetailMulti = ServiceAdapters.MoulinRouge.MoulinRouge.Entities.CatalogDateGetDetailMulti;
using CatalogDateGetList = ServiceAdapters.MoulinRouge.MoulinRouge.Entities.CatalogDateGetList;
using CreateAccount = ServiceAdapters.MoulinRouge.MoulinRouge.Entities.CreateAccount;
using GetOrderEticket = ServiceAdapters.MoulinRouge.MoulinRouge.Entities.GetOrderEticket;
using OrderConfirm = ServiceAdapters.MoulinRouge.MoulinRouge.Entities.OrderConfirm;
using ReleaseSeats = ServiceAdapters.MoulinRouge.MoulinRouge.Entities.ReleaseSeats;
using TempOrderGetDetail = ServiceAdapters.MoulinRouge.MoulinRouge.Entities.TempOrderGetDetail;
using TempOrderGetSendingFees = ServiceAdapters.MoulinRouge.MoulinRouge.Entities.TempOrderGetSendingFees;
using TempOrderSetSendingFees = ServiceAdapters.MoulinRouge.MoulinRouge.Entities.TempOrderSetSendingFees;

namespace ServiceAdapters.MoulinRouge
{
    public sealed class MoulinRougeAdapter : IMoulinRougeAdapter, IAdapter
    {
        #region "Private Members"

        private readonly IAddToBasketCommandHandler _addToBasketCommandHandler;
        private readonly IAllocSeatsAutomaticCommandHandler _allocSeatsAutomaticCommandHandler;
        private readonly ICatalogDateGetDetailMultiCommandHandler _catalogDateGetDetailMultiCommandHandler;
        private readonly ICatalogDateGetListCommandHandler _catalogDateGetListCommandHandler;
        private readonly ICreateAccountCommandHandler _createAccountCommandHandler;
        private readonly IGetOrderEticketCommandHandler _getOrderEticketCommandHandler;
        private readonly IOrderConfirmCommandHandler _orderConfirmCommandHandler;
        private readonly IReleaseSeatsCommandHandler _releaseSeatsCommandHandler;
        private readonly ITempOrderGetDetailCommandHandler _tempOrderGetDetailCommandHandler;
        private readonly ITempOrderGetSendingFeesCommandHandler _tempOrderGetSendingFeesCommandHandler;
        private readonly ITempOrderSetSendingFeesCommandHandler _tempOrderSetSendingFeesCommandHandler;

        private readonly IAddToBasketConverter _addToBasketConverter;
        private readonly IAllocSeatsAutomaticConverter _allocSeatsAutomaticConverter;
        private readonly IDateAndPriceConverter _dateAndPriceConverter;
        private readonly ITempOrderGetDetailConverter _tempOrderGetDetailConverter;
        private readonly bool _isMock;
        private readonly string _path;
        private readonly string _mrMockingPath;

        #endregion "Private Members"

        #region "Constructor"

        public MoulinRougeAdapter(IAddToBasketCommandHandler addToBasketCommandHandler, IAllocSeatsAutomaticCommandHandler allocSeatsAutomaticCommandHandler,
            ICatalogDateGetDetailMultiCommandHandler catalogDateGetDetailMultiCommandHandler, ICatalogDateGetListCommandHandler catalogDateGetListCommandHandler,
            ICreateAccountCommandHandler createAccountCommandHandler, IGetOrderEticketCommandHandler getOrderEticketCommandHandler,
            IOrderConfirmCommandHandler orderConfirmCommandHandler, IReleaseSeatsCommandHandler releaseSeatsCommandHandler,
            ITempOrderGetDetailCommandHandler tempOrderGetDetailCommandHandler, ITempOrderGetSendingFeesCommandHandler tempOrderGetSendingFeesCommandHandler,
            ITempOrderSetSendingFeesCommandHandler tempOrderSetSendingFeesCommandHandler, IAddToBasketConverter addToBasketConverter,
            IAllocSeatsAutomaticConverter allocSeatsAutomaticConverter, IDateAndPriceConverter dateAndPriceConverter,
            ITempOrderGetDetailConverter tempOrderGetDetailConverter)
        {
            _addToBasketCommandHandler = addToBasketCommandHandler;
            _allocSeatsAutomaticCommandHandler = allocSeatsAutomaticCommandHandler;
            _catalogDateGetDetailMultiCommandHandler = catalogDateGetDetailMultiCommandHandler;
            _catalogDateGetListCommandHandler = catalogDateGetListCommandHandler;
            _createAccountCommandHandler = createAccountCommandHandler;
            _getOrderEticketCommandHandler = getOrderEticketCommandHandler;
            _orderConfirmCommandHandler = orderConfirmCommandHandler;
            _releaseSeatsCommandHandler = releaseSeatsCommandHandler;
            _tempOrderGetDetailCommandHandler = tempOrderGetDetailCommandHandler;
            _tempOrderGetSendingFeesCommandHandler = tempOrderGetSendingFeesCommandHandler;
            _tempOrderSetSendingFeesCommandHandler = tempOrderSetSendingFeesCommandHandler;
            _addToBasketConverter = addToBasketConverter;
            _allocSeatsAutomaticConverter = allocSeatsAutomaticConverter;
            _dateAndPriceConverter = dateAndPriceConverter;
            _tempOrderGetDetailConverter = tempOrderGetDetailConverter;
            try
            {
                _isMock = Util.ConfigurationManagerHelper.GetValuefromAppSettings("isMock_MR") == "1";
                _path = AppDomain.CurrentDomain.BaseDirectory;
                _mrMockingPath = $"{_path}Mock-Samples\\MR";
            }
            catch
            {
                _isMock = false;
            }
        }

        #endregion "Constructor"

        #region MoulinRouge Api Calls

        /// <summary>
        /// 03) Its blocks the seat for Moulin Rouge and if booking is not confirmed then it is release automtically in 30 minutes
        /// </summary>
        /// <param name="inputContext"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<object> AllocSeatsAutomaticAsync(MoulinRougeCriteria inputContext, string token)
        {
            var res = await _allocSeatsAutomaticCommandHandler.ExecuteAsync(inputContext, MethodType.Allocseatsautomatic, token);

            if (res == null) return null;
            var objectFromXml = GetObjectFromXml<AllocSeatsAutomatic.Response>(res.ToString());
            if (objectFromXml == null || !objectFromXml.Body.ACP_AllocSeatsAutomaticRequestResponse
                    .ACP_AllocSeatsAutomaticRequestResult) return null;
            var resultConverted = _allocSeatsAutomaticConverter.Convert(objectFromXml);

            if (resultConverted != null && Convert.ToString(resultConverted.GetType().FullName).ToLower(CultureInfo.InvariantCulture).Contains(Constant.AllocatedSeat) && Convert.ToString(resultConverted.GetType().FullName).ToLower(CultureInfo.InvariantCulture).Contains(Constant.List))
            {
                var allocatedSeats = resultConverted as List<AllocatedSeat>;
                var validRateIdContingentIDs = MoulinRougeAPIConfig.GetInstance().ValidRateIdContingentIDs;

                allocatedSeats?.ForEach(seat =>
                    {
                        var configItem = validRateIdContingentIDs.FirstOrDefault(item => item.RateId == inputContext.MoulinRougeContext.RateId && item.ContingentId == inputContext.MoulinRougeContext.ContingentId);
                        seat.ServiceType = configItem != null ? configItem.Type.ToString() : string.Empty;
                        seat.ServiceTypeName = configItem != null ? configItem.Name.ToString() : string.Empty;
                        seat.IsPriceChanged = (inputContext.MoulinRougeContext.Amount != seat.Amount);
                        seat.CatalogDateId = inputContext.MoulinRougeContext.CatalogDateId;
                        seat.CategoryId = inputContext.MoulinRougeContext.CategoryId;
                        seat.ContingentId = inputContext.MoulinRougeContext.ContingentId;
                        seat.BlocId = inputContext.MoulinRougeContext.BlocId;
                        seat.FloorId = inputContext.MoulinRougeContext.FloorId;
                        seat.RateId = inputContext.MoulinRougeContext.RateId;
                        seat.Quantity = inputContext.MoulinRougeContext.Quantity;
                        seat.DateStart = inputContext.CheckinDate;
                        seat.DateEnd = inputContext.CheckoutDate;
                    }
                );

                return allocatedSeats;
            }
            return null;
        }

        /// <summary>
        /// 04) Get Detail of temp order
        /// </summary>
        /// <param name="inputContext"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<object> GetTempOrderGetDetailAsync(MoulinRougeCriteria inputContext, string token)
        {
            var res = await _tempOrderGetDetailCommandHandler.ExecuteAsync(inputContext, MethodType.Tempordergetdetail, token);

            if (res != null)
            {
                var objectFromXml = GetObjectFromXml<TempOrderGetDetail.Response>(res.ToString());
                if (objectFromXml != null)
                {
                    var resultConverted = _tempOrderGetDetailConverter.Convert(objectFromXml);

                    if (!Convert.ToString(resultConverted.GetType().FullName).ToLowerInvariant()
                            .Contains(Constant.AllocatedSeat) ||
                        !Convert.ToString(resultConverted.GetType().FullName).ToLowerInvariant().Contains(Constant.List))
                        return resultConverted;
                    var allocatedSeats = resultConverted as List<AllocatedSeat>;

                    return allocatedSeats;
                }
            }
            return null;
        }

        /// <summary>
        /// 05) It can be used anytime after making AllocSeatsAutomatic calls and before order confirm.
        /// It is used to release blocked seat.
        /// </summary>
        /// <param name="inputContext"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<bool> ReleaseSeatsAsync(MoulinRougeCriteria inputContext, string token)
        {
            var res = await _releaseSeatsCommandHandler.ExecuteAsync(inputContext, MethodType.Releaseseats, token);

            if (res == null) return false;
            var objectFromXml = GetObjectFromXml<ReleaseSeats.Response>(res.ToString());
            if (objectFromXml == null) return false;
            return objectFromXml.Body?.AcpReleaseSeatsRequestResponse != null && objectFromXml.Body.AcpReleaseSeatsRequestResponse.AcpReleaseSeatsRequestResult;
        }

        /// <summary>
        /// 06) For order confirm SendingFees is required to set , and this call gives the sending fee detail for the tempOrder booked so far.
        /// </summary>
        /// <param name="temporaryOrderId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<object> TempOrderGetSendingFeesAsync(string temporaryOrderId, string token)
        {
            var inputContext = new MoulinRougeCriteria { MoulinRougeContext = new APIContextMoulinRouge { TemporaryOrderId = temporaryOrderId } };
            var res = await _tempOrderGetSendingFeesCommandHandler.ExecuteAsync(inputContext, MethodType.Tempordergetsendingfees, token);

            if (res == null) return null;
            var objectFromXml = GetObjectFromXml<TempOrderGetSendingFees.Response>(res.ToString());
            return objectFromXml;
        }

        /// <summary>
        /// A1) Combined Request 01) CatalogDateGetList and 02) CatalogDateGetDetailMulti.
        /// It should be used as 1st call to get date, price and availability for moulin rouge show.
        /// It retuns Converted ActivityOption that will be used in our exiting flow
        /// </summary>
        /// <returns></returns>
        public List<Activity> GetConvertedActivtyDateAndPrice(DateTime dateFrom, DateTime dateTo, int quantity, string token)
        {
            var result = GetDateAndPrice(dateFrom, dateTo, quantity, token);

            if (result?.Count <= 0) return new List<Activity>();
            var convertedResult = _dateAndPriceConverter.Convert(result);
            return convertedResult as List<Activity>;
        }

        /// <summary>
        /// B1) Add product to Cart Asynchronously
        /// </summary>
        /// <returns> Return products details that have been added in Cart </returns>
        public MoulinRougeSelectedProduct AddToCart(MoulinRougeSelectedProduct inputContext, string token)
        {
            var res = new object();

            if (_isMock)
            {
                try
                {
                    var filePath = $"{_mrMockingPath}\\order_00_addtocart_res.xml";
                    if (System.IO.File.Exists(filePath))
                    {
                        res = File.ReadAllText(filePath);
                    }
                }
                catch
                {
                }
            }
            else
            {
                res = _addToBasketCommandHandler.ExecuteAsync(inputContext, MethodType.Addtocart, token).Result;
            }

            if (res == null) return null;

            var objectFromXml = GetObjectFromXml<AllocSeatsAutomatic.Response>(res.ToString());
            if (objectFromXml == null || !objectFromXml.Body.ACP_AllocSeatsAutomaticRequestResponse.ACP_AllocSeatsAutomaticRequestResult) return null;
            var resultConverted = _addToBasketConverter.Convert(objectFromXml) as MoulinRougeSelectedProduct;

            if (resultConverted == null || resultConverted?.TemporaryOrderId == "0") return null;
            resultConverted.CategoryId = 1;
            resultConverted.CatalogDateId = inputContext.CatalogDateId;
            resultConverted.CategoryId = inputContext.CategoryId;
            resultConverted.ContingentId = inputContext.ContingentId;
            resultConverted.BlocId = inputContext.BlocId;
            resultConverted.FloorId = inputContext.FloorId;
            resultConverted.RateId = inputContext.RateId;
            resultConverted.Quantity = inputContext.Quantity;

            return resultConverted;
        }

        public AllocSeatsAutomatic.Response AddToCartAPI(MoulinRougeSelectedProduct inputContext, string token)
        {
            var res = new object();

            if (_isMock)
            {
                try
                {
                    var filePath = $"{_mrMockingPath}\\order_00_addtocart_res.xml";
                    if (System.IO.File.Exists(filePath))
                    {
                        res = File.ReadAllText(filePath);
                    }
                }
                catch
                {
                }
            }
            else
            {
                res = _addToBasketCommandHandler.ExecuteAsync(inputContext, MethodType.Addtocart, token).Result;
            }

            if (res == null) return null;

            var objectFromXml = GetObjectFromXml<AllocSeatsAutomatic.Response>(res.ToString());
            if (objectFromXml == null || !objectFromXml.Body.ACP_AllocSeatsAutomaticRequestResponse.ACP_AllocSeatsAutomaticRequestResult) return null;
            return objectFromXml;
        }

        /// <summary>
        /// C1) Order Confirm For Selected Moulin Rouge Products Asynchronously
        /// this call combines 1)TempOrderGetSendingFeesAsync,2) TempOrderSetSendingFeesAsync, 3)CreateAccount and 4)OrderConfirmAsync
        /// </summary>
        /// <returns> Return products details that have been added in Cart </returns>
        ///
        ///
        public MoulinRougeSelectedProduct OrderConfirmCombined(MoulinRougeSelectedProduct selectedProduct, out string requestXml, out string responseXml, string token)
        {
            var isResultSetSendingFee = false;
            var isResultOrderConfirm = false;
            var temporaryOrderId = "0";
            decimal getAmount = 0;
            requestXml = string.Empty;
            responseXml = string.Empty;

            if (selectedProduct != null)
            {
                var productOptionsSelected = selectedProduct.ProductOptions.Where(x => x.IsSelected).ToList();
                if (productOptionsSelected?.Count > 0)
                {
                    var dateandAvaillabilitySell = productOptionsSelected[0].SellPrice.DatePriceAndAvailabilty.Where(x => x.Value.IsSelected).ToList();
                    if (dateandAvaillabilitySell.Count > 0)
                    {
                        temporaryOrderId = selectedProduct.TemporaryOrderId;
                        getAmount = productOptionsSelected[0].SellPrice.Amount;
                    }
                    temporaryOrderId = string.IsNullOrWhiteSpace(temporaryOrderId) ? "0" : temporaryOrderId;
                }
            }

            var input = new MoulinRougeCriteria
            {
                MoulinRougeContext = new APIContextMoulinRouge
                {
                    TemporaryOrderId = temporaryOrderId,
                    FeeType = Constant.Eticket,
                    UnitAmount = 0,
                    GlobalAmount = 0,
                    SendingFeeId = -1,
                    Nombre = 1,
                    Status = 0,
                    TypeCalcul = 0
                }
            };

            var resultTempOrderSetSendingFees = TempOrderSetSendingFeesAsync(input, token)?.GetAwaiter().GetResult() as TempOrderSetSendingFees.Response;
            if (resultTempOrderSetSendingFees?.Body?.ACP_TempOrderSetSendingFeesRequestResponse != null)
                isResultSetSendingFee = resultTempOrderSetSendingFees.Body.ACP_TempOrderSetSendingFeesRequestResponse.ACP_TempOrderSetSendingFeesRequestResult;
            if (isResultSetSendingFee)
            {
                var resultCustomerCreate =
                    CreateAccountAsync(selectedProduct?.FullName, selectedProduct?.FirstName, token).Result as
                        CreateAccount.Response;
                if (resultCustomerCreate?.Body?.ACP_CreateAccountRequestResponse != null)
                {
                    var consumer = resultCustomerCreate.Body.ACP_CreateAccountRequestResponse;
                    if (consumer?.identity?.ID_Identity > 0)
                    {
                        if (selectedProduct != null)
                            selectedProduct.IdentityConsumerId = consumer.identity.ID_Identity;
                        var resultOrderConfirm = OrderConfirm(temporaryOrderId, getAmount,
                            selectedProduct?.IsangoBookingReferenceNumber, selectedProduct?.IdentityConsumerId ?? 0,
                            out requestXml, out responseXml, token) as OrderConfirm.Response;
                        if (resultOrderConfirm?.Body?.ACP_OrderConfirmRequestResponse != null)
                            isResultOrderConfirm = resultOrderConfirm.Body.ACP_OrderConfirmRequestResponse
                                .ACP_OrderConfirmRequestResult;

                        if (isResultOrderConfirm)
                        {
                            var orderId =
                                Convert.ToString(resultOrderConfirm.Body.ACP_OrderConfirmRequestResponse.id_Order);
                            if (selectedProduct != null)
                            {
                                selectedProduct.OrderId = orderId;
                                selectedProduct.OrderMrid = resultOrderConfirm.Body.ACP_OrderConfirmRequestResponse.id_orderMR;
                            }

                            if (resultOrderConfirm.Body.ACP_OrderConfirmRequestResponse.listOrderRow?.ACPO_ConfirmOrderRow != null)
                            {
                                var eTicketGuiDs = resultOrderConfirm.Body.ACP_OrderConfirmRequestResponse.listOrderRow.ACPO_ConfirmOrderRow.listeticketGUID.ToList();
                                var temporaryOrderRowId = Convert.ToString(resultOrderConfirm.Body.ACP_OrderConfirmRequestResponse.listOrderRow.ACPO_ConfirmOrderRow.ID_TemporaryOrderRow);

                                if (selectedProduct != null)
                                {
                                    selectedProduct.TemporaryOrderRowId = temporaryOrderRowId;
                                    selectedProduct.ETicketGuiDs = eTicketGuiDs;
                                    selectedProduct.MrConfirmedTickets = new List<ConfirmedTicket>();
                                }

                                foreach (var item in resultOrderConfirm.Body.ACP_OrderConfirmRequestResponse
                                    .listOrderRow
                                    .ACPO_ConfirmOrderRow.listBarCodes)
                                {
                                    var confirmedTicket = new ConfirmedTicket
                                    {
                                        BarCode = item.BarCode,
                                        FiscalNumber = item.FiscalNumber,
                                        SeatId = item.ID_Seat,
                                    };
                                    selectedProduct?.MrConfirmedTickets.Add(confirmedTicket);
                                }

                                #region Block to download pdf stream for Moulin Rouge tickets

                                var attachments = new List<Attachment>();
                                var confirmedTicketBytes = new List<Byte[]>();

                                var count = 1;
                                foreach (var ticketGuid in eTicketGuiDs)
                                {
                                    var pdFdoc = GetOrderEticketAsync(orderId: orderId, guid: ticketGuid, token: token)?.GetAwaiter().GetResult();
                                    if (pdFdoc.Length > 0)
                                    {
                                        var memStream = new MemoryStream();
                                        memStream.Write(pdFdoc, 0, pdFdoc.Length);
                                        memStream.Position = 0;
                                        var moulinRougeTicketAttachment = new Attachment(memStream,
                                            $"MoulinRougeTicket-{orderId}-{count}.pdf", "application/pdf");
                                        //attachments.Add(moulinRougeTicketAttachment);
                                        confirmedTicketBytes.Add(pdFdoc);
                                    }

                                    count++;
                                }

                                if (selectedProduct != null)
                                {
                                    selectedProduct.ConfirmedTicketAttachments = attachments;
                                    selectedProduct.ConfirmedTicketBytes = confirmedTicketBytes;
                                }
                            }

                            #endregion Block to download pdf stream for Moulin Rouge tickets

                            return selectedProduct;
                        }
                        //else
                        //{
                        //    throw new Exception("MounlinRougeAPI > Order Confirm failed for tempOrderID " +
                        //                        temporaryOrderId);
                        //}
                    }
                    //else
                    //{
                    //    throw new Exception("MounlinRougeAPI > Consumer account creation failed for tempOrderID " +
                    //                        temporaryOrderId);
                    //}
                }
                //else
                //{
                //    throw new Exception("MounlinRougeAPI > Consumer account creation failed for tempOrderID " +
                //                        temporaryOrderId);
                //}
            }
            //else
            //{
            //    throw new Exception("MounlinRougeAPI > Set Sending Fees failed for tempOrderID " + temporaryOrderId);
            //}
            return selectedProduct;
        }

        #endregion MoulinRouge Api Calls

        #region Other Methods

        /// <summary>
        /// Create object from xml document
        /// </summary>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="xmlString"></param>
        /// <returns></returns>
        public static TOut GetObjectFromXml<TOut>(string xmlString)
        {
            // Create an instance of the XmlSerializer specifying type and namespace.
            var serializer = new XmlSerializer(typeof(TOut));

            // Declare an object variable of the type to be deserialized.
            TOut result;

            // A FileStream is needed to read the XML document.
            using (var xmlStringStream = new MemoryStream(Encoding.UTF8.GetBytes(xmlString)))
            {
                var reader = XmlReader.Create(xmlStringStream);
                // Use the Deserialize method to restore the object's state.
                result = (TOut)serializer.Deserialize(reader);
            }
            // Write out the properties of the object.
            return result;
        }

        /// <summary>
        /// 01) Get Available shows as per selected date
        /// </summary>
        /// <param name="inputContext"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private async Task<object> CatalogDateGetListAsync(MoulinRougeCriteria inputContext, string token)
        {
            var res = await _catalogDateGetListCommandHandler.ExecuteAsync(inputContext, MethodType.Catalogdategetlist, token);

            if (res == null) return null;
            var objectFromXml = GetObjectFromXml<CatalogDateGetList.Response>(res.ToString());
            if (objectFromXml == null) return null;
            var returnValue = objectFromXml;
            var filteredResult = returnValue.Body?.ACP_CatalogDateGetListRequestResponse?.catalogDateList?.Where(dt => dt.DateStart >= DateTime.Now.Date).ToList();
            return filteredResult;
        }

        /// <summary>
        /// 02) Get Price and availability for the selected date
        /// </summary>
        /// <param name="inputContext"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private async Task<object> CatalogDateGetDetailMultiAsync(MoulinRougeCriteria inputContext, string token)
        {
            var res = await _catalogDateGetDetailMultiCommandHandler.ExecuteAsync(inputContext, MethodType.Catalogdategetdetailmulti, token);

            if (res == null) return null;
            var objectFromXml = GetObjectFromXml<CatalogDateGetDetailMulti.Response>(res.ToString());

            var response = objectFromXml?.Body.ACP_CatalogDateGetDetailMultiRequestResponse;
            return response;
        }

        /// <summary>
        /// 07) Set sending fee for temp order so that it can be confirmed
        /// </summary>
        /// <param name="inputContext"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private async Task<object> TempOrderSetSendingFeesAsync(MoulinRougeCriteria inputContext, string token)
        {
            var res = new object();

            if (_isMock)
            {
                try
                {
                    var filePath = $"{_mrMockingPath}\\order_01_tempordersetsendingfees_res.xml";
                    if (System.IO.File.Exists(filePath))
                    {
                        res = File.ReadAllText(filePath);
                    }
                }
                catch
                {
                }
            }
            else
            {
                res = await _tempOrderSetSendingFeesCommandHandler.ExecuteAsync(inputContext, MethodType.Tempordersetsendingfees, token);
            }

            if (res == null) return null;
            var objectFromXml = GetObjectFromXml<TempOrderSetSendingFees.Response>(res.ToString());
            return objectFromXml;
        }

        /// <summary>
        /// 08) Create account at Moulin Rouge API end so that its Generated ID can be used for confirming order.
        /// </summary>
        /// <param name="fullname"></param>
        /// <param name="firstName"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private async Task<object> CreateAccountAsync(string fullname, string firstName, string token)
        {
            var inputContext = new MoulinRougeCriteria
            {
                MoulinRougeContext = new APIContextMoulinRouge { FullName = fullname, FirstName = firstName }
            };
            var res = new object();

            if (_isMock)
            {
                try
                {
                    var filePath = $"{_mrMockingPath}\\order_02_createaccount_res.xml";
                    if (System.IO.File.Exists(filePath))
                    {
                        res = File.ReadAllText(filePath);
                    }
                }
                catch
                {
                }
            }
            else
            {
                res = await _createAccountCommandHandler.ExecuteAsync(inputContext, MethodType.Createaccount, token);
            }

            if (res == null) return null;
            var objectFromXml = GetObjectFromXml<CreateAccount.Response>(res.ToString());
            return objectFromXml;
        }

        /// <summary>
        /// 09) Confirm the order
        /// </summary>
        /// <param name="temporaryOrderId"></param>
        /// <param name="amount"></param>
        /// <param name="isangoBookingReferenceNumber"></param>
        /// <param name="identityConsumerId"></param>
        /// <param name="apiRequest"></param>
        /// <param name="apiResponse"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private object OrderConfirm(string temporaryOrderId, decimal amount, string isangoBookingReferenceNumber, int identityConsumerId, out string apiRequest, out string apiResponse, string token)
        {
            apiRequest = string.Empty;
            apiResponse = string.Empty;
            var inputContext = new MoulinRougeCriteria { MoulinRougeContext = new APIContextMoulinRouge { TemporaryOrderId = temporaryOrderId, Amount = amount, IsangoBookingReferenceNumber = isangoBookingReferenceNumber, IdentityConsumerId = identityConsumerId } };
            var res = new object();

            if (_isMock)
            {
                try
                {
                    var filePath = $"{_mrMockingPath}\\order_03_orderconfirm_res.xml";
                    if (System.IO.File.Exists(filePath))
                    {
                        res = File.ReadAllText(filePath);
                    }
                }
                catch
                {
                }
            }
            else
            {
                res = _orderConfirmCommandHandler.Execute(inputContext, MethodType.Orderconfirm, token, out apiRequest, out apiResponse);
            }

            if (res == null) return null;
            var objectFromXml = GetObjectFromXml<OrderConfirm.Response>(res.ToString());
            return objectFromXml != null &&
                   objectFromXml.Body.ACP_OrderConfirmRequestResponse.ACP_OrderConfirmRequestResult
                ? objectFromXml
                : res;
        }

        /// <summary>
        /// 10) Get Moulin Rouge Eticket
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="guid"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<byte[]> GetOrderEticketAsync(string orderId, string guid, string token)
        {
            try
            {
                var inputContext = new MoulinRougeCriteria { MoulinRougeContext = new APIContextMoulinRouge { OrderId = orderId, Guid = guid } };

                var res = new object();

                if (_isMock)
                {
                    try
                    {
                        var filePath = $"{_mrMockingPath}\\order_04_getordereticket_res_{guid}.xml";
                        if (System.IO.File.Exists(filePath))
                        {
                            res = File.ReadAllText(filePath);
                        }
                        else
                        {
                            filePath = $"{_mrMockingPath}\\order_04_getordereticket_res_5beaef77-2779-4484-acaa-6f7fb2cad9ea.xml";
                            res = File.ReadAllText(filePath);
                        }
                    }
                    catch  
                    {
                    }
                }
                else
                {
                    res = await _getOrderEticketCommandHandler.ExecuteAsync(inputContext, MethodType.Getordereticket, token);
                }

                /*
                //#MOCK
                var response = File.ReadAllText(@"I:\Default Dw\1.json");
                //*/

                if (res == null) return null;

                var objectFromXml = GetObjectFromXml<GetOrderEticket.Response>(res?.ToString());
                var result = objectFromXml != null &&
                       objectFromXml.Body.ACP_GetOrderEticketRequestResponse.ACP_GetOrderEticketRequestResult
                    ? objectFromXml.Body.ACP_GetOrderEticketRequestResponse.eTicketBytes
                    : null;

                //#MOCK
                //System.IO.File.WriteAllBytes(@"c:\temp\Ticket2.pdf", result);
                return result;
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        /// <summary>
        /// A2) Combined Request 01) CatalogDateGetList and 02) CatalogDateGetDetailMulti.
        /// It gives date, price and availlablity for moulin rouge show.
        /// Used by A1) as input to create desired result
        /// </summary>
        /// <returns></returns>
        private List<DateAndPrice> GetDateAndPrice(DateTime dateFrom, DateTime dateTo, int quantity, string token)
        {
            var inputContext = new MoulinRougeCriteria { CheckinDate = dateFrom, CheckoutDate = dateTo.AddDays(1) };
            var resultCatalogDateGetList = new List<ACP_CatalogDateGetListRequestResponseACPO_CatalogDate>();
            var res = new object();
            if (_isMock)
            {
                try
                {
                    var filePath = $"{_mrMockingPath}\\CD_RES.xml";
                    if (System.IO.File.Exists(filePath))
                    {
                        res = File.ReadAllText(filePath);
                        if (res == null) return null;
                        var objectFromXml = GetObjectFromXml<CatalogDateGetList.Response>(res.ToString());
                        if (objectFromXml == null) return null;
                        var returnValue = objectFromXml;
                        var filteredResult = returnValue.Body?.ACP_CatalogDateGetListRequestResponse?.catalogDateList?.Where(dt => dt.DateStart >= DateTime.Now.Date).ToList();
                        resultCatalogDateGetList= filteredResult;
                    }
                    else
                    {
                        filePath = $"{_mrMockingPath}\\CD_RES.xml";
                        res = File.ReadAllText(filePath);
                        if (res == null) return null;
                        var objectFromXml = GetObjectFromXml<CatalogDateGetList.Response>(res.ToString());
                        if (objectFromXml == null) return null;
                        var returnValue = objectFromXml;
                        var filteredResult = returnValue.Body?.ACP_CatalogDateGetListRequestResponse?.catalogDateList?.Where(dt => dt.DateStart >= DateTime.Now.Date).ToList();
                        resultCatalogDateGetList= filteredResult;
                    }
                }
                catch
                {
                }
            }
            else
            {
                 resultCatalogDateGetList = CatalogDateGetListAsync(inputContext, token).Result as List<CatalogDateGetList.ACP_CatalogDateGetListRequestResponseACPO_CatalogDate>;
            }
            if (resultCatalogDateGetList?.Count > 0)
            {
                var query = from item in resultCatalogDateGetList
                            where item != null
                            select new { ID_CatalogDate = item.ID_CatalogDate }.ID_CatalogDate;
                var catalogDates = query.ToList();
                if (catalogDates.Count > 0)
                {
                    inputContext = new MoulinRougeCriteria { MoulinRougeContext = new APIContextMoulinRouge { Ids = catalogDates } };

                    var resultDetailMulti = new CatalogDateGetDetailMulti.ACP_CatalogDateGetDetailMultiRequestResponse();

                    if (_isMock)
                    {
                        try
                        {
                            var filePath = $"{_mrMockingPath}\\CDM.RES.xml";
                            if (System.IO.File.Exists(filePath))
                            {
                                res = File.ReadAllText(filePath);
                                if (res == null) return null;

                               
                                var objectFromXml = GetObjectFromXml<CatalogDateGetDetailMulti.Response>(res.ToString());

                                resultDetailMulti = objectFromXml?.Body.ACP_CatalogDateGetDetailMultiRequestResponse;



                            }
                            else
                            {
                                filePath = $"{_mrMockingPath}\\CDM.RES.xml";
                                res = File.ReadAllText(filePath);
                                if (res == null) return null;
                                var objectFromXml = GetObjectFromXml<CatalogDateGetDetailMulti.Response>(res.ToString());

                                resultDetailMulti = objectFromXml?.Body.ACP_CatalogDateGetDetailMultiRequestResponse;

                            }
                        }
                        catch
                        {
                        }
                    }
                    else
                    {

                        resultDetailMulti = (ACP_CatalogDateGetDetailMultiRequestResponse)CatalogDateGetDetailMultiAsync(inputContext, token).Result;
                    }

                    var response = resultDetailMulti as CatalogDateGetDetailMulti.ACP_CatalogDateGetDetailMultiRequestResponse;
                    var validRateIDsContingentIDs = MoulinRougeAPIConfig.GetInstance().ValidRateIdContingentIDs;

                    if (response != null)
                    {
                        var dinners = from dp in response.ListCatalogDatePrice
                                      from cd in response.ListCatalogDateLock
                                      from v in validRateIDsContingentIDs
                                      from dt in resultCatalogDateGetList
                                      where dp.ID_CatalogDate == cd.ID_CatalogDate
                                      && dp.ID_Category == cd.ID_Category
                                      && dp.ID_Contingent == cd.ID_ContingentRepas
                                      && cd.LockRepas == false
                                      && dp.ID_Contingent == v.ContingentId
                                      && dp.ID_Rate == v.RateId
                                      && v.Type == MoulinRougeServiceType.Dinner
                                      && dt.ID_CatalogDate == dp.ID_CatalogDate
                                      select new DateAndPrice
                                      {
                                          ServiceType = v.Type,
                                          ServiceTypeName = v.Name,
                                          DateStart = dt.DateStart,
                                          DateEnd = dt.DateEnd,
                                          CatalogDateId = dp.ID_CatalogDate,
                                          RateId = dp.ID_Rate,
                                          ContingentId = dp.ID_Contingent,
                                          AcceptDemat = dp.AcceptDemat,
                                          CategoryId = dp.ID_Category,
                                          FloorId = dp.ID_Floor,
                                          Amount = dp.Amount,
                                          AmountDetail = dp.AmountDetail,
                                          BlocId = dp.ID_Bloc,
                                          NbMini = dp.NbMini,
                                          Stock = dp.Stock,
                                          CatalogDateType = cd.CatalogDateType,
                                          Category = cd.Category,
                                          ContingentIdRepas = cd.ID_ContingentRepas,
                                          ContingentIdRevue = cd.ID_ContingentRevue,
                                          LockRepas = cd.LockRepas,
                                          LockRevue = cd.LockRevue,
                                          ContingentRepas = cd.ContingentRepas,
                                          ContingentRevue = cd.ContingentRevue,
                                          ActivityId = 16628,
                                          OptionCode = dp.ID_Rate + "~" + dt.DateStart.ToString("yyyymmdd HH:00:00").Split(' ')[1],
                                          Quantity = quantity
                                      };

                        var shows = from dp in response.ListCatalogDatePrice
                                    from cd in response.ListCatalogDateLock
                                    from v in validRateIDsContingentIDs
                                    from dt in resultCatalogDateGetList
                                    where dp.ID_CatalogDate == cd.ID_CatalogDate
                                          && dp.ID_Category == cd.ID_Category
                                          && dp.ID_Contingent == cd.ID_ContingentRevue
                                          && cd.LockRevue == false
                                          && dp.ID_Contingent == v.ContingentId
                                          && dp.ID_Rate == v.RateId
                                          && v.Type == MoulinRougeServiceType.Show
                                          && dt.ID_CatalogDate == dp.ID_CatalogDate
                                    select new DateAndPrice
                                    {
                                        ServiceType = v.Type,
                                        ServiceTypeName = v.Name,
                                        DateStart = dt.DateStart,
                                        DateEnd = dt.DateEnd,
                                        CatalogDateId = dp.ID_CatalogDate,
                                        RateId = dp.ID_Rate,
                                        ContingentId = dp.ID_Contingent,
                                        AcceptDemat = dp.AcceptDemat,
                                        CategoryId = dp.ID_Category,
                                        FloorId = dp.ID_Floor,
                                        Amount = dp.Amount,
                                        AmountDetail = dp.AmountDetail,
                                        BlocId = dp.ID_Bloc,
                                        NbMini = dp.NbMini,
                                        Stock = dp.Stock,
                                        CatalogDateType = cd.CatalogDateType,
                                        Category = cd.Category,
                                        ContingentIdRepas = cd.ID_ContingentRepas,
                                        ContingentIdRevue = cd.ID_ContingentRevue,
                                        LockRepas = cd.LockRepas,
                                        LockRevue = cd.LockRevue,
                                        ContingentRepas = cd.ContingentRepas,
                                        ContingentRevue = cd.ContingentRevue,
                                        ActivityId = 22645,
                                        OptionCode = $"{dp.ID_Rate}~{dt.DateStart.ToString("yyyymmdd HH:00:00").Split(' ')[1]}",
                                        Quantity = quantity
                                    };
                        var dinnerList = dinners.ToList();
                        var showsList = shows.ToList();
                        var returnResult = dinnerList;
                        returnResult.AddRange(showsList);
                        return returnResult;
                    }
                }
            }
            return new List<DateAndPrice>();
        }

        #endregion Other Methods
    }
}