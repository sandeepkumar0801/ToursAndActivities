using Isango.Entities.Tiqets;
using Logger.Contract;
using Newtonsoft.Json.Linq;
using ServiceAdapters.Tiqets.Constants;
using ServiceAdapters.Tiqets.Tiqets.Commands.Contracts;
using ServiceAdapters.Tiqets.Tiqets.Entities;
using ServiceAdapters.Tiqets.Tiqets.Entities.RequestResponseModels;
using System.Text;
using Util;
using BookingRequest = Isango.Entities.Tiqets.BookingRequest;

namespace ServiceAdapters.Tiqets.Tiqets.Commands
{
    public class CreateOrderCommandHandler : CommandHandlerBase, ICreateOrderCommandHandler
    {
        private string _supportEmail = "support@isango.com";
        private string _supportPhoneNumber = "+4402033551240";
        private string _citySightSeeingDefaultEmailForCreateBooking = "no-reply@city-sightseeing.com";
        private string _citySightSeeingDefaultAffiliateID = "58C11104-34E6-47BA-926D-E89E4242B962";

        public CreateOrderCommandHandler(ILogger log) : base(log)
        {
            try
            {
                _supportPhoneNumber = ConfigurationManagerHelper.GetValuefromAppSettings("SupportPhoneNumer");
                _supportEmail = ConfigurationManagerHelper.GetValuefromAppSettings("mailfrom");
                _citySightSeeingDefaultEmailForCreateBooking = ConfigurationManagerHelper.GetValuefromAppSettings("CitySightSeeingDefaultEmailForCreateBooking");
                _citySightSeeingDefaultAffiliateID = ConfigurationManagerHelper.GetValuefromAppSettings("CitySightSeeingAffiliateID");
            }
            catch
            {
                //Ignored //Default values are give above
            }
        }

        protected override object TiqetsBookingApiRequest<T>(T inputContext)
        {
            var bookingRequest = inputContext as BookingRequest;
            var createOrderRequest = bookingRequest?.RequestObject as CreateOrderRequest;
            var languageCode = bookingRequest?.TiquetsLanguageCode;
            if (!string.IsNullOrWhiteSpace(createOrderRequest?.TimeSlot))
            {
                createOrderRequest.TimeSlot = createOrderRequest.TimeSlot.Substring(0, 5);
            }
            createOrderRequest.external_reference = bookingRequest.IsangoBookingReference;
            if(bookingRequest?.AffiliateId?.ToLower() == _citySightSeeingDefaultAffiliateID?.ToLower())
            {
                if(!string.IsNullOrEmpty(createOrderRequest?.CustomerDetails?.Email))
                {
                    createOrderRequest.CustomerDetails.Email = _citySightSeeingDefaultEmailForCreateBooking;
                }
            }
            var serializedRequestData = SerializeDeSerializeHelper.SerializeWithContractResolver(createOrderRequest);
            var serializedRequest = string.Empty;
            if (serializedRequestData.Contains("passport_ids") || serializedRequestData.Contains("nationality"))
            {
                var removeEntities = JObject.Parse(serializedRequestData);
                removeEntities.Descendants()
                .OfType<JProperty>()
                .Where(attr => (attr.Name == "passport_ids" && attr.Value.ToString() == "") || (attr.Name == "nationality" && attr.Value.ToString() == ""))
                .ToList() // you should call ToList because you're about to changing the result, which is not possible if it is IEnumerable
                .ForEach(attr => attr.Remove()); // removing unwanted attributes

                serializedRequest = removeEntities.ToString();
            }
            else
            {
                serializedRequest = serializedRequestData;
            }
            var signedPayload = GetSignedPayload(serializedRequest, bookingRequest?.AffiliateId);
            var content = new StringContent(signedPayload, Encoding.UTF8, Constant.ApplicationOrJson);
            var url = FormUrl(languageCode);
            using (var httpClient = AddRequestHeadersAndAddressToApi(bookingRequest?.AffiliateId))
            {
                var result = httpClient.PostAsync(url, content);
                result.Wait();
                return result?.GetAwaiter().GetResult();
            }
        }

