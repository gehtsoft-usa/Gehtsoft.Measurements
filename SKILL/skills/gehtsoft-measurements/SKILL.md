---
name: gehtsoft-measurements
description: Use the Gehtsoft.Measurements .NET library for strongly typed measurements and unit conversion in C# - converting between units, measurement arithmetic and comparison, parsing and formatting measurement text, JSON and XML serialization, and declaring or validating your own unit enumerations. Trigger this skill whenever C# code references Measurement<T>, DecimalMeasurement<T>, MeasurementMath, UnitEnumValidator, a *Unit enumeration such as DistanceUnit or AngularUnit, or the Unit and Conversion attributes - and also whenever the user asks to convert or store physical quantities in .NET (distance, weight, angle, temperature, pressure, velocity, energy, torque, volume, density), to add their own unit to such an enumeration, or to read and write values like "300yd" or "1.5MOA", even if they never name the library.
---

# Gehtsoft.Measurements

A .NET library for values that carry a unit: `300yd`, `168gr`, `1.5MOA`. It converts between
units, does arithmetic across them, and reads and writes them as text. Written against **1.1.18**.

Full API reference: https://docs.gehtsoftusa.com/Gehtsoft.Measurements/ — consult it for members
this skill does not cover.

## Mental model

A measurement is a **number plus a unit**, held in a `readonly struct`:

```csharp
var range = new Measurement<DistanceUnit>(300, DistanceUnit.Yard);
```

The unit is a plain `enum` decorated with attributes. Each enumeration nominates exactly one
**base unit**, and every conversion routes through it: `from -> base -> to`. Two things follow
that explain most of the library's behaviour:

- **The base unit is per domain and is not always SI.** `DistanceUnit` is based on the inch,
  `WeightUnit` on the grain, `TemperatureUnit` on Fahrenheit. This is invisible when converting,
  because you always name both units, but it matters when adding your own unit and it explains
  what `ZERO` means.
- **Conversion is derived, not tabulated.** You declare how *your* unit reaches the base unit;
  the reverse direction is inverted automatically. That is why a wrong operation in one
  direction cannot happen — but also why a hand-written custom conversion can be wrong.

Everything is generic over the unit enumeration, so `Measurement<DistanceUnit>` and
`Measurement<WeightUnit>` are different types and cannot be mixed up.

There are two value types, identical apart from the numeric type:

| | Backing | Comparison | Use when |
|---|---|---|---|
| `Measurement<T>` | `double` | tolerant, `1e-12` relative | normal calculation, speed matters |
| `DecimalMeasurement<T>` | `decimal` | exact | money-like exactness matters more than speed |

Both convert to each other implicitly. Going to `DecimalMeasurement` is widening; coming back
narrows through a `double`.

## 1. Units, conversion and storage

Construct, convert, read:

```csharp
var range = new Measurement<DistanceUnit>(300, DistanceUnit.Yard);

double metres = range.In(DistanceUnit.Meter);              // 274.32 - a bare number
var inMetres  = range.To(DistanceUnit.Meter);              // a measurement, still 300 yd worth
```

`In` gives you a `double` in the unit you name. `To` gives you a new measurement carrying that
unit. Prefer `To` while the value is still travelling through your code, and `In` only at the
point where you need a raw number, so the unit stays attached as long as possible.

The fluent constructors read better in expressions and are worth knowing:

```csharp
using Gehtsoft.Measurements;

var a = 300.As(DistanceUnit.Yard);            // from int or double
var b = DistanceUnit.Yard.New(300);           // from the unit
var c = 12.5m.AsDecimal(VolumeUnit.Liter);    // decimal, from the value
var d = VolumeUnit.Liter.NewDecimal(12.5m);   // decimal, from the unit
IEnumerable<Measurement<DistanceUnit>> many = new[] { 1.0, 2.0 }.As(DistanceUnit.Meter);
```

Useful statics per enumeration:

