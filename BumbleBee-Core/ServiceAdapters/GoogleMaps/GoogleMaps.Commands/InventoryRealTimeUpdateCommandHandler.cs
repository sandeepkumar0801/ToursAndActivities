using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Isango.Entities.ConsoleApplication.DataDumping;
using Isango.Entities.GoogleMaps;
using Logger.Contract;
using ServiceAdapters.GoogleMaps.Constants;
using ServiceAdapters.GoogleMaps.GoogleMaps.Commands.Contracts;
using ServiceAdapters.GoogleMaps.GoogleMaps.Entities.DTO;
using Util;

namespace ServiceAdapters.GoogleMaps.GoogleMaps.Commands
{
    internal class InventoryRealTimeUpdateCommandHandler : CommandHandlerBase, IInventoryRealTimeUpdateCommandHandler
    {
        #region Properties

        #endregion Properties

        #region Constructor
        public InventoryRealTimeUpdateCommandHandler(ILogger log) : base(log)
        {
        }

        #endregion Constructor

        #region Protected Methods
        protected override object GoogleMapsApiRequest<T>(T inputContext)
        {
            var methodPath = GenerateMethodPath();
            var availabilities = SerializeDeSerializeHelper.Serialize(inputContext);
            var content = new StringContent(availabilities);
            var httpClient = GetHttpClient();
            var result = httpClient.PostAsync(methodPath, content);
            result.Wait();
            return ValidateApiResponse(result.Result);
        }
        protected override object CreateInputRequest<T>(T inputContext)
        {
            var merchantActivitiesDto = inputContext as MerchantActivitiesDto;
            var extendedServiceAvailabilities = new List<ExtendedServiceAvailability>();
            if (merchantActivitiesDto != null)
            {
                var merchantId = merchantActivitiesDto.MerchantId;
                foreach (var activity in merchantActivitiesDto.Activities)
                {
                    var extendedServiceAvailability = new ExtendedServiceAvailability
                    {
                        Availability = new List<InventoryAvailability>(),
                        MerchantId = merchantId,
                        ServiceId = activity.ActivityId.ToString()
                    };
                    var serviceDetails = activity.ServiceDetails;
                    foreach (var service in serviceDetails)
                    {
                        if (service.DumpingStatus != DumpingStatus.Deleted.ToString())
                        {
                            var availability = new InventoryAvailability
                            {
                                TicketTypeId = new List<string> { service.TicketTypeId },
                                StartTime = DateTimeOffset.FromUnixTimeSeconds(service.UnixStartSec).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
                                ConfirmationMode = Constant.ConfirmationMode,
                                Duration = "10800s",
                                SpotsOpen = service.Capacity.ToString(),
                                SpotsTotal = service.Capacity.ToString()
                            };
                            extendedServiceAvailability.Availability.Add(availability);
                        }
                    }
                    if (extendedServiceAvailability.Availability.Any())
                    {
                        extendedServiceAvailabilities.Add(extendedServiceAvailability);
                    }
                }
            }
            var result = new InventoryAvailabiltyDto
            {
                ExtendedServiceAvailability = extendedServiceAvailabilities
            };
            return result;
        }

        #endregion Protected Methods

        #region Private Methods

        private string GenerateMethodPath()
        {
            return string.Format(UriConstants.InventoryRealTimeUpdate, ConfigurationManagerHelper.GetValuefromAppSettings("GMaps:PartnerId"));
        }
        #endregion Private Methods
    }
}