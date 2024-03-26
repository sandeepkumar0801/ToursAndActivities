using Hangfire;
using Isango.Mailer.ServiceContracts;
using Isango.Service;
using Isango.Service.Contract;
using Logger.Contract;
using System.Diagnostics;

namespace AsyncBooking.HangFire
{
    public class AyncFunctions
    {
        private readonly IAsyncBookingService _asyncBookingService;
        private readonly ICssBookingService _cssbookingservice;
        private readonly IMailDeliveryService _mailerservice;

        private readonly ILogger _log;

        public AyncFunctions(IAsyncBookingService asyncBookingService, ILogger log
           , ICssBookingService cssbookingservice
            , IMailDeliveryService mailerservice
            )
        {
            _asyncBookingService = asyncBookingService;
            _log = log;

          _cssbookingservice = cssbookingservice;

            _cssbookingservice = cssbookingservice;
            _mailerservice = mailerservice;

        }
        
        public void ProcessIncompleteBooking()
        {
            _log.Info("AsyncBooking.Function|ProcessIncompleteBooking|Starting process incomplete booking1");
            var watch = Stopwatch.StartNew();
          _asyncBookingService.ProcessIncompleteBooking();
            watch.Stop();
            _log.Info($"CacheLoader.Webjob|LoadElasticDestination|Loaded Elastic Destination in {watch.Elapsed}");
        }

        public void ProcessCssIncompleteBooking()
        {
            _log.Info("CssBooking.Function|ProcessIncompleteBooking|Starting process incomplete CssBooking");
            _cssbookingservice.ProcessCssIncompleteBooking();
            _log.Info($"CssBooking.Function|ProcessIncompleteBooking|Finished process incomplete CssBooking");
        }
        public void ProcessIncompleteCancellation()
        {
            _log.Info("CssBooking.Function|ProcessIncompleteCancellation|Starting process incomplete Cancellation");
            _cssbookingservice.ProcessIncompleteCancellation();
            _log.Info($"CssBooking.Function|ProcessIncompleteCancellation|Finished process incomplete Cancellation");
        }
        public void ProcessIncompleteRedemption()
        {
            _log.Info("CssBooking.Function|ProcessIncompleteRedemption|Starting process incomplete Redemption");
            _cssbookingservice.ProcessIncompleteRedemption();
            _log.Info($"CssBooking.Function|ProcessIncompleteRedemption|Finished process incomplete Redemption");
        }


        //public void ProcessIncompleteEmail()
        //{
        //    _log.Info("CssBooking.Function|ProcessIncompleteRedemption|Starting process incomplete Redemption");
        //    _mailerservice.ProcessMessagesFromQueue("emailqueue");
        //    _log.Info($"CssBooking.Function|ProcessIncompleteRedemption|Finished process incomplete Redemption");
        //}

    }
}
