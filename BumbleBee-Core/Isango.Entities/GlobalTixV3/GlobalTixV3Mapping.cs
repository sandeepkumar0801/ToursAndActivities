using Isango.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Isango.Entities.GlobalTixV3
{
  public  class GlobalTixV3Mapping
    {
        public int ServiceOptionId { get; set; }

        public int AgeGroupId { get; set; }

        public string AgeGroupCode { get; set; }

        public PassengerType PassengerType { get; set; }

        public APIType APIType { get; set; }
        public string SupplierCode { get; set; }
    }
}
