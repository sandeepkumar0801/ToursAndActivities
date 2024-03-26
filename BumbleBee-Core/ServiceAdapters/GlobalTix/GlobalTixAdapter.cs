using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Booking;
using Isango.Entities.GlobalTix;
using ServiceAdapters.GlobalTix.Constants;
using ServiceAdapters.GlobalTix.GlobalTix.Commands.Contracts;
using ServiceAdapters.GlobalTix.GlobalTix.Converters.Contracts;
using ServiceAdapters.GlobalTix.GlobalTix.Entities;
using ServiceAdapters.GlobalTix.GlobalTix.Entities.RequestResponseModels;
using Util;

namespace ServiceAdapters.GlobalTix
{
    public class GlobalTixAdapter : IGlobalTixAdapter, IAdapter
    {
        #region "Private Members"

        private readonly ICancelByTicketCommandHandler _cancelByTicketCommandHandler;
        private readonly ICancelByBookingCommandHandler _cancelByBookingCommandHandler;
        private IActivityInfoCommandHandler _activityInfoCommandHandler;
        private IActivityListCommandHandler _activityListCommandHandler;
        private IActivityTicketTypeCommandHandler _activityTicketTypeCommandHandler;
        private IActivityTicketTypesCommandHandler _activityTicketTypesCommandHandler;
        private ICityListCommandHandler _cityListCommandHandler;
        private ICountryListCommandHandler _countryListCommandHandler;
        private ICreateBookingCommandHandler _createBookingCommandHandler;
        private IPackageInfoCommandHandler _packageInfoCommandHandler;
        private IPackageListCommandHandler _packageListCommandHandler;

        private readonly IActivityEntityConverter _activityEntityConverter;
        private readonly IActivityInfoConverter _activityInfoConverter;
        private readonly IActivityListConverter _activityListConverter;
        private readonly IBookingConverter _bookingConverter;
        private readonly IPackageEntityConverter _packageEntityConverter;
        private readonly IPackageInfoConverter _packageConverter;
        private readonly int _maxParallelThreadCount;

        #endregion "Private Members"

        #region "Constructor"

        public GlobalTixAdapter(IActivityInfoCommandHandler activityInfoCommandHandler, IActivityListCommandHandler activityListCommandHandler,
            IActivityTicketTypeCommandHandler activityTicketTypeCommandHandler,
            IActivityTicketTypesCommandHandler activityTicketTypesCommandHandler, ICancelByTicketCommandHandler cancelByTicketCommandHandler,
            ICancelByBookingCommandHandler cancelByBookingCommandHandler, ICityListCommandHandler cityListCommandHandler,
            ICountryListCommandHandler countryListCommandHandler, ICreateBookingCommandHandler createBookingCmdHandler,
            IPackageInfoCommandHandler packageInfoCommandHandler, IPackageListCommandHandler packageListCommandHandler,
            IActivityEntityConverter activityEntityConverter, IActivityInfoConverter activityInfoConverter, IActivityListConverter availabilityConverter,
            IBookingConverter bookingConverter, IPackageEntityConverter packageEntityConverter, IPackageInfoConverter packageInfoConverter)
        {
            _activityInfoCommandHandler = activityInfoCommandHandler;
            _activityListCommandHandler = activityListCommandHandler;
            _activityTicketTypeCommandHandler = activityTicketTypeCommandHandler;
            _activityTicketTypesCommandHandler = activityTicketTypesCommandHandler;
            _cancelByTicketCommandHandler = cancelByTicketCommandHandler;
            _cancelByBookingCommandHandler = cancelByBookingCommandHandler;
            _cityListCommandHandler = cityListCommandHandler;
            _countryListCommandHandler = countryListCommandHandler;
            _createBookingCommandHandler = createBookingCmdHandler;
            _packageInfoCommandHandler = packageInfoCommandHandler;
            _packageListCommandHandler = packageListCommandHandler;

            _activityEntityConverter = activityEntityConverter;
            _activityInfoConverter = activityInfoConverter;
            _activityListConverter = availabilityConverter;
            _bookingConverter = bookingConverter;
            _packageConverter = packageInfoConverter;
            _packageEntityConverter = packageEntityConverter;
            _maxParallelThreadCount = MaxParallelThreadCountHelper.GetMaxParallelThreadCount("MaxParallelThreadCount");
        }

