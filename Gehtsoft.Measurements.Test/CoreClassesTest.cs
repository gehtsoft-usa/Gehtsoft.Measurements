using Binaron.Serializer;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Xunit;

namespace Gehtsoft.Measurements.Test
{
    public class TestCoreClasses
    {
        [Fact]
        public void List()
        {
            Measurement<TestUnit>.BaseUnit.Should().Be(TestUnit.Base);

            var names = Measurement<TestUnit>.GetUnitNames();
            names.Should().HaveCount(9);

            names.Should().Contain(new Tuple<TestUnit, string>(TestUnit.Base, "n1"));
            names.Should().Contain(new Tuple<TestUnit, string>(TestUnit.Unit1, "\""));
            names.Should().Contain(new Tuple<TestUnit, string>(TestUnit.Unit2, "u2"));
            names.Should().Contain(new Tuple<TestUnit, string>(TestUnit.Unit3, "u3"));
            names.Should().Contain(new Tuple<TestUnit, string>(TestUnit.Unit4, "u4"));
            names.Should().Contain(new Tuple<TestUnit, string>(TestUnit.Unit5, "u5"));
            names.Should().Contain(new Tuple<TestUnit, string>(TestUnit.Unit6, "u6"));
            names.Should().Contain(new Tuple<TestUnit, string>(TestUnit.Unit7, "u7"));
            names.Should().Contain(new Tuple<TestUnit, string>(TestUnit.Unit8, "u8"));
        }

