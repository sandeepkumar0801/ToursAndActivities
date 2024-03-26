using Newtonsoft.Json;
using System.Collections.Generic;

namespace ServiceAdapters.Ventrata.Ventrata.Entities.Request
{
    public class BookingPackageReservationReq
    {
        [JsonProperty(PropertyName = "productId")]
        public string ProductId { get; set; }
        [JsonProperty(PropertyName = "optionId")]
        public string OptionId { get; set; }

        [JsonProperty(PropertyName = "pickupRequested")]
        public bool IsPickUpRequested { get; set; }

        [JsonProperty(PropertyName = "pickupPointId")]
        public string PickUpPointId { get; set; }


        [JsonProperty(PropertyName = "unitItems")]
        public List<Unititem> UnitItems { get; set; }
        [JsonProperty(PropertyName = "packageBookings")]
        public List<Packagebooking> PackageBookings { get; set; }
    }

    public class Unititem
    {
        [JsonProperty(PropertyName = "unitId")]
        public string UnitId { get; set; }
    }

    public class Packagebooking
    {
        [JsonProperty(PropertyName = "packageIncludeId")]
        public string PackageIncludeId { get; set; }
        [JsonProperty(PropertyName = "availabilityId")]
        public string AvailabilityId { get; set; }
    }


}
