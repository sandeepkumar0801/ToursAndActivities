using Isango.Entities.Ticket;
using System.Collections.Generic;

namespace Isango.Entities.NewCitySightSeeing
{
    public class NewCitySightSeeingCriteria : TicketCriteria
    {
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

        public List<IsangoHBProductMapping> ProductMapping { get; set; }

        
        public int FactSheetId { get; set; }
        public int Days2Fetch { get; set; }
        public int? ServiceOptionID { get; set; }
        public string ActivityId { get; set; }

        public string ProductOptionName { get; set; }
        public string ActivityName { get; set; }

        public string SupplierOptionNewCitySeeing { get; set; }
        #endregion Activity Details input classes
    }
}