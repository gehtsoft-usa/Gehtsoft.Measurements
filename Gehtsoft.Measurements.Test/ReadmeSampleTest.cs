using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using AwesomeAssertions;
using Xunit;

namespace Gehtsoft.Measurements.Test
{
    /// <summary>
    /// The code samples of README.md, so that they keep compiling and keep being true.
    /// </summary>
    /// <remarks>
    /// The README used to open with `new Measurement&lt;DistanceUnit&gt;(10, DistanceUnit.Feet)`
    /// and to call `ToString("N3")`. Neither compiled - the unit is named `Foot` and the single
    /// argument format overload did not exist. Nothing checked the documentation, so nothing
    /// said so. Keep this in step with the README.
    /// </remarks>
    public class ReadmeSampleTest
    {
        private enum MyWeightUnit
        {
            //1 gram
            [Unit("g", 3)]
            [Conversion(ConversionOperation.Base)]
            Gram,

            //1 kilogram (1 kilogram = 1000 gram)
            [Unit("kg", 3)]
            [Conversion(ConversionOperation.Multiply, 1000)]
            Kilogram,
        }

        [Fact]
        public void UsingTheLibrary()
        {
            var v = new Measurement<DistanceUnit>(10, DistanceUnit.Foot);

            var v1 = v * 2;
            string text = v.ToString("N3");
            var v2 = v1.To(DistanceUnit.Meter);

            v1.Should().Be(new Measurement<DistanceUnit>(20, DistanceUnit.Foot));
            text.Should().Be("10.000ft");
            v2.In(DistanceUnit.Foot).Should().BeApproximately(20, 1e-9);

            var x = (10.As(DistanceUnit.Yard) + 36.As(DistanceUnit.Inch)).To(DistanceUnit.Meter);
            x.In(DistanceUnit.Yard).Should().BeApproximately(11, 1e-9);

            (1.As(DistanceUnit.Foot) == 12.As(DistanceUnit.Inch)).Should().BeTrue();
        }

        [Fact]
        public void ParsingAndFormatting()
        {
            var v = Measurement<DistanceUnit>.Parse("10.5in", CultureInfo.InvariantCulture);
            v.Should().Be(new Measurement<DistanceUnit>(10.5, DistanceUnit.Inch));

            Measurement<DistanceUnit>.TryParse(CultureInfo.InvariantCulture, "1e3m", out var thousandMeters).Should().BeTrue();
            thousandMeters.Should().Be(new Measurement<DistanceUnit>(1000, DistanceUnit.Meter));

            const string line = "range=300yd;";
            ReadOnlySpan<char> field = line.AsSpan(6, 5);
            Measurement<DistanceUnit>.TryParse(CultureInfo.InvariantCulture, field, out var range).Should().BeTrue();
            range.Should().Be(new Measurement<DistanceUnit>(300, DistanceUnit.Yard));

            Span<char> buffer = stackalloc char[64];
            range.TryFormat(buffer, out int written, "ND", CultureInfo.InvariantCulture).Should().BeTrue();
            new string(buffer.Slice(0, written)).Should().Be("300.00yd");
        }

        [Fact]
        public void AggregatingAndComparing()
        {
            var ranges = new[] { 100.As(DistanceUnit.Yard), 300.As(DistanceUnit.Yard) };

            ranges.Sum().Should().Be(400.As(DistanceUnit.Yard));
            ranges.Average().Should().Be(200.As(DistanceUnit.Yard));

            var power = MeasurementMath.Power(200.As(TorqueUnit.NewtonMeter), 3000.As(RotationalSpeedUnit.RevolutionsPerMinute));
            power.In(PowerUnit.Kilowatt).Should().BeApproximately(62.8318530717958, 1e-9);

            var force = MeasurementMath.Force(10.As(WeightUnit.Kilogram), 1.As(AccelerationUnit.EarthGravity));
            force.Value.Should().BeApproximately(98.0665, 1e-9);

            var energy = MeasurementMath.KineticEnergy(168.As(WeightUnit.Grain), 2700.As(VelocityUnit.FeetPerSecond));
            energy.Unit.Should().Be(EnergyUnit.Joule);
            energy.Value.Should().BeApproximately(3686.4168, 1e-3);
        }

        [Fact]
        public void Serialization()
        {
            var value = new { range = 300.As(DistanceUnit.Yard) };

            JsonSerializer.Serialize(value).Should().Be("{\"range\":{\"value\":\"300yd\"}}");

            var options = new JsonSerializerOptions();
            options.Converters.Add(new MeasurementJsonConverter());

            JsonSerializer.Serialize(value, options).Should().Be("{\"range\":\"300yd\"}");
        }

        [Fact]
        public void DefiningYourOwnUnits()
        {
            var weight = new Measurement<MyWeightUnit>(2, MyWeightUnit.Kilogram);

            weight.In(MyWeightUnit.Gram).Should().BeApproximately(2000, 1e-9);

            // the parameterless ToString uses "NF", which is the general numeric format
            weight.To(MyWeightUnit.Gram).ToString().Should().Be("2000g");

            // the accuracy declared by the unit is applied by "ND"
            weight.To(MyWeightUnit.Gram).ToString("ND").Should().Be("2,000.000g");
        }
    }
}
