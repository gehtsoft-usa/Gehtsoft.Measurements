using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Gehtsoft.Measurements
{
    internal static class UnitUtils
    {
        public static Tuple<T, string>[] GetUnits<T>(Type type)
            where T : Enum
        {
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);
            Tuple<T, string>[] rv = new Tuple<T, string>[fields.Length];
            for (int i = 0; i < fields.Length; i++)
            {
                UnitAttribute attribute = fields[i].GetCustomAttribute<UnitAttribute>();
                rv[i] = new Tuple<T, string>((T)fields[i].GetRawConstantValue(), attribute.Name);
            }
            return rv;
        }
    }
}
