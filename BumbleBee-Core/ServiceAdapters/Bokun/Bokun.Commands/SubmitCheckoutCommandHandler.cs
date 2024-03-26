using Isango.Entities.Bokun;
using Logger.Contract;
using ServiceAdapters.Bokun.Bokun.Commands.Contracts;
using ServiceAdapters.Bokun.Bokun.Entities;
using ServiceAdapters.Bokun.Bokun.Entities.SubmitCheckout;
using ServiceAdapters.Bokun.Constants;
using System.Net;
using System.Text;
using Util;

namespace ServiceAdapters.Bokun.Bokun.Commands
{
    public class SubmitCheckoutCommandHandler : CommandHandlerBase, ISubmitCheckoutCommandHandler
    {
        private static bool _isNotificationOn;
        private static bool _isSendNotificationToCustomer;
        private static string _notificationEmailAddressIsango;
        private static string _isangoSupportPhoneNumber;

        static SubmitCheckoutCommandHandler()
        {
            try
            {
                var apiConfig = BokunAPIConfig.GetInstance();
                _isNotificationOn = apiConfig.IsNotificationOn;
                if (_isNotificationOn)
                {
                    _isSendNotificationToCustomer = apiConfig.IsSendNotificationToCustomer;
                    _isangoSupportPhoneNumber = apiConfig.SupportPhoneNumer;
                    _notificationEmailAddressIsango = apiConfig.NotificationEmailAddressIsango;

                    if (_isSendNotificationToCustomer)
                    {
                        _notificationEmailAddressIsango = string.Empty;
                    }
                }
            }
            catch (Exception)
            {
                _isNotificationOn = false;
            }
        }

        public SubmitCheckoutCommandHandler(ILogger log) : base(log)
        {
        }

