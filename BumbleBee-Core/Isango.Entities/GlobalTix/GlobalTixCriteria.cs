using Isango.Entities.Enums;
using System.Collections.Generic;

namespace Isango.Entities.GlobalTix
{
    public class GlobalTixCriteria : Criteria
    {
        public string ActivityId { get; set; }
        public int FactSheetId { get; set; }
        public int Days2Fetch { get; set; }
        public int? ServiceOptionID { get; set; }
    }
}
