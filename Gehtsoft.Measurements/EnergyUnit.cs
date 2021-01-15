namespace Gehtsoft.Measurements
{
    public enum EnergyUnit : int
    {
        [Unit("ft·lb", "ft-lb", 0)]
        [Conversion(ConversionOperation.Divide, 0.737562149277)]
        FootPound = 0,
        [Unit("J", 0)]
        [Conversion(ConversionOperation.Base)]
        Joule,
        [Unit("BTU", 0)]
        [Conversion(ConversionOperation.Multiply, 1055)]
        BTU,
        [Unit("hp·h", "hp-h", 0)]
        [Conversion(ConversionOperation.Multiply, 2_684_500)]
        HpH,
        [Unit("w·h", "wh", 0)]
        [Conversion(ConversionOperation.Multiply, 3600)]
        Wh,
        [Unit("kw·h", "kwh", 0)]
        [Conversion(ConversionOperation.Multiply, 3600000)]
        kWh,
    }
}
