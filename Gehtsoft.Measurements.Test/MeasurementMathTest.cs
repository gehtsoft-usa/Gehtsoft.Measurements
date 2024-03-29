﻿using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Gehtsoft.Measurements.Test
{
    public class MeasurementMathTest
    {
        [Theory]
        [InlineData(45, AngularUnit.Degree, 0.70710678118654)]
        [InlineData(30, AngularUnit.Degree, 0.5)]
        public void Sin(double value, AngularUnit unit, double expected)
        {
            var v = new Measurement<AngularUnit>(value, unit);
            MeasurementMath.Sin(v).Should().BeApproximately(expected, 1e-10);
            v.Sin().Should().BeApproximately(expected, 1e-10);
            MeasurementMath.Asin(expected).In(unit).Should().BeApproximately(value, 1e-7);
        }

        [Theory]
        [InlineData(45, AngularUnit.Degree, 0.70710678118654)]
        [InlineData(30, AngularUnit.Degree, 0.86602540378443)]
        [InlineData(60, AngularUnit.Degree, 0.5)]
        public void Cos(double value, AngularUnit unit, double expected)
        {
            var v = new Measurement<AngularUnit>(value, unit);
            MeasurementMath.Cos(v).Should().BeApproximately(expected, 1e-10);
            MeasurementMath.Acos(expected).In(unit).Should().BeApproximately(value, 1e-7);
        }

        [Theory]
        [InlineData(45, AngularUnit.Degree, 1)]
        [InlineData(30, AngularUnit.Degree, 0.57735026918962)]
        public void Tan(double value, AngularUnit unit, double expected)
        {
            var v = new Measurement<AngularUnit>(value, unit);
            MeasurementMath.Tan(v).Should().BeApproximately(expected, 1e-10);
            MeasurementMath.Atan(expected).In(unit).Should().BeApproximately(value, 1e-7);
        }

        [Theory]
        [InlineData(4, AngularUnit.Degree, 2)]
        [InlineData(2, AngularUnit.Degree, 1.41421356237309)]
        public void Sqrt(double value, AngularUnit unit, double expected)
        {
            var v = new Measurement<AngularUnit>(value, unit);
            var v1 = MeasurementMath.Sqrt(v);
            v1.Value.Should().BeApproximately(expected, 1e-10);
            v1.Unit.Should().Be(unit);
        }

        [Theory]
        [InlineData(45, AngularUnit.Degree, 45)]
        [InlineData(-30, AngularUnit.Degree, 30)]
        public void Abs(double value, AngularUnit unit, double expected)
        {
            var v = new Measurement<AngularUnit>(value, unit);
            var v1 = MeasurementMath.Abs(v);
            v1.Value.Should().BeApproximately(expected, 1e-10);
            v1.Unit.Should().Be(unit);

            var v2 = v.Abs();
            v2.Value.Should().BeApproximately(expected, 1e-10);
            v2.Unit.Should().Be(unit);
        }

        [Theory]
        [InlineData(4, AngularUnit.Degree, 2, 16)]
        [InlineData(1.41421356237309, AngularUnit.Degree, 2, 2)]
        [InlineData(2, AngularUnit.Degree, 0.5, 1.41421356237309)]
        public void Pow(double value, AngularUnit unit, double exp, double expected)
        {
            var v = new Measurement<AngularUnit>(value, unit);
            var v1 = MeasurementMath.Pow(v, exp);
            v1.Value.Should().BeApproximately(expected, 1e-10);
            v1.Unit.Should().Be(unit);
        }

        [Theory]
        [InlineData(2, DistanceUnit.Meter, 3, DistanceUnit.Meter, 6, AreaUnit.SquareMeter)]
        [InlineData(2, DistanceUnit.Foot, 3, DistanceUnit.Foot, 0.557418, AreaUnit.SquareMeter)]

        public void RectangleArea(double a, DistanceUnit ua, double b, DistanceUnit ub, double e, AreaUnit eu)
        {
            var v = MeasurementMath.RectangleArea(new Measurement<DistanceUnit>(a, ua), new Measurement<DistanceUnit>(b, ub));
            v.In(eu).Should().BeApproximately(e, 1e-6);
        }

        [Theory]
        [InlineData(2, DistanceUnit.Meter, 3, DistanceUnit.Meter, 4, DistanceUnit.Meter, 24, VolumeUnit.CubicMeter)]
        [InlineData(2, DistanceUnit.Foot, 3, DistanceUnit.Foot, 4, DistanceUnit.Foot, 0.679604, VolumeUnit.CubicMeter)]

        public void RecangularPrismVolume(double a, DistanceUnit ua, double b, DistanceUnit ub, double c, DistanceUnit uc, double e, VolumeUnit eu)
        {
            var x = MeasurementMath.RectangleArea(new Measurement<DistanceUnit>(a, ua), new Measurement<DistanceUnit>(b, ub));
            var v = MeasurementMath.RecangularPrismVolume(x, new Measurement<DistanceUnit>(c, uc));
            v.In(eu).Should().BeApproximately(e, 1e-6);
        }

        [Theory]
        [InlineData(4, WeightUnit.Kilogram, 3, VelocityUnit.MetersPerSecond, 18, EnergyUnit.Joule)]
        [InlineData(55, WeightUnit.Grain, 2300, VelocityUnit.FeetPerSecond, 645.9287446, EnergyUnit.FootPound)]
        public void KineticEnergy(double m, WeightUnit wu, double v, VelocityUnit vu, double e, EnergyUnit eu)
        {
            var r = MeasurementMath.KineticEnergy(new Measurement<WeightUnit>(m, wu), new Measurement<VelocityUnit>(v, vu));
            r.In(eu).Should().BeApproximately(e, 1e-6);
        }

        [Theory]
        [InlineData(25, DistanceUnit.Meter, 5, VelocityUnit.MetersPerSecond, 5)]
        [InlineData(25, DistanceUnit.Foot, 12.5, VelocityUnit.FeetPerSecond, 2)]
        [InlineData(10, DistanceUnit.Kilometer, 5, VelocityUnit.KilometersPerHour, 7200)]
        [InlineData(10, DistanceUnit.NauticalMile, 10, VelocityUnit.Knot, 3600)]

        public void Velocity(double distance, DistanceUnit distanceUnit, double velocity, VelocityUnit velocityUnit, double time)
        {
            var distance1 = new Measurement<DistanceUnit>(distance, distanceUnit);
            var velocity1 = new Measurement<VelocityUnit>(velocity, velocityUnit);
            TimeSpan ts1 = TimeSpan.FromSeconds(time);

            TimeSpan ts2 = MeasurementMath.TravelTime(distance1, velocity1);
            ts2.TotalSeconds.Should().BeApproximately(time, 1e-5);

            var distance2 = MeasurementMath.DistanceTraveled(velocity1, ts1);
            distance2.In(distanceUnit).Should().BeApproximately(distance, 1e-5);

            var velocity2 = MeasurementMath.Velocity(distance1, ts1);
            velocity2.In(velocityUnit).Should().BeApproximately(velocity, 1e-5);
        }

        [Fact]
        public void Acceleration()
        {
            var velocity3 = MeasurementMath.Velocity(new Measurement<AccelerationUnit>(10, AccelerationUnit.MeterPerSecondSquare), TimeSpan.FromSeconds(5));
            velocity3.In(VelocityUnit.MetersPerSecond).Should().BeApproximately(50, 1e-5);

            var distance3 = MeasurementMath.DistanceTraveled(new Measurement<AccelerationUnit>(10, AccelerationUnit.MeterPerSecondSquare), TimeSpan.FromSeconds(5));
            distance3.In(DistanceUnit.Meter).Should().BeApproximately(125, 1e-5);        
        }

        [Fact]
        public void Pressure()
        {
            var pressure = MeasurementMath.Pressure(WeightUnit.Pound.New(10), AreaUnit.SquareInch.New(2));
            pressure.In(PressureUnit.PoundsPerSquareInch).Should().BeApproximately(5, 1e-5);
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
            (DistanceUnit.Centimeter.New(5) + DistanceUnit.Millimeter.New(5)).Should().Be(DistanceUnit.Centimeter.New(5.5));
            (DistanceUnit.Centimeter.New(5) - DistanceUnit.Millimeter.New(5)).Should().Be(DistanceUnit.Centimeter.New(4.5));
            (DistanceUnit.Centimeter.New(5) * 2).Should().Be(DistanceUnit.Centimeter.New(10));
            (2 * DistanceUnit.Centimeter.New(5)).Should().Be(DistanceUnit.Centimeter.New(10));
            (DistanceUnit.Centimeter.New(5) / 2).Should().Be(DistanceUnit.Centimeter.New(2.5));
            (DistanceUnit.Centimeter.New(5) / DistanceUnit.Centimeter.New(2.5)).Should().Be(2.0);

            (+DistanceUnit.Centimeter.New(5)).Should().Be(DistanceUnit.Centimeter.New(5));
            (-DistanceUnit.Centimeter.New(5)).Should().Be(DistanceUnit.Centimeter.New(-5));

            (WeightUnit.UKTonne.New(1) / WeightUnit.USTonne.New(1)).Should().BeApproximately(1.1201764057331863285556780595369, 1e-10);
        }

        [Fact]
        public void Sign()
        {
            AngularUnit.MOA.New(5).Sign().Should().BeGreaterThan(0);
            AngularUnit.MOA.New(0).Sign().Should().Be(0);
            AngularUnit.MOA.New(-5).Sign().Should().BeLessThan(0);
        }

        [Fact]
        public void NewTest()
        {
            var a = AngularUnit.Degree.New(5);
            a.In(AngularUnit.Degree).Should().BeApproximately(5, 1e-10);
            a.Unit.Should().Be(AngularUnit.Degree);
        }

        [Fact]
        public void AsTest()
        {
            var a = (5.0).As(AngularUnit.Degree);
            a.In(AngularUnit.Degree).Should().BeApproximately(5, 1e-10);
            a.Unit.Should().Be(AngularUnit.Degree);
        }

        [Fact]
        public void AsTest1()
        {
            var a = (new double[] { 1, 2, 3 }).As(AngularUnit.Degree).ToArray();
            a.Should().HaveCount(3);
            a[0].In(AngularUnit.Degree).Should().BeApproximately(1, 1e-10);
            a[1].In(AngularUnit.Degree).Should().BeApproximately(2, 1e-10);
            a[2].In(AngularUnit.Degree).Should().BeApproximately(3, 1e-10);
        }
    }
}
