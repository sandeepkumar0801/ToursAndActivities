// Decompiled with JetBrains decompiler
// Type: PreDepartMailer.MainBookingData
// Assembly: DepartureMailer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6F2B8712-40D9-42A3-A0BE-2EB805C030D3
// Assembly location: C:\Users\VaishnaveeJaiswal\AppData\Local\Temp\Tuzotyb\750717edf3\PreDeparture\DepartureMailer.exe

using System.Collections.Generic;

#nullable disable
namespace PreDepartMailer
{
  public class MainBookingData
  {
    public List<PreDepartMailer.Booking> Booking { get; set; }

    public List<PreDepartMailer.AgeDescription> AgeDescription { get; set; }

    public List<PreDepartMailer.CrossSell> CrossSell { get; set; }

    public List<PreDepartMailer.Blogs> Blogs { get; set; }
  }
}
