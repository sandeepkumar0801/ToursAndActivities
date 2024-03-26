namespace Bumblebee.Identity
{
    // AppSettings.cs
    public class AppSettings
    {
        public string Secret { get; set; }
        public int TokenExpirationMinutes { get; set; }
        public string ClientAllowedOrigin { get; set; }
    }

}
