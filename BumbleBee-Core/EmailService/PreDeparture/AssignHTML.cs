// Decompiled with JetBrains decompiler
// Type: PreDepartMailer.AssignHTML
// Assembly: DepartureMailer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6F2B8712-40D9-42A3-A0BE-2EB805C030D3
// Assembly location: C:\Users\VaishnaveeJaiswal\AppData\Local\Temp\Tuzotyb\750717edf3\PreDeparture\DepartureMailer.exe

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#nullable disable
namespace PreDepartMailer
{
  public class AssignHTML
  {
    public string ProductDetail(
      List<Booking> bookingList,
      List<AgeDescription> ageDescription,
      string dynamicImageStaticPath,
      string utmPath,
      string imagepath)
    {
      StringBuilder stringBuilder = new StringBuilder();
      string str1 = utmPath;
      int num = 0;
      foreach (Booking booking in bookingList)
      {
        Booking item = booking;
        string serviceName = item?.ServiceName;
        string optionName = item?.OptionName;
        string str2 = item?.CurrencyIsoCode + " " + (object) Math.Round(Convert.ToDecimal(item?.AmountonWireCard), 2);
        string str3 = item.BookingStartDate.ToString("dd MMM yyyy");
        string languageCode = item?.LanguageCode;
        string str4 = "Tour: ";
        string str5 = "Guests: ";
        string str6 = "Travel date: ";
        string str7 = "Price: ";
        string str8 = item?.Actual_Url;
        if (languageCode.ToLowerInvariant() == "es")
        {
          if (!str8.ToLower().Contains("es"))
            str8 = "/es" + str8;
          str4 = "Tour: ";
          str5 = "Invitados: ";
          str6 = "Fecha de viaje: ";
          str7 = "Precio: ";
        }
        else if (languageCode.ToLowerInvariant() == "de")
        {
          if (!str8.ToLower().Contains("de"))
            str8 = "/de" + str8;
          str4 = "Touren: ";
          str5 = "Gäste: ";
          str6 = "Reisedatum: ";
          str7 = "Preis: ";
        }
        List<string> values;
        if (ageDescription == null)
        {
          values = (List<string>) null;
        }
        else
        {
          IEnumerable<AgeDescription> source1 = ageDescription.Where<AgeDescription>((Func<AgeDescription, bool>) (x => x.BookedOptionId == item.BookedOptionId));
          if (source1 == null)
          {
            values = (List<string>) null;
          }
          else
          {
            IEnumerable<string> source2 = source1.Select<AgeDescription, string>((Func<AgeDescription, string>) (x => x.AgeGroupDesc));
            values = source2 != null ? source2.ToList<string>() : (List<string>) null;
          }
        }
        string str9 = string.Join(",", (IEnumerable<string>) values);
        string empty = string.Empty;
        string str10 = item?.ImageName?.Replace(" ", "%20");
        string str11 = "www.hop-on-hop-off-bus.com";
        if (!string.IsNullOrEmpty(item?.Website))
          str11 = item?.Website;
        string str12 = string.IsNullOrEmpty(str10) ? "'No Image in Database'" : "'" + dynamicImageStaticPath + str10 + "'";
        string str13 = "'https://" + str11 + str8 + str1 + "'";
        if (bookingList.Count > 1 && num != 0)
        {
          string str14 = "'" + imagepath + "img/spcr.gif'";
          stringBuilder.Append("<tr><td height='15' style='font-size:0.0001em;line-height:1px; height:15px;'><img src=" + str14 + " height='15' width='1' alt=''></td></tr>");
        }
        stringBuilder.Append("<tr>");
        stringBuilder.Append("<td class='stack-column-center' style='vertical-align:top;'>");
        stringBuilder.Append("<a href =" + str13 + " target='_blank'><img src = " + str12 + " width='300' height='auto' alt='!isango' border='0' class='fluid'></a>");
        stringBuilder.Append("</td>");
        stringBuilder.Append("<td class='stack-column-center' style='vertical-align:top;'>");
        stringBuilder.Append("<table cellspacing='0' cellpadding='0' border='0'>");
        stringBuilder.Append("<tr>");
        stringBuilder.Append("<td style='font-family: Poppins, sans-serif; padding: 0px 0 0 10px; mso-height-rule: exactly; line-height: 24px; color: #212121; text-align: left;' class='center-on-narrow'>");
        stringBuilder.Append("<a href =" + str13 + " style='text-decoration: none;color: #212121;' target='_blank'>");
        stringBuilder.Append("<strong style='font-size: 16px;'>" + serviceName + "</strong>");
        stringBuilder.Append("<p style='font-size: 14px;line-height:19px;margin-top: 12px;margin-bottom:0;'><strong>" + str4 + "</strong>" + optionName + "</p>");
        stringBuilder.Append("<p style='font-size: 14px;line-height:19px;margin-top: 12px;margin-bottom:0;'><strong>" + str5 + "</strong>" + str9 + "</p>");
        stringBuilder.Append("<p style='font-size: 14px;line-height:19px;margin-top: 12px;margin-bottom:0;'><strong>" + str6 + "</strong>" + str3 + "</p>");
        stringBuilder.Append("<p style='font-size: 14px;line-height:19px;margin-top: 12px;margin-bottom:0;'><strong>" + str7 + "</strong>" + str2 + "</p>");
        stringBuilder.Append("</a>");
        stringBuilder.Append("</td>");
        stringBuilder.Append("</tr>");
        stringBuilder.Append("</table>");
        stringBuilder.Append("</td>");
        stringBuilder.Append("</tr>");
        ++num;
      }
      return stringBuilder.ToString();
    }

