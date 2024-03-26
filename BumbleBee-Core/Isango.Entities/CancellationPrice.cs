using System;

namespace Isango.Entities
{
    public class CancellationPrice
    {
        public decimal CancellationAmount { get; set; }
        public DateTime CancellationFromdate { get; set; }
        public DateTime CancellationToDate { get; set; }
        public float Percentage { get; set; }

        /// <summary>
        /// HB-activity-api-3.0
        /// It specify the operational date to which this cancellation policy will  be applicable for.
        /// </summary>
        public DateTime CancellationDateRelatedToOpreationDate { get; set; }

        public string CancellationDescription { get; set; }
    }
}