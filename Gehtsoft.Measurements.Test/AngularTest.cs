using FluentAssertions;
using System.Globalization;
using Xunit;

namespace Gehtsoft.Measurements.Test
{
    public class AngularTest
    {
        [Theory]
        [InlineData(360, AngularUnit.Degree, 6.28318530717958, AngularUnit.Radian)]
        [InlineData(1000, AngularUnit.MRad, 1, AngularUnit.Radian)]
        [InlineData(30, AngularUnit.MOA, 0.5, AngularUnit.Degree)]
        [InlineData(1, AngularUnit.MRad, 3.6, AngularUnit.InchesPer100Yards, 1e-5)]
        [InlineData(1, AngularUnit.Mil, 3.5343, AngularUnit.InchesPer100Yards, 1e-5)]
        [InlineData(1, AngularUnit.Thousand, 10.47198, AngularUnit.CmPer100Meters, 1e-5)]
        public void Conversion(double value, AngularUnit unit, double expected, AngularUnit targetUnit, double accurracy = 1e-10)
        {
            var v = new Measurement<AngularUnit>(value, unit);
            v.In(targetUnit).Should().BeApproximately(expected, accurracy);
        }
    }
}
