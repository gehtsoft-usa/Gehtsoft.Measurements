using AwesomeAssertions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using Xunit;

namespace Gehtsoft.Measurements.Test
{
    /// <summary>
    /// <para>Exhaustive coverage of the unit names of all unit enumerations of the library.</para>
    /// <para>
    /// Every name of every unit (both the primary and the alternative one) must be accepted by the parser,
    /// and the primary name must always be the one used to format a value.
    /// </para>
    /// <para>
    /// The expected names are re-read from <see cref="UnitAttribute"/> by reflection rather than taken from
    /// the library's own name lists, so that the test validates those lists instead of repeating them.
    /// </para>
    /// </summary>
    public class UnitNamesTest
    {
        /// <summary>
        /// All unit enumerations of the library.
        /// </summary>
        private static readonly Type[] gUnitTypes = new[]
        {
            typeof(AccelerationUnit),
            typeof(AngularUnit),
            typeof(AreaUnit),
            typeof(DensityUnit),
            typeof(DistanceUnit),
            typeof(EnergyUnit),
            typeof(ForceUnit),
            typeof(GasConsumptionUnit),
            typeof(PowerUnit),
            typeof(PressureUnit),
            typeof(RotationalSpeedUnit),
            typeof(SolidAngularUnit),
            typeof(TemperatureUnit),
            typeof(TorqueUnit),
            typeof(VelocityUnit),
            typeof(VolumeUnit),
            typeof(WeightUnit),
        };

        public static IEnumerable<object[]> UnitTypes => gUnitTypes.Select(t => new object[] { t });

        /// <summary>
        /// Guards the list above against a newly added unit enumeration which would otherwise be silently untested.
        /// </summary>
        [Fact]
        public void AllUnitEnumerationsAreTested()
        {
            var all = typeof(Measurement<>).Assembly
                                           .GetTypes()
                                           .Where(t => t.IsEnum && t.IsPublic && UnitsOf(t).Any());

            all.Should().BeEquivalentTo(gUnitTypes);
        }

        [Theory]
        [MemberData(nameof(UnitTypes))]
        public void ParseEveryUnitName(Type unitType) => RunFor(nameof(EveryNameIsParsed), unitType);

        [Theory]
        [MemberData(nameof(UnitTypes))]
        public void FormatUsesPrimaryUnitName(Type unitType) => RunFor(nameof(PrimaryNameIsFormatted), unitType);

        private static void EveryNameIsParsed<T>()
            where T : Enum
        {
            var units = UnitsOf(typeof(T)).ToArray();
            units.Should().NotBeEmpty();

            foreach (var unit in units)
            {
                var u = (T)unit.Unit;

                foreach (string name in unit.Names)
                {
                    string because = $"'{name}' is a name of {typeof(T).Name}.{u}";

                    Measurement<T>.ParseUnitName(name).Should().Be(u, because);
                    DecimalMeasurement<T>.ParseUnitName(name).Should().Be(u, because);

                    // the name must also be recognized as the unit part of a complete measurement,
                    // for an integer, a fractional and a negative value alike
                    ParsesAs("123" + name, 123, u, because);
                    ParsesAs("1.25" + name, 1.25m, u, because);
                    ParsesAs("-1.25" + name, -1.25m, u, because);
                }
            }
        }

        private static void ParsesAs<T>(string text, decimal value, T unit, string because)
            where T : Enum
        {
            Measurement<T>.TryParse(CultureInfo.InvariantCulture, text, out Measurement<T> m).Should().BeTrue(because);
            m.Unit.Should().Be(unit, because);
            m.Value.Should().Be((double)value, because);

            new Measurement<T>(text).Should().Be(new Measurement<T>((double)value, unit), because);

            DecimalMeasurement<T>.TryParse(CultureInfo.InvariantCulture, text, out DecimalMeasurement<T> d).Should().BeTrue(because);
            d.Unit.Should().Be(unit, because);
            d.Value.Should().Be(value, because);

            new DecimalMeasurement<T>(text).Should().Be(new DecimalMeasurement<T>(value, unit), because);
        }

