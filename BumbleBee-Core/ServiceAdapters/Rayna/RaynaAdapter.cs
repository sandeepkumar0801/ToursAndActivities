using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Rayna;
using Logger.Contract;
using ServiceAdapters.Rayna.Rayna.Commands.Contract;
using ServiceAdapters.Rayna.Rayna.Converters.Contracts;
using ServiceAdapters.Rayna.Rayna.Entities;
using Util;

namespace ServiceAdapters.Rayna
{
    public class RaynaAdapter : IRaynaAdapter, IAdapter
    {
        private readonly InputContext _inputContext = new InputContext();
        private readonly ILogger _log;
        private readonly IRaynaCountryCmdHandler _raynaCountryCmdHandler;
        private readonly IRaynaCityCmdHandler _raynaCityCmdHandler;

        private readonly IRaynaTourStaticDataCmdHandler _raynaTourStaticDataCmdHandler;
        private readonly IRaynaTourStaticDataByIdCmdHandler _raynaTourStaticDataByIdCmdHandler;
        private readonly IRaynaTourOptionCmdHandler _raynaTourOptionCmdHandler;

        private readonly IRaynaAvailabilityTourOptionCmdHandler _raynaAvailabilityTourOptionCmdHandler;
        private readonly IRaynaAvailabilityTimeSlotCmdHandler _raynaAvailabilityTimeSlotCmdHandler;

        private readonly IRaynaAvailabilityCmdHandler _raynaAvailabilityCmdHandler;
        private readonly IRaynaAvailabilityConverter _raynaAvailabilityConverter;

        private readonly IRaynaBookingCmdHandler _raynaBookingCmdHandler;
        private readonly IRaynaBookingConverter _raynaBookingConverter;

        private readonly IRaynaTourTicketCmdHandler _raynaTourTicketCmdHandler;

        private readonly IRaynaCancelCmdHandler _raynaCancelCmdHandler;
        private readonly IRaynaCancelConverter _raynaCancelConverter;

        public static string _raynaURL;
        //Command Handlers
        public RaynaAdapter(
            IRaynaCountryCmdHandler raynaCountryCmdHandler,
            IRaynaCityCmdHandler raynaCityCmdHandler,
            IRaynaTourStaticDataCmdHandler raynaTourStaticDataCmdHandler,
            IRaynaTourStaticDataByIdCmdHandler raynaTourStaticDataByIdCmdHandler,
            IRaynaTourOptionCmdHandler raynaTourOptionCmdHandler,
            IRaynaAvailabilityTourOptionCmdHandler raynaAvailabilityTourOptionCmdHandler,
            IRaynaAvailabilityTimeSlotCmdHandler raynaAvailabilityTimeSlotCmdHandler,
            IRaynaAvailabilityCmdHandler raynaAvailabilityCmdHandler,
            IRaynaAvailabilityConverter raynaAvailabilityConverter,

            IRaynaBookingCmdHandler raynaBookingCmdHandler,
            IRaynaBookingConverter raynaBookingConverter,

            IRaynaTourTicketCmdHandler raynaTourTicketCmdHandler,

            IRaynaCancelCmdHandler raynaCancelCmdHandler,
            IRaynaCancelConverter raynaCancelConverter,

            ILogger log
            )
        {
            _raynaCountryCmdHandler = raynaCountryCmdHandler;
            _raynaCityCmdHandler = raynaCityCmdHandler;
            _raynaTourStaticDataCmdHandler = raynaTourStaticDataCmdHandler;
            _raynaTourStaticDataByIdCmdHandler = raynaTourStaticDataByIdCmdHandler;
            _raynaTourOptionCmdHandler = raynaTourOptionCmdHandler;
            _raynaAvailabilityTourOptionCmdHandler = raynaAvailabilityTourOptionCmdHandler;
            _raynaAvailabilityTimeSlotCmdHandler = raynaAvailabilityTimeSlotCmdHandler;
            _raynaAvailabilityCmdHandler = raynaAvailabilityCmdHandler;
            _raynaAvailabilityConverter = raynaAvailabilityConverter;

            _raynaBookingCmdHandler = raynaBookingCmdHandler;
            _raynaBookingConverter = raynaBookingConverter;
            _raynaTourTicketCmdHandler = raynaTourTicketCmdHandler;
            _raynaCancelCmdHandler = raynaCancelCmdHandler;
            _raynaCancelConverter = raynaCancelConverter;
            _log = log;
        }
        static RaynaAdapter()
        {
            _raynaURL = ConfigurationManagerHelper.GetValuefromAppSettings(Constants.Constants.RaynaURL);
        }

