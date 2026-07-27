using System;
using System.Globalization;
using AwesomeAssertions;
using Xunit;

namespace Gehtsoft.Measurements.Test
{
    /// <summary>
    /// Guards the allocation-free parse.
    /// </summary>
    /// <remarks>
    /// The parser slices the input with spans and resolves the unit by an ordinal span scan
    /// over the unit list, so a successful parse allocates nothing. A regression to
    /// substring-based slicing or to a boxed lookup still passes every functional parse test,
    /// but fails here.
    /// </remarks>
    public class ParseAllocationTest
    {
        // Value-type sinks. Consuming the parse result keeps the call under test from
        // being elided without boxing anything (which would be counted as an allocation).
        private static double mDoubleSink;
        private static decimal mDecimalSink;
        private static int mUnitSink;

        private static long AllocatedBy(Action action) => AllocationProbe.AllocatedBy(action);

        private const string Because = "a successful parse must not allocate (span slicing + ordinal span unit lookup)";

        [Theory]
        [InlineData("10.5u2")]      // canonical unit name
        [InlineData("10.5\"")]      // non-alphanumeric unit name
        [InlineData("10.5u1")]      // alternative unit name
        [InlineData("10.5n1")]      // last unit of the parse list: worst-case scan
        [InlineData("10.5n2")]      // alternative name of the last unit
        [InlineData("-1,234.5u2")]  // negative sign and group separator
        public void TryParse_WithCulture_Success_DoesNotAllocate(string text)
        {
            CultureInfo culture = CultureInfo.InvariantCulture;
            Measurement<TestUnit>.TryParse(culture, text, out _).Should().BeTrue("the test input must be parseable");

            Action parse = () =>
            {
                Measurement<TestUnit>.TryParse(culture, text, out Measurement<TestUnit> value);
                mDoubleSink += value.Value;
                mUnitSink += (int)value.Unit;
            };

            AllocatedBy(parse).Should().Be(0, Because);
        }

        [Theory]
        [InlineData("10.5u2")]
        [InlineData("10.5\"")]
        [InlineData("-1,234.5u2")]
        public void TryParse_Span_Success_DoesNotAllocate(string text)
        {
            CultureInfo culture = CultureInfo.InvariantCulture;
            Measurement<TestUnit>.TryParse(culture, text.AsSpan(), out _).Should().BeTrue("the test input must be parseable");

            Action parse = () =>
            {
                Measurement<TestUnit>.TryParse(culture, text.AsSpan(), out Measurement<TestUnit> value);
                mDoubleSink += value.Value;
                mUnitSink += (int)value.Unit;
            };

            AllocatedBy(parse).Should().Be(0, Because);
        }

        [Fact]
        public void Parse_Span_Success_DoesNotAllocate()
        {
            CultureInfo culture = CultureInfo.InvariantCulture;

            Action parse = () =>
            {
                Measurement<TestUnit> value = Measurement<TestUnit>.Parse("10.5u2".AsSpan(), culture);
                mDoubleSink += value.Value;
                mUnitSink += (int)value.Unit;
            };

            AllocatedBy(parse).Should().Be(0, Because);
        }

        [Fact]
        public void Decimal_TryParse_Span_Success_DoesNotAllocate()
        {
            CultureInfo culture = CultureInfo.InvariantCulture;

            Action parse = () =>
            {
                DecimalMeasurement<TestUnit>.TryParse(culture, "10.5u2".AsSpan(), out DecimalMeasurement<TestUnit> value);
                mDecimalSink += value.Value;
                mUnitSink += (int)value.Unit;
            };

            AllocatedBy(parse).Should().Be(0, Because);
        }

        [Fact]
        public void TryParse_WithCurrentCulture_Success_DoesNotAllocate()
        {
            // No decimal separator: the input must parse regardless of the culture the
            // test host happens to run under.
            const string text = "10u2";
            Measurement<TestUnit>.TryParse(text, out _).Should().BeTrue("the test input must be parseable");

            Action parse = () =>
            {
                Measurement<TestUnit>.TryParse(text, out Measurement<TestUnit> value);
                mDoubleSink += value.Value;
                mUnitSink += (int)value.Unit;
            };

            AllocatedBy(parse).Should().Be(0, Because);
        }

        [Fact]
        public void TryParse_NonInvariantCulture_Success_DoesNotAllocate()
        {
            CultureInfo culture = new CultureInfo("de-DE");
            const string text = "10,5u2";
            Measurement<TestUnit>.TryParse(culture, text, out _).Should().BeTrue("the test input must be parseable");

            Action parse = () =>
            {
                Measurement<TestUnit>.TryParse(culture, text, out Measurement<TestUnit> value);
                mDoubleSink += value.Value;
                mUnitSink += (int)value.Unit;
            };

            AllocatedBy(parse).Should().Be(0, Because);
        }

        [Fact]
        public void Constructor_FromText_DoesNotAllocate()
        {
            Action parse = () =>
            {
                var value = new Measurement<TestUnit>("10.5u2");
                mDoubleSink += value.Value;
                mUnitSink += (int)value.Unit;
            };

            AllocatedBy(parse).Should().Be(0, Because);
        }

        [Theory]
        [InlineData("u2")]
        [InlineData("\"")]
        [InlineData("u1")]
        [InlineData("n1")]
        public void ParseUnitName_Success_DoesNotAllocate(string name)
        {
            Action parse = () => mUnitSink += (int)Measurement<TestUnit>.ParseUnitName(name);

            AllocatedBy(parse).Should().Be(0, Because);
        }

        [Fact]
        public void TryParse_RealUnitSet_Success_DoesNotAllocate()
        {
            // A production unit set, which has a longer parse list than TestUnit.
            CultureInfo culture = CultureInfo.InvariantCulture;
            Measurement<DistanceUnit>.TryParse(culture, "10.5in", out _).Should().BeTrue("the test input must be parseable");

            Action parse = () =>
            {
                Measurement<DistanceUnit>.TryParse(culture, "10.5in", out Measurement<DistanceUnit> value);
                mDoubleSink += value.Value;
                mUnitSink += (int)value.Unit;
            };

            AllocatedBy(parse).Should().Be(0, Because);
        }

        [Theory]
        [InlineData("10.5u2")]
        [InlineData("10.5\"")]
        [InlineData("10.5u1")]
        [InlineData("10.5n1")]
        [InlineData("-1,234.5u2")]
        public void Decimal_TryParse_WithCulture_Success_DoesNotAllocate(string text)
        {
            CultureInfo culture = CultureInfo.InvariantCulture;
            DecimalMeasurement<TestUnit>.TryParse(culture, text, out _).Should().BeTrue("the test input must be parseable");

            Action parse = () =>
            {
                DecimalMeasurement<TestUnit>.TryParse(culture, text, out DecimalMeasurement<TestUnit> value);
                mDecimalSink += value.Value;
                mUnitSink += (int)value.Unit;
            };

            AllocatedBy(parse).Should().Be(0, Because);
        }

        [Fact]
        public void Decimal_Constructor_FromText_DoesNotAllocate()
        {
            Action parse = () =>
            {
                var value = new DecimalMeasurement<TestUnit>("10.5u2");
                mDecimalSink += value.Value;
                mUnitSink += (int)value.Unit;
            };

            AllocatedBy(parse).Should().Be(0, Because);
        }

        [Fact]
        public void Decimal_ParseUnitName_Success_DoesNotAllocate()
        {
            Action parse = () => mUnitSink += (int)DecimalMeasurement<TestUnit>.ParseUnitName("u2");

            AllocatedBy(parse).Should().Be(0, Because);
        }
    }
}
