using EmailSuitConsole.Models;
using EmailSuitConsole.Service;
using FeefoDownloader;
using Hangfire;
using Logger.Contract;
using PreDepartMailer;
using System.Diagnostics;


namespace EmailSuitConsole
{
    public class JobSchedulingService
    {
        private readonly UserEmailOptions _userEmailOptions;
        private readonly IEmailService _emailService;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly DataAccess _dataAccess;
        private readonly DepartMethod _departMethod;
        private readonly Engine _engine;
        private readonly ILogger _log;


        public JobSchedulingService(
            UserEmailOptions userEmailOptions,
            IEmailService emailService,
            IBackgroundJobClient backgroundJobClient,
            DataAccess dataAccess,
            DepartMethod departMethod,
            Engine engine,
            ILogger log
            )
        {
            _userEmailOptions = userEmailOptions;
            _emailService = emailService;
            _backgroundJobClient = backgroundJobClient;
            _dataAccess = dataAccess;
            _departMethod = departMethod;
            _engine = engine;
            _log = log;
        }

        [JobDisplayName("SendtiqetStatusEmail")]
        public void SendtiqetStatusEmail()
        {
            _log.Info("EmailService.Function|SendtiqetStatusEmail|Starting EmailSendingFor Tiqet");
            var watch = Stopwatch.StartNew();
            _emailService.SendtiqetStatusEmail(_userEmailOptions);
             watch.Stop();
            _log.Info($"EmailService.Function|SendtiqetStatusEmail|Completed EmailSendingFor Tiqet {watch.Elapsed}");
        }
        
        [JobDisplayName("PrioLongTermStatusEmail")]
        public void SendPrioStatusEmail()
        {
            _log.Info("EmailService.Function|SendPrioStatusEmail|Starting EmailSendingFor Prio");
            var watch = Stopwatch.StartNew();
            _emailService.SendPrioStatusEmail(_userEmailOptions);
            watch.Stop();
            _log.Info($"EmailService.Function|SendPrioStatusEmail|Completed EmailSendingFor Prio {watch.Elapsed}");
        }
        
        [JobDisplayName("TiqetStatusEmail")]

        public void SendTiqetChange()
        {
            _log.Info("EmailService.Function|SendTiqetChange|Starting EmailSendingFor Tiqet");
            var watch = Stopwatch.StartNew();
            _emailService.SendTiqetChange(_userEmailOptions);
            watch.Stop();
            _log.Info($"EmailService.Function|SendTiqetChange|Completed EmailSendingFor Tiqet {watch.Elapsed}");
        }

        [JobDisplayName("TiqetVariantEmail")]

        public void SendVariantChange()
        {
            _log.Info("EmailService.Function|SendVariantChange|Starting EmailSendingFor Tiqet");
            var watch = Stopwatch.StartNew();
            _emailService.SendVariantChange(_userEmailOptions);
            watch.Stop();
            _log.Info($"EmailService.Function|SendVariantChange|Completed EmailSendingFor Tiqet {watch.Elapsed}");
        }

        [JobDisplayName("TourCMSStatusEmail")]

        public void SendTourCMSChange()
        {
            _log.Info("EmailService.Function|SendTourCMSChange|Starting EmailSendingFor TourCMS");
            var watch = Stopwatch.StartNew();
            _emailService.SendTourCMSChange(_userEmailOptions);
            watch.Stop();
            _log.Info($"EmailService.Function|SendTourCMSChange|Completed EmailSendingFor TourCMS {watch.Elapsed}");
        }

        [JobDisplayName("RaynaStatusEmail")]

        public void SendRaynaChange()
        {
            _log.Info("EmailService.Function|SendRaynaChange|Starting EmailSendingFor Rayna");
            var watch = Stopwatch.StartNew();
            _emailService.SendRaynaChange(_userEmailOptions);
            watch.Stop();
            _log.Info($"EmailService.Function|SendRaynaChange|Completed EmailSendingFor Rayna {watch.Elapsed}");
        }
        [JobDisplayName("GlobalTixV3StatusEmail")]

        public void SendGlobalTixV3Change()
        {
            _log.Info("EmailService.Function|SendGlobalTixV3Change|Starting EmailSendingFor GlobalTixV3");
            var watch = Stopwatch.StartNew();
            _emailService.SendGlobalTixV3Change(_userEmailOptions);
            watch.Stop();
            _log.Info($"EmailService.Function|SendGlobalTixV3Change|Completed EmailSendingFor GlobalTixV3 {watch.Elapsed}");
        }


        [JobDisplayName("TourCMSPaxEmail")]

        public void SendTourCMSPaxChange()
        {
            _log.Info("EmailService.Function|SendTourCMSPaxChange|Starting EmailSendingFor TourCMS");
            var watch = Stopwatch.StartNew();
            _emailService.SendTourCMSLabelChange(_userEmailOptions);
            watch.Stop();
            _log.Info($"EmailService.Function|SendTourCMSPaxChange|Completed EmailSendingFor TourCMS {watch.Elapsed}");
        }



        public void SendRaynaOptionsChange()
        {
            _log.Info("EmailService.Function|SendRaynaOptionsChange|Starting EmailSendingFor RaynaOptions");
            var watch = Stopwatch.StartNew();
            _emailService.SendRaynaOptionsChange(_userEmailOptions);
            watch.Stop();
            _log.Info($"EmailService.Function|SendRaynaOptionsChange|Completed EmailSendingFor RaynaOptions {watch.Elapsed}");
        }
        [JobDisplayName("GlobalTixV3StatusEmail")]

        public void SendGlobalTixV3OptionsChange()
        {
            _log.Info("EmailService.Function|SendGlobalTixV3OptionsChange|Starting EmailSendingFor GlobalTixV3Options");
            var watch = Stopwatch.StartNew();
            _emailService.SendGlobalTixV3OptionsChange(_userEmailOptions);
            watch.Stop();
            _log.Info($"EmailService.Function|SendGlobalTixV3OptionsChange|Completed EmailSendingFor GlobalTixV3Options {watch.Elapsed}");
        }


        [JobDisplayName("FeefoDownloaderReviews")]

        public void FeefoDownloaderReviews()
        {
            _log.Info("EmailService.Function|FeefoDownloaderReviews|Starting FeefoDownloaderReviews");
            var watch = Stopwatch.StartNew();
            _engine.Process();
            watch.Stop();
            _log.Info($"EmailService.Function|FeefoDownloaderReviews|Completed FeefoDownloaderReviews {watch.Elapsed}");
        }

        [JobDisplayName("PreDepartureMailSave")]

        public void PreDepartureMailSave()
        {
            _log.Info("EmailService.Function|PreDepartureMailSave|Starting PreDepartureMailSave");
            var watch = Stopwatch.StartNew();
            _departMethod.Save();
            watch.Stop();
            _log.Info($"EmailService.Function|PreDepartureMailSave|Completed PreDepartureMailSave {watch.Elapsed}");
        }
    }

}
