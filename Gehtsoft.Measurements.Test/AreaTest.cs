using FluentAssertions;
using Xunit;

namespace Gehtsoft.Measurements.Test
{
    public class AreaTest
    {
        [Theory]
        [InlineData(120, AreaUnit.Acre, 48.56227, AreaUnit.Hectare, 1e-5)]
        [InlineData(120, AreaUnit.Acre, 0.48562300014476, AreaUnit.SquareKilometer, 1e-5)]
        [InlineData(1, AreaUnit.Ar, 0.01, AreaUnit.Hectare, 1e-5)]
        [InlineData(1, AreaUnit.Ar, 100, AreaUnit.SquareMeter, 1e-5)]
        public void Conversion(double value, AreaUnit unit, double expected, AreaUnit targetUnit, double accurracy = 1e-10)
        {
            var v = new Measurement<AreaUnit>(value, unit);
            v.In(targetUnit).Should().BeApproximately(expected, accurracy);
        }
    }
}
