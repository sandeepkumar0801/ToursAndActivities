using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.HotelBeds;
using Logger.Contract;
using ServiceAdapters.HB.Constants;
using ServiceAdapters.HB.HB.Commands.Contract;
using ServiceAdapters.HB.HB.Entities.Booking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Util;
using BookingDetail = ServiceAdapters.HB.HB.Entities.Booking.BookingDetail;

using HbAPIBooking = ServiceAdapters.HB.HB.Entities.Booking;

using IsangoBooking = Isango.Entities.Booking;

namespace ServiceAdapters.HB.HB.Commands
{
    public class HBBookingConfirmCmdHandler : CommandHandlerBase, IHbBookingConfirmCmdHandler
    {
        public HBBookingConfirmCmdHandler(ILogger iLog) : base(iLog)
        {
        }

        //protected override object CreateInputRequest(InputContext inputContext)
        //{
        //    var bookingConfirmRq = new Booking.BookingConfirmRq(inputContext.Language, inputContext.ClientReference, inputContext.Holder, inputContext.LstBookingActivities);
        //    return bookingConfirmRq;
        //}

        protected override object CreateInputRequest<T>(T bookingContext)
        {
            var bookingConfirmRq = new BookingConfirmRq();
            try
            {
                var booking = bookingContext as IsangoBooking.Booking;

                var hbSelectdProducts = booking.SelectedProducts
                    .Where(p => p.APIType == Isango.Entities.Enums.APIType.Hotelbeds
                    && p.ProductOptions.FirstOrDefault(y => y.Id == booking.BookingId) != null
                    )
                    .Cast<HotelBedsSelectedProduct>()
                    .ToList();

                if (!(hbSelectdProducts?.Count > 0))
                {
                    return null;
                }

                var leadCustomer = hbSelectdProducts?.FirstOrDefault()?.ProductOptions?
                    .Find(f => f.IsSelected.Equals(true))
                    .Customers.Find(f => f.IsLeadCustomer.Equals(true));

                var customerName = leadCustomer?.FirstName.Contains(" ") == true ?
                    leadCustomer?.FirstName?.Split(' ')?.ToList() :
                    new List<string> { { leadCustomer.FirstName }, { leadCustomer.LastName } };

                var lName = customerName.LastOrDefault() ?? "NA";
                var fName = customerName.FirstOrDefault() ?? "NA";//string.Join(" ", customerName.Where(x => x != lName).ToArray());

                //Pending  Name Surname Strategy either in booking or Customer
                var holder = new Holder
                {
                    Name = fName,
                    Surname = lName,// ?? "NA",
                    Email = booking.VoucherEmailAddress,
                    Telephones = new List<string> { booking.VoucherPhoneNumber },
                    Title = leadCustomer?.Title ?? Constant.AnswerPaxTitleDefault
                };

                bookingConfirmRq.Holder = holder;
                bookingConfirmRq.Activities = new List<HbAPIBooking.Activity>();
                var lang = string.Empty;

                foreach (var selectedProduct in hbSelectdProducts)
                {
                    lang = booking?.Language?.Code ?? "en";
                    var selectedOptions = selectedProduct?.ProductOptions
                        ?.FindAll(f => f.IsSelected.Equals(true))
                        ?.Cast<ActivityOption>().ToList();

                    if (!(selectedOptions?.Count > 0))
                    {
                        return null;
                    }

                    foreach (var selectedOption in selectedOptions)
                    {
                        var activity = new HbAPIBooking.Activity
                        {
                            PreferedLanguage = lang, //
                            ServiceLanguage = lang
                        };

                        var answers = new List<Answer>();
                        if (selectedProduct?.ContractQuestions?.Count > 0)
                        {
                            selectedOption.ContractQuestions = selectedProduct?.ContractQuestions;
                            activity.Answers = GetFilledAnswers(booking, selectedOption, leadCustomer);
                        }
                        if (string.IsNullOrWhiteSpace(selectedOption?.RateKey))
                        {
                            throw new Exception("Rate key cannot be null for hotel-bed apitude api bookingConfirm request.");
                        }
                        activity.RateKey = selectedOption.RateKey;
                        activity.Session = null;
                        activity.From = selectedOption.TravelInfo.StartDate.ToString(Constant.DateInyyyyMMdd);

                        int toDate = 0;
                        if (selectedOption?.TravelInfo?.NumberOfNights > 0)
                        {
                            toDate = Convert.ToInt32(selectedOption?.TravelInfo?.NumberOfNights);
                        }
                        activity.To = selectedOption.TravelInfo.StartDate.AddDays(toDate).ToString(Constant.DateInyyyyMMdd);

                        activity.Paxes = GetPaxes(leadCustomer, selectedOption.Customers);

                        bookingConfirmRq.Activities.Add(activity);

                        bookingConfirmRq.ClientReference = $"{booking.ReferenceNumber}-{selectedOption.ServiceOptionId}";
                    }
                }
                bookingConfirmRq.Language = lang;
            }
            catch (Exception)
            {
                throw;
            }

            return bookingConfirmRq;
        }

