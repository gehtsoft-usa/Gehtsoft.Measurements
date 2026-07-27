using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace Gehtsoft.Measurements
{
    internal static class UnitUtils
    {
        /// <summary>
        /// The numeric format strings for the accuracies which a unit can realistically declare.
        /// </summary>
        /// <remarks>
        /// Formatting a value with the unit default accuracy used to build the format string
        /// (`$"N{accuracy}"`) on every single call. The set of possible strings is tiny and fixed,
        /// so they are held here instead.
        /// </remarks>
        private static readonly string[] gAccuracyFormats = new string[]
        {
            "N0", "N1", "N2", "N3", "N4", "N5", "N6", "N7",
            "N8", "N9", "N10", "N11", "N12", "N13", "N14", "N15",
        };

        /// <summary>
        /// Returns the numeric format string for the specified number of decimal places.
        /// </summary>
        public static string AccuracyFormat(int accuracy)
            => accuracy >= 0 && accuracy < gAccuracyFormats.Length
                   ? gAccuracyFormats[accuracy]
                   : "N" + accuracy.ToString(CultureInfo.InvariantCulture);

        /// <summary>
        /// Returns the base unit of the enumeration, validating the enumeration on the way.
        /// </summary>
        /// <remarks>
        /// This is the first thing every closed measurement type does, so it is where a
        /// misdeclared unit enumeration is caught. Without the checks a missing attribute
        /// surfaced as a null reference and a missing base unit did not surface at all - the
        /// base silently became the first enumeration value and every conversion was wrong.
        /// The exception is raised from a static initializer, so callers see it wrapped into
        /// a TypeInitializationException whose inner exception carries the message below.
        /// </remarks>
        public static T GetBase<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] T>()
        {
            Type type = typeof(T);
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);

            bool found = false;
            T baseUnit = default;
            string baseName = null;

            for (int i = 0; i < fields.Length; i++)
            {
                if (fields[i].GetCustomAttribute<UnitAttribute>() == null)
                    throw new InvalidOperationException($"The unit {type.Name}.{fields[i].Name} is not marked with the Unit attribute");

                ConversionAttribute attribute = fields[i].GetCustomAttribute<ConversionAttribute>();
                if (attribute == null)
                    throw new InvalidOperationException($"The unit {type.Name}.{fields[i].Name} is not marked with the Conversion attribute");

                if (attribute.Operation != ConversionOperation.Base)
                    continue;

                if (found)
                    throw new InvalidOperationException($"The unit enumeration {type.Name} declares more than one base unit ({baseName} and {fields[i].Name})");

                found = true;
                baseUnit = (T)fields[i].GetRawConstantValue();
                baseName = fields[i].Name;
            }

            if (!found)
                throw new InvalidOperationException($"The unit enumeration {type.Name} declares no base unit. Exactly one unit must use ConversionOperation.Base");

            return baseUnit;
        }

        public static Tuple<T, string>[] GetUnits<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] T>()
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
        public static (string Name, T Unit)[] GetParseList<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] T>()
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
                AddParseName(list, type, fields[i].Name, attribute.Name, value);
                if (attribute.HasAlternativeName)
                    AddParseName(list, type, fields[i].Name, attribute.AlternativeName, value);
            }
            return list.ToArray();
        }

        /// <summary>
        /// Adds a parse name, rejecting a name which is already taken by another unit.
        /// </summary>
        /// <remarks>
        /// A duplicate name used to be accepted silently and the unit declared first won every
        /// parse, so the shadowed unit could be formatted but never read back.
        /// </remarks>
        private static void AddParseName<T>(List<(string, T)> list, Type type, string field, string name, T value)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (string.Equals(list[i].Item1, name, StringComparison.Ordinal))
                    throw new InvalidOperationException($"The unit enumeration {type.Name} uses the name '{name}' for more than one unit (the latest is {field})");
            }
            list.Add((name, value));
        }
    }
}