```csharp
Measurement<DistanceUnit>.BaseUnit                              // DistanceUnit.Inch
Measurement<DistanceUnit>.ZERO                                  // 0 in the base unit
Measurement<DistanceUnit>.Convert(300, DistanceUnit.Yard, DistanceUnit.Meter);
Measurement<DistanceUnit>.GetUnitName(DistanceUnit.Yard);       // "yd"
Measurement<DistanceUnit>.GetUnitDefaultAccuracy(DistanceUnit.Yard);  // 2
Measurement<DistanceUnit>.GetUnitNames();                       // every unit and its name
```

`GetUnitNames()` returns a defensive copy, so it allocates. Call it once and cache it if you are
populating a dropdown; do not call it in a loop.

**Storing measurements.** The struct is immutable and blittable-ish; store it as a field or a
property like any value type. For a database or a wire format, either store the base-unit
`double` alongside the unit, or store the text form (`range.Text`, always invariant) — the text
form round-trips exactly through the constructor and is the easier of the two to read in a dump.

**Comparison is tolerant and cross-unit.** This is the single most surprising behaviour and it is
deliberate:

```csharp
1.As(DistanceUnit.Foot) == 12.As(DistanceUnit.Inch)   // true
```

`==`, `Equals`, `CompareTo` and the ordering operators all compare the physical value with a
relative tolerance of `1e-12`, so values that differ only by conversion rounding are equal.
`GetHashCode` is consistent with that, so measurements work as dictionary keys. If you need
exact bit equality, compare `Value` and `Unit` yourself, or use `DecimalMeasurement<T>`, whose
comparison is exact.

## 2. Math

The operators do what they look like. The result of a binary operation carries the unit of the
**left** operand:

```csharp
var total = 1.As(DistanceUnit.Foot) + 6.As(DistanceUnit.Inch);   // 1.5 ft
var half  = total / 2.0;                                          // 0.75 ft
var twice = total * 2.0;
var back  = -total;

double ratio = 1.As(DistanceUnit.Foot) / 6.As(DistanceUnit.Inch); // 2.0 - dimensionless
```

Dividing two measurements gives a bare `double` ratio, which is the one case where the unit
correctly disappears.

Both types implement the `System.Numerics` operator interfaces (`IAdditionOperators`,
`IComparisonOperators`, `IMultiplyOperators` and friends), so a measurement can flow into your
own generic-math code.

`MeasurementMath` adds the rest:

```csharp
using Gehtsoft.Measurements;

value.Abs(); value.Sign(); value.Sqrt(); value.Pow(2);
value.Round(2);                     // to 2 decimal places, in the unit it is expressed in
value.Round();                      // to the unit's own default accuracy

MeasurementMath.Min(a, b);
MeasurementMath.Max(a, b);
MeasurementMath.Clamp(value, low, high);

// angles: Sin, Cos and Tan take an angular measurement and return a bare number
double s = 45.As(AngularUnit.Degree).Sin();
double c = 45.As(AngularUnit.Degree).Cos();
double t = 45.As(AngularUnit.Degree).Tan();

// Asin, Acos and Atan go the other way, returning an angle in radians
Measurement<AngularUnit> angle = MeasurementMath.Asin(0.5);
Measurement<AngularUnit> other = MeasurementMath.Acos(0.5);
```

**Cross-dimension helpers.** These combine two kinds of measurement into a third. They compute
in SI and return the SI unit of the result, so convert afterwards if you want something else:

```csharp
MeasurementMath.Velocity(distance, TimeSpan.FromSeconds(2));          // -> m/s
MeasurementMath.Velocity(acceleration, time);
MeasurementMath.Acceleration(velocity, time);                         // -> m/s²
MeasurementMath.DistanceTraveled(velocity, time);
MeasurementMath.DistanceTraveled(acceleration, time);
MeasurementMath.TravelTime(distance, velocity);                       // -> TimeSpan
MeasurementMath.KineticEnergy(weight, velocity);                      // -> J
MeasurementMath.Force(weight, acceleration);                          // -> N
MeasurementMath.Power(torque, rotationalSpeed);                       // -> W
MeasurementMath.Density(weight, volume);                              // -> kg/m³
MeasurementMath.Weight(density, volume);                              // -> kg
MeasurementMath.Pressure(weight, area);                              // -> psi
MeasurementMath.RectangleArea(width, height);                         // -> m²
MeasurementMath.RectangularPrismVolume(area, depth);                  // -> m³

// 200 N·m at 3000 rpm is 62.8 kW
var kw = MeasurementMath.Power(200.As(TorqueUnit.NewtonMeter),
                              3000.As(RotationalSpeedUnit.RevolutionsPerMinute))
                        .In(PowerUnit.Kilowatt);
```

