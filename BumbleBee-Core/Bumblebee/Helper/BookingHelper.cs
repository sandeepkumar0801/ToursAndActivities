using Isango.Entities;
using Isango.Entities.Booking;
using Isango.Entities.Booking.RequestModels;
using Isango.Service.Contract;
using Logger.Contract;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using Util;
using WebAPI.Mapper;
using ILogger = Logger.Contract.ILogger;

namespace WebAPI.Helper
{
    public class BookingHelper
    {
        private readonly BookingMapper _bookingMapper;
        private readonly IBookingService _bookingService;
        private readonly ILogger _log;

        public BookingHelper()
        {
        }

        public BookingHelper(BookingMapper bookingMapper, IBookingService bookingService,
            ILogger log = null)
        {
            _bookingMapper = bookingMapper;
            _bookingService = bookingService;
            _log = log;
        }

        public Tuple<BookingResult, Booking> CreateBooking(CreateBookingRequest request)
        {
            var bookingResult = new BookingResult
            {
                BookingStatus = Isango.Entities.Enums.BookingStatus.Failed
            };
            var booking = default(Booking);
            try
            {
                booking = _bookingMapper.PrepareBookingModel(request, request?.BookingReferenceNumber);

                if (booking == null)
                {
                    var message = WebAPI.Constants.Constant.NoBookingeData;
                    var isangoErrorEntity = new IsangoErrorEntity
                    {
                        ClassName = "BookingMapper",
                        MethodName = "PrepareBookingModel",
                        Params = $"{SerializeDeSerializeHelper.Serialize(request)}"
                    };
                    _log.Error(isangoErrorEntity, new Exception(message));
                    var data = new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        ReasonPhrase = message
                    };
                    bookingResult.UpdateErrors(CommonErrorCodes.BookingError, HttpStatusCode.NotFound, message);
                    return new Tuple<BookingResult, Booking>(bookingResult, booking);
                }

                if (!string.IsNullOrWhiteSpace(booking?.ErrorMessage))
                {
                    bookingResult.UpdateErrors(CommonErrorCodes.BookingError, HttpStatusCode.InternalServerError, booking?.ErrorMessage);
                    return new Tuple<BookingResult, Booking>(bookingResult, booking);
                }

                bookingResult = _bookingService.Book(booking, request.TokenId);

                if (booking?.Errors?.Any() == true)
                {
                    bookingResult.Errors = booking.Errors;
                }
            }
            catch (Exception ex)
            {
                bookingResult.UpdateErrors(CommonErrorCodes.BookingError, HttpStatusCode.InternalServerError, ex.Message);
            }
            return new Tuple<BookingResult, Booking>(bookingResult, booking);
        }

        /// <summary>
        /// Create Booking Reservation
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Tuple<ReservationResponse, Booking> CreateBookingReservation(CreateBookingRequest request)
        {
            var reservationResponse = new ReservationResponse
            {
                Success = false,
                Products = new System.Collections.Generic.List<BookedProductStatus>()
            };
            var booking = default(Booking);
            try
            {
                booking = _bookingMapper.PrepareBookingModel(request);

                if (booking == null)
                {
                    var message = WebAPI.Constants.Constant.NoBookingeData;
                    var isangoErrorEntity = new IsangoErrorEntity
                    {
                        ClassName = "BookingMapper",
                        MethodName = "PrepareBookingModel",
                        Params = $"{SerializeDeSerializeHelper.Serialize(request)}"
                    };
                    _log.Error(isangoErrorEntity, new Exception(message));
                    var data = new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        ReasonPhrase = message
                    };
                    reservationResponse.UpdateErrors(CommonErrorCodes.BookingError, HttpStatusCode.NotFound, message);
                    return new Tuple<ReservationResponse, Booking>(reservationResponse, booking);
                }

                if (!string.IsNullOrWhiteSpace(booking?.ErrorMessage))
                {
                    reservationResponse.UpdateErrors(CommonErrorCodes.BookingError, HttpStatusCode.InternalServerError, booking?.ErrorMessage);
                    return new Tuple<ReservationResponse, Booking>(reservationResponse, booking);
                }

                reservationResponse = _bookingService.Reserve(booking, request.TokenId);

                if (booking?.Errors?.Any() == true)
                {
                    reservationResponse.Errors = booking.Errors;
                }
            }
            catch (Exception ex)
            {
                reservationResponse.UpdateErrors(CommonErrorCodes.BookingError, HttpStatusCode.InternalServerError, ex.Message);
            }
            return new Tuple<ReservationResponse, Booking>(reservationResponse, booking);
        }

        public Tuple<ReservationResponse, Booking> CancelBookingReservation(CancelReservationRequest request)
        {
            var reservationResponse = new ReservationResponse
            {
                Success = false,
                Products = new System.Collections.Generic.List<BookedProductStatus>()
            };
            var booking = default(Booking);
            try
            {
                var reservationDetails = _bookingService.GetResrvationDetailsFromDB(request.BookingReferenceNumber);
                var token = reservationDetails?.FirstOrDefault()?.Token;
                booking = _bookingMapper.PrepareCancelReservationModel(reservationDetails, request.BookingReferenceNumber);

                if (booking == null)
                {
                    var message = WebAPI.Constants.Constant.NoBookingeData;
                    var isangoErrorEntity = new IsangoErrorEntity
                    {
                        ClassName = "BookingMapper",
                        MethodName = "PrepareCancelReservationModel",
                        Params = $"{SerializeDeSerializeHelper.Serialize(request)}"
                    };
                    _log.Error(isangoErrorEntity, new Exception(message));
                    var data = new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        ReasonPhrase = message
                    };
                    reservationResponse.UpdateErrors(CommonErrorCodes.BookingError, HttpStatusCode.NotFound, message);
                    return new Tuple<ReservationResponse, Booking>(reservationResponse, booking);
                }

                if (!string.IsNullOrWhiteSpace(booking?.ErrorMessage))
                {
                    reservationResponse.UpdateErrors(CommonErrorCodes.BookingError, HttpStatusCode.InternalServerError, booking?.ErrorMessage);
                    return new Tuple<ReservationResponse, Booking>(reservationResponse, booking);
                }

                reservationResponse = _bookingService.CancelReservation(booking, token);

                if (booking?.Errors?.Any() == true)
                {
                    reservationResponse.Errors = booking.Errors;
                }
            }
            catch (Exception ex)
            {
                reservationResponse.UpdateErrors(CommonErrorCodes.BookingError, HttpStatusCode.InternalServerError, ex.Message);
            }
            return new Tuple<ReservationResponse, Booking>(reservationResponse, booking);
        }

        /// <summary>
        /// Create Receive Booking
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public BookingResult CreateReceiveBooking(CreateReceiveBookingRequest request)
        {
            var booking = _bookingMapper.PrepareReceiveBookingModel(request);
            if (booking == null) return null;
            var bookingResult = _bookingService.BookReceive(booking, request.TokenId);
            return bookingResult;
        }
    }
}