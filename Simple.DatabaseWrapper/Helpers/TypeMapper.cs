using System;
using System.Data.Common;
using System.Drawing;
using Simple.DatabaseWrapper.TypeReader;

namespace Simple.DatabaseWrapper.Helpers
{
    public class TypeMapper
    {
        public static T MapObject<T>(string[] colNames, DbDataReader reader, ReaderCachedCollection cachedCollection)
        {
            var info = cachedCollection.GetInfo<T>();
            object t = Activator.CreateInstance<T>();

            foreach (var p in info.Items)
            {
                var clnIdx = Array.IndexOf(colNames, p.Name);
                if (clnIdx < 0) continue;

                mapColumn(t, p, reader, clnIdx);
            }
            return (T)t;
        }
        private static void mapColumn<T>(T obj, TypeItemInfo t, DbDataReader reader, int clnIdx)
            where T : new()
        {
            object objVal = ReadValue(reader, t.Type, clnIdx);
            t.SetValue(obj, objVal);
        }

        public static object ReadValue(DbDataReader reader, Type type, int ColumnIndex)
        {
            object objVal;
            if (reader.IsDBNull(ColumnIndex))
            {
                objVal = null;
            }
            else
            {
                if (type == typeof(string)) objVal = reader.GetString(ColumnIndex);
                else if (type == typeof(Uri)) objVal = new Uri((string)reader.GetValue(ColumnIndex));
                else if (type == typeof(double) || type == typeof(double?)) objVal = reader.GetDouble(ColumnIndex);
                else if (type == typeof(float) || type == typeof(float?)) objVal = reader.GetFloat(ColumnIndex);
                else if (type == typeof(decimal) || type == typeof(decimal?)) objVal = reader.GetDecimal(ColumnIndex);
                else if (type == typeof(int) || type == typeof(int?)) objVal = reader.GetInt32(ColumnIndex);
                else if (type == typeof(uint) || type == typeof(uint?)) objVal = Convert.ToUInt32(reader.GetValue(ColumnIndex));
                else if (type == typeof(long) || type == typeof(long?)) objVal = reader.GetInt64(ColumnIndex);
                else if (type == typeof(ulong) || type == typeof(ulong?)) objVal = Convert.ToUInt64(reader.GetValue(ColumnIndex));
                else if (type == typeof(bool) || type == typeof(bool?)) objVal = reader.GetBoolean(ColumnIndex);
                else if (type == typeof(DateTime) || type == typeof(DateTime?)) objVal = reader.GetDateTime(ColumnIndex);
                else if (type == typeof(TimeSpan) || type == typeof(TimeSpan?)) objVal = TimeSpan.FromTicks(reader.GetInt64(ColumnIndex));
                else if (type == typeof(byte[])) objVal = (byte[])reader.GetValue(ColumnIndex);
                else if (type == typeof(Guid) || type == typeof(Guid?)) objVal = reader.GetGuid(ColumnIndex);
                else if (type == typeof(Color) || type == typeof(Color?))
                {
                    var argb = (byte[])reader.GetValue(ColumnIndex);
                    objVal = Color.FromArgb(argb[0], argb[1], argb[2], argb[3]);
                }
                else if (type.IsEnum) objVal = reader.GetInt32(ColumnIndex);
                else objVal = reader.GetValue(ColumnIndex);
            }
            return objVal;
        }
    }
}