        #endregion "Constructor"

        //Test pending Destination based credentials
        public Activity GetActivityInformation(GlobalTixCriteria gtCriteria, string token, bool isNonThailandProduct)
        {
            if (AgentAuthenticateDetails.Instance(isNonThailandProduct).IsAuthenticated)
            {
                Activity actInfo = GetActivityInfo(gtCriteria, token);
                return actInfo ?? GetPackageInfo(gtCriteria, token);
            }

            return null;
        }

        public bool CancelByTicket(string supplierReferenceNumber, string token, out string requestJson, out string responseJson)
        {
            requestJson = string.Empty;
            responseJson = string.Empty;
            if (AgentAuthenticateDetails.Instance(false).IsAuthenticated)
            {
                // enter if authentication works
                var inputContext = new CancelInputContext
                {
                    MethodType = MethodType.CancelBooking,
                    BookingNumber = supplierReferenceNumber
                };

                var returnValue = _cancelByTicketCommandHandler.Execute(inputContext, token, out requestJson, out responseJson);
                CancelByTicketRS cancelRS = SerializeDeSerializeHelper.DeSerialize<CancelByTicketRS>(returnValue.ToString());
                return cancelRS.IsSuccess;
            }

            // If not authenticated, return status of ticket cancellation as false
            return false;
        }

        public bool CancelByBooking(string supplierReferenceNumber, string token, out string requestJson, out string responseJson, bool isNonThailandProduct)
        {
            requestJson = string.Empty;
            responseJson = string.Empty;
            if (AgentAuthenticateDetails.Instance(isNonThailandProduct).IsAuthenticated)
            {
                var inputContext = new CancelInputContext
                {
                    MethodType = MethodType.CancelBooking,
                    BookingReference = supplierReferenceNumber
                };

                var returnValue = _cancelByBookingCommandHandler.Execute(inputContext, token, out requestJson, out responseJson);
                CancelByBookingRS cancelRS = SerializeDeSerializeHelper.DeSerialize<CancelByBookingRS>(returnValue.ToString());
                return cancelRS.IsSuccess && cancelRS.Data.Tickets.Count(tkt => tkt.Status.Name == "REVOKED") == cancelRS.Data.Tickets.Count;
            }

            // If not authenticated, return status of booking cancellation as false
            return false;
        }

        //Pending Destination based credentials
        public Booking CreateBooking(List<SelectedProduct> selectedProducts, string bookingReference, string token, out string requestJson, out string responseJson)
        {
            requestJson = string.Empty;
            responseJson = string.Empty;

            if (selectedProducts == null || selectedProducts.Count <= 0 || AgentAuthenticateDetails.Instance(false).IsAuthenticated == false || AgentAuthenticateDetails.Instance(true).IsAuthenticated == false)
            {
                return null;
            }

            //Code area for separating Thailand and NonThailand products as well as hitting api end point with region based credentials.
            List<SelectedProduct> thailandProducts = new List<SelectedProduct>();
            List<SelectedProduct> nonThailandProducts = new List<SelectedProduct>();
            //Booking object to resend from this code should be made here. For both Thailand and NonThailand Products
            Booking finalBookingResponse = new Booking()
            {
                SelectedProducts = new List<SelectedProduct>()
            };
            //Booking response object for Thailand products
            Booking bookingResponseForThailandProducts = new Booking()
            {
                SelectedProducts = new List<SelectedProduct>()
            };
            //Booking response object for NonThailand products
            Booking bookingResponseForNonThailandProducts = new Booking()
            {
                SelectedProducts = new List<SelectedProduct>()
            };
            bool isNonThailandProduct;

            thailandProducts = selectedProducts.FindAll(thisProduct => thisProduct.RegionId.ToLowerInvariant().Equals("6667"));
            nonThailandProducts = selectedProducts.FindAll(thisProduct => !thisProduct.RegionId.ToLowerInvariant().Equals("6667"));

            if (thailandProducts != null && thailandProducts.Count > 0)
            {
                isNonThailandProduct = false;
                if (AgentAuthenticateDetails.Instance(isNonThailandProduct).IsAuthenticated)
                {
                    bookingResponseForThailandProducts = (Booking)GetBookingResponse(selectedProducts, bookingReference, token, out requestJson, out responseJson);
                }
            }
            if (nonThailandProducts != null && nonThailandProducts.Count > 0)
            {
                isNonThailandProduct = true;
                if (AgentAuthenticateDetails.Instance(isNonThailandProduct).IsAuthenticated)
                {
                    bookingResponseForNonThailandProducts = (Booking)GetBookingResponse(selectedProducts, bookingReference, token, out requestJson, out responseJson);
                }
            }

            //Logic to create final booking response from Thailand and NonThailand product bookings
            if (bookingResponseForThailandProducts != null && bookingResponseForThailandProducts.SelectedProducts.Count > 0 || bookingResponseForNonThailandProducts != null && bookingResponseForNonThailandProducts.SelectedProducts.Count > 0)
            {
                return (Booking)CreateFinalBookingResponse(bookingResponseForThailandProducts, bookingResponseForNonThailandProducts);
            }
            return null;
        }

