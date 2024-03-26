using System;

namespace Util
{
    public static class UniqueTransactionIdGenerator
    {
        public static string GenerateTransactionId()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }
    }
}