**Aggregating a sequence.** `Min`, `Max`, `MinBy`, `MaxBy` and `OrderBy` from `System.Linq`
already work, because both types implement `IComparable`, and they compare across units. The
library adds only what LINQ cannot express for a custom type:

```csharp
using System.Linq;

var shortest = ranges.Min();          // System.Linq, compares physical values
var total    = ranges.Sum();          // this library
var mean     = ranges.Average();      // this library
```

`Sum` and `Average` accumulate in the unit of the **first** element, and `Sum` of an empty
sequence is `ZERO` while `Average` of an empty sequence throws, matching `Enumerable.Average`.

## 3. Parsing and formatting

The text form is the number immediately followed by the unit name, with nothing between them:
`300yd`, `-1.5MOA`, `1e3m`, `168gr`. Any notation the numeric type accepts works for the number,
including exponents and group separators.

### Formatting

```csharp
var range = new Measurement<DistanceUnit>(300.456, DistanceUnit.Yard);

range.ToString();                                   // "300.456yd"  - invariant, all digits
range.ToString(CultureInfo.GetCultureInfo("de-DE")); // "300,456yd"
range.ToString("ND");                                // "300.46yd"   - the unit's own accuracy (2)
range.ToString("N1", CultureInfo.InvariantCulture);  // "300.5yd"
range.Text;                                          // "300.456yd"  - always invariant
```

Two formats are specific to a measurement, and the rest are handed to the numeric type:

- `"ND"` — the default accuracy declared by that unit. Use it for display.
- `"NF"` — every digit the value has. This is what `ToString()` and `Text` use, and it is what
  round-trips.

To format without allocating, into a buffer you own:

```csharp
Span<char> buffer = stackalloc char[64];
if (range.TryFormat(buffer, out int written, "ND", CultureInfo.InvariantCulture))
    Consume(buffer.Slice(0, written));
```

There is a UTF-8 overload of `TryFormat` taking a `Span<byte>` for writing straight into a JSON
or network buffer.

### Parsing

```csharp
// the non-throwing form, with an explicit culture - prefer this
if (Measurement<DistanceUnit>.TryParse(CultureInfo.InvariantCulture, "300yd", out var range))
    Use(range);

// the throwing form, raises FormatException
var r = Measurement<DistanceUnit>.Parse("300yd", CultureInfo.InvariantCulture);

// straight out of a bigger buffer, without cutting a substring
ReadOnlySpan<char> field = line.AsSpan(start, length);
Measurement<DistanceUnit>.TryParse(CultureInfo.InvariantCulture, field, out var fromSpan);

// just a unit name
DistanceUnit unit = Measurement<DistanceUnit>.ParseUnitName("yd");   // throws if unknown
```

Both types implement `IParsable` and `ISpanParsable`, so they work in generic parsing code.

**Be explicit about culture.** There is one asymmetry that will bite you otherwise:

| | Culture used |
|---|---|
| `new Measurement<T>("300yd")` and `Text` | **invariant**, always |
| `TryParse(text, out …)` and `Parse(text)` | **current** culture |
| any overload taking a culture or provider | what you passed |

The constructor is invariant so that a value round-trips through `Text` on any machine, which is
what makes it safe for serialization. `TryParse(string)` follows the .NET convention of using the
current culture. Mixing them silently breaks on a machine whose decimal separator is a comma —
so pass a culture explicitly whenever the text crosses a boundary such as a file or a wire.

A null text is a routine parse failure and returns `false`; the constructor throws
`ArgumentNullException`.

### JSON

The default shape is an object with a single `value` property, so a measurement nests inside a
document without any setup:

```csharp
JsonSerializer.Serialize(new { range = 300.As(DistanceUnit.Yard) });
// {"range":{"value":"300yd"}}
```

For the bare string form, which is usually what an API contract wants, register the converter:

```csharp
var options = new JsonSerializerOptions();
options.Converters.Add(new MeasurementJsonConverter());

JsonSerializer.Serialize(new { range = 300.As(DistanceUnit.Yard) }, options);
// {"range":"300yd"}
```

The converter handles both measurement types and any unit enumeration, and always uses the
invariant culture. It can also be applied to a single property with
`[JsonConverter(typeof(MeasurementJsonConverter))]`. The default shape is untouched unless you
register it.

### XML

`XmlSerializer` cannot serialize a `readonly struct` — it needs a settable public member — so
there is no attribute to add to make it work directly. Expose the text form as a string property
and hide the measurement:

```csharp
[XmlRoot("shot")]
public class Shot
{
    [XmlIgnore]
    public Measurement<DistanceUnit> Range { get; set; }

    [XmlAttribute("range")]
    public string RangeText
    {
        get => Range.Text;
        set => Range = new Measurement<DistanceUnit>(value);
    }
}
```

This works precisely because `Text` and the text constructor are both invariant, so the document
means the same thing on every machine. Use `[XmlElement]` instead of `[XmlAttribute]` if you want
a child element. `Binaron.Serializer` needs none of this and works directly.

### Performance note

Conversion, comparison, arithmetic, parsing, and `TryFormat` into a buffer you supply allocate
**nothing** — the library has tests that measure allocated bytes to keep it that way. `ToString`
allocates only the string it returns. So parsing a large file measurement by measurement is fine;
just prefer `TryFormat` over `ToString` in a tight loop.

## 4. Defining your own units

Declare an `enum`, decorate each member with `[Unit]` and `[Conversion]`, and
`Measurement<YourUnit>` works immediately — the conversion, parsing and formatting code is
generated once per unit type at first use.

```csharp
public enum MyWeightUnit
{
    // the base unit: one gram
    [Unit("g", 1)]
    [Conversion(ConversionOperation.Base)]
    Gram,

    // 1 kg = 1000 g
    [Unit("kg", 3)]
    [Conversion(ConversionOperation.Multiply, 1000)]
    Kilogram,

    // two names: "lb" and "lbs" both parse, "lb" is what formatting produces
    [Unit("lb", "lbs", 3)]
    [Conversion(ConversionOperation.Multiply, 453.59237)]
    Pound,
}
```

`[Unit(name, accuracy)]` or `[Unit(name, alternativeName, accuracy)]`. The name is what parsing
accepts and formatting produces; the accuracy is the decimal places used by the `"ND"` format.
The alternative name parses but is never produced.

`[Conversion(...)]` describes how to get **from your unit to the base unit**, as one or two
operations. The reverse is derived by inverting them in reverse order, so you never write it.

```csharp
[Conversion(ConversionOperation.Base)]                            // exactly one member
[Conversion(ConversionOperation.Multiply, 1000)]                  // one operation
[Conversion(ConversionOperation.Multiply, 2, ConversionOperation.Add, 4)]   // two, in order
```

The operations are `Base`, `Add`, `Subtract`, `SubtractFromFactor`, `Multiply`, `Divide`,
`DivideFactor`, `Negate`, `Tan`, `Atan` and `Custom`. Temperature is the interesting case,
because it needs two operations, and slope-style angular units are why `Tan` exists. See
`references/conversions.md` for each operation's exact arithmetic, worked affine examples, and
how to implement `ICustomConversionOperation` when arithmetic is not enough.

Rules the machinery relies on, each of which the validator checks:

- Exactly one member is `ConversionOperation.Base`.
- Every member has both attributes — a member with neither is not "just an alias", it breaks the
  generated switch.
- No two members share a parse name.
- To rename a unit without breaking callers, keep the old member and mark it `[Obsolete]`. An
  obsolete member still converts and formats, but is excluded from `GetUnitNames()` and from
  parsing, so it can share the replacement's name without colliding.

`references/units.md` lists every unit of all 17 shipped enumerations with its names, default
accuracy and base unit — consult it rather than guessing a member name, because several are not
what you would expect (`AccelerationUnit` is based on the gal, `TemperatureUnit` on Fahrenheit).

## 5. Validating your own units

