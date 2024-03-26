using Isango.Entities.Rezdy;
using System.Collections.Generic;

namespace Isango.Persistence.Contract
{
    public interface IRezdyPersistence
    {
        void SaveRezdyAgeGroup(List<AgeGroupMapping> ageGroupMappings);

        void SaveRezdyProducts(List<RezdyProductDetail> rezdyProductDetails);
        void SaveBookingFields(List<BookingFieldMapping> bookingFieldMappings);
        void SaveExtraDetailsFields(List<ProductWiseExtraDetails> extraDetailsMappings);
        List<int> GetAllRezdySupportedSuppliers();
    }
}
