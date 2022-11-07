using Binaron.Serializer;
using FluentAssertions;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Xml.Serialization;
using Xunit;

namespace Gehtsoft.Measurements.Test
{
    public class DecimalCoreClassesTest
    {
        [Fact]
        public void List()
        {
            DecimalMeasurement<TestUnit>.BaseUnit.Should().Be(TestUnit.Base);

            var names = DecimalMeasurement<TestUnit>.GetUnitNames();
            names.Should().HaveCount(10);

            names.Should().Contain(new Tuple<TestUnit, string>(TestUnit.Base, "n1"));
            names.Should().Contain(new Tuple<TestUnit, string>(TestUnit.Unit1, "\""));
            names.Should().Contain(new Tuple<TestUnit, string>(TestUnit.Unit2, "u2"));
            names.Should().Contain(new Tuple<TestUnit, string>(TestUnit.Unit3, "u3"));
            names.Should().Contain(new Tuple<TestUnit, string>(TestUnit.Unit4, "u4"));
            names.Should().Contain(new Tuple<TestUnit, string>(TestUnit.Unit5, "u5"));
            names.Should().Contain(new Tuple<TestUnit, string>(TestUnit.Unit6, "u6"));
            names.Should().Contain(new Tuple<TestUnit, string>(TestUnit.Unit7, "u7"));
            names.Should().Contain(new Tuple<TestUnit, string>(TestUnit.Unit8, "u8"));
            names.Should().Contain(new Tuple<TestUnit, string>(TestUnit.Unit9, "u9"));
        }

