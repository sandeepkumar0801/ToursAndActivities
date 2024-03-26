using Isango.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Isango.Persistence.Contract
{
    public interface IRedemptionPersistance
    {

        void AddRedemptionDataList(List<RedemptionData> redemptionDataList);
        void AddRedemptionDataList(RedemptionData redemptionData);
    }
}