        private object CreateFinalBookingResponse(Booking bookingResponseForThailandProducts, Booking bookingResponseForNonThailandProducts)
        {
            Booking finalBookingResponse = new Booking()
            {
                SelectedProducts = new List<SelectedProduct>()
            };

            if (bookingResponseForThailandProducts != null && bookingResponseForThailandProducts.SelectedProducts.Count > 0)
            {
                //Reference no used only for setting product option's status
                finalBookingResponse.ReferenceNumber = bookingResponseForThailandProducts.ReferenceNumber;
                finalBookingResponse.SelectedProducts.AddRange(bookingResponseForThailandProducts.SelectedProducts);
            }

            if (bookingResponseForNonThailandProducts != null && bookingResponseForNonThailandProducts.SelectedProducts.Count > 0)
            {
                //Reference no used only for setting product option's status
                finalBookingResponse.ReferenceNumber = bookingResponseForNonThailandProducts.ReferenceNumber;
                finalBookingResponse.SelectedProducts.AddRange(bookingResponseForNonThailandProducts.SelectedProducts);
            }

            return finalBookingResponse;
        }

        private object GetBookingResponse(List<SelectedProduct> selectedProducts, string bookingReference, string token, out string requestJson, out string responseJson)
        {
            var inputContext = new BookInputContext
            {
                MethodType = MethodType.CreateBooking,
                SelectedProducts = selectedProducts,
                BookingReferenceNumber = bookingReference
            };

            // The following code block is not in a try-catch block as exceptions
            // are caught and logged in Isango.Service.SupplierCookingService
            var response = _createBookingCommandHandler.Execute(inputContext, token, out requestJson, out responseJson);
            var bookingRS = SerializeDeSerializeHelper.DeSerialize<BookingRS>(response.ToString());

            if (bookingRS?.BookingData == null || bookingRS.IsSuccess == false || String.IsNullOrEmpty(bookingRS.BookingData.BookReferenceNumber)
                || bookingRS.BookingData.Tickets == null || bookingRS.BookingData.Tickets.Count <= 0)
            {
                return null;
            }

            return (Booking)_bookingConverter.Convert(bookingRS, inputContext);
        }

        public List<CountryCity> GetCountryCityList(string token)
        {
            if (AgentAuthenticateDetails.Instance(true).IsAuthenticated == false)
            {
                return null;
            }

            var countryListRSRaw = _countryListCommandHandler.Execute(null, token);
            CountryListRS countryListRS = SerializeDeSerializeHelper.DeSerialize<CountryListRS>(countryListRSRaw.ToString());
            if (countryListRS == null && countryListRS.IsSuccess == false)
            {
                return null;
            }

            Dictionary<int, string> countryIdToName = new Dictionary<int, string>();
            countryListRS.Countries.ForEach(x => { countryIdToName.Add(x.Id, x.Name); });

            var cityListRSRaw = _cityListCommandHandler.Execute(null, token);
            CityListRS cityListRS = SerializeDeSerializeHelper.DeSerialize<CityListRS>(cityListRSRaw.ToString());
            if (cityListRS == null && cityListRS.IsSuccess == false)
            {
                return null;
            }

            List<CountryCity> countryCities = new List<CountryCity>(cityListRS.Cities.Count);
            foreach (CityData city in cityListRS.Cities)
            {
                string countryName = countryIdToName[city.CountryId] ?? "";
                countryCities.Add(
                    new CountryCity()
                    {
                        CountryId = city.CountryId.ToString(),
                        CountryName = countryIdToName[city.CountryId] ?? "",
                        CityId = city.Id.ToString(),
                        CityName = city.Name
                    }
                );
            }

            return countryCities;
        }