        protected override object GetResponseObject(string responseText)
        {
            var result = default(BookingDetail.BookingDetailRs);
            try
            {
                result = SerializeDeSerializeHelper.DeSerialize<BookingDetail.BookingDetailRs>(responseText);
            }
            catch (Exception ex)
            {
                //ignored
                //#TODO Add logging here
                result = null;
            }
            return result;
        }

        protected override async Task<object> GetResultsAsync(object input)
        {
            var url = string.Format($"{_hotelBedsApitudeServiceURL}{Constant.BookingConfirm}");
            var response = await GetResponseFromAPIEndPoint(input, url, "PUT");
            return response;
        }

        #region Private Methods

        /// <summary>
        /// Get List of Answers for the selected option.
        /// </summary>
        /// <param name="booking"></param>
        /// <param name="selectedOption"></param>
        /// <param name="selCustomer"></param>
        /// <returns>List of Answers for the selected option</returns>
        private List<Answer> GetFilledAnswers(Isango.Entities.Booking.Booking booking, ActivityOption selectedOption, Customer selCustomer)
        {
            var answers = new List<Answer>();
            selectedOption.ContractQuestions.ForEach(contractQuestion =>
            {
                var code = contractQuestion?.Code?.ToUpper();
                switch (code)
                {
                    case Constant.QuestionPaxName:
                        {
                            contractQuestion.Answer = $"{ selCustomer.FirstName} {selCustomer.LastName ?? "NA"}";
                            break;
                        }
                    case Constant.QuestionPhoneNumber:
                        {
                            contractQuestion.Answer = booking.VoucherPhoneNumber;
                            break;
                        }
                    case Constant.QuestionGender:
                        {
                            contractQuestion.Answer = string.IsNullOrWhiteSpace(contractQuestion?.Answer) ? Constant.AnswerGenderDefault : contractQuestion?.Answer;
                            break;
                        }
                    case Constant.QuestionPaxTitle:
                        {
                            contractQuestion.Answer = string.IsNullOrWhiteSpace(contractQuestion?.Answer) ? Constant.AnswerPaxTitleDefault : contractQuestion?.Answer;
                            break;
                        }

                    default:
                        {
                            if (string.IsNullOrWhiteSpace(contractQuestion?.Answer))
                            {
                                contractQuestion.Answer = Constant.AnswerNotAvailable;
                            }
                            break;
                        }
                }
                var answer = new Answer();
                var question = new HbAPIBooking.Question
                {
                    Code = contractQuestion.Code,
                    Text = contractQuestion.Name,
                    Required = contractQuestion.IsRequired
                };

                answer.Question = question;
                answer.Answers = contractQuestion.Answer;
                answers.Add(answer);
            });
            return answers;
        }

        /// <summary>
        /// Get pax types. Paxes must be grouped by type, and all ADULTS must be informed before all CHILD.
        /// </summary>
        /// <param name="leadCustomer"></param>
        /// <param name="customers"></param>
        /// <returns></returns>
        private List<Pax> GetPaxes(Customer leadCustomer, List<Customer> customers)
        {
            var paxes = new List<Pax>();
            try
            {
                var leadPax = new Pax
                {
                    Age = leadCustomer.Age,
                    Name = leadCustomer.FirstName,
                    Surname = leadCustomer.LastName,
                    Type = leadCustomer.PassengerType == Isango.Entities.Enums.PassengerType.Undefined ?
                         Isango.Entities.Enums.PassengerType.Adult.ToString()
                        : leadCustomer.PassengerType.ToString().ToUpper()
                };
                paxes.Add(leadPax);

                var paxesReamining = customers?.Where(x => !x.IsLeadCustomer)
                    ?.Select(c => new Pax
                    {
                        Age = c.Age,
                        Name = c.FirstName,
                        Surname = c.LastName,
                        Type = c.PassengerType.ToString().ToUpper()
                    })?.OrderBy(p => p.Type).ToList();
                paxes.AddRange(paxesReamining);
            }
            catch (Exception)
            {
                throw;
            }

            return paxes;
        }

        #endregion Private Methods
    }
}