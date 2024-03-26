using Logger.Contract;
using ServiceAdapters.GoogleMaps.Constants;
using ServiceAdapters.GoogleMaps.GoogleMaps.Commands.Contracts;
using ServiceAdapters.GoogleMaps.GoogleMaps.Entities.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using Util;

namespace ServiceAdapters.GoogleMaps.GoogleMaps.Commands
{
    internal class AvailabilityFeedCommandHandler : CommandHandlerBase, IAvailabilityFeedCommandHandler
    {
        #region Constructor

        public AvailabilityFeedCommandHandler(ILogger log) : base(log)
        {
        }

        #endregion Constructor

        #region Protected Method

        protected override object MapFeed<T>(T inputContext)
        {
            var serviceAvailabilityDto = inputContext as ServiceAvailabilityDto;
            var googleServiceFeeds = new List<ServiceAvailability>();
            if (serviceAvailabilityDto != null)
            {
                foreach (var merchant in serviceAvailabilityDto.MerchantActivitiesDtos)
                {
                    var merchantId = merchant.MerchantId;
                    foreach (var activity in merchant.Activities)
                    {
                        var serviceAvailability = new ServiceAvailability
                        {
                            Availability = new List<Availability>()
                        };
                        var serviceDetails = activity.ServiceDetails;
                        foreach (var service in serviceDetails)
                        {
                            var availability = new Availability()
                            {
                                TicketTypeId = new List<string>() { service.TicketTypeId },
                                MerchantId = merchantId,
                                StartSec = service.UnixStartSec.ToString(),
                                ServiceId = activity.ActivityId.ToString(),
                                ConfirmationMode = Constant.ConfirmationMode,
                                DurationSec = "10800",
                                SpotsOpen = service.Capacity.ToString(),
                                SpotsTotal = service.Capacity.ToString()
                            };
                            serviceAvailability.Availability.Add(availability);
                        }

                        if (serviceAvailability.Availability.Any())
                        {
                            googleServiceFeeds.Add(serviceAvailability);
                        }
                    }
                }
            }
            var result = new AvailabilityFeedDto()
            {
                ServiceAvailability = googleServiceFeeds,
                Metadata = new AvailabilityMetadata()
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