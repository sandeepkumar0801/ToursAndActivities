using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Enums;
using Isango.Entities.HotelBeds;
using System;
using System.Collections.Generic;
using System.Linq;
using TableStorageOperations.Models.AdditionalPropertiesModels.Availabilities;
using Util;
using Question = WebAPI.Models.ResponseModels.Question;

namespace WebAPI.Mapper
{
    public class CheckoutMapper
    {
        public List<Customer> GetCustomers(bool isPaxDetailRequired, TravelInfo travelInfo)
        {
            var adultCount = Convert.ToInt32((travelInfo?.NoOfPassengers?.FirstOrDefault(x => x.Key == PassengerType.Adult))?.Value);
            var youthCount = Convert.ToInt32((travelInfo?.NoOfPassengers?.FirstOrDefault(x => x.Key == PassengerType.Youth))?.Value);
            var childCount = Convert.ToInt32((travelInfo?.NoOfPassengers?.FirstOrDefault(x => x.Key == PassengerType.Child))?.Value);
            var infantCount = Convert.ToInt32((travelInfo?.NoOfPassengers?.FirstOrDefault(x => x.Key == PassengerType.Infant))?.Value);

            var adultAges = Convert.ToInt32((travelInfo?.Ages?.FirstOrDefault(x => x.Key == PassengerType.Adult))?.Value);
            var youthAges = Convert.ToInt32((travelInfo?.Ages?.FirstOrDefault(x => x.Key == PassengerType.Youth))?.Value);
            var childAges = Convert.ToInt32((travelInfo?.Ages?.FirstOrDefault(x => x.Key == PassengerType.Child))?.Value);
            var infantAges = Convert.ToInt32((travelInfo?.Ages?.FirstOrDefault(x => x.Key == PassengerType.Infant))?.Value);

            var customers = new List<Customer>();
            if (isPaxDetailRequired)
            {
                var customer = new Customer
                {
                    Age = adultAges,
                    PassengerType = PassengerType.Adult,
                    FirstName = "Any",
                    IsLeadCustomer = true,
                    LastName = "Adult"
                };
                customers.Add(customer);
            }
            else
            {
                for (int i = 0; i < adultCount; i++)
                {
                    var customer = new Customer
                    {
                        Age = adultAges,
                        PassengerType = PassengerType.Adult,
                        FirstName = "Any",//+ (i + 1),
                        IsLeadCustomer = i == 0,
                        LastName = "Adult"
                    };
                    customers.Add(customer);
                }
                if (youthCount > 0)
                {
                    //HB supports only child , so for infant,youth,child,
                    //so we are passing CH for these 3 types in HB API request.
                    for (int i = 0; i < youthCount; i++)
                    {
                        var customer = new Customer
                        {
                            Age = youthAges,
                            PassengerType = PassengerType.Youth,
                            FirstName = "Any",// + (i + 1),
                            IsLeadCustomer = i == 0,
                            LastName = "Youth"
                        };
                        customers.Add(customer);
                    }
                }
                if (childCount > 0)
                {
                    //var index = 1;
                    //foreach (int age in travelInfo.Ages.Where(x => x.Key == x.Key == PassengerType.Child).Select(s => s.Value))
                    //{
                    //    var customer = new Customer
                    //    {
                    //        Age = age,
                    //        PassengerType = PassengerType.Child,
                    //        FirstName = "ChildFirstName" + (index).ToString(),
                    //        IsLeadCustomer = false,
                    //        LastName = "ChildLastName"
                    //    };
                    //    customers.Add(customer);
                    //    index++;
                    //}

                    //HB supports only child , so for infant,youth,child,
                    //so we are passing CH for these 3 types in HB API request.
                    for (int i = 0; i < childCount; i++)
                    {
                        var customer = new Customer
                        {
                            Age = childAges,
                            PassengerType = PassengerType.Child,
                            FirstName = "Any",//+ (i + 1),
                            IsLeadCustomer = i == 0,
                            LastName = "Child"
                        };
                        customers.Add(customer);
                    }
                }

                if (infantCount > 0)
                {
                    //HB supports only child , so for infant,youth,child,
                    //so we are passing CH for these 3 types in HB API request.
                    for (int i = 0; i < infantCount; i++)
                    {
                        var customer = new Customer
                        {
                            Age = infantAges,
                            PassengerType = PassengerType.Infant,
                            FirstName = "Any",// + (i + 1),
                            IsLeadCustomer = i == 0,
                            LastName = "Infant"
                        };
                        customers.Add(customer);
                    }
                }
            }

            return customers;
        }

