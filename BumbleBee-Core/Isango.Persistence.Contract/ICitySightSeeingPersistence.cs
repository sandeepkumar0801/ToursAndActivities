using Isango.Entities.NewCitySightSeeing;
using System.Collections.Generic;
using Product = Isango.Entities.NewCitySightSeeing.Product;

namespace Isango.Persistence.Contract
{
    public interface ICitySightSeeingPersistence
    {
		void SaveProductList(List<Product> products);

        void SaveProductVariantList(List<ProductVariant> products);
    }
}
