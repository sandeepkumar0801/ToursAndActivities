using Isango.Entities;
using Isango.Entities.CitySightseeing;
using ServiceAdapters.SightSeeing.SightSeeing.Commands.Contracts;
using ServiceAdapters.SightSeeing.SightSeeing.Converters.Contract;
using ServiceAdapters.SightSeeing.SightSeeing.Entities;
using ServiceAdapters.SightSeeing.SightSeeing.Entities.RequestResponseModels;
using Util;

namespace ServiceAdapters.SightSeeing
{
    public class SightSeeingAdapter : ISightSeeingAdapter, IAdapter
    {
        #region "Private Members"

        private readonly IIssuingCommandHandler _issuingCommandHandler;
        private readonly IConfirmCommandHandler _confirmCommandHandler;
        private readonly IIssuingConverter _issuingConverter;
        private readonly ICancelTicketCommandHandler _cancelTicketCommandHandler;
        private readonly ICancelTicketConverter _cancelTicketConverter;

        #endregion "Private Members"

        #region "Constructor"

        public SightSeeingAdapter(IIssuingCommandHandler issuingCommandHandler, IConfirmCommandHandler confirmCommandHandler, IIssuingConverter issuingConverter, ICancelTicketCommandHandler cancelTicketCommandHandler, ICancelTicketConverter cancelTicketConverter)
        {
            _issuingCommandHandler = issuingCommandHandler;
            _confirmCommandHandler = confirmCommandHandler;
            _issuingConverter = issuingConverter;
            _cancelTicketCommandHandler = cancelTicketCommandHandler;
            _cancelTicketConverter = cancelTicketConverter;
        }

        #endregion "Constructor"

        /// <summary>
        /// Issue Ticket for Sightseeing
        /// </summary>
        /// <returns>List of PNR"s</returns>
        public List<CitySightseeingSelectedProduct> IssueTicket(List<SelectedProduct> lstSelectedProducts, string bookingReferenceNumber,
            string token, out string requestXml, out string responseXml)
        {
            var inputContext = new InputContext
            {
                SelectedProducts = lstSelectedProducts,
                BookingReferenceNumber = bookingReferenceNumber
            };
            var res = _issuingCommandHandler.Execute(inputContext, MethodType.Issuing, token, out requestXml, out responseXml);

            if (res == null) return new List<CitySightseeingSelectedProduct>();
            var response = SerializeDeSerializeHelper.DeSerialize<IssueResponse>(res);
            if (!response.Success) return new List<CitySightseeingSelectedProduct>();
            var result = _issuingConverter.Convert(response);
            return result as List<CitySightseeingSelectedProduct>;
        }

        /// <summary>
        /// Check if requested PNR/Ticket is valid.
        /// </summary>
        /// <returns></returns>
        public bool ConfirmTicket(List<SelectedProduct> lstSelectedProducts, string token, out string requestXml, out string responseXml)
        {
            requestXml = string.Empty;
            responseXml = string.Empty;
            var inputContext = new InputContext
            {
                SelectedProducts = lstSelectedProducts
            };
            var res = _confirmCommandHandler.Execute(inputContext, MethodType.Confirm, token, out requestXml, out responseXml);

            if (res == null) return false;

            var response = SerializeDeSerializeHelper.DeSerialize<ConfirmResponse>(res);
            return response.Success;
        }

        /// <summary>
        /// Issue Ticket asynchronously for Sightseeing
        /// </summary>
        /// <param name="lstSelectedProducts"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<List<CitySightseeingSelectedProduct>> IssueTicketAsync(List<SelectedProduct> lstSelectedProducts, string token)
        {
            var inputContext = new InputContext
            {
                SelectedProducts = lstSelectedProducts
            };
            var res = await _issuingCommandHandler.ExecuteAsync(inputContext, MethodType.Issuing, token);

            if (res == null) return null;
            var response = SerializeDeSerializeHelper.DeSerialize<IssueResponse>(res);

            if (!response.Success) return null;

            var result = _issuingConverter.Convert(response);
            return result as List<CitySightseeingSelectedProduct>;
        }

        /// <summary>
        /// Confirm ticket for SightSeeing
        /// </summary>
        /// <param name="lstSelectedProducts"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<bool> ConfirmTicketAsync(List<SelectedProduct> lstSelectedProducts, string token)
        {
            var inputContext = new InputContext
            {
                SelectedProducts = lstSelectedProducts
            };
            var res = await _issuingCommandHandler.ExecuteAsync(inputContext, MethodType.Confirm, token);

            if (res == null) return false;
            var response = SerializeDeSerializeHelper.DeSerialize<ConfirmResponse>(res);
            return response.Success;
        }

        /// <summary>
        /// Cancel Ticket Async for SightSeeing
        /// </summary>
        /// <param name="lstSelectedProducts"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<Dictionary<string, bool>> CancelTicketAsync(List<SelectedProduct> selectedProducts, string token)
        {
            var inputContext = new InputContext
            {
                SelectedProducts = selectedProducts
            };

            var _returnValue = await _cancelTicketCommandHandler.ExecuteAsync(inputContext, MethodType.Cancel, token);

            var _responseValue = _cancelTicketConverter.Convert(_returnValue, inputContext.SelectedProducts);
            return _responseValue as Dictionary<string, bool>;
        }

        /// <summary>
        /// Cancel Ticket for SightSeeing
        /// </summary>
        /// <param name="selectedProducts"></param>
        /// <param name="token"></param>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        public Dictionary<string, bool> CancelTicket(List<SelectedProduct> selectedProducts, string token, out string request, out string response)
        {
            var inputContext = new InputContext
            {
                SelectedProducts = selectedProducts
            };

            var _returnValue = _cancelTicketCommandHandler.Execute(inputContext, MethodType.Cancel, token, out request, out response);

            var _responseValue = _cancelTicketConverter.Convert(_returnValue, inputContext.SelectedProducts);
            return _responseValue as Dictionary<string, bool>;
        }
    }
}