namespace Gehtsoft.Measurements.Test
{
    public class TestConversion2 : ICustomConversionOperation2
    {
        public double FromBase(double value)
        {
            return 2 / (value + 1);
        }

        public double ToBase(double value)
        {
            return (2 / value) - 1;
        }
        
        public decimal FromBaseDecimal(decimal value)
        {
            return 2m / (value + 1m);
        }

        public decimal ToBaseDecimal(decimal value)
        {
            return (2m / value) - 1m;
        }
    }
}
