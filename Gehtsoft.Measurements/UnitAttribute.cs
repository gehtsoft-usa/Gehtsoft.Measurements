using System;
using System.Collections.Generic;
using System.Text;

namespace Gehtsoft.Measurements
{
    /// <summary>
    /// The attribute to specify the name for the measurmement unit.
    /// 
    ///  The attribute is applied on enum fields which describe one unit of the measurement (e.g. "meter")
    /// </summary>
    [AttributeUsage(validOn: AttributeTargets.Field)]
    public class UnitAttribute : Attribute
    {
        public string Name { get; set; }
        public string AlterantiveName { get; set; }
        public bool HasAlternativeName => !string.IsNullOrEmpty(AlterantiveName);
        public int DefaultAccuracy { get; set; }

        /// <summary>
        /// Constructor for the unit that has one name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="defaultAccuracy"></param>
        public UnitAttribute(string name, int defaultAccuracy)
        {
            Name = name;
            DefaultAccuracy = defaultAccuracy;
        }

        /// <summary>
        /// Constructor for the unit that has more than one names
        /// </summary>
        /// <param name="name"></param>
        /// <param name="defaultAccuracy"></param>
        public UnitAttribute(string name, string alternativeName, int defaultAccuracy)
        {
            Name = name;
            AlterantiveName = alternativeName;
            DefaultAccuracy = defaultAccuracy;
        }
    }
}