        public List<GlobalTixActivity> GetAllActivities(string country, string city, string token, bool isNonThailandProduct)
        {
            if (AgentAuthenticateDetails.Instance(isNonThailandProduct).IsAuthenticated == false)
            {
                return null;
            }

            AvailabilityInputContext availCtx = new AvailabilityInputContext()
            {
                CountryId = country,
                CityId = city,
                PageNumber = 0
            };

            // The GlobalTix API for retrieving list of activities returns one page at a time.
            // Keep calling the API till all the activities are retrieved.
            bool hasMorePages = false;
            List<GlobalTixActivity> allActivities = new List<GlobalTixActivity>();
            do
            {
                var actListRSRaw = _activityListCommandHandler.Execute(availCtx, token);
                if (actListRSRaw == null)
                {
                    break;
                }
                ActivitiesListRS actListRS = SerializeDeSerializeHelper.DeSerialize<ActivitiesListRS>(actListRSRaw.ToString());

                if (actListRS == null || actListRS.IsSuccess == false)
                {
                    break;
                }

                allActivities.AddRange(actListRS.ListData.ConvertAll(x => _activityEntityConverter.Convert(x)).Cast<GlobalTixActivity>());
                hasMorePages = (allActivities.Count < actListRS.TotalActivities);
                if (hasMorePages)
                {
                    availCtx.PageNumber++;
                }
            }
            while (hasMorePages);

            return allActivities;
        }

        public List<GlobalTixPackage> GetAllPackages(string token, bool isNonThailandProduct)
        {
            if (AgentAuthenticateDetails.Instance(isNonThailandProduct).IsAuthenticated == false)
            {
                return null;
            }

            // Set value of PageNumber parameter as '0' to retrieve all packages in single call
            // instead of page by page.
            PackageListInputContext pkgListCtx = new PackageListInputContext()
            {
                PageNumber = 0
            };

            var pkgsListRSRaw = _packageListCommandHandler.Execute(pkgListCtx, token);
            if (pkgsListRSRaw == null)
            {
                return null;
            }
            PackagesListRS pkgsListRS = SerializeDeSerializeHelper.DeSerialize<PackagesListRS>(pkgsListRSRaw.ToString());
            if (pkgsListRS == null || pkgsListRS.IsSuccess == false)
            {
                return null;
            }

            List<GlobalTixPackage> gtPackageList = new List<GlobalTixPackage>();
            foreach (Package pkg in pkgsListRS.Packages)
            {
                GlobalTixPackage gtPkg = _packageEntityConverter.Convert(pkg) as GlobalTixPackage;

                // Retrieve detailed package information to check related packages. In GlobalTix packages,
                // there is one package for each pax type. These related packages are linked together using
                // package element 'linkId'.
                PackageInfoInputContext pkgInfoCtx = new PackageInfoInputContext()
                {
                    PackageId = pkg.Id.ToString()
                };
                var pkgInfoRSRaw = _packageInfoCommandHandler.Execute(pkgInfoCtx, "datadump");
                if (pkgInfoRSRaw == null)
                {
                    return null;
                }
                PackageInfoRS pkgInfoRS = SerializeDeSerializeHelper.DeSerialize<PackageInfoRS>(pkgInfoRSRaw.ToString());
                if (pkgInfoRS != null && pkgInfoRS.IsSuccess)
                {
                    gtPkg.Desc = pkgInfoRS.Data.Desc;
                    gtPkg.RelatedPackages = new List<int>();

                    foreach (Variant pkgVar in pkgInfoRS.Data.Variants)
                    {
                        if (pkgVar.PackageId == pkg.Id)
                        {
                            continue;
                        }

                        Package relPkg = pkgsListRS.Packages.FirstOrDefault(x => x.Id == pkgVar.PackageId);
                        if (relPkg != null)
                        {
                            gtPkg.RelatedPackages.Add(relPkg.Id);
                        }
                    }
                }

                gtPackageList.Add(gtPkg);
            }

            return gtPackageList;
        }

