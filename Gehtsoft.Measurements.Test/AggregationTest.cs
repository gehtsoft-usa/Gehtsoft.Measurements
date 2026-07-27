using System;
using System.Collections.Generic;
using System.Linq;
using AwesomeAssertions;
using Xunit;

namespace Gehtsoft.Measurements.Test
{
    /// <summary>
    /// The sequence aggregations and the value helpers which limit or round a measurement.
    /// </summary>
    public class AggregationTest
    {
        private static readonly Measurement<DistanceUnit>[] gMixed = new[]
        {
            new Measurement<DistanceUnit>(1, DistanceUnit.Foot),      // 12 in
            new Measurement<DistanceUnit>(6, DistanceUnit.Inch),      //  6 in
            new Measurement<DistanceUnit>(1, DistanceUnit.Yard),      // 36 in
        };

        /// <summary>
        /// The total is accumulated in the unit of the first element.
        /// </summary>
        [Fact]
        public void SumConvertsIntoTheUnitOfTheFirstElement()
        {
            var sum = gMixed.Sum();

            sum.Unit.Should().Be(DistanceUnit.Foot);
            sum.In(DistanceUnit.Inch).Should().BeApproximately(54, 1e-9);
        }

        [Fact]
        public void SumOfAnEmptySequenceIsZeroOfTheBaseUnit()
        {
            var sum = Array.Empty<Measurement<DistanceUnit>>().Sum();

            sum.Value.Should().Be(0);
            sum.Unit.Should().Be(Measurement<DistanceUnit>.BaseUnit);
        }

        [Fact]
        public void AverageConvertsIntoTheUnitOfTheFirstElement()
        {
            var average = gMixed.Average();

            average.Unit.Should().Be(DistanceUnit.Foot);
            average.In(DistanceUnit.Inch).Should().BeApproximately(18, 1e-9);
        }

        [Fact]
        public void AverageOfAnEmptySequenceThrows()
        {
            Action average = () => Array.Empty<Measurement<DistanceUnit>>().Average();
            average.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void SumAndAverageWorkForDecimalMeasurements()
        {
            var values = new[]
            {
                new DecimalMeasurement<DistanceUnit>(1, DistanceUnit.Foot),
                new DecimalMeasurement<DistanceUnit>(6, DistanceUnit.Inch),
            };

            values.Sum().In(DistanceUnit.Inch).Should().Be(18m);
            values.Average().In(DistanceUnit.Inch).Should().Be(9m);

            Array.Empty<DecimalMeasurement<DistanceUnit>>().Sum().Value.Should().Be(0m);

            Action average = () => Array.Empty<DecimalMeasurement<DistanceUnit>>().Average();
            average.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void NullSequenceIsRejected()
        {
            Action sum = () => ((IEnumerable<Measurement<DistanceUnit>>)null).Sum();
            sum.Should().Throw<ArgumentNullException>();

            Action average = () => ((IEnumerable<Measurement<DistanceUnit>>)null).Average();
            average.Should().Throw<ArgumentNullException>();
        }

        /// <summary>
        /// The standard operators already compare measurements across units, so the library adds none.
        /// </summary>
        /// <remarks>
        /// This is a guard as much as a demonstration - defining `Min` and `Max` here as well
        /// would make an unqualified call ambiguous for anyone using both namespaces.
        /// </remarks>
        [Fact]
        public void StandardMinAndMaxAlreadyWorkAcrossUnits()
        {
            gMixed.Min().Should().Be(new Measurement<DistanceUnit>(6, DistanceUnit.Inch));
            gMixed.Max().Should().Be(new Measurement<DistanceUnit>(1, DistanceUnit.Yard));
            gMixed.OrderBy(v => v).First().Should().Be(new Measurement<DistanceUnit>(6, DistanceUnit.Inch));
        }

        [Fact]
        public void MinAndMaxOfTwoMeasurementsComparePhysicalValues()
        {
            var foot = new Measurement<DistanceUnit>(1, DistanceUnit.Foot);
            var inch = new Measurement<DistanceUnit>(6, DistanceUnit.Inch);

            MeasurementMath.Min(foot, inch).Should().Be(inch);
            MeasurementMath.Max(foot, inch).Should().Be(foot);

            var decimalFoot = new DecimalMeasurement<DistanceUnit>(1, DistanceUnit.Foot);
            var decimalInch = new DecimalMeasurement<DistanceUnit>(6, DistanceUnit.Inch);

            MeasurementMath.Min(decimalFoot, decimalInch).Should().Be(decimalInch);
            MeasurementMath.Max(decimalFoot, decimalInch).Should().Be(decimalFoot);
        }

        [Fact]
        public void ClampLimitsToTheRange()
        {
            var low = new Measurement<DistanceUnit>(1, DistanceUnit.Inch);
            var high = new Measurement<DistanceUnit>(1, DistanceUnit.Foot);

            MeasurementMath.Clamp(new Measurement<DistanceUnit>(0.5, DistanceUnit.Inch), low, high).Should().Be(low);
            MeasurementMath.Clamp(new Measurement<DistanceUnit>(1, DistanceUnit.Yard), low, high).Should().Be(high);

            var inside = new Measurement<DistanceUnit>(6, DistanceUnit.Inch);
            MeasurementMath.Clamp(inside, low, high).Should().Be(inside);
        }

        [Fact]
        public void ClampRejectsAnInvertedRange()
        {
            var low = new Measurement<DistanceUnit>(1, DistanceUnit.Inch);
            var high = new Measurement<DistanceUnit>(1, DistanceUnit.Foot);

            Action inverted = () => MeasurementMath.Clamp(low, high, low);
            inverted.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void RoundKeepsTheUnitAndRoundsTheValueAsExpressed()
        {
            var value = new Measurement<DistanceUnit>(1.23456, DistanceUnit.Foot);

            value.Round(2).Should().Be(new Measurement<DistanceUnit>(1.23, DistanceUnit.Foot));
            value.Round(2).Unit.Should().Be(DistanceUnit.Foot);

            // the default accuracy of a foot is 2 decimal places
            Measurement<DistanceUnit>.GetUnitDefaultAccuracy(DistanceUnit.Foot).Should().Be(2);
            value.Round().Should().Be(value.Round(2));

            new DecimalMeasurement<DistanceUnit>(1.23456m, DistanceUnit.Foot).Round(2).Value.Should().Be(1.23m);
            new DecimalMeasurement<DistanceUnit>(1.23456m, DistanceUnit.Foot).Round().Value.Should().Be(1.23m);
        }
    }
}
