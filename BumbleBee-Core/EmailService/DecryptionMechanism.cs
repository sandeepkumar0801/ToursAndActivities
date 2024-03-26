using System.Security.Cryptography;
using System.Text;


namespace EmailSuitConsole
{
    public static class DecryptionMechanism
    {
        public const string PassPhrase = "UnchangePhrase";
        public const string SaltPhrase = "UnchangeValue";
        public const string HashAlgorithm = "SHA1";
        public const string InitVector = "abcdefghijklmnop";
        public static string Decrypt(string cipherText)
        {
            var keySize = 256;

            var initVectorBytes = Encoding.ASCII.GetBytes(InitVector);
            var saltValueBytes = Encoding.ASCII.GetBytes(SaltPhrase);

            var cipherTextBytes = Convert.FromBase64String(cipherText);

            var password = new Rfc2898DeriveBytes(PassPhrase, saltValueBytes, 1000); // Iteration count is set to 1000 (adjust it as needed)

            var keyBytes = password.GetBytes(keySize / 8);

            using var aes = Aes.Create();
            aes.Mode = CipherMode.CBC;

            var decryptor = aes.CreateDecryptor(keyBytes, initVectorBytes);

            using var memoryStream = new MemoryStream(cipherTextBytes);
            using var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);

            var plainTextBytes = new byte[cipherTextBytes.Length];

            var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);

            var plainText = Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);

            return plainText;
        }

    }
}
