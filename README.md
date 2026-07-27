# Status

![unit tests](https://github.com/gehtsoft-usa/Gehtsoft.Measurements/actions/workflows/test.yml/badge.svg)
![documentation](https://github.com/gehtsoft-usa/Gehtsoft.Measurements/actions/workflows/doc.yml/badge.svg)

# Gehtsoft.Measurements

The C# library to manipulate and convert measurements (for example distances, weight, angles,
temperatures) expressed in various units (for example distances in inches, yards or meters).

The library may be useful for calculations, for example in math, physics or GIS, that do not
depend on the system of units used (SI/Metric or Imperial), or to create a unit convertor
application.

Seventeen kinds of measurement are supported out of the box:

| | | |
|---|---|---|
| Acceleration | Angle | Area |
| Density | Distance/Length | Energy |
| Force | Gas consumption | Power |
| Pressure | Rotational speed | Solid angle |
| Temperature | Torque | Velocity |
| Volume | Weight | |

You can also define your own — see [Defining your own units](#defining-your-own-units).

The library requires **.NET 8.0** or later and has no package dependencies.

The library is shared under LGPL license.

To use the last stable version of the library in your project please use the package on the nuget
https://www.nuget.org/packages/Gehtsoft.Measurements

## Using Library

The core type of the library is the generic structure `Measurement`. The structure accepts an
enumeration as a parameter and this enumeration defines the measurement unit to be used:

```csharp
var v = new Measurement<DistanceUnit>(10, DistanceUnit.Foot);
```

You can then manipulate this value using C# operators, format it or convert it into another unit:

```csharp
var v1 = v * 2;
string text = v.ToString("N3");
var v2 = v1.To(DistanceUnit.Meter);
```

or

```csharp
var x = (10.As(DistanceUnit.Yard) + 36.As(DistanceUnit.Inch)).To(DistanceUnit.Meter);
```

Comparison and equality work across units, within a relative tolerance of `1e-12`, so
`1.As(DistanceUnit.Foot) == 12.As(DistanceUnit.Inch)` is `true`.

### Decimal values

`DecimalMeasurement` is the same type backed by `decimal` instead of `double`, for cases where
the exactness of the decimal arithmetic matters more than the speed. Its comparison is exact
rather than tolerance-based.

```csharp
var price = new DecimalMeasurement<VolumeUnit>(12.5m, VolumeUnit.Liter);
```

### Parsing and formatting

A measurement is written as the value immediately followed by the unit name, and both
measurement types parse and format that form:

```csharp
var v = Measurement<DistanceUnit>.Parse("10.5in", CultureInfo.InvariantCulture);
Measurement<DistanceUnit>.TryParse(CultureInfo.InvariantCulture, "1e3m", out var thousandMeters);
```

Both types implement `IParsable`, `ISpanParsable`, `ISpanFormattable` and
`IUtf8SpanFormattable`, so they can be parsed straight out of a larger buffer and formatted
straight into one, without a single intermediate string:

```csharp
ReadOnlySpan<char> field = line.AsSpan(start, length);
Measurement<DistanceUnit>.TryParse(CultureInfo.InvariantCulture, field, out var range);

Span<char> buffer = stackalloc char[64];
range.TryFormat(buffer, out int written, "ND", CultureInfo.InvariantCulture);
```

Two formats are specific to a measurement: `"ND"` formats the value with the default accuracy
declared by its unit, and `"NF"` formats it with all the digits it has. Any other format is
passed to the underlying numeric type.

### Aggregating and comparing

`Min`, `Max`, `MinBy`, `MaxBy` and `OrderBy` from `System.Linq` already work on sequences of
measurements and compare them across units. `Sum` and `Average`, which cannot accept a custom
type, are provided by the library; the result carries the unit of the first element:

```csharp
var total = shots.Select(s => s.Range).Sum();
var average = shots.Select(s => s.Range).Average();
```

`MeasurementMath` adds `Min`, `Max` and `Clamp` over two or three values, `Round`, the usual
`Abs`, `Sign`, `Sqrt` and `Pow`, trigonometry for angles, and the helpers which combine kinds:

```csharp
var power = MeasurementMath.Power(200.As(TorqueUnit.NewtonMeter), 3000.As(RotationalSpeedUnit.RevolutionsPerMinute));
var force = MeasurementMath.Force(10.As(WeightUnit.Kilogram), 1.As(AccelerationUnit.EarthGravity));
var energy = MeasurementMath.KineticEnergy(168.As(WeightUnit.Grain), 2700.As(VelocityUnit.FeetPerSecond));
```

### Serialization

The types support `System.Text.Json` and `Binaron.Serializer`
(see https://github.com/zachsaw/Binaron.Serializer).

By default a measurement is serialized as an object with a single `value` property:

```json
{ "range": { "value": "300yd" } }
```

Register `MeasurementJsonConverter` to write it as a plain string instead:

```csharp
var options = new JsonSerializerOptions();
options.Converters.Add(new MeasurementJsonConverter());
```

```json
{ "range": "300yd" }
```

`XmlSerializer` cannot be implemented for a readonly structure without introducing unsafe code.
Please refer to `CoreClassesTest` in the test project for an example of how to implement an XML
serialization.

Read more on http://docs.gehtsoftusa.com/Gehtsoft.Measurements/web-content.html#index.html

## Defining your own units

You can define your own measurement units by creating an enumeration and marking it using the
`Unit` and `Conversion` attributes. The first attribute defines the unit name and the default
accuracy of the values. The second attribute defines the rules of the conversion. Exactly one
unit must be the "base" unit, and the conversion rules of the other units define how to convert
the unit into the base one.

```csharp
enum MyWeightUnit
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
```

`Measurement<MyWeightUnit>` then works immediately — the conversion, parsing and formatting code
is generated once per unit type at runtime.

The enumeration is validated the first time it is used. A unit which is missing either
attribute, an enumeration without a base unit or with more than one, and two units sharing a
name are all reported with a message naming the offending member, instead of failing silently
or much later.

Conversions which cannot be expressed as one or two arithmetic operations can implement
`ICustomConversionOperation` (or `ICustomConversionOperation2` to support `decimal` natively)
and be referenced by type name from the `Conversion` attribute.

## Validating your own units

A unit enumeration is ordinary C# decorated with attributes, so most of the ways to get one
wrong are invisible to the compiler. `UnitEnumValidator` checks a unit type and returns
everything it finds, so you can guard the application while it starts, or in a test:

```csharp
UnitEnumValidator.Validate<MyWeightUnit>().ThrowIfInvalid();
```

```csharp
var report = UnitEnumValidator.Validate<MyWeightUnit>();
foreach (var finding in report.Findings)
    logger.LogWarning(finding.ToString());
```

Each finding carries a stable code, a severity, and the unit at fault:

```
MyWeightUnit:
  GM007 Error: Ounce: the name 'oz' is already used by TroyOunce
  GM011 Warning: Slug: the default accuracy 20 is outside the range 0 to 15 which a numeric format supports
```

Errors mean the enumeration produces wrong numbers or cannot be used at all — a missing or
duplicated base unit, two units sharing a name, a factor of zero. Warnings are legal but
suspicious — a factor on an operation which ignores it, or a name which can swallow part of the
value it follows.

Beyond checking the declaration, the validator exercises the conversions: it converts sample
values to the base unit and back, and it formats a value in every name of every unit and parses
it again. That is what catches a custom conversion whose reverse operation is wrong, which no
amount of reading the declaration will show. All 17 unit kinds shipped with the library are
validated by it in the test suite.

## Trimming and ahead-of-time compilation

The assembly is marked trimmable and builds clean under both the trim and the AOT analyzer.
The reflection over your unit enumeration is annotated so that trimming keeps the unit fields,
and a fully trimmed application converts, parses and formats correctly.

Two features cannot be preserved automatically and say so through
`RequiresUnreferencedCode`/`RequiresDynamicCode`, so you will see a warning if you use them in
a trimmed application:

- a conversion declared with `ConversionOperation.Custom`, because the implementing type is
  located by name at run time — preserve that type explicitly;
- `MeasurementJsonConverter`, because it builds a converter closed over your unit enumeration
  at run time.

The library is not declared AOT-compatible. It works under NativeAOT, but the conversion
delegates fall back to the expression interpreter there and are much slower than the numbers
below.

## Performance

The operations you are likely to run in a loop — conversion, comparison, arithmetic, parsing,
and formatting into a buffer you supply — do not allocate at all. This is enforced by tests
which measure the allocated bytes, not only by review. `CLAUDE/BASELINE_PERF.md` holds the
measurements, and `Gehtsoft.Measurements.Benchmark` reproduces them:

```
dotnet run -c Release --project Gehtsoft.Measurements.Benchmark
```
