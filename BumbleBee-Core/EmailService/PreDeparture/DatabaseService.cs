// Decompiled with JetBrains decompiler
// Type: PreDepartMailer.DatabaseService
// Assembly: DepartureMailer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6F2B8712-40D9-42A3-A0BE-2EB805C030D3
// Assembly location: C:\Users\VaishnaveeJaiswal\AppData\Local\Temp\Tuzotyb\750717edf3\PreDeparture\DepartureMailer.exe

using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Util;

#nullable disable
namespace PreDepartMailer
{
    public class DatabaseService
    {
        protected Database IsangoDataBaseLive => new SqlDatabase(ConfigurationManagerHelper.GetValuefromConfig("IsangoLiveEntities"));
    public MainBookingData GetDataFromDatabase()
    {
      MainBookingData dataFromDatabase = new MainBookingData();
      List<Booking> bookingList = new List<Booking>();
      List<AgeDescription> ageDescriptionList = new List<AgeDescription>();
      List<CrossSell> crossSellList = new List<CrossSell>();
      List<Blogs> blogsList = new List<Blogs>();
      try
      {
        using (DbCommand storedProcCommand = this.IsangoDataBaseLive.GetStoredProcCommand("usp_get_microsite_predeparture_detail"))
        {
          storedProcCommand.CommandType = CommandType.StoredProcedure;
          using (IDataReader reader = this.IsangoDataBaseLive.ExecuteReader(storedProcCommand))
          {
            while (reader.Read())
            {
              Booking booking = new Booking()
              {
                AmountonWireCard = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "AmountonWireCard"),
                BookingId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "BookingId"),
                BookingReferenceNumber = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "BookingReferenceNumber"),
                BookingStartDate = DbPropertyHelper.DateTimePropertyFromRow(reader, "BookingStartDate"),
                CurrencyIsoCode = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "CurrencyIsoCode"),
                OptionName = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "OptionName"),
                ServiceId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "ServiceId"),
                ServiceOptionInServiceId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "ServiceOptionInServiceId"),
                Actual_Url = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "Actual_Url"),
                BookedOptionId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "BookedOptionId"),
                Customer_Email = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "Customer_Email"),
                ImageName = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "ImageName"),
                LanguageCode = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "LanguageCode"),
                Promo_Code = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "Promo_Code"),
                RegionId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "RegionId"),
                ServiceName = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "ServiceName"),
                SMCPassengerId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "SMCPassengerId"),
                Website = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "Website")
              };
              bookingList.Add(booking);
            }
            reader.NextResult();
            while (reader.Read())
            {
              AgeDescription ageDescription = new AgeDescription()
              {
                BookingId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "BookingId"),
                AgeGroupDesc = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "AgeGroupDesc"),
                BookingReferenceNumber = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "BookingReferenceNumber"),
                BookedOptionId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "BookedOptionId")
              };
              ageDescriptionList.Add(ageDescription);
            }
            reader.NextResult();
            while (reader.Read())
            {
              CrossSell crossSell = new CrossSell()
              {
                BookingId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "BookingId"),
                ServiceId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "ServiceId"),
                AffiliateKey = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "AffiliateKey"),
                AffiliateName = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "AffiliateName"),
                Currency = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "Currency"),
                Languagecode = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "Languagecode"),
                Price = DbPropertyHelper.DecimalDefaultNullablePropertyFromRow(reader, "Price"),
                Rating = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "Rating"),
                RegionId = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "RegionId"),
                RegionName = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "RegionName"),
                ServiceLongName = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "ServiceLongName"),
                Website = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "Website"),
                ImageName = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "ImageName"),
                Actual_Url = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "Actual_Url")
              };
              crossSellList.Add(crossSell);
            }
            reader.NextResult();
            while (reader.Read())
            {
              Blogs blogs = new Blogs()
              {
                BlogImage = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "BlogImage"),
                BlogTitle = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "BlogTitle"),
                Blogurl = DbPropertyHelper.StringDefaultPropertyFromRow(reader, "Blogurl"),
                Regionid = DbPropertyHelper.Int32DefaultPropertyFromRow(reader, "Regionid")
              };
              blogsList.Add(blogs);
            }
          }
        }
      }
      catch (Exception ex)
      {
        throw;
      }
      dataFromDatabase.Booking = new List<Booking>();
      dataFromDatabase.AgeDescription = new List<AgeDescription>();
      dataFromDatabase.Blogs = new List<Blogs>();
      dataFromDatabase.CrossSell = new List<CrossSell>();
      dataFromDatabase.Booking = bookingList;
      dataFromDatabase.AgeDescription = ageDescriptionList;
      dataFromDatabase.Blogs = blogsList;
      dataFromDatabase.CrossSell = crossSellList;
      return dataFromDatabase;
    }

    public bool UpdateStatus(string emailid, int? smcpassengerid, int? bookingid)
    {
      bool flag = false;
      try
      {
        using (DbCommand storedProcCommand = this.IsangoDataBaseLive.GetStoredProcCommand("usp_ins_microsite_predeparture_detail"))
        {
          this.IsangoDataBaseLive.AddInParameter(storedProcCommand, "@emailid", DbType.String, (object) emailid);
          this.IsangoDataBaseLive.AddInParameter(storedProcCommand, "@smcpassengerid", DbType.String, (object) smcpassengerid);
          this.IsangoDataBaseLive.AddInParameter(storedProcCommand, "@bookingid", DbType.String, (object) bookingid);
          this.IsangoDataBaseLive.ExecuteNonQuery(storedProcCommand);
          flag = true;
        }
        return flag;
      }
      catch (Exception ex)
      {
        throw;
      }
    }
  }
}
