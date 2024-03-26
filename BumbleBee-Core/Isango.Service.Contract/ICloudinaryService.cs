using CloudinaryDotNet.Actions;
using Isango.Entities.ConsoleApplication.DataDumping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Isango.Service.Contract
{
    public interface ICloudinaryService
    {
        ImageUploadResult UploadImage(string imageName, string folder, List<string> tags, string imagePath = "");

        DeletionResult DeleteImagebyPublicId(string publicID);

        Tuple<List<APIImages>, List<APIImages>> GetAPIImages();

        void SaveImagesUploadResult(List<ImagesUploadResult> imagesUploadResults, List<ImagesDeleteResult> imagesDeleteResults);
    }
}