        protected override object CreateInputRequest<T>(T inputContext)
        {
            var bookingRequest = inputContext as BookingRequest;
            if (bookingRequest?.RequestObject is TiqetsSelectedProduct selectedProduct)
            {
                if (selectedProduct.TimeSlot == "00:00:00")
                {
                    selectedProduct.TimeSlot = string.Empty;
                }
                var productOption = string.IsNullOrEmpty(selectedProduct.TimeSlot) ? selectedProduct.ProductOptions.First() : selectedProduct.ProductOptions.FirstOrDefault(x => x.StartTime.ToString().Substring(0, 5).Trim() == selectedProduct.TimeSlot.Substring(0, 5).Trim());

                var customer = productOption?.Customers?.FirstOrDefault(x => x.IsLeadCustomer) ??
                    productOption?.Customers?.FirstOrDefault() ??
                    selectedProduct.ProductOptions.First().Customers.First();

                var infants = selectedProduct.Variants.FirstOrDefault(x => x.Id == 0);
                selectedProduct.Variants.Remove(infants); //Remove Infant as we are setting its value explicitly in the check availability call, no need to pass it in Create Order call

                var createOrderRequest = new CreateOrderRequest
                {
                    ProductId = selectedProduct.FactSheetId,
                    TimeSlot = selectedProduct.TimeSlot ?? string.Empty,
                    Day = productOption?.TravelInfo.StartDate.Date.ToString("yyyy-MM-dd"),
                    CustomerDetails = new CustomerDetails
                    {
                        FirstName = customer.FirstName,
                        LastName = customer.LastName,
                        Email = _supportEmail,
                        PhoneNumber = _supportPhoneNumber
                    },
                    Variants = selectedProduct.Variants
                };
                //Start-Required FullName in Some Cases
                var dataVisitorsDetails = FullNameData(selectedProduct, productOption?.Customers);
                //var dataVisitorsDetails = ContractQuestions(selectedProduct);
                createOrderRequest.VisitorsDetails = dataVisitorsDetails;

                //End -Required FullName in Some Cases
                /*
                if(string.IsNullOrWhiteSpace(createOrderRequest.external_reference))
                {
                    createOrderRequest.external_reference = bookingRequest.IsangoBookingReference;
                }
                */
                bookingRequest.RequestObject = createOrderRequest;
                return bookingRequest;
            }

            return null;
        }

        private List<VisitorsDetails> FullNameData(
            TiqetsSelectedProduct selectedProduct,
            List<Isango.Entities.Customer> customers)
        {
           
            var visitorsDetailsLst = new List<VisitorsDetails>();
            var requiresVisitorsDetails = selectedProduct.RequiresVisitorsDetails;
            if (requiresVisitorsDetails != null && requiresVisitorsDetails.Count > 0)
            {
                var checkStringFullName = "full_name";
                var checkPassportids = "passport_ids";
                var checkNationality = "nationality";

                var matchedFullName = requiresVisitorsDetails?.FirstOrDefault(stringToCheck => stringToCheck.Contains(checkStringFullName));
                var matchedPassportIds = requiresVisitorsDetails?.FirstOrDefault(stringToCheck => stringToCheck.Contains(checkPassportids));
                var matchedNationality= requiresVisitorsDetails?.FirstOrDefault(stringToCheck => stringToCheck.Contains(checkNationality));

                //if full_name macthed
                if (matchedFullName != null)
                {
                    int i = 0;
                    var sumofVariantsCount = selectedProduct?.Variants?.Sum(x => x.Count);
                    foreach (var itemVariant in selectedProduct.Variants)
                    {
                        var visitorsDataLst = new List<VisitorsData>();
                        var dataCount = itemVariant.Count +i;
                        //Assign Customer Name
                        for (; i < dataCount; i++)
                        {
                            //if customer match the total count
                            if (sumofVariantsCount == customers.Count)
                            {
                                var visitorsData = new VisitorsData();
                                var fullName = customers[i]?.FirstName + " " + customers[i]?.LastName;
                                visitorsData.FullName = fullName;
                                if (fullName.Any(char.IsDigit))
                                {
                                    visitorsData.FullName = customers[0]?.FirstName + " " + customers[0]?.LastName;
                                }

                                if (matchedPassportIds != null)
                                {
                                    visitorsData.PassportNumber = customers[i]?.PassportNumber;
                                }
                                if (matchedNationality != null)
                                {
                                    visitorsData.PassportNationality = customers[i]?.PassportNationality;
                                }

                                visitorsDataLst.Add(visitorsData);
                            }
                            else //if ispaxdetail required =false, then only one customer
                            {
                                var visitorsData = new VisitorsData
                                {
                                    FullName = customers[0]?.FirstName + " " + customers[0]?.LastName,
                                };
                                if (matchedPassportIds != null)
                                {
                                    visitorsData.PassportNumber = customers[0]?.PassportNumber;
                                }
                                if (matchedNationality != null)
                                {
                                    visitorsData.PassportNationality = customers[0]?.PassportNationality;
                                }
                                visitorsDataLst.Add(visitorsData);
                            }

                        }
                        //Assing VariantId + Customer Name
                        var visitorsDetails = new VisitorsDetails
                        {
                            VariantId = itemVariant.Id,
                            VisitorsData = visitorsDataLst
                        };
                        visitorsDetailsLst.Add(visitorsDetails);
                    }
                }
            }
            return visitorsDetailsLst;
        }

       

        private string FormUrl(string languageCode)
        {
            languageCode = new[] { "ca", "cs", "da", "de", "el", "en", "es", "fr", "it", "ja", "ko", "nl", "pl", "pt", "ru", "sv", "zh" }.Contains(languageCode) ? languageCode : "en";

            return $"{UriConstant.CreateOrder}{languageCode}";
        }
    }
}