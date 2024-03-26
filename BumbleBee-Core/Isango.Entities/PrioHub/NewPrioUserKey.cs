using System;
namespace Isango.Entities.PrioHub
{
    public class PrioHubUserKey
    {
        public string User { get; set; }
        public string Password { get; set; }
        public string DistributerID { get; set; }
        public bool IsTestEnvironment { get; set; }
    }
}