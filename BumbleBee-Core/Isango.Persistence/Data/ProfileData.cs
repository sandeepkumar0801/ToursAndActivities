using Isango.Entities;
using Isango.Entities.Enums;
using Isango.Entities.MyIsango;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using Util;

namespace Isango.Persistence.Data
{
    public class ProfileData
    {
        public ISangoUser GetUserInfo(IDataReader reader)
        {
            var userInfo = new ISangoUser();
            while (reader.Read())
            {
                userInfo.FirstName = DbPropertyHelper
                    .StringDefaultPropertyFromRow(reader, "smcpassengerfirstname").Trim();
                userInfo.LastName = DbPropertyHelper
                    .StringDefaultPropertyFromRow(reader, "smcpassengerlastname").Trim();
                userInfo.EmailAddress = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "loginid")
                    .Trim();
                userInfo.Password = DbPropertyHelper
                    .StringDefaultPropertyFromRow(reader, "loginpassword").Trim();
                userInfo.PhoneNumber = DbPropertyHelper
                    .StringDefaultPropertyFromRow(reader, "addresstelephonenumber").Trim();
            }

            return userInfo;
        }

        public void LoadUserEmailPrefQuestion(IDataReader reader,
            ref List<MyUserEmailPreferences> userEmailPreferenceList)
        {
            if (userEmailPreferenceList == null)
            {
                userEmailPreferenceList = new List<MyUserEmailPreferences>();
            }

            while (reader.Read())
            {
                if (userEmailPreferenceList.Count > 0)
                {
                    var alreadyExistingQuestion = userEmailPreferenceList.Find(question =>
                        question.QuestionOrder ==
                        DbPropertyHelper.Int32PropertyFromRow(reader, "QuestionOrder"));
                    if (alreadyExistingQuestion != null)
                    {
                        var myUserAnswer = new MyUserAnswer
                        {
                            AnswerId = DbPropertyHelper.Int32PropertyFromRow(reader, "answerid"),
                            AnswerText =
                                DbPropertyHelper.StringPropertyFromRowWithTrim(reader, "answerText"),
                            AnswerOrder = DbPropertyHelper.Int32PropertyFromRow(reader, "AnswerOrder")
                        };

                        alreadyExistingQuestion.MyUserAnswers.Add(myUserAnswer);
                    }
                    else
                    {
                        var userEmailPrefPerQuestion = GetUserEmailPref(reader);
                        userEmailPreferenceList.Add(userEmailPrefPerQuestion);
                    }
                }
                else
                {
                    var userEmailPrefPerQuestion = GetUserEmailPref(reader);
                    userEmailPreferenceList.Add(userEmailPrefPerQuestion);
                }
            }
        }