        [Fact]
        public void Parse()
        {
            Measurement<TestUnit>.ParseUnitName("n1").Should().Be(TestUnit.Base);
            Measurement<TestUnit>.ParseUnitName("n2").Should().Be(TestUnit.Base);
            Measurement<TestUnit>.ParseUnitName("u1").Should().Be(TestUnit.Unit1);
            Measurement<TestUnit>.ParseUnitName("\"").Should().Be(TestUnit.Unit1);
            Measurement<TestUnit>.ParseUnitName("u2").Should().Be(TestUnit.Unit2);

            Action action = () => Measurement<TestUnit>.ParseUnitName("unknown");
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void GetName()
        {
            Measurement<TestUnit>.GetUnitName(TestUnit.Base).Should().Be("n1");
            Measurement<TestUnit>.GetUnitName(TestUnit.Unit1).Should().Be("\"");
            Measurement<TestUnit>.GetUnitName(TestUnit.Unit2).Should().Be("u2");
            Measurement<TestUnit>.GetUnitName(TestUnit.Unit7).Should().Be("u7");

            Action action = () => Measurement<TestUnit>.GetUnitName((TestUnit)100);
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void DefaultAccuracy()
        {
            Measurement<TestUnit>.GetUnitDefaultAccuracy(TestUnit.Base).Should().Be(5);
            Measurement<TestUnit>.GetUnitDefaultAccuracy(TestUnit.Unit1).Should().Be(3);
            Measurement<TestUnit>.GetUnitDefaultAccuracy(TestUnit.Unit2).Should().Be(2);

            Action action = () => Measurement<TestUnit>.GetUnitName((TestUnit)100);
            action.Should().Throw<ArgumentException>();
        }

        [Theory]
        [InlineData(10, TestUnit.Base, 10)]
        [InlineData(10, TestUnit.Unit1, 12)]
        [InlineData(10, TestUnit.Unit2, 8)]
        [InlineData(10, TestUnit.Unit3, -8)]
        [InlineData(10, TestUnit.Unit4, 20)]
        [InlineData(10, TestUnit.Unit5, 5)]
        [InlineData(10, TestUnit.Unit6, 0.2)]
        [InlineData(10, TestUnit.Unit7, 24)]
        [InlineData(10, TestUnit.Unit8, -0.8)]
        public void Conversion(double value, TestUnit unit, double expected)
        {
            Measurement<TestUnit>.ToBase(value, unit).Should().BeApproximately(expected, 1e-10);
            Measurement<TestUnit>.FromBase(expected, unit).Should().BeApproximately(value, 1e-10);

            var v1 = new Measurement<TestUnit>(value, unit);
            v1.In(TestUnit.Base).Should().BeApproximately(expected, 1e-10);
            v1.To(TestUnit.Base).Value.Should().BeApproximately(expected, 1e-10);
        }

        [Theory]
        [InlineData(8, TestUnit.Unit2, -4, TestUnit.Unit3)]
        [InlineData(20, TestUnit.Unit4, 38, TestUnit.Unit1)]
        public void Conversion2(double value, TestUnit unit, double expected, TestUnit unit1)
        {
            Measurement<TestUnit>.Convert(value, unit, unit1).Should().BeApproximately(expected, 1e-10);
        }

        [Theory]
        [InlineData(1, TestUnit.Unit1, 1, TestUnit.Unit1, 0)]
        [InlineData(2, TestUnit.Unit1, 1, TestUnit.Unit1, 1)]
        [InlineData(1, TestUnit.Unit1, 2, TestUnit.Unit1, -1)]
        [InlineData(12, TestUnit.Unit2, 5, TestUnit.Unit4, 0)]
        [InlineData(13, TestUnit.Unit2, 5, TestUnit.Unit4, 1)]
        [InlineData(12, TestUnit.Unit2, 6, TestUnit.Unit4, -1)]
        public void Compare(double value1, TestUnit unit1, double value2, TestUnit unit2, int expected)
        {
            var v1 = new Measurement<TestUnit>(value1, unit1);
            var v2 = new Measurement<TestUnit>(value2, unit2);
            int rc = v1.CompareTo(v2);
            if (expected == 0)
            {
                rc.Should().Be(0);
                v1.Should().BeEquivalentTo(v2);
            }
            else if (expected > 0)
            {
                rc.Should().BeGreaterThan(0);
                v1.Equals(v2).Should().BeFalse();
            }
            else if (expected < 0)
            {
                rc.Should().BeLessThan(0);
                v1.Equals(v2).Should().BeFalse();
            }
        }

        [Theory]
        [InlineData(null, "1\"", 1, TestUnit.Unit1)]
        [InlineData(null, "1.\"", 1, TestUnit.Unit1)]
        [InlineData(null, "1.23\"", 1.23, TestUnit.Unit1)]
        [InlineData(null, "-1.23\"", -1.23, TestUnit.Unit1)]
        [InlineData(null, "1,234.23\"", 1234.23, TestUnit.Unit1)]
        [InlineData(null, "1,234\"", 1234, TestUnit.Unit1)]
        [InlineData(null, "1.5u2", 1.5, TestUnit.Unit2)]
        [InlineData(null, "+1.5u2", 1.5, TestUnit.Unit2)]
        [InlineData("ru", "1 234\"", 1234, TestUnit.Unit1)]
        [InlineData("ru", "1234,23\"", 1234.23, TestUnit.Unit1)]
        [InlineData("ru", "1 234,23\"", 1234.23, TestUnit.Unit1)]
        public void TryParse(string culture, string text, double value, TestUnit unit)
        {
            CultureInfo ci = culture == null ? CultureInfo.InvariantCulture : CultureInfo.GetCultures(CultureTypes.AllCultures).First(c => c.Name == culture);
            ci.Should().NotBeNull();
            Measurement<TestUnit>.TryParse(ci, text, out Measurement<TestUnit> o).Should().BeTrue();
            o.Value.Should().BeApproximately(value, 1e-10);
            o.Unit.Should().Be(unit);
        }

        [Theory]
        [InlineData("1\"", 1, TestUnit.Unit1)]
        [InlineData("1.2345678u2", 1.2345678, TestUnit.Unit2)]
        public void ParsingConstructor(string text, double value, TestUnit unit)
        {
            var m = new Measurement<TestUnit>(text);
            m.Value.Should().BeApproximately(value, 1e-10);
            m.Unit.Should().Be(unit);
        }

        [Theory]
        [InlineData("1\"", 1, TestUnit.Unit1)]
        [InlineData("1.2345678u2", 1.2345678, TestUnit.Unit2)]
        public void ConversionToString1(string text, double value, TestUnit unit)
        {
            var m = new Measurement<TestUnit>(value, unit);
            m.ToString(CultureInfo.InvariantCulture).Should().Be(text);
        }

        [Theory]
        [InlineData("1.23456789123456u2", "NF", 1.23456789123456, TestUnit.Unit2)]
        [InlineData("1.23u2", "ND", 1.2345678, TestUnit.Unit2)]
        [InlineData("1.2346u2", "N4", 1.2345678, TestUnit.Unit2)]
        public void ConversionToString2(string text, string format, double value, TestUnit unit)
        {
            var m = new Measurement<TestUnit>(value, unit);
            m.ToString(format, CultureInfo.InvariantCulture).Should().Be(text);
        }

        [XmlRoot("container")]
        public class XmlContainerForMeasurement
        {
            [XmlIgnore]
            public Measurement<DistanceUnit> Value { get; set; }

            [XmlAttribute("value")]
            public string XmlValue
            {
                get => Value.Text;
                set => Value = new Measurement<DistanceUnit>(value);
            }
        }

        [Fact]
        public void SerializationXml()
        {
            var serializer = new XmlSerializer(typeof(XmlContainerForMeasurement));
            var container = new XmlContainerForMeasurement() { Value = new Measurement<DistanceUnit>(15, DistanceUnit.Centimeter) };
            using var sw = new StringWriter();
            serializer.Serialize(sw, container);
            string xml = sw.ToString();

            using var sr = new StringReader(xml);
            var container1 = serializer.Deserialize(sr) as XmlContainerForMeasurement;
            container1.Should().NotBeNull();
            container1.Value.Should().Be(new Measurement<DistanceUnit>(15, DistanceUnit.Centimeter));
        }

        [Fact]
        public void SerializationJson()
        {
            var v = new Measurement<TestUnit>(1245.78912345, TestUnit.Unit1);
            string s = JsonSerializer.Serialize(v);
            s.Should().Be("{\"value\":\"1245.78912345\\u0022\"}");
            var v1 = JsonSerializer.Deserialize<Measurement<TestUnit>>(s);
            v1.Value.Should().Be(1245.78912345);
            v1.Unit.Should().Be(TestUnit.Unit1);
        }

        [Fact]
        public void SerializationBinaron()
        {
            var v = new Measurement<TestUnit>(1245.78912345, TestUnit.Unit1);
            byte[] arr;

            using (var ms = new MemoryStream())
            {
                BinaronConvert.Serialize<Measurement<TestUnit>>(v, ms);
                arr = ms.ToArray();
            }

            using (var ms = new MemoryStream(arr))
            {
                var v1 = BinaronConvert.Deserialize<Measurement<TestUnit>>(ms);
                v1.Value.Should().Be(1245.78912345);
                v1.Unit.Should().Be(TestUnit.Unit1);
            }
        }

        [Theory]
        [InlineData(typeof(AccelerationUnit))]
        [InlineData(typeof(AngularUnit))]
        [InlineData(typeof(DensityUnit))]
        [InlineData(typeof(DistanceUnit))]
        [InlineData(typeof(EnergyUnit))]
        [InlineData(typeof(ForceUnit))]
        [InlineData(typeof(PowerUnit))]
        [InlineData(typeof(PressureUnit))]
        [InlineData(typeof(TemperatureUnit))]
        [InlineData(typeof(VelocityUnit))]
        [InlineData(typeof(VolumeUnit))]
        [InlineData(typeof(WeightUnit))]
        public void TestConversion(Type type)
        {
            Type generic = typeof(Measurement<>);
            Type measureType = generic.MakeGenericType(new Type[] { type });

            Array units = (Array)measureType.GetMethod(nameof(Measurement<DistanceUnit>.GetUnitNames)).Invoke(null, null);
            units.Should().NotBeNullOrEmpty();
            int i = 0;
            foreach (object t in units)
            {
                object x = t.GetType().GetProperty("Item1").GetValue(t);
                object n = t.GetType().GetProperty("Item2").GetValue(t);

                x.GetType().Should().Be(type);
                n.GetType().Should().Be(typeof(string));

                object v = Activator.CreateInstance(measureType, new object[] { 123.0, x });

                measureType.GetField (nameof(Measurement<DistanceUnit>.Value)).GetValue(v).Should().Be(123);
                measureType.GetField(nameof(Measurement<DistanceUnit>.Unit)).GetValue(v).Should().Be(x);

                string s = v.ToString();
                s.Should().StartWith("123");
                s.Should().EndWith(n as string);

                object v1 = Activator.CreateInstance(measureType, new object[] { s });
                measureType.GetField(nameof(Measurement<DistanceUnit>.Value)).GetValue(v1).Should().Be(123);
                measureType.GetField(nameof(Measurement<DistanceUnit>.Unit)).GetValue(v1).Should().Be(x);

                i++;
            }
            i.Should().BeGreaterThan(2);
        }

        [Fact]

        public void Formattable()
        {
            var v = new Measurement<DistanceUnit>(1.2345678, DistanceUnit.Meter);
            $"{v}".Should().Be("1.2345678m");
            $"{v:ND}".Should().Be("1.2m");
            $"{v:N2}".Should().Be("1.23m");
            $"{v:N4}".Should().Be("1.2346m");
        }
    }
}


