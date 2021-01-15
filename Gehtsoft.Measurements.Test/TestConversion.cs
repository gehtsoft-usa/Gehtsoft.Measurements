namespace Gehtsoft.Measurements.Test
{
    public class TestConversion : ICustomConversionOperation
    {
        public double FromBase(double value)
        {
            return 2 / (value + 1);
        }

        public double ToBase(double value)
        {
            return 2 / value - 1;
        }
    }
}
