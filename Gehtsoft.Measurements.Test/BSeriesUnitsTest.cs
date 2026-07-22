using System;
using System.Linq;
using AwesomeAssertions;
using Xunit;

namespace Gehtsoft.Measurements.Test
{
    // Verifies the B-series additions (new units, new enums) and typo-alias behavior
    // via known unit relationships rather than hard-coded base factors.
    public class BSeriesUnitsTest
    {
        [Theory]
        // Weight (B1)
        [InlineData(1, WeightUnit.Stone, 14, WeightUnit.Pound, 1e-9)]
        [InlineData(1, WeightUnit.Gram, 1000, WeightUnit.Milligram, 1e-6)]
        [InlineData(1, WeightUnit.Carat, 0.2, WeightUnit.Gram, 1e-9)]
        [InlineData(1, WeightUnit.Slug, 14.5939, WeightUnit.Kilogram, 1e-4)]
        // Distance (B1)
        [InlineData(1, DistanceUnit.Furlong, 660, DistanceUnit.Foot, 1e-6)]
        [InlineData(1, DistanceUnit.Fathom, 6, DistanceUnit.Foot, 1e-9)]
        [InlineData(1, DistanceUnit.Hand, 4, DistanceUnit.Inch, 1e-9)]
        [InlineData(1, DistanceUnit.Thou, 0.001, DistanceUnit.Inch, 1e-9)]
        [InlineData(1, DistanceUnit.Meter, 10, DistanceUnit.Decimeter, 1e-9)]
        [InlineData(1, DistanceUnit.Millimeter, 1000, DistanceUnit.Micrometer, 1e-6)]
        // Velocity (B1)
        [InlineData(1, VelocityUnit.InchesPerSecond, 0.0254, VelocityUnit.MetersPerSecond, 1e-9)]
        [InlineData(1, VelocityUnit.CentimetersPerSecond, 0.01, VelocityUnit.MetersPerSecond, 1e-9)]
        [InlineData(60, VelocityUnit.FeetPerMinute, 1, VelocityUnit.FeetPerSecond, 1e-9)]
        // Pressure (B1)
        [InlineData(1, PressureUnit.Hectopascal, 100, PressureUnit.Pascal, 1e-9)]
        [InlineData(1, PressureUnit.Megapascal, 1_000_000, PressureUnit.Pascal, 1e-3)]
        [InlineData(760, PressureUnit.Torr, 1, PressureUnit.Atmosphere, 1e-9)]
        // Energy (B1)
        [InlineData(1, EnergyUnit.Kilojoule, 1000, EnergyUnit.Joule, 1e-9)]
        [InlineData(1, EnergyUnit.Kilocalorie, 1000, EnergyUnit.Calorie, 1e-9)]
        [InlineData(1, EnergyUnit.Kilocalorie, 4184, EnergyUnit.Joule, 1e-6)]
        [InlineData(1, EnergyUnit.Erg, 1e-7, EnergyUnit.Joule, 1e-16)]
        // Power (B1)
        [InlineData(1, PowerUnit.Kilowatt, 1000, PowerUnit.Watt, 1e-9)]
        [InlineData(1, PowerUnit.Megawatt, 1_000_000, PowerUnit.Watt, 1e-3)]
        // Volume (B1)
        [InlineData(1, VolumeUnit.CubicCentimeter, 1, VolumeUnit.Milliliter, 1e-9)]
        [InlineData(1, VolumeUnit.OilBarrel, 42, VolumeUnit.Gallon, 1e-6)]
        [InlineData(1, VolumeUnit.Tablespoon, 3, VolumeUnit.Teaspoon, 1e-6)]
        [InlineData(1, VolumeUnit.Cup, 8, VolumeUnit.Ounce, 1e-6)]
        // Density (B1)
        [InlineData(1, DensityUnit.KilogramPerLiter, 1000, DensityUnit.KilogramPerCubicMeter, 1e-6)]
        // Acceleration (B1)
        [InlineData(1, AccelerationUnit.InchesPerSecondSquare, 2.54, AccelerationUnit.Gal, 1e-9)]
        public void Convert_Distance_Weight_Velocity_Etc(double value, object unit, double expected, object targetUnit, double accuracy)
        {
            // dispatched by the strongly-typed overloads below
            ConvertDynamic((dynamic)value, (dynamic)unit, (dynamic)expected, (dynamic)targetUnit, accuracy);
        }

        private static void ConvertDynamic<T>(double value, T unit, double expected, T targetUnit, double accuracy)
            where T : Enum
            => new Measurement<T>(value, unit).In(targetUnit).Should().BeApproximately(expected, accuracy);

