using FluentAssertions;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.Dynamic;
using System.IO;
using Binaron.Serializer;
using System.Xml.Serialization;

namespace Gehtsoft.Measurements.Test
{
    public class DistanceTest
    {
        [Theory]
        [InlineData(1, DistanceUnit.Inch, 2.54, DistanceUnit.Centimeter)]
        [InlineData(10, DistanceUnit.RussianLine, 1, DistanceUnit.Inch)]
        [InlineData(12, DistanceUnit.Line, 1, DistanceUnit.Inch)]
        [InlineData(1, DistanceUnit.Foot, 12, DistanceUnit.Inch)]
        [InlineData(1, DistanceUnit.Yard, 36, DistanceUnit.Inch)]
        [InlineData(10, DistanceUnit.RussianLine, 2.54, DistanceUnit.Centimeter)]
        [InlineData(1, DistanceUnit.Millimeter, 0.1, DistanceUnit.Centimeter)]
        [InlineData(1, DistanceUnit.Millimeter, 0.001, DistanceUnit.Meter)]
        [InlineData(1760, DistanceUnit.Yard, 1, DistanceUnit.Mile)]
        [InlineData(1, DistanceUnit.NauticalMile, 1852, DistanceUnit.Meter)]
        [InlineData(11, DistanceUnit.Point, 3.88055555556, DistanceUnit.Millimeter)]
        [InlineData(11, DistanceUnit.Pica, 46.56666666667, DistanceUnit.Millimeter)]
        public void Conversion(double value, DistanceUnit unit, double expected, DistanceUnit targetUnit)
        {
            var v = new Measurement<DistanceUnit>(value, unit);
            v.In(targetUnit).Should().BeApproximately(expected, 1e-10);
        }
    }
}
