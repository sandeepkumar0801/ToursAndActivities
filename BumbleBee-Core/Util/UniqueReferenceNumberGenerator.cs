using System;
using System.Linq;
using System.Numerics;

namespace Util
{
    public static class UniqueReferenceNumberGenerator
    {
        public static string GenerateReferenceNumber()
        {
            var byteArray = Guid.NewGuid().ToByteArray();
            var byteCount = byteArray.Length;
            var bigInteger = byteArray.Aggregate<byte, BigInteger>(0,
                (current, byteFromArray) => current + byteFromArray * BigInteger.Pow(256, --byteCount));
            return bigInteger.ToString().Substring(0, 18);
        }
    }
}