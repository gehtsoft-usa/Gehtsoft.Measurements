using FluentAssertions;
using Xunit;

namespace Gehtsoft.Measurements.Test
{
    public class WeightTest
    {
        [Theory]
        [InlineData(55, WeightUnit.Grain, 3.56394, WeightUnit.Gram, 1e-5)]
        [InlineData(5, WeightUnit.Kilogram, 5000, WeightUnit.Gram, 1e-5)]
        [InlineData(1, WeightUnit.Pound, 0.453592, WeightUnit.Kilogram, 1e-5)]
        [InlineData(1, WeightUnit.Kilogram, 9.80665, WeightUnit.Neuton, 1e-5)]
        [InlineData(5, WeightUnit.Ounce, 141.747615, WeightUnit.Gram, 1e-5)]
        [InlineData(1, WeightUnit.TroyOz, 31.1034768, WeightUnit.Gram, 1e-5)]
        [InlineData(1, WeightUnit.Tonne, 1000, WeightUnit.Kilogram, 1e-5)]
        [InlineData(1, WeightUnit.USTonne, 0.907, WeightUnit.Tonne, 1e-5)]
        [InlineData(1, WeightUnit.UKTonne, 1.016, WeightUnit.Tonne, 1e-5)]
        public void Conversion(double value, WeightUnit unit, double expected, WeightUnit targetUnit, double accurracy = 1e-10)
        {
            var v = new Measurement<WeightUnit>(value, unit);
            v.In(targetUnit).Should().BeApproximately(expected, accurracy);
        }
    }
}
