using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text;
using AwesomeAssertions;
using Xunit;

namespace Gehtsoft.Measurements.Test
{
    /// <summary>
    /// The span-based formatting API of both measurement types.
    /// </summary>
    /// <remarks>
    /// The formats are exercised against every unit of every unit enumeration of the library,
    /// because the unit name is copied into the destination by hand and several of the names
    /// are not ASCII (the degree sign, the superscripts, the middle dot).
    /// </remarks>
    public class FormatTest
    {
        private static readonly CultureInfo gInvariant = CultureInfo.InvariantCulture;

        /// <summary>
        /// The measurement formats, plus a plain numeric one and the empty one.
        /// </summary>
        private static readonly string[] gFormats = new[] { null, "", "NF", "ND", "N2", "N0", "F3" };

        private static readonly Type[] gUnitTypes = typeof(Measurement<>).Assembly
                                                                        .GetTypes()
                                                                        .Where(t => t.IsEnum && t.IsPublic)
                                                                        .Where(t => t.GetFields(BindingFlags.Public | BindingFlags.Static)
                                                                                     .Any(f => f.GetCustomAttribute<UnitAttribute>() != null))
                                                                        .OrderBy(t => t.Name)
                                                                        .ToArray();

        public static IEnumerable<object[]> UnitTypes => gUnitTypes.Select(t => new object[] { t });

        [Fact]
        public void TheUnitEnumerationsWereFound()
        {
            gUnitTypes.Should().HaveCount(17);
        }

        [Theory]
        [MemberData(nameof(UnitTypes))]
        public void TryFormatMatchesToString(Type unitType) => RunFor(nameof(TryFormatAgreesWithToString), unitType);

        [Theory]
        [MemberData(nameof(UnitTypes))]
        public void TryFormatUtf8MatchesToString(Type unitType) => RunFor(nameof(Utf8TryFormatAgreesWithToString), unitType);

        private static void TryFormatAgreesWithToString<T>()
            where T : Enum
        {
            Span<char> buffer = stackalloc char[256];

            foreach (T unit in UnitsOf<T>())
            {
                foreach (string format in gFormats)
                {
                    string because = $"{typeof(T).Name}.{unit} formatted as '{format ?? "<null>"}'";

                    var m = new Measurement<T>(-1234.5678, unit);
                    m.TryFormat(buffer, out int written, format.AsSpan(), gInvariant).Should().BeTrue(because);
                    new string(buffer.Slice(0, written)).Should().Be(m.ToString(format, gInvariant), because);

                    var d = new DecimalMeasurement<T>(-1234.5678m, unit);
                    d.TryFormat(buffer, out int decimalWritten, format.AsSpan(), gInvariant).Should().BeTrue(because);
                    new string(buffer.Slice(0, decimalWritten)).Should().Be(d.ToString(format, gInvariant), because);
                }
            }
        }

        private static void Utf8TryFormatAgreesWithToString<T>()
            where T : Enum
        {
            Span<byte> buffer = stackalloc byte[512];

            foreach (T unit in UnitsOf<T>())
            {
                foreach (string format in gFormats)
                {
                    string because = $"{typeof(T).Name}.{unit} formatted as '{format ?? "<null>"}' into UTF-8";

                    var m = new Measurement<T>(-1234.5678, unit);
                    m.TryFormat(buffer, out int written, format.AsSpan(), gInvariant).Should().BeTrue(because);
                    buffer.Slice(0, written).ToArray().Should().Equal(Encoding.UTF8.GetBytes(m.ToString(format, gInvariant)), because);

                    var d = new DecimalMeasurement<T>(-1234.5678m, unit);
                    d.TryFormat(buffer, out int decimalWritten, format.AsSpan(), gInvariant).Should().BeTrue(because);
                    buffer.Slice(0, decimalWritten).ToArray().Should().Equal(Encoding.UTF8.GetBytes(d.ToString(format, gInvariant)), because);
                }
            }
        }

