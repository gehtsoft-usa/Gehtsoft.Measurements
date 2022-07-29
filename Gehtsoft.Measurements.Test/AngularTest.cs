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
        [InlineData(100, AngularUnit.Percent, 45, AngularUnit.Degree, 1e-5)]
        [InlineData(50, AngularUnit.Percent, 26.56505, AngularUnit.Degree, 1e-5)]
        [InlineData(26.56505, AngularUnit.Degree, 50, AngularUnit.Percent, 1e-5)]
        public void ConversionAngular(double value, AngularUnit unit, double expected, AngularUnit targetUnit, double accurracy = 1e-10)
        {
            var v = new Measurement<AngularUnit>(value, unit);
            v.In(targetUnit).Should().BeApproximately(expected, accurracy);
        }

        [Theory]
        [InlineData(1, SolidAngularUnit.SquareDegree, 3.0461742e-4, SolidAngularUnit.Steradian)]
        [InlineData(1, SolidAngularUnit.SquareDegree, 3600, SolidAngularUnit.SquareMinute)]
        [InlineData(1, SolidAngularUnit.Steradian, 11818102.860042277, SolidAngularUnit.SquareMinute)]
        public void ConversionSolid(double value, SolidAngularUnit unit, double expected, SolidAngularUnit targetUnit, double accurracy = 1e-10)
        {
            var v = new Measurement<SolidAngularUnit>(value, unit);
            v.In(targetUnit).Should().BeApproximately(expected, accurracy);
        }
    }
}
