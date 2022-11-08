namespace Gehtsoft.Measurements
{
    /// <summary>
    /// Units for angular measurements
    /// </summary>
    public enum AngularUnit
    {
        /// <summary>
        /// Radians (2π radians per full circle)
        /// </summary>
        [Unit("rad", 6)]
        [Conversion(ConversionOperation.Base)]
        Radian = 0,

        /// <summary>
        /// Degrees (360 degrees per full circle)
        /// </summary>
        [Unit("°", "deg", 4)]
        [Conversion(ConversionOperation.Divide, 180, ConversionOperation.Multiply, 3.14159265358979)]
        Degree,

        /// <summary>
        /// Minutes of angle (1/60 of a degree)
        /// </summary>
        [Unit("moa", 2)]
        [Conversion(ConversionOperation.Divide, 10800, ConversionOperation.Multiply, 3.14159265358979)]
        //1/3600 of circle
        MOA,
        
        /// <summary>
        /// Military mil (1/6400 of a circle)
        /// </summary>
        //1/6400 of circle
        [Unit("mil", 2)]
        [Conversion(ConversionOperation.Divide, 3200, ConversionOperation.Multiply, 3.14159265358979)]
        Mil,

        /// <summary>
        /// Milliradian (1/1000 of a radian)
        /// </summary>
        [Unit("mrad", 2)]
        [Conversion(ConversionOperation.Divide, 1000)]
        MRad,

        /// <summary>
        /// A thousand (Soviet/Russian or Finish military unit, 1/3000 of a circle)
        /// </summary>
        //1/3000 of circle
        [Unit("ths", 2)]
        [Conversion(ConversionOperation.Divide, 3000, ConversionOperation.Multiply, 3.14159265358979)]
        Thousand,

        /// <summary>
        /// Inches per 100 yards
        /// </summary>
        [Unit("in/100yd", 2)]
        [Conversion(ConversionOperation.Divide, 3600, ConversionOperation.Atan, 0)]
        InchesPer100Yards,

        /// <summary>
        /// Centimeters per 100 meters
        /// </summary>
        [Unit("cm/100m", 2)]
        [Conversion(ConversionOperation.Divide, 10000, ConversionOperation.Atan, 0)]
        CmPer100Meters,

        /// <summary>
        /// The incline measured in percents
        /// </summary>
        [Unit("%", "percent", 0)]
        [Conversion(ConversionOperation.Divide, 100, ConversionOperation.Atan, 0)]
        Percent,

        /// <summary>
        /// The turns (1 turn is one full circle)
        /// </summary>
        [Unit("turn", 0)]
        [Conversion(ConversionOperation.Multiply, 6.28318530717958)]
        Turn,

        /// <summary>
        /// Gradian (1/400 of full circle)
        /// </summary>
        [Unit("gon", "ᵍ", 0)]
        [Conversion(ConversionOperation.Multiply, 6.28318530717958, ConversionOperation.Divide, 400.0)]
        Gradian,
    }
}
