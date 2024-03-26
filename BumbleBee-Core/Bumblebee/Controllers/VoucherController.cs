using Isango.Entities.Enums;
using Isango.Mailer.ServiceContracts;
using Isango.Service.Contract;
using Microsoft.AspNetCore.Mvc;
using ServiceAdapters.MoulinRouge;
using System.Net;
using Util;
using WebAPI.Filters;
using WebAPI.Models.ResponseModels;

namespace WebAPI.Controllers
{
    /// <summary>
    /// Provide details of all Activity related endpoints
    /// </summary>
    [Route("api/voucher")]
    [ApiController]

    public class VoucherController : ControllerBase
    {
        private readonly IMailAttachmentService _mailAttachmentService;
        private readonly IMailerService _mailerService;
        private readonly IMoulinRougeAdapter _moulinRougeAdapter;
        /// <summary>
        /// Parameterized Constructor to initialize all dependencies.
        /// </summary>
        /// <param name="mailAttachmentService"></param>
        /// <param name="mailerService"></param>
        /// <param name="moulinRougeAdapter"></param>
        public VoucherController(IMailAttachmentService mailAttachmentService, IMailerService mailerService, IMoulinRougeAdapter moulinRougeAdapter)
        {
            _mailAttachmentService = mailAttachmentService;
            _mailerService = mailerService;
            _moulinRougeAdapter = moulinRougeAdapter;
        }
        [Route("book/{bookingReferenceNumber}")]
        [Route("v1/book/{bookingReferenceNumber}")]
        [HttpGet]
        [ValidateModel]
        public IActionResult GetBookedVoucher(string bookingReferenceNumber, int? source = 1, int? bookedoptionid = 0, bool? iscancelled = false)
        {
            var isPDFVoucher = ConfigurationManagerHelper.GetValuefromAppSettings("VoucherPDF") == "1";
            var voucherName = isPDFVoucher ? $"{bookingReferenceNumber}.pdf" : $"{bookingReferenceNumber}.html";

            //get byte array
            var byteArrayData = bookedoptionid == null ? _mailAttachmentService.GetBookedVoucher(bookingReferenceNumber, isPDFVoucher, source ?? 3)
                                : _mailAttachmentService.GetBookedVoucherNew(bookingReferenceNumber, isPDFVoucher, source ?? 3, bookedoptionid, iscancelled);

            //Rayna Case
            if (byteArrayData.Item2.ToString() == ((int)APIType.Rayna).ToString() && !string.IsNullOrEmpty(byteArrayData.Item3) && byteArrayData.Item3.ToLower().Contains("http"))
            {
                return Redirect(byteArrayData.Item3);
            }

            var byteArray = byteArrayData?.Item1;

            if (byteArray == null)
            {
                if (isPDFVoucher)
                {
                    try
                    {
                        _mailerService.SendPDFErrorMail(bookingReferenceNumber);
                    }
                    catch (Exception)
                    {
                        //ignored
                    }
                    voucherName = $"{bookingReferenceNumber}.html";
                    byteArray = _mailAttachmentService.GetBookedVoucher(bookingReferenceNumber, false).Item1;
                }
                else
                {
                    return BadRequest(); // Return Bad Request if byteArray is null and not PDF voucher
                }
            }

            //adding bytes to memory stream
            var dataStream = new MemoryStream(byteArray);

            return File(dataStream, "application/octet-stream", voucherName);
        }
    
        [Route("invoice/{bookingReferenceNumber}")]
        [HttpGet]
        [ValidateModel]
        public IActionResult GetBookedInvoice(string bookingReferenceNumber, int? source = 4)
        {
            var httpRequestMessage = new HttpRequestMessage();
            //HttpResponseMessage httpResponseMessage;
            var isPDFVoucher = ConfigurationManagerHelper.GetValuefromAppSettings("VoucherPDF") == "1";
            var voucherName = isPDFVoucher ? $"{bookingReferenceNumber}_Invoice.pdf" : $"{bookingReferenceNumber}_Invoice.html";
            //get byte array
            var byteArray = _mailAttachmentService.GetBookedInvoice(bookingReferenceNumber, isPDFVoucher, source ?? 4);

            if (byteArray == null)
            {
                if (isPDFVoucher)
                {
                    try
                    {
                        _mailerService.SendPDFErrorMail(bookingReferenceNumber);
                    }
                    catch (Exception)
                    {
                        //ignored
                    }
                    voucherName = $"{bookingReferenceNumber}.html";
                    byteArray = _mailAttachmentService.GetBookedInvoice(bookingReferenceNumber, false, 4);
                }
                else
                {
                    //httpResponseMessage = httpRequestMessage.CreateResponse(HttpStatusCode.BadRequest);
                    //return httpResponseMessage;
                    return BadRequest();
                }
            }

            //adding bytes to memory stream
            //var dataStream = new MemoryStream(byteArray);
            //httpResponseMessage = httpRequestMessage.CreateResponse(HttpStatusCode.OK);
            //httpResponseMessage.Content = new StreamContent(dataStream);
            //httpResponseMessage.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment") { FileName = voucherName };
            //httpResponseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

            //return httpResponseMessage;

            //adding bytes to memory stream
            var dataStream = new MemoryStream(byteArray);

            return File(dataStream, "application/octet-stream", voucherName);
        }

