using Isango.Entities;
using Isango.Entities.Enums;
using Isango.Entities.Rezdy;
using Logger.Contract;
using ServiceAdapters.Rezdy.Constants;
using ServiceAdapters.Rezdy.Rezdy.Commands.Contracts;
using ServiceAdapters.Rezdy.Rezdy.Entities;
using ServiceAdapters.Rezdy.Rezdy.Entities.Booking;
using System.Globalization;
using System.Text;

using Util;

namespace ServiceAdapters.Rezdy.Rezdy.Commands
{
    public class CreateBookingCommandHandler : CommandHandlerBase, ICreateBookingCommandHandler
    {
        public CreateBookingCommandHandler(ILogger log) : base(log)
        {
        }

        protected override object RezdyApiRequest<T>(T inputContext)
        {
            var input = inputContext as BookingRequest;
            var methodPath = GenerateMethodPath();
            var content = new StringContent(SerializeDeSerializeHelper.Serialize(input), Encoding.UTF8, Constant.ApplicationJson);
            var result = _httpClient.PostAsync(methodPath, content);
            result.Wait();
            return ValidateApiResponse(result.Result);
        }

        protected override async Task<object> RezdyApiRequestAsync<T>(T inputContext)
        {
            return await Task.FromResult<object>(null);
        }

        protected override object CreateInputRequest<T>(T inputContext)
        {
            var supportPhoneNumber = Convert.ToString(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.SupportPhoneNumber));
            var supportEmail = Convert.ToString(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.BokunNotificationEmailAddressIsango));
            var rezdyProducts = new List<RezdyProduct>();
            var selectedProducts = inputContext as List<SelectedProduct>;
            var customers = selectedProducts.FirstOrDefault().ProductOptions.FirstOrDefault().Customers;
            var leadCustomer = customers.SingleOrDefault(x => x.IsLeadCustomer == true);

            selectedProducts.ForEach(x => rezdyProducts.Add(((RezdySelectedProduct)x).RezdyProduct));
            var bookingReference = ((RezdySelectedProduct)selectedProducts.FirstOrDefault())?.ReferenceNumber;
            var fieldsPerBooking = rezdyProducts.Select(x => x.BookingFields.Where(y => y.RequiredPerBooking == true).ToList());
            var totalAmount = Convert.ToString(Math.Round(selectedProducts.Sum(x => x.Price) + selectedProducts.Sum(x => x.DiscountedPrice) + selectedProducts.Sum(x => x.MultisaveDiscountedPrice), 2));
            var bookingRequest = new BookingRequest
            {
                SupplierId =Convert.ToInt32(rezdyProducts.FirstOrDefault().SupplierId),
                SendNotifications = false,
                ResellerComments = selectedProducts?.FirstOrDefault()?.SpecialRequest,
                ResellerReference = bookingReference,
                Customer = new Rezdy.Entities.Booking.BookingRequestCustomer
                {
                    FirstName = leadCustomer.FirstName,
                    LastName = leadCustomer.LastName,
                    Email = supportEmail,
                    Name = leadCustomer?.FirstName + " " + leadCustomer?.LastName,
                    Mobile = supportPhoneNumber,
                    CountryCode= selectedProducts?.FirstOrDefault()?.CountryCode

                },
                Items = new BookingRequestItem[selectedProducts.Count],
                Payments = new BookingRequestPayment[]
                {
                    new BookingRequestPayment(){
                    Currency = rezdyProducts.FirstOrDefault().Currency,
                    Amount =float.Parse(totalAmount, CultureInfo.InvariantCulture), //Price
                    Date = DateTime.Now.Date.ToString(CultureInfo.InvariantCulture),
                    Type = Constant.CreditCard,
                    Label = Constant.RezdyPaymentLabel,
                    Recipient=Constant.Reseller
                 },
              },
             TotalAmount= float.Parse(totalAmount, CultureInfo.InvariantCulture) //Price
            };

            var itemIndex = 0;
            

