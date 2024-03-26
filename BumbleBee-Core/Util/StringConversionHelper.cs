using System;
using System.Text;

namespace Util
{
    public static class StringConversionHelper
    {
        public static string Hash(string toHash)
        {
            // First we need to convert the string into bytes,
            // which means using a text encoder.
            var enc = Encoding.ASCII.GetEncoder();

            // Create a buffer large enough to hold the string
            var data = new byte[toHash.Length];
            enc.GetBytes(toHash.ToCharArray(), 0, toHash.Length, data, 0, true);

            // This is one implementation of the abstract class MD5.
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            var result = md5.ComputeHash(data);
            var formattedResult = BitConverter.ToString(result).Replace("-", "");
            return formattedResult.ToLowerInvariant();
        }
    }
}