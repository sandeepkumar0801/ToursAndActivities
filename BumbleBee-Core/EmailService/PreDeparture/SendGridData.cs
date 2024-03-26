// Decompiled with JetBrains decompiler
// Type: PreDepartMailer.SendGridData
// Assembly: DepartureMailer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6F2B8712-40D9-42A3-A0BE-2EB805C030D3
// Assembly location: C:\Users\VaishnaveeJaiswal\AppData\Local\Temp\Tuzotyb\750717edf3\PreDeparture\DepartureMailer.exe

using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading;
using Util;

#nullable disable
namespace PreDepartMailer
{
  public class SendGridData
  {
    public bool SMTPServerEmail(string htmlContent, string sendAddress, string languageCode)
    {
      try
      {
        string str = "Get ready for your upcoming trip!";
        string appSetting1 = ConfigurationManager.AppSettings["TestFromEmail"];
        string appSetting2 = ConfigurationManager.AppSettings["TestFromEmailNoReply"];
        if (languageCode.ToLowerInvariant() == "es")
          str = "Prepárese para su próximo viaje.";
        else if (languageCode.ToLowerInvariant() == "de")
          str = "Bereiten Sie sich auf Ihre nächste Reise vor!";
        string valuefromAppSettings1 = ConfigurationManagerHelper.GetValuefromAppSettings("SMTPPort");
        string valuefromAppSettings2 = ConfigurationManagerHelper.GetValuefromAppSettings("SMTPHost");
        string valuefromAppSettings3 = ConfigurationManagerHelper.GetValuefromAppSettings("SMTPFromEmail");
        string valuefromAppSettings4 = ConfigurationManagerHelper.GetValuefromAppSettings("SMTPPassword");
        string valuefromAppSettings5 = ConfigurationManagerHelper.GetValuefromAppSettings("SMTPUserName");
        MailMessage message = new MailMessage();
        SmtpClient smtpClient = new SmtpClient();
        message.From = new MailAddress(valuefromAppSettings3);
        string appSetting3 = ConfigurationManager.AppSettings["TestToEmail"];
        string empty = string.Empty;
        string addresses = string.IsNullOrEmpty(appSetting3) ? sendAddress : appSetting3;
        message.To.Add(addresses);
        if (!string.IsNullOrEmpty(appSetting1))
          message.Bcc.Add(appSetting1);
        message.Subject = str;
        message.IsBodyHtml = true;
        message.Body = htmlContent;
        smtpClient.Port = Convert.ToInt32(valuefromAppSettings1);
        smtpClient.Host = valuefromAppSettings2;
        smtpClient.EnableSsl = true;
        smtpClient.UseDefaultCredentials = false;
        smtpClient.Credentials = (ICredentialsByHost) new NetworkCredential(valuefromAppSettings5, valuefromAppSettings4);
        smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
        smtpClient.Send(message);
        return true;
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    public bool SendGridMail(string htmlContent, string sendAddress, string languageCode)
    {
      try
      {
        string appSetting1 = ConfigurationManager.AppSettings["SendGridKey"];
        string str = "Get ready for your upcoming trip!";
        string appSetting2 = ConfigurationManager.AppSettings["TestFromEmail"];
        string appSetting3 = ConfigurationManager.AppSettings["TestFromEmailNoReply"];
        if (languageCode.ToLowerInvariant() == "es")
          str = "Prepárese para su próximo viaje.";
        else if (languageCode.ToLowerInvariant() == "de")
          str = "Bereiten Sie sich auf Ihre nächste Reise vor!";
        string appSetting4 = ConfigurationManager.AppSettings["TestToEmail"];
        string empty = string.Empty;
        string email = string.IsNullOrEmpty(appSetting4) ? sendAddress : appSetting4;
        SendGridClient sendGridClient = new SendGridClient(appSetting1);
        SendGridMessage sendGridMessage = new SendGridMessage()
        {
          From = new EmailAddress(appSetting3),
          Subject = str,
          HtmlContent = htmlContent
        };
        sendGridMessage.AddTo(new EmailAddress(email));
        if (!string.IsNullOrEmpty(appSetting2))
          sendGridMessage.AddBcc(appSetting2);
        SendGridMessage msg = sendGridMessage;
        CancellationToken cancellationToken = new CancellationToken();
        return sendGridClient.SendEmailAsync(msg, cancellationToken).ConfigureAwait(false).GetAwaiter().GetResult().StatusCode.ToString().ToLower() == "Accepted".ToLower();
      }
      catch (Exception ex)
      {
        throw;
      }
    }
  }
}