        /// <summary>
        /// Get Ticket Type Detail
        ///5081->3215(QRCode)->4260,4261
        ///5082->3216(Barcode)->
        ///5083->3217(PDFVoucher)->
        ///5093->3218-(SeparateEMail)>
        ///5095->3193(Q&A)->
        ///5096->3077(Product with Schedule/Capacity)
        ///5120->3433 (Visit Date)
        /// </summary>
        /// <param name="ticketTypeIds"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Tuple<List<ContractQuestion>, Dictionary<int, string>> GetTicketTypeDetail(List<string> ticketTypeIds, string token)
        {
            var ticketTypeDetails = new Dictionary<int, TicketTypeDetail>();
            var ticketIds = new List<int>();
            var contractQuestions = new List<ContractQuestion>();
            var pickupLoations = new Dictionary<int, string>();
            //Call TicketType command handler to retrieve questions for each TicketType
            foreach (var tktType in ticketTypeIds)
            {
                ActivityTicketTypeInputContext inCtx = new ActivityTicketTypeInputContext
                {
                    MethodType = MethodType.TicketTypeDetail,
                    TicketType = tktType
                };
                var ticketTypeRSRaw = _activityTicketTypeCommandHandler.Execute(inCtx, token);
                if (ticketTypeRSRaw == null)
                {
                    return null;
                }
                TicketTypeRS ticketTypeRS = SerializeDeSerializeHelper.DeSerialize<TicketTypeRS>(ticketTypeRSRaw.ToString());
                if (ticketTypeRS == null || ticketTypeRS.IsSuccess == false)
                {
                    continue;
                }
                ticketTypeDetails.Add(Convert.ToInt32(tktType), ticketTypeRS.Data);
                ticketIds.Add(Convert.ToInt32(tktType));
            }
            contractQuestions = GetContractQuestions(ticketIds, ticketTypeDetails);
            pickupLoations = GetPickUpLocations(ticketIds, ticketTypeDetails);
            return Tuple.Create(contractQuestions, pickupLoations);
        }

        private List<ContractQuestion> GetContractQuestions(List<int> tktIds, Dictionary<int, TicketTypeDetail> tktDtls)
        {
            if (tktIds == null || tktIds.Count <= 0 || tktDtls == null || tktDtls.Count <= 0)
            {
                return null;
            }

            IEnumerable<List<ContractQuestion>> contractQuestions =
                tktIds.Where(tktId => tktDtls.ContainsKey(tktId) && tktDtls[tktId].Questions != null && tktDtls[tktId].Questions.Count > 0)
                    .Select(tktId => ConvertQuestionsToContractQuestions(tktDtls[tktId].Questions, Constant.QUESTION));

            List<ContractQuestion> optionQuestions = new List<ContractQuestion>();
            List<string> questionCodes = new List<string>();
            foreach (List<ContractQuestion> questions in contractQuestions)
            {
                foreach (ContractQuestion q in questions)
                {
                    if (q != null && questionCodes.Contains(q.Code) == false)
                    {
                        optionQuestions.Add(q);
                        questionCodes.Add(q.Code);
                    }
                }
            }

            return optionQuestions;
        }

