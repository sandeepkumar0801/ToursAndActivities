using Isango.Entities;
using Isango.Entities.Booking;
using Isango.Persistence.Contract;
using Isango.Service.Contract;
using Logger.Contract;
using ServiceAdapters.CitySightSeeing.CitySightSeeing.Commands.Contracts;
using ServiceAdapters.CitySightSeeing.CitySightSeeing.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TableStorageOperations.Contracts;
using ReservationResponse = ServiceAdapters.CitySightSeeing.CitySightSeeing.Entities.ReservationResponse;
using Util;
using Newtonsoft.Json;

namespace Isango.Service
{
    public class CssBookingService : ICssBookingService
    {
        private readonly ILogger _log;
        private readonly ITableStorageOperation _tableStorageOperation;
        private readonly ICitySightSeeingAdapter _citysightseeingadapter;
        private readonly IMasterPersistence _masterPersistence;
        public static string isCssBookingRequired = ConfigurationManagerHelper.GetValuefromAppSettings("IsCssBookingRequired");




        public CssBookingService(
            ILogger log
            , ITableStorageOperation tableStorageOperation
            , ICitySightSeeingAdapter citySightSeeingAdapter
            , IMasterPersistence masterPersistence)
        {
            _log = log;
            _tableStorageOperation = tableStorageOperation;
            _citysightseeingadapter = citySightSeeingAdapter;
            _masterPersistence = masterPersistence;
        }
        public void ProcessCssIncompleteBooking()
        {
            try
            {
                if (isCssBookingRequired == "1")
                {
                    var bookingdata = _masterPersistence.GetBookingData();
                    if (bookingdata != null && bookingdata.cssBookingDatas.Count != 0)
                    {
                        var parallelOptions = new ParallelOptions
                        {
                            MaxDegreeOfParallelism = Environment.ProcessorCount // Set the degree of parallelism (number of parallel threads)
                        };
                        Parallel.ForEach(bookingdata.cssBookingDatas, parallelOptions, cssBooking =>
                        {
                            //foreach (var cssBooking in bookingdata.cssBookingDatas)
                            //{
                            var cssBookingRequired = _masterPersistence.IsBookingDoneWithCss(cssBooking.CssProductOptionId, cssBooking.bookingreferencenumber);
                            var IsCssBookingDone = cssBookingRequired == null || cssBookingRequired.Count == 0;

                            if (IsCssBookingDone)
                            {
                                Guid idempotentGuid = Guid.NewGuid();
                                string idempotentKey = idempotentGuid.ToString();
                                var passengerdetails = bookingdata.cssPassengerDetails.Where(x => x.BOOKEDOPTIONID == cssBooking.BOOKEDOPTIONID).ToList();
                                var qrCodes = bookingdata.cssQrCodes.Where(x => x.BOOKEDOPTIONID == cssBooking.BOOKEDOPTIONID).ToList();
                                var cssBookingData = CreateBookingData(cssBooking, passengerdetails, qrCodes);
                                var reservation = CreateReservation(cssBookingData);

                                cssBookingData.reservation = reservation?.reservation ?? null;
                                var createbookingrequest = ConvertCreateBooking(cssBooking.bookingreferencenumber.Trim(), cssBookingData);

                                var booking = _citysightseeingadapter.CssBookingResult(idempotentKey, createbookingrequest);

                                var selectedProduct = createbookingrequest as CreateBookingRequest;
                                var createbookingjson = SerializeDeSerializeHelper.Serialize(selectedProduct);
                                //var bookingreponsejson = SerializeDeSerializeHelper.Serialize(booking as CssBookingResponseResult);

                                var bookingresult = SerializeDeSerializeHelper.DeSerialize<CssBookingResponseResult>(booking);
                                var bookingstatusresult = SerializeDeSerializeHelper.DeSerialize<CssBookingStatus>(booking);
                                if (bookingresult.Tickets != null || bookingstatusresult.Description == "The booking already exists")
                                {
                                    _masterPersistence.SaveAllCssBooking(idempotentKey, "Booking", cssBooking, "SUCCESS", cssBooking.OTAReferenceId, cssBooking.bookingreferencenumber, false, createbookingjson, booking, createbookingrequest.barcode);
                                }
                                else
                                {
                                    _masterPersistence.SaveAllCssBooking(idempotentKey, "Booking", cssBooking, "Failed", cssBooking.OTAReferenceId, cssBooking.bookingreferencenumber, false, createbookingjson, booking, createbookingrequest.barcode);
                                }
                            }
                            //}
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CssBookingService",
                    MethodName = "ProcessIncompleteBooking",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }
        public void ProcessIncompleteRedemption()
        {
            try
            {
                if (isCssBookingRequired == "1")
                {
                    var redemptiondatas = _masterPersistence.GetRedemptionData();
                    if (redemptiondatas != null && redemptiondatas.Count != 0)
                    {
                        //foreach (var redempdata in redemptiondatas)
                        var parallelOptions = new ParallelOptions
                        {
                            MaxDegreeOfParallelism = Environment.ProcessorCount // Set the degree of parallelism (number of parallel threads)
                        };

                        Parallel.ForEach(redemptiondatas, parallelOptions, redempdata =>
                        {
                            Guid idempotentGuid = Guid.NewGuid(); // Generate a new UUID
                            string idempotentKey = idempotentGuid.ToString();
                            var redemptionrequest = GetRedemptionRequest(redempdata.referenceId, redempdata.otaReferenceId);
                            var res = _citysightseeingadapter.RedemptionResult(idempotentKey, redemptionrequest);
                            var cancelbooking = JsonConvert.SerializeObject(redemptionrequest);

                            if (res == 200)
                            {
                                _masterPersistence.SaveAllCssCancellation(redempdata.referenceId, idempotentKey, "Redemption", 0, redempdata.otaReferenceId, false, cancelbooking, res.ToString());
                            }
                            else
                            {
                                _masterPersistence.SaveAllCssCancellation(redempdata.referenceId, idempotentKey, "Redemption", 0, redempdata.otaReferenceId, false, cancelbooking, res.ToString());
                            }
                        });
                        // }

                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CssBookingService",
                    MethodName = "ProcessIncompleteRedemption",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }
        public void ProcessIncompleteCancellation()
        {
            try
            {
                if (isCssBookingRequired == "1")
                {
                    var canceldatas = _masterPersistence.GetCssCancellation();
                    if (canceldatas != null && canceldatas.Count != 0)
                    {
                        Parallel.ForEach(canceldatas, canceldata =>
                        {
                            Guid idempotentGuid = Guid.NewGuid(); // Generate a new UUID
                            string idempotentKey = idempotentGuid.ToString();
                            var cancelrequest = GetCancellationRequest(canceldata.CssReferenceNumber, canceldata.SupplierId, canceldata.Barcode);
                            var res = _citysightseeingadapter.CancellationResult(idempotentKey, cancelrequest);
                            var cancelbooking = SerializeDeSerializeHelper.SerializeWithContractResolver(cancelrequest as CancellationRequest);

                            if (res == 200)
                            {
                                _masterPersistence.SaveAllCssCancellation(canceldata.CssReferenceNumber, idempotentKey, "Cancellation", canceldata.isangoserviceoptionid, null, true, cancelbooking, res.ToString());
                            }
                            else
                            {
                                _masterPersistence.SaveAllCssCancellation(canceldata.CssReferenceNumber, idempotentKey, "Cancellation", canceldata.isangoserviceoptionid, null, false, cancelbooking, res.ToString());
                            }

                        });


                    }
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CssBookingService",
                    MethodName = "ProcessIncompleteCancellation",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }
        public RedemptionRequest GetRedemptionRequest(string ReferenceId, string OtaReferenceId)
        {

            var redemptionRequest = new RedemptionRequest
            {
                referenceId = ReferenceId.Trim(),
                otaReferenceId = OtaReferenceId,
                datetime = null
            };

            return redemptionRequest;
        }

        public CancellationRequest GetCancellationRequest(string CssReferenceNumber, int supplierId, string barcode = null)
        {
            // Create a new CancellationRequest object with the desired values
            CancellationRequest cancellationRequest = new CancellationRequest
            {
                agent = "isango.com",
                barcode = barcode,
                booking = CssReferenceNumber.Trim(),
                supplier_id = supplierId
            };

            // Return the created CancellationRequest object
            return cancellationRequest;
        }

        public ReservationResponse CreateReservation(CssBookingData cssbookingdata)
        {
            try
            {
                Guid idempotentGuid = Guid.NewGuid(); // Generate a new UUID
                string idempotentKey = idempotentGuid.ToString();
                var reservationrequest = new ReservationRequest
                {
                    adult = cssbookingdata?.adult ?? 0,
                    agent = "isango.com",
                    date = cssbookingdata.date,
                    option = cssbookingdata.CssProductOptionId,
                    product = cssbookingdata.CssProductId,
                    supplier_id = cssbookingdata.SupplierId,
                    time = cssbookingdata.timeslot.Substring(0, 5) // Extract the first 5 characters (hh:mm) from timeslot
                };
                var reservation = _citysightseeingadapter.CssReservationResult(idempotentKey, reservationrequest);


                return reservation;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CssBookingService",
                    MethodName = "CreateReservation",
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }
        public CreateBookingRequest ConvertCreateBooking(string Bookingreferenceid, CssBookingData cssbookingdata)
        {
            var passengerdetail = cssbookingdata.customer;
            var time = cssbookingdata.timeslot;
            var createBookingRequest = new CreateBookingRequest
            {
                adult = cssbookingdata?.adult ?? 0,
                agent = "isango.com",
                barcode = cssbookingdata.barcode,
                booking = cssbookingdata.booking,
                child = cssbookingdata?.child ?? 0,
                customer = new BookingCustomer
                {
                    country = null,
                    email = passengerdetail.email,
                    full_name = passengerdetail.full_name,
                    language = passengerdetail.language,
                    lastName = passengerdetail.lastName,
                    mobile = null,
                    name = passengerdetail.name
                },
                date = cssbookingdata.date,
                utcConfirmedAt = cssbookingdata.utcConfirmedAt,
                family = 0,
                infant = cssbookingdata?.infant ?? 0,
                integration_booking_code = Bookingreferenceid,
                notes = null,
                option = cssbookingdata.CssProductOptionId,
                product = cssbookingdata.CssProductId,
                reference = cssbookingdata.OTAReferenceId,
                reservation = null,
                resident = 0,
                senior = cssbookingdata?.senior ?? 0,
                student = cssbookingdata?.student ?? 0,
                supplier_id = cssbookingdata.SupplierId,
                tickets = cssbookingdata.tickets,
                time = cssbookingdata.timeslot.Substring(0, 5),
                youth = cssbookingdata?.youth ?? 0
            };
            return createBookingRequest;
        }
        public CssBookingData CreateBookingData(CssBookingDatas cssBookingDatas, List<CssPassengerDetails> passengerDetail, List<CssQrCode> cssQrCodes = null)
        {
            var cssBookingData = new CssBookingData();
            cssBookingData.customer = new CssCustomer();
            cssBookingData.tickets = new List<Ticket>();

            var passengerTypeMapping = new Dictionary<int, Action<int>>
               {
                   { 1, count => cssBookingData.adult = count },
                   { 2, count => cssBookingData.child = count },
                   { 8, count => cssBookingData.youth = count },
                   { 11, count => cssBookingData.student = count },
                   { 10, count => cssBookingData.senior = count },
                    { 9, count => cssBookingData.infant = count }
                   // Add other mappings here if needed
               };
            foreach (var passenger in passengerDetail)
            {
                if (passengerTypeMapping.TryGetValue(passenger.PASSENGERTYPEID, out var updateProperty))
                {
                    updateProperty(passenger.PaxCount);
                }

            }
            if (!passengerDetail.FirstOrDefault().IsPerPaxQRCode)
            {
                cssBookingData.barcode = cssQrCodes.FirstOrDefault().QRCODE.Replace("QR_CODE~", "");
            }
            else
            {
                foreach (var cssQrCode in cssQrCodes)
                {
                    Ticket ticket = new Ticket
                    {
                        barcode = cssQrCode.QRCODE.Replace("QR_CODE~", ""),
                        reference = cssBookingDatas.OTAReferenceId, // Set the reference value as needed
                        type = MapPassengerType(cssQrCode.PASSENGERTYPEID)
                    };
                    cssBookingData.tickets.Add(ticket);
                }
            }
            cssBookingData.booking = cssQrCodes.Count() > 1 ? cssBookingDatas.bookingreferencenumber.Trim() : null;
            cssBookingData.customer.full_name = cssBookingDatas.LeadPassengerName;
            cssBookingData.date = cssBookingDatas.Traveldate.ToString("yyyy-MM-dd");
            //string firstName = cssBookingDatas.LeadPassengerName?.Split(' ')[0];
            cssBookingData.customer.name = cssBookingDatas.PASSENGERFIRSTNAME;
            cssBookingData.customer.lastName = cssBookingDatas.PASSENGERLASTNAME;
            cssBookingData.CssProductId = cssBookingDatas.CssProductId;
            cssBookingData.CssProductOptionId = cssBookingDatas.CssProductOptionId;
            cssBookingData.SupplierId = cssBookingDatas.SupplierId;
            cssBookingData.timeslot = cssBookingDatas.optiontimeslot;
            cssBookingData.customer.email = cssBookingDatas.voucheremail;
            cssBookingData.OTAReferenceId = cssBookingDatas.OTAReferenceId;
            cssBookingData.utcConfirmedAt = cssBookingDatas.utcConfirmedAt;
            return cssBookingData; // Don't forget to return the converted object
        }

        public string MapPassengerType(int PASSENGERTYPEID)
        {
            string type;
            switch (PASSENGERTYPEID)
            {
                case 1:
                    type = "Adult";
                    break;
                case 2:
                    type = "Child";
                    break;
                case 8:
                    type = "Youth";
                    break;
                case 11:
                    type = "Student";
                    break;
                case 10:
                    type = "Senior";
                    break;
                case 9:
                    type = "Infant";
                    break;
                default:
                    type = "Unknown";
                    break;
            }

            return type;
        }





    }

}


