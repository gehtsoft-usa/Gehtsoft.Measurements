using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Gehtsoft.Measurements
{
    internal static class UnitUtils
    {
        public static T GetBase<T>()
        {
            Type type = typeof(T);
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);
            for (int i = 0; i < fields.Length; i++)
            {
                ConversionAttribute attribute = fields[i].GetCustomAttribute<ConversionAttribute>();
                if (attribute.Operation == ConversionOperation.Base)
                    return (T)fields[i].GetRawConstantValue();
            }
            return default;
        }

        public static Tuple<T, string>[] GetUnits<T>()
            where T : Enum
        {
            Type type = typeof(T);
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
