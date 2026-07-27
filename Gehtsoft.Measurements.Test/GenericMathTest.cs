using System.Numerics;
using AwesomeAssertions;
using Xunit;

namespace Gehtsoft.Measurements.Test
{
    /// <summary>
    /// The generic math operator interfaces implemented by both measurement types.
    /// </summary>
    /// <remarks>
    /// The helper methods below are constrained to the `System.Numerics` operator interfaces, so
    /// they compile at all only if the measurement types really do satisfy those constraints.
    /// That is what these tests check - the operators themselves are covered elsewhere.
    /// </remarks>
    public class GenericMathTest
    {
        private static TValue SumOf<TValue>(TValue first, TValue second)
            where TValue : IAdditionOperators<TValue, TValue, TValue> => first + second;

        private static TValue DifferenceOf<TValue>(TValue first, TValue second)
            where TValue : ISubtractionOperators<TValue, TValue, TValue> => first - second;

        private static TValue Negated<TValue>(TValue value)
            where TValue : IUnaryNegationOperators<TValue, TValue> => -value;

        private static TValue Unchanged<TValue>(TValue value)
            where TValue : IUnaryPlusOperators<TValue, TValue> => +value;

        private static TValue LargerOf<TValue>(TValue first, TValue second)
            where TValue : IComparisonOperators<TValue, TValue, bool> => first > second ? first : second;

        private static bool SameAs<TValue>(TValue first, TValue second)
            where TValue : IEqualityOperators<TValue, TValue, bool> => first == second;

        private static TValue Scaled<TValue, TScalar>(TValue value, TScalar factor)
            where TValue : IMultiplyOperators<TValue, TScalar, TValue> => value * factor;

        private static TValue Divided<TValue, TScalar>(TValue value, TScalar divisor)
            where TValue : IDivisionOperators<TValue, TScalar, TValue> => value / divisor;

        private static TRatio RatioOf<TValue, TRatio>(TValue first, TValue second)
            where TValue : IDivisionOperators<TValue, TValue, TRatio> => first / second;

        [Fact]
        public void MeasurementSatisfiesTheOperatorInterfaces()
        {
            var foot = new Measurement<DistanceUnit>(1, DistanceUnit.Foot);
            var inch = new Measurement<DistanceUnit>(6, DistanceUnit.Inch);

            SumOf(foot, inch).Should().Be(new Measurement<DistanceUnit>(1.5, DistanceUnit.Foot));
            DifferenceOf(foot, inch).Should().Be(new Measurement<DistanceUnit>(0.5, DistanceUnit.Foot));
            Negated(foot).Should().Be(new Measurement<DistanceUnit>(-1, DistanceUnit.Foot));
            Unchanged(foot).Should().Be(foot);
            LargerOf(foot, inch).Should().Be(foot);
            SameAs(foot, new Measurement<DistanceUnit>(12, DistanceUnit.Inch)).Should().BeTrue();
            Scaled(foot, 3.0).Should().Be(new Measurement<DistanceUnit>(3, DistanceUnit.Foot));
            Divided(foot, 2.0).Should().Be(new Measurement<DistanceUnit>(0.5, DistanceUnit.Foot));
            RatioOf<Measurement<DistanceUnit>, double>(foot, inch).Should().Be(2.0);
        }

        [Fact]
        public void DecimalMeasurementSatisfiesTheOperatorInterfaces()
        {
            var foot = new DecimalMeasurement<DistanceUnit>(1, DistanceUnit.Foot);
            var inch = new DecimalMeasurement<DistanceUnit>(6, DistanceUnit.Inch);

            SumOf(foot, inch).Should().Be(new DecimalMeasurement<DistanceUnit>(1.5m, DistanceUnit.Foot));
            DifferenceOf(foot, inch).Should().Be(new DecimalMeasurement<DistanceUnit>(0.5m, DistanceUnit.Foot));
            Negated(foot).Should().Be(new DecimalMeasurement<DistanceUnit>(-1, DistanceUnit.Foot));
            Unchanged(foot).Should().Be(foot);
            LargerOf(foot, inch).Should().Be(foot);
            SameAs(foot, new DecimalMeasurement<DistanceUnit>(12, DistanceUnit.Inch)).Should().BeTrue();
            Scaled(foot, 3.0m).Should().Be(new DecimalMeasurement<DistanceUnit>(3, DistanceUnit.Foot));
            Divided(foot, 2.0m).Should().Be(new DecimalMeasurement<DistanceUnit>(0.5m, DistanceUnit.Foot));
            RatioOf<DecimalMeasurement<DistanceUnit>, decimal>(foot, inch).Should().Be(2.0m);
        }
    }
}
