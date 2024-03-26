using Isango.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAdapters.GlobalTixV3.GlobalTix.Entities
{
    public class GlobalTixV3SelectedProduct : SelectedProduct
    {
        public ApiExtraDetail APIDetails { get; set; }
        public List<ContractQuestion> ContractQuestions { get; set; }
    }
}
