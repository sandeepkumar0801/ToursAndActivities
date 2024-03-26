// Decompiled with JetBrains decompiler
// Type: PreDepartMailer.Booking
// Assembly: DepartureMailer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6F2B8712-40D9-42A3-A0BE-2EB805C030D3
// Assembly location: C:\Users\VaishnaveeJaiswal\AppData\Local\Temp\Tuzotyb\750717edf3\PreDeparture\DepartureMailer.exe

using System;

#nullable disable
namespace PreDepartMailer
{
  public class Booking
  {
    public int BookingId { get; set; }

    public int BookedOptionId { get; set; }

    public string BookingReferenceNumber { get; set; }

    public int ServiceId { get; set; }

    public string ServiceName { get; set; }

    public int ServiceOptionInServiceId { get; set; }

    public string OptionName { get; set; }

    public DateTime BookingStartDate { get; set; }

    public string AmountonWireCard { get; set; }

    public string CurrencyIsoCode { get; set; }

    public string LanguageCode { get; set; }

    public int RegionId { get; set; }

    public string Customer_Email { get; set; }

    public int SMCPassengerId { get; set; }

    public string Promo_Code { get; set; }

    public string Actual_Url { get; set; }

    public string ImageName { get; set; }

    public string Website { get; set; }
  }
}
