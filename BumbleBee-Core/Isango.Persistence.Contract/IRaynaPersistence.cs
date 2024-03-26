
using Isango.Entities.Rayna;
using ServiceAdapters.Rayna.Rayna.Entities;
using System.Collections.Generic;

namespace Isango.Persistence.Contract
{
    public interface IRaynaPersistence
    {
        void SaveCountryCity(List<CountryCity> countrycity);
        void SaveTourList(List<ResultTour> tourData);
        void SaveTourListById(List<ResultTourStaticDataById> tourDataById);
        void SaveTourOptions(List<Touroption> tourOption);
        void SaveTourOptionTransferTime(List<TransferTimeTourOption> transferTimeTourOption);
    }
}
