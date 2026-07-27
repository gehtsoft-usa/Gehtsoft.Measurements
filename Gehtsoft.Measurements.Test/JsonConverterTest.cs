using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using AwesomeAssertions;
using Xunit;

namespace Gehtsoft.Measurements.Test
{
    /// <summary>
    /// The opt-in converter which serializes a measurement as a plain string.
    /// </summary>
    public class JsonConverterTest
    {
        private static JsonSerializerOptions Compact()
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new MeasurementJsonConverter());
            return options;
        }

        private class Shot
        {
            public Measurement<DistanceUnit> Range { get; set; }
            public DecimalMeasurement<WeightUnit> Bullet { get; set; }
        }

        private class Annotated
        {
            [JsonConverter(typeof(MeasurementJsonConverter))]
            public Measurement<DistanceUnit> Range { get; set; }
        }

        [Fact]
        public void WritesABareString()
        {
            var value = new Measurement<DistanceUnit>(10.5, DistanceUnit.Inch);

            JsonSerializer.Serialize(value, Compact()).Should().Be("\"10.5in\"");
        }

        [Fact]
        public void ReadsABareString()
        {
            var value = JsonSerializer.Deserialize<Measurement<DistanceUnit>>("\"10.5in\"", Compact());

            value.Should().Be(new Measurement<DistanceUnit>(10.5, DistanceUnit.Inch));
            value.Unit.Should().Be(DistanceUnit.Inch);
        }

        [Fact]
        public void RoundTripsBothMeasurementTypes()
        {
            var options = Compact();
            var shot = new Shot
            {
                Range = new Measurement<DistanceUnit>(300, DistanceUnit.Yard),
                Bullet = new DecimalMeasurement<WeightUnit>(168, WeightUnit.Grain),
            };

            string json = JsonSerializer.Serialize(shot, options);
            json.Should().Be("{\"Range\":\"300yd\",\"Bullet\":\"168gr\"}");

            var restored = JsonSerializer.Deserialize<Shot>(json, options);
            restored.Range.Should().Be(shot.Range);
            restored.Range.Unit.Should().Be(DistanceUnit.Yard);
            restored.Bullet.Should().Be(shot.Bullet);
            restored.Bullet.Unit.Should().Be(WeightUnit.Grain);
        }

        [Fact]
        public void WorksThroughTheConverterAttribute()
        {
            var value = new Annotated { Range = new Measurement<DistanceUnit>(1, DistanceUnit.Meter) };

            string json = JsonSerializer.Serialize(value);
            json.Should().Be("{\"Range\":\"1m\"}");

            JsonSerializer.Deserialize<Annotated>(json).Range.Should().Be(value.Range);
        }

        [Fact]
        public void WorksForCollections()
        {
            var options = Compact();
            var values = new[]
            {
                new Measurement<DistanceUnit>(1, DistanceUnit.Foot),
                new Measurement<DistanceUnit>(6, DistanceUnit.Inch),
            };

            string json = JsonSerializer.Serialize(values, options);
            json.Should().Be("[\"1ft\",\"6in\"]");

            JsonSerializer.Deserialize<Measurement<DistanceUnit>[]>(json, options).Should().Equal(values);
        }

        [Fact]
        public void WorksAsADictionaryValue()
        {
            var options = Compact();
            var map = new Dictionary<string, Measurement<DistanceUnit>>
            {
                ["range"] = new Measurement<DistanceUnit>(100, DistanceUnit.Yard),
            };

            string json = JsonSerializer.Serialize(map, options);
            json.Should().Be("{\"range\":\"100yd\"}");

            JsonSerializer.Deserialize<Dictionary<string, Measurement<DistanceUnit>>>(json, options)["range"]
                          .Should().Be(map["range"]);
        }

        /// <summary>
        /// The default shape must not change for anyone who does not register the converter.
        /// </summary>
        [Fact]
        public void TheDefaultShapeIsUntouched()
        {
            var value = new Measurement<DistanceUnit>(10.5, DistanceUnit.Inch);

            JsonSerializer.Serialize(value).Should().Be("{\"value\":\"10.5in\"}");
            JsonSerializer.Deserialize<Measurement<DistanceUnit>>("{\"value\":\"10.5in\"}").Should().Be(value);
        }

        [Theory]
        [InlineData("\"nonsense\"")]
        [InlineData("\"10.5unknown\"")]
        [InlineData("12")]
        [InlineData("null")]
        [InlineData("{}")]
        public void InvalidJsonIsRejected(string json)
        {
            Action read = () => JsonSerializer.Deserialize<Measurement<DistanceUnit>>(json, Compact());
            read.Should().Throw<JsonException>();
        }

        /// <summary>
        /// A value which does not fit the buffer of the fast path still round-trips.
        /// </summary>
        [Fact]
        public void HugeValueRoundTrips()
        {
            var options = Compact();
            var value = new Measurement<DistanceUnit>(double.MaxValue, DistanceUnit.Inch);

            string json = JsonSerializer.Serialize(value, options);
            JsonSerializer.Deserialize<Measurement<DistanceUnit>>(json, options).Value.Should().Be(double.MaxValue);
        }

        [Fact]
        public void TheFactoryRejectsAnUnrelatedType()
        {
            var factory = new MeasurementJsonConverter();

            factory.CanConvert(typeof(string)).Should().BeFalse();
            factory.CanConvert(typeof(int)).Should().BeFalse();
            factory.CanConvert(typeof(List<int>)).Should().BeFalse();
            factory.CanConvert(typeof(Measurement<DistanceUnit>)).Should().BeTrue();
            factory.CanConvert(typeof(DecimalMeasurement<DistanceUnit>)).Should().BeTrue();
        }
    }
}
