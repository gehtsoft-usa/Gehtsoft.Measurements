using System;

namespace Gehtsoft.Measurements
{
    /// <summary>
    /// The attribute to specify the conversion rule for the measurmement unit.
    /// 
    /// The attribute is applied on enum fields which describe one unit of the measurement (e.g. "meter")
    /// </summary>
    [AttributeUsage(validOn: AttributeTargets.Field)]
    public class ConversionAttribute : Attribute
    {
        public ConversionOperation Operation { get; set; }
        public double Factor { get; set; }
        public ConversionOperation SecondOperation { get; set; }
        public double SecondFactor { get; set; }


        /// <summary>
        /// The constructor to specify the base unit
        /// </summary>
        /// <param name="operation">Must always be <code>ConversionOperation.Base</code></param>
        public ConversionAttribute(ConversionOperation operation) : this(operation, 0, ConversionOperation.None, 0)
        {
        }


        /// <summary>
        /// The constructor to specify one operation conversion
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="factor"></param>
        public ConversionAttribute(ConversionOperation operation, double factor) : this(operation, factor, ConversionOperation.None, 0)
        {

        }

        /// <summary>
        /// The constructor to specify two-operation conversion
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="factor"></param>
        /// <param name="secondOperation"></param>
        /// <param name="secondFactor"></param>
        public ConversionAttribute(ConversionOperation operation, double factor, ConversionOperation secondOperation, double secondFactor)
        {
            Operation = operation;
            Factor = factor;
            SecondOperation = secondOperation;
            SecondFactor = secondFactor;
        }
    }
}
