//using Azure.Identity;
//using Azure.Security.KeyVault.Secrets;
//using Logger.Contract;
//using Microsoft.Extensions.Caching.Memory;

//namespace Util
//{
//    public static class KeyVaultHelper
//    {
//        private static SecretClient _keyVaultClient;
//        private static IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions());


//        static KeyVaultHelper()
//        {
//            string keyVaultUri = ConfigurationManagerHelper.GetValuefromAzureKeyVaultSettings("VaultUri");
//            string clientId = ConfigurationManagerHelper.GetValuefromAzureKeyVaultSettings("ClientId");
//            string clientSecret = ConfigurationManagerHelper.GetValuefromAzureKeyVaultSettings("ClientSecret");
//            string clientSecret1 = ConfigurationManagerHelper.GetValuefromAzureKeyVaultSettings("TenantId");

//            ClientSecretCredential clientCredential = new ClientSecretCredential(
//                ConfigurationManagerHelper.GetValuefromAzureKeyVaultSettings("TenantId"),
//                clientId,
//                clientSecret);

//            _keyVaultClient = new SecretClient(new Uri(keyVaultUri), clientCredential);
//        }

//        public static async Task<string> GetSecretAsync(string secretName)
//        {
//            if (_cache.TryGetValue(secretName, out string secretValue))
//            {
//                // The secret is found in the cache, return it.
//                return secretValue;
//            }

//            try
//            {
//                KeyVaultSecret secret = await _keyVaultClient.GetSecretAsync(secretName);
//                if (secret != null)
//                {
//                    secretValue = secret.Value;
//                    _cache.Set(secretName, secretValue, TimeSpan.FromHours(2));

//                    return secretValue;
//                }
//            }
//            catch (Exception ex)
//            {
//            }

//            return null;
//        }



//    }
//}
