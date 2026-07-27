using System;
using System.Globalization;
using AwesomeAssertions;
using Xunit;

namespace Gehtsoft.Measurements.Test
{
    /// <summary>
    /// The span-based parsing API and the `IParsable`/`ISpanParsable` implementations.
    /// </summary>
    public class SpanParseTest
    {
        private static readonly CultureInfo gInvariant = CultureInfo.InvariantCulture;

        public static readonly TheoryData<string, double, TestUnit> ParseCases = new TheoryData<string, double, TestUnit>
        {
            { "10.5u2", 10.5, TestUnit.Unit2 },
            { "10.5\"", 10.5, TestUnit.Unit1 },
            { "10.5u1", 10.5, TestUnit.Unit1 },
            { "-1,234.5u2", -1234.5, TestUnit.Unit2 },
            { "10.5n1", 10.5, TestUnit.Base },
            { "10.5n2", 10.5, TestUnit.Base },
        };

        [Theory]
        [MemberData(nameof(ParseCases))]
        public void SpanOverloadsAgreeWithStringOverloads(string text, double expected, TestUnit unit)
        {
            Measurement<TestUnit>.TryParse(gInvariant, text.AsSpan(), out Measurement<TestUnit> bySpan).Should().BeTrue();
            bySpan.Should().Be(new Measurement<TestUnit>(expected, unit));

            Measurement<TestUnit>.TryParse(text.AsSpan(), gInvariant, out Measurement<TestUnit> byProvider).Should().BeTrue();
            byProvider.Should().Be(bySpan);

            Measurement<TestUnit>.TryParse(text, gInvariant, out Measurement<TestUnit> byStringProvider).Should().BeTrue();
            byStringProvider.Should().Be(bySpan);

            Measurement<TestUnit>.Parse(text.AsSpan(), gInvariant).Should().Be(bySpan);
            Measurement<TestUnit>.Parse(text, gInvariant).Should().Be(bySpan);

            DecimalMeasurement<TestUnit>.TryParse(gInvariant, text.AsSpan(), out DecimalMeasurement<TestUnit> decimalBySpan).Should().BeTrue();
            decimalBySpan.Should().Be(new DecimalMeasurement<TestUnit>((decimal)expected, unit));
            DecimalMeasurement<TestUnit>.Parse(text.AsSpan(), gInvariant).Should().Be(decimalBySpan);
        }

        /// <summary>
        /// The point of the span API - reading a value out of a larger buffer without cutting a substring first.
        /// </summary>
        [Fact]
        public void ParsesASliceOfALargerBuffer()
        {
            const string csv = "name,10.5u2,other";
            ReadOnlySpan<char> field = csv.AsSpan(5, 6);

            Measurement<TestUnit>.TryParse(gInvariant, field, out Measurement<TestUnit> value).Should().BeTrue();
            value.Should().Be(new Measurement<TestUnit>(10.5, TestUnit.Unit2));

            DecimalMeasurement<TestUnit>.TryParse(gInvariant, field, out DecimalMeasurement<TestUnit> decimalValue).Should().BeTrue();
            decimalValue.Should().Be(new DecimalMeasurement<TestUnit>(10.5m, TestUnit.Unit2));
        }

        [Theory]
        [InlineData("10.5unknown")]
        [InlineData("abc")]
        [InlineData("10.5")]
        [InlineData("")]
        public void ParseThrowsFormatExceptionOnBadInput(string text)
        {
            Action parse = () => Measurement<TestUnit>.Parse(text, gInvariant);
            parse.Should().Throw<FormatException>();

            Action parseSpan = () => Measurement<TestUnit>.Parse(text.AsSpan(), gInvariant);
            parseSpan.Should().Throw<FormatException>();

            Action parseDecimal = () => DecimalMeasurement<TestUnit>.Parse(text, gInvariant);
            parseDecimal.Should().Throw<FormatException>();

            Measurement<TestUnit>.TryParse(gInvariant, text.AsSpan(), out _).Should().BeFalse();
        }

        [Fact]
        public void NonInvariantCultureIsHonoured()
        {
            var de = new CultureInfo("de-DE");

            Measurement<TestUnit>.TryParse(de, "10,5u2".AsSpan(), out Measurement<TestUnit> value).Should().BeTrue();
            value.Value.Should().Be(10.5);

            Measurement<TestUnit>.TryParse("10,5u2".AsSpan(), de, out Measurement<TestUnit> byProvider).Should().BeTrue();
            byProvider.Value.Should().Be(10.5);
        }

        /// <summary>
        /// A null text is a routine parse failure, not an exception.
        /// </summary>
        [Fact]
        public void NullTextIsNotParseable()
        {
            Measurement<TestUnit>.TryParse((string)null, out _).Should().BeFalse();
            Measurement<TestUnit>.TryParse(gInvariant, (string)null, out _).Should().BeFalse();
            Measurement<TestUnit>.TryParse((string)null, gInvariant, out _).Should().BeFalse();

            DecimalMeasurement<TestUnit>.TryParse((string)null, out _).Should().BeFalse();
            DecimalMeasurement<TestUnit>.TryParse(gInvariant, (string)null, out _).Should().BeFalse();
        }

        /// <summary>
        /// The text constructor reports a null argument as such instead of failing with a null reference.
        /// </summary>
        [Fact]
        public void NullTextConstructorThrowsArgumentNullException()
        {
            Action create = () => new Measurement<TestUnit>((string)null);
            create.Should().Throw<ArgumentNullException>();

            Action createDecimal = () => new DecimalMeasurement<TestUnit>((string)null);
            createDecimal.Should().Throw<ArgumentNullException>();
        }

        // The generic helpers below compile only if the types really do satisfy the interface
        // constraints, which is the whole point of implementing them.
        private static TValue ParseThroughInterface<TValue>(string text, IFormatProvider provider)
            where TValue : IParsable<TValue> => TValue.Parse(text, provider);

        private static bool TryParseThroughInterface<TValue>(ReadOnlySpan<char> text, IFormatProvider provider, out TValue value)
            where TValue : ISpanParsable<TValue> => TValue.TryParse(text, provider, out value);

        [Fact]
        public void UsableThroughTheParsableInterfaces()
        {
            ParseThroughInterface<Measurement<TestUnit>>("10.5u2", gInvariant)
                .Should().Be(new Measurement<TestUnit>(10.5, TestUnit.Unit2));

            ParseThroughInterface<DecimalMeasurement<TestUnit>>("10.5u2", gInvariant)
                .Should().Be(new DecimalMeasurement<TestUnit>(10.5m, TestUnit.Unit2));

            TryParseThroughInterface("10.5u2".AsSpan(), gInvariant, out Measurement<TestUnit> value).Should().BeTrue();
            value.Should().Be(new Measurement<TestUnit>(10.5, TestUnit.Unit2));

            TryParseThroughInterface("nonsense".AsSpan(), gInvariant, out Measurement<TestUnit> _).Should().BeFalse();
        }
    }
}
