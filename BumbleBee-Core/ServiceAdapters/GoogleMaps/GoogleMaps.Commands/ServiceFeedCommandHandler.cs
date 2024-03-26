using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.Enums;
using Isango.Entities.GoogleMaps;
using Logger.Contract;
using ServiceAdapters.GoogleMaps.Constants;
using ServiceAdapters.GoogleMaps.GoogleMaps.Commands.Contracts;
using ServiceAdapters.GoogleMaps.GoogleMaps.Entities.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using Util;
using PassengerType = Isango.Entities.GoogleMaps.PassengerType;
using Price = ServiceAdapters.GoogleMaps.GoogleMaps.Entities.DTO.Price;
using Rating = ServiceAdapters.GoogleMaps.GoogleMaps.Entities.DTO.Rating;

namespace ServiceAdapters.GoogleMaps.GoogleMaps.Commands
{
    internal class ServiceFeedCommandHandler : CommandHandlerBase, IServiceFeedCommandHandler
    {
        #region Constructor

        public ServiceFeedCommandHandler(ILogger log) : base(log)
        {
        }

        #endregion Constructor

        #region Private Methods

        private Rules GetRules(ActivityDto activity)
        {
            var admissionPolicy = activity.ServiceDetails.All(e => e.StartTime == TimeSpan.Zero.ToString()) ? AdmissionPolicy.TIME_FLEXIBLE.ToString() : AdmissionPolicy.TIME_STRICT.ToString();
            var rules = new Rules
            {
                CancellationPolicy = new CancellationPolicy
                {
                    RefundCondition = new List<RefundCondition>()
                },
                AdmissionPolicy = admissionPolicy
            };
            var refundConditions = new List<RefundCondition>();

            var cancellationPolicies = activity.CancellationPolicies;
            if (!cancellationPolicies.Any())
            {
                refundConditions.Add(new RefundCondition
                {
                    MinDurationBeforeStartTimeSec = 0,
                    RefundPercent = 0
                });
            }
            else
            {
                var cancelPolicy = cancellationPolicies.First();
                var maxSellPrice = activity.ServiceDetails.Max(s => s.SellPrice);
                if (!string.IsNullOrWhiteSpace(cancelPolicy.CancellationPrices))
                {
                    var cancellationPrices =
                        SerializeDeSerializeHelper.DeSerialize<List<GoogleCancellationPrice>>(cancelPolicy.CancellationPrices);

                    foreach (var googleCancellationPrice in cancellationPrices)
                    {
                        var minDurationBeforeStartTimeSec = Convert.ToInt64(
                            (string.IsNullOrWhiteSpace(googleCancellationPrice.CutoffHours)
                                ? 0
                                : Convert.ToDecimal(googleCancellationPrice.CutoffHours)) * 3600);
                        var refundPercentage = googleCancellationPrice.IsPercentage
                            ? 100 - Convert.ToInt64(googleCancellationPrice.CancellationCharge)
                            : 100 - Convert.ToInt64((googleCancellationPrice.CancellationCharge / maxSellPrice) * 100);
                        refundPercentage = refundPercentage < 0 ? 0 : refundPercentage;
                        if (refundConditions.All(w => w.RefundPercent != refundPercentage))
                        {
                            refundConditions.Add(new RefundCondition
                            {
                                MinDurationBeforeStartTimeSec = refundPercentage == 0 ? 0 : minDurationBeforeStartTimeSec,
                                RefundPercent = refundPercentage
                            });
                        }
                    }
                }
            }
            rules.CancellationPolicy.RefundCondition = refundConditions;
            return rules;
        }

        private List<Location> GetServiceLocations(AcitvityCollection engActivityCollection)
        {
            var locations = new List<Location>();
            if (!string.IsNullOrWhiteSpace(engActivityCollection.Activity.CoOrdinates))
            {
                var coOrdinate = engActivityCollection.Activity.CoOrdinates.Split(',');
                if (coOrdinate.Length == 2)
                {
                    var location = new Location
                    {
                        LocationType = "VISITED_LOCATION",
                        Geo = new GeoCordinate
                        {
                            Latitude = coOrdinate[0],
                            Longitude = coOrdinate[1]
                        }
                    };
                    locations.Add(location);
                }
            }
            return locations;
        }

