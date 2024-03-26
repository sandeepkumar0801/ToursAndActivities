using Isango.Entities;
using System.Collections.Generic;

namespace CacheManager.Contract
{
    public interface INetPriceCacheManager
    {
        string LoadNetPriceMasterData(List<NetPriceMasterData> netPriceDataList);

        List<NetPriceMasterData> GetNetPriceMasterData();
    }
}