        /// <summary>
        /// Return request required to hit API
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputContext"></param>
        /// <returns></returns>
        protected override object CreateInputRequest<T>(T inputContext)
        {
            SubmitCheckoutRq request = null;
            var bokunSelectedProduct = inputContext as BokunSelectedProduct;

            var selectedOption = bokunSelectedProduct?
                .ProductOptions?
                .FirstOrDefault(x => x.IsSelected);

            var currency = selectedOption?
                .BasePrice
                .Currency
                .IsoCode;

            if (bokunSelectedProduct != null)
            {
                var mainContactsAnswers = new List<Answer>();
                var activityAnswers = new List<Answer>();
                var passengerDetailAnswers = new List<Answer>();
                var passengerAnswers = new List<Answer>();
                var customerFullPayment = new List<Answer>();
                if (bokunSelectedProduct.Questions == null)
                {
                    bokunSelectedProduct.Questions = new List<Question>();
                }

                if (bokunSelectedProduct?.Questions?.Any(x => x.QuestionId == "email") != true)
                {
                    var emailQuestion = new Question
                    {
                        QuestionId = "email",
                        QuestionType = Constant.MainContactDetails
                    };
                    bokunSelectedProduct.Questions.Add(emailQuestion);
                }

                if (bokunSelectedProduct?.Questions?.Any(x => x.QuestionId == "phoneNumber") != true)
                {
                    var emailQuestion = new Question
                    {
                        QuestionId = "phoneNumber",
                        QuestionType = Constant.MainContactDetails
                    };
                    bokunSelectedProduct.Questions.Add(emailQuestion);
                }

                if (bokunSelectedProduct?.Questions?.Any(x => x.QuestionId == "phone") != true)
                {
                    var emailQuestion = new Question
                    {
                        QuestionId = "phone",
                        QuestionType = Constant.MainContactDetails
                    };
                    bokunSelectedProduct.Questions.Add(emailQuestion);
                }
                foreach (var ques in bokunSelectedProduct?.Questions)
                {
                    if (/*_isNotificationOn &&*/ !string.IsNullOrWhiteSpace(_notificationEmailAddressIsango))
                    {
                        if (string.Equals(ques.QuestionId, "email", StringComparison.CurrentCultureIgnoreCase))
                        {
                            ques.Answers = new List<string> { _notificationEmailAddressIsango };
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(_isangoSupportPhoneNumber))
                    {
                        if (ques.QuestionId.IndexOf("phone", StringComparison.CurrentCultureIgnoreCase) >= 0)
                        {
                            ques.Answers = new List<string> { _isangoSupportPhoneNumber };
                        }
                    }
                    var answer = new Answer
                    {
                        QuestionId = ques.QuestionId,
                        Values = ques.Answers
                    };
                    switch (ques.QuestionType)
                    {
                        case Constant.MainContactDetails:
                            {
                                if (mainContactsAnswers.All(x => x.QuestionId != answer.QuestionId))
                                    mainContactsAnswers.Add(answer);
                                break;
                            }
                        case Constant.ActivityQuestion:
                            {
                                if (activityAnswers.All(x => x.QuestionId != answer.QuestionId))
                                    activityAnswers.Add(answer);
                                break;
                            }
                        case Constant.PassengerQuestion:
                            {
                                if (passengerAnswers.All(x => x.QuestionId != answer.QuestionId))
                                    passengerAnswers.Add(answer);
                                break;
                            }
                        case Constant.PassengerDetails:
                            {
                                if (passengerDetailAnswers.All(x => x.QuestionId != answer.QuestionId))
                                    passengerDetailAnswers.Add(answer);
                                break;
                            }
                        case Constant.CustomerFullPayment:
                            {
                                if (customerFullPayment.All(x => x.QuestionId != answer.QuestionId))
                                    customerFullPayment.Add(answer);
                                break;
                            }

                        // ReSharper disable once RedundantEmptySwitchSection
                        default:
                            {
                                break;// throw new Exception("Unexpected Case");
                            }
                    }
                }

                if(passengerDetailAnswers?.Count == 0)
                {
                    passengerDetailAnswers = new List<Answer>();
                    passengerDetailAnswers.AddRange(mainContactsAnswers);
                }

                if (activityAnswers?.Count == 0)
                {
                    activityAnswers = new List<Answer>();
                    activityAnswers.AddRange(mainContactsAnswers);
                }

                var passengers = (from pricingCategoryId in bokunSelectedProduct?.PricingCategoryIds
                                  select new Passenger
                                  {
                                      PricingCategoryId = pricingCategoryId,
                                      PassengerDetails = passengerDetailAnswers,
                                      PassengerAnswers = passengerAnswers
                                  }).ToList();

                //Pickup start
                var pickupIdPartintDesc = bokunSelectedProduct?.HotelPickUpLocation?.Split('-')?.FirstOrDefault();
                int? pickupId = null;
                int.TryParse(pickupIdPartintDesc, out var tempId);
                if (tempId > 0)
                {
                    pickupId = tempId;
                }
                pickupId = pickupId > 0 ? pickupId : null;
                var isPickup = pickupId > 0;
                //Pickup end

                //DropOff start
                var dropupIdIdPartintDesc = bokunSelectedProduct?.HotelDropoffLocation?.Split('-')?.FirstOrDefault();
                int? dropoffId = null;
                int.TryParse(dropupIdIdPartintDesc, out var tempIdDrop);
                if (tempIdDrop > 0)
                {
                    dropoffId = tempIdDrop;
                }
                dropoffId = dropoffId > 0 ? dropoffId : null;
                var isDropOff = dropoffId > 0;
                //DropOff end
                var isSkipPickupDropOff = false;

                try
                {
                    //Chargeable pickup skip
                    isSkipPickupDropOff = bokunSelectedProduct.ProductId == 5266;
                    if (isSkipPickupDropOff)
                    {
                        isPickup = false;
                        pickupId = null;
                        bokunSelectedProduct.HotelDropoffLocation = null;
                        isDropOff = false;
                        dropoffId = null;
                        bokunSelectedProduct.HotelDropoffLocation = null;
                    }
                }
                catch (Exception)
                {

                }
                var activityBooking = new ActivityBookingDto
                {
                    ActivityId = bokunSelectedProduct.FactsheetId,
                    Date = bokunSelectedProduct.DateStart.ToString(Constant.DateInyyyyMMdd),
                    StartTimeId = bokunSelectedProduct.StartTimeId,
                    Answers = activityAnswers,
                    Passengers = passengers,
                    Pickup = isPickup,
                    PickupPlaceId = pickupId,
                    PickupDescription = bokunSelectedProduct?.HotelPickUpLocation,
                    RateId = bokunSelectedProduct?.RateId,
                    //DropOff
                    Dropoff = isDropOff,
                    DropoffPlaceId = isDropOff == true ? dropoffId : null,
                    DropoffDescription = (isDropOff == true ? (bokunSelectedProduct?.HotelDropoffLocation) : null),
                };

                var activityBookings = new List<ActivityBookingDto>
                {
                    activityBooking
                };

                var booking = new DirectBooking
                {
                    MainContactDetails = mainContactsAnswers,
                    ExternalBookingReference = bokunSelectedProduct?.BookingReferenceNumber,
                    ActivityBookings = activityBookings
                };
                request = new SubmitCheckoutRq
                {
                    Token = bokunSelectedProduct.Token,
                    DirectBooking = booking,
                    Currency = currency,
                    SendNotificationToMainContact = _isNotificationOn
                };
            }
            return request;
        }

        /// <summary>
        /// Call API to Submit checkout to API
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputContext"></param>
        /// <returns></returns>
        protected override object BokunApiRequest<T>(T inputContext)
        {
            var submitCheckoutRq = inputContext as SubmitCheckoutRq;
            var currency = submitCheckoutRq?.Currency ?? "GBP";
            var returnValue = default(string);
            var isRetry = false;
            var methodPath = GenerateMethodPath();
            methodPath = string.Format(methodPath, currency);

            var httpClient = AddRequestHeadersAndAddressToApi(Constant.Post, methodPath);

            var orderCreate = SerializeDeSerializeHelper.Serialize(submitCheckoutRq);
            var content = new StringContent(orderCreate, Encoding.UTF32, Constant.ApplicationOrJasonMediaType);

            var result = httpClient.PostAsync(methodPath, content);
            result.Wait();

            if (result.Result.StatusCode != HttpStatusCode.OK)
            {
                var response = result.Result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                Task.Run(() => _log.Write(orderCreate, response, MethodType.Submitcheckout.ToString(), submitCheckoutRq.Token, "Bokun"));
                var submitCheckoutRs = SerializeDeSerializeHelper.DeSerialize<SubmitCheckoutRs>(response);

                /*
                 var apiRequest = File.ReadAllText(@"D:\Sandeep\Isango\API BB\Bokun\sumbitCheckout-nameError.json");
                 var apiResponse = File.ReadAllText(@"D:\Sandeep\Isango\API BB\Bokun\sumbitCheckout-nameError.json");
                 response = apiResponse;
                 //*/

                #region Missing Questions

                if (submitCheckoutRs?.message?.Contains("is.bokun.dtos.questions.validation") == true
                        &&
                        submitCheckoutRs?.fields?.errors?.Any(x => !string.IsNullOrWhiteSpace(x.questionId)) == true
                    )
                {
                    var missingQuestions = submitCheckoutRs?.fields?.errors?.Where(x => !string.IsNullOrWhiteSpace(x.questionId)
                    && x.reason?.ToLower().Contains("missing") == true
                    && x.path?.ToLower().Contains("maincontactdetail") == true
                    )?.ToList();

                    var missingQuestionsPassenger = submitCheckoutRs?.fields?.errors?.Where(x => !string.IsNullOrWhiteSpace(x.questionId)
                    && x.reason?.ToLower().Contains("missing") == true
                    && x.path?.ToLower().Contains("passengerdetails") == true
                    )?.ToList();

                    var answers = new List<Answer>();
                    if (missingQuestions?.Any() == true)
                    {
                        foreach (var missingQuestion in missingQuestions?.ToList())
                        {
                            var answer = new Answer
                            {
                                QuestionId = missingQuestion?.questionId
                            };

                            if (missingQuestion.questionId.ToLowerInvariant() == Constant.MainContactDetails_Title)
                            {
                                answer.Values = new List<string> { "MR" };
                            }
                            else
                            {
                                answer.Values = new List<string> { "NA" };
                            }
                            answers.Add(answer);
                        }
                    }

                    if (missingQuestionsPassenger?.Any() == true)
                    {
                        foreach (var missingQuestion in missingQuestionsPassenger?.ToList())
                        {
                            var quePathArr = missingQuestion.path.Split('/');
                            if (quePathArr[1] == "activityBookings")
                            {
                                var activebookingIndex = Convert.ToInt32(quePathArr[2]);
                                var activebooking = submitCheckoutRq.DirectBooking.ActivityBookings[activebookingIndex];

                                if (quePathArr[3] == "passengers")
                                {
                                    var passengerIndex = Convert.ToInt32(quePathArr[4]);
                                    var activebookingPassenger = activebooking.Passengers[passengerIndex];

                                    if (quePathArr[5] == "passengerDetails")
                                    {
                                        var passengerDetailIndex = Convert.ToInt32(quePathArr[6]);
                                        var activebookingPassengerDetails = default(Answer);

                                        if (missingQuestion.questionId == "phoneNumber")
                                        {
                                            activebookingPassengerDetails = new Answer
                                            {
                                                QuestionId = missingQuestion.questionId,
                                                Values = new List<string> { { $"{_isangoSupportPhoneNumber}" } }
                                            };
                                        }
                                        if (missingQuestion.questionId == "firstName")
                                        {
                                            activebookingPassengerDetails = new Answer
                                            {
                                                QuestionId = missingQuestion.questionId,
                                                Values = new List<string> { { $"Not" } }
                                            };
                                        }
                                        if (missingQuestion.questionId == "lastName")
                                        {
                                            activebookingPassengerDetails = new Answer
                                            {
                                                QuestionId = missingQuestion.questionId,
                                                Values = new List<string> { { $"Required" } }
                                            };
                                        }
                                        if (activebookingPassenger?.PassengerDetails?.Any() == false)
                                        {
                                            activebookingPassenger.PassengerDetails = new List<Answer>();
                                            activebookingPassenger.PassengerDetails.Add(activebookingPassengerDetails);
                                        }
                                        else
                                        {
                                            activebookingPassenger.PassengerDetails[passengerDetailIndex] = activebookingPassengerDetails;
                                        }
                                        isRetry = true;
                                    }
                                }
                            }
                        }
                    }

                    if (answers?.Count > 0)
                    {
                        if (submitCheckoutRq?.DirectBooking?.MainContactDetails?.Any() != true)
                        {
                            submitCheckoutRq.DirectBooking.MainContactDetails = answers;
                        }
                        else
                        {
                            submitCheckoutRq.DirectBooking.MainContactDetails.AddRange(answers);
                        }
                        isRetry = true;
                    }
                }

                #endregion Missing Questions

                #region Missing drop off

                if (submitCheckoutRs?.message?.ToLowerInvariant().Contains("logic.InvalidDataException".ToLowerInvariant()) == true
                            &&
                            submitCheckoutRs?.message?.ToLowerInvariant().Contains("no drop off".ToLowerInvariant()) == true
                        )
                {
                    var pickupPlaceId = submitCheckoutRq?.DirectBooking?.ActivityBookings?.FirstOrDefault(x => x.PickupPlaceId > 0).PickupPlaceId ?? 30274;
                    if (pickupPlaceId == 0)
                    {
                        pickupPlaceId = 30274;// "30274- Not sure yet, I will contact you later to let you know."
                    }
                    if (pickupPlaceId > 0)
                    {
                        foreach (var activityBooking in submitCheckoutRq?.DirectBooking?.ActivityBookings.ToList())
                        {
                            if (activityBooking.DropoffPlaceId == null || activityBooking.DropoffPlaceId == 0)
                            {
                                activityBooking.DropoffPlaceId = pickupPlaceId;
                                activityBooking.DropoffDescription = "Not sure yet, I will contact you later to let you know";
                            }
                        }
                        isRetry = true;
                    }
                }

                #endregion Missing drop off

                #region Missing pickup

                if (submitCheckoutRs?.message?.ToLowerInvariant().Contains("logic.InvalidDataException".ToLowerInvariant()) == true
                    &&
                    submitCheckoutRs?.message?.ToLowerInvariant().Contains("no pick".ToLowerInvariant()) == true
                )
                {
                    var pickupPlaceId = 30274;// "30274- Not sure yet, I will contact you later to let you know."

                    foreach (var activityBooking in submitCheckoutRq?.DirectBooking?.ActivityBookings.ToList())
                    {
                        if (activityBooking.PickupPlaceId == null || activityBooking.PickupPlaceId == 0)
                        {
                            activityBooking.PickupPlaceId = pickupPlaceId;
                            activityBooking.PickupDescription = "Not sure yet, I will contact you later to let you know";
                        }
                    }
                    isRetry = true;
                }

                #endregion Missing pickup
            }
            if (isRetry)
            {
                orderCreate = SerializeDeSerializeHelper.Serialize(submitCheckoutRq);
                content = new StringContent(orderCreate, Encoding.UTF32, Constant.ApplicationOrJasonMediaType);
                result = httpClient.PostAsync(methodPath, content);
                result.Wait();
            }
            returnValue = ValidateApiResponse(result.Result)?.ToString();
            Task.Run(() => _log.Write(orderCreate, returnValue, MethodType.Submitcheckout.ToString() + "Retry", submitCheckoutRq.Token, "Bokun"));

            return returnValue;
        }

        /// <summary>
        /// Call API to Submit checkout to API asynchronously
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputContext"></param>
        /// <returns></returns>
        protected override async Task<object> BokunApiRequestAsync<T>(T inputContext)
        {
            var task = Task.Run(() => BokunApiRequest(inputContext));
            return task.GetAwaiter().GetResult();
        }

        private readonly Func<string> GenerateMethodPath = () => UriConstants.SubmitCheckout;
    }
}