        private List<RelatedMedia> GetRelatedMedia(AcitvityCollection engActivityCollection)
        {
            var relatedMedia = new List<RelatedMedia>();
            foreach (var image in engActivityCollection?.Activity?.Images)
            {
                try
                {
                    if (image.ImageType == ImageType.CloudProduct)
                    {
                        relatedMedia.Add(new RelatedMedia
                        {
                            Type = "PHOTO",
                            Url = new Uri("https://res.cloudinary.com/https-www-isango-com/image/upload/t_m_Prod/" + image?.Name),
                        });
                    }
                }
                catch (Exception ex)
                {
                    //throw;
                }
            }
            return relatedMedia;
        }

        private IntakeForm GetServiceIntakeForm(ActivityDto activity, AcitvityCollection engActivityCollection)
        {
            var serviceIntakeForm = new IntakeForm { Field = new List<Field>() };
            var pickUpDetailsField = GetPickUpDetailsField(activity, engActivityCollection);
            if (pickUpDetailsField != null)
            {
                serviceIntakeForm.Field.Add(pickUpDetailsField);
            }
            foreach (var detail in activity.ExtraDetails)
            {
                if (!string.IsNullOrWhiteSpace(detail.Questions))
                {
                    var questions = SerializeDeSerializeHelper.DeSerialize<List<Question>>(detail.Questions);
                    var ticketTypeIds = activity.ServiceDetails.Where(w => w.ServiceOptionId == detail.OptionId)
                        .Select(s => s.TicketTypeId).Distinct().ToList();
                    foreach (var question in questions)
                    {
                        if (question.AnswerOptions.Any())
                        {
                            var label = new IntakeFormLocalizedDescription
                            {
                                Value = question.Label,
                                LocalizedValue = new List<LocalizedValue>
                                {
                                    new LocalizedValue
                                    {
                                        Value = question.Label,
                                        Locale = "en"
                                    }
                                }
                            };
                            var dyaQuestion = new Field
                            {
                                Id = question.Id,
                                Label = label,
                                IsRequired = question.Required,
                                Type = "DROPDOWN",
                                Value = question.AnswerOptions.Select(s => s.Value).ToList(),
                                TicketIds = ticketTypeIds
                            };
                            serviceIntakeForm.Field.Add(dyaQuestion);
                        }
                    }
                }
            }
            return serviceIntakeForm;
        }

        private Field GetPickUpDetailsField(ActivityDto activity, AcitvityCollection engActivityCollection)
        {
            var activityPickupLocations = activity.ExtraDetails.Select(s => s.PickUpLocations).ToList();
            //If activityPickupLocations count is 0 and PickUpDropOffOptionType is not equal to RequestedByCustomer
            if (activityPickupLocations.Any() == false && engActivityCollection.Activity.PickUpOption != PickUpDropOffOptionType.RequestedByCustomer)
            {
                return null;
            }
            var pickUpDetailsField = new Field
            {
                Id = "PickUp",
                IsRequired = true,
                Label = new IntakeFormLocalizedDescription
                {
                    Value = "Pick Up Details",
                    LocalizedValue = new List<LocalizedValue>
                    {
                        new LocalizedValue
                        {
                            Value = "Pick Up Details",
                            Locale = "en"
                        },
                        new LocalizedValue
                        {
                            Value = "Abholdetails",
                            Locale = "de"
                        },
                        new LocalizedValue
                        {
                            Value = "Detalles de Recogida",
                            Locale = "es"
                        }
                    }
                }
            };
            if (engActivityCollection.Activity.PickUpOption == PickUpDropOffOptionType.RequestedByCustomer)
            {
                pickUpDetailsField.Type = "SHORT_ANSWER";
            }
            else
            {
                pickUpDetailsField.Type = "DROPDOWN";
                pickUpDetailsField.Value = activityPickupLocations;
            }

            return pickUpDetailsField;
        }

