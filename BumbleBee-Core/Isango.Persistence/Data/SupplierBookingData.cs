using Isango.Entities;
using Isango.Entities.Enums;
using Isango.Entities.RedeamV12;
using System;
using System.Collections.Generic;
using System.Data;
using Util;
using Constant = Isango.Persistence.Constants.Constants;

namespace Isango.Persistence.Data
{
    public class SupplierBookingData
    {
        public List<CitySightseeingMapping> GetCitySightseeingMappings(IDataReader reader)
        {
            var lstCitySightseeingMapping = new List<CitySightseeingMapping>();
            while (reader.Read())
            {
                CitySightseeingMapping citySightseeingMapping = null;
                try
                {
                    if (reader["SupplierCode"] != DBNull.Value
                        && reader["ChildFromAge"] != DBNull.Value
                        && reader["ChildToAge"] != DBNull.Value
                        && reader["PaxType"] != DBNull.Value
                    )
                    {
                        citySightseeingMapping = new CitySightseeingMapping
                        {
                            SupplierCode = DbPropertyHelper.StringPropertyFromRow(reader, "SupplierCode"),
                            ChildFromAge = DbPropertyHelper.Int32PropertyFromRow(reader, "ChildFromAge"),
                            ChildToAge = DbPropertyHelper.Int32PropertyFromRow(reader, "ChildToAge"),
                            PassengerType = DbPropertyHelper.StringPropertyFromRow(reader, "PaxType") == Constant.A ? PassengerType.Adult : PassengerType.Child
                        };
                        lstCitySightseeingMapping.Add(citySightseeingMapping);
                    }
                }
                catch (System.Exception ex)
                {
                    throw;
                }
            }

            return lstCitySightseeingMapping;
        }

        public List<SupplierData> GetRedeamV12SupplierData(IDataReader reader)
        {
            var lstSupplierData = new List<SupplierData>();
            while (reader.Read())
            {
                SupplierData supplierIdData = null;
                try
                {
                    if (reader["SupplierID"] != DBNull.Value
                        && reader["ContactEmail"] != DBNull.Value
                    )
                    {
                        supplierIdData = new SupplierData
                        {
                            SupplierId = DbPropertyHelper.StringPropertyFromRow(reader, "SupplierID"),
                            ContactEmail = DbPropertyHelper.StringPropertyFromRow(reader, "ContactEmail")
                        };
                        lstSupplierData.Add(supplierIdData);
                    }
                }
                catch (System.Exception ex)
                {
                    throw;
                }
            }

            return lstSupplierData;
        }
    }
}