    public string CrossSell(
      string imagepath,
      List<PreDepartMailer.CrossSell> crossSellData,
      string dynamicImageStaticPath,
      string utmPath)
    {
      string str1 = utmPath;
      StringBuilder stringBuilder = new StringBuilder();
      int count = crossSellData.Count;
      for (int index1 = 0; index1 < count; index1 += 2)
      {
        stringBuilder.Append("<tr>");
        if (index1 < count && crossSellData[index1] != null)
        {
          string empty = string.Empty;
          string str2 = crossSellData[index1]?.ImageName?.Replace(" ", "%20");
          string str3 = string.IsNullOrEmpty(str2) ? "'No Image in Database'" : "'" + dynamicImageStaticPath + str2 + "'";
          int int32 = Decimal.ToInt32(Convert.ToDecimal(crossSellData[index1].Rating));
          string str4 = this.RatingImage(int32 > 0 ? int32 * 10 : 0, imagepath);
          string languagecode = crossSellData[index1]?.Languagecode;
          string str5 = "'" + imagepath + "img/bookNow-" + languagecode + ".png'";
          string str6 = "www.isango.com";
          if (!string.IsNullOrEmpty(crossSellData[index1]?.Website))
            str6 = crossSellData[index1]?.Website;
          string str7 = "'https://" + str6 + crossSellData[index1].Actual_Url + str1 + "'";
          string str8 = crossSellData[index1].Currency + " " + (object) Math.Round(Convert.ToDecimal(crossSellData[index1].Price), 2);
          string serviceLongName = crossSellData[index1]?.ServiceLongName;
          stringBuilder.Append("<td style='width: 48%;vertical-align: top;' class='stack-column-center'>");
          stringBuilder.Append("<table cellspacing='0' cellpadding='0' border='0'>");
          stringBuilder.Append("<tr>");
          stringBuilder.Append("<td style ='padding: 10px; text-align: center'>");
          stringBuilder.Append("<a href=" + str7 + " target='_blank'><img src =" + str3 + " width ='290' height='auto' alt='!isango' border='0' style='border-radius: 10px 10px 0 0' class='fluid'></a>");
          stringBuilder.Append("</td>");
          stringBuilder.Append("</tr>");
          stringBuilder.Append("<tr>");
          stringBuilder.Append("<td style='font-family:Poppins, sans-serif; font-size: 12px; mso-height-rule: exactly; line-height: 20px; color: #212121; padding: 0 10px 10px; text-align: left;' class='center-on-narrow'>");
          stringBuilder.Append("<a href =" + str7 + " style='text-decoration: none;color: #212121;' target='_blank'><strong style=''>" + serviceLongName + "</strong></a>");
          stringBuilder.Append("</td>");
          stringBuilder.Append("</tr>");
          stringBuilder.Append("<tr>");
          stringBuilder.Append("<td style ='padding: 0 10px;'>");
          stringBuilder.Append("<table style= 'width: 100%;' cellspacing= '0' cellpadding= '0' border= '0' >");
          stringBuilder.Append("<tr>");
          stringBuilder.Append("<td style= 'font-family: Poppins, sans-serif; font-size: 12px; text-align:left;'><span style='color: #BBB;'>");
          stringBuilder.Append("From </span>" + str8 + "</td>");
          stringBuilder.Append("<td style='text-align:right;'>");
          stringBuilder.Append("<img src= " + str4 + " width='68' height= '12' alt= 'isango!' >");
          stringBuilder.Append("</td>");
          stringBuilder.Append("</tr>");
          stringBuilder.Append("</table>");
          stringBuilder.Append("</td>");
          stringBuilder.Append("</tr>");
          stringBuilder.Append("<tr>");
          stringBuilder.Append("<td style= 'padding: 10px; text-align: left;'>");
          stringBuilder.Append("<a href= " + str7 + " target= '_blank' >");
          stringBuilder.Append("<img src=" + str5 + " width ='91' height='24' alt= 'isango!'>");
          stringBuilder.Append("</a>");
          stringBuilder.Append("</td>");
          stringBuilder.Append("</tr>");
          stringBuilder.Append("</table>");
          stringBuilder.Append("</td>");
        }
        stringBuilder.Append("<td style ='width:4%;' class='vspcr column-hide'></td>");
        int index2 = index1 + 1;
        try
        {
          if (index2 < count)
          {
            if (crossSellData[index2] != null)
            {
              string empty = string.Empty;
              string str9 = crossSellData[index2]?.ImageName?.Replace(" ", "%20");
              string str10 = string.IsNullOrEmpty(str9) ? "'No Image in Database'" : "'" + dynamicImageStaticPath + str9 + "'";
              int int32 = Decimal.ToInt32(Convert.ToDecimal(crossSellData[index2].Rating));
              string str11 = this.RatingImage(int32 > 0 ? int32 * 10 : 0, imagepath);
              string languagecode = crossSellData[index2]?.Languagecode;
              string str12 = "'" + imagepath + "img/bookNow-" + languagecode + ".png'";
              string str13 = "www.isango.com";
              if (!string.IsNullOrEmpty(crossSellData[index2]?.Website))
                str13 = crossSellData[index2]?.Website;
              string str14 = "'https://" + str13 + crossSellData[index2]?.Actual_Url + str1 + "'";
              string str15 = crossSellData[index2]?.Currency + " " + (object) Math.Round(Convert.ToDecimal((object) crossSellData[index2]?.Price), 2);
              string serviceLongName = crossSellData[index2]?.ServiceLongName;
              stringBuilder.Append("<td style ='width:48%;vertical-align: top;' class='stack-column-center'>");
              stringBuilder.Append("<table cellspacing='0' cellpadding='0' border='0'>");
              stringBuilder.Append("<tr>");
              stringBuilder.Append("<td style ='padding:10px;text-align:center'>");
              stringBuilder.Append("<a href =" + str14 + " style='text-decoration: none;color: #212121;' target='_blank'><img src=" + str10 + " width='290' height='auto' alt='!isango' style='border-radius: 10px 10px 0 0' border='0' class='fluid'></a>");
              stringBuilder.Append("</td>");
              stringBuilder.Append("</tr>");
              stringBuilder.Append("<tr>");
              stringBuilder.Append("<td style='font-family:Poppins, sans-serif; font-size: 12px; mso-height-rule: exactly; line-height: 20px; color: #212121; padding: 0 10px 10px; text-align: left;'  class='center-on-narrow'>");
              stringBuilder.Append("<a href =" + str14 + " style='text-decoration: none;color: #212121;' target='_blank'><strong style=''>" + serviceLongName + "</strong></a>");
              stringBuilder.Append("</td>");
              stringBuilder.Append("</tr>");
              stringBuilder.Append("<tr>");
              stringBuilder.Append("<td style='padding:0 10px;'>");
              stringBuilder.Append("<table style= 'width: 100%;' cellspacing= '0' cellpadding= '0' border= '0'>");
              stringBuilder.Append("<tr>");
              stringBuilder.Append("<td style= 'font-family: Poppins, sans-serif;font-size: 12px;text-align: left;'><span ");
              stringBuilder.Append("style='color: #BBB;'> From </span>" + str15 + "</td>");
              stringBuilder.Append("<td style ='text-align: right;'>");
              stringBuilder.Append("<img src= " + str11 + " width= '68' height= '12' alt= 'isango!'>");
              stringBuilder.Append("</td>");
              stringBuilder.Append("</tr>");
              stringBuilder.Append("</table>");
              stringBuilder.Append("</td>");
              stringBuilder.Append("</tr>");
              stringBuilder.Append("<tr>");
              stringBuilder.Append("<td style= 'padding:10px;text-align:left;'>");
              stringBuilder.Append("<a href=" + str14 + ">");
              stringBuilder.Append("<img src= " + str12 + " width= '90' height= '24' alt= 'isango!'>");
              stringBuilder.Append("</a>");
              stringBuilder.Append("</td>");
              stringBuilder.Append("</tr>");
              stringBuilder.Append("</table>");
              stringBuilder.Append("</td>");
            }
          }
        }
        catch (Exception ex)
        {
          throw;
        }
      }
      return stringBuilder.ToString();
    }

