// Decompiled with JetBrains decompiler
// Type: PreDepartMailer.CrossSell
// Assembly: DepartureMailer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6F2B8712-40D9-42A3-A0BE-2EB805C030D3
// Assembly location: C:\Users\VaishnaveeJaiswal\AppData\Local\Temp\Tuzotyb\750717edf3\PreDeparture\DepartureMailer.exe

using System;

#nullable disable
namespace PreDepartMailer
{
  public class CrossSell
  {
    public int BookingId { get; set; }

    public int RegionId { get; set; }

    public string RegionName { get; set; }

    public string Languagecode { get; set; }

    public string AffiliateKey { get; set; }

    public string AffiliateName { get; set; }

    public int ServiceId { get; set; }

    public string ServiceLongName { get; set; }

    public Decimal Price { get; set; }

    public string Currency { get; set; }

    public string Rating { get; set; }

    public string Website { get; set; }

    public string Actual_Url { get; set; }

    public string ImageName { get; set; }
  }
}
