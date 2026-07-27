using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AwesomeAssertions;
using Xunit;

namespace Gehtsoft.Measurements.Test
{
    /// <summary>
    /// The validator which reports what is wrong with a unit enumeration.
    /// </summary>
    public class UnitEnumValidatorTest
    {
        // ---- enumerations which are wrong on purpose -------------------------------------

        public enum Empty
        {
        }

        public enum EmptyName
        {
            [Unit("", 1)]
            [Conversion(ConversionOperation.Base)]
            First,
        }

        public enum NotANumberFactor
        {
            [Unit("a", 1)]
            [Conversion(ConversionOperation.Base)]
            First,

            [Unit("b", 1)]
            [Conversion(ConversionOperation.Multiply, double.NaN)]
            Second,
        }

        public enum ZeroFactor
        {
            [Unit("a", 1)]
            [Conversion(ConversionOperation.Base)]
            First,

            [Unit("b", 1)]
            [Conversion(ConversionOperation.Multiply, 0)]
            Second,
        }

        public enum FactorOnAnOperationWhichIgnoresIt
        {
            [Unit("a", 1)]
            [Conversion(ConversionOperation.Base)]
            First,

            [Unit("b", 1)]
            [Conversion(ConversionOperation.Negate, 5)]
            Second,
        }

        public enum SillyAccuracy
        {
            [Unit("a", 1)]
            [Conversion(ConversionOperation.Base)]
            First,

            [Unit("b", 20)]
            [Conversion(ConversionOperation.Multiply, 2)]
            Second,
        }

        public enum OverflowingFactor
        {
            [Unit("a", 1)]
            [Conversion(ConversionOperation.Base)]
            First,

            [Unit("b", 1)]
            [Conversion(ConversionOperation.Multiply, double.MaxValue)]
            Second,
        }

        /// <summary>
        /// The name of the second unit ends the text produced by the first one.
        /// </summary>
        /// <remarks>
        /// `1.25x` ends with `5x`, which is longer than `x`, so it is read as 1.2 of the second
        /// unit rather than 1.25 of the first one.
        /// </remarks>
        public enum CollidingNames
        {
            [Unit("x", 2)]
            [Conversion(ConversionOperation.Base)]
            First,

            [Unit("5x", 2)]
            [Conversion(ConversionOperation.Multiply, 2)]
            Second,
        }

        /// <summary>
        /// A custom conversion whose reverse is not the reverse of its forward operation.
        /// </summary>
        /// <remarks>
        /// This is the one place the library cannot derive the reverse itself, so it is the one
        /// place a conversion can fail to round-trip.
        /// </remarks>
        public class BrokenCustomConversion : ICustomConversionOperation
        {
            public double ToBase(double value) => value * 2;

            public double FromBase(double value) => value / 3;      // should have been / 2
        }

        public enum BrokenCustomConversionUnit
        {
            [Unit("a", 1)]
            [Conversion(ConversionOperation.Base)]
            First,

            [Unit("b", 1)]
            [Conversion(ConversionOperation.Custom, "Gehtsoft.Measurements.Test.UnitEnumValidatorTest+BrokenCustomConversion")]
            Second,
        }

        // ---- helpers ----------------------------------------------------------------------

        private static UnitValidationReport Report<T>() where T : Enum => UnitEnumValidator.Validate<T>();

        private static void ShouldHave(UnitValidationReport report, string code, UnitValidationSeverity severity, string unit = null)
        {
            var matching = report.Findings.Where(f => f.Code == code).ToArray();
            matching.Should().NotBeEmpty($"{code} was expected, the report is:{Environment.NewLine}{report}");
            matching.Should().OnlyContain(f => f.Severity == severity);
            if (unit != null)
                matching.Should().Contain(f => f.Unit == unit);
        }

        // ---- the checks --------------------------------------------------------------------

        [Fact]
        public void EnumerationWithoutUnitsIsRejected()
        {
            var report = Report<Empty>();
            ShouldHave(report, "GM001", UnitValidationSeverity.Error);
            report.HasErrors.Should().BeTrue();
            report.IsValid.Should().BeFalse();
        }

        [Fact]
        public void MissingAttributesAreReported()
        {
            ShouldHave(Report<UnitValidationTest.MissingUnitAttribute>(), "GM002", UnitValidationSeverity.Error, "Second");
            ShouldHave(Report<UnitValidationTest.MissingConversionAttribute>(), "GM003", UnitValidationSeverity.Error, "Second");
        }

