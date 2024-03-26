using Isango.Entities.Activities;
using Isango.Entities.TourCMS;
using Logger.Contract;
using ServiceAdapters.TourCMS.Constants;
using ServiceAdapters.TourCMS.TourCMS.Commands.Contracts;
using ServiceAdapters.TourCMS.TourCMS.Entities.NewBooking;
using System.Text;
using Util;


namespace ServiceAdapters.TourCMS.TourCMS.Commands
{
    public class NewBookingCommandHandler : CommandHandlerBase,
        INewBookingCommandHandler
    {
        public NewBookingCommandHandler(ILogger iLog) : base(iLog)
        {
        }

        protected override object CreateInputRequest<T>(T bookingContext)
        {
            var bookingConfirmRq = new NewBookingRequest();
            var prefixServiceCode = string.Empty;
            try
            {
                var supportEmail = ConfigurationManagerHelper.GetValuefromAppSettings("mailfrom");
                var testMode = ConfigurationManagerHelper.GetValuefromAppSettings("TourCMSTestMode");
                var inputContext = bookingContext as ServiceAdapters.TourCMS.TourCMS.Entities.InputContext;
                var selectedProducts = inputContext.SelectedProducts;
                var languageCode = inputContext.LanguageCode;
                var voucherEmailAddress = inputContext.VoucherEmailAddress;
                var VoucherPhoneNumber = inputContext.VoucherPhoneNumber;
                var TotalCustomers = inputContext.TotalCustomers;
                var IsangoRefNumber = inputContext?.BookingReference;

                var tourCMSSelectdProducts = (TourCMSSelectedProduct)selectedProducts;
                if (!(tourCMSSelectdProducts != null))
                {
                    return null;
                }
                var selectedProduct = tourCMSSelectdProducts;

                var selectedOptions = selectedProduct?.ProductOptions
                    ?.FindAll(f => f.IsSelected.Equals(true))
                    ?.Cast<ActivityOption>().ToList();

                //find TourCMs Channel Id and TourCMS Account Id
                prefixServiceCode = selectedOptions?.FirstOrDefault()?.PrefixServiceCode;
                var leadCustomer = selectedProduct?.ProductOptions?
                    .Find(f => f.IsSelected.Equals(true))
                    .Customers.Find(f => f.IsLeadCustomer.Equals(true));

                var customerName = leadCustomer?.FirstName.Contains(" ") == true ?
                    leadCustomer?.FirstName?.Split(' ')?.ToList() :
                    new List<string> { { leadCustomer.FirstName }, { leadCustomer.LastName } };

                var lName = customerName.LastOrDefault() ?? "NA";
                var fName = customerName.FirstOrDefault() ?? "NA";
                Int64.TryParse(VoucherPhoneNumber, out long voucherPhoneNumber);
                var bookingCustomersCustomer = new BookingCustomersCustomer
                {
                    Title = leadCustomer?.Title,
                    FirstName = fName,
                    Surname = lName,
                    Email = testMode=="1"? voucherEmailAddress : supportEmail,
                    TelHome = voucherPhoneNumber,
                };
                bookingConfirmRq.AgentReference =IsangoRefNumber;
                bookingConfirmRq.Customers = new BookingCustomers
                {
                    customer = new BookingCustomersCustomer()
                };
                bookingConfirmRq.Customers.customer = bookingCustomersCustomer;
                var lang = string.Empty;
                 
                
                lang = languageCode ?? "en";
                

                if (!(selectedOptions?.Count > 0))
                {
                    return null;
                }
                bookingConfirmRq.Components = new BookingComponents
                {
                    component = new BookingComponentsComponent()
                };
                var componentkey =string.Empty;
                var optionkey = string.Empty;
                var CheckRateKey = ((ActivityOption)(selectedOptions.FirstOrDefault())).RateKey;
                //if rate key contain only one value, then it have no extras
                string Delimiter = "!!!";
                var rateKeyResult = CheckRateKey.Split(new[] { Delimiter }, StringSplitOptions.None);
                if (rateKeyResult.Count() > 1)
                {
                    componentkey = rateKeyResult[0];
                    optionkey = rateKeyResult[1];
                    bookingConfirmRq.Components.component.ComponentKey = componentkey;

                    bookingConfirmRq.Components.component.Options = new Option
                    {
                        option = new OptionsComponentKey()
                    };

                    bookingConfirmRq.Components.component.Options.option.ComponentKey = optionkey;
                }
                else
                {
                    componentkey = rateKeyResult[0];
                    bookingConfirmRq.Components.component.ComponentKey = componentkey;
                }
                bookingConfirmRq.TotalCustomers = TotalCustomers;

                //PickUp Key
                if (!string.IsNullOrEmpty(selectedProduct?.HotelPickUpLocation))
                {
                    var pickupId = selectedProduct?.HotelPickUpLocation.Split('-')[0];
                    if (selectedProduct.PickupPointsForTourCMS != null && selectedProduct.PickupPointsForTourCMS.Count > 0)
                    {
                        foreach (var tourCMS in selectedProduct.PickupPointsForTourCMS)
                        {
                            if (tourCMS.PickupId == pickupId)
                            {
                                bookingConfirmRq.Components.component.PickupKey = tourCMS.PickupKey;
                            }
                        }
                    }
                }

                //Check Contract Questions Exist
                var Getquestions = tourCMSSelectdProducts?.ContractQuestions;
                bookingConfirmRq.Components.component.Replies = new List<Reply>();
                
                
               //Start :Questions and Answers
                if (Getquestions != null && Getquestions.Count > 0)
                {
                    foreach (var ques in Getquestions)
                    {
                        var splitData = ques.Code.Split('_');
                       
                        var ReplyAnswers = new ReplyAnswers
                        {
                            QuestionKey = splitData[3],
                            
                        };
                        ReplyAnswers.Answers = new Answer();
                        var AnswerValueGet = new AnswerValue
                        {
                            AnswerValueData = ques.Answer
                        };
                        ReplyAnswers.Answers.AnswerData = AnswerValueGet;

                        var lstReplyAnswers = new List<ReplyAnswers>
                        {
                            ReplyAnswers
                        };

                        var lstReply = new List<Reply>();
                        var reply = new Reply
                        {
                            Answers = new List<ReplyAnswers>()
                        };
                        reply.Answers = lstReplyAnswers;
                        if (reply != null)
                        {
                            lstReply.Add(reply);
                        }
                        bookingConfirmRq.Components.component.Replies.AddRange(lstReply);
                    }
                }
                //End :Questions and Answers
            }
            catch (Exception)
            {
                throw;
            }
            return Tuple.Create(bookingConfirmRq, prefixServiceCode);
        }

        protected override object TourCMSApiRequest<T>(T inputContext, out string inputRequest, out string inputResponse)
        {

            try
            {
                var inputTuple = inputContext as Tuple<NewBookingRequest, string>;
                var channelId =Convert.ToInt32(inputTuple?.Item2?.Split('_')[0]);
                var accountId = inputTuple?.Item2?.Split('_')[1];
                var requestXMl = SerializeDeSerializeHelper.SerializeXml(inputTuple?.Item1);
                var result = new StringContent(requestXMl, Encoding.UTF8, "application/xml");
                inputRequest =Convert.ToString(requestXMl);
                int marketplaceId = Convert.ToInt32(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.TourCMSMarketPlaceAccountID));
                var path = UriConstant.NewBooking;

                string verb = "POST";

                string tourCMSMarketPlaceAccountIDTestMode =
                Convert.ToString(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.TourCMSMarketPlaceAccountIDTestMode));
                string privateKey = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.TourCMSPrivateKey);
                if (tourCMSMarketPlaceAccountIDTestMode != "1")
                {
                    marketplaceId = Convert.ToInt32(accountId);
                }

                using (var httpClient = AddRequestHeadersAndAddressToApi(channelId, marketplaceId,
                   path, verb, privateKey))
                {
                    var requestMessage = new HttpRequestMessage(HttpMethod.Post, path);
                    var response = httpClient.PostAsync(path, result).GetAwaiter().GetResult();
                    byte[] buf = response.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();
                    string content = Encoding.UTF8.GetString(buf);
                    inputResponse = content;
                    return content;
                }
            }
            catch (Exception ex)
            {
                inputRequest = string.Empty;
                inputResponse = string.Empty;
                //ignored
                //#TODO Add logging here
                return null;
            }

        }

        protected override object GetResponseObject(string responseText)
        {
            return null;
        }

        protected override object GetResultsAsync(object input)
        {
            return null;
        }
    }
}