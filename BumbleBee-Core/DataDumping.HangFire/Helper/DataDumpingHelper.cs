using DataDumping.HangFire.Contracts;
using Isango.Entities;
using Isango.Entities.Activities;
using Isango.Entities.ConsoleApplication.DataDumping;
using Isango.Entities.ConsoleApplication.ServiceAvailability;
using Isango.Entities.Enums;
using Isango.Entities.GoogleMaps;
using Isango.Persistence.Contract;
using Isango.Service.Contract;
using Logger.Contract;
using PriceRuleEngine;
using ServiceAdapters.Bokun;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Util;
using Activity = Isango.Entities.Activities.Activity;

namespace DataDumping.HangFire.Helper
{
    public class DataDumpingHelper : IDataDumpingHelper
    {
        private readonly IServiceAvailabilityService _serviceAvailabilityService;
        private readonly IGoogleMapsDataDumpingService _googleMapsDataDumpingService;
        private readonly PricingController _pricingController;
        private readonly ILogger _log;
        //private readonly ICartService _cartService;
        private readonly IBokunAdapter _bokunadapter;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IMasterPersistence _masterPersistence;

        public DataDumpingHelper(ICartService cartService
            , IServiceAvailabilityService serviceAvailabilityService
            , IGoogleMapsDataDumpingService googleMapsDataDumpingService
            , PricingController pricingController
            , ILogger log
            , IBokunAdapter bokunAdapter
            , ICloudinaryService cloudinaryService
            , IMasterPersistence masterPersistence
            )
        {
            _serviceAvailabilityService = serviceAvailabilityService;
            _googleMapsDataDumpingService = googleMapsDataDumpingService;
            _pricingController = pricingController;
            _log = log;
            //_cartService = cartService;
            _bokunadapter = bokunAdapter;
            _cloudinaryService = cloudinaryService;
            _masterPersistence = masterPersistence;
        }

