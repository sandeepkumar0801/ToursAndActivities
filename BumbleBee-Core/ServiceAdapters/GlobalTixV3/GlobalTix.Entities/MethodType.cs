using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAdapters.GlobalTixV3.GlobalTix.Entities
{
    public enum MethodType
    {
        Authentication = 0,
        ProductInfo = 1,
		PackageAvailability = 2,
		CreateBooking = 3,
        CancelBooking = 4,
		ProductOptions = 5,
		Availability = 6,
        Reservation=7,
        BookingDetail=8,
        CountryList=9,
        CityList = 10,
        ProductChanges=11,
        PackageOptions=12,
        ProductList=13
    }
}