        private Field GetLastNameField()
        {
            var lastNameField = new Field
            {
                Id = "LastName",
                IsRequired = true,
                Label = new IntakeFormLocalizedDescription
                {
                    Value = "Last Name",
                    LocalizedValue = new List<LocalizedValue>
                    {
                        new LocalizedValue
                        {
                            Value = "Last Name",
                            Locale = "en"
                        },
                        new LocalizedValue
                        {
                            Value = "Nachname",
                            Locale = "de"
                        },
                        new LocalizedValue
                        {
                            Value = "Apellido",
                            Locale = "es"
                        }
                    }
                },
                Type = "SHORT_ANSWER"
            };
            return lastNameField;
        }

        private Field GetFirstNameField()
        {
            var firstNameField = new Field
            {
                Id = "FirstName",
                IsRequired = true,
                Label = new IntakeFormLocalizedDescription
                {
                    Value = "First Name",
                    LocalizedValue = new List<LocalizedValue>
                    {
                        new LocalizedValue
                        {
                            Value = "First Name",
                            Locale = "en"
                        },
                        new LocalizedValue
                        {
                            Value = "Vorname",
                            Locale = "de"
                        },
                        new LocalizedValue
                        {
                            Value = "Nombre",
                            Locale = "es"
                        }
                    }
                },
                Type = "SHORT_ANSWER"
            };
            return firstNameField;
        }

        private List<TicketType> GetTicketTypes(ActivityDto activity, AcitvityCollection engActivityCollection, List<PassengerType> passengerTypes)
        {
            var ticketTypes = new List<TicketType>();

            var distinctTicketTypes = activity.ServiceDetails.Select(s => new
            {
                s.TicketTypeId,
                s.ActivityId,
                s.Currency,
                s.SellPrice,
                s.ServiceOptionId,
                s.PassengerTypeId,
                s.Variant
            }).Distinct();

            var perTicketIntakeForm = GetPerTicketIntakeForm(engActivityCollection);

            foreach (var ticketType in distinctTicketTypes)
            {
                var isOptionPresent =
                    engActivityCollection.Activity.ProductOptions?.Any(w => w.Id == ticketType.ServiceOptionId) ?? false;
                if (isOptionPresent)
                {
                    var variant = ticketType.Variant;
                    var localpassengerType = passengerTypes.Where(w => w.PassengerTypeId == ticketType.PassengerTypeId).ToList();
                    var passengerInfo = engActivityCollection.Activity.PassengerInfo?.FirstOrDefault(e => e.PassengerTypeId == ticketType.PassengerTypeId);
                    var measurementDesc = passengerInfo != null ? passengerInfo.MeasurementDesc : string.Empty;

                    var localizedShortDescription = GetTicketTypeLocalizedShortDescription(localpassengerType, measurementDesc);

                    var localizedOptionDescription =
                        GetTicketTypeLocalizedOptionDescription(activity, variant, ticketType.ServiceOptionId);

                    ticketTypes.Add(new TicketType
                    {
                        TicketTypeId = ticketType.TicketTypeId,
                        Price = new Price
                        {
                            CurrencyCode = ticketType.Currency,
                            PriceMicros = Convert.ToInt64(ticketType.SellPrice * Constant.MicroUnit).ToString()
                        },
                        LocalizedShortDescription = localizedShortDescription,
                        LocalizedOptionDescription = localizedOptionDescription,
                        IntakeForm = perTicketIntakeForm
                    });
                }
            }
            return ticketTypes;
        }

        private IntakeForm GetPerTicketIntakeForm(AcitvityCollection engActivityCollection)
        {
            var intakeForm = new IntakeForm();
            if (engActivityCollection.Activity.IsPaxDetailRequired)
            {
                intakeForm.Field = new List<Field>
                {
                    GetFirstNameField(),
                    GetLastNameField()
                };
            }
            else
            {
                intakeForm.Field = new List<Field>();
            }

            return intakeForm;
        }

