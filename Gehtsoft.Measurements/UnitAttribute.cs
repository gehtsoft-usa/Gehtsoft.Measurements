using System;
using System.Collections.Generic;
using System.Text;

namespace Gehtsoft.Measurements
{

    [AttributeUsage(validOn: AttributeTargets.Field)]
    internal class UnitAttribute : Attribute
    {
        public string Name { get; set; }
        public string AlterantiveName { get; set; }
        public bool HasAlternativeName => !string.IsNullOrEmpty(AlterantiveName);
        public int DefaultAccuracy { get; set; }

        public UnitAttribute(string name, int defaultAccuracy)
        {
            Name = name;
            DefaultAccuracy = defaultAccuracy;
        }
        public UnitAttribute(string name, string alternativeName, int defaultAccuracy)
        {
            Name = name;
            AlterantiveName = alternativeName;
            DefaultAccuracy = defaultAccuracy;
        }
    }
}
