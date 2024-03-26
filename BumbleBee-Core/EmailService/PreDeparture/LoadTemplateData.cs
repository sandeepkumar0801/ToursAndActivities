// Decompiled with JetBrains decompiler
// Type: PreDepartMailer.LoadTemplateData
// Assembly: DepartureMailer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6F2B8712-40D9-42A3-A0BE-2EB805C030D3
// Assembly location: C:\Users\VaishnaveeJaiswal\AppData\Local\Temp\Tuzotyb\750717edf3\PreDeparture\DepartureMailer.exe

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

#nullable disable
namespace PreDepartMailer
{
  public class LoadTemplateData
  {
    public string AssignDataBaseDataToTemplate(
      string templateText,
      List<Booking> booking,
      List<AgeDescription> ageDescription,
      List<CrossSell> crossSell,
      List<Blogs> blogs,
      string imagepath,
      string languageCode)
    {
      AssignHTML assignHtml = new AssignHTML();
      StringBuilder stringBuilder1 = new StringBuilder(templateText);
      string promoCode = booking != null ? booking.FirstOrDefault<Booking>()?.Promo_Code : (string) null;
      if (blogs != null)
      {
        Blogs blogs1 = blogs.FirstOrDefault<Blogs>();
        if (blogs1 != null)
        {
          string blogTitle = blogs1.BlogTitle;
        }
      }
      if (blogs != null)
      {
        Blogs blogs2 = blogs.FirstOrDefault<Blogs>();
        if (blogs2 != null)
        {
          string blogImage = blogs2.BlogImage;
        }
      }
      if (blogs != null)
      {
        Blogs blogs3 = blogs.FirstOrDefault<Blogs>();
        if (blogs3 != null)
        {
          string blogurl = blogs3.Blogurl;
        }
      }
      string dynamicImageStaticPath = "https://res.cloudinary.com/https-www-isango-com/image/upload/t_m_Prod/";
      string utmPath = "?utm_source=cross+sell&utm_medium=email&utm_campaign=Nov+2021";
      StringBuilder stringBuilder2 = new StringBuilder();
      stringBuilder1.Replace("##logo##", imagepath + "img/logo.png");
      stringBuilder1.Replace("##heroimg##", imagepath + "img/hero.jpg");
      stringBuilder1.Replace("##vouchercode##", promoCode);
      stringBuilder1.Replace("##ProductDetail##", assignHtml.ProductDetail(booking, ageDescription, dynamicImageStaticPath, utmPath, imagepath));
      stringBuilder1.Replace("##CrossSell##", assignHtml.CrossSell(imagepath, crossSell, dynamicImageStaticPath, utmPath));
      stringBuilder1.Replace("##CrossSellHeaderText##", crossSell != null ? crossSell.FirstOrDefault<CrossSell>()?.RegionName : (string) null);
      stringBuilder1.Replace("##image1##", imagepath + "img/isango.png");
      stringBuilder1.Replace("##imagefeefo##", imagepath + "img/feefo.png");
      string language = languageCode;
      if (string.IsNullOrEmpty(language))
        language = "en";
      stringBuilder1.Replace("##imageguide##", imagepath + "img/myBooking-" + language + ".png");
      stringBuilder1.Replace("##imagespacer##", imagepath + "img/spcr.gif");
      stringBuilder1.Replace("##BlogData##", assignHtml.BlogDetail(blogs, language));
      return stringBuilder1.ToString().Replace("???", "");
    }

    public string LoadTemplate(string templatePath)
    {
      FileStream fileStream = File.Open(templatePath, FileMode.Open);
      byte[] buffer = new byte[Convert.ToInt32(fileStream.Length)];
      fileStream.Read(buffer, 0, Convert.ToInt32(fileStream.Length));
      fileStream.Close();
      return Encoding.ASCII.GetString(new MemoryStream(buffer).ToArray());
    }
  }
}