        private LocalizedDescription GetTicketTypeLocalizedOptionDescription(ActivityDto activity, string variant, int optionId)
        {
            var localizedValues = new List<LocalizedValue>();
            foreach (var collection in activity.AcitvityCollections)
            {
                var localOption = collection.Activity.ProductOptions
                    .FirstOrDefault(w => w.Id == optionId);
                localizedValues.Add(new LocalizedValue
                {
                    Value = string.IsNullOrWhiteSpace(variant) ? $"{localOption?.Name.Trim()}" : $"{ localOption?.Name.Trim() }_{ variant.Trim() }",
                    Locale = collection.LanguageCode
                });
            }
            var localizedOptionDescription = new LocalizedDescription { LocalizedValue = localizedValues };

            return localizedOptionDescription;
        }

        private LocalizedDescription GetTicketTypeLocalizedShortDescription(List<PassengerType> passengerTypes, string measurementDesc)
        {
            var localizedValues = new List<LocalizedValue>();
            foreach (var type in passengerTypes)
            {
                localizedValues.Add(new LocalizedValue
                {
                    Value = string.IsNullOrWhiteSpace(measurementDesc) ? $"{type.PassengerTypeName.Trim()}" : $"{type.PassengerTypeName.Trim()} {measurementDesc.Trim()}",
                    Locale = type.LanguageCode
                });
            }
            var localizedShortDescription = new LocalizedDescription { LocalizedValue = localizedValues };

            return localizedShortDescription;
        }

        private ToursAndActivitiesContent GetToursAndActivitiesContent(ActivityDto activity,
            AcitvityCollection engActivityCollection)
        {
            var toursAndActivitiesContent = new ToursAndActivitiesContent();

            var localHighLight = GetHighLights(activity, engActivityCollection);
            toursAndActivitiesContent.Highlights = localHighLight;

            var localInclusions = GetInclusions(activity, engActivityCollection);
            toursAndActivitiesContent.Inclusions = localInclusions;

            var localExclusions = GetExclusions(activity, engActivityCollection);
            toursAndActivitiesContent.Exclusions = localExclusions;

            var localMustKnow = GetMustKnows(activity, engActivityCollection);
            toursAndActivitiesContent.MustKnow = localMustKnow;

            return toursAndActivitiesContent;
        }

        private List<LocalizedDescription> GetMustKnows(ActivityDto activity, AcitvityCollection engActivityCollection)
        {
            var localMustKnow = new List<LocalizedDescription>();
            if (!string.IsNullOrWhiteSpace(engActivityCollection.Activity.PleaseNote))
            {
                var result = new Dictionary<string, List<string>>();
                foreach (var collection in activity.AcitvityCollections)
                {
                    if (string.IsNullOrWhiteSpace(collection.Activity.PleaseNote))
                        continue;
                    var repeatedText = PrepareRepeatedText(collection.Activity.PleaseNote);
                    if (repeatedText.Any())
                        result.Add(collection.LanguageCode, repeatedText);
                }

                var maxLength = result.Values.OrderByDescending(e => e.Count).FirstOrDefault()?.Count ?? 0;
                localMustKnow = GetLocalizedDescriptions(result, maxLength);
            }
            return localMustKnow;
        }

        private List<LocalizedDescription> GetExclusions(ActivityDto activity, AcitvityCollection engActivityCollection)
        {
            var localExclusions = new List<LocalizedDescription>();
            if (!string.IsNullOrWhiteSpace(engActivityCollection.Activity.Exclusions))
            {
                var result = new Dictionary<string, List<string>>();
                foreach (var collection in activity.AcitvityCollections)
                {
                    if (string.IsNullOrWhiteSpace(collection.Activity.Exclusions))
                        continue;
                    var repeatedText = PrepareRepeatedText(collection.Activity.Exclusions);
                    if (repeatedText.Any())
                        result.Add(collection.LanguageCode, repeatedText);
                }

                var maxLength = result.Values.OrderByDescending(e => e.Count).FirstOrDefault()?.Count ?? 0;
                localExclusions = GetLocalizedDescriptions(result, maxLength);
            }
            return localExclusions;
        }

