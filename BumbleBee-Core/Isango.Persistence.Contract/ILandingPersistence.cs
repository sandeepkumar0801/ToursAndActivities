using Isango.Entities;
using System.Collections.Generic;

namespace Isango.Persistence.Contract
{
    public interface ILandingPersistence
    {
        List<LocalizedMerchandising> LoadLocalizedMerchandising();
    }
}