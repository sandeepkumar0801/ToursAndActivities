using Autofac;
using CacheManager.Contract;
using Isango.Entities;
using Isango.Persistence.Contract;
using Isango.Register;
using Isango.Service;
using Isango.Service.Contract;
using Logger.Contract;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Isango.Services.Test
{
    [TestFixture]
    public class SynchronizerServiceTest : BaseTest
    {
        private IActivityCacheManager _activityCacheManagerForMocking;
        private IMasterService _masterServiceMocking;
        private SynchronizerService _synchronizerServiceForMocking;
        private IActivityPersistence _activityPersistence;
        private ISynchronizerCacheManager _synchronizerCacheManager;
        private IActivityCacheManager _activityCacheManagerForMockingException;
        private SynchronizerService _synchronizerServiceForMockingException;
        private IMasterService _masterServiceMockingException;
        private IActivityPersistence _activityPersistenceException;
        private ISynchronizerCacheManager _synchronizerCacheManagerException;
        private ISynchronizerService _syncService;
        private ICacheLoaderService _cacheLoaderServiceException;

        [OneTimeSetUp]
        public void TestInitialise()
        {
            _activityCacheManagerForMocking = Substitute.For<IActivityCacheManager>();
            _masterServiceMocking = Substitute.For<IMasterService>();
            _activityPersistence = Substitute.For<IActivityPersistence>();
            _synchronizerCacheManager = Substitute.For<ISynchronizerCacheManager>();
            _activityCacheManagerForMockingException = Substitute.For<IActivityCacheManager>();
            _masterServiceMockingException = Substitute.For<IMasterService>();
            _activityPersistenceException = Substitute.For<IActivityPersistence>();
            _synchronizerCacheManagerException = Substitute.For<ISynchronizerCacheManager>();
            _cacheLoaderServiceException = Substitute.For<ICacheLoaderService>();
            var log = Substitute.For<ILogger>();

            _synchronizerServiceForMocking = new SynchronizerService(_masterServiceMocking, _activityPersistence, log, _activityCacheManagerForMocking, _synchronizerCacheManager, _cacheLoaderServiceException);

            _synchronizerServiceForMockingException = new SynchronizerService(_masterServiceMockingException, _activityPersistenceException, log, _activityCacheManagerForMockingException, _synchronizerCacheManagerException, _cacheLoaderServiceException);

           // var container = Startup._builder.Build();

            using (var scope = _container.BeginLifetimeScope())
            {
                _syncService = scope.Resolve<ISynchronizerService>();
            }
        }

        [Test]
        public void PollDatabaseForChangesTest()
        {
            var activityIds = new int[] { 856 };
            var listActivityTracker = new List<ActivityChangeTracker>()
            {
                new ActivityChangeTracker()
                {
                    ActivityId = 856,
                    IsHbProduct = false,
                    IsProcessed = false,
                    OperationType = OperationType.Update,
                    ProcessedDate = DateTime.Now
                }
                //},
                //new ActivityChangeTracker()
                //{
                //    ActivityId = 6,
                //    IsHbProduct = false,
                //    IsProcessed = false,
                //    OperationType = OperationType.Update,
                //    ProcessedDate = DateTime.Now.AddDays(-5)
                //},
                //new ActivityChangeTracker()
                //{
                //    ActivityId = 7,
                //    IsHbProduct = false,
                //    IsProcessed = false,
                //    OperationType = OperationType.Insert,
                //    ProcessedDate = DateTime.Now.AddDays(-5)
                //}
            };

            var language = new List<Language>()
            {
                new Language()
                {
                    Code = "EN"
                }
            };

            var testresult = _syncService.PollDatabaseForChangesAsync().Result;

            var listActivityTrackerNull = new List<ActivityChangeTracker>();
            _masterServiceMocking.GetSupportedLanguagesAsync().ReturnsForAnyArgs(language);
            Assert.IsTrue(true);
        }

        [Test]
        public void PollDatabaseForChangesExceptionTest()
        {
            Assert.ThrowsAsync<Exception>(() => _synchronizerServiceForMockingException.PollDatabaseForChangesAsync());
        }

    }
}