        [Fact]
        public void Parse()
        {
            DecimalMeasurement<TestUnit>.ParseUnitName("n1").Should().Be(TestUnit.Base);
            DecimalMeasurement<TestUnit>.ParseUnitName("n2").Should().Be(TestUnit.Base);
            DecimalMeasurement<TestUnit>.ParseUnitName("u1").Should().Be(TestUnit.Unit1);
            DecimalMeasurement<TestUnit>.ParseUnitName("\"").Should().Be(TestUnit.Unit1);
            DecimalMeasurement<TestUnit>.ParseUnitName("u2").Should().Be(TestUnit.Unit2);

            Action action = () => DecimalMeasurement<TestUnit>.ParseUnitName("unknown");
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void GetName()
        {
            DecimalMeasurement<TestUnit>.GetUnitName(TestUnit.Base).Should().Be("n1");
            DecimalMeasurement<TestUnit>.GetUnitName(TestUnit.Unit1).Should().Be("\"");
            DecimalMeasurement<TestUnit>.GetUnitName(TestUnit.Unit2).Should().Be("u2");
            DecimalMeasurement<TestUnit>.GetUnitName(TestUnit.Unit7).Should().Be("u7");

            Action action = () => DecimalMeasurement<TestUnit>.GetUnitName((TestUnit)100);
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void DefaultAccuracy()
        {
            DecimalMeasurement<TestUnit>.GetUnitDefaultAccuracy(TestUnit.Base).Should().Be(5);
            DecimalMeasurement<TestUnit>.GetUnitDefaultAccuracy(TestUnit.Unit1).Should().Be(3);
            DecimalMeasurement<TestUnit>.GetUnitDefaultAccuracy(TestUnit.Unit2).Should().Be(2);

            Action action = () => DecimalMeasurement<TestUnit>.GetUnitName((TestUnit)100);
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
        [InlineData(10, TestUnit.Unit9, -0.8)]
        public void Conversion_Method1(decimal value, TestUnit unit, decimal expected)
        {
            DecimalMeasurement<TestUnit>.ToBase(value, unit).Should().BeApproximately(expected, 1e-10m);
            DecimalMeasurement<TestUnit>.FromBase(expected, unit).Should().BeApproximately(value, 1e-10m);

            var v1 = new DecimalMeasurement<TestUnit>(value, unit);
            v1.In(TestUnit.Base).Should().BeApproximately(expected, 1e-10m);
            v1.To(TestUnit.Base).Value.Should().BeApproximately(expected, 1e-10m);
        }

        [Theory]
        [InlineData(8, TestUnit.Unit2, -4, TestUnit.Unit3)]
        [InlineData(20, TestUnit.Unit4, 38, TestUnit.Unit1)]
        [InlineData(10, TestUnit.Unit8, 10, TestUnit.Unit9)]
        [InlineData(10, TestUnit.Unit9, 10, TestUnit.Unit8)]
        public void Conversion_Method2(decimal value, TestUnit unit, decimal expected, TestUnit unit1)
        {
            DecimalMeasurement<TestUnit>.Convert(value, unit, unit1).Should().BeApproximately(expected, 1e-10m);
        }

        [Theory]
        [InlineData(1, TestUnit.Unit1, 1, TestUnit.Unit1, 0)]
        [InlineData(2, TestUnit.Unit1, 1, TestUnit.Unit1, 1)]
        [InlineData(1, TestUnit.Unit1, 2, TestUnit.Unit1, -1)]
        [InlineData(12, TestUnit.Unit2, 5, TestUnit.Unit4, 0)]
        [InlineData(13, TestUnit.Unit2, 5, TestUnit.Unit4, 1)]
        [InlineData(12, TestUnit.Unit2, 6, TestUnit.Unit4, -1)]
        public void Compare(decimal value1, TestUnit unit1, decimal value2, TestUnit unit2, int expected)
        {
            var v1 = new DecimalMeasurement<TestUnit>(value1, unit1);
            var v2 = new DecimalMeasurement<TestUnit>(value2, unit2);
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
        public void TryParse(string culture, string text, decimal value, TestUnit unit)
        {
            CultureInfo ci = culture == null ? CultureInfo.InvariantCulture : CultureInfo.GetCultures(CultureTypes.AllCultures).First(c => c.Name == culture);
            ci.Should().NotBeNull();
            DecimalMeasurement<TestUnit>.TryParse(ci, text, out DecimalMeasurement<TestUnit> o).Should().BeTrue();
            o.Value.Should().BeApproximately(value, 1e-10m);
            o.Unit.Should().Be(unit);
        }

        [Theory]
        [InlineData("1\"", 1, TestUnit.Unit1)]
        [InlineData("1.2345678u2", 1.2345678, TestUnit.Unit2)]
        public void ParsingConstructor(string text, decimal value, TestUnit unit)
        {
            var m = new DecimalMeasurement<TestUnit>(text);
            m.Value.Should().BeApproximately(value, 1e-10m);
            m.Unit.Should().Be(unit);
        }

        [Theory]
        [InlineData("1\"", 1, TestUnit.Unit1)]
        [InlineData("1.2345678u2", 1.2345678, TestUnit.Unit2)]
        public void ConversionToString1(string text, decimal value, TestUnit unit)
        {
            var m = new DecimalMeasurement<TestUnit>(value, unit);
            m.ToString(CultureInfo.InvariantCulture).Should().Be(text);
        }

        [Theory]
        [InlineData("1.23456789123456u2", "NF", 1.23456789123456, TestUnit.Unit2)]
        [InlineData("1.23u2", "ND", 1.2345678, TestUnit.Unit2)]
        [InlineData("1.2346u2", "N4", 1.2345678, TestUnit.Unit2)]
        public void ConversionToString2(string text, string format, decimal value, TestUnit unit)
        {
            var m = new DecimalMeasurement<TestUnit>(value, unit);
            m.ToString(format, CultureInfo.InvariantCulture).Should().Be(text);
        }

        [XmlRoot("container")]
        public class XmlContainerForMeasurement
        {
            [XmlIgnore]
            public DecimalMeasurement<DistanceUnit> Value { get; set; }

            [XmlAttribute("value")]
            public string XmlValue
            {
                get => Value.Text;
                set => Value = new DecimalMeasurement<DistanceUnit>(value);
            }
        }

        [Fact]
        public void SerializationXml()
        {
            var serializer = new XmlSerializer(typeof(XmlContainerForMeasurement));
            var container = new XmlContainerForMeasurement() { Value = new DecimalMeasurement<DistanceUnit>(15, DistanceUnit.Centimeter) };
            using var sw = new StringWriter();
            serializer.Serialize(sw, container);
            string xml = sw.ToString();

            using var sr = new StringReader(xml);
            var container1 = serializer.Deserialize(sr) as XmlContainerForMeasurement;
            container1.Should().NotBeNull();
            container1.Value.Should().Be(new DecimalMeasurement<DistanceUnit>(15, DistanceUnit.Centimeter));
        }

        [Fact]
        public void SerializationJson()
        {
            var v = new DecimalMeasurement<TestUnit>(1245.78912345m, TestUnit.Unit1);
            string s = JsonSerializer.Serialize(v);
            s.Should().Be("{\"value\":\"1245.78912345\\u0022\"}");
            var v1 = JsonSerializer.Deserialize<DecimalMeasurement<TestUnit>>(s);
            v1.Value.Should().Be(1245.78912345m);
            v1.Unit.Should().Be(TestUnit.Unit1);
        }

        [Fact]
        public void SerializationBinaron()
        {
            var v = new DecimalMeasurement<TestUnit>(1245.78912345m, TestUnit.Unit1);
            byte[] arr;

            using (var ms = new MemoryStream())
            {
                BinaronConvert.Serialize<DecimalMeasurement<TestUnit>>(v, ms);
                arr = ms.ToArray();
            }

            using (var ms = new MemoryStream(arr))
            {
                var v1 = BinaronConvert.Deserialize<DecimalMeasurement<TestUnit>>(ms);
                v1.Value.Should().Be(1245.78912345m);
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

            Array units = (Array)measureType.GetMethod(nameof(DecimalMeasurement<DistanceUnit>.GetUnitNames)).Invoke(null, null);
            units.Should().NotBeNull();
            units.Length.Should().NotBe(0);
            int i = 0;
            foreach (object t in units)
            {
                object x = t.GetType().GetProperty("Item1").GetValue(t);
                object n = t.GetType().GetProperty("Item2").GetValue(t);

                x.GetType().Should().Be(type);
                n.GetType().Should().Be(typeof(string));

                object v = Activator.CreateInstance(measureType, new object[] { 123.0, x });

                measureType.GetField(nameof(DecimalMeasurement<DistanceUnit>.Value)).GetValue(v).Should().Be(123);
                measureType.GetField(nameof(DecimalMeasurement<DistanceUnit>.Unit)).GetValue(v).Should().Be(x);

                string s = v.ToString();
                s.Should().StartWith("123");
                s.Should().EndWith(n as string);

                object v1 = Activator.CreateInstance(measureType, new object[] { s });
                measureType.GetField(nameof(DecimalMeasurement<DistanceUnit>.Value)).GetValue(v1).Should().Be(123);
                measureType.GetField(nameof(DecimalMeasurement<DistanceUnit>.Unit)).GetValue(v1).Should().Be(x);

                i++;
            }
            i.Should().BeGreaterThan(2);
        }

        [Fact]

        public void Formattable()
        {
            var v = new DecimalMeasurement<DistanceUnit>(1.2345678m, DistanceUnit.Meter);
            $"{v}".Should().Be("1.2345678m");
            $"{v:ND}".Should().Be("1.2m");
            $"{v:N2}".Should().Be("1.23m");
            $"{v:N4}".Should().Be("1.2346m");
        }

        [Fact]
        public void NewUnitExtension()
        {
            var u1 = AngularUnit.MOA.NewDecimal(25);
            u1.Should().Be(new DecimalMeasurement<AngularUnit>(25, AngularUnit.MOA));

            var u2 = DistanceUnit.Point.NewDecimal(1.23456m);
            u2.Value.Should().Be(1.23456m);
            u2.Unit.Should().Be(DistanceUnit.Point);
        }

        [Fact]
        public void TupleTest()
        {
            var t1 = new Tuple<decimal, AngularUnit>(15, AngularUnit.MOA);

            var u1 = new DecimalMeasurement<AngularUnit>(t1);
            u1.Should().Be(AngularUnit.MOA.NewDecimal(15));

            var u2 = (DecimalMeasurement<AngularUnit>)t1;
            u2.Should().Be(AngularUnit.MOA.NewDecimal(15));

            var t2 = (Tuple<decimal, AngularUnit>)u1;
            t2.Should().Be(new Tuple<decimal, AngularUnit>(15, AngularUnit.MOA));

            (decimal a, AngularUnit b) t3 = (10.0m, AngularUnit.MOA);

            var u3 = new DecimalMeasurement<AngularUnit>(t3);
            u3.Value.Should().Be(t3.a);
            u3.Unit.Should().Be(t3.b);

            (decimal x, AngularUnit y) t4 = u3;
            t4.x.Should().Be(10);
            t4.y.Should().Be(AngularUnit.MOA);
        }
    }
}