            foreach (var rezdyProduct in rezdyProducts)
            {
                var fieldIndex = 0;
                if (fieldsPerBooking.ToList() != null && fieldsPerBooking.ToList().Count > 0)
                {
                    if (itemIndex < fieldsPerBooking.ToList().Count)
                    {
                        bookingRequest.Fields = new BookingRequestField[fieldsPerBooking.ToList()[itemIndex].Count];
                    }
                }
                var fieldsPerparticipant = rezdyProduct.BookingFields.Where(y => y.VisiblePerParticipant == true || y.RequiredPerParticipant == true).ToList();
                var fieldsPerBookingForCurrentProduct = rezdyProduct.BookingFields.Where(y => y.RequiredPerBooking == true && y.RequiredPerParticipant == false).ToList();
                var selectedProduct = (RezdySelectedProduct)selectedProducts.FirstOrDefault(x => ((RezdySelectedProduct)x).ProductCode == rezdyProduct.ProductCode);
                var bookingQuestions = selectedProduct.BookingQuestions;
                var rezdyselectedProductOption = selectedProduct.ProductOptions.FirstOrDefault(y => y.IsSelected);

                var totalParticipantFields = rezdyselectedProductOption.TravelInfo.NoOfPassengers.Sum(x => x.Value);
                var amountItem = Convert.ToString(Math.Round(selectedProduct.Price + selectedProduct.DiscountedPrice + selectedProduct.MultisaveDiscountedPrice, 2));
                var item = new BookingRequestItem
                {
                    ProductCode = rezdyProduct.ProductCode,
                    Quantities = new BookingRequestQuantity[rezdyselectedProductOption.TravelInfo.NoOfPassengers.Count], // it is equal to count of passenger type
                    Participants = new BookingRequestParticipant[totalParticipantFields], // it is equal to sum of number of passenger type
                    StartTimeLocal = selectedProduct.StartTimeLocal.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"),
                    PickupLocation = new BookingRequestPickupLocation(),
                    ProductName = rezdyProduct.Name,
                    TotalQuantity = totalParticipantFields,
                    //Subtotal is the BookingItem.amount plus extras costs plus taxes and fees 
                    //(but currently , we have no extras)
                    SubTotal = float.Parse(amountItem, CultureInfo.InvariantCulture), //Price
                    //Amount charged for this BookingItem
                    Amount = float.Parse(amountItem, CultureInfo.InvariantCulture), //Price

                };
                //Supplier said again that: revert this code, commented it.
                //if datetime have no time, then remove time and don't pass 
                //otherwise gives error in freesell products scenarios
                //if (selectedProduct?.StartTimeLocal.ToLocalTime().TimeOfDay.TotalSeconds == 0)
                //{
                    //item.StartTimeLocal = selectedProduct.StartTimeLocal.ToLocalTime().ToString("yyyy-MM-dd");
                //}

                if (selectedProduct.RezdyPickUpLocation != null)
                {
                    var bookingRequestPickupLocation = new BookingRequestPickupLocation
                    {
                        LocationName = selectedProduct.RezdyPickUpLocation.LocationName,
                        Longitude = selectedProduct.RezdyPickUpLocation.Longitude,
                        Latitude = selectedProduct.RezdyPickUpLocation.Latitude,
                        MinutesPrior = selectedProduct.RezdyPickUpLocation.MinutesPrior,
                        Address = selectedProduct.RezdyPickUpLocation.Address,
                        AdditionalInstructions = selectedProduct.RezdyPickUpLocation.AdditionalInstructions,
                        PickupTime = selectedProduct.StartTimeLocal.ToLocalTime()
                            .AddMinutes(selectedProduct.RezdyPickUpLocation.MinutesPrior * -1)
                            .ToString("yyyy-MM-dd HH:mm:ss")
                    };

                    item.PickupLocation = bookingRequestPickupLocation;
                }

                var quantityCounter = 0;
                var participantCounter = 0;

                foreach (var passenger in rezdyselectedProductOption.TravelInfo.NoOfPassengers)
                {
                    var optionId = selectedProduct.ProductOptions.FirstOrDefault()?.Id;
                    var ageGroupCode = string.Empty;

                    if (rezdyProduct.PriceOptions[0].MinQuantity > 0 && rezdyProduct.PriceOptions[0].MaxQuantity > 0)
                    {
                        var passengerCount = rezdyselectedProductOption.TravelInfo.NoOfPassengers.Sum(x => x.Value);
                        //Group having Child also condition
                        var groupHaveChildAlso = rezdyselectedProductOption.TravelInfo.NoOfPassengers?.FirstOrDefault(x => x.Key.Equals(PassengerType.Child)).Value;
                        if (Convert.ToString(passenger.Key)?.ToLowerInvariant() == "adult" && groupHaveChildAlso>0)
                        {
                            if (passengerCount > groupHaveChildAlso)
                            {
                                passengerCount = passengerCount - Convert.ToInt32(groupHaveChildAlso);
                            }
                        }
                        ageGroupCode = rezdyProduct.PriceOptions.FirstOrDefault(x =>
                           x.MinQuantity <= passengerCount && x.MaxQuantity >= passengerCount)?.Label.ToString();

                        if (Convert.ToString(passenger.Key)?.ToLowerInvariant() != "adult")
                        {
                            ageGroupCode = selectedProduct.PaxMappings
                           .FirstOrDefault(x => x.ServiceOptionId == optionId && x.PassengerType == passenger.Key)
                           ?.AgeGroupCode;
                        }
                    }
                    else
                    {
                        ageGroupCode = selectedProduct.PaxMappings
                            .FirstOrDefault(x => x.ServiceOptionId == optionId && x.PassengerType == passenger.Key)
                            ?.AgeGroupCode;
                    }

                    var quantityAll = rezdyProduct.PriceOptions.FirstOrDefault(x => x.Label.ToLowerInvariant() == ageGroupCode.ToLowerInvariant()
                    & x.ProductCode== rezdyProduct.ProductCode);

                    var quantityLabel = quantityAll?.Label;
                    var quantityPrice = quantityAll?.Price;

                    var quantity = new BookingRequestQuantity
                    {
                        OptionLabel = quantityLabel,
                        Value =Convert.ToInt32(passenger.Value),
                        OptionPrice= float.Parse(quantityPrice, CultureInfo.InvariantCulture) //Price
                    };

                    item.Quantities[quantityCounter] = quantity;
                    quantityCounter += 1;

                    for (var i = 0; i < passenger.Value; i++)
                    {
                        var fieldsCounter = 0;
                        var participant = new BookingRequestParticipant
                        {
                            Fields = new BookingRequestField[fieldsPerparticipant.Count]
                        };

                        var rezdyFields = selectedProduct.RezdyLabelDetails.ToList();

                        foreach (var perParticipant in fieldsPerparticipant)
                        {
                            var rezdyField = rezdyFields.FirstOrDefault(x => x.Label == perParticipant.Label);
                            if (rezdyField != null)
                            {
                                var participantField = new BookingRequestField();
                                if (rezdyField.Label == Constant.Mobile)
                                {
                                    participantField.Label = rezdyField.Label;
                                    participantField.Value = supportPhoneNumber;
                                }
                                else
                                {
                                    participantField.Label = perParticipant.Label;
                                    participantField.Value = Convert.ToString(GetPropertyValue(selectedProduct, rezdyField.Value, i));
                                }

                                rezdyFields.Remove(rezdyField);

                                participant.Fields[fieldsCounter] = participantField;
                                fieldsCounter += 1;
                            }
                            else if (bookingQuestions.Any(x => x.Question.Contains(perParticipant.Label)))
                            {
                                var bookingQuestion = bookingQuestions?.FirstOrDefault(x => x.Question == perParticipant.Label);
                                if (bookingQuestion != null)
                                {
                                    var participantField = new BookingRequestField
                                    {
                                        Label = bookingQuestion.Question,
                                        Value = bookingQuestion.Answers.FirstOrDefault()
                                    };
                                    bookingQuestions.Remove(bookingQuestion);

                                    participant.Fields[fieldsCounter] = participantField;
                                    fieldsCounter += 1;
                                }
                            }
                        }
                        item.Participants[participantCounter] = participant;
                        participantCounter += 1;
                    }
                }

                bookingRequest.Items[itemIndex] = item;
                itemIndex += 1;

                var unmappedFields = selectedProduct.RezdyLabelDetails.ToList()?.Where(x=>x!=null)?.Select(x => x.Label).Intersect(item.Participants[0].Fields.ToList()?.Where(x => x != null)?.Select(x => x.Label));

                foreach (var perBookingField in fieldsPerBookingForCurrentProduct)
                {
                    var rezdyFields = selectedProduct.RezdyLabelDetails.ToList();

                    if (!unmappedFields.Contains(perBookingField.Label))
                    {
                        var rezdyField = rezdyFields.FirstOrDefault(x => x.Label == perBookingField.Label);

                        if (rezdyField != null)
                        {
                            var field = new BookingRequestField();
                            if (rezdyField.Label == Constant.Mobile)
                            {
                                field.Label = perBookingField.Label;
                                field.Value = supportPhoneNumber;
                            }
                            else
                            {
                                field.Label = perBookingField.Label;
                                field.Value = Convert.ToString(GetPropertyValue(selectedProduct, rezdyField.Value, 0));
                            }

                            bookingRequest.Fields[fieldIndex] = field;
                            fieldIndex += 1;
                        }
                        else if (bookingQuestions.Any(x => x.Question.Contains(perBookingField.Label)))
                        {
                            var bookingQuestion = bookingQuestions.FirstOrDefault(x => x.Question == perBookingField.Label);
                            if (bookingQuestion != null)
                            {
                                var field = new BookingRequestField
                                {
                                    Label = bookingQuestion.Question,
                                    Value = bookingQuestion.Answers.FirstOrDefault()
                                };
                                bookingQuestions.Remove(bookingQuestion);

                                bookingRequest.Fields[fieldIndex] = field;
                                fieldIndex += 1;
                            }
                        }
                    }
                }
            }

            foreach (var item in bookingRequest.Items)
            {
                foreach (var participant in item.Participants)
                {
                    participant.Fields = participant?.Fields.Where(x => x != null).ToArray();
                }
            }

            bookingRequest.Fields = bookingRequest.Fields.ToList().Where(x => x != null).ToArray();

            return bookingRequest;
        }

        private string GenerateMethodPath()
        {
            return UriConstants.CreateBooking;
        }

        private static object GetPropertyValue(object src, string propName, int index)
        {
            if (src == null || propName == null)
                return string.Empty;

            if (propName.Contains("."))
            {
                var temp = propName.Split(new char[] { '.' }, 2);
                return GetPropertyValue(GetPropertyValue(src, temp[0], index), temp[1], index);
            }
            else
            {
                var prop = src.GetType().GetProperty(propName);

                if (prop.PropertyType.IsGenericType)
                {
                    System.Collections.IList a = (System.Collections.IList)prop.GetValue(src, null);
                    if (a.Count == 1)
                    {
                        return a[0];
                    }
                    return a[index];
                }
                return prop?.GetValue(src, null);
            }
        }
    }
}