        [Theory]
        [InlineData(3600, AngularUnit.ArcSecond, 1, AngularUnit.Degree, 1e-6)]
        [InlineData(60, AngularUnit.ArcSecond, 1, AngularUnit.MOA, 1e-6)]
        public void Angular_ArcSecond(double value, AngularUnit unit, double expected, AngularUnit target, double accuracy)
            => new Measurement<AngularUnit>(value, unit).In(target).Should().BeApproximately(expected, accuracy);

        [Theory]
        // Torque (B2)
        [InlineData(1, TorqueUnit.KilogramForceMeter, 9.80665, TorqueUnit.NewtonMeter, 1e-6)]
        [InlineData(1, TorqueUnit.FootPoundForce, 12, TorqueUnit.InchPoundForce, 1e-6)]
        public void Torque(double value, TorqueUnit unit, double expected, TorqueUnit target, double accuracy)
            => new Measurement<TorqueUnit>(value, unit).In(target).Should().BeApproximately(expected, accuracy);

        [Theory]
        // Rotational speed (B2)
        [InlineData(1, RotationalSpeedUnit.Hertz, 60, RotationalSpeedUnit.RevolutionsPerMinute, 1e-6)]
        [InlineData(1, RotationalSpeedUnit.Hertz, 6.28318530717958, RotationalSpeedUnit.RadianPerSecond, 1e-6)]
        public void RotationalSpeed(double value, RotationalSpeedUnit unit, double expected, RotationalSpeedUnit target, double accuracy)
            => new Measurement<RotationalSpeedUnit>(value, unit).In(target).Should().BeApproximately(expected, accuracy);

        [Fact]
        public void GasConsumption_NewUnits()
        {
            // 1 km/l means 100 liters per 100 km.
            new Measurement<GasConsumptionUnit>(1, GasConsumptionUnit.KilometersPerLiter)
                .In(GasConsumptionUnit.LiterPer100Km).Should().BeApproximately(100, 1e-6);

            // 1 imperial mpg travels 1 mile (1.609344 km) on 1 imperial gallon (4.54609 l).
            new Measurement<GasConsumptionUnit>(1, GasConsumptionUnit.ImperialMilesPerGallon)
                .In(GasConsumptionUnit.LiterPer100Km).Should().BeApproximately(4.54609 / 1.609344 * 100, 1e-6);
        }

        [Fact]
        public void Density_PoundsPerGallon()
        {
            // 1 lb per US gallon = 453.59237 g / 3.785411784 l.
            new Measurement<DensityUnit>(1, DensityUnit.PoundsPerGallon)
                .In(DensityUnit.KilogramPerCubicMeter)
                .Should().BeApproximately(453.59237 / 3.785411784, 1e-6);
        }

        [Fact]
#pragma warning disable CS0618 // intentionally references obsolete members to verify they are excluded
        public void ObsoleteEnumMembers_ExcludedFromListingAndParsing_ButStillConvert()
        {
            // Canonical names are what GetUnitNames reports; the misspelled aliases are not.
            var names = Measurement<WeightUnit>.GetUnitNames();
            names.Should().Contain(t => t.Item1 == WeightUnit.Newton);
            names.Any(t => t.Item1 == WeightUnit.Neuton).Should().BeFalse();

            // Parsing the shared name "N" resolves to the canonical member.
            Measurement<WeightUnit>.TryParse(System.Globalization.CultureInfo.InvariantCulture, "1N", out var m)
                .Should().BeTrue();
            m.Unit.Should().Be(WeightUnit.Newton);

            // The obsolete member still converts (for previously-persisted values).
            new Measurement<WeightUnit>(1, WeightUnit.Neuton).In(WeightUnit.Newton)
                .Should().BeApproximately(1, 1e-9);

            // Same treatment for the pressure and power aliases.
            Measurement<PressureUnit>.GetUnitNames().Any(t => t.Item1 == PressureUnit.TechincalAtmosphere)
                .Should().BeFalse();
            Measurement<PressureUnit>.GetUnitNames().Should().Contain(t => t.Item1 == PressureUnit.TechnicalAtmosphere);
            Measurement<PowerUnit>.GetUnitNames().Any(t => t.Item1 == PowerUnit.MetricHoursePower)
                .Should().BeFalse();
            Measurement<PowerUnit>.GetUnitNames().Should().Contain(t => t.Item1 == PowerUnit.MetricHorsePower);
        }
#pragma warning restore CS0618
    }
}
