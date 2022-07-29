namespace Gehtsoft.Measurements
{
    /// <summary>
    /// Solid angle units
    /// </summary>
    public enum SolidAngularUnit
    {
        /// <summary>
        /// Steradian/square radian (4π per full sphere)
        /// </summary>
        [Unit("sr", 6)]
        [Conversion(ConversionOperation.Base)]
        Steradian = 0,
        
        /// <summary>
        /// Square degree
        /// </summary>
        [Unit("deg2", "sqdeg", 6)]
        [Conversion(ConversionOperation.Multiply, 3.0461741978670859934674354937889e-4)]
        SquareDegree = 1,

        /// <summary>
        /// Square degree
        /// </summary>
        [Unit("moa2", "sqmoa", 6)]
        [Conversion(ConversionOperation.Multiply, 8.4615949940752388707428763716359e-8)]
        SquareMinute = 2,
    }
}
