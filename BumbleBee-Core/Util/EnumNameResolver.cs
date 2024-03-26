using System;

namespace Util
{
    public static class EnumNameResolver
    {
        public static string GetNameFromEnum<T>(Type enumType, T index)
        {
            return Enum.GetName(enumType, index)?.ToUpperInvariant();
        }
    }
}