        /// <summary>
        /// A destination which is exactly large enough must succeed, one character short must fail.
        /// </summary>
        [Fact]
        public void TooSmallDestinationFails()
        {
            var m = new Measurement<TestUnit>(1.25, TestUnit.Unit2);
            string expected = m.ToString("NF", gInvariant);      // 1.25u2

            Span<char> exact = stackalloc char[expected.Length];
            m.TryFormat(exact, out int written, "NF".AsSpan(), gInvariant).Should().BeTrue();
            written.Should().Be(expected.Length);
            new string(exact).Should().Be(expected);

            // one short - the unit name no longer fits
            Span<char> tooSmall = stackalloc char[expected.Length - 1];
            m.TryFormat(tooSmall, out int notWritten, "NF".AsSpan(), gInvariant).Should().BeFalse();
            notWritten.Should().Be(0);

            // far too short - even the value does not fit
            Span<char> empty = stackalloc char[1];
            m.TryFormat(empty, out int noneWritten, "NF".AsSpan(), gInvariant).Should().BeFalse();
            noneWritten.Should().Be(0);
        }

        [Fact]
        public void TooSmallUtf8DestinationFails()
        {
            // a unit name which needs more bytes than characters in UTF-8
            var m = new Measurement<TemperatureUnit>(1.25, TemperatureUnit.Fahrenheit);
            byte[] expected = Encoding.UTF8.GetBytes(m.ToString("NF", gInvariant));

            Span<byte> exact = stackalloc byte[expected.Length];
            m.TryFormat(exact, out int written, "NF".AsSpan(), gInvariant).Should().BeTrue();
            written.Should().Be(expected.Length);
            exact.ToArray().Should().Equal(expected);

            Span<byte> tooSmall = stackalloc byte[expected.Length - 1];
            m.TryFormat(tooSmall, out int notWritten, "NF".AsSpan(), gInvariant).Should().BeFalse();
            notWritten.Should().Be(0);
        }

        /// <summary>
        /// The default accuracy format is taken from the unit, exactly as `ToString("ND")` does.
        /// </summary>
        [Fact]
        public void DefaultAccuracyFormatIsResolvedFromTheUnit()
        {
            // TestUnit.Base declares an accuracy of 5, TestUnit.Unit2 declares 2
            new Measurement<TestUnit>(1.5, TestUnit.Base).ToString("ND", gInvariant).Should().Be("1.50000n1");
            new Measurement<TestUnit>(1.5, TestUnit.Unit2).ToString("ND", gInvariant).Should().Be("1.50u2");
        }

        /// <summary>
        /// A format which overflows the stack buffer still produces the right string.
        /// </summary>
        [Fact]
        public void HugeFormatFallsBackAndStillFormats()
        {
            var m = new Measurement<TestUnit>(double.MaxValue, TestUnit.Unit2);

            string formatted = m.ToString("N100", gInvariant);

            formatted.Should().EndWith("u2");
            formatted.Should().Be(double.MaxValue.ToString("N100", gInvariant) + "u2");
        }

        private static IEnumerable<T> UnitsOf<T>()
            where T : Enum
        {
            foreach (var field in typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                if (field.GetCustomAttribute<UnitAttribute>() == null)
                    continue;
                yield return (T)field.GetRawConstantValue();
            }
        }

        /// <summary>
        /// Calls the generic test method for the unit enumeration passed as the theory parameter.
        /// </summary>
        private static void RunFor(string method, Type unitType)
        {
            var generic = typeof(FormatTest).GetMethod(method, BindingFlags.Static | BindingFlags.NonPublic)
                                            .MakeGenericMethod(unitType);
            try
            {
                generic.Invoke(null, null);
            }
            catch (TargetInvocationException e)
            {
                ExceptionDispatchInfo.Capture(e.InnerException).Throw();
            }
        }
    }
}
