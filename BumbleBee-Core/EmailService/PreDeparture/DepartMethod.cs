using EmailSuitConsole.Data;
using EmailSuitConsole.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util;

namespace PreDepartMailer
{
   public class DepartMethod
    {
        private readonly EmailTemplateSettings _emailTemplateSettings;
        public DepartMethod(IOptions<EmailTemplateSettings> emailTemplateSettings)
        {
           _emailTemplateSettings = emailTemplateSettings.Value;
        }
        public void Save()
        {
            try
            {
                SendGridData sendGridData = new SendGridData();
                LoadTemplateData loadTemplateData = new LoadTemplateData();
                DatabaseService databaseService = new DatabaseService();
                Logger.WriteMessageLog("Start- Timer" + (object)DateTime.Now);
                Logger.WriteMessageLog("Start- Start getting Data from Database: " + (object)DateTime.Now);
                MainBookingData dataFromDatabase = databaseService.GetDataFromDatabase();
                Logger.WriteMessageLog("End- Get Data from Database Succcessfully: " + (object)DateTime.Now);
                List<List<Booking>> list = dataFromDatabase != null ? dataFromDatabase.Booking.GroupBy<Booking, int>((Func<Booking, int>)(u => u.BookingId)).Select<IGrouping<int, Booking>, List<Booking>>((Func<IGrouping<int, Booking>, List<Booking>>)(grp => grp.ToList<Booking>())).ToList<List<Booking>>() : (List<List<Booking>>)null;
                if (dataFromDatabase == null || dataFromDatabase?.Booking == null || dataFromDatabase == null)
                    return;
                int? nullable1 = dataFromDatabase.Booking?.Count;
                int num1 = 0;
                if (!(nullable1.GetValueOrDefault() > num1 & nullable1.HasValue) || list == null || dataFromDatabase?.CrossSell == null || dataFromDatabase == null)
                    return;
                nullable1 = dataFromDatabase.CrossSell?.Count;
                int num2 = 0;
                if (!(nullable1.GetValueOrDefault() > num2 & nullable1.HasValue))
                    return;
                foreach (List<Booking> bookingList in list)
                {
                    try
                    {
                        string appSetting = ConfigurationManagerHelper.GetValuefromAppSettings("ImagePath");
                        string languageCode = (bookingList != null ? bookingList.FirstOrDefault<Booking>()?.LanguageCode : (string)null) ?? "en";
                        string str = "PreDepartureTemplate\\" + languageCode;
                        string templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory) + str + ".html";
                        int? nullable2;
                        if (bookingList == null)
                        {
                            nullable1 = new int?();
                            nullable2 = nullable1;
                        }
                        else
                        {
                            Booking booking = bookingList.FirstOrDefault<Booking>();
                            if (booking == null)
                            {
                                nullable1 = new int?();
                                nullable2 = nullable1;
                            }
                            else
                                nullable2 = new int?(booking.BookingId);
                        }
                        int? bookingId = nullable2;
                        int? nullable3;
                        if (bookingList == null)
                        {
                            nullable1 = new int?();
                            nullable3 = nullable1;
                        }
                        else
                        {
                            Booking booking = bookingList.FirstOrDefault<Booking>();
                            if (booking == null)
                            {
                                nullable1 = new int?();
                                nullable3 = nullable1;
                            }
                            else
                                nullable3 = new int?(booking.RegionId);
                        }
                        int? regionID = nullable3;
                        string customerEmail = bookingList != null ? bookingList.FirstOrDefault<Booking>()?.Customer_Email : (string)null;
                        int? nullable4;
                        if (bookingList == null)
                        {
                            nullable1 = new int?();
                            nullable4 = nullable1;
                        }
                        else
                        {
                            Booking booking = bookingList.FirstOrDefault<Booking>();
                            if (booking == null)
                            {
                                nullable1 = new int?();
                                nullable4 = nullable1;
                            }
                            else
                                nullable4 = new int?(booking.SMCPassengerId);
                        }
                        int? smcpassengerid = nullable4;
                        Logger.WriteMessageLog("Start- Start Loading Template for bookingID:" + (object)bookingId);
                        string templateText = loadTemplateData.LoadTemplate(templatePath);
                        Logger.WriteMessageLog("End- Load Template Successfully for bookingID:" + (object)bookingId);
                        Logger.WriteMessageLog("Start- Assign Data in Template for bookingID:" + (object)bookingId);
                        List<AgeDescription> ageDescriptionList;
                        if (dataFromDatabase == null)
                        {
                            ageDescriptionList = (List<AgeDescription>)null;
                        }
                        else
                        {
                            List<AgeDescription> ageDescription = dataFromDatabase.AgeDescription;
                            if (ageDescription == null)
                            {
                                ageDescriptionList = (List<AgeDescription>)null;
                            }
                            else
                            {
                                IEnumerable<AgeDescription> source = ageDescription.Where<AgeDescription>((Func<AgeDescription, bool>)(x =>
                                {
                                    int bookingId1 = x.BookingId;
                                    int? nullable5 = bookingId;
                                    int valueOrDefault = nullable5.GetValueOrDefault();
                                    return bookingId1 == valueOrDefault & nullable5.HasValue;
                                }));
                                ageDescriptionList = source != null ? source.ToList<AgeDescription>() : (List<AgeDescription>)null;
                            }
                        }
                        List<AgeDescription> ageDescription1 = ageDescriptionList;
                        List<CrossSell> crossSellList;
                        if (dataFromDatabase == null)
                        {
                            crossSellList = (List<CrossSell>)null;
                        }
                        else
                        {
                            IEnumerable<CrossSell> source = dataFromDatabase.CrossSell.Where<CrossSell>((Func<CrossSell, bool>)(x =>
                            {
                                int bookingId2 = x.BookingId;
                                int? nullable6 = bookingId;
                                int valueOrDefault = nullable6.GetValueOrDefault();
                                return bookingId2 == valueOrDefault & nullable6.HasValue;
                            }));
                            crossSellList = source != null ? source.ToList<CrossSell>() : (List<CrossSell>)null;
                        }
                        List<CrossSell> crossSell = crossSellList;
                        List<Blogs> blogsList;
                        if (dataFromDatabase == null)
                        {
                            blogsList = (List<Blogs>)null;
                        }
                        else
                        {
                            IEnumerable<Blogs> source = dataFromDatabase.Blogs.Where<Blogs>((Func<Blogs, bool>)(x =>
                            {
                                int regionid = x.Regionid;
                                int? nullable7 = regionID;
                                int valueOrDefault = nullable7.GetValueOrDefault();
                                return regionid == valueOrDefault & nullable7.HasValue;
                            }));
                            blogsList = source != null ? source.ToList<Blogs>() : (List<Blogs>)null;
                        }
                        List<Blogs> blogs = blogsList;
                        string template = loadTemplateData.AssignDataBaseDataToTemplate(templateText, bookingList, ageDescription1, crossSell, blogs, appSetting, languageCode);
                        Logger.WriteMessageLog("End- Successfully Assign Data in Template for bookingID:" + (object)bookingId);
                        Logger.WriteMessageLog("Start- SendGrid for bookingID:" + (object)bookingId);
                        bool flag = !(ConfigurationManagerHelper.GetValuefromAppSettings("MailTypeSMTP") == "1") ? sendGridData.SendGridMail(template, customerEmail, languageCode) : sendGridData.SMTPServerEmail(template, customerEmail, languageCode);
                        Logger.WriteMessageLog("End-  SendGrid - Status:" + flag.ToString() + " for bookingID" + (object)bookingId);
                        if (flag)
                        {
                            Logger.WriteMessageLog("Start- Update data in database for:" + (object)bookingId);
                            Logger.WriteMessageLog("End- Update Status:=" + databaseService.UpdateStatus(customerEmail, smcpassengerid, bookingId).ToString() + "for bookingID:" + (object)bookingId);
                        }
                        Logger.WriteMessageLog("End- Timer" + (object)DateTime.Now);
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteErrorLog(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(ex);
            }
        }
    }
}
