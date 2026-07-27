using System;
using AwesomeAssertions;
using Xunit;

namespace Gehtsoft.Measurements.Test
{
    /// <summary>
    /// The helpers which combine measurements of different kinds into a measurement of a third kind.
    /// </summary>
    /// <remarks>
    /// Every expected value below is worked out from the definition of the quantity rather than
    /// taken from the implementation, so a wrong base unit or a swapped factor is caught.
    /// </remarks>
    public class CrossDimensionMathTest
    {
        /// <summary>
        /// 100 N·m at 3000 rpm. 3000 rpm is 3000 times 2π over 60, which is 314.159... rad/s,
        /// so the power is 31,415.9... W.
        /// </summary>
        [Fact]
        public void PowerFromTorqueAndRotationalSpeed()
        {
            var torque = new Measurement<TorqueUnit>(100, TorqueUnit.NewtonMeter);
            var speed = new Measurement<RotationalSpeedUnit>(3000, RotationalSpeedUnit.RevolutionsPerMinute);

            var power = MeasurementMath.Power(torque, speed);

            power.Unit.Should().Be(PowerUnit.Watt);
            power.Value.Should().BeApproximately(100 * 2 * Math.PI * 3000 / 60, 1e-6);
            power.In(PowerUnit.Kilowatt).Should().BeApproximately(31.4159265, 1e-6);
        }

        /// <summary>
        /// A torque in foot-pounds and a speed in Hz must give the same answer as their SI equivalents.
        /// </summary>
        [Fact]
        public void PowerConvertsTheInputUnits()
        {
            var torque = new Measurement<TorqueUnit>(1, TorqueUnit.FootPoundForce);
            var speed = new Measurement<RotationalSpeedUnit>(1, RotationalSpeedUnit.Hertz);

            var power = MeasurementMath.Power(torque, speed);

            // 1 ft·lbf is 1.3558179483314 N·m, 1 Hz is 2π rad/s
            power.Value.Should().BeApproximately(1.3558179483314 * 2 * Math.PI, 1e-9);
        }

        /// <summary>
        /// One kilogram accelerated by one metre per second squared needs one newton.
        /// </summary>
        [Fact]
        public void ForceFromMassAndAcceleration()
        {
            var mass = new Measurement<WeightUnit>(1, WeightUnit.Kilogram);
            var acceleration = new Measurement<AccelerationUnit>(1, AccelerationUnit.MeterPerSecondSquare);

            var force = MeasurementMath.Force(mass, acceleration);

            force.Unit.Should().Be(ForceUnit.Newton);
            force.Value.Should().BeApproximately(1, 1e-12);
        }

        /// <summary>
        /// A mass under standard gravity weighs its mass times g0.
        /// </summary>
        [Fact]
        public void ForceUnderStandardGravity()
        {
            var mass = new Measurement<WeightUnit>(10, WeightUnit.Kilogram);
            var gravity = new Measurement<AccelerationUnit>(1, AccelerationUnit.EarthGravity);

            MeasurementMath.Force(mass, gravity).Value.Should().BeApproximately(98.0665, 1e-9);
        }

        /// <summary>
        /// One kilogram in one litre is one thousand kilograms per cubic metre.
        /// </summary>
        [Fact]
        public void DensityFromMassAndVolume()
        {
            var mass = new Measurement<WeightUnit>(1, WeightUnit.Kilogram);
            var volume = new Measurement<VolumeUnit>(1, VolumeUnit.Liter);

            var density = MeasurementMath.Density(mass, volume);

            density.Unit.Should().Be(DensityUnit.KilogramPerCubicMeter);
            density.Value.Should().BeApproximately(1000, 1e-9);
            density.In(DensityUnit.KilogramPerLiter).Should().BeApproximately(1, 1e-9);
        }

        /// <summary>
        /// Mass and density are the two ends of the same relation.
        /// </summary>
        [Fact]
        public void WeightFromDensityAndVolumeIsTheInverseOfDensity()
        {
            var density = new Measurement<DensityUnit>(1000, DensityUnit.KilogramPerCubicMeter);
            var volume = new Measurement<VolumeUnit>(2, VolumeUnit.Liter);

            var weight = MeasurementMath.Weight(density, volume);

            weight.Unit.Should().Be(WeightUnit.Kilogram);
            weight.Value.Should().BeApproximately(2, 1e-9);

            MeasurementMath.Density(weight, volume).Value.Should().BeApproximately(density.Value, 1e-9);
        }

        /// <summary>
        /// Reaching 100 metres per second in 10 seconds is an acceleration of 10 metres per second squared.
        /// </summary>
        [Fact]
        public void AccelerationFromVelocityAndTime()
        {
            var velocity = new Measurement<VelocityUnit>(100, VelocityUnit.MetersPerSecond);

            var acceleration = MeasurementMath.Acceleration(velocity, TimeSpan.FromSeconds(10));

            acceleration.Unit.Should().Be(AccelerationUnit.MeterPerSecondSquare);
            acceleration.Value.Should().BeApproximately(10, 1e-9);
        }

        /// <summary>
        /// Acceleration and velocity are the two ends of the same relation.
        /// </summary>
        [Fact]
        public void AccelerationIsTheInverseOfVelocity()
        {
            var acceleration = new Measurement<AccelerationUnit>(9.80665, AccelerationUnit.MeterPerSecondSquare);
            TimeSpan time = TimeSpan.FromSeconds(3);

            var velocity = MeasurementMath.Velocity(acceleration, time);

            MeasurementMath.Acceleration(velocity, time).In(AccelerationUnit.MeterPerSecondSquare)
                           .Should().BeApproximately(9.80665, 1e-9);
        }
    }
}
