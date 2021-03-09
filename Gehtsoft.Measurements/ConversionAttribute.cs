using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace Gehtsoft.Measurements
{
    /// <summary>
    /// The attribute to specify the conversion rule for the measurement unit.
    ///
    /// The attribute is applied on enumeration fields which describe one unit of the measurement (e.g. "meter")
    ///
    /// One of the enumeration fields must always be attributed as a base value.
    /// </summary>
    [AttributeUsage(validOn: AttributeTargets.Field)]
    public class ConversionAttribute : Attribute
    {
        /// <summary>
        /// The first operation to be performed in order to convert the unit into base value
        /// </summary>
        public ConversionOperation Operation { get; set; }

        /// <summary>
        /// The numeric factor for the first operation
        /// </summary>
        public double Factor { get; set; }

        /// <summary>
        /// The second operation to be performed in order to convert the unit into base value
        /// </summary>
        public ConversionOperation SecondOperation { get; set; }

        /// <summary>
        /// The numeric factor for the second operation
        /// </summary>
        public double SecondFactor { get; set; }

        private static readonly ConcurrentDictionary<string, Type> gTypes = new ConcurrentDictionary<string, Type>();

        internal ICustomConversionOperation ConversionInterface { get; }

        /// <summary>
        /// The constructor to specify the base unit
        /// </summary>
        /// <param name="operation">Must always be <c>ConversionOperation.Base</c> or <c>ConversionOperation.Negate</c></param>
        public ConversionAttribute(ConversionOperation operation) : this(operation, 0, ConversionOperation.None, 0)
        {
            if (operation != ConversionOperation.Base && operation != ConversionOperation.Negate)
                throw new ArgumentException("Operation must be either Base or Negate", nameof(operation));
        }

        /// <summary>
        /// The constructor to specify a custom conversion
        /// </summary>
        /// <param name="operation">Must always be <code>ConversionOperation.Base</code></param>
        /// <param name="name">The full name (namespace + name) of the type that implements <see cref="ICustomConversionOperation">ICustomConversionOperation</see> interface</param>
        public ConversionAttribute(ConversionOperation operation, string name) : this(operation, 0, ConversionOperation.None, 0)
        {
            if (operation != ConversionOperation.Custom)
                throw new ArgumentException("Operation must be Custom", nameof(operation));

            if (!gTypes.TryGetValue(name, out Type type))
            {
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                type = null;
                foreach (var assembly in assemblies)
                {
                    foreach (var type1 in assembly.GetTypes())
                    {
                        if (type1.FullName == name)
                        {
                            type = type1;
                            break;
                        }
                    }
                    if (type != null)
                        break;
                }
                if (type == null)
                    throw new ArgumentException($"Type {name} is not found", nameof(name));
                gTypes.TryAdd(name, type);
            }
            ConversionInterface = Activator.CreateInstance(type) as ICustomConversionOperation;
            if (ConversionInterface == null)
                throw new ArgumentException($"Type {name} does not supprt {nameof(ICustomConversionOperation)}", nameof(name));
        }

        /// <summary>
        /// The constructor to specify one operation conversion
        /// </summary>
        /// <param name="operation">The operation to convert the unit into base unit</param>
        /// <param name="factor">The numeric factor for the operation</param>
        public ConversionAttribute(ConversionOperation operation, double factor) : this(operation, factor, ConversionOperation.None, 0)
        {
        }

        /// <summary>
        /// The constructor to specify two-operation conversion
        /// </summary>
        /// <param name="operation">The first operation to convert the unit into base unit</param>
        /// <param name="factor">The numeric factor for the first operation</param>
        /// <param name="secondOperation">The second operation to convert the unit into base unit</param>
        /// <param name="secondFactor">The numeric factor for the second operation</param>
        public ConversionAttribute(ConversionOperation operation, double factor, ConversionOperation secondOperation, double secondFactor)
        {
            Operation = operation;
            Factor = factor;
            SecondOperation = secondOperation;
            SecondFactor = secondFactor;
        }
    }
}
