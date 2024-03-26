using Isango.Service.Contract;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Isango.Persistence.Contract;
using Isango.Entities.ConsoleApplication.DataDumping;
using Isango.Entities;
using Logger.Contract;

namespace Isango.Service.ConsoleApplication
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary cloudinary;
        private readonly IMasterPersistence _masterPersistence;
        private readonly ILogger _log;

        private static string cloudName = !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["CloudinaryName"]) ? ConfigurationManager.AppSettings["CloudinaryName"] : "https-www-isango-com";
        private static string apiKey = !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["CloudinaryApiKey"]) ? ConfigurationManager.AppSettings["CloudinaryApiKey"] : "223732217938948";
        private static string apiSecret = !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["CloudinaryApiSecret"]) ? ConfigurationManager.AppSettings["CloudinaryApiSecret"] : "7bx8WRSISBoWr4bCbap3uJA_Umw";
        private Account account = new Account(cloudName, apiKey, apiSecret);

        public CloudinaryService(IMasterPersistence masterPersistence, ILogger log)
        {
            cloudinary = new Cloudinary(account);
            _masterPersistence = masterPersistence;
            _log = log;
        }

        public ImageUploadResult UploadImage(string imageName, string folder, List<string> tags, string imagePath = "")
        {
            try
            {
                ImageUploadResult uploadResult;
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(imagePath),
                    Folder = folder,
                    Tags = String.Join(",", tags)
                    //Format = "jpg"
                    //UseFilename = true,
                    //NotificationUrl = "http://www.google.com"
                };
                uploadResult = cloudinary.Upload(uploadParams);

                return uploadResult;
            }
            catch(Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "DataDumpingHelper",
                    MethodName = "LoadAPIImages"
                };
                _log.Error(isangoErrorEntity, ex);
                return null;
            }
        }

        public DeletionResult DeleteImagebyPublicId(string publicID)
        {
            try
            {
                var deletionParams = new DeletionParams(publicID)
                {
                    PublicId = publicID
                };
                var deletionResult = cloudinary.Destroy(deletionParams);
                return deletionResult;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "DeleteImagebyPublicId",
                    MethodName = "LoadAPIImages"
                };
                _log.Error(isangoErrorEntity, ex);
                return null;
            }
        }

        public Tuple<List<APIImages>, List<APIImages>> GetAPIImages()
        {
            var APIImages = _masterPersistence.GetAPIImages();

            return APIImages;
        }

        public void SaveImagesUploadResult(List<ImagesUploadResult> imagesUploadResults, List<ImagesDeleteResult> imagesDeleteResults)
        {
            _masterPersistence.SaveImagesUploadResult(imagesUploadResults, imagesDeleteResults);
        }
        
    }
}
