using Isango.Entities;
using ServiceAdapters.CitySightSeeing.CitySightSeeing.Commands.Contracts;
using ServiceAdapters.CitySightSeeing.CitySightSeeing.Converters.Contracts;
using ServiceAdapters.CitySightSeeing.CitySightSeeing.Entities;
using ServiceAdapters.CitySightSeeing.CitySightSeeing.Entities.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ServiceAdapters.CitySightSeeing
{
    public class CitySightSeeingAdapter : ICitySightSeeingAdapter, IAdapter
    {
        private readonly IProductsCommandHandler _productsCommandHandler;
        private readonly IProductsConverter _productConverter;
        private readonly ICreateBookingCommandHandler _createBookingCommandHandler;
        private readonly IBookingConverter _bookingConverter;
        private readonly IreservationCommandHandler _reservationcommandHandler;
        private readonly ICancellationCommandHandler _cancellationcommandHandler;
        private readonly IReservationConverter _reservationconverter;
        private readonly IRedemptionCmmdHandler _redemptionCmmdHandler;


        public CitySightSeeingAdapter(IProductsCommandHandler productsCommandHandler, IProductsConverter productConverter, ICreateBookingCommandHandler createBookingCommandHandler, IBookingConverter bookingConverter,
            IreservationCommandHandler reservationcommandHandler
            , ICancellationCommandHandler cancellationcommandHandler
            , IReservationConverter reservationconverter
            , IRedemptionCmmdHandler redemptionCmmdHandler)
        {
            _productsCommandHandler = productsCommandHandler;
            _productConverter = productConverter;
            _createBookingCommandHandler = createBookingCommandHandler;
            _bookingConverter = bookingConverter;
            _reservationcommandHandler = reservationcommandHandler;
            _cancellationcommandHandler = cancellationcommandHandler;
            _reservationconverter = reservationconverter;
            _redemptionCmmdHandler = redemptionCmmdHandler;
        }
        public List<ExternalProducts> GetProductExtrnalMappings(string token, string optionExternalId = null)
        {
            var res = _productsCommandHandler.Execute(optionExternalId, MethodType.GetProductsGetProductExtrnalMappings, token);
            if (res != null)
            {
                res = _productConverter.Convert(res, null);
                return res as List<ExternalProducts>;
            }
            return null;
        }

        public string CssBookingResult(string token, CreateBookingRequest bookingRequest)
        {
            var res = _createBookingCommandHandler.Execute(bookingRequest, MethodType.CreateBooking, token);
            //if (res != null)
            //{
            //  res = _bookingConverter.Convert(res, null);
            //    if (res is CssBookingResponseResult)
            //    {
            //        return res as CssBookingResponseResult;
            //    }
            //    else
            //    {
            //        return res as CssBookingStatus;
            //    }
            //}

            return res.ToString();
        }

        public ReservationResponse CssReservationResult(string token, ReservationRequest reservationRequest)
        {
            var res = _reservationcommandHandler.Execute(reservationRequest, MethodType.reserve, token);
            if (res != null)
            {
                res = _reservationconverter.Convert(res, null);
                return res as ReservationResponse;
            }

            return null;
        }
        public int CancellationResult(string token, CancellationRequest cancellationRequest)
        {
            var res = _cancellationcommandHandler.Execute(cancellationRequest, MethodType.CancelBooking, token);
            return (int)res;
        }

        public int RedemptionResult(string token, RedemptionRequest redemptionRequest)
        {
            var res = _redemptionCmmdHandler.Execute(redemptionRequest, MethodType.redemption, token);
            return (int)res;
        }


    }
}