        private List<LocalizedDescription> GetInclusions(ActivityDto activity, AcitvityCollection engActivityCollection)
        {
            var localInclusions = new List<LocalizedDescription>();
            if (!string.IsNullOrWhiteSpace(engActivityCollection.Activity.Inclusions))
            {
                var result = new Dictionary<string, List<string>>();
                foreach (var collection in activity.AcitvityCollections)
                {
                    if (string.IsNullOrWhiteSpace(collection.Activity.Inclusions))
                        continue;
                    var repeatedText = PrepareRepeatedText(collection.Activity.Inclusions);
                    if (repeatedText.Any())
                        result.Add(collection.LanguageCode, repeatedText);
                }

                var maxLength = result.Values.OrderByDescending(e => e.Count).FirstOrDefault()?.Count ?? 0;
                localInclusions = GetLocalizedDescriptions(result, maxLength);
            }
            return localInclusions;
        }

        private List<LocalizedDescription> GetHighLights(ActivityDto activity, AcitvityCollection engActivityCollection)
        {
            var result = new Dictionary<string, List<string>>();
            foreach (var collection in activity.AcitvityCollections)
            {
                if (collection.Activity.ReasonToBook != null && collection.Activity.ReasonToBook.Count > 0)
                {
                    collection.Activity.ReasonToBook.ForEach(e => e = Sanitizer.SanitizeHtmlForGoogle(e));
                    result.Add(collection.LanguageCode, collection.Activity.ReasonToBook);
                }
            }
            var maxLength = result.Values.OrderByDescending(e => e.Count).FirstOrDefault()?.Count ?? 0;
            var localHighLight = GetLocalizedDescriptions(result, maxLength);
            return localHighLight;
        }

        private LocalizedDescription GetLocalizedServiceName(ActivityDto activity)
        {
            var localizedServiceName =
                new LocalizedDescription
                {
                    LocalizedValue = new List<LocalizedValue>()
                };

            foreach (var collection in activity.AcitvityCollections)
            {
                if (!string.IsNullOrWhiteSpace(collection.Activity.Name))
                {
                    localizedServiceName.LocalizedValue.Add(new LocalizedValue
                    {
                        Value = Sanitizer.SanitizeHtmlForGoogle(collection.Activity.Name),
                        Locale = collection.LanguageCode
                    });
                }
            }

            return localizedServiceName;
        }

        private LocalizedDescription GetLocalDescription(ActivityDto activity)
        {
            var localDescription =
                new LocalizedDescription
                {
                    LocalizedValue = new List<LocalizedValue>()
                };

            foreach (var collection in activity.AcitvityCollections)
            {
                if (!string.IsNullOrWhiteSpace(collection.Activity.Introduction))
                {
                    localDescription.LocalizedValue.Add(new LocalizedValue
                    {
                        Value = Sanitizer.SanitizeHtmlForGoogle(collection.Activity.Introduction),
                        Locale = collection.LanguageCode
                    });
                }
            }

            return localDescription;
        }

        private List<string> PrepareRepeatedText(string text)
        {
            var sanitizedText = Sanitizer.SanitizeHtmlForGoogle(text);

            var list = new List<string>();
            //TODO: Need to check empty sanitizedText
            var pTagList = Extensions.SplitByHTMLTag(sanitizedText, "p");
            foreach (var p in pTagList)
            {
                var liTagList = Extensions.SplitByHTMLTag(p, "li");
                foreach (var l in liTagList)
                {
                    var brList = l.Split(new[] { "<br>", "<br >", "</br>", "< /br>", "<br/>", "<br />" }, StringSplitOptions.None);
                    foreach (var br in brList)
                    {
                        var rnList = br.Split(new[] { "\r\n" }, StringSplitOptions.None);
                        list.AddRange(rnList);
                    }
                }
            }
            list.RemoveAll(x => string.IsNullOrWhiteSpace(x));
            return list;
        }

