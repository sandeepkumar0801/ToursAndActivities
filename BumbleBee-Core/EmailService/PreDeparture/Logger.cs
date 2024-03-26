// Decompiled with JetBrains decompiler
// Type: PreDepartMailer.Logger
// Assembly: DepartureMailer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6F2B8712-40D9-42A3-A0BE-2EB805C030D3
// Assembly location: C:\Users\VaishnaveeJaiswal\AppData\Local\Temp\Tuzotyb\750717edf3\PreDeparture\DepartureMailer.exe

using System;
using System.Configuration;
using System.IO;

#nullable disable
namespace PreDepartMailer
{
  public static class Logger
  {
    public static void WriteErrorLog(Exception ex)
    {
      try
      {
        if (!(ConfigurationManager.AppSettings["DisableLogFileError"] == "0"))
          return;
        StreamWriter streamWriter = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\LogFileError.txt", true);
        streamWriter.WriteLine(DateTime.Now.ToString() + ": " + ex.Source.ToString().Trim() + "; " + ex.Message.ToString().Trim());
        streamWriter.Flush();
        streamWriter.Close();
      }
      catch (Exception ex1)
      {
        throw;
      }
    }

    public static void WriteMessageLog(string Message)
    {
      try
      {
        if (!(ConfigurationManager.AppSettings["DisableLogFile"] == "0"))
          return;
        StreamWriter streamWriter = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\LogFile.txt", true);
        streamWriter.WriteLine(DateTime.Now.ToString() + ": " + Message);
        streamWriter.Flush();
        streamWriter.Close();
      }
      catch (Exception ex)
      {
        throw;
      }
    }
  }
}
