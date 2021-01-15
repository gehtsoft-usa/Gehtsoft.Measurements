namespace Gehtsoft.Measurements
{
    public enum WeightUnit : int
    {
        [Unit("gr", 0)]
        [Conversion(ConversionOperation.Base)]
        Grain = 0,
        [Unit("oz", 1)]
        [Conversion(ConversionOperation.Multiply, 437.5)]
        Ounce,
        [Unit("g", 1)]
        [Conversion(ConversionOperation.Multiply, 15.4323583529)]
        Gram,
        [Unit("lb", 3)]
        [Conversion(ConversionOperation.Multiply, 7000)]
        Pound,
        [Unit("kg", 3)]
        [Conversion(ConversionOperation.Multiply, 15432.3583529)]
        Kilogram,
        [Unit("N", 3)]
        [Conversion(ConversionOperation.Multiply, 1573.6626)]
        Neuton,

    }
}