    public string BlogDetail(List<Blogs> blogs, string language)
    {
      StringBuilder stringBuilder = new StringBuilder();
      Blogs blogs1 = blogs.FirstOrDefault<Blogs>();
      string str1 = "'" + blogs1.BlogImage + "'";
      string str2 = "'" + blogs1.Blogurl + "'";
      if (!string.IsNullOrEmpty(blogs1.BlogImage) && language == "en")
      {
        stringBuilder.Append("<table style='width: 100%;text-align:center;' cellspacing='0' cellpadding='0' border='0'>");
        stringBuilder.Append("<tr>");
        stringBuilder.Append("<td ");
        stringBuilder.Append("style='height: 20px; font-family: Poppins, sans-serif;  mso-height-rule: exactly; line-height: 20px; color: #212121;'>");
        stringBuilder.Append("<strong style='font-size: 20px;'> The Guidebook </strong>");
        stringBuilder.Append("</td>");
        stringBuilder.Append("</tr>");
        stringBuilder.Append("<tr>");
        stringBuilder.Append("<td ");
        stringBuilder.Append("style='height: 40px;font-family: Poppins, sans-serif;  mso-height-rule: exactly; line-height: 18px; color: #212121;'>");
        stringBuilder.Append("<strong style='font-size: 12px;'>" + blogs1.BlogTitle + "</strong>");
        stringBuilder.Append("</td>");
        stringBuilder.Append("</tr>");
        stringBuilder.Append("<tr>");
        stringBuilder.Append("<td>");
        stringBuilder.Append("<a href=" + str2 + " target='_blank'>");
        stringBuilder.Append("<img style='display:block;width:100%;height:auto;' src=" + str1 + " width='100%' height='auto'");
        stringBuilder.Append("alt='isango! guidebook'></a>");
        stringBuilder.Append("</td>");
        stringBuilder.Append("</tr>");
        stringBuilder.Append("</table>");
      }
      return stringBuilder.ToString();
    }

    public string RatingImage(int ratingValue, string imagepath)
    {
      string str = "'" + imagepath + "img/45.png'";
      if (ratingValue > 0)
        str = "'" + imagepath + "img/" + (object) ratingValue + ".png'";
      return str;
    }
  }
}