        #region [Data save for products]

        public Countries CountryData(string token, out string request, out string response)
        {
            request = string.Empty;
            response = string.Empty;
            var country = default(Countries);
            var inputURL = _raynaURL + Constants.Constants.Countries;
            var returnValue = _raynaCountryCmdHandler.Execute(inputURL, token, MethodType.Country, out request, out response);
            if (returnValue != null)
            {
                country = returnValue as Countries;
            }
            return country;
        }

        public CityByCountry CityData(int? countryId, string token, out string request, out string response)
        {
            request = string.Empty;
            response = string.Empty;
            var city = default(CityByCountry);
            var criteria = new CityByCountryRQ
            {
                CountryId = countryId
            };
            var returnValue = _raynaCityCmdHandler.Execute(criteria, token, MethodType.City, out request, out response);
            if (returnValue != null)
            {
                city = returnValue as CityByCountry;
            }
            return city;
        }
        public TourStaticData TourStaticData(int countryId, int cityId, string token, out string request, out string response)

        {
            request = string.Empty;
            response = string.Empty;
            var tourStaticData = default(TourStaticData);

            var criteria = new TourStaticDataRQ
            {
                CountryId = countryId,
                CityId = cityId
            };
            var returnValue = _raynaTourStaticDataCmdHandler.Execute(criteria, token, MethodType.TourStaticData, out request, out response);
            if (returnValue != null)
            {
                tourStaticData = returnValue as TourStaticData;
            }
            return tourStaticData;
        }
        public TourStaticDataById TourStaticDataById(
            int countryId, int cityId, int tourId, int contractId, string travelDate,
            string token, out string request, out string response)
        {
            request = string.Empty;
            response = string.Empty;
            var tourStaticDataById = default(TourStaticDataById);

            var criteria = new TourStaticDataByIdRQ
            {
                CountryId = countryId,
                CityId = cityId,
                ContractId = contractId,
                TourId = tourId,
                TravelDate = travelDate
            };
            var returnValue = _raynaTourStaticDataByIdCmdHandler.Execute(criteria, token, MethodType.TourStaticDataById, out request, out response);
            if (returnValue != null)
            {
                tourStaticDataById = returnValue as TourStaticDataById;
            }
            return tourStaticDataById;
        }

        public TourOptions TourOptions(
            int tourId, int contractId,
           string token, out string request, out string response)
        {
            request = string.Empty;
            response = string.Empty;
            var tourStaticDataById = default(TourOptions);

            var criteria = new TourOptionsRQ
            {
                ContractId = contractId,
                TourId = tourId
            };
            var returnValue = _raynaTourOptionCmdHandler.Execute(criteria, token, MethodType.TourOptionStaticData, out request, out response);
            if (returnValue != null)
            {
                tourStaticDataById = returnValue as TourOptions;
            }
            return tourStaticDataById;
        }
        #endregion

        #region [Availability]