Most ways of getting a unit enumeration wrong are invisible to the compiler. The fatal ones throw
when the enumeration is first used, one at a time, from a static initializer. `UnitEnumValidator`
instead **returns** everything it finds, which is what makes it usable at start-up or in a test:

```csharp
// refuse to start on a broken unit type
UnitEnumValidator.Validate<MyWeightUnit>().ThrowIfInvalid();

// or look at the detail
var report = UnitEnumValidator.Validate<MyWeightUnit>();
if (report.HasErrors)
    logger.LogError(report.ToString());
foreach (var finding in report.Warnings())
    logger.LogWarning(finding.ToString());
```

`Validate<T>()` is the overload to use. There is a `Validate(Type)` for code that discovers unit
types by reflection; it is not trim-safe or AOT-safe and says so through its annotations.

A report is a list of findings, each with a stable code, a severity, and the offending member:

```
MyWeightUnit:
  GM007 Error: Ounce: the name 'oz' is already used by TroyOunce
  GM011 Warning: Slug: the default accuracy 20 is outside the range 0 to 15 which a numeric format supports
```

**Errors** mean the enumeration is unusable or produces wrong numbers. **Warnings** are legal but
suspicious. `HasErrors` and `IsValid` ignore warnings.

Beyond reading the declaration, the validator **exercises** the conversions: it converts probe
values to the base unit and back, and it formats a value in every name of every unit and parses
it again. That is what catches a custom conversion whose reverse is wrong, which no amount of
reading the attributes will show you.

The best place to call it is a unit test over your own unit types — it costs nothing to run and
turns a class of silent wrong-number bugs into a failing build:

```csharp
[Theory]
[InlineData(typeof(MyWeightUnit))]
[InlineData(typeof(MyPressureUnit))]
public void UnitEnumerationsAreValid(Type unitType)
    => UnitEnumValidator.Validate(unitType).Findings.Should().BeEmpty();
```

`references/validation.md` explains every code from GM001 to GM017 and what to do about it.

## Gotchas worth knowing before you start

- **Affine units break some otherwise sensible operations.** `TemperatureUnit` converts with
  add/subtract, so `ZERO` (zero in the base unit, 0 °F) is not an additive identity: adding it to
  50 °C gives 32.22 °C. Summing or averaging temperatures is arithmetically defined but physically
  meaningless — average the numbers in one unit yourself if that is what you mean. This is why the
  library does not implement `IAdditiveIdentity`.
- **The unit of a result is the left operand's**, for every binary operator and for `Sum` and
  `Average`. Order matters for the unit even though the physical value is the same.
- **`ParseUnitName` throws, `TryParse` does not.** Use `TryParse` on anything user-supplied; the
  failure path is fast and allocation-free.
- **Obsolete units cannot be parsed** and do not appear in `GetUnitNames()`, but still convert and
  format. That is intentional, so a renamed misspelling cannot shadow its replacement.
- **A unit name is matched at the end of the text**, longest first, so `10mm` is millimetres and
  not metres. A name beginning with a digit can swallow the tail of the value; the validator warns
  about this (GM012) and proves it (GM017).
- **Trimming and AOT**: the library is trim-clean and marked trimmable. Two things warn if used in
  a trimmed application, because they cannot be preserved automatically — a `Custom` conversion,
  whose type is found by name at run time, and `MeasurementJsonConverter`. The library is not
  declared AOT-compatible: it works under NativeAOT, but the conversion delegates fall back to the
  expression interpreter and are much slower.

## Reference files

Read these when the task calls for them, not up front. Between them they cover the whole public
surface, so there is no need to inspect the NuGet package or decompile the assembly to find a
signature.

- **`references/api.md`** — every public type and member with its signature, generated from the
  assembly. Go here for anything this file does not show, before reaching for the documentation
  site.
- **`references/units.md`** — all 17 shipped unit enumerations: every member, its names, its
  default accuracy, and which member is the base. Use it to get a member name right.
- **`references/conversions.md`** — what each `ConversionOperation` computes in both directions,
  worked examples for affine and trigonometric units, and the custom conversion interfaces.
- **`references/validation.md`** — every validator code GM001 to GM017, what triggers it, and how
  to fix it.
