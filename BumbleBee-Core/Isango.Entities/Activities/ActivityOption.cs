using Isango.Entities.GlobalTixV3;
using Isango.Entities.PrioHub;
using Isango.Entities.Tiqets;
using Isango.Entities.TourCMS;
using Isango.Entities.Ventrata;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace Isango.Entities.Activities
{
    [BsonIgnoreExtraElements]
    public class ActivityOption : ProductOption
    {
        public List<ActivitySeason> ActivitySeasons { get; set; }

        public string HotelPickUpLocation { get; set; }

        public string ScheduleReturnDetails { get; set; }

        public string RateKey { get; set; }

        #region HotelBeds AvailableModality Properties

        /// <summary>
        /// Modality code in case of Hotelbeds
        /// </summary>
        public string Code { get; set; }

        public string AvailToken { get; set; }

        public Contract Contract { get; set; }

        #endregion HotelBeds AvailableModality Properties

        public List<ContractQuestion> ContractQuestions { get; set; }

        public int PrioTicketClass { get; set; }

        public string PickupPoints { get; set; }

        public PickupPointDetails[] PickupPointDetails { get; set; }

        // Used in GLI and GT
        public Dictionary<int, string> PickupLocations { get; set; }

        #region BokunAPI

        public int StartTimeId { get; set; }
        public List<int> PricingCategoryId { get; set; }

        #endregion BokunAPI

        #region PrioAPI

        public bool IsTimeBasedOption { get; set; }

        #endregion PrioAPI

        #region FareHarbor Properties

        public string UserKey { get; set; }

        #endregion FareHarbor Properties

        #region AOT

        public string OptionType { get; set; }

        public string RoomType { get; set; }

        public string ServiceType { get; set; }

        #endregion AOT

        #region GoldenTours

        public string ScheduleId { get; set; }
        public string ProductType { get; set; }
        public string RefNo { get; set; }

        #endregion GoldenTours

        #region Hb Apitude

        /// <summary>
        /// true if Hb Apitude Api giving sell price for this option then there is no need to apply margin;
        /// </summary>
        public bool IsMandatoryApplyAmount { get; set; }

        #endregion Hb Apitude

        #region Redeam

        public Dictionary<string, string> PriceId { get; set; }
        public string RateId { get; set; }
        public string SupplierId { get; set; }
        public bool Cancellable { get; set; }
        public bool Holdable { get; set; }
        public bool Refundable { get; set; }
        public string Type { get; set; }
        public int HoldablePeriod { get; set; }
        public string Time { get; set; }

        public string RedeamAvailabilityId { get; set; }

        public string RedeamAvailabilityStart { get; set; }

        #endregion Redeam

        #region Rezdy

        public int Seats { get; set; }
        public int SeatsAvailable { get; set; }
        public bool AllDay { get; set; }
        public DateTime StartLocalTime { get; set; }
        public DateTime EndLocalTime { get; set; }
        public int PickUpId { get; set; }
        public bool QuantityRequired { get; set; }
        public int QuantityRequiredMin { get; set; }
        public int QuantityRequiredMax { get; set; }

        #endregion Rezdy

        #region GlobalTix

        public string TicketTypeIds { get; set; }
        public List<int?> ProductIDs { get; set; }

        public List<ContractQuestionsForGlobalTix3> ContractQuestionForGlobalTix3 { get; set; }

        #endregion GlobalTix

        #region Ventrata Properties

        public string VentrataProductId { get; set; }
        public string VentrataSupplierId { get; set; }
        public string VentrataBaseURL { get; set; }

        //TODO - Offer code value may be an object
        public string OfferCode { get; set; }

        public string OfferTitle { get; set; }
        public string pickupPointId { get; set; }
        public List<PickupPointsDetailsForVentrata> PickupPointsDetailsForVentrata { get; set; }
        public MeetingPointDetails MeetingPointDetails { get; set; }
        public List<OpeningHours> OpeningHoursDetails { get; set; }

        #endregion Ventrata Properties

        #region TourCMS Properties
        public string TourCMSProductId { get; set; }
        public List<PickupPointsForTourCMS> PickupPointsForTourCMS { get; set; }
        public string PickupOnRequestKeyTourCMS { get; set; }

        public string SupplierOptionNote { get; set; }

        #endregion TourCMS Properties

        #region Tiqets Properties
        public List<string> RequiresVisitorsDetails { get; set; }
        //public List<ProductVariantIdName> RequiresVisitorsDetailsWithVariant { get; set; }
        #endregion Tiqets Properties
        #region SRL Properties
        public string RateCode { get; set; }
        public bool VariantCondition { get; set; }
        #endregion SRL Properties

        #region PrioHub
        public string PrioHubProductId { get; set; }

        public string PrioHubTicketClass { get; set; }

        public List<PickUpPointForPrioHub> PickUpPointForPrioHub { get; set; }
        public List<ProductCombiDetails> ProductCombiDetails { get; set; }

        public ProductCluster Cluster { get; set; }

        public List<PrioHubProductPaxMapping> PrioHubProductPaxMapping { get; set; }
        public int PrioHubProductTypeStatus { get; set; }
        public bool? PrioHubProductGroupCode { get; set; } //QrCode single or multiple
        public string PrioHubAvailabilityId { get; set; }
        public List<PrioHubComboSubProduct> PrioHubComboSubProduct { get; set; }

        public string PrioHubDistributerId { get; set; }
        #endregion


        #region Rayna Properties
        public string TourId { get; set; }
        public string TourOptionId { get; set; }
        public string TransferId { get; set; }
        public int TimeSlotId { get; set; }

        public string TourStartTime { get; set; }
        #endregion
    }
}