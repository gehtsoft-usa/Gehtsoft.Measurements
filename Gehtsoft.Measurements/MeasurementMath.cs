﻿using System;
using System.Collections.Generic;
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
        public static int Sign<T>(this Measurement<T> value) where T : Enum
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
        public static int Sign<T>(this DecimalMeasurement<T> value) where T : Enum
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
        public static Measurement<T> Sqrt<T>(this Measurement<T> value) where T : Enum => new Measurement<T>(Math.Sqrt(value.Value), value.Unit);

        /// <summary>
        /// Calculate square root of a value
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DecimalMeasurement<T> Sqrt<T>(this DecimalMeasurement<T> value) where T : Enum => new DecimalMeasurement<T>((decimal)Math.Sqrt((double)value.Value), value.Unit);

        /// <summary>
        /// Raise the value in the power specified
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Measurement<T> Pow<T>(this Measurement<T> value, double exp) where T : Enum => new Measurement<T>(Math.Pow(value.Value, exp), value.Unit);

        /// <summary>
        /// Raise the value in the power specified
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DecimalMeasurement<T> Pow<T>(this DecimalMeasurement<T> value, decimal exp) where T : Enum => new DecimalMeasurement<T>((decimal)Math.Pow((double)value.Value, (double)exp), value.Unit);

        /// <summary>
        /// Calculate the absolute value
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Measurement<T> Abs<T>(this Measurement<T> value) where T : Enum => new Measurement<T>(Math.Abs(value.Value), value.Unit);


        /// <summary>
        /// Calculate the absolute value
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DecimalMeasurement<T> Abs<T>(this DecimalMeasurement<T> value) where T : Enum => new DecimalMeasurement<T>(Math.Abs(value.Value), value.Unit);

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
        public static Measurement<VolumeUnit> RecangularPrismVolume(Measurement<AreaUnit> area, Measurement<DistanceUnit> depth) => new Measurement<VolumeUnit>(area.In(AreaUnit.SquareMeter) * depth.In(DistanceUnit.Meter), VolumeUnit.CubicMeter);

        /// <summary>
        /// Calculate pressure
        /// </summary>
        /// <param name="weight">Weight</param>
        /// <param name="area">Area</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Measurement<PressureUnit> Pressure(Measurement<WeightUnit> weight, Measurement<AreaUnit> area) => new Measurement<PressureUnit>(weight.In(WeightUnit.Pound) / area.In(AreaUnit.SquareInch), PressureUnit.PoundsPerSquareInch);

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
