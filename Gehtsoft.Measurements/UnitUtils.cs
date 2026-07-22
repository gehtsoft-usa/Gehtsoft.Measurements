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
            var rv = new List<Tuple<T, string>>(fields.Length);
            for (int i = 0; i < fields.Length; i++)
            {
                // Obsolete members (e.g. renamed misspellings) still convert and format,
                // but are excluded from the public unit listing.
                if (fields[i].GetCustomAttribute<ObsoleteAttribute>() != null)
                    continue;
                UnitAttribute attribute = fields[i].GetCustomAttribute<UnitAttribute>();
                rv.Add(new Tuple<T, string>((T)fields[i].GetRawConstantValue(), attribute.Name));
            }
            return rv.ToArray();
        }

        /// <summary>
        /// Builds a (name, unit) list (including alternative names) for a non-throwing,
        /// allocation-free unit-name parse. An array of short names is scanned with an
        /// ordinal span comparison, which lets the parser slice the input with spans
        /// instead of allocating substrings; the unit sets are small enough that the
        /// linear scan is faster than allocating a probe string for a dictionary.
        /// </summary>
        public static (string Name, T Unit)[] GetParseList<T>()
            where T : Enum
        {
            Type type = typeof(T);
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);
            var list = new List<(string, T)>(fields.Length);
            for (int i = 0; i < fields.Length; i++)
            {
                // Skip obsolete members so a renamed misspelling does not shadow the
                // canonical unit (they share the same name) or introduce an ambiguity.
                if (fields[i].GetCustomAttribute<ObsoleteAttribute>() != null)
                    continue;
                UnitAttribute attribute = fields[i].GetCustomAttribute<UnitAttribute>();
                T value = (T)fields[i].GetRawConstantValue();
                list.Add((attribute.Name, value));
                if (attribute.HasAlternativeName)
                    list.Add((attribute.AlternativeName, value));
            }
            return list.ToArray();
        }
    }
}
