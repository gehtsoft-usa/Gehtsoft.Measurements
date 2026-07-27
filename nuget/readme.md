# About

The C# library to manipulate and convert measurements (for example distances, weight, angles,
temperatures) expressed in various units (for example distances in inches, yards or meters).

The library may be useful for calculations, for example in math, physics or GIS, that do not
depend on the system of units used (SI/Metric or Imperial), or to create a unit convertor
application.

Seventeen kinds of measurement are supported out of the box — acceleration, angle, area,
density, distance/length, energy, force, gas consumption, power, pressure, rotational speed,
solid angle, temperature, torque, velocity, volume and weight — and you can define your own.

Requires **.NET 8.0** or later. No package dependencies.

The library is shared under LGPL license.

| | |
|---|---|
| API documentation | https://docs.gehtsoftusa.com/Gehtsoft.Measurements/ |
| Source | https://github.com/gehtsoft-usa/Gehtsoft.Measurements |
| Claude Code skill | https://github.com/gehtsoft-usa/Gehtsoft.Measurements/tree/main/SKILL |

## Using the library

The core type is the generic structure `Measurement`. It takes an enumeration as its parameter,
and that enumeration defines the units available:

```csharp
var v = new Measurement<DistanceUnit>(10, DistanceUnit.Foot);
```

or simply

```csharp
var v = DistanceUnit.Foot.New(10);
var w = 10.As(DistanceUnit.Foot);
```

You can then manipulate the value with C# operators, format it, or convert it to another unit:

```csharp
var doubled = v * 2;
string text = v.ToString("N3");
var inMeters = v.To(DistanceUnit.Meter);
double meters = v.In(DistanceUnit.Meter);
```

or

```csharp
var x = (10.As(DistanceUnit.Yard) + 36.As(DistanceUnit.Inch)).To(DistanceUnit.Meter);
```

Comparison works across units within a relative tolerance of `1e-12`, so
`1.As(DistanceUnit.Foot) == 12.As(DistanceUnit.Inch)` is `true`.

`DecimalMeasurement` is the same type backed by `decimal` instead of `double`, for when exactness
matters more than speed. Its comparison is exact.

`MeasurementMath` adds trigonometry for angles, `Abs`, `Sqrt`, `Pow`, `Round`, `Min`, `Max`,
`Clamp`, and helpers which combine kinds — kinetic energy, force, density, travel time, and power
from torque and rotational speed. `Sum` and `Average` work over sequences of measurements;
`Min`, `Max` and `OrderBy` from LINQ already do.

## Parsing and formatting

A measurement is written as the value immediately followed by the unit name — `300yd`, `1.5moa`,
`1e3m`:

```csharp
var range = Measurement<DistanceUnit>.Parse("300yd", CultureInfo.InvariantCulture);
Measurement<DistanceUnit>.TryParse(CultureInfo.InvariantCulture, "1e3m", out var thousandMeters);
```

Both types implement `IParsable`, `ISpanParsable`, `ISpanFormattable` and `IUtf8SpanFormattable`,
so a value can be parsed straight out of a buffer and formatted straight into one:

```csharp
Span<char> buffer = stackalloc char[64];
range.TryFormat(buffer, out int written, "ND", CultureInfo.InvariantCulture);
```

`"ND"` formats with the default accuracy declared by the unit, `"NF"` with every digit; any other
format is passed to the numeric type.

Conversion, comparison, arithmetic, parsing and formatting into a caller buffer allocate nothing.

## Serialization

`System.Text.Json` and `Binaron.Serializer` are supported directly. By default a measurement is
an object with a single `value` property; register `MeasurementJsonConverter` to write a bare
string instead:

```csharp
var options = new JsonSerializerOptions();
options.Converters.Add(new MeasurementJsonConverter());
// {"range":"300yd"} instead of {"range":{"value":"300yd"}}
```

`XmlSerializer` cannot serialize a readonly structure, so expose the text form as a string
property and hide the measurement:

```csharp
[XmlIgnore]
public Measurement<DistanceUnit> Range { get; set; }

[XmlAttribute("range")]
public string RangeText
{
    get => Range.Text;
    set => Range = new Measurement<DistanceUnit>(value);
}
```

This round-trips on any machine because `Text` and the text constructor are both invariant.

## Defining your own units

Create an enumeration and mark it with the `Unit` and `Conversion` attributes. The first defines
the unit name and the default accuracy; the second defines how to convert the unit **into the
base unit**. Exactly one unit must be the base, and the reverse conversion is derived for you.

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

Conversions which cannot be expressed as one or two arithmetic operations can implement
`ICustomConversionOperation` (or `ICustomConversionOperation2` for native `decimal` support).

## Validating your own units

Most ways of getting a unit enumeration wrong are invisible to the compiler. `UnitEnumValidator`
checks one and returns everything it finds, so you can guard your application at start-up or in
a test:

```csharp
UnitEnumValidator.Validate<MyWeightUnit>().ThrowIfInvalid();
```

Besides checking the declaration, it exercises the conversions — converting sample values to the
base unit and back, and formatting and re-parsing every name of every unit — which is what
catches a custom conversion whose reverse operation is wrong.

## Trimming

The assembly is marked trimmable and builds clean under the trim and AOT analyzers. Two features
cannot be preserved automatically and warn in a trimmed application: a conversion declared with
`ConversionOperation.Custom`, whose type is located by name at run time, and
`MeasurementJsonConverter`. The library is not declared AOT-compatible — it runs under NativeAOT,
but the conversion delegates fall back to the expression interpreter and are much slower.