        [Fact]
        public void MissingAndDuplicatedBaseUnitsAreReported()
        {
            ShouldHave(Report<UnitValidationTest.NoBaseUnit>(), "GM004", UnitValidationSeverity.Error);
            ShouldHave(Report<UnitValidationTest.TwoBaseUnits>(), "GM005", UnitValidationSeverity.Error);
        }

        [Fact]
        public void EmptyNameIsReported() => ShouldHave(Report<EmptyName>(), "GM006", UnitValidationSeverity.Error, "First");

        [Fact]
        public void DuplicateNamesAreReported()
        {
            ShouldHave(Report<UnitValidationTest.DuplicateUnitName>(), "GM007", UnitValidationSeverity.Error, "Second");
            ShouldHave(Report<UnitValidationTest.DuplicateAlternativeName>(), "GM007", UnitValidationSeverity.Error, "Second");
        }

        [Fact]
        public void BadFactorsAreReported()
        {
            ShouldHave(Report<NotANumberFactor>(), "GM008", UnitValidationSeverity.Error, "Second");
            ShouldHave(Report<ZeroFactor>(), "GM009", UnitValidationSeverity.Error, "Second");
        }

        [Fact]
        public void AFactorOnAnOperationWhichIgnoresItIsAWarning()
        {
            var report = Report<FactorOnAnOperationWhichIgnoresIt>();
            ShouldHave(report, "GM010", UnitValidationSeverity.Warning, "Second");
            report.HasErrors.Should().BeFalse("an ignored factor does not stop the unit from working");
        }

        [Fact]
        public void AnAccuracyOutsideTheSupportedRangeIsAWarning()
        {
            var report = Report<SillyAccuracy>();
            ShouldHave(report, "GM011", UnitValidationSeverity.Warning, "Second");
            report.HasErrors.Should().BeFalse();
        }

        [Fact]
        public void ANameWhichCanSwallowTheValueIsAWarning()
        {
            var report = Report<CollidingNames>();
            ShouldHave(report, "GM012", UnitValidationSeverity.Warning, "Second");
        }

        /// <summary>
        /// The warning is advisory; the parse round-trip proves the names really do collide.
        /// </summary>
        [Fact]
        public void CollidingNamesAreProvenByTheParseRoundTrip()
        {
            var report = Report<CollidingNames>();

            ShouldHave(report, "GM017", UnitValidationSeverity.Error, "First");
            report.HasErrors.Should().BeTrue();

            // and this is what actually happens
            Measurement<CollidingNames>.TryParse(System.Globalization.CultureInfo.InvariantCulture, "1.25x", out var parsed).Should().BeTrue();
            parsed.Unit.Should().Be(CollidingNames.Second);
            parsed.Value.Should().Be(1.2);
        }

        [Fact]
        public void ACustomConversionWithoutTheDecimalInterfaceIsAWarning()
        {
            // TestUnit.Unit8 uses TestConversion, which implements only ICustomConversionOperation
            var report = Report<TestUnit>();
            ShouldHave(report, "GM013", UnitValidationSeverity.Warning, "Unit8");
            report.HasErrors.Should().BeFalse("TestUnit is otherwise a valid enumeration");
        }

        [Fact]
        public void AConversionWhichOverflowsIsReported()
            => ShouldHave(Report<OverflowingFactor>(), "GM016", UnitValidationSeverity.Error, "Second");

        /// <summary>
        /// The check with the most teeth: a custom conversion whose reverse is wrong.
        /// </summary>
        [Fact]
        public void AConversionWhichDoesNotRoundTripIsReported()
        {
            var report = Report<BrokenCustomConversionUnit>();
            ShouldHave(report, "GM015", UnitValidationSeverity.Error, "Second");
            report.HasErrors.Should().BeTrue();
        }

        public enum Trigonometric
        {
            [Unit("n", 5)]
            [Conversion(ConversionOperation.Base)]
            Base,

            [Unit("t", 2)]
            [Conversion(ConversionOperation.Tan)]
            Tangent,

            [Unit("a", 2)]
            [Conversion(ConversionOperation.Atan)]
            Arctangent,
        }

