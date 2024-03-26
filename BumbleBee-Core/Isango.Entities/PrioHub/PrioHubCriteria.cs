using Isango.Entities.Activities;
using Isango.Entities.Ticket;
using System;
using System.Collections.Generic;

namespace Isango.Entities.PrioHub
{
    public class PrioHubCriteria : TicketCriteria
    {
        public string IsangoActivityId { get; set; }

        /// <summary>
        /// Isango Service option Id. Example "140450"
        /// </summary>
        public List<IsangoHBProductMapping> ProductMapping { get; set; }

        public int PagingStartIndex { get; set; }
        public int ItemPerPage { get; set; }

        public string ProductsIds { get; set; }

        public string FromDate { get; set; }

        public string ToDate { get; set; }

        public string ProductId { get; set; }
        public List<string> SupplierOptionCodes { get; set; }

        public Dictionary<string,int> SupplierOptionCodesWithDistributerId { get; set; }

        public string SupplierMultipleCodes { get; set; }

        public object ProductDetailResponseAPI { get; set; }
        public ActivityOption ActivityOptionAPI { get; set; }

        public int TimeBased { get; set; }

        public int DistributorId { get; set; }

        public decimal CommissionPercent { get; set; }
    }
}