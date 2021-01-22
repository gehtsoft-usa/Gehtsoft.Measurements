using FluentAssertions;
using Xunit;

namespace Gehtsoft.Measurements.Test
{
    public class PressureTest
    {
        [Theory]
        [InlineData(1, PressureUnit.KiloPascal, 1000, PressureUnit.Pascal, 1e-5)]
        [InlineData(1, PressureUnit.Atmosphere, 760, PressureUnit.MillimetersOfMercury, 1e-3)]
        [InlineData(29.92, PressureUnit.InchesOfMercury, 759.96801, PressureUnit.MillimetersOfMercury, 1e-3)]
        [InlineData(1, PressureUnit.PoundsPerSquareInch, 0.0689476, PressureUnit.Bar, 1e-5)]

        public void Conversion(double value, PressureUnit unit, double expected, PressureUnit targetUnit, double accurracy = 1e-10)
        {
            Measurement<PressureUnit> v = new Measurement<PressureUnit>(value, unit);
            v.In(targetUnit).Should().BeApproximately(expected, accurracy);
        }
    }
}
