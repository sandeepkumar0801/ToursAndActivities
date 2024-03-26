using ServiceAdapters.PrioTicket.PrioTicket.Entities;
using System.Collections.Generic;

namespace Isango.Persistence.Contract
{
    public interface IPrioPersistence
    {
        void SyncDataBetweenDataBases();

        void SaveAllAgeGroups(List<Entities.ConsoleApplication.AgeGroup.Prio.AgeGroup> masterAgeGroups);
		
		void SavePrioRouteMaps(List<Entities.ConsoleApplication.RouteMap.Prio.RouteMap> masterRouteMaps);
		void SavePrioProductDetails(List<Entities.ConsoleApplication.AgeGroup.Prio.ProductDetails> masterProductDetails);
        void SavePrioTicketList(TicketListRs prioTicketList);
    }
}