        private List<LocalizedDescription> GetLocalizedDescriptions(Dictionary<string, List<string>> result, int maxLength)
        {
            var localizedDescriptions = new List<LocalizedDescription>();
            if (result.Count > 0)
            {
                for (int i = 0; i < maxLength; i++)
                {
                    var localizedDescription =
               new LocalizedDescription
               {
                   LocalizedValue = new List<LocalizedValue>()
               };

                    foreach (var item in result)
                    {
                        if (i < item.Value.Count)
                        {
                            var localizedValue = new LocalizedValue
                            {
                                Value = item.Value[i],
                                Locale = item.Key
                            };
                            localizedDescription.LocalizedValue.Add(localizedValue);
                        }
                    }
                    localizedDescriptions.Add(localizedDescription);
                }
            }
            return localizedDescriptions;
        }

        #endregion Private Methods

        #region Protected Method

        protected override object MapFeed<T>(T inputContext)
        {
            var serviceAvailabilityDto = inputContext as ServiceAvailabilityDto;

            var googleServiceFeeds = new List<Service>();
            if (serviceAvailabilityDto != null)
            {
                foreach (var merchant in serviceAvailabilityDto?.MerchantActivitiesDtos?.ToList())
                {
                    try
                    {
                        var merchantId = merchant.MerchantId;
                        foreach (var activity in merchant.Activities)
                        {
                            try
                            {
                                var engActivityCollection =
                                                    activity?.AcitvityCollections.FirstOrDefault(w => w.LanguageCode == "en");
                                if (engActivityCollection == null || !activity.ServiceDetails.Any())
                                {
                                    continue;
                                }

                                var localDescription = GetLocalDescription(activity);

                                var localizedServiceName = GetLocalizedServiceName(activity);

                                var rating = new Rating();

                                var toursAndActivitiesContent = GetToursAndActivitiesContent(activity, engActivityCollection);

                                //Fetching Adult price for Service
                                var adultPriceMicros = activity.ServiceDetails.FirstOrDefault(x => x.PassengerTypeId == 1);
                                var adultPrice = new Price
                                {
                                    CurrencyCode = adultPriceMicros.Currency.ToString(),
                                    PriceMicros = Convert.ToInt64(adultPriceMicros.SellPrice * Constant.MicroUnit).ToString()
                                };

                                var ticketTypes = GetTicketTypes(activity, engActivityCollection,
                                    serviceAvailabilityDto.PassengerTypes);

                                var serviceIntakeForm = GetServiceIntakeForm(activity, engActivityCollection);

                                var relatedMedia = GetRelatedMedia(engActivityCollection);

                                var locations = GetServiceLocations(engActivityCollection);

                                var rules = GetRules(activity);

                                googleServiceFeeds.Add(new Service
                                {
                                    MerchantId = merchantId,
                                    ServiceId = activity.ActivityId.ToString(),
                                    LocalizedDescription = localDescription,
                                    LocalizedServiceName = localizedServiceName,
                                    Price = adultPrice,
                                    PrepaymentType = Constant.Required,
                                    IntegrationType = Constant.IntegrationType,
                                    Rating = rating,
                                    RelatedMedia = relatedMedia,
                                    Rules = rules,
                                    TicketType = ticketTypes,
                                    ToursAndActivitiesContent = toursAndActivitiesContent,
                                    IntakeForm = serviceIntakeForm,
                                    Location = locations
                                });
                            }
                            catch (Exception ex)
                            {
                                //throw;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        //throw;
                    }
                }
            }
            var result = new ServiceFeedDto
            {
                Service = googleServiceFeeds,
                Metadata = new ServiceMetadata
                {
                    GenerationTimestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(),
                    TotalShards = 1,
                    ProcessingInstruction = Constant.ProcessingInstruction
                }
            };
            return SerializeDeSerializeHelper.Serialize(result);
        }

        #endregion Protected Method
    }
}