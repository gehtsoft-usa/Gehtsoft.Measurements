using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Gehtsoft.Measurements
{
    /// <summary>
    /// Math operations for measurements
    /// </summary>
    public static class MeasurementMath
    {
        /// <summary>
        /// <para>Returns sign of the value</para>
        /// <para>The method return `-1` for negative values, `0` for zero value and `1` for positive values</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int Sign<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] T>(this Measurement<T> value) where T : Enum
        {
            if (value.Value < 0)
                return -1;
            else if (value.Value == 0)
                return 0;
            return 1;
        }

        /// <summary>
        /// <para>Returns sign of the value</para>
        /// <para>The method return `-1` for negative values, `0` for zero value and `1` for positive values</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int Sign<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] T>(this DecimalMeasurement<T> value) where T : Enum
        {
            if (value.Value < 0)
                return -1;
            else if (value.Value == 0)
                return 0;
            return 1;
        }

        /// <summary>
        /// Calculate sine of angular value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Sin(this Measurement<AngularUnit> value) => Math.Sin(value.In(AngularUnit.Radian));

        /// <summary>
        /// Calculate sine of angular value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Sin(this DecimalMeasurement<AngularUnit> value) => Math.Sin((double)value.In(AngularUnit.Radian));

        /// <summary>
        /// Calculate cosine of angular value
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Cos(this Measurement<AngularUnit> value) => Math.Cos(value.In(AngularUnit.Radian));

        /// <summary>
        /// Calculate cosine of angular value
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Cos(this DecimalMeasurement<AngularUnit> value) => Math.Cos((double)value.In(AngularUnit.Radian));

        /// <summary>
        /// Calculate tangent of angular value
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Tan(this Measurement<AngularUnit> value) => Math.Tan(value.In(AngularUnit.Radian));

        /// <summary>
        /// Calculate tangent of angular value
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Tan(this DecimalMeasurement<AngularUnit> value) => Math.Tan((double)value.In(AngularUnit.Radian));

        /// <summary>
        /// Calculate arcsine as angular value
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Measurement<AngularUnit> Asin(double value) => new Measurement<AngularUnit>(Math.Asin(value), AngularUnit.Radian);


        /// <summary>
        /// Calculate arccosine as angular value
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Measurement<AngularUnit> Acos(double value) => new Measurement<AngularUnit>(Math.Acos(value), AngularUnit.Radian);

        /// <summary>
        /// Calculate arctangent as angular value
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Measurement<AngularUnit> Atan(double value) => new Measurement<AngularUnit>(Math.Atan(value), AngularUnit.Radian);

        /// <summary>
        /// Calculate square root of a value
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Measurement<T> Sqrt<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] T>(this Measurement<T> value) where T : Enum => new Measurement<T>(Math.Sqrt(value.Value), value.Unit);

        /// <summary>
        /// Calculate square root of a value
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DecimalMeasurement<T> Sqrt<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] T>(this DecimalMeasurement<T> value) where T : Enum => new DecimalMeasurement<T>((decimal)Math.Sqrt((double)value.Value), value.Unit);

        /// <summary>
        /// Raise the value in the power specified
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Measurement<T> Pow<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] T>(this Measurement<T> value, double exp) where T : Enum => new Measurement<T>(Math.Pow(value.Value, exp), value.Unit);

        /// <summary>
        /// Raise the value in the power specified
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DecimalMeasurement<T> Pow<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] T>(this DecimalMeasurement<T> value, decimal exp) where T : Enum => new DecimalMeasurement<T>((decimal)Math.Pow((double)value.Value, (double)exp), value.Unit);

        /// <summary>
        /// Calculate the absolute value
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Measurement<T> Abs<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] T>(this Measurement<T> value) where T : Enum => new Measurement<T>(Math.Abs(value.Value), value.Unit);


        /// <summary>
        /// Calculate the absolute value
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DecimalMeasurement<T> Abs<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] T>(this DecimalMeasurement<T> value) where T : Enum => new DecimalMeasurement<T>(Math.Abs(value.Value), value.Unit);

        /// <summary>
        /// Returns the smaller of two measurements
        /// </summary>
        /// <remarks>
        /// The measurements are compared by their physical value, so the result may well be the
        /// one with the larger number, expressed in a smaller unit.
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Measurement<T> Min<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] T>(Measurement<T> first, Measurement<T> second) where T : Enum => first.CompareTo(second) <= 0 ? first : second;

        /// <summary>
        /// Returns the smaller of two measurements
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DecimalMeasurement<T> Min<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] T>(DecimalMeasurement<T> first, DecimalMeasurement<T> second) where T : Enum => first.CompareTo(second) <= 0 ? first : second;

        /// <summary>
        /// Returns the larger of two measurements
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Measurement<T> Max<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] T>(Measurement<T> first, Measurement<T> second) where T : Enum => first.CompareTo(second) >= 0 ? first : second;

        /// <summary>
        /// Returns the larger of two measurements
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DecimalMeasurement<T> Max<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] T>(DecimalMeasurement<T> first, DecimalMeasurement<T> second) where T : Enum => first.CompareTo(second) >= 0 ? first : second;

        /// <summary>
        /// Limits a measurement to the specified range
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="min">The lowest value of the range</param>
        /// <param name="max">The highest value of the range</param>
        /// <returns></returns>
        public static Measurement<T> Clamp<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] T>(Measurement<T> value, Measurement<T> min, Measurement<T> max)
            where T : Enum
        {
            if (min.CompareTo(max) > 0)
                throw new ArgumentException("The lowest value of the range is greater than the highest one", nameof(min));
            if (value.CompareTo(min) < 0)
                return min;
            if (value.CompareTo(max) > 0)
                return max;
            return value;
        }

        /// <summary>
        /// Limits a measurement to the specified range
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="min">The lowest value of the range</param>
        /// <param name="max">The highest value of the range</param>
        /// <returns></returns>
        public static DecimalMeasurement<T> Clamp<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] T>(DecimalMeasurement<T> value, DecimalMeasurement<T> min, DecimalMeasurement<T> max)
            where T : Enum
        {
            if (min.CompareTo(max) > 0)
                throw new ArgumentException("The lowest value of the range is greater than the highest one", nameof(min));
            if (value.CompareTo(min) < 0)
                return min;
            if (value.CompareTo(max) > 0)
                return max;
            return value;
        }

        /// <summary>
        /// Rounds the value of the measurement to the specified number of decimal places
        /// </summary>
        /// <remarks>
        /// The unit is not changed, so the value is rounded as it is expressed, not in the base unit.
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="digits"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Measurement<T> Round<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] T>(this Measurement<T> value, int digits) where T : Enum => new Measurement<T>(Math.Round(value.Value, digits), value.Unit);

        /// <summary>
        /// Rounds the value of the measurement to the default accuracy of its unit
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Measurement<T> Round<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] T>(this Measurement<T> value) where T : Enum => value.Round(Measurement<T>.GetUnitDefaultAccuracy(value.Unit));

        /// <summary>
        /// Rounds the value of the measurement to the specified number of decimal places
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="digits"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DecimalMeasurement<T> Round<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] T>(this DecimalMeasurement<T> value, int digits) where T : Enum => new DecimalMeasurement<T>(Math.Round(value.Value, digits), value.Unit);

        /// <summary>
        /// Rounds the value of the measurement to the default accuracy of its unit
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DecimalMeasurement<T> Round<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] T>(this DecimalMeasurement<T> value) where T : Enum => value.Round(DecimalMeasurement<T>.GetUnitDefaultAccuracy(value.Unit));

        /// <summary>
        /// Calculate velocity from distance and time
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Measurement<VelocityUnit> Velocity(Measurement<DistanceUnit> distance, TimeSpan time) => new Measurement<VelocityUnit>(distance.In(DistanceUnit.Meter) / time.TotalSeconds, VelocityUnit.MetersPerSecond);

        /// <summary>
        /// Calculate velocity from acceleration and time
        /// </summary>
        /// <param name="acceleration"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Measurement<VelocityUnit> Velocity(Measurement<AccelerationUnit> acceleration, TimeSpan time) => new Measurement<VelocityUnit>(acceleration.In(AccelerationUnit.MeterPerSecondSquare) * time.TotalSeconds, VelocityUnit.MetersPerSecond);

        /// <summary>
        /// Calculate kinetic energy
        /// </summary>
        /// <param name="weight"></param>
        /// <param name="velocity"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Measurement<EnergyUnit> KineticEnergy(Measurement<WeightUnit> weight, Measurement<VelocityUnit> velocity) => new Measurement<EnergyUnit>(0.5 * weight.In(WeightUnit.Kilogram) * Math.Pow(velocity.In(VelocityUnit.MetersPerSecond), 2), EnergyUnit.Joule);

        /// <summary>
        /// Calculate area of a rectangle
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Measurement<AreaUnit> RectangleArea(Measurement<DistanceUnit> width, Measurement<DistanceUnit> height) => new Measurement<AreaUnit>(width.In(DistanceUnit.Meter) * height.In(DistanceUnit.Meter), AreaUnit.SquareMeter);

        /// <summary>
        /// Calculate the volume of a rectangular prism
        /// </summary>
        /// <param name="area"></param>
        /// <param name="depth"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Measurement<VolumeUnit> RectangularPrismVolume(Measurement<AreaUnit> area, Measurement<DistanceUnit> depth) => new Measurement<VolumeUnit>(area.In(AreaUnit.SquareMeter) * depth.In(DistanceUnit.Meter), VolumeUnit.CubicMeter);

        /// <summary>
        /// Calculate the volume of a rectangular prism (misspelled name kept for compatibility, use RectangularPrismVolume instead)
        /// </summary>
        /// <param name="area"></param>
        /// <param name="depth"></param>
        /// <returns></returns>
        [Obsolete("Use RectangularPrismVolume instead.")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Measurement<VolumeUnit> RecangularPrismVolume(Measurement<AreaUnit> area, Measurement<DistanceUnit> depth) => RectangularPrismVolume(area, depth);

        /// <summary>
        /// Calculate pressure
        /// </summary>
        /// <param name="weight">Weight</param>
        /// <param name="area">Area</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Measurement<PressureUnit> Pressure(Measurement<WeightUnit> weight, Measurement<AreaUnit> area) => new Measurement<PressureUnit>(weight.In(WeightUnit.Pound) / area.In(AreaUnit.SquareInch), PressureUnit.PoundsPerSquareInch);

        /// <summary>
        /// Calculate the mechanical power delivered by a torque at a rotational speed
        /// </summary>
        /// <remarks>
        /// The rotational speed is taken as an angular velocity in radians per second, which is
        /// what makes the product a power. Multiplying by a speed given in revolutions per
        /// minute works because the unit is converted first.
        /// </remarks>
        /// <param name="torque">Torque</param>
        /// <param name="rotationalSpeed">Rotational speed</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Measurement<PowerUnit> Power(Measurement<TorqueUnit> torque, Measurement<RotationalSpeedUnit> rotationalSpeed) => new Measurement<PowerUnit>(torque.In(TorqueUnit.NewtonMeter) * rotationalSpeed.In(RotationalSpeedUnit.RadianPerSecond), PowerUnit.Watt);

        /// <summary>
        /// Calculate the force needed to accelerate a mass
        /// </summary>
        /// <param name="weight">Mass</param>
        /// <param name="acceleration">Acceleration</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Measurement<ForceUnit> Force(Measurement<WeightUnit> weight, Measurement<AccelerationUnit> acceleration) => new Measurement<ForceUnit>(weight.In(WeightUnit.Kilogram) * acceleration.In(AccelerationUnit.MeterPerSecondSquare), ForceUnit.Newton);

        /// <summary>
        /// Calculate the density of a mass filling a volume
        /// </summary>
        /// <param name="weight">Mass</param>
        /// <param name="volume">Volume</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Measurement<DensityUnit> Density(Measurement<WeightUnit> weight, Measurement<VolumeUnit> volume) => new Measurement<DensityUnit>(weight.In(WeightUnit.Kilogram) / volume.In(VolumeUnit.CubicMeter), DensityUnit.KilogramPerCubicMeter);

        /// <summary>
        /// Calculate the mass of a volume of a substance of the specified density
        /// </summary>
        /// <param name="density">Density</param>
        /// <param name="volume">Volume</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Measurement<WeightUnit> Weight(Measurement<DensityUnit> density, Measurement<VolumeUnit> volume) => new Measurement<WeightUnit>(density.In(DensityUnit.KilogramPerCubicMeter) * volume.In(VolumeUnit.CubicMeter), WeightUnit.Kilogram);

        /// <summary>
        /// Calculate the acceleration which reaches a velocity in the specified time
        /// </summary>
        /// <param name="velocity">Velocity</param>
        /// <param name="time">Time</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Measurement<AccelerationUnit> Acceleration(Measurement<VelocityUnit> velocity, TimeSpan time) => new Measurement<AccelerationUnit>(velocity.In(VelocityUnit.MetersPerSecond) / time.TotalSeconds, AccelerationUnit.MeterPerSecondSquare);

        /// <summary>
        /// Calculate time of travel
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="velocity"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TimeSpan TravelTime(Measurement<DistanceUnit> distance, Measurement<VelocityUnit> velocity) => TimeSpan.FromSeconds(distance.In(DistanceUnit.Meter) / velocity.In(VelocityUnit.MetersPerSecond));

        /// <summary>
        /// Calculate traveled distance for constant velocity
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Measurement<DistanceUnit> DistanceTraveled(Measurement<VelocityUnit> velocity, TimeSpan travelTime) => new Measurement<DistanceUnit>(velocity.In(VelocityUnit.MetersPerSecond) * travelTime.TotalSeconds, DistanceUnit.Meter);

        /// <summary>
        /// Calculate traveled distance for constant acceleration
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Measurement<DistanceUnit> DistanceTraveled(Measurement<AccelerationUnit> acceleration, TimeSpan travelTime) => new Measurement<DistanceUnit>(Velocity(acceleration, travelTime).In(VelocityUnit.MetersPerSecond) / 2 * travelTime.TotalSeconds, DistanceUnit.Meter);
    }
}