        public void LoadGrayLineIceLandData()
        {
            try
            {
                var watch = Stopwatch.StartNew();
                // Get Activities and ServiceDetails data
                var availabilityData = _serviceAvailabilityService.GetGrayLineIceLandAvailabilities();
                watch.Stop();
                _log.WriteTimer("GetGrayLineIceLandAvailabilities", "DataDumping", APIType.Graylineiceland.ToString(), watch.Elapsed.ToString());
                if (availabilityData != null)
                {
                    // Save the data in the DB, apply PRE and dump the data in the storage
                    SaveAvailabilitiesData(new Tuple<List<Activity>, List<TempHBServiceDetail>>(availabilityData.Item1, availabilityData.Item2), APIType.Graylineiceland);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "DataDumpingHelper",
                    MethodName = "LoadGrayLineIceLandData"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public void LoadMoulinRougeData()
        {
            try
            {
                var watch = Stopwatch.StartNew();
                // Get Activities and ServiceDetails data
                var availabilityData = _serviceAvailabilityService.GetMoulinRougeAvailabilities();
                watch.Stop();
                _log.WriteTimer("GetMoulinRougeAvailabilities", "DataDumping", APIType.Moulinrouge.ToString(), watch.Elapsed.ToString());

                // Save the data in the DB, apply PRE and dump the data in the storage
                SaveAvailabilitiesData(availabilityData, APIType.Moulinrouge);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "DataDumpingHelper",
                    MethodName = "LoadMoulinRougeData"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public void LoadPrioData()
        {
            try
            {
                var watch = Stopwatch.StartNew();
                // Get Activities and ServiceDetails data
                var availabilityData = _serviceAvailabilityService.GetPrioAvailabilities();
                watch.Stop();
                _log.WriteTimer("GetPrioAvailabilities", "DataDumping", APIType.Prio.ToString(), watch.Elapsed.ToString());

                // Save the data in the DB, apply PRE and dump the data in the storage
                SaveAvailabilitiesData(availabilityData, APIType.Prio);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "DataDumpingHelper",
                    MethodName = "LoadPrioData"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public void LoadPrioHubData()
        {
            try
            {
                var watch = Stopwatch.StartNew();
                // Get Activities and ServiceDetails data
                var availabilityData = _serviceAvailabilityService.GetPrioHubAvailabilities();
                watch.Stop();
                _log.WriteTimer("GetPrioHubAvailabilities", "DataDumping", APIType.PrioHub.ToString(), watch.Elapsed.ToString());

                // Save the data in the DB, apply PRE and dump the data in the storage
                SaveAvailabilitiesData(availabilityData, APIType.PrioHub);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "DataDumpingHelper",
                    MethodName = "LoadPrioData"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public void LoadFareHarborData()
        {
            try
            {
                var watch = Stopwatch.StartNew();
                // Get Activities and ServiceDetails data
                var availabilityData = _serviceAvailabilityService.GetFareHarborAvailabilities();
                watch.Stop();
                _log.WriteTimer("GetFareHarborAvailabilities", "DataDumping", APIType.Fareharbor.ToString(), watch.Elapsed.ToString());

                // Save the data in the DB, apply PRE and dump the data in the storage
                SaveAvailabilitiesData(availabilityData, APIType.Fareharbor);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "DataDumpingHelper",
                    MethodName = "LoadFareHarborData"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public void LoadBokunData()
        {
            try
            {
                var watch = Stopwatch.StartNew();
                // Get Activities and ServiceDetails data
                var availabilityData = _serviceAvailabilityService.GetBokunAvailabilities();
                watch.Stop();
                _log.WriteTimer("GetBokunAvailabilities", "DataDumping", APIType.Bokun.ToString(), watch.Elapsed.ToString());

                // Save the data in the DB, apply PRE and dump the data in the storage

                SaveAvailabilitiesData(new Tuple<List<Activity>, List<TempHBServiceDetail>>(availabilityData.Item1, availabilityData.Item2), APIType.Bokun);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "DataDumpingHelper",
                    MethodName = "LoadBokunData"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public void LoadAotData()
        {
            try
            {
                var watch = Stopwatch.StartNew();
                // Get Activities and ServiceDetails data
                var availabilityData = _serviceAvailabilityService.GetAotAvailabilities();
                watch.Stop();
                _log.WriteTimer("GetAotAvailabilities", "DataDumping", APIType.Aot.ToString(), watch.Elapsed.ToString());

                // Save the data in the DB, apply PRE and dump the data in the storage
                SaveAvailabilitiesData(availabilityData, APIType.Aot);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "DataDumpingHelper",
                    MethodName = "LoadAotData"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public void LoadTiqetsData()
        {
            try
            {
                var watch = Stopwatch.StartNew();
                // Get Activities and ServiceDetails data
                var availabilityData = _serviceAvailabilityService.GetTiqetsAvailabilities();
                watch.Stop();
                _log.WriteTimer("GetTiqetsAvailabilities", "DataDumping", APIType.Tiqets.ToString(), watch.Elapsed.ToString());

                // Save the data in the DB, apply PRE and dump the data in the storage
                SaveAvailabilitiesData(availabilityData, APIType.Tiqets);
                try
                {
                    SaveAvailabilitiesDataForTiqets(availabilityData, APIType.Tiqets);
                }
                catch (Exception e)
                {
                    var isangoErrorEntity = new IsangoErrorEntity
                    {
                        ClassName = "DataDumpingHelper",
                        MethodName = "SaveAvailabilitiesDataForTiqets"
                    };
                    _log.Error(isangoErrorEntity, e);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "DataDumpingHelper",
                    MethodName = "LoadTiqetsData"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public void LoadGoldenToursData()
        {
            try
            {
                var watch = Stopwatch.StartNew();
                // Get Activities and ServiceDetails data
                var availabilityData = _serviceAvailabilityService.GetGoldenToursAvailabilities();
                watch.Stop();
                _log.WriteTimer("GetGoldenToursAvailabilities", "DataDumping", APIType.Goldentours.ToString(), watch.Elapsed.ToString());

                // Save the data in the DB, apply PRE and dump the data in the storage
                if (availabilityData != null)
                {
                    SaveAvailabilitiesData(new Tuple<List<Activity>, List<TempHBServiceDetail>>(availabilityData.Item1, availabilityData.Item2), APIType.Goldentours);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "DataDumpingHelper",
                    MethodName = "LoadGoldenToursData"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public void LoadTourCMSData()
        {
            try
            {
                var watch = Stopwatch.StartNew();
                // Get Activities and ServiceDetails data
                var availabilityData = _serviceAvailabilityService.GetTourCMSAvailabilities();

                watch.Stop();
                _log.WriteTimer("GetTourCMSAvailabilities", "DataDumping", APIType.TourCMS.ToString(), watch.Elapsed.ToString());

                // Save the data in the DB, apply PRE and dump the data in the storage
                SaveAvailabilitiesData(new Tuple<List<Activity>, List<TempHBServiceDetail>>(availabilityData.Item1, availabilityData.Item2), APIType.TourCMS);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "DataDumpingHelper",
                    MethodName = "LoadTourCMSData"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }
        public void LoadHBApitudeData()
        {
            try
            {
                var watch = Stopwatch.StartNew();
                // Get Activities and ServiceDetails data
                //Get ApiTude products
                var products = _masterPersistence.LoadLiveHBOptions()?
                               .Where(x => x.ApiType.Equals(APIType.Hotelbeds)
                                            && !string.IsNullOrWhiteSpace(x.SupplierCode)
                                     )
                               .Distinct().OrderBy(x => x.FactSheetId)
                               .ToList();

                foreach (var bacth in products.Batch(500))
                {
                    try
                    {
                        var productsBatch = bacth.ToList();
                        var availabilityData = _serviceAvailabilityService.SaveApiTudeAvailabilities(productsBatch);
                        SaveAvailabilitiesData(availabilityData, APIType.Hotelbeds);
                    }
                    catch (Exception ex)
                    {
                        var isangoErrorEntity = new IsangoErrorEntity
                        {
                            ClassName = "DataDumpingHelper",
                            MethodName = "LoadHBApitudeData"
                        };
                        _log.Error(isangoErrorEntity, ex);
                    }
                }
                watch.Stop();
                _log.WriteTimer("SaveApiTudeAvailablities", "DataDumping", APIType.Hotelbeds.ToString(), watch.Elapsed.ToString());

                // Save the data in the DB, apply PRE and dump the data in the storage
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "DataDumpingHelper",
                    MethodName = "LoadHBApitudeData"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public void LoadRezdyData()
        {
            try
            {
                var watch = Stopwatch.StartNew();
                // Get Activities and ServiceDetails data
                var serviceDetails = _serviceAvailabilityService.GetRezdyServiceDetails();
                watch.Stop();
                _log.WriteTimer("GetRezdyServiceDetails", "DataDumping", APIType.Rezdy.ToString(), watch.Elapsed.ToString());

                // Save the data in the DB, apply PRE and dump the data in the storage
                SaveAvailabilitiesData(serviceDetails, APIType.Rezdy);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "DataDumpingHelper",
                    MethodName = "LoadRezdyData"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public void LoadRedeamData()
        {
            try
            {
                var watch = Stopwatch.StartNew();
                // Get Activities and ServiceDetails data
                var serviceDetails = _serviceAvailabilityService.GetRedeamServiceDetails();
                watch.Stop();
                _log.WriteTimer("GetRedeamServiceDetails", "DataDumping", APIType.Hotelbeds.ToString(), watch.Elapsed.ToString());

                // Save the data in the DB, apply PRE and dump the data in the storage
                SaveAvailabilitiesData(serviceDetails, APIType.Redeam);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "DataDumpingHelper",
                    MethodName = "LoadRedeamData"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }
        public void LoadRedeamV12Data()
        {
            try
            {
                var watch = Stopwatch.StartNew();
                // Get Activities and ServiceDetails data
                var serviceDetails = _serviceAvailabilityService.GetRedeamV12ServiceDetails();

                watch.Stop();
                _log.WriteTimer("GetRedeamServiceDetails", "DataDumping", APIType.RedeamV12.ToString(), watch.Elapsed.ToString());

                // Save the data in the DB, apply PRE and dump the data in the storage
                SaveAvailabilitiesData(serviceDetails, APIType.RedeamV12);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "DataDumpingHelper",
                    MethodName = "LoadRedeamV12Data"
                };
                _log.Error(isangoErrorEntity, ex);
                //throw;
            }
        }

        public void LoadGlobalTixData()
        {
            try
            {
                var watch = Stopwatch.StartNew();
                // Get Activities and ServiceDetails data
                var availabilityData = _serviceAvailabilityService.SaveGlobalTixAvailabilities();
                watch.Stop();
                _log.WriteTimer("LoadGlobalTixData", "DataDumping", APIType.GlobalTix.ToString(), watch.Elapsed.ToString());

                // Save the data in the DB, apply PRE and dump the data in the storage
                SaveAvailabilitiesData(availabilityData, APIType.GlobalTix);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "DataDumpingHelper",
                    MethodName = "LoadHBApitudeData"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }
        public void LoadGlobalTixV3Data()
        {
            try
            {
                var watch = Stopwatch.StartNew();
                // Get Activities and ServiceDetails data
                var availabilityData = _serviceAvailabilityService.SaveGlobalTixV3Availabilities();
                watch.Stop();
                _log.WriteTimer("LoadGlobalTixV3Data", "DataDumping", APIType.GlobalTixV3.ToString(), watch.Elapsed.ToString());

                // Save the data in the DB, apply PRE and dump the data in the storage
                SaveAvailabilitiesData(availabilityData, APIType.GlobalTixV3);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "DataDumpingHelper",
                    MethodName = "LoadGlobalTixV3Data"
                };
                _log.Error(isangoErrorEntity, ex);
                //throw;
            }
        }

        public void LoadVentrataData()
        {
            try
            {
                var watch = Stopwatch.StartNew();
                // Get Activities and ServiceDetails data
                var availabilityData = _serviceAvailabilityService.GetVentrataAvailabilities();
                watch.Stop();
                _log.WriteTimer("LoadVentrataData", "DataDumping", APIType.Ventrata.ToString(), watch.Elapsed.ToString());

                // Save the data in the DB, apply PRE and dump the data in the storage
                SaveAvailabilitiesData(availabilityData, APIType.Ventrata);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "DataDumpingHelper",
                    MethodName = "LoadHBApitudeData"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public void LoadAPIImages()
        {
            if (ConfigurationManagerHelper.GetValuefromAppSettings("CloudinaryImagesUpload") == "1")
            {
                try
                {
                    var watch = Stopwatch.StartNew();
                    var AllImages = _cloudinaryService.GetAPIImages();
                    var imagesUploadResult = new List<ImagesUploadResult>();
                    var imagesDeleteResult = new List<ImagesDeleteResult>();
                    var ImagesToUpload = AllImages.Item1;
                    var ImagesToDelete = AllImages.Item2;
                    if (ImagesToUpload?.Count > 0)
                    {
                        foreach (var Image in ImagesToUpload)
                        {
                            try
                            {
                                var Tags = new List<string>();
                                var ServiceIDs = Image.ServiceID.Split(',');
                                foreach (var ID in ServiceIDs)
                                {
                                    var ServiceTag = "p" + Image.ServiceID;
                                    Tags.Add(ServiceTag);
                                }
                                var APITag = (APIType)Image.APITypeID;
                                Tags.Add(APITag.ToString());
                                var result = _cloudinaryService.UploadImage("NotRequired", Image.Path.ToLower(), Tags, Image.ImageURL);
                                if (result != null)
                                {
                                    foreach (var ID in ServiceIDs)
                                    {
                                        var imgresult = new ImagesUploadResult
                                        {
                                            serviceid = Convert.ToInt32(ID),
                                            APIurl = Image.ImageURL,
                                            imagekey = "/" + result.PublicId,
                                            Imagepath = result.Uri.AbsoluteUri.Replace("http://res.cloudinary.com/https-www-isango-com/image/upload/", "").Replace("https://res.cloudinary.com/https-www-isango-com/image/upload/", "").ToString(),
                                            imagesorder = Image.Sequence.ToString(),
                                            Apitypeid = Image.APITypeID,
                                            Supplierproductid = Image.SupplierProductID
                                        };
                                        imagesUploadResult.Add(imgresult);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                var isangoErrorEntity = new IsangoErrorEntity
                                {
                                    ClassName = "DataDumpingHelper",
                                    MethodName = "LoadAPIImages"
                                };
                                _log.Error(isangoErrorEntity, ex);
                            }
                        }
                    }
                    if (ImagesToDelete?.Count > 0)
                    {
                        foreach (var Image in ImagesToDelete)
                        {
                            try
                            {
                                if (!string.IsNullOrEmpty(Image.PublicID))
                                {
                                    var publicid = Image.PublicID?.Substring(1);
                                    var DeleteResult = _cloudinaryService.DeleteImagebyPublicId(publicid);
                                    if (DeleteResult != null && DeleteResult.Error == null)
                                    {
                                        var imgresult = new ImagesDeleteResult
                                        {
                                            ID = Image.ID ?? 0,
                                            serviceid = Convert.ToInt32(Image.ServiceID),
                                            CloudinaryURL = Image.ImageURL,
                                            Apitypeid = Image.APITypeID,
                                            Supplierproductid = Image.SupplierProductID
                                        };
                                        imagesDeleteResult.Add(imgresult);
                                    }
                                }
                                else
                                {
                                    var imgresult = new ImagesDeleteResult
                                    {
                                        ID = Image.ID ?? 0,
                                        serviceid = Convert.ToInt32(Image.ServiceID),
                                        CloudinaryURL = Image.ImageURL,
                                        Apitypeid = Image.APITypeID,
                                        Supplierproductid = Image.SupplierProductID
                                    };
                                    imagesDeleteResult.Add(imgresult);
                                }
                            }
                            catch (Exception ex)
                            {
                                var isangoErrorEntity = new IsangoErrorEntity
                                {
                                    ClassName = "DataDumpingHelper",
                                    MethodName = "LoadAPIImages"
                                };
                                _log.Error(isangoErrorEntity, ex);
                            }
                        }
                    }
                    _cloudinaryService.SaveImagesUploadResult(imagesUploadResult, imagesDeleteResult);
                    watch.Stop();
                    _log.WriteTimer("LoadAPIImages", "DataDumping", APIType.Undefined.ToString(), watch.Elapsed.ToString());
                }
                catch (Exception ex)
                {
                    var isangoErrorEntity = new IsangoErrorEntity
                    {
                        ClassName = "DataDumpingHelper",
                        MethodName = "LoadAPIImages"
                    };
                    _log.Error(isangoErrorEntity, ex);
                    //throw;
                }
            }
        }

        public void LoadIsangoData()
        {
            try
            {
                var watch = Stopwatch.StartNew();
                // Get Activities and ServiceDetails data
                var availabilityData = _serviceAvailabilityService.GetIsangoAvailabilities();
                watch.Stop();
                _log.WriteTimer("GetIsangoAvailabilities", "DataDumping", APIType.Undefined.ToString(), watch.Elapsed.ToString());

                // Save the data in the DB, apply PRE and dump the data in the storage
                SaveAvailabilitiesData(availabilityData, APIType.Undefined);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "DataDumpingHelper",
                    MethodName = "LoadIsangoData"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        public void LoadCancellationPolicies()
        {
            try
            {
                var token = $"DataDumping";
                var watch = Stopwatch.StartNew();
                // Fetch the cancellation prices for the valid suppliers
                var cancellationPolicies = _serviceAvailabilityService.GetCancellationPolicies();
                watch.Stop();
                _log.WriteTimer($"GetCancellationPolicies:{cancellationPolicies.Count}", token, string.Empty, watch.Elapsed.ToString());

                watch.Restart();
                // Load the cancellation prices in the table storage
                _googleMapsDataDumpingService.DumpCancellationPolicies(cancellationPolicies);
                watch.Stop();
                _log.WriteTimer($"GoogleDumpCancellationPolicies:{cancellationPolicies.Count}", token, string.Empty, watch.Elapsed.ToString());
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "DataDumpingHelper",
                    MethodName = "LoadCancellationPolicies"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }
        public void LoadNewCitySightSeeingData()
        {
            try
            {
                var watch = Stopwatch.StartNew();
                // Get Activities and ServiceDetails data
                var serviceDetails = _serviceAvailabilityService.GetNewCitySightSeeingServiceDetails();
                watch.Stop();
                _log.WriteTimer("LoadNewCitySightSeeingServiceAvailabilities", "DataDumping", APIType.NewCitySightSeeing.ToString(), watch.Elapsed.ToString());

                // Save the data in the DB, apply PRE and dump the data in the storage
                SaveAvailabilitiesData(serviceDetails, APIType.NewCitySightSeeing);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "DataDumpingHelper",
                    MethodName = "LoadNewCitySightSeeingData"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }
        public void LoadRaynaData()
        {
            try
            {
                var watch = Stopwatch.StartNew();
                // Get Activities and ServiceDetails data
                var serviceDetails = _serviceAvailabilityService.GetRaynaServiceDetails();
                watch.Stop();
                _log.WriteTimer("LoadRaynaServiceAvailabilities", "DataDumping", APIType.Rayna.ToString(), watch.Elapsed.ToString());

                // Save the data in the DB, apply PRE and dump the data in the storage
                SaveAvailabilitiesData(serviceDetails, APIType.Rayna);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "DataDumpingHelper",
                    MethodName = "LoadRaynaData"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }
        #region Private Methods

        private void SaveAvailabilitiesData(Tuple<List<Activity>, List<TempHBServiceDetail>> availabilityData, APIType apiType)
        {


            if (availabilityData == null)
                return;
            var activities = availabilityData.Item1;
            var serviceDetails = availabilityData.Item2;
            var token = $"DataDumping";
            var watch = Stopwatch.StartNew();

            if (apiType != APIType.Undefined)
            {
                try
                {
                    // Save the ServiceDetails in the DB
                    _serviceAvailabilityService.SaveServiceAvailabilitiesInDatabase(serviceDetails);
                    watch.Stop();
                    _log.WriteTimer($"SaveServiceAvailabilitiesInDatabase_serviceDetails:{serviceDetails.Count}", token, apiType.ToString(), watch.Elapsed.ToString());

                    watch.Restart();
                    // Sync the Price and Availabilities databases
                    Task.Run(() => _serviceAvailabilityService.SyncPriceAvailabilities());
                    watch.Stop();
                    _log.WriteTimer("SyncPriceAvailabilities", token, apiType.ToString(), watch.Elapsed.ToString());
                }
                catch (Exception ex)
                {
                    var isangoErrorEntity = new IsangoErrorEntity
                    {
                        ClassName = "DataDumpingHelper",
                        MethodName = "SaveAvailabilitiesData"
                    };
                    _log.Error(isangoErrorEntity, ex);
                    throw;
                }
            }

            watch.Restart();
            // Apply PRE on the activities
            var processedActivities = ApplyPriceRuleEngine(activities);
            watch.Stop();
            _log.WriteTimer($"ApplyPriceRuleEngine_activities:{activities.Count}_processedActivities:{processedActivities.Count}", token, apiType.ToString(), watch.Elapsed.ToString());

            watch.Restart();
            var allProductOptions = new List<ProductOption>();
            if (processedActivities?.Count > 0)
            {
                foreach (var processedActivity in processedActivities)
                {
                    if (processedActivity?.ProductOptions?.Count > 0)
                    {
                        allProductOptions.AddRange(processedActivity.ProductOptions);
                    }
                }
            }

            //Save Price and Availabilities in the storage
            //var storageServiceDetailCount = _googleMapsDataDumpingService.DumpPriceAndAvailabilities(serviceDetails.Where(x => x.UnitType.Equals(UnitType.PerPerson.ToString())).ToList(), allProductOptions, apiType);
            watch.Stop();
            //_log.WriteTimer($"GoogleDumpPriceAndAvailabilities_storageServiceDetailCount:{storageServiceDetailCount}", token, apiType.ToString(), watch.Elapsed.ToString());

            watch.Restart();
            //Save Extra Details in the storage
            var Extradetails = _googleMapsDataDumpingService.DumpExtraDetail(processedActivities, apiType);

            if (Extradetails != null)
            {
                if (apiType == APIType.Bokun)
                {
                    var extra = GetBokunRequiredQuestions(availabilityData.Item1, Extradetails);
                    SaveQuestionsData(extra, Convert.ToInt32(APIType.Bokun));
                }
                else
                {
                    SaveQuestionsData(Extradetails, Convert.ToInt32(apiType));
                }
            }

            watch.Stop();
            _log.WriteTimer("GoogleDumpExtraDetail", token, apiType.ToString(), watch.Elapsed.ToString());

        }

        private void SaveAvailabilitiesDataForTiqets(Tuple<List<Activity>, List<TempHBServiceDetail>> availabilityData, APIType apiType)
        {
            if (availabilityData == null)
                return;
            var activities = availabilityData.Item1;
            var serviceDetails = availabilityData.Item2;
            var token = $"DataDumping";
            var watch = Stopwatch.StartNew();

            if (apiType != APIType.Undefined)
            {
                // Save the ServiceDetails in the DB
                _serviceAvailabilityService.SaveServiceAvailabilitiesInDatabaseForTiqets(serviceDetails);
                watch.Stop();
                _log.WriteTimer($"SaveServiceAvailabilitiesInDatabaseForTiqets_serviceDetails:{serviceDetails.Count}", token, apiType.ToString(), watch.Elapsed.ToString());
            }
            watch.Stop();
        }

        private Isango.Entities.GoogleMaps.ExtraDetail GetBokunRequiredQuestions(List<Activity> availabilities, Isango.Entities.GoogleMaps.ExtraDetail ExtraDetails)
        {
            var RequiredQuestionsandFields = new List<ServiceAdapters.Bokun.Bokun.Entities.GetActivity.GetActivityRs>();
            foreach (var item in availabilities)
            {
                if (item?.ProductOptions?.Count > 0)
                {
                    var Fields = _bokunadapter.GetActivity(item?.ProductOptions?.FirstOrDefault()?.SupplierOptionCode.ToString(), "Avail_Bokun");
                    RequiredQuestionsandFields.Add(Fields);
                }
            }

            if (RequiredQuestionsandFields != null && RequiredQuestionsandFields.Count > 0)
            {
                foreach (var extra in RequiredQuestionsandFields)
                {
                    if (extra?.MainContactFields != null && extra?.MainContactFields.Count > 0)
                    {
                        foreach (var contact in extra?.MainContactFields)
                        {
                            var ID = contact.Field;
                            bool IsRequired = contact.Required ?? false;
                            try
                            {
                                foreach (var item in ExtraDetails?.PaymentExtraDetails)
                                {
                                    if (item.ActivityId == Convert.ToInt32(extra.PricingCategories[0].Id) || item.ActivityId == Convert.ToInt32(extra.ExternalId))
                                    {
                                        if (extra?.RequiredCustomerFields != null)
                                        {
                                            var QuestionData = new Isango.Entities.GoogleMaps.Question()
                                            {
                                                Id = ID,
                                                QuestionType = ID,
                                                Required = IsRequired
                                            };
                                            var temp = item.Questions.Find(x => x.Id.ToLower() == ID.ToLower());
                                            if (temp == null)
                                            {
                                                item.Questions.Add(QuestionData);
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                //ignored
                            }
                        }
                    }
                }
            }

            return ExtraDetails;
        }

        private void SaveQuestionsData(Isango.Entities.GoogleMaps.ExtraDetail ExtraDetails, int ApiType)
        {
            var Details = new List<ExtraDetailQuestions>();
            //var PickUpLocations = new List<ExtraDetailPickUpLocations>();
            //var DropOffLocations = new List<ExtraDetailDropOffLocations>();

            foreach (var PayExtraDetail in ExtraDetails?.PaymentExtraDetails)
            {
                try
                {
                    var ques = new ExtraDetailQuestions() { Questions = new List<Isango.Entities.GoogleMaps.Question>(), DropOffLocationID = new List<int>(), DropOffLocationName = new List<string>(), PickUpLocationID = new List<int>(), PickUpLocationName = new List<string>() };

                    ques.ActivityId = PayExtraDetail.ActivityId;
                    ques.OptionId = PayExtraDetail.OptionId;
                    ques.StartTime = PayExtraDetail.StartTime;
                    ques.Variant = PayExtraDetail.Variant;

                    //Questions
                    if (PayExtraDetail?.Questions != null && PayExtraDetail?.Questions.Count > 0)
                    {
                        foreach (var q in PayExtraDetail?.Questions)
                        {
                            var addQuestion = new Isango.Entities.GoogleMaps.Question() { AnswerOptions = new List<AnswerOption>() };

                            if (q?.AnswerOptions != null && q?.AnswerOptions.Count > 0)
                            {
                                foreach (var ans in q?.AnswerOptions)
                                {
                                    var addAnswer = new Isango.Entities.GoogleMaps.AnswerOption();
                                    addAnswer.Label = ans.Label;
                                    addAnswer.Value = ans.Value;
                                    addQuestion.AnswerOptions.Add(addAnswer);
                                }
                            }

                            addQuestion.DataType = q.DataType;
                            addQuestion.DefaultValue = q.DefaultValue;
                            addQuestion.Id = q.Id;
                            addQuestion.Label = q.Label;
                            addQuestion.QuestionType = q.QuestionType;
                            addQuestion.Required = q.Required;
                            addQuestion.SelectFromOptions = q.SelectFromOptions;

                            ques.Questions.Add(addQuestion);
                        }
                    }

                    //PickUpLocations
                    if (PayExtraDetail?.PickupLocations?.Count > 0)
                    {
                        foreach (var location in PayExtraDetail?.PickupLocations)
                        {
                            ques.PickUpLocationID.Add(location.Key);
                            ques.PickUpLocationName.Add(location.Value);
                        }
                    }

                    //DropOffLocations
                    if (PayExtraDetail?.DropoffLocations?.Count > 0)
                    {
                        foreach (var location in PayExtraDetail?.DropoffLocations)
                        {
                            ques.DropOffLocationID.Add(location.Key);
                            ques.DropOffLocationName.Add(location.Value);
                        }
                    }

                    Details.Add(ques);
                }
                catch (Exception ex)
                {
                    var isangoErrorEntity = new IsangoErrorEntity
                    {
                        ClassName = "DataDumpingHelper",
                        MethodName = "SaveQuestionsData"
                    };
                    _log.Error(isangoErrorEntity, ex);
                }
            }

            _serviceAvailabilityService.SaveQuestionsInDatabase(Details, ApiType);
        }

        private List<Activity> ApplyPriceRuleEngine(List<Activity> activities)
        {
            var processedActivities = new List<Activity>();
            var maxParallelThreadCount = MaxParallelThreadCountHelper.GetMaxParallelThreadCount("MaxParallelThreadCount");
            Parallel.ForEach(activities, new ParallelOptions { MaxDegreeOfParallelism = maxParallelThreadCount }, activity =>
            {
                try
                {
                    var productOptions = activity?.ProductOptions;
                    if (productOptions == null) return;

                    //productOptions.ForEach(x => x.ServiceOptionId = x.ServiceOptionId);
                    var pricingRequest = CreatePricingRequest(productOptions);
                    pricingRequest.PriceTypeId = activity.PriceTypeId;
                    var processedProductOptions = _pricingController.Process(pricingRequest);
                    activity.ProductOptions = processedProductOptions;
                    processedActivities.Add(activity);
                }
                catch (Exception ex)
                {
                    var isangoErrorEntity = new IsangoErrorEntity
                    {
                        ClassName = "DataDumpingHelper",
                        MethodName = "ApplyPriceRuleEngine",
                    };
                    _log.Error(isangoErrorEntity, ex);
                    // ignored
                }
            });
            return processedActivities;
        }

        private PricingRuleRequest CreatePricingRequest(List<ProductOption> productOptions)
        {
            var affiliateId = ConfigurationManagerHelper.GetValuefromAppSettings("GoogleMapsAffiliatedId");
            var countryIp = ConfigurationManagerHelper.GetValuefromAppSettings("GoogleMapsCountryIp");
            var pricingRequest = new PricingRuleRequest
            {
                PriceTypeId = PriceTypeId.Undefined,
                ProductOptions = productOptions.Where(x => x.AvailabilityStatus != AvailabilityStatus.NOTAVAILABLE).ToList(),
                ClientInfo = new ClientInfo
                {
                    IsSupplementOffer = true,
                    AffiliateId = affiliateId,
                    CountryIp = countryIp,
                    IsB2BAffiliate = true
                }
            };
            return pricingRequest;
        }

        public List<string> GetAgeDumpingAPIs()
        {
            try
            {
                var watch = Stopwatch.StartNew();
                var ageDumpingAPIs = _serviceAvailabilityService.GetAgeDumpingAPIs();
                watch.Stop();
                _log.WriteTimer("GetAgeDumpingAPIs", "DataDumping", APIType.Undefined.ToString(), watch.Elapsed.ToString());

                return ageDumpingAPIs;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "DataDumpingHelper",
                    MethodName = "LoadIsangoData"
                };
                _log.Error(isangoErrorEntity, ex);
                throw;
            }
        }

        #endregion Private Methods
    }
}