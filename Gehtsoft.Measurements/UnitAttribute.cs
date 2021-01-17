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
        /// <summary>
        /// The primary name of the unit
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The alternative name of the unit
        /// 
        /// An example of two names if inch, that may be records as 5in as well as 5".
        /// </summary>
        public string AlterantiveName { get; set; }

        /// <summary>
        /// The flag indicating whether the unit has alternative name
        /// </summary>
        public bool HasAlternativeName => !string.IsNullOrEmpty(AlterantiveName);

        /// <summary>
        /// The default accuracy (number of digits after decimal point). 
        /// </summary>
        public int DefaultAccuracy { get; set; }

        /// <summary>
        /// Constructor for the unit that has one name
        /// </summary>
        /// <param name="name">The name of the unit</param>
        /// <param name="defaultAccuracy">The default accuracy (number of digits after decimal point)</param>
        public UnitAttribute(string name, int defaultAccuracy)
        {
            Name = name;
            DefaultAccuracy = defaultAccuracy;
        }

        /// <summary>
        /// Constructor for the unit that has more than one names
        /// </summary>
        /// <param name="name">The name of the unit</param>
        /// <param name="alternativeName">The alternative name (e.g. 5in and 5" are two options to specify inches)</param>
        /// <param name="defaultAccuracy">The default accuracy (number of digits after decimal point)</param>
        public UnitAttribute(string name, string alternativeName, int defaultAccuracy)
        {
            Name = name;
            AlterantiveName = alternativeName;
            DefaultAccuracy = defaultAccuracy;
        }
    }
}
