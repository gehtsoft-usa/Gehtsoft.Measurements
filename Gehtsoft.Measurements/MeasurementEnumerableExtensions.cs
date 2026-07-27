using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Gehtsoft.Measurements
{
    /// <summary>
    /// The aggregation extensions for sequences of measurements.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Only the aggregations which the standard `System.Linq` operators cannot express are
    /// defined here. `Min`, `Max`, `MinBy` and `MaxBy` already work on sequences of measurements
    /// because both measurement types implement `IComparable`, and they compare across units,
    /// so `Min` of `1ft` and `6in` is `6in`. `Sum` and `Average` accept only the built-in
    /// numeric types, which is why they are provided here.
    /// </para>
    /// <para>
    /// The result carries the unit of the first element of the sequence, because that is the
    /// unit every other element is converted into while the total is accumulated.
    /// </para>
    /// </remarks>
    public static class MeasurementEnumerableExtensions
    {
        /// <summary>
        /// Calculates the sum of a sequence of measurements.
        /// </summary>
        /// <remarks>
        /// The sum of an empty sequence is a zero of the base unit.
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        public static Measurement<T> Sum<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] T>(this IEnumerable<Measurement<T>> values)
            where T : Enum
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            using (var enumerator = values.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                    return Measurement<T>.ZERO;

                Measurement<T> sum = enumerator.Current;
                while (enumerator.MoveNext())
                    sum += enumerator.Current;
                return sum;
            }
        }

        /// <summary>
        /// Calculates the sum of a sequence of measurements.
        /// </summary>
        /// <remarks>
        /// The sum of an empty sequence is a zero of the base unit.
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        public static DecimalMeasurement<T> Sum<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] T>(this IEnumerable<DecimalMeasurement<T>> values)
            where T : Enum
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            using (var enumerator = values.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                    return DecimalMeasurement<T>.ZERO;

                DecimalMeasurement<T> sum = enumerator.Current;
                while (enumerator.MoveNext())
                    sum += enumerator.Current;
                return sum;
            }
        }

        /// <summary>
        /// Calculates the average of a sequence of measurements.
        /// </summary>
        /// <remarks>
        /// The sequence must not be empty, exactly as for the standard `Average` operator.
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        public static Measurement<T> Average<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] T>(this IEnumerable<Measurement<T>> values)
            where T : Enum
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            using (var enumerator = values.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                    throw new InvalidOperationException("The sequence contains no elements");

                Measurement<T> sum = enumerator.Current;
                int count = 1;
                while (enumerator.MoveNext())
                {
                    sum += enumerator.Current;
                    count++;
                }
                return sum / count;
            }
        }

        /// <summary>
        /// Calculates the average of a sequence of measurements.
        /// </summary>
        /// <remarks>
        /// The sequence must not be empty, exactly as for the standard `Average` operator.
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        public static DecimalMeasurement<T> Average<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] T>(this IEnumerable<DecimalMeasurement<T>> values)
            where T : Enum
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            using (var enumerator = values.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                    throw new InvalidOperationException("The sequence contains no elements");

                DecimalMeasurement<T> sum = enumerator.Current;
                int count = 1;
                while (enumerator.MoveNext())
                {
                    sum += enumerator.Current;
                    count++;
                }
                return sum / count;
            }
        }
    }
}
