using System;
using System.Data;

namespace Util
{
    public static class DbPropertyHelper
    {
        #region Property mapping helpers from Reader

        //********************************************************************
        // Helpers to move data from the DataRow to the business object
        //********************************************************************
        public static bool BoolPropertyFromRow(IDataReader reader, string propName)
        {
            var result = default(bool);
            try
            {
                if (reader[propName] != DBNull.Value)
                {
                    result = Convert.ToBoolean(reader[propName]);
                }
            }
            catch
            {
            }
            return result;
        }

        public static bool BoolDefaultPropertyFromRow(IDataReader reader, string propName)
        {
            try
            {
                return reader[propName] != DBNull.Value && Convert.ToBoolean(reader[propName]);
            }
            catch
            {
                return false;
            }
        }

        public static bool? BoolNullablePropertyFromRow(IDataReader reader, string propName)
        {
            var result = default(bool?);
            try
            {
                if (reader[propName] != DBNull.Value)
                {
                    result = Convert.ToBoolean(reader[propName]);
                }
            }
            catch
            {
            }
            return result;
        }

        public static string StringDefaultPropertyFromRow(IDataReader reader, string propName)
        {
            var result = string.Empty;
            try
            {
                if (reader[propName] != DBNull.Value)
                {
                    result = Convert.ToString(reader[propName]);
                }
            }
            catch
            {
            }
            return result;
        }

        public static string StringPropertyFromRow(IDataReader reader, string propName)
        {
            var result = StringDefaultPropertyFromRow(reader, propName);
            return result;
        }

        public static string StringPropertyFromRowWithTrim(IDataReader reader, string propName)
        {
            var result = StringDefaultPropertyFromRow(reader, propName);
            return result.Trim();
        }

        public static sbyte SBytePropertyFromRow(IDataReader reader, string propName)
        {
            var result = default(sbyte);
            try
            {
                if (reader[propName] != DBNull.Value)
                {
                    result = Convert.ToSByte(reader[propName]);
                }
            }
            catch
            {
            }
            return result;
        }

        public static sbyte? SByteNullablePropertyFromRow(IDataReader reader, string propName)
        {
            var result = default(sbyte?);
            try
            {
                if (reader[propName] != DBNull.Value)
                {
                    result = Convert.ToSByte(reader[propName]);
                }
            }
            catch
            {
            }
            return result;
        }

        public static byte BytePropertyFromRow(IDataReader reader, string propName)
        {
            var result = default(byte);
            try
            {
                if (reader[propName] != DBNull.Value)
                {
                    result = Convert.ToByte(reader[propName]);
                }
            }
            catch
            {
            }
            return result;
        }

        public static byte? ByteNullablePropertyFromRow(IDataReader reader, string propName)
        {
            var result = default(byte?);
            try
            {
                if (reader[propName] != DBNull.Value)
                {
                    result = Convert.ToByte(reader[propName]);
                }
            }
            catch
            {
            }
            return result;
        }

        public static Int16 Int16PropertyFromRow(IDataReader reader, string propName)
        {
            var result = default(Int16);
            try
            {
                if (reader[propName] != DBNull.Value)
                {
                    result = Convert.ToInt16(reader[propName]);
                }
            }
            catch
            {
            }
            return result;
        }

        public static Int16? Int16NullablePropertyFromRow(IDataReader reader, string propName)
        {
            var result = default(Int16?);
            try
            {
                if (reader[propName] != DBNull.Value)
                {
                    result = Convert.ToInt16(reader[propName]);
                }
            }
            catch
            {
            }
            return result;
        }

        public static int Int32PropertyFromRow(IDataReader reader, string propName)
        {
            var result = default(int);
            try
            {
                if (reader[propName] != DBNull.Value)
                {
                    result = Convert.ToInt32(reader[propName]);
                }
            }
            catch
            {
            }
            return result;
        }

        public static int Int32DefaultPropertyFromRow(IDataReader reader, string propName)
        {
            return Int32PropertyFromRow(reader, propName);
        }

        public static Int32? Int32NullablePropertyFromRow(IDataReader reader, string propName)
        {
            return Int32PropertyFromRow(reader, propName);
        }

        public static Int64 Int64PropertyFromRow(IDataReader reader, string propName)
        {
            var result = default(Int64);
            try
            {
                if (reader[propName] != DBNull.Value)
                {
                    result = Convert.ToInt64(reader[propName]);
                }
            }
            catch
            {
            }
            return result;
        }

