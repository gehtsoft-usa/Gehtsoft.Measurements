using FluentAssertions;
using System;
using System.Linq;
using Xunit;

namespace Gehtsoft.Measurements.Test
{
    public class DecimalMeasurementMathTest
    {
        [Theory]
        [InlineData(45, AngularUnit.Degree, 0.70710678118654)]
        [InlineData(30, AngularUnit.Degree, 0.5)]
        public void Sin(decimal value, AngularUnit unit, double expected)
        {
            var v = new DecimalMeasurement<AngularUnit>(value, unit);
            MeasurementMath.Sin(v).Should().BeApproximately(expected, 1e-10);
            v.Sin().Should().BeApproximately(expected, 1e-10);
        }

        [Theory]
        [InlineData(45, AngularUnit.Degree, 0.70710678118654)]
        [InlineData(30, AngularUnit.Degree, 0.86602540378443)]
        [InlineData(60, AngularUnit.Degree, 0.5)]
        public void Cos(decimal value, AngularUnit unit, double expected)
        {
            var v = new DecimalMeasurement<AngularUnit>(value, unit);
            MeasurementMath.Cos(v).Should().BeApproximately(expected, 1e-10);
        }

        [Theory]
        [InlineData(45, AngularUnit.Degree, 1)]
        [InlineData(30, AngularUnit.Degree, 0.57735026918962)]
        public void Tan(decimal value, AngularUnit unit, double expected)
        {
            var v = new DecimalMeasurement<AngularUnit>(value, unit);
            MeasurementMath.Tan(v).Should().BeApproximately(expected, 1e-10);
        }

        [Theory]
        [InlineData(4, AngularUnit.Degree, 2)]
        [InlineData(2, AngularUnit.Degree, 1.41421356237309)]
        public void Sqrt(decimal value, AngularUnit unit, decimal expected)
        {
            var v = new DecimalMeasurement<AngularUnit>(value, unit);
            var v1 = MeasurementMath.Sqrt(v);
            v1.Value.Should().BeApproximately(expected, 1e-10m);
            v1.Unit.Should().Be(unit);
        }

        [Theory]
        [InlineData(45, AngularUnit.Degree, 45)]
        [InlineData(-30, AngularUnit.Degree, 30)]
        public void Abs(decimal value, AngularUnit unit, decimal expected)
        {
            var v = new DecimalMeasurement<AngularUnit>(value, unit);
            var v1 = MeasurementMath.Abs(v);
            v1.Value.Should().BeApproximately(expected, 1e-10m);
            v1.Unit.Should().Be(unit);

            var v2 = v.Abs();
            v2.Value.Should().BeApproximately(expected, 1e-10m);
            v2.Unit.Should().Be(unit);
        }

        [Theory]
        [InlineData(4, AngularUnit.Degree, 2, 16)]
        [InlineData(1.41421356237309, AngularUnit.Degree, 2, 2)]
        [InlineData(2, AngularUnit.Degree, 0.5, 1.41421356237309)]
        public void Pow(decimal value, AngularUnit unit, decimal exp, decimal expected)
        {
            var v = new DecimalMeasurement<AngularUnit>(value, unit);
            var v1 = MeasurementMath.Pow(v, exp);
            v1.Value.Should().BeApproximately(expected, 1e-10m);
            v1.Unit.Should().Be(unit);
        }

