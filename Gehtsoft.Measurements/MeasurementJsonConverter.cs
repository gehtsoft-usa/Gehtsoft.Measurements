using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Gehtsoft.Measurements
{
    /// <summary>
    /// Serializes a measurement as a plain string such as `"10.5in"`.
    /// </summary>
    /// <remarks>
    /// <para>
    /// By default a measurement is serialized as an object with a single `value` property,
    /// which is `{"value":"10.5in"}`. That shape is kept for compatibility. Registering this
    /// converter replaces it with the bare string `"10.5in"`, which is usually what an API
    /// contract calls for.
    /// </para>
    /// <para>
    /// The converter handles both measurement types and any unit enumeration. Register it once
    /// for the whole serializer, or apply it to a single property.
    /// </para>
    /// <para>
    /// The text is always written and read in the invariant culture, exactly as the `Text`
    /// property and the text constructor do, so the value round-trips on any machine.
    /// </para>
    /// </remarks>
    /// <example>
    /// @code
    /// var options = new JsonSerializerOptions();
    /// options.Converters.Add(new MeasurementJsonConverter());
    /// string json = JsonSerializer.Serialize(new Measurement&lt;DistanceUnit&gt;(10.5, DistanceUnit.Inch), options);
    /// @endcode
    /// </example>
    [RequiresDynamicCode("The converter for a measurement is a generic type closed over the unit enumeration at run time, which ahead-of-time compilation cannot generate in advance.")]
    [RequiresUnreferencedCode("The converter for a measurement is built by reflection over the unit enumeration, which trimming can remove.")]
    public sealed class MeasurementJsonConverter : JsonConverterFactory
    {
        /// <summary>
        /// Checks whether the type is a measurement.
        /// </summary>
        /// <param name="typeToConvert"></param>
        /// <returns></returns>
        public override bool CanConvert(Type typeToConvert)
        {
            if (typeToConvert == null || !typeToConvert.IsGenericType)
                return false;

            Type definition = typeToConvert.GetGenericTypeDefinition();
            return definition == typeof(Measurement<>) || definition == typeof(DecimalMeasurement<>);
        }

        /// <summary>
        /// Creates the converter for the specified measurement type.
        /// </summary>
        /// <param name="typeToConvert"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            if (!CanConvert(typeToConvert))
                throw new ArgumentException($"The type {typeToConvert} is not a measurement", nameof(typeToConvert));

            Type unitType = typeToConvert.GetGenericArguments()[0];
            Type converterType = typeToConvert.GetGenericTypeDefinition() == typeof(Measurement<>)
                                     ? typeof(DoubleConverter<>).MakeGenericType(unitType)
                                     : typeof(DecimalConverter<>).MakeGenericType(unitType);

            return (JsonConverter)Activator.CreateInstance(converterType);
        }

        private sealed class DoubleConverter<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] T> : JsonConverter<Measurement<T>>
            where T : Enum
        {
            public override Measurement<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType != JsonTokenType.String)
                    throw new JsonException($"A measurement of {typeof(T).Name} must be a string, found {reader.TokenType}");

                string text = reader.GetString();
                if (!Measurement<T>.TryParse(text, CultureInfo.InvariantCulture, out Measurement<T> value))
                    throw new JsonException($"'{text}' is not a valid measurement of {typeof(T).Name}");

                return value;
            }

            public override void Write(Utf8JsonWriter writer, Measurement<T> value, JsonSerializerOptions options)
            {
                if (writer == null)
                    throw new ArgumentNullException(nameof(writer));

                Span<char> buffer = stackalloc char[256];
                if (value.TryFormat(buffer, out int written, "NF".AsSpan(), CultureInfo.InvariantCulture))
                    writer.WriteStringValue(buffer.Slice(0, written));
                else
                    writer.WriteStringValue(value.ToString("NF", CultureInfo.InvariantCulture));
            }
        }

        private sealed class DecimalConverter<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] T> : JsonConverter<DecimalMeasurement<T>>
            where T : Enum
        {
            public override DecimalMeasurement<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType != JsonTokenType.String)
                    throw new JsonException($"A measurement of {typeof(T).Name} must be a string, found {reader.TokenType}");

                string text = reader.GetString();
                if (!DecimalMeasurement<T>.TryParse(text, CultureInfo.InvariantCulture, out DecimalMeasurement<T> value))
                    throw new JsonException($"'{text}' is not a valid measurement of {typeof(T).Name}");

                return value;
            }

            public override void Write(Utf8JsonWriter writer, DecimalMeasurement<T> value, JsonSerializerOptions options)
            {
                if (writer == null)
                    throw new ArgumentNullException(nameof(writer));

                Span<char> buffer = stackalloc char[256];
                if (value.TryFormat(buffer, out int written, "NF".AsSpan(), CultureInfo.InvariantCulture))
                    writer.WriteStringValue(buffer.Slice(0, written));
                else
                    writer.WriteStringValue(value.ToString("NF", CultureInfo.InvariantCulture));
            }
        }
    }
}