        public static Int64? Int64NullablePropertyFromRow(IDataReader reader, string propName)
        {
            var result = default(Int64?);
            try
            {
                if (reader[propName] != DBNull.Value)
                {
                    result = Convert.ToInt64(reader[propName]);
                }
            }
            catch
            {
            }
            return result;
        }

        public static DateTime DateTimePropertyFromRow(IDataReader reader, string propName)
        {
            var result = default(DateTime);
            try
            {
                result = reader[propName] != DBNull.Value ? Convert.ToDateTime(reader[propName]) : result;
            }
            catch
            {
            }
            return result;
        }

        public static DateTime? DateTimeNullablePropertyFromRow(IDataReader reader, string propName)
        {
            var result = default(DateTime?);
            try
            {
                if (reader[propName] != DBNull.Value)
                {
                    result = Convert.ToDateTime(reader[propName]);
                }
            }
            catch
            {
            }
            return result;
        }

        public static decimal DecimalPropertyFromRow(IDataReader reader, string propName)
        {
            var result = default(decimal);
            try
            {
                result = reader[propName] != DBNull.Value ? Convert.ToDecimal(reader[propName]) : result;
            }
            catch
            {
            }
            return result;
        }

        public static Decimal? DecimalNullablePropertyFromRow(IDataReader reader, string propName)
        {
            var result = default(Decimal?);
            try
            {
                if (reader[propName] != DBNull.Value)
                {
                    result = Convert.ToDecimal(reader[propName]);
                }
            }
            catch
            {
            }
            return result;
        }

        public static decimal DecimalDefaultNullablePropertyFromRow(IDataReader reader, string propName)
        {
            var result = default(Decimal);
            try
            {
                if (reader[propName] != DBNull.Value)
                {
                    result = Convert.ToDecimal(reader[propName]);
                }
            }
            catch
            {
            }
            return result;
        }

        public static float FloatPropertyFromRow(IDataReader reader, string propName)
        {
            var result = default(float);
            try
            {
                if (reader[propName] != DBNull.Value)
                {
                    result = Convert.ToSingle(reader[propName]);
                }
            }
            catch
            {
            }
            return result;
        }

        public static float? FloatNullablePropertyFromRow(IDataReader reader, string propName)
        {
            return reader[propName] == DBNull.Value ? (Single?)null : Convert.ToSingle(reader[propName]);
        }

        public static double DoublePropertyFromRow(IDataReader reader, string propName)
        {
            var result = default(double);
            try
            {
                if (reader[propName] != DBNull.Value)
                {
                    result = Convert.ToDouble(reader[propName]);
                }
            }
            catch
            {
            }
            return result;
        }

        public static double? DoubleNullablePropertyFromRow(IDataReader reader, string propName)
        {
            var result = default(double?);
            try
            {
                if (reader[propName] != DBNull.Value)
                {
                    result = Convert.ToDouble(reader[propName]);
                }
            }
            catch
            {
            }
            return result;
        }

        public static TimeSpan TimeSpanPropertyFromRow(IDataReader reader, string propName)
        {
            var data = Convert.ToString(reader[propName]);
            return string.IsNullOrWhiteSpace(data) ? TimeSpan.Zero : Convert.ToDateTime(reader[propName]).TimeOfDay;
        }

        #endregion Property mapping helpers from Reader

        #region Property mapping helpers from DataRow

        public static bool BoolPropertyFromRow(DataRow row, string propName)
        {
            return Convert.ToBoolean(row[propName]);
        }

        public static bool BoolDefaultPropertyFromRow(DataRow row, string propName)
        {
            try
            {
                return row[propName] != null && Convert.ToBoolean(row[propName]);
            }
            catch
            {
                return false;
            }
        }

        public static bool? BoolNullablePropertyFromRow(DataRow row, string propName)
        {
            return row[propName] == null && Convert.ToBoolean(row[propName]);
        }

