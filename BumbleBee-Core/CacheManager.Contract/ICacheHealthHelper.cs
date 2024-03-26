using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheManager.Contract
{
    public interface ICacheHealthHelper
    {
        Task<bool> IsMongoHealthy();
    }
}
