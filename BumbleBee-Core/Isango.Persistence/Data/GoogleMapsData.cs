using Isango.Entities.GoogleMaps;
using System.Data;
using Util;

namespace Isango.Persistence.Data
{
    public class GoogleMapsData
    {
        public MerchantFeed GetMerchantData(IDataReader reader)
        {
            return new MerchantFeed()
            {
                Supplierid = DbPropertyHelper.StringPropertyFromRow(reader, "Supplierid"),
                Addresscity = DbPropertyHelper.StringPropertyFromRow(reader, "Addresscity"),
                Addressline1 = DbPropertyHelper.StringPropertyFromRow(reader, "Addressline1"),
                Addressline2 = DbPropertyHelper.StringPropertyFromRow(reader, "Addressline2"),
                Addresspostcode = DbPropertyHelper.StringPropertyFromRow(reader, "Addresspostcode"),
                Addresstelephonenumber = DbPropertyHelper.StringPropertyFromRow(reader, "Addresstelephonenumber"),
                Countryisocode = DbPropertyHelper.StringPropertyFromRow(reader, "Countryisocode"),
                Countryname = DbPropertyHelper.StringPropertyFromRow(reader, "Countryname"),
                Suppliername = DbPropertyHelper.StringPropertyFromRow(reader, "Suppliername"),
                Category = DbPropertyHelper.StringPropertyFromRow(reader, "Category"),
            };
        }

        public AssignedServiceMerchant GetAssignedServiceMerchant(IDataReader reader)
        {
            return new AssignedServiceMerchant()
            {
                ActivityId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "serviceid"),
                MerchantId = DbPropertyHelper.StringPropertyFromRow(reader, "SUPPLIERID")
            };
        }

        public PassengerType GetPassengerType(IDataReader reader)
        {
            return new PassengerType()
            {
                PassengerTypeId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "passengertypeid"),
                PassengerTypeName = DbPropertyHelper.StringPropertyFromRowWithTrim(reader, "passengertypename"),
                LanguageCode = DbPropertyHelper.StringPropertyFromRowWithTrim(reader, "languagecode")
            };
        }
    }
}