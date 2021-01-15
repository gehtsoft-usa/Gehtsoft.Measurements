namespace Gehtsoft.Measurements
{
    public enum TemperatureUnit : int
    {
        [Unit("°F", "F", 1)]
        [Conversion(ConversionOperation.Base)]
        Fahrenheit = 0,
        
        [Unit("°C", "C", 1)]
        [Conversion(ConversionOperation.Multiply, 1.8, ConversionOperation.Add, 32)]
        Celsius,
        
        [Unit("°K", "K", 1)]
        [Conversion(ConversionOperation.Multiply, 1.8, ConversionOperation.Subtract, 459.67)]
        Kelvin,

        [Unit("°R", "R", 1)]
        [Conversion(ConversionOperation.Subtract, 459.67)]
        Rankin,
    }
}
