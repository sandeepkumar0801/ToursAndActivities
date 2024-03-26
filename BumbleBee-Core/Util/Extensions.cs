using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

// ReSharper disable AssignNullToNotNullAttribute
namespace Util
{
    public static class Extensions
    {
        /// <summary>
        /// Converts int value to Enum
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T ConvertToEnum<T>(this int value)
        {
            if (!typeof(T).IsEnum)
                throw new NotSupportedException("T must be an Enum");

            try
            {
                return (T)Enum.Parse(typeof(T), Enum.GetName(typeof(T), value));
            }
            catch
            {
                return default(T);
            }
        }

        /// <summary>
        /// Converts from Enum1 to Enum2
        /// </summary>
        /// <typeparam name="T">Enum2 Object equlant to Enum 1 value</typeparam>
        /// <param name="value">Enum1 Value</param>
        /// <returns></returns>
        public static T ConvertToEnum<T>(this Enum value)
        {
            if (!typeof(T).IsEnum)
                throw new NotSupportedException("T must be an Enum");

            try
            {
                return (T)Enum.Parse(typeof(T), Enum.GetName(typeof(T), value));
            }
            catch
            {
                return default(T);
            }
        }

        /// <summary>
        /// Converts string to Enum object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">String equlant to Enum name</param>
        /// <returns></returns>
        public static T ConvertToEnum<T>(this string value) where T : new()
        {
            if (!typeof(T).IsEnum)
                throw new NotSupportedException("T must be an Enum");

            try
            {
                return (T)Enum.Parse(typeof(T), value);
            }
            catch
            {
                return default(T);
            }
        }

        /// <summary>
        /// Checks if an Enumerable is null or empty
        /// </summary>
        /// <typeparam name="T">Any object type</typeparam>
        /// <param name="list">Enumerable object</param>
        /// <returns>True if found Null or Empty, else false</returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> list)
        {
            return list == null || !list.Any();
        }

        public static T ConvertToObject<T>(this string xml)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(new StringReader(xml));
            }
            catch
            {
                return default(T);
            }
        }

        public static T ConvertTo<T>(this object obj)
        {
            try
            {
                return (T)Convert.ChangeType(obj, typeof(T));
            }
            catch
            {
                return default(T);
            }
        }

        public static int ToInt(this string str)
        {
            try { return Convert.ToInt32(str); }
            catch { return 0; }
        }

        public static decimal ToDecimal(this string str)
        {
            try { return Convert.ToDecimal(str); }
            catch { return (decimal)0; }
        }

        public static DateTime ToDateTimeExact(this string str)
        {
            try
            {
                return DateTime.ParseExact(str, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
            }
            catch
            {
                return new DateTime();
            }
        }

        /// <summary>
        /// Covertion from yyyy-MM-dd format
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static DateTime ToDateTimeExactV1(this string str)
        {
            try
            {
                return DateTime.ParseExact(Convert.ToDateTime(str).ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture), "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
            }
            catch
            {
                return new DateTime();
            }
        }

        public static DateTime ToDateTime(this string str)
        {
            try { return Convert.ToDateTime(str); }
            catch { return new DateTime(); }
        }

        public static float ToFloat(this string str)
        {
            try { return float.Parse(str); }
            catch { return 0; }
        }

        public static List<List<T>> ChunkBy<T>(this IList<T> source, int chunkSize)
        {
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }

        /// <summary>
        /// To get string array of given html tags
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static List<string> SplitByHTMLTag(this string text, string htmlTag)
        {
            var textByTag = new List<string>();
            var stringPattern = $"<\\s*{htmlTag}[^>]*>([\\s\\S]*?)<\\s*\\/\\s*{htmlTag}>";
            var matches = Regex.Matches(text, stringPattern);
            if(matches.Count == 0)
                textByTag.Add(text);

            foreach (Match match in matches)
            {
                textByTag.Add(match.Groups[1].Value);
            }
            return textByTag;
        }
    }
}