        public void LoadUserPrefAnswer(IDataReader reader, ref List<MyUserEmailPreferences> userEmailPreferenceList)
        {
            reader.NextResult();
            //HardCoded
            var recordCount = 1;
            while (reader.Read())
            {
                if (userEmailPreferenceList.Count > 0)
                {
                    var questionAgainstChosenAnswer = userEmailPreferenceList.Find(desiredQuestion =>
                        desiredQuestion.QuestionOrder == recordCount);
                    questionAgainstChosenAnswer.UserPreferredAnswer =
                        DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "EmailPrefAnswerId");
                }

                recordCount++;
            }
        }

        public void LoadUserBookingDetails(IDataReader reader, ref List<MyBookingSummary> userBookingDetailsList,
            bool isAgent)
        {
            if (userBookingDetailsList == null)
            {
                userBookingDetailsList = new List<MyBookingSummary>();
            }
            while (reader.Read())
            {
                var userBookingDetails = new MyBookingSummary
                {
                    BookingId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "bookingid"),
                    BookingRefenceNumber = DbPropertyHelper
                        .StringDefaultPropertyFromRow(reader, "bookingRefNo").Trim()
                };
                if (!string.IsNullOrEmpty(DbPropertyHelper.StringPropertyFromRowWithTrim(reader, "bookingDate")))
                {
                    userBookingDetails.BookingDate =
                        DbPropertyHelper.DateTimePropertyFromRow(reader, "bookingDate");
                    if (!isAgent)
                    {
                        userBookingDetails.GetBookingDate = DateTime
                            .Parse(DbPropertyHelper.StringPropertyFromRowWithTrim(reader, "bookingDate"),
                                CultureInfo.InvariantCulture).ToString("dd/M/yyyy");
                    }
                }

                userBookingDetails.BookingAmountCurrency = DbPropertyHelper
                    .StringDefaultPropertyFromRow(reader, "CurrencyShortSymbol").Trim();

                userBookingDetailsList.Add(userBookingDetails);
            }
        }

        public void LoadBookedProductDetails(IDataReader reader, ref List<MyBookingSummary> userBookingDetailsList,
            bool isAgent)
        {
            reader.NextResult();
            while (reader.Read())
            {
                var bookingFoundAgainstCurrentBookingId = userBookingDetailsList.Find(thisBooking =>
                    thisBooking.BookingId ==
                    DbPropertyHelper.Int32PropertyFromRow(reader, "bookingid"));
                if (bookingFoundAgainstCurrentBookingId != null)
                {
                    if (bookingFoundAgainstCurrentBookingId.BookedProducts == null)
                    {
                        bookingFoundAgainstCurrentBookingId.BookedProducts =
                            new List<MyBookedProduct>();
                    }

                    var bookedProductDetails = new MyBookedProduct
                    {
                        BookedProductName = DbPropertyHelper
                            .StringDefaultPropertyFromRow(reader, "servicename").Trim()
                    };

                    if (!string.IsNullOrEmpty(
                        DbPropertyHelper.StringPropertyFromRowWithTrim(reader, "travelDate")))
                    {
                        bookedProductDetails.TravelDate =
                            DbPropertyHelper.DateTimePropertyFromRow(reader, "travelDate");
                        if (!isAgent)
                        {
                            bookedProductDetails.GetTravelDate = DateTime
                            .Parse(DbPropertyHelper.StringPropertyFromRowWithTrim(reader, "travelDate"),
                                CultureInfo.InvariantCulture).ToString("dd/M/yyyy");
                        }
                    }

                    if (!string.IsNullOrEmpty(
                        DbPropertyHelper.StringPropertyFromRowWithTrim(reader, "amountonwirecard")))
                    {
                        bookedProductDetails.BookingAmountPaid =
                            DbPropertyHelper.DecimalPropertyFromRow(reader, "amountonwirecard");
                    }

                    if (!string.IsNullOrEmpty(
                        DbPropertyHelper.StringPropertyFromRowWithTrim(reader, "bookedoptionstatusid")))
                    {
                        var statusOfBookedProduct =
                            DbPropertyHelper.Int32PropertyFromRow(reader, "bookedoptionstatusid");
                        if (statusOfBookedProduct == 2)
                            bookedProductDetails.BookingStatus = "Confirmed";
                        else if (statusOfBookedProduct == 1)
                            bookedProductDetails.BookingStatus = "On Request";
                        else if (statusOfBookedProduct == 3)
                            bookedProductDetails.BookingStatus = "Cancelled";
                        else
                            bookedProductDetails.BookingStatus = "UnKnown";
                    }

                    bookedProductDetails.NoOfAdults =
                        DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "AdultCount");
                    bookedProductDetails.NoOfChildren =
                        DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "childcount");

                    bookedProductDetails.TicketDetail = DbPropertyHelper
                           .StringDefaultPropertyFromRow(reader, "servicetypeoptionname").Trim();

                    if (!string.IsNullOrEmpty(
                       DbPropertyHelper.StringPropertyFromRowWithTrim(reader, "Amount_Before_Discount")))
                    {
                        bookedProductDetails.AmountBeforeDiscount =
                            DbPropertyHelper.DecimalPropertyFromRow(reader, "Amount_Before_Discount");
                    }

                    bookedProductDetails.ServiceId =
                       DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "ServiceId");

                    bookedProductDetails.BookedOptionId =
                      DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "BookedOptionId");

                    bookedProductDetails.IsReceipt = DbPropertyHelper.BoolDefaultPropertyFromRow(reader, "IsReceipt");

                    bookingFoundAgainstCurrentBookingId.BookedProducts.Add(bookedProductDetails);
                }
            }
        }

        public void LoadBookedPaxProductPrice(IDataReader reader, ref List<MyBookingSummary> userBookingDetailsList,
            bool isAgent)
        {
            var bookingFoundAgainstCurrentBookingId = new MyBookingSummary();
            reader.NextResult();
            var myPaxPriceInfoList = new List<MyPaxPriceInfo>();
            while (reader.Read())
            {
                var bookingid = DbPropertyHelper.Int32PropertyFromRow(reader, "bookingid");

                bookingFoundAgainstCurrentBookingId = userBookingDetailsList.Find
                  (thisBooking => thisBooking.BookingId == bookingid);

                if (bookingFoundAgainstCurrentBookingId != null)
                {
                    if (bookingFoundAgainstCurrentBookingId.PaxPriceInfo == null)
                    {
                        bookingFoundAgainstCurrentBookingId.PaxPriceInfo =
                            new List<MyPaxPriceInfo>();
                    }

                    var bookedProductDetails = new MyPaxPriceInfo
                    {
                        Subject = DbPropertyHelper
                            .StringDefaultPropertyFromRow(reader, "subject").Trim(),
                        PassengerCount = DbPropertyHelper
                            .Int32DefaultPropertyFromRow(reader, "PassengerCount")
                    };

                    if (!string.IsNullOrEmpty(
                        DbPropertyHelper.StringPropertyFromRowWithTrim(reader, "BookedPassengerRateSellAmount")))
                    {
                        bookedProductDetails.BookedPassengerRateSellAmount =
                            DbPropertyHelper.DecimalPropertyFromRow(reader, "BookedPassengerRateSellAmount");
                    }

                    if (!string.IsNullOrEmpty(
                        DbPropertyHelper.StringPropertyFromRowWithTrim(reader, "BookedPassengerRateOriginalSellAmount")))
                    {
                        bookedProductDetails.BookedPassengerRateOriginalSellAmount =
                            DbPropertyHelper.DecimalPropertyFromRow(reader, "BookedPassengerRateOriginalSellAmount");
                    }

                    if (!string.IsNullOrEmpty(
                       DbPropertyHelper.StringPropertyFromRowWithTrim(reader, "PassengerTypeId")))
                    {
                        bookedProductDetails.PassengerTypeId =
                            DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "PassengerTypeId");

                        if (bookedProductDetails.PassengerTypeId > 0)
                        {
                            bookedProductDetails.PassengerType = Convert.ToString((PassengerType)bookedProductDetails.PassengerTypeId);
                        }
                    }

                    bookedProductDetails.BookedOptionId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "BookedOptionId");
                    bookedProductDetails.BookingId = bookingid;
                    bookingFoundAgainstCurrentBookingId.PaxPriceInfo.Add(bookedProductDetails);
                }
            }
        }

        private MyUserEmailPreferences GetUserEmailPref(IDataReader reader)
        {
            var userEmailPrefPerQuestion = new MyUserEmailPreferences
            {
                QuestionText = DbPropertyHelper
                    .StringDefaultPropertyFromRow(reader, "questionText").Trim()
            };
            if (!string.IsNullOrEmpty(
                DbPropertyHelper.StringPropertyFromRowWithTrim(reader, "QuestionOrder")))
            {
                userEmailPrefPerQuestion.QuestionOrder =
                    DbPropertyHelper.Int32PropertyFromRow(reader, "QuestionOrder");

                userEmailPrefPerQuestion.MyUserAnswers = new List<MyUserAnswer>();
                var myUserAnswer = new MyUserAnswer
                {
                    AnswerId = DbPropertyHelper.Int32PropertyFromRow(reader, "answerid"),
                    AnswerText = DbPropertyHelper.StringPropertyFromRowWithTrim(reader, "answerText"),
                    AnswerOrder = DbPropertyHelper.Int32PropertyFromRow(reader, "AnswerOrder")
                };

                userEmailPrefPerQuestion.MyUserAnswers.Add(myUserAnswer);
            }

            return userEmailPrefPerQuestion;
        }
    }
}