        private static void PrimaryNameIsFormatted<T>()
            where T : Enum
        {
            var units = UnitsOf(typeof(T)).ToArray();
            units.Should().NotBeEmpty();

            // only the primary names are listed as the names of the units
            Measurement<T>.GetUnitNames().Should().BeEquivalentTo(units.Select(u => new Tuple<T, string>((T)u.Unit, u.Primary)));
            DecimalMeasurement<T>.GetUnitNames().Should().BeEquivalentTo(units.Select(u => new Tuple<T, string>((T)u.Unit, u.Primary)));

            foreach (var unit in units)
            {
                var u = (T)unit.Unit;
                string because = $"the primary name of {typeof(T).Name}.{unit.Unit} is '{unit.Primary}'";
                string byDefault = "1.25" + unit.Primary;

                // the accuracy of the unit is applied to the value, each numeric type rounding as it always does
                // (double and decimal do not round a midpoint the same way, which is not what is tested here)
                string byAccuracy = 1.25.ToString($"N{unit.DefaultAccuracy}", CultureInfo.InvariantCulture) + unit.Primary;
                string byAccuracyDecimal = 1.25m.ToString($"N{unit.DefaultAccuracy}", CultureInfo.InvariantCulture) + unit.Primary;

                Measurement<T>.GetUnitName(u).Should().Be(unit.Primary, because);
                DecimalMeasurement<T>.GetUnitName(u).Should().Be(unit.Primary, because);

                var m = new Measurement<T>(1.25, u);
                m.ToString().Should().Be(byDefault, because);
                m.ToString(CultureInfo.InvariantCulture).Should().Be(byDefault, because);
                m.ToString("NF", CultureInfo.InvariantCulture).Should().Be(byDefault, because);
                m.ToString("N2", CultureInfo.InvariantCulture).Should().Be(byDefault, because);
                m.ToString("ND", CultureInfo.InvariantCulture).Should().Be(byAccuracy, because);
                m.Text.Should().Be(byDefault, because);
                $"{m}".Should().Be(byDefault, because);

                var d = new DecimalMeasurement<T>(1.25m, u);
                d.ToString().Should().Be(byDefault, because);
                d.ToString(CultureInfo.InvariantCulture).Should().Be(byDefault, because);
                d.ToString("NF", CultureInfo.InvariantCulture).Should().Be(byDefault, because);
                d.ToString("N2", CultureInfo.InvariantCulture).Should().Be(byDefault, because);
                d.ToString("ND", CultureInfo.InvariantCulture).Should().Be(byAccuracyDecimal, because);
                d.Text.Should().Be(byDefault, because);
                $"{d}".Should().Be(byDefault, because);

                // the formatted value must be readable back as the very same measurement
                Measurement<T>.TryParse(CultureInfo.InvariantCulture, m.ToString(), out Measurement<T> m1).Should().BeTrue(because);
                m1.Value.Should().Be(1.25, because);
                m1.Unit.Should().Be(u, because);

                DecimalMeasurement<T>.TryParse(CultureInfo.InvariantCulture, d.ToString(), out DecimalMeasurement<T> d1).Should().BeTrue(because);
                d1.Value.Should().Be(1.25m, because);
                d1.Unit.Should().Be(u, because);
            }
        }

        /// <summary>
        /// The units of the enumeration and their names, as they are declared by <see cref="UnitAttribute"/>.
        /// </summary>
        /// <remarks>
        /// The obsolete units are skipped - they are excluded from the unit listing and from parsing on purpose,
        /// so that a renamed misspelling does not shadow the unit which replaces it.
        /// </remarks>
        private static IEnumerable<(object Unit, string Primary, string[] Names, int DefaultAccuracy)> UnitsOf(Type unitType)
        {
            foreach (var field in unitType.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                if (field.GetCustomAttribute<ObsoleteAttribute>() != null)
                    continue;

                UnitAttribute attribute = field.GetCustomAttribute<UnitAttribute>();
                if (attribute == null)
                    continue;

                string[] names = attribute.HasAlternativeName
                    ? new[] { attribute.Name, attribute.AlternativeName }
                    : new[] { attribute.Name };

                yield return (Enum.ToObject(unitType, field.GetRawConstantValue()), attribute.Name, names, attribute.DefaultAccuracy);
            }
        }

        /// <summary>
        /// Calls the generic test method for the unit enumeration passed as the theory parameter.
        /// </summary>
        private static void RunFor(string method, Type unitType)
        {
            var generic = typeof(UnitNamesTest).GetMethod(method, BindingFlags.Static | BindingFlags.NonPublic)
                                               .MakeGenericMethod(unitType);
            try
            {
                generic.Invoke(null, null);
            }
            catch (TargetInvocationException e)
            {
                // rethrow the assertion failure itself rather than the reflection wrapper
                ExceptionDispatchInfo.Capture(e.InnerException).Throw();
            }
        }
    }
}