        //Calendar API data with converter
        public Activity GetActivity(
            RaynaCriteria raynaCriteria,
            string token,
            out string request, out string response)
        {
            request = string.Empty;
            response = string.Empty;

            if (raynaCriteria == null)
                return null;

            //null means no database mapping
            if (raynaCriteria.SupplierOptionIds == null || raynaCriteria.SupplierOptionIds.Count == 0)
            {
                return null;
            }

            var resultIsangoActivity = default(Isango.Entities.Activities.Activity);

            var finalReturnAPIData = new AvailabilityReturnData();
            finalReturnAPIData.AvailabilityTourOptionRS = new List<Dictionary<DateTime, AvailabilityTourOptionRS>>();
            finalReturnAPIData.AvailabilityTimeSlotRS = new List<Tuple<DateTime, int, int, int, AvailabilityTimeSlotRS>>();
            finalReturnAPIData.AvailabilityRES = new List<Tuple<DateTime, int, int, int, AvailabilityRES>>();

            var startDate = raynaCriteria.CheckinDate;
            var endDate = raynaCriteria.CheckoutDate;
            var dicAvailabilityTourOptionRS = new Dictionary<DateTime, AvailabilityTourOptionRS>();
            //loop of dates
            var source = Enumerable.Range(0, (endDate - startDate).Days+1).Select(i => startDate.AddDays(i));
            var loadOptionsTask = new Task<Tuple<DateTime,object>>[source.Count()];
            var dicAvailabilityTourOptionRSTask = new Dictionary<DateTime, object>();
            var count = 0;
            var noOfPassengersTask = raynaCriteria.NoOfPassengers;
            var tourIdTask = raynaCriteria.TourId;
            var modalityCodeTask = raynaCriteria.ModalityCode;
            //Step1: Tour Options
            #region [Step1: Get Tour Options]
            for (DateTime dateData = startDate; dateData.Date <= endDate.Date; dateData = dateData.AddDays(1))
            {
                try
                {
                    //make object independent..not shared object
                    var raynaCriteriaTask = new RaynaCriteria
                    {
                        NoOfPassengers = noOfPassengersTask,
                        TourId = tourIdTask,
                        ModalityCode = modalityCodeTask,
                        PassDate = dateData
                    };
                    //EndPoint Call 1: AvailabilityTourOptions
                    loadOptionsTask[count] = Task.Run(() => Tuple.Create(raynaCriteriaTask.PassDate, _raynaAvailabilityTourOptionCmdHandler.Execute(raynaCriteriaTask, token, MethodType.AvailabilityTourOption)));
                    count++;
                }
                catch (Exception ex)
                {
                    var isangoErrorEntity = new IsangoErrorEntity
                    {
                        ClassName = "RaynaAdapter",
                        MethodName = "GetActivity"
                    };
                    _log.Error(isangoErrorEntity, ex);
                }
            }
            if (loadOptionsTask?.Length > 0)
            {
                loadOptionsTask = loadOptionsTask?.Where(t => t != null).ToArray();
                if (loadOptionsTask.Length > 0)
                {
                    Task.WaitAll(loadOptionsTask);
                    foreach (var task in loadOptionsTask)
                    {
                        try
                        {

                            var data = task?.GetAwaiter().GetResult();
                            if (data != null)
                            {
                                if (!dicAvailabilityTourOptionRS.Keys.Contains(data.Item1))
                                {
                                    dicAvailabilityTourOptionRS.Add(data.Item1, data.Item2 as AvailabilityTourOptionRS);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            var isangoErrorEntity = new IsangoErrorEntity
                            {
                                ClassName = "RaynaAdapter",
                                MethodName = "GetActivity"
                            };
                            _log.Error(isangoErrorEntity, ex);
                        }
                    }
                }
            }
            #endregion

            #region [Step2: Get Availability and Slot]

            var noOfPassengersAvailabiityTask = raynaCriteria.NoOfPassengers;
            var modalityCodeAvailabiityTask = raynaCriteria.ModalityCode;

            //data add only if match with database option id
            foreach (var data in dicAvailabilityTourOptionRS?.Values)
            {
                var responseData = data?.AvailabilityOptionResult;
                responseData?.RemoveAll(x => !raynaCriteria.SupplierOptionIds.Contains(Convert.ToString(x.TourOptionId)));
            }

            dicAvailabilityTourOptionRS = dicAvailabilityTourOptionRS.Where(x => x.Value != null)?.ToDictionary(i => i.Key, i => i.Value);

           


            var parentItemCount = dicAvailabilityTourOptionRS.Count;
            var childItemsCount= dicAvailabilityTourOptionRS.Sum(x=>x.Value.AvailabilityOptionResult.Count);
            var totalCount = (parentItemCount * childItemsCount);
            var loadOptionsSecondTask = new Task<Tuple<DateTime,int,int,int, object,object>>[totalCount];
            var tupAvailabilityRS = new List<Tuple<DateTime, int, int, int, AvailabilityRES>>();
            var tupAvailabilityTimeSlotRS = new List<Tuple<DateTime, int, int, int, AvailabilityTimeSlotRS>>();
            if (dicAvailabilityTourOptionRS != null && dicAvailabilityTourOptionRS.Count > 0)
            {
                //Step2
                var countAvailability = 0;
                foreach (var singleAvailabilityTourOption in dicAvailabilityTourOptionRS)
                {
                    try
                    {
                        if (singleAvailabilityTourOption.Value.AvailabilityOptionResult != null && singleAvailabilityTourOption.Value.AvailabilityOptionResult.Count > 0)
                        {
                            foreach (var optionItem in singleAvailabilityTourOption.Value.AvailabilityOptionResult)
                            {
                                try
                                {
                                    var passData = singleAvailabilityTourOption.Key;
                                    var tourId = optionItem.TourId;
                                    var tourOptionId = optionItem.TourOptionId;
                                    var transferId = optionItem.TransferId;

                                    var raynaCriteriaOptionTask = new RaynaCriteria
                                    {
                                        NoOfPassengers = noOfPassengersAvailabiityTask,
                                        ModalityCode = modalityCodeAvailabiityTask,
                                        PassDate = passData,
                                        TourOptionId = tourOptionId,
                                        TransferId = transferId,
                                        TourId = tourId
                                    };

                                    //in case of calendar dumping -timeslot should not hit.
                                    //because it is dynamic pricing , only hit in case of check availabiity
                                    loadOptionsSecondTask[countAvailability] = Task.Run(() => Tuple.Create
                                          (
                                          raynaCriteriaOptionTask.PassDate,
                                          raynaCriteriaOptionTask.TourId,
                                          raynaCriteriaOptionTask.TourOptionId,
                                          raynaCriteriaOptionTask.TransferId,
                                          _raynaAvailabilityCmdHandler.Execute(raynaCriteriaOptionTask, token, MethodType.Availability)
                                          , (optionItem.IsSlot == true && raynaCriteria.IsCalendarDumping == false) ?
                                          _raynaAvailabilityTimeSlotCmdHandler.Execute(raynaCriteriaOptionTask, token, MethodType.AvailabilityTimeSlot)
                                          : null)
                                        );
                                    countAvailability++;
                                }
                                catch (Exception ex)
                                {
                                    var isangoErrorEntity = new IsangoErrorEntity
                                    {
                                        ClassName = "RaynaAdapter",
                                        MethodName = "GetActivity"
                                    };
                                    _log.Error(isangoErrorEntity, ex);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        var isangoErrorEntity = new IsangoErrorEntity
                        {
                            ClassName = "RaynaAdapter",
                            MethodName = "GetActivity"
                        };
                        _log.Error(isangoErrorEntity, ex);
                    }
                }
                if (loadOptionsSecondTask?.Length > 0)
                {
                    loadOptionsSecondTask = loadOptionsSecondTask?.Where(t => t != null).ToArray();
                    if (loadOptionsSecondTask.Length > 0)
                    {
                        Task.WaitAll(loadOptionsSecondTask);
                        foreach (var task in loadOptionsSecondTask)
                        {
                            try
                            {
                                var data = task?.GetAwaiter().GetResult();
                                if (data != null)
                                {
                                    var singleAvailability = data.Item5 as AvailabilityRES;
                                    var singleAvailabilityTimeSlot = data.Item6 != null ? data.Item6 as AvailabilityTimeSlotRS : null;

                                    if (singleAvailability != null && singleAvailability.ResultAvailability != null)
                                    {
                                        var tupleAvailability = Tuple.Create(data.Item1.Date, data.Item2, data.Item3, data.Item4, singleAvailability);
                                        tupAvailabilityRS.Add(tupleAvailability);
                                    }
                                    if (singleAvailabilityTimeSlot != null && singleAvailabilityTimeSlot.ResultAvailabilityTimeSlot != null && singleAvailabilityTimeSlot.ResultAvailabilityTimeSlot.Count > 0)
                                    {
                                        foreach (var slotData in singleAvailabilityTimeSlot?.ResultAvailabilityTimeSlot)
                                        {
                                            slotData.TransferId = data.Item4;
                                        }
                                        var tupleAvailabilityTimeSlot = Tuple.Create(data.Item1.Date, data.Item2, data.Item3, data.Item4, singleAvailabilityTimeSlot);
                                        if (singleAvailabilityTimeSlot != null)
                                        {
                                            tupAvailabilityTimeSlotRS.Add(tupleAvailabilityTimeSlot);
                                        }
                                    }

                                }
                            }
                            catch (Exception ex)
                            {
                                var isangoErrorEntity = new IsangoErrorEntity
                                {
                                    ClassName = "RaynaAdapter",
                                    MethodName = "GetActivity"
                                };
                                _log.Error(isangoErrorEntity, ex);
                            }
                        }
                    }
                }
                if (dicAvailabilityTourOptionRS != null && dicAvailabilityTourOptionRS.Count > 0)
                {
                    finalReturnAPIData.AvailabilityTourOptionRS.Add(dicAvailabilityTourOptionRS);
                }
                if (tupAvailabilityTimeSlotRS != null && tupAvailabilityTimeSlotRS.Count > 0)
                {
                    finalReturnAPIData.AvailabilityTimeSlotRS.AddRange(tupAvailabilityTimeSlotRS);
                }
                if (tupAvailabilityRS != null && tupAvailabilityRS.Count > 0)
                {
                    finalReturnAPIData.AvailabilityRES.AddRange(tupAvailabilityRS);
                }
            }
            #endregion


            if (finalReturnAPIData != null)
            {
                var responseObject = finalReturnAPIData as AvailabilityReturnData;
                if (responseObject != null)
                {
                    var convertedActivity = _raynaAvailabilityConverter.Convert(responseObject, MethodType.Availability, raynaCriteria);
                    resultIsangoActivity = convertedActivity as Isango.Entities.Activities.Activity;
                }
            }
            return resultIsangoActivity;
         }

        #endregion

        #region [Booking]
        public List<SelectedProduct> BookingConfirm(
           List<SelectedProduct> selectedProduct,
           string bookingReference,string voucherPhoneNumber,
           string token, out string request, out string response)
        {
            request = string.Empty;
            response = string.Empty;

            var finalBookingResponse = new BookingRES();
            var finalVoucherResponse = new List<TourTicketRES>();

            var convertedResultReturnValue = new List<SelectedProduct>();
            object convertedResult = null;
            if (selectedProduct != null)
            {
                var inputContext = new InputContext
                {
                    SelectedProductsLst = selectedProduct,
                    BookingReference= bookingReference,
                    VoucherPhoneNumber= voucherPhoneNumber
                };

                var responseObject = _raynaBookingCmdHandler.Execute(inputContext, token, MethodType.Booking, out request, out response);
                finalBookingResponse = responseObject as BookingRES;
                if (finalBookingResponse != null )
                {
                    var dataCheck = finalBookingResponse?.Result;
                    foreach (var singleItem in dataCheck?.Details)
                    {
                        if (singleItem?.Status?.ToLower() == "success" && singleItem?.DownloadRequired == true)
                        {
                            var inputContextTicket = new InputContext
                            {
                                UniqueNo = bookingReference,
                                ReferenceNo = dataCheck?.ReferenceNo,
                                BookingId = singleItem.BookingId,
                                ServiceUniqueId = singleItem?.ServiceUniqueId
                            };

                            //call ticket detail endpoint
                            var responseTicketObject = _raynaTourTicketCmdHandler.Execute(inputContextTicket, token, MethodType.TicketDetail, out request, out response);
                            finalVoucherResponse.Add(responseTicketObject as TourTicketRES);
                        }
                    }
                }
                if (responseObject != null)
                {
                    try
                    {
                        convertedResult = _raynaBookingConverter.Convert(Tuple.Create(finalBookingResponse, finalVoucherResponse, selectedProduct), MethodType.Booking, selectedProduct);
                        convertedResultReturnValue = convertedResult as List<SelectedProduct>;
                    }
                    catch (Exception ex)
                    {
                        var isangoErrorEntity = new IsangoErrorEntity
                        {
                            ClassName = nameof(BookingConfirm),
                            MethodName = nameof(BookingConfirm),
                            Token = token,
                            Params = $"{SerializeDeSerializeHelper.Serialize(selectedProduct)}"
                        };

                    }
                }
            }
            return convertedResultReturnValue;
        }
        #endregion


        public CancelRES CancelBooking(int bookingId, string referenceNo,
            string cancellationReason,  string token,
          out string apiRequest, out string apiResponse)
        {
            apiRequest = string.Empty;
            apiResponse = string.Empty;
            if (bookingId == 0)
            {
                return null;
            }
            var cancelREQ = new CancelREQ
            {
                BookingId = bookingId,
                ReferenceNo = referenceNo,
                CancellationReason = cancellationReason
            };
            var response = _raynaCancelCmdHandler.Execute(cancelREQ, token, MethodType.Cancelaltion, out apiRequest, out apiResponse);
            if (response == null) return null;
            var cancelBookingResponse = _raynaCancelConverter.Convert(response, MethodType.Cancelaltion);
            return cancelBookingResponse as CancelRES;
        }
    }
}