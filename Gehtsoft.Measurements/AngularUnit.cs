namespace Gehtsoft.Measurements
{
    public enum AngularUnit : int
    {
        [Unit("rad", 6)]
        [Conversion(ConversionOperation.Base)]
        Radian = 0,
        [Unit("°", "deg", 4)]
        [Conversion(ConversionOperation.Divide, 180, ConversionOperation.Multiply, 3.14159265358979)]
        Degree,
        [Unit("moa", 2)]
        [Conversion(ConversionOperation.Divide, 10800, ConversionOperation.Multiply, 3.14159265358979)]
        //1/3600 of circle
        MOA,
        //1/6400 of circle
        [Unit("mil", 2)]
        [Conversion(ConversionOperation.Divide, 3200, ConversionOperation.Multiply, 3.14159265358979)]
        Mil,
        //milliradian (1 / (2 * PI * 1000) ~ 1 / 6283.1853)
        [Unit("mrad", 2)]
        [Conversion(ConversionOperation.Divide, 1000)]
        MRad,
        //1/3000 of circle
        [Unit("ths", 2)]
        [Conversion(ConversionOperation.Divide, 3000, ConversionOperation.Multiply, 3.14159265358979)]
        Thousand,               
        [Unit("in/100yd", 2)]
        [Conversion(ConversionOperation.Divide, 3600, ConversionOperation.Atan, 0)]
        InchesPer100Yards,
        [Unit("cm/100m", 2)]
        [Conversion(ConversionOperation.Divide, 10000, ConversionOperation.Atan, 0)]
        CmPer100Meters,
    }
}
