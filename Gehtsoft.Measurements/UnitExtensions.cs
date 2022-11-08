using System;
using System.Collections.Generic;
using System.Text;

namespace Gehtsoft.Measurements
{
    /// <summary>
    /// The extensions for unit type enumerations
    /// </summary>
    public static class UnitExtensions
    {
        /// <summary>
        /// <para>Creates a new value of the specified unit.</para>
        /// <para>The method is an extension for a `enum` value, i.e. you can use it as `AngularUnit.MOA.New(10)` instead of writing `new Measurement&lt;AngularUnit&gt;(10, AngularUnit.MOA)`</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="unit"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Measurement<T> New<T>(this T unit, double value) where T : Enum => new Measurement<T>(value, unit);

        /// <summary>
        /// <para>Creates a new value of the specified unit.</para>
        /// <para>The method is an extension for a `enum` value, i.e. you can use it as `AngularUnit.MOA.New(10)` instead of writing `new Measurement&lt;AngularUnit&gt;(10, AngularUnit.MOA)`</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="unit"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DecimalMeasurement<T> NewDecimal<T>(this T unit, decimal value) where T : Enum => new DecimalMeasurement<T>(value, unit);

        /// <summary>
        /// Creates a new value of the specified unit from a double value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        public static Measurement<T> As<T>(this double value, T unit) where T : Enum => new Measurement<T>(value, unit);

        /// <summary>
        /// Creates a new value of the specified unit from an integer value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        public static Measurement<T> As<T>(this int value, T unit) where T : Enum => new Measurement<T>(value, unit);

        /// <summary>
        /// Converts an enumeration of doubles into an enumeration of measures of the specified unit.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        public static IEnumerable<Measurement<T>> As<T>(this IEnumerable<double> values, T unit) where T : Enum
        {
            foreach (var v in values)
                yield return new Measurement<T>(v, unit);
        }

        /// <summary>
        /// Creates a new value of the specified unit from a double value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        public static DecimalMeasurement<T> AsDecimal<T>(this decimal value, T unit) where T : Enum => new DecimalMeasurement<T>(value, unit);
    }
}