        /// <summary>
        /// Get PickUp Locations
        /// </summary>
        /// <param name="tktIds"></param>
        /// <param name="tktDtls"></param>
        /// <returns></returns>
        private Dictionary<int, string> GetPickUpLocations(List<int> tktIds, Dictionary<int, TicketTypeDetail> tktDtls)
        {
            if (tktIds == null || tktIds.Count <= 0 || tktDtls == null || tktDtls.Count <= 0)
            {
                return null;
            }

            IEnumerable<List<ContractQuestion>> contractQuestions =
                tktIds.Where(tktId => tktDtls.ContainsKey(tktId) && tktDtls[tktId].Questions != null && tktDtls[tktId].Questions.Count > 0)
                    .Select(tktId => ConvertQuestionsToContractQuestions(tktDtls[tktId].Questions, Constant.PICKUP));

            var pickUpDetails = new Dictionary<int, string>();
            List<string> questionCodes = new List<string>();
            foreach (List<ContractQuestion> questions in contractQuestions)
            {
                foreach (ContractQuestion q in questions)
                {
                    if (q != null && !string.IsNullOrEmpty(q.Answer) && questionCodes.Contains(q.Code) == false)
                    {
                        string[] splitOptions = q.Answer.Split('|');
                        if (splitOptions.Count() > 1)
                        {
                            foreach (var item in splitOptions)
                            {
                                pickUpDetails.Add(Math.Abs(Guid.NewGuid().GetHashCode()), item.ToString());
                            }
                        }
                    }
                }
            }
            return pickUpDetails;
        }

        protected List<ContractQuestion> ConvertQuestionsToContractQuestions(List<Question> questions, string condition = null)
        {
            if (questions == null || questions.Count <= 0)
            {
                return null;
            }

            return questions.Select(q => ConvertQuestionToContractQuestion(q, condition)).ToList();
        }

        protected ContractQuestion ConvertQuestionToContractQuestion(Question q, string condition = null)
        {
            string opts = (q.Options != null && q.Options.Length > 0) ? $" {string.Join("|", q.Options)}" : string.Empty;

            return
                    new ContractQuestion
                    {
                        Code = q.Id.ToString(),
                        Name = q.Id.ToString(),
                        Description = $"{q.QuestionText}",
                        IsRequired = (q.IsOptional == false),
                        Answer = opts
                    };
        }

        #region Private Methods

        private Activity GetActivityInfo(GlobalTixCriteria gtCriteria, string token)
        {
            ActivityInfoInputContext inputContext = new ActivityInfoInputContext
            {
                MethodType = MethodType.ActivityAvailability,
                ServiceOptionID = gtCriteria?.ServiceOptionID ?? 0,
                ActivityId = gtCriteria.ActivityId,
                FactSheetId = gtCriteria.FactSheetId,
                Days2Fetch = gtCriteria.Days2Fetch,
                NoOfPassengers = gtCriteria.NoOfPassengers,
                CheckinDate = gtCriteria.CheckinDate
            };

            // Call ActivityInfo command handler to invoke supplier API
            //attraction/get
            var activityInfoRSRaw = _activityInfoCommandHandler.Execute(inputContext, token);
            if (activityInfoRSRaw == null)
            {
                return null;
            }
            ActivityInfoRS activityInfoRS = SerializeDeSerializeHelper.DeSerialize<ActivityInfoRS>(activityInfoRSRaw.ToString());
            if (activityInfoRS == null || activityInfoRS.IsSuccess == false)
            {
                return null;
            }

            // Call TicketTypes command handler //TicketTypesLIST
            inputContext.MethodType = MethodType.TicketTypes;
            var ticketTypesRSRaw = _activityTicketTypesCommandHandler.Execute(inputContext, token);
            if (ticketTypesRSRaw == null)
            {
                return null;
            }
            var ticketTypesRS = SerializeDeSerializeHelper.DeSerialize<TicketTypesRS>(ticketTypesRSRaw.ToString());
            if (ticketTypesRS?.Data == null || ticketTypesRS?.IsSuccess == false)
            {
                return null;
            }

            //Code for integrating "ticketType/get" for Original price and Payable amount and other details for creating Pricing Unit.

            var query = from tktType in ticketTypesRS?.Data
                        from paxType in gtCriteria?.NoOfPassengers.Keys
                        where tktType?.Variation?.Name?.ToLower().Contains(paxType.ToString().ToLower()) == true
                        select tktType;

            var gtPaxIdsToGetDetailsFor = query.Select(x => x.Id).Distinct().ToList();

            //new List<int>();
            //foreach (var ticketTypeGroup in activityInfoRS?.Data?.TicketTypeGroups)
            //{
            //    foreach (var product in ticketTypeGroup?.Products)
            //    {
            //        gtPaxIdsToGetDetailsFor.Add(product.Id);
            //    }
            //}

            //gtPaxIdsToGetDetailsFor = gtPaxIdsToGetDetailsFor.Distinct().ToList();

            var listOfGTPaxTypeDetailsForPricingUnitCreation = new List<TicketTypeDetail>();
            //foreach (var paxId in gtPaxIdsToGetDetailsFor)

            Parallel.ForEach(gtPaxIdsToGetDetailsFor, new ParallelOptions { MaxDegreeOfParallelism = _maxParallelThreadCount }, (paxId) =>
            {
                ActivityTicketTypeInputContext inCtx = new ActivityTicketTypeInputContext
                {
                    MethodType = MethodType.TicketTypeDetail,
                    TicketType = paxId.ToString()
                };
                var ticketTypeRSRaw = _activityTicketTypeCommandHandler.Execute(inCtx, token);
                if (ticketTypeRSRaw != null)
                {
                    var ticketTypeRS = SerializeDeSerializeHelper.DeSerialize<TicketTypeRS>(ticketTypeRSRaw.ToString());
                    if (ticketTypeRS?.Data != null && ticketTypeRS.IsSuccess == true)
                    {
                        listOfGTPaxTypeDetailsForPricingUnitCreation.Add(ticketTypeRS.Data);
                    }
                }
            }
            );

            inputContext.ActivityInfo = activityInfoRS.Data;
            inputContext.PaxTypeDetails = listOfGTPaxTypeDetailsForPricingUnitCreation;
            return (Activity)_activityInfoConverter.Convert(inputContext);
        }

