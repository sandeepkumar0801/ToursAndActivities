using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Isango.Entities.ConsoleApplication.DataDumping
{
    public class ImagesUploadResult
    {
        public int serviceid;
        public string imagekey;
        public string Imagepath;
        public string imagesorder;
        public string APIurl;
        public int Apitypeid;
        public string Supplierproductid;
    }

    public class ImagesDeleteResult
    {
        public int ID;
        public int serviceid;
        public int Apitypeid;
        public string Supplierproductid;
        public string CloudinaryURL;
    }
}
