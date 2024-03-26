using Isango.Entities.Ticket;
using System.Collections.Generic;

namespace Isango.Entities.HotelBeds
{
    public class HotelbedCriteria : TicketCriteria
    {
        public bool IsPaging { get; set; }
        public int ItemsPerPage { get; set; }
    }

    public class HotelbedCriteriaApitude : HotelbedCriteria
    {
        //#region SearchRQ input classes

        ////public string Order { get; set; }

        //#endregion SearchRQ input classes

        #region Activity Details input classes

        /// <summary>
        /// Hotelbeds Apitude Activity Code. Example "E-U10-SANGO"
        /// </summary>
        public string ActivityCode { get; set; }

        /// <summary>
        /// Hotelbeds Apitude Activity Code list. Example "E-U10-SANGO". If an iSango activity has multiple options
        /// then each option can be mapped with single API activity code. ActivityCode field will contain the value for current option
        /// </summary>
        public List<string> ActivityCodes { get; set; }

        /// <summary>
        /// Hotelbeds Apitude Activity -> ModalityCode. On activity can have multiple modality code. Example "872437426#1DAY@STANDARD||"
        /// </summary>
        public string ModalityCode { get; set; }

        /// <summary>
        /// iSango ActivityId/ServiceId. Example "6591"
        /// </summary>
        public string IsangoActivityId { get; set; }

        /// <summary>
        /// Isango Service option Id. Example "140450"
        /// </summary>
        public string ServiceOptionId { get; set; }

        #endregion Activity Details input classes
    }

    public class HotelbedCriteriaApitudeFilter : HotelbedCriteriaApitude
    {
        public List<Filters> Filters { get; set; }
        public List<IsangoHBProductMapping> ProductMapping { get; set; }
    }

    public class Filters
    {
        public List<SearchFilterItems> SearchFilterItems { get; set; }
    }

    public class SearchFilterItems
    {
        public string Type { get; set; }
        public string Value { get; set; }
    }
}