        private Activity GetPackageInfo(GlobalTixCriteria gtCriteria, string token)
        {
            PackageInfoInputContext inputContext = new PackageInfoInputContext
            {
                MethodType = MethodType.PackageAvailability,
                ServiceOptionID = gtCriteria.ServiceOptionID ?? 0,
                PackageId = gtCriteria.ActivityId,
                FactSheetId = gtCriteria.FactSheetId,
                Days2Fetch = gtCriteria.Days2Fetch,
                CheckinDate = gtCriteria.CheckinDate
            };

            // Call ActivityInfo command handler to invoke supplier API
            var packageInfoRSRaw = _packageInfoCommandHandler.Execute(inputContext, token);
            if (packageInfoRSRaw == null)
            {
                return null;
            }
            PackageInfoRS packageInfoRS = SerializeDeSerializeHelper.DeSerialize<PackageInfoRS>(packageInfoRSRaw.ToString());
            if (packageInfoRS == null || packageInfoRS.IsSuccess == false)
            {
                return null;
            }

            List<PackageInfo> linkedPackages = new List<PackageInfo>() { packageInfoRS.Data };
            foreach (Variant psgrTypeVar in packageInfoRS.Data.Variants)
            {
                if (psgrTypeVar.PackageId != packageInfoRS.Data.Id)
                {
                    PackageInfoInputContext inCtx = new PackageInfoInputContext
                    {
                        PackageId = psgrTypeVar.PackageId.ToString(),
                        FactSheetId = gtCriteria.FactSheetId,
                        Days2Fetch = gtCriteria.Days2Fetch
                    };

                    var linkedPkgInfoRSRaw = _packageInfoCommandHandler.Execute(inCtx, token);
                    if (linkedPkgInfoRSRaw == null)
                    {
                        return null;
                    }
                    PackageInfoRS linkedPkgInfoRS = SerializeDeSerializeHelper.DeSerialize<PackageInfoRS>(linkedPkgInfoRSRaw.ToString());
                    if (linkedPkgInfoRS != null && linkedPkgInfoRS.IsSuccess)
                    {
                        linkedPackages.Add(linkedPkgInfoRS.Data);
                    }
                }
            }

            inputContext.LinkedPackages = linkedPackages;
            inputContext.TicketTypes = null;
            return (Activity)_packageConverter.Convert(inputContext);
        }

        #endregion Private Methods
    }
}