        #region No longer used

        /*
        [Route("cancel/{bookingReferenceNumber}/{bookingOptionId}")]
        [HttpGet]
        [ValidateModel]
        public HttpResponseMessage GetCancelledVoucher(string bookingReferenceNumber, string bookingOptionId)
        {
            var httpRequestMessage = new HttpRequestMessage();
            HttpResponseMessage httpResponseMessage;

            var isPDFVoucher = ConfigurationManagerHelper.GetValuefromAppSettings("VoucherPDF") == "1";
            var voucherName = isPDFVoucher ? $"{bookingReferenceNumber}.pdf" : $"{bookingReferenceNumber}.html";

            //get byte array
            var byteArray = _mailAttachmentService.GetCancelledVoucher(bookingReferenceNumber, bookingOptionId, isPDFVoucher);

            if (byteArray == null)
            {
                if (isPDFVoucher)
                {
                    try
                    {
                        _mailerService.SendPDFErrorMail(bookingReferenceNumber);
                    }
                    catch (Exception)
                    {
                        //ignored
                    }
                    voucherName = $"{bookingReferenceNumber}.html";
                    byteArray = _mailAttachmentService.GetCancelledVoucher(bookingReferenceNumber, bookingOptionId, false);
                }
                else
                {
                    httpResponseMessage = httpRequestMessage.CreateResponse(HttpStatusCode.BadRequest);
                    return httpResponseMessage;
                }
            }

            //adding bytes to memory stream
            var dataStream = new MemoryStream(byteArray);
            httpResponseMessage = httpRequestMessage.CreateResponse(HttpStatusCode.OK);
            httpResponseMessage.Content = new StreamContent(dataStream);
            httpResponseMessage.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment") { FileName = voucherName };
            httpResponseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

            return httpResponseMessage;
        } 
        */
        #endregion

        [HttpGet]
        [Route("~/api/email/customer/{referenceNumber}")]
        public IActionResult CustomerEmail(string referenceNumber)
        {
            var emailReponse = new ReceiveResponse
            {
                Status = WebAPI.Constants.Constant.error,
                Message = string.Empty
            };
            try
            {
                var status = _mailerService.SendMailCustomer(referenceNumber);
                if (status == false)
                {
                    emailReponse.Message = WebAPI.Constants.Constant.MoulinRougeNotSpported;
                }
                else
                {
                    emailReponse.Status = WebAPI.Constants.Constant.success;
                }
            }
            catch (Exception ex)
            {
                emailReponse.Message = ex.Message;
            }
            return Ok(emailReponse);
        }
        [HttpGet]
        [Route("~/api/email/supplier/{referenceNumber}/{productids}")]
        public IActionResult SupplierEmail(string referenceNumber, string productids)
        {
            var emailReponse = new ReceiveResponse
            {
                Status = WebAPI.Constants.Constant.error,
                Message = string.Empty
            };
            try
            {
                _mailerService.SendSupplierMail(referenceNumber, productids);
                emailReponse.Status = WebAPI.Constants.Constant.success;
            }
            catch (Exception ex)
            {
                emailReponse.Message = ex.Message;
            }
            return Ok(emailReponse);
        }

        /// <summary>
        /// Hit MR API and try to download ticket byte array again if it failed during  booking process
        /// </summary>
        /// <param name="bookingReferenceNumber"></param>
        /// <param name="mrOrderId"></param>
        /// <param name="mrOrderTikcetGuid"></param>
        /// <returns></returns>
        [Route("moulinrougeticket")]
        [HttpGet]
        public HttpResponseMessage GetMRTicket(string bookingReferenceNumber, string mrOrderId, string mrOrderTikcetGuid)
        {
            var httpRequestMessage = new HttpRequestMessage();
            HttpResponseMessage httpResponseMessage;
            
            //get byte array
            var byteArray = _moulinRougeAdapter.GetOrderEticketAsync(mrOrderId, mrOrderTikcetGuid, mrOrderTikcetGuid).GetAwaiter().GetResult();
            var voucherName = $"{bookingReferenceNumber}-{mrOrderId}-Ticket.pdf";

            if (byteArray == null)
            {

                httpResponseMessage = httpRequestMessage.CreateResponse(HttpStatusCode.NotFound);
                return httpResponseMessage;
            }

            //adding bytes to memory stream
            var dataStream = new MemoryStream(byteArray);
            httpResponseMessage = httpRequestMessage.CreateResponse(HttpStatusCode.OK);
            httpResponseMessage.Content = new StreamContent(dataStream);
            httpResponseMessage.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment") { FileName = voucherName };
            httpResponseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

            return httpResponseMessage;
        }
        
    }
}