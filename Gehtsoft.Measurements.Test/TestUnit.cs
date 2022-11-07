namespace Gehtsoft.Measurements.Test
{
    public enum TestUnit
    {
        [Unit("\"", "u1", 3)]
        [Conversion(ConversionOperation.Add, 2)]
        Unit1,

        [Unit("u2", 2)]
        [Conversion(ConversionOperation.Subtract, 2)]
        Unit2,

        [Unit("u3", 2)]
        [Conversion(ConversionOperation.SubtractFromFactor, 2)]
        Unit3,

        [Unit("u4", 2)]
        [Conversion(ConversionOperation.Multiply, 2)]
        Unit4,

        [Unit("u5", 2)]
        [Conversion(ConversionOperation.Divide, 2)]
        Unit5,

        [Unit("u6", 2)]
        [Conversion(ConversionOperation.DivideFactor, 2)]
        Unit6,

        [Unit("u7", 2)]
        [Conversion(ConversionOperation.Multiply, 2, ConversionOperation.Add, 4)]
        Unit7,

        [Unit("u8", 2)]
        [Conversion(ConversionOperation.Custom, "Gehtsoft.Measurements.Test.TestConversion")]
        Unit8,

        [Unit("u9", 2)]
        [Conversion(ConversionOperation.Custom, "Gehtsoft.Measurements.Test.TestConversion2")]
        Unit9,

        [Unit("n1", "n2", 5)]
        [Conversion(ConversionOperation.Base)]
        Base,
    }
}
