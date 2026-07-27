using System;
using AwesomeAssertions;
using Xunit;

namespace Gehtsoft.Measurements.Test
{
    /// <summary>
    /// A unit enumeration which is declared wrongly must be rejected with an actionable message.
    /// </summary>
    /// <remarks>
    /// The enumerations below are deliberately broken. Every check runs from a static
    /// initializer of the closed measurement type, so the failure reaches the caller wrapped
    /// into a TypeInitializationException.
    /// </remarks>
    public class UnitValidationTest
    {
        public enum NoBaseUnit
        {
            [Unit("a", 1)]
            [Conversion(ConversionOperation.Multiply, 2)]
            First,

            [Unit("b", 1)]
            [Conversion(ConversionOperation.Multiply, 3)]
            Second,
        }

        public enum TwoBaseUnits
        {
            [Unit("a", 1)]
            [Conversion(ConversionOperation.Base)]
            First,

            [Unit("b", 1)]
            [Conversion(ConversionOperation.Base)]
            Second,
        }

        public enum MissingUnitAttribute
        {
            [Unit("a", 1)]
            [Conversion(ConversionOperation.Base)]
            First,

            [Conversion(ConversionOperation.Multiply, 2)]
            Second,
        }

        public enum MissingConversionAttribute
        {
            [Unit("a", 1)]
            [Conversion(ConversionOperation.Base)]
            First,

            [Unit("b", 1)]
            Second,
        }

        public enum DuplicateUnitName
        {
            [Unit("a", 1)]
            [Conversion(ConversionOperation.Base)]
            First,

            [Unit("a", 1)]
            [Conversion(ConversionOperation.Multiply, 2)]
            Second,
        }

        public enum DuplicateAlternativeName
        {
            [Unit("a", "x", 1)]
            [Conversion(ConversionOperation.Base)]
            First,

            [Unit("b", "x", 1)]
            [Conversion(ConversionOperation.Multiply, 2)]
            Second,
        }

        private static string FailureFor<T>()
            where T : Enum
        {
            Action use = () => Measurement<T>.BaseUnit.GetHashCode();

            // the first touch of the type raises TypeInitializationException, later ones
            // raise it again from the cached failure - either way the inner exception is ours
            var exception = use.Should().Throw<TypeInitializationException>().Which;
            exception.InnerException.Should().BeOfType<InvalidOperationException>();
            return exception.InnerException.Message;
        }

        [Fact]
        public void NoBaseUnitIsRejected()
            => FailureFor<NoBaseUnit>().Should().Contain("declares no base unit").And.Contain(nameof(NoBaseUnit));

        [Fact]
        public void TwoBaseUnitsAreRejected()
            => FailureFor<TwoBaseUnits>().Should().Contain("more than one base unit").And.Contain("First").And.Contain("Second");

        [Fact]
        public void MissingUnitAttributeIsRejected()
            => FailureFor<MissingUnitAttribute>().Should().Contain("not marked with the Unit attribute").And.Contain("Second");

        [Fact]
        public void MissingConversionAttributeIsRejected()
            => FailureFor<MissingConversionAttribute>().Should().Contain("not marked with the Conversion attribute").And.Contain("Second");

        [Fact]
        public void DuplicateUnitNameIsRejected()
            => FailureFor<DuplicateUnitName>().Should().Contain("more than one unit").And.Contain("'a'");

        [Fact]
        public void DuplicateAlternativeNameIsRejected()
            => FailureFor<DuplicateAlternativeName>().Should().Contain("more than one unit").And.Contain("'x'");

        /// <summary>
        /// The library's own enumerations must of course still pass every check.
        /// </summary>
        [Fact]
        public void TheLibraryOwnUnitsAreValid()
        {
            Measurement<DistanceUnit>.BaseUnit.Should().Be(DistanceUnit.Inch);
            Measurement<TemperatureUnit>.BaseUnit.Should().Be(TemperatureUnit.Fahrenheit);
            Measurement<TestUnit>.BaseUnit.Should().Be(TestUnit.Base);
        }
    }
}