        public HotelBedsSelectedProduct GetHotelBedsSelectedProduct(TicketsAvailabilities ticketsAvailabilities, Models.RequestModels.PaymentExtraDetail extraDetailsRequest)
        {
            var savedTravelInfo = SerializeDeSerializeHelper.DeSerialize<TravelInfo>(ticketsAvailabilities.TravelInfo);
            var contractQuestions = SerializeDeSerializeHelper.DeSerialize<List<ContractQuestion>>(ticketsAvailabilities.ContractQuestions);
            var activityOptions = new List<ActivityOption>
                            {
                                new ActivityOption
                                {
                                    TravelInfo = savedTravelInfo,
                                    IsSelected = true,
                                    Customers = GetCustomers(ticketsAvailabilities.IsPaxDetailRequired, savedTravelInfo),
                                    AvailToken = ticketsAvailabilities.AvailToken,
                                    Code = ticketsAvailabilities.ModalityCode,
                                    ContractQuestions = contractQuestions
                                }
                            };

            var selectedProduct = new HotelBedsSelectedProduct
            {
                Code = ticketsAvailabilities.SupplierOptionCode,//.Trim().Split(new[] { "~#~" }, StringSplitOptions.RemoveEmptyEntries)[0],
                ProductOptions = activityOptions.Cast<ProductOption>().ToList(),
                ContractQuestions = contractQuestions
            };

            return selectedProduct;
        }

        /// <summary>
        /// Map Payment Extra Details Response
        /// </summary>
        /// <param name="entity"> Baisc info of activity</param>
        /// <param name="questions">List of checkout questions </param>
        /// <param name="keyValues">Pickup locations </param>
        /// <param name="keyValues1">Drop off locations</param>
        /// <returns></returns>
        public Models.ResponseModels.PaymentExtraDetail MapPaymentExtraDetailsResponse(BaseAvailabilitiesEntity entity
            , List<Question> questions
            , Dictionary<int, string> keyValues
            , Dictionary<int, string> keyValues1 = null
        )
        {
            var details = new Models.ResponseModels.PaymentExtraDetail
            {
                ActivityId = entity.ActivityId,
                ReferenceId = entity.RowKey,
                Questions = questions,
                PickupLocations = keyValues,
                DropoffLocations = keyValues1,
            };

            return details;
        }

        public List<Question> MapBokunQuestions(List<Isango.Entities.Bokun.Question> bokunQuestions)
        {
            var questions = new List<Question>();
            if (bokunQuestions?.Count > 0)
            {
                foreach (var ques in bokunQuestions)
                {
                    var answerOptions = new List<Models.ResponseModels.AnswerOption>();
                    ques?.AnswerOptions?.ForEach(ans => answerOptions.Add(new Models.ResponseModels.AnswerOption
                    {
                        Label = ans.Label,
                        Value = ans.Value
                    }));
                    questions.Add(new Question
                    {
                        DataType = ques?.DataType,
                        Id = ques?.QuestionId,
                        Required = ques?.Required ?? false,
                        QuestionType = ques?.QuestionType,
                        SelectFromOptions = ques?.SelectFromOptions ?? false,
                        AnswerOptions = answerOptions,
                        DefaultValue = ques?.DefaultValue,
                        Label = ques?.Label
                    });
                }
            }

            return questions;
        }

        public List<Question> MapHotelBedQuestions(List<ContractQuestion> contractQuestions)
        {
            var questions = contractQuestions?.Where(q => !string.IsNullOrWhiteSpace(q.Description))
                            ?.Select(x => new Question
                            {
                                Id = x.Code,
                                Label = x.Description,
                                Required = x.IsRequired
                            })?.ToList();
            return questions;
        }
    }
}