        public static string StringDefaultPropertyFromRow(DataRow row, string propName, bool isEnum = false)
        {
            try
            {
                var result = row[propName] == null ? "" : Convert.ToString(row[propName]);
                if (isEnum && string.IsNullOrWhiteSpace(result))
                {
                    result = "0";
                }
                return result;
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string StringPropertyFromRow(DataRow row, string propName)
        {
            var result = string.Empty;
            try
            {
                if (row[propName] != DBNull.Value)
                {
                    result = Convert.ToString(row[propName]);
                }
            }
            catch
            {
            }
            return result;
        }

        public static string StringPropertyFromRowWithTrim(DataRow row, string propName)
        {
            var result = StringPropertyFromRow(row, propName);
            return result.Trim();
        }

        public static sbyte SBytePropertyFromRow(DataRow row, string propName)
        {
            return Convert.ToSByte(row[propName]);
        }

        public static sbyte? SByteNullablePropertyFromRow(DataRow row, string propName)
        {
            return row[propName] == null ? (sbyte?)null : Convert.ToSByte(row[propName]);
        }

        public static byte BytePropertyFromRow(DataRow row, string propName)
        {
            return Convert.ToByte(row[propName]);
        }

        public static Byte? ByteNullablePropertyFromRow(DataRow row, string propName)
        {
            return row[propName] == null ? (Byte?)null : Convert.ToByte(row[propName]);
        }

        public static Int16 Int16PropertyFromRow(DataRow row, string propName)
        {
            return Convert.ToInt16(row[propName]);
        }

        public static Int16? Int16NullablePropertyFromRow(DataRow row, string propName)
        {
            return row[propName] == null ? (Int16?)null : Convert.ToInt16(row[propName]);
        }

        public static int Int32PropertyFromRow(DataRow row, string propName)
        {
            var result = default(int);
            try
            {
                if (row[propName] != DBNull.Value)
                {
                    result = Convert.ToInt32(row[propName]);
                }
            }
            catch
            {
            }
            return result;
        }

        public static int Int32DefaultPropertyFromRow(DataRow row, string propName)
        {
            try
            {
                return row[propName] == DBNull.Value ? 0 : Convert.ToInt32(row[propName]);
            }
            catch
            {
                return 0;
            }
        }

        public static Int32? Int32NullablePropertyFromRow(DataRow row, string propName)
        {
            var result = default(Int32?);
            try
            {
                if (row[propName] != DBNull.Value)
                {
                    result = Convert.ToInt32(row[propName]);
                }
            }
            catch
            {
            }
            return result;
        }

        public static Int64 Int64PropertyFromRow(DataRow row, string propName)
        {
            return Convert.ToInt64(row[propName]);
        }

        public static Int64? Int64NullablePropertyFromRow(DataRow row, string propName)
        {
            return row[propName] == null ? (Int64?)null : Convert.ToInt64(row[propName]);
        }

        public static DateTime DateTimePropertyFromRow(DataRow row, string propName)
        {
            var result = default(DateTime);
            try
            {
                if (row[propName] != DBNull.Value)
                {
                    result = Convert.ToDateTime(row[propName]);
                }
            }
            catch
            {
            }
            return result;
        }

        public static DateTime? DateTimeNullablePropertyFromRow(DataRow row, string propName)
        {
            try
            {
                return row[propName] == null ? (DateTime?)null : Convert.ToDateTime(row[propName]);
            }
            catch
            {
                return null;
            }
        }

        public static decimal DecimalPropertyFromRow(DataRow row, string propName)
        {
            var result = default(decimal);
            try
            {
                if (row[propName] != DBNull.Value)
                {
                    result = Convert.ToDecimal(row[propName]);
                }
            }
            catch
            {
            }
            return result;
        }

        public static Decimal? DecimalNullablePropertyFromRow(DataRow row, string propName)
        {
            return row[propName] == null ? (Decimal?)null : Convert.ToDecimal(row[propName]);
        }

        public static decimal DecimalDefaultNullablePropertyFromRow(DataRow row, string propName)
        {
            return row[propName] == null ? 0 : Convert.ToDecimal(row[propName]);
        }

        public static float FloatPropertyFromRow(DataRow row, string propName)
        {
            try
            {
                return row[propName] == DBNull.Value ? 0 : Convert.ToSingle(row[propName]);
            }
            catch
            {
                return 0;
            }
        }

        public static float? FloatNullablePropertyFromRow(DataRow row, string propName)
        {
            return row[propName] == null ? (Single?)null : Convert.ToSingle(row[propName]);
        }

        public static double DoublePropertyFromRow(DataRow row, string propName)
        {
            try
            {
                return row[propName] == DBNull.Value ? 0 : Convert.ToDouble(row[propName]);
            }
            catch
            {
                return 0;
            }
        }

        public static double? DoubleNullablePropertyFromRow(DataRow row, string propName)
        {
            return row[propName] == null ? (Double?)null : Convert.ToDouble(row[propName]);
        }

        #endregion Property mapping helpers from DataRow
    }
}