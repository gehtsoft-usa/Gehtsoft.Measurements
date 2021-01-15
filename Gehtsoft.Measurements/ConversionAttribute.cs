using System;

namespace Gehtsoft.Measurements
{
    [AttributeUsage(validOn: AttributeTargets.Field)]
    internal class ConversionAttribute : Attribute
    {
        public ConversionOperation Operation { get; set; }
        public double Factor { get; set; }
        public ConversionOperation SecondOperation { get; set; }
        public double SecondFactor { get; set; }

        public ConversionAttribute(ConversionOperation operation) : this(operation, 0, ConversionOperation.None, 0)
        {
        }

        public ConversionAttribute(ConversionOperation operation, double factor) : this(operation, factor, ConversionOperation.None, 0)
        {

        }

        public ConversionAttribute(ConversionOperation operation, double factor, ConversionOperation secondOperation, double secondFactor)
        {
            Operation = operation;
            Factor = factor;
            SecondOperation = secondOperation;
            SecondFactor = secondFactor;
        }
    }
}
