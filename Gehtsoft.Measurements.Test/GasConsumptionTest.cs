using FluentAssertions;
using Xunit;

namespace Gehtsoft.Measurements.Test
{
    public class GasConsumptionTest
    {
        [Theory]
        [InlineData(5, GasConsumptionUnit.MilesPerGallon, 47.0429, GasConsumptionUnit.LiterPer100Km, 1e-4)]
        [InlineData(12, GasConsumptionUnit.LiterPer100Km, 19.6012, GasConsumptionUnit.MilesPerGallon, 1e-4)]
        public void Conversion(double value1, GasConsumptionUnit unit1, double value2, GasConsumptionUnit unit2, double accuracy)
        {
            Measurement<GasConsumptionUnit>.Convert(value1, unit1, unit2).Should().BeApproximately(value2, accuracy);
        }
    }
}