        /// <summary>
        /// A tangent conversion must not be reported as broken merely for being periodic.
        /// </summary>
        /// <remarks>
        /// The arc tangent only undoes the tangent between minus and plus a quarter turn, so
        /// probing such a unit with a larger value round-trips to a different number through no
        /// fault of the unit. The first version of the round-trip check did exactly that.
        /// </remarks>
        [Fact]
        public void APeriodicConversionIsNotReportedAsBroken()
            => Report<Trigonometric>().Findings.Should().BeEmpty();

        // ---- behaviour of the validator itself ----------------------------------------------

        /// <summary>
        /// A broken enumeration must not be left unusable by the act of validating it.
        /// </summary>
        /// <remarks>
        /// The conversion checks need `Measurement` of the enumeration, and touching that type
        /// runs the validation built into its static initializer, which throws and leaves the
        /// closed type faulted for the rest of the process. The validator must therefore stop
        /// after the structural checks when they found something.
        /// </remarks>
        [Fact]
        public void ValidatingABrokenEnumerationReturnsInsteadOfThrowing()
        {
            Action validate = () => UnitEnumValidator.Validate<UnitValidationTest.NoBaseUnit>();
            validate.Should().NotThrow();

            var report = Report<UnitValidationTest.NoBaseUnit>();
            report.HasErrors.Should().BeTrue();

            // the behavioural checks were skipped, so nothing but the structural finding is there
            report.Findings.Should().OnlyContain(f => f.Code == "GM004");
        }

        [Fact]
        public void TheReportReadsWell()
        {
            var report = Report<UnitValidationTest.DuplicateUnitName>();

            string text = report.ToString();
            text.Should().Contain("DuplicateUnitName");
            text.Should().Contain("GM007");
            text.Should().Contain("'a'");

            Action throwIfInvalid = () => report.ThrowIfInvalid();
            throwIfInvalid.Should().Throw<InvalidOperationException>().WithMessage("*GM007*");
        }

        [Fact]
        public void AValidEnumerationThrowsNothing()
        {
            var report = UnitEnumValidator.Validate<DistanceUnit>();

            report.IsValid.Should().BeTrue();
            report.ToString().Should().Be("DistanceUnit: valid");

            Action throwIfInvalid = () => report.ThrowIfInvalid();
            throwIfInvalid.Should().NotThrow();
        }

        [Fact]
        public void TheNonGenericOverloadAgreesWithTheGenericOne()
        {
            UnitEnumValidator.Validate(typeof(DistanceUnit)).IsValid.Should().BeTrue();

            var broken = UnitEnumValidator.Validate(typeof(UnitValidationTest.DuplicateUnitName));
            broken.HasErrors.Should().BeTrue();
            broken.Findings.Should().Contain(f => f.Code == "GM007");

            UnitEnumValidator.Validate(typeof(string)).Findings.Should().Contain(f => f.Code == "GM001");

            Action nullType = () => UnitEnumValidator.Validate(null);
            nullType.Should().Throw<ArgumentNullException>();
        }

        // ---- the library's own enumerations --------------------------------------------------

        public static IEnumerable<object[]> LibraryUnitTypes
            => typeof(Measurement<>).Assembly
                                    .GetTypes()
                                    .Where(t => t.IsEnum && t.IsPublic)
                                    .Where(t => t.GetFields(BindingFlags.Public | BindingFlags.Static)
                                                 .Any(f => f.GetCustomAttribute<UnitAttribute>() != null))
                                    .OrderBy(t => t.Name)
                                    .Select(t => new object[] { t });

        /// <summary>
        /// Every unit enumeration the library ships must validate without a single finding.
        /// </summary>
        /// <remarks>
        /// The warnings are asserted as well as the errors. A warning on a shipped enumeration
        /// means either the enumeration or the check is wrong, and either way it must be looked
        /// at rather than accumulated.
        /// </remarks>
        [Theory]
        [MemberData(nameof(LibraryUnitTypes))]
        public void EveryLibraryUnitEnumerationIsValid(Type unitType)
        {
            var report = UnitEnumValidator.Validate(unitType);

            report.Findings.Should().BeEmpty($"the report is:{Environment.NewLine}{report}");
        }

        [Fact]
        public void AllSeventeenLibraryUnitEnumerationsAreCovered()
            => LibraryUnitTypes.Should().HaveCount(17);
    }
}
