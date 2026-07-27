using System;
using System.Globalization;
using AwesomeAssertions;
using Xunit;

namespace Gehtsoft.Measurements.Test
{
    /// <summary>
    /// The syntax accepted by the parser, and the cases which must stay rejected.
    /// </summary>
    /// <remarks>
    /// The parser looks for the unit at the end of the text, so the value in front of it may
    /// use any notation the numeric type itself accepts, the exponent notation included.
    /// </remarks>
    public class ParseSyntaxTest
    {
        private static readonly CultureInfo gInvariant = CultureInfo.InvariantCulture;

        [Theory]
        [InlineData("1e3u2", 1000)]
        [InlineData("1E3u2", 1000)]
        [InlineData("1e-3u2", 0.001)]
        [InlineData("1.5e2u2", 150)]
        [InlineData("-1e3u2", -1000)]
        [InlineData("1e+3u2", 1000)]
        public void ExponentNotationIsParsed(string text, double expected)
        {
            Measurement<TestUnit>.TryParse(gInvariant, text, out Measurement<TestUnit> value).Should().BeTrue();
            value.Value.Should().Be(expected);
            value.Unit.Should().Be(TestUnit.Unit2);

            DecimalMeasurement<TestUnit>.TryParse(gInvariant, text, out DecimalMeasurement<TestUnit> decimalValue).Should().BeTrue();
            decimalValue.Value.Should().Be((decimal)expected);
            decimalValue.Unit.Should().Be(TestUnit.Unit2);
        }

        /// <summary>
        /// Everything the parser accepted before it started looking for the unit at the end.
        /// </summary>
        [Theory]
        [InlineData("10.5u2", 10.5)]
        [InlineData("-10.5u2", -10.5)]
        [InlineData("+10.5u2", 10.5)]
        [InlineData("-1,234.5u2", -1234.5)]
        [InlineData("10 u2", 10)]
        [InlineData(" 10u2", 10)]
        [InlineData("0u2", 0)]
        public void ClassicNotationStillParses(string text, double expected)
        {
            Measurement<TestUnit>.TryParse(gInvariant, text, out Measurement<TestUnit> value).Should().BeTrue();
            value.Value.Should().Be(expected);
            value.Unit.Should().Be(TestUnit.Unit2);
        }

        [Theory]
        [InlineData("10.5")]            // no unit
        [InlineData("u2")]              // no value
        [InlineData("")]
        [InlineData("x")]
        [InlineData("10.5unknown")]     // unknown unit
        [InlineData("10u2junk")]        // trailing garbage
        [InlineData("abc10u2")]         // the value is not a number
        [InlineData("1.2.3u2")]         // neither is this one
        [InlineData("10u25")]           // the unit does not end the text
        [InlineData("10u2 ")]           // trailing space after the unit
        [InlineData("--10u2")]
        public void InvalidTextIsRejected(string text)
        {
            Measurement<TestUnit>.TryParse(gInvariant, text, out _).Should().BeFalse();
            DecimalMeasurement<TestUnit>.TryParse(gInvariant, text, out _).Should().BeFalse();
        }

        /// <summary>
        /// The longest unit name which ends the text wins, so a name ending with another name is read correctly.
        /// </summary>
        [Fact]
        public void TheLongestMatchingUnitNameWins()
        {
            Measurement<AngularUnit>.TryParse(gInvariant, "1.5mrad", out Measurement<AngularUnit> mrad).Should().BeTrue();
            mrad.Unit.Should().Be(AngularUnit.MRad);

            Measurement<AngularUnit>.TryParse(gInvariant, "1.5rad", out Measurement<AngularUnit> rad).Should().BeTrue();
            rad.Unit.Should().Be(AngularUnit.Radian);

            Measurement<GasConsumptionUnit>.TryParse(gInvariant, "8imp.mpg", out Measurement<GasConsumptionUnit> imperial).Should().BeTrue();
            imperial.Unit.Should().Be(GasConsumptionUnit.ImperialMilesPerGallon);

            Measurement<GasConsumptionUnit>.TryParse(gInvariant, "8mpg", out Measurement<GasConsumptionUnit> us).Should().BeTrue();
            us.Unit.Should().Be(GasConsumptionUnit.MilesPerGallon);

            Measurement<DistanceUnit>.TryParse(gInvariant, "10mm", out Measurement<DistanceUnit> mm).Should().BeTrue();
            mm.Unit.Should().Be(DistanceUnit.Millimeter);

            Measurement<DistanceUnit>.TryParse(gInvariant, "10m", out Measurement<DistanceUnit> m).Should().BeTrue();
            m.Unit.Should().Be(DistanceUnit.Meter);
        }

        /// <summary>
        /// A unit name may hold digits and separators without confusing the value in front of it.
        /// </summary>
        [Fact]
        public void UnitNamesHoldingDigitsAreParsed()
        {
            Measurement<AngularUnit>.TryParse(gInvariant, "2in/100yd", out Measurement<AngularUnit> slope).Should().BeTrue();
            slope.Value.Should().Be(2);
            slope.Unit.Should().Be(AngularUnit.InchesPer100Yards);

            Measurement<GasConsumptionUnit>.TryParse(gInvariant, "8l/100km", out Measurement<GasConsumptionUnit> per100).Should().BeTrue();
            per100.Value.Should().Be(8);
            per100.Unit.Should().Be(GasConsumptionUnit.LiterPer100Km);

            Measurement<GasConsumptionUnit>.TryParse(gInvariant, "8l/km", out Measurement<GasConsumptionUnit> perKm).Should().BeTrue();
            perKm.Value.Should().Be(8);
            perKm.Unit.Should().Be(GasConsumptionUnit.LiterPerKm);
        }

        /// <summary>
        /// The alternative name of a unit is matched the same way as the primary one.
        /// </summary>
        [Fact]
        public void AlternativeNamesAreParsed()
        {
            Measurement<AngularUnit>.TryParse(gInvariant, "50percent", out Measurement<AngularUnit> spelled).Should().BeTrue();
            spelled.Unit.Should().Be(AngularUnit.Percent);

            Measurement<AngularUnit>.TryParse(gInvariant, "50%", out Measurement<AngularUnit> sign).Should().BeTrue();
            sign.Unit.Should().Be(AngularUnit.Percent);
        }

        /// <summary>
        /// A non-invariant culture still drives the value, and the exponent works there too.
        /// </summary>
        [Fact]
        public void ExponentNotationHonoursTheCulture()
        {
            var de = new CultureInfo("de-DE");

            Measurement<TestUnit>.TryParse(de, "1,5e2u2", out Measurement<TestUnit> value).Should().BeTrue();
            value.Value.Should().Be(150);
        }
    }
}