        [Fact]
        public void CompareStatements()
        {
            (DistanceUnit.Centimeter.New(10) == DistanceUnit.Meter.New(0.1)).Should().BeTrue();
            (DistanceUnit.Centimeter.New(10) != DistanceUnit.Meter.New(0.1)).Should().BeFalse();
            (DistanceUnit.Centimeter.New(10) > DistanceUnit.Meter.New(0.1)).Should().BeFalse();
            (DistanceUnit.Centimeter.New(10) >= DistanceUnit.Meter.New(0.1)).Should().BeTrue();
            (DistanceUnit.Centimeter.New(10) < DistanceUnit.Meter.New(0.1)).Should().BeFalse();
            (DistanceUnit.Centimeter.New(10) <= DistanceUnit.Meter.New(0.1)).Should().BeTrue();

            (DistanceUnit.Centimeter.New(20) == DistanceUnit.Meter.New(0.1)).Should().BeFalse();
            (DistanceUnit.Centimeter.New(20) != DistanceUnit.Meter.New(0.1)).Should().BeTrue();
            (DistanceUnit.Centimeter.New(20) > DistanceUnit.Meter.New(0.1)).Should().BeTrue();
            (DistanceUnit.Centimeter.New(20) >= DistanceUnit.Meter.New(0.1)).Should().BeTrue();
            (DistanceUnit.Centimeter.New(20) < DistanceUnit.Meter.New(0.1)).Should().BeFalse();
            (DistanceUnit.Centimeter.New(20) <= DistanceUnit.Meter.New(0.1)).Should().BeFalse();

            (DistanceUnit.Centimeter.New(5) == DistanceUnit.Meter.New(0.1)).Should().BeFalse();
            (DistanceUnit.Centimeter.New(5) != DistanceUnit.Meter.New(0.1)).Should().BeTrue();
            (DistanceUnit.Centimeter.New(5) > DistanceUnit.Meter.New(0.1)).Should().BeFalse();
            (DistanceUnit.Centimeter.New(5) >= DistanceUnit.Meter.New(0.1)).Should().BeFalse();
            (DistanceUnit.Centimeter.New(5) < DistanceUnit.Meter.New(0.1)).Should().BeTrue();
            (DistanceUnit.Centimeter.New(5) <= DistanceUnit.Meter.New(0.1)).Should().BeTrue();
        }

        [Fact]
        public void CompareVerySmall()
        {
            1e-200.As(DistanceUnit.Centimeter).CompareTo(1e-201.As(DistanceUnit.Centimeter)).Should().Be(1);
            1e-201.As(DistanceUnit.Centimeter).CompareTo(1e-200.As(DistanceUnit.Centimeter)).Should().Be(-1);
            1e-200.As(DistanceUnit.Centimeter).CompareTo(1e-202.As(DistanceUnit.Meter)).Should().Be(0);
        }

        [Fact]
        public void MathStatements()
        {
            (DistanceUnit.Centimeter.NewDecimal(5m) + DistanceUnit.Millimeter.NewDecimal(5m)).Should().Be(DistanceUnit.Centimeter.NewDecimal(5.5m));
            (DistanceUnit.Centimeter.NewDecimal(5m) - DistanceUnit.Millimeter.NewDecimal(5m)).Should().Be(DistanceUnit.Centimeter.NewDecimal(4.5m));
            (DistanceUnit.Centimeter.NewDecimal(5m) * 2m).Should().Be(DistanceUnit.Centimeter.NewDecimal(10m));
            (2 * DistanceUnit.Centimeter.NewDecimal(5)).Should().Be(DistanceUnit.Centimeter.NewDecimal(10m));
            (DistanceUnit.Centimeter.NewDecimal(5m) / 2m).Should().Be(DistanceUnit.Centimeter.NewDecimal(2.5m));
            (DistanceUnit.Centimeter.NewDecimal(5m) / DistanceUnit.Centimeter.NewDecimal(2.5m)).Should().Be(2.0m);

            (+DistanceUnit.Centimeter.NewDecimal(5m)).Should().Be(DistanceUnit.Centimeter.NewDecimal(5m));
            (-DistanceUnit.Centimeter.NewDecimal(5m)).Should().Be(DistanceUnit.Centimeter.NewDecimal(-5m));

            (WeightUnit.UKTonne.NewDecimal(1m) / WeightUnit.USTonne.NewDecimal(1m)).Should().BeApproximately(1.1201764057331863285556780595369m, 1e-10m);
        }

        [Fact]
        public void Sign()
        {
            AngularUnit.MOA.NewDecimal(5m).Sign().Should().BeGreaterThan(0);
            AngularUnit.MOA.NewDecimal(0m).Sign().Should().Be(0);
            AngularUnit.MOA.NewDecimal(-5m).Sign().Should().BeLessThan(0);
        }

        [Fact]
        public void NewTest()
        {
            var a = AngularUnit.Degree.NewDecimal(5m);
            a.In(AngularUnit.Degree).Should().BeApproximately(5m, 1e-10m);
            a.Unit.Should().Be(AngularUnit.Degree);
        }

        [Fact]
        public void AsTest()
        {
            var a = (5.0m).AsDecimal(AngularUnit.Degree);
            a.In(AngularUnit.Degree).Should().BeApproximately(5m, 1e-10m);
            a.Unit.Should().Be(AngularUnit.Degree);
        }
    }
}
