// Decompiled with JetBrains decompiler
// Type: DataDumping.WebJob.Program
// Assembly: DataDumpingConsole, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1F4B5367-06CA-45C3-B94F-AD4D7E3F50ED
// Assembly location: C:\Users\amalik\AppData\Local\Temp\Xykadyt\fa796c3894\Debug\app.publish\DataDumpingConsole.exe

using System;

namespace DataDumping.WebJob
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Functions functions = new Functions();
            switch (args[0])
            {
                case "DeleteExistingAvailabilityDetails":
                    functions.DeleteExistingAvailabilityDetails();
                    break;
                case "InitiateAgeDumping":
                    functions.InitiateAgeDumping();
                    break;
                case "InitiateGoogleFeeds":
                    functions.InitiateGoogleFeeds();
                    break;
                case "InitiateInventoryRealTimeUpdate":
                    functions.InitiateInventoryRealTimeUpdate();
                    break;
                case "InitiateOrderNotificationRealTimeUpdate":
                    functions.InitiateOrderNotificationRealTimeUpdate();
                    break;
                case "LoadAPIImagesToCloudinary":
                    functions.LoadAPIImagesToCloudinary();
                    break;
                case "LoadAotServiceAvailabilities":
                    functions.LoadAotServiceAvailabilities();
                    break;
                case "Bokun":
                    functions.LoadBokunServiceAvailabilities();
                    break;
                case "LoadCancellationPolicies":
                    functions.LoadCancellationPolicies();
                    break;
                case "FareHarbor":
                    functions.LoadFareHarborServiceAvailabilities();
                    break;
                case "GlobalTix":
                    functions.LoadGlobalTixServiceAvailabilities();
                    break;
                case "GoldenTours":
                    functions.LoadGoldenToursServiceAvailabilities();
                    break;
                case "GrayLineIceLand":
                    functions.LoadGrayLineIceLandServiceAvailabilities();
                    break;
                case "Hotelbeds":
                    functions.LoadHBApitudeServiceAvailabilities();
                    break;
                case "Isango":
                    functions.LoadIsangoServiceAvailabilities();
                    break;
                case "MoulinRouge":
                    functions.LoadMoulinRougeServiceAvailabilities();
                    break;
                case "CitySightSeeing":
                    functions.LoadNewCitySightSeeingServiceAvailabilities();
                    break;
                case "Prio":
                    functions.LoadPrioServiceAvailabilities();
                    break;
                case "Redeam":
                    functions.LoadRedeamServiceAvailabilities();
                    break;
                case "Rezdy":
                    functions.LoadRezdyServiceAvailabilities();
                    break;
                case "Tiqets":
                    functions.LoadTiqetsServiceAvailabilities();
                    break;
                case "TourCMS":
                    functions.LoadTourCMSServiceAvailabilities();
                    break;
                case "Ventrata":
                    functions.LoadVentrataAvailabilities();
                    break;
                case "fhb":
                    functions.LoadFareHarborServiceAvailabilities();
                    break;
                case "SendAbandonCartEmails":
                    functions.SendAbandonCartEmails();
                    break;
                case "ElasticProduct":
                    functions.ElasticProductDataSave();
                    break;
                default:
                    Console.WriteLine("Error: Did Not Match any Function");
                    break;
            }
        }
    }
}
